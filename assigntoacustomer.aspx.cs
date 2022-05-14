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
using System.Collections.Generic;
using System.Drawing;
using System.Web.Services;

public partial class assigntoacustomer : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetCompany(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.company.ToLower().StartsWith(prefixText.ToLower())
                    select c.company).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.company.StartsWith(prefixText)
                    select c.company).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.last_name1.ToLower().StartsWith(prefixText.ToLower())
                    select c.last_name1).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.last_name1.StartsWith(prefixText)
                    select c.last_name1).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetFirstName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.first_name1.ToLower().StartsWith(prefixText.ToLower())
                    select c.first_name1).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.first_name1.StartsWith(prefixText)
                    select c.first_name1).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetAddress(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.address.ToLower().StartsWith(prefixText.ToLower())
                    select c.address).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.address.StartsWith(prefixText)
                    select c.address).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetEmail(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.email.ToLower().StartsWith(prefixText.ToLower())
                    select c.email).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.email.StartsWith(prefixText)
                    select c.email).Take<String>(count).ToArray();
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Request.QueryString.Get("meid") != null)
                hdnModelEstimateId.Value = Request.QueryString.Get("meid");
            if (Request.QueryString.Get("spid") != null)
            {
                hdnSalesPersonId.Value = Convert.ToInt32(Request.QueryString.Get("spid")).ToString();
            }
            else
            {
                userinfo obj = (userinfo)Session["oUser"];
                hdnSalesPersonId.Value = obj.sales_person_id.ToString();
            }
            DataClassesDataContext _db = new DataClassesDataContext();

            sales_person sp = new sales_person();
            sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
            lblSalesPersonName.Text = sp.first_name + " " + sp.last_name;
            lblAddress.Text = sp.address;
            lblPhone.Text = sp.phone;
            lblEmail.Text = sp.email;

            var item = from cus in _db.customers
                       where cus.status_id != 5 && cus.status_id != 4
                       select cus;
            if (Convert.ToInt32(hdnSalesPersonId.Value) > 0)
            {
                item = from cus in _db.customers
                       where cus.status_id != 5 && cus.status_id != 4 && cus.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value)
                       select cus;

            }
            List<customer> LeadList = item.ToList();
            Session.Add("cSearch", LeadList);
            //if (Convert.ToInt32(hdnSalesPersonId.Value) > 0)
            //{
            //    var itemCustomer = from cus in _db.customers
            //                       where cus.status_id != 5 && cus.status_id != 4 && cus.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && cus.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
            //                       orderby cus.first_name1 ascending, cus.registration_date descending
            //                       select new CustomerModel()
            //                       {
            //                           customer_id = (int)cus.customer_id,
            //                           customer_name = cus.first_name1 + " " + cus.last_name1

            //                       };

            //    ddlCustomers.DataSource = itemCustomer;
            //    ddlCustomers.DataTextField = "customer_name";
            //    ddlCustomers.DataValueField = "customer_id";
            //    ddlCustomers.DataBind();
            //    ddlCustomers.Items.Insert(0, "Select Customer");
            //    ddlCustomers.SelectedValue = "0";
            //}
            //else 
            //{
            //    var itemCustomer = from cus in _db.customers
            //                       where cus.status_id != 5 && cus.status_id != 4 && cus.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
            //                       orderby cus.registration_date descending, cus.last_name1 ascending
            //                       select new CustomerModel()
            //                       {
            //                           customer_id = (int)cus.customer_id,
            //                           customer_name = cus.first_name1 + " " + cus.last_name1

            //                       };

            //    ddlCustomers.DataSource = itemCustomer;
            //    ddlCustomers.DataTextField = "customer_name";
            //    ddlCustomers.DataValueField = "customer_id";
            //    ddlCustomers.DataBind();
            //    ddlCustomers.Items.Insert(0, "Select Customer");
            //    ddlCustomers.SelectedValue = "0";
            //}

            model_estimate me = new model_estimate();
            me = _db.model_estimates.Single(mest => mest.model_estimate_id == Convert.ToInt32(hdnModelEstimateId.Value) && mest.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

            lblModelEstimateName.Text = me.model_estimate_name;
            lblCreateDate.Text = Convert.ToDateTime(me.create_date).ToShortDateString();
            lblLastUpdatedDate.Text = Convert.ToDateTime(me.last_udated_date).ToShortDateString();
            lblNewEstimateName.Text = me.model_estimate_name;
        }
    }
    protected void btnGotoModelEstimate_Click(object sender, EventArgs e)
    {
        Response.Redirect("me_pricing.aspx?meid=" + hdnModelEstimateId.Value + "&spid=" + hdnSalesPersonId.Value);
    }
    protected void GetCustomersNew(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        grdLeadList.PageIndex = nPageNo;
        string strCondition = "";

        if (txtSearch.Text.Trim() != "")
        {
            string str = txtSearch.Text.Trim();
            if (ddlSearchBy.SelectedValue == "1")
            {
                strCondition = " customers.first_name1 LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "2")
            {
                strCondition = "  customers.last_name1 LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "3")
            {

                strCondition = "  customers.email LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "4")
            {
                strCondition = "  customers.address LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "6")
            {
                strCondition = "  customers.company LIKE '%" + str + "%'";
            }
        }

        if (strCondition.Length > 0)
        {
            strCondition = " and " + strCondition;
        }
        string strQ = string.Empty;

        if (Convert.ToInt32(hdnSalesPersonId.Value) > 0)
        {
            strQ = " SELECT client_id, customers.customer_id, first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                         " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,company,islead,isCustomer " +
                         " FROM customers " +
                         " WHERE customers.status_id NOT IN(4,5) AND customers.sales_person_id = " + Convert.ToInt32(hdnSalesPersonId.Value) + "  " + strCondition + " order by last_name1 asc,registration_date desc";
        }
        else
        {
            strQ = " SELECT client_id, customers.customer_id, first_name1+' '+ last_name1 AS customer_name, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date, " +
                        " sales_person_id, update_date, status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId,company,islead,isCustomer " +
                        " FROM customers " +
                        " WHERE customers.status_id NOT IN(4,5) " + strCondition + " order by last_name1 asc,registration_date desc";
        }

        DataTable dt = csCommonUtility.GetDataTable(strQ);
        Session.Add("sCustList", dt);
        grdLeadList.DataSource = dt;
        grdLeadList.DataKeyNames = new string[] { "customer_id", "sales_person_id", "status_id", "isCustomer", "islead", "first_name1", "last_name1" };
        grdLeadList.DataBind();

        if (grdLeadList.Rows.Count == 1)
        {
            int index = 0;
            GridViewRow row = grdLeadList.Rows[index];
            grdLeadList.SelectedIndex = row.RowIndex;
            row.BackColor = ColorTranslator.FromHtml("#efe2e2");
            int nCustID = Convert.ToInt32(grdLeadList.DataKeys[row.RowIndex].Values[0]);

            string firstName = grdLeadList.DataKeys[row.RowIndex].Values[5].ToString();
            string LastName = grdLeadList.DataKeys[row.RowIndex].Values[6].ToString();
            hdnHdnPrevIndex.Value = index.ToString();





            lblResult.Text = "";
            lblError.Text = "";
            if (nCustID > 0)
            {
                hdnCustomerId.Value = nCustID.ToString();
                Session.Add("CustomerId", hdnCustomerId.Value);
                hdnCustName.Value = firstName + " " + LastName;
                lblSelectedCustName.Text = firstName + " " + LastName;

                // Model Estimate Sections
                var section = from sec in _db.sectioninfos
                              join mes in _db.model_estimate_sections on sec.section_id equals mes.section_id
                              where mes.model_estimate_id == Convert.ToInt32(hdnModelEstimateId.Value) && mes.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mes.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                              select new SectionInfo()
                              {
                                  section_id = (int)mes.section_id,
                                  section_name = sec.section_name
                              };

                chkModelEstimateSections.DataSource = section;
                chkModelEstimateSections.DataTextField = "section_name";
                chkModelEstimateSections.DataValueField = "section_id";
                chkModelEstimateSections.DataBind();

                tblModelEstimateSections.Visible = true;

                // Model Estimate Locations
                var Locations = from loc in _db.locations
                                join mel in _db.model_estimate_locations on loc.location_id equals mel.location_id
                                where mel.model_estimate_id == Convert.ToInt32(hdnModelEstimateId.Value) && mel.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mel.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                select new LocationModel()
                                {
                                    location_id = (int)mel.location_id,
                                    location_name = loc.location_name
                                };

                chkModelEstimatelocations.DataSource = Locations;
                chkModelEstimatelocations.DataTextField = "location_name";
                chkModelEstimatelocations.DataValueField = "location_id";
                chkModelEstimatelocations.DataBind();

                tblModelEstimateLocations.Visible = true;


                // Customer Estimates without (Sold)
                var CustomerEstimate = from cest in _db.customer_estimates
                                       where cest.status_id != 3 && cest.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                       orderby cest.estimate_name ascending
                                       select cest;
                int n = CustomerEstimate.Count();
                if (CustomerEstimate != null && n > 0)
                {
                    tblExistingEstimates.Visible = true;
                    tblNoExistingEstimates.Visible = false;

                    ddlChoseEstimate.DataSource = CustomerEstimate;
                    ddlChoseEstimate.DataTextField = "estimate_name";
                    ddlChoseEstimate.DataValueField = "estimate_id";
                    ddlChoseEstimate.DataBind();
                    ddlChoseEstimate.Items.Insert(0, "Choose Estimate");
                    ddlChoseEstimate.SelectedIndex = 0;
                }
                else
                {
                    tblNoExistingEstimates.Visible = true;
                    tblExistingEstimates.Visible = false;
                }

            }
            else
            {
                tblNoExistingEstimates.Visible = false;
                tblExistingEstimates.Visible = false;
                tblModelEstimateSections.Visible = false;
                tblModelEstimateLocations.Visible = false;
                return;
            }
        }

    }
    protected void btnAssign_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAssign.ID, btnAssign.GetType().Name, "Click"); 
        lblResult.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        if (hdnCustomerId.Value == "0")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please select Assign to Customer.");

            return;
        }
        if (tblExistingEstimates.Visible == true)
        {
            if (ddlChoseEstimate.SelectedItem.Text == "Choose Estimate")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please select Choose Estimate.");

                return;
            }
            else
            {
                if (chkModelEstimateSections.SelectedItem == null)
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please Select Estimate Sections.");

                    return;
                }
                if (chkModelEstimatelocations.SelectedItem == null)
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please Select Estimate Locations.");

                    return;
                }
            }


            // Update Customer Estimate New Payment
            model_estimate_payment objes = _db.model_estimate_payments.SingleOrDefault(mest => mest.estimate_id == Convert.ToInt32(hdnModelEstimateId.Value) && mest.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

            if (objes != null)
            {
               // estimate_payment mod_es = new estimate_payment();
                estimate_payment mod_es = _db.estimate_payments.SingleOrDefault(mest => mest.estimate_id == Convert.ToInt32(hdnCustomerEstimateId.Value) && mest.customer_id == Convert.ToInt32(hdnCustomerId.Value) && mest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
                if (mod_es!= null)
                {
                    mod_es.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    mod_es.client_id = objes.client_id;
                    mod_es.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                    mod_es.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                    mod_es.project_subtotal =objes.project_subtotal;
                    mod_es.tax_rate = objes.tax_rate;
                    mod_es.tax_amount =objes.tax_amount;
                    mod_es.total_with_tax =objes.total_with_tax;
                    mod_es.adjusted_price = objes.adjusted_price;
                    mod_es.adjusted_tax_amount = objes.adjusted_tax_amount;
                    mod_es.adjusted_tax_rate = objes.adjusted_tax_rate;
                    mod_es.new_total_with_tax = objes.new_total_with_tax;
                    mod_es.is_incentives = objes.is_incentives;
                    mod_es.total_incentives = objes.total_incentives;
                    mod_es.incentive_ids =objes.incentive_ids;
                    mod_es.deposit_percent = objes.deposit_percent;
                    mod_es.deposit_amount = objes.deposit_amount;
                    mod_es.countertop_percent = objes.countertop_percent;
                    mod_es.countertop_amount = objes.countertop_amount;
                    mod_es.start_job_percent = objes.start_job_percent;
                    mod_es.start_job_amount = objes.start_job_amount;
                    mod_es.due_completion_percent = objes.due_completion_percent;
                    mod_es.due_completion_amount = objes.due_completion_amount;
                    mod_es.final_measure_percent = objes.final_measure_percent;
                    mod_es.final_measure_amount = objes.final_measure_amount;
                    mod_es.deliver_caninet_percent = objes.deliver_caninet_percent;
                    mod_es.deliver_cabinet_amount = objes.deliver_cabinet_amount;
                    mod_es.substantial_percent = objes.substantial_percent;
                    mod_es.substantial_amount = objes.substantial_amount;
                    mod_es.other_value = objes.other_value;
                    mod_es.other_percent = objes.other_percent;
                    mod_es.other_amount = objes.other_amount;
                    mod_es.based_on_percent = objes.based_on_percent;
                    mod_es.based_on_dollar = objes.based_on_dollar;
                    mod_es.create_date = objes.create_date;
                    mod_es.updated_date = objes.updated_date;
                   
                    mod_es.deposit_date = objes.deposit_date;
                    mod_es.countertop_date = objes.countertop_date;
                    mod_es.startof_job_date = objes.startof_job_date;
                    mod_es.due_completion_date = objes.due_completion_date;
                    mod_es.measure_date = objes.measure_date;
                    mod_es.delivery_date = objes.deposit_date;
                    mod_es.substantial_date = objes.substantial_date;
                    mod_es.other_date = objes.other_date;
                    mod_es.lead_time = objes.lead_time;
                    mod_es.contract_date = objes.contract_date;
                    mod_es.deposit_value = objes.deposit_value;
                    mod_es.countertop_value = objes.countertop_value;
                    mod_es.start_job_value = objes.start_job_value;
                    mod_es.due_completion_value = objes.due_completion_value;
                    mod_es.final_measure_value = objes.final_measure_value;
                    mod_es.deliver_caninet_value = objes.deliver_caninet_value;
                    mod_es.substantial_value = objes.substantial_value;
                    mod_es.special_note = objes.special_note;
                    mod_es.start_date = objes.start_date;
                    mod_es.completion_date = objes.completion_date;
                    mod_es.is_KithenSheet = objes.is_KithenSheet;
                    mod_es.is_BathSheet = objes.is_BathSheet;
                    mod_es.is_ShowerSheet = objes.is_ShowerSheet;
                    mod_es.is_TubSheet = objes.is_TubSheet;
                    mod_es.drywall_percent = objes.drywall_percent;
                    mod_es.drywall_amount = objes.drywall_amount;
                    mod_es.drywall_date = objes.drywall_date;
                    mod_es.drywall_value = objes.drywall_value;
                    mod_es.flooring_percent = objes.flooring_percent;
                    mod_es.flooring_amount = objes.flooring_amount;
                    mod_es.flooring_date = objes.flooring_date;
                    mod_es.flooring_value = objes.flooring_value;
                    _db.SubmitChanges();
                }
            }

            // insert Customer Estimate New Sections (Not Exist in Estimate but in Exist in Model Estimate)
            for (int i = 0; i < chkModelEstimateSections.Items.Count; i++)
            {
                if (chkModelEstimateSections.Items[i].Selected == true)
                {
                    int nSectionId = Convert.ToInt32(chkModelEstimateSections.Items[i].Value);
                    if (_db.customer_sections.Where(cs => cs.section_id == nSectionId && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnCustomerEstimateId.Value) && cs.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() == null)
                    {
                        customer_section cusSec = new customer_section();
                        cusSec.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                        cusSec.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                        cusSec.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                        cusSec.section_id = nSectionId;
                        cusSec.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                        _db.customer_sections.InsertOnSubmit(cusSec);

                        _db.SubmitChanges();
                    }
                }
            }

            // Insert Customer Estimate New Locations (Not Exist in Estimate but in Model Estimate)

            for (int i = 0; i < chkModelEstimatelocations.Items.Count; i++)
            {
                if (chkModelEstimatelocations.Items[i].Selected == true)
                {
                    int nLocationId = Convert.ToInt32(chkModelEstimatelocations.Items[i].Value);

                    if (_db.customer_locations.Where(cloc => cloc.location_id == nLocationId && cloc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cloc.estimate_id == Convert.ToInt32(hdnCustomerEstimateId.Value) && cloc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() == null)
                    {
                        customer_location cusLoc = new customer_location();
                        cusLoc.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                        cusLoc.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                        cusLoc.location_id = nLocationId;
                        cusLoc.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                        _db.customer_locations.InsertOnSubmit(cusLoc);

                        _db.SubmitChanges();
                    }
                }
            }

            // Insert Pricing Details Data 
            for (int i = 0; i < chkModelEstimateSections.Items.Count; i++)
            {
                if (chkModelEstimateSections.Items[i].Selected == true)
                {
                    int nSectionId = Convert.ToInt32(chkModelEstimateSections.Items[i].Value);
                    for (int j = 0; j < chkModelEstimatelocations.Items.Count; j++)
                    {
                        if (chkModelEstimatelocations.Items[j].Selected == true)
                        {
                            int nLocationId = Convert.ToInt32(chkModelEstimatelocations.Items[j].Value);

                            List<model_estimate_pricing> Pricinglist = _db.model_estimate_pricings.Where(mePr => mePr.section_level == nSectionId && mePr.location_id == nLocationId && mePr.model_estimate_id == Convert.ToInt32(hdnModelEstimateId.Value) && mePr.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mePr.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
                            foreach (model_estimate_pricing objPd in Pricinglist)
                            {

                                int nItemId = Convert.ToInt32(objPd.item_id);
                                int nDirect = Convert.ToInt32(objPd.is_direct);
                                string ShortNote = objPd.short_notes;
                                if (!_db.pricing_details.Any(cpd => cpd.item_id == objPd.item_id && cpd.is_direct == nDirect && cpd.section_level == nSectionId && cpd.location_id == nLocationId && cpd.short_notes == ShortNote && cpd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cpd.estimate_id == Convert.ToInt32(hdnCustomerEstimateId.Value) && cpd.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && cpd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])))
                                {
                                    //arefin 07-20-2019 
                                    //------------------------------------------------------------------------------------------------------
                                    string strItemName = "";
                                    decimal dQty = (decimal)objPd.quantity;

                                    decimal dMultiplier = (decimal)objPd.retail_multiplier;
                                    decimal dItemCost = (decimal)objPd.item_cost;

                                    decimal newdItemCost = dItemCost;

                                    decimal dLaborRate = (decimal)objPd.labor_rate;
                                    decimal dMinimumQty = (decimal)objPd.minimum_qty;

                                    bool isMandatory = false;
                                    string strMeasureUnit = objPd.measure_unit;

                                    decimal nTotalPrice = objPd.total_retail_price;

                                    if ((int)objPd.item_id == 12233)
                                    {
                                        var test = objPd;
                                    }

                                    strItemName = GetItemNameForUpdateItem((int)objPd.item_id);

                                    // get Updated Item Price & details. If exist, because item would be not found, if other item added afer Template created,
                                    var item = from it in _db.item_prices
                                               join s in _db.sectioninfos on it.item_id equals s.section_id
                                               where it.item_id == objPd.item_id
                                               select new
                                               {
                                                   item_id = it.item_id,
                                                   item_cost = it.item_cost,
                                                   labor_rate = it.labor_rate,
                                                   minimum_qty = it.minimum_qty,
                                                   retail_multiplier = it.retail_multiplier,
                                                   is_mandatory = s.is_mandatory,
                                                   measure_unit = it.measure_unit
                                               };
                                    if (item.Count() > 0 && strItemName == objPd.item_name) //  If exist // check strItemName == objPd.item_name if item name exist on sectioninfos
                                    {


                                        dQty = (decimal)objPd.quantity;

                                        dMultiplier = (decimal)item.FirstOrDefault().retail_multiplier;
                                        dItemCost = (decimal)item.FirstOrDefault().item_cost;

                                        newdItemCost = dItemCost * dMultiplier;

                                        dLaborRate = (decimal)item.FirstOrDefault().labor_rate;
                                        dMinimumQty = (decimal)item.FirstOrDefault().minimum_qty;

                                        isMandatory = (bool)item.FirstOrDefault().is_mandatory;
                                        strMeasureUnit = item.FirstOrDefault().measure_unit;

                                        nTotalPrice = ((dItemCost + dLaborRate) * dMultiplier) * dQty;
                                    }
                                    else
                                    {
                                        strItemName = objPd.item_name;
                                    }
                                    
                                    //------------------------------------------------------------------------------------------------------

                                    pricing_detail pd = new pricing_detail();
                                    pd.client_id = objPd.client_id; ;
                                    pd.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                                    pd.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                                    pd.location_id = objPd.location_id; ;
                                    pd.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                                    pd.section_level = objPd.section_level;
                                    pd.item_id = objPd.item_id;
                                    pd.section_name = objPd.section_name;
                                    pd.item_name = strItemName;//objPd.item_name;
                                    pd.measure_unit = strMeasureUnit;//objPd.measure_unit;
                                    pd.minimum_qty = dMinimumQty;//objPd.minimum_qty;
                                    pd.quantity = objPd.quantity;
                                    pd.retail_multiplier = dMultiplier;//objPd.retail_multiplier;
                                    pd.labor_id = objPd.labor_id;
                                    pd.is_direct = objPd.is_direct;
                                    pd.item_cost = newdItemCost;//objPd.item_cost;
                                    pd.total_direct_price = objPd.total_direct_price;
                                    pd.total_retail_price = nTotalPrice;//objPd.total_retail_price;
                                    pd.labor_rate = dLaborRate;// objPd.labor_rate;
                                    pd.section_serial = objPd.section_serial;
                                    pd.item_cnt = objPd.item_cnt;
                                    pd.short_notes = objPd.short_notes;
                                    pd.pricing_type = objPd.pricing_type;
                                    pd.create_date = objPd.create_date;
                                    pd.last_update_date = DateTime.Today;
                                    pd.sort_id = objPd.sort_id;
                                    pd.is_mandatory = false;
                                    pd.is_CommissionExclude = false;

                                    _db.pricing_details.InsertOnSubmit(pd);

                                    _db.SubmitChanges();
                                }
                            }
                        }
                    }
                }
            }
        }
        else  // New Estimate, Locations, Sections, Pricing
        {
            if (chkModelEstimateSections.SelectedItem == null)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please Select Estimate Sections.");

                return;
            }
            if (chkModelEstimatelocations.SelectedItem == null)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please Select Estimate Locations.");

                return;
            }

            model_estimate me = new model_estimate();
            me = _db.model_estimates.Single(mest => mest.model_estimate_id == Convert.ToInt32(hdnModelEstimateId.Value) && mest.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

            // Insert Customer Estimate
            int nEstId = 0;
            var result = (from ce in _db.customer_estimates
                          where ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                          select ce.estimate_id);

            int n = result.Count();
            if (result != null && n > 0)
                nEstId = result.Max();
            nEstId = nEstId + 1;
            hdnCustomerEstimateId.Value = nEstId.ToString();

            customer_estimate cus_est = new customer_estimate();
            cus_est.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            cus_est.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            cus_est.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
            cus_est.status_id = 1;
            cus_est.sale_date = "";
            cus_est.estimate_comments = me.estimate_comments;
            cus_est.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
            cus_est.estimate_name = lblNewEstimateName.Text;
            cus_est.IsEstimateActive = true;
            cus_est.IsCustDisplay = false;
            cus_est.create_date = DateTime.Now;
            cus_est.last_update_date = DateTime.Now;
            cus_est.JobId = 0;

            _db.customer_estimates.InsertOnSubmit(cus_est);

           


            // insert Customer Estimate New Sections
            for (int i = 0; i < chkModelEstimateSections.Items.Count; i++)
            {
                if (chkModelEstimateSections.Items[i].Selected == true)
                {
                    int nSectionId = Convert.ToInt32(chkModelEstimateSections.Items[i].Value);
                    customer_section cusSec = new customer_section();
                    cusSec.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                    cusSec.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    cusSec.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                    cusSec.section_id = nSectionId;
                    cusSec.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                    _db.customer_sections.InsertOnSubmit(cusSec);
                }
            }

            // Insert Customer Estimate New Locations

            for (int i = 0; i < chkModelEstimatelocations.Items.Count; i++)
            {
                if (chkModelEstimatelocations.Items[i].Selected == true)
                {
                    int nLocationId = Convert.ToInt32(chkModelEstimatelocations.Items[i].Value);
                    customer_location cusLoc = new customer_location();
                    cusLoc.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                    cusLoc.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    cusLoc.location_id = nLocationId;
                    cusLoc.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                    _db.customer_locations.InsertOnSubmit(cusLoc);
                }
            }
            // Insert Pricing Details Data 
            for (int i = 0; i < chkModelEstimateSections.Items.Count; i++)
            {
                if (chkModelEstimateSections.Items[i].Selected == true)
                {
                    int nSectionId = Convert.ToInt32(chkModelEstimateSections.Items[i].Value);
                    for (int j = 0; j < chkModelEstimatelocations.Items.Count; j++)
                    {
                        if (chkModelEstimatelocations.Items[j].Selected == true)
                        {
                            int nLocationId = Convert.ToInt32(chkModelEstimatelocations.Items[j].Value);

                            List<model_estimate_pricing> Pricinglist = _db.model_estimate_pricings.Where(mePr => mePr.section_level == nSectionId && mePr.location_id == nLocationId && mePr.model_estimate_id == Convert.ToInt32(hdnModelEstimateId.Value) && mePr.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mePr.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
                            foreach (model_estimate_pricing objPd in Pricinglist)
                            {
                                //arefin 07-20-2019 
                                //------------------------------------------------------------------------------------------------------
                                string strItemName = "";
                                decimal dQty = (decimal)objPd.quantity;

                                decimal dMultiplier = (decimal)objPd.retail_multiplier;
                                decimal dItemCost = (decimal)objPd.item_cost;

                                decimal newdItemCost = dItemCost;

                                decimal dLaborRate = (decimal)objPd.labor_rate;
                                decimal dMinimumQty = (decimal)objPd.minimum_qty;

                                bool isMandatory = false;
                                string strMeasureUnit = objPd.measure_unit;

                                decimal nTotalPrice = objPd.total_retail_price;


                                strItemName = GetItemNameForUpdateItem((int)objPd.item_id);


                                // get Updated Item Price & details. If exist, because item would be not found, if other item added afer Template created,
                                var item = from it in _db.item_prices
                                           join s in _db.sectioninfos on it.item_id equals s.section_id
                                           where it.item_id == objPd.item_id
                                           select new
                                           {
                                               item_id = it.item_id,
                                               item_cost = it.item_cost,
                                               labor_rate = it.labor_rate,
                                               minimum_qty = it.minimum_qty,
                                               retail_multiplier = it.retail_multiplier,
                                               is_mandatory = s.is_mandatory,
                                               measure_unit = it.measure_unit
                                           };

                                if (item.Count() > 0 && strItemName == objPd.item_name) // check strItemName == objPd.item_name if item name exist on sectioninfos
                                {
                                    dQty = (decimal)objPd.quantity;

                                    dMultiplier = (decimal)item.FirstOrDefault().retail_multiplier;
                                    dItemCost = (decimal)item.FirstOrDefault().item_cost;

                                    newdItemCost = dItemCost * dMultiplier;

                                    dLaborRate = (decimal)item.FirstOrDefault().labor_rate;
                                    dMinimumQty = (decimal)item.FirstOrDefault().minimum_qty;

                                    isMandatory = (bool)item.FirstOrDefault().is_mandatory;
                                    strMeasureUnit = item.FirstOrDefault().measure_unit;

                                    nTotalPrice = ((dItemCost + dLaborRate) * dMultiplier) * dQty;
                                }
                                else
                                {
                                    strItemName = objPd.item_name;
                                }
                                //------------------------------------------------------------------------------------------------------

                                pricing_detail pd = new pricing_detail();
                                pd.client_id = objPd.client_id; ;
                                pd.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                                pd.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                                pd.location_id = objPd.location_id; ;
                                pd.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                                pd.section_level = objPd.section_level;
                                pd.item_id = objPd.item_id;
                                pd.section_name = objPd.section_name;
                                pd.item_name = strItemName;//objPd.item_name;
                                pd.measure_unit = strMeasureUnit;// objPd.measure_unit;
                                pd.minimum_qty = dMinimumQty;// objPd.minimum_qty;
                                pd.quantity = objPd.quantity;
                                pd.retail_multiplier = dMultiplier;// objPd.retail_multiplier;
                                pd.labor_id = objPd.labor_id;
                                pd.is_direct = objPd.is_direct;
                                pd.item_cost = newdItemCost;// objPd.item_cost;
                                pd.total_direct_price = objPd.total_direct_price;
                                pd.total_retail_price = nTotalPrice;// objPd.total_retail_price;
                                pd.labor_rate = dLaborRate;// objPd.labor_rate;
                                pd.section_serial = objPd.section_serial;
                                pd.item_cnt = objPd.item_cnt;
                                pd.short_notes = objPd.short_notes;
                                pd.pricing_type = objPd.pricing_type;
                                pd.create_date = objPd.create_date;
                                pd.last_update_date = DateTime.Today;
                                pd.sort_id = objPd.sort_id;
                                pd.is_mandatory = false;
                                pd.is_CommissionExclude = false;
                                _db.pricing_details.InsertOnSubmit(pd);

                            }
                        }
                    }
                }
            }
            _db.SubmitChanges();

            // Insert Customer Estimate New Payment
            model_estimate_payment objes = _db.model_estimate_payments.SingleOrDefault(mest => mest.estimate_id == Convert.ToInt32(hdnModelEstimateId.Value) && mest.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

            if (objes != null)
            {
                estimate_payment mod_es = new estimate_payment();
                mod_es.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                mod_es.client_id = objes.client_id;
                mod_es.estimate_id = Convert.ToInt32(hdnCustomerEstimateId.Value);
                mod_es.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                mod_es.project_subtotal = objes.project_subtotal;
                mod_es.tax_rate = objes.tax_rate;
                mod_es.tax_amount = objes.tax_amount;
                mod_es.total_with_tax = objes.total_with_tax;
                mod_es.adjusted_price = objes.adjusted_price;
                mod_es.adjusted_tax_amount = objes.adjusted_tax_amount;
                mod_es.adjusted_tax_rate = objes.adjusted_tax_rate;
                mod_es.new_total_with_tax = objes.new_total_with_tax;
                mod_es.is_incentives = objes.is_incentives;
                mod_es.total_incentives = objes.total_incentives;
                mod_es.incentive_ids = objes.incentive_ids;
                mod_es.deposit_percent = objes.deposit_percent;
                mod_es.deposit_amount = objes.deposit_amount;
                mod_es.countertop_percent = objes.countertop_percent;
                mod_es.countertop_amount = objes.countertop_amount;
                mod_es.start_job_percent = objes.start_job_percent;
                mod_es.start_job_amount = objes.start_job_amount;
                mod_es.due_completion_percent = objes.due_completion_percent;
                mod_es.due_completion_amount = objes.due_completion_amount;
                mod_es.final_measure_percent = objes.final_measure_percent;
                mod_es.final_measure_amount = objes.final_measure_amount;
                mod_es.deliver_caninet_percent = objes.deliver_caninet_percent;
                mod_es.deliver_cabinet_amount = objes.deliver_cabinet_amount;
                mod_es.substantial_percent = objes.substantial_percent;
                mod_es.substantial_amount = objes.substantial_amount;
                mod_es.other_value = objes.other_value;
                mod_es.other_percent = objes.other_percent;
                mod_es.other_amount = objes.other_amount;
                mod_es.based_on_percent = objes.based_on_percent;
                mod_es.based_on_dollar = objes.based_on_dollar;
                mod_es.create_date = objes.create_date;
                mod_es.updated_date = objes.updated_date;
                
                mod_es.deposit_date = objes.deposit_date;
                mod_es.countertop_date = objes.countertop_date;
                mod_es.startof_job_date = objes.startof_job_date;
                mod_es.due_completion_date = objes.due_completion_date;
                mod_es.measure_date = objes.measure_date;
                mod_es.delivery_date = objes.deposit_date;
                mod_es.substantial_date = objes.substantial_date;
                mod_es.other_date = objes.other_date;
                mod_es.lead_time = objes.lead_time;
                mod_es.contract_date = objes.contract_date;
                mod_es.deposit_value = objes.deposit_value;
                mod_es.countertop_value = objes.countertop_value;
                mod_es.start_job_value = objes.start_job_value;
                mod_es.due_completion_value = objes.due_completion_value;
                mod_es.final_measure_value = objes.final_measure_value;
                mod_es.deliver_caninet_value = objes.deliver_caninet_value;
                mod_es.substantial_value = objes.substantial_value;
                mod_es.special_note = objes.special_note;
                mod_es.start_date = objes.start_date;
                mod_es.completion_date = objes.completion_date;
                mod_es.is_KithenSheet = objes.is_KithenSheet;
                mod_es.is_BathSheet = objes.is_BathSheet;
                mod_es.is_ShowerSheet = objes.is_ShowerSheet;
                mod_es.is_TubSheet = objes.is_TubSheet;
                mod_es.drywall_percent = objes.drywall_percent;
                mod_es.drywall_amount = objes.drywall_amount;
                mod_es.drywall_date = objes.drywall_date;
                mod_es.drywall_value = objes.drywall_value;
                mod_es.flooring_percent = objes.flooring_percent;
                mod_es.flooring_amount = objes.flooring_amount;
                mod_es.flooring_date = objes.flooring_date;
                mod_es.flooring_value = objes.flooring_value;
                _db.estimate_payments.InsertOnSubmit(mod_es);
                _db.SubmitChanges();
            }
        }

        if (tblExistingEstimates.Visible == true)
        {
            lblResult.Text = csCommonUtility.GetSystemMessage(hdnCustName.Value + "'s " + ddlChoseEstimate.SelectedItem.Text + " has updated successfully.");
            btnGotoCustomer.Visible = true;
        }
        else
        {
            lblResult.Text = csCommonUtility.GetSystemMessage(hdnCustName.Value + "'s " + lblNewEstimateName.Text + " has saved successfully.");
            btnGotoCustomer.Visible = true;

        }

    }

    public string GetItemNameForUpdateItem(int SectionId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strDetails = "";
        string sql = "with section_tree as " +
                    " (" +
                       " select section_id, parent_id, section_name " +
                       " from sectioninfo " +
                       " where section_id = " + SectionId + " " + //-- this is the starting point for recursion
                       " union all " +
                       " select C.section_id, C.parent_id, C.section_name " +
                       " from sectioninfo c " +
                       " join section_tree p on C.section_id = P.parent_id   " +
            //-- this is the recursion
            //-- Since the parent id is not NULL the recursion will happen continously.
            //-- For that the condition C.id<>C.parentid AND C.parent_id != 0 using for top parent exclude
                       " AND C.section_id<>C.parent_id AND C.parent_id != 0 " +
                       " ) " +

                        // --STUFF function : Delete 2 characters from a string, starting in position 1, and then insert empty in position 1. 
                        " SELECT STUFF((SELECT ' ^ ' + section_name " +
                        " FROM section_tree " +
                        " ORDER BY section_id " +
                        " FOR XML PATH('')), 1, 2, '') AS section_name";

        var list = _db.ExecuteQuery<string>(sql, string.Empty).ToList();

        if ((list[0] ?? "").Length > 0)
            strDetails = (list[0] ?? "").ToString().TrimStart().Replace("^", ">>") + " >> ";

        //List<sectioninfo> list = _db.sectioninfos.Where(c => c.section_id == SectionId && c.parent_id > 0 && c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).ToList();
        //foreach (sectioninfo sec1 in list)
        //{
        //    strDetails = sec1.section_name + " >> " + strDetails;
        //    GetItemDetialsForUpdateItem(Convert.ToInt32(sec1.parent_id));
        //}
        return strDetails;
    }

    protected void btnGotoCustomer_Click(object sender, EventArgs e)
    {
        // Response.Redirect("customerlist.aspx");

        Response.Redirect("pricing.aspx?eid=" + Convert.ToInt32(hdnCustomerEstimateId.Value) + "&cid=" + Convert.ToInt32(hdnCustomerId.Value));
    }
    protected void ddlCustomers_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void ddlChoseEstimate_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlChoseEstimate.ID, ddlChoseEstimate.GetType().Name, "Click"); 
        lblResult.Text = "";
        lblError.Text = "";
        foreach (ListItem li in chkModelEstimateSections.Items)
        {
            if (li.Selected)
                li.Selected = false;
        }
        foreach (ListItem li in chkModelEstimatelocations.Items)
        {
            if (li.Selected)
                li.Selected = false;
        }
        chkCheckAllLocation.Checked = false;
        chkCheckAllSections.Checked = false;
        if (ddlChoseEstimate.SelectedItem.Text != "Choose Estimate")
        {
            int nCustomerEstimateId = Convert.ToInt32(ddlChoseEstimate.SelectedValue);
            hdnCustomerEstimateId.Value = nCustomerEstimateId.ToString();

            CheckExistingLocations(Convert.ToInt32(hdnCustomerId.Value), nCustomerEstimateId);
            CheckExistingSections(Convert.ToInt32(hdnCustomerId.Value), nCustomerEstimateId);
        }
    }
    protected void CheckExistingLocations(int nCustomerId, int nEstimateId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = _db.customer_locations.Where(cl => cl.customer_id == nCustomerId && cl.estimate_id == nEstimateId);
        foreach (ListItem li in chkModelEstimatelocations.Items)
        {
            foreach (customer_location loc in item)
            {
                if (loc.location_id == Convert.ToInt32(li.Value.ToString()))
                {
                    //li.Selected = true;
                    li.Attributes.CssStyle.Add("font-weight", "bold");
                }
            }
        }
    }
    protected void CheckExistingSections(int nCustomerId, int nEstimateId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = _db.customer_sections.Where(cs => cs.customer_id == nCustomerId && cs.estimate_id == nEstimateId);
        foreach (ListItem li in chkModelEstimateSections.Items)
        {
            foreach (customer_section cus_sec in item)
            {
                if (cus_sec.section_id == Convert.ToInt32(li.Value.ToString()))
                {
                    //li.Selected = true;
                    li.Attributes.CssStyle.Add("font-weight", "bold");
                }
            }
        }
    }
    protected void chkCheckAllLocation_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkCheckAllLocation.ID, chkCheckAllLocation.GetType().Name, "CheckedChanged"); 
        lblResult.Text = "";
        lblError.Text = "";
        if (tblExistingEstimates.Visible == true)
        {
            if (ddlChoseEstimate.SelectedItem.Text == "Choose Estimate")
            {
                lblError.Text = csCommonUtility.GetSystemRequiredMessage("Please choose an estimate.");
                foreach (ListItem li in chkModelEstimatelocations.Items)
                {
                    li.Selected = false;
                }
                chkCheckAllLocation.Checked = false;
                return;
            }
            else
            {
                if (chkCheckAllLocation.Checked)
                {
                    foreach (ListItem li in chkModelEstimatelocations.Items)
                    {
                        li.Selected = true;
                    }
                }
                else
                {
                    foreach (ListItem li in chkModelEstimatelocations.Items)
                    {
                        li.Selected = false;
                    }
                }
                CheckExistingLocations(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnCustomerEstimateId.Value));
            }
        }
        else
        {
            if (chkCheckAllLocation.Checked)
            {
                foreach (ListItem li in chkModelEstimatelocations.Items)
                {
                    li.Selected = true;
                }
            }
            else
            {
                foreach (ListItem li in chkModelEstimatelocations.Items)
                {
                    li.Selected = false;
                }
            }
        }

    }
    protected void chkCheckAllSections_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkCheckAllSections.ID, chkCheckAllSections.GetType().Name, "CheckedChanged"); 
        lblResult.Text = "";
        lblError.Text = "";
        if (tblExistingEstimates.Visible)
        {
            if (ddlChoseEstimate.SelectedItem.Text == "Choose Estimate")
            {
                lblError.Text = csCommonUtility.GetSystemRequiredMessage("Please choose an estimate.");

                foreach (ListItem li in chkModelEstimateSections.Items)
                {
                    li.Selected = false;
                }
                chkCheckAllSections.Checked = false;
                return;

            }
            else
            {
                if (chkCheckAllSections.Checked)
                {
                    foreach (ListItem li in chkModelEstimateSections.Items)
                    {
                        li.Selected = true;
                    }
                }
                else
                {
                    foreach (ListItem li in chkModelEstimateSections.Items)
                    {
                        li.Selected = false;
                    }
                }
                CheckExistingSections(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnCustomerEstimateId.Value));
            }
        }
        else
        {
            if (chkCheckAllSections.Checked)
            {
                foreach (ListItem li in chkModelEstimateSections.Items)
                {
                    li.Selected = true;
                }
            }
            else
            {
                foreach (ListItem li in chkModelEstimateSections.Items)
                {
                    li.Selected = false;
                }
            }
        }
    }
    protected void grdLeadList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int ncid = Convert.ToInt32(grdLeadList.DataKeys[e.Row.RowIndex].Values[0].ToString());
            int nSalesID = Convert.ToInt32(grdLeadList.DataKeys[e.Row.RowIndex].Values[1].ToString());
            int nIsCust = Convert.ToInt32(grdLeadList.DataKeys[e.Row.RowIndex].Values[3].ToString());
            int nIslead = Convert.ToInt32(grdLeadList.DataKeys[e.Row.RowIndex].Values[4].ToString());

            DataClassesDataContext _db = new DataClassesDataContext();
            // Customer Address
            customer cust = new customer();
            cust = _db.customers.Single(c => c.customer_id == ncid);
            string strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;

            sales_person sp = new sales_person();
            sp = _db.sales_persons.Single(s => s.sales_person_id == nSalesID);
            e.Row.Cells[4].Text = sp.first_name + " " + sp.last_name + "<br/>" + Convert.ToDateTime(cust.registration_date).ToShortDateString();

            if (nIsCust == 1)
                e.Row.Cells[5].Text = "Customer";
            else
                e.Row.Cells[5].Text = "Lead";



            Label lblPhone = (Label)e.Row.FindControl("lblPhone");
            lblPhone.Text = cust.phone;
            lblPhone.Attributes.CssStyle.Add("padding", "5px 0 5px 0");

            // Customer Email
            Label lEmail = (Label)e.Row.FindControl("lEmail");
            lEmail.Text = cust.email;

            // Customer Address in Google Map
            Label lAddress = (Label)e.Row.FindControl("lAddress");
            lAddress.Text = strAddress;

        }
    }
    protected void grdLeadList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdLeadList.ID, grdLeadList.GetType().Name, "PageIndexChanging"); 
        GetCustomersNew(e.NewPageIndex);
    }
    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "SelectedIndexChanged"); 
        txtSearch.Text = "";
        wtmFileNumber.WatermarkText = "Search by " + ddlSearchBy.SelectedItem.Text;
        if (ddlSearchBy.SelectedValue == "2")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetLastName";
        }
        else if (ddlSearchBy.SelectedValue == "1")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetFirstName";
        }
        else if (ddlSearchBy.SelectedValue == "4")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetAddress";
        }
        else if (ddlSearchBy.SelectedValue == "3")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetEmail";
        }
        else if (ddlSearchBy.SelectedValue == "6")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetCompany";
        }

        GetCustomersNew(0);
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        GetCustomersNew(0);
    }
    protected void grdLeadList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Select")
        {
            if (Convert.ToInt32(hdnHdnPrevIndex.Value) > -1)
            {
                GridViewRow Prevrow = grdLeadList.Rows[Convert.ToInt32(hdnHdnPrevIndex.Value)];
                Prevrow.BackColor = Color.WhiteSmoke;
            }
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = grdLeadList.Rows[index];
            grdLeadList.SelectedIndex = row.RowIndex;
            row.BackColor = ColorTranslator.FromHtml("#efe2e2");
            // row.BackColor = ColorTranslator.FromHtml("#A1DCF2");
            int nCustID = Convert.ToInt32(grdLeadList.DataKeys[row.RowIndex].Values[0]);

            string firstName = grdLeadList.DataKeys[row.RowIndex].Values[5].ToString();
            string LastName = grdLeadList.DataKeys[row.RowIndex].Values[6].ToString();
            hdnHdnPrevIndex.Value = index.ToString();





            lblResult.Text = "";
            lblError.Text = "";
            if (nCustID > 0)
            {
                hdnCustomerId.Value = nCustID.ToString();
                Session.Add("CustomerId", hdnCustomerId.Value);
                hdnCustName.Value = firstName + " " + LastName;
                lblSelectedCustName.Text = firstName + " " + LastName;
                DataClassesDataContext _db = new DataClassesDataContext();

                // Model Estimate Sections
                var section = from sec in _db.sectioninfos
                              join mes in _db.model_estimate_sections on sec.section_id equals mes.section_id
                              where mes.model_estimate_id == Convert.ToInt32(hdnModelEstimateId.Value) && mes.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mes.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                              select new SectionInfo()
                              {
                                  section_id = (int)mes.section_id,
                                  section_name = sec.section_name
                              };

                chkModelEstimateSections.DataSource = section;
                chkModelEstimateSections.DataTextField = "section_name";
                chkModelEstimateSections.DataValueField = "section_id";
                chkModelEstimateSections.DataBind();

                tblModelEstimateSections.Visible = true;

                // Model Estimate Locations
                var Locations = from loc in _db.locations
                                join mel in _db.model_estimate_locations on loc.location_id equals mel.location_id
                                where mel.model_estimate_id == Convert.ToInt32(hdnModelEstimateId.Value) && mel.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mel.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                select new LocationModel()
                                {
                                    location_id = (int)mel.location_id,
                                    location_name = loc.location_name
                                };

                chkModelEstimatelocations.DataSource = Locations;
                chkModelEstimatelocations.DataTextField = "location_name";
                chkModelEstimatelocations.DataValueField = "location_id";
                chkModelEstimatelocations.DataBind();

                tblModelEstimateLocations.Visible = true;


                // Customer Estimates without (Sold)
                var CustomerEstimate = from cest in _db.customer_estimates
                                       where cest.status_id != 3 && cest.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cest.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                       orderby cest.estimate_name ascending
                                       select cest;
                int n = CustomerEstimate.Count();
                if (CustomerEstimate != null && n > 0)
                {
                    tblExistingEstimates.Visible = true;
                    tblNoExistingEstimates.Visible = false;

                    ddlChoseEstimate.DataSource = CustomerEstimate;
                    ddlChoseEstimate.DataTextField = "estimate_name";
                    ddlChoseEstimate.DataValueField = "estimate_id";
                    ddlChoseEstimate.DataBind();
                    ddlChoseEstimate.Items.Insert(0, "Choose Estimate");
                    ddlChoseEstimate.SelectedIndex = 0;
                }
                else
                {
                    tblNoExistingEstimates.Visible = true;
                    tblExistingEstimates.Visible = false;
                }

            }
            else
            {
                tblNoExistingEstimates.Visible = false;
                tblExistingEstimates.Visible = false;
                tblModelEstimateSections.Visible = false;
                tblModelEstimateLocations.Visible = false;
                return;
            }
        }
    }
    protected void btnClose_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
}
