using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mcomposite_sow : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetItemName(String prefixText, Int32 count)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        int nCustID = Convert.ToInt32(HttpContext.Current.Session["cCustId"]);
        int nEstID = Convert.ToInt32(HttpContext.Current.Session["cEstId"]);

        int nCrewID = 0;

        if (HttpContext.Current.Session["oUser"] != null)
        {
            userinfo obj = (userinfo)HttpContext.Current.Session["oUser"];
        }
        if (HttpContext.Current.Session["oCrew"] != null)
        {
            Crew_Detail objC = (Crew_Detail)HttpContext.Current.Session["oCrew"];
            nCrewID = Convert.ToInt32(HttpContext.Current.Session["cCrewId"]);
        }

        string strCondition = string.Empty;
        string[] strIds = prefixText.Split(',');
        foreach (string strId in strIds)
        {
            if (strCondition.Length == 0)
                strCondition = " section_name LIKE '%" + strId.Trim().Replace("'", "''") + "%'" + " or item_name LIKE '%" + strId.Trim().Replace("'", "''") + "%'";
            else
                strCondition += "or section_name LIKE '%" + strId.Trim().Replace("'", "''") + "%'" + " or item_name LIKE '%" + strId.Trim().Replace("'", "''") + "%'";

        }
        string sSql = string.Empty;
        if (HttpContext.Current.Session["oCrew"] != null)
        {
            if (_db.co_pricing_masters.Where(cl => cl.customer_id == nCustID && cl.estimate_id == nEstID).ToList().Count == 0)
            {
                sSql = "SELECT DISTINCT section_name + '>>' + item_name as item_name FROM pricing_details WHERE  customer_id =" + nCustID + " AND estimate_id = " + nEstID + " AND section_level IN (SELECT section_id FROM crew_section WHERE crew_Id =  " + nCrewID + ")  AND (" + strCondition + ")";

            }
            else
            {
                sSql = "SELECT DISTINCT section_name + '>>' + item_name as item_name FROM co_pricing_master WHERE  customer_id =" + nCustID + " AND estimate_id = " + nEstID + " AND item_status_id <>3 AND section_level IN (SELECT section_id FROM crew_section WHERE crew_Id =  " + nCrewID + ")  AND (" + strCondition + ")";
            }
        }
        if (HttpContext.Current.Session["oUser"] != null)
        {
            if (_db.co_pricing_masters.Where(cl => cl.customer_id == nCustID && cl.estimate_id == nEstID).ToList().Count == 0)
            {
                sSql = "SELECT DISTINCT section_name + '>>' + item_name as item_name FROM pricing_details WHERE  customer_id =" + nCustID + " AND estimate_id = " + nEstID + " AND (" + strCondition + ")";

            }
            else
            {
                sSql = "SELECT DISTINCT section_name + '>>' + item_name as item_name FROM co_pricing_master WHERE customer_id =" + nCustID + " AND estimate_id = " + nEstID + " AND item_status_id <>3 AND  (" + strCondition + ")";
            }
        }

        return _db.ExecuteQuery<string>(sSql).Take<String>(count).ToArray();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null && Session["oCrew"] == null)
            {
                Response.Redirect("mobile.aspx");
            }
            if (Request.QueryString.Get("cid") != null)
            {
                hdnCustomerId.Value = Convert.ToInt32(Request.QueryString.Get("cid")).ToString();
                Session.Add("cCustId", hdnCustomerId.Value);
            }
            if (Request.QueryString.Get("nestid") != null)
            {
                hdnEstimateId.Value = Convert.ToInt32(Request.QueryString.Get("nestid")).ToString();
                Session.Add("cEstId", hdnEstimateId.Value);
            }
            if (Session["oCrew"] != null)
            {
                Crew_Detail objC = (Crew_Detail)Session["oCrew"];
                hdnCrewId.Value = objC.crew_id.ToString();
                Session.Add("cCrewId", hdnCrewId.Value);
            }

            GetCustomerName(Convert.ToInt32(hdnCustomerId.Value));


            BindEstimate();
           
            Session.Remove("Item_list");
            Session.Remove("Item_list_Direct");
            Session.Remove("sItem_list");
            Session.Remove("sItem_list_Direct");

            BindSelectedItemGrid();
            BindSelectedItemGrid_Direct();


           

            
        }
    }

    private void GetCustomerName(int nCustId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        customer objCust = _db.customers.SingleOrDefault(c => c.customer_id ==nCustId);
        if (objCust != null)
            lblCustomerName.Text = "(" + objCust.last_name1 + ", " + GetJobNumber(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnEstimateId.Value)) + ")";
        else
            lblCustomerName.Text = "";

        hdnClientId.Value = objCust.client_id.ToString();
    }

    private string GetJobNumber(int nCustId,int nEstId)
    {
        string strJobNumber = csCommonUtility.GetCustomerEstimateInfo(nCustId, nEstId).alter_job_number ?? "";
        if (strJobNumber == "")
        {
            strJobNumber = csCommonUtility.GetCustomerEstimateInfo(nCustId, nEstId).job_number ?? "";
        }
        return strJobNumber;
    }

    private void BindEstimate()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select customer_estimate_id, estimate_id, customer_id,estimate_name+' ('+ sale_date+')' as estimate_name, client_id, sales_person_id, status_id, " +
                         
                         " create_date, last_update_date, sale_date, estimate_comments, job_start_date, tax_rate, " +
                         " job_number, IsEstimateActive, IsCustDisplay, JobId " +
                         " from customer_estimate where status_id = 3  and customer_id=" + hdnCustomerId.Value + " and client_id=" + Convert.ToInt32(hdnClientId.Value) +
                         " Order by convert(datetime,sale_date) desc";

          DataTable dt = csCommonUtility.GetDataTable(strQ);
       // IEnumerable<customer_estimate_model> list = _db.ExecuteQuery<customer_estimate_model>(strQ, string.Empty);

        if (dt.Rows.Count > 1)
        {
            pnlEstimate.Visible = true;
            rdbEstimate.DataSource = dt;
            rdbEstimate.DataTextField = "estimate_name";
            rdbEstimate.DataValueField = "estimate_id";
            rdbEstimate.DataBind();
            rdbEstimate.SelectedValue = hdnEstimateId.Value;
        }
        else
        {
            pnlEstimate.Visible = false;
        }
       
       
    }
    public void BindSelectedItemGrid()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        try
        {

            string strQ1 = string.Empty;
            string strQ2 = string.Empty;
            if (Session["oCrew"] != null)
            {
              
                strQ1 = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                     " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " and section_id IN (SELECT section_id FROM crew_section WHERE crew_Id = " + Convert.ToInt32(hdnCrewId.Value) + ")) " +
                     " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc ";
            }

            if (Session["oUser"] != null)
            {
               
                strQ1 = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                     " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) +" ) " + 
                     " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc ";

            }
            
            

            DataTable dtsec = csCommonUtility.GetDataTable(strQ1);
            if ((dtsec == null) || (dtsec.Rows.Count == 0))
            {
                dtsec = LoadMasterTable();
            }

            if (Session["oCrew"] != null)
            {
                strQ2 = "select DISTINCT cop.section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list cop " +
                              " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                               " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct=1 AND cop.client_id = "+ Convert.ToInt32(hdnClientId.Value)+" and cop.section_level IN (SELECT section_id FROM crew_section WHERE crew_Id = " + Convert.ToInt32(hdnCrewId.Value) + ")  " +
                               " order by section_level asc";
            }


            if (Session["oUser"] != null)
            {
                strQ2 = "select DISTINCT cop.section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list cop " +
                              " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                               " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct=1 AND cop.client_id = " + Convert.ToInt32(hdnClientId.Value) + 
                               " order by section_level asc";
            }
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
            Session.Add("MainDTSec", dtsec);
            grdGrouping.DataSource = dtsec;
            grdGrouping.DataKeyNames = new string[] { "colId" };
            grdGrouping.DataBind();
        }
        catch (Exception ex)
        {
            string ext = ex.StackTrace;
            throw ex;
        }


    }
    public void BindSelectedItemGrid_Direct()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ1 = string.Empty;
        string strQ2 = string.Empty;
        if (Session["oCrew"] != null)
        {
             strQ1 = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  ) " +
                 " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " and section_id IN (SELECT section_id FROM crew_section WHERE crew_Id = " + Convert.ToInt32(hdnCrewId.Value) + ") ) " +
                 " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = 2 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
        }
        if (Session["oUser"] != null)
        {
            strQ1 = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  ) " +
                 " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) +" ) " + 
                 " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = 2 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
        }

        DataTable dtsec = csCommonUtility.GetDataTable(strQ1);
        if ((dtsec == null) || (dtsec.Rows.Count == 0))
        {
            dtsec = LoadMasterTable();
        }
        if (Session["oCrew"] != null)
        {
             strQ2 = "select DISTINCT cop.section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list cop " +
                     " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                      " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct = 2 AND cop.client_id = "+ hdnClientId.Value + " and cop.section_level IN (SELECT section_id FROM crew_section WHERE crew_Id = " + Convert.ToInt32(hdnCrewId.Value) + ")  " +
                      " order by section_level asc";
        }
        if (Session["oUser"] != null)
        {
            strQ2 = "select DISTINCT cop.section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list cop " +
                    " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                     " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct = 2 AND cop.client_id = " + hdnClientId.Value +
                     " order by section_level asc";
        }
        DataTable dtsec2 = csCommonUtility.GetDataTable(strQ2);
        DataRow drNew = null;
        if (dtsec2.Rows.Count > 0)
        {
            foreach (DataRow dr in dtsec2.Rows)
            {
                int ncolId = Convert.ToInt32(dr["colId"]);
                string strColName = dr["colName"].ToString();
                bool Iexists = dtsec.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(ncolId)).Count() > 0;
                if (!Iexists)  //if (!ContainDataRowInDataTable(dtsec, dr))
                {
                    drNew = dtsec.NewRow();
                    drNew["colId"] = ncolId;
                    drNew["colName"] = strColName;
                    dtsec.Rows.Add(drNew);
                }
            }
        }
        Session.Add("MainDTSecDirect", dtsec);
        grdGroupingDirect.DataSource = dtsec;
        grdGroupingDirect.DataKeyNames = new string[] { "colId" };
        grdGroupingDirect.DataBind();
    }
    private DataTable LoadMasterTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("colId", typeof(int));
        table.Columns.Add("colName", typeof(string));
        return table;
    }
    protected void grdGrouping_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGrouping.DataKeys[e.Row.RowIndex].Values[0]);
            GridView gv = e.Row.FindControl("grdSelectedItem1") as GridView;
            
             if (Session["sItem_list"] != null)
            {
                DataTable dtItemdata = (DataTable)Session["sItem_list"];
                DataView dv = dtItemdata.DefaultView;
                dv.RowFilter = "section_level =" + colId;
                dv.Sort = "last_update_date ASC";
                gv.DataSource = dv;
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price" };
                gv.DataBind();
            }
            else if (Session["Item_list"] != null)
            {
                DataTable dtItemdata = (DataTable)Session["Item_list"];
                DataView dv = dtItemdata.DefaultView;
                dv.RowFilter = "section_level =" + colId;
                dv.Sort = "last_update_date ASC";


                gv.DataSource = dv;
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price" };
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
                Label lblHeader = headerRow.FindControl("lblHeader") as Label;
                lblHeader.Text = "Location";
            }



        }

    }
    protected void grdSelectedItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv1 = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //string str = e.Row.Cells[0].Text.Replace("&nbsp;", "");
            //if (str != "" && str.Length > 100)
            //{
            //    e.Row.Cells[0].ToolTip = str;
            //    e.Row.Cells[0].Text = str.Substring(0, 100) + "...";
            //}
            //else
            //{
            //    e.Row.Cells[0].ToolTip = str;
            //    e.Row.Cells[0].Text = str;

            //}

            //string strNew = e.Row.Cells[5].Text.Replace("&nbsp;", "");
            //if (strNew != "" && strNew.Length > 100)
            //{
            //    e.Row.Cells[5].ToolTip = strNew;
            //    e.Row.Cells[5].Text = strNew.Substring(0, 100) + "...";
            //}
            //else
            //{
            //    e.Row.Cells[5].ToolTip = strNew;
            //    e.Row.Cells[5].Text = strNew;

            //}

            Label lblDleted1 = (Label)e.Row.FindControl("lblDleted1");
            int nItemStatusId = Convert.ToInt32(gv1.DataKeys[e.Row.RowIndex].Values[1]);
            int ncoPricingUd = Convert.ToInt32(gv1.DataKeys[e.Row.RowIndex].Values[0]);
            if (nItemStatusId == 2) //BABU
            {
                e.Row.Cells[0].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[1].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[2].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[3].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[4].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[5].Attributes.CssStyle.Add("text-decoration", "none; color: red ;");
                //e.Row.Cells[5].Text = "Item Deleted" + e.Row.Cells[5].Text;

            }
            else if (nItemStatusId == 3)
            {
                e.Row.Attributes.CssStyle.Add("color", "green");
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
                    
                    lblDleted1.Visible = true;
                    lblDleted1.ForeColor = Color.Red;
                }
            }
            else
            {
               // e.Row.Cells[5].Text = "No Change";
            }




        }

    }
    private void GetData(int colId, GridView grd, int nDirectId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strP = string.Empty;
        //if (hdnCOMasterDataExist.Value == "1")
        //{
        //    strP = " SELECT co_pricing_list_id, item_id, labor_id, section_serial, " +
        //       " location.location_name as section_name,section_name as location_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
        //      " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,short_notes_new,1 as item_status_id,last_update_date, " +
        //      " is_direct,section_level,location.location_id,'' as tmpCo  " +
        //      " FROM co_pricing_master " +
        //      " INNER JOIN location on location.location_id = co_pricing_master.location_id where co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //      " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //      " AND co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND   co_pricing_master.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND " +
        //      " co_pricing_master.co_pricing_list_id  NOT IN (  SELECT PD.co_pricing_list_id FROM co_pricing_master PD INNER JOIN " +
        //      " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
        //       " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND PD.client_id = 1 ) " +
        //      " order by location.location_name";
        //}
        //else
        //{
        //     strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, " +
        //       " location.location_name as section_name,section_name as location_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
        //      " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,short_notes_new,1 as item_status_id,last_update_date, " +
        //      " is_direct,section_level,location.location_id,'' as tmpCo  " +
        //      " FROM pricing_details " +
        //      " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //      " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //      " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND   pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND " +
        //      " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
        //      " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
        //       " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND PD.client_id = 1 ) " +
        //      " order by location.location_name";
        //}

        strP = " SELECT pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, " +
              " location.location_name as section_name,section_name as location_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
             " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,short_notes_new,1 as item_status_id,last_update_date, " +
             " is_direct,section_level,location.location_id,'' as tmpCo  " +
             " FROM pricing_details " +
             " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
             " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
             " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND   pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + " AND " +
             " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
             " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
              " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND PD.client_id = 1 ) " +
             " order by location.location_name";
       
        DataTable dt = csCommonUtility.GetDataTable(strP);
        DataRow drNew = null;
        string strQ = " SELECT co_pricing_list_id, item_id,1 as labor_id, section_serial, " +
                       " location.location_name AS section_name,section_name AS location_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
                       " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes, short_notes_new,item_status_id,last_update_date, " +
                       " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date " +
                       " FROM change_order_pricing_list " +
                       " INNER JOIN location on location.location_id = change_order_pricing_list.location_id " +
                       " INNER JOIN changeorder_estimate  on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
                       " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and is_direct = " + nDirectId + " AND changeorder_estimate.change_order_status_id = 3 order by location.location_name";

        //string strQ = " SELECT change_order_pricing_list.co_pricing_list_id, change_order_pricing_list.item_id,1 as labor_id, change_order_pricing_list.section_serial,  location.location_name AS section_name,change_order_pricing_list.section_name AS location_name,change_order_pricing_list.item_name,change_order_pricing_list.measure_unit,1 as item_cost, "+
        //              " change_order_pricing_list.total_retail_price,change_order_pricing_list.total_direct_price,  1 as minimum_qty,change_order_pricing_list.quantity,1 as retail_multiplier,1 as labor_rate,change_order_pricing_list.short_notes,co_pricing_master.short_notes_new,change_order_pricing_list.item_status_id,change_order_pricing_list.last_update_date, "+
        //               " change_order_pricing_list.is_direct,change_order_pricing_list.section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date  FROM change_order_pricing_list " +
        //               " INNER JOIN location on location.location_id = change_order_pricing_list.location_id "+
        //               " INNER JOIN changeorder_estimate  on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id  "+
        //               " left outer join co_pricing_master on co_pricing_master.customer_id = change_order_pricing_list.customer_id AND  co_pricing_master.estimate_id = change_order_pricing_list.estimate_id AND  co_pricing_master.co_pricing_list_id = change_order_pricing_list.co_pricing_list_id  "+
        //               " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and change_order_pricing_list.is_direct = " + nDirectId + " AND changeorder_estimate.change_order_status_id = 3 order by location.location_name";
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
            drNew["short_notes_new"] = dr["short_notes_new"];
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
        dv.RowFilter = "section_level =" + colId;
        dv.Sort = "last_update_date asc";
        grd.DataSource = dv;
        grd.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price" };
        grd.DataBind();


    }

    protected void grdGroupingDirect_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int colId = Convert.ToInt32(grdGroupingDirect.DataKeys[e.Row.RowIndex].Values[0]);
            int nDirectId = 2;
            GridView gv = e.Row.FindControl("grdSelectedItem2") as GridView;

             if (Session["sItem_listDirect"] != null)
            {
                DataTable dtItemDirect = (DataTable)Session["sItem_listDirect"];
                DataView dv = dtItemDirect.DefaultView;
                dv.RowFilter = "section_level =" + colId;
                dv.Sort = "last_update_date DESC";
                gv.DataSource = dv;
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price" };
                gv.DataBind();
            }
            else if (Session["Item_list_Direct"] != null)
            {
                DataTable dtItemDirect = (DataTable)Session["Item_list_Direct"];
                DataView dv = dtItemDirect.DefaultView;
                dv.RowFilter = "section_level =" + colId;
                dv.Sort = "last_update_date DESC";
                gv.DataSource = dv;
                gv.DataKeyNames = new string[] { "co_pricing_list_id", "item_status_id", "total_retail_price", "total_direct_price" };
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
                Label lblHeader2 = headerRow.FindControl("lblHeader2") as Label;

                lblHeader2.Text = "Location";
            }

        }
    }

    protected void grdSelectedItem2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv2 = (GridView)sender;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string str = e.Row.Cells[0].Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 25)
            {
                e.Row.Cells[0].ToolTip = str;
                e.Row.Cells[0].Text = str.Substring(0, 25) + "...";
            }
            else
            {
                e.Row.Cells[0].ToolTip = str;
                e.Row.Cells[0].Text = str;

            }
            int nItemStatusId = Convert.ToInt32(gv2.DataKeys[e.Row.RowIndex].Values[1]);
            int ncoPricingUd = Convert.ToInt32(gv2.DataKeys[e.Row.RowIndex].Values[0]);
            Label lblDleted2 = (Label)e.Row.FindControl("lblDlete2");

            if (nItemStatusId == 2) //BABU
            {
                e.Row.Cells[0].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[1].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[2].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[3].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[4].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[5].Attributes.CssStyle.Add("text-decoration", "none; color: red ;");
                e.Row.Cells[5].Text = "Item Deleted" + e.Row.Cells[5].Text;


            }
            else if (nItemStatusId == 3)
            {
                e.Row.Attributes.CssStyle.Add("color", "green");
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

                
                    lblDleted2.Visible = true;
                    lblDleted2.ForeColor = Color.Red;
                }
                
            }
           



        }
    }

    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        txtSearchItemName.Text = "";
        Session.Remove("sItem_list");
        Session.Remove("sItem_list_Direct");
        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();
    }
    public void BindSelectedSecORLoc()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        try
        {
            string strQ1 = string.Empty;
           string strQ2 = string.Empty;
        if (Session["oCrew"] != null)
        {
             strQ1 = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                      " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " and section_id IN (SELECT section_id FROM crew_section WHERE crew_Id = " + Convert.ToInt32(hdnCrewId.Value) + ") ) " +
                      " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
        }

        if (Session["oUser"] != null)
        {
            strQ1 = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                     " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) +" ) " + 
                     " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
        }
               

                DataTable dtsec = csCommonUtility.GetDataTable(strQ1);
                if ((dtsec == null) || (dtsec.Rows.Count == 0))
                {
                    dtsec = LoadMasterTable();
                }

                if (Session["oCrew"] != null)
                {
                    strQ2 = "select DISTINCT cop.section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list cop " +
                              " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                               " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct=1 AND cop.client_id =1 and cop.section_level IN (SELECT section_id FROM crew_section WHERE crew_Id = " + Convert.ToInt32(hdnCrewId.Value) + ")  " +
                               " order by section_level asc";
                }
                if (Session["oUser"] != null)
                {
                    strQ2 = "select DISTINCT cop.section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list cop " +
                              " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                               " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct=1 AND cop.client_id =1 " +
                               " order by section_level asc";
                }
                
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
                Session.Add("MainDTSec", dtsec);
           
        }
        catch (Exception ex)
        {
            string ext = ex.StackTrace;
            throw ex;
        }


    }
    public void BindSelectedSecORLoc_Direct()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

           string strQ1 = string.Empty;
           string strQ2 = string.Empty;
           if (Session["oCrew"] != null)
           {
               strQ1 = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value) + " and section_id IN (SELECT section_id FROM crew_section WHERE crew_Id = " + Convert.ToInt32(hdnCrewId.Value) + ") ) " +
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = 2 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
           }
           if (Session["oUser"] != null)
           {
               strQ1 = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from pricing_details where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(hdnClientId.Value) + " ) " +
                   " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(hdnClientId.Value)  +" ) " + 
                   " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = 2 AND pricing_details.client_id =" + Convert.ToInt32(hdnClientId.Value) + "  order by section_level asc";
           }
       

        DataTable dtsec = csCommonUtility.GetDataTable(strQ1);
        if ((dtsec == null) || (dtsec.Rows.Count == 0))
        {
            dtsec = LoadMasterTable();
        }
        if (Session["oCrew"] != null)
        {
            strQ2 = "select DISTINCT cop.section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list cop " +
                           " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                            " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct = 2 AND cop.client_id = "+ hdnClientId.Value + " and cop.section_level IN (SELECT section_id FROM crew_section WHERE crew_Id = " + Convert.ToInt32(hdnCrewId.Value) + ")   " +
                            " order by section_level asc";
        }
        if (Session["oUser"] != null)
        {
            strQ2 = "select DISTINCT cop.section_level AS colId,'SECTION: '+ section_name as colName from change_order_pricing_list cop " +
                           " INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id " +
                            " where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  and ce.change_order_status_id = 3 AND cop.is_direct = 2 AND cop.client_id =  " + hdnClientId.Value +
                            " order by section_level asc";
        }
        DataTable dtsec2 = csCommonUtility.GetDataTable(strQ2);
        DataRow drNew = null;
        if (dtsec2.Rows.Count > 0)
        {
            foreach (DataRow dr in dtsec2.Rows)
            {
                int ncolId = Convert.ToInt32(dr["colId"]);
                string strColName = dr["colName"].ToString();
                bool Iexists = dtsec.AsEnumerable().Where(c => c.Field<Int32>("colId").Equals(ncolId)).Count() > 0;
                if (!Iexists)  //if (!ContainDataRowInDataTable(dtsec, dr))
                {
                    drNew = dtsec.NewRow();
                    drNew["colId"] = ncolId;
                    drNew["colName"] = strColName;
                    dtsec.Rows.Add(drNew);
                }
            }
        }
        Session.Add("MainDTSecDirect", dtsec);
       
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        if (txtSearchItemName.Text.Trim() != "")
        {
            BindSelectedSecORLoc();
            BindSelectedSecORLoc_Direct();
            Session.Remove("sItem_list");
            Session.Remove("sItem_list_Direct");
            DataTable dtMainDTSec = new DataTable();
            DataTable dtMainDTSecDirect = new DataTable();

            DataTable dtItemdata = new DataTable();
            DataTable dtItemDirect = new DataTable();
            if (Session["Item_list"] != null)
            {
                dtItemdata = (DataTable)Session["Item_list"];
            }
            if (Session["Item_list_Direct"] != null)
            {
                dtItemDirect = (DataTable)Session["Item_list_Direct"];
            }
            if (Session["MainDTSec"] != null)
            {
                dtMainDTSec = (DataTable)Session["MainDTSec"];
            }
           
            if (Session["MainDTSecDirect"] != null)
            {
                dtMainDTSecDirect = (DataTable)Session["MainDTSecDirect"];
            }
           

            DataTable dtItemNew = new DataTable();
            DataTable dtItemNewDirect = new DataTable();

            string prefixText = txtSearchItemName.Text.Trim();
            if (prefixText.IndexOf(">>") != -1)
            {
                var corrected = prefixText.Substring(0, prefixText.Length - 2);
                if (prefixText.IndexOf(">>") != -1)
                {
                    prefixText = corrected.Substring(corrected.LastIndexOf(">>") + 2);
                }
                else
                {
                    prefixText = corrected;

                }
            }
            int nCustID = Convert.ToInt32(hdnCustomerId.Value);
            int nEstID = Convert.ToInt32(hdnEstimateId.Value);
            if (prefixText.IndexOf(",") > 0)
            {
                string strCondition = string.Empty;
                string[] strIds = prefixText.Split(',');
                foreach (string strId in strIds)
                {
                    if (strCondition.Length == 0)
                        strCondition = " section_name LIKE '%" + strId.Trim().Replace("'", "''") + "%'" + " or item_name LIKE '%" + strId.Trim().Replace("'", "''") + "%'";
                    else
                        strCondition += "or section_name LIKE '%" + strId.Trim().Replace("'", "''") + "%'" + " or item_name LIKE '%" + strId.Trim().Replace("'", "''") + "%'";

                }
                if (dtItemdata.Rows.Count > 0)
                {
                    DataRow[] filteredRows = dtItemdata.Select(strCondition);
                    if (filteredRows.Length != 0)
                    {
                        dtItemNew = filteredRows.CopyToDataTable();
                        Session.Add("sItem_list", dtItemNew);

                    }
                }
                if (dtItemDirect.Rows.Count > 0)
                {
                    DataRow[] filteredRows = dtItemDirect.Select(strCondition);
                    if (filteredRows.Length != 0)
                    {
                        dtItemNewDirect = filteredRows.CopyToDataTable();
                        Session.Add("sItem_listDirect", dtItemNewDirect);

                    }
                }

            }
            else
            {

                string strquery = "section_name LIKE '%" + prefixText.Trim().Replace("'", "''") + "%'" + " or item_name LIKE '%" + prefixText.Trim().Replace("'", "''") + "%'";
                if (dtItemdata.Rows.Count > 0)
                {
                    DataRow[] filteredRows = dtItemdata.Select(strquery);
                    if (filteredRows.Length != 0)
                    {
                        dtItemNew = filteredRows.CopyToDataTable();
                        Session.Add("sItem_list", dtItemNew);

                    }
                }
                if (dtItemDirect.Rows.Count > 0)
                {
                    DataRow[] filteredRows = dtItemDirect.Select(strquery);
                    if (filteredRows.Length != 0)
                    {
                        dtItemNewDirect = filteredRows.CopyToDataTable();
                        Session.Add("sItem_listDirect", dtItemNewDirect);

                    }
                }

            }



            if (Session["sItem_list"] == null && Session["sItem_listDirect"] == null)
            {
                return;
            }
            if (dtItemNew.Rows.Count > 0)
            {
                DataTable dtTemp = dtMainDTSec.Copy();
                foreach (DataRow dr in dtTemp.Rows)
                {
                    int ncolId = Convert.ToInt32(dr["colId"]);
                    bool Iexists = dtItemNew.AsEnumerable().Where(c => c.Field<Int32>("section_level").Equals(ncolId)).Count() > 0;
                    if (!Iexists)
                    {
                        var rows = dtMainDTSec.Select("colId =" + ncolId + "");
                        foreach (var row in rows)
                        {
                            row.Delete();
                            dtMainDTSec.AcceptChanges();
                        }
                    }
                }
            }
            if (dtItemNewDirect.Rows.Count > 0)
            {
                DataTable dtTemp = dtMainDTSecDirect.Copy();
                foreach (DataRow dr in dtTemp.Rows)
                {
                    int ncolId = Convert.ToInt32(dr["colId"]);
                    bool Iexists = dtItemNewDirect.AsEnumerable().Where(c => c.Field<Int32>("section_level").Equals(ncolId)).Count() > 0;
                    if (!Iexists)
                    {
                        var rows = dtMainDTSecDirect.Select("colId =" + ncolId + "");
                        foreach (var row in rows)
                        {
                            row.Delete();
                            dtMainDTSecDirect.AcceptChanges();
                        }
                    }
                }
            }

            if (dtMainDTSec.Rows.Count > 0)
            {
                Session.Add("sMainDTSec", dtMainDTSec);
                grdGrouping.DataSource = dtMainDTSec;
                grdGrouping.DataKeyNames = new string[] { "colId" };
                grdGrouping.DataBind();
            }
            if (dtMainDTSecDirect.Rows.Count > 0)
            {
                Session.Add("sMainDTSecDirect", dtMainDTSecDirect);
                grdGroupingDirect.DataSource = dtMainDTSecDirect;
                grdGroupingDirect.DataKeyNames = new string[] { "colId" };
                grdGroupingDirect.DataBind();
            }
        }

    }

    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("mlandingpage.aspx");
    }
    protected void rdbEstimate_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdbEstimate.ID, rdbEstimate.GetType().Name, "SelectedIndexChanged"); 
         hdnEstimateId.Value = rdbEstimate.SelectedValue;
         Session.Add("cEstId", hdnEstimateId.Value);
        Session.Remove("Item_list");
        Session.Remove("Item_list_Direct");
        Session.Remove("sItem_list");
        Session.Remove("sItem_list_Direct");

        GetCustomerName(Convert.ToInt32(hdnCustomerId.Value));
        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();

    }
}