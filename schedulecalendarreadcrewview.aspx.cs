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

public partial class schedulecalendarreadcrewview : System.Web.UI.Page
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
            HttpContext.Current.Session.Add("crsCusId", 0);
            HttpContext.Current.Session.Add("crsSecName", "");
            HttpContext.Current.Session.Add("crsKeySearchUserName", "");
            HttpContext.Current.Session.Add("crsKeySearchSuperintendentName", "");


            userinfo objUName = (userinfo)Session["oUser"];

            int nTypeId = 0;

            DataClassesDataContext _db = new DataClassesDataContext();



            nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeID"));
            HttpContext.Current.Session.Add("crsTypeID", nTypeId);
            if (nTypeId == 1)
            {
                lbltopHead.Text = "Resource Calendar";
                txtSearch.Visible = true;
                txtSection.Visible = true;
                btnSearch.Visible = true;
                lnkViewAll.Visible = true;

                BindSuperintendent();
               

            }
        }
    }

    public void BindSuperintendent()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = from e in _db.user_infos
                   join c in _db.customers on e.user_id equals c.SuperintendentId
                   where e.is_active == true 
                   orderby e.first_name
                   select new
                   {
                       user_id = e.user_id,
                       cssClassName = e.cssClassName,
                       employee_name = e.first_name + ' ' + e.last_name
                   };

        grdSuperintendent.DataSource = item.Distinct().OrderBy(x=>x.employee_name).ToList();
        grdSuperintendent.DataKeyNames = new string[] { "user_id", "cssClassName", "employee_name" };
        grdSuperintendent.DataBind();
    }

    //this method only updates title and description //this is called when a event is clicked on the calendar
    [System.Web.Services.WebMethod]
    public static csProjectLink GetEvent(int nCustId, string SectionName, string UserName, string SuperintendentName)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        HttpContext.Current.Session.Add("crsCusId", nCustId);
        HttpContext.Current.Session.Add("crsSecName", SectionName);
        HttpContext.Current.Session.Add("crsKeySearchUserName", UserName);
        HttpContext.Current.Session.Add("crsKeySearchSuperintendentName", SuperintendentName);

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

            HttpContext.Current.Session.Add("crsEstSelectedByCustSearch", nEstimateID);

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

            HttpContext.Current.Session.Add("crsEstSelectedByCustSearch", nEstimateID);

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

            HttpContext.Current.Session.Add("crsEstSelectedByCustSearch", nEstimateID);

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

            HttpContext.Current.Session.Add("crsEstSelectedByCustSearch", nEstimateID);

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



            HttpContext.Current.Session.Add("crsEstSelectedByCustSearch", nEstimateID);

            csProjectLink objPL = new csProjectLink();

            objPL.customer_id = nCustId;
            objPL.estimate_id = nEstimateID;
            objPL.date = date;

            return objPL;
        }
        else if (SectionName != "")
        {
            var dtDate = _db.ScheduleCalendars.Where(c => c.section_name == SectionName && c.event_start >= DateTime.Now).Min(x => x.event_start);

            HttpContext.Current.Session.Add("crsEstSelectedByCustSearch", null);

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

            HttpContext.Current.Session.Add("crsEstSelectedByCustSearch", null);

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
        else if (SuperintendentName != "")
        {
            var dtDate = (
                        from sc in _db.ScheduleCalendars
                        join c in _db.customers on sc.customer_id equals c.customer_id
                        join u in _db.user_infos on c.SuperintendentId equals u.user_id
                        where (u.first_name.Trim().ToLower() + ' ' + u.first_name.Trim().ToLower()).Contains(SuperintendentName.Trim().ToLower())
                        && sc.event_start >= DateTime.Now
                        select sc.event_start).Min();

                
                //_db.ScheduleCalendars.Where(c => c.employee_name.Contains(UserName) && c.event_start >= DateTime.Now).Min(x => x.event_start);

            HttpContext.Current.Session.Add("crsEstSelectedByCustSearch", null);

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
            HttpContext.Current.Session.Add("crsEstSelectedByCustSearch", null);
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



        var item = from cr in _db.Crew_Details
                   where (cr.first_name.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) || cr.last_name.Trim().ToUpper().Contains(keyword.Trim().ToUpper())) && cr.is_active == true
                   orderby cr.first_name
                   select new userinfo
                   {
                       first_name = cr.first_name.Trim(),
                       last_name = cr.last_name.Trim(),
                       sales_person_id = cr.crew_id,
                       sales_person_name = cr.first_name.Trim() + " " + cr.last_name.Trim()
                   };
          

        if (item.ToList().Count() == 0 && keyword.ToLower().Contains("*"))
        {
            item =  from cr in _db.Crew_Details
                   where cr.is_active == true
                   orderby cr.first_name
                   select new userinfo
                   {
                       first_name = cr.first_name.Trim(),
                       last_name = cr.last_name.Trim(),
                       sales_person_id = cr.crew_id,
                       sales_person_name = cr.first_name.Trim() + " " + cr.last_name.Trim()
                   };
              
        }

        return item.Distinct().OrderBy(f => f.first_name).ToList();

        //return item.Where(e => !listDupliEmps.Contains(e.first_name.Trim().ToLower() + " " + e.last_name.Trim().ToLower())).ToList().Distinct().OrderBy(f => f.first_name).ToList();
    }

    [System.Web.Services.WebMethod]
    public static List<userinfo> GetSuperintendentName(string keyword)
    {
        DataClassesDataContext _db = new DataClassesDataContext();



        var item = from u in _db.user_infos
                   join c in _db.customers on u.user_id equals c.SuperintendentId
                   where (u.first_name.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) || u.last_name.Trim().ToUpper().Contains(keyword.Trim().ToUpper())) && u.is_active == true 
                   orderby u.first_name
                   select new userinfo
                   {
                       first_name = u.first_name.Trim(),
                       last_name = u.last_name.Trim(),
                       sales_person_id = u.user_id,
                       sales_person_name = u.first_name.Trim() + " " + u.last_name.Trim()
                   };
       

        if (item.ToList().Count() == 0 && keyword.ToLower().Contains("*"))
        {
            item = from u in _db.user_infos
                   join c in _db.customers on u.user_id equals c.SuperintendentId
                   where u.is_active == true 
                   orderby u.first_name
                   select new userinfo
                   {
                       first_name = u.first_name.Trim(),
                       last_name = u.last_name.Trim(),
                       sales_person_id = u.user_id,
                       sales_person_name = u.first_name.Trim() + " " + u.last_name.Trim()
                   };
           
        }

        return item.Distinct().OrderBy(f => f.first_name).ToList();

       
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
