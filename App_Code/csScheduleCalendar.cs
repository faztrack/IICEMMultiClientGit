using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for ScheduleCalendar
/// </summary>
public class csScheduleCalendar
{
	public csScheduleCalendar()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    //public int schedule_id { get; set; }
    //public int sales_person_id { get; set; }
    //public string SalesPerson { get; set; }
    //public int customer_id { get; set; }
    //public string Customer { get; set; }
    //public int estimate_id { get; set; }
    //public int location_id { get; set; }
    //public int section_level { get; set; }
    //public string Description { get; set; }   
    //public string Dates { get; set; }


    public int event_id  { get; set; }
	public string title { get; set; }
	public string description { get; set; }
    public DateTime event_start { get; set; }
    public DateTime event_end { get; set; }
	public int customer_id { get; set; }
	public int estimate_id { get; set; }
	public int employee_id { get; set; }
	public string section_name { get; set; }
	public string location_name { get; set; }
    public DateTime create_date { get; set; }
    public DateTime last_updated_date { get; set; }
	public string last_updated_by { get; set; }
	public int type_id { get; set; }
	public int parent_id { get; set; }
    public DateTime job_start_date { get; set; }
	public int co_pricing_list_id { get; set; }
	public string cssClassName { get; set; }
	public string google_event_id { get; set; }
	public string operation_notes { get; set; }
	public bool is_complete { get; set; }
	public bool IsEstimateActive { get; set; }


    
}
public class csScheduleSMS
{
    public int ScheduleSMSId { get; set; }

    public string title { get; set; }

    public int reference_id { get; set; }

    public int reference_type { get; set; }

    public int estimate_id { get; set; }

    public string mobile { get; set; }

    public string reponse { get; set; }

    public int event_id { get; set; }

    public bool is_success { get; set; }

    public DateTime send_date { get; set; }

    public DateTime schedule_date { get; set; }

    public DateTime create_date { get; set; }

    public string created_by { get; set; }

}

public class csScheduleNotification
{
    public int event_id { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public string first_name1 { get; set; }
    public string last_name1 { get; set; }
    public string customer_name { get; set; }
    public string title { get; set; }
    public string employee_name { get; set; }
    public string mobile { get; set; }
    public DateTime event_start { get; set; }
    public DateTime event_end { get; set; }
    public string section_name { get; set; }
    public string location_name { get; set; }
    public string superfirstname { get; set; }
    public string superlastname { get; set; }
    public string supermobile { get; set; }
    public string CustAddress { get; set; }
    public bool IsScheduleDayException { get; set; }
}