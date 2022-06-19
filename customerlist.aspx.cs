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
using System.Net;

public partial class customerlist : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<csCustomer> cList = (List<csCustomer>)HttpContext.Current.Session["cSearch"];
           
            return (from c in cList
                    where c.last_name1.ToLower().Trim().StartsWith(prefixText.ToLower().Trim())
                    select c.last_name1.Trim()).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            return (from c in _db.customers
                    where c.last_name1.ToLower().Trim().StartsWith(prefixText.ToLower().Trim())
                    select c.last_name1.Trim()).Distinct().Take<String>(count).ToArray();
        }
    }
    [WebMethod]
    public static string[] GetCompany(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<csCustomer> cList = (List<csCustomer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.company.ToLower().Trim().StartsWith(prefixText.ToLower().Trim())
                    select c.company.Trim()).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.company.ToLower().Trim().StartsWith(prefixText.ToLower().Trim())
                    select c.company.Trim()).Distinct().Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetFirstName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<csCustomer> cList = (List<csCustomer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.first_name1.ToLower().Trim().StartsWith(prefixText.ToLower().Trim())
                    orderby c.first_name1
                    select c.first_name1.Trim()).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.first_name1.ToLower().Trim().StartsWith(prefixText.ToLower().Trim())
                    orderby c.first_name1
                    select c.first_name1.Trim()).Distinct().Take<String>(count).ToArray();

        }
    }

    [WebMethod]
    public static string[] GetAddress(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<csCustomer> cList = (List<csCustomer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.address.ToLower().StartsWith(prefixText.ToLower())
                    select c.address).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.address.ToLower().StartsWith(prefixText.ToLower())
                    select c.address.Trim()).Distinct().Take<String>(count).ToArray();
        }
    }
    [WebMethod]
    public static string[] GetEmail(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<csCustomer> cList = (List<csCustomer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.email.ToLower().StartsWith(prefixText.ToLower())
                    select c.email.Trim()).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.email.StartsWith(prefixText)
                    select c.email.Trim()).Distinct().Take<String>(count).ToArray();
        }
    }
    [WebMethod]
    public static string[] GetPhone(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["lSearch"] != null)
        {
            List<csCustomer> cList = (List<csCustomer>)HttpContext.Current.Session["cSearch"];
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
    [WebMethod]
    public static string[] GetJobNumber(String prefixText, Int32 count)
    {
        List<csCustomer> cList = (List<csCustomer>)HttpContext.Current.Session["JobSearch"];
        return (from c in cList
                where c.jobNumber.ToLower().Contains(prefixText.ToLower())
                select c.jobNumber).Take<String>(count).ToArray();

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

                hdnPrimaryDivision.Value = oUser.primaryDivision.ToString();
                hdnClientId.Value = oUser.client_id.ToString();
                hdnDivisionName.Value = oUser.divisionName;
            }



            if (Page.User.IsInRole("cus001") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            GetTools();
            BindJobNumber();
            DataClassesDataContext _db = new DataClassesDataContext();
            Session.Remove("LeadId");


            if (Session["CustSreach"] != null)
            {
                string strSearch = Session["CustSreach"].ToString();
                txtSearch.Text = strSearch;
            }
            if (Session["CustomerId"] != null)
            {

                int nCustomerId = Convert.ToInt32(Session["CustomerId"]);

                customer cust = new customer();
                cust = _db.customers.SingleOrDefault(c => c.customer_id == nCustomerId);
                if (cust != null)
                {
                    if (cust.isCustomer == 0)
                    {
                        Session.Add("LeadId", nCustomerId.ToString());
                        Session.Remove("CustomerId");
                        nCustomerId = 0;
                        Response.Redirect("leadlist.aspx");
                    }
                    hdnCustomerId.Value = nCustomerId.ToString();
                }
            }
            Session.Remove("LeadSreach");
            Session.Remove("CustSreach");
            Session.Remove("SelectionFilter");

            // Get Customers
            #region Get Customers

            // List<customer> CustomerList = _db.customers.Where(c => c.client_id.ToString().Contains(hdnClientId.Value)).ToList();
            
            string strQ = "select * from customers where client_id in (" + hdnClientId.Value + " )  ";
            IEnumerable<csCustomer> CustomerList = _db.ExecuteQuery<csCustomer>(strQ, string.Empty).ToList();
           
            Session.Add("cSearch", CustomerList);

            # endregion
            BindDivision();
            BindSalesPerson();
            BindSuperintendent();

            if (hdnDivisionName.Value != "" && hdnDivisionName.Value.Contains(","))
            {
                ddlDivision.Enabled = true;
            }
            else
            {
                ddlDivision.Enabled = false;
            }

            // updateCustomersLatLng();
            GetCustomersNew(0);
        }
    }
    private void BindJobNumber()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            string strCondition = "";
            if (Convert.ToInt32(ddlStatus.SelectedValue) != 1)
            {
                if (Convert.ToInt32(ddlStatus.SelectedValue) == 2)
                {

                    strCondition += " AND c.status_id NOT IN(4,5,7)  AND ce.IsEstimateActive = 1 ";
                }
                else if (Convert.ToInt32(ddlStatus.SelectedValue) == 6)
                {
                    if (strCondition.Length > 0)
                    {
                        strCondition += " AND c.customer_id IN (SELECT Distinct customer_estimate.customer_id FROM customer_estimate WHERE customer_estimate.status_id = 3)  "; // AND customers.status_id NOT IN(4,5)
                    }
                    else
                    {
                        strCondition += " and  c.customer_id IN (SELECT Distinct customer_estimate.customer_id FROM customer_estimate WHERE customer_estimate.status_id = 3) "; // AND customers.status_id NOT IN(4,5)
                    }

                }
                else if (Convert.ToInt32(ddlStatus.SelectedValue) == 8)
                {
                    if (strCondition.Length > 0)
                    {
                        strCondition += " AND ce.IsEstimateActive = 0";

                    }
                    else
                    {
                        strCondition += " and  ce.IsEstimateActive = 0";

                    }

                }
                else
                {
                    if (strCondition.Length > 0)
                    {
                        strCondition += " AND c.status_id = " + Convert.ToInt32(ddlStatus.SelectedValue);
                    }
                    else
                    {
                        strCondition += " and c.status_id = " + Convert.ToInt32(ddlStatus.SelectedValue);
                    }

                }

            }
            string strQ = @"select   case when ce.alter_job_number!='' then  ce.alter_job_number 
                           else   ce.job_number end as jobNumber
                           from customers as c
                           inner join customer_estimate as ce on c.customer_id = ce.customer_id
                           where ce.status_id = 3 AND c.client_id in ( "+ hdnClientId.Value + " ) " + strCondition;
            List<csCustomer> mList = _db.ExecuteQuery<csCustomer>(strQ, string.Empty).ToList();
            Session.Add("JobSearch", mList);
        }
        catch (Exception ex)
        {
            //lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
    private void updateCustomersLatLng()
    {
        XDocument xdoc;
        WebResponse response = null;
        DataClassesDataContext _db = new DataClassesDataContext();
        List<customer> customresForUpdate = new List<customer>(); ;
        if (System.Web.HttpContext.Current.Session["cSearch"] != null)
        {
            customresForUpdate = (List<customer>)System.Web.HttpContext.Current.Session["cSearch"];
        }
        try
        {

            foreach (var c in customresForUpdate)
            {
                customer Cuustomer = _db.customers.SingleOrDefault(x => x.customer_id == c.customer_id);


                if (Cuustomer.customer_id != 1)
                {
                    string address = Cuustomer.address.Replace(".", "").Replace("#", "");
                    // string address = "2401 E Rio Salado PKWY  UNIT 1085";
                    string fulladdress = address + "," + Cuustomer.city.Trim() + "," + Cuustomer.state.Trim() + " " + Cuustomer.zip_code;
                    //  string fulladdress = address;

                    string url = String.Format("https://maps.googleapis.com/maps/api/geocode/xml?address=" +
                        fulladdress + "&key=AIzaSyAg9x2CEaBTyFmwXm75gvfmQVuOGcSND0Y");

                    WebRequest request = WebRequest.Create(url);
                    request.UseDefaultCredentials = true;
                    try
                    {
                        response = request.GetResponse();

                        xdoc = XDocument.Load(response.GetResponseStream());

                        XElement result = xdoc.Element("GeocodeResponse").Element("result");
                        XElement locationElement = result.Element("geometry").Element("location");
                        XElement Latitude = locationElement.Element("lat");
                        XElement Longitude = locationElement.Element("lng");

                        string lat = (string)Latitude.Value;
                        string lang = (string)Longitude.Value;
                        Cuustomer.Latitude = lat;
                        Cuustomer.Longitude = lang;

                    }
                    catch
                    {
                        response = null;
                    }




                    if (response != null)
                    {
                        _db.SubmitChanges();
                    }

                }

            }





        }
        catch (Exception ex)
        {
            Console.WriteLine("There is an error . Because " + ex);

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
        string strQ = "select first_name+' '+last_name AS sales_person_name,sales_person_id from sales_person WHERE is_active=1 and is_sales=1 " + csCommonUtility.GetSalesPersonSql(hdnDivisionName.Value) + " order by sales_person_name asc";
        DataTable mList = csCommonUtility.GetDataTable(strQ);
        ddlSalesRep.DataSource = mList;
        ddlSalesRep.DataTextField = "sales_person_name";
        ddlSalesRep.DataValueField = "sales_person_id";
        ddlSalesRep.DataBind();
        ddlSalesRep.Items.Insert(0, "All");

    }

    private void BindSuperintendent()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select first_name+' '+last_name AS Superintendent_name,user_id from user_info WHERE user_id != 6 order by first_name"; // role_id = 4";
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
            //ddlSuperintendent.SelectedValue = obj.user_id.ToString(); // Superintendents role 4 logic blocked as per requiest from II
            //ddlSuperintendent.Enabled = false;
        }
        else
            if (nRoleId == 3)
            {
                ddlSalesRep.SelectedValue = nSalePersonId.ToString();
                ddlSalesRep.Enabled = false;
            }
        grdCustomerList.PageIndex = nPageNo;


        string strCondition = "";
        //strCondition = " Where customers.client_id in (" + obj.client_id + ") ";

        if (txtSearch.Text.Trim() != "")
        {

            string str = txtSearch.Text.Trim();


            if (ddlSearchBy.SelectedValue == "1")
            {
                strCondition = "  customers.first_name1 LIKE '%" + str.Replace("'", "''") + "%'";
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
           
            else if (ddlSearchBy.SelectedValue == "7")
            {
                strCondition = " ND customers.phone LIKE '%" + str.Replace("'", "''") + "%'";
            }

            else if (ddlSearchBy.SelectedValue == "5")
            {
                strCondition = "  (ce.job_number LIKE '%" + str.Replace("'", "''") + "%' OR ce.alter_job_number LIKE '%" + str.Replace("'", "''") + "%')";
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
                        strCondition += " AND customers.status_id NOT IN(4,5,7) AND IsEstimateActive = 1  ";
                    }
                    else
                    {
                        strCondition += " customers.status_id NOT IN(4,5,7) AND IsEstimateActive = 1  ";
                    }
                }
                else if (Convert.ToInt32(ddlStatus.SelectedValue) == 6)
                {
                    if (strCondition.Length > 0)
                    {
                        strCondition += " AND customers.customer_id IN (SELECT Distinct customer_estimate.customer_id FROM customer_estimate WHERE customer_estimate.status_id = 3)  "; // AND customers.status_id NOT IN(4,5)
                    }
                    else
                    {
                        strCondition += " customers.customer_id IN (SELECT Distinct customer_estimate.customer_id FROM customer_estimate WHERE customer_estimate.status_id = 3) "; // AND customers.status_id NOT IN(4,5)
                    }

                }
                else if (Convert.ToInt32(ddlStatus.SelectedValue) == 8)
                {
                    if (strCondition.Length > 0)
                    {
                        strCondition += " AND IsEstimateActive = 0";
                    }
                    else
                    {
                        strCondition += "  IsEstimateActive = 0";
                    }

                }
                else
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
           

            if (ddlDivision.SelectedItem.Text != "All")
            {
                if (strCondition.Length > 2)
                    strCondition += " AND customers.client_id in (" + ddlDivision.SelectedValue + ") ";
                else
                    strCondition = " WHERE  customers.client_id in (" + ddlDivision.SelectedValue + ") ";
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
        //    strCondition = "Where " + strCondition + " AND isCustomer = 1 ";
        //}
        //else
        //{
        //    strCondition = "Where  isCustomer = 1 ";

        //}

        if (ddlDivision.SelectedItem.Text != "All")
        {
            if (strCondition.Length > 2)
                strCondition += " AND customers.client_id in ('" + ddlDivision.SelectedValue + "') ";
            else
                strCondition = " WHERE  customers.client_id in ('" + ddlDivision.SelectedValue + "') ";
        }


        string strQ = string.Empty;
        if (Convert.ToInt32(hdnCustomerId.Value) > 0)
        {
            int nCustomerId = Convert.ToInt32(hdnCustomerId.Value);
            strQ = " SELECT client_id,customers.islead,customers.isCustomer,customers.customer_id, first_name1+' '+ last_name1 AS customer_name,first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                          " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date, CAST (ISNULL(t1.IsEstimateActive,1) AS bit) AS  IsEstimateActive  " +
                          " FROM customers " +
                          " LEFT OUTER JOIN (SELECT  customer_estimate.customer_id,MAX(CONVERT(DATETIME,sale_date)) AS SaleDate,CAST(MIN(CAST(IsEstimateActive as INT)) AS BIT) as IsEstimateActive FROM customer_estimate WHERE customer_estimate.status_id=3 AND customer_estimate.customer_id = " + nCustomerId + " GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                          " WHERE customers.customer_id =" + nCustomerId + " AND isCustomer = 1 order by t1.SaleDate asc";
            hdnCustomerId.Value = "0";
        }
        else
        {
            if (nRoleId == 4) // Superintendents role 4 logic blocked as per requiest from II
            {
                if (ddlSearchBy.SelectedValue == "5")
                {
                    strQ = " SELECT client_id,customers.islead,customers.isCustomer, customers.customer_id,  first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                                   " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date,CAST (ISNULL(t1.IsEstimateActive,1) AS bit) AS  IsEstimateActive " +
                                   " FROM customers " +
                                   " INNER JOIN (SELECT  customer_estimate.customer_id,MAX(CONVERT(DATETIME,sale_date)) AS SaleDate,CAST(MIN(CAST(IsEstimateActive as INT)) AS BIT) as IsEstimateActive FROM customer_estimate WHERE customer_estimate.status_id = 3 AND IsEstimateActive = 1 AND isCustomer = 1 AND job_number LIKE '%" + txtSearch.Text + "%' GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                                   "  order by t1.SaleDate desc";
                }
                else
                {
                    strQ = " SELECT client_id,customers.islead,customers.isCustomer,customers.customer_id,  first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                                  " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date,CAST (ISNULL(t1.IsEstimateActive,1) AS bit) AS  IsEstimateActive " +
                                  " FROM customers " +
                                  " LEFT OUTER JOIN  (SELECT  customer_estimate.customer_id,MAX(CONVERT(DATETIME,sale_date)) AS SaleDate, CAST(MIN(CAST(IsEstimateActive as INT)) AS BIT) as IsEstimateActive FROM customer_estimate WHERE customer_estimate.status_id=3 GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                                  " " + strCondition + " order by t1.SaleDate desc";

                }
            }
            else
            {
                if (ddlSearchBy.SelectedValue == "5")
                {
                    strQ = " SELECT client_id, customers.islead,customers.isCustomer,customers.customer_id, first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                                  " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date,CAST (ISNULL(t1.IsEstimateActive,1) AS bit) AS  IsEstimateActive " +
                                  " FROM customers " +
                                  " INNER JOIN  (SELECT  customer_estimate.customer_id,MAX(CONVERT(DATETIME,sale_date)) AS SaleDate,CAST(MIN(CAST(IsEstimateActive as INT)) AS BIT) as IsEstimateActive FROM customer_estimate WHERE customer_estimate.status_id=3  AND job_number LIKE '%" + txtSearch.Text + "%' GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                                  "  order by t1.SaleDate desc, customers.last_name1 asc";
                }
                else
                {
                    strQ = " SELECT client_id,customers.islead,customers.isCustomer, customers.customer_id, first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                                  " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,Convert(VARCHAR,t1.SaleDate,101) AS Sold_date,CAST (ISNULL(t1.IsEstimateActive,1) AS bit) AS  IsEstimateActive " +
                                  " FROM customers " +
                                  " LEFT OUTER JOIN   (SELECT  customer_estimate.customer_id,MAX(CONVERT(DATETIME,sale_date)) AS SaleDate, CAST(MIN(CAST(IsEstimateActive as INT)) AS BIT) as IsEstimateActive FROM customer_estimate WHERE customer_estimate.status_id=3 GROUP BY customer_estimate.customer_id) AS t1 ON t1.customer_id = customers.customer_id " +
                                  " " + strCondition + " order by t1.SaleDate desc, customers.last_name1 asc";

                }
            }
        }

        IEnumerable<csCustomer> mList = _db.ExecuteQuery<csCustomer>(strQ, string.Empty).ToList();
        DataTable dt = csCommonUtility.LINQToDataTable(mList);

        //arefin (09-11-2018)
        //if (dt.Rows.Count == 1)
        //{
        //    if (!mList.SingleOrDefault().IsEstimateActive)
        //        ddlStatus.SelectedValue = "8";
        //    else
        //        ddlStatus.SelectedValue = "2";
        //}
        //arefin (09-11-2018)

        if (dt.Rows.Count > 0)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = "isCustomer = 1";
            if (dv.Count > 0)
            {
                Session.Add("sCustList", dv.ToTable());
                if (ddlItemPerPage.SelectedValue != "4")
                {
                    grdCustomerList.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
                }
                else
                {
                    grdCustomerList.PageSize = 200;
                }
                grdCustomerList.DataSource = dv.ToTable();
                grdCustomerList.DataKeyNames = new string[] { "customer_id", "sales_person_id", "Sold_date", "IsEstimateActive", "status_id" };
                grdCustomerList.DataBind();
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

                if (grdCustomerList.PageCount == nPageNo + 1)
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
                    dv.RowFilter = "islead = 1";
                    if (dv.Count > 0)
                    {
                        Session.Add("LeadSreach", txtSearch.Text);
                        Response.Redirect("leadlist.aspx");
                    }
                }
                else
                {
                    if (ddlItemPerPage.SelectedValue != "4")
                    {
                        grdCustomerList.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
                    }
                    else
                    {
                        grdCustomerList.PageSize = 200;
                    }
                    grdCustomerList.DataSource = null;
                    // grdCustomerList.DataKeyNames = new string[] { "customer_id", "sales_person_id", "Sold_date" };
                    grdCustomerList.DataBind();
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

                    if (grdCustomerList.PageCount == nPageNo + 1)
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
                grdCustomerList.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
            }
            else
            {
                grdCustomerList.PageSize = 200;
            }
            grdCustomerList.DataSource = mList;
            grdCustomerList.DataKeyNames = new string[] { "customer_id", "sales_person_id", "Sold_date", "IsEstimateActive", "status_id" };
            grdCustomerList.DataBind();
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

            if (grdCustomerList.PageCount == nPageNo + 1)
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
        Session.Remove("CustomerId");
        Response.Redirect("customer_details.aspx");
    }
    private decimal GetRetailTotal(int EstID, int ncustid, int clientId)
    {
        decimal dRetail = 0;
        DataClassesDataContext _db = new DataClassesDataContext();

        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        decimal total_incentives = 0;
        estimate_payment esp = new estimate_payment();
        if (_db.estimate_payments.Where(ep => ep.estimate_id == EstID && ep.customer_id == ncustid && ep.client_id == clientId).SingleOrDefault() != null)
        {
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == EstID && ep.customer_id == ncustid && ep.client_id == clientId);
            totalwithtax = Convert.ToDecimal(esp.new_total_with_tax);
            total_incentives = Convert.ToDecimal(esp.total_incentives);
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);

            if (totalwithtax == 0)
                totalwithtax = project_subtotal + tax_amount;
            if (totalwithtax == 0)
            {
                var result = (from pd in _db.pricing_details
                              where (from clc in _db.customer_locations
                                     where clc.estimate_id == EstID && clc.customer_id == ncustid && clc.client_id == clientId
                                     select clc.location_id).Contains(pd.location_id) &&
                                     (from cs in _db.customer_sections
                                      where cs.estimate_id == EstID && cs.customer_id == ncustid && cs.client_id == clientId
                                      select cs.section_id).Contains(pd.section_level) && pd.estimate_id == EstID && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == clientId && pd.pricing_type == "A" && pd.is_CommissionExclude == false
                              select pd.total_retail_price);
                int n = result.Count();
                if (result != null && n > 0)
                    dRetail = result.Sum();

                totalwithtax = dRetail;

            }
        }

        return totalwithtax;
    }

    private void BindDivision()
    {                
        string sql = "select id, division_name from division order by division_name ";
        DataTable dt = csCommonUtility.GetDataTable(sql);
        ddlDivision.DataSource = dt;
        ddlDivision.DataTextField = "division_name";
        ddlDivision.DataValueField = "id";
        ddlDivision.DataBind();
        ddlDivision.Items.Insert(0, "All");
        ddlDivision.SelectedValue = hdnPrimaryDivision.Value;       

    }


    protected void grdCustomerList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int ncid = Convert.ToInt32(grdCustomerList.DataKeys[e.Row.RowIndex].Values[0].ToString());
            DataClassesDataContext _db = new DataClassesDataContext();

            int nid = Convert.ToInt32(grdCustomerList.DataKeys[e.Row.RowIndex].Values[1].ToString());
            int status_id = Convert.ToInt32(grdCustomerList.DataKeys[e.Row.RowIndex].Values[4].ToString());

            string sDate = string.Empty;
            if (grdCustomerList.DataKeys[e.Row.RowIndex].Values[2] != null)
            {
                sDate = grdCustomerList.DataKeys[e.Row.RowIndex].Values[2].ToString();
            }

            Label lblDivision = (Label)e.Row.FindControl("lblDivision");
            

            string strSalesPerson = string.Empty;
            sales_person sp = new sales_person();
            sp = _db.sales_persons.Single(s => s.sales_person_id == nid);
            strSalesPerson = sp.first_name + " " + sp.last_name;

            HyperLink hypMessage = (HyperLink)e.Row.FindControl("hypMessage");



            // Customer Messsage Center
            HyperLink hyp_Custd = (HyperLink)e.Row.FindControl("hyp_Custd");
            hyp_Custd.NavigateUrl = "customer_details.aspx?cid=" + ncid;

            HyperLink hypCusLogin = (HyperLink)e.Row.FindControl("hypCusLogin");
            hypCusLogin.NavigateUrl = "customerlogin.aspx?cid=" + ncid + "&aType=1";

            if (_db.customeruserinfos.Where(cu => cu.isactive == 1 && cu.customerid == ncid).ToList().Count > 0)
            {
                hypCusLogin.Visible = true;
            }
            else
            {
                hypCusLogin.Visible = false;
            }

            if (hdnEmailType.Value == "1")
                hypMessage.NavigateUrl = "customermessagecenteroutlook.aspx?cid=" + ncid;
            else
                hypMessage.NavigateUrl = "customermessagecenter.aspx?cid=" + ncid;

            HyperLink hyp_ProjectNotes = (HyperLink)e.Row.FindControl("hyp_ProjectNotes");
            hyp_ProjectNotes.NavigateUrl = "ProjectNotes.aspx?cid=" + ncid + "&TypeId=2";

            HyperLink hyp_CallLog = (HyperLink)e.Row.FindControl("hyp_CallLog");
            hyp_CallLog.NavigateUrl = "CallLogInfo.aspx?cid=" + ncid + "&TypeId=2";

            // Customer Address

            Label lblSales = (Label)e.Row.FindControl("lblSales");
            lblSales.Text = strSalesPerson;

            Label lblSaleDate = (Label)e.Row.FindControl("lblSaleDate");
            lblSaleDate.Text = sDate;

            //lblSales.Attributes.CssStyle.Add("padding", "5px 0 5px 0");
            customer cust = new customer();
            cust = _db.customers.Single(c => c.customer_id == ncid);
            string strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
            //e.Row.Cells[2].Text = strAddress;

            lblDivision.Text = "<span style='font-weight:bold; color:#000;'>Division: </span>" + csCommonUtility.GetDivisionName(cust.client_id.ToString());

            //Label lblPhone = (Label)e.Row.FindControl("lblPhone");
            //lblPhone.Text = cust.phone;
            //lblPhone.Attributes.CssStyle.Add("padding", "5px 0 5px 0");

            // Customer Email
            //HyperLink hypEmail = (HyperLink)e.Row.FindControl("hypEmail");
            //hypEmail.Text = cust.email;
            //hypEmail.NavigateUrl = "mailto:" + cust.email + "?subject=Contact";

            // Customer Address in Google Map
            HyperLink hypAddress = (HyperLink)e.Row.FindControl("hypAddress");
            hypAddress.Text = strAddress;
            //hypAddress.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
            string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
            hypAddress.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

            HyperLink hyp_DocumentManagement = (HyperLink)e.Row.FindControl("hyp_DocumentManagement");
            hyp_DocumentManagement.NavigateUrl = "DocumentManagement.aspx?cid=" + ncid + "&nbackId=1";

            // Customer Estimates
            DropDownList ddlEst = (DropDownList)e.Row.FindControl("ddlEst");

            HyperLink hypEstDetail = (HyperLink)e.Row.FindControl("hypEstDetail");
            HyperLink hypCostLoc = (HyperLink)e.Row.FindControl("hypCostLoc");
            //HyperLink hypCostSec = (HyperLink)e.Row.FindControl("hypCostSec");
            HyperLink hypCommon = (HyperLink)e.Row.FindControl("hypCommon");

            HyperLink hyp_vendor = (HyperLink)e.Row.FindControl("hyp_vendor");
            HyperLink hyp_Payment = (HyperLink)e.Row.FindControl("hyp_Payment");
            HyperLink hyp_jstatus = (HyperLink)e.Row.FindControl("hyp_jstatus");
            HyperLink hyp_Schedule = (HyperLink)e.Row.FindControl("hyp_Schedule");
            HyperLink hyp_survey = (HyperLink)e.Row.FindControl("hyp_survey");
            HyperLink hypWarrenty = (HyperLink)e.Row.FindControl("hypWarrenty");
            HyperLink hypChangeOrderList = (HyperLink)e.Row.FindControl("hypChangeOrderList");
            HyperLink hyp_Sow = (HyperLink)e.Row.FindControl("hyp_Sow");
            HyperLink hyp_PreCon = (HyperLink)e.Row.FindControl("hyp_PreCon");


            HyperLink hyp_Job = (HyperLink)e.Row.FindControl("hyp_Job");
            HyperLink hyp_Allowance = (HyperLink)e.Row.FindControl("hyp_Allowance");

            HyperLink hyp_Section_Selection = (HyperLink)e.Row.FindControl("hyp_Section_Selection");
            HyperLink hyp_MaterialTracking = (HyperLink)e.Row.FindControl("hyp_MaterialTracking");
            //HyperLink hyp_MaterialTracking = (HyperLink)e.Row.FindControl("hyp_MaterialTracking");

            HyperLink hyp_SiteReview = (HyperLink)e.Row.FindControl("hyp_SiteReview");

            Label lblOtherJob = (Label)e.Row.FindControl("lblOtherJob");


            // Customer CO
            DropDownList ddlEstCO = (DropDownList)e.Row.FindControl("ddlEstCO");
            HyperLink hypEstCODetail = (HyperLink)e.Row.FindControl("hypEstCODetail");
            HyperLink hypCOCommon = (HyperLink)e.Row.FindControl("hypCOCommon");
            Label lblJobJost = (Label)e.Row.FindControl("lblJobJost");

            HyperLink hyp_SMS = (HyperLink)e.Row.FindControl("hyp_SMS");
            hyp_SMS.Attributes.Add("onClick", "DisplayWindow(" + ncid + ");");

            //arefin 03-15-2018
            //string strddlCondition = "";
            //if (Convert.ToInt32(ddlStatus.SelectedValue) == 8)
            //{
            //    strddlCondition = "IsEstimateActive = 0 AND";
            //}
            //else
            //{
            //    strddlCondition = "IsEstimateActive = 1 AND";
            //}
            //arefin 03-15-2018


            //arefin (09-11-2018)
            string strEstimateActive = "";

            if (grdCustomerList.Rows.Count > 1)
            {
                if (Convert.ToInt32(ddlStatus.SelectedValue) == 2) //Active
                {
                    strEstimateActive = "AND IsEstimateActive = 1";
                }
                else if (Convert.ToInt32(ddlStatus.SelectedValue) == 8) // Est.InActive
                {
                    strEstimateActive = "AND IsEstimateActive = 0";
                }
            }
            //arefin (09-11-2018)



            string strQ = "select customer_estimate_id, estimate_id, customer_id, client_id, sales_person_id, status_id, " +
                          " CASE WHEN status_id = 3 THEN estimate_name+ ' (SOLD)' " +
                          " ELSE estimate_name END AS estimate_name, " +
                          " create_date, last_update_date, sale_date, estimate_comments, job_start_date, tax_rate, " +
                          " job_number, IsEstimateActive, IsCustDisplay, JobId " +
                          " from customer_estimate where customer_id=" + ncid + " and client_id = " + cust.client_id +
                          " Order by convert(datetime,sale_date) desc";
            IEnumerable<customer_estimate_model> list = _db.ExecuteQuery<customer_estimate_model>(strQ, string.Empty);

            ddlEst.DataSource = list;
            ddlEst.DataTextField = "estimate_name";
            ddlEst.DataValueField = "estimate_id";
            ddlEst.DataBind();


            //var resultCount = (from ce in _db.customer_estimates
            //                   where ce.customer_id == ncid && ce.client_id == 1 && ce.status_id == 3
            //                   select ce.estimate_id);
            //int nEstCount = resultCount.Count();

            //if (_db.customer_estimates.Where(ce => ce.customer_id == ncid && ce.client_id == 1 && ce.status_id == 3).ToList().Count > 0)
            //{
            //    int nEstId = 0;
            //    var result = (from ce in _db.customer_estimates
            //                  where ce.customer_id == ncid && ce.client_id == 1 && ce.status_id == 3
            //                  select ce.estimate_id);

            //    int n = result.Count();
            //    if (result != null && n > 0)
            //        nEstId = result.Max();
            //    ddlEst.SelectedValue = nEstId.ToString();

            //}
            //else
            //{
            //    int nEstId = 0;
            //    var result = (from ce in _db.customer_estimates
            //                  where ce.customer_id == ncid && ce.client_id == 1
            //                  select ce.estimate_id);

            //    int n = result.Count();
            //    if (result != null && n > 0)
            //        nEstId = result.Max();
            //    ddlEst.SelectedValue = nEstId.ToString();
            //}

            if (ddlEst.Items.Count > 0)
            {

                decimal TotalExCom_WithIntevcise = GetRetailTotal(Convert.ToInt32(ddlEst.SelectedValue), ncid, (int)cust.client_id);

                lblJobJost.Text = "Estimate Amount: " + TotalExCom_WithIntevcise.ToString("c");

                string strQ2 = "select * from customer_estimate where customer_id=" + ncid + " AND estimate_id != " + Convert.ToInt32(ddlEst.SelectedValue) + " and client_id = " + cust.client_id + " Order by convert(datetime,sale_date) desc";
                IEnumerable<customer_estimate_model> list2 = _db.ExecuteQuery<customer_estimate_model>(strQ2, string.Empty);

                string strOtherJobNum = string.Empty;

                foreach (customer_estimate_model cus_est2 in list2)
                {
                    if (cus_est2.job_number != null)
                    {
                      

                        if (strOtherJobNum == "")
                        {
                            if (Convert.ToBoolean(cus_est2.IsEstimateActive))
                            {
                                if (cus_est2.alter_job_number != null && cus_est2.alter_job_number != "")
                                    strOtherJobNum = "<p>" + cus_est2.alter_job_number.Trim() + "<p/>";
                                else
                                    strOtherJobNum = "<p>" + cus_est2.job_number.Trim() + "<p/>";

                            }
                            else
                            {
                                if (cus_est2.alter_job_number != null && cus_est2.alter_job_number != "")
                                    strOtherJobNum = "<p>" + cus_est2.alter_job_number.Trim() + "<p/>" + "<p>" + "Est.InActive" + "<p/>";
                                else
                                    strOtherJobNum = "<p>" + cus_est2.job_number.Trim() + "<p/>" + "<p>" + "Est.InActive" + "<p/>";
                            }


                        }
                        else
                        {
                            if (Convert.ToBoolean(cus_est2.IsEstimateActive))
                            {
                                if (cus_est2.alter_job_number != null && cus_est2.alter_job_number != "")
                                    strOtherJobNum =strOtherJobNum+ "<p>" + cus_est2.alter_job_number.Trim() + "<p/>";
                                else
                                    strOtherJobNum =strOtherJobNum+ "<p>" + cus_est2.job_number.Trim() + "<p/>";

                            }
                            else
                            {
                                if (cus_est2.alter_job_number != null && cus_est2.alter_job_number != "")
                                    strOtherJobNum =strOtherJobNum+ "<p>" + cus_est2.alter_job_number.Trim() + "<p/>" + "<p>" + "Est.InActive" + "<p/>";
                                else
                                    strOtherJobNum =strOtherJobNum+"<p>" + cus_est2.job_number.Trim() + "<p/>" + "<p>" + "Est.InActive" + "<p/>";
                            }

                        }
                    }
                }
                lblOtherJob.Text = strOtherJobNum;

                string strQ1 = "select * from customer_estimate where customer_id=" + ncid + " AND estimate_id = " + Convert.ToInt32(ddlEst.SelectedValue) + " and client_id = " + cust.client_id + " Order by convert(datetime,sale_date) desc";
                IEnumerable<customer_estimate_model> list1 = _db.ExecuteQuery<customer_estimate_model>(strQ1, string.Empty);

                Label lblActiveCust = (Label)e.Row.FindControl("lblActiveCust");

                if (status_id == 4)
                {
                    lblActiveCust.Text = "<p>" + "Archive" + "<p/>";
                }
                else if (status_id == 5)
                {
                    lblActiveCust.Text = "<p>" + "InActive" + "<p/>"; ;
                }
                else
                {
                    lblActiveCust.Text = string.Empty;
                }

                foreach (customer_estimate_model cus_est in list1)
                {
                    //string strJobNum = cus_est.job_number;
                    string strJobNum = string.Empty;
                    if (cus_est.alter_job_number != null && cus_est.alter_job_number != "")
                        strJobNum = cus_est.alter_job_number;
                    else
                        strJobNum = cus_est.job_number;
                    string strestimateName = cus_est.estimate_name;
                    int nestid = Convert.ToInt32(cus_est.estimate_id);
                    int nest_status_id = Convert.ToInt32(cus_est.status_id);

                    hypMessage.NavigateUrl += "&eid=" + nestid;
                    hyp_ProjectNotes.NavigateUrl += "&eid=" + nestid;
                    hyp_CallLog.NavigateUrl += "&eid=" + nestid;
                    hyp_DocumentManagement.NavigateUrl += "&eid=" + nestid;
                    hyp_Custd.NavigateUrl += "&eid=" + nestid;

                    if (nest_status_id == 3)
                    {
                        //hypSurvey Exit Questionnaire

                        hypEstDetail.ToolTip = "(SOLD) View Details ";
                        hypEstDetail.ImageUrl = "~/images/view_details_sold.png";
                        hyp_Job.Text = strJobNum;
                        hyp_Job.NavigateUrl = "sold_estimate.aspx?eid=" + nestid + "&cid=" + ncid;
                        Label lblActiveEst = (Label)e.Row.FindControl("lblActiveEst");

                        if (!Convert.ToBoolean(cus_est.IsEstimateActive))
                        {
                            lblActiveEst.Text = "<p>" + "Est.InActive" + "<p/>";
                        }
                        else
                        {
                            lblActiveEst.Text = string.Empty;
                        }
                        hyp_vendor.NavigateUrl = "Vendor_cost_details.aspx?eid=" + nestid + "&cid=" + ncid;
                        hyp_jstatus.NavigateUrl = "customer_job_status_info.aspx?eid=" + nestid + "&cid=" + ncid;
                        hyp_survey.NavigateUrl = "Customer_survey.aspx?eid=" + nestid + "&cid=" + ncid;
                        hyp_survey.Target = "_blank";

                        hypWarrenty.NavigateUrl = "warrentycertificate.aspx?eid=" + nestid + "&cid=" + ncid;
                        hypWarrenty.Target = "_blank";

                        hyp_Section_Selection.NavigateUrl = "selectionofsection.aspx?eid=" + nestid + "&nbackid=1&cid=" + ncid;
                        hyp_MaterialTracking.NavigateUrl = "material_traknig.aspx?eid=" + nestid + "&cid=" + ncid;
                        hyp_MaterialTracking.NavigateUrl = "material_traknig.aspx?eid=" + nestid + "&cid=" + ncid;
                        hyp_SiteReview.NavigateUrl = "sitereviewlist.aspx?nestid=" + nestid + "&nbackId=1&cid=" + ncid;


                        //arefin 03-15-2018
                        bool bIsEstimateActive = Convert.ToBoolean(cus_est.IsEstimateActive);
                        if (bIsEstimateActive)
                        {

                            hyp_Schedule.NavigateUrl = "schedulecalendar.aspx?eid=" + nestid + "&cid=" + ncid + "&TypeID=1";
                            hyp_Schedule.Attributes.Add("onclick", "return ShowPopUp('" + bIsEstimateActive + "');");
                        }
                        else
                        {
                            // hyp_Schedule.NavigateUrl = "";
                            hyp_Schedule.Attributes.Add("href", "");
                            hyp_Schedule.Attributes.Add("onclick", "return ShowPopUp('" + bIsEstimateActive + "');");
                        }
                        //arefin 03-15-2018

                        if (_db.customersurveys.Where(cl => cl.customerid == ncid && cl.estimate_id == nestid).ToList().Count > 0)
                        {
                            hyp_survey.Visible = true;
                        }
                        else
                        {
                            hyp_survey.Visible = false;
                        }



                        hyp_Sow.NavigateUrl = "composite_sow.aspx?eid=" + nestid + "&cid=" + ncid;
                        hypChangeOrderList.NavigateUrl = "changeorderlist.aspx?eid=" + nestid + "&cid=" + ncid;

                        hyp_PreCon.NavigateUrl = "PreconstructionCheckList.aspx?eid=" + nestid + "&cid=" + ncid;

                        if (_db.estimate_payments.Where(est_p => est_p.estimate_id == nestid && est_p.customer_id == ncid && est_p.client_id == 1).SingleOrDefault() == null)
                        {
                            hyp_Payment.NavigateUrl = "payment_info.aspx?eid=" + nestid + "&cid=" + ncid;

                        }
                        else
                        {
                            estimate_payment objEstPay = new estimate_payment();
                            objEstPay = _db.estimate_payments.Single(pay => pay.estimate_id == nestid && pay.customer_id == ncid && pay.client_id == 1);
                            hyp_Payment.NavigateUrl = "payment_recieved.aspx?cid=" + ncid + "&epid=" + objEstPay.est_payment_id + "&eid=" + nestid;

                        }
                        lblSaleDate.Text = Convert.ToDateTime(cus_est.sale_date).ToShortDateString();

                        // Estimate Change Order
                        string strQuery = "select change_order_estimate_id, chage_order_id, estimate_id, customer_id, client_id, sales_person_id, change_order_status_id, changeorder_name, change_order_type_id, payment_terms, other_terms, is_total, is_tax, tax, total_payment_due, execute_date, is_close, comments, notes1, changeorder_date, create_date, last_updated_date from changeorder_estimate where customer_id=" + ncid + " AND estimate_id = " + nestid + " AND client_id = " + cust.client_id ;
                        IEnumerable<ChangeOrderEstimateModel> COlistItem = _db.ExecuteQuery<ChangeOrderEstimateModel>(strQuery, string.Empty);
                        ddlEstCO.DataSource = COlistItem;
                        ddlEstCO.DataTextField = "changeorder_name";
                        ddlEstCO.DataValueField = "chage_order_id";
                        ddlEstCO.DataBind();

                        if (_db.changeorder_estimates.Where(ce => ce.customer_id == ncid && ce.estimate_id == nestid && ce.client_id == 1).ToList().Count > 0)
                        {
                            ddlEstCO.Visible = true;
                            hypEstCODetail.Visible = true;
                            int nEstCOId = 0;
                            var result = (from ce in _db.changeorder_estimates
                                          where ce.customer_id == ncid && ce.estimate_id == nestid && ce.client_id == 1
                                          select ce.chage_order_id);

                            int n = result.Count();
                            if (result != null && n > 0)
                                nEstCOId = result.Max();
                            ddlEstCO.SelectedValue = nEstCOId.ToString();

                        }
                        else
                        {
                            ddlEstCO.Visible = false;
                            hypEstCODetail.Visible = false;

                        }
                        int nCOID = 0;
                        if (ddlEstCO.SelectedValue != "")
                        {
                            nCOID = Convert.ToInt32(ddlEstCO.SelectedValue);
                        }
                        string strQueryCO = "select change_order_estimate_id, chage_order_id, estimate_id, customer_id, client_id, sales_person_id, change_order_status_id, changeorder_name, change_order_type_id, payment_terms, other_terms, is_total, is_tax, tax, total_payment_due, execute_date, is_close, comments, notes1, changeorder_date, create_date, last_updated_date from changeorder_estimate where chage_order_id =" + nCOID + " and  customer_id=" + ncid + " AND estimate_id = " + nestid + " AND client_id = " + cust.client_id;
                        IEnumerable<ChangeOrderEstimateModel> listItemCO = _db.ExecuteQuery<ChangeOrderEstimateModel>(strQueryCO, string.Empty);



                        foreach (ChangeOrderEstimateModel cho in listItemCO)
                        {
                            string strChangeOrderName = cho.changeorder_name;
                            int nChangeOrderId = Convert.ToInt32(cho.chage_order_id);
                            int est_status_id = Convert.ToInt32(cho.change_order_status_id);


                            if (est_status_id == 3)
                            {
                                hypEstCODetail.ToolTip = "( Executed ) View Details";
                                hypEstCODetail.ImageUrl = "~/images/view_details_executed.png";


                            }
                            else if (est_status_id == 4)
                            {
                                hypEstCODetail.ToolTip = "( Declined ) View Details";
                                hypEstCODetail.ImageUrl = "~/images/view_details_declined.png";
                            }
                            else
                            {
                                hypEstCODetail.ToolTip = "Currently Open Change Order";
                                hypEstCODetail.ImageUrl = "~/images/view_details.png";
                            }
                            hypEstCODetail.NavigateUrl = "change_order_locations.aspx?coestid=" + nChangeOrderId + "&eid=" + nestid + "&cid=" + ncid;

                        }
                        if (_db.changeorder_estimates.Where(ce => ce.customer_id == ncid && ce.estimate_id == nestid && ce.client_id == 1 && ce.is_close == false).ToList().Count > 0)
                        {
                            hypCOCommon.Visible = false;


                        }
                        else
                        {
                            hypCOCommon.Visible = true;
                            hypCOCommon.NavigateUrl = "change_order_locations.aspx?eid=" + nestid + "&cid=" + ncid;

                        }

                    }
                    else
                    {

                        hypEstDetail.ToolTip = "View Details";//strestimateName;
                        hypEstDetail.ImageUrl = "~/images/view_details.png";
                    }
                    //hypCostLoc.Text = " [C by Loc]";
                    hypCostLoc.NavigateUrl = "ProjectSummaryReport.aspx?TypeId=1&eid=" + nestid + "&cid=" + ncid;
                    hypCostLoc.Target = "_blank";
                    //hypCostSec.Text = " [C by Sec]";
                    //hypCostSec.NavigateUrl = "CostPerEstimateReport.aspx?TypeId=2&eid=" + nestid + "&cid=" + ncid;
                    //hypCostSec.Target = "_blank";
                    hypEstDetail.NavigateUrl = "customer_locations.aspx?eid=" + nestid + "&cid=" + ncid;
                    hyp_Allowance.NavigateUrl = "AllowanceReport.aspx?eid=" + nestid + "&cid=" + ncid;

                }


                hypCommon.Text = "New Estimate";
                hypCommon.NavigateUrl = "customer_locations.aspx?cid=" + ncid;
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
                if (tools.Contains("Payment"))
                    hyp_Payment.Visible = true;
                else
                    hyp_Payment.Visible = false;
                if (tools.Contains("JobStatus"))
                    hyp_jstatus.Visible = true;
                else
                    hyp_jstatus.Visible = false;
                if (tools.Contains("Schedule"))
                    hyp_Schedule.Visible = true;
                else
                    hyp_Schedule.Visible = false;
                if (tools.Contains("CompositeSow"))
                    hyp_Sow.Visible = true;
                else
                    hyp_Sow.Visible = false;



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
                if (tools.Contains("PreConCheckList"))
                    hyp_PreCon.Visible = true;
                else
                    hyp_PreCon.Visible = false;
                if (tools.Contains("SiteReview"))
                    hyp_SiteReview.Visible = true;
                else
                    hyp_SiteReview.Visible = false;
                if (tools.Contains("DocumentManagement"))
                    hyp_DocumentManagement.Visible = true;
                else
                    hyp_DocumentManagement.Visible = false;

                if (tools.Contains("Selection"))
                    hyp_Section_Selection.Visible = true;
                else
                    hyp_Section_Selection.Visible = false;
                if (tools.Contains("MaterialTracking"))
                    hyp_MaterialTracking.Visible = true;
                else
                    hyp_MaterialTracking.Visible = false;

                if (tools.Contains("ProjectSummary"))
                    hypCostLoc.Visible = true;
                else
                    hypCostLoc.Visible = false;
                if (tools.Contains("ProjectComWarrenty"))
                    hypWarrenty.Visible = true;
                else
                    hypWarrenty.Visible = false;

                if (tools.Contains("ChangeOrderList"))
                    hypChangeOrderList.Visible = true;
                else
                    hypChangeOrderList.Visible = false;
            }
        }
    }

    protected void GotoPricing(object sender, EventArgs e)
    {
        LinkButton lnk = (LinkButton)sender;
        int ncid = Convert.ToInt32(lnk.CommandArgument);

        Response.Redirect("customer_locations.aspx?cid=" + ncid);
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
        Session.Remove("CustomerId");
        GetCustomersNew(0);
    }
    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlStatus.ID, ddlStatus.GetType().Name, "SelectedIndexChanged"); 
        Session.Remove("CustomerId");
        txtSearch.Text = string.Empty;
        BindJobNumber();
        GetCustomersNew(0);
    }
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        ddlStatus.SelectedValue = "2";
        ddlSalesRep.SelectedIndex = -1;
        ddlDivision.SelectedValue = hdnPrimaryDivision.Value;
        ddlSuperintendent.SelectedIndex = -1;
        ddlSearchBy.SelectedValue = "2";
        ddlSearchBy_SelectedIndexChanged(sender, e);
        Session.Remove("CustomerId");
        BindJobNumber();
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
            string strQ = " SELECT customers.customer_id,customer_estimate.estimate_id,customer_estimate.job_number,ISNULL(t1.Total_Price,0) AS Total_Price,ISNULL (estimate_payments.new_total_with_tax,0) AS NewTotal_Price ,customer_estimate.estimate_name,customer_estimate.sale_date, " +
                          " customers.first_name1, customers.last_name1, customers.address,  customers.city, customers.state, customers.zip_code, customers.email, customers.phone, customer_estimate.sales_person_id, " +
                          " sales_person.first_name +' '+last_name AS SalesRep,  customers.status_id, customers.notes, customers.appointment_date, lead_source.lead_source_id,lead_source.lead_name " +
                          " FROM customers  " +
                          " INNER JOIN customer_estimate ON  customer_estimate.customer_id = customers.customer_id " +
                          " INNER JOIN lead_source ON  lead_source.lead_source_id = customers.lead_source_id " +
                          " INNER JOIN sales_person ON  sales_person.sales_person_id = customer_estimate.sales_person_id " +
                          " LEFT OUTER JOIN (SELECT pd.customer_id,pd.estimate_id,ISNULL(SUM(pd.total_retail_price),0) AS Total_Price FROM pricing_details pd  GROUP BY pd.customer_id,pd.estimate_id) AS t1 ON t1.customer_id = customer_estimate.customer_id AND t1.estimate_id = customer_estimate.estimate_id " +
                          " LEFT OUTER JOIN estimate_payments ON estimate_payments.customer_id = customer_estimate.customer_id AND estimate_payments.estimate_id = customer_estimate.estimate_id " +
                          " WHERE customers.customer_id in (" + strID + ")  AND customer_estimate.status_id=3 order by CONVERT(DATETIME,customer_estimate.sale_date) desc";

            DataTable dtReport = csCommonUtility.GetDataTable(strQ);

            DataTable tmpTable = LoadTmpDataTable();
            DataClassesDataContext _db = new DataClassesDataContext();

            foreach (DataRow dr in dtReport.Rows)
            {
                decimal nTotal_Price = 0;

                if (Convert.ToDecimal(dr["NewTotal_Price"]) == 0)
                {
                    nTotal_Price = Convert.ToDecimal(dr["Total_Price"]);
                }
                else
                {
                    nTotal_Price = Convert.ToDecimal(dr["NewTotal_Price"]);

                }
                DataRow drNew = tmpTable.NewRow();
                drNew["Job Number"] = dr["job_number"];
                drNew["Estimate"] = dr["estimate_name"];
                drNew["First Name"] = dr["first_name1"];
                drNew["Last Name"] = dr["last_name1"];
                drNew["Phone"] = dr["phone"];
                drNew["Email"] = dr["email"];
                drNew["Address"] = dr["address"].ToString() + ' ' + dr["city"] + ',' + ' ' + dr["state"] + ' ' + dr["zip_code"];
                drNew["Lead Source"] = dr["lead_name"];
                drNew["Sale Date"] = dr["sale_date"];
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

        table.Columns.Add("Job Number", typeof(string));
        table.Columns.Add("Estimate", typeof(string));
        table.Columns.Add("First Name", typeof(string));
        table.Columns.Add("Last Name", typeof(string));
        table.Columns.Add("Phone", typeof(string));
        table.Columns.Add("Email", typeof(string));
        table.Columns.Add("Address", typeof(string));
        table.Columns.Add("Lead Source", typeof(string));
        table.Columns.Add("Sale Date", typeof(string));
        table.Columns.Add("Sales Rep", typeof(string));
        table.Columns.Add("Sale Amount", typeof(string));

        return table;
    }
    protected void ddlSalesRep_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSalesRep.ID, ddlSalesRep.GetType().Name, "SelectedIndexChanged"); 
        Session.Remove("CustomerId");
        txtSearch.Text = string.Empty;
        
        GetCustomersNew(0);
    }
    protected void ddlSuperintendent_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSuperintendent.ID, ddlSuperintendent.GetType().Name, "SelectedIndexChanged"); 
        txtSearch.Text = string.Empty;
        Session.Remove("CustomerId");
        GetCustomersNew(0);
    }
    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "SelectedIndexChanged"); 
        txtSearch.Text = "";
        wtmFileNumber.WatermarkText = "Search by " + ddlSearchBy.SelectedItem.Text;

        if (ddlSearchBy.SelectedValue == "1")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetFirstName";
        }
        else if (ddlSearchBy.SelectedValue == "2")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetLastName";
        }
        else if (ddlSearchBy.SelectedValue == "4")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetAddress";
        }
        else if (ddlSearchBy.SelectedValue == "3")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetEmail";
        }
        else if (ddlSearchBy.SelectedValue == "7")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetPhone";
        }
        else if (ddlSearchBy.SelectedValue == "5")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetJobNumber";
        }
       // GetCustomersNew(0);
    }
    protected void Load_Est_Info(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        GridViewRow diitem = ((GridViewRow)((DropDownList)sender).NamingContainer);
        GridView grdCustomerList = (GridView)diitem.NamingContainer;
        DropDownList ddlEst = (DropDownList)diitem.FindControl("ddlEst");
        HyperLink hypEstDetail = (HyperLink)diitem.FindControl("hypEstDetail");
        HyperLink hypCostLoc = (HyperLink)diitem.FindControl("hypCostLoc");
        //HyperLink hypCostSec = (HyperLink)diitem.FindControl("hypCostSec");
        HyperLink hypCommon = (HyperLink)diitem.FindControl("hypCommon");
        HyperLink hyp_vendor = (HyperLink)diitem.FindControl("hyp_vendor");
        HyperLink hyp_Payment = (HyperLink)diitem.FindControl("hyp_Payment");
        HyperLink hyp_jstatus = (HyperLink)diitem.FindControl("hyp_jstatus");
        HyperLink hyp_Schedule = (HyperLink)diitem.FindControl("hyp_Schedule");
        HyperLink hyp_survey = (HyperLink)diitem.FindControl("hyp_survey");
        HyperLink hyp_Sow = (HyperLink)diitem.FindControl("hyp_Sow");
        HyperLink hyp_Job = (HyperLink)diitem.FindControl("hyp_Job");
        Label lblSaleDate = (Label)diitem.FindControl("lblSaleDate");
        HyperLink hyp_Allowance = (HyperLink)diitem.FindControl("hyp_Allowance");
        Label lblOtherJob = (Label)diitem.FindControl("lblOtherJob");
        Label lblJobJost = (Label)diitem.FindControl("lblJobJost");

        HyperLink hyp_Section_Selection = (HyperLink)diitem.FindControl("hyp_Section_Selection");
        HyperLink hyp_MaterialTracking = (HyperLink)diitem.FindControl("hyp_MaterialTracking");
        HyperLink hyp_SiteReview = (HyperLink)diitem.FindControl("hyp_SiteReview");
        HyperLink hyp_PreCon = (HyperLink)diitem.FindControl("hyp_PreCon");
        HyperLink hypMessage = (HyperLink)diitem.FindControl("hypMessage");

        HyperLink hyp_Custd = (HyperLink)diitem.FindControl("hyp_Custd");
        HyperLink hyp_ProjectNotes = (HyperLink)diitem.FindControl("hyp_ProjectNotes");
        HyperLink hyp_CallLog = (HyperLink)diitem.FindControl("hyp_CallLog");
        HyperLink hyp_DocumentManagement = (HyperLink)diitem.FindControl("hyp_DocumentManagement");
        HyperLink hypWarrenty = (HyperLink)diitem.FindControl("hypWarrenty");

        // Customer CO
        DropDownList ddlEstCO = (DropDownList)diitem.FindControl("ddlEstCO");
        HyperLink hypEstCODetail = (HyperLink)diitem.FindControl("hypEstCODetail");
        HyperLink hypCOCommon = (HyperLink)diitem.FindControl("hypCOCommon");


        int ncid = Convert.ToInt32(grdCustomerList.DataKeys[diitem.RowIndex].Values[0]);

        customer cust = new customer();
        cust = _db.customers.Single(c => c.customer_id == ncid);

        decimal TotalExCom_WithIntevcise = GetRetailTotal(Convert.ToInt32(ddlEst.SelectedValue), ncid, (int)cust.client_id);
        lblJobJost.Text = "Estimate Amount: " + TotalExCom_WithIntevcise.ToString("c");
        string strQ2 = "select * from customer_estimate where customer_id=" + ncid + " AND estimate_id != " + Convert.ToInt32(ddlEst.SelectedValue) + " and client_id=" + Convert.ToInt32(cust.client_id) + " Order by convert(datetime,sale_date) desc";
        IEnumerable<customer_estimate_model> list2 = _db.ExecuteQuery<customer_estimate_model>(strQ2, string.Empty);
        string strOtherJobNum = string.Empty;
        foreach (customer_estimate_model cus_est2 in list2)
        {
            if (cus_est2.job_number != null)
            {
                //if (strOtherJobNum == "")
                //    strOtherJobNum = "<p>" + cus_est2.job_number.Trim() + "<p/>";
                //else
                //    strOtherJobNum = strOtherJobNum + "<p>" + cus_est2.job_number.Trim() + "<p/>";
                if (strOtherJobNum == "")
                {
                    if (Convert.ToBoolean(cus_est2.IsEstimateActive))
                    {
                        if (cus_est2.alter_job_number != null && cus_est2.alter_job_number != "")
                            strOtherJobNum = "<p>" + cus_est2.alter_job_number.Trim() + "<p/>";
                        else
                            strOtherJobNum = "<p>" + cus_est2.job_number.Trim() + "<p/>";

                    }
                    else
                    {
                        if (cus_est2.alter_job_number != null && cus_est2.alter_job_number != "")
                            strOtherJobNum = "<p>" + cus_est2.alter_job_number.Trim() + "<p/>" + "<p>" + "Est.InActive" + "<p/>";
                        else
                            strOtherJobNum = "<p>" + cus_est2.job_number.Trim() + "<p/>" + "<p>" + "Est.InActive" + "<p/>";
                    }


                }
                else
                {
                    if (Convert.ToBoolean(cus_est2.IsEstimateActive))
                    {
                        if (cus_est2.alter_job_number != null && cus_est2.alter_job_number != "")
                            strOtherJobNum =strOtherJobNum+ "<p>" + cus_est2.alter_job_number.Trim() + "<p/>";
                        else
                            strOtherJobNum =strOtherJobNum+"<p>" + cus_est2.job_number.Trim() + "<p/>";

                    }
                    else
                    {
                        if (cus_est2.alter_job_number != null && cus_est2.alter_job_number != "")
                            strOtherJobNum =strOtherJobNum+ "<p>" + cus_est2.alter_job_number.Trim() + "<p/>" + "<p>" + "Est.InActive" + "<p/>";
                        else
                            strOtherJobNum =strOtherJobNum+ "<p>" + cus_est2.job_number.Trim() + "<p/>" + "<p>" + "Est.InActive" + "<p/>";
                    }

                }
            }
        }
        lblOtherJob.Text = strOtherJobNum;

        string strQ1 = "select * from customer_estimate where customer_id=" + ncid + " AND estimate_id = " + Convert.ToInt32(ddlEst.SelectedValue) + " and client_id=" + Convert.ToInt32(cust.client_id) + " Order by convert(datetime,sale_date) desc";
        IEnumerable<customer_estimate_model> list1 = _db.ExecuteQuery<customer_estimate_model>(strQ1, string.Empty);

        foreach (customer_estimate_model cus_est in list1)
        {
            //string strJobNum = cus_est.job_number;
            string strJobNum = string.Empty;
            if (cus_est.alter_job_number != null && cus_est.alter_job_number != "")
                strJobNum = cus_est.alter_job_number;
            else
                strJobNum = cus_est.job_number;
            string strestimateName = cus_est.estimate_name;
            int nestid = Convert.ToInt32(cus_est.estimate_id);
            int nest_status_id = Convert.ToInt32(cus_est.status_id);

            if (hdnEmailType.Value == "1")
                hypMessage.NavigateUrl = "customermessagecenteroutlook.aspx?cid=" + ncid + "&eid=" + nestid;
            else
                hypMessage.NavigateUrl = "customermessagecenter.aspx?cid=" + ncid + "&eid=" + nestid;

            hyp_Custd.NavigateUrl = "customer_details.aspx?cid=" + ncid + "&eid=" + nestid;
            hyp_ProjectNotes.NavigateUrl = "ProjectNotes.aspx?cid=" + ncid + "&TypeId=2&eid=" + nestid;
            hyp_CallLog.NavigateUrl = "CallLogInfo.aspx?cid=" + ncid + "&TypeId=2&eid=" + nestid;
            hyp_DocumentManagement.NavigateUrl = "DocumentManagement.aspx?cid=" + ncid + "&nbackId=1&eid=" + nestid;
            hyp_Section_Selection.NavigateUrl = "selectionofsection.aspx?eid=" + nestid + "&nbackid=1&cid=" + ncid;
            hyp_MaterialTracking.NavigateUrl = "material_traknig.aspx?eid=" + nestid + "&cid=" + ncid;

            if (nest_status_id == 3)
            {
                //hypSurvey Exit Questionnaire
                hyp_vendor.Visible = true;
                hyp_Payment.Visible = true;
                hyp_jstatus.Visible = true;
                hyp_Schedule.Visible = true;
                hyp_Sow.Visible = true;
                hyp_Job.Visible = true;
                hyp_PreCon.Visible = true;

                hypEstDetail.ToolTip = "(SOLD) View Details ";
                hypEstDetail.ImageUrl = "~/images/view_details_sold.png";
                hyp_Job.Text = strJobNum;
                hyp_Job.NavigateUrl = "sold_estimate.aspx?eid=" + nestid + "&cid=" + ncid;
                hyp_vendor.NavigateUrl = "Vendor_cost_details.aspx?eid=" + nestid + "&cid=" + ncid;
                hyp_jstatus.NavigateUrl = "customer_job_status_info.aspx?eid=" + nestid + "&cid=" + ncid;
                hyp_survey.NavigateUrl = "Customer_survey.aspx?eid=" + nestid + "&cid=" + ncid;
                hyp_survey.Target = "_blank";

                hypWarrenty.NavigateUrl = "warrentycertificate.aspx?eid=" + nestid + "&cid=" + ncid;
                hypWarrenty.Target = "_blank";

                hyp_Section_Selection.NavigateUrl = "selectionofsection.aspx?eid=" + nestid + "&nbackid=1&cid=" + ncid;
                hyp_MaterialTracking.NavigateUrl = "material_traknig.aspx?eid=" + nestid + "&cid=" + ncid;
                hyp_SiteReview.NavigateUrl = "sitereviewlist.aspx?nestid=" + nestid + "&nbackId=1&cid=" + ncid;

                lblSaleDate.Text = Convert.ToDateTime(cus_est.sale_date).ToShortDateString();

                //arefin 03-15-2018
                bool IsEstimateActive = Convert.ToBoolean(cus_est.IsEstimateActive);
                if (IsEstimateActive)
                {

                    hyp_Schedule.NavigateUrl = "schedulecalendar.aspx?eid=" + Convert.ToInt32(ddlEst.SelectedValue) + "&cid=" + ncid + "&TypeID=1";
                    hyp_Schedule.Attributes.Add("onclick", "return ShowPopUp('" + IsEstimateActive + "');");
                }
                else
                {
                    // hyp_Schedule.NavigateUrl = "";
                    hyp_Schedule.Attributes.Add("href", "");
                    hyp_Schedule.Attributes.Add("onclick", "return ShowPopUp('" + IsEstimateActive + "');");
                }
                //arefin 03-15-2018

                if (_db.customersurveys.Where(cl => cl.customerid == ncid && cl.estimate_id == nestid).ToList().Count > 0)
                {
                    hyp_survey.Visible = true;
                }
                else
                {
                    hyp_survey.Visible = false;
                }
                // hyp_Schedule.NavigateUrl = "schedulecalendar.aspx?eid=" + nestid + "&cid=" + ncid + "&TypeID=1";
                hyp_Sow.NavigateUrl = "composite_sow.aspx?eid=" + nestid + "&cid=" + ncid;
                hyp_PreCon.NavigateUrl = "PreconstructionCheckList.aspx?eid=" + nestid + "&cid=" + ncid;

                if (_db.estimate_payments.Where(est_p => est_p.estimate_id == nestid && est_p.customer_id == ncid && est_p.client_id == cust.client_id).SingleOrDefault() == null)
                {
                    hyp_Payment.NavigateUrl = "payment_info.aspx?eid=" + nestid + "&cid=" + ncid;

                }
                else
                {
                    estimate_payment objEstPay = new estimate_payment();
                    objEstPay = _db.estimate_payments.Single(pay => pay.estimate_id == nestid && pay.customer_id == ncid && pay.client_id == cust.client_id);
                    hyp_Payment.NavigateUrl = "payment_recieved.aspx?cid=" + ncid + "&epid=" + objEstPay.est_payment_id + "&eid=" + nestid;

                }

                // Estimate Change Order
                string strQuery = "select change_order_estimate_id, chage_order_id, estimate_id, customer_id, client_id, sales_person_id, change_order_status_id, changeorder_name, change_order_type_id, payment_terms, other_terms, is_total, is_tax, tax, total_payment_due, execute_date, is_close, comments, notes1, changeorder_date, create_date, last_updated_date from changeorder_estimate where customer_id=" + ncid + " AND estimate_id = " + nestid + " AND client_id=" + Convert.ToInt32(cust.client_id);
                IEnumerable<ChangeOrderEstimateModel> COlistItem = _db.ExecuteQuery<ChangeOrderEstimateModel>(strQuery, string.Empty);
                ddlEstCO.DataSource = COlistItem;
                ddlEstCO.DataTextField = "changeorder_name";
                ddlEstCO.DataValueField = "chage_order_id";
                ddlEstCO.DataBind();
                int nChOrderId = 0;
                if (_db.changeorder_estimates.Where(ce => ce.customer_id == ncid && ce.estimate_id == nestid && ce.client_id == cust.client_id).ToList().Count > 0)
                {
                    ddlEstCO.Visible = true;
                    hypEstCODetail.Visible = true;
                    int nEstCOId = 0;
                    var result = (from ce in _db.changeorder_estimates
                                  where ce.customer_id == ncid && ce.estimate_id == nestid && ce.client_id == cust.client_id
                                  select ce.chage_order_id);

                    int n = result.Count();
                    if (result != null && n > 0)
                        nEstCOId = result.Max();
                    ddlEstCO.SelectedValue = nEstCOId.ToString();
                    nChOrderId = nEstCOId;

                }
                else
                {
                    hypEstCODetail.Visible = false;
                    ddlEstCO.Visible = false;

                }
                string strQueryCO = "select change_order_estimate_id, chage_order_id, estimate_id, customer_id, client_id, sales_person_id, change_order_status_id, changeorder_name, change_order_type_id, payment_terms, other_terms, is_total, is_tax, tax, total_payment_due, execute_date, is_close, comments, notes1, changeorder_date, create_date, last_updated_date from changeorder_estimate where chage_order_id =" + nChOrderId + " and  customer_id=" + ncid + " AND estimate_id = " + nestid + " AND client_id=" + Convert.ToInt32(cust.client_id);
                IEnumerable<ChangeOrderEstimateModel> listItemCO = _db.ExecuteQuery<ChangeOrderEstimateModel>(strQueryCO, string.Empty);

                foreach (ChangeOrderEstimateModel cho in listItemCO)
                {
                    string strChangeOrderName = cho.changeorder_name;
                    int nChangeOrderId = Convert.ToInt32(cho.chage_order_id);
                    int est_status_id = Convert.ToInt32(cho.change_order_status_id);



                    if (est_status_id == 3)
                    {
                        hypEstCODetail.ToolTip = "( Executed ) View Details";
                        hypEstCODetail.ImageUrl = "~/images/view_details_executed.png";


                    }
                    else if (est_status_id == 4)
                    {
                        hypEstCODetail.ToolTip = "( Declined ) View Details";
                        hypEstCODetail.ImageUrl = "~/images/view_details_declined.png";
                    }
                    else
                    {
                        hypEstCODetail.ToolTip = "Currently Open Change Order";
                        hypEstCODetail.ImageUrl = "~/images/view_details.png";
                    }

                    hypEstCODetail.NavigateUrl = "change_order_locations.aspx?coestid=" + nChangeOrderId + "&eid=" + nestid + "&cid=" + ncid;

                }
                if (_db.changeorder_estimates.Where(ce => ce.customer_id == ncid && ce.estimate_id == nestid && ce.client_id == cust.client_id && ce.is_close == false).ToList().Count > 0)
                {
                    hypCOCommon.Visible = false;


                }
                else
                {
                    hypCOCommon.Visible = true;
                    hypCOCommon.NavigateUrl = "change_order_locations.aspx?eid=" + nestid + "&cid=" + ncid;

                }


            }
            else
            {

                hypEstDetail.ToolTip = "View Details ";
                hypEstDetail.ImageUrl = "~/images/view_details.png";
                hyp_vendor.Visible = false;
                hyp_Payment.Visible = false;
                hyp_jstatus.Visible = false;
                hyp_Schedule.Visible = false;
                hyp_survey.Visible = false;
                hyp_Sow.Visible = false;
                hyp_Job.Visible = false;
                lblSaleDate.Text = "";
                hyp_PreCon.Visible = false;
                hypCOCommon.Visible = false;
                ddlEstCO.Visible = false;
                hypEstCODetail.Visible = false;

            }
            //hypCostLoc.Text = " [C by Loc]";
            hypCostLoc.NavigateUrl = "ProjectSummaryReport.aspx?TypeId=1&eid=" + nestid + "&cid=" + ncid;
            hypCostLoc.Target = "_blank";
            //hypCostSec.Text = " [C by Sec]";
            //hypCostSec.NavigateUrl = "CostPerEstimateReport.aspx?TypeId=2&eid=" + nestid + "&cid=" + ncid;
            //hypCostSec.Target = "_blank";
            hypEstDetail.NavigateUrl = "customer_locations.aspx?eid=" + nestid + "&cid=" + ncid;
            hyp_Allowance.NavigateUrl = "AllowanceReport.aspx?eid=" + nestid + "&cid=" + ncid;

        }




    }

    protected void Load_COEst_Info(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        GridViewRow diitem = ((GridViewRow)((DropDownList)sender).NamingContainer);
        GridView grdCustomerList = (GridView)diitem.NamingContainer;
        DropDownList ddlEst = (DropDownList)diitem.FindControl("ddlEst");
        // Customer CO
        DropDownList ddlEstCO = (DropDownList)diitem.FindControl("ddlEstCO");
        HyperLink hypEstCODetail = (HyperLink)diitem.FindControl("hypEstCODetail");
        HyperLink hypCOCommon = (HyperLink)diitem.FindControl("hypCOCommon");


        int ncid = Convert.ToInt32(grdCustomerList.DataKeys[diitem.RowIndex].Values[0]);

        customer cust = _db.customers.Single(c => c.customer_id == ncid);

        int nestid = Convert.ToInt32(ddlEst.SelectedValue);


        string strQueryCO = "select change_order_estimate_id, chage_order_id, estimate_id, customer_id, client_id, sales_person_id, change_order_status_id, changeorder_name, change_order_type_id, payment_terms, other_terms, is_total, is_tax, tax, total_payment_due, execute_date, is_close, comments, notes1, changeorder_date, create_date, last_updated_date from changeorder_estimate where chage_order_id =" + Convert.ToInt32(ddlEstCO.SelectedValue) + " and  customer_id=" + ncid + " AND estimate_id = " + nestid + " AND client_id=" + Convert.ToInt32(cust.client_id);
        IEnumerable<ChangeOrderEstimateModel> listItemCO = _db.ExecuteQuery<ChangeOrderEstimateModel>(strQueryCO, string.Empty);



        foreach (ChangeOrderEstimateModel cho in listItemCO)
        {
            string strChangeOrderName = cho.changeorder_name;
            int nChangeOrderId = Convert.ToInt32(cho.chage_order_id);
            int est_status_id = Convert.ToInt32(cho.change_order_status_id);



            if (est_status_id == 3)
            {
                hypEstCODetail.ToolTip = "( Executed ) View Details";
                hypEstCODetail.ImageUrl = "~/images/view_details_executed.png";


            }
            else if (est_status_id == 4)
            {
                hypEstCODetail.ToolTip = "( Declined ) View Details";
                hypEstCODetail.ImageUrl = "~/images/view_details_declined.png";
            }
            else
            {
                hypEstCODetail.ToolTip = "Currently Open Change Order";
                hypEstCODetail.ImageUrl = "~/images/view_details.png";
            }


            hypEstCODetail.NavigateUrl = "change_order_locations.aspx?coestid=" + nChangeOrderId + "&eid=" + nestid + "&cid=" + ncid;

        }
        if (_db.changeorder_estimates.Where(ce => ce.customer_id == ncid && ce.estimate_id == nestid && ce.client_id == cust.client_id && ce.is_close == false).ToList().Count > 0)
        {
            hypCOCommon.Visible = false;


        }
        else
        {
            hypCOCommon.Visible = true;
            hypCOCommon.NavigateUrl = "change_order_locations.aspx?eid=" + nestid + "&cid=" + ncid;

        }
    }

    protected void btnOperationCalendar_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("schedulecalendar.aspx?TypeID=1");
    }

    protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetCustomersNew(0);
    }
}
