using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SuperintendentConfirm : System.Web.UI.Page
{


    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    { 
        if (HttpContext.Current.Session["clSearch_Superintendent"] != null)
        {
            List<csCustomer> cList = (List<csCustomer>)HttpContext.Current.Session["clSearch_Superintendent"];
            return (from c in cList
                    where c.last_name1.ToLower().StartsWith(prefixText.ToLower())
                    select c.last_name1).Take<String>(count).ToArray();
            //tesr
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
        if (HttpContext.Current.Session["clSearch_Superintendent"] != null)
        {
            List<csCustomer> cList = (List<csCustomer>)HttpContext.Current.Session["clSearch_Superintendent"];
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

    string selectedvalue = "";
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
        DateTime end = (DateTime)Session["loadstarttime"];
        TimeSpan loadtime = DateTime.Now - end;
        lblLoadTime.Text = (Math.Round(Convert.ToDecimal(loadtime.TotalSeconds), 3).ToString()) + " Sec";
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Session.Add("loadstarttime", DateTime.Now);
            if (!IsPostBack)
            {
                KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
                if (Session["oUser"] == null)
                {
                    Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
                }
                if (Page.User.IsInRole("calllog004") == false)
                {
                    // No Permission Page.
                    Response.Redirect("nopermission.aspx");
                }

             
                #region Get Customer

                DataClassesDataContext _db = new DataClassesDataContext();
             


                List<csCustomer> CustomerList = (from cList in _db.SuperConfirmTomorrows

                               select new csCustomer
                               {
                                   first_name1 = cList.first_name1,
                                   last_name1 = cList.last_name1
                               }).Distinct().ToList();

                Session.Add("clSearch_Superintendent", CustomerList);

                #endregion


              
                txtSearch.Text = "";
                txtStartDate.Text = DateTime.Now.ToShortDateString();
                txtEndDate.Text = DateTime.Now.ToShortDateString();
                BindSuperintendent();
                
              getSupperData();

            }
        }
        catch (Exception ex)
        {

            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    private void BindSuperintendent()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select Distinct superlastname AS Superintendent_name,SuperintendentId AS user_id from SuperConfirmTomorrow ";
        List<userinfo> mList = _db.ExecuteQuery<userinfo>(strQ, string.Empty).ToList();
        lstEmployee.DataSource = mList;
        lstEmployee.DataTextField = "Superintendent_name";
        lstEmployee.DataValueField = "user_id";
        lstEmployee.DataBind();
    }





    protected void getSupperData()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
           
            string strCondition = "";
            selectedvalue = "";

            foreach (ListItem item in lstEmployee.Items)
            {
                if (item.Selected)
                {
                    selectedvalue += "'"+item.Text +"',";
                }
            }

            if (txtStartDate.Text != "" && txtEndDate.Text != "")
            {
                DateTime strStartDate = Convert.ToDateTime(txtStartDate.Text.Trim());
                DateTime strEndDate = Convert.ToDateTime(txtEndDate.Text.Trim());
                strCondition += " CONVERT(DATETIME,CreateDate)>='" + strStartDate + "' AND  CONVERT(DATETIME,CreateDate) <'" + strEndDate.AddDays(1).ToString() + "' ";
            }

            if (selectedvalue.Length > 0)
                strCondition += " AND superlastname in (" + selectedvalue.TrimEnd(',') + ")";

           

            if (ddlStatus.SelectedItem.Text != "All")
                strCondition += " AND StartNo =" + ddlStatus.SelectedValue + "";

            if (txtSearch.Text.Trim() != "")
            {

                string str = txtSearch.Text.Trim();
                if (ddlSearchBy.SelectedValue == "1")
                {
                    if (strCondition.Length > 0)
                        strCondition += " AND first_name1 LIKE '%" + str + "%'";
                    else
                        strCondition = " first_name1 LIKE '%" + str + "%'";
                }
                else if (ddlSearchBy.SelectedValue == "2")
                {
                    if (strCondition.Length > 0)
                        strCondition += " AND last_name1 LIKE '%" + str + "%'";
                    else
                        strCondition = "  last_name1 LIKE '%" + str + "%'";

                }

            }

            if (strCondition.Length > 0)
            {

                strCondition = "Where  " + strCondition;
            }


            string strQ = " SELECT Id, SuperintendentId, superlastname, event_id, estimate_id, customer_id, first_name1, last_name1,first_name1+ ' '+last_name1 AS CustomerName,section_name, " +
                    " location_name, estimate_name, event_start, event_end, CreateDate, StartYes, StartNo, employee_name  " +
                    " FROM SuperConfirmTomorrow  " + strCondition + " order by CreateDate desc ";
            DataTable dt = csCommonUtility.GetDataTable(strQ);
            Session.Add("SuperConfirms", dt);

            BindSupperData(0);
        }
        catch (Exception ex)
        {

            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    private void BindSupperData(int nPageNo)
    {
        try
        {
            DataTable dt = (DataTable)Session["SuperConfirms"];
            grdPMNotesList.PageIndex = nPageNo;
            grdPMNotesList.DataSource = dt;
            grdPMNotesList.DataKeyNames = new string[] { "Id", "customer_id", "estimate_id", "superlastname" };
            grdPMNotesList.DataBind();
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

            if (grdPMNotesList.PageCount == nPageNo + 1)
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
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }




    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Search Click");
        
        getSupperData();
    }

    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {

        getSupperData();
    }

    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, LinkButton2.ID, LinkButton2.GetType().Name, "View All Click");
        try
        {
            txtStartDate.Text = DateTime.Now.ToShortDateString();
            txtEndDate.Text = DateTime.Now.ToShortDateString();
            txtSearch.Text = "";
            selectedvalue = "";
            ddlStatus.SelectedIndex = 0;
            BindSuperintendent();
            getSupperData();

        }
        catch (Exception ex)
        {

            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void btnView_Click(object sender, EventArgs e)
    {

        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnView.ID, btnView.GetType().Name, "View Click");
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

        getSupperData();
    }

    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "Search Changed");
        try
        {
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

          getSupperData();
        }
        catch (Exception ex)
        {

            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }


    protected void grdCustCOList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdPMNotesList.ID, grdPMNotesList.GetType().Name, "Cust COList Changed");
        try
        {
           
            BindSupperData(e.NewPageIndex);
        }
        catch (Exception ex)
        {

            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        try
        {
            int nCurrentPage = 0;
            nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
          
            BindSupperData(nCurrentPage);
        }
        catch (Exception ex)
        {

            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        try
        {
            int nCurrentPage = 0;
            nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
          
            BindSupperData(nCurrentPage - 2);
        }
        catch (Exception ex)
        {

            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    




    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        getSupperData();
    }
}