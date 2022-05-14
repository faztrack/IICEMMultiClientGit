using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mmyjobs : System.Web.UI.Page
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
            BindPastJobs();
            BindMyJobs();
            BindFutureJobs();
          }

     
    }

    private void BindFutureJobs()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            if (Session["oCrew"] != null)
            {
                Crew_Detail objC = (Crew_Detail)Session["oCrew"];

                string CrewName = objC.first_name.Trim() + " " + objC.last_name.Trim();
                string date = DateTime.Now.ToString("dd-MMM-yy");
                string strDateBetween = "ScheduleCalendar.event_start > '" + Convert.ToDateTime(date).AddHours(23)+"'";

                string strQ = " SELECT DISTINCT customers.customer_id AS customer_id,customers.last_name1 AS last_name,customers.address,customers.city,customers.state,customers.zip_code,customer_estimate.estimate_name AS ProjectName,t1.ScheduleDate FROM ScheduleCalendar " +
                       " INNER JOIN customer_estimate on  customer_estimate.customer_id = ScheduleCalendar.customer_id AND customer_estimate.estimate_id = ScheduleCalendar.estimate_id " +
                       " INNER JOIN customers on  customers.customer_id = customer_estimate.customer_id " +
                       " INNER JOIN (SELECT MIN(ScheduleCalendar.event_start) AS ScheduleDate,ScheduleCalendar.customer_id,ScheduleCalendar.estimate_id FROM ScheduleCalendar  WHERE ScheduleCalendar.employee_name  LIKE '%" + CrewName + "%' " + " AND DaTEDiff(DAY,GetDate(),ISNULL(ScheduleCalendar.event_start,'1900-01-01 00:00:00.000'))>= 0 AND " + strDateBetween + " GROUP BY ScheduleCalendar.customer_id,ScheduleCalendar.estimate_id) AS t1 on t1.customer_id = ScheduleCalendar.customer_id AND t1.estimate_id = ScheduleCalendar.estimate_id  " +
                       " WHERE customers.IsCustomer= 1 AND customer_estimate.IsEstimateActive = 1 AND ScheduleCalendar.employee_name  LIKE '%" + CrewName + "%' " +
                       " AND "+strDateBetween+" ORDER BY ScheduleDate ASC";
                List<csMyJobs> mList = _db.ExecuteQuery<csMyJobs>(strQ, string.Empty).ToList();


                if (mList.Count() > 0)
                {


                    grdFuture.DataSource = mList;
                    grdFuture.DataKeyNames = new string[] { "customer_id" };
                    grdFuture.DataBind();
                }
            }


            //User 
            if (Session["oUser"] != null)
            {
                userinfo objUser = (userinfo)Session["oUser"];
                int userId = objUser.user_id;
                string date = DateTime.Now.ToString("dd-MMM-yy");
                string strDateBetween = "ScheduleCalendar.event_start > '" + Convert.ToDateTime(date).AddHours(23) + "'";

                string strQ = " SELECT DISTINCT customers.customer_id AS customer_id,customers.last_name1 AS last_name,customers.address,customers.city,customers.state,customers.zip_code,customer_estimate.estimate_name AS ProjectName,t1.ScheduleDate FROM ScheduleCalendar " +
                       " INNER JOIN customer_estimate on  customer_estimate.customer_id = ScheduleCalendar.customer_id AND customer_estimate.estimate_id = ScheduleCalendar.estimate_id " +
                       " INNER JOIN customers on  customers.customer_id = customer_estimate.customer_id " +
                       " INNER JOIN (SELECT MIN(ScheduleCalendar.event_start) AS ScheduleDate,ScheduleCalendar.customer_id,ScheduleCalendar.estimate_id FROM ScheduleCalendar  WHERE  DaTEDiff(DAY,GetDate(),ISNULL(ScheduleCalendar.event_start,'1900-01-01 00:00:00.000'))>= 0 AND " + strDateBetween + " GROUP BY ScheduleCalendar.customer_id,ScheduleCalendar.estimate_id) AS t1 on t1.customer_id = ScheduleCalendar.customer_id AND t1.estimate_id = ScheduleCalendar.estimate_id  " +
                       " WHERE customers.IsCustomer= 1 AND customer_estimate.IsEstimateActive = 1 AND customers.SuperintendentId='" + userId + "'" +
                       " AND " + strDateBetween + " ORDER BY ScheduleDate ASC";
                List<csMyJobs> mList = _db.ExecuteQuery<csMyJobs>(strQ, string.Empty).ToList();


                if (mList.Count() > 0)
                {


                    grdFuture.DataSource = mList;
                    grdFuture.DataKeyNames = new string[] { "customer_id" };
                    grdFuture.DataBind();
                }
            }



        }
        catch (Exception ex)
        {
        }
    }

    private void BindPastJobs()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            if (Session["oCrew"] != null)
            {
                Crew_Detail objC = (Crew_Detail)Session["oCrew"];
                string date = DateTime.Now.ToString("dd-MMM-yy");
                string strDateBetween = "ScheduleCalendar.event_start < '" +Convert.ToDateTime(date)+"'" ;
                string CrewName = objC.first_name.Trim() + " " + objC.last_name.Trim();
                string strQ = " SELECT  DISTINCT TOP(5) customers.customer_id AS customer_id,customers.last_name1 AS last_name,customers.address,customers.city,customers.state,customers.zip_code,customer_estimate.estimate_name AS ProjectName,t1.ScheduleDate FROM ScheduleCalendar " +
                       " INNER JOIN customer_estimate on  customer_estimate.customer_id = ScheduleCalendar.customer_id AND customer_estimate.estimate_id = ScheduleCalendar.estimate_id " +
                       " INNER JOIN customers on  customers.customer_id = customer_estimate.customer_id " +
                       " INNER JOIN (SELECT Max(ScheduleCalendar.event_start) AS ScheduleDate,ScheduleCalendar.customer_id,ScheduleCalendar.estimate_id FROM ScheduleCalendar  WHERE ScheduleCalendar.employee_name  LIKE '%" + CrewName + "%' " + " AND DaTEDiff(DAY,GetDate(),ISNULL(ScheduleCalendar.event_start,'1900-01-01 00:00:00.000'))< 0 AND " + strDateBetween + " GROUP BY ScheduleCalendar.customer_id,ScheduleCalendar.estimate_id) AS t1 on t1.customer_id = ScheduleCalendar.customer_id AND t1.estimate_id = ScheduleCalendar.estimate_id  " +
                       " WHERE customers.IsCustomer= 1 AND customer_estimate.IsEstimateActive = 1 AND customer_estimate.IsEstimateActive = 1 AND ScheduleCalendar.employee_name  LIKE '%" + CrewName + "%' " +
                       " AND "+strDateBetween+" ORDER BY ScheduleDate desc";
                List<csMyJobs> mList = _db.ExecuteQuery<csMyJobs>(strQ, string.Empty).ToList();


                if (mList.Count() > 0)
                {


                    grdPast.DataSource = mList;
                    grdPast.DataKeyNames = new string[] { "customer_id" };
                    grdPast.DataBind();
                }
            }

            //UserInfo
            if (Session["oUser"] != null)
            {
                userinfo objUser = (userinfo)Session["oUser"];
                string date = DateTime.Now.ToString("dd-MMM-yy");
                string strDateBetween = "ScheduleCalendar.event_start < '" + Convert.ToDateTime(date) + "'";
                int userId = objUser.user_id;
                string strQ = " SELECT  DISTINCT TOP(5) customers.customer_id AS customer_id,customers.last_name1 AS last_name,customers.address,customers.city,customers.state,customers.zip_code,customer_estimate.estimate_name AS ProjectName,t1.ScheduleDate FROM ScheduleCalendar " +
                       " INNER JOIN customer_estimate on  customer_estimate.customer_id = ScheduleCalendar.customer_id AND customer_estimate.estimate_id = ScheduleCalendar.estimate_id " +
                       " INNER JOIN customers on  customers.customer_id = customer_estimate.customer_id " +
                       " INNER JOIN (SELECT Max(ScheduleCalendar.event_start) AS ScheduleDate,ScheduleCalendar.customer_id,ScheduleCalendar.estimate_id FROM ScheduleCalendar  WHERE  DaTEDiff(DAY,GetDate(),ISNULL(ScheduleCalendar.event_start,'1900-01-01 00:00:00.000'))< 0 AND " + strDateBetween + " GROUP BY ScheduleCalendar.customer_id,ScheduleCalendar.estimate_id) AS t1 on t1.customer_id = ScheduleCalendar.customer_id AND t1.estimate_id = ScheduleCalendar.estimate_id  " +
                       " WHERE customers.IsCustomer= 1 AND customer_estimate.IsEstimateActive = 1 AND customer_estimate.IsEstimateActive = 1  AND customers.SuperintendentId='" + userId + "'" +
                       " AND " + strDateBetween + " ORDER BY ScheduleDate desc";
                List<csMyJobs> mList = _db.ExecuteQuery<csMyJobs>(strQ, string.Empty).ToList();


                if (mList.Count() > 0)
                {


                    grdPast.DataSource = mList;
                    grdPast.DataKeyNames = new string[] { "customer_id" };
                    grdPast.DataBind();
                }
            }


        }
        catch (Exception ex)
        {
        }
    }

    private void BindMyJobs()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            if (Session["oCrew"] != null)
            {
                Crew_Detail objC = (Crew_Detail)Session["oCrew"];
                string date = DateTime.Now.ToString("dd-MMM-yy");
                string strDateBetween = "ScheduleCalendar.event_start between '" + Convert.ToDateTime(date) + "' and '" + Convert.ToDateTime(date).AddHours(23)+"'";

                string CrewName = objC.first_name.Trim() + " " + objC.last_name.Trim();
                string strQ = " SELECT DISTINCT customers.customer_id AS customer_id,customers.last_name1 AS last_name,customers.address,customers.city,customers.state,customers.zip_code,customer_estimate.estimate_name AS ProjectName,t1.ScheduleDate FROM ScheduleCalendar " +
                       " INNER JOIN customer_estimate on  customer_estimate.customer_id = ScheduleCalendar.customer_id AND customer_estimate.estimate_id = ScheduleCalendar.estimate_id " +
                       " INNER JOIN customers on  customers.customer_id = customer_estimate.customer_id " +
                       " INNER JOIN (SELECT MIN(ScheduleCalendar.event_start) AS ScheduleDate,ScheduleCalendar.customer_id,ScheduleCalendar.estimate_id FROM ScheduleCalendar  WHERE ScheduleCalendar.employee_name  LIKE '%" + CrewName + "%' " + " AND DaTEDiff(DAY,GetDate(),ISNULL(ScheduleCalendar.event_start,'1900-01-01 00:00:00.000'))>= 0 AND " + strDateBetween + " GROUP BY ScheduleCalendar.customer_id,ScheduleCalendar.estimate_id) AS t1 on t1.customer_id = ScheduleCalendar.customer_id AND t1.estimate_id = ScheduleCalendar.estimate_id  " +
                       " WHERE customers.IsCustomer= 1 AND customer_estimate.IsEstimateActive = 1 AND ScheduleCalendar.employee_name  LIKE '%" + CrewName + "%' " +
                       " AND "+strDateBetween+" ORDER BY ScheduleDate ASC";
                 List<csMyJobs> mList = _db.ExecuteQuery<csMyJobs>(strQ, string.Empty).ToList();


                 //string strAll = "SELECT DISTINCT customers.customer_id,customers.last_name1 AS LastName,customers.address,customers.city,customers.state,customers.zip_code,customer_estimate.estimate_name AS ProjectName,ISNULL(t1.ScheduleDate,'2100-01-01 00:00:00.000') AS ScheduleDate  FROM ScheduleCalendar " +
                 //                " INNER JOIN customer_estimate on  customer_estimate.customer_id = ScheduleCalendar.customer_id AND customer_estimate.estimate_id = ScheduleCalendar.estimate_id " +
                 //                " INNER JOIN customers on  customers.customer_id = customer_estimate.customer_id " +
                 //                " LEFT OUTER JOIN (SELECT MIN(ScheduleCalendar.event_start) AS ScheduleDate,ScheduleCalendar.customer_id,ScheduleCalendar.estimate_id FROM ScheduleCalendar  WHERE ScheduleCalendar.employee_name='John smith'AND DaTEDiff(DAY,GetDate(),ISNULL(ScheduleCalendar.event_start,'1900-01-01 00:00:00.000'))>= 0 GROUP BY ScheduleCalendar.customer_id,ScheduleCalendar.estimate_id) AS t1 on t1.customer_id = ScheduleCalendar.customer_id AND t1.estimate_id = ScheduleCalendar.estimate_id " +
                 //                " WHERE customers.IsCustomer= 1 AND ScheduleCalendar.employee_name='John smith' ORDER BY ScheduleDate ASC ";
                if (mList.Count() > 0)
                {


                    grdMyJobs.DataSource = mList;
                    grdMyJobs.DataKeyNames = new string[] { "customer_id" };
                    grdMyJobs.DataBind();
                }
            }


            if (Session["oUser"] != null)
            {
                userinfo objUser = (userinfo)Session["oUser"];
                string date = DateTime.Now.ToString("dd-MMM-yy");
                string strDateBetween = "ScheduleCalendar.event_start between '" + Convert.ToDateTime(date) + "' and '" + Convert.ToDateTime(date).AddHours(23) + "'";

                int userId =objUser.user_id;
                string strQ = " SELECT DISTINCT customers.customer_id AS customer_id,customers.last_name1 AS last_name,customers.address,customers.city,customers.state,customers.zip_code,customer_estimate.estimate_name AS ProjectName,t1.ScheduleDate FROM ScheduleCalendar " +
                       " INNER JOIN customer_estimate on  customer_estimate.customer_id = ScheduleCalendar.customer_id AND customer_estimate.estimate_id = ScheduleCalendar.estimate_id " +
                       " INNER JOIN customers on  customers.customer_id = customer_estimate.customer_id " +
                       " INNER JOIN (SELECT MIN(ScheduleCalendar.event_start) AS ScheduleDate,ScheduleCalendar.customer_id,ScheduleCalendar.estimate_id FROM ScheduleCalendar  WHERE   DaTEDiff(DAY,GetDate(),ISNULL(ScheduleCalendar.event_start,'1900-01-01 00:00:00.000'))>= 0 AND " + strDateBetween + " GROUP BY ScheduleCalendar.customer_id,ScheduleCalendar.estimate_id) AS t1 on t1.customer_id = ScheduleCalendar.customer_id AND t1.estimate_id = ScheduleCalendar.estimate_id  " +
                       " WHERE customers.IsCustomer= 1 AND customer_estimate.IsEstimateActive = 1 AND customers.SuperintendentId='" + userId + "'" +
                       " AND " + strDateBetween + " ORDER BY ScheduleDate ASC";
                List<csMyJobs> mList = _db.ExecuteQuery<csMyJobs>(strQ, string.Empty).ToList();


                if (mList.Count() > 0)
                {


                    grdMyJobs.DataSource = mList;
                    grdMyJobs.DataKeyNames = new string[] { "customer_id" };
                    grdMyJobs.DataBind();
                }
            }
           

        }
        catch( Exception ex)
        {
        }
    }
    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("mlandingpage.aspx");
    }
    protected void grdMyJobs_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int ncid = Convert.ToInt32(grdMyJobs.DataKeys[e.Row.RowIndex].Values[0].ToString());
                DataClassesDataContext _db = new DataClassesDataContext();
                customer cust = new customer();
                LinkButton InkLastName = (LinkButton)e.Row.FindControl("InkLastName");

                cust = _db.customers.Single(c => c.customer_id == ncid);
                InkLastName.Text = cust.last_name1;
                string strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;

                HyperLink hypAddress = (HyperLink)e.Row.FindControl("hypAddress");
                hypAddress.Text = strAddress;

                string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                hypAddress.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

            }
        }
        catch(Exception ex)
        {
        }
        
    }
    protected void grdMyJobs_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "LastName")
            {
                
                int index = Convert.ToInt32(e.CommandArgument);
                string nCId = grdMyJobs.DataKeys[index].Values[0].ToString();
                insertCustomer(nCId);
                
           }
        }
        catch (Exception ex)
        {
            
        }

    }

    private void insertCustomer(string nCId)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = string.Empty;


            if (Session["oUser"] != null)
            {
                userinfo obj = (userinfo)Session["oUser"];


                customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(nCId));

                strQ = "delete from searchcustomers WHERE customer_id =" + nCId + " AND userId=" + obj.user_id + " AND IsCrew=0 ";
                _db.ExecuteCommand(strQ, string.Empty);

                strQ = "insert into searchcustomers " +
                             " SELECT  [client_id],[customer_id],[first_name1],[last_name1] ,[first_name2],[last_name2],[address] ,[cross_street], " +
                             " [city] ,[state] ,[zip_code] ,[fax] ,[email] ,[phone] ,[is_active],[registration_date],[sales_person_id],[update_date], " +
                             " [status_id],[notes] ,[appointment_date],[lead_source_id] ,[status_note],[company] ,[email2],[SuperintendentId], " +
                              " [mobile],[lead_status_id],[islead],[isCustomer],[website],[isCalendarOnline], " + obj.user_id + ",getdate(),0 " +
                               " FROM customers " +
                               " WHERE customer_id ='" + nCId + "'";

                _db.ExecuteCommand(strQ, string.Empty);

            }

            if (Session["oCrew"] != null)
            {
                Crew_Detail objC = (Crew_Detail)Session["oCrew"];


                customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(nCId));

                strQ = "delete from searchcustomers WHERE customer_id =" + nCId + " AND userId=" + objC.crew_id + " AND IsCrew=1 ";
                _db.ExecuteCommand(strQ, string.Empty);

                strQ = "insert into searchcustomers " +
                             " SELECT  [client_id],[customer_id],[first_name1],[last_name1] ,[first_name2],[last_name2],[address] ,[cross_street], " +
                             " [city] ,[state] ,[zip_code] ,[fax] ,[email] ,[phone] ,[is_active],[registration_date],[sales_person_id],[update_date], " +
                             " [status_id],[notes] ,[appointment_date],[lead_source_id] ,[status_note],[company] ,[email2],[SuperintendentId], " +
                              " [mobile],[lead_status_id],[islead],[isCustomer],[website],[isCalendarOnline], " + objC.crew_id + ",getdate(),1 " +
                               " FROM customers " +
                               " WHERE customer_id ='" + nCId + "'";

                _db.ExecuteCommand(strQ, string.Empty);
            }

            Response.Redirect("mlandingpage.aspx");
        }
        catch (Exception ex)
        {
        }
    }


    protected void grdFuture_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int ncid = Convert.ToInt32(grdFuture.DataKeys[e.Row.RowIndex].Values[0].ToString());
                DataClassesDataContext _db = new DataClassesDataContext();
                customer cust = new customer();
                LinkButton InkLastName = (LinkButton)e.Row.FindControl("InkLastName");

                cust = _db.customers.Single(c => c.customer_id == ncid);
                InkLastName.Text = cust.last_name1;
                string strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;

                HyperLink hypAddress = (HyperLink)e.Row.FindControl("hypAddress");
                hypAddress.Text = strAddress;

                string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                hypAddress.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

            }
        }
        catch (Exception ex)
        {
        }

    }

    protected void grdFuture_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "LastName")
            {

                int index = Convert.ToInt32(e.CommandArgument);
                string nCId = grdFuture.DataKeys[index].Values[0].ToString();
                insertCustomer(nCId);

            }
        }
        catch (Exception ex)
        {

        }

    }

    protected void grdPast_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int ncid = Convert.ToInt32(grdPast.DataKeys[e.Row.RowIndex].Values[0].ToString());
                DataClassesDataContext _db = new DataClassesDataContext();
                customer cust = new customer();
                LinkButton InkLastName = (LinkButton)e.Row.FindControl("InkLastName");

                cust = _db.customers.Single(c => c.customer_id == ncid);
                InkLastName.Text = cust.last_name1;
                string strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;

                HyperLink hypAddress = (HyperLink)e.Row.FindControl("hypAddress");
                hypAddress.Text = strAddress;

                string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                hypAddress.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

            }
        }
        catch (Exception ex)
        {
        }

    }

    protected void grdPast_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "LastName")
            {

                int index = Convert.ToInt32(e.CommandArgument);
                string nCId = grdPast.DataKeys[index].Values[0].ToString();
                insertCustomer(nCId);

            }
        }
        catch (Exception ex)
        {

        }

    }

}