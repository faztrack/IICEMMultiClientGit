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
using System.Collections.Generic;
using System.Web.Services;

public partial class VendorList : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetVendorName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["vSearch"] != null)
        {
            List<Vendor> cList = (List<Vendor>)HttpContext.Current.Session["vSearch"];
            return (from c in cList
                    where c.vendor_name.ToLower().StartsWith(prefixText.ToLower())
                    select c.vendor_name).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.Vendors
                    where c.vendor_name.StartsWith(prefixText)
                    select c.vendor_name).Take<String>(count).ToArray();
        }
    }
    [WebMethod]
    public static string[] GetPhone(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["vSearch"] != null)
        {
            List<Vendor> cList = (List<Vendor>)HttpContext.Current.Session["vSearch"];
            return (from c in cList
                    where c.phone.ToLower().StartsWith(prefixText.ToLower())
                    select c.phone).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.Vendors
                    where c.phone.StartsWith(prefixText)
                    select c.phone).Take<String>(count).ToArray();
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
            DataClassesDataContext _db = new DataClassesDataContext();
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin013") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            List<Vendor> VendorList = _db.Vendors.ToList();
            Session.Add("vSearch", VendorList);

            BindSection();

            GetVendor(0);
        }
    }

    private void BindSection()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        List<sectioninfo> SectionList = _db.sectioninfos.Where(si => si.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && si.parent_id == 0).OrderBy(s => s.section_name).ToList();


        ddlSection.DataSource = SectionList;
        ddlSection.DataTextField = "section_name";
        ddlSection.DataValueField = "section_id";
        ddlSection.DataBind();
        ddlSection.Items.Insert(0, "All");
        ddlSection.SelectedIndex = 0;

    }

    protected void GetVendor(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        grdVendorList.PageIndex = nPageNo;

        //var item = from u in _db.Vendors
        //           where u.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
        //           orderby u.vendor_name ascending
        //           select u;

        //if (txtSearch.Text.Trim() != "")
        //{
        //    string str = txtSearch.Text.Trim();

        //    if (ddlSearchBy.SelectedValue == "1") // First Name
        //    {
        //        item = from uinfo in _db.Vendors
        //               where uinfo.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && uinfo.phone.Contains(str)
        //               orderby  uinfo.vendor_name ascending
        //               select uinfo;
        //    }
        //    else if (ddlSearchBy.SelectedValue == "2") // Last Name
        //    {
        //        item = from uinfo in _db.Vendors
        //               where uinfo.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && uinfo.vendor_name.Contains(str)
        //               orderby uinfo.vendor_name ascending
        //               select uinfo;
        //    }
        //}

        string strCondition = string.Empty;
        if (Convert.ToInt32(ddlStatus.SelectedValue) == 1)
        {
            strCondition = "WHERE is_active = 1 ";
        }
        else if (Convert.ToInt32(ddlStatus.SelectedValue) == 2)
        {
            strCondition = "WHERE is_active = 0 ";

        }

        if (txtSearch.Text.Trim() != "")
        {
            string str = txtSearch.Text.Trim();

            str = str.Replace("'", "''");

            if (ddlSearchBy.SelectedValue == "1") // Phone
            {
                if (strCondition.Length > 2)
                    strCondition += "AND phone LIKE '%" + str + "%'";
                else
                    strCondition = "WHERE phone LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "2") // Vendor
            {
                if (strCondition.Length > 2)
                    strCondition += "AND vendor_name LIKE '%" + str + "%'";
                else
                    strCondition = "WHERE vendor_name LIKE '%" + str + "%'";

            }
            else if (ddlSearchBy.SelectedValue == "3") // section
            {
                if (strCondition.Length > 2)
                    strCondition += "AND vs.SectionName LIKE '%" + str + "%'";
                else
                    strCondition = "WHERE vs.SectionName LIKE '%" + str + "%'";

            }
        }

        if (ddlSection.SelectedValue != "All") // section
        {
            if (strCondition.Length > 2)
                strCondition += "AND vs.SectionName = '" + ddlSection.SelectedItem.Text + "'";
            else
                strCondition = "WHERE vs.SectionName = '" + ddlSection.SelectedItem.Text + "'";

        }

        string StrQ = " SELECT DISTINCT v.vendor_id, v.client_id, v.vendor_name, v.Address, v.city, v.state, v.zip_code, v.phone, v.fax, v.email, v.is_active, '' AS section " +
                      "  FROM Vendor AS v " +
                      " LEFT OUTER JOIN vendor_section vs on vs.vendor_id = v.vendor_id " +
                      " " + strCondition + " order by v.vendor_name asc";

        DataTable dtVendor = csCommonUtility.GetDataTable(StrQ);

        //if (ddlItemPerPage.SelectedValue != "4")
        //{
        //    grdVendorList.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
        //}
        //else
        //{
        // grdVendorList.PageSize = 200;
        //}
        grdVendorList.PageSize = 20;

        grdVendorList.DataSource = dtVendor;
        grdVendorList.DataKeyNames = new string[] { "vendor_id", "Address", "city", "state", "zip_code", "is_active" };
        grdVendorList.DataBind();
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

        if (grdVendorList.PageCount == nPageNo + 1)
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
        Response.Redirect("VendorDetails.aspx");
    }
    protected void grdVendorList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int Vid = Convert.ToInt32(grdVendorList.DataKeys[e.Row.RowIndex].Values[0]);
            string address = grdVendorList.DataKeys[e.Row.RowIndex].Values[1].ToString();
            string city = grdVendorList.DataKeys[e.Row.RowIndex].Values[2].ToString();
            string state = grdVendorList.DataKeys[e.Row.RowIndex].Values[3].ToString();
            string zip = grdVendorList.DataKeys[e.Row.RowIndex].Values[4].ToString();
            bool bAcitve = Convert.ToBoolean(grdVendorList.DataKeys[e.Row.RowIndex].Values[5]);
            if (bAcitve)
            {
                e.Row.Cells[5].Text = "Yes";
            }
            else
            {
                e.Row.Cells[5].Text = "No";
            }
            //  Address

            string strSecion = "";

            if (_db.vendor_sections.Any(v => v.Vendor_Id == Vid))
            {
                var item = _db.vendor_sections.Where(v => v.Vendor_Id == Vid).OrderBy(s => s.SectionName).ToList();

                foreach (vendor_section v in item)
                {
                    strSecion += v.SectionName.ToString() + ", ";
                }
                strSecion = strSecion.Trim().TrimEnd(',');

                e.Row.Cells[4].Text = strSecion;
            }

            string strAddress = address + "</br>" + city + ", " + state + " " + zip;
            e.Row.Cells[1].Text = strAddress;
        }
    }
    protected void grdVendorList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdVendorList.ID, grdVendorList.GetType().Name, "PageIndexChanging"); 
        GetVendor(e.NewPageIndex);
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        GetVendor(0);
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetVendor(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetVendor(nCurrentPage - 2);
    }
    //protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    GetVendor(0);
    //}
    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetVendor(0);
    }

    protected void ddlSection_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetVendor(0);
    }

    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "SelectedIndexChanged"); 
        txtSearch.Text = "";
        wtmFileNumber.WatermarkText = "Search by " + ddlSearchBy.SelectedItem.Text;

        if (ddlSearchBy.SelectedValue == "1")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetVendorName";
        }
        else if (ddlSearchBy.SelectedValue == "2")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetPhone";
        }

        GetVendor(0);
    }
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        ddlStatus.SelectedValue = "0";
        ddlSection.SelectedValue = "All";
        GetVendor(0);
    }
}