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
using System.Drawing;

public partial class change_order_sections : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin018") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            DataClassesDataContext _db = new DataClassesDataContext();

            int ncid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = ncid.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstId = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstId.ToString();

            int nChEstId = Convert.ToInt32(Request.QueryString.Get("coestid"));
            hdnChEstId.Value = nChEstId.ToString();



            // Get Customer Information
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                if (cust != null)
                {
                    lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                    string strAddress = "";
                    strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                    lblAddress.Text = strAddress;
                    lblPhone.Text = cust.phone;
                    lblEmail.Text = cust.email;
                    hdnSalesPersonId.Value = cust.sales_person_id.ToString();

                    //hypGoogleMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
                    string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                    hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

                    hdnClientId.Value = cust.client_id.ToString();
                }

            }


            chkSections.DataSource = _db.sectioninfos.Where(si => si.client_id == Convert.ToInt32(hdnClientId.Value) && si.parent_id == 0 && si.is_active == true).ToList();
            chkSections.DataTextField = "section_name";
            chkSections.DataValueField = "section_id";
            chkSections.DataBind();

            // Get Estimate Info
            if (Convert.ToInt32(hdnEstimateId.Value) > 0)
            {
                customer_estimate cus_est = new customer_estimate();
                cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

                lblEstimateName.Text = cus_est.estimate_name;
            }
            // Get Change Order Info
            if (Convert.ToInt32(hdnChEstId.Value) > 0)
            {
                changeorder_estimate cho = new changeorder_estimate();
                cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.chage_order_id == Convert.ToInt32(hdnChEstId.Value));

                lblChangeOrderName.Text = cho.changeorder_name;
            }

            MarkExistingSections(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnEstimateId.Value));

            if (Request.QueryString.Get("csid") != null)
            {
                CheckExistingChangeOrderSections(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnEstimateId.Value));
                MarkExistingSections(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnEstimateId.Value));
            }
            else
            {
                // Check Change Order Location Exist.
                if (_db.changeorder_sections.Where(cs => cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).ToList().Count > 0)
                {
                    Response.Redirect("changeorder_pricing.aspx?coestid=" + hdnChEstId.Value + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
                }
            }
        }
    }

    protected void MarkExistingSections(int nCustomerId, int nEstimateId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = _db.customer_sections.Where(cs => cs.customer_id == nCustomerId && cs.estimate_id == nEstimateId);
        foreach (ListItem li in chkSections.Items)
        {
            foreach (customer_section cus_sec in item)
            {
                if (cus_sec.section_id == Convert.ToInt32(li.Value.ToString()))
                {
                    li.Attributes.CssStyle.Add("color", "red");
                    li.Attributes.CssStyle.Add("font-weight", "bold");
                }
            }
        }
    }


    protected void CheckExistingChangeOrderSections(int nCustomerId, int nEstimateId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = _db.changeorder_sections.Where(cs => cs.customer_id == nCustomerId && cs.estimate_id == nEstimateId);
        foreach (ListItem li in chkSections.Items)
        {
            foreach (changeorder_section cus_sec in item)
            {
                if (cus_sec.section_id == Convert.ToInt32(li.Value.ToString()))
                {
                    li.Selected = true;
                }
            }
        }
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnContinue.ID, btnContinue.GetType().Name, "Click"); 
        lblMessage.Text = "";

        if (chkSections.SelectedItem == null)
        {
            lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Please select change order sections.");
             
            return;
        }
         DataClassesDataContext _db = new DataClassesDataContext();
                string strQ = "DELETE changeorder_sections WHERE  customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + "  AND client_id=" + Convert.ToInt32(hdnClientId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
                _db.ExecuteCommand(strQ, string.Empty);
      
        for (int i = 0; i < chkSections.Items.Count; i++)
        {
            if (chkSections.Items[i].Selected == true)
            {
                int nSectionId = Convert.ToInt32(chkSections.Items[i].Value);
                // Add Customer locations
                changeorder_section co_sec = new changeorder_section();
                co_sec.client_id = Convert.ToInt32(hdnClientId.Value);
                co_sec.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                co_sec.section_id = nSectionId;
                co_sec.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                co_sec.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                _db.changeorder_sections.InsertOnSubmit(co_sec);

            }
        }
        _db.SubmitChanges();
        // Redirect to change order pricing page
        Response.Redirect("changeorder_pricing.aspx?coestid=" + hdnChEstId.Value + "&eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
    }
}
