using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PMNotesListALL : System.Web.UI.Page
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
                if (Page.User.IsInRole("calllog003") == false)
                {
                    // No Permission Page.
                    Response.Redirect("nopermission.aspx");
                }

                // Get Leads
                #region Get Customer

                DataClassesDataContext _db = new DataClassesDataContext();
                List<customer> CustomerList = _db.customers.Where(c => c.isCustomer == 1 && c.is_active == true).ToList();
                Session.Add("clSearch", CustomerList);

                #endregion


                int nPage = 0;
                txtSearch.Text = "";
                txtStartDate.Text = DateTime.Now.AddDays(-180).ToShortDateString();
                txtEndDate.Text = DateTime.Now.ToShortDateString();
                BindSuperintendent();
                if (Session["sPMNotes"] != null)
                {
                    if (Session["spPage"] != null)
                    {
                        nPage = Convert.ToInt32(Session["spPage"]);

                    }
                    if (Session["txtSearch"] != null)
                    {
                        txtSearch.Text = Session["txtSearch"].ToString();
                    }
                    if (Session["ddlsearchBy"] != null)
                    {
                        ddlSearchBy.SelectedValue = Session["ddlsearchBy"].ToString();
                    }
                    
                }
              getPMNote();

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
        string strQ = "select first_name+' '+last_name AS Superintendent_name,user_id from user_info WHERE role_id = 4 and is_active=1";
        List<userinfo> mList = _db.ExecuteQuery<userinfo>(strQ, string.Empty).ToList();
        lstEmployee.DataSource = mList;
        lstEmployee.DataTextField = "Superintendent_name";
        lstEmployee.DataValueField = "user_id";
        lstEmployee.DataBind();
    }

    //protected void ddlSuperintendent_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSuperintendent.ID, ddlSuperintendent.GetType().Name, "Super intendent Changed");
    //    getPMNote();

    //}

    protected void lnkOpen_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton btnsubmit = sender as LinkButton;

            GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
            Label lblDescription = gRow.Cells[5].Controls[0].FindControl("lblDescription") as Label;
            Label lblDescription_r = gRow.Cells[5].Controls[1].FindControl("lblDescription_r") as Label;
            LinkButton lnkOpen = gRow.Cells[5].Controls[2].FindControl("lnkOpen") as LinkButton;

            if (lnkOpen.Text == "More")
            {
                lblDescription.Visible = false;
                lblDescription_r.Visible = true;
                lnkOpen.Text = " Less";
                lnkOpen.ToolTip = "Click here to view less";
            }
            else
            {
                lblDescription.Visible = true;
                lblDescription_r.Visible = false;
                lnkOpen.Text = "More";
                lnkOpen.ToolTip = "Click here to view more";
            }
        }
        catch (Exception ex)
        {

            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }


    protected void getPMNote()
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
                strCondition += " AND pii.CreatedBy in (" + selectedvalue.TrimEnd(',') + ")";

            if (ddlStatus.SelectedItem.Text!= "All")
                strCondition += " AND pii.IsComplete ="+ddlStatus.SelectedValue +"";

            if (txtSearch.Text.Trim() != "")
            {

                string str = txtSearch.Text.Trim();
                if (ddlSearchBy.SelectedValue == "1")
                {
                    if (strCondition.Length > 0)
                        strCondition += " AND c.first_name1 LIKE '%" + str + "%'";
                    else
                        strCondition = " c.first_name1 LIKE '%" + str + "%'";
                }
                else if (ddlSearchBy.SelectedValue == "2")
                {
                    if (strCondition.Length > 0)
                        strCondition += " AND c.last_name1 LIKE '%" + str + "%'";
                    else
                        strCondition = "  c.last_name1 LIKE '%" + str + "%'";

                }

            }

            if (strCondition.Length > 0)
            {

                strCondition = "Where  " + strCondition;
            }


            string strQ = " select distinct si.section_name, pii.customer_id,pii.PMNoteId, c.first_name1 + ' ' + c.last_name1 as CustomerName, pii.estimate_id,pii.section_id, pii.NoteDetails,pii.CreateDate,pii.CreatedBy,pii.vendor_id,pii.vendor_name ," +
                    " case when pii.IsComplete = 1 then 'Yes' else 'No' end as Complete from PMNoteInfo as pii inner join customers as c on c.customer_id = pii.customer_id" +
                    " left join sectioninfo as si on si.section_id = pii.section_id  " + strCondition + " order by CreateDate desc ";
            DataTable dt = csCommonUtility.GetDataTable(strQ);
            Session.Add("sPMNotes", dt);

            BindPMNote(0);
        }
        catch (Exception ex)
        {

            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    private void BindPMNote(int nPageNo)
    {
        try
        {
            DataTable dt = (DataTable)Session["sPMNotes"];
            grdPMNotesList.PageIndex = nPageNo;
            grdPMNotesList.DataSource = dt;
            grdPMNotesList.DataKeyNames = new string[] { "PMNoteId", "customer_id", "estimate_id", "CustomerName" };
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

    protected void grdCustCOList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int nCustomer_id = Convert.ToInt32(grdPMNotesList.DataKeys[e.Row.RowIndex].Values[1]);
                int nEstimate_id = Convert.ToInt32(grdPMNotesList.DataKeys[e.Row.RowIndex].Values[2]);
                string CustomerName = grdPMNotesList.DataKeys[e.Row.RowIndex].Values[3].ToString();
                HyperLink hypCustomerName = (HyperLink)e.Row.FindControl("hypCustomerName");
                hypCustomerName.Text = CustomerName;
                hypCustomerName.NavigateUrl = "schedulecalendar.aspx?TypeID=1&eid=" + nEstimate_id + "&cid=" + nCustomer_id;

                Label lblDescription = (Label)e.Row.FindControl("lblDescription");
                LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
                string str = lblDescription.Text.Replace("&nbsp;", "");



                if (str != "" && str.Length > 250)
                {
                    lblDescription.Text = str.Substring(0, 250) + " ...";
                    lblDescription.ToolTip = str;
                    lnkOpen.Visible = true;
                }
                else
                {
                    lblDescription.Text = str;
                    lnkOpen.Visible = false;
                }

            }
        }
        catch (Exception ex)
        {

            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Search Click");
        Session.Add("txtSearch", txtSearch.Text);
        getPMNote();
    }

    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {

        getPMNote();
    }

    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, LinkButton2.ID, LinkButton2.GetType().Name, "View All Click");
        try
        {
            txtStartDate.Text = DateTime.Now.AddDays(-180).ToShortDateString();
            txtEndDate.Text = DateTime.Now.ToShortDateString();
            txtSearch.Text = "";
            selectedvalue = "";
            ddlStatus.SelectedIndex = 0;
            //ddlSuperintendent.SelectedIndex = 0;
            Session.Remove("spPage");
            Session.Remove("txtSearch");
            Session.Remove("ddlsearchBy"); 
            Session.Remove("sPMNotes");
           
            BindSuperintendent();
            getPMNote();

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

        getPMNote();
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

            Session.Add("ddlsearchBy",ddlSearchBy.SelectedValue);

            getPMNote();
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
            Session.Add("spPage", e.NewPageIndex.ToString());
            BindPMNote(e.NewPageIndex);
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
            Session.Add("spPage", nCurrentPage.ToString());
            BindPMNote(nCurrentPage);
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
            Session.Add("spPage", (nCurrentPage - 2).ToString());
            BindPMNote(nCurrentPage - 2);
        }
        catch (Exception ex)
        {

            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    protected void grdCustCOList_Sorting(object sender, GridViewSortEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdPMNotesList.ID, grdPMNotesList.GetType().Name, "Cust COList Sorting");
        try
        {
            int nPage = 0;
            if (Session["spPage"] != null)
                nPage = Convert.ToInt32(Session["spPage"]);
            DataTable dtCallList = (DataTable)Session["sPMNotes"];
            if (hdnOrder.Value == "ASC")
                hdnOrder.Value = "DESC";
            else
                hdnOrder.Value = "ASC";

            string strShort = e.SortExpression + " " + hdnOrder.Value;
            DataView dv = dtCallList.DefaultView;
            dv.Sort = strShort;
            Session["sPMNotes"] = dv.ToTable();
            BindPMNote(nPage);
            
        }
        catch (Exception ex)
        {

            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }




    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        getPMNote();
    }
}