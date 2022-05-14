using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class leadsource_list : System.Web.UI.Page
{
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
            if (Page.User.IsInRole("admin008") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            GetLeadSource(0);
        }
    }

    protected void GetLeadSource(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        grdLeadSource.PageIndex = nPageNo;

        var item = from l in _db.lead_sources
                   where l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                   orderby l.lead_name
                   select l;

        if (txtSearch.Text.Trim() != "")
        {
            string str = txtSearch.Text.Trim();
            item = from lead in _db.lead_sources
                   where lead.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && lead.lead_name.Contains(str)
                   orderby lead.lead_name
                   select lead;
        }
        if (ddlItemPerPage.SelectedValue != "4")
        {
            grdLeadSource.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
        }
        else
        {
            grdLeadSource.PageSize = 200;
        }
        grdLeadSource.DataSource = item;
        grdLeadSource.DataBind();

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

        if (grdLeadSource.PageCount == nPageNo + 1)
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
    protected void grdLeadSource_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdLeadSource.ID, grdLeadSource.GetType().Name, "PageIndexChanging"); 
        GetLeadSource(e.NewPageIndex);
    }
    protected void grdLeadSource_RowDataBound(object sender, GridViewRowEventArgs e)
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
        GetLeadSource(0);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetLeadSource(nCurrentPage - 2);
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNext.ID, btnNext.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetLeadSource(nCurrentPage);
    }
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        Response.Redirect("leadsourcedetails.aspx");
    }
    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "SelectedIndexChanged"); 
        GetLeadSource(0);
    }
}
