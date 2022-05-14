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

public partial class schedulecalendarreadonly : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);

            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            //if (Page.User.IsInRole("Call003") == false)
            //{
            //    // No Permission Page.
            //    Response.Redirect("nopermission.aspx");
            //}



            //Clear Search
            HttpContext.Current.Session.Add("rsCusId", 0);
            HttpContext.Current.Session.Add("rsSecName", "");
            HttpContext.Current.Session.Add("rsKeySearchUserName", "");

            userinfo objUName = (userinfo)Session["oUser"];

            int nTypeId = 0;

            DataClassesDataContext _db = new DataClassesDataContext();



            nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeID"));
            HttpContext.Current.Session.Add("rsTypeID", nTypeId);
            if (nTypeId == 1)
            {
                lbltopHead.Text = "Operation Calendar (Read-only)";
                txtSearch.Visible = true;
                txtSection.Visible = true;
                btnSearch.Visible = true;
                lnkViewAll.Visible = true;


               

            }
        }
    }
    //this method only updates title and description //this is called when a event is clicked on the calendar
    [System.Web.Services.WebMethod]
    public static csProjectLink GetEvent(int nCustId, string SectionName, string UserName)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        HttpContext.Current.Session.Add("rsCusId", nCustId);
        HttpContext.Current.Session.Add("rsSecName", SectionName);
        HttpContext.Current.Session.Add("rsKeySearchUserName", UserName);

        int nEstimateID = 0;
        string date = "";

        if (nCustId != 0 && SectionName != "" && UserName != "")
        {
            var objSC = _db.ScheduleCalendars.Where(c => c.customer_id == nCustId && c.section_name == SectionName && c.employee_name.Contains(UserName));
            if (objSC.Any())
            {
                nEstimateID = Convert.ToInt32(objSC.Max(x => x.estimate_id));
                var tempDate = objSC.Where(c => c.event_start >= DateTime.Today).Min(x => x.event_start);
                if (tempDate != null)
                    date = ConvertToTimestamp((DateTime)tempDate);
            }

            HttpContext.Current.Session.Add("rsEstSelectedByCustSearch", nEstimateID);

            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = nCustId;
            objPL.estimate_id = nEstimateID;
            objPL.date = date;

            return objPL;
        }
        else if (nCustId != 0 && UserName != "")
        {
            var objSC = _db.ScheduleCalendars.Where(c => c.customer_id == nCustId && c.employee_name.Contains(UserName));
            if (objSC.Any())
            {
                nEstimateID = Convert.ToInt32(objSC.Max(x => x.estimate_id));
                var tempDate = objSC.Where(c => c.event_start >= DateTime.Today).Min(x => x.event_start);
                if (tempDate != null)
                    date = ConvertToTimestamp((DateTime)tempDate);
            }

            HttpContext.Current.Session.Add("rsEstSelectedByCustSearch", nEstimateID);

            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = nCustId;
            objPL.estimate_id = nEstimateID;
            objPL.date = date;

            return objPL;
        }
        else if (nCustId != 0 && SectionName != "")
        {
            var objSC = _db.ScheduleCalendars.Where(c => c.customer_id == nCustId && c.section_name == SectionName);
            if (objSC.Any())
            {
                nEstimateID = Convert.ToInt32(objSC.Max(x => x.estimate_id));
                var tempDate = objSC.Where(c => c.event_start >= DateTime.Today).Min(x => x.event_start);
                if (tempDate != null)
                    date = ConvertToTimestamp((DateTime)tempDate);
            }

            HttpContext.Current.Session.Add("rsEstSelectedByCustSearch", nEstimateID);

            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = nCustId;
            objPL.estimate_id = nEstimateID;
            objPL.date = date;

            return objPL;
        }
        else if (SectionName != "" && UserName != "")
        {
            var objSC = _db.ScheduleCalendars.Where(c => c.section_name == SectionName && c.employee_name.Contains(UserName));
            if (objSC.Any())
            {
                nEstimateID = Convert.ToInt32(objSC.Max(x => x.estimate_id));
                var tempDate = objSC.Where(c => c.event_start >= DateTime.Today).Min(x => x.event_start);
                if (tempDate != null)
                    date = ConvertToTimestamp((DateTime)tempDate);
            }

            HttpContext.Current.Session.Add("rsEstSelectedByCustSearch", nEstimateID);

            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = nCustId;
            objPL.estimate_id = nEstimateID;
            objPL.date = date;

            return objPL;
        }
        else if (nCustId != 0)
        {
            var objSC = _db.ScheduleCalendars.Where(c => c.customer_id == nCustId);
            if (objSC.Any())
            {
                nEstimateID = Convert.ToInt32(objSC.Max(x => x.estimate_id));
                var tempDate = objSC.Where(c => c.event_start >= DateTime.Today).Min(x => x.event_start);
                if (tempDate != null)
                    date = ConvertToTimestamp((DateTime)tempDate);
            }



            HttpContext.Current.Session.Add("rsEstSelectedByCustSearch", nEstimateID);

            csProjectLink objPL = new csProjectLink();

            objPL.customer_id = nCustId;
            objPL.estimate_id = nEstimateID;
            objPL.date = date;

            return objPL;
        }
        else if (SectionName != "")
        {
            var dtDate = _db.ScheduleCalendars.Where(c => c.section_name == SectionName && c.event_start >= DateTime.Now).Min(x => x.event_start);

            HttpContext.Current.Session.Add("rsEstSelectedByCustSearch", null);

            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = nCustId;
            objPL.estimate_id = 0;
            if (dtDate != null)
            {
                objPL.date = ConvertToTimestamp((DateTime)dtDate);
            }
            else
            {
                objPL.date = "";
            }

            return objPL;
        }
        else if (UserName != "")
        {
            var dtDate = _db.ScheduleCalendars.Where(c => c.employee_name.Contains(UserName) && c.event_start >= DateTime.Now).Min(x => x.event_start);

            HttpContext.Current.Session.Add("rsEstSelectedByCustSearch", null);

            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = nCustId;
            objPL.estimate_id = 0;
            if (dtDate != null)
            {
                objPL.date = ConvertToTimestamp((DateTime)dtDate);
            }
            else
            {
                objPL.date = "";
            }

            return objPL;
        }
        else
        {
            HttpContext.Current.Session.Add("rsEstSelectedByCustSearch", null);
            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = 0;
            objPL.estimate_id = 0;
            objPL.date = "";

            return objPL;
        }
    }

    

    [System.Web.Services.WebMethod]
    public static List<csCustomer> GetCustomer(string keyword)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = (from c in _db.customers
                    join sc in _db.ScheduleCalendars on c.customer_id equals sc.customer_id
                    where c.last_name1.ToUpper().StartsWith(keyword.Trim().ToUpper()) && sc.estimate_id != 0
                    select new csCustomer
                    {
                        customer_id = c.customer_id,
                        customer_name = c.first_name1.Trim() + " " + c.last_name1.Trim()
                    }).Distinct().ToList();
        return item.ToList();
    }

    [System.Web.Services.WebMethod]
    public static string GetEmployeeById(int empId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = (from sp in _db.sales_persons
                    where sp.sales_person_id == empId
                    select sp).SingleOrDefault();

        return item.first_name.ToString() + " " + item.last_name.ToString();
    }

    [System.Web.Services.WebMethod]
    public static List<SectionInfo> GetSection(string keyword)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        //var item = from c in _db.ScheduleCalendars
        //           where c.section_name.ToUpper().StartsWith(keyword.Trim().ToUpper()) && c.customer_id == keyword2
        //           select new SectionInfo
        //           {
        //               section_name = c.section_name.Trim()
        //           };

        var item = from c in _db.sectioninfos
                   where c.section_name.ToUpper().StartsWith(keyword.Trim().ToUpper()) && c.parent_id == 0
                   orderby c.section_name
                   select new SectionInfo
                   {
                       section_name = c.section_name.Trim()
                   };

        return item.ToList();
    }

    [System.Web.Services.WebMethod]
    public static List<userinfo> GetUserName(string keyword)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        //string sSql = "Select LOWER(first_name+' '+last_name) as sales_person_name from Crew_Details where LOWER(first_name+' '+last_name) in (select LOWER(first_name+' '+ last_name) from user_info)";

        //List<string> listDupliEmps = _db.ExecuteQuery<string>(sSql).ToList();

        //var listEmps = (from sc in _db.ScheduleCalendars
        //                select new
        //                {
        //                    employee_name = sc.employee_name
        //                });
        //string strEmp = "";

        //foreach (var e in listEmps)
        //{
        //    strEmp += "," + e.employee_name.Trim();
        //}

        //string[] aryEmps = strEmp.Split(',');
        //List<userinfo> list = new List<userinfo>();
        //for (int i = 0; i < aryEmps.Length; i++)
        //{
        //    userinfo obj = new userinfo();
        //    if (aryEmps[i].Trim().Length > 0)
        //    {
        //        obj.sales_person_name = aryEmps[i].Trim();

        //        list.Add(obj);
        //    }
        //}

        //var item = (from u in list
        //            where u.sales_person_name.ToUpper().Contains(keyword.Trim().ToUpper())
        //            select new userinfo
        //               {
        //                   sales_person_name = u.sales_person_name
        //               });

        //if (item.ToList().Count() == 0 && keyword.ToLower().Contains("*"))
        //{
        //    item = (from u in list

        //            select new userinfo
        //            {
        //                sales_person_name = u.sales_person_name
        //            });
        //}

        //return list.Distinct().Take(10).OrderBy(f => f.sales_person_name).ToList();

        var item = (from sp in _db.sales_persons
                    where (sp.first_name.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) || sp.last_name.Trim().ToUpper().Contains(keyword.Trim().ToUpper())) && sp.is_active == true
                    orderby sp.first_name
                    select new userinfo
                    {
                        first_name = sp.first_name.Trim(),
                        last_name = sp.last_name.Trim(),
                        sales_person_id = sp.sales_person_id,
                        sales_person_name = sp.first_name.Trim() + " " + sp.last_name.Trim()
                    }).Union(
                   from cr in _db.Crew_Details
                   where (cr.first_name.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) || cr.last_name.Trim().ToUpper().Contains(keyword.Trim().ToUpper())) && cr.is_active == true
                   orderby cr.first_name
                   select new userinfo
                   {
                       first_name = cr.first_name.Trim(),
                       last_name = cr.last_name.Trim(),
                       sales_person_id = cr.crew_id,
                       sales_person_name = cr.first_name.Trim() + " " + cr.last_name.Trim()
                   });

        if (item.ToList().Count() == 0 && keyword.ToLower().Contains("*"))
        {
            item = (from sp in _db.sales_persons
                    where sp.is_active == true
                    orderby sp.first_name
                    select new userinfo
                    {
                        first_name = sp.first_name.Trim(),
                        last_name = sp.last_name.Trim(),
                        sales_person_id = sp.sales_person_id,
                        sales_person_name = sp.first_name.Trim() + " " + sp.last_name.Trim()
                    }).Union(
                   from cr in _db.Crew_Details
                   where cr.is_active == true
                   orderby cr.first_name
                   select new userinfo
                   {
                       first_name = cr.first_name.Trim(),
                       last_name = cr.last_name.Trim(),
                       sales_person_id = cr.crew_id,
                       sales_person_name = cr.first_name.Trim() + " " + cr.last_name.Trim()
                   });
        }

        return item.Distinct().OrderBy(f => f.first_name).ToList();

        //return item.Where(e => !listDupliEmps.Contains(e.first_name.Trim().ToLower() + " " + e.last_name.Trim().ToLower())).ToList().Distinct().OrderBy(f => f.first_name).ToList();
    }

    [System.Web.Services.WebMethod]
    public static List<userinfo> GetSalesPerson(string keyword)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        string sSql = "Select LOWER(first_name+' '+last_name) as sales_person_name from Crew_Details where LOWER(first_name+' '+last_name) in (select LOWER(first_name+' '+ last_name) from user_info)";

        List<string> listDupliEmps = _db.ExecuteQuery<string>(sSql).ToList();

        var item = (from sp in _db.sales_persons
                    where sp.first_name.ToUpper().StartsWith(keyword.Trim().ToUpper()) && sp.is_active == true
                    orderby sp.first_name
                    select new userinfo
                    {
                        first_name = sp.first_name.Trim(),
                        last_name = sp.last_name.Trim(),
                        sales_person_id = sp.sales_person_id,
                        sales_person_name = sp.first_name.Trim() + " " + sp.last_name.Trim()
                    }).Union(
                   from cr in _db.Crew_Details
                   where cr.first_name.ToUpper().StartsWith(keyword.Trim().ToUpper()) && cr.is_active == true
                   orderby cr.first_name
                   select new userinfo
                   {
                       first_name = cr.first_name.Trim(),
                       last_name = cr.last_name.Trim(),
                       sales_person_id = cr.crew_id,
                       sales_person_name = cr.first_name.Trim() + " " + cr.last_name.Trim()
                   });

        if (item.ToList().Count() == 0 && keyword.ToLower().Contains("*"))
        {
            item = (from sp in _db.sales_persons
                    where sp.is_active == true
                    orderby sp.first_name
                    select new userinfo
                    {
                        first_name = sp.first_name.Trim(),
                        last_name = sp.last_name.Trim(),
                        sales_person_id = sp.sales_person_id,
                        sales_person_name = sp.first_name.Trim() + " " + sp.last_name.Trim()
                    }).Union(
                   from cr in _db.Crew_Details
                   where cr.is_active == true
                   orderby cr.first_name
                   select new userinfo
                   {
                       first_name = cr.first_name.Trim(),
                       last_name = cr.last_name.Trim(),
                       sales_person_id = cr.crew_id,
                       sales_person_name = cr.first_name.Trim() + " " + cr.last_name.Trim()
                   });
        }

        return item.Distinct().OrderBy(f => f.first_name).ToList();

        //return item.Where(e => !listDupliEmps.Contains(e.first_name.Trim().ToLower() + " " + e.last_name.Trim().ToLower())).ToList().Distinct().OrderBy(f => f.first_name).ToList();
    }



    [System.Web.Services.WebMethod]
    public static List<ImproperCalendarLinkEvent> GetChildEventTable(int parentEventId, int ncid, int neid)
    {
        List<ImproperCalendarLinkEvent> objlnkEvent = new List<ImproperCalendarLinkEvent>();
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            bool IsCalendarOnline = true;
            var objCust = _db.customers.Where(c => c.customer_id == ncid);

            if (objCust != null)
            {
                IsCalendarOnline = (bool)objCust.SingleOrDefault().isCalendarOnline;
            }

            if (IsCalendarOnline)
            {


                var results = (from sc in _db.ScheduleCalendars
                               join link in _db.ScheduleCalendarLinks on sc.event_id equals link.child_event_id
                               where link.customer_id == ncid && link.estimate_id == neid && link.parent_event_id == parentEventId
                               orderby sc.event_start
                               select new ImproperCalendarLinkEvent
                               {
                                   link_id = (int)link.link_id,
                                   customer_id = (int)link.customer_id,
                                   estimate_id = (int)link.estimate_id,
                                   title = sc.title,
                                   start = sc.event_start.ToString(),
                                   end = sc.event_end.ToString(),
                                   parent_event_id = (int)link.parent_event_id,
                                   child_event_id = (int)link.child_event_id,
                                   dependency = (int)link.dependencyType == 1 ? "Start Same Time" : (int)link.dependencyType == 2 ? "Start After Finish" : "Offset days",
                                   lag = (int)link.dependencyType == 3 ? "(" + (int)link.lag + ")" : "",
                                   IsSuccess = "Ok"
                               });
                var newresults = from item in results.ToList()
                                 select new ImproperCalendarLinkEvent
                                 {
                                     link_id = item.link_id,
                                     customer_id = item.customer_id,
                                     estimate_id = item.estimate_id,
                                     title = item.title,
                                     start = Convert.ToDateTime(item.start).ToString("MM/dd/yyyy hh:mm tt"),
                                     end = Convert.ToDateTime(item.end).ToString("MM/dd/yyyy hh:mm tt"),
                                     parent_event_id = item.parent_event_id,
                                     child_event_id = item.child_event_id,
                                     dependency = item.dependency,
                                     lag = item.lag,
                                     IsSuccess = item.IsSuccess
                                 };
                objlnkEvent = newresults.ToList();
            }
            else
            {
                var results = (from scTemp in _db.ScheduleCalendarTemps
                               join linktemp in _db.ScheduleCalendarLinkTemps on scTemp.event_id equals linktemp.child_event_id
                               where linktemp.customer_id == ncid && linktemp.estimate_id == neid && linktemp.parent_event_id == parentEventId
                               orderby scTemp.event_start
                               select new ImproperCalendarLinkEvent
                               {
                                   link_id = (int)linktemp.link_id,
                                   customer_id = (int)linktemp.customer_id,
                                   estimate_id = (int)linktemp.estimate_id,
                                   title = scTemp.title,
                                   start = scTemp.event_start.ToString(),
                                   end = scTemp.event_end.ToString(),
                                   parent_event_id = (int)linktemp.parent_event_id,
                                   child_event_id = (int)linktemp.child_event_id,
                                   dependency = (int)linktemp.dependencyType == 1 ? "Start Same Time" : (int)linktemp.dependencyType == 2 ? "Start After Finish" : "Offset days",
                                   lag = (int)linktemp.dependencyType == 3 ? "(" + (int)linktemp.lag + ")" : "",
                                   IsSuccess = "Ok"
                               });
                var newresults = from item in results.ToList()
                                 select new ImproperCalendarLinkEvent
                                 {
                                     link_id = item.link_id,
                                     customer_id = item.customer_id,
                                     estimate_id = item.estimate_id,
                                     title = item.title,
                                     start = Convert.ToDateTime(item.start).ToString("MM/dd/yyyy hh:mm tt"),
                                     end = Convert.ToDateTime(item.end).ToString("MM/dd/yyyy hh:mm tt"),
                                     parent_event_id = item.parent_event_id,
                                     child_event_id = item.child_event_id,
                                     dependency = item.dependency,
                                     lag = item.lag,
                                     IsSuccess = item.IsSuccess
                                 };
                objlnkEvent = newresults.ToList();
            }

        }
        catch (Exception ex)
        {
            //  objlnkEvent.IsSuccess = ex.Message;
        }

        return objlnkEvent;
    }

    [System.Web.Services.WebMethod]
    public static List<ImproperCalendarLinkEvent> GetParentEventTable(int childEventId, int ncid, int neid)
    {
        List<ImproperCalendarLinkEvent> objlnkEvent = new List<ImproperCalendarLinkEvent>();
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            bool IsCalendarOnline = true;
            var objCust = _db.customers.Where(c => c.customer_id == ncid);

            if (objCust != null)
            {
                IsCalendarOnline = (bool)objCust.SingleOrDefault().isCalendarOnline;
            }

            if (IsCalendarOnline)
            {
                var results = (from sc in _db.ScheduleCalendars
                               join link in _db.ScheduleCalendarLinks on sc.event_id equals link.parent_event_id
                               where link.customer_id == ncid && link.estimate_id == neid && link.child_event_id == childEventId
                               orderby sc.event_start
                               select new ImproperCalendarLinkEvent
                               {
                                   link_id = (int)link.link_id,
                                   customer_id = (int)link.customer_id,
                                   estimate_id = (int)link.estimate_id,
                                   title = sc.title,
                                   start = sc.event_start.ToString(),
                                   end = sc.event_end.ToString(),
                                   parent_event_id = (int)link.parent_event_id,
                                   child_event_id = (int)link.child_event_id,
                                   dependency = (int)link.dependencyType == 1 ? "Start Same Time" : (int)link.dependencyType == 2 ? "Start After Finish" : "Offset days",
                                   lag = (int)link.dependencyType == 3 ? "(" + (int)link.lag + ")" : "",
                                   IsSuccess = "Ok"
                               });
                var newresults = from item in results.ToList()
                                 select new ImproperCalendarLinkEvent
                                 {
                                     link_id = item.link_id,
                                     customer_id = item.customer_id,
                                     estimate_id = item.estimate_id,
                                     title = item.title,
                                     start = Convert.ToDateTime(item.start).ToString("MM/dd/yyyy hh:mm tt"),
                                     end = Convert.ToDateTime(item.end).ToString("MM/dd/yyyy hh:mm tt"),
                                     parent_event_id = item.parent_event_id,
                                     child_event_id = item.child_event_id,
                                     dependency = item.dependency,
                                     lag = item.lag,
                                     IsSuccess = item.IsSuccess
                                 };
                objlnkEvent = newresults.ToList();
            }
            else
            {
                var results = (from scTemp in _db.ScheduleCalendarTemps
                               join linktemp in _db.ScheduleCalendarLinkTemps on scTemp.event_id equals linktemp.parent_event_id
                               where linktemp.customer_id == ncid && linktemp.estimate_id == neid && linktemp.child_event_id == childEventId
                               orderby scTemp.event_start
                               select new ImproperCalendarLinkEvent
                               {
                                   link_id = (int)linktemp.link_id,
                                   customer_id = (int)linktemp.customer_id,
                                   estimate_id = (int)linktemp.estimate_id,
                                   title = scTemp.title,
                                   start = scTemp.event_start.ToString(),
                                   end = scTemp.event_end.ToString(),
                                   parent_event_id = (int)linktemp.parent_event_id,
                                   child_event_id = (int)linktemp.child_event_id,
                                   dependency = (int)linktemp.dependencyType == 1 ? "Start Same Time" : (int)linktemp.dependencyType == 2 ? "Start After Finish" : "Offset days",
                                   lag = (int)linktemp.dependencyType == 3 ? "(" + (int)linktemp.lag + ")" : "",
                                   IsSuccess = "Ok"
                               });
                var newresults = from item in results.ToList()
                                 select new ImproperCalendarLinkEvent
                                 {
                                     link_id = item.link_id,
                                     customer_id = item.customer_id,
                                     estimate_id = item.estimate_id,
                                     title = item.title,
                                     start = Convert.ToDateTime(item.start).ToString("MM/dd/yyyy hh:mm tt"),
                                     end = Convert.ToDateTime(item.end).ToString("MM/dd/yyyy hh:mm tt"),
                                     parent_event_id = item.parent_event_id,
                                     child_event_id = item.child_event_id,
                                     dependency = item.dependency,
                                     lag = item.lag,
                                     IsSuccess = item.IsSuccess
                                 };
                objlnkEvent = newresults.ToList();
            }

        }
        catch (Exception ex)
        {
            //  objlnkEvent.IsSuccess = ex.Message;
        }

        return objlnkEvent;
    }

    public class scEventLink
    {
        public int link_id { get; set; }
        public int dependencyType { get; set; }
        public DateTime event_start { get; set; }
        public DateTime event_end { get; set; }
        public int offsetdays { get; set; }
        public int child_event_id { get; set; }
        public int parent_event_id { get; set; }
        public int customer_id { get; set; }
        public int estimate_id { get; set; }
    }

    public class csProjectLink
    {
        public string date { get; set; }
        public int customer_id { get; set; }
        public int estimate_id { get; set; }
    }

    public static string ConvertToTimestamp(DateTime value)
    {
        // var text = value.ToString("'\"'yyyy-MM-dd'T'HH:mm:ss'\"'", System.Globalization.CultureInfo.InvariantCulture); // 2014-11-10T10:00:00
        var text = value.ToString("yyyy-MM-dd'T'HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture); // 2014-11-10T10:00:00
        long epoch = (value.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        return text;
    }

























}
