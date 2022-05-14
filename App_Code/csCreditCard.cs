using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for csCreditCard
/// </summary>
public class csCreditCard
{
	public csCreditCard()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public int PaymentProfileId { get; set; }
    public int CustomerId { get; set; }
    public string AuthorisedPaymentId { get; set; }
    public int AuthorisedCustomerId { get; set; }
    public string CardNumber { get; set; }
    public string NameOnCard { get; set; }
    public string CardType { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    public string LastUpdatedBy { get; set; }
    public string BillAddress { get; set; }
    public string BillCity { get; set; }
    public string BillState { get; set; }
    public string BillZip { get; set; }
    
}