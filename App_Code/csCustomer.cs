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
/// Summary description for csCustomer
/// </summary>
public class csCustomer
{
    public csCustomer()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int client_id { get; set; }
    public int customer_id { get; set; }
    public string first_name1 { get; set; }
    public string last_name1 { get; set; }
    public string first_name2 { get; set; }
    public string last_name2 { get; set; }
    public string address { get; set; }
    public string cross_street { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string zip_code { get; set; }
    public string fax { get; set; }
    public string email { get; set; }
    public string email2 { get; set; }
    public string phone { get; set; }
    public bool is_active { get; set; }
    public DateTime registration_date { get; set; }
    public int sales_person_id { get; set; }
    public DateTime update_date { get; set; }
    public int status_id { get; set; }
    public string notes { get; set; }
    public string customer_name { get; set; }
    public int lead_source_id { get; set; }
    public int SuperintendentId { get; set; }
    public string Sold_date { get; set; }

    public DateTime SaleDate { get; set; }
    public DateTime appointment_date { get; set; }
    public string company { get; set; }
    public int isCustomer { get; set; }
    public int islead { get; set; }
    public bool IsEstimateActive { get; set; }
    public string jobNumber { get; set; }
    public int customer_estimate_id { get; set; }

  


}
public class csCustomerCall
{
    public csCustomerCall()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int customer_id { get; set; }
    public string first_name1 { get; set; }
    public string last_name1 { get; set; }
    public string phone { get; set; }
    public string mobile { get; set; }
    public DateTime appointment_date { get; set; }
    public int lead_source_id { get; set; }
    public string company { get; set; }
    public int lead_status_id { get; set; }
    public int CallLogID { get; set; }
    public string CallDate { get; set; }
    public string CallHour { get; set; }
    public string CallMinutes { get; set; }
    public string CallAMPM { get; set; }
    public string CallDuration { get; set; }
    public string DurationHour { get; set; }
    public string DurationMinutes { get; set; }
    public string Description { get; set; }
    public DateTime CallDateTime { get; set; }
    public string CallSubject { get; set; }
    public int CallTypeId { get; set; }
    public string FollowDate { get; set; }
    public string FollowHour { get; set; }
    public string FollowMinutes { get; set; }
    public string FollowAMPM { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public DateTime FollowDateTime { get; set; }
    public bool IsFollowUp { get; set; }
    public string CreatedByUser { get; set; }
    public DateTime CreateDate { get; set; }
    public string lead_name { get; set; }
    public bool IsDoNotCall { get; set; }
    

}
public class csScheduleReport
{
    public csScheduleReport()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string title { get; set; }
    public string description { get; set; }
    public DateTime event_start { get; set; }
    public DateTime event_end { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public int employee_id { get; set; }
    public string section_name { get; set; }
    public string location_name { get; set; }
    public string cssClassName { get; set; }
    public string customer_name { get; set; }
    public string estimater_name { get; set; }
    public string employee_name { get; set; }
    public int type_id { get; set; }

}


public class csRC_Customer
{
    public csRC_Customer()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int customer_id { get; set; }
    public string CustomerName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Phone { get; set; }
    public string Mobile { get; set; }
    public string Fax { get; set; }
    public string Email { get; set; }
    public string SalesPerson { get; set; }
    public string Notes { get; set; }
    public string LeadSource { get; set; }
    public string Company { get; set; }
    public string Superintendent { get; set; }

}

public class csRC_SavedColumn
{
    public csRC_SavedColumn()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int rc_store_id { get; set; }
    public string user_name { get; set; }
    public string rc_tbl_columns { get; set; }
}

public class csZipMap
{
    public csZipMap()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string Zip { get; set; }
    public string SalesDescription { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}

    public class csSearchcustomer
    {
         public csSearchcustomer()
       {
        //
        // TODO: Add constructor logic here
        //
      }

         public int client_id { get; set; }
         public int customer_id { get; set; }
         public string first_name1 { get; set; }
         public string last_name1 { get; set; }
         public string first_name2 { get; set; }
         public string last_name2 { get; set; }
         public string address { get; set; }
         public string cross_street { get; set; }
         public string city { get; set; }
         public string state { get; set; }
         public string zip_code { get; set; }
         public string fax { get; set; }
         public string email { get; set; }
         public string email2 { get; set; }
         public string phone { get; set; }
         public string mobile { get; set; }
         public bool is_active { get; set; }
         public DateTime registration_date { get; set; }
         public int sales_person_id { get; set; }
         public DateTime update_date { get; set; }
         public int status_id { get; set; }
         public string notes { get; set; }
         public string customer_name { get; set; }
         public int lead_source_id { get; set; }
         public int SuperintendentId { get; set; }
         public string Sold_date { get; set; }

         public DateTime SaleDate { get; set; }
         public DateTime appointment_date { get; set; }
         public string company { get; set; }
         public int isCustomer { get; set; }
         public int islead { get; set; }
         public bool IsEstimateActive { get; set; }
         public string jobNumber { get; set; }
         public int customer_estimate_id { get; set; }
         public int searchCustomerId { get; set; }
         public DateTime searchDate { get; set; }
    }