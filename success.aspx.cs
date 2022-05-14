using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class success : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            int nEstimateId = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstimateId.ToString();
            int nCustomerId = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCustomerId.ToString();
            int nEstPayId = Convert.ToInt32(Request.QueryString.Get("epid"));
            hdnEstPaymentId.Value = nEstPayId.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);

        }
    }
    protected void btnPaymentInfo_Click(object sender, EventArgs e)
    {
        Response.Redirect("payment.aspx?cid=" + hdnCustomerId.Value + "&epid=" + hdnEstPaymentId.Value + "&eid=" + hdnEstimateId.Value);
    }
    protected void btnCustomerDetails_Click(object sender, EventArgs e)
    {
        Response.Redirect("customer_details.aspx?cid=" + hdnCustomerId.Value);
    }
}
