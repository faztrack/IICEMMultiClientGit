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

public partial class mcustomerlist : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["mcSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["mcSearch"];
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
            if (Session["CustomerId"] != null)
            {
                int nCustomerId = Convert.ToInt32(Session["CustomerId"]);
                hdnCustomerId.Value = nCustomerId.ToString();
            }

            // Get Customers
            # region Get Customers
            userinfo obj = (userinfo)Session["oUser"];

            DataClassesDataContext _db = new DataClassesDataContext();
            if (obj.role_id == 3)
            {
                List<customer> CustomerList = _db.customers.Where(c => c.status_id != 4 && c.status_id != 5 &&c.sales_person_id==obj.sales_person_id).ToList();
                Session.Add("mcSearch", CustomerList);
            }
            else
            {
                List<customer> CustomerList = _db.customers.Where(c => c.status_id != 4 && c.status_id != 5).ToList();
                Session.Add("mcSearch", CustomerList);
            }

            # endregion

            GetCustomersNew(0);
        }
    }

    protected void GetCustomersNew(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        userinfo obj = (userinfo)Session["oUser"];
        int nSalePersonId = obj.sales_person_id;
        int nRoleId = obj.role_id;
        grdCustomerList.PageIndex = nPageNo;
        string strQ = string.Empty;

     

        string strCondition = "";
        if (nRoleId==3)
        {
            strCondition = " and customers.sales_person_id =" + nSalePersonId;
        }
       
        if (txtSearch.Text.Trim() != "")
        {
            string str = txtSearch.Text.Trim();

            strCondition+= " and customers.last_name1 LIKE '%" + str + "%'";

        }
       
        if (strCondition.Length > 0)
        {
            strCondition = "Where status_id NOT IN(4,5) " + strCondition;
        }

        if (Convert.ToInt32(hdnCustomerId.Value) > 0)
        {
            int nCustomerId = Convert.ToInt32(hdnCustomerId.Value);
            strQ = " SELECT client_id, customers.customer_id, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                          " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date " +
                          " FROM customers " +
                          " LEFT OUTER JOIN (SELECT  customer_estimate.customer_id,MIN(CONVERT(DATETIME,sale_date)) AS SaleDate FROM customer_estimate WHERE customer_estimate.status_id=3 AND customer_estimate.customer_id = " + nCustomerId + " GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                          " WHERE customers.customer_id =" + nCustomerId + " order by t1.SaleDate asc";
            hdnCustomerId.Value = "0";
        }
        else
        {
            strQ = " SELECT client_id, customers.customer_id, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                              " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date " +
                              " FROM customers " +
                              " LEFT OUTER JOIN (SELECT  customer_estimate.customer_id,MIN(CONVERT(DATETIME,sale_date)) AS SaleDate FROM customer_estimate WHERE customer_estimate.status_id=3 GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                              " " + strCondition + " order by customers.registration_date desc, customers.last_name1 asc";
        }

        IEnumerable<csCustomer> mList = _db.ExecuteQuery<csCustomer>(strQ, string.Empty).ToList();

        grdCustomerList.DataSource = mList;
        grdCustomerList.DataKeyNames = new string[] { "customer_id", "sales_person_id" };
        grdCustomerList.DataBind();

        hdnCurrentPageNo.Value = Convert.ToString(nPageNo + 1);
    }

    protected void grdCustomerList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int ncid = Convert.ToInt32(grdCustomerList.DataKeys[e.Row.RowIndex].Values[0]);


            DataClassesDataContext _db = new DataClassesDataContext();

            int nid = Convert.ToInt32(grdCustomerList.DataKeys[e.Row.RowIndex].Values[1]);

            sales_person sp = new sales_person();
            sp = _db.sales_persons.Single(s => s.sales_person_id == nid);

            Label lblSaleaPerson = (Label)e.Row.FindControl("lblSaleaPerson");
            lblSaleaPerson.Text = sp.first_name + " " + sp.last_name;

            // Customer Address
            customer cust = new customer();
            cust = _db.customers.Single(c => c.customer_id == ncid);
            string strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;


            HyperLink hypCustomerName = (HyperLink)e.Row.FindControl("hypCustomerName");
            hypCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
            hypCustomerName.NavigateUrl = "mcustomer_details.aspx?cid=" + ncid;

            HyperLink hypEdit = (HyperLink)e.Row.FindControl("hypEdit");
            hypEdit.Text = "";
            hypEdit.NavigateUrl = "mcustomer_details.aspx?cid=" + ncid;

            HyperLink hypUpload = (HyperLink)e.Row.FindControl("hypUpload");
            hypUpload.Text = "Upload<span class='glyphicon glyphicon-upload'></span>";
            hypUpload.NavigateUrl = "projectfileupload.aspx?cid=" + ncid;

            HyperLink hypProjectNotes = (HyperLink)e.Row.FindControl("hypProjectNotes");
          //  hypProjectNotes.Text = "Project Notes<span class='glyphicon glyphicon-list-alt'></span>";
            hypProjectNotes.NavigateUrl = "mProjectNotes.aspx?nbackId=1&cid=" + ncid;


            HyperLink hyp_DocumentManagement = (HyperLink)e.Row.FindControl("hyp_DocumentManagement");

            hyp_DocumentManagement.NavigateUrl = "mDocumentManagement.aspx?nbackId=1&cid=" + ncid;



            HyperLink hypPhone = (HyperLink)e.Row.FindControl("hypPhone");
            HyperLink hypMobile = (HyperLink)e.Row.FindControl("hypMobile");
            if (cust.mobile.Length > 2)
            {
                hypPhone.NavigateUrl = "tel:" + cust.phone;
                hypMobile.NavigateUrl = "tel:" + cust.mobile;
                hypPhone.Text = cust.phone;
                hypMobile.Text = cust.mobile;
            }
            else
            {
                hypPhone.NavigateUrl = "tel:" + cust.phone;
                hypPhone.Text = cust.phone;

            }
            // Customer Email
        
            HyperLink hypEmail = (HyperLink)e.Row.FindControl("hypEmail");
            hypEmail.NavigateUrl = "mailto:" + cust.email;
            hypEmail.Text = cust.email;

            // Customer Address in Google Map  
            HyperLink hypAddress = (HyperLink)e.Row.FindControl("hypAddress");
            hypAddress.Text = strAddress;


            string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
            hypAddress.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;
            hypAddress.ToolTip = "Google Map";




            //// Customer Estimates
            DropDownList ddlEst = (DropDownList)e.Row.FindControl("ddlEst");
            string strQ = "select * from customer_estimate where customer_id=" + ncid + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            IEnumerable<customer_estimate_model> list = _db.ExecuteQuery<customer_estimate_model>(strQ, string.Empty);

            ddlEst.DataSource = list;
            ddlEst.DataTextField = "estimate_name";
            ddlEst.DataValueField = "estimate_id";
            ddlEst.DataBind();

            HyperLink hyp_SiteReview = (HyperLink)e.Row.FindControl("hyp_SiteReview");

            hyp_SiteReview.NavigateUrl = "msiteviewlist.aspx?nestid=1&nbackId=1&cid=" + ncid;

            if (_db.customer_estimates.Where(ce => ce.customer_id == ncid && ce.client_id == 1).ToList().Count > 0)
            {
                int nEstId = 0;
                var result = (from ce in _db.customer_estimates
                              where ce.customer_id == ncid && ce.client_id == 1
                              select ce.estimate_id);

                int n = result.Count();
                if (result != null && n > 0)
                    nEstId = result.Max();
                ddlEst.SelectedValue = nEstId.ToString();
                string strQ1 = "select * from customer_estimate where customer_id=" + ncid + " AND estimate_id = " + Convert.ToInt32(ddlEst.SelectedValue) + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                IEnumerable<customer_estimate_model> list1 = _db.ExecuteQuery<customer_estimate_model>(strQ1, string.Empty);


                foreach (customer_estimate_model cus_est in list1)
                {
                    int nestid = Convert.ToInt32(cus_est.estimate_id);
                    hyp_SiteReview.NavigateUrl = "msiteviewlist.aspx?nestid=" + nestid + "&nbackId=1&cid=" + ncid;


                }
            }
            else
            {
                ddlEst.Visible = false;

            }


        }
    }

   

    protected void grdCustomerList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCustomerList.ID, grdCustomerList.GetType().Name, "PageIndexChanging"); 
        GetCustomersNew(e.NewPageIndex);
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        Session.Remove("CustomerId");
        GetCustomersNew(0);
    }

    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkViewAll.ID, lnkViewAll.GetType().Name, "Click"); 
        txtSearch.Text = "";
        Session.Remove("CustomerId");
        GetCustomersNew(0);

    }
    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("mlandingpage.aspx");
    }

    protected void Load_Est_Info(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        GridViewRow diitem = ((GridViewRow)((DropDownList)sender).NamingContainer);
        GridView grdCustomerList = (GridView)diitem.NamingContainer;

        int ncid = Convert.ToInt32(grdCustomerList.DataKeys[diitem.RowIndex].Values[0]);

        int nid = Convert.ToInt32(grdCustomerList.DataKeys[diitem.RowIndex].Values[1]);

        sales_person sp = new sales_person();
        sp = _db.sales_persons.Single(s => s.sales_person_id == nid);

        Label lblSaleaPerson = (Label)diitem.FindControl("lblSaleaPerson");
        lblSaleaPerson.Text = sp.first_name + " " + sp.last_name;

        // Customer Address
        customer cust = new customer();
        cust = _db.customers.Single(c => c.customer_id == ncid);
        string strAddress = cust.address;
        string strCityStateZip = cust.city + ", " + cust.state + " " + cust.zip_code;

        HyperLink hypCustomerName = (HyperLink)diitem.FindControl("hypCustomerName");
        hypCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
        hypCustomerName.NavigateUrl = "mcustomer_details.aspx?cid=" + ncid;

        HyperLink hypEdit = (HyperLink)diitem.FindControl("hypEdit");
        hypEdit.Text = "";
        hypEdit.NavigateUrl = "mcustomer_details.aspx?cid=" + ncid;

        HyperLink hypUpload = (HyperLink)diitem.FindControl("hypUpload");
        hypUpload.Text = "Upload<span class='glyphicon glyphicon-upload'></span>";
        hypUpload.NavigateUrl = "projectfileupload.aspx?cid=" + ncid;

        HyperLink hypProjectNotes = (HyperLink)diitem.FindControl("hypProjectNotes");
        hypProjectNotes.Text = "Project Notes<span class='glyphicon glyphicon-list-alt'></span>";
        hypProjectNotes.NavigateUrl = "mProjectNotes.aspx?cid=" + ncid;

        Label lblPhone = (Label)diitem.FindControl("lblPhone");
        lblPhone.Text = cust.phone;

        // Customer Email
        Label lblEmail = (Label)diitem.FindControl("lblEmail");
        lblEmail.Text = cust.email;

        // Customer Address in Google Map
        Label lblAddress = (Label)diitem.FindControl("lblAddress");
        lblAddress.Text = strAddress;

        // Customer Address in Google Map
        Label lblCityStateZip = (Label)diitem.FindControl("lblCityStateZip");
        lblCityStateZip.Text = strCityStateZip;


        DropDownList ddlEst = (DropDownList)diitem.FindControl("ddlEst");
        HyperLink hyp_SiteReview = (HyperLink)diitem.FindControl("hyp_SiteReview");


        string strQ2 = "select * from customer_estimate where customer_id=" + ncid + " AND estimate_id != " + Convert.ToInt32(ddlEst.SelectedValue) + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        IEnumerable<customer_estimate_model> list2 = _db.ExecuteQuery<customer_estimate_model>(strQ2, string.Empty);
        string strQ1 = "select * from customer_estimate where customer_id=" + ncid + " AND estimate_id = " + Convert.ToInt32(ddlEst.SelectedValue) + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        IEnumerable<customer_estimate_model> list1 = _db.ExecuteQuery<customer_estimate_model>(strQ1, string.Empty);

        foreach (customer_estimate_model cus_est in list1)
        {
            int nestid = Convert.ToInt32(cus_est.estimate_id);
            hyp_SiteReview.NavigateUrl = "msiteviewlist.aspx?nestid=" + nestid + "&nbackId=1&cid=" + ncid;
        }

    }
   
}
