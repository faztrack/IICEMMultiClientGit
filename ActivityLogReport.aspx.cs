using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ActivityLogReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
        if (!IsPostBack)
        {
            string divisionName = "";
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                userinfo oUser = (userinfo)Session["oUser"];
                hdnDivisionName.Value = oUser.divisionName;
                hdnPrimaryDivision.Value = oUser.primaryDivision.ToString();
                divisionName = oUser.divisionName;
            }
            
            if (Page.User.IsInRole("rpt008") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            BindDivision();
            BindSalesPersons();


            if (divisionName != "" && divisionName.Contains(","))
            {
                pnlDivision.Visible = true;
            }
            else
            {
                pnlDivision.Visible = false;
            }

            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnViewReport", "ddlSalesPersons" });
        }
    }

    private void BindDivision()
    {
        try
        {
            string sql = "select id, division_name from division order by division_name";
            DataTable dt = csCommonUtility.GetDataTable(sql);
            ddlDivision.DataSource = dt;
            ddlDivision.DataTextField = "division_name";
            ddlDivision.DataValueField = "id";
            ddlDivision.DataBind();
            ddlDivision.SelectedValue = hdnPrimaryDivision.Value;

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.ToString());
        }
    }

    private void BindSalesPersons()
    {

        string strQ = "select first_name+' '+last_name AS sales_person_name,sales_person_id from sales_person WHERE is_active=1 and is_sales=1 " + csCommonUtility.GetSalesPersonSql(hdnDivisionName.Value) + " order by sales_person_id asc";

        DataTable mList = csCommonUtility.GetDataTable(strQ);
        ddlSalesPersons.DataSource = mList;
        ddlSalesPersons.DataTextField = "sales_person_name";
        ddlSalesPersons.DataValueField = "sales_person_id";
        ddlSalesPersons.DataBind();
        ddlSalesPersons.Items.Insert(0, "All");
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }
    protected void btnViewReport_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnViewReport.ID, btnViewReport.GetType().Name, "Click"); 
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
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Start Date");
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
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid End Date");
                return;
            }
            strEndDate = Convert.ToDateTime(txtEndDate.Text).AddHours(23).AddMinutes(59);
        }
        if (strStartDate > strEndDate)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid Date Range");
            return;
        }
        string strCondition = string.Empty;
        if (ddlSalesPersons.SelectedItem.Text == "All")
        {
            strCondition = "  WHERE CONVERT(DATETIME,ccl.CallDate) BETWEEN '" + strStartDate + "' AND '" + strEndDate + "'";
        }
        else
        {
            strCondition = "  WHERE ccl.sales_person_id =" + Convert.ToInt32(ddlSalesPersons.SelectedValue) + " AND CONVERT(DATETIME,ccl.CallDate) BETWEEN '" + strStartDate + "' AND '" + strEndDate + "'";
        }

        if(strCondition.Length > 2)
        {
            strCondition += " AND ccl.client_id = " + Convert.ToInt32(ddlDivision.SelectedValue) + " ";
        }
        else
        {
            strCondition += " WHERE ccl.client_id = " + Convert.ToInt32(ddlDivision.SelectedValue) + " ";
        }


        DataClassesDataContext _db = new DataClassesDataContext();

        string strQ = " SELECT DISTINCT CONVERT(DATETIME,ccl.CallDate) as CallDate, t1.NOOfCall, Isnull(t2.NOOfPitched,0) as NOOfPitched,Isnull(t3.NOOfBooked,0) as NOOfBooked " +
                    " FROM CustomerCallLog ccl " +
                    " LEFT OUTER JOIN (select count(CallLogID) as NOOfCall, CONVERT(DATETIME,CallDate) as CallDate  from CustomerCallLog group by CONVERT(DATETIME,CallDate)) AS t1 on CONVERT(DATETIME, t1.CallDate) = CONVERT(DATETIME, ccl.CallDate) " +
                    " LEFT OUTER JOIN (select count(CallTypeId) as NOOfPitched , CONVERT(DATETIME,CallDate) as CallDate  from CustomerCallLog where CallTypeId = 2  group by CONVERT(DATETIME,CallDate)) AS t2 on CONVERT(DATETIME, t2.CallDate) = CONVERT(DATETIME, ccl.CallDate) " +
                    " LEFT OUTER JOIN (select count(CallTypeId) as NOOfBooked , CONVERT(DATETIME,CallDate) as CallDate  from CustomerCallLog where CallTypeId = 3  group by CONVERT(DATETIME,CallDate)) AS t3 on CONVERT(DATETIME, t3.CallDate) = CONVERT(DATETIME, ccl.CallDate) " + strCondition;

        DataTable dtReport = csCommonUtility.GetDataTable(strQ);

        if (dtReport.Rows.Count == 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("No data exist.");
            return;
        }

        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptActivitylogReport.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(dtReport);

        string strDate = "Date Range: " + txtStartDate.Text + " To " + txtEndDate.Text + "";

        Hashtable ht = new Hashtable();
        ht.Add("p_date", strDate);

        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);

    }

   
}