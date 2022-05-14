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

public partial class leadstatus_list : System.Web.UI.Page
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
            if (Page.User.IsInRole("admin040") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            GetLeadStatus(0);
        }
    }

    protected void GetLeadStatus(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        grdLeadStatus.PageIndex = nPageNo;

        var item = from l in _db.lead_status
                   where  l.lead_status_id > 7 && l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                   orderby l.lead_status_name
                   select l;

        if (txtSearch.Text.Trim() != "")
        {
            string str = txtSearch.Text.Trim();
            item = from lead in _db.lead_status
                   where lead.lead_status_id > 7 && lead.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && lead.lead_status_name.Contains(str)
                   orderby lead.lead_status_name
                   select lead;
        }
        if (ddlItemPerPage.SelectedValue != "4")
        {
            grdLeadStatus.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
        }
        else
        {
            grdLeadStatus.PageSize = 200;
        }
        grdLeadStatus.DataSource = item;
        grdLeadStatus.DataBind();

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

        if (grdLeadStatus.PageCount == nPageNo + 1)
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
    protected void grdLeadStatus_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdLeadStatus.ID, grdLeadStatus.GetType().Name, "PageIndexChanging"); 
        GetLeadStatus(e.NewPageIndex);
    }
    protected void grdLeadStatus_RowDataBound(object sender, GridViewRowEventArgs e)
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
        GetLeadStatus(0);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetLeadStatus(nCurrentPage - 2);
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNext.ID, btnNext.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetLeadStatus(nCurrentPage);
    }
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        Response.Redirect("leadstatusdetails.aspx");
    }
    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "SelectedIndexChanged"); 
        GetLeadStatus(0);
    }
}
