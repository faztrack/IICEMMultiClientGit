using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class assigntomodelstimate : System.Web.UI.Page
{
    string strDetails = "";
    public static string strDetailsFull = "";
    private double subtotal = 0.0;
    private double grandtotal = 0.0;

    private double subtotal_diect = 0.0;
    private double grandtotal_direct = 0.0;
    [WebMethod]
    public static string[] GetItemName(String prefixText, Int32 count)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (HttpContext.Current.Session["cParent"] != null)
        {

            int nPId = Convert.ToInt32(HttpContext.Current.Session["cParent"]);
            return (from c in _db.sectioninfos
                    where c.section_name.Contains(prefixText) && c.parent_id == nPId
                    select c.section_name).Take<String>(count).ToArray();
        }
        else
        {

            return (from c in _db.sectioninfos
                    where c.section_name.Contains(prefixText)
                    select c.section_name).Take<String>(count).ToArray();
        }

    }
   

     [WebMethod]
    public static string[] GetCompany(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.company.ToLower().StartsWith(prefixText.ToLower())
                    select c.company).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.company.StartsWith(prefixText)
                    select c.company).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.last_name1.ToLower().StartsWith(prefixText.ToLower())
                    select c.last_name1).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.last_name1.StartsWith(prefixText)
                    select c.last_name1).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetFirstName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.first_name1.ToLower().StartsWith(prefixText.ToLower())
                    select c.first_name1).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.first_name1.StartsWith(prefixText)
                    select c.first_name1).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetAddress(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.address.ToLower().StartsWith(prefixText.ToLower())
                    select c.address).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.address.StartsWith(prefixText)
                    select c.address).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetEmail(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.email.ToLower().StartsWith(prefixText.ToLower())
                    select c.email).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.email.StartsWith(prefixText)
                    select c.email).Take<String>(count).ToArray();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
        if (!IsPostBack)
        {
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }

            Session.Remove("AddMoreMELocation");
            Session.Remove("AddMoreMESection");
            Session.Remove("gspSearch");

            if (Request.QueryString.Get("meid") != null)
                hdnEstimateId.Value = Request.QueryString.Get("meid");
            if (Request.QueryString.Get("locid") != null)
                hdnLocationId.Value = Request.QueryString.Get("locid");

            if (Request.QueryString.Get("type") != null)
               hdnType.Value = Request.QueryString.Get("type");

            userinfo obj = (userinfo)Session["oUser"];

            if (Request.QueryString.Get("spid") != null)
            {
                hdnSalesPersonId.Value = Convert.ToInt32(Request.QueryString.Get("spid")).ToString();
            }
            else
            {
                hdnSalesPersonId.Value = obj.sales_person_id.ToString();
            }


            DataClassesDataContext _db = new DataClassesDataContext();


            sales_person sp = new sales_person();
            sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
            if (sp != null)
            {
                lblSalesPersonName.Text = sp.first_name + " " + sp.last_name;
                lblAddress.Text = sp.address;
                lblPhone.Text = sp.phone;
                lblEmail.Text = sp.email;
            }
          
            model_estimate me = new model_estimate();
            me = _db.model_estimates.Single(mest => mest.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && mest.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

            lblModelEstimateName.Text = me.model_estimate_name;
            lblCreateDate.Text = Convert.ToDateTime(me.create_date).ToShortDateString();
            lblLastUpdatedDate.Text = Convert.ToDateTime(me.last_udated_date).ToShortDateString();
            lblNewEstimateName.Text = me.model_estimate_name;

            var item2 = from cus in _db.customers
                       where cus.status_id != 5 && cus.status_id != 4
                       select cus;
            if (Convert.ToInt32(hdnSalesPersonId.Value) > 0)
            {
                item2 = from cus in _db.customers
                       where cus.status_id != 5 && cus.status_id != 4 && cus.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value)
                       select cus;

            }
            List<customer> LeadList = item2.ToList();
            Session.Add("cSearch", LeadList);


            //var item = from loc in _db.locations
            //           join mel in _db.model_estimate_locations on loc.location_id equals mel.location_id
            //           where mel.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && mel.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mel.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
            //           select new LocationModel()
            //           {
            //               location_id = (int)mel.location_id,
            //               location_name = loc.location_name
            //           };

            //ddlCustomerLocations.DataSource = item;
            //ddlCustomerLocations.DataTextField = "location_name";
            //ddlCustomerLocations.DataValueField = "location_id";
            //ddlCustomerLocations.DataBind();
            //ddlCustomerLocations.Items.Insert(0, "Select Location");
            //ddlCustomerLocations.SelectedValue = "0";

            //var section = from sec in _db.sectioninfos
            //              join mes in _db.model_estimate_sections on sec.section_id equals mes.section_id
            //              where mes.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && mes.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mes.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
            //              select new SectionInfo()
            //              {
            //                  section_id = (int)mes.section_id,
            //                  section_name = sec.section_name
            //              };

            //DataTable dtSectionId = csCommonUtility.LINQToDataTable(section);

            //ddlSections.DataSource = section;
            //ddlSections.DataTextField = "section_name";
            //ddlSections.DataValueField = "section_id";
            //ddlSections.DataBind();
            //ddlSections.Items.Insert(0, "Select Section");
            //ddlSections.SelectedValue = "0";
            //ddlSections.Items.Insert(1, "All Selected Section");
            //ddlSections.SelectedValue = "-1";
            //int[] terms = new int[dtSectionId.Rows.Count]; ;
            //for (int i = 0; i < dtSectionId.Rows.Count; i++)
            //{
            //    terms[i] = Convert.ToInt32(dtSectionId.Rows[i]["section_id"]);
            //}
            //Session["myIds"] = terms;
            // DataTable dtNew = LoadFullSection();
            hdnSortDesc.Value = "0";
            BindSelectedItemGrid();
            BindSelectedItemGrid_Direct();
            if (grdGrouping.Rows.Count == 0)
            {
                lblRetailPricingHeader.Visible = false;
            }
            else
            {
                lblRetailPricingHeader.Visible = true;
            }
            if (grdGroupingDirect.Rows.Count == 0)
            {
                lblDirectPricingHeader.Visible = false;
            }
            else
            {
                lblDirectPricingHeader.Visible = true;
            }
            Calculate_Total();
            if (grdGrouping.Rows.Count == 0 && grdGroupingDirect.Rows.Count == 0)
            {
                rdoSort.Visible = false;
                //tblTotalProjectPrice.Visible = false;
                if (grdGrouping.Rows.Count == 0)
                {
                    lblRetailPricingHeader.Visible = false;
                }
                else
                {
                    lblRetailPricingHeader.Visible = true;
                }
                if (grdGroupingDirect.Rows.Count == 0)
                {
                    lblDirectPricingHeader.Visible = false;
                }
                else
                {
                    lblDirectPricingHeader.Visible = true;
                }
            }
            else
            {
                rdoSort.Visible = true;
                //tblTotalProjectPrice.Visible = true;
                if (grdGrouping.Rows.Count == 0)
                {
                    lblRetailPricingHeader.Visible = false;
                }
                else
                {
                    lblRetailPricingHeader.Visible = true;
                }
                if (grdGroupingDirect.Rows.Count == 0)
                {
                    lblDirectPricingHeader.Visible = false;
                }
                else
                {
                    lblDirectPricingHeader.Visible = true;
                }
            }
            //if (ddlSections.SelectedItem.Text == "Select Section")
            //{
            //    trvSection.Nodes.Clear();
            //}



            //csCommonUtility.SetPagePermission(this.Page, new string[] { "btnAssignToCustomer"});
            //csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "grdSelectedItem1_0_chkIsSelectedAll", "grdSelectedItem1_0_chkIsSelected", "", "", });

        }

    }
    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "SelectedIndexChanged"); 
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
        else if (ddlSearchBy.SelectedValue == "4")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetAddress";
        }
        else if (ddlSearchBy.SelectedValue == "3")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetEmail";
        }
        else if (ddlSearchBy.SelectedValue == "6")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetCompany";
        }

        GetCustomersNew(0);
    }

    
   
   
  
   


   
    
   
    
  




    private decimal GetRetailTotal()
    {
        decimal dRetail = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.model_estimate_pricings
                      where (from clc in _db.model_estimate_locations
                             where clc.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                             select clc.location_id).Contains(pd.location_id) &&
                             (from cs in _db.model_estimate_sections
                              where cs.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                              select cs.section_id).Contains(pd.section_level) && pd.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A"
                      select pd.total_retail_price);
        int n = result.Count();
        if (result != null && n > 0)
            dRetail = result.Sum();

        return dRetail;
    }
    private decimal GetDirctTotal()
    {
        decimal dDirect = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        var result = (from pd in _db.model_estimate_pricings
                      where (from clc in _db.model_estimate_locations
                             where clc.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                             select clc.location_id).Contains(pd.location_id) &&
                             (from cs in _db.model_estimate_sections
                              where cs.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                              select cs.section_id).Contains(pd.section_level) && pd.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A"
                      select pd.total_direct_price);
        int n = result.Count();
        if (result != null && n > 0)
            dDirect = result.Sum();

        return dDirect;
    }
    public void BindSelectedItemGrid()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "";
        if (rdoSort.SelectedValue == "2")
        {
            strQ = " select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from model_estimate_pricing where model_estimate_pricing.location_id IN (Select location_id from model_estimate_locations WHERE model_estimate_locations.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_locations.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND model_estimate_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND model_estimate_pricing.section_level IN (Select section_id from model_estimate_sections WHERE model_estimate_sections.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_sections.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND model_estimate_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND model_estimate_pricing.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_pricing.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND is_direct=1 AND model_estimate_pricing.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + "  order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT model_estimate_pricing.location_id AS colId,'LOCATION: '+ location.location_name as colName,sort_id from model_estimate_pricing  INNER JOIN location on location.location_id = model_estimate_pricing.location_id where model_estimate_pricing.location_id IN (Select location_id from model_estimate_locations WHERE model_estimate_locations.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_locations.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND model_estimate_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                " AND model_estimate_pricing.section_level IN (Select section_id from model_estimate_sections WHERE model_estimate_sections.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_sections.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND model_estimate_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                " AND model_estimate_pricing.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + "AND model_estimate_pricing.location_id =" + Convert.ToInt32(hdnLocationId.Value) + " AND model_estimate_pricing.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND is_direct=1 AND model_estimate_pricing.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " order by sort_id asc";
        }
        List<PricingMaster> mList = _db.ExecuteQuery<PricingMaster>(strQ, string.Empty).ToList();
        grdGrouping.DataSource = mList;
        grdGrouping.DataKeyNames = new string[] { "colId" };
        grdGrouping.DataBind();

    }
    public void BindSelectedItemGrid_Direct()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "";
        if (rdoSort.SelectedValue == "2")
        {
            strQ = "select DISTINCT section_level AS colId,'SECTION: '+ section_name as colName from model_estimate_pricing where model_estimate_pricing.location_id IN (Select location_id from model_estimate_locations WHERE model_estimate_locations.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_locations.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND model_estimate_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND model_estimate_pricing.section_level IN (Select section_id from model_estimate_sections WHERE model_estimate_sections.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_sections.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND model_estimate_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND model_estimate_pricing.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_pricing.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND is_direct=2 AND model_estimate_pricing.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + "  order by section_level asc";
        }
        else
        {
            strQ = "select DISTINCT model_estimate_pricing.location_id AS colId,'LOCATION: '+ location.location_name as colName,sort_id from model_estimate_pricing  INNER JOIN location on location.location_id = model_estimate_pricing.location_id where model_estimate_pricing.location_id IN (Select location_id from model_estimate_locations WHERE model_estimate_locations.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_locations.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND model_estimate_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND model_estimate_pricing.section_level IN (Select section_id from model_estimate_sections WHERE model_estimate_sections.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_sections.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND model_estimate_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                   " AND model_estimate_pricing.model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND model_estimate_pricing.sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND is_direct=2 AND model_estimate_pricing.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " order by sort_id asc";
        }
        List<PricingMaster> mList = _db.ExecuteQuery<PricingMaster>(strQ, string.Empty).ToList();
        grdGroupingDirect.DataSource = mList;
        grdGroupingDirect.DataKeyNames = new string[] { "colId" };
        grdGroupingDirect.DataBind();

    }
    public void Calculate_Total()
    {
        decimal direct = 0;
        decimal retail = 0;
        decimal grandtotal = 0;
        direct = GetDirctTotal();
        retail = GetRetailTotal();
        grandtotal = direct + retail;
        hdnProjectTotal.Value = grandtotal.ToString();
       

    }


   
  


   

    protected void grdSelectedItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblshort_notes = (Label)e.Row.FindControl("lblshort_notes");
            TextBox txtshort_notes = (TextBox)e.Row.FindControl("txtshort_notes");
            string str = lblshort_notes.Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 50)
            {
                txtshort_notes.Text = str;
                lblshort_notes.Text = str.Substring(0, 50) + "...";
                lblshort_notes.ToolTip = str;

            }
            else
            {
                txtshort_notes.Text = str;
                lblshort_notes.Text = str;

            }

        }

    }
   

    protected string GetTotalPrice()
    {
        return "Total: " + grandtotal.ToString("c");
    }
    protected string GetTotalPriceDirect()
    {
        return "Total: " + grandtotal_direct.ToString("c");
    }
    protected void grdGrouping_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGrouping.DataKeys[e.Row.RowIndex].Values[0]);
            
            LinkButton InkAssign = (LinkButton)e.Row.FindControl("InkAssign");
            if (rdoSort.SelectedValue == "1")
            {
                if (e.Row.RowIndex == 0)
                {
                    
                    //InkAssign.Visible = false;
                }
                else
                {
                   
                    //InkAssign.Visible = true;
                }
            }
            else
            {
               
                //InkAssign.Visible = false;

            }
            GridView gv = e.Row.FindControl("grdSelectedItem1") as GridView;
            int nDirectId = 1;
            GetData(colId, gv, nDirectId);
            GridViewRow footerRow = gv.FooterRow;
            GridViewRow headerRow = gv.HeaderRow;
            foreach (GridViewRow row in gv.Rows)
            {
                Label labelTotal = footerRow.FindControl("lblSubTotal") as Label;
                Label lblSubTotalLabel = footerRow.FindControl("lblSubTotalLabel") as Label;
                Label lblHeader = headerRow.FindControl("lblHeader") as Label;
                subtotal += Double.Parse((row.FindControl("lblTotal_price") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                labelTotal.Text = subtotal.ToString("c");
                if (rdoSort.SelectedValue == "1")
                {
                    lblHeader.Text = "Section";
                }
                else
                {
                    lblHeader.Text = "Location";
                }
                lblSubTotalLabel.Text = "Sub Total:";
            }
            grandtotal += subtotal;
            subtotal = 0.0;
        }
    }
    private void GetData(int colId, GridView grd, int nDirectId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        if (rdoSort.SelectedValue == "1")
        {
            var price_detail = from p in _db.model_estimate_pricings
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.model_estimate_locations
                                      where clc.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.model_estimate_sections
                                       where cs.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                       select cs.section_id).Contains(p.section_level) && p.location_id == colId && p.is_direct == nDirectId && p.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && p.pricing_type == "A"
                               orderby p.section_level ascending

                               select new PricingDetailModel()
                               {
                                   pricing_id = (int)p.me_pricing_id,
                                   item_id = (int)p.item_id,
                                   labor_id = (int)p.labor_id,
                                   section_serial = (decimal)p.section_serial,
                                   location_name = lc.location_name,
                                   section_name = p.section_name,
                                   item_name = p.item_name,
                                   measure_unit = p.measure_unit,
                                   item_cost = (decimal)p.item_cost,
                                   total_retail_price = (decimal)p.total_retail_price,
                                   total_direct_price = (decimal)p.total_direct_price,
                                   minimum_qty = (decimal)p.minimum_qty,
                                   quantity = (decimal)p.quantity,
                                   retail_multiplier = (decimal)p.retail_multiplier,
                                   labor_rate = (decimal)p.labor_rate,
                                   short_notes = p.short_notes,
                                   tmpCol = string.Empty,
                                   pricing_type = "A",
                                   last_update_date = (DateTime)p.last_updated_date
                               };
            if (hdnSortDesc.Value == "1")
                grd.DataSource = price_detail.ToList().OrderByDescending(c => c.last_update_date);
            else
                grd.DataSource = price_detail.ToList();
            grd.DataKeyNames = new string[] { "pricing_id", "item_id" };
            grd.DataBind();
        }
        else
        {
            var price_detail = from p in _db.model_estimate_pricings
                               join lc in _db.locations on p.location_id equals lc.location_id
                               where (from clc in _db.model_estimate_locations
                                      where clc.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                      select clc.location_id).Contains(p.location_id) &&
                                      (from cs in _db.model_estimate_sections
                                       where cs.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                       select cs.section_id).Contains(p.section_level)
                                      && p.section_level == colId && p.is_direct == nDirectId && p.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && p.pricing_type == "A"
                               orderby lc.location_name ascending

                               select new PricingDetailModel()
                               {
                                   pricing_id = (int)p.me_pricing_id,
                                   item_id = (int)p.item_id,
                                   labor_id = (int)p.labor_id,
                                   section_serial = (decimal)p.section_serial,
                                   location_name = p.section_name,
                                   section_name = lc.location_name,
                                   item_name = p.item_name,
                                   measure_unit = p.measure_unit,
                                   item_cost = (decimal)p.item_cost,
                                   total_retail_price = (decimal)p.total_retail_price,
                                   total_direct_price = (decimal)p.total_direct_price,
                                   minimum_qty = (decimal)p.minimum_qty,
                                   quantity = (decimal)p.quantity,
                                   retail_multiplier = (decimal)p.retail_multiplier,
                                   labor_rate = (decimal)p.labor_rate,
                                   short_notes = p.short_notes,
                                   tmpCol = string.Empty,
                                   pricing_type = "A",
                                   last_update_date = (DateTime)p.last_updated_date
                               };
            if (hdnSortDesc.Value == "1")
                grd.DataSource = price_detail.ToList().OrderByDescending(c => c.last_update_date);
            else
                grd.DataSource = price_detail.ToList();
            grd.DataKeyNames = new string[] { "pricing_id", "item_id" };
            grd.DataBind();
        }


    }
    protected void rdoSort_SelectedIndexChanged(object sender, EventArgs e)
    {


        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdoSort.ID, rdoSort.GetType().Name, "SelectedIndexChanged"); 
        lblResult1.Text = "";
        hdnSortDesc.Value = "0";
        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();
        if (grdGrouping.Rows.Count == 0)
        {
            lblRetailPricingHeader.Visible = false;
        }
        else
        {
            lblRetailPricingHeader.Visible = true;
        }
        if (grdGroupingDirect.Rows.Count == 0)
        {
            lblDirectPricingHeader.Visible = false;
        }
        else
        {
            lblDirectPricingHeader.Visible = true;
        }
        Calculate_Total();
    }
    protected void grdSelectedItem2_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
       
       
        lblResult1.Text = "";

        GridView grdSelectedItem2 = (GridView)sender;
        DataClassesDataContext _db = new DataClassesDataContext();
        hdnPricingId.Value = grdSelectedItem2.DataKeys[e.RowIndex].Values[0].ToString();
        string strQ = "Delete model_estimate_pricing WHERE me_pricing_id =" + Convert.ToInt32(hdnPricingId.Value) + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        _db.ExecuteCommand(strQ, string.Empty);
        hdnSortDesc.Value = "0";
        BindSelectedItemGrid_Direct();
        BindSelectedItemGrid();
        if (grdGrouping.Rows.Count == 0)
        {
            lblRetailPricingHeader.Visible = false;
        }
        else
        {
            lblRetailPricingHeader.Visible = true;
        }
        if (grdGroupingDirect.Rows.Count == 0)
        {
            lblDirectPricingHeader.Visible = false;
        }
        else
        {
            lblDirectPricingHeader.Visible = true;
        }
        Calculate_Total();
        hdnPricingId.Value = "0";
        lblResult1.Text = csCommonUtility.GetSystemMessage("Item deleted successfully");
        
    }

    protected void grdSelectedItem2_RowEditing(object sender, GridViewEditEventArgs e)
    {
       
    
        lblResult1.Text = "";

        GridView grdSelectedItem2 = (GridView)sender;
        TextBox txtquantity1 = (TextBox)grdSelectedItem2.Rows[e.NewEditIndex].FindControl("txtquantity1");
        Label lblquantity1 = (Label)grdSelectedItem2.Rows[e.NewEditIndex].FindControl("lblquantity1");
        TextBox txtshort_notes1 = (TextBox)grdSelectedItem2.Rows[e.NewEditIndex].FindControl("txtshort_notes1");
        Label lblshort_notes1 = (Label)grdSelectedItem2.Rows[e.NewEditIndex].FindControl("lblshort_notes1");
        txtquantity1.Visible = true;
        lblquantity1.Visible = false;
        txtshort_notes1.Visible = true;
        lblshort_notes1.Visible = false;
        LinkButton btn = (LinkButton)grdSelectedItem2.Rows[e.NewEditIndex].Cells[10].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";
    }
    protected void grdSelectedItem2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblshort_notes1 = (Label)e.Row.FindControl("lblshort_notes1");
            TextBox txtshort_notes1 = (TextBox)e.Row.FindControl("txtshort_notes1");
            string str = lblshort_notes1.Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 50)
            {
                txtshort_notes1.Text = str;
                lblshort_notes1.Text = str.Substring(0, 50) + "...";
                lblshort_notes1.ToolTip = str;

            }
            else
            {
                txtshort_notes1.Text = str;
                lblshort_notes1.Text = str;

            }

        }
    }
    protected void grdGroupingDirect_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGroupingDirect.DataKeys[e.Row.RowIndex].Values[0]);
            int nDirectId = 2;
            LinkButton lnkMove2 = (LinkButton)e.Row.FindControl("lnkMove2");
            if (rdoSort.SelectedValue == "1")
            {
                if (e.Row.RowIndex == 0)
                {
                    lnkMove2.Visible = false;
                }
                else
                {
                    lnkMove2.Visible = true;

                }
            }
            else
            {
                lnkMove2.Visible = false;

            }

            GridView gv = e.Row.FindControl("grdSelectedItem2") as GridView;
            GetData(colId, gv, nDirectId);
            GridViewRow footerRow = gv.FooterRow;
            GridViewRow headerRow = gv.HeaderRow;
            foreach (GridViewRow row in gv.Rows)
            {
                Label labelTotal2 = footerRow.FindControl("lblSubTotal2") as Label;
                Label lblSubTotalLabel2 = footerRow.FindControl("lblSubTotalLabel2") as Label;
                Label lblHeader2 = headerRow.FindControl("lblHeader2") as Label;
                subtotal_diect += Double.Parse((row.FindControl("lblTotal_price2") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                labelTotal2.Text = subtotal_diect.ToString("c");
                if (rdoSort.SelectedValue == "1")
                {
                    lblHeader2.Text = "Section";
                }
                else
                {
                    lblHeader2.Text = "Location";
                }
                lblSubTotalLabel2.Text = "Sub Total:";
            }
            grandtotal_direct += subtotal_diect;
            subtotal_diect = 0.0;
        }
    }
   
    private void GridRowUpdatingNonDirect(GridView gvr, int nIndex)
    {
        if (nIndex < 0) return;

    
  
        lblResult1.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        TextBox txtquantity = (TextBox)gvr.Rows[nIndex].FindControl("txtquantity");
        Label lblTotal_price = (Label)gvr.Rows[nIndex].FindControl("lblTotal_price");
        TextBox txtshort_notes = (TextBox)gvr.Rows[nIndex].FindControl("txtshort_notes");
        int nPricingId = Convert.ToInt32(gvr.DataKeys[Convert.ToInt32(nIndex)].Values[0]);
        decimal dTotalPrice = 0;
        try
        {
            dTotalPrice = dTotalPrice = Convert.ToDecimal(lblTotal_price.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        }
        catch
        {
            dTotalPrice = 0;

        }

        decimal nQty = 0;
        try
        {
            nQty = Convert.ToDecimal(txtquantity.Text);
        }
        catch
        {
            nQty = 1;

        }
        string strQ = "UPDATE model_estimate_pricing SET quantity=" + nQty + " ,total_retail_price=" + dTotalPrice + " ,short_notes='" + txtshort_notes.Text.Replace("'", "''") + "' WHERE me_pricing_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        _db.ExecuteCommand(strQ, string.Empty);


        subtotal = 0.0;
        subtotal_diect = 0.0;
        hdnSortDesc.Value = "0";
        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();
        if (grdGrouping.Rows.Count == 0)
        {
            lblRetailPricingHeader.Visible = false;
        }
        else
        {
            lblRetailPricingHeader.Visible = true;
        }
        if (grdGroupingDirect.Rows.Count == 0)
        {
            lblDirectPricingHeader.Visible = false;
        }
        else
        {
            lblDirectPricingHeader.Visible = true;
        }
        hdnPricingId.Value = "0";
        Calculate_Total();
        lblResult1.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
        


    }


    protected void grdSelectedItem2_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridView gvr = (GridView)sender;
        GridRowUpdatingDirect(gvr, e.RowIndex);
        #region BlockCode

        #endregion

    }

    private void GridRowUpdatingDirect(GridView gvr, int nIndex)
    {
        if (nIndex < 0) return;

      
     
        lblResult1.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();
        TextBox txtquantity1 = (TextBox)gvr.Rows[nIndex].FindControl("txtquantity1");
        Label lblTotal_price2 = (Label)gvr.Rows[nIndex].FindControl("lblTotal_price2");
        TextBox txtshort_notes1 = (TextBox)gvr.Rows[nIndex].FindControl("txtshort_notes1");
        int nPricingId = Convert.ToInt32(gvr.DataKeys[Convert.ToInt32(nIndex)].Values[0]);

        decimal dTotalDirectPrice = 0;
        try
        {
            dTotalDirectPrice = Convert.ToDecimal(lblTotal_price2.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        }
        catch
        {
            dTotalDirectPrice = 0;

        }

        decimal nQty = 0;
        try
        {
            nQty = Convert.ToDecimal(txtquantity1.Text);
        }
        catch
        {
            nQty = 1;

        }
        string strQ = "UPDATE model_estimate_pricing SET quantity=" + nQty + " ,total_direct_price=" + dTotalDirectPrice + " ,short_notes='" + txtshort_notes1.Text.Replace("'", "''") + "' WHERE me_pricing_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        _db.ExecuteCommand(strQ, string.Empty);


        subtotal = 0.0;
        subtotal_diect = 0.0;
        hdnSortDesc.Value = "0";
        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();
        if (grdGrouping.Rows.Count == 0)
        {
            lblRetailPricingHeader.Visible = false;
        }
        else
        {
            lblRetailPricingHeader.Visible = true;
        }
        if (grdGroupingDirect.Rows.Count == 0)
        {
            lblDirectPricingHeader.Visible = false;
        }
        else
        {
            lblDirectPricingHeader.Visible = true;
        }
        hdnPricingId.Value = "0";
        Calculate_Total();

        lblResult1.Text = csCommonUtility.GetSystemMessage("Item updated successfully");
      


    }
    protected void btnSave_Click(object sender, EventArgs e)
    {

        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        lblResult1.Text = "";

        model_estimate obj = new model_estimate();
        DataClassesDataContext _db = new DataClassesDataContext();
        obj = _db.model_estimates.Single(me => me.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && me.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && me.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        obj.estimate_comments = txtComments.Text;
         obj.last_udated_date = DateTime.Now;
        _db.SubmitChanges();

        lblResult1.Text = csCommonUtility.GetSystemMessage(lblModelEstimateName.Text + " updated successfully.");

    }
 


   



    protected void NonDirect_calculation(object sender, EventArgs e)
    {
        GridView grvFind = null;
        TextBox txt = (TextBox)sender;
        int nIndex = -1;
        foreach (GridViewRow dimaster1 in grdGrouping.Rows)
        {
            int i = -1;
            GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
            foreach (GridViewRow diitem in grdSelectedItem1.Rows)
            {
                i++;
                TextBox txtquantity = (TextBox)diitem.FindControl("txtquantity");
                Label lblquantity = (Label)diitem.FindControl("lblquantity");
                Label lblTotal_price = (Label)diitem.FindControl("lblTotal_price");


                if (txtquantity.Text.Trim() == "")
                {
                    lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Quantity.");
                  
                    return;
                }
                else
                {
                    try
                    {
                        Convert.ToDecimal(txtquantity.Text.Trim());
                    }
                    catch (Exception ex)
                    {
                        lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Invalid  quantity.");
                     
                        return;
                    }
                }
                if (txtquantity == txt)
                {
                    nIndex = i;
                    grvFind = grdSelectedItem1;
                    DataClassesDataContext _db = new DataClassesDataContext();
                    hdnPricingId.Value = grdSelectedItem1.DataKeys[diitem.RowIndex].Values[0].ToString();
                    int LaborId = 1;
                    decimal nCost1 = 0;
                    if (_db.model_estimate_pricings.Where(ep => ep.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.me_pricing_id == Convert.ToInt32(hdnPricingId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() != null)
                    {
                        model_estimate_pricing pd = _db.model_estimate_pricings.Single(p => p.item_id == Convert.ToInt32(diitem.Cells[0].Text) && p.me_pricing_id == Convert.ToInt32(hdnPricingId.Value) && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
                        LaborId = Convert.ToInt32(pd.labor_id);
                        nCost1 = Convert.ToDecimal(pd.item_cost);
                    }

                    decimal nQty1 = Convert.ToDecimal(txtquantity.Text);
                    decimal nLaborRate = 0;
                    decimal nTotalPrice1 = 0;

                    if (LaborId == 2)
                    {
                        item_price itm = _db.item_prices.Single(it => it.item_id == Convert.ToInt32(diitem.Cells[0].Text) && it.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
                        nLaborRate = (decimal)itm.labor_rate * (decimal)itm.retail_multiplier;
                        nLaborRate = nLaborRate * nQty1;
                    }

                    nTotalPrice1 = nCost1 * nQty1 + nLaborRate;
                    lblTotal_price.Text = nTotalPrice1.ToString("c");
                }


                if (nIndex > -1)
                    break;

            }
            if (nIndex > -1)
                break;
        }
        GridRowUpdatingNonDirect(grvFind, nIndex);
    }

    protected void Direct_calculation(object sender, EventArgs e)
    {
        GridView grvFind = null;
        TextBox txt = (TextBox)sender;
        int nIndex = -1;
        foreach (GridViewRow dimaster2 in grdGroupingDirect.Rows)
        {
            int i = -1;
            GridView grdSelectedItem2 = (GridView)dimaster2.FindControl("grdSelectedItem2");
            foreach (GridViewRow diitem2 in grdSelectedItem2.Rows)
            {
                i++;
                TextBox txtquantity1 = (TextBox)diitem2.FindControl("txtquantity1");
                Label lblquantity1 = (Label)diitem2.FindControl("lblquantity1");
                Label lblTotal_price2 = (Label)diitem2.FindControl("lblTotal_price2");


                if (txtquantity1.Text.Trim() == "")
                {
                    lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Quantity.");
                    return;
                }
                else
                {
                    try
                    {
                        Convert.ToDecimal(txtquantity1.Text.Trim());
                    }
                    catch (Exception ex)
                    {
                        lblResult1.Text = csCommonUtility.GetSystemRequiredMessage("Invalid  quantity.");
                     
                        return;
                    }
                }

                if (txtquantity1 == txt)
                {
                    nIndex = i;
                    grvFind = grdSelectedItem2;
                    DataClassesDataContext _db = new DataClassesDataContext();
                    hdnPricingId.Value = grdSelectedItem2.DataKeys[diitem2.RowIndex].Values[0].ToString();
                    int LaborId2 = 1;
                    decimal nLaborRate2 = 0;
                    //decimal nCost2 = Convert.ToDecimal(diitem2.Cells[5].Text.Replace("$", ""));
                    decimal nCost2 = 0;
                    decimal nQty2 = Convert.ToDecimal(txtquantity1.Text);
                    decimal nTotalPrice2 = 0;
                    if (_db.model_estimate_pricings.Where(ep => ep.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.me_pricing_id == Convert.ToInt32(hdnPricingId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() != null)
                    {
                        model_estimate_pricing pd = _db.model_estimate_pricings.Single(p => p.item_id == Convert.ToInt32(diitem2.Cells[0].Text) && p.me_pricing_id == Convert.ToInt32(hdnPricingId.Value) && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
                        LaborId2 = Convert.ToInt32(pd.labor_id);
                        nCost2 = Convert.ToDecimal(pd.item_cost);
                    }
                    if (LaborId2 == 2)
                    {
                        item_price itm = _db.item_prices.Single(it => it.item_id == Convert.ToInt32(diitem2.Cells[0].Text) && it.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
                        nLaborRate2 = (decimal)itm.labor_rate * (decimal)itm.retail_multiplier;
                        nLaborRate2 = nLaborRate2 * nQty2;
                    }

                    nTotalPrice2 = nCost2 * nQty2 + nLaborRate2;
                    lblTotal_price2.Text = nTotalPrice2.ToString("c");

                }

                if (nIndex > -1)
                    break;
            }
            if (nIndex > -1)
                break;
        }
        GridRowUpdatingDirect(grvFind, nIndex);
    }

   
 
  

   
   
 

  

   


    protected void grdLeadList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int ncid = Convert.ToInt32(grdLeadList.DataKeys[e.Row.RowIndex].Values[0].ToString());
            int nSalesID = Convert.ToInt32(grdLeadList.DataKeys[e.Row.RowIndex].Values[1].ToString());
            int nIsCust = Convert.ToInt32(grdLeadList.DataKeys[e.Row.RowIndex].Values[3].ToString());
            int nIslead = Convert.ToInt32(grdLeadList.DataKeys[e.Row.RowIndex].Values[4].ToString());

            DataClassesDataContext _db = new DataClassesDataContext();
            // Customer Address
            customer cust = new customer();
            cust = _db.customers.Single(c => c.customer_id == ncid);
            string strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;

            sales_person sp = new sales_person();
            sp = _db.sales_persons.Single(s => s.sales_person_id == nSalesID);
            e.Row.Cells[4].Text = sp.first_name + " " + sp.last_name + "<br/>" + Convert.ToDateTime(cust.registration_date).ToShortDateString();

            if (nIsCust == 1)
                e.Row.Cells[5].Text = "Customer";
            else
                e.Row.Cells[5].Text = "Lead";



            Label lblPhone = (Label)e.Row.FindControl("lblPhone");
            lblPhone.Text = cust.phone;
            lblPhone.Attributes.CssStyle.Add("padding", "5px 0 5px 0");

            // Customer Email
            Label lEmail = (Label)e.Row.FindControl("lEmail");
            lEmail.Text = cust.email;

            // Customer Address in Google Map
            Label lAddress = (Label)e.Row.FindControl("lAddress");
            lAddress.Text = strAddress;

        }
    }
    protected void grdLeadList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GetCustomersNew(e.NewPageIndex);
    }
  
   
    protected void grdLeadList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Select")
        {
            if (Convert.ToInt32(hdnHdnPrevIndex.Value) > -1)
            {
                GridViewRow Prevrow = grdLeadList.Rows[Convert.ToInt32(hdnHdnPrevIndex.Value)];
                Prevrow.BackColor = Color.WhiteSmoke;
            }
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = grdLeadList.Rows[index];
            grdLeadList.SelectedIndex = row.RowIndex;
            row.BackColor = ColorTranslator.FromHtml("#efe2e2");
            // row.BackColor = ColorTranslator.FromHtml("#A1DCF2");
            int nCustID = Convert.ToInt32(grdLeadList.DataKeys[row.RowIndex].Values[0]);
              DataClassesDataContext _db = new DataClassesDataContext();
            string firstName = grdLeadList.DataKeys[row.RowIndex].Values[5].ToString();
            string LastName = grdLeadList.DataKeys[row.RowIndex].Values[6].ToString();
            hdnHdnPrevIndex.Value = index.ToString();
            lblError.Text = "";
            if (nCustID > 0)
            {
                hdnCustomerId.Value = nCustID.ToString();
                Session.Add("CustomerId", hdnCustomerId.Value);
                hdnCustName.Value = firstName + " " + LastName;
                lblSelectedCustName.Text = firstName + " " + LastName;
               

              

                // Customer Estimates without (Sold)
                var CustomerEstimate = from cest in _db.customer_estimates
                                       where cest.status_id != 3 && cest.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                       orderby cest.estimate_name ascending
                                       select cest;
                int n = CustomerEstimate.Count();
                if (CustomerEstimate != null && n > 0)
                {
                    tblExistingEstimates.Visible = true;
                    tblNoExistingEstimates.Visible = false;

                    ddlChoseEstimate.DataSource = CustomerEstimate;
                    ddlChoseEstimate.DataTextField = "estimate_name";
                    ddlChoseEstimate.DataValueField = "estimate_id";
                    ddlChoseEstimate.DataBind();
                    ddlChoseEstimate.Items.Insert(0, "Choose Estimate");
                    ddlChoseEstimate.SelectedIndex = 0;
                }
                else
                {
                   tblNoExistingEstimates.Visible = true;
                   tblExistingEstimates.Visible = false;
                }

            }
            else
            {
                tblNoExistingEstimates.Visible = false;
                tblExistingEstimates.Visible = false;
               
                //return;
            }
        }
    }
    protected void GetCustomersNew(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        grdLeadList.PageIndex = nPageNo;
        string strCondition = "";

        if (txtSearch.Text.Trim() != "")
        {
            string str = txtSearch.Text.Trim();
            if (ddlSearchBy.SelectedValue == "1")
            {
                strCondition = " customers.first_name1 LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "2")
            {
                strCondition = "  customers.last_name1 LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "3")
            {

                strCondition = "  customers.email LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "4")
            {
                strCondition = "  customers.address LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "6")
            {
                strCondition = "  customers.company LIKE '%" + str + "%'";
            }
        }

        if (strCondition.Length > 0)
        {
            strCondition = " and " + strCondition;
        }
        string strQ = string.Empty;

        if (Convert.ToInt32(hdnSalesPersonId.Value) > 0)
        {
            strQ = " SELECT client_id, customers.customer_id, first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                         " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,company,islead,isCustomer " +
                         " FROM customers " +
                         " WHERE customers.status_id NOT IN(4,5) AND customers.sales_person_id = " + Convert.ToInt32(hdnSalesPersonId.Value) + "  " + strCondition + " order by last_name1 asc,registration_date desc";
        }
        else
        {
            strQ = " SELECT client_id, customers.customer_id, first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                        " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,company,islead,isCustomer " +
                        " FROM customers " +
                        " WHERE customers.status_id NOT IN(4,5) " + strCondition + " order by last_name1 asc,registration_date desc";
        }

        DataTable dt = csCommonUtility.GetDataTable(strQ);
        Session.Add("sCustList", dt);
        grdLeadList.DataSource = dt;
        grdLeadList.DataKeyNames = new string[] { "customer_id", "sales_person_id", "status_id", "isCustomer", "islead", "first_name1", "last_name1" };
        grdLeadList.DataBind();

        if (grdLeadList.Rows.Count == 1)
        {
            int index = 0;
            GridViewRow row = grdLeadList.Rows[index];
            grdLeadList.SelectedIndex = row.RowIndex;
            row.BackColor = ColorTranslator.FromHtml("#efe2e2");
            int nCustID = Convert.ToInt32(grdLeadList.DataKeys[row.RowIndex].Values[0]);

            string firstName = grdLeadList.DataKeys[row.RowIndex].Values[5].ToString();
            string LastName = grdLeadList.DataKeys[row.RowIndex].Values[6].ToString();
            hdnHdnPrevIndex.Value = index.ToString();
            lblError.Text = "";
            if (nCustID > 0)
            {
                hdnCustomerId.Value = nCustID.ToString();
                Session.Add("CustomerId", hdnCustomerId.Value);
                hdnCustName.Value = firstName + " " + LastName;
                lblSelectedCustName.Text = firstName + " " + LastName;

              

               

                // Customer Estimates without (Sold)
                var CustomerEstimate = from cest in _db.customer_estimates
                                       where cest.status_id != 3 && cest.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                       orderby cest.estimate_name ascending
                                       select cest;
                int n = CustomerEstimate.Count();
                if (CustomerEstimate != null && n > 0)
                {
                    tblExistingEstimates.Visible = true;
                    tblNoExistingEstimates.Visible = false;

                    ddlChoseEstimate.DataSource = CustomerEstimate;
                    ddlChoseEstimate.DataTextField = "estimate_name";
                    ddlChoseEstimate.DataValueField = "estimate_id";
                    ddlChoseEstimate.DataBind();
                    ddlChoseEstimate.Items.Insert(0, "Choose Estimate");
                    ddlChoseEstimate.SelectedIndex = 0;
                }
                else
                {
                    tblNoExistingEstimates.Visible = true;
                    tblExistingEstimates.Visible = false;
                }

            }
            else
            {
                tblNoExistingEstimates.Visible = false;
                tblExistingEstimates.Visible = false;
                
                return;
            }
        }

    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        GetCustomersNew(0);

    }
    public string GetItemNameForUpdateItem(int SectionId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strDetails = "";
        string sql = "with section_tree as " +
                    " (" +
                       " select section_id, parent_id, section_name " +
                       " from sectioninfo " +
                       " where section_id = " + SectionId + " " + //-- this is the starting point for recursion
                       " union all " +
                       " select C.section_id, C.parent_id, C.section_name " +
                       " from sectioninfo c " +
                       " join section_tree p on C.section_id = P.parent_id   " +
           
                       " AND C.section_id<>C.parent_id AND C.parent_id != 0 " +
                       " ) " +

                        // --STUFF function : Delete 2 characters from a string, starting in position 1, and then insert empty in position 1. 
                        " SELECT STUFF((SELECT ' ^ ' + section_name " +
                        " FROM section_tree " +
                        " ORDER BY section_id " +
                        " FOR XML PATH('')), 1, 2, '') AS section_name";

        var list = _db.ExecuteQuery<string>(sql, string.Empty).ToList();

        if ((list[0] ?? "").Length > 0)
            strDetails = (list[0] ?? "").ToString().TrimStart().Replace("^", ">>") + " >> ";

        return strDetails;
    }

    protected void btnAssignToCustomer_Click(object sender, EventArgs e)
    {
        try
        {
            lblResult.Text = "";

            DataClassesDataContext _db = new DataClassesDataContext();

            if (hdnCustomerId.Value == "0")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please select Assign to Customer.");

                return;
            }
            if (tblExistingEstimates.Visible == true)
            {
                if (ddlChoseEstimate.SelectedItem.Text == "Choose Estimate")
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please select Choose Estimate.");

                    return;
                }
            }

            if (tblExistingEstimates.Visible == true)
            {
                //Existing  Estimate 

                // Customer section
                string sql = "select Distinct section_level from model_estimate_pricing  where sales_person_id=" + Convert.ToInt32(hdnSalesPersonId.Value) + " and model_estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " and location_id=" + Convert.ToInt32(hdnLocationId.Value);
                DataTable dt = csCommonUtility.GetDataTable(sql);
                DataView dvFinal = dt.DefaultView;
                for (int i = 0; i < dvFinal.Count; i++)
                {
                    DataRowView dr = dvFinal[i];

                    if (_db.customer_sections.Where(cs => cs.section_id == Convert.ToInt32(dr["section_level"].ToString()) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnCustomerEstimateId.Value) && cs.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList().Count()==0)
                    {

                        if (_db.customer_sections.Where(cs => cs.section_id == Convert.ToInt32(dr["section_level"].ToString()) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnCustomerEstimateId.Value) && cs.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() == null)
                        {
                            customer_section cusSec = new customer_section();
                            cusSec.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                            cusSec.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                            cusSec.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                            cusSec.section_id = Convert.ToInt32(dr["section_level"].ToString());
                            cusSec.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                            _db.customer_sections.InsertOnSubmit(cusSec);
                            _db.SubmitChanges();
                        }
                    }

                }

                // Customer Location
                if(_db.customer_locations.Where(cloc => cloc.location_id == Convert.ToInt32(hdnLocationId.Value) && cloc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cloc.estimate_id == Convert.ToInt32(hdnCustomerEstimateId.Value) && cloc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList().Count()==0)
                {

                    if (_db.customer_locations.Where(cloc => cloc.location_id == Convert.ToInt32(hdnLocationId.Value) && cloc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cloc.estimate_id == Convert.ToInt32(hdnCustomerEstimateId.Value) && cloc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() == null)
                    {
                        customer_location cusLoc = new customer_location();
                        cusLoc.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                        cusLoc.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                        cusLoc.location_id = Convert.ToInt32(hdnLocationId.Value);
                        cusLoc.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                        _db.customer_locations.InsertOnSubmit(cusLoc);
                        _db.SubmitChanges();
                    }
                }

                foreach (GridViewRow dimaster1 in grdGrouping.Rows)
                {
                    GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
                    foreach (GridViewRow diitem in grdSelectedItem1.Rows)
                    {
                        CheckBox chkIsSelected = (CheckBox)diitem.FindControl("chkIsSelected");
                        if (chkIsSelected.Checked)
                        {
                            int me_priceid = Convert.ToInt32(grdSelectedItem1.DataKeys[diitem.RowIndex].Values[0]);

                            model_estimate_pricing objPd = _db.model_estimate_pricings.SingleOrDefault(mePr => mePr.me_pricing_id == me_priceid && mePr.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

                            pricing_detail pd = new pricing_detail();
                            pd.client_id = objPd.client_id; ;
                            pd.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                            pd.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                            pd.location_id = objPd.location_id; ;
                            pd.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                            pd.section_level = objPd.section_level;
                            pd.item_id = objPd.item_id;
                            pd.section_name = objPd.section_name;
                            pd.item_name = objPd.item_name;
                            pd.measure_unit = objPd.measure_unit;
                            pd.minimum_qty = objPd.minimum_qty;
                            pd.quantity = objPd.quantity;
                            pd.retail_multiplier = objPd.retail_multiplier;
                            pd.labor_id = objPd.labor_id;
                            pd.is_direct = objPd.is_direct;
                            pd.item_cost = objPd.item_cost;
                            pd.total_direct_price = objPd.total_direct_price;
                            pd.total_retail_price = objPd.total_retail_price;
                            pd.labor_rate = objPd.labor_rate;
                            pd.section_serial = objPd.section_serial;
                            pd.item_cnt = objPd.item_cnt;
                            pd.short_notes = objPd.short_notes;
                            pd.pricing_type = objPd.pricing_type;
                            pd.create_date = objPd.create_date;
                            pd.last_update_date = DateTime.Today;
                            pd.sort_id = objPd.sort_id;
                            pd.is_mandatory = false;
                            pd.is_CommissionExclude = false;
                            _db.pricing_details.InsertOnSubmit(pd);


                        }
                    }

                    _db.SubmitChanges();
                }
                
                lblResult.Text = csCommonUtility.GetSystemMessage(hdnCustName.Value + "'s " + ddlChoseEstimate.SelectedItem.Text + " has updated successfully.");
                btnGotoCustomer.Visible = true;
            }
            else
            {
                //New Estimate 

                model_estimate me = new model_estimate();
                me = _db.model_estimates.Single(mest => mest.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && mest.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

                // Insert Customer Estimate
                int nEstId = 0;
                var result = (from ce in _db.customer_estimates
                              where ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                              select ce.estimate_id);

                int n = result.Count();
                if (result != null && n > 0)
                    nEstId = result.Max();
                nEstId = nEstId + 1;
                hdnCustomerEstimateId.Value = nEstId.ToString();

                customer_estimate cus_est = new customer_estimate();
                cus_est.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                cus_est.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                cus_est.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                cus_est.status_id = 1;
                cus_est.sale_date = "";
                cus_est.estimate_comments = me.estimate_comments;
                cus_est.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                cus_est.estimate_name = lblNewEstimateName.Text;
                cus_est.IsEstimateActive = true;
                cus_est.IsCustDisplay = false;
                cus_est.create_date = DateTime.Now;
                cus_est.last_update_date = DateTime.Now;
                cus_est.JobId = 0;

                _db.customer_estimates.InsertOnSubmit(cus_est);

                // Customer section
                string sql = "select Distinct section_level from model_estimate_pricing  where sales_person_id=" + Convert.ToInt32(hdnSalesPersonId.Value) + " and model_estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " and location_id=" + Convert.ToInt32(hdnLocationId.Value);
                DataTable dt = csCommonUtility.GetDataTable(sql);
                DataView dvFinal = dt.DefaultView;
                for (int i = 0; i < dvFinal.Count; i++)
                {
                    DataRowView dr = dvFinal[i];
                    if (_db.customer_sections.Where(cs => cs.section_id == Convert.ToInt32(dr["section_level"].ToString()) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnCustomerEstimateId.Value) && cs.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList().Count() == 0)
                    {

                        if (_db.customer_sections.Where(cs => cs.section_id == Convert.ToInt32(dr["section_level"].ToString()) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnCustomerEstimateId.Value) && cs.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() == null)
                        {
                            customer_section cusSec = new customer_section();
                            cusSec.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                            cusSec.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                            cusSec.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                            cusSec.section_id = Convert.ToInt32(dr["section_level"].ToString());
                            cusSec.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                            _db.customer_sections.InsertOnSubmit(cusSec);
                            _db.SubmitChanges();
                        }
                    }

                }

                // Customer Location

                if (_db.customer_locations.Where(cloc => cloc.location_id == Convert.ToInt32(hdnLocationId.Value) && cloc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cloc.estimate_id == Convert.ToInt32(hdnCustomerEstimateId.Value) && cloc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList().Count() == 0)
                {

                    if (_db.customer_locations.Where(cloc => cloc.location_id == Convert.ToInt32(hdnLocationId.Value) && cloc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cloc.estimate_id == Convert.ToInt32(hdnCustomerEstimateId.Value) && cloc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() == null)
                    {
                        customer_location cusLoc = new customer_location();
                        cusLoc.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                        cusLoc.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                        cusLoc.location_id = Convert.ToInt32(hdnLocationId.Value);
                        cusLoc.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                        _db.customer_locations.InsertOnSubmit(cusLoc);
                        _db.SubmitChanges();
                    }
                }

                foreach (GridViewRow dimaster1 in grdGrouping.Rows)
                {
                    GridView grdSelectedItem1 = (GridView)dimaster1.FindControl("grdSelectedItem1");
                    foreach (GridViewRow diitem in grdSelectedItem1.Rows)
                    {
                        CheckBox chkIsSelected = (CheckBox)diitem.FindControl("chkIsSelected");
                        if (chkIsSelected.Checked)
                        {
                            int me_priceid = Convert.ToInt32(grdSelectedItem1.DataKeys[diitem.RowIndex].Values[0]);

                            model_estimate_pricing objPd = _db.model_estimate_pricings.SingleOrDefault(mePr => mePr.me_pricing_id == me_priceid && mePr.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

                            pricing_detail pd = new pricing_detail();
                            pd.client_id = objPd.client_id; ;
                            pd.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                            pd.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                            pd.location_id = objPd.location_id; ;
                            pd.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                            pd.section_level = objPd.section_level;
                            pd.item_id = objPd.item_id;
                            pd.section_name = objPd.section_name;
                            pd.item_name = objPd.item_name;
                            pd.measure_unit = objPd.measure_unit;
                            pd.minimum_qty = objPd.minimum_qty;
                            pd.quantity = objPd.quantity;
                            pd.retail_multiplier = objPd.retail_multiplier;
                            pd.labor_id = objPd.labor_id;
                            pd.is_direct = objPd.is_direct;
                            pd.item_cost = objPd.item_cost;
                            pd.total_direct_price = objPd.total_direct_price;
                            pd.total_retail_price = objPd.total_retail_price;
                            pd.labor_rate = objPd.labor_rate;
                            pd.section_serial = objPd.section_serial;
                            pd.item_cnt = objPd.item_cnt;
                            pd.short_notes = objPd.short_notes;
                            pd.pricing_type = objPd.pricing_type;
                            pd.create_date = objPd.create_date;
                            pd.last_update_date = DateTime.Today;
                            pd.sort_id = objPd.sort_id;
                            pd.is_mandatory = false;
                            pd.is_CommissionExclude = false;

                            _db.pricing_details.InsertOnSubmit(pd);
                        }
                    }

                    _db.SubmitChanges();
                }

                lblResult.Text = csCommonUtility.GetSystemMessage(hdnCustName.Value + "'s " + lblNewEstimateName.Text + " has saved successfully.");
                btnGotoCustomer.Visible = true;
            }
        }
        catch(Exception ex)
        {
            lblResult.Text = ex.Message;
        }
      }


    protected void ddlChoseEstimate_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblResult.Text = "";
        lblError.Text = "";
       
        if (ddlChoseEstimate.SelectedItem.Text != "Choose Estimate")
        {
            int nCustomerEstimateId = Convert.ToInt32(ddlChoseEstimate.SelectedValue);
            hdnCustomerEstimateId.Value = nCustomerEstimateId.ToString();   
        }
    }

    //protected void btnClose_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect("customerlist.aspx");
    //}
    protected void btnGotoModelEstimate_Click(object sender, EventArgs e)
    {
        if(hdnType.Value=="1")
           Response.Redirect("me_pricing.aspx?meid=" + hdnEstimateId.Value + "&spid=" + hdnSalesPersonId.Value);
         else
            Response.Redirect("PublicMe_Pricing.aspx?meid=" + hdnEstimateId.Value + "&spid=" + hdnSalesPersonId.Value);
    }
    protected void btnGotoCustomer_Click(object sender, EventArgs e)
    {
         Response.Redirect("pricing.aspx?eid=" + Convert.ToInt32(hdnCustomerEstimateId.Value) + "&cid=" + Convert.ToInt32(hdnCustomerId.Value));
    }

 }

   

