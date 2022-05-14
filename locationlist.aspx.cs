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

public partial class locationlist : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetLocationName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["lSearch"] != null)
        {
            List<location> cList = (List<location>)HttpContext.Current.Session["lSearch"];
            return (from c in cList
                    where c.location_name.ToLower().StartsWith(prefixText.ToLower())
                    select c.location_name).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.locations
                    where c.location_name.StartsWith(prefixText)
                    select c.location_name).Distinct().Take<String>(count).ToArray();
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
            if (Page.User.IsInRole("loc001") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            DataClassesDataContext _db = new DataClassesDataContext();
            List<location> lList = _db.locations.ToList();
            Session.Add("lSearch", lList);

            GetLocations(0);
        }
    }
    protected void GetLocations(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        int nClientId = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        string strCondition = "";
        grdLocationList.PageIndex = nPageNo;

       

        if (txtSearch.Text.Trim() != "")
        {
            string strSearch = txtSearch.Text.Trim();

            strCondition = " location_name LIKE '%" + strSearch + "%'";            
        }

        if (Convert.ToInt32(ddlStatus.SelectedValue) == 1)
        {
            if (strCondition.Length > 0)
                strCondition += " AND is_active = 1 ";
            else
                strCondition = " is_active = 1 ";
        }
        else if (Convert.ToInt32(ddlStatus.SelectedValue) == 2)
        {

            if (strCondition.Length > 0)
                strCondition += " AND is_active = 0 ";
            else
                strCondition = " is_active = 0 ";
        }
        else
        {
            if (strCondition.Length > 0)
                strCondition += " AND client_id =" + nClientId;
            else
                strCondition = " client_id =" + nClientId;
        }

        if (strCondition.Length > 0)
        {
            strCondition = "Where " + strCondition;
        }
        
        string strQ = string.Empty;
        strQ = "SELECT  location_id, location_name, loation_desc, client_id, is_active "+
                " FROM location" +
                " " + strCondition + " ORDER BY location_name ";

        IEnumerable<LocationModel> lList = _db.ExecuteQuery<LocationModel>(strQ, string.Empty).ToList();


        if (ddlItemPerPage.SelectedValue != "4")
        {
            grdLocationList.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
        }
        else
        {
            grdLocationList.PageSize = 200;
        }
        grdLocationList.DataSource = lList;
        grdLocationList.DataBind();
        
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

        if (grdLocationList.PageCount == nPageNo + 1)
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
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        Response.Redirect("locationdetails.aspx");
    }
    protected void grdLocationList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdLocationList.ID, grdLocationList.GetType().Name, "PageIndexChanging"); 
        GetLocations(e.NewPageIndex);
    }
    protected void grdLocationList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (Convert.ToBoolean(e.Row.Cells[1].Text) == true)
                e.Row.Cells[1].Text = "Yes";
            else
                e.Row.Cells[1].Text = "No";
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        GetLocations(0);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetLocations(nCurrentPage - 2);
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNext.ID, btnNext.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetLocations(nCurrentPage);
    }
    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "SelectedIndexChanged"); 
        GetLocations(0);
    }
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        GetLocations(0);
    }

    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlStatus.ID, ddlStatus.GetType().Name, "SelectedIndexChanged"); 
        GetLocations(0);
    }
}
