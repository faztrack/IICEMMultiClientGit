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
/// Summary description for SalesStatisticsModel
/// </summary>
public class SalesStatisticsModel
{
	public SalesStatisticsModel()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public int client_id { get; set; }
    public int sales_person_id { get; set; }
    public string Sales_Person { get; set; }
    public decimal Total_Price { get; set; }
    public int NumberOfSales { get; set; }
    public int NumberOfAppt { get; set; }

}
