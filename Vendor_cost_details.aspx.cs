using DataStreams.Csv;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using System.Xml.Linq;

public partial class Vendor_cost_details : System.Web.UI.Page
{
    public DataTable dtVendor;
    public DataTable dtSection;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);


            try
            {
                if (Session["oUser"] == null)
                {
                    Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
                }
                else
                {
                    userinfo oUser = (userinfo)Session["oUser"];
                    hdnEmailType.Value = oUser.EmailIntegrationType.ToString();
                   
                }

                if (Page.User.IsInRole("admin014") == false)
                {
                    // No Permission Page.
                    Response.Redirect("nopermission.aspx");
                }
                DataClassesDataContext _db = new DataClassesDataContext();
                if (Request.QueryString.Get("cid") != null)
                    hdnCustomerId.Value = Convert.ToInt32(Request.QueryString.Get("cid")).ToString();
                if (Request.QueryString.Get("eid") != null)
                {
                    hdnEstimateId.Value = Convert.ToInt32(Request.QueryString.Get("eid")).ToString();

                    if (_db.co_pricing_masters.Any(ch => ch.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ch.estimate_id == Convert.ToInt32(hdnEstimateId.Value)))
                    {
                        hdnAllowance.Value = "1"; // Set value 1 
                    }
                }

                Session.Add("CustomerId", hdnCustomerId.Value);
                 

                int nCustId = Convert.ToInt32(hdnCustomerId.Value);
                int nEstId = Convert.ToInt32(hdnEstimateId.Value);

                // Get Customer Information
                if (nCustId > 0)
                {
                    customer cust = new customer();
                    cust = _db.customers.Single(c => c.customer_id == nCustId);

                    hdnClientId.Value = cust.client_id.ToString();

                    lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                    hdnSalesPersonId.Value = cust.sales_person_id.ToString();
                    GetEstimate(nCustId);
                }
                EstimateInfo(nEstId);

                string strJobNumber = csCommonUtility.GetCustomerEstimateInfo(nCustId, nEstId).job_number ?? "";

                if (strJobNumber.Length > 0)
                {
                    lblTitelJobNumber.Text = " ( Job Number: " + strJobNumber + " )";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnGoToPayment", "ddlVendor", "ddlTrade", "ddlCategory", "btnSubmit", "ddlEstimate", "btnAddnewRow" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "Delete", "grdVendorCost_file_upload", "grdVendorCost_imgbtnUpload", "grdVendorCost_imgDelete", "grdVendorCost_imgEditNote", "grdCom_CO", "grdCom", "Save" });
        }

    }
    private void EstimateInfo(int nEsimateID)
    {
        hdnEstimateId.Value = nEsimateID.ToString();
        DataClassesDataContext _db = new DataClassesDataContext();
        // Get Estimate Info
        var item = from l in _db.estimate_commissions
                   where l.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && l.customer_id == Convert.ToInt32(hdnCustomerId.Value) && l.client_id == Convert.ToInt32(hdnClientId.Value)
                   orderby l.estimate_com_id
                   select l;

        grdCom.DataSource = item;
        grdCom.DataBind();
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        decimal total_incentives = 0;
        estimate_payment esp = new estimate_payment();//ZAM
        if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.new_total_with_tax);
            total_incentives = Convert.ToDecimal(esp.total_incentives);
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);

            if (totalwithtax == 0)
                totalwithtax = project_subtotal + tax_amount;
            lblProjectTotal.Text = totalwithtax.ToString("c"); //with Taxes
            hdnProjectSubTotal.Value = project_subtotal.ToString();
            //lblProjectTotal.Text = project_subtotal.ToString("c");// Without Taxes
            hdnEstPaymentId.Value = esp.est_payment_id.ToString();

        }
        //var item = from it in _db.partial_payment_infos
        //       where it.customer_id == Convert.ToInt32(hdnCustomerId.Value) && it.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && it.client_id == 1
        decimal payAmount = 0;
        var result = (from ppi in _db.New_partial_payments
                      where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.client_id == Convert.ToInt32(hdnClientId.Value)
                      select ppi.pay_amount);
        int n = result.Count();
        if (result != null && n > 0)
            payAmount = result.Sum();
        lblTotalRecive.Text = payAmount.ToString("c");

        //decimal COAmount = 0;
        //var Co_result = (from cpi in _db.change_order_pricing_lists
        //                 join cho in _db.changeorder_estimates on new { cpi.chage_order_id, cpi.customer_id, cpi.estimate_id } equals new { chage_order_id = cho.chage_order_id, customer_id = cho.customer_id, estimate_id = cho.estimate_id }
        //                 where cpi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cpi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cho.change_order_status_id == 3 && cho.change_order_type_id != 3 && cpi.client_id == Convert.ToInt32(hdnClientId.Value)
        //                 select cpi.EconomicsCost);

        //int co_AM = Co_result.Count();
        //if (result != null && co_AM > 0)
        //    COAmount = Co_result.Sum();
        decimal TotalCOAmount = 0;
        decimal TotalCOWithoutTax = 0;
        decimal TotalCOExCom = 0;
        var COitem = from co in _db.changeorder_estimates
                     where co.customer_id == Convert.ToInt32(hdnCustomerId.Value) && co.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && co.change_order_status_id == 3
                     orderby co.changeorder_name ascending
                     select co;

        //ZAM
        foreach (changeorder_estimate cho in COitem)
        {
            int ncoeid = cho.chage_order_id;
            decimal CoTaxRate = 0;
            decimal CoPrice = 0;
            decimal CoTax = 0;
            decimal CoPriceWithoutTax = 0;
            decimal CoPriceExCom = 0;
            CoTaxRate = Convert.ToDecimal(cho.tax);
            decimal dEconCost = 0;
            decimal dECOCostExCom = 0;
            var Coresult = (from chpl in _db.change_order_pricing_lists
                            where chpl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && chpl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && chpl.client_id == Convert.ToInt32(hdnClientId.Value) && chpl.chage_order_id == ncoeid
                            select chpl.EconomicsCost);
            int cn = Coresult.Count();
            if (Coresult != null && cn > 0)
                dEconCost = Coresult.Sum();

            var CoresultExCom = (from chpl in _db.change_order_pricing_lists
                                 where chpl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && chpl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && chpl.client_id == Convert.ToInt32(hdnClientId.Value) && chpl.chage_order_id == ncoeid
                                 select chpl.EconomicsCost);

            int cnExCom = CoresultExCom.Count();
            if (CoresultExCom != null && cnExCom > 0)
                dECOCostExCom = CoresultExCom.Sum();


            if (CoTaxRate > 0)
            {
                CoTax = dEconCost * (CoTaxRate / 100);
                CoPrice = dEconCost + CoTax;
                CoPriceWithoutTax = dEconCost;
                CoPriceExCom = dECOCostExCom;

            }
            else
            {
                CoPrice = dEconCost;
                CoPriceWithoutTax = dEconCost;
                CoPriceExCom = dECOCostExCom;
            }
            TotalCOAmount += CoPrice;
            TotalCOWithoutTax += CoPriceWithoutTax;
            TotalCOExCom += CoPriceExCom;

        }
        lblTotalCOAmount.Text = TotalCOAmount.ToString("c");
        hdnTotalCOWithoutTax.Value = TotalCOWithoutTax.ToString();
        hdnTotalCOExCom.Value = TotalCOExCom.ToString();

        decimal TotalCostAmount = 0;
        TotalCostAmount = totalwithtax + TotalCOAmount;
        lblTotalAmount.Text = TotalCostAmount.ToString("c");

        decimal TotalBalanceAmount = 0;
        TotalBalanceAmount = TotalCostAmount - payAmount;
        lblTotalBalanceAmount.Text = TotalBalanceAmount.ToString("c");

        decimal TotalExCom_WithIntevcise = GetRetailTotal();
        decimal TotalPriceExCom = TotalExCom_WithIntevcise - total_incentives;
        hdnProjectSubTotalExCom.Value = TotalPriceExCom.ToString();
        // Get vendor from DB
        LoadVendor();
        LoadSection();

        LoadDscription();
        LoadCommission();
        LoadCO_Commission();
        Calculate();
    }
    private void GetEstimate(int nCustId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select * from customer_estimate where customer_id=" + nCustId + " and client_id= "+ Convert.ToInt32(hdnClientId.Value) + " and status_id = 3 order by estimate_id desc ";
        IEnumerable<customer_estimate_model> clist = _db.ExecuteQuery<customer_estimate_model>(strQ, string.Empty);
        DataTable dtEstTable = LoadEstimateTable();
        foreach (customer_estimate_model ce in clist)
        {
            decimal TotalExCom_WithIntevcise = GetRetailTotal(ce.estimate_id, nCustId);
            DataRow drNew = dtEstTable.NewRow();
            drNew["estimate_id"] = ce.estimate_id;
            drNew["estimate_name"] = ce.estimate_name + " (" + TotalExCom_WithIntevcise.ToString("c") + ")";
            dtEstTable.Rows.Add(drNew);
        }
        ddlEstimate.DataSource = dtEstTable;
        ddlEstimate.DataTextField = "estimate_name";
        ddlEstimate.DataValueField = "estimate_id";
        ddlEstimate.DataBind();

        if (Request.QueryString.Get("eid") != null)
        {
            ddlEstimate.SelectedValue = Request.QueryString.Get("eid").ToString();
        }

        lblCurrentEstimate.Text = dtEstTable.Rows[0]["estimate_name"].ToString();
        if (dtEstTable.Rows.Count > 1)
        {
            lblCurrentEstimate.Visible = false;
            ddlEstimate.Visible = true;
        }
        else
        {
            lblCurrentEstimate.Visible = true;
            ddlEstimate.Visible = false;

        }


    }
    private decimal GetRetailTotal(int EstID, int ncustid)
    {
        decimal dRetail = 0;
        DataClassesDataContext _db = new DataClassesDataContext();

        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        decimal total_incentives = 0;
        estimate_payment esp = new estimate_payment();
        if (_db.estimate_payments.Where(ep => ep.estimate_id == EstID && ep.customer_id == ncustid && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
        {
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == EstID && ep.customer_id == ncustid && ep.client_id == Convert.ToInt32(hdnClientId.Value));
            totalwithtax = Convert.ToDecimal(esp.new_total_with_tax);
            total_incentives = Convert.ToDecimal(esp.total_incentives);
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
            if (Convert.ToDecimal(esp.adjusted_tax_amount) > 0)
                tax_amount = Convert.ToDecimal(esp.adjusted_tax_amount);
            else
                tax_amount = Convert.ToDecimal(esp.tax_amount);

            if (totalwithtax == 0)
                totalwithtax = project_subtotal + tax_amount;
            if (totalwithtax == 0)
            {
                if (Convert.ToInt32(hdnAllowance.Value) > 0)
                {
                    var result = (from pd in _db.co_pricing_masters
                                  where (from clc in _db.changeorder_locations
                                         where clc.estimate_id == EstID && clc.customer_id == ncustid && clc.client_id == Convert.ToInt32(hdnClientId.Value)
                                         select clc.location_id).Contains(pd.location_id) &&
                                         (from cs in _db.changeorder_sections
                                          where cs.estimate_id == EstID && cs.customer_id == ncustid && cs.client_id == Convert.ToInt32(hdnClientId.Value)
                                          select cs.section_id).Contains(pd.section_level) && pd.estimate_id == EstID && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.item_status_id == 1 && pd.is_CommissionExclude == false
                                  select pd.total_retail_price);
                    int n = result.Count();
                    if (result != null && n > 0)
                        dRetail = result.Sum();

                    totalwithtax = dRetail;
                }
                else
                {
                    var result = (from pd in _db.pricing_details
                                  where (from clc in _db.customer_locations
                                         where clc.estimate_id == EstID && clc.customer_id == ncustid && clc.client_id == Convert.ToInt32(hdnClientId.Value)
                                         select clc.location_id).Contains(pd.location_id) &&
                                         (from cs in _db.customer_sections
                                          where cs.estimate_id == EstID && cs.customer_id == ncustid && cs.client_id == Convert.ToInt32(hdnClientId.Value)
                                          select cs.section_id).Contains(pd.section_level) && pd.estimate_id == EstID && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.pricing_type == "A" && pd.is_CommissionExclude == false
                                  select pd.total_retail_price);
                    int n = result.Count();
                    if (result != null && n > 0)
                        dRetail = result.Sum();

                    totalwithtax = dRetail;
                }


            }
        }

        return totalwithtax;
    }
    private void Calculate()
    {
        decimal nProjecttotal = Convert.ToDecimal(lblProjectTotal.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
        decimal nTotalRecive = Convert.ToDecimal(lblTotalRecive.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));

        decimal nLabor = 0;
        decimal nMatrial = 0;
        decimal nMatrialLabor = 0;
        decimal nCommission = 0;
        decimal ncoCommission = 0;
        decimal nTotalCommission = 0;
        string strId = string.Empty;
        foreach (GridViewRow di in grdVendorCost.Rows)
        {
            
                DropDownList ddlCategory = (DropDownList)di.FindControl("ddlCategory");
                TextBox txtAmount = (TextBox)di.FindControl("txtAmount");
                DropDownList ddlTrade = (DropDownList)di.FindControl("ddlTrade");

                if (Convert.ToInt32(ddlCategory.SelectedValue) == 1)
                {
                    nMatrial += Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    if (strId.Length == 0)
                    {
                        strId = Convert.ToInt32(ddlTrade.SelectedValue).ToString();
                    }
                    else
                    {
                        strId = strId + ", " + Convert.ToInt32(ddlTrade.SelectedValue).ToString();
                    }

                }
                else if (Convert.ToInt32(ddlCategory.SelectedValue) == 2)
                    nLabor += Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                else
                    nMatrialLabor += Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            

        }
        foreach (GridViewRow di in grdCom.Rows)
        {
            {
                TextBox txtComAmount = (TextBox)di.FindControl("txtComAmount");
                Label lblComAmount = (Label)di.FindControl("lblComAmount");
                nCommission += Convert.ToDecimal(lblComAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

            }

        }
        foreach (GridViewRow di in grdCom_CO.Rows)
        {
            {
                TextBox txtComAmount = (TextBox)di.FindControl("txtComAmount");
                Label lblComAmount = (Label)di.FindControl("lblComAmount");
                ncoCommission += Convert.ToDecimal(lblComAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

            }

        }




        nTotalCommission = nCommission + ncoCommission;


        decimal ntotalcost = nCommission + nLabor + nMatrial + nMatrialLabor + ncoCommission;
        decimal nProfit = nTotalRecive - ntotalcost;

        lblTotalCom.Text = nTotalCommission.ToString("c");

        lblLabor.Text = nLabor.ToString("c");
        lblMaterial.Text = nMatrial.ToString("c");
        lblMaterialLabor.Text = nMatrialLabor.ToString("c");
        // lblComission.Text = nCom.ToString("c");
        lblTotalCost.Text = ntotalcost.ToString("c");
        lblProfit.Text = nProfit.ToString("c");
        if (nProfit > 0)
            lblProfit.ForeColor = System.Drawing.Color.Green;
        else
            lblProfit.ForeColor = System.Drawing.Color.Red;



    }
    private decimal GetRetailTotal()
    {
        decimal dRetail = 0;
        DataClassesDataContext _db = new DataClassesDataContext();

        if (Convert.ToInt32(hdnAllowance.Value) > 0)
        {
            var result = (from pd in _db.co_pricing_masters
                          where (from clc in _db.changeorder_locations
                                 where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(hdnClientId.Value)
                                 select clc.location_id).Contains(pd.location_id) &&
                                 (from cs in _db.changeorder_sections
                                  where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(hdnClientId.Value)
                                  select cs.section_id).Contains(pd.section_level) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.item_status_id == 1 && pd.is_CommissionExclude == false
                          select pd.total_retail_price);
            int n = result.Count();
            if (result != null && n > 0)
                dRetail = result.Sum();
        }
        else
        {
            var result = (from pd in _db.pricing_details
                          where (from clc in _db.customer_locations
                                 where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(hdnClientId.Value)
                                 select clc.location_id).Contains(pd.location_id) &&
                                 (from cs in _db.customer_sections
                                  where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(hdnClientId.Value)
                                  select cs.section_id).Contains(pd.section_level) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(hdnClientId.Value) && pd.pricing_type == "A" && pd.is_CommissionExclude == false
                          select pd.total_retail_price);
            int n = result.Count();
            if (result != null && n > 0)
                dRetail = result.Sum();
        }

        return dRetail;
    }
    private void LoadCommission()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpCTable = LoadComTable();
        decimal Comper = 0;
        decimal sCommission = 0;
        if (Convert.ToInt32(hdnSalesPersonId.Value) > 0)
        {
            sales_person sp_info = new sales_person();
            sp_info = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
            if (sp_info.com_per != null)
                Comper = Convert.ToDecimal(sp_info.com_per);
            //decimal nProjecttotal = Convert.ToDecimal(lblProjectTotal.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
            decimal nProjecttotal = Convert.ToDecimal(hdnProjectSubTotal.Value.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
            decimal nProjecttotalExCom = Convert.ToDecimal(hdnProjectSubTotalExCom.Value.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));


            decimal nMatrial = 0;
            string strId = string.Empty;
            foreach (GridViewRow di in grdVendorCost.Rows)
            {
                
                    DropDownList ddlCategory = (DropDownList)di.FindControl("ddlCategory");
                    TextBox txtAmount = (TextBox)di.FindControl("txtAmount");
                    DropDownList ddlTrade = (DropDownList)di.FindControl("ddlTrade");

                    if (Convert.ToInt32(ddlCategory.SelectedValue) == 1)
                    {
                        nMatrial += Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                        if (strId.Length == 0)
                        {
                            strId = Convert.ToInt32(ddlTrade.SelectedValue).ToString();
                        }
                        else
                        {
                            strId = strId + ", " + Convert.ToInt32(ddlTrade.SelectedValue).ToString();
                        }

                    }


                

            }
            if (nMatrial > 0)
            {
                if (strId.Length > 0)
                {
                    decimal excludeAmount = 0;
                    string strExQ = "SELECT ISNULL(Sum(total_retail_price),0) AS Total FROM pricing_details WHERE customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and section_level IN(" + strId + ") and is_CommissionExclude = 1 ";
                    DataTable dt = csCommonUtility.GetDataTable(strExQ);
                    if (dt.Rows.Count > 0)
                    {
                        excludeAmount = Convert.ToDecimal(dt.Rows[0]["Total"]);
                    }
                    nProjecttotalExCom = (nProjecttotalExCom + excludeAmount) - nMatrial;
                }
            }


            lblComEligibleContract.Text = nProjecttotalExCom.ToString("c");


            if (Comper > 0 && nProjecttotalExCom > 0)
            {
                sCommission = nProjecttotalExCom * (Comper / 100);
            }
            lblSalesPerson.Text = sp_info.first_name + " " + sp_info.last_name;

        }

        var item = from it in _db.estimate_commissions
                   where it.customer_id == Convert.ToInt32(hdnCustomerId.Value) && it.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && it.client_id == Convert.ToInt32(hdnClientId.Value)
                   select new EstCom()
                   {
                       client_id = (int)it.client_id,
                       estimate_com_id = (int)it.estimate_com_id,
                       customer_id = (int)it.customer_id,
                       estimate_id = (int)it.estimate_id,
                       comission_amount = (decimal)it.comission_amount

                   };
        foreach (EstCom cominfo in item)
        {

            DataRow drNew = tmpCTable.NewRow();
            drNew["client_id"] = cominfo.client_id;
            drNew["estimate_com_id"] = cominfo.estimate_com_id;
            drNew["customer_id"] = cominfo.customer_id;
            drNew["estimate_id"] = cominfo.estimate_id;
            drNew["comission_amount"] = cominfo.comission_amount;
            tmpCTable.Rows.Add(drNew);
        }
        if (tmpCTable.Rows.Count == 0)
        {
            DataRow drNew = tmpCTable.NewRow();
            drNew["client_id"] = 1;
            drNew["estimate_com_id"] = 0;
            drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew["comission_amount"] = sCommission;
            tmpCTable.Rows.Add(drNew);
        }
        Session.Add("Com_est", tmpCTable);
        grdCom.DataSource = tmpCTable;
        grdCom.DataKeyNames = new string[] { "estimate_com_id" };
        grdCom.DataBind();



    }

    private void LoadCO_Commission()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpCTable = LoadCO_ComTable();
        decimal CoComper = 0;
        decimal sCOCommission = 0;
        if (Convert.ToInt32(hdnSalesPersonId.Value) > 0)
        {
            sales_person sp_info = new sales_person();
            sp_info = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
            if (sp_info.co_com_per != null)
                CoComper = Convert.ToDecimal(sp_info.co_com_per);
            // decimal nCOtotal = Convert.ToDecimal(lblTotalCOAmount.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
            decimal nCOtotal = Convert.ToDecimal(hdnTotalCOWithoutTax.Value.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
            decimal nCOtotalExCom = Convert.ToDecimal(hdnTotalCOExCom.Value.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
            lblComEligibleCO.Text = nCOtotalExCom.ToString("c");
            if (CoComper > 0 && nCOtotalExCom > 0)
            {
                sCOCommission = nCOtotalExCom * (CoComper / 100);
            }


        }

        var item = from it in _db.co_estimate_commissions
                   where it.customer_id == Convert.ToInt32(hdnCustomerId.Value) && it.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && it.client_id == Convert.ToInt32(hdnClientId.Value)
                   select new EstCO_Com()
                   {
                       client_id = (int)it.client_id,
                       co_estimate_com_id = (int)it.co_estimate_com_id,
                       customer_id = (int)it.customer_id,
                       estimate_id = (int)it.estimate_id,
                       comission_amount = (decimal)it.comission_amount

                   };
        foreach (EstCO_Com cominfo in item)
        {

            DataRow drNew = tmpCTable.NewRow();
            drNew["client_id"] = cominfo.client_id;
            drNew["co_estimate_com_id"] = cominfo.co_estimate_com_id;
            drNew["customer_id"] = cominfo.customer_id;
            drNew["estimate_id"] = cominfo.estimate_id;
            drNew["comission_amount"] = cominfo.comission_amount;
            tmpCTable.Rows.Add(drNew);
        }
        if (tmpCTable.Rows.Count == 0)
        {
            DataRow drNew = tmpCTable.NewRow();
            drNew["client_id"] = Convert.ToInt32(hdnClientId.Value);
            drNew["co_estimate_com_id"] = 0;
            drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew["comission_amount"] = sCOCommission;
            tmpCTable.Rows.Add(drNew);
        }
        Session.Add("CO_Com_est", tmpCTable);
        grdCom_CO.DataSource = tmpCTable;
        grdCom_CO.DataKeyNames = new string[] { "co_estimate_com_id" };
        grdCom_CO.DataBind();



    }


    private void LoadSection()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpSTable = LoadSectionTable();
        DataRow dr = tmpSTable.NewRow();
        dr["section_id"] = -1;
        dr["section_name"] = "Select";
        tmpSTable.Rows.Add(dr);

        var item = from it in _db.customer_sections
                   join si in _db.sectioninfos on it.section_id equals si.section_id
                   where it.customer_id == Convert.ToInt32(hdnCustomerId.Value) && it.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && it.client_id == Convert.ToInt32(hdnClientId.Value)
                   select new SectionInfo()
                   {
                       section_id = (int)it.section_id,
                       section_name = si.section_name

                   };
        foreach (SectionInfo sinfo in item)
        {

            DataRow drNew = tmpSTable.NewRow();
            drNew["section_id"] = sinfo.section_id;
            drNew["section_name"] = sinfo.section_name;
            tmpSTable.Rows.Add(drNew);
        }



        Session.Add("Section", tmpSTable);
        dtSection = (DataTable)Session["Section"];

    }
    private void LoadVendor()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpVTable = LoadVendorTable();
        DataRow dr = tmpVTable.NewRow();
        dr["vendor_id"] = -1;
        dr["vendor_name"] = "Select";
        tmpVTable.Rows.Add(dr);

        var item = from it in _db.Vendors
                   where it.client_id == 1 && it.is_active == true
                   orderby it.vendor_name
                   select new vendorInfrmation()
                   {
                       vendor_id = (int)it.vendor_id,
                       vendor_name = (string)it.vendor_name
                   };
        foreach (vendorInfrmation vi in item)
        {

            DataRow drNew = tmpVTable.NewRow();
            drNew["vendor_id"] = vi.vendor_id;
            drNew["vendor_name"] = vi.vendor_name;
            tmpVTable.Rows.Add(drNew);
        }



        Session.Add("Vendor", tmpVTable);
        dtVendor = (DataTable)Session["Vendor"];

    }

    private void LoadDscription()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpTable = LoadDataTable();

        var item = from it in _db.vendor_costs
                   where it.customer_id == Convert.ToInt32(hdnCustomerId.Value) && it.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && it.client_id == Convert.ToInt32(hdnClientId.Value)
                   select new VendorCost()
                   {
                       vendor_cost_id = (int)it.vendor_cost_id,
                       vendor_id = (int)it.vendor_id,
                       client_id = (int)it.client_id,
                       customer_id = (int)it.customer_id,
                       estimate_id = (int)it.estimate_id,
                       section_id = (int)it.section_id,
                       category_id = (int)it.category_id,
                       cost_description = it.cost_description,
                       cost_amount = (decimal)it.cost_amount,
                       cost_date = (DateTime)it.cost_date,
                       create_date = (DateTime)it.create_date
                   };
        foreach (VendorCost jsc in item)
        {

            DataRow drNew = tmpTable.NewRow();
            drNew["vendor_cost_id"] = jsc.vendor_cost_id;
            drNew["vendor_id"] = jsc.vendor_id;
            drNew["client_id"] = jsc.client_id;
            drNew["customer_id"] = jsc.customer_id;
            drNew["estimate_id"] = jsc.estimate_id;
            drNew["section_id"] = jsc.section_id;
            drNew["cost_description"] = jsc.cost_description;
            drNew["category_id"] = jsc.category_id;
            drNew["cost_amount"] = jsc.cost_amount;
            drNew["cost_date"] = jsc.cost_date;
            drNew["create_date"] = jsc.create_date;
            drNew["vendor_name"] = "";
            tmpTable.Rows.Add(drNew);
        }
        if (tmpTable.Rows.Count == 0)
        {
            DataRow drNew = tmpTable.NewRow();
            drNew["vendor_cost_id"] = 0;
            drNew["vendor_id"] = -1;
            drNew["client_id"] = Convert.ToInt32(hdnClientId.Value);
            drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew["section_id"] = -1;
            drNew["cost_description"] = "";
            drNew["category_id"] = 1;
            drNew["cost_amount"] = 0;
            drNew["cost_date"] = DateTime.Now;
            drNew["create_date"] = DateTime.Now;
            drNew["vendor_name"] = "";
            tmpTable.Rows.Add(drNew);
        }
        Session.Add("vendor_cost_desc", tmpTable);
        DataView dv = tmpTable.DefaultView;
        dv.Sort = "cost_date ASC";
        grdVendorCost.DataSource = dv.ToTable();
        grdVendorCost.DataKeyNames = new string[] { "vendor_cost_id", "vendor_id" };
        grdVendorCost.DataBind();

    }
    private DataTable LoadDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("vendor_cost_id", typeof(int));
        table.Columns.Add("vendor_id", typeof(int));
        table.Columns.Add("client_id", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("section_id", typeof(int));
        table.Columns.Add("category_id", typeof(int));
        table.Columns.Add("cost_amount", typeof(decimal));
        table.Columns.Add("cost_description", typeof(string));
        table.Columns.Add("cost_date", typeof(DateTime));
        table.Columns.Add("create_date", typeof(DateTime));
        table.Columns.Add("vendor_name", typeof(string));
        return table;
    }
    private DataTable LoadEstimateTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("estimate_name", typeof(string));
        return table;
    }
    private DataTable LoadComTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("estimate_com_id", typeof(int));
        table.Columns.Add("client_id", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("comission_amount", typeof(decimal));

        return table;
    }
    private DataTable LoadCO_ComTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("co_estimate_com_id", typeof(int));
        table.Columns.Add("client_id", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("comission_amount", typeof(decimal));

        return table;
    }
    private DataTable LoadVendorTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("vendor_id", typeof(int));
        table.Columns.Add("vendor_name", typeof(string));

        return table;
    }
    private DataTable LoadSectionTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("section_id", typeof(int));
        table.Columns.Add("section_name", typeof(string));
        return table;
    }
    protected void grdVendorCost_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "Save")
            {
                bool IsRequiredPass = true;
                DataClassesDataContext _db = new DataClassesDataContext();
                DataTable table = (DataTable)Session["vendor_cost_desc"];
                dtVendor = (DataTable)Session["Vendor"];
                string strRequired = "";
                string strVendorName = "";
                int nIndex = -1;
                foreach (GridViewRow di in grdVendorCost.Rows)
                {
                    DropDownList ddlVendor = (DropDownList)di.FindControl("ddlVendor");
                    DropDownList ddlTrade = (DropDownList)di.FindControl("ddlTrade");
                    DropDownList ddlCategory = (DropDownList)di.FindControl("ddlCategory");
                    TextBox txtDescription = (TextBox)di.FindControl("txtDescription");
                    TextBox txtDate = (TextBox)di.FindControl("txtDate");
                    TextBox txtAmount = (TextBox)di.FindControl("txtAmount");

                    Label lblDescription = (Label)di.FindControl("lblDescription");
                    Label lblDate = (Label)di.FindControl("lblDate");
                    Label lblAmount = (Label)di.FindControl("lblAmount");
                    DataRow dr = table.Rows[di.RowIndex];

                    if (Convert.ToInt32(ddlVendor.SelectedValue) == -1)
                    {
                        strRequired = "Vendor is required.<br/>";
                        ddlVendor.BorderColor = Color.Red;
                    }

                    if (Convert.ToInt32(ddlTrade.SelectedValue) == -1)
                    {
                        strRequired += "Section is required.<br/>";
                        ddlTrade.BorderColor = Color.Red;
                    }

                    if (Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) == 0)
                    {
                        strRequired += "Amount is required.<br/>";
                        txtAmount.BorderColor = Color.Red;
                    }


                    if (strRequired.Length > 0)
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                        IsRequiredPass = false;
                    }

                    else
                    {

                        dr["vendor_cost_id"] = Convert.ToInt32(grdVendorCost.DataKeys[di.RowIndex].Values[0]);
                        dr["vendor_id"] = Convert.ToInt32(ddlVendor.SelectedValue);
                        dr["client_id"] = Convert.ToInt32(hdnClientId.Value);
                        dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                        dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                        dr["section_id"] = Convert.ToInt32(ddlTrade.SelectedValue);
                        dr["cost_description"] = txtDescription.Text;
                        dr["category_id"] = Convert.ToInt32(ddlCategory.SelectedValue);
                        dr["cost_amount"] = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                        dr["cost_date"] = Convert.ToDateTime(txtDate.Text);
                        dr["create_date"] = DateTime.Now;
                        dr["vendor_name"] = ddlVendor.SelectedItem.Text.Replace("&", "and");

                        if (Convert.ToInt32(dr["vendor_cost_id"]) == 0)
                        {
                            nIndex = Convert.ToInt32(di.RowIndex);
                        }
                    }
                }
                if (IsRequiredPass)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        strVendorName = dr["vendor_name"].ToString();

                        vendor_cost ven_cost = new vendor_cost();
                        if (Convert.ToInt32(dr["vendor_cost_id"]) > 0)
                            ven_cost = _db.vendor_costs.Single(l => l.vendor_cost_id == Convert.ToInt32(dr["vendor_cost_id"]));

                        if (Convert.ToDecimal(dr["cost_amount"]) != 0)
                        {
                            ven_cost.vendor_cost_id = Convert.ToInt32(dr["vendor_cost_id"]);
                            ven_cost.vendor_id = Convert.ToInt32(dr["vendor_id"]);
                            ven_cost.client_id = Convert.ToInt32(dr["client_id"]);
                            ven_cost.customer_id = Convert.ToInt32(dr["customer_id"]);
                            ven_cost.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                            ven_cost.section_id = Convert.ToInt32(dr["section_id"]);
                            ven_cost.category_id = Convert.ToInt32(dr["category_id"]);
                            ven_cost.cost_description = dr["cost_description"].ToString();
                            ven_cost.cost_amount = Convert.ToDecimal(dr["cost_amount"]);
                            ven_cost.cost_date = Convert.ToDateTime(dr["cost_date"]);
                            ven_cost.create_date = Convert.ToDateTime(dr["create_date"]);
                        }
                        if (Convert.ToInt32(dr["vendor_cost_id"]) == 0)
                        {
                            if (Convert.ToInt32(dr["category_id"]) == 4)
                            {
                                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Allowance Item addition not allowed, Allowance Item will be added from popup for Allowance available section.<br/>");
                                return;
                            }

                            _db.vendor_costs.InsertOnSubmit(ven_cost);
                            _db.SubmitChanges();

                            //File Upload
                            FileUpload file_upload = (FileUpload)grdVendorCost.Rows[nIndex].FindControl("file_upload");
                            int nVendorCostID = ven_cost.vendor_cost_id;
                            string strFileExt = "";
                            if (file_upload.HasFiles)
                            {
                                foreach (var file in file_upload.PostedFiles)
                                {
                                    file_upload_info fui = new file_upload_info();



                                    strFileExt = Path.GetExtension(file.FileName);
                                    string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                                    if (strFileExt != ".pdf" && strFileExt != ".doc" && strFileExt != ".docx" && strFileExt != ".xls" && strFileExt != ".xlsx" && strFileExt != ".csv" && strFileExt != ".txt" && strFileExt != ".jpg" && strFileExt != ".jpeg" && strFileExt != ".png" && strFileExt != ".gif")
                                    {
                                        strRequired = "Invalid file type, This (" + strFileExt + ")  file type is not allowed to upload.";

                                    }

                                    if (file.FileName.Length == 0)
                                    {
                                        strRequired = "Invalid file name.";
                                    }

                                    if (strRequired.Length > 0)
                                    {
                                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                                        return;
                                    }

                                    if (strRequired.Length == 0)
                                    {
                                        string sFileName = originalFileName + "_" + DateTime.Now.Ticks.ToString() + strFileExt;

                                        string sFilePath = System.Configuration.ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "\\VENDOR\\" + strVendorName + "\\";
                                        if (Directory.Exists(sFilePath) == false)
                                        {
                                            Directory.CreateDirectory(sFilePath);
                                        }
                                        sFilePath = sFilePath + "\\" + sFileName;
                                        file.SaveAs(sFilePath);

                                        fui.client_id = Convert.ToInt32(hdnClientId.Value);
                                        fui.CustomerId = Convert.ToInt32(hdnCustomerId.Value);
                                        fui.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                                        fui.Desccription = "";
                                        fui.ImageName = sFileName;
                                        fui.is_design = false;
                                        fui.IsSiteProgress = false;
                                        fui.type = 1; // Vendor Upload
                                        fui.vendor_cost_id = nVendorCostID;
                                        fui.dms_dirid = 0;
                                        fui.dms_fileid = 0;
                                        _db.file_upload_infos.InsertOnSubmit(fui);
                                        _db.SubmitChanges();
                                    }
                                }
                            }

                        }
                    }

                    lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");

                    _db.SubmitChanges();

                    dtVendor = (DataTable)Session["Vendor"];
                    dtSection = (DataTable)Session["Section"];
                    LoadDscription();
                    Calculate();
                }
            }
        }
        catch (Exception ex) { lblResult.Text = ex.Message; }
    }

    protected void grdVendorCost_RowEditing(object sender, GridViewEditEventArgs e)
    {

        HtmlControl div = (HtmlControl)grdVendorCost.Rows[e.NewEditIndex].FindControl("dvCalender");
        DropDownList ddlVendor = (DropDownList)grdVendorCost.Rows[e.NewEditIndex].FindControl("ddlVendor");
        DropDownList ddlTrade = (DropDownList)grdVendorCost.Rows[e.NewEditIndex].FindControl("ddlTrade");
        DropDownList ddlCategory = (DropDownList)grdVendorCost.Rows[e.NewEditIndex].FindControl("ddlCategory");
        TextBox txtDescription = (TextBox)grdVendorCost.Rows[e.NewEditIndex].FindControl("txtDescription");
        TextBox txtDate = (TextBox)grdVendorCost.Rows[e.NewEditIndex].FindControl("txtDate");
        TextBox txtAmount = (TextBox)grdVendorCost.Rows[e.NewEditIndex].FindControl("txtAmount");

        Label lblDescription = (Label)grdVendorCost.Rows[e.NewEditIndex].FindControl("lblDescription");
        Label lblDate = (Label)grdVendorCost.Rows[e.NewEditIndex].FindControl("lblDate");
        Label lblAmount = (Label)grdVendorCost.Rows[e.NewEditIndex].FindControl("lblAmount");
        string str = lblDescription.Text.Replace("&nbsp;", "");
        if (str.IndexOf(">>") != -1)
        {
            txtDescription.Visible = false;
            lblDescription.Visible = true;
        }
        else
        {
            txtDescription.Visible = true;
            lblDescription.Visible = false;
        }
        // txtDate.Visible = true;
        div.Visible = true;
        lblDate.Visible = false;
        txtAmount.Visible = true;
        lblAmount.Visible = false;

        ddlVendor.Enabled = true;
        ddlTrade.Enabled = true;
        ddlCategory.Enabled = true;

        LinkButton btn = (LinkButton)grdVendorCost.Rows[e.NewEditIndex].Cells[7].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

        //Set Focus      
        txtDate.Focus();

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);

    }
    protected void grdVendorCost_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strRequired = "";
        DropDownList ddlVendor = (DropDownList)grdVendorCost.Rows[e.RowIndex].FindControl("ddlVendor");
        DropDownList ddlTrade = (DropDownList)grdVendorCost.Rows[e.RowIndex].FindControl("ddlTrade");
        DropDownList ddlCategory = (DropDownList)grdVendorCost.Rows[e.RowIndex].FindControl("ddlCategory");
        TextBox txtDescription = (TextBox)grdVendorCost.Rows[e.RowIndex].FindControl("txtDescription");
        TextBox txtDate = (TextBox)grdVendorCost.Rows[e.RowIndex].FindControl("txtDate");
        TextBox txtAmount = (TextBox)grdVendorCost.Rows[e.RowIndex].FindControl("txtAmount");

        Label lblDescription = (Label)grdVendorCost.Rows[e.RowIndex].FindControl("lblDescription");
        Label lblDate = (Label)grdVendorCost.Rows[e.RowIndex].FindControl("lblDate");
        Label lblAmount = (Label)grdVendorCost.Rows[e.RowIndex].FindControl("lblAmount");
        if (Convert.ToInt32(ddlVendor.SelectedValue) == -1)
        {
            strRequired = "Vendor is required.<br>";
            ddlVendor.BorderColor = Color.Red;

        }

        if (Convert.ToInt32(ddlTrade.SelectedValue) == -1)
        {
            strRequired += "Section is required.<br>";
            ddlTrade.BorderColor = Color.Red;

        }

        if (Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) == 0)
        {
            strRequired += "Amount is required.<br>";
            txtAmount.BorderColor = Color.Red;

        }

        if (strRequired.Length > 0)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
        }
        else
        {
            int nvendor_cost_id = Convert.ToInt32(grdVendorCost.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
            string strQ = "UPDATE vendor_cost SET category_id=" + Convert.ToInt32(ddlCategory.SelectedValue) + " , section_id=" + Convert.ToInt32(ddlTrade.SelectedValue) + " , vendor_id=" + Convert.ToInt32(ddlVendor.SelectedValue) + " ,cost_description='" + txtDescription.Text.Replace("'", "''") + "',cost_date='" + Convert.ToDateTime(txtDate.Text.Replace("'", "''")) + "' ,cost_amount=" + Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) + "  WHERE vendor_cost_id =" + nvendor_cost_id + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);
            _db.ExecuteCommand(strQ, string.Empty);
            dtVendor = (DataTable)Session["Vendor"];
            dtSection = (DataTable)Session["Section"];
            LoadDscription();
            Calculate();
            lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
        }

    }

    protected void grdVendorCost_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            HtmlControl div = (HtmlControl)e.Row.FindControl("dvCalender");

            int nVendorCostId = Convert.ToInt32(grdVendorCost.DataKeys[e.Row.RowIndex].Values[0]);
            int nVendorId = Convert.ToInt32(grdVendorCost.DataKeys[e.Row.RowIndex].Values[1]);

            dtVendor = (DataTable)Session["Vendor"];
            dtSection = (DataTable)Session["Section"];
            DropDownList ddlVendor = (DropDownList)e.Row.FindControl("ddlVendor");
            DropDownList ddlTrade = (DropDownList)e.Row.FindControl("ddlTrade");
            DropDownList ddlCategory = (DropDownList)e.Row.FindControl("ddlCategory");
            TextBox txtDescription = (TextBox)e.Row.FindControl("txtDescription");
            TextBox txtDate = (TextBox)e.Row.FindControl("txtDate");
            TextBox txtAmount = (TextBox)e.Row.FindControl("txtAmount");

            Label lblDescription = (Label)e.Row.FindControl("lblDescription");
            Label lblDate = (Label)e.Row.FindControl("lblDate");
            Label lblAmount = (Label)e.Row.FindControl("lblAmount");
            string str = lblDescription.Text.Replace("&nbsp;", "");

            ListItem item = ddlVendor.Items.FindByValue(nVendorId.ToString());
            if (item != null)
                ddlVendor.Items.FindByValue(Convert.ToString(nVendorId)).Selected = true;
            else
            {
                string str1 = nVendorId.ToString();
                DataClassesDataContext _db = new DataClassesDataContext();

                var objVendor = _db.Vendors.FirstOrDefault(w => w.vendor_id == nVendorId);
                if (objVendor != null)
                {
                    ddlVendor.Items.Insert(0, new ListItem(objVendor.vendor_name, str1));
                }
            }
            LinkButton btn = (LinkButton)e.Row.Cells[7].Controls[0];
            if (Convert.ToDecimal(lblAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) == 0)
            {
                txtDescription.Visible = true;
                lblDescription.Visible = false;
                //txtDate.Visible = true;
                div.Visible = true;
                lblDate.Visible = false;
                txtAmount.Visible = true;
                lblAmount.Visible = false;
                ddlVendor.Enabled = true;
                ddlTrade.Enabled = true;
                ddlCategory.Enabled = true;

                btn.Text = "Save";
                btn.CommandName = "Save";

            }
            else
            {
                if (str != "" && str.Length > 60)
                {
                    txtDescription.Text = str;
                    lblDescription.Text = str.Substring(0, 60) + "...";
                    lblDescription.ToolTip = str;
                    //lnkOpen.Visible = true;

                }
                else
                {
                    txtDescription.Text = str;
                    lblDescription.Text = str;
                    lblDescription.ToolTip = str;
                    //lnkOpen.Visible = false;

                }
            }
            if (nVendorCostId != 0)
            {
                GridView gv = e.Row.FindControl("grdUploadedFileList") as GridView;
                GetUploadedFileListData(gv, nVendorCostId);
            }

            btn.Attributes.Add("onclick", "ShowProgress();");
        }

    }

    protected void grdVendorCost_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {

            lblResult.Text = "";
            string strVendorName = "";
            DataClassesDataContext _db = new DataClassesDataContext();
            int nvendor_cost_id = Convert.ToInt32(grdVendorCost.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);

            int nVendorId = Convert.ToInt32(grdVendorCost.DataKeys[Convert.ToInt32(e.RowIndex)].Values[1]);
            DropDownList ddlVendor = (DropDownList)grdVendorCost.Rows[e.RowIndex].FindControl("ddlVendor");
            ListItem LstVendorItem = ddlVendor.Items.FindByValue(nVendorId.ToString());

            if (LstVendorItem != null)
            {
                strVendorName = LstVendorItem.Text;
            }

            if (nvendor_cost_id > 0)
            {
                string strQ = "Delete vendor_cost  WHERE vendor_cost_id =" + nvendor_cost_id + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);
                _db.ExecuteCommand(strQ, string.Empty);

                //Delete From DB Table
                List<file_upload_info> fui = _db.file_upload_infos.Where(f => f.vendor_cost_id == nvendor_cost_id && f.CustomerId == Convert.ToInt32(hdnCustomerId.Value) && f.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).ToList();

                //Delete From Physical Path/Folder
                foreach (file_upload_info li in fui)
                {
                    string sFileName = li.ImageName;

                    string strPhysicalPath = System.Configuration.ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "\\VENDOR\\" + strVendorName + "\\";
                    string strMainPath = strPhysicalPath + "\\" + sFileName;

                    if (File.Exists(strMainPath))
                    {
                        File.Delete(strMainPath);
                    }

                }

                dtVendor = (DataTable)Session["Vendor"];
                dtSection = (DataTable)Session["Section"];
                LoadDscription();
                Calculate();

                lblResult.Text = csCommonUtility.GetSystemMessage("Item deleted successfully");
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = ex.Message;
        }

    }

    private void GetUploadedFileListData(GridView grd, int nVendorCostId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        List<file_upload_info> fui = new List<file_upload_info>();

        fui = _db.file_upload_infos.Where(f => f.vendor_cost_id == nVendorCostId).ToList();

        grd.DataSource = fui;
        grd.DataKeyNames = new string[] { "upload_fileId", "ImageName" };
        grd.DataBind();
    }

    protected void grdUploadedFileList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdUploadedFileList = (GridView)sender;


            int nUploadFileId = Convert.ToInt32(grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[0]);
            string strImageName = grdUploadedFileList.DataKeys[e.Row.RowIndex].Values[1].ToString();

            string strVendorName = "";

            DropDownList ddlVendor = (DropDownList)e.Row.Parent.Parent.Parent.FindControl("ddlVendor");

            if (ddlVendor != null)
            {
                strVendorName = ddlVendor.SelectedItem.Text.Replace("&", "and");
            }

            HyperLink hypUploadedFile = (HyperLink)e.Row.FindControl("hypUploadedFile");
            hypUploadedFile.Text = strImageName;
            hypUploadedFile.NavigateUrl = "UploadedFiles/" + hdnCustomerId.Value + "/VENDOR/" + strVendorName + "/" + strImageName;
            hypUploadedFile.Target = "_blank";

            Button btnDeleteUploadedFile = (Button)e.Row.FindControl("btnDeleteUploadedFile");
            btnDeleteUploadedFile.CommandArgument = nUploadFileId.ToString();

        }
    }

    protected void DeleteUploadedFile(object sender, EventArgs e)
    {
        try
        {
            lblResult.Text = "";
            lblResult2.Text = "";
            DataClassesDataContext _db = new DataClassesDataContext();

            Button btnDeleteUploadedFile = (Button)sender;
            int nUploadFileId = Convert.ToInt32(btnDeleteUploadedFile.CommandArgument);

            GridViewRow grdUploadedFileListc = (GridViewRow)((Button)sender).NamingContainer;
            GridView Childgrid = (GridView)(grdUploadedFileListc.Parent.Parent);
            GridViewRow grdVendorCostp = (GridViewRow)(Childgrid.NamingContainer);
            int Index = grdVendorCostp.RowIndex;

            int nvendor_cost_id = Convert.ToInt32(grdVendorCost.DataKeys[Index].Values[0]);

            int nVendorId = Convert.ToInt32(grdVendorCost.DataKeys[Index].Values[1]);
            DropDownList ddlVendor = (DropDownList)grdVendorCost.Rows[Index].FindControl("ddlVendor");
            ListItem LstVendorItem = ddlVendor.Items.FindByValue(nVendorId.ToString());
            string strVendorName = "";

            if (LstVendorItem != null)
            {
                strVendorName = LstVendorItem.Text;
            }

            //Delete From DB Table
            List<file_upload_info> fui = _db.file_upload_infos.Where(f => f.vendor_cost_id == nvendor_cost_id && f.CustomerId == Convert.ToInt32(hdnCustomerId.Value) && f.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).ToList();

            //Delete From Physical Path/Folder
            foreach (file_upload_info li in fui)
            {
                string sFileName = li.ImageName;

                string strPhysicalPath = System.Configuration.ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "\\VENDOR\\" + strVendorName + "\\";
                string strMainPath = strPhysicalPath + "\\" + sFileName;

                if (File.Exists(strMainPath))
                {
                    File.Delete(strMainPath);
                }

            }

            string strQ = "Delete file_upload_info WHERE upload_fileId=" + nUploadFileId + " AND client_id =" + Convert.ToInt32(hdnClientId.Value);
            _db.ExecuteCommand(strQ, string.Empty);

            dtVendor = (DataTable)Session["Vendor"];
            dtSection = (DataTable)Session["Section"];
            LoadDscription();
            Calculate();


            //Set Focus
            int j = 0;
            int nIndx = Index + 1;
            if (nIndx < grdVendorCost.Rows.Count)
            {
                j = nIndx;
                grdVendorCost.Rows[j].Cells[6].Focus();
            }
            else
                btnAddnewRow.Focus();
            //End Set Focus


            lblResult.Text = csCommonUtility.GetSystemMessage("Data Updated successfully");
            lblResult2.Text = csCommonUtility.GetSystemMessage("Data Updated successfully");
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            lblResult2.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    }
    protected void btnAddnewRow_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddnewRow.ID, btnAddnewRow.GetType().Name, "Click"); 
        DataTable table = (DataTable)Session["vendor_cost_desc"];
        dtVendor = (DataTable)Session["Vendor"];
        dtSection = (DataTable)Session["Section"];

        int i = 0;
        if (table.Rows.Count > 0)
        {
            btnAddnewRow.Focus();
            i = table.Rows.Count - 1;
            if (Convert.ToInt32(table.Rows[i]["vendor_cost_id"]) == 0)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("You have already a pending item to save.");
                return;
            }
        }

        //for (int i = 0; i < table.Rows.Count; i++)
        //{
        //    if (Convert.ToInt32(table.Rows[i]["vendor_cost_id"]) == 0)
        //    {
        //        return;
        //    }
        //}

        foreach (GridViewRow di in grdVendorCost.Rows)
        {
            {
                DropDownList ddlVendor = (DropDownList)di.FindControl("ddlVendor");
                DropDownList ddlTrade = (DropDownList)di.FindControl("ddlTrade");
                DropDownList ddlCategory = (DropDownList)di.FindControl("ddlCategory");
                TextBox txtDescription = (TextBox)di.FindControl("txtDescription");
                TextBox txtDate = (TextBox)di.FindControl("txtDate");
                TextBox txtAmount = (TextBox)di.FindControl("txtAmount");

                Label lblDescription = (Label)di.FindControl("lblDescription");
                Label lblDate = (Label)di.FindControl("lblDate");
                Label lblAmount = (Label)di.FindControl("lblAmount");
                DataRow dr = table.Rows[di.RowIndex];

                dr["vendor_cost_id"] = Convert.ToInt32(grdVendorCost.DataKeys[di.RowIndex].Values[0]);
                dr["vendor_id"] = Convert.ToInt32(ddlVendor.SelectedValue);
                dr["client_id"] = Convert.ToInt32(hdnClientId.Value);
                dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                dr["section_id"] = Convert.ToInt32(ddlTrade.SelectedValue);
                dr["cost_description"] = txtDescription.Text;
                dr["category_id"] = Convert.ToInt32(ddlCategory.SelectedValue);
                dr["cost_amount"] = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                dr["cost_date"] = Convert.ToDateTime(txtDate.Text);
                dr["create_date"] = DateTime.Now;

            }

        }
        DataRow drNew = table.NewRow();
        drNew["vendor_cost_id"] = 0;
        drNew["vendor_id"] = -1;
        drNew["client_id"] = Convert.ToInt32(hdnClientId.Value);
        drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
        drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
        drNew["section_id"] = -1;
        drNew["cost_description"] = "";
        drNew["category_id"] = 1;
        drNew["cost_amount"] = 0;
        drNew["cost_date"] = DateTime.Now;
        drNew["create_date"] = DateTime.Now;
        table.Rows.Add(drNew);

        Session.Add("vendor_cost_desc", table);
        grdVendorCost.DataSource = table;
        grdVendorCost.DataKeyNames = new string[] { "vendor_cost_id", "vendor_id" };
        grdVendorCost.DataBind();
        lblResult.Text = "";

        ////Set Focus           
        //int j = 0;
        //int nIndx = i + 1;
        //if (nIndx < grdVendorCost.Rows.Count)
        //{
        //    j = nIndx;
        //    grdVendorCost.Rows[j].Cells[6].Focus();
        //}
        //else
        btnAddnewRow.Focus();
        //End Set Focus

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);

    }

    protected void grdCom_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TextBox txtComAmount = (TextBox)e.Row.FindControl("txtComAmount");
            Label lblComAmount = (Label)e.Row.FindControl("lblComAmount");


        }
    }
    protected void grdCom_RowEditing(object sender, GridViewEditEventArgs e)
    {

        TextBox txtComAmount = (TextBox)grdCom.Rows[e.NewEditIndex].FindControl("txtComAmount");
        Label lblComAmount = (Label)grdCom.Rows[e.NewEditIndex].FindControl("lblComAmount");

        txtComAmount.Visible = true;
        lblComAmount.Visible = false;

        LinkButton btn = (LinkButton)grdCom.Rows[e.NewEditIndex].Cells[1].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdCom_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        int nCom_id = Convert.ToInt32(grdCom.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);

        DataTable dtCom = (DataTable)Session["Com_est"];
        foreach (GridViewRow di in grdCom.Rows)
        {
            {

                TextBox txtComAmount = (TextBox)di.FindControl("txtComAmount");
                Label lblComAmount = (Label)di.FindControl("lblComAmount");

                DataRow dr = dtCom.Rows[di.RowIndex];

                dr["estimate_com_id"] = Convert.ToInt32(grdCom.DataKeys[di.RowIndex].Values[0]);
                dr["client_id"] = Convert.ToInt32(hdnClientId.Value);
                dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                dr["comission_amount"] = Convert.ToDecimal(txtComAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            }

        }

        foreach (DataRow dr in dtCom.Rows)
        {
            estimate_commission est_cost = new estimate_commission();
            if (Convert.ToInt32(dr["estimate_com_id"]) > 0)
                est_cost = _db.estimate_commissions.Single(l => l.estimate_com_id == Convert.ToInt32(dr["estimate_com_id"]));
            if (Convert.ToDecimal(dr["comission_amount"]) != 0)
            {

                est_cost.estimate_com_id = Convert.ToInt32(dr["estimate_com_id"]);
                est_cost.client_id = Convert.ToInt32(dr["client_id"]);
                est_cost.customer_id = Convert.ToInt32(dr["customer_id"]);
                est_cost.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                est_cost.comission_amount = Convert.ToDecimal(dr["comission_amount"]);
            }

            if (Convert.ToInt32(dr["estimate_com_id"]) == 0)
            {
                _db.estimate_commissions.InsertOnSubmit(est_cost);
            }
        }
        _db.SubmitChanges();
        LoadCommission();
        LoadCO_Commission();
        Calculate();

        string strMessage = csCommonUtility.GetSystemMessage("Data updated successfully");
        lblResult.Text = strMessage;
        lblResult2.Text = strMessage;


    }

    protected void grdCom_CO_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TextBox txtComAmount = (TextBox)e.Row.FindControl("txtComAmount");
            Label lblComAmount = (Label)e.Row.FindControl("lblComAmount");


        }
    }
    protected void grdCom_CO_RowEditing(object sender, GridViewEditEventArgs e)
    {

        TextBox txtComAmount = (TextBox)grdCom_CO.Rows[e.NewEditIndex].FindControl("txtComAmount");
        Label lblComAmount = (Label)grdCom_CO.Rows[e.NewEditIndex].FindControl("lblComAmount");

        txtComAmount.Visible = true;
        lblComAmount.Visible = false;

        LinkButton btn = (LinkButton)grdCom_CO.Rows[e.NewEditIndex].Cells[1].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";
    }
    protected void grdCom_CO_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        int nCOCom_id = Convert.ToInt32(grdCom_CO.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);

        DataTable dtcoCom = (DataTable)Session["CO_Com_est"];
        foreach (GridViewRow di in grdCom_CO.Rows)
        {
            {

                TextBox txtComAmount = (TextBox)di.FindControl("txtComAmount");
                Label lblComAmount = (Label)di.FindControl("lblComAmount");

                DataRow dr = dtcoCom.Rows[di.RowIndex];

                dr["co_estimate_com_id"] = Convert.ToInt32(grdCom_CO.DataKeys[di.RowIndex].Values[0]);
                dr["client_id"] = Convert.ToInt32(hdnClientId.Value);
                dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                dr["comission_amount"] = Convert.ToDecimal(txtComAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            }

        }

        foreach (DataRow dr in dtcoCom.Rows)
        {
            co_estimate_commission co_est_cost = new co_estimate_commission();
            if (Convert.ToInt32(dr["co_estimate_com_id"]) > 0)
                co_est_cost = _db.co_estimate_commissions.Single(l => l.co_estimate_com_id == Convert.ToInt32(dr["co_estimate_com_id"]));
            if (Convert.ToDecimal(dr["comission_amount"]) != 0)
            {

                co_est_cost.co_estimate_com_id = Convert.ToInt32(dr["co_estimate_com_id"]);
                co_est_cost.client_id = Convert.ToInt32(dr["client_id"]);
                co_est_cost.customer_id = Convert.ToInt32(dr["customer_id"]);
                co_est_cost.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                co_est_cost.comission_amount = Convert.ToDecimal(dr["comission_amount"]);
            }

            if (Convert.ToInt32(dr["co_estimate_com_id"]) == 0)
            {
                _db.co_estimate_commissions.InsertOnSubmit(co_est_cost);
            }
        }
        _db.SubmitChanges();
        LoadCommission();
        LoadCO_Commission();
        Calculate();

        string strMessage = csCommonUtility.GetSystemMessage("Data updated successfully");
        lblResult.Text = strMessage;
        lblResult2.Text = strMessage;

    }
    protected void btnAcceptPayment_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAcceptPayment.ID, btnAcceptPayment.GetType().Name, "Click"); 

        if (Convert.ToInt32(hdnEstPaymentId.Value) > 0)
        {
            Response.Redirect("payment_withco.aspx?cid=" + hdnCustomerId.Value + "&epid=" + hdnEstPaymentId.Value + "&eid=" + hdnEstimateId.Value);
        }
        else
        {
            string strMessage = csCommonUtility.GetSystemRequiredMessage("Payment terms not yet saved Please click on Go To Payment to set payment terms ");
            lblResult.Text = strMessage;
            lblResult2.Text = strMessage;
            return;

        }
    }
    protected void btnGoToPayment_Click(object sender, EventArgs e)
    {

        Response.Redirect("payment_info.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {

        lblResult.Text = "";
        lblResult2.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        string strFileExt = "";
        string strRequired = string.Empty;

        int i = 0;
        ImageButton btnUpload1 = (ImageButton)grdVendorCost.FindControl("btnUpload");
        btnUpload1 = (ImageButton)sender;
        GridViewRow gvr = (GridViewRow)btnUpload1.NamingContainer;
        i = gvr.RowIndex;
        FileUpload file_upload = (FileUpload)grdVendorCost.Rows[i].FindControl("file_upload");
        int nVendorCostID = Convert.ToInt32(grdVendorCost.DataKeys[i].Values[0].ToString());


        try
        {
            if (file_upload.HasFiles)
            {
                DropDownList ddlVendor = (DropDownList)grdVendorCost.Rows[i].FindControl("ddlVendor");
                DropDownList ddlTrade = (DropDownList)grdVendorCost.Rows[i].FindControl("ddlTrade");
                DropDownList ddlCategory = (DropDownList)grdVendorCost.Rows[i].FindControl("ddlCategory");
                TextBox txtDescription = (TextBox)grdVendorCost.Rows[i].FindControl("txtDescription");
                TextBox txtDate = (TextBox)grdVendorCost.Rows[i].FindControl("txtDate");
                TextBox txtAmount = (TextBox)grdVendorCost.Rows[i].FindControl("txtAmount");
                string strVendorName = "";

                if (Convert.ToInt32(ddlVendor.SelectedValue) == -1)
                {
                    strRequired = "Vendor is required.<br/>";
                    ddlVendor.BorderColor = Color.Red;
                }

                if (Convert.ToInt32(ddlTrade.SelectedValue) == -1)
                {
                    strRequired += "Trade is required.<br/>";
                    ddlTrade.BorderColor = Color.Red;
                }

                if (Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) == 0)
                {
                    strRequired += "Amount is required";
                    txtAmount.BorderColor = Color.Red;
                }

                if (strRequired.Length > 0)
                {
                    lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                    lblResult2.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                    return;
                }
                strVendorName = ddlVendor.SelectedItem.Text.Replace("&", "and");
                vendor_cost ven_cost = new vendor_cost();
                if (nVendorCostID == 0)
                {

                    ven_cost.vendor_id = Convert.ToInt32(ddlVendor.SelectedValue);
                    ven_cost.client_id = Convert.ToInt32(hdnClientId.Value);
                    ven_cost.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    ven_cost.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                    ven_cost.section_id = Convert.ToInt32(ddlTrade.SelectedValue);
                    ven_cost.category_id = Convert.ToInt32(ddlCategory.SelectedValue);
                    ven_cost.cost_description = txtDescription.Text.ToString();
                    ven_cost.cost_amount = Convert.ToDecimal(txtAmount.Text.Replace("$", ""));
                    ven_cost.cost_date = Convert.ToDateTime(txtDate.Text);
                    ven_cost.create_date = DateTime.Now;

                    _db.vendor_costs.InsertOnSubmit(ven_cost);
                    _db.SubmitChanges();
                    nVendorCostID = ven_cost.vendor_cost_id;
                }
                //else
                //{
                //    ven_cost = _db.vendor_costs.SingleOrDefault(v => v.vendor_cost_id == nVendorCostID);


                //    ven_cost.section_id = Convert.ToInt32(ddlTrade.SelectedValue);
                //    ven_cost.category_id = Convert.ToInt32(ddlCategory.SelectedValue);

                //    ven_cost.cost_amount = Convert.ToDecimal(txtAmount.Text.Replace("$", ""));
                //    ven_cost.cost_date = Convert.ToDateTime(txtDate.Text);
                //    ven_cost.create_date = DateTime.Now;
                //}


                foreach (var file in file_upload.PostedFiles)
                {
                    file_upload_info fui = new file_upload_info();
                    strFileExt = Path.GetExtension(file.FileName);
                    string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    if (strFileExt != ".pdf" && strFileExt != ".doc" && strFileExt != ".docx" && strFileExt != ".xls" && strFileExt != ".xlsx" && strFileExt != ".csv" && strFileExt != ".txt" && strFileExt != ".jpg" && strFileExt != ".jpeg" && strFileExt != ".png" && strFileExt != ".gif")
                    {
                        strRequired = "Invalid file type, This (" + strFileExt + ")  file type is not allowed to upload.";

                    }

                    if (file.FileName.Length == 0)
                    {
                        strRequired = "Invalid file name.";
                    }

                    if (strRequired.Length > 0)
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                        lblResult2.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                        return;
                    }

                    if (strRequired.Length == 0)
                    {

                        string sFileName = originalFileName + "_" + DateTime.Now.Ticks.ToString() + strFileExt;


                        string sFilePath = System.Configuration.ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "\\VENDOR\\" + strVendorName + "\\";
                        if (Directory.Exists(sFilePath) == false)
                        {
                            Directory.CreateDirectory(sFilePath);

                        }
                        sFilePath = sFilePath + "\\" + sFileName;
                        file.SaveAs(sFilePath);

                        fui.client_id = Convert.ToInt32(hdnClientId.Value);
                        fui.CustomerId = Convert.ToInt32(hdnCustomerId.Value);
                        fui.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                        fui.Desccription = "";
                        fui.ImageName = sFileName;
                        fui.is_design = false;
                        fui.IsSiteProgress = false;
                        fui.type = 1; // Vendor Upload
                        fui.vendor_cost_id = nVendorCostID;
                        fui.dms_dirid = 0;
                        fui.dms_fileid = 0;
                        _db.file_upload_infos.InsertOnSubmit(fui);
                        _db.SubmitChanges();
                    }

                }

                dtVendor = (DataTable)Session["Vendor"];
                dtSection = (DataTable)Session["Section"];
                LoadDscription();
                Calculate();

                string strMessage = csCommonUtility.GetSystemMessage("Data saved and file uploaded successfully");
                lblResult.Text = strMessage;
                lblResult2.Text = strMessage;

            }
            else
            {
                strRequired = csCommonUtility.GetSystemErrorMessage("Please Select File");
                lblResult.Text = strRequired;
                lblResult2.Text = strRequired;
                return;
            }

            if (strRequired.Length > 0)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage(strRequired);
                lblResult2.Text = csCommonUtility.GetSystemErrorMessage(strRequired);
                return;
            }

            //Set Focus           
            int j = 0;
            int nIndx = i + 1;
            if (nIndx < grdVendorCost.Rows.Count)
            {
                j = nIndx;
                grdVendorCost.Rows[j].Cells[6].Focus();
            }
            else
                btnAddnewRow.Focus();
            //End Set Focus

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
            lblResult2.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    }

    private DataTable LoadReportTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("Date", typeof(string));
        table.Columns.Add("Vendor", typeof(string));
        table.Columns.Add("Trade", typeof(string));
        table.Columns.Add("Description", typeof(string));
        table.Columns.Add("Category", typeof(string));
        table.Columns.Add("Amount", typeof(string));

        return table;
    }

    protected void btnViewReport_Click(object sender, EventArgs e)
    {

        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnViewReport.ID, btnViewReport.GetType().Name, "Click"); 
        DataTable tmpTable = LoadReportTable();

        decimal nCommission = 0;
        decimal ncoCommission = 0;
        foreach (GridViewRow di in grdCom.Rows)
        {
            {
                TextBox txtComAmount = (TextBox)di.FindControl("txtComAmount");
                Label lblComAmount = (Label)di.FindControl("lblComAmount");
                nCommission += Convert.ToDecimal(lblComAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

            }

        }
        foreach (GridViewRow di in grdCom_CO.Rows)
        {
            {
                TextBox txtComAmount = (TextBox)di.FindControl("txtComAmount");
                Label lblComAmount = (Label)di.FindControl("lblComAmount");
                ncoCommission += Convert.ToDecimal(lblComAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

            }

        }



        DataRow drNew = tmpTable.NewRow();
        drNew["Date"] = " Vendor Cost Details";
        drNew["Vendor"] = "";
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "Customer Name:";
        drNew["Vendor"] = lblCustomerName.Text;
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "Sales Person:";
        drNew["Vendor"] = lblSalesPerson.Text;
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "Contract Amount:";
        drNew["Vendor"] = lblProjectTotal.Text;
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = " C/O  Amount:";
        drNew["Vendor"] = lblTotalCOAmount.Text;
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "Total Amount (Contract + C/O):";
        drNew["Vendor"] = lblTotalAmount.Text;
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "Total Received Amount:";
        drNew["Vendor"] = lblTotalRecive.Text;
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "Balance Due:";
        drNew["Vendor"] = lblTotalBalanceAmount.Text;
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "Contract Commission:";
        drNew["Vendor"] = nCommission.ToString("c");
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "C/O Commission:";
        drNew["Vendor"] = ncoCommission.ToString("c");
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "Total Commission:";
        drNew["Vendor"] = lblTotalCom.Text;
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "Labor Cost:";
        drNew["Vendor"] = lblLabor.Text;
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "Material Cost:";
        drNew["Vendor"] = lblMaterial.Text;
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "Material & Labor Cost:";
        drNew["Vendor"] = lblMaterialLabor.Text;
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "Total Cost:";
        drNew["Vendor"] = lblTotalCost.Text;
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "Profit/Loss:";
        drNew["Vendor"] = lblProfit.Text;
        drNew["Trade"] = "";
        drNew["Description"] = "";
        drNew["Category"] = "";
        drNew["Amount"] = "";
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        tmpTable.Rows.Add(drNew);

        drNew = tmpTable.NewRow();
        drNew["Date"] = "DATE";
        drNew["Vendor"] = "VENDOR";
        drNew["Trade"] = "TRADE";
        drNew["Description"] = "DESCRIPTION";
        drNew["Category"] = "CATEGORY";
        drNew["Amount"] = "AMOUNT";
        tmpTable.Rows.Add(drNew);

        foreach (GridViewRow di in grdVendorCost.Rows)
        {
            DropDownList ddlVendor = (DropDownList)di.FindControl("ddlVendor");
            DropDownList ddlTrade = (DropDownList)di.FindControl("ddlTrade");
            DropDownList ddlCategory = (DropDownList)di.FindControl("ddlCategory");
            TextBox txtDescription = (TextBox)di.FindControl("txtDescription");
            TextBox txtDate = (TextBox)di.FindControl("txtDate");
            TextBox txtAmount = (TextBox)di.FindControl("txtAmount");

            drNew = tmpTable.NewRow();
            drNew["Date"] = txtDate.Text;
            drNew["Vendor"] = ddlVendor.SelectedItem.Text.Replace("&", "and");
            drNew["Trade"] = ddlTrade.SelectedItem.Text;
            drNew["Description"] = txtDescription.Text;
            drNew["Category"] = ddlCategory.SelectedItem.Text;
            drNew["Amount"] = txtAmount.Text;
            tmpTable.Rows.Add(drNew);

        }

        Response.Clear();
        Response.ClearHeaders();

        using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
        {
            //string[] str1 = { "Margin Report" };
            //string[] str2 = { " " };

            //writer.WriteRecord(str1);
            //writer.WriteRecord(str2, true);
            writer.WriteAll(tmpTable, false);
        }
        Response.ContentType = "application/vnd.ms-excel";
        Response.AddHeader("Content-Disposition", "attachment; filename=VendorCost.csv");
        Response.End();

    }

    private bool IsAllowanceCheckForAllItem(int nSectionLevel, int nVendorId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        bool bFlag = false;
        string StrQ = string.Empty;
        //string StrQ = " SELECT PD.customer_id,PD.estimate_id,PD.pricing_id,PD.item_id,l.location_name, PD.section_name,PD.item_name,PD.short_notes,PD.measure_unit,PD.total_retail_price, " +
        //          " ISNULL(AD.actual_price,0) AS actual_price, PD.total_retail_price-ISNULL(AD.actual_price,0) AS price_difference,ISNULL(AD.allowance_id,0) AS allowance_id " +
        //          " FROM pricing_details PD " +
        //          " INNER JOIN location l ON l.location_id = PD.location_id " +
        //          " LEFT OUTER JOIN allowance_details AD ON  PD.customer_id = AD.customer_id AND   PD.estimate_id = AD.estimate_id AND PD.pricing_id = AD.pricing_id AND  PD.item_id = AD.item_id " +
        //          " WHERE PD.section_level  = " + nSectionLevel + " AND PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " and PD.item_name like '%Allowance%' ORDER BY PD.section_level ";

        // Babu Vai
        //string StrQ = " SELECT PD.customer_id,PD.estimate_id,PD.pricing_id,PD.item_id,l.location_name, PD.section_name,PD.item_name,PD.short_notes,PD.measure_unit,PD.total_retail_price,  " +
        //             " ISNULL(AD.actual_price,0) AS actual_price, PD.total_retail_price-ISNULL(AD.actual_price,0) AS price_difference,ISNULL(AD.allowance_id,0) AS allowance_id,ISNULL(VC.vendor_cost_id,0) AS vendor_cost_id,ISNULL(VC.vendor_id,0) AS vendor_id,ISNULL(VC.section_id,0) AS section_id,ISNULL(VC.category_id,0) AS category_id,ISNULL(VC.cost_date,GETDATE()) AS cost_date,ISNULL(VC.cost_amount,0) AS cost_amount " +
        //             " FROM pricing_details PD  " +
        //             " INNER JOIN location l ON l.location_id = PD.location_id " +
        //             " LEFT OUTER JOIN allowance_details AD ON  PD.customer_id = AD.customer_id AND   PD.estimate_id = AD.estimate_id AND PD.pricing_id = AD.pricing_id AND  PD.item_id = AD.item_id " +
        //             " LEFT OUTER JOIN vendor_cost VC ON  PD.customer_id = VC.customer_id AND   PD.estimate_id = VC.estimate_id AND PD.section_level = VC.section_id AND  PD.item_name = VC.cost_description " +
        //             " WHERE  PD.location_id IN (SELECT location_id FROM customer_locations WHERE estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + ")  " +
        //             " AND PD.section_level IN (SELECT section_id  FROM  customer_sections  WHERE  estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + ") " +
        //             " AND PD.section_level  = " + nSectionLevel + " AND PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " and PD.item_name like '%Allowance%' ORDER BY PD.section_level ";

        //SHOHIDULLAH
        if (Convert.ToInt32(hdnAllowance.Value) > 0)
        {
            StrQ = " SELECT PD.customer_id,PD.estimate_id,PD.co_pricing_list_id as pricing_id,PD.item_id,l.location_name, PD.section_name,PD.item_name,PD.short_notes,PD.measure_unit,PD.total_retail_price,  " +
                      " ISNULL(AD.actual_price,0) AS actual_price, PD.total_retail_price-ISNULL(AD.actual_price,0) AS price_difference,ISNULL(AD.allowance_id,0) AS allowance_id,ISNULL(VC.vendor_cost_id,0) AS vendor_cost_id,ISNULL(VC.vendor_id,0) AS vendor_id,ISNULL(VC.section_id,0) AS section_id,ISNULL(VC.category_id,0) AS category_id,ISNULL(VC.cost_date,GETDATE()) AS cost_date,ISNULL(VC.cost_amount,0) AS cost_amount " +
                     " FROM co_pricing_master PD  " +
                     " INNER JOIN location l ON l.location_id = PD.location_id " +
                     " LEFT OUTER JOIN allowance_details AD ON  PD.customer_id = AD.customer_id AND   PD.estimate_id = AD.estimate_id AND PD.co_pricing_list_id = AD.pricing_id AND  PD.item_id = AD.item_id " +
                     " LEFT OUTER JOIN vendor_cost VC ON  PD.customer_id = VC.customer_id AND   PD.estimate_id = VC.estimate_id AND PD.section_level = VC.section_id AND  PD.item_name = VC.cost_description " +
                     " WHERE  PD.location_id IN (SELECT location_id FROM changeorder_locations WHERE estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + ")  " +
                     " AND PD.section_level IN (SELECT section_id  FROM  changeorder_sections  WHERE  estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + ") " +
                     " AND PD.section_level  = " + nSectionLevel + " AND PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " and PD.item_status_id=1 and PD.item_name like '%Allowance%' ORDER BY PD.section_level ";
        }
        else
        {
            StrQ = " SELECT PD.customer_id,PD.estimate_id,PD.pricing_id,PD.item_id,l.location_name, PD.section_name,PD.item_name,PD.short_notes,PD.measure_unit,PD.total_retail_price,  " +
                    " ISNULL(AD.actual_price,0) AS actual_price, PD.total_retail_price-ISNULL(AD.actual_price,0) AS price_difference,ISNULL(AD.allowance_id,0) AS allowance_id,ISNULL(VC.vendor_cost_id,0) AS vendor_cost_id,ISNULL(VC.vendor_id,0) AS vendor_id,ISNULL(VC.section_id,0) AS section_id,ISNULL(VC.category_id,0) AS category_id,ISNULL(VC.cost_date,GETDATE()) AS cost_date,ISNULL(VC.cost_amount,0) AS cost_amount " +
                   " FROM pricing_details PD  " +
                   " INNER JOIN location l ON l.location_id = PD.location_id " +
                   " LEFT OUTER JOIN allowance_details AD ON  PD.customer_id = AD.customer_id AND   PD.estimate_id = AD.estimate_id AND PD.pricing_id = AD.pricing_id AND  PD.item_id = AD.item_id " +
                   " LEFT OUTER JOIN vendor_cost VC ON  PD.customer_id = VC.customer_id AND   PD.estimate_id = VC.estimate_id AND PD.section_level = VC.section_id AND  PD.item_name = VC.cost_description " +
                   " WHERE  PD.location_id IN (SELECT location_id FROM customer_locations WHERE estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + ")  " +
                   " AND PD.section_level IN (SELECT section_id  FROM  customer_sections  WHERE  estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + ") " +
                   " AND PD.section_level  = " + nSectionLevel + " AND PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " and PD.item_name like '%Allowance%' ORDER BY PD.section_level ";
        }


        DataTable dtAllowance = csCommonUtility.GetDataTable(StrQ);

        foreach (DataRow dr in dtAllowance.Rows)
        {
            allowance_detail objalwncdtl = new allowance_detail();
            int nAllowanceId = Convert.ToInt32(dr["allowance_id"]);
            if (nAllowanceId > 0)
                objalwncdtl = _db.allowance_details.SingleOrDefault(al => al.allowance_id == nAllowanceId);

            objalwncdtl.client_id = Convert.ToInt32(hdnClientId.Value);
            objalwncdtl.customer_id = Convert.ToInt32(hdnCustomerId.Value);
            objalwncdtl.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
            objalwncdtl.pricing_id = Convert.ToInt32(dr["pricing_id"]);
            objalwncdtl.item_id = Convert.ToInt32(dr["item_id"]);
            objalwncdtl.location_name = dr["location_name"].ToString();
            objalwncdtl.section_name = dr["section_name"].ToString();
            objalwncdtl.item_name = dr["item_name"].ToString();
            objalwncdtl.measure_unit = dr["measure_unit"].ToString();
            objalwncdtl.total_retail_price = Convert.ToDecimal(dr["total_retail_price"]);
            objalwncdtl.actual_price = Convert.ToDecimal(dr["actual_price"]);
            objalwncdtl.price_difference = Convert.ToDecimal(dr["price_difference"]);
            objalwncdtl.short_notes = dr["short_notes"].ToString();
            objalwncdtl.create_date = DateTime.Now;
            objalwncdtl.last_update_date = DateTime.Now;
            if (nAllowanceId == 0)
                _db.allowance_details.InsertOnSubmit(objalwncdtl);

        }

        _db.SubmitChanges();
        if (dtAllowance.Rows.Count > 0)
        {
            grdAllowanceItem.DataSource = dtAllowance;
            grdAllowanceItem.DataKeyNames = new string[] { "allowance_id", "pricing_id", "customer_id", "estimate_id", "vendor_cost_id" };
            //grdAllowanceItem.DataKeyNames = new string[] { "item_id", "labor_rate", "retail_multiplier", "is_mandatory", "section_level", "parent_id", "section_serial" };
            grdAllowanceItem.DataBind();

            hdnASectionId.Value = nSectionLevel.ToString();
            hdnAVendorId.Value = nVendorId.ToString();

            bFlag = false;


        }
        else
        {
            bFlag = true;
        }
        return bFlag;

    }
    protected void btnAddAllowanceItem_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddAllowanceItem.ID, btnAddAllowanceItem.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        foreach (GridViewRow di in grdAllowanceItem.Rows)
        {

            Label lblAmount = (Label)di.FindControl("lblAmount");
            TextBox txtAmount = (TextBox)di.FindControl("txtAmount");
            int nAllowanceId = Convert.ToInt32(grdAllowanceItem.DataKeys[di.RowIndex].Values[0].ToString());
            int nvendor_cost_id = Convert.ToInt32(grdAllowanceItem.DataKeys[di.RowIndex].Values[4].ToString());
            decimal nActualCost = 0; //Convert.ToDecimal(txtActualCost.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

            try
            {
                nActualCost = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            }
            catch
            {
            }

            decimal nCost = Convert.ToDecimal(lblAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

            decimal dPriceDifference = 0;

            dPriceDifference = nCost - nActualCost;
            allowance_detail objalwncdtl = new allowance_detail();

            objalwncdtl = _db.allowance_details.SingleOrDefault(al => al.allowance_id == nAllowanceId);

            if (objalwncdtl != null)
            {
                objalwncdtl.actual_price = nActualCost;
                objalwncdtl.price_difference = dPriceDifference;
            }
            vendor_cost ven_cost = new vendor_cost();
            if (nvendor_cost_id > 0)
                ven_cost = _db.vendor_costs.Single(l => l.vendor_cost_id == nvendor_cost_id);
            if (nvendor_cost_id == 0)
            {
                if (nActualCost != 0)
                {
                    string strDetails = System.Web.HttpUtility.HtmlDecode(di.Cells[3].Text);
                    ven_cost.vendor_cost_id = nvendor_cost_id;
                    ven_cost.vendor_id = Convert.ToInt32(hdnAVendorId.Value);
                    ven_cost.client_id = Convert.ToInt32(hdnClientId.Value);
                    ven_cost.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    ven_cost.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                    ven_cost.section_id = Convert.ToInt32(hdnASectionId.Value);
                    ven_cost.category_id = 4;
                    ven_cost.cost_description = strDetails;
                    ven_cost.cost_amount = nActualCost;
                    ven_cost.cost_date = DateTime.Today;
                    ven_cost.create_date = DateTime.Now;
                    _db.vendor_costs.InsertOnSubmit(ven_cost);
                    _db.SubmitChanges();
                }
            }



        }

        try
        {
            dtVendor = (DataTable)Session["Vendor"];
            dtSection = (DataTable)Session["Section"];
            LoadDscription();
            Calculate();
            lblResult.Text = csCommonUtility.GetSystemMessage("Allowance Items added successfully");


        }
        catch (Exception ex)
        {
            string str = ex.Message;
        }

        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HidePopUp", "HidePopUp();", true);
        ClientScript.RegisterStartupScript(this.GetType(), "HidePopUp", "HidePopUp();", true);
    }
    protected void btnNoItemClose_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNoItemClose.ID, btnNoItemClose.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        //Do Nothing

        try
        {
            dtVendor = (DataTable)Session["Vendor"];
            dtSection = (DataTable)Session["Section"];
            LoadDscription();
            Calculate();
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Allowance Item not saved, Allowance not available for this section.");


        }
        catch (Exception ex)
        {
            string str = ex.Message;
        }

        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HidePopUp", "HidePopUp();", true);
        ClientScript.RegisterStartupScript(this.GetType(), "HidePopUp", "HidePopUp();", true);
    }
    protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlCategory = (DropDownList)sender;
        if (ddlCategory.SelectedValue == "4")
        {
            GridViewRow di = (GridViewRow)ddlCategory.Parent.Parent;
            int mIndex = di.RowIndex;
            DropDownList ddlTrade = (DropDownList)di.FindControl("ddlTrade");
            DropDownList ddlVendor = (DropDownList)di.FindControl("ddlVendor");
            if (!IsAllowanceCheckForAllItem(Convert.ToInt32(ddlTrade.SelectedValue), Convert.ToInt32(ddlVendor.SelectedValue)))
            {
                tdNOAllowance.Visible = false;
                tdAllowance.Visible = true;
            }
            else
            {
                tdNOAllowance.Visible = true;
                tdAllowance.Visible = false;
            }
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", "ShowPopUp();", true);
            return;
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
    protected void ddlEstimate_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlEstimate.ID, ddlEstimate.GetType().Name, "SelectedIndexChanged"); 
        Session.Remove("vendor_cost_desc");
        lblResult.Text = "";
        lblResult2.Text = "";
        EstimateInfo(Convert.ToInt32(ddlEstimate.SelectedValue));
    }

}


