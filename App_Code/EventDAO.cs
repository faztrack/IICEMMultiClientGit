using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Globalization;

/// <summary>
/// EventDAO class is the main class which interacts with the database. SQL Server express edition
/// has been used.
/// the event information is stored in a table named 'event' in the database.
///
/// Here is the table format:
/// event(event_id int, title varchar(100), description varchar(200),event_start datetime, event_end datetime)
/// event_id is the primary key
/// </summary>
public class EventDAO
{

    //change the connection string as per your database connection.
    //private static string connectionString = "Data Source=192.168.0.10;Initial Catalog=EventCalender;User ID=sa";

    //this method retrieves all events within range start-end
    public static List<CalendarEvent> getEvents(DateTime start, DateTime end, int nCusId, string strSecName, string strSearchUserName, string strSearchSuperintendentName)
    {

        TimeSpan diff = end - start;


        if (diff.Days < 8)
        {
            DateTime dt = new DateTime(start.Year, start.Month, 1);

            int ndiff = dt.DayOfWeek - DayOfWeek.Sunday;

            DateTime dtNewStart = dt.AddDays(-ndiff);

            DateTime dtNewEnd = dtNewStart.AddDays(42);

            start = dtNewStart;

            end = dtNewEnd;
        }



        int nTypeId = 0;
        int nOprationExtTypeID = 0;

        int ncid = 0;
        int neid = 0;

        if (System.Web.HttpContext.Current.Session["TypeID"] != null)
        {
            nTypeId = (int)System.Web.HttpContext.Current.Session["TypeID"];
        }
        if (System.Web.HttpContext.Current.Session["cid"] != null)
        {
            ncid = (int)System.Web.HttpContext.Current.Session["cid"];
        }
        if (System.Web.HttpContext.Current.Session["eid"] != null)
        {
            neid = (int)System.Web.HttpContext.Current.Session["eid"];
        }
        IEnumerable<int> listCheckedEstId = new int[] { };

        if (System.Web.HttpContext.Current.Session["sSelectedEstIdList"] != null)
        {
            listCheckedEstId = System.Web.HttpContext.Current.Session["sSelectedEstIdList"] as IEnumerable<int>;
        }

        bool IsCalendarOnline = true;

        if (System.Web.HttpContext.Current.Session["sIsCalendarOnline"] != null)
            IsCalendarOnline = (bool)System.Web.HttpContext.Current.Session["sIsCalendarOnline"];

        //Opration/Sales Extra Type ID
        if (nTypeId == 1)
            nOprationExtTypeID = 11;
        if (nTypeId == 2)
            nOprationExtTypeID = 22;

        var typelist = new int[] { 0, 5, nTypeId, nOprationExtTypeID };
        // var typelist = new int[] { nTypeId };
        DataClassesDataContext _db = new DataClassesDataContext();
        List<CalendarEvent> events = new List<CalendarEvent>();

        IQueryable<CalendarEvent> item = Enumerable.Empty<CalendarEvent>().AsQueryable();

        //IQueryable<CalendarEvent> item = null;

        if (ncid != 0 && IsCalendarOnline && (nTypeId == 1 || nTypeId == 11))
        {
            item = (from sc in _db.ScheduleCalendars
                    where typelist.Contains((int)sc.type_id) && sc.IsEstimateActive == true && sc.customer_id == ncid && listCheckedEstId.Contains((int)sc.estimate_id) && sc.type_id != 5
                    && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                    select new CalendarEvent()
                    {
                        id = (int)sc.event_id,
                        //title = HttpUtility.HtmlDecode(sc.title),
                        title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " ") + (sc.employee_name == "" ? "" : sc.employee_name.Contains("TBD") ? "" : " (" + sc.employee_name + ") "), @"\t|\n|\r", " ")),
                        description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        start = (DateTime)sc.event_start,
                        end = (DateTime)sc.event_end,
                        customer_id = (int)sc.customer_id,
                        estimate_id = (int)sc.estimate_id,
                        employee_id = (int)sc.employee_id,
                        section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        cssClassName = sc.cssClassName,
                        type_id = (int)sc.type_id,
                        operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        employee_name = sc.employee_name,
                        customer_last_name = "",
                        IsScheduleDayException = sc.IsScheduleDayException != null ? (bool)sc.IsScheduleDayException : false,
                        is_complete = sc.is_complete != null ? (bool)sc.is_complete : false,
                        has_parent_link = (_db.ScheduleCalendarLinks.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.parent_event_id == sc.event_id)) == true ? true : false,
                        has_child_link = (_db.ScheduleCalendarLinks.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.child_event_id == sc.event_id)) == true ? true : false,
                        estimate_name = (_db.customer_estimates.SingleOrDefault(ce => ce.customer_id == sc.customer_id && ce.estimate_id == sc.estimate_id).estimate_name),
                        duration = (int)sc.duration,
                        selectedweekends = sc.selectedweekends
                    })
                      .Union(
                      from sc in _db.ScheduleCalendars
                      where (int)sc.type_id == 5 && sc.IsEstimateActive == true && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                      select new CalendarEvent()
                      {
                          id = (int)sc.event_id,
                          //title = HttpUtility.HtmlDecode(sc.title),
                          title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                          description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                          start = (DateTime)sc.event_start,
                          end = (DateTime)sc.event_end,
                          customer_id = (int)sc.customer_id,
                          estimate_id = (int)sc.estimate_id,
                          employee_id = (int)sc.employee_id,
                          section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                          location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                          cssClassName = sc.cssClassName,
                          type_id = (int)sc.type_id,
                          operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                          employee_name = sc.employee_name,
                          customer_last_name = "",
                          IsScheduleDayException = true,
                          is_complete = false,
                          has_parent_link = false,
                          has_child_link = false,
                          estimate_name = "",
                          duration = 0,
                          selectedweekends = sc.selectedweekends
                      })
                      ;
        }


        else if (ncid != 0 && !IsCalendarOnline && (nTypeId == 1 || nTypeId == 11))
        {
            item = (from sc in _db.ScheduleCalendarTemps
                    where typelist.Contains((int)sc.type_id) && sc.IsEstimateActive == true && sc.customer_id == ncid && listCheckedEstId.Contains((int)sc.estimate_id) && sc.type_id != 5
                    && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                    select new CalendarEvent()
                    {
                        id = (int)sc.event_id,
                        //title = HttpUtility.HtmlDecode(sc.title),
                        title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " ") + (sc.employee_name == "" ? "" : sc.employee_name.Contains("TBD") ? "" : " (" + sc.employee_name + ") "), @"\t|\n|\r", " ")),
                        description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        start = (DateTime)sc.event_start,
                        end = (DateTime)sc.event_end,
                        customer_id = (int)sc.customer_id,
                        estimate_id = (int)sc.estimate_id,
                        employee_id = (int)sc.employee_id,
                        section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        cssClassName = sc.cssClassName,
                        type_id = (int)sc.type_id,
                        operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        employee_name = sc.employee_name,
                        customer_last_name = "",
                        IsScheduleDayException = sc.IsScheduleDayException != null ? (bool)sc.IsScheduleDayException : false,
                        is_complete = sc.is_complete != null ? (bool)sc.is_complete : false,
                        has_parent_link = (_db.ScheduleCalendarLinkTemps.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.parent_event_id == sc.event_id)) == true ? true : false,
                        has_child_link = (_db.ScheduleCalendarLinkTemps.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.child_event_id == sc.event_id)) == true ? true : false,
                        estimate_name = (_db.customer_estimates.SingleOrDefault(ce => ce.customer_id == sc.customer_id && ce.estimate_id == sc.estimate_id).estimate_name),
                        duration = (int)sc.duration,
                        selectedweekends = sc.selectedweekends
                    }).Union(
                    from sc in _db.ScheduleCalendarTemps
                    where (int)sc.type_id == 5 && sc.IsEstimateActive == true && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))

                    select new CalendarEvent()
                    {
                        id = (int)sc.event_id,
                        //title = HttpUtility.HtmlDecode(sc.title),
                        title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        start = (DateTime)sc.event_start,
                        end = (DateTime)sc.event_end,
                        customer_id = (int)sc.customer_id,
                        estimate_id = (int)sc.estimate_id,
                        employee_id = (int)sc.employee_id,
                        section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        cssClassName = sc.cssClassName,
                        type_id = (int)sc.type_id,
                        operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        employee_name = sc.employee_name,
                        customer_last_name = "",
                        IsScheduleDayException = true,
                        is_complete = false,
                        has_parent_link = false,
                        has_child_link = false,
                        estimate_name = "",
                        duration = 0,
                        selectedweekends = sc.selectedweekends
                    });
        }

        else if ((ncid == 0 || listCheckedEstId.Count() == 0) && (nTypeId == 1 || nTypeId == 11)) // for main/general calendar
        {
            item = (from sc in _db.ScheduleCalendars
                    join cust in _db.customers on sc.customer_id equals cust.customer_id into cs
                    from c in cs.DefaultIfEmpty()
                    join user in _db.user_infos on c.SuperintendentId equals user.user_id into ui
                    from u in ui.DefaultIfEmpty()
                    where typelist.Contains((int)sc.type_id) && (bool)sc.IsEstimateActive == true && (int)sc.type_id != 5 && (c.is_active != null ? c.is_active : true) == true
                    && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                    select new CalendarEvent()
                    {
                        id = (int)sc.event_id,
                        //title = HttpUtility.HtmlDecode(sc.title),
                        title = HttpUtility.HtmlDecode(Regex.Replace((c.last_name1 != null ? "(" + (c.last_name1 ?? "") + ") " : "") + sc.title.Replace(Environment.NewLine, " ") + (sc.employee_name == "" ? "" : sc.employee_name.Contains("TBD") ? "" : " (" + sc.employee_name + ") "), @"\t|\n|\r", " ")),
                        description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        start = (DateTime)sc.event_start,
                        end = (DateTime)sc.event_end,
                        customer_id = (int)sc.customer_id,
                        estimate_id = (int)sc.estimate_id,
                        employee_id = (int)sc.employee_id,
                        section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        cssClassName = sc.cssClassName,
                        type_id = (int)sc.type_id,
                        operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        employee_name = sc.employee_name,
                        customer_last_name = c.last_name1 ?? "",
                        IsScheduleDayException = sc.IsScheduleDayException != null ? (bool)sc.IsScheduleDayException : false,
                        is_complete = sc.is_complete != null ? (bool)sc.is_complete : false,
                        has_parent_link = (_db.ScheduleCalendarLinks.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.parent_event_id == sc.event_id)) == true ? true : false,
                        has_child_link = (_db.ScheduleCalendarLinks.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.child_event_id == sc.event_id)) == true ? true : false,
                        estimate_name = (_db.customer_estimates.SingleOrDefault(ce => ce.customer_id == sc.customer_id && ce.estimate_id == sc.estimate_id).estimate_name),
                        duration = (int)sc.duration,
                        SuperintendentName = u.first_name + ' ' + u.last_name,
                        selectedweekends = sc.selectedweekends
                    })
                    .Union(
                    from sc in _db.ScheduleCalendars
                    where (int)sc.type_id == 5 && sc.IsEstimateActive == true && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                    select new CalendarEvent()
                    {
                        id = (int)sc.event_id,
                        //title = HttpUtility.HtmlDecode(sc.title),
                        title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        start = (DateTime)sc.event_start,
                        end = (DateTime)sc.event_end,
                        customer_id = (int)sc.customer_id,
                        estimate_id = (int)sc.estimate_id,
                        employee_id = (int)sc.employee_id,
                        section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        cssClassName = sc.cssClassName,
                        type_id = (int)sc.type_id,
                        operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        employee_name = sc.employee_name,
                        customer_last_name = "",
                        IsScheduleDayException = true,
                        is_complete = false,
                        has_parent_link = false,
                        has_child_link = false,
                        estimate_name = "",
                        duration = 0,
                        SuperintendentName = "",
                        selectedweekends = sc.selectedweekends
                    })
                    ;
        }

        else if (ncid != 0 && (nTypeId == 2 || nTypeId == 22)) // for Sales calendar by customer
        {
            item = (from sc in _db.ScheduleCalendars
                    where typelist.Contains((int)sc.type_id) && sc.customer_id == ncid && sc.type_id != 5
                    && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                    select new CalendarEvent()
                    {
                        id = (int)sc.event_id,
                        //title = HttpUtility.HtmlDecode(sc.title),
                        title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " ") + (sc.employee_name == "" ? "" : sc.employee_name.Contains("TBD") ? "" : " (" + sc.employee_name + ") "), @"\t|\n|\r", " ")),
                        description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        start = (DateTime)sc.event_start,
                        end = (DateTime)sc.event_end,
                        customer_id = (int)sc.customer_id,
                        estimate_id = (int)sc.estimate_id,
                        employee_id = (int)sc.employee_id,
                        section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        cssClassName = sc.cssClassName,
                        type_id = (int)sc.type_id,
                        operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        employee_name = sc.employee_name,
                        customer_last_name = "",
                        IsScheduleDayException = sc.IsScheduleDayException != null ? (bool)sc.IsScheduleDayException : false,
                        is_complete = sc.is_complete != null ? (bool)sc.is_complete : false,
                        has_parent_link = (_db.ScheduleCalendarLinks.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.parent_event_id == sc.event_id)) == true ? true : false,
                        has_child_link = (_db.ScheduleCalendarLinks.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.child_event_id == sc.event_id)) == true ? true : false,
                        estimate_name = (_db.customer_estimates.SingleOrDefault(ce => ce.customer_id == sc.customer_id && ce.estimate_id == sc.estimate_id).estimate_name),
                        duration = (int)sc.duration,
                        selectedweekends = sc.selectedweekends
                    })
                     .Union(
                     from sc in _db.ScheduleCalendars
                     where (int)sc.type_id == 5 && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                     select new CalendarEvent()
                     {
                         id = (int)sc.event_id,
                         //title = HttpUtility.HtmlDecode(sc.title),
                         title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                         description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                         start = (DateTime)sc.event_start,
                         end = (DateTime)sc.event_end,
                         customer_id = (int)sc.customer_id,
                         estimate_id = (int)sc.estimate_id,
                         employee_id = (int)sc.employee_id,
                         section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                         location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                         cssClassName = sc.cssClassName,
                         type_id = (int)sc.type_id,
                         operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                         employee_name = sc.employee_name,
                         customer_last_name = "",
                         IsScheduleDayException = true,
                         is_complete = false,
                         has_parent_link = false,
                         has_child_link = false,
                         estimate_name = "",
                         duration = 0,
                         selectedweekends = sc.selectedweekends
                     })
                     ;
        }

        else if (ncid == 0 && (nTypeId == 2 || nTypeId == 22)) // for Sales calendar
        {
            item = (from sc in _db.ScheduleCalendars
                    where typelist.Contains((int)sc.type_id) && sc.type_id != 5
                    && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                    select new CalendarEvent()
                    {
                        id = (int)sc.event_id,
                        //title = HttpUtility.HtmlDecode(sc.title),
                        title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " ") + (sc.employee_name == "" ? "" : sc.employee_name.Contains("TBD") ? "" : " (" + sc.employee_name + ") "), @"\t|\n|\r", " ")),
                        description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        start = (DateTime)sc.event_start,
                        end = (DateTime)sc.event_end,
                        customer_id = (int)sc.customer_id,
                        estimate_id = (int)sc.estimate_id,
                        employee_id = (int)sc.employee_id,
                        section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        cssClassName = sc.cssClassName,
                        type_id = (int)sc.type_id,
                        operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        employee_name = sc.employee_name,
                        customer_last_name = "",
                        IsScheduleDayException = sc.IsScheduleDayException != null ? (bool)sc.IsScheduleDayException : false,
                        is_complete = sc.is_complete != null ? (bool)sc.is_complete : false,
                        has_parent_link = (_db.ScheduleCalendarLinks.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.parent_event_id == sc.event_id)) == true ? true : false,
                        has_child_link = (_db.ScheduleCalendarLinks.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.child_event_id == sc.event_id)) == true ? true : false,
                        estimate_name = (_db.customer_estimates.SingleOrDefault(ce => ce.customer_id == sc.customer_id && ce.estimate_id == sc.estimate_id).estimate_name),
                        duration = (int)sc.duration,
                        selectedweekends = sc.selectedweekends
                    })
                     .Union(
                     from sc in _db.ScheduleCalendars
                     where (int)sc.type_id == 5 && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                     select new CalendarEvent()
                     {
                         id = (int)sc.event_id,
                         //title = HttpUtility.HtmlDecode(sc.title),
                         title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                         description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                         start = (DateTime)sc.event_start,
                         end = (DateTime)sc.event_end,
                         customer_id = (int)sc.customer_id,
                         estimate_id = (int)sc.estimate_id,
                         employee_id = (int)sc.employee_id,
                         section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                         location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                         cssClassName = sc.cssClassName,
                         type_id = (int)sc.type_id,
                         operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                         employee_name = sc.employee_name,
                         customer_last_name = "",
                         IsScheduleDayException = true,
                         is_complete = false,
                         has_parent_link = false,
                         has_child_link = false,
                         estimate_name = "",
                         duration = 0,
                         selectedweekends = sc.selectedweekends
                     })
                     ;
        }

        //DataTable dtTest = csCommonUtility.LINQToDataTable(item);
        if ((nCusId != null && nCusId != 0) && (strSecName != null && strSecName != "") && (strSearchUserName != null && strSearchUserName != ""))
        {
            events = item.Distinct().ToList().Where(sc => sc.customer_id == nCusId && sc.section_name.ToUpper().Contains(strSecName.ToUpper()) && sc.employee_name.ToUpper().Contains(strSearchUserName.ToUpper())).ToList();
        }
        else if ((nCusId != null && nCusId != 0) && (strSecName != "" && strSecName != null))
        {
            events = item.Distinct().ToList().Where(sc => sc.customer_id == nCusId && sc.section_name.ToUpper().Contains(strSecName.ToUpper())).ToList();
        }
        else if ((nCusId != null && nCusId != 0) && (strSearchUserName != "" && strSearchUserName != null))
        {
            events = item.Distinct().ToList().Where(sc => sc.customer_id == nCusId && sc.employee_name.ToUpper().Contains(strSearchUserName.ToUpper())).ToList();
        }
        else if ((strSecName != "" && strSecName != null) && (strSearchUserName != "" && strSearchUserName != null))
        {
            events = item.Distinct().ToList().Where(sc => sc.section_name.ToUpper().Contains(strSecName.ToUpper()) && sc.employee_name.ToUpper().Contains(strSearchUserName.ToUpper())).ToList();
        }
        else if (strSecName != "")
        {
            events = item.Distinct().ToList().Where(sc => sc.section_name.ToUpper().Contains(strSecName.ToUpper())).ToList();
        }
        else if (nCusId != 0)
        {
            events = item.Distinct().ToList().Where(sc => sc.customer_id == nCusId).ToList();
        }
        else if (strSearchUserName != "")
        {
            events = item.Distinct().ToList().Where(sc => sc.employee_name.ToUpper().Contains(strSearchUserName.ToUpper())).ToList();
        }
        else if (strSearchSuperintendentName != "")
        {
            events = item.Distinct().ToList().Where(sc => (sc.SuperintendentName ?? "").ToUpper().Contains(strSearchSuperintendentName.ToUpper())).ToList();
        }
        else
        {
            events = item.Distinct().ToList();
        }

       // SetDayOfWeek(events);
       // var testrerwer = getWeekendDays(start, end, events);
        //DataTable testdt = csCommonUtility.LINQToDataTable(events.ToList());
        return events;
        //side note: if you want to show events only related to particular users,
        //if user id of that user is stored in session as Session["userid"]
        //the event table also contains a extra field named 'user_id' to mark the event for that particular user
        //then you can modify the SQL as:
        //SELECT event_id, description, title, event_start, event_end FROM event where user_id=@user_id AND event_start>=@start AND event_end<=@end
        //then add paramter as:cmd.Parameters.AddWithValue("@user_id", HttpContext.Current.Session["userid"]);
    }

    //For Customer Calendar
    public static List<CalendarEvent> getEventsByCusId(DateTime start, DateTime end, int nCusId, string strSecName)
    {
        TimeSpan diff = end - start;


        if (diff.Days < 8)
        {
            DateTime dt = new DateTime(start.Year, start.Month, 1);

            int ndiff = dt.DayOfWeek - DayOfWeek.Sunday;

            DateTime dtNewStart = dt.AddDays(-ndiff);

            DateTime dtNewEnd = dtNewStart.AddDays(42);

            start = dtNewStart;

            end = dtNewEnd;
        }

        int nTypeId = 0;
        int nEstimateID = 0;
        if (System.Web.HttpContext.Current.Session["TypeID"] != null)
        {
            nTypeId = (int)System.Web.HttpContext.Current.Session["TypeID"];
        }
        //if (System.Web.HttpContext.Current.Session["eid"] != null)
        //{
        //    nEstimateID = (int)System.Web.HttpContext.Current.Session["eid"];
        //}

        IEnumerable<int> listCheckedEstId = new int[] { };

        if (System.Web.HttpContext.Current.Session["sSelectedCustEstIdList"] != null)
        {
            listCheckedEstId = System.Web.HttpContext.Current.Session["sSelectedCustEstIdList"] as IEnumerable<int>;
        }

        var typelist = new int[] { 5, nTypeId };
        // var typelist = new int[] { nTypeId };
        DataClassesDataContext _db = new DataClassesDataContext();
        List<CalendarEvent> events = new List<CalendarEvent>();

        string[] strDayOfWeek = new string[] { "Sunday" };

        var item = (from sc in _db.ScheduleCalendars
                    where typelist.Contains((int)sc.type_id) && sc.customer_id == nCusId && listCheckedEstId.Contains((int)sc.estimate_id) //&& sc.estimate_id == nEstimateID //&& ((sc.employee_name ?? "") != "" && !(sc.employee_name ?? "").Trim().ToUpper().Contains("TBD"))
                    && sc.IsEstimateActive == true
                    && (Convert.ToDateTime(sc.event_start).DayOfWeek != DayOfWeek.Sunday
                    && Convert.ToDateTime(sc.event_start).DayOfWeek != DayOfWeek.Saturday)
                    select new CalendarEvent()
                    {
                        id = (int)sc.event_id,
                        //title = HttpUtility.HtmlDecode(sc.title),
                        title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        start = (DateTime)sc.event_start,
                        end = (DateTime)sc.event_end,
                        customer_id = (int)sc.customer_id,
                        estimate_id = (int)sc.estimate_id,
                        employee_id = (int)sc.employee_id,
                        section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        cssClassName = sc.cssClassName,
                        type_id = (int)sc.type_id,
                        operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        employee_name = sc.employee_name ?? "",
                        estimate_name = (_db.customer_estimates.SingleOrDefault(ce => ce.customer_id == sc.customer_id && ce.estimate_id == sc.estimate_id).estimate_name),
                        selectedweekends = sc.selectedweekends
                    }).Union
                   (from sc in _db.ScheduleCalendars
                    where sc.type_id == 11 && sc.customer_id == nCusId && sc.estimate_id == nEstimateID
                   && sc.IsEstimateActive == true
                    select new CalendarEvent()
                    {
                        id = (int)sc.event_id,
                        //title = HttpUtility.HtmlDecode(sc.title),
                        title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        start = (DateTime)sc.event_start,
                        end = (DateTime)sc.event_end,
                        customer_id = (int)sc.customer_id,
                        estimate_id = (int)sc.estimate_id,
                        employee_id = (int)sc.employee_id,
                        section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        cssClassName = sc.cssClassName,
                        type_id = (int)sc.type_id,
                        operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        employee_name = sc.employee_name ?? "",
                        estimate_name = "",
                        selectedweekends = sc.selectedweekends
                    });
        var tDayOfWeek = "";

        foreach (CalendarEvent ce in item)
        {
            if (strDayOfWeek.Contains(Convert.ToDateTime(ce.start).DayOfWeek.ToString()))
                tDayOfWeek = Convert.ToDateTime(ce.start).DayOfWeek.ToString();
        }

        var result = (from sc in item.AsEnumerable()
                      where sc.type_id != 11
                      select new CalendarEvent()
                      {
                          id = sc.id,
                          //title = HttpUtility.HtmlDecode(sc.title),
                          title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                          description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                          start = (DateTime)sc.start,
                          end = DateTime.Parse(GetDayOfWeekWithOutHoliday(Convert.ToDateTime(sc.end).ToShortDateString()) + " " + Convert.ToDateTime(sc.end).ToShortTimeString()),
                          customer_id = (int)sc.customer_id,
                          estimate_id = (int)sc.estimate_id,
                          employee_id = (int)sc.employee_id,
                          section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                          location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                          cssClassName = sc.cssClassName,
                          type_id = (int)sc.type_id,
                          operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                          employee_name = sc.employee_name ?? "",
                          estimate_name = sc.estimate_name,
                          selectedweekends = sc.selectedweekends
                      }).Union
                     (from sc in item.AsEnumerable()
                      where sc.type_id == 11
                      select new CalendarEvent()
                      {
                          id = sc.id,
                          //title = HttpUtility.HtmlDecode(sc.title),
                          title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                          description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                          start = (DateTime)sc.start,
                          end = (DateTime)sc.end,
                          customer_id = (int)sc.customer_id,
                          estimate_id = (int)sc.estimate_id,
                          employee_id = (int)sc.employee_id,
                          section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                          location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                          cssClassName = sc.cssClassName,
                          type_id = (int)sc.type_id,
                          operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                          employee_name = sc.employee_name ?? "",
                          estimate_name = sc.estimate_name,
                          selectedweekends = sc.selectedweekends
                      });
        events = result.ToList();
        var testrerwer = getWeekendDays(start, end, events);
        //  DataTable test = csCommonUtility.LINQToDataTable(item);
        return events;
        //side note: if you want to show events only related to particular users,
        //if user id of that user is stored in session as Session["userid"]
        //the event table also contains a extra field named 'user_id' to mark the event for that particular user
        //then you can modify the SQL as:
        //SELECT event_id, description, title, event_start, event_end FROM event where user_id=@user_id AND event_start>=@start AND event_end<=@end
        //then add paramter as:cmd.Parameters.AddWithValue("@user_id", HttpContext.Current.Session["userid"]);
    }

    public static List<CalendarEvent> getEventsByCrewUserId(DateTime start, DateTime end, int nSuperId, string strSecName, int nEmpid, string strEmpName)
    {
        TimeSpan diff = end - start;


        if (diff.Days < 8)
        {
            DateTime dt = new DateTime(start.Year, start.Month, 1);

            int ndiff = dt.DayOfWeek - DayOfWeek.Sunday;

            DateTime dtNewStart = dt.AddDays(-ndiff);

            DateTime dtNewEnd = dtNewStart.AddDays(42);

            start = dtNewStart;

            end = dtNewEnd;
        }

        int nTypeId = 1;
        int nEstimateID = 0;
        if (System.Web.HttpContext.Current.Session["TypeID"] != null)
        {
            nTypeId = (int)System.Web.HttpContext.Current.Session["TypeID"];
        }
        if (System.Web.HttpContext.Current.Session["eid"] != null)
        {
            nEstimateID = (int)System.Web.HttpContext.Current.Session["eid"];
        }
        var typelist = new int[] { nTypeId };
        // var typelist = new int[] { nTypeId };
        DataClassesDataContext _db = new DataClassesDataContext();
        List<CalendarEvent> events = new List<CalendarEvent>();

        string[] strDayOfWeek = new string[] { "Sunday" };
        if (strEmpName == "" && nSuperId > 0)
        {
            var item = (from sc in _db.ScheduleCalendars
                        join cust in _db.customers on sc.customer_id equals cust.customer_id into cs
                        from c in cs.DefaultIfEmpty()
                        where typelist.Contains((int)sc.type_id) && (bool)sc.IsEstimateActive == true && sc.type_id != 5 && c.SuperintendentId == nSuperId
                        && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                        select new CalendarEvent()
                        {
                            id = (int)sc.event_id,
                            //title = HttpUtility.HtmlDecode(sc.title),
                            title = HttpUtility.HtmlDecode(Regex.Replace((c.last_name1 != null ? "(" + (c.last_name1 ?? "") + ") " : "") + sc.title.Replace(Environment.NewLine, " ") + (sc.employee_name == "" ? "" : sc.employee_name.Contains("TBD") ? "" : " (" + sc.employee_name + ") "), @"\t|\n|\r", " ")),
                            description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                            start = (DateTime)sc.event_start,
                            end = (DateTime)sc.event_end,
                            customer_id = (int)sc.customer_id,
                            estimate_id = (int)sc.estimate_id,
                            employee_id = (int)sc.employee_id,
                            section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                            location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                            cssClassName = sc.cssClassName,
                            type_id = (int)sc.type_id,
                            operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                            employee_name = sc.employee_name,
                            is_complete = sc.is_complete != null ? (bool)sc.is_complete : false,
                            has_parent_link = (_db.ScheduleCalendarLinks.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.parent_event_id == sc.event_id)) == true ? true : false,
                            has_child_link = (_db.ScheduleCalendarLinks.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.child_event_id == sc.event_id)) == true ? true : false,
                            estimate_name = (_db.customer_estimates.SingleOrDefault(ce => ce.customer_id == sc.customer_id && ce.estimate_id == sc.estimate_id).estimate_name),
                            duration = (int)sc.duration,
                            IsScheduleDayException = sc.IsScheduleDayException != null ? (bool)sc.IsScheduleDayException : false
                        })
                        .Union(
                        from sc in _db.ScheduleCalendars
                        where (int)sc.type_id == 5 && sc.IsEstimateActive == true && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                        select new CalendarEvent()
                        {
                            id = (int)sc.event_id,
                            //title = HttpUtility.HtmlDecode(sc.title),
                            title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                            description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                            start = (DateTime)sc.event_start,
                            end = (DateTime)sc.event_end,
                            customer_id = (int)sc.customer_id,
                            estimate_id = (int)sc.estimate_id,
                            employee_id = (int)sc.employee_id,
                            section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                            location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                            cssClassName = sc.cssClassName,
                            type_id = (int)sc.type_id,
                            operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                            employee_name = sc.employee_name,
                            is_complete = sc.is_complete != null ? (bool)sc.is_complete : false,
                            has_parent_link = false,
                            has_child_link = false,
                            estimate_name = "",
                            duration = 0,
                            IsScheduleDayException = true
                        })
                        ;

            events = item.ToList();
        }
        else
        {
            var itemCrew = (from sc in _db.ScheduleCalendars
                            join cust in _db.customers on sc.customer_id equals cust.customer_id into cs
                            from c in cs.DefaultIfEmpty()
                            where typelist.Contains((int)sc.type_id) && (bool)sc.IsEstimateActive == true && sc.type_id != 5 && sc.employee_name.ToLower().Contains(strEmpName.ToLower())
                            && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                            select new CalendarEvent()
                            {
                                id = (int)sc.event_id,
                                //title = HttpUtility.HtmlDecode(sc.title),
                                title = HttpUtility.HtmlDecode(Regex.Replace((c.last_name1 != null ? "(" + (c.last_name1 ?? "") + ") " : "") + sc.title.Replace(Environment.NewLine, " ") + (sc.employee_name == "" ? "" : sc.employee_name.Contains("TBD") ? "" : " (" + sc.employee_name + ") "), @"\t|\n|\r", " ")),
                                description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                                start = (DateTime)sc.event_start,
                                end = (DateTime)sc.event_end,
                                customer_id = (int)sc.customer_id,
                                estimate_id = (int)sc.estimate_id,
                                employee_id = (int)sc.employee_id,
                                section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                                location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                                cssClassName = sc.cssClassName,
                                type_id = (int)sc.type_id,
                                operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                                employee_name = sc.employee_name,
                                is_complete = sc.is_complete != null ? (bool)sc.is_complete : false,
                                has_parent_link = (_db.ScheduleCalendarLinks.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.parent_event_id == sc.event_id)) == true ? true : false,
                                has_child_link = (_db.ScheduleCalendarLinks.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.child_event_id == sc.event_id)) == true ? true : false,
                                estimate_name = (_db.customer_estimates.SingleOrDefault(ce => ce.customer_id == sc.customer_id && ce.estimate_id == sc.estimate_id).estimate_name),
                                duration = (int)sc.duration,
                                IsScheduleDayException = sc.IsScheduleDayException != null ? (bool)sc.IsScheduleDayException : false
                            })
                   .Union(
                   from sc in _db.ScheduleCalendars
                   where (int)sc.type_id == 5 && sc.IsEstimateActive == true && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                   select new CalendarEvent()
                   {
                       id = (int)sc.event_id,
                       //title = HttpUtility.HtmlDecode(sc.title),
                       title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                       description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                       start = (DateTime)sc.event_start,
                       end = (DateTime)sc.event_end,
                       customer_id = (int)sc.customer_id,
                       estimate_id = (int)sc.estimate_id,
                       employee_id = (int)sc.employee_id,
                       section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                       location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                       cssClassName = sc.cssClassName,
                       type_id = (int)sc.type_id,
                       operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                       employee_name = sc.employee_name,
                       is_complete = sc.is_complete != null ? (bool)sc.is_complete : false,
                       has_parent_link = false,
                       has_child_link = false,
                       estimate_name = "",
                       duration = 0,
                       IsScheduleDayException = true
                   })
                   ;

            events = itemCrew.ToList();
        }

        //  DataTable test = csCommonUtility.LINQToDataTable(item);
        return events;

    }

    public static List<CalendarEvent> getEventsreadonly(DateTime start, DateTime end, int nCusId, string strSecName, string strSearchUserName)
    {

        TimeSpan diff = end - start;


        if (diff.Days < 8)
        {
            DateTime dt = new DateTime(start.Year, start.Month, 1);

            int ndiff = dt.DayOfWeek - DayOfWeek.Sunday;

            DateTime dtNewStart = dt.AddDays(-ndiff);

            DateTime dtNewEnd = dtNewStart.AddDays(42);

            start = dtNewStart;

            end = dtNewEnd;
        }

        int nTypeId = 0;
        int nOprationExtTypeID = 0;



        if (System.Web.HttpContext.Current.Session["TypeID"] != null)
        {
            nTypeId = (int)System.Web.HttpContext.Current.Session["TypeID"];
        }

        bool IsCalendarOnline = true;

        if (System.Web.HttpContext.Current.Session["sIsCalendarOnline"] != null)
            IsCalendarOnline = (bool)System.Web.HttpContext.Current.Session["sIsCalendarOnline"];

        //Opration/Sales Extra Type ID
        if (nTypeId == 1)
            nOprationExtTypeID = 11;
        if (nTypeId == 2)
            nOprationExtTypeID = 22;

        var typelist = new int[] { 0, 5, nTypeId, nOprationExtTypeID };
        // var typelist = new int[] { nTypeId };
        DataClassesDataContext _db = new DataClassesDataContext();
        List<CalendarEvent> events = new List<CalendarEvent>();




        var item = (from sc in _db.ScheduleCalendars
                    join cust in _db.customers on sc.customer_id equals cust.customer_id into cs
                    from c in cs.DefaultIfEmpty()
                    where typelist.Contains((int)sc.type_id) && (bool)sc.IsEstimateActive == true && (int)sc.type_id != 5 && (c.is_active != null ? c.is_active : true) == true
                    && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                    select new CalendarEvent()
                    {
                        id = (int)sc.event_id,
                        //title = HttpUtility.HtmlDecode(sc.title),
                        title = HttpUtility.HtmlDecode(Regex.Replace((c.last_name1 != null ? "(" + (c.last_name1 ?? "") + ") " : "") + sc.title.Replace(Environment.NewLine, " ") + (sc.employee_name == "" ? "" : sc.employee_name.Contains("TBD") ? "" : " (" + sc.employee_name + ") "), @"\t|\n|\r", " ")),
                        description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        start = (DateTime)sc.event_start,
                        end = (DateTime)sc.event_end,
                        customer_id = (int)sc.customer_id,
                        estimate_id = (int)sc.estimate_id,
                        employee_id = (int)sc.employee_id,
                        section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        cssClassName = sc.cssClassName,
                        type_id = (int)sc.type_id,
                        operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        employee_name = sc.employee_name,
                        customer_last_name = c.last_name1 ?? "",
                        is_complete = sc.is_complete != null ? (bool)sc.is_complete : false,
                        has_parent_link = (_db.ScheduleCalendarLinks.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.parent_event_id == sc.event_id)) == true ? true : false,
                        has_child_link = (_db.ScheduleCalendarLinks.Any(scl => scl.customer_id == sc.customer_id && scl.estimate_id == sc.estimate_id && scl.child_event_id == sc.event_id)) == true ? true : false,
                        estimate_name = (_db.customer_estimates.SingleOrDefault(ce => ce.customer_id == sc.customer_id && ce.estimate_id == sc.estimate_id).estimate_name),
                        duration = (int)sc.duration
                    })
                    .Union(
                    from sc in _db.ScheduleCalendars
                    where (int)sc.type_id == 5 && sc.IsEstimateActive == true && ((sc.event_start >= start && sc.event_start <= end) || (sc.event_end >= start && sc.event_end <= end))
                    select new CalendarEvent()
                    {
                        id = (int)sc.event_id,
                        //title = HttpUtility.HtmlDecode(sc.title),
                        title = HttpUtility.HtmlDecode(Regex.Replace(sc.title.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        description = HttpUtility.HtmlDecode(Regex.Replace(sc.description.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        start = (DateTime)sc.event_start,
                        end = (DateTime)sc.event_end,
                        customer_id = (int)sc.customer_id,
                        estimate_id = (int)sc.estimate_id,
                        employee_id = (int)sc.employee_id,
                        section_name = HttpUtility.HtmlDecode(Regex.Replace(sc.section_name != null ? sc.section_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        location_name = HttpUtility.HtmlDecode(Regex.Replace(sc.location_name != null ? sc.location_name.Replace(Environment.NewLine, " ") : "", @"\t|\n|\r", " ")),
                        cssClassName = sc.cssClassName,
                        type_id = (int)sc.type_id,
                        operation_notes = HttpUtility.HtmlDecode(Regex.Replace(sc.operation_notes.Replace(Environment.NewLine, " "), @"\t|\n|\r", " ")),
                        employee_name = sc.employee_name,
                        customer_last_name = "",
                        is_complete = sc.is_complete != null ? (bool)sc.is_complete : false,
                        has_parent_link = false,
                        has_child_link = false,
                        estimate_name = "",
                        duration = 0
                    })
                    ;

        //  DataTable dtTest = csCommonUtility.LINQToDataTable(item);
        if ((nCusId != null && nCusId != 0) && (strSecName != null && strSecName != "") && (strSearchUserName != null && strSearchUserName != ""))
        {
            events = item.ToList().Where(sc => sc.customer_id == nCusId && sc.section_name.ToUpper().Contains(strSecName.ToUpper()) && sc.employee_name.ToUpper().Contains(strSearchUserName.ToUpper())).ToList();
        }
        else if ((nCusId != null && nCusId != 0) && (strSecName != "" && strSecName != null))
        {
            events = item.ToList().Where(sc => sc.customer_id == nCusId && sc.section_name.ToUpper().Contains(strSecName.ToUpper())).ToList();
        }
        else if ((nCusId != null && nCusId != 0) && (strSearchUserName != "" && strSearchUserName != null))
        {
            events = item.ToList().Where(sc => sc.customer_id == nCusId && sc.employee_name.ToUpper().Contains(strSearchUserName.ToUpper())).ToList();
        }
        else if ((strSecName != "" && strSecName != null) && (strSearchUserName != "" && strSearchUserName != null))
        {
            events = item.ToList().Where(sc => sc.section_name.ToUpper().Contains(strSecName.ToUpper()) && sc.employee_name.ToUpper().Contains(strSearchUserName.ToUpper())).ToList();
        }
        else if (strSecName != "")
        {
            events = item.ToList().Where(sc => sc.section_name.ToUpper().Contains(strSecName.ToUpper())).ToList();
        }
        else if (nCusId != 0)
        {
            events = item.ToList().Where(sc => sc.customer_id == nCusId).ToList();
        }
        else if (strSearchUserName != "")
        {
            events = item.ToList().Where(sc => sc.employee_name.ToUpper().Contains(strSearchUserName.ToUpper())).ToList();
        }
        else
        {
            events = item.ToList();
        }

        return events;
        //side note: if you want to show events only related to particular users,
        //if user id of that user is stored in session as Session["userid"]
        //the event table also contains a extra field named 'user_id' to mark the event for that particular user
        //then you can modify the SQL as:
        //SELECT event_id, description, title, event_start, event_end FROM event where user_id=@user_id AND event_start>=@start AND event_end<=@end
        //then add paramter as:cmd.Parameters.AddWithValue("@user_id", HttpContext.Current.Session["userid"]);
    }

    public static List<CalendarEvent> getEventsForCrewCal(DateTime start, DateTime end, int nCusId, string strSecName, string strSearchUserName, string strSearchSuperintendentName)
    {

        TimeSpan diff = end - start;


        if (diff.Days < 8)
        {
            DateTime dt = new DateTime(start.Year, start.Month, 1);

            int ndiff = dt.DayOfWeek - DayOfWeek.Sunday;

            DateTime dtNewStart = dt.AddDays(-ndiff);

            DateTime dtNewEnd = dtNewStart.AddDays(42);

            start = dtNewStart;

            end = dtNewEnd;
        }

        int nTypeId = 0;
        int nOprationExtTypeID = 0;



        if (System.Web.HttpContext.Current.Session["crsTypeID"] != null)
        {
            nTypeId = (int)System.Web.HttpContext.Current.Session["crsTypeID"];
        }


        string strCondition = "";

        if (nCusId != 0)
        {
            strCondition = "AND cte.customer_id = " + nCusId;
        }

        if (strSecName != "")
        {
            if (strCondition.Length > 0)
            {
                strCondition += " AND cte.section_name = '" + strSecName + "'";
            }
            else
            {
                strCondition = " AND cte.section_name = '" + strSecName + "'";
            }
        }

        if (strSearchUserName != "")
        {
            if (strCondition.Length > 0)
            {
                strCondition += " AND empItem like '%" + strSearchUserName + "%'";
            }
            else
            {
                strCondition = " AND empItem like '%" + strSearchUserName + "%'";
            }
        }

        if (strSearchSuperintendentName != "")
        {
            if (strSearchSuperintendentName.Length > 0)
            {
                strCondition += " AND (RTRIM(LTRIM(u.first_name)) +' '+RTRIM(LTRIM(u.last_name))) like '%" + strSearchSuperintendentName.Trim() + "%'";
            }
            else
            {
                strCondition = " AND (RTRIM(LTRIM(u.first_name)) +' '+RTRIM(LTRIM(u.last_name))) like '%" + strSearchSuperintendentName.Trim() + "%'";
            }
        }


        //Opration/Sales Extra Type ID
        if (nTypeId == 1)
            nOprationExtTypeID = 11;
        if (nTypeId == 2)
            nOprationExtTypeID = 22;

        var typelist = new int[] { 0, 5, nTypeId, nOprationExtTypeID };
        // var typelist = new int[] { nTypeId };
        DataClassesDataContext _db = new DataClassesDataContext();
        List<CalendarEvent> events = new List<CalendarEvent>();

        string strQ = "with cte (title, description, event_start, event_end, customer_id, estimate_id, employee_id, section_name, location_name, type_id, employee_name," +
                    "  cssClassName, operation_notes, is_complete, IsEstimateActive,  event_id, duration, empItem, emp) as " +
                    " ( " +
                    " select title, description, event_start, event_end, customer_id, estimate_id, employee_id, section_name, location_name, type_id, employee_name," +
                    " cssClassName, operation_notes, is_complete, IsEstimateActive,  event_id, duration, " +
                    " cast(left(employee_name, charindex(',',employee_name+',')-1) as varchar(50)) empItem," +
                    " stuff(employee_name, 1, charindex(',',employee_name+','), '') emp " +
                    " from ScheduleCalendar " +
                    " union all " +
                    " select title, description, event_start, event_end, customer_id, estimate_id, employee_id, section_name, location_name, type_id, employee_name," +
                    " cssClassName, operation_notes, is_complete, IsEstimateActive,  event_id, duration, " +
                    " cast(left(emp, charindex(',',emp+',')-1) as varchar(50)) empItem, " +
                    " stuff(emp, 1, charindex(',',emp+','), '') emp " +
                    " from cte " +
                    " where (LEN(emp) > 0) " +
                    " )  " +
                   " select  " +
                    " (RTRIM(LTRIM(empItem)) +' ('+RTRIM(LTRIM(c.last_name1)) +')') AS title,  " +
                    " '' as description, event_start as [start], event_end as [end], cte.customer_id, 0 as estimate_id, employee_id, section_name as section_name, '' as location_name, type_id, " +
                    " ISNULL(u.cssClassName, 'fc-default') as cssClassName, '' as operation_notes, CONVERT(BIT,0) as is_complete,  " +
                    " Max(event_id) AS id,  " +
                    " duration as duration, CONVERT(BIT,0) AS has_parent_link, CONVERT(BIT,0) AS has_child_link, '' AS estimate_name, '' AS customer_last_name, u.first_name + ' ' + u.last_name as employee_name " +
                    " from cte  " +
                    " inner join customers as c on cte.customer_id = c.customer_id  " +
                    " left outer join user_info as u on c.SuperintendentId = u.user_id " +
                    " where c.status_id = 2 AND  (LEN(empItem) > 0) and type_id in (0, 5," + nTypeId + ", " + nOprationExtTypeID + ") and empItem not like '%TBD%' " +
                    " and IsEstimateActive = 1 and ((event_start between '" + start + "' and '" + end + "' ) OR (event_end between '" + start + "'  and '" + end + "' )) " + strCondition +
                    " group by empItem, event_start, event_end, cte.customer_id, type_id, u.cssClassName, c.last_name1, cte.employee_id,section_name, u.first_name, u.last_name, duration";

        IEnumerable<CalendarEvent> item = _db.ExecuteQuery<CalendarEvent>(strQ, string.Empty);

        events = item.ToList();

        return events;

    }

    //this method updates the event title and description
    public static int updateEvent(int id, String title, String section, String location, String description, string cssClassName, DateTime start, DateTime end, int empId, string empName,
        int child_event_id, int dependencyType, int offsetDays, int parentDependencyType, int parentOffsetDays, string linkType, int custId, int estId, bool IsScheduleDayException, bool IsComplete, string selectedWeekend)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        int nTypeId = 0;
        int nCheck = 0;
        int ncid = 0;
        int neid = 0;

        string strUName = (string)System.Web.HttpContext.Current.Session["uname"];
        int nEmployeeID = 0;
        string strEmployeeName = "";
        int nOffsetDays = 0;

        int nWeekCount = GetHolidayCount(start, end);

        int nDuration = GetDuration(start, end);
        string strSection = "";

        DateTime dtStart = DateTime.Parse(Convert.ToDateTime(start).ToShortDateString() + " " + Convert.ToDateTime(start).ToShortTimeString());
        DateTime dtEnd = DateTime.Parse(Convert.ToDateTime(end).ToShortDateString() + " " + Convert.ToDateTime(end).ToShortTimeString());

        if (!IsScheduleDayException)
        {
            dtStart = DateTime.Parse(GetDayOfWeek(Convert.ToDateTime(start).ToShortDateString()) + " " + Convert.ToDateTime(start).ToShortTimeString());
            dtEnd = DateTime.Parse(GetDayOfWeek(Convert.ToDateTime(end).ToShortDateString()) + " " + Convert.ToDateTime(end).ToShortTimeString());
        }

        if (empId != null)
        {
            nEmployeeID = empId;
            strEmployeeName = empName.Trim().TrimEnd(',');
        }


        if (section != null)
        {
            strSection = section.Replace("'", "''");
        }

        string strTitle = strSection;

        if (location.Length > 0)
        {
            strTitle = strSection + " (" + location.Replace("'", "''") + ")";
        }


        if (System.Web.HttpContext.Current.Session["TypeID"] != null)
        {
            nTypeId = (int)System.Web.HttpContext.Current.Session["TypeID"];
            if (nTypeId == 1) // Operation Calendar
            {


                if (custId != null && custId > 0)
                {
                    System.Web.HttpContext.Current.Session.Add("cid", custId);
                }

                if (System.Web.HttpContext.Current.Session["cid"] != null)
                {
                    ncid = (int)System.Web.HttpContext.Current.Session["cid"];
                }

                if (estId != null && estId > 0)
                {
                    System.Web.HttpContext.Current.Session.Add("eid", estId);
                }

                if (System.Web.HttpContext.Current.Session["eid"] != null)
                {
                    neid = (int)System.Web.HttpContext.Current.Session["eid"];
                }

                if (ncid == 0 && id == 0)
                {
                    nCheck = 5;
                    return nCheck;
                }



                customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == ncid);
                //if (objCust != null)
                //{
                //    if (objCust.isCalendarOnline == true)
                //    {
                //        nCheck = 2;
                //        return nCheck;
                //    }
                //}
                //else
                //{
                //    nCheck = 5;
                //    return nCheck;
                //}



                if (offsetDays != null)
                {
                    nOffsetDays = offsetDays;
                }



                // Update Online Calendar 
                nCheck = UpdateOnlineSchedule(id, strTitle, strSection, location.Replace("'", "''"), description.Replace("'", "''"), cssClassName, dtStart, dtEnd, nEmployeeID, strEmployeeName, child_event_id, dependencyType, offsetDays, parentDependencyType, parentOffsetDays, linkType, ncid, neid, nDuration, strUName, nOffsetDays, IsScheduleDayException, IsComplete, selectedWeekend);

                if (!_db.ScheduleCalendarTemps.Any(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid))
                {
                    return nCheck;
                }

                if (_db.ScheduleCalendarLinkTemps.Any(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid))
                {
                    var objChildLink = _db.ScheduleCalendarLinkTemps.SingleOrDefault(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid);

                    var objP = _db.ScheduleCalendarTemps.SingleOrDefault(sc => sc.event_id == objChildLink.parent_event_id);

                    DateTime dtNewStartC = new DateTime(start.Year, start.Month, start.Day);
                    DateTime dtNewEndP = new DateTime(Convert.ToDateTime(objP.event_end).Year, Convert.ToDateTime(objP.event_end).Month, Convert.ToDateTime(objP.event_end).Day);

                    DateTime dtNewStartP = new DateTime(Convert.ToDateTime(objP.event_start).Year, Convert.ToDateTime(objP.event_start).Month, Convert.ToDateTime(objP.event_start).Day);

                    TimeSpan diffPrntEndToChldStart = dtNewStartC - dtNewEndP; // Parent End date to Child Start Date
                    TimeSpan diffPrnttStartToChldStart = dtNewStartC - dtNewStartP; // Parent Start date to Child Start Date

                    int nDaysPrntEndToChldStart = diffPrntEndToChldStart.Days;// Parent End date to Child Start Date
                    int nDaysPrnttStartToChldStart = diffPrnttStartToChldStart.Days;// Parent Start date to Child Start Date

                    if (nDaysPrntEndToChldStart > 0)// Parent End date to Child Start Date
                    {
                        nCheck = 0;

                    }
                    else if (nDaysPrntEndToChldStart < 0)
                    {
                        nCheck = nDaysPrntEndToChldStart;

                        if (nDaysPrnttStartToChldStart == 0)// Parent Start date to Child Start Date
                        {
                            nCheck = 0;
                        }
                        else if (nDaysPrnttStartToChldStart > 0)// Parent Start date to Child Start Date
                        {
                            nCheck = -1;
                        }
                    }
                    else if (nDaysPrntEndToChldStart == 0 && nDaysPrnttStartToChldStart == 0)
                    {
                        nCheck = 0;
                    }
                    else
                    {
                        nCheck = -1;
                    }


                    if (nCheck < 0)
                    {
                        return nCheck;
                    }
                }





                string sql = "UPDATE ScheduleCalendarTemp SET title='" + strTitle + "', section_name='" + strSection + "', location_name='" + location.Replace("'", "''") + "', description='" + description.Replace("'", "''") + "', " +
                            " cssClassName='" + cssClassName + "', event_start='" + dtStart + "', event_end='" + dtEnd + "',  duration='" + nDuration + "', employee_id='" + nEmployeeID + "',  employee_name='" + strEmployeeName + "', is_complete='" + Convert.ToInt32(IsComplete) + "'," +
                            " last_updated_by='" + strUName + "', last_updated_date='" + DateTime.Now + "',  " +
                            " IsScheduleDayException=" + Convert.ToInt32(IsScheduleDayException) + ", IsEWSCalendarSynch = '" + Convert.ToBoolean(0) + "' " + 
                            " , weekends = '" + setWeekendDays(start, end) + "'" +
                            " , selectedweekends = '" + selectedWeekend + "'" +
                            " WHERE event_id=" + id + " AND customer_id =" + ncid + " AND estimate_id = " + neid;

                _db.ExecuteCommand(sql, string.Empty);

                if (!IsScheduleDayException)
                {
                    int npEventId = 0;
                    int ncEventId = 0;

                    var testIsNullOrEmpty = String.IsNullOrEmpty(linkType);

                    if ((!String.IsNullOrEmpty(linkType)) && linkType.Equals("Parent"))
                    {
                        npEventId = id;
                        ncEventId = child_event_id;
                        id = ncEventId;
                        child_event_id = npEventId;

                        dependencyType = parentDependencyType;
                        nOffsetDays = parentOffsetDays;
                    }

                    if (child_event_id != 0)//New Event Link Insert
                    {
                        ScheduleCalendarLinkTemp objSCLinkTmp = new ScheduleCalendarLinkTemp();

                        ScheduleCalendarTemp objParentSCTmp = _db.ScheduleCalendarTemps.SingleOrDefault(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid);
                        ScheduleCalendarTemp objChildSCTmp = _db.ScheduleCalendarTemps.SingleOrDefault(sc => sc.event_id == child_event_id && sc.customer_id == ncid && sc.estimate_id == neid);

                        if (objParentSCTmp != null && objChildSCTmp != null)
                        {

                            int nLinkId = 1;

                            if (_db.ScheduleCalendarLinkTemps.Any())
                            {
                                int nMaxSCLink = Convert.ToInt32(_db.ScheduleCalendarLinks.DefaultIfEmpty().Max(e => e == null ? 0 : e.link_id));
                                int nMaxSCLinkTemp = Convert.ToInt32(_db.ScheduleCalendarLinkTemps.DefaultIfEmpty().Max(e => e == null ? 0 : e.link_id));

                                if (nMaxSCLinkTemp > nMaxSCLink)
                                    nLinkId = nMaxSCLinkTemp + 1;
                                else
                                    nLinkId = nMaxSCLink + 1;
                            }
                            objSCLinkTmp.link_id = nLinkId;
                            objSCLinkTmp.parent_event_id = id;
                            objSCLinkTmp.child_event_id = child_event_id;
                            objSCLinkTmp.customer_id = objParentSCTmp.customer_id;
                            objSCLinkTmp.estimate_id = objParentSCTmp.estimate_id;
                            objSCLinkTmp.dependencyType = dependencyType;
                            objSCLinkTmp.lag = nOffsetDays;

                            _db.ScheduleCalendarLinkTemps.InsertOnSubmit(objSCLinkTmp);
                            _db.SubmitChanges();

                            if (dependencyType == 1) // Start Same Time
                            {
                                nDuration = Convert.ToInt32(objChildSCTmp.duration) - 1;
                                DateTime dtOldStart = Convert.ToDateTime(objChildSCTmp.event_start);
                                DateTime dtOldEnd = Convert.ToDateTime(objChildSCTmp.event_end);
                                DateTime dtNewStart = Convert.ToDateTime(objParentSCTmp.event_start);

                                dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                for (int i = 1; i <= nDuration; i++)
                                {
                                    dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                                }

                                objChildSCTmp.event_start = dtNewStart;
                                objChildSCTmp.event_end = dtNewEnd;
                                objChildSCTmp.IsEWSCalendarSynch = false;
                                _db.SubmitChanges();
                            }

                            if (dependencyType == 2) // Start After Finish
                            {

                                nDuration = Convert.ToInt32(objChildSCTmp.duration) - 1;
                                DateTime dtOldStart = Convert.ToDateTime(objChildSCTmp.event_start);
                                DateTime dtOldEnd = Convert.ToDateTime(objChildSCTmp.event_end);


                                DateTime dtNewStart = GetWorkingDay(Convert.ToDateTime(objParentSCTmp.event_end).AddDays(1));

                                dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                for (int i = 1; i <= nDuration; i++)
                                {
                                    dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                                }



                                objChildSCTmp.event_start = dtNewStart;

                                objChildSCTmp.event_end = dtNewEnd;

                                objChildSCTmp.IsEWSCalendarSynch = false;
                                _db.SubmitChanges();
                            }

                            if (dependencyType == 3) // Offset days
                            {


                                nDuration = Convert.ToInt32(objChildSCTmp.duration) - 1;


                                DateTime dtOldStart = Convert.ToDateTime(objChildSCTmp.event_start);
                                DateTime dtOldEnd = Convert.ToDateTime(objChildSCTmp.event_end);


                                DateTime dtNewStart = Convert.ToDateTime(objParentSCTmp.event_end);

                                for (int i = 0; i <= nOffsetDays; i++)
                                {
                                    dtNewStart = GetWorkingDay(dtNewStart.AddDays(1));

                                }


                                dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                for (int i = 1; i <= nDuration; i++)
                                {
                                    dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                                }

                                objChildSCTmp.event_start = dtNewStart;

                                objChildSCTmp.event_end = dtNewEnd;

                                objChildSCTmp.IsEWSCalendarSynch = false;
                                _db.SubmitChanges();
                            }


                            updateAllLink(child_event_id);
                        }
                    }
                    else//Update Event Link 
                    {
                        ScheduleCalendarTemp ocjSCTempUpdate = _db.ScheduleCalendarTemps.SingleOrDefault(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid);
                        //If Event is Parent
                        List<ScheduleCalendarLinkTemp> objSCLinkTmpP = new List<ScheduleCalendarLinkTemp>();

                        if (_db.ScheduleCalendarLinkTemps.Any(sl => sl.parent_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid))
                        {
                            objSCLinkTmpP = _db.ScheduleCalendarLinkTemps.Where(sl => sl.parent_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid).ToList();

                            foreach (ScheduleCalendarLinkTemp slT in objSCLinkTmpP)
                            {
                                ScheduleCalendarTemp ocjSCTempC = _db.ScheduleCalendarTemps.FirstOrDefault(s => s.event_id == slT.child_event_id);


                                nOffsetDays = (int)slT.lag;

                                if (slT.dependencyType == 1) // Start Same Time
                                {
                                    nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;
                                    DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);
                                    DateTime dtNewStart = Convert.ToDateTime(ocjSCTempUpdate.event_start);

                                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                    for (int i = 1; i <= nDuration; i++)
                                    {
                                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                                    }

                                    ocjSCTempC.event_start = dtNewStart;
                                    ocjSCTempC.event_end = dtNewEnd;
                                    ocjSCTempC.IsEWSCalendarSynch = false;
                                    _db.SubmitChanges();
                                }

                                if (slT.dependencyType == 2) // Start After Finish
                                {

                                    nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;
                                    DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);


                                    DateTime dtNewStart = GetWorkingDay(Convert.ToDateTime(ocjSCTempUpdate.event_end).AddDays(1));

                                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                    for (int i = 1; i <= nDuration; i++)
                                    {
                                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                                    }

                                    ocjSCTempC.event_start = dtNewStart;
                                    ocjSCTempC.event_end = dtNewEnd;
                                    ocjSCTempC.IsEWSCalendarSynch = false;
                                    _db.SubmitChanges();
                                }

                                if (slT.dependencyType == 3) // Offset days
                                {
                                    nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;


                                    DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);


                                    DateTime dtNewStart = Convert.ToDateTime(ocjSCTempUpdate.event_end);

                                    for (int i = 0; i <= nOffsetDays; i++)
                                    {
                                        dtNewStart = GetWorkingDay(dtNewStart.AddDays(1));

                                    }


                                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                    for (int i = 1; i <= nDuration; i++)
                                    {
                                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                                    }

                                    ocjSCTempC.event_start = dtNewStart;
                                    ocjSCTempC.event_end = dtNewEnd;
                                    ocjSCTempC.IsEWSCalendarSynch = false;
                                    _db.SubmitChanges();
                                }

                                updateAllLink((int)slT.child_event_id);
                            }
                        }

                        //If Event is Child
                        ScheduleCalendarLinkTemp objSCLinkTmpC = new ScheduleCalendarLinkTemp();
                        if (_db.ScheduleCalendarLinkTemps.Any(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid))
                        {
                            objSCLinkTmpC = _db.ScheduleCalendarLinkTemps.FirstOrDefault(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid);

                            ScheduleCalendarTemp ocjSCTempP = _db.ScheduleCalendarTemps.FirstOrDefault(s => s.event_id == objSCLinkTmpC.parent_event_id);

                            //DateTime pStartDate = Convert.ToDateTime(ocjSCTempP.event_start).Date;
                            //DateTime pEndDate = Convert.ToDateTime(ocjSCTempP.event_end).Date;

                            //DateTime cStartDate = Convert.ToDateTime(ocjSCTempUpdate.event_start).Date; // here ocjSCTempUpdate is now as child 

                            //new
                            DateTime pStartDate = Convert.ToDateTime(ocjSCTempP.event_start);
                            pStartDate = new DateTime(pStartDate.Year, pStartDate.Month, pStartDate.Day); // Parent Start

                            DateTime pEndDate = Convert.ToDateTime(ocjSCTempP.event_end);
                            pEndDate = new DateTime(pEndDate.Year, pEndDate.Month, pEndDate.Day); // Parent End

                            DateTime cStartDate = Convert.ToDateTime(ocjSCTempUpdate.event_start).Date;
                            cStartDate = new DateTime(cStartDate.Year, cStartDate.Month, cStartDate.Day); // Child Start

                            //new

                            int nlagDays = GetOffsetDays(pEndDate, cStartDate); // Convert.ToDateTime(ocjSCTempUpdate.event_start).Day - Convert.ToDateTime(ocjSCTempP.event_end).Day - 1;



                            if (pStartDate == cStartDate) // Start Same Time
                            {
                                objSCLinkTmpC.dependencyType = 1;
                                objSCLinkTmpC.lag = 0;
                                _db.SubmitChanges();
                            }
                            else if (nlagDays == 1) // Start After Finish
                            {
                                objSCLinkTmpC.dependencyType = 2;
                                objSCLinkTmpC.lag = 1;
                                _db.SubmitChanges();
                            }
                            else if (nlagDays > 1) // Offset Days
                            {
                                objSCLinkTmpC.dependencyType = 3;
                                objSCLinkTmpC.lag = nlagDays - 1;
                                _db.SubmitChanges();
                            }
                        }
                    }
                }
            }
            else
            {
                /////----------------------------------------------------------------------- Type ID 2, Sales ---------------------------------------------
                #region Google Calendar UPDATE (Type ID 2, Sales)------------
                try
                {
                    dtStart = DateTime.Parse(Convert.ToDateTime(start).ToShortDateString() + " " + Convert.ToDateTime(start).ToShortTimeString());
                    dtEnd = DateTime.Parse(Convert.ToDateTime(end).ToShortDateString() + " " + Convert.ToDateTime(end).ToShortTimeString());

                    string strCondition = "";

                    string calendarId = "";
                    int nCustomerId = 0;
                    int nSalesPersonID = 0;
                    var objSchCal = _db.ScheduleCalendars.SingleOrDefault(sc => sc.event_id == id);

                    nTypeId = Convert.ToInt32(objSchCal.type_id); // Type ID 2 = Sales // TypeID == 22 (SalesCalendar) // TypeID == 11 (OperationCalendar) 

                    if (nTypeId == 2)
                    {
                        var cust = _db.customers.SingleOrDefault(c => c.customer_id == objSchCal.customer_id);


                        if (cust != null)
                        {
                            nCustomerId = cust.customer_id;
                            nSalesPersonID = (int)cust.sales_person_id;

                            cust.notes = description;
                            cust.appointment_date = start;
                            _db.SubmitChanges();
                            strCondition = " AND customer_id=" + nCustomerId;
                        }

                        CustomerCallLog custCall = new CustomerCallLog();
                        if (objSchCal.estimate_id != 0)
                        {
                            custCall = _db.CustomerCallLogs.Single(c => c.customer_id == nCustomerId && c.CallTypeId == 3 && c.CallLogID == objSchCal.estimate_id);
                            custCall.AppointmentDateTime = start;
                            custCall.CallSubject = title.Replace("'", "''");
                            custCall.Description = description.Replace("'", "''");
                        }

                        _db.SubmitChanges();

                    }

                    string sql = "UPDATE ScheduleCalendar SET title='" + strTitle + "', section_name='" + strSection + "', location_name='" + location.Replace("'", "''") + "', description='" + description.Replace("'", "''") + "', " +
                               " cssClassName='" + cssClassName + "', event_start='" + dtStart + "', event_end='" + dtEnd + "',  duration='" + nDuration + "', employee_id='" + nEmployeeID + "',  employee_name='" + strEmployeeName + "', is_complete='" + Convert.ToInt32(IsComplete) + "', " +
                               " last_updated_by='" + strUName + "', last_updated_date='" + DateTime.Now + "', IsEWSCalendarSynch = '" + Convert.ToBoolean(0) + "' " +
                               " , weekends = '" + setWeekendDays(start, end) + "'" +
                               " , selectedweekends = '" + selectedWeekend + "'" +
                               " WHERE event_id=" + id + " AND type_id =" + nTypeId + strCondition;

                    _db.ExecuteCommand(sql, string.Empty);

                    //if (ConfigurationManager.AppSettings["IsProduction"].ToString() == "True")
                    //{

                    //    if (nTypeId == 2) // Type ID 2 = Sales
                    //    {
                    //        //Get calendarId by Sales Person ID
                    //        var objSP = _db.sales_persons.SingleOrDefault(sp => sp.sales_person_id == nSalesPersonID && sp.google_calendar_account != null && sp.google_calendar_account != "");

                    //        if (objSP != null)
                    //        {
                    //            calendarId = objSP.google_calendar_id ?? "";
                    //        }


                    //        if (calendarId != "" && (objSchCal.google_event_id ?? "") != "")
                    //        {
                    //            var calendarEvent = new gCalendarEvent()
                    //            {
                    //                CalendarId = calendarId,
                    //                Id = objSchCal.google_event_id,
                    //                StartDate = Convert.ToDateTime(objSchCal.event_start),
                    //                EndDate = Convert.ToDateTime(objSchCal.event_end),
                    //                Description = description
                    //            };

                    //            var authenticator = GetAuthenticator(nSalesPersonID); // Sales Persion ID
                    //            var service = new GoogleCalendarServiceProxy(authenticator);
                    //            service.UpdateEvent(calendarEvent);
                    //        }
                    //    }
                    //    else if (nTypeId == 22 || nTypeId == 11) // Schedule, TypeID == 22 (SalesCalendar) // TypeID == 11 (OperationCalendar)
                    //    {
                    //        calendarId = ConfigurationManager.AppSettings["GoogleScheduleCalendarID"];

                    //        objSchCal = _db.ScheduleCalendars.SingleOrDefault(sc => sc.type_id == nTypeId && sc.event_id == id);

                    //        if (calendarId != "" && objSchCal.google_event_id != "")
                    //        {
                    //            var calendarEvent = new gCalendarEvent()
                    //            {
                    //                CalendarId = calendarId,
                    //                Id = objSchCal.google_event_id,
                    //                StartDate = Convert.ToDateTime(objSchCal.event_start),
                    //                EndDate = Convert.ToDateTime(objSchCal.event_end),
                    //                Title = objSchCal.title.ToString(),
                    //                Description = description
                    //            };

                    //            var authenticator = GetAuthenticator(123456);// 123456 ID created for IICEM Schedule Entry
                    //            var service = new GoogleCalendarServiceProxy(authenticator);
                    //            service.UpdateEvent(calendarEvent);
                    //        }
                    //    }

                    //}



                }
                catch (Exception ex)
                {
                    throw ex;
                }
                #endregion
            }
        }




        return nCheck;
    }


    public static void updateAllLink(int id)
    {
        int ncid = 0;
        int neid = 0;
        if (System.Web.HttpContext.Current.Session["cid"] != null)
        {
            ncid = (int)System.Web.HttpContext.Current.Session["cid"];
        }
        if (System.Web.HttpContext.Current.Session["eid"] != null)
        {
            neid = (int)System.Web.HttpContext.Current.Session["eid"];
        }

        DataClassesDataContext _db = new DataClassesDataContext();

        ScheduleCalendarTemp ocjSCTempUpdate = _db.ScheduleCalendarTemps.SingleOrDefault(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid);
        //If Event is Parent
        List<ScheduleCalendarLinkTemp> objSCLinkTmpP = new List<ScheduleCalendarLinkTemp>();

        if (_db.ScheduleCalendarLinkTemps.Any(sl => sl.parent_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid))
        {
            objSCLinkTmpP = _db.ScheduleCalendarLinkTemps.Where(sl => sl.parent_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid).ToList();

            foreach (ScheduleCalendarLinkTemp slT in objSCLinkTmpP)
            {
                ScheduleCalendarTemp ocjSCTempC = _db.ScheduleCalendarTemps.FirstOrDefault(s => s.event_id == slT.child_event_id);

                //  int nDays = (Convert.ToDateTime(ocjSCTempC.event_end) - Convert.ToDateTime(ocjSCTempC.event_start)).Days;

                if (slT.dependencyType == 1) // Start Same Time
                {
                    int nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;
                    DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);
                    DateTime dtNewStart = Convert.ToDateTime(ocjSCTempUpdate.event_start);

                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }

                    ocjSCTempC.event_start = dtNewStart;
                    ocjSCTempC.event_end = dtNewEnd;
                    ocjSCTempC.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }

                if (slT.dependencyType == 2) // Start After Finish
                {
                    int nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;
                    DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);


                    DateTime dtNewStart = GetWorkingDay(Convert.ToDateTime(ocjSCTempUpdate.event_end).AddDays(1));

                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }


                    ocjSCTempC.event_start = dtNewStart;
                    ocjSCTempC.event_end = dtNewEnd;
                    ocjSCTempC.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }

                if (slT.dependencyType == 3) // Offset days
                {
                    int nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;


                    DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);


                    DateTime dtNewStart = Convert.ToDateTime(ocjSCTempUpdate.event_end);

                    for (int i = 0; i <= (int)slT.lag; i++)
                    {
                        dtNewStart = GetWorkingDay(dtNewStart.AddDays(1));

                    }


                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }


                    ocjSCTempC.event_start = dtNewStart;
                    ocjSCTempC.event_end = dtNewEnd;
                    ocjSCTempC.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }

                updateAllLink((int)slT.child_event_id);
            }
        }
    }

    public static void updateAllLinkOnline(int id)
    {
        int ncid = 0;
        int neid = 0;
        if (System.Web.HttpContext.Current.Session["cid"] != null)
        {
            ncid = (int)System.Web.HttpContext.Current.Session["cid"];
        }
        if (System.Web.HttpContext.Current.Session["eid"] != null)
        {
            neid = (int)System.Web.HttpContext.Current.Session["eid"];
        }

        DataClassesDataContext _db = new DataClassesDataContext();

        ScheduleCalendar ocjSCOnlinUpdate = _db.ScheduleCalendars.SingleOrDefault(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid);
        //If Event is Parent
        List<ScheduleCalendarLink> objSCLinkOnlinP = new List<ScheduleCalendarLink>();

        if (_db.ScheduleCalendarLinks.Any(sl => sl.parent_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid))
        {
            objSCLinkOnlinP = _db.ScheduleCalendarLinks.Where(sl => sl.parent_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid).ToList();

            foreach (ScheduleCalendarLink slT in objSCLinkOnlinP)
            {
                ScheduleCalendar ocjSCOnlinC = _db.ScheduleCalendars.FirstOrDefault(s => s.event_id == slT.child_event_id);

                //  int nDays = (Convert.ToDateTime(ocjSCOnlinC.event_end) - Convert.ToDateTime(ocjSCOnlinC.event_start)).Days;

                if (slT.dependencyType == 1) // Start Same Time
                {
                    int nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;
                    DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);
                    DateTime dtNewStart = Convert.ToDateTime(ocjSCOnlinUpdate.event_start);

                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }

                    ocjSCOnlinC.event_start = dtNewStart;
                    ocjSCOnlinC.event_end = dtNewEnd;
                    ocjSCOnlinC.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }

                if (slT.dependencyType == 2) // Start After Finish
                {
                    int nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;
                    DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);


                    DateTime dtNewStart = GetWorkingDay(Convert.ToDateTime(ocjSCOnlinUpdate.event_end).AddDays(1));

                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }


                    ocjSCOnlinC.event_start = dtNewStart;
                    ocjSCOnlinC.event_end = dtNewEnd;
                    ocjSCOnlinC.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }

                if (slT.dependencyType == 3) // Offset days
                {
                    int nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;


                    DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);


                    DateTime dtNewStart = Convert.ToDateTime(ocjSCOnlinUpdate.event_end);

                    for (int i = 0; i <= (int)slT.lag; i++)
                    {
                        dtNewStart = GetWorkingDay(dtNewStart.AddDays(1));

                    }


                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }


                    ocjSCOnlinC.event_start = dtNewStart;
                    ocjSCOnlinC.event_end = dtNewEnd;
                    ocjSCOnlinC.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }

                updateAllLinkOnline((int)slT.child_event_id);
            }
        }
    }

    //this method updates the event start and end time
    //this is called when a event is dragged or resized in the calendar
    public static int updateEventTime(int id, DateTime start, DateTime end, int customer_id, int estimate_id, bool IsScheduleDayException, string selectedWeekend)
    {


        int ncid = 0;
        int neid = 0;
        if (System.Web.HttpContext.Current.Session["cid"] != null)
        {
            ncid = (int)System.Web.HttpContext.Current.Session["cid"];
        }
        //if (System.Web.HttpContext.Current.Session["eid"] != null)
        //{
        //    neid = (int)System.Web.HttpContext.Current.Session["eid"];
        //}

        if (estimate_id != null)
            neid = estimate_id;

        int nCheck = 0;


        if (ncid == 0 && id == 0)
        {
            nCheck = 5;
            return nCheck;
        }

        DataClassesDataContext _db = new DataClassesDataContext();

        //customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == ncid);
        //if (objCust != null)
        //{
        //    if (objCust.isCalendarOnline == true)
        //    {
        //        nCheck = 2;
        //        return nCheck;
        //    }
        //}
        //else
        //{
        //    nCheck = 5;
        //    return nCheck;
        //}

        List<ScheduleCalendarTemp> objSchClndlist = new List<ScheduleCalendarTemp>();

        customer_estimate objCusEst = new customer_estimate();
        sales_person objSP = new sales_person();
        string operationCalendarId = string.Empty;
        operationCalendarId = ConfigurationManager.AppSettings["GoogleCalendarID"];
        int nTypeId = 0;

        if (System.Web.HttpContext.Current.Session["TypeID"] != null)
        {
            nTypeId = (int)System.Web.HttpContext.Current.Session["TypeID"];
            {
                if (nTypeId == 1)
                {
                    if (_db.ScheduleCalendarTemps.SingleOrDefault(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid).type_id == 1) // Type ID 1 = Operation
                    {
                        try
                        {
                            if (_db.ScheduleCalendarLinkTemps.Any(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid))
                            {
                                var objChildLink = _db.ScheduleCalendarLinkTemps.SingleOrDefault(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid);

                                var objP = _db.ScheduleCalendarTemps.SingleOrDefault(sc => sc.event_id == objChildLink.parent_event_id);

                                DateTime dtNewStartC = new DateTime(start.Year, start.Month, start.Day);
                                DateTime dtNewEndP = new DateTime(Convert.ToDateTime(objP.event_end).Year, Convert.ToDateTime(objP.event_end).Month, Convert.ToDateTime(objP.event_end).Day);

                                DateTime dtNewStartP = new DateTime(Convert.ToDateTime(objP.event_start).Year, Convert.ToDateTime(objP.event_start).Month, Convert.ToDateTime(objP.event_start).Day);

                                TimeSpan diffPrntEndToChldStart = dtNewStartC - dtNewEndP; // Parent End date to Child Start Date
                                TimeSpan diffPrnttStartToChldStart = dtNewStartC - dtNewStartP; // Parent Start date to Child Start Date

                                int nDaysPrntEndToChldStart = diffPrntEndToChldStart.Days;// Parent End date to Child Start Date
                                int nDaysPrnttStartToChldStart = diffPrnttStartToChldStart.Days;// Parent Start date to Child Start Date

                                if (nDaysPrntEndToChldStart > 0)// Parent End date to Child Start Date
                                {
                                    nCheck = 0;

                                }
                                else if (nDaysPrntEndToChldStart < 0)
                                {
                                    nCheck = nDaysPrntEndToChldStart;

                                    if (nDaysPrnttStartToChldStart == 0)// Parent Start date to Child Start Date
                                    {
                                        nCheck = 0;
                                    }
                                    else if (nDaysPrnttStartToChldStart > 0)// Parent Start date to Child Start Date
                                    {
                                        nCheck = -1;
                                    }
                                }
                                else if (nDaysPrntEndToChldStart == 0 && nDaysPrnttStartToChldStart == 0)
                                {
                                    nCheck = 0;
                                }
                                else
                                {
                                    nCheck = -1;
                                }


                                if (nCheck < 0)
                                {
                                    return nCheck;
                                }


                            }

                            ScheduleCalendarTemp ocjSCTempUpdate = _db.ScheduleCalendarTemps.Where(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid).FirstOrDefault();


                            int nDuration = Convert.ToInt32(ocjSCTempUpdate.duration) - 1;
                            DateTime dtOldStart = Convert.ToDateTime(ocjSCTempUpdate.event_start);
                            DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempUpdate.event_end);

                            DateTime dtNewStart = start;
                            DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                            dtNewEnd = dtNewEnd.AddDays(nDuration);

                            if (!IsScheduleDayException)
                            {
                                dtNewStart = GetWorkingDay(start);

                                dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                for (int i = 1; i <= nDuration; i++)
                                {
                                    dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));


                                }
                            }



                            DateTime tempDate = DateTime.Today;

                            ocjSCTempUpdate.event_start = dtNewStart;
                            ocjSCTempUpdate.event_end = dtNewEnd;
                            ocjSCTempUpdate.IsScheduleDayException = IsScheduleDayException;
                            ocjSCTempUpdate.IsEWSCalendarSynch = false;
                            _db.SubmitChanges();
                            if (!IsScheduleDayException)
                            {
                                //If Event is Parent
                                List<ScheduleCalendarLinkTemp> objSCLinkTmpP = new List<ScheduleCalendarLinkTemp>();

                                if (_db.ScheduleCalendarLinkTemps.Any(sl => sl.parent_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid))
                                {
                                    objSCLinkTmpP = _db.ScheduleCalendarLinkTemps.Where(sl => sl.parent_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid).ToList();

                                    foreach (ScheduleCalendarLinkTemp slT in objSCLinkTmpP)
                                    {
                                        ScheduleCalendarTemp ocjSCTempC = _db.ScheduleCalendarTemps.FirstOrDefault(s => s.event_id == slT.child_event_id);




                                        int nLagDays = (int)slT.lag;// Convert.ToDateTime(ocjSCTempC.event_start).Day - Convert.ToDateTime(ocjSCTempUpdate.event_end).Day;

                                        if (slT.dependencyType == 1) // Start Same Time
                                        {

                                            int nChildDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;
                                            DateTime dtChildOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                                            DateTime dtChildOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);
                                            DateTime dtChildNewStart = GetWorkingDay(start);

                                            DateTime dtChildNewEnd = new DateTime(dtChildNewStart.Year, dtChildNewStart.Month, dtChildNewStart.Day, dtChildOldEnd.Hour, dtChildOldEnd.Minute, dtChildOldEnd.Second);
                                            for (int i = 1; i <= nChildDuration; i++)
                                            {
                                                dtChildNewEnd = GetWorkingDay(dtChildNewEnd.AddDays(1));
                                            }

                                            ocjSCTempC.event_start = dtChildNewStart;
                                            ocjSCTempC.event_end = dtChildNewEnd;
                                            ocjSCTempC.IsEWSCalendarSynch = false;
                                            _db.SubmitChanges();
                                        }

                                        if (slT.dependencyType == 2) // Start After Finish
                                        {



                                            tempDate = GetWorkingDay(dtNewEnd.AddDays(1));
                                            dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);


                                            ocjSCTempC.event_start = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);

                                            nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;

                                            dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);

                                            DateTime dtChildNewEnd = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);

                                            for (int i = 1; i <= nDuration; i++)
                                            {
                                                dtChildNewEnd = GetWorkingDay(dtChildNewEnd.AddDays(1));

                                            }

                                            ocjSCTempC.event_end = dtChildNewEnd;
                                            ocjSCTempC.IsEWSCalendarSynch = false;
                                            _db.SubmitChanges();
                                        }
                                        if (slT.dependencyType == 3) // Offset Days
                                        {

                                            nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;


                                            dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                                            dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);


                                            DateTime dtChildNewStart = Convert.ToDateTime(ocjSCTempUpdate.event_end);

                                            for (int i = 0; i <= nLagDays; i++)
                                            {
                                                dtChildNewStart = GetWorkingDay(dtChildNewStart.AddDays(1));

                                            }


                                            dtChildNewStart = new DateTime(dtChildNewStart.Year, dtChildNewStart.Month, dtChildNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                            DateTime dtChildNewEnd = new DateTime(dtChildNewStart.Year, dtChildNewStart.Month, dtChildNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                            for (int i = 1; i <= nDuration; i++)
                                            {
                                                dtChildNewEnd = GetWorkingDay(dtChildNewEnd.AddDays(1));

                                            }

                                            ocjSCTempC.event_start = dtChildNewStart;

                                            ocjSCTempC.event_end = dtChildNewEnd;
                                            ocjSCTempC.IsEWSCalendarSynch = false;
                                            _db.SubmitChanges();


                                        }

                                        updateAllLink((int)slT.child_event_id);
                                    }
                                }

                                //If Event is Child
                                ScheduleCalendarLinkTemp objSCLinkTmpC = new ScheduleCalendarLinkTemp();
                                if (_db.ScheduleCalendarLinkTemps.Any(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid))
                                {
                                    objSCLinkTmpC = _db.ScheduleCalendarLinkTemps.FirstOrDefault(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid);

                                    ScheduleCalendarTemp ocjSCTempP = _db.ScheduleCalendarTemps.FirstOrDefault(s => s.event_id == objSCLinkTmpC.parent_event_id);



                                    DateTime pStartDate = Convert.ToDateTime(ocjSCTempP.event_start);
                                    pStartDate = new DateTime(pStartDate.Year, pStartDate.Month, pStartDate.Day); // Parent Start

                                    DateTime pEndDate = Convert.ToDateTime(ocjSCTempP.event_end);
                                    pEndDate = new DateTime(pEndDate.Year, pEndDate.Month, pEndDate.Day); // Parent End

                                    DateTime cStartDate = Convert.ToDateTime(ocjSCTempUpdate.event_start).Date;
                                    cStartDate = new DateTime(cStartDate.Year, cStartDate.Month, cStartDate.Day); // Child Start



                                    int nlagDays = GetOffsetDays(pEndDate, cStartDate);

                                    if (pStartDate == cStartDate) // Start Same Time
                                    {
                                        objSCLinkTmpC.dependencyType = 1;
                                        objSCLinkTmpC.lag = 0;
                                        _db.SubmitChanges();
                                    }
                                    else if (nlagDays == 1) // Start After Finish
                                    {
                                        objSCLinkTmpC.dependencyType = 2;
                                        objSCLinkTmpC.lag = 1;
                                        _db.SubmitChanges();
                                    }

                                    else if (nlagDays > 1)// Offset Days
                                    {
                                        objSCLinkTmpC.dependencyType = 3;
                                        objSCLinkTmpC.lag = nlagDays - 1;
                                        _db.SubmitChanges();
                                    }
                                }



                                #region Blocked  Google Calendar UPDATE (Type ID 1, Operation)------------
                                //try
                                //{
                                //    if (ConfigurationManager.AppSettings["IsProductionOpeartion"].ToString() == "True")
                                //    {
                                //        var scItem = _db.ScheduleCalendarTemps.Single(sc => sc.customer_id == customer_id && sc.estimate_id == estimate_id && sc.event_id == id);

                                //        var calendarEvent = new gCalendarEvent()
                                //            {
                                //                CalendarId = operationCalendarId,
                                //                Title = scItem.title.Trim(),
                                //                Id = scItem.google_event_id,
                                //                StartDate = Convert.ToDateTime(start),
                                //                EndDate = Convert.ToDateTime(end),
                                //                Description = scItem.description.Trim()
                                //            };

                                //        // var authenticator = GetAuthenticator(Convert.ToInt32(objCusEst.sales_person_id));
                                //        var authenticator = GetAuthenticator(6);
                                //        var service = new GoogleCalendarServiceProxy(authenticator);

                                //        if (scItem.google_event_id != "")
                                //        {
                                //            service.UpdateEvent(calendarEvent);
                                //        }
                                //        else
                                //        {

                                //            string strGoogleEventId = service.CreateEvent(calendarEvent);

                                //            string sql2 = "UPDATE ScheduleCalendarTemp SET  google_event_id ='" + strGoogleEventId + "' WHERE event_id=" + scItem.event_id;
                                //            _db.ExecuteCommand(sql2, string.Empty);
                                //        }
                                //    }
                                //}
                                //catch (Exception ex)
                                //{
                                //    throw ex;
                                //}
                                #endregion

                                // Update "week_id" in co_pricing_master ------------
                                //List<co_pricing_master> objcpmList = new List<co_pricing_master>();

                                //List<ScheduleCalendarTemp> objSCLList = new List<ScheduleCalendarTemp>();
                                //objSCLList = _db.ScheduleCalendarTemps.Where(sc => sc.customer_id == customer_id && sc.estimate_id == estimate_id && sc.event_id == id).ToList();

                                //foreach (ScheduleCalendarTemp objSC in objSCLList)
                                //{
                                //    objcpmList = _db.co_pricing_masters.Where(c => c.customer_id == objSC.customer_id && c.estimate_id == objSC.estimate_id
                                //        && c.section_name == objSC.section_name && c.CalEventId == objSC.event_id).ToList();

                                //    foreach (co_pricing_master objcpm in objcpmList)
                                //    {
                                //        objcpm.week_id = objSC.week_id;
                                //        _db.SubmitChanges();
                                //    }
                                //}
                                //-------------------------------------------
                            }
                        }
                        catch (Exception ex)
                        {
                            nCheck = -1;
                            string msg = ex.StackTrace;
                        }
                    }
                }
                else
                {

                    var objSC = _db.ScheduleCalendars.SingleOrDefault(sc => sc.event_id == id);

                    nTypeId = Convert.ToInt32(objSC.type_id); // Type ID 2 = Sales // TypeID == 22 (SalesCalendar) // TypeID == 11 (OperationCalendar) 

                    string strCondition = "";
                    int nCustomerId = 0;
                    int nSalesPersonID = 0;
                    string calendarId = string.Empty;

                    if (nTypeId == 2)
                    {
                        var cust = _db.customers.SingleOrDefault(c => c.customer_id == objSC.customer_id);

                        if (cust != null)
                        {
                            nCustomerId = cust.customer_id;

                            strCondition = " AND customer_id =" + nCustomerId;

                            nSalesPersonID = (int)cust.sales_person_id;
                            cust.appointment_date = start;

                        }



                        CustomerCallLog custCall = new CustomerCallLog();
                        if (estimate_id != 0)
                        {
                            custCall = _db.CustomerCallLogs.Single(c => c.customer_id == objSC.customer_id && c.CallTypeId == 3 && c.CallLogID == objSC.estimate_id);
                            custCall.AppointmentDateTime = start;
                        }

                        _db.SubmitChanges();
                    }

                    string sql = "UPDATE ScheduleCalendar SET  event_start='" + start + "', event_end='" + end + "', IsEWSCalendarSynch = '" + Convert.ToBoolean(0) + "', " +
                         " weekends = '" + setWeekendDays(start, end) + "', " +
                         " selectedweekends = '" + selectedWeekend + "'" +
                        "' WHERE event_id=" + id + " AND type_id = " + nTypeId + strCondition;
                    _db.ExecuteCommand(sql, string.Empty);

                    #region Blocked  Google Calendar UPDATE (Type ID 2, Sales)------------
                    if (ConfigurationManager.AppSettings["IsProduction"].ToString() == "True")
                    {
                        if (nTypeId == 2) // Type ID 2 = Sales
                        {


                            var objSalesPerson = _db.sales_persons.SingleOrDefault(sp => sp.sales_person_id == nSalesPersonID && sp.google_calendar_account != null && sp.google_calendar_account != "");

                            if (objSalesPerson != null)
                            {
                                calendarId = objSalesPerson.google_calendar_id ?? "";
                            }

                            if (calendarId != "" && objSC.google_event_id != "")
                            {
                                var calendarEvent = new gCalendarEvent()
                                {
                                    CalendarId = calendarId,
                                    Title = objSC.title.Trim(),
                                    Id = objSC.google_event_id,
                                    StartDate = Convert.ToDateTime(start),
                                    EndDate = Convert.ToDateTime(end),
                                    Description = objSC.description
                                };

                                var authenticator = GetAuthenticator(nSalesPersonID); // Sales Persion ID
                                var service = new GoogleCalendarServiceProxy(authenticator);

                                service.UpdateEvent(calendarEvent);

                            }
                        }
                        else if (nTypeId == 22 || nTypeId == 11) // Schedule, TypeID == 22 (SalesCalendar) // TypeID == 11 (OperationCalendar)
                        {

                            calendarId = ConfigurationManager.AppSettings["GoogleScheduleCalendarID"];

                            var scItem = _db.ScheduleCalendars.SingleOrDefault(sc => sc.type_id == nTypeId && sc.event_id == id);

                            if (calendarId != "" && scItem.google_event_id != "")
                            {
                                var calendarEvent = new gCalendarEvent()
                                {
                                    CalendarId = calendarId,
                                    Id = scItem.google_event_id,
                                    StartDate = Convert.ToDateTime(start),
                                    EndDate = Convert.ToDateTime(start),
                                    Title = scItem.title.ToString(),
                                    Description = scItem.description
                                };

                                var authenticator = GetAuthenticator(123456);// 123456 ID created for IICEM Schedule Entry
                                var service = new GoogleCalendarServiceProxy(authenticator);
                                service.UpdateEvent(calendarEvent);
                            }
                        }
                    }

                    #endregion

                }
            }
        }

        return nCheck;
    }

    //this method updates the All event start and end time
    //this is called when a event is dragged or resized in the calendar
    public static void updateEventTimeAll(int id, DateTime start, DateTime end, int customer_id, int estimate_id)
    {
        int ncid = 0;
        int neid = 0;
        if (System.Web.HttpContext.Current.Session["cid"] != null)
        {
            ncid = (int)System.Web.HttpContext.Current.Session["cid"];
        }
        if (System.Web.HttpContext.Current.Session["eid"] != null)
        {
            neid = (int)System.Web.HttpContext.Current.Session["eid"];
        }
        DataClassesDataContext _db = new DataClassesDataContext();

        List<ScheduleCalendarTemp> objSchClndlist = new List<ScheduleCalendarTemp>();

        if (_db.ScheduleCalendarTemps.SingleOrDefault(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid).type_id != 1)
        {
            string sql = "UPDATE ScheduleCalendarTemp SET  event_start='" + start + "', event_end='" + end + "', IsEWSCalendarSynch = '" + Convert.ToBoolean(0) + "', " +
                " weekends = '" + setWeekendDays(start, end) + "', " +
                " selectedweekends = ''" +
                " WHERE event_id=" + id + " AND customer_id =" + ncid + " AND estimate_id = " + neid;
            _db.ExecuteCommand(sql, string.Empty);
        }
        else // Type ID 1 = Operation
        {
            try
            {
                var selected_event_Start = _db.ScheduleCalendarTemps.FirstOrDefault(sc => sc.customer_id == customer_id && sc.estimate_id == estimate_id && sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid).event_start;

                objSchClndlist = _db.ScheduleCalendarTemps.Where(sc => sc.customer_id == customer_id && sc.estimate_id == estimate_id && sc.event_start >= selected_event_Start).OrderBy(sc => sc.event_start).ToList();

                string dtStart = Convert.ToDateTime(start).ToShortDateString();

                // int nDays = start.Day - Convert.ToDateTime(selected_event_Start).Day;
                int nDays = (start.Date - Convert.ToDateTime(selected_event_Start)).Days;

                customer_estimate objCusEst = new customer_estimate();
                sales_person objSP = new sales_person();
                //string calendarId = string.Empty;
                string operationCalendarId = string.Empty;
                operationCalendarId = ConfigurationManager.AppSettings["GoogleCalendarID"];
                //objCusEst = _db.customer_estimates.Single(ce => ce.customer_id == customer_id && ce.estimate_id == estimate_id);
                //if (_db.sales_persons.Where(sp => sp.sales_person_id == Convert.ToInt32(objCusEst.sales_person_id) && sp.google_calendar_account != null && sp.google_calendar_account != "").Count() > 0)
                //{
                //    objSP = _db.sales_persons.Where(sp => sp.sales_person_id == Convert.ToInt32(objCusEst.sales_person_id) && sp.google_calendar_account != null && sp.google_calendar_account != "").SingleOrDefault();
                //    calendarId = objSP.google_calendar_id;
                //}
                int nt = 0;
                foreach (ScheduleCalendarTemp objsc in objSchClndlist)
                {
                    //if (nDays < 0)
                    //    nt = nDays;
                    //----Old
                    //objsc.event_start = DateTime.Parse(GetDayOfWeek(Convert.ToDateTime(objsc.event_start).AddDays(nDays).ToShortDateString()) + " " + "12:00:00 AM");
                    //objsc.event_end = Convert.ToDateTime(objsc.event_start);
                    //-------
                    objsc.event_start = DateTime.Parse(GetDayOfWeek(Convert.ToDateTime(objsc.event_start).AddDays(nDays).ToShortDateString()) + " " + Convert.ToDateTime(objsc.event_start).ToShortTimeString());
                    int nDateDifference = (Convert.ToDateTime(objsc.event_end) - Convert.ToDateTime(objsc.event_start)).Days;
                    DateTime dtEndDate = Convert.ToDateTime(objsc.event_start).AddDays(nDateDifference).AddDays(nDays);
                    // objsc.event_end = DateTime.Parse(GetDayOfWeek(Convert.ToDateTime(dtEndDate).ToShortDateString()) + " " + Convert.ToDateTime(dtEndDate).ToShortTimeString());
                    objsc.event_end = dtEndDate;
                    objsc.last_updated_date = DateTime.Now;
                    //int nWk = GetWeek(Convert.ToDateTime(objsc.event_start), Convert.ToDateTime(objsc.job_start_date));
                    //objsc.week_id = nWk;

                    //objsc.event_start = DateTime.Parse(GetDayOfWeek(dtStart) + " " + "06:00:00.000");
                    //dtStart = Convert.ToDateTime(dtStart).AddDays(7).ToShortDateString();
                    //objsc.event_end = Convert.ToDateTime(objsc.event_start).AddHours(1);
                    //objsc.last_updated_date = DateTime.Now;
                    //int nWk = GetWeek(Convert.ToDateTime(objsc.event_start), Convert.ToDateTime(objsc.job_start_date));
                    //objsc.week_id = nWk;

                    #region Blocked  Google Calendar UPDATE (Type ID 1, Operation)------------
                    //try
                    //{
                    //    if (ConfigurationManager.AppSettings["IsProductionOpeartion"].ToString() == "True")
                    //    {
                    //        var calendarEvent = new gCalendarEvent()
                    //        {
                    //            CalendarId = operationCalendarId,
                    //            Id = objsc.google_event_id,
                    //            Title = objsc.title.Trim(),
                    //            StartDate = Convert.ToDateTime(objsc.event_start),
                    //            EndDate = Convert.ToDateTime(objsc.event_end),
                    //            Description = objsc.description.Trim()
                    //        };

                    //        // var authenticator = GetAuthenticator(Convert.ToInt32(objCusEst.sales_person_id));
                    //        var authenticator = GetAuthenticator(6);
                    //        var service = new GoogleCalendarServiceProxy(authenticator);

                    //        if (objsc.google_event_id != "")
                    //        {
                    //            service.UpdateEvent(calendarEvent);
                    //        }
                    //        else
                    //        {
                    //            objsc.google_event_id = service.CreateEvent(calendarEvent);
                    //            //string sql2 = "UPDATE ScheduleCalendarTemp SET  google_event_id ='" + service.CreateEvent(calendarEvent) + "' WHERE event_id=" + objsc.event_id;
                    //            //_db.ExecuteCommand(sql2, string.Empty);
                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    throw ex;
                    //}
                    #endregion

                    _db.SubmitChanges();

                }

                // Update "week_id" in co_pricing_master ------------
                //List<co_pricing_master> objcpmList = new List<co_pricing_master>();

                //List<ScheduleCalendarTemp> objSCLList = new List<ScheduleCalendarTemp>();
                //objSCLList = _db.ScheduleCalendarTemps.Where(sc => sc.customer_id == customer_id && sc.estimate_id == estimate_id).ToList();

                //foreach (ScheduleCalendarTemp objSC in objSCLList)
                //{
                //    objcpmList = _db.co_pricing_masters.Where(c => c.customer_id == objSC.customer_id && c.estimate_id == objSC.estimate_id
                //        && c.section_name == objSC.section_name && c.CalEventId == objSC.event_id).ToList();

                //    foreach (co_pricing_master objcpm in objcpmList)
                //    {
                //        objcpm.week_id = objSC.week_id;
                //        _db.SubmitChanges();
                //    }
                //}
                //-------------------------------------------
            }
            catch (Exception ex)
            {
                string msg = ex.StackTrace;
            }
        }
    }

    public static void UpdateEventNotes(int id, DateTime start, DateTime end, string operation_notes)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        int ncid = 0;
        int neid = 0;
        if (System.Web.HttpContext.Current.Session["cid"] != null)
        {
            ncid = (int)System.Web.HttpContext.Current.Session["cid"];
        }
        if (System.Web.HttpContext.Current.Session["eid"] != null)
        {
            neid = (int)System.Web.HttpContext.Current.Session["eid"];
        }

        string sql = "UPDATE ScheduleCalendarTemp SET event_start='" + start + "', event_end='" + end + "',  operation_notes='" + operation_notes + "', IsEWSCalendarSynch = '" + Convert.ToBoolean(0) + "', " +
            " weekends = '" + setWeekendDays(start, end) + "', " +
            " selectedweekends = ''" +
            " WHERE event_id=" + id + " AND customer_id =" + ncid + " AND estimate_id = " + neid;

    }

    public static int GetWeek(DateTime Event_Start, DateTime Job_Start)
    {
        TimeSpan t = Event_Start - Job_Start;
        var dt1 = t.Days + 1;
        int wk = 1;
        int nwk = 0;
        for (int i = 7; i < dt1; i += 7)
        {
            if (dt1 <= i)
            {
                break;
            }
            wk++;
        }
        nwk = wk;
        return nwk;
    }

    //this mehtod deletes event with the id passed in.
    public static void deleteEvent(int id)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        try
        {
            int ncid = 0;
            int neid = 0;
            if (System.Web.HttpContext.Current.Session["cid"] != null)
            {
                ncid = (int)System.Web.HttpContext.Current.Session["cid"];
            }
            if (System.Web.HttpContext.Current.Session["eid"] != null)
            {
                neid = (int)System.Web.HttpContext.Current.Session["eid"];
            }
            //---------For  Google Calendar & CallLog------------
            int nTypeID = 0;
            int nCustomerID = 0;
            int nCallLogID = 0;
            int nSalesPersonID = 0;

            DateTime dt = Convert.ToDateTime("1900-01-01");

            string strUName = "";
            DateTime dtDeletedBy = DateTime.Now;
            if (System.Web.HttpContext.Current.Session["uname"] != null)
            {
                strUName = (string)System.Web.HttpContext.Current.Session["uname"];
            }
           
            ScheduleCalendar objSchClnd = _db.ScheduleCalendars.SingleOrDefault(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid && sc.type_id == 2);

          
            if (objSchClnd != null)
            {
                nTypeID = (int)objSchClnd.type_id;
                nCustomerID = (int)objSchClnd.customer_id;
                nCallLogID = (int)objSchClnd.estimate_id;              
            }

            string strSclTBL = "";
            string deletedFrom = "";
            if(_db.ScheduleCalendars.Any(sc => sc.event_id == id && sc.customer_id == ncid))
            {
                strSclTBL = "ScheduleCalendar";
                deletedFrom = "Online";
            }
            else  if(_db.ScheduleCalendarTemps.Any(sc => sc.event_id == id && sc.customer_id == ncid))
            {
                strSclTBL = "ScheduleCalendarTemp";
                deletedFrom = "Offline";
            }

            if (strSclTBL.Length > 0)
            {
                //Insert
                string sSQLINSERT = "INSERT INTO [DeletedSchedule] " +
                                " SELECT [title],[description],[event_start],[event_end],[customer_id],[estimate_id],[employee_id],[section_name], " +
                                " [location_name],[create_date],[last_updated_date],[last_updated_by],[type_id],[parent_id],[job_start_date], " +
                                " [co_pricing_list_id],[cssClassName],[google_event_id],[operation_notes],[is_complete],[IsEstimateActive],[employee_name],[event_id], [duration], [IsScheduleDayException], " +
                                " [IsEWSCalendarSynch], [auto_event_id] AS [ScheduleCalendar_id], '" + deletedFrom + "' AS [deletedFrom], '" + strUName + "' AS deletedBy,  CONVERT(DATETIME, '" + dtDeletedBy + "', 101) AS deletedDate, CAST (0 AS bit) as IsDeletedSync " +
                                " FROM " + strSclTBL + " " +
                                " WHERE [customer_id] = " + ncid + " AND event_id =" + id;

                _db.ExecuteCommand(sSQLINSERT, string.Empty);
            }

            #region Blocked  Google Calendar DELETE
            //--------------------------------------------------------------------
            //Google Calendar
            string calendarId = string.Empty;
            // calendarId = ConfigurationManager.AppSettings["GoogleCalendarID"];

            //if (ConfigurationManager.AppSettings["IsProduction"].ToString() == "True")
            //{
            //    if (_db.customers.Where(sp => sp.customer_id == nCustomerID).Count() > 0)
            //    {
            //        objcust = _db.customers.Where(c => c.customer_id == nCustomerID).SingleOrDefault();
            //        nSalesPersonID = (int)objcust.sales_person_id;
            //    }

            //    if (_db.sales_persons.Where(sp => sp.sales_person_id == nSalesPersonID && sp.google_calendar_account != null && sp.google_calendar_account != "").Count() > 0)
            //    {
            //        objSP = _db.sales_persons.Where(sp => sp.sales_person_id == nSalesPersonID && sp.google_calendar_account != null && sp.google_calendar_account != "").SingleOrDefault();
            //        calendarId = objSP.google_calendar_id;
            //    }

            //    if (calendarId != "")
            //    {
            //        List<ScheduleCalendarTemp> sclist = _db.ScheduleCalendarTemps.Where(sc => sc.event_id == id && sc.type_id == 2).ToList();
            //        foreach (ScheduleCalendarTemp sc in sclist)
            //        {
            //            if (sc.google_event_id != "")
            //            {
            //                var authenticator = GetAuthenticator(objSP.sales_person_id);
            //                var service = new GoogleCalendarServiceProxy(authenticator);
            //                service.DeleteEvent(calendarId, sc.google_event_id); // Delete
            //            }
            //        }
            //    }
            //}
            #endregion


            string sqlTemp = "DELETE ScheduleCalendarTemp WHERE event_id=" + id;
            _db.ExecuteCommand(sqlTemp, string.Empty);

            string sqlTempLinkp = "DELETE ScheduleCalendarLinkTemp WHERE parent_event_id=" + id;
            _db.ExecuteCommand(sqlTempLinkp, string.Empty);

            string sqlTempLinkc = "DELETE ScheduleCalendarLinkTemp WHERE child_event_id=" + id;
            _db.ExecuteCommand(sqlTempLinkc, string.Empty);


            string sql1 = "DELETE ScheduleCalendar WHERE event_id=" + id;
            _db.ExecuteCommand(sql1, string.Empty);

            string sqlLinkp = "DELETE ScheduleCalendarLink WHERE parent_event_id=" + id;
            _db.ExecuteCommand(sqlLinkp, string.Empty);

            string sqlLinkc = "DELETE ScheduleCalendarLink WHERE child_event_id=" + id;
            _db.ExecuteCommand(sqlLinkc, string.Empty);

            string sql3 = "DELETE CustomerCallLog WHERE CallLogID=" + nCallLogID;
            _db.ExecuteCommand(sql3, string.Empty);

            string sql2 = "UPDATE customers SET appointment_date='" + dt + "' WHERE customer_id=" + nCustomerID;
            _db.ExecuteCommand(sql2, string.Empty);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //this mehtod deletes event with the id passed in.
    public static void cancelEvent()
    {
        //DataClassesDataContext _db = new DataClassesDataContext();

        //ScheduleCalendarTemp objSC = new ScheduleCalendarTemp();
        //Customer objCust = new Customer();
        //ServiceCall objServiceCall = new ServiceCall();

        //string strSerTktNumber = "0";
        //int nCustomerID = 0;
        //int nServiceCallId = 0;

        //string sql2 = "";

        //strSerTktNumber = (string)System.Web.HttpContext.Current.Session["STktNo"];
        //nCustomerID = (int)System.Web.HttpContext.Current.Session["cid"];
        //nServiceCallId = (int)System.Web.HttpContext.Current.Session["scallid"];

        //if (_db.ServiceTickets.Where(s => s.CustomerID == nCustomerID && s.SerTktNumber == strSerTktNumber && s.ServiceCallId == nServiceCallId).Count() == 0)
        //{
        //    sql2 = "DELETE ServiceCall WHERE CustomerID=" + nCustomerID + " AND ServiceCallId=" + nServiceCallId + " AND CallNumber ='" + strSerTktNumber + "'";
        //    _db.ExecuteCommand(sql2, string.Empty);
        //}
    }

    //this method adds events to the database
    public static int addEvent(CalendarEvent cevent)
    {
        try
        {
            //add event to the database and return the primary key of the added event row
            int key = 0;
            DataClassesDataContext _db = new DataClassesDataContext();
            ScheduleCalendar objSC = new ScheduleCalendar();
            ScheduleCalendarTemp objSCTemp = new ScheduleCalendarTemp();
            co_pricing_master objCOPM = new co_pricing_master();
            customer_estimate cus_est = new customer_estimate();
            location objLocation = new location();

            int nCustomerID = 0;
            int nEstimateID = 0;
            int nEmployeeID = 0;
            if (cevent.employee_id != null)
                nEmployeeID = cevent.employee_id;

            string strEmployeeName = "";
            if (cevent.employee_name != null)
                strEmployeeName = cevent.employee_name;

            bool IsScheduleDayException = cevent.IsScheduleDayException;
            int nTypeId = 0;
            int nPricingId = 0;
            string strSectionName = "";
            string StrLocationName = "";


            DateTime dtNewStart = cevent.start;
            DateTime dtNewEnd = cevent.end;

            if (!IsScheduleDayException)
            {
                DateTime dtTempStart = new DateTime(cevent.start.Year, cevent.start.Month, cevent.start.Day);
                dtNewStart = GetWorkingDay(dtTempStart);
                dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, cevent.start.Hour, cevent.start.Minute, cevent.start.Second);

                DateTime dtTempEnd = new DateTime(cevent.end.Year, cevent.end.Month, cevent.end.Day);
                dtNewEnd = GetWorkingDay(dtTempEnd);
                dtNewEnd = new DateTime(dtNewEnd.Year, dtNewEnd.Month, dtNewEnd.Day, cevent.end.Hour, cevent.end.Minute, cevent.end.Second);

            }

            DateTime dtJobStartDate = dtNewStart;// cevent.start;
            DateTime dtStartDate = dtNewStart;// cevent.start;
            DateTime dtEndDate = dtNewEnd;// cevent.end;
            string strUName = "";
            string strTitle = cevent.section_name;
            string strCssClassName = "fc-default";
            string GoogleEventID = "";
            string strCreatedBy = "";

            // For Operation Calendar
            if (System.Web.HttpContext.Current.Session["TypeID"] != null)
            {
                nTypeId = (int)System.Web.HttpContext.Current.Session["TypeID"];
                if (nTypeId == 1) // Operation Calendar
                {
                    if (System.Web.HttpContext.Current.Session["CusId"] != null)
                    {
                        nCustomerID = (int)System.Web.HttpContext.Current.Session["CusId"];

                        if (System.Web.HttpContext.Current.Session["sEstSelectedByCustSearch"] != null)
                            nEstimateID = (int)System.Web.HttpContext.Current.Session["sEstSelectedByCustSearch"];

                        nTypeId = 11; // Opration Extra event TypeID for Holidays Date event (Operation Calendar) with Customer ID
                    }
                }
            }


            if (System.Web.HttpContext.Current.Session["uname"] != null)
            {
                strUName = (string)System.Web.HttpContext.Current.Session["uname"];
            }
            if (cevent.cssClassName != null)
            {
                strCssClassName = cevent.cssClassName;
            }

            if (System.Web.HttpContext.Current.Session["cid"] != null)
            {
                nCustomerID = (int)System.Web.HttpContext.Current.Session["cid"];





                dtStartDate = DateTime.Parse(dtStartDate.ToShortDateString() + " " + dtStartDate.ToShortTimeString());
                dtEndDate = DateTime.Parse(dtEndDate.ToShortDateString() + " " + dtEndDate.ToShortTimeString());
                if (System.Web.HttpContext.Current.Session["TypeID"] != null)
                {
                    nTypeId = (int)System.Web.HttpContext.Current.Session["TypeID"];
                }
            }
            if (cevent.estimate_id != null)
            {
                nEstimateID = cevent.estimate_id;
                if (nEstimateID == 0)
                {
                    if (System.Web.HttpContext.Current.Session["eid"] != null)
                        nEstimateID = (int)System.Web.HttpContext.Current.Session["eid"];
                }
            }
            //if (System.Web.HttpContext.Current.Session["empid"] != null)
            //{
            //    nEmployeeID = (int)System.Web.HttpContext.Current.Session["empid"];
            //}
            if (System.Web.HttpContext.Current.Session["sCreatedBy"] != null)
            {
                strCreatedBy = System.Web.HttpContext.Current.Session["sCreatedBy"].ToString();
            }

            customer objcust = new customer();
            sales_person objSP = new sales_person();
            string strCustInfo = "";
            if (nTypeId == 2 && nCustomerID != 0)// Type ID 2 = Sales
            {

                CustomerCallLog custCall = new CustomerCallLog();
                if (nEstimateID != 0)
                {
                    custCall = _db.CustomerCallLogs.Single(c => c.customer_id == nCustomerID && c.CallTypeId == 3 && c.CallLogID == nEstimateID);
                }
                string strFollowupDate = "1900-01-01 00:00:00.000";

                custCall.CallSubject = cevent.title;
                custCall.Description = cevent.description;
                custCall.AppointmentDateTime = dtStartDate;



                if (nEstimateID == 0)
                {
                    custCall.customer_id = nCustomerID;
                    custCall.CallDate = DateTime.Today.ToShortDateString();
                    custCall.CallHour = DateTime.Now.ToString("hh", CultureInfo.InvariantCulture);
                    custCall.CallMinutes = DateTime.Now.ToString("mm", CultureInfo.InvariantCulture);
                    custCall.CallAMPM = DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);

                    custCall.CallDuration = "0";
                    custCall.DurationHour = "0";

                    string strCallDateTime = custCall.CallDate + " " + custCall.CallHour + ":" + custCall.CallMinutes + " " + custCall.CallAMPM;

                    custCall.DurationMinutes = "0";

                    custCall.CreatedByUser = strCreatedBy;
                    custCall.CreateDate = Convert.ToDateTime(DateTime.Now);
                    custCall.CallDateTime = Convert.ToDateTime(strCallDateTime);

                    custCall.CallTypeId = 3;
                    custCall.IsFollowUp = false;
                    custCall.FollowDate = Convert.ToDateTime(strFollowupDate).ToShortDateString();
                    custCall.FollowHour = Convert.ToDateTime(strFollowupDate).ToString("hh", CultureInfo.InvariantCulture);
                    custCall.FollowMinutes = Convert.ToDateTime(strFollowupDate).ToString("mm", CultureInfo.InvariantCulture);
                    custCall.FollowAMPM = Convert.ToDateTime(strFollowupDate).ToString("tt", CultureInfo.InvariantCulture);



                    custCall.FollowDateTime = Convert.ToDateTime(strFollowupDate);
                    custCall.IsDoNotCall = false;
                    custCall.sales_person_id = nEmployeeID;


                    _db.CustomerCallLogs.InsertOnSubmit(custCall);
                }
                _db.SubmitChanges();

                nEstimateID = custCall.CallLogID;

                #region Blocked Google Calendar
                //int nSalesPersonID = 0;
                //string calendarId = string.Empty;

                //// calendarId = ConfigurationManager.AppSettings["GoogleCalendarID"];

                //if (ConfigurationManager.AppSettings["IsProduction"].ToString() == "True")
                //{
                //    if (_db.customers.Where(sp => sp.customer_id == nCustomerID).Count() > 0)
                //    {
                //        objcust = _db.customers.Where(c => c.customer_id == nCustomerID).SingleOrDefault();
                //        nSalesPersonID = (int)objcust.sales_person_id;
                //        strCustInfo = "\n\n" + objcust.first_name1.Trim() + " " + objcust.last_name1.Trim() + "\n\n" + objcust.phone.Trim() + "\n\n" + objcust.email.Trim();
                //    }

                //    if (_db.sales_persons.Where(sp => sp.sales_person_id == nSalesPersonID && sp.google_calendar_account != null && sp.google_calendar_account != "").Count() > 0)
                //    {
                //        objSP = _db.sales_persons.Where(sp => sp.sales_person_id == nSalesPersonID && sp.google_calendar_account != null && sp.google_calendar_account != "").SingleOrDefault();
                //        calendarId = objSP.google_calendar_id;
                //    }

                //    if (calendarId != "")
                //    {
                //        //// Google Calendar DELETE------------
                //        List<ScheduleCalendarTemp> sclist = _db.ScheduleCalendarTemps.Where(sc => sc.customer_id == nCustomerID && sc.type_id == 2).ToList();
                //        foreach (ScheduleCalendarTemp sc in sclist)
                //        {
                //            if (sc.google_event_id != "")
                //            {
                //                var authenticator = GetAuthenticator(objSP.sales_person_id);
                //                var service = new GoogleCalendarServiceProxy(authenticator);
                //                service.DeleteEvent(calendarId, sc.google_event_id); // Delete
                //            }
                //        }
                //        //// Calendar DELETE------------


                //        //Google Calendar Insert----------------------------------------------------------

                //        var calendarEvent = new gCalendarEvent()
                //        {
                //            CalendarId = calendarId,
                //            Title = (objcust.first_name1.Trim() + " " + objcust.last_name1.Trim() + " " + objcust.phone.Trim()).Trim(),
                //            Location = objcust.address + ", " + objcust.city + ", " + objcust.state + " " + objcust.zip_code,
                //            StartDate = dtStartDate,
                //            EndDate = dtEndDate,
                //            Description = cevent.description + strCustInfo,
                //            ColorId = 1
                //        };

                //        var authenticatorr = GetAuthenticator(objSP.sales_person_id);
                //        var servicee = new GoogleCalendarServiceProxy(authenticatorr);
                //        GoogleEventID = servicee.CreateEvent(calendarEvent);

                //        //Google Calendar Insert End Code----------------------------------------------------------
                //    }
                //}
                #endregion



                //Customer Appointment Insert/Update
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == nCustomerID);
                cust.appointment_date = dtStartDate;
                cust.notes = cevent.description;
                _db.SubmitChanges();
            }
            else if (nTypeId == 2)// Type ID 2 = Sales
            {
                nTypeId = 22;// Sales Extra event TypeID  (Sales Calendar) without Customer ID
            }

            int nEventId = 1;

            if (_db.ScheduleCalendarTemps.Any())
            {
                int nMaxSC = Convert.ToInt32(_db.ScheduleCalendars.DefaultIfEmpty().Max(e => e == null ? 0 : e.event_id));
                int nMaxSCTemp = Convert.ToInt32(_db.ScheduleCalendarTemps.DefaultIfEmpty().Max(e => e == null ? 0 : e.event_id));

                if (nMaxSCTemp > nMaxSC)
                    nEventId = nMaxSCTemp + 1;
                else
                    nEventId = nMaxSC + 1;
            }

            if (cevent.location_name != null && cevent.location_name != "")
            {
                strTitle = cevent.section_name.Trim() + " (" + cevent.location_name.Trim() + ")";
            }
            if (nTypeId == 1)
            {
                objSCTemp.event_id = nEventId;
                objSCTemp.title = strTitle;
                objSCTemp.description = cevent.description + strCustInfo;
                objSCTemp.event_start = dtStartDate;
                objSCTemp.event_end = dtEndDate;
                objSCTemp.client_id = cevent.client_id;
                objSCTemp.customer_id = nCustomerID;
                objSCTemp.estimate_id = nEstimateID;
                objSCTemp.employee_id = nEmployeeID;
                objSCTemp.employee_name = strEmployeeName;
                objSCTemp.section_name = cevent.section_name.Trim();//.Replace(")", "").Replace("(", "").Trim();
                objSCTemp.location_name = cevent.location_name.Trim().Replace(")", "").Replace("(", "").Trim();
                objSCTemp.create_date = DateTime.Now;
                objSCTemp.type_id = nTypeId;
                objSCTemp.last_updated_by = strUName;
                objSCTemp.last_updated_date = DateTime.Now;
                objSCTemp.parent_id = 0;
                objSCTemp.job_start_date = dtJobStartDate;
                objSCTemp.co_pricing_list_id = nPricingId;
                objSCTemp.cssClassName = strCssClassName;
                objSCTemp.google_event_id = GoogleEventID;
                objSCTemp.operation_notes = "";
                objSCTemp.IsEstimateActive = true;
                objSCTemp.duration = GetDuration(dtStartDate, dtEndDate);
                objSCTemp.IsScheduleDayException = IsScheduleDayException;
                objSCTemp.is_complete = cevent.is_complete;
                objSCTemp.IsEWSCalendarSynch = false;
                objSCTemp.weekends = setWeekendDays((DateTime)objSCTemp.event_start, (DateTime)objSCTemp.event_end);
                objSCTemp.selectedweekends = "";


                _db.ScheduleCalendarTemps.InsertOnSubmit(objSCTemp);
                _db.SubmitChanges();

                //////------------------ Add Event Link -------------------------------////////////
                if (!IsScheduleDayException)
                {
                    int npEventId = 0;
                    int ncEventId = 0;
                    string linkType = cevent.linkType;
                    var testIsNullOrEmpty = String.IsNullOrEmpty(cevent.linkType);
                    int id = objSCTemp.event_id;
                    int child_event_id = cevent.child_event_id;
                    int dependencyType = cevent.dependencyType;
                    int parentDependencyType = cevent.parentDependencyType;

                    int nOffsetDays = 0;
                    if (cevent.offsetDays != null)
                    {
                        nOffsetDays = cevent.offsetDays;
                    }

                    int parentOffsetDays = 0;
                    if (cevent.parentOffsetDays != null)
                    {
                        parentOffsetDays = cevent.parentOffsetDays;
                    }

                    int nDuration = GetDuration(cevent.start, cevent.end);

                    if ((!String.IsNullOrEmpty(linkType)) && linkType.Equals("Parent"))
                    {
                        npEventId = id;
                        ncEventId = child_event_id;
                        id = ncEventId;
                        child_event_id = npEventId;

                        dependencyType = parentDependencyType;
                        nOffsetDays = parentOffsetDays;
                    }

                    if (child_event_id != 0)//New Event Link Insert
                    {
                        ScheduleCalendarLinkTemp objSCLinkTmp = new ScheduleCalendarLinkTemp();

                        ScheduleCalendarTemp objParentSCTmp = _db.ScheduleCalendarTemps.SingleOrDefault(sc => sc.event_id == id && sc.customer_id == nCustomerID && sc.estimate_id == nEstimateID);
                        ScheduleCalendarTemp objChildSCTmp = _db.ScheduleCalendarTemps.SingleOrDefault(sc => sc.event_id == child_event_id && sc.customer_id == nCustomerID && sc.estimate_id == nEstimateID);

                        if (objParentSCTmp != null && objChildSCTmp != null)
                        {

                            int nLinkId = 1;

                            if (_db.ScheduleCalendarLinkTemps.Any())
                            {
                                int nMaxSCLink = Convert.ToInt32(_db.ScheduleCalendarLinks.DefaultIfEmpty().Max(e => e == null ? 0 : e.link_id));
                                int nMaxSCLinkTemp = Convert.ToInt32(_db.ScheduleCalendarLinkTemps.DefaultIfEmpty().Max(e => e == null ? 0 : e.link_id));

                                if (nMaxSCLinkTemp > nMaxSCLink)
                                    nLinkId = nMaxSCLinkTemp + 1;
                                else
                                    nLinkId = nMaxSCLink + 1;
                            }
                            objSCLinkTmp.link_id = nLinkId;
                            objSCLinkTmp.parent_event_id = id;
                            objSCLinkTmp.child_event_id = child_event_id;
                            objSCLinkTmp.customer_id = objParentSCTmp.customer_id;
                            objSCLinkTmp.estimate_id = objParentSCTmp.estimate_id;
                            objSCLinkTmp.dependencyType = dependencyType;
                            objSCLinkTmp.lag = nOffsetDays;

                            _db.ScheduleCalendarLinkTemps.InsertOnSubmit(objSCLinkTmp);
                            _db.SubmitChanges();

                            if (dependencyType == 1) // Start Same Time
                            {
                                nDuration = Convert.ToInt32(objChildSCTmp.duration) - 1;
                                DateTime dtOldStart = Convert.ToDateTime(objChildSCTmp.event_start);
                                DateTime dtOldEnd = Convert.ToDateTime(objChildSCTmp.event_end);
                                DateTime dtNewStartl = Convert.ToDateTime(objParentSCTmp.event_start);

                                dtNewStartl = new DateTime(dtNewStartl.Year, dtNewStartl.Month, dtNewStartl.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                DateTime dtNewEndl = new DateTime(dtNewStartl.Year, dtNewStartl.Month, dtNewStartl.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                for (int i = 1; i <= nDuration; i++)
                                {
                                    dtNewEndl = GetWorkingDay(dtNewEndl.AddDays(1));

                                }

                                objChildSCTmp.event_start = dtNewStartl;
                                objChildSCTmp.event_end = dtNewEndl;
                                objChildSCTmp.IsEWSCalendarSynch = false;
                                _db.SubmitChanges();
                            }

                            if (dependencyType == 2) // Start After Finish
                            {

                                nDuration = Convert.ToInt32(objChildSCTmp.duration) - 1;
                                DateTime dtOldStart = Convert.ToDateTime(objChildSCTmp.event_start);
                                DateTime dtOldEnd = Convert.ToDateTime(objChildSCTmp.event_end);


                                DateTime dtNewStartl = GetWorkingDay(Convert.ToDateTime(objParentSCTmp.event_end).AddDays(1));

                                dtNewStartl = new DateTime(dtNewStartl.Year, dtNewStartl.Month, dtNewStartl.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                DateTime dtNewEndl = new DateTime(dtNewStartl.Year, dtNewStartl.Month, dtNewStartl.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                for (int i = 1; i <= nDuration; i++)
                                {
                                    dtNewEndl = GetWorkingDay(dtNewEndl.AddDays(1));

                                }



                                objChildSCTmp.event_start = dtNewStartl;

                                objChildSCTmp.event_end = dtNewEndl;
                                objChildSCTmp.IsEWSCalendarSynch = false;
                                _db.SubmitChanges();
                            }

                            if (dependencyType == 3) // Offset days
                            {


                                nDuration = Convert.ToInt32(objChildSCTmp.duration) - 1;


                                DateTime dtOldStart = Convert.ToDateTime(objChildSCTmp.event_start);
                                DateTime dtOldEnd = Convert.ToDateTime(objChildSCTmp.event_end);


                                DateTime dtNewStartl = Convert.ToDateTime(objParentSCTmp.event_end);

                                for (int i = 0; i <= nOffsetDays; i++)
                                {
                                    dtNewStartl = GetWorkingDay(dtNewStartl.AddDays(1));

                                }


                                dtNewStartl = new DateTime(dtNewStartl.Year, dtNewStartl.Month, dtNewStartl.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                DateTime dtNewEndl = new DateTime(dtNewStartl.Year, dtNewStartl.Month, dtNewStartl.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                for (int i = 1; i <= nDuration; i++)
                                {
                                    dtNewEndl = GetWorkingDay(dtNewEndl.AddDays(1));

                                }

                                objChildSCTmp.event_start = dtNewStartl;

                                objChildSCTmp.event_end = dtNewEndl;
                                objChildSCTmp.IsEWSCalendarSynch = false;
                                _db.SubmitChanges();
                            }


                            updateAllLink(child_event_id);
                        }
                    }
                }
                //////------------------- Add Event Link Code End--------------------///////////////////

                key = Convert.ToInt32(objSCTemp.event_id);
            }
            else
            {
                objSC.event_id = nEventId;
                objSC.title = strTitle;
                objSC.description = cevent.description + strCustInfo;
                objSC.event_start = cevent.start;// dtStartDate;
                objSC.event_end = cevent.end;// dtEndDate;
                objSC.client_id = cevent.client_id;               
                objSC.customer_id = nCustomerID;
                objSC.estimate_id = nEstimateID;
                objSC.employee_id = nEmployeeID;
                objSC.employee_name = strEmployeeName;
                objSC.section_name = cevent.section_name.Trim().Replace(")", "").Replace("(", "").Trim();
                objSC.location_name = cevent.location_name.Trim().Replace(")", "").Replace("(", "").Trim();
                objSC.create_date = DateTime.Now;
                objSC.type_id = nTypeId;
                objSC.last_updated_by = strUName;
                objSC.last_updated_date = DateTime.Now;
                objSC.parent_id = 0;
                objSC.job_start_date = dtJobStartDate;
                objSC.co_pricing_list_id = nPricingId;
                objSC.cssClassName = strCssClassName;
                objSC.google_event_id = GoogleEventID;
                objSC.operation_notes = "";
                objSC.IsEstimateActive = true;
                objSC.duration = GetDuration(dtStartDate, dtEndDate);
                objSC.IsScheduleDayException = true;
                objSC.is_complete = cevent.is_complete;
                objSC.IsEWSCalendarSynch = false;
                _db.ScheduleCalendars.InsertOnSubmit(objSC);
                _db.SubmitChanges();

                key = Convert.ToInt32(objSC.event_id);
            }



            //System.Web.HttpContext.Current.Session["cid"] = null;
            //System.Web.HttpContext.Current.Session["eid"] = null;
            //System.Web.HttpContext.Current.Session["empid"] = null;
            // System.Web.HttpContext.Current.Session["TypeID"] = null;

            return key;
        }
        catch (Exception ex)
        {
            return 0;
        }

    }

    private static string GetDayOfWeekWithOutHoliday(string strdt)
    {
        int cnt = 0;
        DateTime dt = Convert.ToDateTime(strdt);

        if (dt.DayOfWeek == DayOfWeek.Saturday)
            cnt--;
        else if (dt.DayOfWeek == DayOfWeek.Sunday)
            cnt = -2;

        return dt.AddDays(cnt).ToShortDateString();
    }

    private static string GetDayOfWeek(string strdt)
    {
        int cnt = 0;
        DateTime dt = Convert.ToDateTime(strdt);

        if (dt.DayOfWeek == DayOfWeek.Saturday)
            cnt = +2;
        else if (dt.DayOfWeek == DayOfWeek.Sunday)
            cnt++;
        else if (IsHoliday(dt))
        {
            DateTime hdt = dt.AddDays(1);

            if (hdt.DayOfWeek == DayOfWeek.Saturday)
                cnt = +3;
            else if (hdt.DayOfWeek == DayOfWeek.Sunday)
                cnt = +2;
            else
                cnt++;
        }

        return dt.AddDays(cnt).ToShortDateString();
    }

    private static bool IsHoliday(DateTime dt)
    {
        bool IsHoliday = false;
        DateTime date = DateTime.Parse(dt.ToShortDateString());
        HolidayCalculator hc;
        if (HttpContext.Current.Session["hc"] == null)
        {
            hc = new HolidayCalculator(date, "Holidays.xml");
        }
        else
        {
            hc = (HolidayCalculator)HttpContext.Current.Session["hc"];
        }


        foreach (HolidayCalculator.Holiday h in hc.OrderedHolidays)
        {
            if (h.Date.ToShortDateString() == date.ToShortDateString())
            {
                IsHoliday = true;
            }
        }
        return IsHoliday;
    }

    private static GoogleAuthenticator GetAuthenticator(int salespersonid)
    {
        //var authenticator = (GoogleAuthenticator)System.Web.HttpContext.Current.Session["authenticator"];

        //if (authenticator == null || !authenticator.IsValid)
        //{
        DataClassesDataContext _db = new DataClassesDataContext();
        // Get a new Authenticator using the Refresh Token
        int nUserID = salespersonid;
        var refreshToken = _db.GoogleRefreshTokens.FirstOrDefault(c => c.UserID == salespersonid).RefreshToken;
        var authenticator = GoogleAuthorizationHelper.RefreshAuthenticator(refreshToken);
        //    System.Web.HttpContext.Current.Session["authenticator"] = authenticator;
        //}

        return authenticator;
    }

    private static int GetHolidayCount(DateTime startDate, DateTime endDate)
    {
        int count = 0;
        TimeSpan diff = endDate - startDate;
        int days = diff.Days;

        bool bFound = false;
        for (var i = 0; i <= days; i++)
        {
            bFound = false;
            var testDate = startDate.AddDays(i);
            switch (testDate.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    Console.WriteLine(testDate.ToShortDateString());
                    count++;
                    bFound = true;
                    break;
            }

            if (bFound == false && IsHoliday(testDate))
                count++;


        }

        return count;
    }

    private static int GetDuration(DateTime startDate, DateTime endDate)
    {
        startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
        endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

        int nOffDay = GetHolidayCount(startDate, endDate);

        int nDuration = 1;

        TimeSpan diff = endDate - startDate;
        nDuration += diff.Days;

        return (nDuration - nOffDay);
    }

    private static DateTime GetWorkingDay(DateTime Date)
    {
        if (IsHoliday(Date))
            Date = Date.AddDays(1);

        switch (Date.DayOfWeek)
        {
            case DayOfWeek.Saturday:
                Date = Date.AddDays(2);
                break;
            case DayOfWeek.Sunday:
                Date = Date.AddDays(1);
                break;

        }

        return Date;
    }

    private static int GetOffsetDays(DateTime startDate, DateTime endDate)
    {
        startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
        endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

        int nOffDay = GetHolidayCount(startDate, endDate);

        int nOffsetDays = 0;

        TimeSpan diff = endDate - startDate;
        nOffsetDays = diff.Days;

        return (nOffsetDays - nOffDay);
    }


    private static void GoOnline(int nCustId, int nEstid)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        customer objcpmList = _db.customers.SingleOrDefault(c => c.customer_id == nCustId);

        if (_db.ScheduleCalendarTemps.Any(sc => sc.customer_id == nCustId && sc.estimate_id == nEstid))
        {
            //Delete  table
            string sqlDELETE = "DELETE ScheduleCalendar WHERE type_id <>5 AND [customer_id] = " + nCustId + " AND [estimate_id] = " + nEstid + "";
            _db.ExecuteCommand(sqlDELETE, string.Empty);

            string sqlDELETELink = "DELETE ScheduleCalendarLink WHERE [customer_id] = " + nCustId + " AND [estimate_id] = " + nEstid + "";
            _db.ExecuteCommand(sqlDELETELink, string.Empty);

            //Insert
            string sSQLINSERT = "INSERT INTO [ScheduleCalendar] " +
                            " SELECT [title],[description],[event_start],[event_end],[customer_id],[estimate_id],[employee_id],[section_name], " +
                            " [location_name],[create_date],[last_updated_date],[last_updated_by],[type_id],[parent_id],[job_start_date], " +
                            " [co_pricing_list_id],[cssClassName],[google_event_id],[operation_notes],[is_complete],[IsEstimateActive],[employee_name],[event_id], [duration] " +
                            " FROM [ScheduleCalendarTemp] " +
                            " WHERE type_id <>5 AND [customer_id] = " + nCustId + " AND [estimate_id] = " + nEstid + "";

            _db.ExecuteCommand(sSQLINSERT, string.Empty);

            //Insert
            string sSqlLinkINSERT = "INSERT INTO [ScheduleCalendarLink] " +
                                   " SELECT [parent_event_id], " +
                                   " [child_event_id], " +
                                   " [customer_id], " +
                                   " [estimate_id], " +
                                   " [dependencyType], " +
                                   " [lag],[link_id] " +
                                   " FROM [ScheduleCalendarLinkTemp] " +
                                   " WHERE [customer_id] = " + nCustId + " AND [estimate_id] = " + nEstid + "";

            _db.ExecuteCommand(sSqlLinkINSERT, string.Empty);

            //Delete Temp table
            string sqlDELETETemp = "DELETE ScheduleCalendarTemp WHERE type_id <>5 AND [customer_id] = " + nCustId + " AND [estimate_id] = " + nEstid + "";
            _db.ExecuteCommand(sqlDELETETemp, string.Empty);

            //Delete Temp table
            string sqlDELETELinkTemp = "DELETE ScheduleCalendarLinkTemp WHERE [customer_id] = " + nCustId + " AND [estimate_id] = " + nEstid + "";
            _db.ExecuteCommand(sqlDELETELinkTemp, string.Empty);
        }

        objcpmList.isCalendarOnline = true;


        System.Web.HttpContext.Current.Session.Add("sIsCalendarOnline", true);

        _db.SubmitChanges();

    }

    private static void GoOffline(int nCustId, int nEstid)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        customer objcpmList = _db.customers.SingleOrDefault(c => c.customer_id == nCustId);

        if (_db.ScheduleCalendars.Any(sc => sc.customer_id == nCustId && sc.estimate_id == nEstid))
        {
            //Insert
            string sSqlINSERT = "INSERT INTO [ScheduleCalendarTemp] " +
                            " SELECT [title],[description],[event_start],[event_end],[customer_id],[estimate_id],[employee_id],[section_name], " +
                            " [location_name],[create_date],[last_updated_date],[last_updated_by],[type_id],[parent_id],[job_start_date], " +
                            " [co_pricing_list_id],[cssClassName],[google_event_id],[operation_notes],[is_complete],[IsEstimateActive],[employee_name],[event_id], [duration] " +
                            " FROM [ScheduleCalendar] " +
                            " WHERE type_id <>5 AND [customer_id] = " + nCustId + " AND [estimate_id] = " + nEstid + "";

            _db.ExecuteCommand(sSqlINSERT, string.Empty);

            //Insert
            string sSqlLinkINSERT = "INSERT INTO [ScheduleCalendarLinkTemp] " +
                                   " SELECT [parent_event_id], " +
                                   " [child_event_id], " +
                                   " [customer_id], " +
                                   " [estimate_id], " +
                                   " [dependencyType], " +
                                   " [lag],[link_id] " +
                                   " FROM [ScheduleCalendarLink] " +
                                   " WHERE [customer_id] = " + nCustId + " AND [estimate_id] = " + nEstid + "";

            _db.ExecuteCommand(sSqlLinkINSERT, string.Empty);
        }

        objcpmList.isCalendarOnline = false;

        System.Web.HttpContext.Current.Session.Add("sIsCalendarOnline", false);

        _db.SubmitChanges();

    }

    private static int UpdateOnlineSchedule(int id, String title, String section, String location, String description, String cssClassName, DateTime start, DateTime end, int empId, string empName,
        int child_event_id, int dependencyType, int offsetDays, int parentDependencyType, int parentOffsetDays, string linkType, int ncid, int neid, int nDuration, string strUpdatedBy, int nOffsetDays, bool IsScheduleDayException, bool IsComplete, string selectedWeekend)
    {
        int nCheck = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        if (!_db.ScheduleCalendars.Any(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid))
        {
            nCheck = 5;
            return nCheck;
        }

        if (_db.ScheduleCalendarLinks.Any(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid))
        {
            var objChildLink = _db.ScheduleCalendarLinks.SingleOrDefault(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid);

            var objP = _db.ScheduleCalendars.SingleOrDefault(sc => sc.event_id == objChildLink.parent_event_id);

            DateTime dtNewStartC = new DateTime(start.Year, start.Month, start.Day);
            DateTime dtNewEndP = new DateTime(Convert.ToDateTime(objP.event_end).Year, Convert.ToDateTime(objP.event_end).Month, Convert.ToDateTime(objP.event_end).Day);

            DateTime dtNewStartP = new DateTime(Convert.ToDateTime(objP.event_start).Year, Convert.ToDateTime(objP.event_start).Month, Convert.ToDateTime(objP.event_start).Day);

            TimeSpan diffPrntEndToChldStart = dtNewStartC - dtNewEndP; // Parent End date to Child Start Date
            TimeSpan diffPrnttStartToChldStart = dtNewStartC - dtNewStartP; // Parent Start date to Child Start Date

            int nDaysPrntEndToChldStart = diffPrntEndToChldStart.Days;// Parent End date to Child Start Date
            int nDaysPrnttStartToChldStart = diffPrnttStartToChldStart.Days;// Parent Start date to Child Start Date

            if (nDaysPrntEndToChldStart > 0)// Parent End date to Child Start Date
            {
                nCheck = 0;

            }
            else if (nDaysPrntEndToChldStart < 0)
            {
                nCheck = nDaysPrntEndToChldStart;

                if (nDaysPrnttStartToChldStart == 0)// Parent Start date to Child Start Date
                {
                    nCheck = 0;
                }
                else if (nDaysPrnttStartToChldStart > 0)// Parent Start date to Child Start Date
                {
                    nCheck = -1;
                }
            }
            else if (nDaysPrntEndToChldStart == 0 && nDaysPrnttStartToChldStart == 0)
            {
                nCheck = 0;
            }
            else
            {
                nCheck = -1;
            }


            if (nCheck < 0)
            {
                return nCheck;
            }
        }


        string sql = "UPDATE ScheduleCalendar SET title='" + title + "', section_name='" + section + "', location_name='" + location.Replace("'", "''") + "', description='" + description.Replace("'", "''") + "', " +
                    " cssClassName='" + cssClassName + "', event_start='" + start + "', event_end='" + end + "',  duration='" + nDuration + "', employee_id='" + empId + "',  employee_name='" + empName + "', " +
                    " last_updated_by='" + strUpdatedBy + "', last_updated_date='" + DateTime.Now + "', is_complete='" + Convert.ToInt32(IsComplete) + "', " +
                    " IsScheduleDayException=" + Convert.ToInt32(IsScheduleDayException) + ", IsEWSCalendarSynch = '" + Convert.ToBoolean(0) + "' " + ", " +
                    " weekends = '" + setWeekendDays(start, end) + "', " +
                    " selectedweekends = '" + selectedWeekend + "'" +
                    " WHERE event_id=" + id + " AND customer_id =" + ncid + " AND estimate_id = " + neid;

        _db.ExecuteCommand(sql, string.Empty);

        if (!IsScheduleDayException)
        {
            int npEventId = 0;
            int ncEventId = 0;

            var testIsNullOrEmpty = String.IsNullOrEmpty(linkType);

            if ((!String.IsNullOrEmpty(linkType)) && linkType.Equals("Parent"))
            {
                npEventId = id;
                ncEventId = child_event_id;
                id = ncEventId;
                child_event_id = npEventId;

                dependencyType = parentDependencyType;
                nOffsetDays = parentOffsetDays;
            }

            if (child_event_id != 0)//New Event Link Insert
            {
                ScheduleCalendarLink objSCLink = new ScheduleCalendarLink();

                ScheduleCalendar objParentSC = _db.ScheduleCalendars.SingleOrDefault(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid);
                ScheduleCalendar objChildSC = _db.ScheduleCalendars.SingleOrDefault(sc => sc.event_id == child_event_id && sc.customer_id == ncid && sc.estimate_id == neid);

                if (objParentSC != null && objChildSC != null)
                {
                    int nLinkId = 1;

                    if (_db.ScheduleCalendarLinks.Any())
                    {
                        int nMaxSCLink = Convert.ToInt32(_db.ScheduleCalendarLinks.DefaultIfEmpty().Max(e => e == null ? 0 : e.link_id));
                        int nMaxSCLinkOnlin = Convert.ToInt32(_db.ScheduleCalendarLinkTemps.DefaultIfEmpty().Max(e => e == null ? 0 : e.link_id));

                        if (nMaxSCLinkOnlin > nMaxSCLink)
                            nLinkId = nMaxSCLinkOnlin + 1;
                        else
                            nLinkId = nMaxSCLink + 1;
                    }
                    objSCLink.link_id = nLinkId;
                    objSCLink.parent_event_id = id;
                    objSCLink.child_event_id = child_event_id;
                    objSCLink.customer_id = objParentSC.customer_id;
                    objSCLink.estimate_id = objParentSC.estimate_id;
                    objSCLink.dependencyType = dependencyType;
                    objSCLink.lag = nOffsetDays;

                    _db.ScheduleCalendarLinks.InsertOnSubmit(objSCLink);
                    _db.SubmitChanges();

                    if (dependencyType == 1) // Start Same Time
                    {
                        nDuration = Convert.ToInt32(objChildSC.duration) - 1;
                        DateTime dtOldStart = Convert.ToDateTime(objChildSC.event_start);
                        DateTime dtOldEnd = Convert.ToDateTime(objChildSC.event_end);
                        DateTime dtNewStart = Convert.ToDateTime(objParentSC.event_start);

                        dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                        DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                        for (int i = 1; i <= nDuration; i++)
                        {
                            dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                        }

                        objChildSC.event_start = dtNewStart;
                        objChildSC.event_end = dtNewEnd;
                        objChildSC.IsEWSCalendarSynch = false;
                        _db.SubmitChanges();
                    }

                    if (dependencyType == 2) // Start After Finish
                    {

                        nDuration = Convert.ToInt32(objChildSC.duration) - 1;
                        DateTime dtOldStart = Convert.ToDateTime(objChildSC.event_start);
                        DateTime dtOldEnd = Convert.ToDateTime(objChildSC.event_end);


                        DateTime dtNewStart = GetWorkingDay(Convert.ToDateTime(objParentSC.event_end).AddDays(1));

                        dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                        DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                        for (int i = 1; i <= nDuration; i++)
                        {
                            dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                        }



                        objChildSC.event_start = dtNewStart;

                        objChildSC.event_end = dtNewEnd;
                        objChildSC.IsEWSCalendarSynch = false;
                        _db.SubmitChanges();
                    }

                    if (dependencyType == 3) // Offset days
                    {


                        nDuration = Convert.ToInt32(objChildSC.duration) - 1;


                        DateTime dtOldStart = Convert.ToDateTime(objChildSC.event_start);
                        DateTime dtOldEnd = Convert.ToDateTime(objChildSC.event_end);


                        DateTime dtNewStart = Convert.ToDateTime(objParentSC.event_end);

                        for (int i = 0; i <= nOffsetDays; i++)
                        {
                            dtNewStart = GetWorkingDay(dtNewStart.AddDays(1));

                        }


                        dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                        DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                        for (int i = 1; i <= nDuration; i++)
                        {
                            dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                        }

                        objChildSC.event_start = dtNewStart;

                        objChildSC.event_end = dtNewEnd;
                        objChildSC.IsEWSCalendarSynch = false;
                        _db.SubmitChanges();
                    }


                    updateAllLinkOnline(child_event_id);
                }
            }
            else//Update Event Link 
            {
                ScheduleCalendar ocjSCOnlinUpdate = _db.ScheduleCalendars.SingleOrDefault(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid);
                //If Event is Parent
                List<ScheduleCalendarLink> objSCLinkTmpP = new List<ScheduleCalendarLink>();

                if (_db.ScheduleCalendarLinks.Any(sl => sl.parent_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid))
                {
                    objSCLinkTmpP = _db.ScheduleCalendarLinks.Where(sl => sl.parent_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid).ToList();

                    foreach (ScheduleCalendarLink slT in objSCLinkTmpP)
                    {
                        ScheduleCalendar ocjSCOnlinC = _db.ScheduleCalendars.FirstOrDefault(s => s.event_id == slT.child_event_id);


                        nOffsetDays = (int)slT.lag;

                        if (slT.dependencyType == 1) // Start Same Time
                        {
                            nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;
                            DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                            DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);
                            DateTime dtNewStart = Convert.ToDateTime(ocjSCOnlinUpdate.event_start);

                            dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                            DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                            for (int i = 1; i <= nDuration; i++)
                            {
                                dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                            }

                            ocjSCOnlinC.event_start = dtNewStart;
                            ocjSCOnlinC.event_end = dtNewEnd;
                            ocjSCOnlinC.IsEWSCalendarSynch = false;
                            _db.SubmitChanges();
                        }

                        if (slT.dependencyType == 2) // Start After Finish
                        {

                            nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;
                            DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                            DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);


                            DateTime dtNewStart = GetWorkingDay(Convert.ToDateTime(ocjSCOnlinUpdate.event_end).AddDays(1));

                            dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                            DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                            for (int i = 1; i <= nDuration; i++)
                            {
                                dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                            }

                            ocjSCOnlinC.event_start = dtNewStart;
                            ocjSCOnlinC.event_end = dtNewEnd;
                            ocjSCOnlinC.IsEWSCalendarSynch = false;
                            _db.SubmitChanges();
                        }

                        if (slT.dependencyType == 3) // Offset days
                        {
                            nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;


                            DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                            DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);


                            DateTime dtNewStart = Convert.ToDateTime(ocjSCOnlinUpdate.event_end);

                            for (int i = 0; i <= nOffsetDays; i++)
                            {
                                dtNewStart = GetWorkingDay(dtNewStart.AddDays(1));

                            }


                            dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                            DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                            for (int i = 1; i <= nDuration; i++)
                            {
                                dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                            }

                            ocjSCOnlinC.event_start = dtNewStart;
                            ocjSCOnlinC.event_end = dtNewEnd;
                            ocjSCOnlinC.IsEWSCalendarSynch = false;
                            _db.SubmitChanges();
                        }

                        updateAllLinkOnline((int)slT.child_event_id);
                    }
                }

                //If Event is Child
                ScheduleCalendarLink objSCLinkTmpC = new ScheduleCalendarLink();
                if (_db.ScheduleCalendarLinks.Any(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid))
                {
                    objSCLinkTmpC = _db.ScheduleCalendarLinks.FirstOrDefault(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid);

                    ScheduleCalendar ocjSCOnlinP = _db.ScheduleCalendars.FirstOrDefault(s => s.event_id == objSCLinkTmpC.parent_event_id);

                    //new
                    DateTime pStartDate = Convert.ToDateTime(ocjSCOnlinP.event_start);
                    pStartDate = new DateTime(pStartDate.Year, pStartDate.Month, pStartDate.Day); // Parent Start

                    DateTime pEndDate = Convert.ToDateTime(ocjSCOnlinP.event_end);
                    pEndDate = new DateTime(pEndDate.Year, pEndDate.Month, pEndDate.Day); // Parent End

                    DateTime cStartDate = Convert.ToDateTime(ocjSCOnlinUpdate.event_start).Date;
                    cStartDate = new DateTime(cStartDate.Year, cStartDate.Month, cStartDate.Day); // Child Start

                    //new

                    int nlagDays = GetOffsetDays(pEndDate, cStartDate); // Convert.ToDateTime(ocjSCOnlinUpdate.event_start).Day - Convert.ToDateTime(ocjSCOnlinP.event_end).Day - 1;

                    if (nlagDays == 0) // Start Same Time
                    {
                        objSCLinkTmpC.dependencyType = 1;
                        objSCLinkTmpC.lag = 0;
                        _db.SubmitChanges();
                    }
                    else if (nlagDays == 1) // Start After Finish
                    {
                        objSCLinkTmpC.dependencyType = 2;
                        objSCLinkTmpC.lag = 1;
                        _db.SubmitChanges();
                    }
                    else if (nlagDays > 1) // Offset Days
                    {
                        objSCLinkTmpC.dependencyType = 3;
                        objSCLinkTmpC.lag = nlagDays - 1;
                        _db.SubmitChanges();
                    }
                }
            }
        }
        return nCheck;
    }

    public static string setWeekendDays(DateTime start, DateTime end)
    {
        int hourStart = start.Hour;
        int minuteStart = start.Minute;
        int secondStart = start.Second;
        string weeekendList = "";
        int hourEnd = end.Hour;
        int minuteEnd = end.Minute;
        int secondEns = end.Second;

        DateTime dtStart = Convert.ToDateTime(start);
        DateTime dtEnd = Convert.ToDateTime(end);

        DateTime startDate = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day);
        DateTime endDate = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day);

        TimeSpan diff = endDate - startDate;

        for (int i = 0; i <= diff.Days; i++)
        {
            DateTime dtCurrentTime = start.AddDays(i);
            if (dtCurrentTime.DayOfWeek == DayOfWeek.Sunday)
            {
                weeekendList += "Sun " + dtCurrentTime.ToShortDateString() + ',';
            }
            else if (dtCurrentTime.DayOfWeek == DayOfWeek.Saturday)
            {
                weeekendList += "Sat " + dtCurrentTime.ToShortDateString() + ',';
            }
        }
        weeekendList.TrimEnd(',');
        return weeekendList;
    }
    public static List<CalendarEvent> getWeekendDays(DateTime start, DateTime end, List<CalendarEvent> events)
    {
        int hourStart = start.Hour;
        int minuteStart = start.Minute;
        int secondStart = start.Second;

        int hourEnd = end.Hour;
        int minuteEnd = end.Minute;
        int secondEns = end.Second;

        DateTime dtStart = Convert.ToDateTime(start);
        DateTime dtEnd = Convert.ToDateTime(end);

        DateTime startDate = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day);
        DateTime endDate = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day);

        TimeSpan diff = endDate - startDate;

        for (int i = 0; i < diff.Days; i++)
        {
            DateTime dtCurrentTime = start.AddDays(i);
            if (dtCurrentTime.DayOfWeek == DayOfWeek.Sunday || dtCurrentTime.DayOfWeek == DayOfWeek.Saturday)
            {

                CalendarEvent calEvntObjforWeekend = new CalendarEvent();

                calEvntObjforWeekend.start = dtCurrentTime.AddHours(hourStart).AddMinutes(minuteStart).AddSeconds(secondStart);
                calEvntObjforWeekend.end = dtCurrentTime.AddHours(hourEnd).AddMinutes(minuteEnd).AddSeconds(secondEns);
                calEvntObjforWeekend.id = i;
                calEvntObjforWeekend.title = "No Work Day";
                calEvntObjforWeekend.description = "";
                calEvntObjforWeekend.customer_id = 0;
                calEvntObjforWeekend.estimate_id = 0;
                calEvntObjforWeekend.employee_id = 0;
                calEvntObjforWeekend.section_name = "";
                calEvntObjforWeekend.location_name = "";
                calEvntObjforWeekend.cssClassName = "fc-workday";
                calEvntObjforWeekend.type_id = 55;
                calEvntObjforWeekend.operation_notes = "";
                calEvntObjforWeekend.employee_name = "";
                calEvntObjforWeekend.customer_last_name = "";
                calEvntObjforWeekend.IsScheduleDayException = true;
                calEvntObjforWeekend.is_complete = false;
                calEvntObjforWeekend.has_parent_link = false;
                calEvntObjforWeekend.has_child_link = false;
                calEvntObjforWeekend.estimate_name = "";
                calEvntObjforWeekend.duration = 0;
                calEvntObjforWeekend.WeekEnds = "";

                if (events.Any(e => (e.selectedweekends ?? "").Contains(calEvntObjforWeekend.start.ToShortDateString()) && e.IsScheduleDayException))
                {
                    //
                }
                else
                {
                    events.Add(calEvntObjforWeekend);
                }

            }
        }


        return events;

    }

    public static void SetDayOfWeek(List<CalendarEvent> events)
    {
        DateTime dt = new DateTime();
        foreach (var e in events)
        {
            TimeSpan diff = e.end - e.start;
            if (diff.Days > 0)
            {
                for (int i = 0; i <= diff.Days; i++)
                {
                    DateTime dtCurrentTime = e.start.AddDays(i);
                    if (dtCurrentTime.DayOfWeek == DayOfWeek.Sunday || dtCurrentTime.DayOfWeek == DayOfWeek.Saturday)
                    {
                        e.WeekEnds += dtCurrentTime.ToShortDateString() + ", ";
                    }
                }
            }
            else
            {
                if (e.start.DayOfWeek == DayOfWeek.Sunday || e.start.DayOfWeek == DayOfWeek.Saturday)
                {
                    e.WeekEnds += e.start.ToShortDateString() + ", ";
                }
            }
        }

    }

}
