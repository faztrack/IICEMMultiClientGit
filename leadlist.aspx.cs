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
using System.Data.OleDb;
using System.IO;

public partial class leadlist : System.Web.UI.Page
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
    public static string[] GetCompany(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["lSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["lSearch"];
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
        if (HttpContext.Current.Session["lSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["lSearch"];
            return (from c in cList
                    where c.last_name1.Trim().ToLower().StartsWith(prefixText.Trim().ToLower())
                    select c.last_name1).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.last_name1.Trim().ToLower().StartsWith(prefixText.Trim().ToLower())
                    select c.last_name1).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetFirstName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["lSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["lSearch"];
            return (from c in cList
                    where c.first_name1.Trim().ToLower().StartsWith(prefixText.Trim().ToLower())
                    select c.first_name1).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.first_name1.Trim().ToLower().StartsWith(prefixText.Trim().ToLower())
                    select c.first_name1).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetAddress(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["lSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["lSearch"];
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
        if (HttpContext.Current.Session["lSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["lSearch"];
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
    [WebMethod]
    public static string[] GetPhone(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["lSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["lSearch"];
            return (from c in cList
                    where c.phone.ToLower().Contains(prefixText.ToLower())
                    select c.phone).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.phone.Contains(prefixText)
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
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                userinfo oUser = (userinfo)Session["oUser"];
                hdnEmailType.Value = oUser.EmailIntegrationType.ToString();

            }

            if (Page.User.IsInRole("le01") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            GetTools();
            DataClassesDataContext _db = new DataClassesDataContext();
            if (Session["CustomerId"] != null)
            {

                int nCustomerId = Convert.ToInt32(Session["CustomerId"]);


                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == nCustomerId);
                if (cust.isCustomer == 0)
                {
                    Session.Add("LeadId", nCustomerId.ToString());
                    hdnLeadId.Value = nCustomerId.ToString();
                }

                Session.Remove("CustomerId");
            }

            if (Session["LeadId"] != null)
            {
                int nCustomerId = Convert.ToInt32(Session["LeadId"]);
                hdnLeadId.Value = nCustomerId.ToString();
            }
            if (Session["LeadSreach"] != null)
            {
                string strSearch = Session["LeadSreach"].ToString();
                txtSearch.Text = strSearch;


            }
            Session.Remove("LeadSreach");
            Session.Remove("CustSreach");


            // Get Leads
            # region Get Leads


            List<customer> LeadList = _db.customers.ToList();
            Session.Add("lSearch", LeadList);

            # endregion
            BindSalesPerson();
            BindSuperintendent();
            BindLeadSource();
            BindLeadStatus();
            ddlStatus.SelectedValue = "2";
            GetCustomersNew(0);
        }
    }

    private void GetTools()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string tools = string.Empty;
            userinfo obj = (userinfo)Session["oUser"];
            user_info objUser = _db.user_infos.SingleOrDefault(u => u.user_id == obj.user_id);
            if (objUser != null)
            {
                Session.Add("tools", objUser.tools);
            }
        }
        catch (Exception ex)
        {
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
                         orderby st.lead_status_id
                         select st;
        ddlStatus.DataSource = LeadStatus;
        ddlStatus.DataTextField = "lead_status_name";
        ddlStatus.DataValueField = "lead_status_id";
        ddlStatus.DataBind();


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
            //ddlSuperintendent.SelectedValue = obj.user_id.ToString();
            //ddlSuperintendent.Enabled = false;
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
                strCondition = " customers.first_name1 LIKE '%" + str.Replace("'", "''") + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "2")
            {
                strCondition = "  customers.last_name1 LIKE '%" + str.Replace("'", "''") + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "3")
            {

                strCondition = "  customers.email LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "4")
            {
                strCondition = "  customers.address LIKE '%" + str.Replace("'", "''") + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "6")
            {
                strCondition = "  customers.company LIKE '%" + str.Replace("'", "''") + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "7")
            {
                strCondition = "  customers.phone LIKE '%" + str.Replace("'", "''") + "%'";
            }
        }
        else
        {

            if (Convert.ToInt32(ddlStatus.SelectedValue) != 1)
            {
                if (Convert.ToInt32(ddlStatus.SelectedValue) == 2)
                {
                    if (strCondition.Length > 0)
                    {
                        strCondition += " AND customers.status_id NOT IN(4,5) ";
                    }
                    else
                    {
                        strCondition += " customers.status_id NOT IN(4,5) ";
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
                else if (Convert.ToInt32(ddlStatus.SelectedValue) == 5)
                {
                    if (strCondition.Length > 0)
                    {
                        strCondition += " AND customers.status_id = 5 ";
                    }
                    else
                    {
                        strCondition += " customers.status_id = 5 ";
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
            if (ddlLeadSource.SelectedItem.Text != "All")
            {
                if (strCondition.Length > 0)
                {
                    strCondition += " AND customers.lead_source_id =" + Convert.ToInt32(ddlLeadSource.SelectedValue);
                }
                else
                {
                    strCondition += " customers.lead_source_id =" + Convert.ToInt32(ddlLeadSource.SelectedValue);
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
        }
        if (strCondition.Length > 0)
        {
            strCondition = "Where " + strCondition;
        }
        //if (strCondition.Length > 0)
        //{
        //    strCondition = "Where customers.islead = 1 and " + strCondition;
        //}
        //else
        //{
        //    strCondition = "Where customers.islead = 1";
        //}
        string strQ = string.Empty;
        if (Convert.ToInt32(hdnLeadId.Value) > 0)
        {
            int nCustomerId = Convert.ToInt32(hdnLeadId.Value);
            strQ = " SELECT client_id,customers.islead,customers.isCustomer, customers.customer_id, first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                          " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date, company,CAST(1 AS BIT) AS IsEstimateActive " +
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
                    strQ = " SELECT client_id,customers.status_id,customers.islead,customers.isCustomer, customers.customer_id, first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                                   " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date,company,CAST(1 AS BIT) AS IsEstimateActive " +
                                   " FROM customers " +
                                   " INNER JOIN (SELECT  customer_estimate.customer_id,MIN(CONVERT(DATETIME,sale_date)) AS SaleDate FROM customer_estimate WHERE customers.islead = 1 and customer_estimate.status_id = 3 AND job_number LIKE '%" + txtSearch.Text + "%' GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                                   "  order by t1.SaleDate asc";
                }
                else
                {
                    strQ = " SELECT client_id,customers.status_id,customers.islead,customers.isCustomer, customers.customer_id, first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                                  " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date,company,CAST(1 AS BIT) AS IsEstimateActive " +
                                  " FROM customers " +
                                  " LEFT OUTER JOIN  (SELECT  customer_estimate.customer_id,MIN(CONVERT(DATETIME,sale_date)) AS SaleDate FROM customer_estimate WHERE customer_estimate.status_id=3 GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                                  " " + strCondition + " order by t1.SaleDate asc";

                }
            }
            else
            {
                if (ddlSearchBy.SelectedValue == "5")
                {
                    strQ = " SELECT client_id,customers.status_id, customers.islead,customers.isCustomer,customers.customer_id, first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                                  " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date, company,CAST(1 AS BIT) AS IsEstimateActive " +
                                  " FROM customers " +
                                  " INNER JOIN  (SELECT  customer_estimate.customer_id,MIN(CONVERT(DATETIME,sale_date)) AS SaleDate FROM customer_estimate WHERE customers.islead = 1 and customer_estimate.status_id=3 AND job_number LIKE '%" + txtSearch.Text + "%' GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                                  "  order by customers.registration_date desc, customers.last_name1 asc";
                }
                else
                {
                    strQ = " SELECT client_id,customers.status_id,customers.islead,customers.isCustomer, customers.customer_id, first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                                  " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date,company, CAST(1 AS BIT) AS IsEstimateActive " +
                                  " FROM customers " +
                                  " LEFT OUTER JOIN   (SELECT  customer_estimate.customer_id,MIN(CONVERT(DATETIME,sale_date)) AS SaleDate FROM customer_estimate WHERE customer_estimate.status_id=3 GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                                  " " + strCondition + " order by customers.registration_date desc, customers.last_name1 asc";

                }
            }
        }

        IEnumerable<csCustomer> mList = _db.ExecuteQuery<csCustomer>(strQ, string.Empty).ToList();
        DataTable dt = csCommonUtility.LINQToDataTable(mList);
        if (dt.Rows.Count > 0)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = "islead = 1";
            if (dv.Count > 0)
            {
                Session.Add("sCustList", dv.ToTable());
                if (ddlItemPerPage.SelectedValue != "4")
                {
                    grdLeadList.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
                }
                else
                {
                    grdLeadList.PageSize = 200;
                }
                grdLeadList.DataSource = dv.ToTable();
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
            else
            {
                if (txtSearch.Text.Length > 0)
                {
                    dv.RowFilter = "isCustomer = 1";
                    if (dv.Count > 0)
                    {
                        Session.Add("CustSreach", txtSearch.Text);
                        Response.Redirect("customerlist.aspx");
                    }
                }
                else
                {
                    if (ddlItemPerPage.SelectedValue != "4")
                    {
                        grdLeadList.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
                    }
                    else
                    {
                        grdLeadList.PageSize = 200;
                    }
                    grdLeadList.DataSource = null;
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

            }
        }
        else
        {
            if (ddlItemPerPage.SelectedValue != "4")
            {
                grdLeadList.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
            }
            else
            {
                grdLeadList.PageSize = 200;
            }
            grdLeadList.DataSource = mList;
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
    }



    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddNew.ID, btnAddNew.GetType().Name, "Click"); 
        Session.Remove("LeadId");
        Response.Redirect("lead_details.aspx");
    }
    protected void grdLeadList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int ncid = Convert.ToInt32(grdLeadList.DataKeys[e.Row.RowIndex].Value.ToString());

            DataClassesDataContext _db = new DataClassesDataContext();

            int nid = Convert.ToInt32(e.Row.Cells[2].Text);
            string strLatestActivity = "";
            string strCallQ = "SELECT Description as CallActivity FROM CustomerCallLog WHERE customer_id = " + ncid + " and " +
             " CallLogID=(SELECT max(CallLogID) FROM CustomerCallLog where  customer_id =  " + ncid + "  ) ";
            IEnumerable<customer_CallNotes> Calllist = _db.ExecuteQuery<customer_CallNotes>(strCallQ, string.Empty);
            foreach (customer_CallNotes cus_Notet in Calllist)
            {
                strLatestActivity = cus_Notet.CallActivity;
            }
            Label lblActivity = (Label)e.Row.FindControl("lblActivity");
            Label lblActivity_r = (Label)e.Row.FindControl("lblActivity_r");
            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
            lblActivity.Text = strLatestActivity;

            if (strLatestActivity != "" && strLatestActivity.Length > 90)
            {
                lblActivity.Text = strLatestActivity.Substring(0, 90) + " ...";
                lblActivity.ToolTip = strLatestActivity;
                lblActivity_r.Text = strLatestActivity;
                lnkOpen.Visible = true;
            }
            else
            {
                lblActivity.Text = strLatestActivity;
                lblActivity_r.Text = strLatestActivity;
                lnkOpen.Visible = false;
            }


            //int nSuperintendentId = Convert.ToInt32(e.Row.Cells[8].Text);
            //if (nSuperintendentId > 0)
            //{
            //    user_info uinfo = _db.user_infos.Single(u => u.user_id == nSuperintendentId);
            //    e.Row.Cells[8].Text = uinfo.first_name + " " + uinfo.last_name;
            //}
            //else
            //{
            //    e.Row.Cells[8].Text = "";
            //}
            HyperLink hyp_Custd = (HyperLink)e.Row.FindControl("hyp_Custd");
            hyp_Custd.NavigateUrl = "lead_details.aspx?cid=" + ncid + "&eid=1";

            HyperLink hyp_DocumentManagement = (HyperLink)e.Row.FindControl("hyp_DocumentManagement");
            hyp_DocumentManagement.NavigateUrl = "DocumentManagement.aspx?cid=" + ncid + "&nbackId=2&eid=1";


            // Customer Address
            customer cust = new customer();
            cust = _db.customers.Single(c => c.customer_id == ncid);
            string strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
            //e.Row.Cells[2].Text = strAddress;

            sales_person sp = new sales_person();
            sp = _db.sales_persons.Single(s => s.sales_person_id == nid);
            e.Row.Cells[2].Text = sp.first_name + " " + sp.last_name + "<br/>" + Convert.ToDateTime(cust.registration_date).ToShortDateString();

            Label lblPhone = (Label)e.Row.FindControl("lblPhone");
            lblPhone.Text = cust.phone;
            lblPhone.Attributes.CssStyle.Add("padding", "5px 0 5px 0");

            // Customer Email
            HyperLink hypEmail = (HyperLink)e.Row.FindControl("hypEmail");
            hypEmail.Text = cust.email;
            //hypEmail.NavigateUrl = "mailto:" + cust.email + "?subject=Contact";
            hypEmail.NavigateUrl = "sendemailoutlook.aspx?custId=" + ncid;
            //hypEmail.Target = "MyWindow";

            // Customer Address in Google Map
            HyperLink hypAddress = (HyperLink)e.Row.FindControl("hypAddress");
            hypAddress.Text = strAddress;
            //hypAddress.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
            string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
            hypAddress.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;
            hypAddress.ToolTip = "Google Map";


            // Customer Messsage Center
            HyperLink hypMessage = (HyperLink)e.Row.FindControl("hypMessage");
            hypMessage.NavigateUrl = "customermessagecenteroutlook.aspx?cid=" + ncid + "&eid=1";


            HyperLink hyp_ProjectNotes = (HyperLink)e.Row.FindControl("hyp_ProjectNotes");
            hyp_ProjectNotes.NavigateUrl = "ProjectNotes.aspx?cid=" + ncid + "&TypeId=3&eid=1";

            HyperLink hyp_CallLog = (HyperLink)e.Row.FindControl("hyp_CallLog");
            hyp_CallLog.NavigateUrl = "CallLogInfo.aspx?cid=" + ncid + "&TypeId=3&eid=1";

            HyperLink hyp_SiteReview = (HyperLink)e.Row.FindControl("hyp_SiteReview");
            hyp_SiteReview.NavigateUrl = "sitereviewlist.aspx?nestid=1&nbackId=1&cid=" + ncid;



            //------- Customer Estimates
            DropDownList ddlEst = (DropDownList)e.Row.FindControl("ddlEst");
            HyperLink hyp_vendor = (HyperLink)e.Row.FindControl("hyp_vendor");
            HyperLink hypEstDetail = (HyperLink)e.Row.FindControl("hypEstDetail");
            HyperLink hypCostLoc = (HyperLink)e.Row.FindControl("hypCostLoc");
            HyperLink hypCommon = (HyperLink)e.Row.FindControl("hypCommon");
            HyperLink hypCommon2 = (HyperLink)e.Row.FindControl("hypCommon2");

            hypCommon2.Visible = false;
            hypCommon.Visible = true;

            hypCommon.Text = "New Estimate";
            hypCommon.NavigateUrl = "customer_locations.aspx?cid=" + ncid;

            HyperLink hyp_Allowance = (HyperLink)e.Row.FindControl("hyp_Allowance");

            HyperLink hyp_Section_Selection = (HyperLink)e.Row.FindControl("hyp_Section_Selection");
            HyperLink hyp_MaterialTracking = (HyperLink)e.Row.FindControl("hyp_MaterialTracking");

            Label lblJobJost = (Label)e.Row.FindControl("lblJobJost");




            string strQ = "select * from customer_estimate where customer_id=" + ncid + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            IEnumerable<customer_estimate_model> list = _db.ExecuteQuery<customer_estimate_model>(strQ, string.Empty);

            ddlEst.DataSource = list;
            ddlEst.DataTextField = "estimate_name";
            ddlEst.DataValueField = "estimate_id";
            ddlEst.DataBind();


            //var resultCount = (from ce in _db.customer_estimates
            //                   where ce.customer_id == ncid && ce.client_id == 1 && ce.status_id == 3
            //                   select ce.estimate_id);
            //int nEstCount = resultCount.Count();

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




                string strQ2 = "select * from customer_estimate where customer_id=" + ncid + " AND estimate_id != " + Convert.ToInt32(ddlEst.SelectedValue) + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                IEnumerable<customer_estimate_model> list2 = _db.ExecuteQuery<customer_estimate_model>(strQ2, string.Empty);
                string strOtherJobNum = string.Empty;
                foreach (customer_estimate_model cus_est2 in list2)
                {
                    if (cus_est2.job_number != null)
                    {
                        if (strOtherJobNum == "")
                        {
                            if (Convert.ToBoolean(cus_est2.IsEstimateActive))
                                strOtherJobNum = "<p>" + cus_est2.job_number.Trim() + "<p/>";
                            else
                                strOtherJobNum = "<p>" + cus_est2.job_number.Trim() + "<p/>" + "<p>" + "Est.InActive" + "<p/>";

                        }
                        else
                        {
                            if (Convert.ToBoolean(cus_est2.IsEstimateActive))
                                strOtherJobNum = strOtherJobNum + "<p>" + cus_est2.job_number.Trim() + "<p/>";
                            else
                                strOtherJobNum = strOtherJobNum + "<p>" + cus_est2.job_number.Trim() + "<p/>" + "<p>" + "Est.InActive" + "<p/>";

                        }
                    }
                }

                string strQ1 = "select * from customer_estimate where customer_id=" + ncid + " AND estimate_id = " + Convert.ToInt32(ddlEst.SelectedValue) + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                IEnumerable<customer_estimate_model> list1 = _db.ExecuteQuery<customer_estimate_model>(strQ1, string.Empty);



                foreach (customer_estimate_model cus_est in list1)
                {
                    string strJobNum = cus_est.job_number;
                    string strestimateName = cus_est.estimate_name;
                    int nestid = Convert.ToInt32(cus_est.estimate_id);
                    int nest_status_id = Convert.ToInt32(cus_est.status_id);

                    hypMessage.NavigateUrl = "customermessagecenteroutlook.aspx?cid=" + ncid + "&eid=" + nestid;
                    hyp_ProjectNotes.NavigateUrl = "ProjectNotes.aspx?cid=" + ncid + "&TypeId=3&eid=" + nestid;
                    hyp_CallLog.NavigateUrl = "CallLogInfo.aspx?cid=" + ncid + "&TypeId=3&eid=" + nestid;
                    hyp_DocumentManagement.NavigateUrl = "DocumentManagement.aspx?cid=" + ncid + "&nbackId=2&eid=" + nestid;
                    hyp_Custd.NavigateUrl = "lead_details.aspx?cid=" + ncid + "&eid=" + nestid;

                    if (nest_status_id == 3)
                    {
                        //hypSurvey Exit Questionnaire

                        hypEstDetail.ToolTip = "(SOLD) View Details ";
                        hypEstDetail.ImageUrl = "~/images/view_details_sold.png";

                        Label lblActiveEst = (Label)e.Row.FindControl("lblActiveEst");

                        if (!Convert.ToBoolean(cus_est.IsEstimateActive))
                        {
                            lblActiveEst.Text = "<p>" + "Est.InActive" + "<p/>";
                        }
                        else
                        {
                            lblActiveEst.Text = "";
                        }


                        // hyp_Selection.NavigateUrl = "SelectionSheetNew.aspx?eid=" + nestid + "&cid=" + ncid;


                    }
                    else
                    {

                        hypEstDetail.ToolTip = "View Details";//strestimateName;
                        hypEstDetail.ImageUrl = "~/images/view_details.png";
                    }
                    hypCostLoc.NavigateUrl = "ProjectSummaryReport.aspx?TypeId=1&eid=" + nestid + "&cid=" + ncid;
                    hypCostLoc.Target = "_blank";
                    hyp_vendor.NavigateUrl = "leadvendorcost.aspx?eid=" + nestid + "&cid=" + ncid;
                    hypEstDetail.NavigateUrl = "customer_locations.aspx?eid=" + nestid + "&cid=" + ncid;
                    hyp_Allowance.NavigateUrl = "AllowanceReport.aspx?eid=" + nestid + "&cid=" + ncid;
                    hyp_SiteReview.NavigateUrl = "sitereviewlist.aspx?nestid=" + nestid + "&nbackId=1&cid=" + ncid;
                    hyp_Section_Selection.NavigateUrl = "selectionofsection.aspx?eid=" + nestid + "&cid=" + ncid;
                    hyp_MaterialTracking.NavigateUrl = "material_traknig.aspx?eid=" + nestid + "&cid=" + ncid;

                }

                if (ddlEst.SelectedValue != "")
                {
                    decimal TotalExCom_WithIntevcise = GetRetailTotal(Convert.ToInt32(ddlEst.SelectedValue), ncid);
                    lblJobJost.Text = "Estimate Amount: " + TotalExCom_WithIntevcise.ToString("c");
                }


                if (Session["tools"] != null)
                {
                    string tools = Session["tools"].ToString();
                    if (tools.Contains("Message"))
                        hypMessage.Visible = true;
                    else
                        hypMessage.Visible = false;
                    if (tools.Contains("Vendor"))
                        hyp_vendor.Visible = true;
                    else
                        hyp_vendor.Visible = false;
                    if (tools.Contains("ProjectNotes"))
                        hyp_ProjectNotes.Visible = true;
                    else
                        hyp_ProjectNotes.Visible = false;
                    if (tools.Contains("ActivityLog"))
                        hyp_CallLog.Visible = true;
                    else
                        hyp_CallLog.Visible = false;
                    if (tools.Contains("AllowanceReport"))
                        hyp_Allowance.Visible = true;
                    else
                        hyp_Allowance.Visible = false;
                    if (tools.Contains("Selection"))
                        hyp_Section_Selection.Visible = true;
                    else
                        hyp_Section_Selection.Visible = false;
                    if (tools.Contains("SiteReview"))
                        hyp_SiteReview.Visible = true;
                    else
                        hyp_SiteReview.Visible = false;
                    if (tools.Contains("DocumentManagement"))
                        hyp_DocumentManagement.Visible = true;
                    else
                        hyp_DocumentManagement.Visible = false;
                    if (tools.Contains("ProjectSummary"))
                        hypCostLoc.Visible = true;
                    else
                        hypCostLoc.Visible = false;
                    if (tools.Contains("MaterialTracking"))
                        hyp_MaterialTracking.Visible = true;
                    else
                        hyp_MaterialTracking.Visible = false;
                }
            }
            else
            {
                ddlEst.Visible = false;
                hypEstDetail.Visible = false;
                hypCostLoc.Visible = false;
                hyp_Allowance.Visible = false;
                hyp_Section_Selection.Visible = false;
                hyp_MaterialTracking.Visible = false;
                hyp_vendor.Visible = false;
                // hyp_SiteReview.Visible = false;
                //hyp_DocumentManagement.Visible = false;
                hypCommon2.Visible = true;
                hypCommon.Visible = false;

                hypCommon2.Text = "New Estimate";
                hypCommon2.NavigateUrl = "customer_locations.aspx?cid=" + ncid;
            }


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
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdLeadList.ID, grdLeadList.GetType().Name, "PageIndexChanging"); 
        GetCustomersNew(e.NewPageIndex);
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        Session.Remove("LeadId");
        GetCustomersNew(0);
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNext.ID, btnNext.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetCustomersNew(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        GetCustomersNew(nCurrentPage - 2);
    }
    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "SelectedIndexChanged"); 
        Session.Remove("LeadId");
        GetCustomersNew(0);
    }
    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlStatus.ID, ddlStatus.GetType().Name, "SelectedIndexChanged"); 
        Session.Remove("LeadId");
        txtSearch.Text = string.Empty;
        GetCustomersNew(0);
    }
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        ddlStatus.SelectedValue = "2";
        // ddlStatus.SelectedIndex = -1;
        ddlSalesRep.SelectedIndex = -1;
        ddlLeadSource.SelectedIndex = -1;
        ddlSuperintendent.SelectedIndex = -1;
        Session.Remove("LeadId");
        GetCustomersNew(0);

    }
    protected void btnExpCustList_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnExpCustList.ID, btnExpCustList.GetType().Name, "Click"); 
        string strID = string.Empty;
        DataTable tblCustList = (DataTable)Session["sCustList"];
        if (tblCustList.Rows.Count > 0)
        {
            foreach (DataRow dr in tblCustList.Rows)
            {
                if (strID == "")
                    strID = dr["customer_id"].ToString();
                else
                    strID += "," + dr["customer_id"].ToString();
            }
            string strQ = " SELECT customers.customer_id,customer_estimate.estimate_id,customer_estimate.job_number,ISNULL(t1.Total_Price,0) AS Total_Price,ISNULL (estimate_payments.new_total_with_tax,0) AS NewTotal_Price, " +
                          " customers.registration_date,customer_estimate.estimate_name,customer_estimate.sale_date, " +
                          " customers.first_name1, customers.last_name1, customers.address,  customers.city, customers.state, customers.zip_code, customers.email, customers.phone, customer_estimate.sales_person_id, " +
                          " sales_person.first_name +' '+last_name AS SalesRep,  customers.status_id, customers.notes, customers.appointment_date, lead_source.lead_source_id,lead_source.lead_name " +
                          " FROM customers  " +
                          " INNER JOIN customer_estimate ON  customer_estimate.customer_id = customers.customer_id " +
                          " INNER JOIN lead_source ON  lead_source.lead_source_id = customers.lead_source_id " +
                          " INNER JOIN sales_person ON  sales_person.sales_person_id = customers.sales_person_id " +
                          " LEFT OUTER JOIN (SELECT pd.customer_id,pd.estimate_id,ISNULL(SUM(pd.total_retail_price),0) AS Total_Price FROM pricing_details pd  GROUP BY pd.customer_id,pd.estimate_id) AS t1 ON t1.customer_id = customer_estimate.customer_id AND t1.estimate_id = customer_estimate.estimate_id " +
                          " LEFT OUTER JOIN estimate_payments ON estimate_payments.customer_id = customer_estimate.customer_id AND estimate_payments.estimate_id = customer_estimate.estimate_id " +
                          " WHERE customers.customer_id in (" + strID + ")  order by CONVERT(DATETIME,customers.registration_date) desc,customers.first_name1  asc";

            DataTable dtReport = csCommonUtility.GetDataTable(strQ);

            DataTable tmpTable = LoadTmpDataTable();
            DataClassesDataContext _db = new DataClassesDataContext();

            foreach (DataRow dr in dtReport.Rows)
            {
                decimal nTotal_Price = 0;

                DateTime dtregistration_date = Convert.ToDateTime(dr["registration_date"]);
                DateTime dtappointment_date = Convert.ToDateTime(dr["appointment_date"]);

                if (Convert.ToDecimal(dr["NewTotal_Price"]) == 0)
                {
                    nTotal_Price = Convert.ToDecimal(dr["Total_Price"]);
                }
                else
                {
                    nTotal_Price = Convert.ToDecimal(dr["NewTotal_Price"]);

                }
                DataRow drNew = tmpTable.NewRow();


                drNew["First Name"] = dr["first_name1"];
                drNew["Last Name"] = dr["last_name1"];
                drNew["Phone"] = dr["phone"];
                drNew["Email"] = dr["email"];
                drNew["Address"] = dr["address"].ToString() + ' ' + dr["city"] + ',' + ' ' + dr["state"] + ' ' + dr["zip_code"];
                drNew["Entry Date"] = dtregistration_date.ToShortDateString();
                if (dtappointment_date.Year == 1999 || dtappointment_date.Year == 1900)
                {
                    drNew["Appt Date"] = "";
                }
                else
                {
                    drNew["Appt Date"] = dtappointment_date.ToShortDateString();

                }
                drNew["Lead Source"] = dr["lead_name"];
                drNew["Estimate"] = dr["estimate_name"];
                drNew["Sale Date"] = dtregistration_date.ToShortDateString(); //dr["sale_date"];
                drNew["Sales Rep"] = dr["SalesRep"];
                drNew["Sale Amount"] = nTotal_Price.ToString("c");
                tmpTable.Rows.Add(drNew);
            }

            Response.Clear();
            Response.ClearHeaders();

            using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
            {
                writer.WriteAll(tmpTable, true);
            }
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=CustomerList.csv");
            Response.End();
        }
    }
    private DataTable LoadTmpDataTable()
    {
        DataTable table = new DataTable();

        // table.Columns.Add("Company", typeof(string));

        table.Columns.Add("First Name", typeof(string));
        table.Columns.Add("Last Name", typeof(string));
        table.Columns.Add("Phone", typeof(string));
        table.Columns.Add("Email", typeof(string));
        table.Columns.Add("Address", typeof(string));
        table.Columns.Add("Entry Date", typeof(string));
        table.Columns.Add("Appt Date", typeof(string));
        table.Columns.Add("Lead Source", typeof(string));
        table.Columns.Add("Estimate", typeof(string));
        table.Columns.Add("Sale Date", typeof(string));
        table.Columns.Add("Sales Rep", typeof(string));
        table.Columns.Add("Sale Amount", typeof(string));
        return table;
    }
    protected void ddlSalesRep_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSalesRep.ID, ddlSalesRep.GetType().Name, "SelectedIndexChanged"); 
        Session.Remove("LeadId");
        txtSearch.Text = string.Empty;
        GetCustomersNew(0);
    }
    protected void ddlSuperintendent_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSuperintendent.ID, ddlSuperintendent.GetType().Name, "SelectedIndexChanged"); 
        Session.Remove("LeadId");
        txtSearch.Text = string.Empty;
        GetCustomersNew(0);
    }
    protected void ddlLeadSource_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlLeadSource.ID, ddlLeadSource.GetType().Name, "SelectedIndexChanged"); 
        Session.Remove("LeadId");
        txtSearch.Text = string.Empty;
        GetCustomersNew(0);
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
        else if (ddlSearchBy.SelectedValue == "6")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetCompany";
        }
        else if (ddlSearchBy.SelectedValue == "7")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetPhone";
        }

        GetCustomersNew(0);
    }
    protected void btnSalesCalendar_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("schedulecalendar.aspx?TypeID=2");
    }
    protected void btnLeadReport_Click(object sender, EventArgs e)
    {
        Response.Redirect("lead_report.aspx");
    }
    protected void lnkOpen_Click(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblActivity = gRow.Cells[4].Controls[0].FindControl("lblActivity") as Label;
        Label lblActivity_r = gRow.Cells[4].Controls[1].FindControl("lblActivity_r") as Label;
        LinkButton lnkOpen = gRow.Cells[4].Controls[2].FindControl("lnkOpen") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblActivity.Visible = false;
            lblActivity_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblActivity.Visible = true;
            lblActivity_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }

    protected void Load_Est_Info(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        GridViewRow diitem = ((GridViewRow)((DropDownList)sender).NamingContainer);
        GridView grdCustomerList = (GridView)diitem.NamingContainer;
        DropDownList ddlEst = (DropDownList)diitem.FindControl("ddlEst");
        HyperLink hypEstDetail = (HyperLink)diitem.FindControl("hypEstDetail");
        HyperLink hypCostLoc = (HyperLink)diitem.FindControl("hypCostLoc");
        HyperLink hypCommon = (HyperLink)diitem.FindControl("hypCommon");
        HyperLink hyp_Allowance = (HyperLink)diitem.FindControl("hyp_Allowance");
        HyperLink hyp_SiteReview = (HyperLink)diitem.FindControl("hyp_SiteReview");
        HyperLink hyp_Section_Selection = (HyperLink)diitem.FindControl("hyp_Section_Selection");
        HyperLink hyp_MaterialTracking = (HyperLink)diitem.FindControl("hyp_MaterialTracking");
        HyperLink hyp_vendor = (HyperLink)diitem.FindControl("hyp_vendor");
        HyperLink hyp_Custd = (HyperLink)diitem.FindControl("hyp_Custd");
        HyperLink hypMessage = (HyperLink)diitem.FindControl("hypMessage");
        HyperLink hyp_ProjectNotes = (HyperLink)diitem.FindControl("hyp_ProjectNotes");
        HyperLink hyp_CallLog = (HyperLink)diitem.FindControl("hyp_CallLog");
        HyperLink hyp_DocumentManagement = (HyperLink)diitem.FindControl("hyp_DocumentManagement");

        Label lblJobJost = (Label)diitem.FindControl("lblJobJost");

        int ncid = Convert.ToInt32(grdCustomerList.DataKeys[diitem.RowIndex].Values[0]);

        decimal TotalExCom_WithIntevcise = GetRetailTotal(Convert.ToInt32(ddlEst.SelectedValue), ncid);
        lblJobJost.Text = "Estimate Amount: " + TotalExCom_WithIntevcise.ToString("c");

        string strQ2 = "select * from customer_estimate where customer_id=" + ncid + " AND estimate_id != " + Convert.ToInt32(ddlEst.SelectedValue) + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        IEnumerable<customer_estimate_model> list2 = _db.ExecuteQuery<customer_estimate_model>(strQ2, string.Empty);


        string strQ1 = "select * from customer_estimate where customer_id=" + ncid + " AND estimate_id = " + Convert.ToInt32(ddlEst.SelectedValue) + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        IEnumerable<customer_estimate_model> list1 = _db.ExecuteQuery<customer_estimate_model>(strQ1, string.Empty);

        foreach (customer_estimate_model cus_est in list1)
        {
            string strJobNum = cus_est.job_number;
            string strestimateName = cus_est.estimate_name;
            int nestid = Convert.ToInt32(cus_est.estimate_id);
            int nest_status_id = Convert.ToInt32(cus_est.status_id);



            if (nest_status_id == 3)
            {


                hypEstDetail.ToolTip = "(SOLD) View Details ";
                hypEstDetail.ImageUrl = "~/images/view_details_sold.png";

            }
            else
            {

                hypEstDetail.ToolTip = "View Details ";
                hypEstDetail.ImageUrl = "~/images/view_details.png";


            }

            if (hdnEmailType.Value == "1")
                hypMessage.NavigateUrl = "customermessagecenteroutlook.aspx?cid=" + ncid + "&eid=" + nestid;
            else
                hypMessage.NavigateUrl = "customermessagecenter.aspx?cid=" + ncid + "&eid=" + nestid;

            hypCostLoc.NavigateUrl = "ProjectSummaryReport.aspx?TypeId=1&eid=" + nestid + "&cid=" + ncid;
            hypCostLoc.Target = "_blank";
            hypEstDetail.NavigateUrl = "customer_locations.aspx?eid=" + nestid + "&cid=" + ncid;
            hyp_Allowance.NavigateUrl = "AllowanceReport.aspx?eid=" + nestid + "&cid=" + ncid;
            hyp_SiteReview.NavigateUrl = "sitereviewlist.aspx?nestid=" + nestid + "&nbackId=1&cid=" + ncid;
            hyp_Section_Selection.NavigateUrl = "selectionofsection.aspx?eid=" + nestid + "&cid=" + ncid;
            hyp_MaterialTracking.NavigateUrl = "material_traknig.aspx?eid=" + nestid + "&cid=" + ncid;
            hyp_vendor.NavigateUrl = "leadvendorcost.aspx?eid=" + nestid + "&cid=" + ncid;
            hyp_Custd.NavigateUrl = "customer_details.aspx?cid=" + ncid + "&eid=" + nestid;
            hyp_ProjectNotes.NavigateUrl = "ProjectNotes.aspx?cid=" + ncid + "&TypeId=2&eid=" + nestid;
            hyp_CallLog.NavigateUrl = "CallLogInfo.aspx?cid=" + ncid + "&TypeId=2&eid=" + nestid;
            hyp_DocumentManagement.NavigateUrl = "DocumentManagement.aspx?cid=" + ncid + "&nbackId=1&eid=" + nestid;
        }

    }
    private decimal GetRetailTotal(int EstID, int ncustid)
    {
        decimal dRetail = 0;
        DataClassesDataContext _db = new DataClassesDataContext();


        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        decimal total_incentives = 0;
        estimate_payment esp = new estimate_payment();

        if (_db.estimate_payments.Where(ep => ep.estimate_id == EstID && ep.customer_id == ncustid && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() != null)
        {
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == EstID && ep.customer_id == ncustid && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
            dRetail = Convert.ToDecimal(esp.new_total_with_tax);
            total_incentives = Convert.ToDecimal(esp.total_incentives);
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);

            if (dRetail == 0)
                dRetail = project_subtotal + tax_amount;
        }
        else
        {
            var result = (from pd in _db.pricing_details
                          where (from clc in _db.customer_locations
                                 where clc.estimate_id == EstID && clc.customer_id == ncustid && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                 select clc.location_id).Contains(pd.location_id) &&
                                 (from cs in _db.customer_sections
                                  where cs.estimate_id == EstID && cs.customer_id == ncustid && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                  select cs.section_id).Contains(pd.section_level) && pd.estimate_id == EstID && pd.customer_id == ncustid && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A"
                          select pd.total_retail_price);
            int n = result.Count();
            if (result != null && n > 0)
                dRetail = result.Sum();
        }



        return dRetail;
    }
}
