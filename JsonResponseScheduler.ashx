<%@ WebHandler Language="C#" Class="JsonResponseScheduler" %>

using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Web.SessionState;

public class JsonResponseScheduler : IHttpHandler, IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        try {
        context.Response.ContentType = "application/json";

        DateTime start = new DateTime(1970, 1, 1);
        DateTime end = new DateTime(1970, 1, 1);
        var test = context.Request.QueryString["start"];
        start = Convert.ToDateTime(context.Request.QueryString["start"].ToString());//start.AddSeconds(double.Parse(context.Request.QueryString["start"])); // 
        end = Convert.ToDateTime(context.Request.QueryString["end"].ToString());// end.AddSeconds(double.Parse(context.Request.QueryString["end"])); //

        //start = Convert.ToDateTime("2019-01-24");
        //end = Convert.ToDateTime("2019-01-25");

        int nCusId = 0;
        string strSecName = "";
        string strSearchUserName = "";
        string strSearchSuperintendentName = "";

        if (System.Web.HttpContext.Current.Session["CusId"] != null)
            nCusId = (int)System.Web.HttpContext.Current.Session["CusId"];

        if (System.Web.HttpContext.Current.Session["sSecName"] != null)
            strSecName = (string)System.Web.HttpContext.Current.Session["sSecName"];

        if (System.Web.HttpContext.Current.Session["sKeySearchUserName"] != null)
            strSearchUserName = (string)System.Web.HttpContext.Current.Session["sKeySearchUserName"];

        if (System.Web.HttpContext.Current.Session["sKeySearchSuperintendentName"] != null)
            strSearchSuperintendentName = (string)System.Web.HttpContext.Current.Session["sKeySearchSuperintendentName"];

        String result = String.Empty;

        result += "[";

        List<int> idList = new List<int>();
        foreach (CalendarEvent cevent in EventDAO.getEvents(start, end, nCusId, strSecName, strSearchUserName, strSearchSuperintendentName))
        //foreach (CalendarEvent cevent in EventDAO.getEvents(start, end))
        {
            result += convertCalendarEventIntoString(cevent);
            idList.Add(cevent.id);
        }

        if (result.EndsWith(","))
        {
            result = result.Substring(0, result.Length - 1);
        }

        result += "]";

        //result = "[{" +
        //            "\"title\": \"Test & Event\"," +
        //            "\"start\": \"2019-01-01T00:00:00\"," +
        //            "\"end\": \"2019-01-01T00:00:00\"" +
        //        "}]";

        //store list of event ids in Session, so that it can be accessed in web methods
        context.Session["idList"] = idList;

