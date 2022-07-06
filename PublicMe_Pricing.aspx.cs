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

public partial class PublicMe_Pricing : System.Web.UI.Page
{
    string strDetails = "";
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

            if (Request.QueryString.Get("meid") != null)
                hdnEstimateId.Value = Request.QueryString.Get("meid");

            userinfo obj = (userinfo)Session["oUser"];
            hdnSalesId.Value = obj.sales_person_id.ToString();

            if (Request.QueryString.Get("spid") != null)
            {
                hdnSalesPersonId.Value = Convert.ToInt32(Request.QueryString.Get("spid")).ToString();
            }
            else
            {
                hdnSalesPersonId.Value = obj.sales_person_id.ToString();
            }


            DataClassesDataContext _db = new DataClassesDataContext();
            model_estimate me = new model_estimate();
            me = _db.model_estimates.Single(mest => mest.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && mest.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) );

            lblModelEstimateName.Text = me.model_estimate_name;
            lblComments.Text = me.estimate_comments;
            chkIsPublic.Checked = Convert.ToBoolean(me.IsPublic);

       
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
                //tblTotalProjectPrice.Visible = true;
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

    public void BindSelectedItemGrid()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "";
        if (rdoSort.SelectedValue == "2")
        {
            strQ = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from model_estimate_pricing where model_estimate_pricing.location_id IN (Select location_id from model_estimate_locations WHERE model_estimate_locations.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_locations.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value)  + " ) " +
                   " AND model_estimate_pricing.section_level IN (Select section_id from model_estimate_sections WHERE model_estimate_sections.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_sections.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " ) " +
                   " AND model_estimate_pricing.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_pricing.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND is_direct=1   order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT model_estimate_pricing.location_id AS colId,'LOCATION: '+ location.location_name as colName,sort_id from model_estimate_pricing  INNER JOIN location on location.location_id = model_estimate_pricing.location_id where model_estimate_pricing.location_id IN (Select location_id from model_estimate_locations WHERE model_estimate_locations.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_locations.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) +  " ) " +
                " AND model_estimate_pricing.section_level IN (Select section_id from model_estimate_sections WHERE model_estimate_sections.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_sections.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " ) " +
                " AND model_estimate_pricing.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_pricing.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND is_direct=1  order by sort_id asc";
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
            strQ = "select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from model_estimate_pricing where model_estimate_pricing.location_id IN (Select location_id from model_estimate_locations WHERE model_estimate_locations.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_locations.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) +" ) " +
                   " AND model_estimate_pricing.section_level IN (Select section_id from model_estimate_sections WHERE model_estimate_sections.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_sections.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " ) " +
                   " AND model_estimate_pricing.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_pricing.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND is_direct=2   order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT model_estimate_pricing.location_id AS colId,'LOCATION: '+ location.location_name as colName,sort_id from model_estimate_pricing  INNER JOIN location on location.location_id = model_estimate_pricing.location_id where model_estimate_pricing.location_id IN (Select location_id from model_estimate_locations WHERE model_estimate_locations.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_locations.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) +  " ) " +
                   " AND model_estimate_pricing.section_level IN (Select section_id from model_estimate_sections WHERE model_estimate_sections.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_sections.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) +  " ) " +
                   " AND model_estimate_pricing.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_pricing.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND is_direct=2   order by sort_id asc";
        }
        List<PricingMaster> mList = _db.ExecuteQuery<PricingMaster>(strQ, string.Empty).ToList();
        grdGroupingDirect.DataSource = mList;
        grdGroupingDirect.DataKeyNames = new string[] { "colId" };
        grdGroupingDirect.DataBind();

    }
    private decimal GetRetailTotal()
    {
        decimal dRetail = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.model_estimate_pricings
                      where (from clc in _db.model_estimate_locations
                             where clc.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) 
                             select clc.location_id).Contains(pd.location_id) &&
                             (from cs in _db.model_estimate_sections
                              where cs.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) 
                              select cs.section_id).Contains(pd.section_level) && pd.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && pd.pricing_type == "A"
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
        var result = (from pd in _db.model_estimate_pricings
                      where (from clc in _db.model_estimate_locations
                             where clc.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) 
                             select clc.location_id).Contains(pd.location_id) &&
                             (from cs in _db.model_estimate_sections
                              where cs.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value)
                              select cs.section_id).Contains(pd.section_level) && pd.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && pd.pricing_type == "A"
                      select pd.total_direct_price);
        int n = result.Count();
        if (result != null && n > 0)
            dDirect = result.Sum();

        return dDirect;
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
    protected string GetTotalPrice()
    {
        return "Total: " + grandtotal.ToString("c");
    }
    protected string GetTotalPriceDirect()
    {
        return "Total: " + grandtotal_direct.ToString("c");
    }

    protected void grdGrouping_RowCommand(object sender, GridViewCommandEventArgs e)
    {
       
        if (e.CommandName == "assigntocustomer")
        {

            int index = Convert.ToInt32(e.CommandArgument);
            int nLocId = Convert.ToInt32(grdGrouping.DataKeys[index].Values[0]);
            Response.Redirect("assigntomodelstimate.aspx? type=2&locid=" + nLocId + "&meid=" + Convert.ToInt32(hdnEstimateId.Value) + "&spid=" + Convert.ToInt32(hdnSalesPersonId.Value));

        }

    }
    protected void grdGrouping_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGrouping.DataKeys[e.Row.RowIndex].Values[0]);
            LinkButton InkAssign = (LinkButton)e.Row.FindControl("InkAssign");
            GridView gv = e.Row.FindControl("grdSelectedItem1") as GridView;

            if (rdoSort.SelectedValue == "1")
            {
               
                InkAssign.Visible = true;
            }
            else
            {
               
                InkAssign.Visible = false;

            }

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
            var price_detail = from p in _db.model_estimate_pricings
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.model_estimate_locations
                                      where clc.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) 
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.model_estimate_sections
                                       where cs.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) 
                                       select cs.section_id).Contains(p.section_level) && p.location_id == colId && p.is_direct == nDirectId && p.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) &&  p.pricing_type == "A"
                               orderby p.section_level ascending

                               select new PricingDetailModel()
                               {
                                   pricing_id = (int)p.me_pricing_id,
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
                                   pricing_type = "A",
                                   last_update_date = (DateTime)p.last_updated_date
                               };
         
            grd.DataSource = price_detail.ToList();
            grd.DataKeyNames = new string[] { "pricing_id" };
            grd.DataBind();
        }
        else
        {
            var price_detail = from p in _db.model_estimate_pricings
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.model_estimate_locations
                                      where clc.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) 
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.model_estimate_sections
                                       where cs.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) 
                                       select cs.section_id).Contains(p.section_level)
                                      && p.section_level == colId && p.is_direct == nDirectId && p.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value)  && p.pricing_type == "A"
                               orderby lc.location_name ascending

                               select new PricingDetailModel()
                               {
                                   pricing_id = (int)p.me_pricing_id,
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
                                   pricing_type = "A",
                                   last_update_date = (DateTime)p.last_updated_date
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
            Label lblshort_notes1 = (Label)e.Row.FindControl("lblshort_notes1");
            TextBox txtshort_notes1 = (TextBox)e.Row.FindControl("txtshort_notes1");
            string str = lblshort_notes1.Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 25)
            {
                txtshort_notes1.Text = str;
                lblshort_notes1.Text = str.Substring(0, 25) + "...";
                lblshort_notes1.ToolTip = str;

            }
            else
            {
                txtshort_notes1.Text = str;
                lblshort_notes1.Text = str;

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
    protected void btnCopyEstimate_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnCopyEstimate.ID, btnCopyEstimate.GetType().Name, "Click"); 
        lblResult1.Text = "";


        DataClassesDataContext _db = new DataClassesDataContext();
        if (Convert.ToInt32(hdnEstimateId.Value) > 0)
        {

            List<model_estimate_location> me_LocList = _db.model_estimate_locations.Where(cl => cl.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cl.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value)).ToList();
            List<model_estimate_section> me_SecList = _db.model_estimate_sections.Where(cs => cs.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) ).ToList();
            List<model_estimate_pricing> Pd_List = _db.model_estimate_pricings.Where(pd => pd.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value)  && pd.pricing_type == "A").ToList();

            int nEstId = 0;
            var result = (from ce in _db.model_estimates
                          where ce.sales_person_id == Convert.ToInt32(hdnSalesId.Value) 
                          select ce.model_estimate_id);

            int n = result.Count();
            if (result != null && n > 0)
                nEstId = result.Max();
            nEstId = nEstId + 1;
            hdnModEstimateNewId.Value = nEstId.ToString();
            model_estimate Mod_est = new model_estimate();
            Mod_est.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            Mod_est.model_estimate_id = Convert.ToInt32(hdnModEstimateNewId.Value);
            Mod_est.status_id = 1;
            Mod_est.estimate_comments = lblComments.Text;
            Mod_est.sales_person_id = Convert.ToInt32(hdnSalesId.Value);
            string est = "Copy of" + " " + lblModelEstimateName.Text + " #" + hdnModEstimateNewId.Value;
            Mod_est.model_estimate_name = est;
            Mod_est.create_date = DateTime.Now;
            Mod_est.last_udated_date = DateTime.Now;
            Mod_est.IsPublic = false;
            _db.model_estimates.InsertOnSubmit(Mod_est);
            foreach (model_estimate_location objcl in me_LocList)
            {
                model_estimate_location me_loc = new model_estimate_location();
                me_loc.client_id = objcl.client_id;
                me_loc.location_id = objcl.location_id;
                me_loc.sales_person_id = Convert.ToInt32(hdnSalesId.Value);
                me_loc.model_estimate_id = Convert.ToInt32(hdnModEstimateNewId.Value);
                _db.model_estimate_locations.InsertOnSubmit(me_loc);
            }
            foreach (model_estimate_section objcs in me_SecList)
            {
                model_estimate_section me_sec = new model_estimate_section();
                me_sec.client_id = objcs.client_id;
                me_sec.section_id = objcs.section_id;
                me_sec.model_estimate_id = Convert.ToInt32(hdnModEstimateNewId.Value);
                me_sec.sales_person_id = Convert.ToInt32(hdnSalesId.Value);
                _db.model_estimate_sections.InsertOnSubmit(me_sec);
            }
            foreach (model_estimate_pricing objPd in Pd_List)
            {
                model_estimate_pricing pd = new model_estimate_pricing();
                pd.client_id = objPd.client_id; ;
                // pd.customer_id = objPd.customer_id;
                pd.model_estimate_id = Convert.ToInt32(hdnModEstimateNewId.Value);
                pd.location_id = objPd.location_id; ;
                pd.sales_person_id = Convert.ToInt32(hdnSalesId.Value);
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

        }
        Response.Redirect("me_pricing.aspx?meid=" + hdnModEstimateNewId.Value + "&spid=" + hdnSalesId.Value);
    }
    protected void grdSelectedItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblshort_notes = (Label)e.Row.FindControl("lblshort_notes");
            TextBox txtshort_notes = (TextBox)e.Row.FindControl("txtshort_notes");
            string str = lblshort_notes.Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 25)
            {
                txtshort_notes.Text = str;
                lblshort_notes.Text = str.Substring(0, 25) + "...";
                lblshort_notes.ToolTip = str;

            }
            else
            {
                txtshort_notes.Text = str;
                lblshort_notes.Text = str;

            }

        }

    }
}
