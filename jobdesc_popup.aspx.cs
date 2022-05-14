using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class jobdesc_popup : System.Web.UI.Page
{
    private int serial = 0;
    private string strSection = "";
    private int i = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            lnkClose.Attributes.Add("onClick", "CloseWindow();");
            lnkPrint.Attributes.Add("onClick", "PrintWindow();");
            HyperLink1.Attributes.Add("onClick", "DisplayWindow();");

            if (Request.QueryString.Get("jsid") != null)
            {
                int nCustomerId = 0;
                int nEstimateId = 0;
                if (Request.QueryString.Get("cid") != null)
                    nCustomerId = Convert.ToInt32(Request.QueryString.Get("cid"));
                if (Request.QueryString.Get("eid") != null)
                    nEstimateId = Convert.ToInt32(Request.QueryString.Get("eid"));
                int nJobStatusId = Convert.ToInt32(Request.QueryString.Get("jsid"));
                hdnCustomerId.Value = nCustomerId.ToString();
                hdnEstimateId.Value = nEstimateId.ToString();
                DataClassesDataContext _db = new DataClassesDataContext();
                var item = from jd in _db.job_status_descs
                           where jd.jobstatusid == nJobStatusId && jd.customer_id == nCustomerId && jd.estimate_id == nEstimateId
                           orderby jd.status_serial ascending
                           select jd;
                grdDescriptions.DataSource = item;
                grdDescriptions.DataBind();

                if (nJobStatusId == 1)
                    lblJobStatusFor.Text = "Design";
                else if (nJobStatusId == 2)
                    lblJobStatusFor.Text = "Selection";
                else if (nJobStatusId == 3)
                    lblJobStatusFor.Text = "Site Progress & Photos";
                else if (nJobStatusId == 4)
                {
                    lblJobStatusFor.Text = "Schedule";
                    HyperLink1.Visible = true;
                }
                else if (nJobStatusId == 5)
                    lblJobStatusFor.Text = "Final Project Review";
                else if (nJobStatusId == 6)
                    lblJobStatusFor.Text = "Completion Certificate";
                else if (nJobStatusId == 7)
                    lblJobStatusFor.Text = "Warranty";

                if (nJobStatusId == 4)
                {
                    //string strQ = "select * from customer_estimate where customer_id=" + nCustomerId + " and status_id=3 and client_id=1";
                    //IEnumerable<customer_estimate_model> list = _db.ExecuteQuery<customer_estimate_model>(strQ, string.Empty);

                    //foreach (customer_estimate_model cus_est in list)
                    //{
                    //    int nestid = Convert.ToInt32(cus_est.estimate_id);
                    //    hdnEstimateId.Value = nestid.ToString();
                    //}
                    BindSelectedItemGrid();
                    BindSelectedItemGrid_Direct();
                    
                }
                else if (nJobStatusId == 3)
                {
 
                }
              

            }
        }
    }
    protected void grdDescriptions_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nJobStatusid = Convert.ToInt32(e.Row.Cells[0].Text);
            if (nJobStatusid == 1)
                e.Row.Cells[0].Text = "Design";
            else if (nJobStatusid == 2)
                e.Row.Cells[0].Text = "Selection";
            else if (nJobStatusid == 3)
                e.Row.Cells[0].Text = "Site Progress & Photos";
            else if (nJobStatusid == 4)
                e.Row.Cells[0].Text = "Schedule";
            else if (nJobStatusid == 5)
                e.Row.Cells[0].Text = "Final Project Review";
            else if (nJobStatusid == 6)
                e.Row.Cells[0].Text = "Completion Certificate";
            else if (nJobStatusid == 7)
                e.Row.Cells[0].Text = "Warranty";
        }
    }
    public void BindSelectedItemGrid()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "";
        if (rdoSortByWeek.SelectedValue == "2")
        {
            strQ = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from co_pricing_master where co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =1 ) " +
                   " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =1 ) " +
                   " AND  co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND co_pricing_master.client_id =1  order by section_level asc";
            //strQ = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1 ) " +
            //           " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 ) " +
            //           " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =1  order by section_level asc";
        }
        else
        {
            strQ = " select DISTINCT week_id AS colId,'Week: '+ CAST(week_id AS VARCHAR(10)) as colName from co_pricing_master where co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =1 ) " +
                   " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =1 ) " +
                   " AND  co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND co_pricing_master.client_id =1  order by week_id asc";
            //strQ = "select  0 AS colId,'SORTED BY WEEK OF EXCUTION' as colName ";
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
        if (rdoSortByWeek.SelectedValue == "2")
        {
            strQ = "select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from co_pricing_master where co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =1 ) " +
                  " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =1 ) " +
                  " AND  co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND co_pricing_master.client_id =1  order by section_level asc";
            //strQ = "select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =1 ) " +
            //       " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =1 ) " +
            //       " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND pricing_details.client_id =1  order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT week_id AS colId,'Week: '+ CAST(week_id AS VARCHAR(10)) as colName from co_pricing_master where co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =1 ) " +
                   " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =1 ) " +
                   " AND  co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND co_pricing_master.client_id =1  order by week_id asc";
            // strQ = "select  0 AS colId,'SORTED BY WEEK OF EXCUTION' as colName ";
        }
        List<CO_PricingMaster> mList = _db.ExecuteQuery<CO_PricingMaster>(strQ, string.Empty).ToList();
        grdGroupingDirect.DataSource = mList;
        grdGroupingDirect.DataKeyNames = new string[] { "colId" };
        grdGroupingDirect.DataBind();

    }

    protected void btnGotoCustomerList_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
    protected void grdSelectedItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdSelecterdItem = (GridView)sender;
            Label lblUnitExe = (Label)e.Row.FindControl("lblUnitExe");
            DropDownList ddlWeek = (DropDownList)e.Row.FindControl("ddlWeek");
            CheckBox chkComplete = (CheckBox)e.Row.FindControl("chkComplete");
            Label lblScheduleNotes = (Label)e.Row.FindControl("lblScheduleNotes");
            Label lblComplete = (Label)e.Row.FindControl("lblComplete");
            int nPricingId = Convert.ToInt32(grdSelecterdItem.DataKeys[Convert.ToInt32(e.Row.RowIndex)].Values[0]);
            List<change_order_pricing_list> CO_List = _db.change_order_pricing_lists.Where(chl => chl.co_pricing_list_id == nPricingId && chl.customer_id == Convert.ToInt32(hdnCustomerId.Value)).ToList();
            foreach (change_order_pricing_list objcl in CO_List)
            {
                e.Row.Attributes.CssStyle.Add("color", "red");
                ddlWeek.Attributes.CssStyle.Add("color", "red");
                chkComplete.Attributes.CssStyle.Add("color", "red");
                lblScheduleNotes.Attributes.CssStyle.Add("red", "red");
                lblUnitExe.Attributes.CssStyle.Add("color", "red");
            }
            //if (_db.change_order_pricing_lists.Where(chl => chl.co_pricing_list_id == nPricingId && chl.customer_id == Convert.ToInt32(hdnCustomerId.Value)).SingleOrDefault() != null)
            //{
            //    e.Row.Attributes.CssStyle.Add("color", "red");
            //    ddlWeek.Attributes.CssStyle.Add("color", "red");
            //    chkComplete.Attributes.CssStyle.Add("color", "red");
            //    lblScheduleNotes.Attributes.CssStyle.Add("red", "red");
            //    lblUnitExe.Attributes.CssStyle.Add("color", "red");

            //}

            if (chkComplete.Checked)
            {
                lblComplete.Text = "Completed";
                e.Row.Attributes.CssStyle.Add("color", "green");
                ddlWeek.Attributes.CssStyle.Add("color", "green");
                chkComplete.Attributes.CssStyle.Add("color", "green");
                lblScheduleNotes.Attributes.CssStyle.Add("color", "green");
                lblUnitExe.Attributes.CssStyle.Add("color", "green");
                lblComplete.Attributes.CssStyle.Add("color", "green");
                chkComplete.Enabled = false;
            }
            ddlWeek.Enabled = false;

        }

    }


    protected void grdGrouping_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGrouping.DataKeys[e.Row.RowIndex].Values[0]);

            GridView gv = e.Row.FindControl("grdSelectedItem1") as GridView;
            int nDirectId = 1;
            GetData(colId, gv, nDirectId);
            if (rdoSortByWeek.SelectedValue == "2")
            {
                gv.Columns[2].Visible = false;
            }
            else
            {
                gv.Columns[2].Visible = true;
            }
            foreach (GridViewRow row in gv.Rows)
            {
                Label lblUnitExe = (Label)row.FindControl("lblUnitExe");

                serial = serial + 1;
                if (Convert.ToDecimal(lblUnitExe.Text) == 0)
                {
                    lblUnitExe.Text = serial.ToString();
                    //lblUnitExe.Text = serial.ToString();
                }


            }

        }
    }
    private void GetData(int colId, GridView grd, int nDirectId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (rdoSortByWeek.SelectedValue == "2")
        {
            var price_detail = from p in _db.co_pricing_masters
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.changeorder_locations
                                      where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == 1
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.changeorder_sections
                                       where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == 1
                                       select cs.section_id).Contains(p.section_level)
                                      && p.item_status_id != 3  && p.section_level == colId && p.is_direct == nDirectId && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.client_id == 1
                               orderby p.week_id,p.section_level, p.execution_unit, lc.location_name ascending

                               select new CO_PricingDeatilModel()
                               {
                                   co_pricing_list_id = (int)p.co_pricing_list_id,
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
                                   item_status_id = (int)p.item_status_id,
                                   tmpCol = string.Empty,
                                   week_id = (int)p.week_id,
                                   execution_unit = (decimal)p.execution_unit,
                                   is_complete = (bool)p.is_complete,
                                   schedule_note = p.schedule_note
                               };

            
            grd.DataSource = price_detail.ToList();
            grd.DataKeyNames = new string[] { "co_pricing_list_id", "section_name" };
            grd.DataBind();
        }
        else
        {
            var price_detail = from p in _db.co_pricing_masters
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.customer_locations
                                      where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == 1
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.customer_sections
                                       where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == 1
                                       select cs.section_id).Contains(p.section_level)
                                       && p.item_status_id != 3 && p.week_id == colId && p.is_direct == nDirectId && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.client_id == 1
                               orderby p.week_id,p.section_level, p.execution_unit, lc.location_name ascending

                               select new CO_PricingDeatilModel()
                               {
                                   co_pricing_list_id = (int)p.co_pricing_list_id,
                                   item_id = (int)p.item_id,
                                   labor_id = (int)p.labor_id,
                                   section_serial = (decimal)p.section_serial,
                                   section_name = p.section_name,
                                   location_name = lc.location_name,
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
                                   week_id = (int)p.week_id,
                                   execution_unit = (decimal)p.execution_unit,
                                   is_complete = (bool)p.is_complete,
                                   schedule_note = p.schedule_note
                               };
            grd.DataSource = price_detail.ToList();
            grd.DataKeyNames = new string[] { "co_pricing_list_id", "section_name" };
            grd.DataBind();
        }




    }

    protected void grdSelectedItem2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdSelecterdItem1 = (GridView)sender;
            Label lblUnitExe1 = (Label)e.Row.FindControl("lblUnitExe1");
            DropDownList ddlWeek1 = (DropDownList)e.Row.FindControl("ddlWeek1");
            CheckBox chkComplete1 = (CheckBox)e.Row.FindControl("chkComplete1");
            Label lblScheduleNotes1 = (Label)e.Row.FindControl("lblScheduleNotes1");
            Label lblComplete1 = (Label)e.Row.FindControl("lblComplete1");
            int serial = e.Row.RowIndex + 1;
            if (Convert.ToDecimal(lblUnitExe1.Text) == 0)
            {
                lblUnitExe1.Text = serial.ToString();
                //lblUnitExe1.Text = serial.ToString();
            }
            int nPricingId = Convert.ToInt32(grdSelecterdItem1.DataKeys[Convert.ToInt32(e.Row.RowIndex)].Values[0]);
            List<change_order_pricing_list> CO_List = _db.change_order_pricing_lists.Where(chl => chl.co_pricing_list_id == nPricingId && chl.customer_id == Convert.ToInt32(hdnCustomerId.Value)).ToList();
            foreach (change_order_pricing_list objcl in CO_List)
            {
                e.Row.Attributes.CssStyle.Add("color", "red");
                ddlWeek1.Attributes.CssStyle.Add("color", "red");
                chkComplete1.Attributes.CssStyle.Add("color", "red");
                lblScheduleNotes1.Attributes.CssStyle.Add("red", "red");
                lblUnitExe1.Attributes.CssStyle.Add("color", "red");
            }

            //if (_db.change_order_pricing_lists.Where(chl => chl.co_pricing_list_id == nPricingId && chl.customer_id == Convert.ToInt32(hdnCustomerId.Value)).SingleOrDefault() != null)
            //{
            //    e.Row.Attributes.CssStyle.Add("color", "red");
            //    ddlWeek1.Attributes.CssStyle.Add("color", "red");
            //    chkComplete1.Attributes.CssStyle.Add("color", "red");
            //    lblScheduleNotes1.Attributes.CssStyle.Add("red", "red");
            //    lblUnitExe1.Attributes.CssStyle.Add("color", "red");

            //}
            if (chkComplete1.Checked)
            {
                lblComplete1.Text = "Completed";
                e.Row.Attributes.CssStyle.Add("color", "green");
                ddlWeek1.Attributes.CssStyle.Add("color", "green");
                chkComplete1.Attributes.CssStyle.Add("color", "green");
                lblScheduleNotes1.Attributes.CssStyle.Add("color", "green");
                lblUnitExe1.Attributes.CssStyle.Add("color", "green");
                lblComplete1.Attributes.CssStyle.Add("color", "green");
                
                chkComplete1.Enabled = false;
            }
            ddlWeek1.Enabled = false;

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
            foreach (GridViewRow row in gv.Rows)
            {
                
                Label lblUnitExe1 = (Label)row.FindControl("lblUnitExe1");
                serial = serial + 1;
                if (Convert.ToDecimal(lblUnitExe1.Text) == 0)
                {
                    lblUnitExe1.Text = serial.ToString();
                   
                }
            }
            if (rdoSortByWeek.SelectedValue == "2")
            {
                gv.Columns[2].Visible = false;
            }
            else
            {
                gv.Columns[2].Visible = true;
            }
            if (gv.Rows.Count == 0)
            {
                grdGroupingDirect.Visible = false;
            }


        }
    }
   
}
