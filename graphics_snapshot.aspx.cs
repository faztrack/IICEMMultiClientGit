using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class graphics_snapshot : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.last_name1.ToLower().StartsWith(prefixText.ToLower())
                    select c.last_name1).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.last_name1.StartsWith(prefixText)
                    select c.last_name1).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetFirstName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.first_name1.ToLower().StartsWith(prefixText.ToLower())
                    select c.first_name1).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.first_name1.StartsWith(prefixText)
                    select c.first_name1).Take<String>(count).ToArray();
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                hdnClientId.Value = ((userinfo)Session["oUser"]).client_id.ToString();
            }
            if (Page.User.IsInRole("calllog001") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            # region Get Leads

            DataClassesDataContext _db = new DataClassesDataContext();
            List<customer> LeadList = _db.customers.ToList();
            Session.Add("cSearch", LeadList);

            # endregion

            GetSnapShotDataList(0);
        }

    }
    private void GetSnapShotDataList(int nPageNo)
    {
        grdGraphicsSnap.PageIndex = nPageNo;
        try
        {

            string strQ = " SELECT  ROW_NUMBER() OVER (ORDER BY customers.customer_id) AS [serial_number], customers.customer_id,customers.first_name1+' '+customers.last_name1 AS CustomerName,customer_estimate.estimate_name,customer_estimate.estimate_id,ISNULL(jobstatusid,0) AS jobstatusid FROM customers " +
                          " INNER JOIN customer_estimate ON customers.customer_id  = customer_estimate.customer_id " +
                          " LEFT OUTER JOIN customer_jobstatus ON customer_estimate.customer_id  = customer_jobstatus.customerid AND customer_estimate.estimate_id  = customer_jobstatus.estimate_id " +
                          " WHERE customers.status_id NOT IN(4,5) AND customer_estimate.IsEstimateActive = 1 and (jobstatusid = 0 OR jobstatusid IS NULL)";

            DataTable dtDesign = csCommonUtility.GetDataTable(strQ);

            string strQ1 = " SELECT  ROW_NUMBER() OVER (ORDER BY customers.customer_id) AS [serial_number], customers.customer_id,customers.first_name1+' '+customers.last_name1 AS CustomerName,customer_estimate.estimate_name,customer_estimate.estimate_id,ISNULL(jobstatusid,0) AS jobstatusid FROM customers " +
                          " INNER JOIN customer_estimate ON customers.customer_id  = customer_estimate.customer_id " +
                          " LEFT OUTER JOIN customer_jobstatus ON customer_estimate.customer_id  = customer_jobstatus.customerid AND customer_estimate.estimate_id  = customer_jobstatus.estimate_id " +
                          " WHERE customers.status_id NOT IN(4,5) AND customer_estimate.IsEstimateActive = 1 and jobstatusid = 1";

            DataTable dtSelection = csCommonUtility.GetDataTable(strQ1);

            string strQ2 = " SELECT  ROW_NUMBER() OVER (ORDER BY customers.customer_id) AS [serial_number], customers.customer_id,customers.first_name1+' '+customers.last_name1 AS CustomerName,customer_estimate.estimate_name,customer_estimate.estimate_id,ISNULL(jobstatusid,0) AS jobstatusid FROM customers " +
                         " INNER JOIN customer_estimate ON customers.customer_id  = customer_estimate.customer_id " +
                         " LEFT OUTER JOIN customer_jobstatus ON customer_estimate.customer_id  = customer_jobstatus.customerid AND customer_estimate.estimate_id  = customer_jobstatus.estimate_id " +
                         " WHERE customers.status_id NOT IN(4,5) AND customer_estimate.IsEstimateActive = 1 and jobstatusid = 2";

            DataTable dtSiteProgress = csCommonUtility.GetDataTable(strQ2);

            string strQ3 = " SELECT  ROW_NUMBER() OVER (ORDER BY customers.customer_id) AS [serial_number], customers.customer_id,customers.first_name1+' '+customers.last_name1 AS CustomerName,customer_estimate.estimate_name,customer_estimate.estimate_id,ISNULL(jobstatusid,0) AS jobstatusid FROM customers " +
                        " INNER JOIN customer_estimate ON customers.customer_id  = customer_estimate.customer_id " +
                        " LEFT OUTER JOIN customer_jobstatus ON customer_estimate.customer_id  = customer_jobstatus.customerid AND customer_estimate.estimate_id  = customer_jobstatus.estimate_id " +
                        " WHERE customers.status_id NOT IN(4,5) AND customer_estimate.IsEstimateActive = 1 and jobstatusid = 3";

            DataTable dtSchedule = csCommonUtility.GetDataTable(strQ3);

            string strQ4 = " SELECT  ROW_NUMBER() OVER (ORDER BY customers.customer_id) AS [serial_number], customers.customer_id,customers.first_name1+' '+customers.last_name1 AS CustomerName,customer_estimate.estimate_name,customer_estimate.estimate_id,ISNULL(jobstatusid,0) AS jobstatusid FROM customers " +
                        " INNER JOIN customer_estimate ON customers.customer_id  = customer_estimate.customer_id " +
                        " LEFT OUTER JOIN customer_jobstatus ON customer_estimate.customer_id  = customer_jobstatus.customerid AND customer_estimate.estimate_id  = customer_jobstatus.estimate_id " +
                        " WHERE customers.status_id NOT IN(4,5) AND customer_estimate.IsEstimateActive = 1 and jobstatusid = 4";

            DataTable dtFinalProject = csCommonUtility.GetDataTable(strQ4);

            string strQ5 = " SELECT  ROW_NUMBER() OVER (ORDER BY customers.customer_id) AS [serial_number], customers.customer_id,customers.first_name1+' '+customers.last_name1 AS CustomerName,customer_estimate.estimate_name,customer_estimate.estimate_id,ISNULL(jobstatusid,0) AS jobstatusid FROM customers " +
                       " INNER JOIN customer_estimate ON customers.customer_id  = customer_estimate.customer_id " +
                       " LEFT OUTER JOIN customer_jobstatus ON customer_estimate.customer_id  = customer_jobstatus.customerid AND customer_estimate.estimate_id  = customer_jobstatus.estimate_id " +
                       " WHERE customers.status_id NOT IN(4,5) AND customer_estimate.IsEstimateActive = 1 and jobstatusid = 5";

            DataTable dtCompletion = csCommonUtility.GetDataTable(strQ5);

            string strQ6 = " SELECT  ROW_NUMBER() OVER (ORDER BY customers.customer_id) AS [serial_number], customers.customer_id,customers.first_name1+' '+customers.last_name1 AS CustomerName,customer_estimate.estimate_name,customer_estimate.estimate_id,ISNULL(jobstatusid,0) AS jobstatusid FROM customers " +
                      " INNER JOIN customer_estimate ON customers.customer_id  = customer_estimate.customer_id " +
                      " LEFT OUTER JOIN customer_jobstatus ON customer_estimate.customer_id  = customer_jobstatus.customerid AND customer_estimate.estimate_id  = customer_jobstatus.estimate_id " +
                      " WHERE customers.status_id NOT IN(4,5) AND customer_estimate.IsEstimateActive = 1 and jobstatusid = 6";

            DataTable dtWarranty = csCommonUtility.GetDataTable(strQ6);

            string strQ7 = " SELECT  ROW_NUMBER() OVER (ORDER BY customers.customer_id) AS [serial_number], customers.customer_id,customers.first_name1+' '+customers.last_name1 AS CustomerName,customer_estimate.estimate_name,customer_estimate.estimate_id,ISNULL(jobstatusid,0) AS jobstatusid FROM customers " +
                    " INNER JOIN customer_estimate ON customers.customer_id  = customer_estimate.customer_id " +
                    " LEFT OUTER JOIN customer_jobstatus ON customer_estimate.customer_id  = customer_jobstatus.customerid AND customer_estimate.estimate_id  = customer_jobstatus.estimate_id " +
                    " WHERE customers.status_id NOT IN(4,5) AND customer_estimate.IsEstimateActive = 1 and jobstatusid = 7";

            DataTable dtCompleted = csCommonUtility.GetDataTable(strQ7);

            DataTable dtMaster = LoadMasterTable();
            int nCount = 0;

            if (dtDesign.Rows.Count > 0)
            {
                nCount = dtDesign.Rows.Count;

            }
            if (dtSelection.Rows.Count > 0)
            {
                if (nCount <= dtSelection.Rows.Count)
                {
                    nCount = dtSelection.Rows.Count;
                }

            }
            if (dtSiteProgress.Rows.Count > 0)
            {
                if (nCount <= dtSiteProgress.Rows.Count)
                {
                    nCount = dtSiteProgress.Rows.Count;
                }

            }
            if (dtSchedule.Rows.Count > 0)
            {
                if (nCount <= dtSchedule.Rows.Count)
                {
                    nCount = dtSchedule.Rows.Count;
                }

            }
            if (dtFinalProject.Rows.Count > 0)
            {
                if (nCount <= dtFinalProject.Rows.Count)
                {
                    nCount = dtFinalProject.Rows.Count;
                }

            }
            if (dtCompletion.Rows.Count > 0)
            {
                if (nCount <= dtCompletion.Rows.Count)
                {
                    nCount = dtCompletion.Rows.Count;
                }

            }
            if (dtWarranty.Rows.Count > 0)
            {
                if (nCount <= dtWarranty.Rows.Count)
                {
                    nCount = dtWarranty.Rows.Count;
                }

            }
            if (dtCompleted.Rows.Count > 0)
            {
                if (nCount <= dtCompleted.Rows.Count)
                {
                    nCount = dtCompleted.Rows.Count;
                }

            }

            for (int i = 1; i <= nCount; i++)
            {
                DataRow drNew = dtMaster.NewRow();
                //design
                DataView dv = dtDesign.DefaultView;
                if (dtDesign.Rows.Count > 0)
                {
                    dv.RowFilter = "serial_number = " + i;

                    if (dv.Count > 0)
                    {
                        string strCustName = dv[0]["CustomerName"].ToString();
                        string strEstName = dv[0]["estimate_name"].ToString();
                        int nCustID = Convert.ToInt32(dv[0]["customer_id"]);
                        int nEstID = Convert.ToInt32(dv[0]["estimate_id"]);
                        decimal TotalExCom_WithIntevcise = GetRetailTotal(nEstID, nCustID);
                        string strColumn = strEstName + "<br/>" + TotalExCom_WithIntevcise.ToString("c");
                        drNew["Design"] = strCustName;
                        drNew["DesignEst"] = strColumn;
                        drNew["DesignID"] = nCustID.ToString() + "," + nEstID.ToString();
                    }
                    else
                    {
                        drNew["Design"] = "";
                        drNew["DesignEst"] = "";
                        drNew["DesignID"] = "0" + "," + "0";
                    }
                }
                else
                {
                    drNew["Design"] = "";
                    drNew["DesignEst"] = "";
                    drNew["DesignID"] = "0" + "," + "0";
                }

                //Selection
                if (dtSelection.Rows.Count > 0)
                {
                    dv = dtSelection.DefaultView;
                    dv.RowFilter = "serial_number = " + i;
                    if (dv.Count > 0)
                    {
                        string strCustName = dv[0]["CustomerName"].ToString();
                        string strEstName = dv[0]["estimate_name"].ToString();
                        int nCustID = Convert.ToInt32(dv[0]["customer_id"]);
                        int nEstID = Convert.ToInt32(dv[0]["estimate_id"]);
                        decimal TotalExCom_WithIntevcise = GetRetailTotal(nEstID, nCustID);
                        string strColumn = strEstName + "<br/>" + TotalExCom_WithIntevcise.ToString("c");
                        drNew["Selection"] = strCustName;
                        drNew["SelectionEst"] = strColumn;
                        drNew["SelectionID"] = nCustID.ToString() + "," + nEstID.ToString();
                    }
                    else
                    {
                        drNew["Selection"] = "";
                        drNew["SelectionEst"] = "";
                        drNew["SelectionID"] = "0" + "," + "0";
                    }
                }
                else
                {
                    drNew["Selection"] = "";
                    drNew["SelectionEst"] = "";
                    drNew["SelectionID"] = "0" + "," + "0";
                }

                //SiteProgress
                if (dtSiteProgress.Rows.Count > 0)
                {
                    dv = dtSiteProgress.DefaultView;
                    dv.RowFilter = "serial_number = " + i;
                    if (dv.Count > 0)
                    {
                        string strCustName = dv[0]["CustomerName"].ToString();
                        string strEstName = dv[0]["estimate_name"].ToString();
                        int nCustID = Convert.ToInt32(dv[0]["customer_id"]);
                        int nEstID = Convert.ToInt32(dv[0]["estimate_id"]);
                        decimal TotalExCom_WithIntevcise = GetRetailTotal(nEstID, nCustID);
                        string strColumn = strEstName + "<br/>" + TotalExCom_WithIntevcise.ToString("c");
                        drNew["SiteProgress"] = strCustName;
                        drNew["SiteProgressEst"] = strColumn;
                        drNew["SiteProgressID"] = nCustID.ToString() + "," + nEstID.ToString();
                    }
                    else
                    {
                        drNew["SiteProgress"] = "";
                        drNew["SiteProgressEst"] = "";
                        drNew["SiteProgressID"] = "0" + "," + "0";
                    }
                }
                else
                {
                    drNew["SiteProgress"] = "";
                    drNew["SiteProgressEst"] = "";
                    drNew["SiteProgressID"] = "0" + "," + "0";
                }

                //Schedule
                if (dtSchedule.Rows.Count > 0)
                {
                    dv = dtSchedule.DefaultView;
                    dv.RowFilter = "serial_number = " + i;
                    if (dv.Count > 0)
                    {
                        string strCustName = dv[0]["CustomerName"].ToString();
                        string strEstName = dv[0]["estimate_name"].ToString();
                        int nCustID = Convert.ToInt32(dv[0]["customer_id"]);
                        int nEstID = Convert.ToInt32(dv[0]["estimate_id"]);
                        decimal TotalExCom_WithIntevcise = GetRetailTotal(nEstID, nCustID);
                        string strColumn = strEstName + "<br/>" + TotalExCom_WithIntevcise.ToString("c");
                        drNew["Schedule"] = strCustName;
                        drNew["ScheduleEst"] = strColumn;
                        drNew["ScheduleID"] = nCustID.ToString() + "," + nEstID.ToString();
                    }
                    else
                    {
                        drNew["Schedule"] = "";
                        drNew["ScheduleEst"] = "";
                        drNew["ScheduleID"] = "0" + "," + "0";
                    }
                }
                else
                {
                    drNew["Schedule"] = "";
                    drNew["ScheduleEst"] = "";
                    drNew["ScheduleID"] = "0" + "," + "0";
                }

                //FinalProject
                if (dtFinalProject.Rows.Count > 0)
                {
                    dv = dtFinalProject.DefaultView;
                    dv.RowFilter = "serial_number = " + i;
                    if (dv.Count > 0)
                    {
                        string strCustName = dv[0]["CustomerName"].ToString();
                        string strEstName = dv[0]["estimate_name"].ToString();
                        int nCustID = Convert.ToInt32(dv[0]["customer_id"]);
                        int nEstID = Convert.ToInt32(dv[0]["estimate_id"]);
                        decimal TotalExCom_WithIntevcise = GetRetailTotal(nEstID, nCustID);
                        string strColumn = strEstName + "<br/>" + TotalExCom_WithIntevcise.ToString("c");
                        drNew["FinalProject"] = strCustName;
                        drNew["FinalProjectEst"] = strColumn;
                        drNew["FinalProjectID"] = nCustID.ToString() + "," + nEstID.ToString();
                    }
                    else
                    {
                        drNew["FinalProject"] = "";
                        drNew["FinalProjectEst"] = "";
                        drNew["FinalProjectID"] = "0" + "," + "0";
                    }
                }
                else
                {
                    drNew["FinalProject"] = "";
                    drNew["FinalProjectEst"] = "";
                    drNew["FinalProjectID"] = "0" + "," + "0";
                }

                //Completion
                if (dtCompletion.Rows.Count > 0)
                {
                    dv = dtCompletion.DefaultView;
                    dv.RowFilter = "serial_number = " + i;
                    if (dv.Count > 0)
                    {
                        string strCustName = dv[0]["CustomerName"].ToString();
                        string strEstName = dv[0]["estimate_name"].ToString();
                        int nCustID = Convert.ToInt32(dv[0]["customer_id"]);
                        int nEstID = Convert.ToInt32(dv[0]["estimate_id"]);
                        decimal TotalExCom_WithIntevcise = GetRetailTotal(nEstID, nCustID);
                        string strColumn = strEstName + "<br/>" + TotalExCom_WithIntevcise.ToString("c");
                        drNew["Completion"] = strCustName;
                        drNew["CompletionEst"] = strColumn;
                        drNew["CompletionID"] = nCustID.ToString() + "," + nEstID.ToString();
                    }
                    else
                    {
                        drNew["Completion"] = "";
                        drNew["CompletionEst"] = "";
                        drNew["CompletionID"] = "0" + "," + "0";
                    }
                }
                else
                {
                    drNew["Completion"] = "";
                    drNew["CompletionEst"] = "";
                    drNew["CompletionID"] = "0" + "," + "0";

                }

                //Warranty
                if (dtWarranty.Rows.Count > 0)
                {
                    dv = dtWarranty.DefaultView;
                    dv.RowFilter = "serial_number = " + i;
                    if (dv.Count > 0)
                    {
                        string strCustName = dv[0]["CustomerName"].ToString();
                        string strEstName = dv[0]["estimate_name"].ToString();
                        int nCustID = Convert.ToInt32(dv[0]["customer_id"]);
                        int nEstID = Convert.ToInt32(dv[0]["estimate_id"]);
                        decimal TotalExCom_WithIntevcise = GetRetailTotal(nEstID, nCustID);
                        string strColumn = strEstName + "<br/>" + TotalExCom_WithIntevcise.ToString("c");
                        drNew["Warranty"] = strCustName;
                        drNew["WarrantyEst"] = strColumn;
                        drNew["WarrantyID"] = nCustID.ToString() + "," + nEstID.ToString();
                    }
                    else
                    {
                        drNew["Warranty"] = "";
                        drNew["WarrantyEst"] = "";
                        drNew["WarrantyID"] = "0" + "," + "0";
                    }
                }
                else
                {
                    drNew["Warranty"] = "";
                    drNew["WarrantyEst"] = "";
                    drNew["WarrantyID"] = "0" + "," + "0";
                }

                //Completed
                if (dtCompleted.Rows.Count > 0)
                {
                    dv = dtCompleted.DefaultView;
                    dv.RowFilter = "serial_number = " + i;
                    if (dv.Count > 0)
                    {
                        string strCustName = dv[0]["CustomerName"].ToString();
                        string strEstName = dv[0]["estimate_name"].ToString();
                        int nCustID = Convert.ToInt32(dv[0]["customer_id"]);
                        int nEstID = Convert.ToInt32(dv[0]["estimate_id"]);
                        decimal TotalExCom_WithIntevcise = GetRetailTotal(nEstID, nCustID);
                        string strColumn = strEstName + "<br/>" + TotalExCom_WithIntevcise.ToString("c");
                        drNew["Completed"] = strCustName;
                        drNew["CompletedEst"] = strColumn;
                        drNew["CompletedID"] = nCustID.ToString() + "," + nEstID.ToString();
                    }
                    else
                    {
                        drNew["Completed"] = "";
                        drNew["CompletedEst"] = "";
                        drNew["CompletedID"] = "0" + "," + "0";
                    }
                }
                else
                {
                    drNew["Completed"] = "";
                    drNew["CompletedEst"] = "";
                    drNew["CompletedID"] = "0" + "," + "0";
                }

                dtMaster.Rows.Add(drNew);

            }
            if (ddlItemPerPage.SelectedValue != "4")
            {
                grdGraphicsSnap.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
            }
            else
            {
                grdGraphicsSnap.PageSize = 200;
            }
            grdGraphicsSnap.DataSource = dtMaster;
            grdGraphicsSnap.DataKeyNames = new string[] { "DesignID", "SelectionID", "SiteProgressID", "ScheduleID", "FinalProjectID", "CompletionID", "WarrantyID", "CompletedID" };
            grdGraphicsSnap.DataBind();
            // DataTable dt = dtMaster.Copy();

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

            if (grdGraphicsSnap.PageCount == nPageNo + 1)
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
            // lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    private decimal GetRetailTotal(int EstID, int ncustid)
    {
        decimal dRetail = 0;
        DataClassesDataContext _db = new DataClassesDataContext();

        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        decimal total_incentives = 0;
        estimate_payment esp = new estimate_payment();
        if (_db.estimate_payments.Where(ep => ep.estimate_id == EstID && ep.customer_id == ncustid && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == EstID && ep.customer_id == ncustid && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.new_total_with_tax);
            total_incentives = Convert.ToDecimal(esp.total_incentives);
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
            if (totalwithtax == 0)
            {
                var result = (from pd in _db.pricing_details
                              where (from clc in _db.customer_locations
                                     where clc.estimate_id == EstID && clc.customer_id == ncustid && clc.client_id == Convert.ToInt32(hdnClientId.Value)
                                     select clc.location_id).Contains(pd.location_id) &&
                                     (from cs in _db.customer_sections
                                      where cs.estimate_id == EstID && cs.customer_id == ncustid && cs.client_id == Convert.ToInt32(hdnClientId.Value)
                                      select cs.section_id).Contains(pd.section_level) && pd.estimate_id == EstID && pd.customer_id == ncustid && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.pricing_type == "A" && pd.is_CommissionExclude == false
                              select pd.total_retail_price);
                int n = result.Count();
                if (result != null && n > 0)
                    dRetail = result.Sum();

                totalwithtax = dRetail;

            }
        }

        return totalwithtax;
    }

    private DataTable LoadMasterTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("Design", typeof(string));
        table.Columns.Add("DesignID", typeof(string));
        table.Columns.Add("DesignEst", typeof(string));

        table.Columns.Add("Selection", typeof(string));
        table.Columns.Add("SelectionID", typeof(string));
        table.Columns.Add("SelectionEst", typeof(string));

        table.Columns.Add("SiteProgress", typeof(string));
        table.Columns.Add("SiteProgressID", typeof(string));
        table.Columns.Add("SiteProgressEst", typeof(string));

        table.Columns.Add("Schedule", typeof(string));
        table.Columns.Add("ScheduleID", typeof(string));
        table.Columns.Add("ScheduleEst", typeof(string));

        table.Columns.Add("FinalProject", typeof(string));
        table.Columns.Add("FinalProjectID", typeof(string));
        table.Columns.Add("FinalProjectEst", typeof(string));

        table.Columns.Add("Completion", typeof(string));
        table.Columns.Add("CompletionID", typeof(string));
        table.Columns.Add("CompletionEst", typeof(string));

        table.Columns.Add("Warranty", typeof(string));
        table.Columns.Add("WarrantyID", typeof(string));
        table.Columns.Add("WarrantyEst", typeof(string));

        table.Columns.Add("Completed", typeof(string));
        table.Columns.Add("CompletedID", typeof(string));
        table.Columns.Add("CompletedEst", typeof(string));


        return table;
    }
    protected void grdGraphicsSnap_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string DesignID = grdGraphicsSnap.DataKeys[e.Row.RowIndex].Values[0].ToString();
            string SelectionID = grdGraphicsSnap.DataKeys[e.Row.RowIndex].Values[1].ToString();
            string SiteProgressID = grdGraphicsSnap.DataKeys[e.Row.RowIndex].Values[2].ToString();
            string ScheduleID = grdGraphicsSnap.DataKeys[e.Row.RowIndex].Values[3].ToString();
            string FinalProjectID = grdGraphicsSnap.DataKeys[e.Row.RowIndex].Values[4].ToString();
            string CompletionID = grdGraphicsSnap.DataKeys[e.Row.RowIndex].Values[5].ToString();
            string WarrantyID = grdGraphicsSnap.DataKeys[e.Row.RowIndex].Values[6].ToString();
            string CompletedID = grdGraphicsSnap.DataKeys[e.Row.RowIndex].Values[7].ToString();

            var items = DesignID.Split(',');
            string a = items[0];
            string b = items[1];

            HyperLink hyp_Design = (HyperLink)e.Row.FindControl("hyp_Design");
            hyp_Design.NavigateUrl = "customerlist.aspx?cid=" + a + "&eid=" + b;

            items = SelectionID.Split(',');
            a = items[0];
            b = items[1];
            HyperLink hyp_Selection = (HyperLink)e.Row.FindControl("hyp_Selection");
            hyp_Selection.NavigateUrl = "customerlist.aspx?cid=" + a + "&eid=" + b;

            items = SiteProgressID.Split(',');
            a = items[0];
            b = items[1];
            HyperLink hyp_SiteProgress = (HyperLink)e.Row.FindControl("hyp_SiteProgress");
            hyp_SiteProgress.NavigateUrl = "customerlist.aspx?cid=" + a + "&eid=" + b;

            items = ScheduleID.Split(',');
            a = items[0];
            b = items[1];
            HyperLink hyp_Schedule = (HyperLink)e.Row.FindControl("hyp_Schedule");
            hyp_Schedule.NavigateUrl = "customerlist.aspx?cid=" + a + "&eid=" + b;

            items = FinalProjectID.Split(',');
            a = items[0];
            b = items[1];
            HyperLink hyp_FinalProject = (HyperLink)e.Row.FindControl("hyp_FinalProject");
            hyp_FinalProject.NavigateUrl = "customerlist.aspx?cid=" + a + "&eid=" + b;

            items = CompletionID.Split(',');
            a = items[0];
            b = items[1];
            HyperLink hyp_Completion = (HyperLink)e.Row.FindControl("hyp_Completion");
            hyp_Completion.NavigateUrl = "customerlist.aspx?cid=" + a + "&eid=" + b;

            items = WarrantyID.Split(',');
            a = items[0];
            b = items[1];
            HyperLink hyp_Warranty = (HyperLink)e.Row.FindControl("hyp_Warranty");
            hyp_Warranty.NavigateUrl = "customerlist.aspx?cid=" + a + "&eid=" + b;

            items = CompletedID.Split(',');
            a = items[0];
            b = items[1];
            HyperLink hyp_Completed = (HyperLink)e.Row.FindControl("hyp_Completed");
            hyp_Completed.NavigateUrl = "customerlist.aspx?cid=" + a + "&eid=" + b;
        }

    }
    protected void grdGraphicsSnap_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdGraphicsSnap.ID, grdGraphicsSnap.GetType().Name, "PageIndexChanging"); 
        GetSnapShotDataList(e.NewPageIndex);
    }
   
    protected void btnNext_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNext.ID, btnNext.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetSnapShotDataList(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetSnapShotDataList(nCurrentPage - 2);
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        GetSnapShotDataList(0);
    }

    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "SelectedIndexChanged"); 
        GetSnapShotDataList(0);
    }

    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        ddlStatus.SelectedIndex = -1;
        GetSnapShotDataList(0);

    }


    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "SelectedIndexChanged"); 
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

        GetSnapShotDataList(0);
    }
    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlStatus.ID, ddlStatus.GetType().Name, "SelectedIndexChanged"); 
        txtSearch.Text = string.Empty;
        GetSnapShotDataList(0);
    }
}