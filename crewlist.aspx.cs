using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class crewlist : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["sSearch"] != null)
        {
            List<Crew_Detail> cList = (List<Crew_Detail>)HttpContext.Current.Session["sSearch"];
            return (from c in cList
                    where c.last_name.ToLower().Contains(prefixText.ToLower())
                    select c.last_name).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.Crew_Details
                    where c.last_name.Contains(prefixText)
                    select c.last_name).Distinct().Take<String>(count).ToArray();
        }
    }
    [WebMethod]
    public static string[] GetFirstName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["sSearch"] != null)
        {
            List<Crew_Detail> cList = (List<Crew_Detail>)HttpContext.Current.Session["sSearch"];
            return (from c in cList
                    where c.first_name.ToLower().Contains(prefixText.ToLower())
                    select c.first_name).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.Crew_Details
                    where c.first_name.Contains(prefixText)
                    select c.first_name).Distinct().Take<String>(count).ToArray();
        }
    }
    [WebMethod]
    public static string[] GetUserName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["sSearch"] != null)
        {
            List<Crew_Detail> cList = (List<Crew_Detail>)HttpContext.Current.Session["sSearch"];
            return (from c in cList
                    where c.username.ToLower().Contains(prefixText.ToLower())
                    select c.username).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.Crew_Details
                    where c.username.Contains(prefixText)
                    select c.username).Distinct().Take<String>(count).ToArray();
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
            if (Page.User.IsInRole("t01") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            DataClassesDataContext _db = new DataClassesDataContext();
            List<Crew_Detail> crewList = (from c in _db.Crew_Details where c.is_active==true select c).ToList();
            Session.Add("sSearch", crewList);


            //var item = from u in _db.Crew_Details
            //           where u.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && u.is_active == true
            //           orderby u.full_name ascending
            //           select u;
            //Session.Add("sCrews", csCommonUtility.LINQToDataTable(item));

            string strCondition = "";

            string strQ = "";

            strCondition += " where is_active =1 AND client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            strQ = "SELECT crew_id, client_id, first_name, last_name, phone, is_active, full_name,email,username,CreatedDate " +
               " FROM Crew_Details " +
               "" + strCondition + "" +
              " ORDER BY full_name asc";

            IEnumerable<csCrewList> clist = _db.ExecuteQuery<csCrewList>(strQ, string.Empty).ToList();
            Session.Add("sCrews", csCommonUtility.LINQToDataTable(clist));
            GetCrew(0);
           // GetSearchedCrews();
        }
    }
    protected void GetCrew(int nPageNo)
    {

        try
        {
            if (Session["sCrews"] != null)
            {

                if (ddlItemPerPage.SelectedValue != "4")
                {
                    grdCrewList.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
                }
                else
                {
                    grdCrewList.PageSize = 200;
                }

                DataTable dtCrews = (DataTable)Session["sCrews"];
               // grdCrewList.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
                grdCrewList.PageIndex = nPageNo;
                grdCrewList.DataSource = dtCrews;
                grdCrewList.DataKeyNames = new string[] { "crew_id", "is_active" };
                grdCrewList.DataBind();

            }
        }
        catch (Exception ex)
        {

        }

       
        
        //grdCrewList.DataSource = item;
        //grdCrewList.DataKeyNames = new string[] { "crew_id", "is_active" };
        //grdCrewList.DataBind();
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

        if (grdCrewList.PageCount == nPageNo + 1)
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

   private void GetSearchedCrews()
   {

       try
       {
           DataClassesDataContext _db = new DataClassesDataContext();

           string str = txtSearch.Text.Trim();
           string strCondition = "";

           string strQ = "";
           string strKey = "";
           strCondition += " where client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);

           if (txtSearch.Text.Trim() != "")
           {
               strKey = txtSearch.Text.Trim().ToUpper();

               if (strCondition.Length > 0)
                   strCondition += " AND (last_name LIKE '%" + strKey + "%' OR " +
                                   " first_name LIKE '%" + strKey + "%' OR " +
                                   " username LIKE '%" + strKey + "%') ";

               else
                   strCondition = " where (last_name LIKE '%" + strKey + "%' OR " +
                                   " first_name LIKE '%" + strKey + "%' OR " +
                                   " username LIKE '%" + strKey + "%' )";

           }
           if (ddlStatus.SelectedItem.Text != "All")
           {


               strCondition += " AND is_active=" + Convert.ToInt32(ddlStatus.SelectedValue);
           }

           strQ = "SELECT crew_id, client_id, first_name, last_name, phone, is_active, full_name,email,username,CreatedDate " +
                " FROM Crew_Details " +
                "" + strCondition + "" +
               " ORDER BY full_name asc";

           IEnumerable<csCrewList> clist = _db.ExecuteQuery<csCrewList>(strQ, string.Empty).ToList();
           Session.Add("sCrews", csCommonUtility.LINQToDataTable(clist));

           

           GetCrew(0);
       }
       catch( Exception ex)
       {

       }
   }
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        Response.Redirect("crewdetails.aspx");
    }
    protected void grdCrewList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            int crId = Convert.ToInt32(grdCrewList.DataKeys[e.Row.RowIndex].Values[0]);
           
            if (Convert.ToBoolean(e.Row.Cells[3].Text) == true)
                e.Row.Cells[3].Text = "Yes";
            else
                e.Row.Cells[3].Text = "No";
        }
    }
    protected void grdCrewList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GetCrew(e.NewPageIndex);
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        GetSearchedCrews();
        // GetCrew(0);
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNext.ID, btnNext.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetCrew(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetCrew(nCurrentPage - 2);
    }
    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "SelectedIndexChanged"); 
       // GetSearchedCrews();
        GetCrew(0);
    }
    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlStatus.ID, ddlStatus.GetType().Name, "SelectedIndexChanged"); 
        GetSearchedCrews();
        
    }

    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "SelectedIndexChanged"); 
        txtSearch.Text = "";
        wtmFileNumber.WatermarkText = "Search by " + ddlSearchBy.SelectedItem.Text;

        if (ddlSearchBy.SelectedValue == "1")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetLastName";
        }
        else if (ddlSearchBy.SelectedValue == "2")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetFirstName";
        }
        else if (ddlSearchBy.SelectedValue == "3")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetUserName";
        }

        GetCrew(0);
    }

    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        ddlStatus.SelectedValue = "1";
        txtSearch.Text = "";
        GetSearchedCrews();
    }
    protected void grdCrewList_Sorting(object sender, GridViewSortEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCrewList.ID, grdCrewList.GetType().Name, "Sorting"); 
        DataTable dtCrews = (DataTable)Session["sCrews"];

        string strShort = e.SortExpression + " " + hdnOrder.Value;

        DataView dv = dtCrews.DefaultView;
        dv.Sort = strShort;
        Session["sCrews"] = dv.ToTable();

        if (hdnOrder.Value == "ASC")
        {
            hdnOrder.Value = "DESC";
            lblSortedBy.Text = "''" + e.SortExpression + "''" + " Ascending";
        }
        else
        {
            hdnOrder.Value = "ASC";
            lblSortedBy.Text = "''" + e.SortExpression + "''" + " Descending";
        }

     

        lblSortedBy.Text = lblSortedBy.Text.Replace("full_name", "Crew Name");
        lblSortedBy.Text = lblSortedBy.Text.Replace("username", "Username");

        GetCrew(0);
      
        
    }
}