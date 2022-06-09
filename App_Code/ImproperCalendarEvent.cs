using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//Do not use this object, it is used just as a go between between javascript and asp.net
public class ImproperCalendarEvent
{
    public int id { get; set; }
    public string title { get; set; }  
    public string description { get; set; }
    public string start { get; set; }
    public string end { get; set; }

    public int client_id { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public int employee_id { get; set; }
    public string section_name { get; set; }
    public string location_name { get; set; }
    public string cssClassName { get; set; }
    public int type_id { get; set; }
    public string operation_notes { get; set; }
    public string employee_name { get; set; }
    public int child_event_id { get; set; }
    public int dependencyType { get; set; }
    public int offsetDays { get; set; }
    public int parentDependencyType { get; set; }
    public int parentOffsetDays { get; set; }
    public string linkType { get; set; }
    public string customer_last_name { get; set; }
    public bool IsScheduleDayException { get; set; }
    public bool is_complete { get; set; }
    public bool has_link { get; set; }
    public string selectedweekends { get; set; }
}

public class ImproperCalendarLinkEvent
{
    public int link_id { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public string title { get; set; }
    public string start { get; set; }
    public string end { get; set; }
    public int parent_event_id { get; set; }
    public int child_event_id { get; set; }
    public string IsSuccess { get; set; }
    public int dependencyType { get; set; }
    public string dependency { get; set; }
    public string lag { get; set; }
    public int offsetDays { get; set; }
}
