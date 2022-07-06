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

public partial class CompositeSOWCost : System.Web.UI.Page
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
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin042") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }


            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCid.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);
            Session.Remove("Item_list");
            Session.Remove("Item_list_Direct");

            int nEstid = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstid.ToString();

            hyp_SowPrice.NavigateUrl = "composite_sow.aspx?eid=" + nEstid + "&cid=" + nCid;
            hyp_SowPrice.ToolTip = "Click here to view Composite SOW Price";

            DataClassesDataContext _db = new DataClassesDataContext();
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

                    //hypGoogleMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
                    string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                    hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

                    hdnClientId.Value = cust.client_id.ToString();
                }

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
                //Calculate();
                if (grdGrouping.Rows.Count == 0 && grdGroupingDirect.Rows.Count == 0)
                {
                    rdoSort.Visible = false;
                    //tblTotalProjectPrice.Visible = false;
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
                    userinfo obj = (userinfo)Session["oUser"];

                    if (obj.role_id == 1)
                    {
                        // tblTotalProjectPrice.Visible = true;
                        if (grdGrouping.FooterRow != null)
                            grdGrouping.FooterRow.Visible = true;
                        if (grdGroupingDirect.FooterRow != null)
                            grdGroupingDirect.FooterRow.Visible = true;

                      
                    }
                    else if (obj.role_id == 4)
                    {
                        // tblTotalProjectPrice.Visible = true;
                        if (grdGrouping.FooterRow != null)
                            grdGrouping.FooterRow.Visible = true;
                        if (grdGroupingDirect.FooterRow != null)
                            grdGroupingDirect.FooterRow.Visible = true;
                    }
                    else if (obj.role_id == 5)
                    {
                        // tblTotalProjectPrice.Visible = true;
                        if (grdGrouping.FooterRow != null)
                            grdGrouping.FooterRow.Visible = true;
                        if (grdGroupingDirect.FooterRow != null)
                            grdGroupingDirect.FooterRow.Visible = true;
                    }
                    else
                    {
                        if (grdGrouping.FooterRow != null)
                            grdGrouping.FooterRow.Visible = false;
                        if (grdGroupingDirect.FooterRow != null)
                            grdGroupingDirect.FooterRow.Visible = false;
                      
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
            }
        }
    }

    protected void btnGotoCustomerList_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }


    public void BindSelectedItemGrid()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        try
        {
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
                               " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                                " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct=1 AND cop.client_id =1  " +
                                " order by section_level asc";
                DataTable dtsec2 = csCommonUtility.GetDataTable(strQ2);
                DataRow drNew = null;
                if (dtsec2.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtsec2.Rows)
                    {
                        int ncolId = Convert.ToInt32(dr["colId"]);
                        string strColName = dr["colName"].ToString();

                        bool Iexists = dtsec.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(ncolId)).Count() > 0;
                        if (!Iexists)  // if (!ContainDataRowInDataTable(dtsec, dr))
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
                                " AND is_direct=1 AND cop.client_id =1 order by colName asc ";
                DataTable dtLoc2 = csCommonUtility.GetDataTable(strQ4);
                DataRow drNew = null;
                if (dtLoc2.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtLoc2.Rows)
                    {
                        int ncolId = Convert.ToInt32(dr["colId"]);
                        string strColName = dr["colName"].ToString();
                        bool Iexists = dtLoc.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(ncolId)).Count() > 0;
                        if (!Iexists) //  if (!ContainDataRowInDataTable(dtLoc, dr))
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
        catch (Exception ex)
        {
            string ext = ex.StackTrace;
            throw ex;
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
                            " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct = 2 AND cop.client_id =1  " +
                            " order by section_level asc";
            DataTable dtsec2 = csCommonUtility.GetDataTable(strQ2);
            DataRow drNew = null;
            if (dtsec2.Rows.Count > 0)
            {
                foreach (DataRow dr in dtsec2.Rows)
                {
                    int ncolId = Convert.ToInt32(dr["colId"]);
                    string strColName = dr["colName"].ToString();
                    bool Iexists = dtsec.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(ncolId)).Count() > 0;
                    if (!Iexists) //   if (!ContainDataRowInDataTable(dtsec, dr))
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
                            " AND is_direct = 2 order by colName asc ";
            DataTable dtLoc2 = csCommonUtility.GetDataTable(strQ4);
            DataRow drNew = null;
            if (dtLoc2.Rows.Count > 0)
            {
                foreach (DataRow dr in dtLoc2.Rows)
                {
                    int ncolId = Convert.ToInt32(dr["colId"]);
                    string strColName = dr["colName"].ToString();
                    bool Iexists = dtLoc.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(ncolId)).Count() > 0;
                    if (!Iexists) //    if (!ContainDataRowInDataTable(dtLoc, dr))
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

    protected void grdSelectedItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv1 = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string str = e.Row.Cells[2].Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 25)
            {
                e.Row.Cells[2].ToolTip = str;
                e.Row.Cells[2].Text = str.Substring(0, 25) + "...";
            }
            else
            {
                e.Row.Cells[2].ToolTip = str;
                e.Row.Cells[2].Text = str;

            }
            int nItemStatusId = Convert.ToInt32(gv1.DataKeys[e.Row.RowIndex].Values[1]);
            int ncoPricingUd = Convert.ToInt32(gv1.DataKeys[e.Row.RowIndex].Values[0]);
            Label lblDleted = (Label)e.Row.FindControl("lblDleted");
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
                e.Row.Cells[8].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[9].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[10].Attributes.CssStyle.Add("text-decoration", "none; color: red ;");
                e.Row.Cells[10].Text = "Item Deleted" + e.Row.Cells[10].Text;
                Label lblT_price1 = (Label)e.Row.Cells[8].FindControl("lblT_price1");
                lblT_price1.Text = "0.00";

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
                    Label lblT_price1 = (Label)e.Row.Cells[8].FindControl("lblT_price1");
                    lblT_price1.Text = "0.00";
                    lblDleted.Visible = true;
                    lblDleted.ForeColor = Color.Red;
                }
                e.Row.Attributes.CssStyle.Add("color", "green");
                e.Row.Cells[10].Text = "Item Added" + e.Row.Cells[10].Text;
            }
            else
            {
                e.Row.Cells[10].Text = "No Change";
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

        //if (lblTax.Text.ToString().Trim() == "")
        //    nTaxRate = Convert.ToDecimal(0.00);
        //else
        //    nTaxRate = Convert.ToDecimal(lblTax.Text.ToString().Trim());

        //tax_amount = Convert.ToDecimal(nGrandtotal * nTaxRate / 100);

        //grandtotal = Convert.ToDouble(nGrandtotal + tax_amount);

        //lblRunning.Text = grandtotal.ToString("c");

        return "Total Cost: " + grandtotal.ToString("c");

    }
    protected string GetTotalPriceDirect()
    {
        decimal nGrandtotal_direct = Convert.ToDecimal(grandtotal_direct);

        //if (lblTax.Text.ToString().Trim() == "")
        //    nTaxRate = Convert.ToDecimal(0.00);
        //else
        //    nTaxRate = Convert.ToDecimal(lblTax.Text.ToString().Trim());

        //tax_amount = Convert.ToDecimal(nGrandtotal_direct * nTaxRate / 100);

        //grandtotal_direct = Convert.ToDouble(nGrandtotal_direct + tax_amount);

        return "Total Direct Cost: " + grandtotal_direct.ToString("c");
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
                //subtotal += Double.Parse((row.FindControl("lblTotal_price") as Label).Text);
                subtotal += Double.Parse((row.FindControl("lblT_price1") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
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

            userinfo obj = (userinfo)Session["oUser"];

            if (obj.role_id == 1)
            {
                gv.Columns[8].Visible = true;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = true;

            }
            else if (obj.role_id == 4)
            {
                gv.Columns[8].Visible = true;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = true;

            }
            else if (obj.role_id == 5)
            {
                gv.Columns[8].Visible = true;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = true;

            }
            else
            {
                gv.Columns[8].Visible = false;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = false;
            }



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
                   " is_direct,section_level,location.location_id,'' as tmpCo,  cast(0 as decimal(18,2)) as total_retail_Cost, cast(0 as decimal(18,2)) as total_direct_Cost, 1 as TypeId " +
                   " FROM pricing_details " +
                   " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " AND " +
                    " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                   " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                    " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " ) " +
                   " order by section_level";
            DataTable dt = csCommonUtility.GetDataTable(strP);
            DataRow drNew = null;
            string strQ = "  SELECT co_pricing_list_id, change_order_pricing_list.item_id,ISNULL(item_price.labor_id,0) AS labor_id, section_serial,  location.location_name,section_name,item_name,change_order_pricing_list.measure_unit, " +
                         " ISNULL(item_price.item_cost,0) AS item_cost,total_retail_price,total_direct_price, ISNULL(item_price.minimum_qty,0) AS minimum_qty,quantity,ISNULL(item_price.retail_multiplier,0) AS retail_multiplier,ISNULL(item_price.labor_rate,0) AS labor_rate, "+
                         " short_notes,item_status_id,last_update_date,  is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date  "+
                          " FROM change_order_pricing_list  "+
                         " INNER JOIN location on location.location_id = change_order_pricing_list.location_id  "+
                         " INNER JOIN changeorder_estimate on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id "+
                         " LEFT OUTER JOIN item_price ON item_price.item_id = change_order_pricing_list.item_id "+
                           " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and is_direct = " + nDirectId + " AND changeorder_estimate.change_order_status_id = 3 order by section_level";
            DataTable dtcol = csCommonUtility.GetDataTable(strQ);
            foreach (DataRow dr in dtcol.Rows)
            {
                drNew = dt.NewRow();
                int nItemStatusId = Convert.ToInt32(dr["item_status_id"]);
                int ncoPricingUd = Convert.ToInt32(dr["co_pricing_list_id"]);
                if (_db.co_pricing_masters.Any(cpd => cpd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cpd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cpd.co_pricing_list_id == ncoPricingUd))
                {
                    co_pricing_master obj = _db.co_pricing_masters.SingleOrDefault(cpd => cpd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cpd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cpd.co_pricing_list_id == ncoPricingUd);

                    drNew["item_cost"] = obj.item_cost;
                    drNew["labor_rate"] = obj.labor_rate;
                    drNew["minimum_qty"] = obj.minimum_qty;
                    drNew["retail_multiplier"] = obj.retail_multiplier;

                }
                else
                {
                    drNew["item_cost"] = dr["item_cost"];
                    drNew["labor_rate"] = dr["labor_rate"];
                    drNew["minimum_qty"] = dr["minimum_qty"];
                    drNew["retail_multiplier"] = dr["retail_multiplier"];

                }
               
                string strTmp = " (" + dr["changeorder_name"].ToString() + ", " + Convert.ToDateTime(dr["execute_date"]).ToShortDateString() + ")";

               
                drNew["co_pricing_list_id"] = dr["co_pricing_list_id"];
                drNew["item_id"] = dr["item_id"];
                drNew["labor_id"] = dr["labor_id"];
                drNew["section_serial"] = dr["section_serial"];
                drNew["location_name"] = dr["location_name"];
                drNew["section_name"] = dr["section_name"];
                drNew["item_name"] = dr["item_name"];
                drNew["measure_unit"] = dr["measure_unit"];
                drNew["total_retail_price"] = dr["total_retail_price"];
                drNew["total_direct_price"] = dr["total_direct_price"];
                drNew["quantity"] = dr["quantity"];
                drNew["short_notes"] = dr["short_notes"];
                drNew["item_status_id"] = dr["item_status_id"];
                drNew["last_update_date"] = dr["last_update_date"];
                drNew["is_direct"] = dr["is_direct"];
                drNew["section_level"] = dr["section_level"];
                drNew["location_id"] = dr["location_id"];
                drNew["tmpCo"] = strTmp;
                drNew["total_retail_Cost"] = 0;
                drNew["total_direct_Cost"] = 0;
                drNew["TypeId"] = 2;
                
                dt.Rows.Add(drNew);

            }
            foreach (DataRow dr in dt.Rows)
            {
                decimal dOrginalCost = 0;
                decimal dOrginalTotalCost = 0;
                decimal dLaborTotal = 0;
                decimal dLineItemTotal = 0;
                string sItemName = dr["item_name"].ToString();

                int nType = Convert.ToInt32(dr["TypeId"]);

                decimal dItemCost = Convert.ToDecimal(dr["item_cost"]);
                decimal dRetail_multiplier = Convert.ToDecimal(dr["retail_multiplier"]);
                decimal dQuantity = Convert.ToDecimal(dr["quantity"]);
                decimal dLabor_rate = Convert.ToDecimal(dr["labor_rate"]);
                decimal dTPrice = Convert.ToDecimal(dr["total_retail_price"]);
                if (sItemName.IndexOf("Other") > 0)
                {
                    if (dQuantity > 0)
                    {
                        dOrginalCost = (dTPrice / dQuantity) / 2;
                        dLabor_rate = 0;
                    }
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

                if (nDirectId == 1)
                    dr["total_retail_Cost"] = dLineItemTotal;
                else
                    dr["total_direct_Cost"] = dLineItemTotal;
            
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
                   " is_direct,section_level,location.location_id,'' as tmpCo,  cast(0 as decimal(18,2)) as total_retail_Cost, cast(0 as decimal(18,2)) as total_direct_Cost, 1 as TypeId  " +
                   " FROM pricing_details " +
                   " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND   pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " AND " +
                   " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                   " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                    " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " ) " +
                   " order by location.location_name";
            DataTable dt = csCommonUtility.GetDataTable(strP);
            DataRow drNew = null;
            string strQ = "SELECT change_order_pricing_list.co_pricing_list_id, change_order_pricing_list.item_id, ISNULL(item_price.labor_id,0) AS labor_id, change_order_pricing_list.section_serial, location.location_name AS section_name, " +
                         " change_order_pricing_list.section_name AS location_name, change_order_pricing_list.item_name, change_order_pricing_list.measure_unit, ISNULL(item_price.item_cost,0) AS item_cost, change_order_pricing_list.total_retail_price, " +
                        " change_order_pricing_list.total_direct_price, ISNULL(item_price.minimum_qty,0) AS minimum_qty, change_order_pricing_list.quantity, ISNULL(item_price.retail_multiplier,0) AS retail_multiplier, ISNULL(item_price.labor_rate,0) AS labor_rate, change_order_pricing_list.short_notes, change_order_pricing_list.item_status_id, " +
                        " change_order_pricing_list.last_update_date, change_order_pricing_list.is_direct, change_order_pricing_list.section_level, location.location_id, changeorder_estimate.changeorder_name, " +
                       "  changeorder_estimate.execute_date " +
                        " FROM  change_order_pricing_list " +
                        " INNER JOIN location ON location.location_id = change_order_pricing_list.location_id " +
                        " INNER JOIN  changeorder_estimate ON changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
                         " LEFT OUTER JOIN item_price ON item_price.item_id = change_order_pricing_list.item_id " +
                           " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and is_direct = " + nDirectId + " AND changeorder_estimate.change_order_status_id = 3 order by location.location_name";
            DataTable dtcol = csCommonUtility.GetDataTable(strQ);
            foreach (DataRow dr in dtcol.Rows)
            {
                 drNew = dt.NewRow();

             int nItemStatusId = Convert.ToInt32(dr["item_status_id"]);
             int ncoPricingUd = Convert.ToInt32(dr["co_pricing_list_id"]);
             if (_db.co_pricing_masters.Any(cpd => cpd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cpd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cpd.co_pricing_list_id == ncoPricingUd))
             {
                 co_pricing_master obj = _db.co_pricing_masters.SingleOrDefault(cpd => cpd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cpd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cpd.co_pricing_list_id == ncoPricingUd);

                 drNew["item_cost"] = obj.item_cost;
                 drNew["labor_rate"] = obj.labor_rate;
                 drNew["minimum_qty"] = obj.minimum_qty;
                 drNew["retail_multiplier"] = obj.retail_multiplier;

             }
             else
             {
                 drNew["item_cost"] = dr["item_cost"];
                 drNew["labor_rate"] = dr["labor_rate"];
                 drNew["minimum_qty"] = dr["minimum_qty"];
                 drNew["retail_multiplier"] = dr["retail_multiplier"];
 
             }

               
                string strTmp = " (" + dr["changeorder_name"].ToString() + ", " + Convert.ToDateTime(dr["execute_date"]).ToShortDateString() + ")";

               
                drNew["co_pricing_list_id"] = dr["co_pricing_list_id"];
                drNew["item_id"] = dr["item_id"];
                drNew["labor_id"] = dr["labor_id"];
                drNew["section_serial"] = dr["section_serial"];
                drNew["location_name"] = dr["location_name"];
                drNew["section_name"] = dr["section_name"];
                drNew["item_name"] = dr["item_name"];
                drNew["measure_unit"] = dr["measure_unit"];
                drNew["total_retail_price"] = dr["total_retail_price"];
                drNew["total_direct_price"] = dr["total_direct_price"];
                drNew["quantity"] = dr["quantity"];
                drNew["short_notes"] = dr["short_notes"];
                drNew["item_status_id"] = dr["item_status_id"];
                drNew["last_update_date"] = dr["last_update_date"];
                drNew["is_direct"] = dr["is_direct"];
                drNew["section_level"] = dr["section_level"];
                drNew["location_id"] = dr["location_id"];
                drNew["tmpCo"] = strTmp;
                drNew["total_retail_Cost"] = 0;
                drNew["total_direct_Cost"] = 0;
                drNew["TypeId"] = 2;
                dt.Rows.Add(drNew);

            }
            foreach (DataRow dr in dt.Rows)
            {
                decimal dOrginalCost = 0;
                decimal dOrginalTotalCost = 0;
                decimal dLaborTotal = 0;
                decimal dLineItemTotal = 0;
                string sItemName = dr["item_name"].ToString();
                int nType = Convert.ToInt32(dr["TypeId"]);
                decimal dItemCost = Convert.ToDecimal(dr["item_cost"]);
                decimal dRetail_multiplier = Convert.ToDecimal(dr["retail_multiplier"]);
                decimal dQuantity = Convert.ToDecimal(dr["quantity"]);
                decimal dLabor_rate = Convert.ToDecimal(dr["labor_rate"]);
                decimal dTPrice = Convert.ToDecimal(dr["total_retail_price"]);
                if (sItemName.IndexOf("Other") > 0)
                {
                    if (dQuantity > 0)
                    {
                        dOrginalCost = (dTPrice / dQuantity) / 2;
                        dLabor_rate = 0;
                    }
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

                if (nDirectId == 1)
                    dr["total_retail_Cost"] = dLineItemTotal;
                else
                    dr["total_direct_Cost"] = dLineItemTotal;

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
            Label lblDleted2 = (Label)e.Row.FindControl("lblDleted2");
            string str = e.Row.Cells[2].Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 25)
            {
                e.Row.Cells[2].ToolTip = str;
                e.Row.Cells[2].Text = str.Substring(0, 25) + "...";
            }
            else
            {
                e.Row.Cells[2].ToolTip = str;
                e.Row.Cells[2].Text = str;

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
                e.Row.Cells[6].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[7].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[8].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[9].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[10].Attributes.CssStyle.Add("text-decoration", "none; color: red ;");
                Label lblT_price2 = (Label)e.Row.Cells[8].FindControl("lblT_price2");
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
                    Label lblT_price2 = (Label)e.Row.Cells[8].FindControl("lblT_price2");
                    lblT_price2.Text = "0.00";
                    lblDleted2.Visible = true;
                    lblDleted2.ForeColor = Color.Red;
                }
                e.Row.Attributes.CssStyle.Add("color", "green");
                //e.Row.Attributes.CssStyle.Add("font-weight", "bold");
            }
            else
            {
                e.Row.Cells[10].Text = "No Change";
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
                //subtotal_diect += Double.Parse((row.FindControl("lblTotal_price2") as Label).Text);
                subtotal_diect += Double.Parse((row.FindControl("lblT_price2") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

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

            userinfo obj = (userinfo)Session["oUser"];

            //if (obj.role_id != 1)
            //{
            //    gv.Columns[7].Visible = false;
            //    if (gv.FooterRow != null)
            //        gv.FooterRow.Visible = false;

            //}
            if (obj.role_id == 1)
            {
                gv.Columns[8].Visible = true;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = true;

            }
            else if (obj.role_id == 4)
            {
                gv.Columns[8].Visible = true;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = true;

            }
            else if (obj.role_id == 5)
            {
                gv.Columns[8].Visible = true;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = true;

            }
            else
            {
                gv.Columns[8].Visible = false;
                if (gv.FooterRow != null)
                    gv.FooterRow.Visible = false;
            }

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
                  " is_direct,section_level,location.location_id,'' as tmpCo,  cast(0 as decimal(18,2)) as total_retail_Cost, cast(0 as decimal(18,2)) as total_direct_Cost, 1 as TypeId " +
                  " FROM pricing_details " +
                  " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                  " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                  " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " AND " +
                   " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                  " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                   " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND  ) " +
                  " order by section_level";
        DataTable dtP = csCommonUtility.GetDataTable(strP);
        DataRow drNew = null;
        string strQ = "  SELECT co_pricing_list_id, change_order_pricing_list.item_id,ISNULL(item_price.labor_id,0) AS labor_id, section_serial,  location.location_name,section_name,item_name,change_order_pricing_list.measure_unit, " +
                     " ISNULL(item_price.item_cost,0) AS item_cost,total_retail_price,total_direct_price, ISNULL(item_price.minimum_qty,0) AS minimum_qty,quantity,ISNULL(item_price.retail_multiplier,0) AS retail_multiplier,ISNULL(item_price.labor_rate,0) AS labor_rate, " +
                     " short_notes,item_status_id,last_update_date,  is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date  " +
                      " FROM change_order_pricing_list  " +
                     " INNER JOIN location on location.location_id = change_order_pricing_list.location_id  " +
                     " INNER JOIN changeorder_estimate on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
                     " LEFT OUTER JOIN item_price ON item_price.item_id = change_order_pricing_list.item_id " +
                       " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND changeorder_estimate.change_order_status_id = 3 order by section_level";
        DataTable dtcol = csCommonUtility.GetDataTable(strQ);
        foreach (DataRow dr in dtcol.Rows)
        {
            drNew = dtP.NewRow();
            int nItemStatusId = Convert.ToInt32(dr["item_status_id"]);
            int ncoPricingUd = Convert.ToInt32(dr["co_pricing_list_id"]);
            if (_db.co_pricing_masters.Any(cpd => cpd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cpd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cpd.co_pricing_list_id == ncoPricingUd))
            {
                co_pricing_master objp = _db.co_pricing_masters.SingleOrDefault(cpd => cpd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cpd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cpd.co_pricing_list_id == ncoPricingUd);

                drNew["item_cost"] = objp.item_cost;
                drNew["labor_rate"] = objp.labor_rate;
                drNew["minimum_qty"] = objp.minimum_qty;
                drNew["retail_multiplier"] = objp.retail_multiplier;

            }
            else
            {
                drNew["item_cost"] = dr["item_cost"];
                drNew["labor_rate"] = dr["labor_rate"];
                drNew["minimum_qty"] = dr["minimum_qty"];
                drNew["retail_multiplier"] = dr["retail_multiplier"];

            }

            string strTmp = " (" + dr["changeorder_name"].ToString() + ", " + Convert.ToDateTime(dr["execute_date"]).ToShortDateString() + ")";


            drNew["co_pricing_list_id"] = dr["co_pricing_list_id"];
            drNew["item_id"] = dr["item_id"];
            drNew["labor_id"] = dr["labor_id"];
            drNew["section_serial"] = dr["section_serial"];
            drNew["location_name"] = dr["location_name"];
            drNew["section_name"] = dr["section_name"];
            drNew["item_name"] = dr["item_name"];
            drNew["measure_unit"] = dr["measure_unit"];
            drNew["total_retail_price"] = dr["total_retail_price"];
            drNew["total_direct_price"] = dr["total_direct_price"];
            drNew["quantity"] = dr["quantity"];
            drNew["short_notes"] = dr["short_notes"];
            drNew["item_status_id"] = dr["item_status_id"];
            drNew["last_update_date"] = dr["last_update_date"];
            drNew["is_direct"] = dr["is_direct"];
            drNew["section_level"] = dr["section_level"];
            drNew["location_id"] = dr["location_id"];
            drNew["tmpCo"] = strTmp;
            drNew["total_retail_Cost"] = 0;
            drNew["total_direct_Cost"] = 0;
            drNew["TypeId"] = 2;

            dtP.Rows.Add(drNew);

        }
        foreach (DataRow dr in dtP.Rows)
        {
            decimal dOrginalCost = 0;
            decimal dOrginalTotalCost = 0;
            decimal dLaborTotal = 0;
            decimal dLineItemTotal = 0;
            string sItemName = dr["item_name"].ToString();

            int nType = Convert.ToInt32(dr["TypeId"]);
            int nDirectId = Convert.ToInt32(dr["is_direct"]);
            int nItemStatusId = Convert.ToInt32(dr["item_status_id"]);
            int ncoPricingUd = Convert.ToInt32(dr["co_pricing_list_id"]);

            decimal dItemCost = Convert.ToDecimal(dr["item_cost"]);
            decimal dRetail_multiplier = Convert.ToDecimal(dr["retail_multiplier"]);
            decimal dQuantity = Convert.ToDecimal(dr["quantity"]);
            decimal dLabor_rate = Convert.ToDecimal(dr["labor_rate"]);
            decimal dTPrice = Convert.ToDecimal(dr["total_retail_price"]);
            if (sItemName.IndexOf("Other") > 0)
            {
                if (dQuantity > 0)
                {
                    dOrginalCost = (dTPrice / dQuantity) / 2;
                    dLabor_rate = 0;
                }
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

            if (nDirectId == 1)
                dr["total_retail_price"] = dLineItemTotal; //total_retail_Cost
            else
                dr["total_direct_price"] = dLineItemTotal;

            if (nItemStatusId == 3)
            {
                decimal nAmount = 0;
                var result = (from ppi in _db.co_pricing_masters
                              where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.co_pricing_list_id == ncoPricingUd
                              select ppi.total_retail_price);
                int n = result.Count();
                if (result != null && n > 0)
                    nAmount = result.Sum();
                else
                {
                    dr["item_status_id"] = 4;
                }
              
            }

        }

        DataView dv = dtP.DefaultView;
        dv.Sort = "last_update_date asc";
        userinfo obj = (userinfo)Session["oUser"];
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
        string strJob = string.Empty;
        if (cus_est.job_number != null) { 
            //strJob = cus_est.job_number;
            if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                strJob = cus_est.job_number;
            else
                strJob = cus_est.alter_job_number;
        }
        ReportDocument rptFile = new ReportDocument();
        string strReportPath = "";
        if (nTypeId == 1)
            strReportPath = Server.MapPath(@"Reports\rpt\CompositeSOWCostbyLocation.rpt");
        else if (nTypeId == 2)
            strReportPath = Server.MapPath(@"Reports\rpt\CompositeSOWCostbySection.rpt");
        else
            strReportPath = Server.MapPath(@"Reports\rpt\CompositeSOWCostbySection.rpt");
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
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;

        int IsQty = 0;
        int IsUom = 0;
        for (int i = 0; i < chkCVOptions.Items.Count; i++)
        {
            if (chkCVOptions.Items[i].Selected == true)
            {
                if (Convert.ToInt32(chkCVOptions.Items[i].Value) == 1)
                    IsQty = 1;
                else if (Convert.ToInt32(chkCVOptions.Items[i].Value) == 2)
                    IsUom = 2;

            }
        }

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
        if (obj.role_id == 1)
        {
            ht.Add("p_Role", 1);
        }
        else if (obj.role_id == 4)
        {
            ht.Add("p_Role", 1);
        }
        else if (obj.role_id == 5)
        {
            ht.Add("p_Role", 1);
        }
        else
        {

            ht.Add("p_Role", obj.role_id);
        }
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
}