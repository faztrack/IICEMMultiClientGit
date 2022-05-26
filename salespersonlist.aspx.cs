using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class salespersonlist : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["sSearch"] != null)
        {
            List<sales_person> cList = (List<sales_person>)HttpContext.Current.Session["sSearch"];
            return (from c in cList
                    where c.last_name.ToLower().StartsWith(prefixText.ToLower())
                    select c.last_name).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.sales_persons
                    where c.last_name.StartsWith(prefixText)
                    select c.last_name).Distinct().Take<String>(count).ToArray();
        }
    }
    [WebMethod]
    public static string[] GetFirstName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["sSearch"] != null)
        {
            List<sales_person> cList = (List<sales_person>)HttpContext.Current.Session["sSearch"];
            return (from c in cList
                    where c.first_name.ToLower().StartsWith(prefixText.ToLower())
                    select c.first_name).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.sales_persons
                    where c.first_name.StartsWith(prefixText)
                    select c.first_name).Distinct().Take<String>(count).ToArray();
        }
    }
    [WebMethod]
    public static string[] GetEmail(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["sSearch"] != null)
        {
            List<sales_person> cList = (List<sales_person>)HttpContext.Current.Session["sSearch"];
            return (from c in cList
                    where c.email.ToLower().StartsWith(prefixText.ToLower())
                    select c.email).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.sales_persons
                    where c.email.StartsWith(prefixText)
                    select c.email).Distinct().Take<String>(count).ToArray();
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
            else
            {
                hdnClientId.Value = ((userinfo)Session["oUser"]).client_id.ToString();
            }
            if (Page.User.IsInRole("sales001") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            DataClassesDataContext _db = new DataClassesDataContext();
            List<sales_person> salesList = _db.sales_persons.ToList();
            Session.Add("sSearch", salesList);
            GetSalesPersons(0);
        }
    }
    protected void GetSalesPersons(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strCondition = "";
        grdSalesPersonList.PageIndex = nPageNo;
               

        if (txtSearch.Text.Trim() != "")
        {
            string strSearch = txtSearch.Text.Trim();

            if (ddlSearchBy.SelectedValue == "1")// First Name
            {
                strCondition = " first_name LIKE '%" + strSearch + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "2") // Last Name
            {
                strCondition = " last_name LIKE '%" + strSearch + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "3") // Email
            {
                strCondition = " email LIKE '%" + strSearch + "%'";
            }
        }

        if (Convert.ToInt32(ddlStatus.SelectedValue) == 1)
        {
            if (strCondition.Length > 0)
                strCondition += " AND sales_person_id != 0 AND is_active = 1 ";
            else
                strCondition = " sales_person_id != 0 AND is_active = 1 ";
        }
        else if (Convert.ToInt32(ddlStatus.SelectedValue) == 2)
        {

            if (strCondition.Length > 0)
                strCondition += " AND sales_person_id != 0 AND is_active = 0 ";
            else
                strCondition = " sales_person_id != 0 AND is_active = 0 ";

        }
        else
        {
            if (strCondition.Length > 0)
                strCondition += " AND sales_person_id != 0 AND client_id in (" + hdnClientId.Value + ")";
            else
                strCondition = " sales_person_id != 0 AND client_id in (" + hdnClientId.Value + ")";
        }

        if (strCondition.Length > 0)
        {
            strCondition = "Where " + strCondition;
        }

        string strQ = string.Empty;
        strQ = "SELECT sales_person_id, first_name, last_name, address, city, state, zip, phone, fax, email, " +
                " role_id, is_active, is_sales, is_service, is_install,  " +
                " create_date, last_login_time, client_id, com_per, " +
                " google_calendar_account, google_calendar_id, co_com_per " +
                " FROM  sales_person " +
                " " + strCondition + " ORDER BY first_name ";

        IEnumerable<SalesPersonModel> sList = _db.ExecuteQuery<SalesPersonModel>(strQ, string.Empty).ToList();
        if (ddlItemPerPage.SelectedValue != "4")
        {
            grdSalesPersonList.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
        }
        else
        {
            grdSalesPersonList.PageSize = 200;
        }
        grdSalesPersonList.DataSource = sList;
        grdSalesPersonList.DataBind();

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

        if (grdSalesPersonList.PageCount == nPageNo + 1)
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
    protected void btnNext_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetSalesPersons(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetSalesPersons(nCurrentPage - 2);
    }
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        Response.Redirect("salesperson.aspx");
    }
    protected void grdSalesPersonList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdSalesPersonList.ID, grdSalesPersonList.GetType().Name, "PageIndexChanging"); 
        GetSalesPersons(e.NewPageIndex);
    }
    protected void grdSalesPersonList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (Convert.ToBoolean(e.Row.Cells[4].Text) == true)
                e.Row.Cells[4].Text = "Yes";
            else
                e.Row.Cells[4].Text = "No";
        }
    }
    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "PageIndexChanging"); 
        GetSalesPersons(0);
    }
    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlStatus.ID, ddlStatus.GetType().Name, "PageIndexChanging"); 
        GetSalesPersons(0);
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        GetSalesPersons(0);
    }
    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "Click"); 
        txtSearch.Text = "";
        wtmFileNumber.WatermarkText = "Search by " + ddlSearchBy.SelectedItem.Text;

        if (ddlSearchBy.SelectedValue == "1")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetFirstName";
        }
        else if (ddlSearchBy.SelectedValue == "2")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetLastName";
        }
        else if (ddlSearchBy.SelectedValue == "3")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetEmail";
        }

        GetSalesPersons(0);
    }
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        GetSalesPersons(0);
    }
}
