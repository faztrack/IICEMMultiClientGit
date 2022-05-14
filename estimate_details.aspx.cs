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


public partial class estimate_details : System.Web.UI.Page
{
    private double subtotal = 0.0;
    private double grandtotal = 0.0;

    private double subtotal_diect = 0.0;
    private double grandtotal_direct = 0.0;

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin026") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
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
                string strAddress = "";
                strAddress = cust.address + "</br>" + cust.city + ", " + cust.state + ", " + cust.zip_code;
                lblAddress.Text = strAddress;
                lblEmail.Text = cust.email;
                lblPhone.Text = cust.phone;

                // Get Estimate Info
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                    lblEstimateName.Text = cus_est.estimate_name;
                    ddlStatus.SelectedValue = cus_est.status_id.ToString();
                    ddlStatus.Enabled = false;
                    if (ddlStatus.SelectedValue == "3")
                    {
                        lblSaleDate.Visible = true;
                        txtSaleDate.Text = Convert.ToDateTime(cus_est.sale_date).ToShortDateString();
                        txtSaleDate.Visible = true;
                        
                    }
                    else
                    {
                        lblSaleDate.Visible = false;
                        txtSaleDate.Visible = false;
                       
                    }
                    txtComments.Text = cus_est.estimate_comments.Replace("&nbsp;", "");
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
            }
        }
    }

    protected void btnGotoCustomerList_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx?cid=" + Convert.ToInt32(hdnCustomerId.Value));
    }

    private decimal GetRetailTotal()
    {
        decimal dRetail = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.pricing_details
                      where (from clc in _db.customer_locations
                             where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                             select clc.location_id).Contains(pd.location_id) &&
                             (from cs in _db.customer_sections
                              where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                              select cs.section_id).Contains(pd.section_level) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A"
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
                             where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                             select clc.location_id).Contains(pd.location_id) &&
                             (from cs in _db.customer_sections
                              where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                              select cs.section_id).Contains(pd.section_level) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A"
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
            strQ = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + "  order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT pricing_details.location_id AS colId,'LOCATION: '+ location.location_name as colName from pricing_details  INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " order by colName asc";
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
            strQ = "select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + "  order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT pricing_details.location_id AS colId,'LOCATION: '+ location.location_name as colName from pricing_details  INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " order by colName asc";
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
            string str = e.Row.Cells[9].Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 25)
            {
                e.Row.Cells[9].ToolTip = str;
                e.Row.Cells[9].Text = str.Substring(0, 25) + "...";
            }
            else
            {
                e.Row.Cells[9].ToolTip = str;
                e.Row.Cells[9].Text = str;

            }

        }

    }

    protected void btnGoToPayment_Click(object sender, EventArgs e)
    {
        Response.Redirect("payment_info.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
    }
    protected string GetTotalPrice()
    {
        return "Total: " + grandtotal.ToString("c");
    }
    protected string GetTotalPriceDirect()
    {
        return "Total: " + grandtotal_direct.ToString("c");
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
                subtotal += Double.Parse((row.FindControl("lblTotal_price") as Label).Text);
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
            var price_detail = from p in _db.pricing_details
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.customer_locations
                                      where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.customer_sections
                                       where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                       select cs.section_id).Contains(p.section_level) && p.location_id == colId && p.is_direct == nDirectId && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && p.pricing_type == "A"
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
                                   pricing_type = "A"
                               };
            grd.DataSource = price_detail.ToList();
            grd.DataKeyNames = new string[] { "pricing_id" };
            grd.DataBind();
        }
        else
        {
            var price_detail = from p in _db.pricing_details
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.customer_locations
                                      where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.customer_sections
                                       where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                       select cs.section_id).Contains(p.section_level)
                                      && p.section_level == colId && p.is_direct == nDirectId && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && p.pricing_type == "A"
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
                                   pricing_type = "A"
                               };
            grd.DataSource = price_detail.ToList();
            grd.DataKeyNames = new string[] { "pricing_id" };
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
            string str = e.Row.Cells[9].Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 25)
            {
                e.Row.Cells[9].ToolTip = str;
                e.Row.Cells[9].Text = str.Substring(0, 25) + "...";
            }
            else
            {
                e.Row.Cells[9].ToolTip = str;
                e.Row.Cells[9].Text = str;

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
                subtotal_diect += Double.Parse((row.FindControl("lblTotal_price2") as Label).Text);
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
}
