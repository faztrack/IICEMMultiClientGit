using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using DataStreams.Csv;

public partial class mvendorlist : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetVendorName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["vSearch"] != null)
        {
            List<Vendor> cList = (List<Vendor>)HttpContext.Current.Session["vSearch"];
            return (from c in cList
                    where c.vendor_name.ToLower().Contains(prefixText.ToLower())
                    select c.vendor_name).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.Vendors
                    where c.vendor_name.Contains(prefixText)
                    select c.vendor_name).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetSection(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["vSearch"] != null)
        {
            List<sectioninfo> cList = (List<sectioninfo>)HttpContext.Current.Session["vSearch"];
            return (from c in cList
                    where c.section_name.ToLower().Contains(prefixText.ToLower())
                    select c.section_name).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.Vendors
                    where c.vendor_name.Contains(prefixText)
                    select c.vendor_name).Take<String>(count).ToArray();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect("mobile.aspx");
            }
            if (Page.User.IsInRole("cus001") == false)
            {
                // No Permission Page.
               // Response.Redirect("mobile.aspx");
            }
           
            

            GetVendorList();
            
            GetVendor(0);
        }
    }

    private void GetVendorList()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        List<Vendor> VendorList = _db.Vendors.ToList();
        Session.Add("vSearch", VendorList);
    }

   

    protected void GetVendor(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        grdVendorList.PageIndex = nPageNo;

      
        string strCondition = string.Empty;

        strCondition = " WHERE v.is_active=1";
        if (txtSearch.Text.Trim() != "")
        {
            string str = txtSearch.Text.Trim();

            str = str.Replace("'", "''");
            if (ddlSearchBy.SelectedValue == "1") // Vendor
            {
                if (strCondition.Length > 2)
                    strCondition += " AND vendor_name LIKE '%" + str + "%'";
                else
                    strCondition = " WHERE vendor_name LIKE '%" + str + "%'";

            }
            else if (ddlSearchBy.SelectedValue == "2") // section
            {
                if (strCondition.Length > 2)
                    strCondition += " AND vs.SectionName LIKE '%" + str + "%'";
                else
                    strCondition = " WHERE vs.SectionName LIKE '%" + str + "%'";

            }
        }
        
        string StrQ = " SELECT DISTINCT v.vendor_id, v.client_id, v.vendor_name, v.Address, v.city, v.state, v.zip_code, v.phone, v.fax, v.email, v.is_active, '' AS section " +
                      "  FROM Vendor AS v " +
                      " LEFT OUTER JOIN vendor_section vs on vs.vendor_id = v.vendor_id " +
                      " " + strCondition + " order by v.vendor_name asc";

        DataTable dtVendor = csCommonUtility.GetDataTable(StrQ);

        grdVendorList.PageSize = 20;

        grdVendorList.DataSource = dtVendor;
        grdVendorList.DataKeyNames = new string[] { "vendor_id", "Address", "city", "state", "zip_code", "phone", "vendor_name", "email", "fax" };
        grdVendorList.DataBind();
        
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
            string phone = grdVendorList.DataKeys[e.Row.RowIndex].Values[5].ToString();
            string vendor_name = grdVendorList.DataKeys[e.Row.RowIndex].Values[6].ToString();
            string email = grdVendorList.DataKeys[e.Row.RowIndex].Values[7].ToString();
            string fax = grdVendorList.DataKeys[e.Row.RowIndex].Values[8].ToString();
            string strAddress = address + "</br>" + city + ", " + state + " " + zip;

           // HyperLink hypVendorName = (HyperLink)e.Row.FindControl("hypVendorName");
           // hypVendorName.Text = vendor_name;
          //  hypVendorName.NavigateUrl = "mcustomer_details.aspx?cid=" + Vid;
            Label hypVendorName = (Label)e.Row.FindControl("hypVendorName");
            hypVendorName.Text = vendor_name;

            HyperLink hypPhone = (HyperLink)e.Row.FindControl("hypPhone");
            hypPhone.NavigateUrl = "tel:" + phone;
            hypPhone.Text = phone;

            HyperLink hypEmail = (HyperLink)e.Row.FindControl("hypEmail");
            hypEmail.NavigateUrl = "mailto:" + email;
            hypEmail.Text = email;

            HyperLink hypAddress = (HyperLink)e.Row.FindControl("hypAddress");
            hypAddress.Text = strAddress;


            string address2 = address + ",+" + city + ",+" + state + ",+" + zip;
            hypAddress.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address2;
            hypAddress.ToolTip = "Google Map";

            string strSecion = "";

            Label lblSection = (Label)e.Row.FindControl("lblSection");
            

            if (_db.vendor_sections.Any(v => v.Vendor_Id == Vid))
            {
                var item = _db.vendor_sections.Where(v => v.Vendor_Id == Vid).OrderBy(s => s.SectionName).ToList();

                foreach (vendor_section v in item)
                {
                    strSecion += v.SectionName.ToString() + ", ";
                }
                strSecion = strSecion.Trim().TrimEnd(',');

                //e.Row.Cells[4].Text = strSecion;
            }
            if(strSecion.Length>0)
               lblSection.Text = "<font style='font-weight:bold'>Sections:</font> "+strSecion;

            LinkButton inkViewSalesRep = (LinkButton)e.Row.FindControl("inkViewSalesRep");
            if (_db.vendorDetails.Any(v => v.vendor_id == Vid))
            {
                inkViewSalesRep.Visible = true;
                inkViewSalesRep.Text = "Contact Details";
                inkViewSalesRep.CommandArgument = Vid.ToString();
            }
            else
                inkViewSalesRep.Visible = false;
          
        }
    }

    protected void viewDetails(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        try
        {
            LinkButton InkView = (LinkButton)sender;
            int nVendorId = Convert.ToInt32(InkView.CommandArgument);
            var list = (from v in _db.vendorDetails   where v.vendor_id==nVendorId  select v).ToList();
            grdVendorDetails.PageSize = 20;
            grdVendorDetails.DataSource = list;
            grdVendorDetails.DataKeyNames = new string[] { "vendorDetailId","vendor_id","FirstName","LastName", "Phone", "Email" };
            grdVendorDetails.DataBind();
            
           ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void grdVendorDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int nVdid = Convert.ToInt32(grdVendorDetails.DataKeys[e.Row.RowIndex].Values[0]);
            int nVid = Convert.ToInt32(grdVendorDetails.DataKeys[e.Row.RowIndex].Values[1]);
            string firstName = grdVendorDetails.DataKeys[e.Row.RowIndex].Values[2].ToString();
            string lastName = grdVendorDetails.DataKeys[e.Row.RowIndex].Values[3].ToString();
            string phone = grdVendorDetails.DataKeys[e.Row.RowIndex].Values[4].ToString();
            string email = grdVendorDetails.DataKeys[e.Row.RowIndex].Values[5].ToString();



            Label hypSalesRep = (Label)e.Row.FindControl("hypSalesRep");
            hypSalesRep.Text = firstName + " " + lastName;

            HyperLink hypPhone = (HyperLink)e.Row.FindControl("hypVPhone");
            hypPhone.NavigateUrl = "tel:" + phone;
            hypPhone.Text = phone;

            HyperLink hypEmail = (HyperLink)e.Row.FindControl("hypVEmail");
            hypEmail.NavigateUrl = "mailto:" + email;
            hypEmail.Text = email;


           

        }
    }
    protected void grdVendorList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdVendorList.ID, grdVendorList.GetType().Name, "Click"); 
        GetVendor(e.NewPageIndex);
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        GetVendor(0);
    }
   

    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        GetVendorList();
        txtSearch.Text = "";
        ddlSearchBy.SelectedValue = "1";
        GetSearchBY();
        GetVendor(0);
    }
    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("mlandingpage.aspx");
    }
    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "SelectedIndexChanged"); 
        GetSearchBY();
    }

    private void GetSearchBY()
    {
        txtSearch.Text = "";
        wtmFileNumber.WatermarkText = "Search by " + ddlSearchBy.SelectedItem.Text;
        DataClassesDataContext _db = new DataClassesDataContext();
        if (ddlSearchBy.SelectedValue == "1")
        {
            List<Vendor> VendorList = _db.Vendors.ToList();
            Session.Add("vSearch", VendorList);
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetVendorName";
        }
        else if (ddlSearchBy.SelectedValue == "2")
        {
            List<sectioninfo> SectionList = _db.sectioninfos.Where(si => si.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && si.parent_id == 0).OrderBy(s => s.section_name).Distinct().ToList();
            Session.Add("vSearch", SectionList);
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetSection";
        }

        GetVendor(0);
    }
  
   
}
