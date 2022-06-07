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
using System.Drawing;
using System.Web.Services;
using System.Collections.Generic;

public partial class incentive_list : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetIncentiveName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["iSearch"] != null)
        {
            List<incentive> cList = (List<incentive>)HttpContext.Current.Session["iSearch"];
            return (from c in cList
                    where c.incentive_name.ToLower().StartsWith(prefixText.ToLower())
                    select c.incentive_name).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.incentives
                    where c.incentive_name.StartsWith(prefixText)
                    select c.incentive_name).Distinct().Take<String>(count).ToArray();
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
            string divisionName = "";
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                userinfo oUser = (userinfo)Session["oUser"];
                hdnClientId.Value = oUser.client_id.ToString();
                hdnPrimaryDivision.Value = oUser.primaryDivision.ToString();
                divisionName = oUser.divisionName;
            }
            if (Page.User.IsInRole("admin005") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            DataClassesDataContext _db = new DataClassesDataContext();
            List<incentive> iList = _db.incentives.ToList();
            Session.Add("iSearch", iList);

            BindDivision();
            if (divisionName != "" && divisionName.Contains(","))
            {
                ddlDivision.Enabled = true;
            }
            else
            {
                ddlDivision.Enabled = false;
            }
            GetIncentives(0);
        }
    }

    private void BindDivision()
    {

        string sql = "select id, division_name from division order by division_name";
        DataTable dt = csCommonUtility.GetDataTable(sql);
        ddlDivision.DataSource = dt;
        ddlDivision.DataTextField = "division_name";
        ddlDivision.DataValueField = "id";
        ddlDivision.DataBind();
        ddlDivision.Items.Insert(0, "All"); 
        ddlDivision.SelectedValue = hdnPrimaryDivision.Value;

    }

    protected void GetIncentives(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        lblResult.Text = "";

        

        grdIncentive.PageIndex = nPageNo;
               

        string condition = "";


        if (txtSearch.Text.Trim() != "")
        {
            condition += " where incentive_name in ('" + txtSearch.Text.Trim() + "') ";
        }


        if (ddlDivision.SelectedItem.Text != "All")
        {
            if (condition.Length > 2)
                condition += " AND division_name like '%" + ddlDivision.SelectedItem.Text.Trim() + "%' ";
            else
                condition = " WHERE  division_name like '%" + ddlDivision.SelectedItem.Text.Trim() + "%' ";
        }




        string sql = "select * from incentives " + condition;
        DataTable items = csCommonUtility.GetDataTable(sql);

        

        if (ddlItemPerPage.SelectedValue != "4")
        {
            grdIncentive.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
        }
        else
        {
            grdIncentive.PageSize = 200;
        }
        grdIncentive.DataSource = items;
        grdIncentive.DataKeyNames = new string[] { "incentive_type", "discount", "amount", "client_id" };
        grdIncentive.DataBind();

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

        if (grdIncentive.PageCount == nPageNo + 1)
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
    protected void grdIncentive_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdIncentive.ID, grdIncentive.GetType().Name, "PageIndexChanging"); 
        GetIncentives(e.NewPageIndex);
    }
    protected void grdIncentive_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int incentiveType = Convert.ToInt32(grdIncentive.DataKeys[e.Row.RowIndex].Values[0]);
            string discount = grdIncentive.DataKeys[e.Row.RowIndex].Values[1].ToString();
            string amount = grdIncentive.DataKeys[e.Row.RowIndex].Values[2].ToString();

            string clientId = grdIncentive.DataKeys[e.Row.RowIndex].Values[3].ToString();
            Label lblDivision = e.Row.FindControl("lblDivision") as Label;

            lblDivision.Text = csCommonUtility.GetDivisionName(clientId);

            Label lblDiscount = (Label)e.Row.FindControl("lblDiscount");
            if (incentiveType == 1)
            {
                lblDiscount.Text = discount + "%";
            }
            else
            {
                lblDiscount.Text = "$" + amount;
            }

            if (Convert.ToBoolean(e.Row.Cells[4].Text) == true)
                e.Row.Cells[4].Text = "Yes";
            else
                e.Row.Cells[4].Text = "No";
        }
    }
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        Response.Redirect("incentivedetails.aspx");
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        lblResult.Text = "";
        GetIncentives(0);
    }
    protected void chkIsActive_click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkIsActive.ID, chkIsActive.GetType().Name, "Click"); 
        try
        {
            if (ddlDivision.SelectedItem.Text != "All")
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                company_profile objComp = new company_profile();

                objComp = _db.company_profiles.SingleOrDefault(c => c.client_id == Convert.ToInt32(ddlDivision.SelectedValue));

                objComp.IsIncentiveActive = chkIsActive.Checked;
                _db.SubmitChanges();
                lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");
            }
           
            
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetIncentives(nCurrentPage - 2);
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNext.ID, btnNext.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetIncentives(nCurrentPage);
    }

    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "SelectedIndexChanged"); 
        GetIncentives(0);
    }

    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        ddlDivision.SelectedIndex = 0;
        ddlDivision.SelectedValue = hdnPrimaryDivision.Value;
        GetIncentives(0);
    }

    protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        if(ddlDivision.SelectedItem.Text != "All")
        {
            company_profile objComp = new company_profile();
            objComp = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(ddlDivision.SelectedValue));
            // Is Incentive Active/Inactive
            chkIsActive.Checked = Convert.ToBoolean(objComp.IsIncentiveActive);
        }
        else
        {
            chkIsActive.Checked = false;
        }

        GetIncentives(0);
    }
}