        context.Response.Write(result);
        }
        catch (Exception ex)
        {
            string esss = ex.Message;
        }
    }

    private String convertCalendarEventIntoString(CalendarEvent cevent)
    {
        bool allDay = true;
        //  if (ConvertToTimestamp(cevent.start).ToString().Equals(ConvertToTimestamp(cevent.end).ToString()))
        if (Newtonsoft.Json.JsonConvert.SerializeObject(cevent.start).ToString().Equals(Newtonsoft.Json.JsonConvert.SerializeObject(cevent.end).ToString()))
        {

            if (cevent.start.Hour == 0 && cevent.start.Minute == 0 && cevent.start.Second == 0)
            {
                allDay = true;
            }
            else
            {
                allDay = false;
            }
        }
        else
        {
            if (cevent.start.Hour == 0 && cevent.start.Minute == 0 && cevent.start.Second == 0
                && cevent.end.Hour == 0 && cevent.end.Minute == 0 && cevent.end.Second == 0)
            {
                allDay = true;
            }
            else
            {
                allDay = false;
            }
        }
        // allDay = true;
        string nEstimateID = "";
        string nCustomerID = "";
        if (System.Web.HttpContext.Current.Session["EstSelected"] != null)
        {
            nEstimateID = Convert.ToString(System.Web.HttpContext.Current.Session["EstSelected"]);
        }
        if (System.Web.HttpContext.Current.Session["CustSelected"] != null)
        {
            nCustomerID = Convert.ToString(System.Web.HttpContext.Current.Session["CustSelected"]);
        }


        string serviceColor = "fc-DarkGreen";
        //if (cevent.type_id == 1)
        //    serviceColor = "fc-contract";
        //else if (cevent.type_id == 2)
        //    serviceColor = "fc-ticket";
        //else if (cevent.type_id == 3)
        //    serviceColor = "fc-sales";
        //else if (cevent.type_id == 5)
        //    serviceColor = "fc-holoday";
        //else
        //    serviceColor = "fc-default";
        if (cevent.cssClassName != null && cevent.cssClassName != "")
            serviceColor = cevent.cssClassName;

        if (nCustomerID == cevent.customer_id.ToString() && nEstimateID == cevent.estimate_id.ToString() && cevent.type_id == 1)
            serviceColor = cevent.cssClassName + " fc-selected";
        if (nCustomerID == cevent.customer_id.ToString() && cevent.type_id == 2)
            serviceColor = cevent.cssClassName + " fc-selected";
        if (cevent.type_id == 0)
            serviceColor = "fc-default";

        if (cevent.cssClassName.Contains("fc-PaymentTerms")) // if (cevent.type_id == 5) // Only was For Holiday Event Item, Now for all item Time will not show before event title in Calendar
        {
            serviceColor += " hideCalendarTime ";
        }

        bool IsUnassignedChecked = true;
        if (System.Web.HttpContext.Current.Session["sUnassignedCheck"] != null)
        {
            IsUnassignedChecked = (bool)System.Web.HttpContext.Current.Session["sUnassignedCheck"];
        }
        var nTypeId = new List<int> { 5, 55 };

        if ((!nTypeId.Contains(cevent.type_id) && (cevent.employee_name ?? "") == "" || (cevent.employee_name ?? "").Trim().ToUpper().Contains("TBD")) && IsUnassignedChecked)
            serviceColor += " fc-Unassigned";

        if (cevent.type_id == 55)
        {
            //     serviceColor = serviceColor.Replace("","");
        }

        var encodeTitle = HttpContext.Current.Server.HtmlEncode(cevent.title);
        var jesonTitle = Newtonsoft.Json.JsonConvert.SerializeObject(cevent.title).ToString();

        var strSection = Newtonsoft.Json.JsonConvert.SerializeObject(cevent.section_name).ToString();

        var strLocation = "";
        if (cevent.location_name != null)
            strLocation = Newtonsoft.Json.JsonConvert.SerializeObject(cevent.location_name).ToString();

        var jesonDescription = Newtonsoft.Json.JsonConvert.SerializeObject(cevent.description).ToString();

        return "{" +
                  "\"id\": " + "\"" + cevent.id + "\"," +
                  "\"title\":" + jesonTitle + "," +
                  "\"start\":" + "\"" + ConvertToTimestamp(cevent.start).ToString() + "\"" + "," +
                  "\"end\": " + "\"" + ConvertToTimestamp(cevent.end).ToString() + "\"" + "," +
                  "\"allDay\":" + "\"" + allDay + "\"," +
                  "\"section_name\": " + strSection + "," +
                  "\"location_name\": " + strLocation + "," +
                  "\"description\":" + jesonDescription + "," +
                  "\"EstimateID\": " + "\"" + cevent.estimate_id + "\"," +
                  "\"CustomerID\": " + "\"" + cevent.customer_id + "\"," +
                  "\"employee_id\": " + "\"" + cevent.employee_id + "\"," +
                  "\"employee_name\": " + "\"" + cevent.employee_name + "\"," +
                  "\"customer_last_name\": " + "\"" + cevent.customer_last_name + "\"," +
                  "\"operation_notes\": " + "\"" + cevent.operation_notes + "\"," +
                  "\"TypeID\": " + "\"" + cevent.type_id + "\"," +
                  "\"editable\": " + "\"" + true + "\"," +
                  "\"IsScheduleDayException\": " + "\"" + cevent.IsScheduleDayException + "\"," +
                  "\"is_complete\": " + "\"" + cevent.is_complete + "\"," +
                  "\"has_parent_link\": " + "\"" + cevent.has_parent_link + "\"," +
                  "\"has_child_link\": " + "\"" + cevent.has_child_link + "\"," +
                  "\"estimate_name\": " + "\"" + cevent.estimate_name + "\"," +
                  "\"duration\": " + "\"" + cevent.duration + "\"," +
                  "\"className\": " + "\"" + serviceColor + "\"," +
                  "\"selectedweekends\": " + "\"" + cevent.selectedweekends + "\"" +
                  "},";
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    private string ConvertToTimestamp(DateTime value)
    {
        // var text = value.ToString("'\"'yyyy-MM-dd'T'HH:mm:ss'\"'", System.Globalization.CultureInfo.InvariantCulture); // 2014-11-10T10:00:00
        var text = value.ToString("yyyy-MM-dd'T'HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture); // 2014-11-10T10:00:00
        long epoch = (value.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        return text;
    }
}