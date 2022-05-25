using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class projectcyclereport : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["clSearch"] != null)
        {
            List<csCustomer> cList = (List<csCustomer>)HttpContext.Current.Session["clSearch"];
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
            List<csCustomer> cList = (List<csCustomer>)HttpContext.Current.Session["clSearch"];
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


    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Add("loadstarttime", DateTime.Now);
        if (!IsPostBack)
        {
            string nClientId = "";
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                nClientId = ((userinfo)Session["oUser"]).client_id.ToString();
            }
            if (Page.User.IsInRole("pcr01") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            // Get Leads
            # region Get Customer

            DataClassesDataContext _db = new DataClassesDataContext();
            // List<customer> CustomerList = _db.customers.Where(c => c.isCustomer == 1 && c.is_active == true).ToList();
            List<csCustomer> CustomerList = (from c in _db.customers
                                             join s in _db.ScheduleCalendars on c.customer_id equals s.customer_id
                                             where c.isCustomer == 1 && c.is_active == true && s.IsEstimateActive == true && c.client_id.ToString().Contains(nClientId)
                                             select new csCustomer
                                             {
                                                 first_name1 = c.first_name1,
                                                 last_name1 = c.last_name1,

                                             }).Distinct().ToList();
            Session.Add("clSearch", CustomerList);

            # endregion
            int nPage = 0;
            txtSearch.Text = "";
            GetCustomerProjectList();

        }
    }

    private DataTable GetData()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        string strCondition = "";

        if (txtStartDate.Text != "" && txtEndDate.Text != "")
        {
            DateTime strStartDate = Convert.ToDateTime(txtStartDate.Text.Trim());
            DateTime strEndDate = Convert.ToDateTime(txtEndDate.Text.Trim());
            strCondition += " and  CONVERT(DATETIME,ce.sale_date)>='" + strStartDate + "' AND  CONVERT(DATETIME,ce.sale_date) <'" + strEndDate.AddDays(1).ToString() + "' ";
        }

        if (txtSearch.Text.Trim() != "")
        {


            string str = txtSearch.Text.Trim();
            if (ddlSearchBy.SelectedValue == "1")
            {
                strCondition += " and customers.first_name1 LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "2")
            {
                strCondition += " and  customers.last_name1 LIKE '%" + str + "%'";
            }

        }
        if (ddlStatus.SelectedItem.Text != "All")
        {
            strCondition += " and customers.status_id=" + ddlStatus.SelectedValue;
        }

        if (ddlEstStatus.SelectedItem.Text != "All")
        {
            strCondition += " and ce.IsEstimateActive=" + ddlEstStatus.SelectedValue;
        }

        if (strCondition.Length > 0)
        {

            strCondition = " where  ce.status_id=3  and s.employee_name != ''  and s.employee_name != 'TBD TBD' and s.event_end<GETDATE()  " + strCondition;
        }
        else
        {
            strCondition = " where  ce.status_id=3  and s.employee_name != ''  and s.employee_name != 'TBD TBD' and s.event_end<GETDATE() ";
        }



        string strQ = " select customers.first_name1 + ' ' + customers.last_name1 as customername,s.customer_id,s.estimate_id,u.first_name + ' ' + u.last_name as suername,customers.SuperintendentId,sp.first_name + ' ' + sp.last_name as salesperson, MIN(event_start) as EventFisrtDay, getdate() as currentdate, MAX(event_end) as EventLastDay ," +
                    " DATEDIFF(DAY, MAX(event_end),getdate()) AS LastAcitivityDate, DATEDIFF(DAY, MIN(event_start),getdate()) AS StartActivityDate, ce.sale_date,ce.job_number " +
                    " from[ScheduleCalendar]  as s " +
                    " inner join customers on customers.customer_id = s.customer_id " +
                    " inner join customer_estimate as ce on ce.customer_id = s.customer_id and ce.estimate_id = s.estimate_id " +
                    " inner join user_info as u on u.user_id = customers.SuperintendentId " +
                    " inner join sales_person as sp on sp.sales_person_id = customers.sales_person_id " + strCondition +
                    " group by s.customer_id,s.estimate_id,customers.last_name1,customers.first_name1,ce.sale_date,ce.job_number,u.first_name,u.last_name,sp.first_name,sp.last_name,customers.SuperintendentId order by CONVERT(DATETIME, ce.sale_date)";


        DataTable dt = csCommonUtility.GetDataTable(strQ);
        return dt;
    }
    protected void GetCustomerProjectList()
    {

        DataTable dt = GetData();
        Session.Add("sProjectList", dt);
        BindProjectCycle(0);


    }

    private void BindProjectCycle(int nPageNo)
    {
        try
        {
            DataTable dt = (DataTable)Session["sProjectList"];
            grdProjectRecycle.DataSource = dt;
            grdProjectRecycle.PageIndex = nPageNo;
            grdProjectRecycle.DataKeyNames = new string[] { "customer_id", "estimate_id" };
            grdProjectRecycle.DataBind();
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

            if (grdProjectRecycle.PageCount == nPageNo + 1)
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
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void grdProjectRecycle_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nCustomer_id = Convert.ToInt32(grdProjectRecycle.DataKeys[e.Row.RowIndex].Values[0]);
            int nEstimate_id = Convert.ToInt32(grdProjectRecycle.DataKeys[e.Row.RowIndex].Values[1]);
        }
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Search Click");
        GetCustomerProjectList();
    }

    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {

        GetCustomerProjectList();
    }

    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, LinkButton2.ID, LinkButton2.GetType().Name, "View All Click");
        lblResult.Text = "";
        txtStartDate.Text = "";
        txtEndDate.Text = "";
        txtSearch.Text = "";
        Session.Remove("sPage");
        ddlEstStatus.SelectedValue = "1";
        ddlStatus.SelectedValue = "2";
        GetCustomerProjectList();

    }
    protected void btnView_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnView.ID, btnView.GetType().Name, "View Click");

        lblResult.Text = "";
        DateTime strStartDate = DateTime.Now;
        DateTime strEndDate = DateTime.Now;
        if (txtStartDate.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Sold Start Date is a required field");

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
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Sold Start Date");

                return;
            }
            strStartDate = Convert.ToDateTime(txtStartDate.Text);
        }

        if (txtEndDate.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Sold End Date is a required field");

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
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Sold End Date");

                return;
            }
            strEndDate = Convert.ToDateTime(txtEndDate.Text);
        }
        if (strStartDate > strEndDate)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Date Range");

            return;
        }

        GetCustomerProjectList();
    }

    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "Search By Changed");
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


        BindProjectCycle(0);
    }


    protected void grdProjectRecycle_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdProjectRecycle.ID, grdProjectRecycle.GetType().Name, "Project Recycle Changed");
        Session.Add("sPage", e.NewPageIndex.ToString());
        BindProjectCycle(e.NewPageIndex);
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        Session.Add("sPage", nCurrentPage.ToString());
        BindProjectCycle(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        Session.Add("sPage", (nCurrentPage - 2).ToString());
        BindProjectCycle(nCurrentPage - 2);
    }




    protected void grdProjectRecycle_Sorting(object sender, GridViewSortEventArgs e)
    {


        try
        {
            int nPage = 0;
            if (Session["sPage"] != null)
                nPage = Convert.ToInt32(Session["sPage"]);
            DataTable dtCallList = (DataTable)Session["sProjectList"];
            if (hdnOrder.Value == "ASC")
                hdnOrder.Value = "DESC";
            else
                hdnOrder.Value = "ASC";

            string strShort = e.SortExpression + " " + hdnOrder.Value;
            DataView dv = dtCallList.DefaultView;
            dv.Sort = strShort;
            Session["sProjectList"] = dv.ToTable();
            BindProjectCycle(nPage);

        }
        catch (Exception ex)
        {

            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }


    protected void ddlEstStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetCustomerProjectList();
    }

    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetCustomerProjectList();
    }

    protected void btnExpCustList_Click(object sender, ImageClickEventArgs e)
    {
        int Total = 0;
        DataTable dtMaster = GetData();
        Total = dtMaster.Rows.Count;
        string SourceFilePath = Server.MapPath(@"Reports") + @"\";
        string FilePath = Server.MapPath(@"Reports\excel_report") + @"\";
        string sFileName = "ProjectCycleReport" + DateTime.Now.Ticks.ToString() + ".xlsx";

        FileInfo tempFile = new FileInfo(SourceFilePath + "ProjectCycleReport.xlsx");

        tempFile.CopyTo(FilePath + sFileName);

        FileInfo newFile = new FileInfo(FilePath + sFileName);
        using (ExcelPackage package = new ExcelPackage(newFile))
        {
            ExcelWorksheet worksheet0 = package.Workbook.Worksheets["ProjectCycle"];

            int counter = 0;
            var sSuperintendentName = (from r in dtMaster.AsEnumerable()
                                       select new
                                       {
                                           suername = r.Field<string>("suername"),
                                           SuperintendentId = r.Field<int>("SuperintendentId")
                                       }).Distinct();
            int nRowCount = 2;
     

            foreach (var p in sSuperintendentName)
            {
                int nCount = 1;
                string nSuername = p.suername;
                int nSuperintendentId = p.SuperintendentId;
                DataView dv = dtMaster.DefaultView;
                dv.RowFilter = " SuperintendentId=" + nSuperintendentId;





            
                worksheet0.Cells["A" + nRowCount + ":D" + nRowCount].Merge = true;
                worksheet0.Cells["F" + nRowCount + ":J" + nRowCount].Merge = true;
                worksheet0.Cells[nRowCount, 5].Value = nSuername.ToString();
                worksheet0.Cells[nRowCount, 5].Style.Font.Bold = true;
                worksheet0.Cells[nRowCount, 5].Style.Font.Size = 16;
                worksheet0.Cells[nRowCount, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

              
                nRowCount++;
                foreach (DataRow dr2 in dv.ToTable().Rows)
                {

                    worksheet0.Cells[nRowCount, 1].Value = nCount++;
                    worksheet0.Cells[nRowCount, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet0.Cells[nRowCount, 2].Value = dr2["customername"].ToString();
                    worksheet0.Cells[nRowCount, 3].Value = nSuername.ToString();
                    worksheet0.Cells[nRowCount, 4].Value = dr2["salesperson"].ToString();
                    worksheet0.Cells[nRowCount, 5].Value = dr2["job_number"].ToString();
                    worksheet0.Cells[nRowCount, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet0.Cells[nRowCount, 6].Value = Convert.ToDateTime(dr2["sale_date"]).ToString("MM/dd/yyyy");
                    worksheet0.Cells[nRowCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet0.Cells[nRowCount, 7].Value =Convert.ToDateTime(dr2["EventFisrtDay"]).ToString("MM/dd/yyyy");
                    worksheet0.Cells[nRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet0.Cells[nRowCount, 8].Value = dr2["StartActivityDate"].ToString();
                    worksheet0.Cells[nRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet0.Cells[nRowCount, 9].Value = Convert.ToDateTime(dr2["EventLastDay"]).ToString("MM/dd/yyyy");
                    worksheet0.Cells[nRowCount, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet0.Cells[nRowCount, 10].Value = dr2["LastAcitivityDate"].ToString();
                    worksheet0.Cells[nRowCount, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    nRowCount++;

                }
             

            }
            worksheet0.Cells["A:L"].AutoFitColumns();
            package.Save();

            lblResult.Text = csCommonUtility.GetSystemMessage("Your report is being downloaded.  Please check your browser's download folder.");

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/excel_report/" + sFileName + "');", true);
        }
    }
}