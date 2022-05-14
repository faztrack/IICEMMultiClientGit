using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for csSiteReview
/// </summary>
public class csSiteReview
{
	public csSiteReview()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int SiteReviewsId { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
    public int StateOfMindID { get; set; }
    public string SiteReviewsNotes { get; set; }
    public DateTime SiteReviewsDate { get; set; }
    public bool IsUserView { get; set; }
    public bool IsCustomerView { get; set; }
    public bool IsVendorView { get; set; }
    public bool HasAttachments { get; set; }
    public string AttachmentList { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreateDate { get; set; }
    public string LastUpdatedBy { get; set; }
    public DateTime LastUpdateDate { get; set; }
}

public class csSiteReviewUpload
{
	public csSiteReviewUpload()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public int SiteReview_attach_id { get; set; }
    public int SiteReviewsId { get; set; }
    public int customer_id { get; set; }
    public int estimate_id { get; set; }
   public string SiteReview_file_name { get; set; }
  
   
}