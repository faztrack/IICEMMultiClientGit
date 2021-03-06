using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class coverpage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
         hdnClientId.Value = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]).ToString();

        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("cp01") == false)
            {
                // No Permission Page.
                  Response.Redirect("nopermission.aspx");
            }

            try
            {

                DataClassesDataContext _db = new DataClassesDataContext();
                cover_page cop = new cover_page();
                if (_db.cover_pages.Where(cp => cp.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
                {
                    cop = _db.cover_pages.Single(cp => cp.client_id == Convert.ToInt32(hdnClientId.Value));
                    txtCoverPage.Text = cop.cover_page_content;
                }
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            }
            
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        try
        {
            
            lblResult.Text = "";

            DataClassesDataContext _db = new DataClassesDataContext();

            string strQ = "Delete cover_page WHERE client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            _db.ExecuteCommand(strQ, string.Empty);
            cover_page cop = new cover_page();
            //string IsTestServer = System.Configuration.ConfigurationManager.AppSettings["IsTestServer"];
            //if (IsTestServer == "true")
            //{
            //    cop.cover_page_content = txtCoverPage.Text.Replace("/logouploads", "D:/Faztimate/testiicrm/logouploads/");
            //}
            //else
            //{
            //    cop.cover_page_content = txtCoverPage.Text.Replace("/logouploads", "D:/Faztimate/iicrm/logouploads/");
            //}
            cop.cover_page_content = txtCoverPage.Text;//.Replace("/IICEM", "http://localhost:7854/IICEM");
            cop.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            _db.cover_pages.InsertOnSubmit(cop);
            _db.SubmitChanges();

            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");
        }
        catch(Exception ex)
        {
            lblResult.Text=csCommonUtility.GetSystemErrorMessage(ex.Message);
        }


    }
    protected void btnClose_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }

    protected void GetPDF(object sender, EventArgs e)
    {
        int nEstId =1;

        DataClassesDataContext _db = new DataClassesDataContext();

        company_profile oCom = new company_profile();
        oCom = _db.company_profiles.Single(com => com.client_id == Convert.ToInt32(hdnClientId.Value));
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal total_incentives = 0;
        decimal tax_amount = 0;
        string strPayment = "";
        string strLeadTime = "";
        string strContract_date = "";
        string strdate = "";

        string strStart_date = "";
        string strCompletion_date = "";


        string SpecialNote = "";
        string DepositValue = "";
        string CountertopValue = "";
        string StartOfJobValue = "";
        string DueCompletionValue = "";
        string MeasureValue = "";
        string DeliveryValue = "";
        string SubstantialValue = "";
        string StartofFlooringValue = "";
        string StartofDrywallValue = "";

        string DepositDate = "";
        string CountertopDate = "";
        string StartOfJobDate = "";
        string DueCompletionDate = "";
        string MeasureDate = "";
        string DeliveryDate = "";
        string SubstantialDate = "";
        string OtherDate = "";
        string StartofFlooringDate = "";
        string StartofDrywallDate = "";
        int IsQty = 1;
        int IsSubtotal = 2;


        bool is_KithenSheet = true;
        bool is_BathSheet = true;
        bool is_ShowerSheet = true;
        bool is_TubSheet = true;


        if (_db.estimate_payments.Where(ep => ep.estimate_id == nEstId && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() == null)
        {
            totalwithtax = 0;
        }
        else
        {
            estimate_payment esp = new estimate_payment();
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == nEstId && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
            totalwithtax = Convert.ToDecimal(esp.total_with_tax); //new_total_with_tax

            if (esp.is_KithenSheet != null)
            {
                is_KithenSheet = Convert.ToBoolean(esp.is_KithenSheet);
            }
            if (esp.is_BathSheet != null)
            {
                is_BathSheet = Convert.ToBoolean(esp.is_BathSheet);
            }
            if (esp.is_ShowerSheet != null)
            {
                is_ShowerSheet = Convert.ToBoolean(esp.is_ShowerSheet);
            }
            if (esp.is_TubSheet != null)
            {
                is_TubSheet = Convert.ToBoolean(esp.is_TubSheet);
            }
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            total_incentives = Convert.ToDecimal(esp.total_incentives);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);
            if (esp.lead_time.ToString() != "")
                strLeadTime = esp.lead_time.ToString();
            if (esp.contract_date.ToString() != "")
            {
                strContract_date = esp.contract_date.ToString();
                DateTime dt = Convert.ToDateTime(strContract_date);
                for (int i = 0; i < 3; i++)
                {
                    dt = dt.AddDays(1);
                    if (dt.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dt = dt.AddDays(1);
                    }
                    if (dt.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dt = dt.AddDays(1);
                    }

                }
                strdate = dt.ToShortDateString();
            }

            if (esp.start_date != null)
                strStart_date = esp.start_date.ToString();
            if (esp.completion_date != null)
                strCompletion_date = esp.completion_date.ToString();


            if (esp.special_note != null)
                SpecialNote = esp.special_note.Replace("^", "'").ToString();

            if (esp.deposit_value != null)
                DepositValue = esp.deposit_value.Replace("^", "'").ToString();
            else
                DepositValue = "Deposit";
            if (esp.countertop_value != null)
                CountertopValue = esp.countertop_value.Replace("^", "'").ToString();
            else
                CountertopValue = "At Countertop Template";
            if (esp.start_job_value != null)
                StartOfJobValue = esp.start_job_value.Replace("^", "'").ToString();
            else
                StartOfJobValue = "Start of Job";
            if (esp.due_completion_value != null)
                DueCompletionValue = esp.due_completion_value.Replace("^", "'").ToString();
            else
                DueCompletionValue = "Balance Due at Completion";
            if (esp.final_measure_value != null)
                MeasureValue = esp.final_measure_value.Replace("^", "'").ToString();
            else
                MeasureValue = "At Final Measure";
            if (esp.deliver_caninet_value != null)
                DeliveryValue = esp.deliver_caninet_value.Replace("^", "'").ToString();
            else
                DeliveryValue = "At Delivery of Cabinets";
            if (esp.substantial_value != null)
                SubstantialValue = esp.substantial_value.Replace("^", "'").ToString();
            else
                SubstantialValue = "At Substantial Completion";

            if (esp.flooring_value != null)
                StartofFlooringValue = esp.flooring_value.Replace("^", "'").ToString();
            else
                StartofFlooringValue = "At Start of Flooring";

            if (esp.drywall_value != null)
                StartofDrywallValue = esp.drywall_value.Replace("^", "'").ToString();
            else
                StartofDrywallValue = "At Start of Drywall";

            if (esp.deposit_date != null)
                DepositDate = esp.deposit_date.ToString();
            if (esp.countertop_date != null)
                CountertopDate = esp.countertop_date.ToString();
            if (esp.startof_job_date != null)
                StartOfJobDate = esp.startof_job_date.ToString();
            if (esp.due_completion_date != null)
                DueCompletionDate = esp.due_completion_date.ToString();
            if (esp.measure_date != null)
                MeasureDate = esp.measure_date.ToString();
            if (esp.delivery_date != null)
                DeliveryDate = esp.delivery_date.ToString();
            if (esp.substantial_date != null)
                SubstantialDate = esp.substantial_date.ToString();
            if (esp.other_date != null)
                OtherDate = esp.other_date.ToString();
            if (esp.flooring_date != null)
                StartofFlooringDate = esp.flooring_date.ToString();
            if (esp.drywall_date != null)
                StartofDrywallDate = esp.drywall_date.ToString();


            if (Convert.ToDecimal(esp.deposit_amount) > 0)
            {
                strPayment = "" + DepositValue + ":            $ " + Convert.ToDecimal(esp.deposit_amount) + "    " + DepositDate + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.countertop_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.countertop_amount) + "    " + CountertopDate + "    " + CountertopValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.start_job_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.start_job_amount) + "    " + StartOfJobDate + "    " + StartOfJobValue + Environment.NewLine;
            }

            if (Convert.ToDecimal(esp.flooring_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.flooring_amount) + "    " + StartofFlooringDate + "    " + StartofFlooringValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.drywall_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.drywall_amount) + "    " + StartofDrywallDate + "    " + StartofDrywallValue + Environment.NewLine;
            }

            if (Convert.ToDecimal(esp.final_measure_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.final_measure_amount) + "    " + MeasureDate + "    " + MeasureValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.deliver_caninet_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.deliver_cabinet_amount) + "    " + DeliveryDate + "    " + DeliveryValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.substantial_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.substantial_amount) + "    " + SubstantialDate + "    " + SubstantialValue + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.other_amount) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.other_amount) + "    " + OtherDate + "    " + esp.other_value + Environment.NewLine;
            }
            if (Convert.ToDecimal(esp.due_completion_percent) > 0)
            {
                strPayment += "Amount due:     $ " + Convert.ToDecimal(esp.due_completion_amount) + "    " + DueCompletionDate + "    " + DueCompletionValue + Environment.NewLine;
            }

        }


        string strLendingInst = "";
        string strApprovalCode = "";
        string strAmountApproval = "";

        finance_project objfp = new finance_project();
        if (_db.finance_projects.Where(fp => fp.estimate_id == nEstId && fp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fp.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() != null)
        {
            objfp = _db.finance_projects.Single(fip => fip.estimate_id == nEstId && fip.customer_id == Convert.ToInt32(hdnCustomerId.Value) && fip.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

            strLendingInst = objfp.lending_inst;
            strApprovalCode = objfp.approval_code;
            strAmountApproval = Convert.ToDecimal(objfp.amount_approved).ToString("c");
        }
        string strCoverLetter = "";
        company_cover_letter objComcl = new company_cover_letter();
        if (_db.company_cover_letters.Where(ccl => ccl.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() != null)
        {
            objComcl = _db.company_cover_letters.Single(ccl => ccl.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

            strCoverLetter = objComcl.cover_letter;
        }
        string strCompanyName = oCom.company_name;
        string strComPhone = oCom.phone;
        string strComEmail = oCom.email;
        string strContractEmail = oCom.contract_email;
        string strComAddress = Regex.Replace(oCom.address, @"\r\n?|\n", " ");
        string strComCity = oCom.city;
        string strComState = string.Empty;
        if (oCom.state == "AZ")
            strComState = "Arizona";
        else
            strComState = oCom.state;

        string strComZip = oCom.zip_code;
        string strComWeb = oCom.website;

        DataTable dtKitchenSheet = new DataTable();
        DataTable dtBathroom = new DataTable();

        DataTable dtKitchen = new DataTable();
        DataTable dtShower = new DataTable();
        DataTable dtTub = new DataTable();
        // DataTable dtCabinet = new DataTable();

        string strQ = " SELECT  pricing_id, pricing_details.client_id, customer_id, estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name,ISNULL(sort_id,0) AS sort_id " +
                    " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
                    " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + nEstId + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                    " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                    " AND estimate_id=" + nEstId + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);

        List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();
        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == nEstId);

        string strQ1 = " SELECT  * from disclaimers WHERE disclaimers.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + nEstId + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " )  AND disclaimers.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " " +
                        "  Union " +
                          " SELECT  * from disclaimers WHERE disclaimers.section_level IN (410001,420001)";
        List<SectionDisclaimer> des_List = _db.ExecuteQuery<SectionDisclaimer>(strQ1, string.Empty).ToList();
        string strQ2 = " SELECT  * from company_terms_condition WHERE client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        List<TermsAndCondition> term_List = _db.ExecuteQuery<TermsAndCondition>(strQ2, string.Empty).ToList();

        string strQBath = "select * from BathroomSheetSelections where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtBath = csCommonUtility.GetDataTable(strQBath);
        if (dtBath.Rows.Count > 0)
        {
            dtBathroom = dtBath;
        }
        else
        {
            DataTable tmpTable = LoadBathRoomTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["BathroomID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["BathroomSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Sink_Valve_Qty"] = "";
            drNew1["Sink_Valve_Style"] = "";
            drNew1["Sink_Valve_WhereToOrder"] = "";
            drNew1["Bathtub_Qty"] = "";
            drNew1["Bathtub_Style"] = "";
            drNew1["Bathtub_WhereToOrder"] = "";
            drNew1["Tub_Faucet_Qty"] = "";
            drNew1["Tub_Faucet_Style"] = "";
            drNew1["Tub_Faucet_WhereToOrder"] = "";
            drNew1["Tub_Valve_Qty"] = "";
            drNew1["Tub_Valve_Style"] = "";
            drNew1["Tub_Valve_WhereToOrder"] = "";
            drNew1["Tub_Drain_Qty"] = "";
            drNew1["Tub_Drain_Style"] = "";
            drNew1["Tub_Drain_WhereToOrder"] = "";
            drNew1["Tollet_Qty"] = "";
            drNew1["Tollet_Style"] = "";
            drNew1["Tollet_WhereToOrder"] = "";
            drNew1["Shower_TubSystem_Qty"] = "";
            drNew1["Shower_TubSystem_Style"] = "";
            drNew1["Shower_TubSystem_WhereToOrder"] = "";
            drNew1["Shower_Value_Qty"] = "";
            drNew1["Shower_Value_Style"] = "";
            drNew1["Shower_Value_WhereToOrder"] = "";
            drNew1["Handheld_Shower_Qty"] = "";
            drNew1["Handheld_Shower_Style"] = "";
            drNew1["Handheld_Shower_WhereToOrder"] = "";
            drNew1["Body_Spray_Qty"] = "";
            drNew1["Body_Spray_Style"] = "";
            drNew1["Body_Spray_WhereToOrder"] = "";
            drNew1["Body_Spray_Valve_Qty"] = "";
            drNew1["Body_Spray_Valve_Style"] = "";
            drNew1["Body_Spray_Valve_WhereToOrder"] = "";
            drNew1["Shower_Drain_Qty"] = "";
            drNew1["Shower_Drain_Style"] = "";
            drNew1["Shower_Drain_WhereToOrder"] = "";
            drNew1["Shower_Drain_Body_Plug_Qty"] = "";
            drNew1["Shower_Drain_Body_Plug_Style"] = "";
            drNew1["Shower_Drain_Body_Plug_WhereToOrder"] = "";
            drNew1["Shower_Drain_Cover_Qty"] = "";
            drNew1["Shower_Drain_Cover_Style"] = "";
            drNew1["Shower_Drain_Cover_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Tub_Wall_Tile_Qty"] = "";
            drNew1["Tub_Wall_Tile_Style"] = "";
            drNew1["Tub_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Tub_skirt_tile_Qty"] = "";
            drNew1["Tub_skirt_tile_Style"] = "";
            drNew1["Tub_skirt_tile_WhereToOrder"] = "";
            drNew1["Shower_Wall_Tile_Qty"] = "";
            drNew1["Shower_Wall_Tile_Style"] = "";
            drNew1["Shower_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Shower_Floor_Tile_Qty"] = "";
            drNew1["Shower_Floor_Tile_Style"] = "";
            drNew1["Shower_Floor_Tile_WhereToOrder"] = "";
            drNew1["Shower_Tub_Tile_Height_Qty"] = "";
            drNew1["Shower_Tub_Tile_Height_Style"] = "";
            drNew1["Shower_Tub_Tile_Height_WhereToOrder"] = "";
            drNew1["Floor_Tile_Qty"] = "";
            drNew1["Floor_Tile_Style"] = "";
            drNew1["Floor_Tile_WhereToOrder"] = "";
            drNew1["Floor_Tile_layout_Qty"] = "";
            drNew1["Floor_Tile_layout_Style"] = "";
            drNew1["Floor_Tile_layout_WhereToOrder"] = "";
            drNew1["BullnoseTile_Qty"] = "";
            drNew1["BullnoseTile_Style"] = "";
            drNew1["BullnoseTile_WhereToOrder"] = "";
            drNew1["Deco_Band_Qty"] = "";
            drNew1["Deco_Band_Style"] = "";
            drNew1["Deco_Band_WhereToOrder"] = "";
            drNew1["Deco_Band_Height_Qty"] = "";
            drNew1["Deco_Band_Height_Style"] = "";
            drNew1["Deco_Band_Height_WhereToOrder"] = "";
            drNew1["Tile_Baseboard_Qty"] = "";
            drNew1["Tile_Baseboard_Style"] = "";
            drNew1["Tile_Baseboard_WhereToOrder"] = "";
            drNew1["Grout_Selection_Qty"] = "";
            drNew1["Grout_Selection_Style"] = "";
            drNew1["Grout_Selection_WhereToOrder"] = "";
            drNew1["Niche_Location_Qty"] = "";
            drNew1["Niche_Location_Style"] = "";
            drNew1["Niche_Location_WhereToOrder"] = "";
            drNew1["Niche_Size_Qty"] = "";
            drNew1["Niche_Size_Style"] = "";
            drNew1["Niche_Size_WhereToOrder"] = "";
            drNew1["Glass_Qty"] = "";
            drNew1["Glass_Style"] = "";
            drNew1["Glass_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Grab_Bar_Qty"] = "";
            drNew1["Grab_Bar_Style"] = "";
            drNew1["Grab_Bar_WhereToOrder"] = "";
            drNew1["Cabinet_Door_Style_Color_Qty"] = "";
            drNew1["Cabinet_Door_Style_Color_Style"] = "";
            drNew1["Cabinet_Door_Style_Color_WhereToOrder"] = "";
            drNew1["Medicine_Cabinet_Qty"] = "";
            drNew1["Medicine_Cabinet_Style"] = "";
            drNew1["Medicine_Cabinet_WhereToOrder"] = "";
            drNew1["Mirror_Qty"] = "";
            drNew1["Mirror_Style"] = "";
            drNew1["Mirror_WhereToOrder"] = "";
            drNew1["Wood_Baseboard_Qty"] = "";
            drNew1["Wood_Baseboard_Style"] = "";
            drNew1["Wood_Baseboard_WhereToOrder"] = "";
            drNew1["Paint_Color_Qty"] = "";
            drNew1["Paint_Color_Style"] = "";
            drNew1["Paint_Color_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";
            drNew1["TowelRing_Qty"] = "";
            drNew1["TowelRing_Style"] = "";
            drNew1["TowelRing_WhereToOrder"] = "";
            drNew1["TowelBar_Qty"] = "";
            drNew1["TowelBar_Style"] = "";
            drNew1["TowelBar_WhereToOrder"] = "";
            drNew1["TissueHolder_Qty"] = "";
            drNew1["TissueHolder_Style"] = "";
            drNew1["TissueHolder_WhereToOrder"] = "";
            drNew1["ClosetDoorSeries"] = "";
            drNew1["ClosetDoorOpeningSize"] = "";
            drNew1["ClosetDoorNumberOfPanels"] = "";
            drNew1["ClosetDoorFinish"] = "";
            drNew1["ClosetDoorInsert"] = "";
            drNew1["UpdateBy"] = User.Identity.Name;
            drNew1["LastUpdatedDate"] = DateTime.Now;

            tmpTable.Rows.InsertAt(drNew1, 0);
            dtBathroom = tmpTable;

        }

        string strQKit2 = "select * from KitchenSelections where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        DataTable dtK2 = csCommonUtility.GetDataTable(strQKit2);
        if (dtK2.Rows.Count > 0)
        {
            dtKitchenSheet = dtK2;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["KitchenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["KitchenSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Cabinets_Qty"] = "";
            drNew1["Cabinets_Style"] = "";
            drNew1["Cabinets_WhereToOrder"] = "";
            drNew1["Disposal_Qty"] = "";
            drNew1["Disposal_Style"] = "";
            drNew1["Disposal_WhereToOrder"] = "";
            drNew1["Baseboard_Qty"] = "";
            drNew1["Baseboard_Style"] = "";
            drNew1["Baseboard_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";
            drNew1["LastUpdatedDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;

            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchenSheet = tmpTable;

        }

        string strQKit = "select * from KitchenSheetSelection where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtK = csCommonUtility.GetDataTable(strQKit);
        if (dtK.Rows.Count > 0)
        {
            dtKitchen = dtK;
        }
        else
        {
            DataTable tmpTable = LoadKitchenTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["AutoKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["KitchenTileSheetName"] = "";
            drNew1["BacksplashQTY"] = "";
            drNew1["BacksplashMOU"] = "";
            drNew1["BacksplashStyle"] = "";
            drNew1["BacksplashColor"] = "";
            drNew1["BacksplashSize"] = "";
            drNew1["BacksplashVendor"] = "";
            drNew1["BacksplashPattern"] = "";
            drNew1["BacksplashGroutColor"] = "";
            drNew1["BBullnoseQTY"] = "";
            drNew1["BBullnoseMOU"] = "";
            drNew1["BBullnoseStyle"] = "";
            drNew1["BBullnoseColor"] = "";
            drNew1["BBullnoseSize"] = "";
            drNew1["BBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtKitchen = tmpTable;

        }

        string strQShower = "select * from ShowerSheetSelection where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtS = csCommonUtility.GetDataTable(strQShower);
        if (dtS.Rows.Count > 0)
        {
            dtShower = dtS;
        }
        else
        {
            DataTable tmpTable = LoadShowerTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["ShowerKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["ShowerTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["ShowerPanQTY"] = "";
            drNew1["ShowerPanMOU"] = "";
            drNew1["ShowerPanStyle"] = "";
            drNew1["ShowerPanColor"] = "";
            drNew1["ShowerPanSize"] = "";
            drNew1["ShowerPanVendor"] = "";
            drNew1["ShowerPanGroutColor"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["BenchTileQTY"] = "";
            drNew1["BenchTileMOU"] = "";
            drNew1["BenchTileStyle"] = "";
            drNew1["BenchTileColor"] = "";
            drNew1["BenchTileSize"] = "";
            drNew1["BenchTileVendor"] = "";
            drNew1["BenchLocation"] = "";
            drNew1["BenchSize"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;
            tmpTable.Rows.InsertAt(drNew1, 0);
            dtShower = tmpTable;

        }


        string strQTub = "select * from TubSheetSelection where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);

        DataTable dtT = csCommonUtility.GetDataTable(strQTub);
        if (dtT.Rows.Count > 0)
        {
            dtTub = dtT;
        }
        else
        {
            DataTable tmpTable = LoadTubTileTable();
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["TubID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = nEstId;
            drNew1["TubTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["ShelfLocation"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
            dtTub = tmpTable;

        }

        //string strQCabinet = "select * from CabinetSheetSelection where  estimate_id =" + nEstId + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value);
        //DataTable dtCabi = csCommonUtility.GetDataTable(strQCabinet);
        //if (dtCabi.Rows.Count > 0)
        //{
        //    dtCabinet = dtCabi;
        //}
        //else
        //{
        //    DataTable tmpTable = LoadSectionTable();
        //    DataRow drNew1 = tmpTable.NewRow();

        //    drNew1["CabinetSheetID"] = 0;
        //    drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
        //    drNew1["estimate_id"] = nEstId;
        //    drNew1["UpperWallDoor"] = "";
        //    drNew1["UpperWallWood"] = "";
        //    drNew1["UpperWallStain"] = "";
        //    drNew1["UpperWallExterior"] = "";
        //    drNew1["UpperWallInterior"] = "";
        //    drNew1["UpperWallOther"] = "";
        //    drNew1["BaseDoor"] = "";
        //    drNew1["BaseWood"] = "";
        //    drNew1["BaseStain"] = "";
        //    drNew1["BaseExterior"] = "";
        //    drNew1["BaseInterior"] = "";
        //    drNew1["BaseOther"] = "";
        //    drNew1["MiscDoor"] = "";
        //    drNew1["MiscWood"] = "";
        //    drNew1["MiscStain"] = "";
        //    drNew1["MiscExterior"] = "";
        //    drNew1["MiscInterior"] = "";
        //    drNew1["MiscOther"] = "";
        //    drNew1["LastUpdateDate"] = DateTime.Now;
        //    drNew1["UpdateBy"] = User.Identity.Name;

        //    tmpTable.Rows.InsertAt(drNew1, 0);
        //    dtCabinet = tmpTable;

        //}


        ReportDocument rptFile = new ReportDocument();
        string strReportPath = Server.MapPath(@"Reports\rpt\rptContact.rpt");

        // string strReportPath = Server.MapPath(@"Reports\rpt\rptContact.rpt");
        rptFile.Load(strReportPath);
        rptFile.SetDataSource(CList);
        ReportDocument subReport = rptFile.OpenSubreport("rptDisclaimer.rpt");
        subReport.SetDataSource(des_List);
        ReportDocument subReport1 = rptFile.OpenSubreport("rptTermsCon.rpt");
        subReport1.SetDataSource(term_List);

        ReportDocument subKitSheet2 = rptFile.OpenSubreport("rptMultiKitchenSelection.rpt");
        subKitSheet2.SetDataSource(dtKitchenSheet);

        ReportDocument subBathSheet = rptFile.OpenSubreport("rptMultiBathSelection.rpt");
        subBathSheet.SetDataSource(dtBathroom);

        ReportDocument subKitchenSheet = rptFile.OpenSubreport("rptMultiKitchenTileSheet.rpt");
        subKitchenSheet.SetDataSource(dtKitchen);

        ReportDocument subShowerSheet = rptFile.OpenSubreport("rptMultiShowerTileSheet.rpt");
        subShowerSheet.SetDataSource(dtShower);

        ReportDocument subTubSheet = rptFile.OpenSubreport("rptMultiTubTileSheet.rpt");
        subTubSheet.SetDataSource(dtTub);

        string ContactAddress = string.Empty;
        cover_page objCP = _db.cover_pages.SingleOrDefault(c => c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
        if (objCP != null)
            ContactAddress = objCP.cover_page_content;
        if (ConfigurationManager.AppSettings["IsContactProductionServer"] == "true")
        {
            string IsTestServer = System.Configuration.ConfigurationManager.AppSettings["IsTestServer"];
            if (IsTestServer == "true")
            {
                ContactAddress = ContactAddress.Replace("/logouploads", "D:/Faztimate/testiicrm/logouploads/");
            }
            else
            {
                ContactAddress = ContactAddress.Replace("/logouploads", "D:/Faztimate/iicrm/logouploads/");
            }
        }
        else
        {
            ContactAddress = ContactAddress.Replace("/IICEM", "http://localhost:7854/IICEM");
        }

        //Cover Page Shohid
        try
        {
            string sImagePath = Server.MapPath("Reports\\Common\\pdf_report") + @"\" + DateTime.Now.Ticks.ToString() + ".png";
            csCommonUtility.CreateContactAddressImage(ContactAddress, sImagePath);

            rptFile.DataDefinition.FormulaFields["picturepath"].Text = @"'" + sImagePath + "'";
        }
        catch (Exception ex)
        {
            throw ex;
        }



        sales_person sp = new sales_person();
        sp = _db.sales_persons.SingleOrDefault(s => s.sales_person_id ==0);
        string strSalesPerson = sp.first_name + " " + sp.last_name;
        string strSalesPersonEmail = sp.email.ToString();

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName2 = objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName = objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = Regex.Replace(objCust.address, @"\r\n?|\n", " ");
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        if (strCustomerPhone.Length == 0)
        {
            strCustomerPhone = objCust.mobile;
        }
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;
        string CustPortalUrl = ConfigurationManager.AppSettings["CustPortalUrl"];
        CustPortalUrl = CustPortalUrl + "?cid=" + hdnCustomerId.Value;

        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_SalesPersonName", strSalesPerson);
        ht.Add("p_CompanyName", strCompanyName);
        ht.Add("p_CompanyAddress", strComAddress);
        ht.Add("p_CompanyCity", strComCity);
        ht.Add("p_CompanyState", strComState);
        ht.Add("p_CompanyZip", strComZip);
        ht.Add("p_CompanyPhone", strComPhone);
        ht.Add("p_CompanyWeb", strComWeb);
        ht.Add("p_CompanyEmail", strComEmail);
        ht.Add("p_ContractEmail", strContractEmail);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
        ht.Add("p_totalwithtax", totalwithtax);
        ht.Add("p_project_subtotal", project_subtotal);
        ht.Add("p_total_incentives", total_incentives);
        ht.Add("p_tax_amount", tax_amount);
        ht.Add("p_strPayment", strPayment);
        ht.Add("p_LeadTime", strLeadTime);
        ht.Add("p_Contractdate", strContract_date);
        ht.Add("p_CustomerAddress", strCustomerAddress);
        ht.Add("p_CustomerCity", strCustomerCity);
        ht.Add("p_CustomerState", strCustomerState);
        ht.Add("p_CustomerZip", strCustomerZip);
        ht.Add("p_CustomerPhone", strCustomerPhone);
        ht.Add("p_CustomerEmail", strCustomerEmail);
        ht.Add("p_CustomerCompany", strCustomerCompany);

        ht.Add("p_date", strdate);
        ht.Add("p_lendinginst", strLendingInst);
        ht.Add("p_appcode", strApprovalCode);
        ht.Add("p_amountapp", strAmountApproval);
        ht.Add("p_customername2", strCustomerName2);
        ht.Add("p_salesemail", strSalesPersonEmail);
        ht.Add("p_specialnote", SpecialNote);

        ht.Add("p_StartDate", strStart_date);
        ht.Add("p_CompletionDate", strCompletion_date);
        ht.Add("p_CoverLettter", strCoverLetter);
        ht.Add("p_IsQty", IsQty);
        ht.Add("p_IsSubtotal", IsSubtotal);
        ht.Add("p_KithenSheet", is_KithenSheet);
        ht.Add("p_BathSheet", is_BathSheet);
        ht.Add("p_ShowerSheet", is_ShowerSheet);
        ht.Add("p_TubSheet", is_TubSheet);
        ht.Add("p_CustPortalUrl", CustPortalUrl);




        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
    }
    private DataTable LoadSectionTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("CabinetSheetID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("UpperWallDoor", typeof(string));
        table.Columns.Add("UpperWallWood", typeof(string));
        table.Columns.Add("UpperWallStain", typeof(string));
        table.Columns.Add("UpperWallExterior", typeof(string));
        table.Columns.Add("UpperWallInterior", typeof(string));
        table.Columns.Add("UpperWallOther", typeof(string));
        table.Columns.Add("BaseDoor", typeof(string));
        table.Columns.Add("BaseWood", typeof(string));
        table.Columns.Add("BaseStain", typeof(string));
        table.Columns.Add("BaseExterior", typeof(string));
        table.Columns.Add("BaseInterior", typeof(string));
        table.Columns.Add("BaseOther", typeof(string));
        table.Columns.Add("MiscDoor", typeof(string));
        table.Columns.Add("MiscWood", typeof(string));
        table.Columns.Add("MiscStain", typeof(string));
        table.Columns.Add("MiscExterior", typeof(string));
        table.Columns.Add("MiscInterior", typeof(string));
        table.Columns.Add("MiscOther", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));
        table.Columns.Add("UpdateBy", typeof(string));

        return table;
    }
    private DataTable LoadBathRoomTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("BathroomID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("BathroomSheetName", typeof(string));
        table.Columns.Add("Sink_Qty", typeof(string));
        table.Columns.Add("Sink_Style", typeof(string));
        table.Columns.Add("Sink_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Fuacet_Qty", typeof(string));
        table.Columns.Add("Sink_Fuacet_Style", typeof(string));
        table.Columns.Add("Sink_Fuacet_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Drain_Qty", typeof(string));
        table.Columns.Add("Sink_Drain_Style", typeof(string));
        table.Columns.Add("Sink_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Valve_Qty", typeof(string));
        table.Columns.Add("Sink_Valve_Style", typeof(string));
        table.Columns.Add("Sink_Valve_WhereToOrder", typeof(string));
        table.Columns.Add("Bathtub_Qty", typeof(string));
        table.Columns.Add("Bathtub_Style", typeof(string));
        table.Columns.Add("Bathtub_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Faucet_Qty", typeof(string));
        table.Columns.Add("Tub_Faucet_Style", typeof(string));
        table.Columns.Add("Tub_Faucet_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Valve_Qty", typeof(string));
        table.Columns.Add("Tub_Valve_Style", typeof(string));
        table.Columns.Add("Tub_Valve_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Drain_Qty", typeof(string));
        table.Columns.Add("Tub_Drain_Style", typeof(string));
        table.Columns.Add("Tub_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Tollet_Qty", typeof(string));
        table.Columns.Add("Tollet_Style", typeof(string));
        table.Columns.Add("Tollet_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_TubSystem_Qty", typeof(string));
        table.Columns.Add("Shower_TubSystem_Style", typeof(string));
        table.Columns.Add("Shower_TubSystem_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Value_Qty", typeof(string));
        table.Columns.Add("Shower_Value_Style", typeof(string));
        table.Columns.Add("Shower_Value_WhereToOrder", typeof(string));
        table.Columns.Add("Handheld_Shower_Qty", typeof(string));
        table.Columns.Add("Handheld_Shower_Style", typeof(string));
        table.Columns.Add("Handheld_Shower_WhereToOrder", typeof(string));
        table.Columns.Add("Body_Spray_Qty", typeof(string));
        table.Columns.Add("Body_Spray_Style", typeof(string));
        table.Columns.Add("Body_Spray_WhereToOrder", typeof(string));
        table.Columns.Add("Body_Spray_Valve_Qty", typeof(string));
        table.Columns.Add("Body_Spray_Valve_Style", typeof(string));
        table.Columns.Add("Body_Spray_Valve_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Drain_Qty", typeof(string));
        table.Columns.Add("Shower_Drain_Style", typeof(string));
        table.Columns.Add("Shower_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Drain_Body_Plug_Qty", typeof(string));
        table.Columns.Add("Shower_Drain_Body_Plug_Style", typeof(string));
        table.Columns.Add("Shower_Drain_Body_Plug_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Drain_Cover_Qty", typeof(string));
        table.Columns.Add("Shower_Drain_Cover_Style", typeof(string));
        table.Columns.Add("Shower_Drain_Cover_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Style", typeof(string));
        table.Columns.Add("Counter_Top_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_To_Edge_Qty", typeof(string));
        table.Columns.Add("Counter_To_Edge_Style", typeof(string));
        table.Columns.Add("Counter_To_Edge_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Style", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_WhereToOrder", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Qty", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Style", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_WhereToOrder", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Qty", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Style", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Wall_Tile_Qty", typeof(string));
        table.Columns.Add("Tub_Wall_Tile_Style", typeof(string));
        table.Columns.Add("Tub_Wall_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Wall_Tile_Layout_Qty", typeof(string));
        table.Columns.Add("Wall_Tile_Layout_Style", typeof(string));
        table.Columns.Add("Wall_Tile_Layout_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_skirt_tile_Qty", typeof(string));
        table.Columns.Add("Tub_skirt_tile_Style", typeof(string));
        table.Columns.Add("Tub_skirt_tile_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Wall_Tile_Qty", typeof(string));
        table.Columns.Add("Shower_Wall_Tile_Style", typeof(string));
        table.Columns.Add("Shower_Wall_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Floor_Tile_Qty", typeof(string));
        table.Columns.Add("Shower_Floor_Tile_Style", typeof(string));
        table.Columns.Add("Shower_Floor_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Tub_Tile_Height_Qty", typeof(string));
        table.Columns.Add("Shower_Tub_Tile_Height_Style", typeof(string));
        table.Columns.Add("Shower_Tub_Tile_Height_WhereToOrder", typeof(string));
        table.Columns.Add("Floor_Tile_Qty", typeof(string));
        table.Columns.Add("Floor_Tile_Style", typeof(string));
        table.Columns.Add("Floor_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Floor_Tile_layout_Qty", typeof(string));
        table.Columns.Add("Floor_Tile_layout_Style", typeof(string));
        table.Columns.Add("Floor_Tile_layout_WhereToOrder", typeof(string));
        table.Columns.Add("BullnoseTile_Qty", typeof(string));
        table.Columns.Add("BullnoseTile_Style", typeof(string));
        table.Columns.Add("BullnoseTile_WhereToOrder", typeof(string));
        table.Columns.Add("Deco_Band_Qty", typeof(string));
        table.Columns.Add("Deco_Band_Style", typeof(string));
        table.Columns.Add("Deco_Band_WhereToOrder", typeof(string));
        table.Columns.Add("Deco_Band_Height_Qty", typeof(string));
        table.Columns.Add("Deco_Band_Height_Style", typeof(string));
        table.Columns.Add("Deco_Band_Height_WhereToOrder", typeof(string));
        table.Columns.Add("Tile_Baseboard_Qty", typeof(string));
        table.Columns.Add("Tile_Baseboard_Style", typeof(string));
        table.Columns.Add("Tile_Baseboard_WhereToOrder", typeof(string));
        table.Columns.Add("Grout_Selection_Qty", typeof(string));
        table.Columns.Add("Grout_Selection_Style", typeof(string));
        table.Columns.Add("Grout_Selection_WhereToOrder", typeof(string));
        table.Columns.Add("Niche_Location_Qty", typeof(string));
        table.Columns.Add("Niche_Location_Style", typeof(string));
        table.Columns.Add("Niche_Location_WhereToOrder", typeof(string));
        table.Columns.Add("Niche_Size_Qty", typeof(string));
        table.Columns.Add("Niche_Size_Style", typeof(string));
        table.Columns.Add("Niche_Size_WhereToOrder", typeof(string));
        table.Columns.Add("Glass_Qty", typeof(string));
        table.Columns.Add("Glass_Style", typeof(string));
        table.Columns.Add("Glass_WhereToOrder", typeof(string));
        table.Columns.Add("Window_Qty", typeof(string));
        table.Columns.Add("Window_Style", typeof(string));
        table.Columns.Add("Window_WhereToOrder", typeof(string));
        table.Columns.Add("Door_Qty", typeof(string));
        table.Columns.Add("Door_Style", typeof(string));
        table.Columns.Add("Door_WhereToOrder", typeof(string));
        table.Columns.Add("Grab_Bar_Qty", typeof(string));
        table.Columns.Add("Grab_Bar_Style", typeof(string));
        table.Columns.Add("Grab_Bar_WhereToOrder", typeof(string));
        table.Columns.Add("Cabinet_Door_Style_Color_Qty", typeof(string));
        table.Columns.Add("Cabinet_Door_Style_Color_Style", typeof(string));
        table.Columns.Add("Cabinet_Door_Style_Color_WhereToOrder", typeof(string));
        table.Columns.Add("Medicine_Cabinet_Qty", typeof(string));
        table.Columns.Add("Medicine_Cabinet_Style", typeof(string));
        table.Columns.Add("Medicine_Cabinet_WhereToOrder", typeof(string));
        table.Columns.Add("Mirror_Qty", typeof(string));
        table.Columns.Add("Mirror_Style", typeof(string));
        table.Columns.Add("Mirror_WhereToOrder", typeof(string));
        table.Columns.Add("Wood_Baseboard_Qty", typeof(string));
        table.Columns.Add("Wood_Baseboard_Style", typeof(string));
        table.Columns.Add("Wood_Baseboard_WhereToOrder", typeof(string));
        table.Columns.Add("Paint_Color_Qty", typeof(string));
        table.Columns.Add("Paint_Color_Style", typeof(string));
        table.Columns.Add("Paint_Color_WhereToOrder", typeof(string));
        table.Columns.Add("Lighting_Qty", typeof(string));
        table.Columns.Add("Lighting_Style", typeof(string));
        table.Columns.Add("Lighting_WhereToOrder", typeof(string));
        table.Columns.Add("Hardware_Qty", typeof(string));
        table.Columns.Add("Hardware_Style", typeof(string));
        table.Columns.Add("Hardware_WhereToOrder", typeof(string));
        table.Columns.Add("Special_Notes", typeof(string));
        table.Columns.Add("TowelRing_Qty", typeof(string));
        table.Columns.Add("TowelRing_Style", typeof(string));
        table.Columns.Add("TowelRing_WhereToOrder", typeof(string));
        table.Columns.Add("TowelBar_Qty", typeof(string));
        table.Columns.Add("TowelBar_Style", typeof(string));
        table.Columns.Add("TowelBar_WhereToOrder", typeof(string));
        table.Columns.Add("TissueHolder_Qty", typeof(string));
        table.Columns.Add("TissueHolder_Style", typeof(string));
        table.Columns.Add("TissueHolder_WhereToOrder", typeof(string));
        table.Columns.Add("ClosetDoorSeries", typeof(string));
        table.Columns.Add("ClosetDoorOpeningSize", typeof(string));
        table.Columns.Add("ClosetDoorNumberOfPanels", typeof(string));
        table.Columns.Add("ClosetDoorFinish", typeof(string));
        table.Columns.Add("ClosetDoorInsert", typeof(string));
        table.Columns.Add("LastUpdatedDate", typeof(DateTime));
        table.Columns.Add("UpdateBy", typeof(string));

        return table;
    }

    private DataTable LoadKitchenTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("KitchenID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("KitchenSheetName", typeof(string));
        table.Columns.Add("Sink_Qty", typeof(string));
        table.Columns.Add("Sink_Style", typeof(string));
        table.Columns.Add("Sink_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Fuacet_Qty", typeof(string));
        table.Columns.Add("Sink_Fuacet_Style", typeof(string));
        table.Columns.Add("Sink_Fuacet_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Drain_Qty", typeof(string));
        table.Columns.Add("Sink_Drain_Style", typeof(string));
        table.Columns.Add("Sink_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Style", typeof(string));
        table.Columns.Add("Counter_Top_WhereToOrder", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Qty", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Style", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Style", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_WhereToOrder", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Qty", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Style", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_To_Edge_Qty", typeof(string));
        table.Columns.Add("Counter_To_Edge_Style", typeof(string));
        table.Columns.Add("Counter_To_Edge_WhereToOrder", typeof(string));
        table.Columns.Add("Cabinets_Qty", typeof(string));
        table.Columns.Add("Cabinets_Style", typeof(string));
        table.Columns.Add("Cabinets_WhereToOrder", typeof(string));
        table.Columns.Add("Disposal_Qty", typeof(string));
        table.Columns.Add("Disposal_Style", typeof(string));
        table.Columns.Add("Disposal_WhereToOrder", typeof(string));
        table.Columns.Add("Baseboard_Qty", typeof(string));
        table.Columns.Add("Baseboard_Style", typeof(string));
        table.Columns.Add("Baseboard_WhereToOrder", typeof(string));
        table.Columns.Add("Window_Qty", typeof(string));
        table.Columns.Add("Window_Style", typeof(string));
        table.Columns.Add("Window_WhereToOrder", typeof(string));
        table.Columns.Add("Door_Qty", typeof(string));
        table.Columns.Add("Door_Style", typeof(string));
        table.Columns.Add("Door_WhereToOrder", typeof(string));
        table.Columns.Add("Lighting_Qty", typeof(string));
        table.Columns.Add("Lighting_Style", typeof(string));
        table.Columns.Add("Lighting_WhereToOrder", typeof(string));
        table.Columns.Add("Hardware_Qty", typeof(string));
        table.Columns.Add("Hardware_Style", typeof(string));
        table.Columns.Add("Hardware_WhereToOrder", typeof(string));
        table.Columns.Add("Special_Notes", typeof(string));
        table.Columns.Add("LastUpdatedDate", typeof(DateTime));
        table.Columns.Add("UpdateBy", typeof(string));

        return table;
    }

    private DataTable LoadKitchenTileTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("AutoKithenID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("KitchenTileSheetName", typeof(string));
        table.Columns.Add("BacksplashQTY", typeof(string));
        table.Columns.Add("BacksplashMOU", typeof(string));
        table.Columns.Add("BacksplashStyle", typeof(string));
        table.Columns.Add("BacksplashColor", typeof(string));
        table.Columns.Add("BacksplashSize", typeof(string));
        table.Columns.Add("BacksplashVendor", typeof(string));
        table.Columns.Add("BacksplashPattern", typeof(string));
        table.Columns.Add("BacksplashGroutColor", typeof(string));
        table.Columns.Add("BBullnoseQTY", typeof(string));
        table.Columns.Add("BBullnoseMOU", typeof(string));
        table.Columns.Add("BBullnoseStyle", typeof(string));
        table.Columns.Add("BBullnoseColor", typeof(string));
        table.Columns.Add("BBullnoseSize", typeof(string));
        table.Columns.Add("BBullnoseVendor", typeof(string));
        table.Columns.Add("SchluterNOSticks", typeof(string));
        table.Columns.Add("SchluterColor", typeof(string));
        table.Columns.Add("SchluterProfile", typeof(string));
        table.Columns.Add("SchluterThickness", typeof(string));
        table.Columns.Add("FloorQTY", typeof(string));
        table.Columns.Add("FloorMOU", typeof(string));
        table.Columns.Add("FloorStyle", typeof(string));
        table.Columns.Add("FloorColor", typeof(string));
        table.Columns.Add("FloorSize", typeof(string));
        table.Columns.Add("FloorVendor", typeof(string));
        table.Columns.Add("FloorPattern", typeof(string));
        table.Columns.Add("FloorDirection", typeof(string));
        table.Columns.Add("BaseboardQTY", typeof(string));
        table.Columns.Add("BaseboardMOU", typeof(string));
        table.Columns.Add("BaseboardStyle", typeof(string));
        table.Columns.Add("BaseboardColor", typeof(string));
        table.Columns.Add("BaseboardSize", typeof(string));
        table.Columns.Add("BaseboardVendor", typeof(string));
        table.Columns.Add("FloorGroutColor", typeof(string));
        table.Columns.Add("UpdateBy", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));


        return table;
    }

    private DataTable LoadTubTileTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("TubID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("TubTileSheetName", typeof(string));
        table.Columns.Add("WallTileQTY", typeof(string));
        table.Columns.Add("WallTileMOU", typeof(string));
        table.Columns.Add("WallTileStyle", typeof(string));
        table.Columns.Add("WallTileColor", typeof(string));
        table.Columns.Add("WallTileSize", typeof(string));
        table.Columns.Add("WallTileVendor", typeof(string));
        table.Columns.Add("WallTilePattern", typeof(string));
        table.Columns.Add("WallTileGroutColor", typeof(string));
        table.Columns.Add("WBullnoseQTY", typeof(string));
        table.Columns.Add("WBullnoseMOU", typeof(string));
        table.Columns.Add("WBullnoseStyle", typeof(string));
        table.Columns.Add("WBullnoseColor", typeof(string));
        table.Columns.Add("WBullnoseSize", typeof(string));
        table.Columns.Add("WBullnoseVendor", typeof(string));
        table.Columns.Add("SchluterNOSticks", typeof(string));
        table.Columns.Add("SchluterColor", typeof(string));
        table.Columns.Add("SchluterProfile", typeof(string));
        table.Columns.Add("SchluterThickness", typeof(string));
        table.Columns.Add("DecobandQTY", typeof(string));
        table.Columns.Add("DecobandMOU", typeof(string));
        table.Columns.Add("DecobandStyle", typeof(string));
        table.Columns.Add("DecobandColor", typeof(string));
        table.Columns.Add("DecobandSize", typeof(string));
        table.Columns.Add("DecobandVendor", typeof(string));
        table.Columns.Add("DecobandHeight", typeof(string));
        table.Columns.Add("NicheTileQTY", typeof(string));
        table.Columns.Add("NicheTileMOU", typeof(string));
        table.Columns.Add("NicheTileStyle", typeof(string));
        table.Columns.Add("NicheTileColor", typeof(string));
        table.Columns.Add("NicheTileSize", typeof(string));
        table.Columns.Add("NicheTileVendor", typeof(string));
        table.Columns.Add("NicheLocation", typeof(string));
        table.Columns.Add("NicheSize", typeof(string));
        table.Columns.Add("ShelfLocation", typeof(string));
        table.Columns.Add("FloorQTY", typeof(string));
        table.Columns.Add("FloorMOU", typeof(string));
        table.Columns.Add("FloorStyle", typeof(string));
        table.Columns.Add("FloorColor", typeof(string));
        table.Columns.Add("FloorSize", typeof(string));
        table.Columns.Add("FloorVendor", typeof(string));
        table.Columns.Add("FloorPattern", typeof(string));
        table.Columns.Add("FloorDirection", typeof(string));
        table.Columns.Add("BaseboardQTY", typeof(string));
        table.Columns.Add("BaseboardMOU", typeof(string));
        table.Columns.Add("BaseboardStyle", typeof(string));
        table.Columns.Add("BaseboardColor", typeof(string));
        table.Columns.Add("BaseboardSize", typeof(string));
        table.Columns.Add("BaseboardVendor", typeof(string));
        table.Columns.Add("FloorGroutColor", typeof(string));
        table.Columns.Add("TileTo", typeof(string));
        table.Columns.Add("UpdateBy", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));

        return table;
    }

    private DataTable LoadShowerTileTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("ShowerKithenID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("ShowerTileSheetName", typeof(string));
        table.Columns.Add("WallTileQTY", typeof(string));
        table.Columns.Add("WallTileMOU", typeof(string));
        table.Columns.Add("WallTileStyle", typeof(string));
        table.Columns.Add("WallTileColor", typeof(string));
        table.Columns.Add("WallTileSize", typeof(string));
        table.Columns.Add("WallTileVendor", typeof(string));
        table.Columns.Add("WallTilePattern", typeof(string));
        table.Columns.Add("WallTileGroutColor", typeof(string));
        table.Columns.Add("WBullnoseQTY", typeof(string));
        table.Columns.Add("WBullnoseMOU", typeof(string));
        table.Columns.Add("WBullnoseStyle", typeof(string));
        table.Columns.Add("WBullnoseColor", typeof(string));
        table.Columns.Add("WBullnoseSize", typeof(string));
        table.Columns.Add("WBullnoseVendor", typeof(string));
        table.Columns.Add("SchluterNOSticks", typeof(string));
        table.Columns.Add("SchluterColor", typeof(string));
        table.Columns.Add("SchluterProfile", typeof(string));
        table.Columns.Add("SchluterThickness", typeof(string));
        table.Columns.Add("ShowerPanQTY", typeof(string));
        table.Columns.Add("ShowerPanMOU", typeof(string));
        table.Columns.Add("ShowerPanStyle", typeof(string));
        table.Columns.Add("ShowerPanColor", typeof(string));
        table.Columns.Add("ShowerPanSize", typeof(string));
        table.Columns.Add("ShowerPanVendor", typeof(string));
        table.Columns.Add("ShowerPanPattern", typeof(string));
        table.Columns.Add("ShowerPanGroutColor", typeof(string));
        table.Columns.Add("DecobandQTY", typeof(string));
        table.Columns.Add("DecobandMOU", typeof(string));
        table.Columns.Add("DecobandStyle", typeof(string));
        table.Columns.Add("DecobandColor", typeof(string));
        table.Columns.Add("DecobandSize", typeof(string));
        table.Columns.Add("DecobandVendor", typeof(string));
        table.Columns.Add("DecobandHeight", typeof(string));
        table.Columns.Add("NicheTileQTY", typeof(string));
        table.Columns.Add("NicheTileMOU", typeof(string));
        table.Columns.Add("NicheTileStyle", typeof(string));
        table.Columns.Add("NicheTileColor", typeof(string));
        table.Columns.Add("NicheTileSize", typeof(string));
        table.Columns.Add("NicheTileVendor", typeof(string));
        table.Columns.Add("NicheLocation", typeof(string));
        table.Columns.Add("NicheSize", typeof(string));
        table.Columns.Add("BenchTileQTY", typeof(string));
        table.Columns.Add("BenchTileMOU", typeof(string));
        table.Columns.Add("BenchTileStyle", typeof(string));
        table.Columns.Add("BenchTileColor", typeof(string));
        table.Columns.Add("BenchTileSize", typeof(string));
        table.Columns.Add("BenchTileVendor", typeof(string));
        table.Columns.Add("BenchLocation", typeof(string));
        table.Columns.Add("BenchSize", typeof(string));
        table.Columns.Add("FloorQTY", typeof(string));
        table.Columns.Add("FloorMOU", typeof(string));
        table.Columns.Add("FloorStyle", typeof(string));
        table.Columns.Add("FloorColor", typeof(string));
        table.Columns.Add("FloorSize", typeof(string));
        table.Columns.Add("FloorVendor", typeof(string));
        table.Columns.Add("FloorPattern", typeof(string));
        table.Columns.Add("FloorDirection", typeof(string));
        table.Columns.Add("BaseboardQTY", typeof(string));
        table.Columns.Add("BaseboardMOU", typeof(string));
        table.Columns.Add("BaseboardStyle", typeof(string));
        table.Columns.Add("BaseboardColor", typeof(string));
        table.Columns.Add("BaseboardSize", typeof(string));
        table.Columns.Add("BaseboardVendor", typeof(string));
        table.Columns.Add("FloorGroutColor", typeof(string));
        table.Columns.Add("TileTo", typeof(string));
        table.Columns.Add("UpdateBy", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));

        return table;
    }
    

}