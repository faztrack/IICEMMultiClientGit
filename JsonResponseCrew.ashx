<%@ WebHandler Language="C#" Class="JsonResponseCrew" %>

using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Web.SessionState;

public class JsonResponseCrew : IHttpHandler, IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        DateTime start = new DateTime(1970, 1, 1);
        DateTime end = new DateTime(1970, 1, 1);



        start = Convert.ToDateTime(context.Request.QueryString["start"].ToString());//start.AddSeconds(double.Parse(context.Request.QueryString["start"])); // 
        end = Convert.ToDateTime(context.Request.QueryString["end"].ToString());// end.AddSeconds(double.Parse(context.Request.QueryString["end"])); //

        int nSuperId = 0;
        int nEmpid = 0;
        string strEmpName = "";
        string strSecName = "";

        if (System.Web.HttpContext.Current.Session["superid"] != null)
            nSuperId = (int)System.Web.HttpContext.Current.Session["superid"];

        if (System.Web.HttpContext.Current.Session["sSecName"] != null)
            strSecName = (string)System.Web.HttpContext.Current.Session["sSecName"];

        if (System.Web.HttpContext.Current.Session["empid"] != null)
            nEmpid = (int)System.Web.HttpContext.Current.Session["empid"];

        if (System.Web.HttpContext.Current.Session["empname"] != null)
            strEmpName = System.Web.HttpContext.Current.Session["empname"].ToString();

        String result = String.Empty;

        result += "[";

        List<int> idList = new List<int>();
        foreach (CalendarEvent cevent in EventDAO.getEventsByCrewUserId(start, end, nSuperId, strSecName, nEmpid, strEmpName))
        //foreach (CalendarEvent cevent in EventDAO.getEventsByCusId(start, end))
        {
            result += convertCalendarEventIntoString(cevent);
            idList.Add(cevent.id);
        }

        if (result.EndsWith(","))
        {
            result = result.Substring(0, result.Length - 1);
        }

        result += "]";
        //store list of event ids in Session, so that it can be accessed in web methods
        context.Session["idList"] = idList;

        context.Response.Write(result);
    }

    private String convertCalendarEventIntoString(CalendarEvent cevent)
    {
        String allDay = "true";
        //  if (ConvertToTimestamp(cevent.start).ToString().Equals(ConvertToTimestamp(cevent.end).ToString()))
        //if (Newtonsoft.Json.JsonConvert.SerializeObject(cevent.start).ToString().Equals(Newtonsoft.Json.JsonConvert.SerializeObject(cevent.end).ToString()))
        //{

        //    if (cevent.start.Hour == 0 && cevent.start.Minute == 0 && cevent.start.Second == 0)
        //    {
        //        allDay = "true";
        //    }
        //    else
        //    {
        //        allDay = "false";
        //    }
        //}
        //else
        //{
        //    if (cevent.start.Hour == 0 && cevent.start.Minute == 0 && cevent.start.Second == 0
        //        && cevent.end.Hour == 0 && cevent.end.Minute == 0 && cevent.end.Second == 0)
        //    {
        //        allDay = "true";
        //    }
        //    else
        //    {
        //        allDay = "false";
        //    }
        //}

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

        string strTime = "";

        int ntest = 0;
        if (cevent.id == 6831)
            ntest = cevent.id;

        //TimeSpan start = new TimeSpan(8, 0, 0); //10 o'clock
        //TimeSpan end = new TimeSpan(12, 0, 0); //12 o'clock
        //TimeSpan now = cevent.start.TimeOfDay;

        //if ((now >= start) && (now <= end))
        //{
        //    strTime = "Morning";
        //}
        //else
        //{
        //    strTime = "Afternoon ";
        //}
        if (cevent.start.ToShortTimeString().Contains("AM"))
            strTime = "Morning";
        else
            strTime = "Afternoon";

        // if (cevent.type_id == 5) // Only was For Holiday Event Item, Now for all item Time will not show before event title in Calendar
        {
            serviceColor += " hideCalendarTime";
        }

        //string strTitle = "";
        //if (cevent.operation_notes.Trim() != "")
        //    strTitle = HttpContext.Current.Server.HtmlEncode(cevent.title.Replace("'", "\\'")) + " " + strTime + " - " + HttpContext.Current.Server.HtmlEncode(cevent.operation_notes.Replace("'", "\\'"));
        //else
        //    strTitle = HttpContext.Current.Server.HtmlEncode(cevent.title.Replace("'", "\\'"));

        string strDesciption = "";

        if (cevent.description.Trim().Length > 0)
        {
            strDesciption = "\n\r\tNotes: " + cevent.description.Replace("'", "\\'");
        }


      
        var jesonTitle = Newtonsoft.Json.JsonConvert.SerializeObject(cevent.title + strDesciption).ToString();


      

        var jesonDescription = Newtonsoft.Json.JsonConvert.SerializeObject(cevent.description).ToString();




        return "{" +
                 "\"id\": " + "\"" + cevent.id + "\"," +
                 "\"title\":" + jesonTitle + "," + 
                 "\"start\":" + "\"" + ConvertToTimestamp(cevent.start).ToString() + "\"" + "," +
                 "\"end\": " + "\"" + ConvertToTimestamp(cevent.end).ToString() + "\"" + "," +
                 "\"allDay\":" + "\"" + allDay + "\"," +               
                 "\"description\":" + jesonDescription + "," +
                 "\"EstimateID\": " + "\"" + cevent.estimate_id + "\"," +
                 "\"CustomerID\": " + "\"" + cevent.customer_id + "\"," +
                 "\"TypeID\": " + "\"" + cevent.type_id + "\"," +
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