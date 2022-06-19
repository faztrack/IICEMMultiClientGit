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
using System.Net;
using System.Text.RegularExpressions;

public partial class companyprofile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!IsPostBack)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin001") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            DataClassesDataContext _db = new DataClassesDataContext();
            var states = from st in _db.states
                         orderby st.abbreviation
                         select st;
            ddlState.DataSource = states;
            ddlState.DataTextField = "abbreviation";
            ddlState.DataValueField = "abbreviation";
            ddlState.DataBind();

            BindDivision();
            GetCompanyProfile();



            btnSave.Attributes.Add("OnClick", "return ValueChanged();");


            csCommonUtility.SetPagePermission(this.Page, new string[] { "lnkCusServiceEmailAddress", "lnkEmailBilling", "txtEmailInfoPassword", "rdoCompletionType", "chkShowSOW", "rdoChangeOrderQTY", "rdoChangeOrderQTY", "chkIsShowPriceInSOW", "btnSave" });
        }
    }

    private void GetCompanyProfile()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            company_profile com = new company_profile();
            if (_db.company_profiles.Any(cp => cp.client_id == Convert.ToInt32(ddlDivision.SelectedValue)))
            {
               com = _db.company_profiles.SingleOrDefault(cp => cp.client_id == Convert.ToInt32(ddlDivision.SelectedValue));
               if(com != null)
               {
                    txtCompanyName.Text = com.company_name;
                    txtAddress.Text = com.address;
                    txtCity.Text = com.city;
                    ddlState.SelectedValue = com.state;
                    txtZipCode.Text = com.zip_code;
                    txtPhone.Text = com.phone;
                    txtFax.Text = com.fax;
                    txtEmail.Text = com.email;
                    txtContractEmail.Text = com.contract_email;
                    txtCOEmail.Text = com.co_email; // using as Transaction related Emails
                    txtProjectNotesEmail.Text = com.ProjectNotesEmail;
                    txtChangeOrdersEmail.Text = com.ChangeOrdersEmail;
                    txtSelectionEmail.Text = com.SelectionEmail;
                    txtWebsite.Text = com.website;
                    txtCustomerPortalURL.Text = com.CustomerPortalURL;
                    //txtMarkup.Text = com.markup.ToString();
                    hdnMarkup.Value = com.markup.ToString();
                    hdnCompanyProfileId.Value = com.company_profile_id.ToString();
                    if (com.CompletionTypeId != null)
                        rdoCompletionType.SelectedValue = com.CompletionTypeId.ToString();
                    if (com.ChangeQtyView != null)
                        rdoChangeOrderQTY.SelectedValue = com.ChangeQtyView.ToString();
                    txtExCostDecrease.Text = com.ExCostPercentage.ToString();
               }                
            }
            else
            {
                Reset();
            }
        }
        catch
        {

        }
    }

    private void Reset()
    {
        txtCompanyName.Text = "";
        txtAddress.Text = "";
        txtCity.Text = "";
        ddlState.SelectedIndex = 3;
        txtZipCode.Text = "";
        txtPhone.Text = "";
        txtFax.Text = "";
        txtEmail.Text = "";
        txtContractEmail.Text = "";
        txtCOEmail.Text = ""; // using as Transaction related Emails
        txtProjectNotesEmail.Text = ""; ;
        txtChangeOrdersEmail.Text = "";
        txtSelectionEmail.Text = "";
        txtWebsite.Text = ""; 
        txtCustomerPortalURL.Text = "";
        hdnMarkup.Value = "0";
        hdnCompanyProfileId.Value = "0";
        rdoCompletionType.SelectedValue = "1";
        txtExCostDecrease.Text = "";
    }


    private void BindDivision()
    {
        try
        {
            string sql = "select id, division_name from division order by division_name";
            DataTable dt = csCommonUtility.GetDataTable(sql);
            ddlDivision.DataSource = dt;
            ddlDivision.DataTextField = "division_name";
            ddlDivision.DataValueField = "id";
            ddlDivision.DataBind();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.ToString());
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match1 = regex.Match(txtContractEmail.Text.Trim());
       

        if (txtContractEmail.Text.Trim() != "")
        {
            if (!match1.Success)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Invalid email address.");
                return;
            }
        }

        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        lblResult.Text = "";
        if (txtCompanyName.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Company Name.");

            return;
        }
        if (txtAddress.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Address.");

            return;
        }
        if (txtCity.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: City.");

            return;
        }
        if (txtZipCode.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Zip Code.");

            return;
        }
        
        if (txtPhone.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Phone.");

            return;
        }
        if (txtCOEmail.Text.Length > 999)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Transaction related Email field Maximum length exceeded");

            return;
        }

        if (txtCOEmail.Text.Trim().Length != 0)
        {
            if (!csCommonUtility.ValidMultipleEmail(txtCOEmail.Text.Trim()))
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Valid Transaction Email is required and multiple emails separated by comma");
                return;
            }
        }

        if (txtProjectNotesEmail.Text.Length > 999)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Project Notes related Email field Maximum length exceeded");

            return;
        }

        if (txtProjectNotesEmail.Text.Trim().Length != 0)
        {
            if (!csCommonUtility.ValidMultipleEmail(txtProjectNotesEmail.Text.Trim()))
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Valid Project Notes Email is required and multiple emails separated by comma");
                return;
            }
        }

        if (txtChangeOrdersEmail.Text.Length > 999)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Change Orders related Email field Maximum length exceeded");

            return;
        }

        if (txtChangeOrdersEmail.Text.Trim().Length != 0)
        {
            if (!csCommonUtility.ValidMultipleEmail(txtChangeOrdersEmail.Text.Trim()))
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Valid Change Orders Email is required and multiple emails separated by comma");


                return;
            }
        }

        if (txtSelectionEmail.Text.Length > 999)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Selection related Email field Maximum length exceeded");

            return;
        }

        if (txtSelectionEmail.Text.Trim().Length != 0)
        {
            if (!csCommonUtility.ValidMultipleEmail(txtSelectionEmail.Text.Trim()))
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Valid Selection Email is required and multiple emails separated by comma");
                return;
            }
        }

        if (txtCustomerPortalURL.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Customer Portal URL.");

            return;
        }
        //if (txtMarkup.Text.Trim() == "")
        //{
        //    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Markup.");

        //    return;
        //}
        //else
        //{
        //    try
        //    {
        //        Convert.ToDecimal(txtMarkup.Text.Trim());
        //    }
        //    catch (Exception ex)
        //    {
        //        lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Markup.");

        //        return;
        //    }
        //}

        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile com = new company_profile();

        //update 
        if (_db.company_profiles.Any(cp => cp.client_id == Convert.ToInt32(ddlDivision.SelectedValue)))
        {
            com = _db.company_profiles.SingleOrDefault(cp => cp.client_id == Convert.ToInt32(ddlDivision.SelectedValue));
            if (com == null)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Data not found");
                return;
            }                
        }
       
       
       



        //if (_db.company_profiles.Where(cp => cp.client_id == Convert.ToInt32(ddlDivision.SelectedValue)).SingleOrDefault() != null)
             

        txtPhone.Text = csCommonUtility.GetPhoneFormat(txtPhone.Text.Trim());
        txtFax.Text = csCommonUtility.GetPhoneFormat(txtFax.Text.Trim());

        com.client_id = Convert.ToInt32(ddlDivision.SelectedValue);
        com.company_name = txtCompanyName.Text;
        com.address = txtAddress.Text;
        com.city = txtCity.Text;
        com.state = ddlState.SelectedItem.Text;
        com.zip_code = txtZipCode.Text;
        com.phone = txtPhone.Text;
        com.fax = txtFax.Text;
        com.email = txtEmail.Text;
        com.contract_email = txtContractEmail.Text;
        com.co_email = txtCOEmail.Text;  // using as Transaction related Emails
        com.ProjectNotesEmail = txtProjectNotesEmail.Text;
        com.ChangeOrdersEmail = txtChangeOrdersEmail.Text;
        com.SelectionEmail = txtSelectionEmail.Text;
        com.website = txtWebsite.Text;
        com.CustomerPortalURL = txtCustomerPortalURL.Text;
        com.markup = Convert.ToDecimal("1.85");
        com.CompletionTypeId = Convert.ToInt32(rdoCompletionType.SelectedValue);
        com.ChangeQtyView = Convert.ToInt32(rdoChangeOrderQTY.SelectedValue);
        if (txtExCostDecrease.Text != "")
        {
            com.ExCostPercentage = Convert.ToDecimal(txtExCostDecrease.Text.Replace("%", ""));
        }
        else
        {
            com.ExCostPercentage = 0;
        }
        //string strQ = "DELETE company_profile where client_id=" + Convert.ToInt32(hdnClientId.Value);
        //_db.ExecuteCommand(strQ, string.Empty);
        //if (hdnSID.Value == "1")
        //{
        //    decimal dRetail_multiplier = Convert.ToDecimal(txtMarkup.Text.Trim());
        //    string strQMarkup = "UPDATE item_price SET retail_multiplier =" + Convert.ToDecimal(txtMarkup.Text.Trim());
        //    _db.ExecuteCommand(strQMarkup, string.Empty);
        //    string strQ1 = "UPDATE model_estimate_pricing SET item_cost = (round((item_cost/retail_multiplier),2))*" + dRetail_multiplier + ", retail_multiplier=" + dRetail_multiplier + ",total_retail_price=(round((item_cost/retail_multiplier),2)+labor_rate)*" + dRetail_multiplier + "*quantity WHERE is_direct=1 AND retail_multiplier>0 AND labor_id=2";
        //    _db.ExecuteCommand(strQ1, string.Empty);
        //    string strQ2 = "UPDATE model_estimate_pricing SET item_cost = (round((item_cost/retail_multiplier),2))*" + dRetail_multiplier + ", retail_multiplier=" + dRetail_multiplier + ",total_direct_price=(round((item_cost/retail_multiplier),2)+labor_rate)*" + dRetail_multiplier + "*quantity WHERE is_direct=2 AND retail_multiplier>0 AND labor_id=2";
        //    _db.ExecuteCommand(strQ2, string.Empty);
        //    string strQ3 = "UPDATE model_estimate_pricing SET item_cost = (round((item_cost/retail_multiplier),2))*" + dRetail_multiplier + ", retail_multiplier=" + dRetail_multiplier + ",total_retail_price=(round((item_cost/retail_multiplier),2))*" + dRetail_multiplier + "*quantity WHERE is_direct=1 AND retail_multiplier>0 AND labor_id=1";
        //    _db.ExecuteCommand(strQ3, string.Empty);
        //    string strQ4 = "UPDATE model_estimate_pricing SET item_cost = (round((item_cost/retail_multiplier),2))*" + dRetail_multiplier + ", retail_multiplier=" + dRetail_multiplier + ",total_direct_price=(round((item_cost/retail_multiplier),2))*" + dRetail_multiplier + "*quantity WHERE is_direct=2 AND retail_multiplier>0 AND labor_id=1";
        //    _db.ExecuteCommand(strQ4, string.Empty);
        //}

        // _db.SubmitChanges();
        if (Convert.ToInt32(hdnCompanyProfileId.Value) == 0)
        {
            _db.company_profiles.InsertOnSubmit(com);
            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");
        }
        else
        {
            lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");
        }
            

        _db.SubmitChanges();

        


    }
    protected void btnClose_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
    //protected void txtMarkup_TextChanged(object sender, EventArgs e)
    //{
    //    if (txtMarkup.Text.Trim() == "")
    //    {
    //        lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing  Markup.");

    //        return;
    //    }
    //    else
    //    {
    //        try
    //        {
    //            Convert.ToDecimal(txtMarkup.Text.Trim());
    //        }
    //        catch (Exception ex)
    //        {
    //            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid  Markup.");

    //            return;
    //        }
    //    }
    //    if (Convert.ToDecimal(txtMarkup.Text.Trim()) == Convert.ToDecimal(hdnMarkup.Value.Trim()))
    //    {
    //        hdnSID.Value = "0";
    //    }
    //    else
    //    {
    //        hdnSID.Value = "1";
    //    }
    //}


    protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblResult.Text = "";
        GetCompanyProfile();
    }
}
