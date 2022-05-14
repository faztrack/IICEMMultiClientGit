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

public partial class lead_report : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetJobNumber(String prefixText, Int32 count)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        return (from c in _db.customer_estimates
                where c.status_id == 3 && c.job_number != null && c.job_number.StartsWith(prefixText)
                select c.job_number).Take<String>(count).ToArray();
    }

    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["ldrSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["ldrSearch"];
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
        if (HttpContext.Current.Session["ldrSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["ldrSearch"];
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
        if (HttpContext.Current.Session["ldrSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["ldrSearch"];
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
        if (HttpContext.Current.Session["ldrSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["ldrSearch"];
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
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("lead001") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            Session.Remove("CustomerId");
            if (Session["LeadId"] != null)
            {
                int nCustomerId = Convert.ToInt32(Session["LeadId"]);
                hdnLeadId.Value = nCustomerId.ToString();
            }

            // Get Leads
            # region Get Leads

            DataClassesDataContext _db = new DataClassesDataContext();
            List<customer> LeadList = _db.customers.ToList();
            Session.Add("ldrSearch", LeadList);

            # endregion
            BindSalesPerson();
            BindSuperintendent();
            BindLeadStatus();
            BindLeadSource();
           ddlStatus.SelectedValue = "7";
            GetCustomersNew(0);
        }
    }
    private void BindSalesPerson()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select first_name+' '+last_name AS sales_person_name,sales_person_id from sales_person WHERE is_active=1  and is_sales=1 and sales_person.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " order by sales_person_id asc";
        List<userinfo> mList = _db.ExecuteQuery<userinfo>(strQ, string.Empty).ToList();
        ddlSalesRep.DataSource = mList;
        ddlSalesRep.DataTextField = "sales_person_name";
        ddlSalesRep.DataValueField = "sales_person_id";
        ddlSalesRep.DataBind();
        ddlSalesRep.Items.Insert(0, "All");

    }
    private void BindLeadStatus()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var LeadStatus = from st in _db.lead_status
                         where st.lead_status_id != 6 && st.lead_status_id != 4 && st.lead_status_id != 5  && st.is_active==true
                         orderby st.lead_status_id
                         select st;
        ddlStatus.DataSource = LeadStatus;
        ddlStatus.DataTextField = "lead_status_name";
        ddlStatus.DataValueField = "lead_status_id";
        ddlStatus.DataBind();
      //  ddlStatus.SelectedValue = "7";
       // ddlLeadSource.Items.Insert(0, "All");
       // ddlLeadSource.SelectedIndex = 0;
    }

    private void BindLeadSource()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var item = from l in _db.lead_sources
                   where l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && l.is_active == Convert.ToBoolean(1)
                   orderby l.lead_name
                   select l;
        ddlLeadSource.DataSource = item;
        ddlLeadSource.DataTextField = "lead_name";
        ddlLeadSource.DataValueField = "lead_source_id";

        ddlLeadSource.DataBind();
        ddlLeadSource.Items.Insert(0, "All");
        ddlLeadSource.SelectedIndex = 0;
    }

    private void BindSuperintendent()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select first_name+' '+last_name AS Superintendent_name,user_id from user_info WHERE role_id = 4";
        List<userinfo> mList = _db.ExecuteQuery<userinfo>(strQ, string.Empty).ToList();
        ddlSuperintendent.DataSource = mList;
        ddlSuperintendent.DataTextField = "Superintendent_name";
        ddlSuperintendent.DataValueField = "user_id";
        ddlSuperintendent.DataBind();
        ddlSuperintendent.Items.Insert(0, "All");
    }
    protected void GetCustomersNew(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        userinfo obj = (userinfo)Session["oUser"];
        int nSalePersonId = obj.sales_person_id;
        int nRoleId = obj.role_id;
        if (nRoleId == 4)
        {
            ddlSuperintendent.SelectedValue = obj.user_id.ToString();
            ddlSuperintendent.Enabled = false;
        }
        else if (nRoleId == 3)
        {
            ddlSalesRep.SelectedValue = nSalePersonId.ToString();
            ddlSalesRep.Enabled = false;
        }
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
        }

        if (ddlLeadSource.SelectedValue != "All")
        {
            if (strCondition.Length > 0)
            {
                strCondition += " AND customers.lead_source_id IN(" + Convert.ToInt32(ddlLeadSource.SelectedValue) + ")  ";
            }
            else
            {
                strCondition += " customers.lead_source_id IN(" + Convert.ToInt32(ddlLeadSource.SelectedValue) + ")  ";
            }
        }

        if (Convert.ToInt32(ddlStatus.SelectedValue) == 7)
        {
            if (strCondition.Length > 0)
            {
                strCondition += " AND customers.status_id NOT IN(4,5)  ";
            }
            else
            {
                strCondition += " customers.status_id NOT IN(4,5)  ";
            }

        }
        else if (Convert.ToInt32(ddlStatus.SelectedValue) == 6)
        {
            if (strCondition.Length > 0)
            {
                strCondition += " AND customers.customer_id IN (SELECT Distinct customer_estimate.customer_id FROM customer_estimate WHERE customer_estimate.status_id=3) AND customers.status_id NOT IN(4,5) ";
            }
            else
            {
                strCondition += " customers.customer_id IN (SELECT Distinct customer_estimate.customer_id FROM customer_estimate WHERE customer_estimate.status_id=3) AND customers.status_id NOT IN(4,5) ";
            }

        }
        else if (Convert.ToInt32(ddlStatus.SelectedValue) > 7)
        {
            if (strCondition.Length > 0)
            {
                strCondition += " AND customers.lead_status_id = " + Convert.ToInt32(ddlStatus.SelectedValue);
            }
            else
            {
                strCondition += " customers.lead_status_id = " + Convert.ToInt32(ddlStatus.SelectedValue);
            }

        }
        else
        {
            if (ddlStatus.SelectedItem.Text != "All")
           {
            if (strCondition.Length > 0)
            {
                strCondition += " AND customers.status_id = " + Convert.ToInt32(ddlStatus.SelectedValue);
            }
            else
            {
               strCondition += " customers.status_id = " + Convert.ToInt32(ddlStatus.SelectedValue);
            }
         }

        }
        if (ddlSalesRep.SelectedItem.Text != "All")
        {
            if (strCondition.Length > 0)
            {
                strCondition += " AND customers.sales_person_id =" + Convert.ToInt32(ddlSalesRep.SelectedValue);
            }
            else
            {
                strCondition += " customers.sales_person_id =" + Convert.ToInt32(ddlSalesRep.SelectedValue);
            }

        }
        if (ddlSuperintendent.SelectedItem.Text != "All")
        {
            if (strCondition.Length > 0)
            {
                strCondition += " AND  customers.SuperintendentId = " + Convert.ToInt32(ddlSuperintendent.SelectedValue);
            }
            else
            {
                strCondition += "  customers.SuperintendentId = " + Convert.ToInt32(ddlSuperintendent.SelectedValue);
            }

        }

        if (txtApptStartDate.Text.Trim() != "" && txtApptEndDate.Text.Trim() != "")
        {
            try
            {
                DateTime dt1 = Convert.ToDateTime(txtApptStartDate.Text.Trim());
                DateTime dt2 = Convert.ToDateTime(txtApptEndDate.Text.Trim());
                if (Convert.ToDateTime(txtApptEndDate.Text.Trim()) >= Convert.ToDateTime(txtApptStartDate.Text.Trim()))
                {
                    if (strCondition.Length == 0)
                    {

                        strCondition = " WHERE customers.appointment_date  between '" + dt1 + "' and '" + dt2 + "'";
                    }
                    else
                    {
                        strCondition += " AND customers.appointment_date between '" + dt1 + "' and '" + dt2 + "'";
                    }
                }
            }
            catch
            {
            }
        }

        if (txtEntStartDate.Text.Trim() != "" && txtEntEndDate.Text.Trim() != "")
        {
            try
            {
                DateTime dt1 = Convert.ToDateTime(txtEntStartDate.Text.Trim());
                DateTime dt2 = Convert.ToDateTime(txtEntEndDate.Text.Trim());
                if (Convert.ToDateTime(txtEntEndDate.Text.Trim()) >= Convert.ToDateTime(txtEntStartDate.Text.Trim()))
                {
                    if (strCondition.Length == 0)
                    {

                        strCondition = " WHERE customers.registration_date  between '" + dt1 + "' and '" + dt2 + "'";
                    }
                    else
                    {
                        strCondition += " AND customers.registration_date between '" + dt1 + "' and '" + dt2 + "'";
                    }
                }
            }
            catch
            {
            }
        }

        if (strCondition.Length > 0)
        {
            strCondition = "Where customers.islead = 1 and " + strCondition;
        }
        string strQ = string.Empty;
        if (Convert.ToInt32(hdnLeadId.Value) > 0)
        {
            int nCustomerId = Convert.ToInt32(hdnLeadId.Value);
            //strQ = "SELECT * FROM customers WHERE customers.customer_id =" + nCustomerId;
            strQ = " SELECT client_id, customers.customer_id,first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                          " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date " +
                          " FROM customers " +
                          " LEFT OUTER JOIN (SELECT  customer_estimate.customer_id,MIN(CONVERT(DATETIME,sale_date)) AS SaleDate FROM customer_estimate WHERE customer_estimate.status_id=3 AND customer_estimate.customer_id = " + nCustomerId + " GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                          " WHERE customers.islead = 1 and customers.customer_id =" + nCustomerId + " order by t1.SaleDate asc";
            hdnLeadId.Value = "0";
        }
        else
        {
            if (nRoleId == 4)
            {
                if (ddlSearchBy.SelectedValue == "5")
                {
                    strQ = " SELECT client_id, customers.customer_id,first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                                   " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date " +
                                   " FROM customers " +
                                   " INNER JOIN (SELECT  customer_estimate.customer_id,MIN(CONVERT(DATETIME,sale_date)) AS SaleDate FROM customer_estimate WHERE customers.islead = 1 and customer_estimate.status_id = 3 AND job_number LIKE '%" + txtSearch.Text + "%' GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                                   "  order by t1.SaleDate asc";
                }
                else
                {
                    strQ = " SELECT client_id, customers.customer_id,first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                                  " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date " +
                                  " FROM customers " +
                                  " LEFT OUTER JOIN  (SELECT  customer_estimate.customer_id,MIN(CONVERT(DATETIME,sale_date)) AS SaleDate FROM customer_estimate WHERE customer_estimate.status_id=3 GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                                  " " + strCondition + " order by t1.SaleDate asc";

                }
            }
            else
            {
                if (ddlSearchBy.SelectedValue == "5")
                {
                    strQ = " SELECT client_id, customers.customer_id,first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                                  " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date " +
                                  " FROM customers " +
                                  " INNER JOIN  (SELECT  customer_estimate.customer_id,MIN(CONVERT(DATETIME,sale_date)) AS SaleDate FROM customer_estimate WHERE customers.islead = 1 and customer_estimate.status_id=3 AND job_number LIKE '%" + txtSearch.Text + "%' GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                                  "  order by customers.registration_date desc, customers.last_name1 asc";
                }
                else
                {
                    strQ = " SELECT client_id, customers.customer_id,first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                                  " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date " +
                                  " FROM customers " +
                                  " LEFT OUTER JOIN   (SELECT  customer_estimate.customer_id,MIN(CONVERT(DATETIME,sale_date)) AS SaleDate FROM customer_estimate WHERE customer_estimate.status_id=3 GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                                  " " + strCondition + " order by customers.registration_date desc, customers.last_name1 asc";

                }
            }
        }

        IEnumerable<csCustomer> mList = _db.ExecuteQuery<csCustomer>(strQ, string.Empty).ToList();
        DataTable dt = csCommonUtility.LINQToDataTable(mList);
        Session.Add("Fil_Cust", dt);
        if (ddlItemPerPage.SelectedValue != "4")
        {
            grdLeadList.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
        }
        else
        {
            grdLeadList.PageSize = 200;
        }
        grdLeadList.DataSource = mList;
        grdLeadList.DataKeyNames = new string[] { "customer_id", "sales_person_id", "status_id", "lead_source_id" };
        grdLeadList.DataBind();
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

        if (grdLeadList.PageCount == nPageNo + 1)
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
        Session.Remove("LeadId");
        Response.Redirect("lead_details.aspx");
    }
    protected void grdLeadList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int ncid = Convert.ToInt32(grdLeadList.DataKeys[e.Row.RowIndex].Values[0].ToString());
            DataClassesDataContext _db = new DataClassesDataContext();

            int nid = Convert.ToInt32(grdLeadList.DataKeys[e.Row.RowIndex].Values[1].ToString());

            sales_person sp = new sales_person();
            sp = _db.sales_persons.Single(s => s.sales_person_id == nid);
            e.Row.Cells[2].Text = sp.first_name + " " + sp.last_name;
            // Lead Status
            int nstatus_id = Convert.ToInt32(grdLeadList.DataKeys[e.Row.RowIndex].Values[2].ToString());

            // lead_source
            int nlead_source_id = Convert.ToInt32(grdLeadList.DataKeys[e.Row.RowIndex].Values[3].ToString());

            lead_source lsc = _db.lead_sources.Single(l => l.lead_source_id == nlead_source_id);
            e.Row.Cells[5].Text = lsc.lead_name;

            // Customer Address
            customer cust = new customer();
            cust = _db.customers.Single(c => c.customer_id == ncid);
            string strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
            //e.Row.Cells[2].Text = strAddress;

            Label lblPhone = (Label)e.Row.FindControl("lblPhone");
            lblPhone.Text = cust.phone;
            lblPhone.Attributes.CssStyle.Add("padding", "5px 0 5px 0");

            // Customer Email
            HyperLink hypEmail = (HyperLink)e.Row.FindControl("hypEmail");
            hypEmail.Text = cust.email;
            hypEmail.NavigateUrl = "mailto:" + cust.email + "?subject=Contact";

            // Customer Address in Google Map
            HyperLink hypAddress = (HyperLink)e.Row.FindControl("hypAddress");
            hypAddress.Text = strAddress;
            //hypAddress.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
            string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
            hypAddress.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

            if (Convert.ToDateTime(cust.appointment_date).Year == 1900)
            {
                e.Row.Cells[3].Text = "";
                e.Row.Cells[4].Text = "";


            }
            lead_status ls = _db.lead_status.Single(l => l.lead_status_id == cust.lead_status_id);
            e.Row.Cells[6].Text = ls.lead_status_name;


        }
    }
    protected void GotoPricing(object sender, EventArgs e)
    {
        LinkButton lnk = (LinkButton)sender;
        int ncid = Convert.ToInt32(lnk.CommandArgument);

        Response.Redirect("customer_locations.aspx?cid=" + ncid);
    }
    protected void grdLeadList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdLeadList.ID, grdLeadList.GetType().Name, "Page Index Changed");
        GetCustomersNew(e.NewPageIndex);
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Search Click");
        Session.Remove("LeadId");
        GetCustomersNew(0);
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetCustomersNew(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetCustomersNew(nCurrentPage - 2);
    }
    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "Selected Index Changed");
        Session.Remove("LeadId");
        GetCustomersNew(0);
    }
    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlStatus.ID, ddlStatus.GetType().Name, "Status Selected Index Changed");
        Session.Remove("LeadId");
        GetCustomersNew(0);
    }
    protected void ddlLeadSource_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlLeadSource.ID, ddlLeadSource.GetType().Name, "Source Selected Index Changed");
        Session.Remove("LeadId");
        GetCustomersNew(0);
    }
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        ddlStatus.SelectedValue = "1";
        ddlSalesRep.SelectedIndex = -1;
        ddlSuperintendent.SelectedIndex = -1;
        ddlLeadSource.SelectedIndex = -1;
        txtApptEndDate.Text = "";
        txtApptStartDate.Text = "";
        txtEntStartDate.Text = "";
        txtEntEndDate.Text = "";
        Session.Remove("LeadId");
        GetCustomersNew(0);

    }

    protected void ddlSalesRep_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSalesRep.ID, ddlSalesRep.GetType().Name, "SalesRep Selected Index Changed");
        Session.Remove("LeadId");
        GetCustomersNew(0);
    }
    protected void ddlSuperintendent_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSuperintendent.ID, ddlSuperintendent.GetType().Name, "Superintendent Changed");
        Session.Remove("LeadId");
        GetCustomersNew(0);
    }
    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "Search By Changed");
        txtSearch.Text = "";
        wtmFileNumber.WatermarkText = "Search by " + ddlSearchBy.SelectedItem.Text;
        if (ddlSearchBy.SelectedValue == "2")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetLastName";
        }
        else if (ddlSearchBy.SelectedValue == "5")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetJobNumber";
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


        GetCustomersNew(0);
    }

    protected void btnExpCustList_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnExpCustList.ID, btnExpCustList.GetType().Name, "ExpCustomer List Click");
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable dtCust = (DataTable)Session["Fil_Cust"];

        DataTable tmpTable = LoadReportDataTable();

        foreach (DataRow dr in dtCust.Rows)
        {

            int ncid = Convert.ToInt32(dr["customer_id"]);

            int nid = Convert.ToInt32(dr["sales_person_id"]);

            sales_person sp = new sales_person();
            sp = _db.sales_persons.Single(s => s.sales_person_id == nid);
            string SalesPerson = sp.first_name + " " + sp.last_name;

            // lead_source
            int nlead_source_id = Convert.ToInt32(dr["lead_source_id"]);

            lead_source lsc = _db.lead_sources.Single(l => l.lead_source_id == nlead_source_id);
            string leadSource = lsc.lead_name;

            // Customer Address
            customer cust = new customer();
            cust = _db.customers.Single(c => c.customer_id == ncid);
            string strAddress = cust.address; // +" " + cust.city + ", " + cust.state + " " + cust.zip_code;
            string city = cust.city;
            string state = cust.state;
            string zip = cust.zip_code;


            string phone = cust.phone;


            // Customer Email

            string email = cust.email;
            string apptDate = "";
            string apptTime = "";
            string entrydate = Convert.ToDateTime(cust.registration_date).ToShortDateString();
            if (Convert.ToDateTime(cust.appointment_date).Year != 1900)
            {
                apptDate = Convert.ToDateTime(cust.appointment_date).ToShortDateString();
                apptTime = Convert.ToDateTime(cust.appointment_date).ToShortTimeString();
            }
            lead_status ls = _db.lead_status.Single(l => l.lead_status_id == cust.lead_status_id);
            string LeadStatus = ls.lead_status_name;

            DataRow drNew = tmpTable.NewRow();
            drNew["customer_id"] = dr["customer_id"];
            drNew["customer_name"] = dr["customer_name"];
            drNew["Address"] = strAddress;
            drNew["City"] = city;
            drNew["State"] = state;
            drNew["Zip"] = zip;
            drNew["Phone"] = phone;
            drNew["Email"] = email;
            drNew["Sales_Person"] = SalesPerson;
            drNew["appointment_date"] = apptDate;
            drNew["appointment_time"] = apptTime;
            drNew["Lead_Source"] = leadSource;
            drNew["Lead_Status"] = LeadStatus;
            drNew["Entry_Date"] = entrydate;
            drNew["notes"] = dr["notes"];

            tmpTable.Rows.Add(drNew);
        }
        if (tmpTable.Rows.Count > 0)
        {
            tmpTable.Columns.Remove("customer_id");
            tmpTable.Columns["customer_name"].ColumnName = "Customer Name";
            tmpTable.Columns["Sales_Person"].ColumnName = "Sales Person";
            tmpTable.Columns["appointment_date"].ColumnName = "Appointment Date";
            tmpTable.Columns["appointment_time"].ColumnName = "Appointment Time";
            tmpTable.Columns["Lead_Source"].ColumnName = "Lead Source";
            tmpTable.Columns["Lead_Status"].ColumnName = "Lead Status";
            tmpTable.Columns["Entry_Date"].ColumnName = "Entry Date";
            tmpTable.Columns["notes"].ColumnName = "Notes";

            Response.Clear();
            Response.ClearHeaders();

            using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
            {
                writer.WriteAll(tmpTable, true);
            }
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=LeadReport.csv");
            Response.End();
        }

    }
    private DataTable LoadReportDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("customer_name", typeof(string));
        table.Columns.Add("Address", typeof(string));
        table.Columns.Add("City", typeof(string));
        table.Columns.Add("State", typeof(string));
        table.Columns.Add("Zip", typeof(string));
        table.Columns.Add("Phone", typeof(string));
        table.Columns.Add("Email", typeof(string));
        table.Columns.Add("Sales_Person", typeof(string));
        table.Columns.Add("appointment_date", typeof(string));
        table.Columns.Add("appointment_time", typeof(string));
        table.Columns.Add("Lead_Source", typeof(string));
        table.Columns.Add("Lead_Status", typeof(string));
        table.Columns.Add("Entry_Date", typeof(string));
        table.Columns.Add("notes", typeof(string));

        return table;
    }
    protected void txtApptStartDate_TextChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, txtApptStartDate.ID, txtApptStartDate.GetType().Name, "AppStart Date Changed");
        Session.Remove("LeadId");
        GetCustomersNew(0);
    }
    protected void txtApptEndDate_TextChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, txtApptEndDate.ID, txtApptEndDate.GetType().Name, "App EndDate Changed");
        Session.Remove("LeadId");
        GetCustomersNew(0);
    }
    protected void txtEntStartDate_TextChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, txtEntStartDate.ID, txtEntStartDate.GetType().Name, "End Start Date Changed");
        Session.Remove("LeadId");
        GetCustomersNew(0);
    }
    protected void txtEntEndDate_TextChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, txtEntEndDate.ID, txtEntEndDate.GetType().Name, "End Date Changed");
        Session.Remove("LeadId");
        GetCustomersNew(0);
    }
}
