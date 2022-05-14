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
/// Summary description for vendorInfrmation
/// </summary>
/// 

public class EstCom
{
    public int estimate_com_id { get; set; }
    public int client_id { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public decimal comission_amount { get; set; }



}
public class EstCO_Com
{
    public int co_estimate_com_id { get; set; }
    public int client_id { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public decimal comission_amount { get; set; }



}
public class VendorCost
{
    public int vendor_cost_id { get; set; }
    public int vendor_id { get; set; }
    public int client_id { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public int section_id { get; set; }
    public int category_id { get; set; }
    public string cost_description { get; set; }
    public decimal cost_amount { get; set; }
    public DateTime cost_date { get; set; }
    public DateTime create_date { get; set; }


}
public class vendorInfrmation
{
    public vendorInfrmation()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int vendor_id { get; set; }
    public int client_id { get; set; }
    public string vendor_name { get; set; }
    public bool is_active { get; set; }
    public DateTime create_date { get; set; }
}
public class PartialPayment
{
    public int payment_id { get; set; }
    public int pay_term_id { get; set; }
    public int pay_type_id { get; set; }
    public int client_id { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public string reference { get; set; }
    public decimal pay_amount { get; set; }
    public DateTime pay_date { get; set; }
    public DateTime create_date { get; set; }


}
public class PartialPayment_new
{
    public int payment_id { get; set; }
    public string pay_term_ids { get; set; }
    public string pay_term_desc { get; set; }
    public int pay_type_id { get; set; }
    public int client_id { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public string reference { get; set; }
    public decimal pay_amount { get; set; }
    public DateTime pay_date { get; set; }
    public DateTime create_date { get; set; }
    public string TransactionId { get; set; }
    public string CreditCardNum { get; set; }
    public string CreditCardType { get; set; }




}

public class COPartialPayment
{
    public int co_payment_id { get; set; }
    public int co_pay_term_id { get; set; }
    public int co_pay_type_id { get; set; }
    public int change_order_id { get; set; }
    public int client_id { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public string co_reference { get; set; }
    public decimal co_pay_amount { get; set; }
    public DateTime co_pay_date { get; set; }
    public DateTime create_date { get; set; }


}
public class COName
{

    public int change_order_id { get; set; }
    public string changeorder_name { get; set; }

}
public class COName_Tax
{

    public int change_order_id { get; set; }
    public string changeorder_name { get; set; }
    public decimal tax { get; set; }

}

public class PrjectNoteInfrmation
{
    public PrjectNoteInfrmation()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int ProjectNoteId { get; set; }
    public int customer_id { get; set; }
    public string NoteDetails { get; set; }
    public bool is_complete { get; set; }
    public DateTime ProjectDate { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreatedBy { get; set; }
    public int section_id { get; set; }
    public bool isSOW{ get; set; }
    public string MaterialTrack { get; set; }
    public string DesignUpdates { get; set; }
    public string SuperintendentNotes { get; set; }
    public string SectionName { get; set; }

}

public class csPrjectNoteReport
{
    public csPrjectNoteReport()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int ProjectNoteId { get; set; }
    public int customer_id { get; set; }
    public string first_name1 { get; set; }
    public string last_name1 { get; set; }
    public string customer_name { get; set; }
    public string NoteDetails { get; set; }
    public bool is_complete { get; set; }
    public DateTime ProjectDate { get; set; }
    public DateTime CreateDate { get; set; }
    public int section_id { get; set; }
    public bool isSOW { get; set; }
    public string MaterialTrack { get; set; }
    public string DesignUpdates { get; set; }
    public string SuperintendentNotes { get; set; }
    public string SectionName { get; set; }

}

public class LaborHour
{
    public int labor_entry_id { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public int section_id { get; set; }
    public decimal labor_hour { get; set; }
    public DateTime labor_date { get; set; }
    public int installer_id { get; set; }
    public int serial { get; set; }
    public string last_name { get; set; }


}

public class CrewTrack
{
    public int GPSTrackID { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public int section_id { get; set; }
    public string CustomerName { get; set; }
    public string SectionName { get; set; }
    public string StartPlace { get; set; }
    public string EndPlace { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string DeviceName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime labor_date { get; set; }
    public int UserID { get; set; }
    public int ID { get; set; }
    public string deleteBy { get; set; }
    public DateTime deleteDate { get; set; }
    public Boolean IsCrew { get; set; }
    public string StartCustomerAddress { get; set; }

    public string EndCustomerAddress { get; set; }
    public string full_name { get; set; }
    public string Notes { get; set; }
    public string WorkingDayName { get; set; }

}
public class csCrewList
{
    public int crew_id { get; set; }
    public int client_id { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string phone { get; set; }
    public Boolean is_active { get; set; }
    public string full_name { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public DateTime CreatedDate { get; set; }
  
    
    
}

public class csMyJobs
{
    public int customer_id { get; set; }
    public string ProjectName { get; set; }
    public string last_name { get; set; }
    public DateTime ScheduleDate { get; set; }


}

public class csCrewActivity
{

    public int CrewIId { get; set; }
    public int EventId { get; set; }
    public int CustomerId { get; set; }
    public DateTime ScheduleTime { get; set; }
    public DateTime ProcessRunTime { get; set; }
    public double CrewLatitude { get; set; }
    public double CrewLongitude { get; set; }
    public double CustLatitude { get; set; }
    public double CustLongitude { get; set; }
    public double Distance { get; set; }
    public int StatusType { get; set; }
    public string Status { get; set; }
    public string CustomerAddress { get; set; }
    public string crew_name { get; set; }
}