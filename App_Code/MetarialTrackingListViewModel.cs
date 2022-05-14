using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MetarialTrackingListViewModel
/// </summary>
public class MetarialTrackingListViewModel
{
	public MetarialTrackingListViewModel(){}
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
    public int Order_id { get; set; }

   // Public int Order_id{ get; set; }

    public int Customer_id{ get; set; }

    public int Estimate_id{ get; set; }

    public int Section_id{ get; set; }

    public string Section_name{ get; set; }

    public int  Vendor_id { get; set; }

    public string Vendor_name{ get; set; }

    public string Item_text{ get; set; }

    public string Item_note{ get; set; }

    public string Shipped_by{ get; set; }

    public string Shipped_note{ get; set; }

    public string Received_by{ get; set; }

    public string Received_note{ get; set; }

    public string Picked_by{ get; set; }

    public string Picked_note{ get; set; }

    public string Confirmed_by{ get; set; }

    public string Confirmed_note{ get; set; }

    public DateTime Order_date { get; set; }

    public DateTime Create_date{ get; set; }

    public DateTime Last_update_date{ get; set; }

    public string CreateBy{ get; set; }

    public bool Is_Shipped{ get; set; }

    public bool Is_Received{ get; set; }

    public bool Is_Picked{ get; set; }

    public bool Is_Confirmed{ get; set; }

    public bool Is_Active{ get; set; }

    public DateTime Shipped_date{ get; set; }

    public DateTime Received_date{ get; set; }

    public DateTime Picked_date{ get; set; }

    public DateTime Confirmed_date{ get; set; }
}

