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

public partial class payment_recieved : System.Web.UI.Page
{
    public DataTable dtTerms;
    public DataTable dtCO_NAME;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                userinfo oUser = (userinfo)Session["oUser"];
                hdnEmailType.Value = oUser.EmailIntegrationType.ToString();

            }

            if (Page.User.IsInRole("admin033") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            int nEstimateId = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstimateId.ToString();
            int nCustomerId = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCustomerId.ToString();
            int nEstPayId = Convert.ToInt32(Request.QueryString.Get("epid"));
            hdnEstPaymentId.Value = nEstPayId.ToString();
            Session.Add("CustomerId", hdnCustomerId.Value);
            DataClassesDataContext _db = new DataClassesDataContext();

            // Get Customer Information
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                string strSecondName = cust.first_name2 + " " + cust.last_name2;
                if (strSecondName.Trim() == "")
                    lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                else
                    lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1 + " & " + strSecondName;

                string strAddress = "";
                strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                lblAddress.Text = strAddress;
                lblPhone.Text = cust.phone;
                lblEmail.Text = cust.email;

                hdnSalesPersonId.Value = cust.sales_person_id.ToString();

                if (_db.sales_persons.Any(c => c.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && c.sales_person_id > 0))
                {
                    sales_person sp_info = new sales_person();
                    sp_info = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
                    lblSalesPerson.Text = sp_info.first_name + " " + sp_info.last_name;
                }

                //// Get Estimate Info
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value)  && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

                    lblEstimateName.Text = cus_est.estimate_name;
                    lblJobNumber.Text = cus_est.job_number;
                    GetChangeOrders(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnEstimateId.Value));

                    if ((cus_est.job_number ?? "").Length > 0)
                    {
                        //lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                        if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                            lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                        else
                            lblTitelJobNumber.Text = " ( Job Number: " + cus_est.alter_job_number + " )";
                    }
                }
                if (Convert.ToInt32(hdnEstPaymentId.Value) > 0)
                {
                    GetPaymentInfo(Convert.ToInt32(hdnEstPaymentId.Value), Convert.ToInt32(hdnEstimateId.Value), Convert.ToInt32(hdnCustomerId.Value));
                }
                LoadTerms();
                LoadDscription();
                LoadCO();

               csCommonUtility.SetPagePermission(this.Page, new string[] { "btnAddnewRow",  "imgStatement", "btnSavePayDate", "chkPayterm" });
                csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "Edit", "Delete", "Save", "update", "grdPyement_ddlType", "grdPaymentTerm_ddlStatus" });
            }

        }
    }
    private void GetPaymentInfo(int nPaymentId, int nEstimateId, int nCustomerId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        estimate_payment objEstPay = new estimate_payment();


        objEstPay = _db.estimate_payments.Single(pay => pay.est_payment_id == nPaymentId && pay.estimate_id == nEstimateId && pay.customer_id == nCustomerId );


        txtnDeposit.Text = Convert.ToDecimal(objEstPay.deposit_amount).ToString("c");
        txtnCountertop.Text = Convert.ToDecimal(objEstPay.countertop_amount).ToString("c");
        txtnJob.Text = Convert.ToDecimal(objEstPay.start_job_amount).ToString("c");
        txtnBalance.Text = Convert.ToDecimal(objEstPay.due_completion_amount).ToString("c");
        txtnMeasure.Text = Convert.ToDecimal(objEstPay.final_measure_amount).ToString("c");
        txtnDelivery.Text = Convert.ToDecimal(objEstPay.deliver_cabinet_amount).ToString("c");
        txtnSubstantial.Text = Convert.ToDecimal(objEstPay.substantial_amount).ToString("c");
        txtnDrywall.Text = Convert.ToDecimal(objEstPay.drywall_amount).ToString("c");
        txtnFlooring.Text = Convert.ToDecimal(objEstPay.flooring_amount).ToString("c");
        txtOthers.Text = objEstPay.other_value.ToString();
        txtnOthers.Text = Convert.ToDecimal(objEstPay.other_amount).ToString("c");
        if (objEstPay.deposit_date != null)
            txtDepositDate.Text = objEstPay.deposit_date.ToString();
        if (objEstPay.countertop_date != null)
            txtCountertopDate.Text = objEstPay.countertop_date.ToString();
        if (objEstPay.startof_job_date != null)
            txtStartOfJobDate.Text = objEstPay.startof_job_date.ToString();
        if (objEstPay.due_completion_date != null)
            txtDueCompletionDate.Text = objEstPay.due_completion_date.ToString();
        if (objEstPay.measure_date != null)
            txtMeasureDate.Text = objEstPay.measure_date.ToString();
        if (objEstPay.delivery_date != null)
            txtDeliveryDate.Text = objEstPay.delivery_date.ToString();
        if (objEstPay.substantial_date != null)
            txtSubstantialDate.Text = objEstPay.substantial_date.ToString();
        if (objEstPay.drywall_date != null)
            txtStartofDrywallDate.Text = objEstPay.drywall_date.ToString();
        if (objEstPay.flooring_date != null)
            txtStartofFlooringDate.Text = objEstPay.flooring_date.ToString();
        if (objEstPay.other_date != null)
            txtOtherDate.Text = objEstPay.other_date.ToString();

        if (objEstPay.deposit_value != null)
        {
            lblDepositValue.ToolTip = objEstPay.deposit_value.Replace("^", "'").ToString();
            lblDepositValue.Text = objEstPay.deposit_value.Replace("^", "'").ToString();
        }
        if (objEstPay.countertop_value != null)
        {
            lblCountertopValue.ToolTip = objEstPay.countertop_value.Replace("^", "'").ToString();
            lblCountertopValue.Text = objEstPay.countertop_value.Replace("^", "'").ToString();
        }
        if (objEstPay.start_job_value != null)
        {
            lblStartJobValue.ToolTip = objEstPay.start_job_value.Replace("^", "'").ToString();
            lblStartJobValue.Text = objEstPay.start_job_value.Replace("^", "'").ToString();
        }
        if (objEstPay.due_completion_value != null)
        {
            lblBalanceDueValue.ToolTip = objEstPay.due_completion_value.Replace("^", "'").ToString();
            lblBalanceDueValue.Text = objEstPay.due_completion_value.Replace("^", "'").ToString();
        }
        if (objEstPay.final_measure_value != null)
        {
            lblMeasureValue.ToolTip = objEstPay.final_measure_value.Replace("^", "'").ToString();
            lblMeasureValue.Text = objEstPay.final_measure_value.Replace("^", "'").ToString();
        }
        if (objEstPay.deliver_caninet_value != null)
        {
            lblDeliveryValue.ToolTip = objEstPay.deliver_caninet_value.Replace("^", "'").ToString();
            lblDeliveryValue.Text = objEstPay.deliver_caninet_value.Replace("^", "'").ToString();
        }
        if (objEstPay.substantial_value != null)
        {
            lblSubstantialValue.ToolTip = objEstPay.substantial_value.Replace("^", "'").ToString();
            lblSubstantialValue.Text = objEstPay.substantial_value.Replace("^", "'").ToString();
        }
        if (objEstPay.drywall_value != null)
        {
            lblStartofDrywallValue.ToolTip = objEstPay.drywall_value.Replace("^", "'").ToString();
            lblStartofDrywallValue.Text = objEstPay.drywall_value.Replace("^", "'").ToString();
        }
        if (objEstPay.flooring_value != null)
        {
            lblStartofFlooringValue.ToolTip = objEstPay.flooring_value.Replace("^", "'").ToString();
            lblStartofFlooringValue.Text = objEstPay.flooring_value.Replace("^", "'").ToString();
        }
        Calculate();


    }
    private void Calculate()
    {
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        estimate_payment esp = new estimate_payment();
        if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value)).SingleOrDefault() != null)
        {
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value));
            totalwithtax = Convert.ToDecimal(esp.new_total_with_tax);
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
            //lblProjectTotal.Text = project_subtotal.ToString("c");// Without Taxes
            hdnEstPaymentId.Value = esp.est_payment_id.ToString();

        }
        decimal TotalCOAmount = 0;
        var item = from co in _db.changeorder_estimates
                   where co.customer_id == Convert.ToInt32(hdnCustomerId.Value) && co.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && co.change_order_status_id == 3
                   orderby co.changeorder_name ascending
                   select co;
        foreach (changeorder_estimate cho in item)
        {
            int ncoeid = cho.chage_order_id;
            decimal CoTaxRate = 0;
            decimal CoPrice = 0;
            decimal CoTax = 0;
            CoTaxRate = Convert.ToDecimal(cho.tax);
            decimal dEconCost = 0;
            var Coresult = (from chpl in _db.change_order_pricing_lists
                            where chpl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && chpl.customer_id == Convert.ToInt32(hdnCustomerId.Value)  && chpl.chage_order_id == ncoeid
                            select chpl.EconomicsCost);
            int cn = Coresult.Count();
            if (Coresult != null && cn > 0)
                dEconCost = Coresult.Sum();

            if (CoTaxRate > 0)
            {
                CoTax = dEconCost * (CoTaxRate / 100);
                CoPrice = dEconCost + CoTax;

            }
            else
            {
                CoPrice = dEconCost;
            }
            TotalCOAmount += CoPrice;

        }

        decimal payAmount = 0;
        var result = (from ppi in _db.New_partial_payments
                      where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) 
                      select ppi.pay_amount);
        int n = result.Count();
        if (result != null && n > 0)
            payAmount = result.Sum();
        lblTotalRecievedAmount.Text = payAmount.ToString("c");
        lblTotalCOAmount.Text = TotalCOAmount.ToString("c");

        decimal TotalCostAmount = 0;
        TotalCostAmount = totalwithtax + TotalCOAmount;
        lblTotalAmount.Text = TotalCostAmount.ToString("c");

        decimal TotalBalanceAmount = 0;
        TotalBalanceAmount = TotalCostAmount - payAmount;
        lblTotalBalanceAmount.Text = TotalBalanceAmount.ToString("c");

    }
    private void GetChangeOrders(int nCustId, int nEstId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = from co in _db.changeorder_estimates
                   where co.customer_id == nCustId && co.estimate_id == nEstId && co.change_order_status_id == 3
                   orderby co.changeorder_name ascending
                   select co;
        grdChangeOrders.DataSource = item;
        grdChangeOrders.DataBind();
    }
    protected void grdChangeOrders_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int ncoeid = Convert.ToInt32(grdChangeOrders.DataKeys[e.Row.RowIndex].Value.ToString());
            if (ncoeid > 0)
            {
                decimal CoTaxRate = 0;
                decimal CoPrice = 0;
                decimal CoTax = 0;

                changeorder_estimate cho = new changeorder_estimate();
                cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value)  && ce.chage_order_id == ncoeid);
                if (Convert.ToBoolean(cho.is_tax) == true)
                {
                    CoTaxRate = Convert.ToDecimal(cho.tax);
                }
                decimal dEconCost = 0;
                var result = (from chpl in _db.change_order_pricing_lists
                              where chpl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && chpl.customer_id == Convert.ToInt32(hdnCustomerId.Value)  && chpl.chage_order_id == ncoeid
                              select chpl.EconomicsCost);
                int n = result.Count();
                if (result != null && n > 0)
                    dEconCost = result.Sum();
                if (e.Row.Cells[6].Text.Trim() == "")
                {
                    if (CoTaxRate > 0)
                    {
                        CoTax = dEconCost * (CoTaxRate / 100);
                        CoPrice = dEconCost + CoTax;

                    }
                    else
                    {
                        CoPrice = dEconCost;
                    }
                    e.Row.Cells[7].Text = CoPrice.ToString("c");
                }

                // CO Payment Term

                string strUponSignValue = string.Empty;
                string strUponCompletionValue = string.Empty;
                string strBalanceDueValue = string.Empty;
                string strOtherValue = string.Empty;

           
                decimal dUponSignAmount = 0;
                decimal dUponCompletionAmount = 0;
                decimal dBalanceDueAmount = 0;
                decimal dOtherAmount = 0;


                if (_db.Co_PaymentTerms.Any(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == ncoeid ))
                {
                    Co_PaymentTerm objPayTerm = _db.Co_PaymentTerms.FirstOrDefault(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == ncoeid );
                    //hdnChPaymentId.Value = objPayTerm.co_payment_id.ToString();


                    if (objPayTerm.UponSign_value != null)
                    {
                        strUponSignValue = objPayTerm.UponSign_value.ToString().Replace("^", "'");
                    }
                    if (objPayTerm.UponCompletion_value != null)
                    {
                        strUponCompletionValue = objPayTerm.UponCompletion_value.ToString().Replace("^", "'");
                    }
                    if (objPayTerm.BalanceDue_value != null)
                    {
                        strBalanceDueValue = objPayTerm.BalanceDue_value.Replace("^", "'");
                    }
                    if (objPayTerm.other_value != null)
                    {
                        strOtherValue = objPayTerm.other_value.Replace("^", "'");
                    }

                    dUponSignAmount = Convert.ToDecimal(objPayTerm.UponSign_amount);

                    dUponCompletionAmount = Convert.ToDecimal(objPayTerm.UponCompletion_amount);

                    dBalanceDueAmount = Convert.ToDecimal(objPayTerm.BalanceDue_amount);

                    dOtherAmount = Convert.ToDecimal(objPayTerm.other_amount);
                   

                    string strTerms = string.Empty;

                    if (dUponSignAmount != 0)
                    {
                        strTerms = strUponSignValue + ": " + dUponSignAmount.ToString("c")+"</br>";
                    }
                    if (dUponCompletionAmount != 0)
                    {

                        strTerms += strUponCompletionValue + ": " + dUponCompletionAmount.ToString("c") + "</br>";
                    }
                    if (dBalanceDueAmount != 0)
                    {
                        strTerms += strBalanceDueValue + ": " + dBalanceDueAmount.ToString("c") + "</br>";

                    }
                    if (dOtherAmount != 0)
                    {
                        strTerms += strOtherValue + ": " + dOtherAmount.ToString("c") + "</br>";

                    }
                    e.Row.Cells[6].Text = strTerms;
                }
                else
                {

                    if (cho.payment_terms == "Other")
                        e.Row.Cells[6].Text = cho.other_terms;
                    else
                        e.Row.Cells[6].Text = cho.payment_terms;

                }
            
            }

            sales_person sp = new sales_person();
            sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
            e.Row.Cells[5].Text = sp.first_name + " " + sp.last_name;

            if (e.Row.Cells[1].Text.Trim() != "")
            {
                int nStatusId = Convert.ToInt32(e.Row.Cells[1].Text.Trim());
                if (nStatusId == 1)
                    e.Row.Cells[1].Text = "Draft";
                else if (nStatusId == 2)
                    e.Row.Cells[1].Text = "Pending";
                else if (nStatusId == 3)
                    e.Row.Cells[1].Text = "Executed";
                else if (nStatusId == 4)
                    e.Row.Cells[1].Text = "Declined";
            }
            if (e.Row.Cells[2].Text.Trim() != "")
            {
                int nChangeOrderType = Convert.ToInt32(e.Row.Cells[2].Text.Trim());
                if (nChangeOrderType == 1)
                    e.Row.Cells[2].Text = "Change Order";
                else if (nChangeOrderType == 2)
                    e.Row.Cells[2].Text = "Clarification";
                else if (nChangeOrderType == 3)
                    e.Row.Cells[2].Text = "Internal Use Only";
            }
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {

    }
    protected void btnBackToPaymentInfo_Click(object sender, EventArgs e)
    {
        Response.Redirect("payment_info.aspx?cid=" + hdnCustomerId.Value + "&eid=" + hdnEstimateId.Value);
    }
    protected void btnCustomerDetails_Click(object sender, EventArgs e)
    {
        Response.Redirect("customer_details.aspx?cid=" + hdnCustomerId.Value);
    }
    protected void btnCustomerList_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }
    protected void btnSchedule_Click(object sender, EventArgs e)
    {
        Response.Redirect("schedulecalendar.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value + "&TypeID=1");
    }
    private void LoadDscription()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpTable = LoadDataTable();

        var item = from it in _db.New_partial_payments
                   where it.customer_id == Convert.ToInt32(hdnCustomerId.Value) && it.estimate_id == Convert.ToInt32(hdnEstimateId.Value) 
                   select new PartialPayment_new()
                   {
                       payment_id = (int)it.payment_id,
                       pay_term_ids = it.pay_term_ids,
                       pay_term_desc = it.pay_term_desc,
                       pay_type_id = (int)it.pay_type_id,
                       client_id = (int)it.client_id,
                       customer_id = (int)it.customer_id,
                       estimate_id = (int)it.estimate_id,
                       reference = it.reference,
                       pay_amount = (decimal)it.pay_amount,
                       pay_date = (DateTime)it.pay_date,
                       create_date = (DateTime)it.create_date
                   };
        foreach (PartialPayment_new pp in item)
        {
            DataRow drNew = tmpTable.NewRow();
            drNew["payment_id"] = pp.payment_id;
            drNew["pay_term_ids"] = pp.pay_term_ids;
            drNew["pay_term_desc"] = pp.pay_term_desc;
            drNew["pay_type_id"] = pp.pay_type_id;
            drNew["client_id"] = pp.client_id;
            drNew["customer_id"] = pp.customer_id;
            drNew["estimate_id"] = pp.estimate_id;
            drNew["reference"] = pp.reference;
            drNew["pay_amount"] = pp.pay_amount;
            drNew["pay_date"] = pp.pay_date;
            drNew["create_date"] = pp.create_date;
            tmpTable.Rows.Add(drNew);
        }
        if (tmpTable.Rows.Count == 0)
        {
            DataRow drNew = tmpTable.NewRow();
            drNew["payment_id"] = 0;
            drNew["pay_term_ids"] = "";
            drNew["pay_term_desc"] = "";
            drNew["pay_type_id"] = 1;
            drNew["client_id"] = 1;
            drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew["reference"] = "";
            drNew["pay_amount"] = 0;
            drNew["pay_date"] = DateTime.Now;
            drNew["create_date"] = DateTime.Now;
            tmpTable.Rows.Add(drNew);
        }
        Session.Add("part_payment", tmpTable);
        grdPyement.DataSource = tmpTable;
        grdPyement.DataKeyNames = new string[] { "payment_id", "pay_term_ids", "pay_term_desc" };
        grdPyement.DataBind();

    }
    private DataTable LoadDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("payment_id", typeof(int));
        table.Columns.Add("pay_term_ids", typeof(string));
        table.Columns.Add("pay_term_desc", typeof(string));
        table.Columns.Add("pay_type_id", typeof(int));
        table.Columns.Add("client_id", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("reference", typeof(string));
        table.Columns.Add("pay_amount", typeof(decimal));
        table.Columns.Add("pay_date", typeof(DateTime));
        table.Columns.Add("create_date", typeof(DateTime));
        return table;
    }
    private void LoadCO()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpCTable = LoadCOTable();
        DataRow dr = tmpCTable.NewRow();
        dr["change_order_id"] = -1;
        dr["changeorder_name"] = "Select";
        tmpCTable.Rows.Add(dr);

        var item = from co in _db.changeorder_estimates
                   where co.customer_id == Convert.ToInt32(hdnCustomerId.Value) && co.estimate_id == Convert.ToInt32(hdnEstimateId.Value)
                   select new COName()
                   {
                       change_order_id = (int)co.chage_order_id,
                       changeorder_name = (string)co.changeorder_name
                   };
        foreach (COName cn in item)
        {

            DataRow drNew = tmpCTable.NewRow();
            drNew["change_order_id"] = cn.change_order_id;
            drNew["changeorder_name"] = cn.changeorder_name;
            tmpCTable.Rows.Add(drNew);
        }



        Session.Add("CO_NAME", tmpCTable);
        dtCO_NAME = (DataTable)Session["CO_NAME"];

    }
    private DataTable LoadCOTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("change_order_id", typeof(int));
        table.Columns.Add("changeorder_name", typeof(string));

        return table;
    }
    private void LoadTerms()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var item = from co in _db.changeorder_estimates
                   where co.customer_id == Convert.ToInt32(hdnCustomerId.Value) && co.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && co.change_order_status_id == 3
                   select new COName()
                   {
                       change_order_id = (int)co.chage_order_id,
                       changeorder_name = (string)co.changeorder_name
                   };

        DataTable tmpTTable = LoadTermTable();
        DataRow dr = tmpTTable.NewRow();
        dr = tmpTTable.NewRow();
        dr["pay_term_id"] = 1;
        dr["term_name"] = lblDepositValue.Text;
        tmpTTable.Rows.Add(dr);

        dr = tmpTTable.NewRow();
        dr["pay_term_id"] = 2;
        dr["term_name"] = lblCountertopValue.Text;
        tmpTTable.Rows.Add(dr);
        dr = tmpTTable.NewRow();

        dr["pay_term_id"] = 3;
        dr["term_name"] = lblStartJobValue.Text;
        tmpTTable.Rows.Add(dr);
        dr = tmpTTable.NewRow();

        dr["pay_term_id"] = 4;
        dr["term_name"] = lblBalanceDueValue.Text;
        tmpTTable.Rows.Add(dr);
        dr = tmpTTable.NewRow();

        dr["pay_term_id"] = 5;
        dr["term_name"] = lblMeasureValue.Text;
        tmpTTable.Rows.Add(dr);
        dr = tmpTTable.NewRow();

        dr["pay_term_id"] = 6;
        dr["term_name"] = lblDeliveryValue.Text;
        tmpTTable.Rows.Add(dr);
        dr = tmpTTable.NewRow();

        dr["pay_term_id"] = 7;
        dr["term_name"] = lblSubstantialValue.Text;
        tmpTTable.Rows.Add(dr);
        dr = tmpTTable.NewRow();

        dr["pay_term_id"] = 8;
        dr["term_name"] = lblStartofDrywallValue.Text;
        tmpTTable.Rows.Add(dr);
        dr = tmpTTable.NewRow();

        dr["pay_term_id"] = 9;
        dr["term_name"] = lblStartofFlooringValue.Text;
        tmpTTable.Rows.Add(dr);
        dr = tmpTTable.NewRow();

        dr["pay_term_id"] = 10;
        dr["term_name"] = "Other: " + txtOthers.Text;
        tmpTTable.Rows.Add(dr);
        foreach (COName cn in item)
        {
            // CO Payment Term

            string strUponSignValue = string.Empty;
            string strUponCompletionValue = string.Empty;
            string strBalanceDueValue = string.Empty;
            string strOtherValue = string.Empty;

            string strUponSignDate = string.Empty;
            string strUponCompletionDate = string.Empty;
            string strBalanceDueDate = string.Empty;
            string strOtherDate = string.Empty;

            decimal dUponSignAmount = 0;
            decimal dUponCompletionAmount = 0;
            decimal dBalanceDueAmount = 0;
            decimal dOtherAmount = 0;

            if (_db.Co_PaymentTerms.Any(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(cn.change_order_id) && est_p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])))
            {
                Co_PaymentTerm objPayTerm = _db.Co_PaymentTerms.FirstOrDefault(est_p => est_p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && est_p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && est_p.ChangeOrderId == Convert.ToInt32(cn.change_order_id) && est_p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
                //hdnChPaymentId.Value = objPayTerm.co_payment_id.ToString();


                if (objPayTerm.UponSign_value != null)
                {
                    strUponSignValue = objPayTerm.UponSign_value.ToString().Replace("^", "'");
                }
                if (objPayTerm.UponCompletion_value != null)
                {
                    strUponCompletionValue = objPayTerm.UponCompletion_value.ToString().Replace("^", "'");
                }
                if (objPayTerm.BalanceDue_value != null)
                {
                    strBalanceDueValue = objPayTerm.BalanceDue_value.Replace("^", "'");
                }
                if (objPayTerm.other_value != null)
                {
                    strOtherValue = objPayTerm.other_value.Replace("^", "'");
                }

                dUponSignAmount = Convert.ToDecimal(objPayTerm.UponSign_amount);

                dUponCompletionAmount = Convert.ToDecimal(objPayTerm.UponCompletion_amount);

                dBalanceDueAmount = Convert.ToDecimal(objPayTerm.BalanceDue_amount);

                dOtherAmount = Convert.ToDecimal(objPayTerm.other_amount);

                strUponSignDate = objPayTerm.UponSign_date;
                strUponCompletionDate = objPayTerm.UponCompletion_date;
                strBalanceDueDate = objPayTerm.BalanceDue_date;
                strOtherDate = objPayTerm.other_date;

                if (dUponSignAmount != 0)
                {
                   
                    dr = tmpTTable.NewRow();
                    dr["pay_term_id"] = 1100 + cn.change_order_id;
                    dr["term_name"] = cn.changeorder_name + " (" + strUponSignValue+": " +dUponSignAmount.ToString("c")+ ")";
                    tmpTTable.Rows.Add(dr);
 
                }
                if (dUponCompletionAmount != 0)
                {

                    dr = tmpTTable.NewRow();
                    dr["pay_term_id"] = 2100 + cn.change_order_id;
                    dr["term_name"] = cn.changeorder_name + " (" + strUponCompletionValue + ": " + dUponCompletionAmount.ToString("c") + ")";
                    tmpTTable.Rows.Add(dr);

                }
                if (dBalanceDueAmount != 0)
                {

                    dr = tmpTTable.NewRow();
                    dr["pay_term_id"] = 3100 + cn.change_order_id;
                    dr["term_name"] = cn.changeorder_name + " (" + strBalanceDueValue + ": " + dBalanceDueAmount.ToString("c") + ")";
                    tmpTTable.Rows.Add(dr);

                }
                if (dOtherAmount != 0)
                {

                    dr = tmpTTable.NewRow();
                    dr["pay_term_id"] = 4100 + cn.change_order_id;
                    dr["term_name"] = cn.changeorder_name + " (" + strOtherValue + ": " + dOtherAmount.ToString("c") + ")";
                    tmpTTable.Rows.Add(dr);

                }

            }
            else
            {

                dr = tmpTTable.NewRow();
                dr["pay_term_id"] = 100 + cn.change_order_id;
                dr["term_name"] = cn.changeorder_name;
                tmpTTable.Rows.Add(dr);
            }
        }

        Session.Add("Terms", tmpTTable);
        dtTerms = (DataTable)Session["Terms"];

    }
    private DataTable LoadTermTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("pay_term_id", typeof(int));
        table.Columns.Add("term_name", typeof(string));

        return table;
    }
    protected void grdPyement_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable table = (DataTable)Session["part_payment"];
            dtTerms = (DataTable)Session["Terms"];

            foreach (GridViewRow di in grdPyement.Rows)
            {
                {
                    CheckBoxList chkPayterm = (CheckBoxList)di.FindControl("chkPayterm");
                    DropDownList ddlType = (DropDownList)di.FindControl("ddlType");
                    TextBox txtReference = (TextBox)di.FindControl("txtReference");
                    TextBox txtDate = (TextBox)di.FindControl("txtDate");
                    TextBox txtAmount = (TextBox)di.FindControl("txtAmount");

                    Label lblReference = (Label)di.FindControl("lblReference");
                    Label lblDate = (Label)di.FindControl("lblDate");
                    Label lblAmount = (Label)di.FindControl("lblAmount");
                    DataRow dr = table.Rows[di.RowIndex];

                    dr["payment_id"] = Convert.ToInt32(grdPyement.DataKeys[di.RowIndex].Values[0]);
                    string strPayTermId = "";
                    string strPayTermDesc = "";

                    for (int i = 0; i < chkPayterm.Items.Count; i++)
                    {
                        if (chkPayterm.Items[i].Selected == true)
                        {
                            if (strPayTermId.Trim().Length > 0)
                            {
                                strPayTermId += "," + Convert.ToInt32(chkPayterm.Items[i].Value).ToString();
                            }
                            else
                            {
                                strPayTermId = Convert.ToInt32(chkPayterm.Items[i].Value).ToString();
                            }
                            if (strPayTermDesc.Trim().Length > 0)
                            {
                                strPayTermDesc += ", " + chkPayterm.Items[i].Text.ToString();
                            }
                            else
                            {
                                strPayTermDesc = chkPayterm.Items[i].Text.ToString();
                            }
                        }
                    }

                    dr["pay_term_ids"] = strPayTermId;
                    dr["pay_term_desc"] = strPayTermDesc;
                    dr["client_id"] = 1;
                    dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                    dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                    dr["reference"] = txtReference.Text;
                    dr["pay_type_id"] = Convert.ToInt32(ddlType.SelectedValue);
                    dr["pay_amount"] = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    dr["pay_date"] = Convert.ToDateTime(txtDate.Text);
                    dr["create_date"] = DateTime.Now;

                }

            }
            foreach (DataRow dr in table.Rows)
            {
                New_partial_payment pay_cost = new New_partial_payment();
                if (Convert.ToInt32(dr["payment_id"]) > 0)
                    pay_cost = _db.New_partial_payments.Single(l => l.payment_id == Convert.ToInt32(dr["payment_id"]));

                if (Convert.ToDecimal(dr["pay_amount"]) != 0)
                {
                    pay_cost.payment_id = Convert.ToInt32(dr["payment_id"]);

                    pay_cost.pay_term_ids = dr["pay_term_ids"].ToString();
                    pay_cost.pay_term_desc = dr["pay_term_desc"].ToString();
                    pay_cost.client_id = Convert.ToInt32(dr["client_id"]);
                    pay_cost.customer_id = Convert.ToInt32(dr["customer_id"]);
                    pay_cost.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                    pay_cost.pay_type_id = Convert.ToInt32(dr["pay_type_id"]);
                    pay_cost.reference = dr["reference"].ToString();
                    pay_cost.pay_amount = Convert.ToDecimal(dr["pay_amount"]);
                    pay_cost.pay_date = Convert.ToDateTime(dr["pay_date"]);
                    pay_cost.create_date = Convert.ToDateTime(dr["create_date"]);
                    if (Convert.ToInt32(dr["payment_id"]) == 0)
                    {
                        _db.New_partial_payments.InsertOnSubmit(pay_cost);
                    }


                }

            }

            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");

            _db.SubmitChanges();
            dtTerms = (DataTable)Session["Terms"];
            LoadDscription();
            Calculate();

        }
    }

    protected void grdPyement_RowEditing(object sender, GridViewEditEventArgs e)
    {

        HtmlControl div = (HtmlControl)grdPyement.Rows[e.NewEditIndex].FindControl("dvCalender");
        CheckBoxList chkPayterm = (CheckBoxList)grdPyement.Rows[e.NewEditIndex].FindControl("chkPayterm");
        DropDownList ddlType = (DropDownList)grdPyement.Rows[e.NewEditIndex].FindControl("ddlType");
        TextBox txtReference = (TextBox)grdPyement.Rows[e.NewEditIndex].FindControl("txtReference");
        TextBox txtDate = (TextBox)grdPyement.Rows[e.NewEditIndex].FindControl("txtDate");
        TextBox txtAmount = (TextBox)grdPyement.Rows[e.NewEditIndex].FindControl("txtAmount");

        Label lblReference = (Label)grdPyement.Rows[e.NewEditIndex].FindControl("lblReference");
        Label lblDate = (Label)grdPyement.Rows[e.NewEditIndex].FindControl("lblDate");
        Label lblAmount = (Label)grdPyement.Rows[e.NewEditIndex].FindControl("lblAmount");
        Label lblPayTerms = (Label)grdPyement.Rows[e.NewEditIndex].FindControl("lblPayTerms");

        chkPayterm.Visible = true;
        lblPayTerms.Visible = false;

        txtReference.Visible = true;
        lblReference.Visible = false;
        // txtDate.Visible = true;
        div.Visible = true;
        lblDate.Visible = false;
        txtAmount.Visible = true;
        lblAmount.Visible = false;

        // chkPayterm.Enabled = true;
        ddlType.Enabled = true;

        LinkButton btn = (LinkButton)grdPyement.Rows[e.NewEditIndex].Cells[5].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdPyement_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        CheckBoxList chkPayterm = (CheckBoxList)grdPyement.Rows[e.RowIndex].FindControl("chkPayterm");

        DropDownList ddlType = (DropDownList)grdPyement.Rows[e.RowIndex].FindControl("ddlType");
        TextBox txtReference = (TextBox)grdPyement.Rows[e.RowIndex].FindControl("txtReference");
        TextBox txtDate = (TextBox)grdPyement.Rows[e.RowIndex].FindControl("txtDate");
        TextBox txtAmount = (TextBox)grdPyement.Rows[e.RowIndex].FindControl("txtAmount");

        Label lblReference = (Label)grdPyement.Rows[e.RowIndex].FindControl("lblReference");
        Label lblDate = (Label)grdPyement.Rows[e.RowIndex].FindControl("lblDate");
        Label lblAmount = (Label)grdPyement.Rows[e.RowIndex].FindControl("lblAmount");
        Label lblPayTerms = (Label)grdPyement.Rows[e.RowIndex].FindControl("lblPayTerms");

        int nvendor_cost_id = Convert.ToInt32(grdPyement.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
        string strPayTermId = "";
        string strPayTermDesc = "";

        for (int i = 0; i < chkPayterm.Items.Count; i++)
        {
            if (chkPayterm.Items[i].Selected == true)
            {
                if (strPayTermId.Trim().Length > 0)
                {
                    strPayTermId += "," + Convert.ToInt32(chkPayterm.Items[i].Value).ToString();
                }
                else
                {
                    strPayTermId = Convert.ToInt32(chkPayterm.Items[i].Value).ToString();
                }
                if (strPayTermDesc.Trim().Length > 0)
                {
                    strPayTermDesc += ", " + chkPayterm.Items[i].Text.ToString();
                }
                else
                {
                    strPayTermDesc = chkPayterm.Items[i].Text.ToString();
                }
            }
        }
        string strQ = "UPDATE New_partial_payment SET  pay_term_ids='" + strPayTermId.Replace("'", "''") + "', pay_term_desc='" + strPayTermDesc.Replace("'", "''") + "' ,reference='" + txtReference.Text.Replace("'", "''") + "', pay_type_id ="+ Convert.ToInt32(ddlType.SelectedValue)+" ,pay_date='" + Convert.ToDateTime(txtDate.Text.Replace("'", "''")) + "' ,pay_amount=" + Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) + "  WHERE payment_id =" + nvendor_cost_id + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        dtTerms = (DataTable)Session["Terms"];
        LoadDscription();
        Calculate();
        lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");


    }

    protected void grdPyement_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HtmlControl div = (HtmlControl)e.Row.FindControl("dvCalender");

            dtTerms = (DataTable)Session["Terms"];
            CheckBoxList chkPayterm = (CheckBoxList)e.Row.FindControl("chkPayterm");
            DropDownList ddlType = (DropDownList)e.Row.FindControl("ddlType");
            TextBox txtReference = (TextBox)e.Row.FindControl("txtReference");
            TextBox txtDate = (TextBox)e.Row.FindControl("txtDate");
            TextBox txtAmount = (TextBox)e.Row.FindControl("txtAmount");

            Label lblPayTerms = (Label)e.Row.FindControl("lblPayTerms");
            Label lblReference = (Label)e.Row.FindControl("lblReference");
            Label lblDate = (Label)e.Row.FindControl("lblDate");
            Label lblAmount = (Label)e.Row.FindControl("lblAmount");
            string strPayTermsId = grdPyement.DataKeys[e.Row.RowIndex].Values[1].ToString();
            string strPayTerms = grdPyement.DataKeys[e.Row.RowIndex].Values[2].ToString();
            if (strPayTermsId.Trim().Length > 0)
            {
                lblPayTerms.Text = strPayTerms;
                chkPayterm.Visible = false;
                lblPayTerms.Visible = true;
            }
            else
            {
                lblPayTerms.Visible = false;
                chkPayterm.Visible = true;
            }
            if (strPayTermsId.Trim().Length > 0)
            {
                string[] strId = strPayTermsId.Split(',');

                //	bool bFlag = false;
                foreach (string str in strId)
                {
                    for (int i = 0; i < chkPayterm.Items.Count; i++)
                    {
                        if (Convert.ToInt32(chkPayterm.Items[i].Value) == Convert.ToInt32(str))
                        {
                            chkPayterm.Items[i].Selected = true;
                        }
                    }
                }
            }
            if (Convert.ToDecimal(lblAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) == 0)
            {
                txtReference.Visible = true;
                lblReference.Visible = false;
                //txtDate.Visible = true;
                div.Visible = true;
                lblDate.Visible = false;
                txtAmount.Visible = true;
                lblAmount.Visible = false;
                chkPayterm.Enabled = true;

                ddlType.Enabled = true;
                LinkButton btn = (LinkButton)e.Row.Cells[5].Controls[0];
                btn.Text = "Save";
                btn.CommandName = "Save";

            }


        }

    }
    protected void btnAddnewRow_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddnewRow.ID, btnAddnewRow.GetType().Name, "Click"); 
        DataTable table = (DataTable)Session["part_payment"];
        dtTerms = (DataTable)Session["Terms"];
        for (int i = 0; i < table.Rows.Count; i++)
        {
            if (Convert.ToInt32(table.Rows[i]["payment_id"]) == 0)
            {
                return;
            }
        }
        foreach (GridViewRow di in grdPyement.Rows)
        {
            {
                CheckBoxList chkPayterm = (CheckBoxList)di.FindControl("chkPayterm");
                DropDownList ddlType = (DropDownList)di.FindControl("ddlType");
                TextBox txtReference = (TextBox)di.FindControl("txtReference");
                TextBox txtDate = (TextBox)di.FindControl("txtDate");
                TextBox txtAmount = (TextBox)di.FindControl("txtAmount");

                Label lblReference = (Label)di.FindControl("lblReference");
                Label lblDate = (Label)di.FindControl("lblDate");
                Label lblAmount = (Label)di.FindControl("lblAmount");

                DataRow dr = table.Rows[di.RowIndex];
                dr["payment_id"] = Convert.ToInt32(grdPyement.DataKeys[di.RowIndex].Values[0]);
                string strPayTermId = "";
                string strPayTermDesc = "";

                for (int i = 0; i < chkPayterm.Items.Count; i++)
                {
                    if (chkPayterm.Items[i].Selected == true)
                    {
                        if (strPayTermId.Trim().Length > 0)
                        {
                            strPayTermId += "," + Convert.ToInt32(chkPayterm.Items[i].Value).ToString();
                        }
                        else
                        {
                            strPayTermId = Convert.ToInt32(chkPayterm.Items[i].Value).ToString();
                        }
                        if (strPayTermDesc.Trim().Length > 0)
                        {
                            strPayTermDesc += ", " + chkPayterm.Items[i].Text.ToString();
                        }
                        else
                        {
                            strPayTermDesc = chkPayterm.Items[i].Text.ToString();
                        }
                    }
                }

                dr["pay_term_ids"] = strPayTermId;
                dr["pay_term_desc"] = strPayTermDesc;
                dr["client_id"] = 1;
                dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                dr["reference"] = txtReference.Text;
                dr["pay_type_id"] = Convert.ToInt32(ddlType.SelectedValue);
                dr["pay_amount"] = Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                dr["pay_date"] = Convert.ToDateTime(txtDate.Text);
                dr["create_date"] = DateTime.Now;

            }

        }
        DataRow drNew = table.NewRow();
        drNew["payment_id"] = 0;
        drNew["pay_term_ids"] = "";
        drNew["pay_term_desc"] = "";
        drNew["client_id"] = 1;
        drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
        drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
        drNew["reference"] = "";
        drNew["pay_type_id"] = 1;
        drNew["pay_amount"] = 0;
        drNew["pay_date"] = DateTime.Now;
        drNew["create_date"] = DateTime.Now;
        table.Rows.Add(drNew);

        Session.Add("part_payment", table);
        grdPyement.DataSource = table;
        grdPyement.DataKeyNames = new string[] { "payment_id", "pay_term_ids", "pay_term_desc" };
        grdPyement.DataBind();
        lblResult.Text = "";

    }

    string CreateHtml()
    {
        string TABLE = "";
        string strName = "";
        TABLE = "<br/> <br/> <br/> <br/> <br/> <br/>";
        TABLE += "<table cellpadding='0' cellspacing='2' width='100%' style='margin: 0 auto;font-family:Arial, Helvetica, sans-serif;'>" +
             "<tr style='color:#555; font-size:14px; line-height:24px;'><td align='left'>To make your payment online, please click <a target='_blank' href='https://ii.faztrack.com/customerlogin.aspx'> here </a></td></tr>" +
               "<tr style='color:#555; font-size:14px; line-height:24px;'><td align='left'><b>Customer Name: </b>" + lblCustomerName.Text + "</td></tr>" +
               "<tr style='color:#555; font-size:14px; line-height:24px;'><td align='left'><b>Estimate: </b>" + lblEstimateName.Text + "</td></tr>" +
               "<tr style='color:#555; font-size:18px; line-height:24px;'><td align='left'><b>Payment Schedule & Terms</b><br/></td></tr>" +
                "<tr><td align='center' width='10px'></td></tr>" +
                "<tr><td align='center' width='98%'>" +
                    "<table style='border:1px solid #c1c1c1;' cellpadding='5' cellspacing='0' width='60%'>" +
                        "<tr style='color: #021bcf; font-size: 14px; font-weight: bold; text-align:center; background-color:#f1f1f1; height:32px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Schedule Date</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Payment Term</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Amount</td></tr>";
        if (Convert.ToDecimal(txtnDeposit.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) > 0)
            TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 14px; font-weight: normal;text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtDepositDate.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblDepositValue.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtnDeposit.Text + "</td></tr>";
        if (Convert.ToDecimal(txtnCountertop.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) > 0)
            TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 14px; font-weight: normal;text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtCountertopDate.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblCountertopValue.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtnCountertop.Text + "</td></tr>";
        if (Convert.ToDecimal(txtnJob.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) > 0)
            TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 14px; font-weight: normal;text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtStartOfJobDate.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblStartJobValue.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtnJob.Text + "</td></tr>";
        if (Convert.ToDecimal(txtnFlooring.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) > 0)
            TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 14px; font-weight: normal;text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtStartofFlooringDate.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblStartofFlooringValue.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtnFlooring.Text + "</td></tr>";

        if (Convert.ToDecimal(txtnDrywall.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) > 0)
            TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 14px; font-weight: normal;text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtStartofDrywallDate.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblStartofDrywallValue.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtnDrywall.Text + "</td></tr>";
        if (Convert.ToDecimal(txtnMeasure.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) > 0)
            TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 14px; font-weight: normal;text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtMeasureDate.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblMeasureValue.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtnMeasure.Text + "</td></tr>";
        if (Convert.ToDecimal(txtnDelivery.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) > 0)
            TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 14px; font-weight: normal;text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtDeliveryDate.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblDeliveryValue.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtnDelivery.Text + "</td></tr>";
        if (Convert.ToDecimal(txtnSubstantial.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) > 0)
            TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 14px; font-weight: normal;text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtSubstantialDate.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblSubstantialValue.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtnSubstantial.Text + "</td></tr>";
        if (Convert.ToDecimal(txtnOthers.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) > 0)
            TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 14px; font-weight: normal;text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtOtherDate.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>Other:" + txtOthers.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtnOthers.Text + "</td></tr>";
        if (Convert.ToDecimal(txtnBalance.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) > 0)
            TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 14px; font-weight: normal;text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtDueCompletionDate.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblBalanceDueValue.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + txtnBalance.Text + "</td></tr>";

        TABLE += "</table>" +
            // Invoice Items
                "<tr><td align='center' width='98%'>" +
                    "<table style='border:1px solid #c1c1c1;' cellpadding='5' cellspacing='0' width='100%'>" +
                      "<tr style='color:#555; font-size:18px; line-height:24px;'><td align='left'><b>Executed Change Order</b><br/></td></tr>" +
                        "<tr style='color: #021bcf; font-size: 14px; font-weight: bold; text-align:center; background-color:#f1f1f1; height:32px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>C/O Title</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Status</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Type</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Change Order Date</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Updated Date</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Updated By</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Payment Term</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Amount</td></tr>";
        if (grdChangeOrders.Rows.Count > 0)
        {
            foreach (GridViewRow di in grdChangeOrders.Rows)
            {

                TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 14px; font-weight: normal; text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + di.Cells[0].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + di.Cells[1].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + di.Cells[2].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + di.Cells[3].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='right'>" + di.Cells[4].Text + "&nbsp;" + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + di.Cells[5].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + di.Cells[6].Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + di.Cells[7].Text + "</td></tr>";

            }
        }
        TABLE += "</table>" +
          "<tr><td align='center' width='98%'>" +
                     "<table style='border:1px solid #c1c1c1;' cellpadding='5' cellspacing='0' width='100%'>" +
                      "<tr style='color:#555; font-size:18px; line-height:24px;'><td align='left'><b>Payment Received</b><br/></td></tr>" +
                         "<tr style='color: #021bcf; font-size: 14px; font-weight: bold; text-align:center; background-color:#f1f1f1; height:32px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Date</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Payment Term</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Payment Type</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Reference</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>Amount</td></tr>";
        if (grdPyement.Rows.Count > 0)
        {
            foreach (GridViewRow di in grdPyement.Rows)
            {
                CheckBoxList chkPayterm = (CheckBoxList)di.FindControl("chkPayterm");
                DropDownList ddlType = (DropDownList)di.FindControl("ddlType");
                TextBox txtReference = (TextBox)di.FindControl("txtReference");
                TextBox txtDate = (TextBox)di.FindControl("txtDate");
                TextBox txtAmount = (TextBox)di.FindControl("txtAmount");

                Label lblReference = (Label)di.FindControl("lblReference");
                Label lblDate = (Label)di.FindControl("lblDate");
                Label lblAmount = (Label)di.FindControl("lblAmount");
                Label lblPayTerms = (Label)di.FindControl("lblPayTerms");
                TABLE += "<tr style='border: 1px solid #c1c1c1; color: #222; font-size: 14px; font-weight: normal; text-align:center; background-color:#fff; height:40px;'><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblDate.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='left'>" + lblPayTerms.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + ddlType.SelectedItem.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='center'>" + lblReference.Text + "</td><td style='border-right:1px solid #c1c1c1;border-bottom:1px solid #c1c1c1;' align='right'>" + lblAmount.Text + "&nbsp;" + "</td></tr>";

            }
        }


        TABLE += "</table>" +
         "</td></tr>" +
         "<tr><td align='center' width='10px'></td></tr>" +
            // Footer Items
             "<tr><td align='center' width='98%'>" +
                 "<table style='background-color: #F5F5F5;border: 1px solid #DDDDDD; box-shadow: 0 0 2px #EEEEEE; margin: 0 auto; font-size:14px; color: #555;' cellpadding='5' cellspacing='0' width='100%'>" +
             "<tr><td align='center' width='10px'></td></tr>" +
             "<tr style='color:#555; font-size:14px; line-height:24px;'><td align='left'><b>Contract Amount: </b>" + lblProjectTotal.Text + "</td></tr>" +
             "<tr style='color:#555; font-size:14px; line-height:24px;'><td align='left'><b>C/O Amount: </b>" + lblTotalCOAmount.Text + "</td></tr>" +
             "<tr style='color:#555; font-size:14px; line-height:24px;'><td align='left'><b>Total Amount (Contract + C/O ): </b>" + lblTotalAmount.Text + "</td></tr>" +
             "<tr style='color:#555; font-size:14px; line-height:24px;'><td align='left'><b>Total Received Amount: </b>" + lblTotalRecievedAmount.Text + "</td></tr>" +
             "<tr style='color:#555; font-size:14px; line-height:24px;'><td align='left'><b>Balance Due: </b>" + lblTotalBalanceAmount.Text + "</td></tr>" +
     "</table>";

        return TABLE;
    }

    protected void imgStatement_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgStatement.ID, imgStatement.GetType().Name, "Click"); 
        string strMessage = CreateHtml();
        Session.Add("MessBody", strMessage);
        //string url = "statement_email.aspx?custId=" + hdnCustomerId.Value + "&eid=" + hdnEstimateId.Value;
        //string Script = @"<script language=JavaScript>window.open('" + url + "'); opener.document.forms[0].submit(); </script>";
        //if (!IsClientScriptBlockRegistered("OpenFile"))
        //    this.RegisterClientScriptBlock("OpenFile", Script);

        ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", "DisplayEmailWindow()", true);
    }
    //protected void imgStatement_Click(object sender, ImageClickEventArgs e)
    //{
    //    string strMessage = CreateHtml();
    //    Session.Add("MessBody", strMessage);
    //    string url = "statement_email.aspx?custId=" + hdnCustomerId.Value + "&eid=" + hdnEstimateId.Value;
    //    string Script = @"<script language=JavaScript>window.open('" + url + "'); opener.document.forms[0].submit(); </script>";
    //    if (!IsClientScriptBlockRegistered("OpenFile"))
    //        this.RegisterClientScriptBlock("OpenFile", Script);

    //}
    protected void btnSavePayDate_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSavePayDate.ID, btnSavePayDate.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        try
        {
            if (Convert.ToInt32(hdnEstPaymentId.Value) > 0)
            {
                estimate_payment obj = _db.estimate_payments.Single(ep => ep.est_payment_id == Convert.ToInt32(hdnEstPaymentId.Value));
                if (obj != null)
                {
                    obj.deposit_date = txtDepositDate.Text;
                    obj.countertop_date = txtCountertopDate.Text;
                    obj.startof_job_date = txtStartOfJobDate.Text;
                    obj.due_completion_date = txtDueCompletionDate.Text;
                    obj.measure_date = txtMeasureDate.Text;
                    obj.delivery_date = txtDeliveryDate.Text;
                    obj.substantial_date = txtSubstantialDate.Text;
                    obj.drywall_date = txtStartofDrywallDate.Text;
                    obj.flooring_date = txtStartofFlooringDate.Text;
                    obj.other_date = txtOtherDate.Text;

                    if (Convert.ToInt32(hdnEstPaymentId.Value) > 0)
                    {
                        obj.updated_date = DateTime.Now;
                        _db.SubmitChanges();

                        lblResultMess.Text = "Data updated successfully.";
                        lblResultMess.ForeColor = System.Drawing.Color.Green;

                    }
                }
            }
        }
        catch
        {

        }
    }
    protected void grdPyement_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        int nvendor_cost_id = Convert.ToInt32(grdPyement.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);

        string strQ = "Delete New_partial_payment WHERE payment_id =" + nvendor_cost_id + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);
        _db.ExecuteCommand(strQ, string.Empty);

        LoadDscription();
        Calculate();
        lblResult.Text = csCommonUtility.GetSystemMessage("Item deleted successfully");

    }

}
