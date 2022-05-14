using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ChangeOrderListAll : System.Web.UI.Page
{


    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["clSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["clSearch"];
            return (from c in cList
                    where c.last_name1.ToLower().StartsWith(prefixText.ToLower())
                    select c.last_name1).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.isCustomer == 1 && c.is_active == true && c.last_name1.StartsWith(prefixText)
                    select c.last_name1).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetFirstName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["clSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["clSearch"];
            return (from c in cList
                    where c.first_name1.ToLower().StartsWith(prefixText.ToLower())
                    select c.first_name1).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.isCustomer == 1 && c.is_active == true && c.first_name1.StartsWith(prefixText)
                    select c.first_name1).Take<String>(count).ToArray();
        }
    }


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
            if (Page.User.IsInRole("GC01") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            // Get Leads
            # region Get Customer

            DataClassesDataContext _db = new DataClassesDataContext();
            List<customer> CustomerList = _db.customers.Where(c => c.isCustomer == 1 && c.is_active == true).ToList();
            Session.Add("clSearch", CustomerList);

            # endregion
            BindSalesPerson();


            int nPage = 0;
            int nLeadSourceId = 0;
            if (Session["sPage"] != null)
            {
                nPage = Convert.ToInt32(Session["sPage"]);
            }
            if (Session["sSalePerson"] != null)
            {
                nLeadSourceId = Convert.ToInt32(Session["sSalePerson"]);
            }

            if (nLeadSourceId > 0)
                ddlSalesRep.SelectedValue = nLeadSourceId.ToString();
            else
                ddlSalesRep.SelectedIndex = -1;

            txtSearch.Text = "";


            txtStartDate.Text = DateTime.Now.AddDays(-730).ToShortDateString();
            txtEndDate.Text = DateTime.Now.ToShortDateString();
            GetCustomerCOList(nPage);



        }
    }

    private void BindSalesPerson()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select first_name+' '+last_name AS sales_person_name,sales_person_id from sales_person WHERE is_active=1  and is_sales=1 and sales_person.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " order by sales_person_id asc";
        List<userinfo> mList = _db.ExecuteQuery<userinfo>(strQ, string.Empty).ToList();
        ddlSalesRep.DataSource = mList;
        ddlSalesRep.DataTextField = "sales_person_name";
        ddlSalesRep.DataValueField = "sales_person_id";
        ddlSalesRep.DataBind();
        ddlSalesRep.Items.Insert(0, "All");

    }


    protected void GetCustomerCOList(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        grdCustCOList.PageIndex = nPageNo;
        string strCondition = "";

        if (txtStartDate.Text != "" && txtEndDate.Text != "")
        {
            DateTime strStartDate = Convert.ToDateTime(txtStartDate.Text.Trim());
            DateTime strEndDate = Convert.ToDateTime(txtEndDate.Text.Trim());
            strCondition += " CONVERT(DATETIME,T1.changeorder_date)>='" + strStartDate + "' AND  CONVERT(DATETIME,T1.changeorder_date) <'" + strEndDate.AddDays(1).ToString() + "' ";
        }

        if (txtSearch.Text.Trim() != "")
        {
            Session.Remove("sPage");
            Session.Remove("sSalePerson");
            Session.Remove("sCOType");

            string str = txtSearch.Text.Trim();
            if (ddlSearchBy.SelectedValue == "1")
            {
                strCondition = " customers.first_name1 LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "2")
            {
                strCondition = "  customers.last_name1 LIKE '%" + str + "%'";
            }

        }



        if (ddlSalesRep.SelectedItem.Text != "All")
        {
            if (strCondition.Length > 0)
            {
                strCondition += " AND customers.sales_person_id =" + Convert.ToInt32(ddlSalesRep.SelectedValue);
            }
            else
            {
                strCondition += " customers.sales_person_id =" + Convert.ToInt32(ddlSalesRep.SelectedValue);
            }

        }

        if (strCondition.Length > 0)
        {

            strCondition = "Where  " + strCondition;
        }

        string strQ = " SELECT   CONVERT(DATETIME,T1.changeorder_date) as changeorder_date,customers.last_name1+', '+customers.first_name1 as [CustomerName],customer_estimate.estimate_name AS [EstimateName], " +
                        " T1.changeorder_name ,sales_person.first_name+' '+sales_person.last_name AS [SalesPerson], customers.sales_person_id , " +
                        " T1.chage_order_id,T1.COAmount,ISNULL(T1.is_cutomer_viewable,2) AS is_cutomer_viewable,COType,customers.customer_id,customer_estimate.estimate_id,T1.change_order_status_id FROM customers  " +
                        " INNER JOIN customer_estimate ON customer_estimate.customer_id = customers.customer_id  " +
                        " INNER JOIN sales_person ON  sales_person.sales_person_id = customers.sales_person_id  " +
                        " INNER JOIN  (SELECT ISNULL(SUM(EconomicsCost) +SUM(ISNULL(EconomicsCost,0)) * (ISNULL(changeorder_estimate.tax,0) / 100),0) AS COAmount, " +
                        " changeorder_estimate.customer_id,changeorder_estimate.estimate_id,changeorder_estimate.chage_order_id,changeorder_estimate.changeorder_name, " +
                        " changeorder_estimate.changeorder_date,changeorder_estimate.is_cutomer_viewable,CASE change_order_type_id WHEN 1 THEN 'Change Order' WHEN 2 THEN 'Clarification'  ELSE 'Internal Use Only' END AS [COType],change_order_status_id  FROM changeorder_estimate  " +
                        " INNER JOIN change_order_pricing_list ON changeorder_estimate.customer_id = change_order_pricing_list.customer_id " +
                        " AND  changeorder_estimate.estimate_id=change_order_pricing_list.estimate_id  AND changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id  " +
                        " and changeorder_estimate.change_order_status_id NOT IN (3,4)  WHERE changeorder_estimate.is_close = 0  " +
                        " GROUP BY changeorder_estimate.customer_id,changeorder_estimate.estimate_id,changeorder_estimate.chage_order_id," +
                        " changeorder_estimate.customer_id,changeorder_estimate.tax,changeorder_estimate.changeorder_name,changeorder_estimate.changeorder_date,changeorder_estimate.is_cutomer_viewable,change_order_type_id,change_order_status_id) AS T1 ON customers.customer_id = T1.customer_id  AND customer_estimate.estimate_id = T1.estimate_id " + strCondition + " ORDER BY CONVERT(DATETIME,T1.changeorder_date) DESC";


        DataTable dt = csCommonUtility.GetDataTable(strQ);
        Session.Add("sCustCOList", dt);

        grdCustCOList.DataSource = dt;
        grdCustCOList.DataKeyNames = new string[] { "customer_id", "estimate_id", "chage_order_id", "sales_person_id", "is_cutomer_viewable", "change_order_status_id", "COAmount" };
        grdCustCOList.DataBind();
        lblCurrentPageNo.Text = Convert.ToString(nPageNo + 1);
        if (nPageNo == 0)
        {
            btnPrevious.Enabled = false;
            btnPrevious0.Enabled = false;
        }
        else
        {
            btnPrevious.Enabled = true;
            btnPrevious0.Enabled = true;
        }

        if (grdCustCOList.PageCount == nPageNo + 1)
        {
            btnNext.Enabled = false;
            btnNext0.Enabled = false;
        }
        else
        {
            btnNext.Enabled = true;
            btnNext0.Enabled = true;
        }
    }



    protected void grdCustCOList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nCustomer_id = Convert.ToInt32(grdCustCOList.DataKeys[e.Row.RowIndex].Values[0]);
            int nEstimate_id = Convert.ToInt32(grdCustCOList.DataKeys[e.Row.RowIndex].Values[1]);
            int nChangeOrderId = Convert.ToInt32(grdCustCOList.DataKeys[e.Row.RowIndex].Values[2]);
            int nSales_person_id = Convert.ToInt32(grdCustCOList.DataKeys[e.Row.RowIndex].Values[3]);
            int nis_cutomer_viewable = Convert.ToInt32(grdCustCOList.DataKeys[e.Row.RowIndex].Values[4]);
            int nchange_order_status_id = Convert.ToInt32(grdCustCOList.DataKeys[e.Row.RowIndex].Values[5]);
            decimal dAmount = Convert.ToInt32(grdCustCOList.DataKeys[e.Row.RowIndex].Values[6]);

            HyperLink hypEstCODetail = (HyperLink)e.Row.FindControl("hypEstCODetail");
           // hypEstCODetail.NavigateUrl = "change_order_locations.aspx?coestid=" + nChangeOrderId + "&eid=" + nEstimate_id + "&cid=" + nCustomer_id;
            hypEstCODetail.NavigateUrl = "change_order_worksheet.aspx?gType=1&coestid=" + nChangeOrderId + "&eid=" + nEstimate_id + "&cid=" + nCustomer_id;

            Label lblCOstatus = (Label)e.Row.FindControl("lblCOstatus");
            Label lblcutomer_viewable = (Label)e.Row.FindControl("lblcutomer_viewable");
             Label lblCOAmount = (Label)e.Row.FindControl("lblCOAmount");

            

            if (nis_cutomer_viewable == 1)
                lblcutomer_viewable.Text = "Yes";
            else
                lblcutomer_viewable.Text = "No";

            if (nchange_order_status_id == 1)
                lblCOstatus.Text = "Draft";
            else if (nchange_order_status_id == 2)
                lblCOstatus.Text = "Pending";

            if (dAmount < 0)
            {
                lblCOAmount.ForeColor = System.Drawing.Color.Red;
            }

            
        }
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        GetCustomerCOList(0);
    }

    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {

       
        GetCustomerCOList(0);
    }

    protected void lnkViewAll_Click(object sender, EventArgs e)
    {

        Session.Remove("sPage");
        Session.Remove("sSalePerson");
        txtStartDate.Text = DateTime.Now.AddDays(-730).ToShortDateString();
        txtEndDate.Text = DateTime.Now.ToShortDateString();
        txtSearch.Text = "";
        ddlSalesRep.SelectedIndex = -1;
        GetCustomerCOList(0);

    }
    protected void btnView_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnView.ID, btnView.GetType().Name, "Click"); 

        lblResult.Text = "";
        DateTime strStartDate = DateTime.Now;
        DateTime strEndDate = DateTime.Now;
        if (txtStartDate.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Start Date is a required field");

            return;
        }
        else
        {
            try
            {
                Convert.ToDateTime(txtStartDate.Text);
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Start Date");

                return;
            }
            strStartDate = Convert.ToDateTime(txtStartDate.Text);
        }

        if (txtEndDate.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("End Date is a required field");

            return;
        }
        else
        {
            try
            {
                Convert.ToDateTime(txtEndDate.Text);
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid End Date");

                return;
            }
            strEndDate = Convert.ToDateTime(txtEndDate.Text);
        }
        if (strStartDate > strEndDate)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Date Range");

            return;
        }

        GetCustomerCOList(0);
    }

    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "Click"); 
        txtSearch.Text = "";
        wtmFileNumber.WatermarkText = "Search by " + ddlSearchBy.SelectedItem.Text;
        if (ddlSearchBy.SelectedValue == "2")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetLastName";
        }
        else if (ddlSearchBy.SelectedValue == "1")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetFirstName";
        }


        GetCustomerCOList(0);
    }


    protected void grdCustCOList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCustCOList.ID, grdCustCOList.GetType().Name, "Click"); 
        Session.Add("sPage", e.NewPageIndex.ToString());
        GetCustomerCOList(e.NewPageIndex);
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNext.ID, btnNext.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        Session.Add("sPage", nCurrentPage.ToString());
        GetCustomerCOList(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        Session.Add("sPage", (nCurrentPage - 2).ToString());
        GetCustomerCOList(nCurrentPage - 2);
    }


    protected void ddlSalesRep_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSalesRep.ID, ddlSalesRep.GetType().Name, "Click"); 
        if (ddlSalesRep.Text != "All")
            Session.Add("sSalePerson", ddlSalesRep.SelectedValue);

        GetCustomerCOList(0);
    }

    protected void grdCustCOList_Sorting(object sender, GridViewSortEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCustCOList.ID, grdCustCOList.GetType().Name, "Click"); 
        int nPageNo = 0;
        DataTable dtCallList = (DataTable)Session["sCustCOList"];
        if (hdnOrder.Value == "ASC")
            hdnOrder.Value = "DESC";
        else
            hdnOrder.Value = "ASC";

        string strShort = e.SortExpression + " " + hdnOrder.Value;




        DataView dv = dtCallList.DefaultView;
        dv.Sort = strShort;
        Session["sCustCOList"] = dv.ToTable();

        dtCallList = (DataTable)Session["sCustCOList"];
        grdCustCOList.DataSource = dtCallList;

        grdCustCOList.DataKeyNames = new string[] { "customer_id", "estimate_id", "chage_order_id", "sales_person_id", "is_cutomer_viewable", "change_order_status_id", "COAmount" };
        grdCustCOList.DataBind();
        lblCurrentPageNo.Text = Convert.ToString(nPageNo + 1);
        if (nPageNo == 0)
        {
            btnPrevious.Enabled = false;
            btnPrevious0.Enabled = false;
        }
        else
        {
            btnPrevious.Enabled = true;
            btnPrevious0.Enabled = true;
        }

        if (grdCustCOList.PageCount == nPageNo + 1)
        {
            btnNext.Enabled = false;
            btnNext0.Enabled = false;
        }
        else
        {
            btnNext.Enabled = true;
            btnNext0.Enabled = true;
        }

    }

}