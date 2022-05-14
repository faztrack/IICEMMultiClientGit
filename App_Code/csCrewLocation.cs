using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for csCrewLocation
/// </summary>
public class csCrewLocation
{
	public csCrewLocation()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public int event_id { get; set; }
    public int CrewId { get; set; }
    public int CustomerId { get; set; }

    public string employee_name { get; set; }
    public DateTime event_start { get; set; }
    public DateTime event_end { get; set; }

    public string CrewLatitude { get; set; }
    public string CrewLongitude { get; set; }

    public string CustLatitude { get; set; }
    public string CustLongitude { get; set; }

    public string SuperintendentLatitude { get; set; }
    public string SuperintendentLongitude { get; set; }
    public string CfullName { get; set; }

     public string superName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CustomerAddress { get; set; }
}
public class CrewLocationTracker
{



    public string StartLatitude { get; set; }
    public string StartLogitude { get; set; }
    public string EndLatitude { get; set; }
    public string EndLogitude { get; set; }
    public string CrewfullName { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public int StartCount { get; set; }
    public double StartTimeDistance { get; set; }
    public double EndTimeDistance { get; set; }
    public string CustomerName { get; set; }
    public string StartPlace { get; set; }
    public string EndPlace { get; set; }
    public DateTime labor_date { get; set; }

}
public class CustomerInfoCrewLocationTracker
{
    public string CustomerLat { get; set; }
    public string Customerlong { get; set; }
    public string customerName { get; set; }
    public string cuAddress { get; set; }
}
