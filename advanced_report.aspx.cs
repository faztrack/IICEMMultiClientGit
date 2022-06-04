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
using System.Globalization;

public partial class advanced_report : System.Web.UI.Page
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
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                hdnClientId.Value = ((userinfo)Session["oUser"]).client_id.ToString();
                hdnDivisionName.Value = ((userinfo)Session["oUser"]).divisionName.ToString();
            }
            if (Page.User.IsInRole("rpt004") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            BindSavedSearch();
            BindSalesPerson();
            BindLeadStatus();
            BindLeadSource();
            BindAllLocation();
            BindAllSection();
            GetTblColumns();
            LoadSearch(2);
            ddlSaveSearches.SelectedValue = "2";
            DisbaleItem(2);
            GetCustomersNew(0);

            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSaveSearch", "btnUpdateSearch", "btnDeleteSearch", "ddlSaveSearches", });
        }
    }

    private void BindSavedSearch()
    {

        ddlSaveSearches.Items.Clear();

        DataClassesDataContext _db = new DataClassesDataContext();

        userinfo objUser = (userinfo)Session["oUser"];

        List<advanced_search_report> list = _db.advanced_search_reports.Where(s => s.user_id == objUser.user_id).ToList();
        ddlSaveSearches.DataSource = list.OrderBy(o => o.search_name).ToList(); ;
        ddlSaveSearches.DataTextField = "search_name";
        ddlSaveSearches.DataValueField = "advanced_report_id";
        ddlSaveSearches.DataBind();


        ddlSaveSearches.Items.Insert(0, new ListItem("Select", "0"));

        ddlSaveSearches.SelectedIndex = 0;
    }
    private void BindSalesPerson()
    {
        string strQ = "select first_name+' '+last_name AS sales_person_name,sales_person_id from sales_person WHERE is_active=1 and is_sales=1 " + csCommonUtility.GetSalesPersonSql(hdnDivisionName.Value) + " order by sales_person_id asc";
        DataTable dt = csCommonUtility.GetDataTable(strQ);
        lsbSalesRep.DataSource = dt;
        lsbSalesRep.DataTextField = "sales_person_name";
        lsbSalesRep.DataValueField = "sales_person_id";
        lsbSalesRep.DataBind();

    }
    private void BindLeadStatus()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var LeadStatus = from st in _db.lead_status
                         where st.lead_status_id != 7 && st.lead_status_id != 4 && st.lead_status_id != 5 
                         orderby st.lead_status_id
                         select st;
        lsbStatus.DataSource = LeadStatus;
        lsbStatus.DataTextField = "lead_status_name";
        lsbStatus.DataValueField = "lead_status_id";
        lsbStatus.DataBind();
    }

    private void BindLeadSource()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var item = from l in _db.lead_sources
                   where l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && l.is_active == Convert.ToBoolean(1)
                   orderby l.lead_name
                   select l;
        lsbLeadSource.DataSource = item;
        lsbLeadSource.DataTextField = "lead_name";
        lsbLeadSource.DataValueField = "lead_source_id";
        lsbLeadSource.DataBind();

    }


    private void BindAllLocation()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
       
        lsbLocation.DataSource = _db.locations.Where(cl => cl.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && Convert.ToBoolean(cl.is_active) == true).ToList();
        lsbLocation.DataTextField = "location_name";
        lsbLocation.DataValueField = "location_id";
        lsbLocation.DataBind();
    }
    private void BindAllSection()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        lsbSection.DataSource = _db.sectioninfos.Where(si => si.client_id.ToString().Contains(hdnClientId.Value) && si.parent_id == 0).ToList();
        lsbSection.DataTextField = "section_name";
        lsbSection.DataValueField = "section_id";
        lsbSection.DataBind();
    }
    private void GetTblColumns()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var item = from v in _db.tbl_rc_customers
                   orderby v.rc_column_id
                   select v;
        lsbDisplayColumn.DataSource = item;
        lsbDisplayColumn.DataTextField = "rc_column_name";
        lsbDisplayColumn.DataValueField = "rc_db_tbl_column";
        lsbDisplayColumn.DataBind();

    }
    protected void GetCustomersNew(int nPageNo)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            userinfo obj = (userinfo)Session["oUser"];
            int nSalePersonId = obj.sales_person_id;
            int nRoleId = obj.role_id;
            grdCustomerReport.PageIndex = nPageNo;


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

            if (lsbLeadSource.SelectedValue != null)
            {
                string strLeadSource = string.Empty;
                foreach (ListItem item in lsbLeadSource.Items)
                {
                    if (item.Selected)
                    {
                        if (strLeadSource.Length == 0)
                            strLeadSource = item.Value;
                        else
                            strLeadSource += ", " + item.Value;

                    }
                }
                if (strLeadSource.Length > 0)
                {
                    if (strCondition.Length > 0)
                    {
                        strCondition += " AND customers.lead_source_id IN (" + strLeadSource + ")  ";
                    }
                    else
                    {
                        strCondition += " customers.lead_source_id IN (" + strLeadSource + ")  ";
                    }
                }
            }
            if (lsbStatus.SelectedValue != null)
            {
                string strCustStatus = string.Empty;
                string strLeadStatus = string.Empty;
                string strSoldStatus = string.Empty;

                foreach (ListItem item in lsbStatus.Items)
                {
                    if (item.Selected)
                    {
                        if (Convert.ToInt32(item.Value) == 6)
                        {
                            strSoldStatus = "6";
                        }
                        else if (Convert.ToInt32(item.Value) < 6)
                        {
                            if (strCustStatus.Length == 0)
                                strCustStatus = item.Value;
                            else
                                strCustStatus += ", " + item.Value;
                        }
                        else if (Convert.ToInt32(item.Value) > 6)
                        {
                            if (strLeadStatus.Length == 0)
                                strLeadStatus = item.Value;
                            else
                                strLeadStatus += ", " + item.Value;
                        }

                    }
                }
                if (strLeadStatus.Length > 0)
                {
                    if (strCondition.Length > 0)
                    {
                        strCondition += " AND customers.lead_status_id IN  (" + strLeadStatus + ")  ";
                    }
                    else
                    {
                        strCondition += " customers.lead_status_id IN  (" + strLeadStatus + ")  ";
                    }
                }
                if (strSoldStatus.Length > 0)
                {
                    if (strCondition.Length > 0)
                    {
                        strCondition += " AND customer_estimate.status_id = 3 ";
                    }
                    else
                    {
                        strCondition += " customer_estimate.status_id = 3";
                    }
                }
                if (strCustStatus.Length > 0)
                {
                    if (strCondition.Length > 0)
                    {
                        strCondition += " AND customers.status_id IN  (" + strCustStatus + ")  ";
                    }
                    else
                    {
                        strCondition += " customers.status_id IN  (" + strCustStatus + ")  ";
                    }

                }


            }

            if (lsbSalesRep.SelectedValue != null)
            {
                string strSalesrep = string.Empty;
                foreach (ListItem item in lsbSalesRep.Items)
                {
                    if (item.Selected)
                    {
                        if (strSalesrep.Length == 0)
                            strSalesrep = item.Value;
                        else
                            strSalesrep += ", " + item.Value;

                    }
                }
                if (strSalesrep.Length > 0)
                {
                    if (strCondition.Length > 0)
                    {
                        strCondition += " AND customer_estimate.sales_person_id IN  (" + strSalesrep + ")  ";
                    }
                    else
                    {
                        strCondition += " customer_estimate.sales_person_id IN  (" + strSalesrep + ")  ";
                    }
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

                            strCondition = "  customers.appointment_date  between '" + dt1 + "' and '" + dt2 + "'";
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

                            strCondition = "  customers.registration_date  between '" + dt1 + "' and '" + dt2 + "'";
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
            if (txtSaleStartDate.Text.Trim() != "" && txtSaleEndDate.Text.Trim() != "")
            {
                try
                {
                    DateTime dt1 = Convert.ToDateTime(txtSaleStartDate.Text.Trim());
                    DateTime dt2 = Convert.ToDateTime(txtSaleEndDate.Text.Trim());
                    if (Convert.ToDateTime(txtSaleEndDate.Text.Trim()) >= Convert.ToDateTime(txtSaleStartDate.Text.Trim()))
                    {
                        if (strCondition.Length > 0)
                        {
                            strCondition += " AND CONVERT(DATETIME,customer_estimate.sale_date) between '" + dt1 + "' and '" + dt2 + "'";
                        }
                        else
                        {
                            strCondition += " CONVERT(DATETIME,customer_estimate.sale_date) between '" + dt1 + "' and '" + dt2 + "'";
                        }
                    }
                }
                catch
                {
                }
            }
            if (txtMinAmount.Text.Trim() != "" && txtMaxAmount.Text.Trim() != "")
            {
                try
                {
                    decimal dMinAmount = Convert.ToDecimal(txtMinAmount.Text.Replace("$", ""));
                    decimal dMAXAmount = Convert.ToDecimal(txtMaxAmount.Text.Replace("$", ""));

                    if (dMAXAmount >= dMinAmount)
                    {
                        if (strCondition.Length > 0)
                        {
                            strCondition += " AND estimate_payments.new_total_with_tax >= " + dMinAmount + " AND estimate_payments.new_total_with_tax <= " + dMAXAmount + "";
                        }
                        else
                        {
                            strCondition += " estimate_payments.new_total_with_tax >= " + dMinAmount + " AND estimate_payments.new_total_with_tax <= " + dMAXAmount + "";
                        }
                    }
                }
                catch
                {
                }
            }
            if (txtGreaterAmount.Text.Trim() != "")
            {
                try
                {
                    decimal dGreatAmount = Convert.ToDecimal(txtGreaterAmount.Text.Replace("$", ""));

                    if (dGreatAmount >= 0)
                    {
                        if (strCondition.Length > 0)
                        {
                            strCondition += " AND estimate_payments.new_total_with_tax >= " + dGreatAmount + " ";
                        }
                        else
                        {
                            strCondition += " estimate_payments.new_total_with_tax >= " + dGreatAmount + " ";
                        }
                    }
                }
                catch
                {
                }
            }
            if (txtLessAmount.Text.Trim() != "")
            {
                try
                {
                    decimal dLessAmount = Convert.ToDecimal(txtLessAmount.Text.Replace("$", ""));

                    if (dLessAmount >= 0)
                    {
                        if (strCondition.Length > 0)
                        {
                            strCondition += " AND estimate_payments.new_total_with_tax <= " + dLessAmount + " ";
                        }
                        else
                        {
                            strCondition += " estimate_payments.new_total_with_tax <= " + dLessAmount + " ";
                        }
                    }
                }
                catch
                {
                }
            }
            if (txtMinRcvdAmount.Text.Trim() != "" && txtMaxRcvdAmount.Text.Trim() != "")
            {
                try
                {
                    decimal dMinrAmount = Convert.ToDecimal(txtMinRcvdAmount.Text.Replace("$", ""));
                    decimal dMAXrAmount = Convert.ToDecimal(txtMaxRcvdAmount.Text.Replace("$", ""));

                    if (dMAXrAmount >= dMinrAmount)
                    {
                        if (strCondition.Length > 0)
                        {
                            strCondition += " AND t1.RecievedAmount >= " + dMinrAmount + " AND t1.RecievedAmount <= " + dMAXrAmount + "";
                        }
                        else
                        {
                            strCondition += " t1.RecievedAmount >= " + dMinrAmount + " AND t1.RecievedAmount <= " + dMAXrAmount + "";
                        }
                    }
                }
                catch
                {
                }
            }
            if (txtGreaterRcvdAmount.Text.Trim() != "")
            {
                try
                {
                    decimal dGreatrAmount = Convert.ToDecimal(txtGreaterRcvdAmount.Text.Replace("$", ""));

                    if (dGreatrAmount >= 0)
                    {
                        if (strCondition.Length > 0)
                        {
                            strCondition += " AND t1.RecievedAmount >= " + dGreatrAmount + " ";
                        }
                        else
                        {
                            strCondition += " t1.RecievedAmount >= " + dGreatrAmount + " ";
                        }
                    }
                }
                catch
                {
                }
            }
            if (txtLessRcvdAmount.Text.Trim() != "")
            {
                try
                {
                    decimal dLessrAmount = Convert.ToDecimal(txtLessRcvdAmount.Text.Replace("$", ""));

                    if (dLessrAmount >= 0)
                    {
                        if (strCondition.Length > 0)
                        {
                            strCondition += " AND t1.RecievedAmount <= " + dLessrAmount + " ";
                        }
                        else
                        {
                            strCondition += " t1.RecievedAmount <= " + dLessrAmount + " ";
                        }
                    }
                }
                catch
                {
                }
            }
            if (txtMinDueAmount.Text.Trim() != "" && txtMaxDueAmount.Text.Trim() != "")
            {
                try
                {
                    decimal dMinDAmount = Convert.ToDecimal(txtMinDueAmount.Text.Replace("$", ""));
                    decimal dMAXDAmount = Convert.ToDecimal(txtMaxDueAmount.Text.Replace("$", ""));

                    if (dMAXDAmount >= dMinDAmount)
                    {
                        if (strCondition.Length > 0)
                        {
                            strCondition += " AND (ISNULL(estimate_payments.new_total_with_tax,0) - t1.RecievedAmount ) >= " + dMinDAmount + " AND (ISNULL(estimate_payments.new_total_with_tax,0) - t1.RecievedAmount ) <= " + dMAXDAmount + "";
                        }
                        else
                        {
                            strCondition += " (ISNULL(estimate_payments.new_total_with_tax,0) - t1.RecievedAmount ) >= " + dMinDAmount + " AND (ISNULL(estimate_payments.new_total_with_tax,0) - t1.RecievedAmount ) <= " + dMAXDAmount + "";
                        }
                    }
                }
                catch
                {
                }
            }
            if (txtDueGreaterAmount.Text.Trim() != "")
            {
                try
                {
                    decimal dGreatDAmount = Convert.ToDecimal(txtDueGreaterAmount.Text.Replace("$", ""));

                    if (dGreatDAmount >= 0)
                    {
                        if (strCondition.Length > 0)
                        {
                            strCondition += " AND (ISNULL(estimate_payments.new_total_with_tax,0) - t1.RecievedAmount ) >= " + dGreatDAmount + " ";
                        }
                        else
                        {
                            strCondition += " (ISNULL(estimate_payments.new_total_with_tax,0) - t1.RecievedAmount ) >= " + dGreatDAmount + " ";
                        }
                    }
                }
                catch
                {
                }
            }
            if (txtLessDueAmount.Text.Trim() != "")
            {
                try
                {
                    decimal dLessDAmount = Convert.ToDecimal(txtLessDueAmount.Text.Replace("$", ""));

                    if (dLessDAmount >= 0)
                    {
                        if (strCondition.Length > 0)
                        {
                            strCondition += " AND (ISNULL(estimate_payments.new_total_with_tax,0) - t1.RecievedAmount ) <= " + dLessDAmount + " ";
                        }
                        else
                        {
                            strCondition += " (ISNULL(estimate_payments.new_total_with_tax,0) - t1.RecievedAmount ) <= " + dLessDAmount + " ";
                        }
                    }
                }
                catch
                {
                }
            }
            if (lsbLocation.SelectedValue != null)
            {
                string strLocationId = string.Empty;
                foreach (ListItem item in lsbLocation.Items)
                {
                    if (item.Selected)
                    {
                        if (strLocationId.Length == 0)
                            strLocationId = item.Value;
                        else
                            strLocationId += ", " + item.Value;

                    }
                }
                if (strLocationId.Length > 0)
                {
                    if (strCondition.Length > 0)
                    {
                        strCondition += " AND customers.customer_id IN (SELECT Distinct customer_locations.customer_id FROM customer_locations WHERE location_id IN  (" + strLocationId + ") ) ";
                    }
                    else
                    {
                        strCondition += " customers.customer_id IN (SELECT Distinct customer_locations.customer_id FROM customer_locations WHERE location_id IN  (" + strLocationId + ") ) ";
                    }
                }
            }
            if (lsbSection.SelectedValue != null)
            {
                string strSectionId = string.Empty;
                foreach (ListItem item in lsbSection.Items)
                {
                    if (item.Selected)
                    {
                        if (strSectionId.Length == 0)
                            strSectionId = item.Value;
                        else
                            strSectionId += ", " + item.Value;

                    }
                }
                if (strSectionId.Length > 0)
                {
                    if (strCondition.Length > 0)
                    {
                        strCondition += " AND customers.customer_id IN (SELECT Distinct customer_sections.customer_id FROM customer_sections WHERE section_id IN  (" + strSectionId + ") ) ";
                    }
                    else
                    {
                        strCondition += " customers.customer_id IN (SELECT Distinct customer_sections.customer_id FROM customer_sections WHERE section_id IN  (" + strSectionId + ") ) ";
                    }
                }
            }


            if (strCondition.Length > 0)
            {
                strCondition = "Where " + strCondition;
            }
            string strQ = string.Empty;
            string nColmns = "";
            //string nColmnsText = "";
            List<string> nColmnsText = new List<string>();

            foreach (ListItem item in lsbDisplayColumn.Items)
            {
                var Text = item.Text;
                var value = item.Value;
                nColmns += item.Value + ", ";
                //  nColmnsText += item.Text + ", ";
                nColmnsText.Add(item.Text);
            }


            if (nColmns != "")
            {
                strQ = " SELECT " + nColmns.Trim().TrimEnd(',') + " " +
                            " FROM customers " +
                            " LEFT OUTER JOIN  customer_estimate ON customer_estimate.customer_id = customers.customer_id " +
                            " LEFT OUTER JOIN  estimate_payments ON estimate_payments.customer_id = customer_estimate.customer_id and estimate_payments.estimate_id = customer_estimate.estimate_id " +
                            " INNER JOIN lead_status ON lead_status.lead_status_id = customers.lead_status_id " +
                            " INNER JOIN lead_source ON lead_source.lead_source_id = customers.lead_source_id " +
                            " INNER JOIN sales_person ON sales_person.sales_person_id = customer_estimate.sales_person_id " +
                            " LEFT OUTER JOIN (SELECT customer_id,estimate_id,SUM(pay_amount) AS RecievedAmount from New_partial_payment Group By customer_id, estimate_id) as t1 ON t1.customer_id = customer_estimate.customer_id  AND  t1.estimate_id = customer_estimate.estimate_id " + strCondition + " ";
            }
            else
            {
                strQ = " SELECT ISNULL(customer_estimate.job_number,'') as job_number, customers.first_name1+' '+ customers.last_name1 AS customer_name, customers.address, customers.cross_street, customers.city, customers.state, customers.zip_code, customers.fax, customers.email, customers.phone,customers.mobile,customer_estimate.estimate_name,sales_person.first_name+' '+sales_person.last_name AS SalesRep, CASE customer_estimate.status_id WHEN 1 THEN 'Pending' WHEN 3 THEN 'Sold' END as [Status],lead_status.lead_status_name, customers.registration_date, appointment_date, lead_source.lead_name as [LeadSource], status_note, company, email2, CONVERT(DATETIME,customer_estimate.sale_date) AS SaleDate,ISNULL(estimate_payments.new_total_with_tax,0) AS SaleAmount, ISNULL(t1.RecievedAmount,0) AS RecievedAmount, ISNULL(estimate_payments.new_total_with_tax,0) - ISNULL(t1.RecievedAmount,0) AS DueAmount ,notes " +
                            " FROM customers " +
                            " LEFT OUTER JOIN  customer_estimate ON customer_estimate.customer_id = customers.customer_id " +
                            " LEFT OUTER JOIN  estimate_payments ON estimate_payments.customer_id = customer_estimate.customer_id and estimate_payments.estimate_id = customer_estimate.estimate_id " +
                            " INNER JOIN lead_status ON lead_status.lead_status_id = customers.lead_status_id " +
                            " INNER JOIN lead_source ON lead_source.lead_source_id = customers.lead_source_id " +
                            " INNER JOIN sales_person ON sales_person.sales_person_id = customer_estimate.sales_person_id " +
                            " LEFT OUTER JOIN (SELECT customer_id,estimate_id,SUM(pay_amount) AS RecievedAmount from New_partial_payment Group By customer_id, estimate_id) as t1 ON t1.customer_id = customer_estimate.customer_id  AND  t1.estimate_id = customer_estimate.estimate_id " + strCondition + " ";
            }

            List<csAdvancedReport> mList = _db.ExecuteQuery<csAdvancedReport>(strQ, string.Empty).ToList();

            DataTable tblCustList = SessionInfo.LINQToDataTable(mList);

            if (tblCustList.Rows.Count > 0)
            {
                tblCustList.Columns["customer_name"].ColumnName = "Customer Name";
                tblCustList.Columns["address"].ColumnName = "Address";
                tblCustList.Columns["cross_street"].ColumnName = "Cross Street";
                tblCustList.Columns["city"].ColumnName = "City";
                tblCustList.Columns["state"].ColumnName = "State";
                tblCustList.Columns["zip_code"].ColumnName = "Zip";
                tblCustList.Columns["fax"].ColumnName = "Fax";
                tblCustList.Columns["email"].ColumnName = "Email";
                tblCustList.Columns["email2"].ColumnName = "Email2";
                tblCustList.Columns["phone"].ColumnName = "Phone";
                tblCustList.Columns["mobile"].ColumnName = "Mobile";
                tblCustList.Columns["SalesRep"].ColumnName = "Sales Person";
                tblCustList.Columns["Status"].ColumnName = "Status";
                tblCustList.Columns["lead_status_name"].ColumnName = "Lead Status";
                tblCustList.Columns["LeadSource"].ColumnName = "Lead Source";
                tblCustList.Columns["registration_date"].ColumnName = "Entry Date";
                tblCustList.Columns["notes"].ColumnName = "Notes";
                tblCustList.Columns["SaleDate"].ColumnName = "Sale Date";
                tblCustList.Columns["appointment_date"].ColumnName = "Appointment Date";
                tblCustList.Columns["SaleAmount"].ColumnName = "Sale Amount";
                tblCustList.Columns["estimate_name"].ColumnName = "Estimate";
                tblCustList.Columns["job_number"].ColumnName = "Job Number";
                tblCustList.Columns["Company"].ColumnName = "Company";
                tblCustList.Columns["status_note"].ColumnName = "Status Note";
                tblCustList.Columns["RecievedAmount"].ColumnName = "Recieved Amount";
                tblCustList.Columns["DueAmount"].ColumnName = "Due Amount";



                tblCustList.Columns.Remove("customer_id");

                //int i = 0;
                //DateTime dt = Convert.ToDateTime("1900-01-01");
                //foreach (DataRow dr in tblCustList.Rows)
                //{
                    
                //    string startTime = Convert.ToDateTime(dr["Appointment Date"]).ToShortTimeString();
                   
                //    if (startTime == "12:00 AM")
                //    {
                //        tblCustList.Rows[i]["Appointment Date"] = DateTime.Parse(Convert.ToDateTime(dr["Appointment Date"]).ToShortDateString());
                //    }
                //    else
                //    {
                //        tblCustList.Rows[i]["Appointment Date"] = DateTime.Parse(Convert.ToDateTime(dr["Appointment Date"]).ToShortDateString() + " " + startTime);
                //    }
                    
                //    i++;
                //}

                //foreach (var column in tblCustList.Columns.Cast<DataColumn>().ToArray())
                //{                    
                //    //if (tblCustList.AsEnumerable().All(dr => dr.IsNull(column)))
                //    //    tblCustList.Columns.Remove(column);
                //    //else if (tblCustList.AsEnumerable().All(dr => dr[column].ToString().Contains("01/01/0001") || dr[column].ToString().Contains("1/1/0001")))
                //    //    tblCustList.Columns.Remove(column);
                //    //else if (tblCustList.AsEnumerable().All(dr => dr[column].ToString().Equals("0")))
                //    //    tblCustList.Columns.Remove(column);
                //}

                foreach (ListItem l in lsbHideColumn.Items)
                {
                    tblCustList.Columns.Remove(l.Text);
                }

                ReorderTable(tblCustList, nColmnsText);
            }

            if (ddlItemPerPage.SelectedValue != "0")
            {
                grdCustomerReport.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
            }
            else
            {
                grdCustomerReport.PageSize = 1000;
            }
            Session.Add("Fil_Cust", tblCustList);
            grdCustomerReport.DataSource = tblCustList;
            grdCustomerReport.DataBind();

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

            if (grdCustomerReport.PageCount == nPageNo + 1)
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
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    public static void ReorderTable(DataTable table, List<string> columns)
    {
        if (columns.Count != table.Columns.Count)
        {
            throw new ArgumentException("Count of columns must be equal to table.Column.Count", "columns");
        }
        else
        {
            for (int i = 0; i < columns.Count; i++)
            {
                table.Columns[columns[i]].SetOrdinal(i);
            }
        }
    }


    protected void grdCustomerReport_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCustomerReport.ID, grdCustomerReport.GetType().Name, "PageIndexChanging"); 
        GetCustomersNew(e.NewPageIndex);
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        lblMessage.Text = "";
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
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlItemPerPage.ID, ddlItemPerPage.GetType().Name, "PageIndexChanging"); 
        GetCustomersNew(0);
    }

    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, lnkViewAll.ID, lnkViewAll.GetType().Name, "Click"); 
        ddlSaveSearches.SelectedIndex = 0;
        DisbaleItem(0);
        ClearControls();
        GetCustomersNew(0);
    }

    protected void ClearControls()
    {
        lblMessage.Text = "";
        txtSaveSearch.Text = "";
        lblCurrentSearch.Text = "";

        txtSearch.Text = "";
        txtApptEndDate.Text = "";
        txtApptStartDate.Text = "";
        txtEntStartDate.Text = "";
        txtEntEndDate.Text = "";
        txtSaleStartDate.Text = "";
        txtSaleEndDate.Text = "";

        txtMinAmount.Text = "";
        txtMaxAmount.Text = "";
        txtGreaterAmount.Text = "";
        txtLessAmount.Text = "";

        txtMinRcvdAmount.Text = "";
        txtMaxRcvdAmount.Text = "";
        txtGreaterRcvdAmount.Text = "";
        txtLessRcvdAmount.Text = "";

        txtMinDueAmount.Text = "";
        txtMaxDueAmount.Text = "";
        txtDueGreaterAmount.Text = "";
        txtLessDueAmount.Text = "";

        ClearListBox(lsbSalesRep);
        ClearListBox(lsbStatus);
        ClearListBox(lsbLeadSource);
        ClearListBox(lsbLocation);
        ClearListBox(lsbSection);
        ClearListBox(lsbDisplayColumn);



        btnDeleteSearch.Enabled = false;
        btnUpdateSearch.Enabled = false;

    }

    protected void ClearListBox(ListBox nListbox)
    {
        foreach (ListItem item in nListbox.Items)
        {
            item.Selected = false;
        }
    }

    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "PageIndexChanging"); 
        lblMessage.Text = "";
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
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnExpCustList.ID, btnExpCustList.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable dtCust = (DataTable)Session["Fil_Cust"];

        if (dtCust.Rows.Count > 0)
        {
            Response.Clear();
            Response.ClearHeaders();

            using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
            {
                writer.WriteAll(dtCust, true);
            }
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=LeadReport.csv");
            Response.End();
        }

    }
   

    protected void first2second_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, first2second.ID, first2second.GetType().Name, "Click"); 
        //while (lsbDisplayColumn.SelectedIndex > -1)
        //{
        //    lsbHideColumn.Items.Add(lsbDisplayColumn.Items[lsbDisplayColumn.SelectedIndex]);
        //    lsbDisplayColumn.Items.RemoveAt(lsbDisplayColumn.SelectedIndex);
        //}
        for (int i = lsbDisplayColumn.Items.Count - 1; i >= 0; i--)
        {
            ListItem item = lsbDisplayColumn.Items[i];
            if (item.Selected)
            {
                lsbHideColumn.Items.Add(item);
                lsbDisplayColumn.Items.RemoveAt(i);
            }
        }
    }
    protected void second2first_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, second2first.ID, second2first.GetType().Name, "Click"); 
        while (lsbHideColumn.SelectedIndex > -1)
        {
            lsbDisplayColumn.Items.Add(lsbHideColumn.Items[lsbHideColumn.SelectedIndex]);
            lsbHideColumn.Items.RemoveAt(lsbHideColumn.SelectedIndex);
        }
    }
    //private void sortListBox(ref ListBox pList, bool pByValue)
    //{
    //    SortedList lListItems = new SortedList();
    //    //add listbox items to SortedList 
    //    foreach (ListItem lItem in pList.Items)
    //    {
    //        if (pByValue) lListItems.Add(lItem.Value, lItem);
    //        else lListItems.Add(lItem.Text, lItem);
    //    }


    //    //clear list box
    //    pList.Items.Clear();

    //    //add sorted items to listbox
    //    for (int i = 0; i < lListItems.Count; i++)
    //    {
    //        pList.Items.Add((ListItem)lListItems[lListItems.GetKey(i)]);
    //    }
    //}
    protected void btnUp_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUp.ID, btnUp.GetType().Name, "Click"); 
        MoveUp(lsbDisplayColumn);
    }

    protected void btnDown_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnDown.ID, btnDown.GetType().Name, "Click"); 
        MoveDown(lsbDisplayColumn);
    }

    void MoveUp(ListBox myListBox)
    {
        int selectedIndex = myListBox.SelectedIndex;
        if (selectedIndex > 0)
        {
            myListBox.Items.Insert(selectedIndex - 1, myListBox.Items[selectedIndex]);
            myListBox.Items.RemoveAt(selectedIndex + 1);
            myListBox.SelectedIndex = selectedIndex - 1;
        }
    }

    void MoveDown(ListBox myListBox)
    {
        int selectedIndex = myListBox.SelectedIndex;
        if (selectedIndex < myListBox.Items.Count - 1 & selectedIndex != -1)
        {
            myListBox.Items.Insert(selectedIndex + 2, myListBox.Items[selectedIndex]);
            myListBox.Items.RemoveAt(selectedIndex);
            myListBox.SelectedIndex = selectedIndex + 1;

        }
    }


    protected void ddlSaveSearches_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSaveSearches.ID, ddlSaveSearches.GetType().Name, "PageIndexChanging"); 
        ClearControls();

        if (ddlSaveSearches.SelectedIndex > 0)
        {            
            LoadSearch(Convert.ToInt32(ddlSaveSearches.SelectedValue));
            DisbaleItem(Convert.ToInt32(ddlSaveSearches.SelectedValue));
            GetCustomersNew(0);
        }
    }

    private void LoadSearch(int nSearchId)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        advanced_search_report objSearch = _db.advanced_search_reports.SingleOrDefault(r => r.advanced_report_id == nSearchId);

        if (objSearch != null)
        {
            lblCurrentSearch.Text = objSearch.search_name;

            txtApptStartDate.Text = objSearch.appointment_date_start;
            txtApptEndDate.Text = objSearch.appointment_date_end;

            txtEntStartDate.Text = objSearch.entry_date_start;
            txtEntEndDate.Text = objSearch.entry_date_end;

            txtSaleStartDate.Text = objSearch.sale_date_start;
            txtSaleEndDate.Text = objSearch.sale_date_end;

            txtMinAmount.Text = objSearch.sale_amount_start;
            txtMaxAmount.Text = objSearch.sale_amount_end;
            txtGreaterAmount.Text = objSearch.sale_amount_greater;
            txtLessAmount.Text = objSearch.sale_amount_less;


            txtMinRcvdAmount.Text = objSearch.received_amount_start;
            txtMaxRcvdAmount.Text = objSearch.received_amount_end;
            txtGreaterRcvdAmount.Text = objSearch.received_amount_greater;
            txtLessRcvdAmount.Text = objSearch.received_amount_less;

            txtMinDueAmount.Text = objSearch.due_amount_start;
            txtMaxDueAmount.Text = objSearch.due_amount_end;
            txtDueGreaterAmount.Text = objSearch.due_amount_greater;
            txtLessDueAmount.Text = objSearch.due_amount_less;


            ddlItemPerPage.SelectedValue = objSearch.item_per_page.ToString();

            // Get Details

            List<advanced_report_detail> objDetails = _db.advanced_report_details.Where(d => d.advanced_report_id == objSearch.advanced_report_id).ToList();
            if (objDetails.Count > 0)
            {
                SelectDetails(objDetails, 1, lsbSalesRep);

                SelectDetails(objDetails, 2, lsbStatus);

                SelectDetails(objDetails, 3, lsbLeadSource);
                SelectDetails(objDetails, 4, lsbLocation);
                SelectDetails(objDetails, 5, lsbSection);
                LoadDisplayColumns(objDetails, 6, lsbDisplayColumn);
                LoadDisplayColumns(objDetails, 7, lsbHideColumn);

            }

            btnDeleteSearch.Enabled = true;
            btnUpdateSearch.Enabled = true;

           // GetCustomersNew(0);
        }

    }

    private void LoadDisplayColumns(List<advanced_report_detail> objDetails, int Item_Type, ListBox lst)
    {

        lst.Items.Clear();

        List<advanced_report_detail> objList = objDetails.Where(l => l.item_type_id == Item_Type).ToList();


        lst.DataSource = objList;
        lst.DataTextField = "item_text";
        lst.DataValueField = "item_value";
        lst.DataBind();

    }

    private void SelectDetails(List<advanced_report_detail> objDetails, int Item_Type, ListBox lst)
    {
        try
        {
            List<advanced_report_detail> objList = objDetails.Where(l => l.item_type_id == Item_Type).ToList();

            foreach (advanced_report_detail detail in objList)
            {

                ListItem item = lst.Items.FindByValue(detail.item_value);
                if (item != null)
                    item.Selected = true;
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void btnSaveSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSaveSearch.ID, btnSaveSearch.GetType().Name, "Click"); 

        if (txtSaveSearch.Text.Trim() == "")
        {
            lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Save Search.");

            return;
        }

        DataClassesDataContext _db = new DataClassesDataContext();




        try
        {
            userinfo objUser = (userinfo)Session["oUser"];


            advanced_search_report objAdvanceReport = _db.advanced_search_reports.SingleOrDefault(r => r.search_name == txtSaveSearch.Text.Trim() && r.user_id == objUser.user_id);
            if (objAdvanceReport != null)
            {
                lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Search name already exist in the system.");

                return;
            }


            objAdvanceReport = new advanced_search_report();


            objAdvanceReport.user_id = objUser.user_id;
            objAdvanceReport.user_name = objUser.username;
            objAdvanceReport.search_name = txtSaveSearch.Text.Trim();

            objAdvanceReport.appointment_date_start = txtApptStartDate.Text.Trim();
            objAdvanceReport.appointment_date_end = txtApptEndDate.Text.Trim();

            objAdvanceReport.entry_date_start = txtEntStartDate.Text.Trim();
            objAdvanceReport.entry_date_end = txtEntEndDate.Text.Trim();

            objAdvanceReport.sale_date_start = txtSaleStartDate.Text.Trim();
            objAdvanceReport.sale_date_end = txtSaleEndDate.Text.Trim();

            objAdvanceReport.sale_amount_start = txtMinAmount.Text.Trim();
            objAdvanceReport.sale_amount_end = txtMaxAmount.Text.Trim();
            objAdvanceReport.sale_amount_greater = txtGreaterAmount.Text.Trim();
            objAdvanceReport.sale_amount_less = txtLessAmount.Text.Trim();


            objAdvanceReport.received_amount_start = txtMinRcvdAmount.Text.Trim();
            objAdvanceReport.received_amount_end = txtMaxRcvdAmount.Text.Trim();
            objAdvanceReport.received_amount_greater = txtGreaterRcvdAmount.Text.Trim();
            objAdvanceReport.received_amount_less = txtLessRcvdAmount.Text.Trim();

            objAdvanceReport.due_amount_start = txtMinDueAmount.Text.Trim();
            objAdvanceReport.due_amount_end = txtMaxDueAmount.Text.Trim();
            objAdvanceReport.due_amount_greater = txtDueGreaterAmount.Text.Trim();
            objAdvanceReport.due_amount_less = txtLessDueAmount.Text.Trim();


            objAdvanceReport.item_per_page = Convert.ToInt32(ddlItemPerPage.SelectedValue);
            objAdvanceReport.create_date = DateTime.Now;
            objAdvanceReport.last_updated_date = DateTime.Now;

            _db.advanced_search_reports.InsertOnSubmit(objAdvanceReport);

            _db.SubmitChanges();


            int nReportId = objAdvanceReport.advanced_report_id;
            // Add details


            // Sales Person Item_Type =1
            AddSelectedDetails(_db, 1, lsbSalesRep, nReportId);

            //Status Item_Type =2
            AddSelectedDetails(_db, 2, lsbStatus, nReportId);

            //Lead Source Item_Type =3
            AddSelectedDetails(_db, 3, lsbLeadSource, nReportId);

            //Location Item_Type =4
            AddSelectedDetails(_db, 4, lsbLocation, nReportId);


            // Section Item_Type =5
            AddSelectedDetails(_db, 5, lsbSection, nReportId);

            //Display Column Item_Type =6
            AddDetails(_db, 6, lsbDisplayColumn, nReportId);


            //Hide Column Item_Type =7
            AddDetails(_db, 7, lsbHideColumn, nReportId);


            _db.SubmitChanges();

            BindSavedSearch();

            ddlSaveSearches.SelectedValue = nReportId.ToString();

            btnDeleteSearch.Enabled = true;
            btnUpdateSearch.Enabled = true;

            lblCurrentSearch.Text = txtSaveSearch.Text;
            txtSaveSearch.Text = "";

            lblMessage.Text = csCommonUtility.GetSystemMessage("Search information update successfully.");
        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    private void DeleteDetails(DataClassesDataContext _db, int nReportId)
    {
        try
        {
            List<advanced_report_detail> objDetails = _db.advanced_report_details.Where(d => d.advanced_report_id == nReportId).ToList();
            foreach (advanced_report_detail detail in objDetails)
            {
                _db.advanced_report_details.DeleteOnSubmit(detail);

            }
            _db.SubmitChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    private void AddSelectedDetails(DataClassesDataContext _db, int Item_Type, ListBox lst, int nReportId)
    {
        try
        {
            foreach (ListItem item in lst.Items)
            {
                if (item.Selected)
                {
                    advanced_report_detail details = new advanced_report_detail();
                    details.advanced_report_id = nReportId;
                    details.item_type_id = Item_Type;
                    details.item_text = item.Text;
                    details.item_value = item.Value;

                    _db.advanced_report_details.InsertOnSubmit(details);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private void AddDetails(DataClassesDataContext _db, int Item_Type, ListBox lst, int nReportId)
    {
        try
        {
            foreach (ListItem item in lst.Items)
            {

                advanced_report_detail details = new advanced_report_detail();
                details.advanced_report_id = nReportId;
                details.item_type_id = Item_Type;
                details.item_text = item.Text;
                details.item_value = item.Value;

                _db.advanced_report_details.InsertOnSubmit(details);

            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void btnDeleteSearch_Click(object sender, EventArgs e)
    {
        try
        {
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnDeleteSearch.ID, btnDeleteSearch.GetType().Name, "Click"); 
            DataClassesDataContext _db = new DataClassesDataContext();

            advanced_search_report objReport = _db.advanced_search_reports.SingleOrDefault(r => r.advanced_report_id == Convert.ToInt32(ddlSaveSearches.SelectedValue));

            if (objReport != null)
            {
                DeleteDetails(_db, objReport.advanced_report_id);

                _db.advanced_search_reports.DeleteOnSubmit(objReport);

            }
            _db.SubmitChanges();


            lblMessage.Text = csCommonUtility.GetSystemMessage("Search information deleted successfully.");

            BindSavedSearch();

            ddlSaveSearches.SelectedValue = "0";

            ClearControls();

            btnDeleteSearch.Enabled = false;
            btnUpdateSearch.Enabled = false;

        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }


    }

    protected void btnUpdateSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpdateSearch.ID, btnUpdateSearch.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();

        try
        {
            userinfo objUser = (userinfo)Session["oUser"];


            advanced_search_report objAdvanceReport = _db.advanced_search_reports.SingleOrDefault(r => r.advanced_report_id == Convert.ToInt32(ddlSaveSearches.SelectedValue) && r.user_id == objUser.user_id);
            if (objAdvanceReport == null)
            {
                lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Invalid search for update.");

                return;
            }

            objAdvanceReport.appointment_date_start = txtApptStartDate.Text.Trim();
            objAdvanceReport.appointment_date_end = txtApptEndDate.Text.Trim();

            objAdvanceReport.entry_date_start = txtEntStartDate.Text.Trim();
            objAdvanceReport.entry_date_end = txtEntEndDate.Text.Trim();

            objAdvanceReport.sale_date_start = txtSaleStartDate.Text.Trim();
            objAdvanceReport.sale_date_end = txtSaleEndDate.Text.Trim();

            objAdvanceReport.sale_amount_start = txtMinAmount.Text.Trim();
            objAdvanceReport.sale_amount_end = txtMaxAmount.Text.Trim();
            objAdvanceReport.sale_amount_greater = txtGreaterAmount.Text.Trim();
            objAdvanceReport.sale_amount_less = txtLessAmount.Text.Trim();


            objAdvanceReport.received_amount_start = txtMinRcvdAmount.Text.Trim();
            objAdvanceReport.received_amount_end = txtMaxRcvdAmount.Text.Trim();
            objAdvanceReport.received_amount_greater = txtGreaterRcvdAmount.Text.Trim();
            objAdvanceReport.received_amount_less = txtLessRcvdAmount.Text.Trim();

            objAdvanceReport.due_amount_start = txtMinDueAmount.Text.Trim();
            objAdvanceReport.due_amount_end = txtMaxDueAmount.Text.Trim();
            objAdvanceReport.due_amount_greater = txtDueGreaterAmount.Text.Trim();
            objAdvanceReport.due_amount_less = txtLessDueAmount.Text.Trim();


            objAdvanceReport.item_per_page = Convert.ToInt32(ddlItemPerPage.SelectedValue);

            objAdvanceReport.last_updated_date = DateTime.Now;


            _db.SubmitChanges();


            int nReportId = objAdvanceReport.advanced_report_id;

            //Delete Report details
            DeleteDetails(_db, nReportId);


            // Sales Person Item_Type =1
            AddSelectedDetails(_db, 1, lsbSalesRep, nReportId);

            //Status Item_Type =2
            AddSelectedDetails(_db, 2, lsbStatus, nReportId);

            //Lead Source Item_Type =3
            AddSelectedDetails(_db, 3, lsbLeadSource, nReportId);

            //Location Item_Type =4
            AddSelectedDetails(_db, 4, lsbLocation, nReportId);


            // Section Item_Type =5
            AddSelectedDetails(_db, 5, lsbSection, nReportId);

            //Display Column Item_Type =6
            AddDetails(_db, 6, lsbDisplayColumn, nReportId);


            //Hide Column Item_Type =7
            AddDetails(_db, 7, lsbHideColumn, nReportId);


            _db.SubmitChanges();

            txtSaveSearch.Text = "";

            lblMessage.Text = csCommonUtility.GetSystemMessage("Search information updated successfully.");
        }
        catch (Exception ex)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    private void DisbaleItem(int IsDefault)
    {
        if (IsDefault == 2)
        {
            btnUpdateSearch.Enabled = false;
            btnDeleteSearch.Enabled = false;

            btnUpdateSearch.CssClass = "disableBtn";
            btnDeleteSearch.CssClass = "disableBtn";
        }
        else
        {
            btnUpdateSearch.Enabled = true;
            btnDeleteSearch.Enabled = true;

            btnUpdateSearch.CssClass = "button";
            btnDeleteSearch.CssClass = "button";
        }
    }

    protected void grdCustomerReport_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // State
            DataRow rowState = ((DataRowView)e.Row.DataItem).Row;
            int indexState = GetstateColumnIndex(rowState, "State");
            if (indexState != -1)
            {
                e.Row.Cells[indexState].HorizontalAlign = HorizontalAlign.Center;
            }

            // Status
            DataRow rowStatus = ((DataRowView)e.Row.DataItem).Row;
            int indexStatus = GetstatusColumnIndex(rowStatus, "Status");
            if (indexStatus != -1)
            {
                e.Row.Cells[indexStatus].HorizontalAlign = HorizontalAlign.Center;
            }

            // Lead Status
            DataRow rowLeadStatus = ((DataRowView)e.Row.DataItem).Row;
            int indexLeadStatus = GetleadstatusColumnIndex(rowLeadStatus, "Lead Status");
            if (indexLeadStatus != -1)
            {
                e.Row.Cells[indexLeadStatus].HorizontalAlign = HorizontalAlign.Center;
            }

            // Appointment Date
            DataRow rowAppt = ((DataRowView)e.Row.DataItem).Row;
            int indexAppt = GetDateColumnIndex(rowAppt, "Appointment Date");
            if (indexAppt != -1)
            {
                string test = rowAppt[indexAppt].ToString();
                if (Convert.ToDateTime(rowAppt[indexAppt]).Year != 1900)
                {
                    string apptTime = Convert.ToDateTime(rowAppt[indexAppt]).ToShortTimeString();
                    e.Row.Cells[indexAppt].Text = ((DateTime)rowAppt[indexAppt]).ToString("dd/MM/yyyy");
                    if (apptTime == "12:00 AM")
                    {
                        e.Row.Cells[indexAppt].Text = ((DateTime)rowAppt[indexAppt]).ToString("MM/dd/yyyy");
                        e.Row.Cells[indexAppt].HorizontalAlign = HorizontalAlign.Center;
                    }
                    else
                    {
                        e.Row.Cells[indexAppt].Text = ((DateTime)rowAppt[indexAppt]).ToString("MM/dd/yyyy hh:mm tt");
                        e.Row.Cells[indexAppt].HorizontalAlign = HorizontalAlign.Center;
                    }
                }
                else
                {
                    e.Row.Cells[indexAppt].Text = "";
                }
            }

            // Entry Date
            DataRow rowEntry = ((DataRowView)e.Row.DataItem).Row;
            int indexEntry = GetEntryColumnIndex(rowEntry, "Entry Date");
            if (indexEntry != -1)
            {
                string entryTime = Convert.ToDateTime(rowEntry[indexEntry]).ToShortTimeString();
                if (entryTime == "12:00 AM")
                {
                    e.Row.Cells[indexEntry].Text = ((DateTime)rowEntry[indexEntry]).ToString("MM/dd/yyyy");
                }
                else
                {
                    e.Row.Cells[indexEntry].Text = ((DateTime)rowEntry[indexEntry]).ToString("MM/dd/yyyy hh:mm tt");
                }
            }

            // Sale Date
            DataRow rowSale = ((DataRowView)e.Row.DataItem).Row;
            int indexSale = GetSaleColumnIndex(rowSale, "Sale Date");
            if (indexSale != -1)
            {
                string saleTime = Convert.ToDateTime(rowSale[indexSale]).ToShortTimeString();
                if (saleTime == "12:00 AM")
                {
                    e.Row.Cells[indexSale].Text = ((DateTime)rowSale[indexSale]).ToString("MM/dd/yyyy");
                }
                else
                {
                    e.Row.Cells[indexSale].Text = ((DateTime)rowSale[indexSale]).ToString("MM/dd/yyyy hh:mm tt");
                }
            }

            // Sale Amount
            DataRow rowSaleAmount = ((DataRowView)e.Row.DataItem).Row;
            int indexSaleAmount = GetSaleAmountColumnIndex(rowSaleAmount, "Sale Amount");
            if (indexSaleAmount != -1)
            {
                e.Row.Cells[indexSaleAmount].HorizontalAlign = HorizontalAlign.Right;
            }
        }
    }

    private int stateColumnIndex = -1;
    private int GetstateColumnIndex(DataRow row, string colmnName)
    {
        if (this.stateColumnIndex == -1)
        {
            this.stateColumnIndex = row.Table.Columns.IndexOf(colmnName);

            if (stateColumnIndex < 0)
            {
                throw new Exception(
                "datasource does not contain the " + colmnName + " column");
            }
        }
        return this.stateColumnIndex;
    }

    private int statusColumnIndex = -1;
    private int GetstatusColumnIndex(DataRow row, string colmnName)
    {
        if (this.statusColumnIndex == -1)
        {
            this.statusColumnIndex = row.Table.Columns.IndexOf(colmnName);

            if (statusColumnIndex < 0)
            {
                throw new Exception(
                "datasource does not contain the " + colmnName + " column");
            }
        }
        return this.statusColumnIndex;
    }

    private int leadstatusColumnIndex = -1;
    private int GetleadstatusColumnIndex(DataRow row, string colmnName)
    {
        if (this.leadstatusColumnIndex == -1)
        {
            this.leadstatusColumnIndex = row.Table.Columns.IndexOf(colmnName);

            if (leadstatusColumnIndex < 0)
            {
                throw new Exception(
                "datasource does not contain the " + colmnName + " column");
            }
        }
        return this.leadstatusColumnIndex;
    }

    private int SaleAmountColumnIndex = -1;
    private int GetSaleAmountColumnIndex(DataRow row, string colmnName)
    {
        if (this.SaleAmountColumnIndex == -1)
        {
            this.SaleAmountColumnIndex = row.Table.Columns.IndexOf(colmnName);

            if (SaleAmountColumnIndex < 0)
            {
                throw new Exception(
                "datasource does not contain the " + colmnName + " column");
            }
        }
        return this.SaleAmountColumnIndex;
    }   

    private int dateColumnIndex = -1;
    private int GetDateColumnIndex(DataRow row, string colmnName)
    {
        if (this.dateColumnIndex == -1)
        {
            this.dateColumnIndex = row.Table.Columns.IndexOf(colmnName);

            if (dateColumnIndex < 0)
            {
                throw new Exception(
                "datasource does not contain the " + colmnName + " column");
            }
        }
        return this.dateColumnIndex;
    }

    private int saleColumnIndex = -1;
    private int GetSaleColumnIndex(DataRow row, string colmnName)
    {
        if (this.saleColumnIndex == -1)
        {
            this.saleColumnIndex = row.Table.Columns.IndexOf(colmnName);

            if (saleColumnIndex < 0)
            {
                throw new Exception(
                "datasource does not contain the " + colmnName + " column");
            }
        }
        return this.saleColumnIndex;
    }

    private int entryColumnIndex = -1;
    private int GetEntryColumnIndex(DataRow row, string colmnName)
    {
        if (this.entryColumnIndex == -1)
        {
            this.entryColumnIndex = row.Table.Columns.IndexOf(colmnName);

            if (entryColumnIndex < 0)
            {
                //throw new Exception(
                //"datasource does not contain the " + colmnName + " column");
            }
        }
        return this.entryColumnIndex;
    }
}
