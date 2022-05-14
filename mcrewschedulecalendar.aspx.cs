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
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Net.Mail;

public partial class mcrewschedulecalendar : System.Web.UI.Page
{


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);

            if (Session["oUser"] == null && Session["oCrew"] == null)
            {
                Response.Redirect("mobile.aspx");
            }
            string strUName = "";
            int nEmployeeID = 0;
            int superid = 0;

            if (Session["oCrew"] != null)
            {
                Crew_Detail objC = (Crew_Detail)Session["oCrew"];

                strUName = objC.first_name.Trim() + " " + objC.last_name.Trim();
                nEmployeeID = objC.crew_id;
                HttpContext.Current.Session.Add("empname", strUName);
                HttpContext.Current.Session.Add("empid", nEmployeeID);
            }
            else if (Session["oUser"] != null)
            {
               
                    userinfo objUser = (userinfo)Session["oUser"];
                    //nCustomerID = Convert.ToInt32(Request.QueryString.Get("cid"));
                    if (objUser.role_id == 4)
                    {
                        superid = objUser.user_id;
                        HttpContext.Current.Session.Add("superid", superid);
                    }
                
            }

           





        }


    }

    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("mlandingpage.aspx");
    }


    [System.Web.Services.WebMethod]
    public static String SetLandingPageByCustIdandUserId(int custid)
    {


        string result = "";
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();



            string strQ = string.Empty;


            if (HttpContext.Current.Session["oUser"] != null)
            {
                userinfo obj = (userinfo)HttpContext.Current.Session["oUser"];



                customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == custid);

                strQ = "delete from searchcustomers WHERE last_name1 ='" + objCust.last_name1 + "' AND userId=" + obj.user_id + " AND first_name1='" + objCust.first_name1 + "' AND IsCrew=0 AND email='" + objCust.email + "'";
                _db.ExecuteCommand(strQ, string.Empty);

                strQ = "insert into searchcustomers " +
                             " SELECT  [client_id],[customer_id],[first_name1],[last_name1] ,[first_name2],[last_name2],[address] ,[cross_street], " +
                             " [city] ,[state] ,[zip_code] ,[fax] ,[email] ,[phone] ,[is_active],[registration_date],[sales_person_id],[update_date], " +
                             " [status_id],[notes] ,[appointment_date],[lead_source_id] ,[status_note],[company] ,[email2],[SuperintendentId], " +
                              " [mobile],[lead_status_id],[islead],[isCustomer],[website],[isCalendarOnline], " + obj.user_id + ",getdate(),0 " +
                               " FROM customers " +
                               " WHERE customer_id ='" + custid + "'";

                _db.ExecuteCommand(strQ, string.Empty);

            }

            if (HttpContext.Current.Session["oCrew"] != null)
            {
                Crew_Detail objC = (Crew_Detail)HttpContext.Current.Session["oCrew"];


                customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == custid);

                strQ = "delete from searchcustomers WHERE last_name1 ='" + objCust.last_name1 + "' AND userId=" + objC.crew_id + " AND first_name1='" + objCust.first_name1 + "' AND IsCrew=1  AND email='" + objCust.email + "'";
                _db.ExecuteCommand(strQ, string.Empty);

                strQ = "insert into searchcustomers " +
                             " SELECT  [client_id],[customer_id],[first_name1],[last_name1] ,[first_name2],[last_name2],[address] ,[cross_street], " +
                             " [city] ,[state] ,[zip_code] ,[fax] ,[email] ,[phone] ,[is_active],[registration_date],[sales_person_id],[update_date], " +
                             " [status_id],[notes] ,[appointment_date],[lead_source_id] ,[status_note],[company] ,[email2],[SuperintendentId], " +
                              " [mobile],[lead_status_id],[islead],[isCustomer],[website],[isCalendarOnline], " + objC.crew_id + ",getdate(),1 " +
                               " FROM customers " +
                               " WHERE customer_id ='" + custid + "'";

                _db.ExecuteCommand(strQ, string.Empty);

            }



            result = "Ok";
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        return result;
    }
}
