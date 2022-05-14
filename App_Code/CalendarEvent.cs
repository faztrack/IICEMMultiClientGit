using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class CalendarEvent
{
    public int id { get; set; }
    public string title { get; set; } 
    public string description { get; set; }  
    public DateTime start { get; set; }
    public DateTime end { get; set; }    
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
    public bool has_parent_link { get; set; }
    public bool has_child_link { get; set; }

    public string estimate_name { get; set; }
    public int duration { get; set; }

    public string SuperintendentName { get; set; }
    public string selectedweekends { get; set; }
    public string WeekEnds { get; set; }
}
