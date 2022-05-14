using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for csAdvancedReport
/// </summary>
public class csAdvancedReport
{
	public csAdvancedReport()
	{
		//
		// TODO: Add constructor logic here
		//
	}
   
        public int customer_id { get; set; }
        public string customer_name { get; set; }
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
        public string SalesRep { get; set; }
        public string Status { get; set; }
        public string lead_status_name { get; set; }
        public string LeadSource { get; set; }
        public DateTime registration_date { get; set; }
        public string notes { get; set; }
        public DateTime SaleDate { get; set; }
        public DateTime appointment_date { get; set; }
        public decimal SaleAmount { get; set; }
        public string estimate_name { get; set; }
        public string job_number { get; set; }
        public string Company { get; set; }
        public string status_note { get; set; }
        public decimal RecievedAmount { get; set; }
        public decimal DueAmount { get; set; }
    
        

}