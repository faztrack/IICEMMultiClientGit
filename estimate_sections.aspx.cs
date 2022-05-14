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

public partial class estimate_sections : System.Web.UI.Page
{
    int ListItemCount = 0;
    int checkItemCount = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin027") == false)
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

            chkSections.DataSource = _db.sectioninfos.Where(si => si.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && si.parent_id == 0 && si.is_active==true).OrderBy(s => s.section_name).ToList();
            chkSections.DataTextField = "section_name";
            chkSections.DataValueField = "section_id";
            chkSections.DataBind();

            // Get Customer Information
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

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

            }
            // Get Estimate Info
            if (Convert.ToInt32(hdnEstimateId.Value) > 0)
            {
                customer_estimate cus_est = new customer_estimate();
                cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

                lblEstimateName.Text = cus_est.estimate_name;
            }

            if (Request.QueryString.Get("sid") != null)
            {
                CheckExistingSections(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnEstimateId.Value));
                MarkExistItemToSections(Convert.ToInt32(hdnEstimateId.Value), Convert.ToInt32(hdnCustomerId.Value));

            }
            else
            {
                // Check Customer Location Exist.
                if (_db.customer_sections.Where(cs => cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).ToList().Count > 0)
                {
                    Response.Redirect("pricing.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
                }
            }


            csCommonUtility.SetPagePermission(this.Page, new string[] { "chkSections", "btnContinue"});
        }
    }
    protected void CheckExistingSections(int nCustomerId, int nEstimateId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = _db.customer_sections.Where(cs => cs.customer_id == nCustomerId && cs.estimate_id == nEstimateId);
        foreach (ListItem li in chkSections.Items)
        {
            ListItemCount++;
            foreach (customer_section cus_sec in item)
            {
                if (cus_sec.section_id == Convert.ToInt32(li.Value.ToString()))
                {
                      li.Attributes.CssStyle.Add("font-weight", "bold");
                    li.Selected = true;
                    checkItemCount++;
                  
                }
            }
        }

        if (ListItemCount == checkItemCount)
        {
            chkAll.Checked = true;
        }
        else
        {
            chkAll.Checked = false;
        }
    }
    private void MarkExistItemToSections(int nEstId, int nCustId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = _db.pricing_details.Where(cl => cl.customer_id == nCustId && cl.estimate_id == nEstId);
        foreach (ListItem li in chkSections.Items)
        {
            foreach (pricing_detail pd in item)
            {
                if (pd.section_level == Convert.ToInt32(li.Value.ToString()))
                {
                    li.Attributes.CssStyle.Add("font-weight", "bold");
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
            lblMessage.Text = csCommonUtility.GetSystemRequiredMessage("Please select estimate sections.");
             
            return;
        }
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "DELETE customer_sections WHERE customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        for (int i = 0; i < chkSections.Items.Count; i++)
        {
            if (chkSections.Items[i].Selected == true)
            {
                int nSectionId = Convert.ToInt32(chkSections.Items[i].Value);
                // Add Customer locations
                customer_section cus_sec = new customer_section();
                cus_sec.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                cus_sec.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                cus_sec.section_id = nSectionId;
                cus_sec.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                cus_sec.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                _db.customer_sections.InsertOnSubmit(cus_sec);
            }
        }
        _db.SubmitChanges();

        // Redirect to pricing page
        Response.Redirect("pricing.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
    }

    protected void chkAll_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkAll.ID, chkAll.GetType().Name, "CheckedChanged"); 
        DataClassesDataContext _db = new DataClassesDataContext();

        if (_db.pricing_details.Any(cl => cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value)))
            MarkExistItemToSections(Convert.ToInt32(hdnEstimateId.Value), Convert.ToInt32(hdnCustomerId.Value));
        else
        {
            var item = _db.customer_sections.Where(cs => cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
            foreach (ListItem li in chkSections.Items)
            {
                ListItemCount++;
                foreach (customer_section cus_sec in item)
                {
                    if (cus_sec.section_id == Convert.ToInt32(li.Value.ToString()))
                    {
                        li.Attributes.CssStyle.Add("font-weight", "bold");
                        li.Selected = true;
                        checkItemCount++;

                    }
                }
            }
        }

        if (chkAll.Checked)
        {
            foreach (ListItem li in chkSections.Items)
            {

                li.Selected = true;

            }
        }
        else
        {
            foreach (ListItem li in chkSections.Items)
            {

                li.Selected = false;

            }
        }

    }
    protected void chkSections_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkSections.ID, chkSections.GetType().Name, "SelectedIndexChanged"); 
        DataClassesDataContext _db = new DataClassesDataContext();

        if (_db.pricing_details.Any(cl => cl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cl.estimate_id == Convert.ToInt32(hdnEstimateId.Value)))
            MarkExistItemToSections(Convert.ToInt32(hdnEstimateId.Value), Convert.ToInt32(hdnCustomerId.Value));
        else
        {
            var item = _db.customer_sections.Where(cs => cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
            foreach (ListItem li in chkSections.Items)
            {
              
                foreach (customer_section cus_sec in item)
                {
                    if (cus_sec.section_id == Convert.ToInt32(li.Value.ToString()))
                    {
                        li.Attributes.CssStyle.Add("font-weight", "bold");
                      
                   }
                }
            }
        }
        checkItemCount = 0;
        foreach (ListItem li in chkSections.Items)
        {
            ListItemCount++;
            if (li.Selected)
            {
                checkItemCount++;
            }


        }

        if (ListItemCount == checkItemCount)
            chkAll.Checked = true;
        else
            chkAll.Checked = false;
    }
}
