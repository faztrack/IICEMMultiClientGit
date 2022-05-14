<%@ WebHandler Language="C#" Class="JsonResponsereadonly" %>

using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Web.SessionState;

public class JsonResponsereadonly : IHttpHandler, IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        DateTime start = new DateTime(1970, 1, 1);
        DateTime end = new DateTime(1970, 1, 1);
        var test = context.Request.QueryString["start"];
        start = Convert.ToDateTime(context.Request.QueryString["start"].ToString());//start.AddSeconds(double.Parse(context.Request.QueryString["start"])); // 
        end = Convert.ToDateTime(context.Request.QueryString["end"].ToString());// end.AddSeconds(double.Parse(context.Request.QueryString["end"])); //



        int nCusId = 0;
        string strSecName = "";
        string strSearchUserName = "";

        if (System.Web.HttpContext.Current.Session["rsCusId"] != null)
            nCusId = (int)System.Web.HttpContext.Current.Session["rsCusId"];

        if (System.Web.HttpContext.Current.Session["rsSecName"] != null)
            strSecName = (string)System.Web.HttpContext.Current.Session["rsSecName"];

        if (System.Web.HttpContext.Current.Session["rsKeySearchUserName"] != null)
            strSearchUserName = (string)System.Web.HttpContext.Current.Session["rsKeySearchUserName"];

        String result = String.Empty;

        result += "[";

        List<int> idList = new List<int>();
        foreach (CalendarEvent cevent in EventDAO.getEventsreadonly(start, end, nCusId, strSecName, strSearchUserName))
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

        //result = "[{"+
        //            "\"title\": \"Labor Day\","+
        //            "\"start\": \"2018-09-03T00:00:00\","+
        //            "\"end\": \"2018-09-08T00:00:00\""+
        //        "}]";

        //store list of event ids in Session, so that it can be accessed in web methods
        context.Session["idList"] = idList;

        context.Response.Write(result);
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

        // if (cevent.type_id == 5) // Only was For Holiday Event Item, Now for all item Time will not show before event title in Calendar
        {
            serviceColor += " hideCalendarTime";
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
                  "\"title\":" + jesonTitle + "," + //.Replace("'", "\\'")
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
                  "\"is_complete\": " + "\"" + cevent.is_complete + "\"," +
                  "\"has_parent_link\": " + "\"" + cevent.has_parent_link + "\"," +
                  "\"has_child_link\": " + "\"" + cevent.has_child_link + "\"," +
                  "\"className\": " + "\"" + serviceColor + "\"" +
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