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

public partial class payment_withco : System.Web.UI.Page
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

                // Get Estimate Info
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

                    lblEstimateName.Text = cus_est.estimate_name;
                    GetChangeOrders(Convert.ToInt32(hdnCustomerId.Value), Convert.ToInt32(hdnEstimateId.Value));
                }
                if (Convert.ToInt32(hdnEstPaymentId.Value) > 0)
                {
                    GetPaymentInfo(Convert.ToInt32(hdnEstPaymentId.Value), Convert.ToInt32(hdnEstimateId.Value), Convert.ToInt32(hdnCustomerId.Value));
                }
                LoadTerms();
                LoadDscription();
                LoadCO();
                LoadCODscription();
                
               
            }
            
        }
    }
    private void GetPaymentInfo(int nPaymentId, int nEstimateId, int nCustomerId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        estimate_payment objEstPay = new estimate_payment();


        objEstPay = _db.estimate_payments.Single(pay => pay.est_payment_id == nPaymentId && pay.estimate_id == nEstimateId && pay.customer_id == nCustomerId && pay.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));


        txtnDeposit.Text = Convert.ToDecimal(objEstPay.deposit_amount).ToString("c");
        txtnCountertop.Text = Convert.ToDecimal(objEstPay.countertop_amount).ToString("c");
        txtnJob.Text = Convert.ToDecimal(objEstPay.start_job_amount).ToString("c");
        txtnBalance.Text = Convert.ToDecimal(objEstPay.due_completion_amount).ToString("c");
        txtnMeasure.Text = Convert.ToDecimal(objEstPay.final_measure_amount).ToString("c");
        txtnDelivery.Text = Convert.ToDecimal(objEstPay.deliver_cabinet_amount).ToString("c");
        txtnSubstantial.Text = Convert.ToDecimal(objEstPay.substantial_amount).ToString("c");
        txtOthers.Text = objEstPay.other_value.ToString();
        txtnOthers.Text = Convert.ToDecimal(objEstPay.other_amount).ToString("c");
        if (objEstPay.deposit_date != null)
            lblDepositDate.Text = objEstPay.deposit_date.ToString();
        if (objEstPay.countertop_date != null)
            lblCountertopDate.Text = objEstPay.countertop_date.ToString();
        if (objEstPay.startof_job_date != null)
            lblStartJobDate.Text = objEstPay.startof_job_date.ToString();
        if (objEstPay.due_completion_date != null)
            lblBalanceDueDate.Text = objEstPay.due_completion_date.ToString();
        if (objEstPay.measure_date != null)
            lblMeasureDate.Text = objEstPay.measure_date.ToString();
        if (objEstPay.delivery_date != null)
            lblDeliveryDate.Text = objEstPay.delivery_date.ToString();
        if (objEstPay.substantial_date != null)
            lblSubstantialDate.Text = objEstPay.substantial_date.ToString();
        if (objEstPay.other_date != null)
            lblOtherDate.Text = objEstPay.other_date.ToString();

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

    }
    private void GetChangeOrders(int nCustId, int nEstId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
       
        var item = from co in _db.changeorder_estimates
                   where co.customer_id == nCustId && co.estimate_id == nEstId 
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
                cho = _db.changeorder_estimates.Single(ce => ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.chage_order_id == ncoeid);
                if (Convert.ToBoolean(cho.is_tax)==true)
                {
                    CoTaxRate = Convert.ToDecimal(cho.tax);
                }
                decimal dEconCost = 0;
                var result = (from chpl in _db.change_order_pricing_lists
                              where chpl.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && chpl.customer_id == Convert.ToInt32(hdnCustomerId.Value) && chpl.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && chpl.chage_order_id==ncoeid
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
                if (cho.payment_terms == "Other")
                    e.Row.Cells[6].Text = cho.other_terms;
                else
                    e.Row.Cells[6].Text = cho.payment_terms;
               
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
    private void LoadDscription()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpTable = LoadDataTable();

        var item = from it in _db.partial_payment_infos
                   where it.customer_id == Convert.ToInt32(hdnCustomerId.Value) && it.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && it.client_id == 1
                   select new PartialPayment()
                   {
                       payment_id = (int)it.payment_id,
                       pay_term_id = (int)it.pay_term_id,
                       pay_type_id = (int)it.pay_type_id,
                       client_id = (int)it.client_id,
                       customer_id = (int)it.customer_id,
                       estimate_id = (int)it.estimate_id,
                       reference = it.reference,
                       pay_amount = (decimal)it.pay_amount,
                       pay_date = (DateTime)it.pay_date,
                       create_date = (DateTime)it.create_date
                   };
        foreach (PartialPayment pp in item)
        {
            DataRow drNew = tmpTable.NewRow();
            drNew["payment_id"] = pp.payment_id;
            drNew["pay_term_id"] = pp.pay_term_id;
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
            drNew["pay_term_id"] = -1;
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
        grdPyement.DataKeyNames = new string[] { "payment_id" };
        grdPyement.DataBind();

    }
    private DataTable LoadDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("payment_id", typeof(int));
        table.Columns.Add("pay_term_id", typeof(int));
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
    private void LoadTerms()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpTTable = LoadTermTable();
        DataRow dr = tmpTTable.NewRow();
        dr["pay_term_id"] = -1;
        dr["term_name"] = "Select";
        tmpTTable.Rows.Add(dr);

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
        dr["term_name"] ="Other: "+ txtOthers.Text;
        tmpTTable.Rows.Add(dr);

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
                    DropDownList ddlPayTerm = (DropDownList)di.FindControl("ddlPayTerm");
                    DropDownList ddlType = (DropDownList)di.FindControl("ddlType");
                    TextBox txtReference = (TextBox)di.FindControl("txtReference");
                    TextBox txtDate = (TextBox)di.FindControl("txtDate");
                    TextBox txtAmount = (TextBox)di.FindControl("txtAmount");

                    Label lblReference = (Label)di.FindControl("lblReference");
                    Label lblDate = (Label)di.FindControl("lblDate");
                    Label lblAmount = (Label)di.FindControl("lblAmount");
                    DataRow dr = table.Rows[di.RowIndex];

                    dr["payment_id"] = Convert.ToInt32(grdPyement.DataKeys[di.RowIndex].Values[0]);
                    dr["pay_term_id"] = Convert.ToInt32(ddlPayTerm.SelectedValue);
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
                partial_payment_info pay_cost = new partial_payment_info();
                if (Convert.ToInt32(dr["payment_id"]) > 0)
                    pay_cost = _db.partial_payment_infos.Single(l => l.payment_id == Convert.ToInt32(dr["payment_id"]));
               
                if (Convert.ToDecimal(dr["pay_amount"]) != 0)
                {
                    pay_cost.payment_id = Convert.ToInt32(dr["payment_id"]);
                    pay_cost.pay_term_id = Convert.ToInt32(dr["pay_term_id"]);
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
                    _db.partial_payment_infos.InsertOnSubmit(pay_cost);
                }


                }
                
            }

            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            
            _db.SubmitChanges();
            dtTerms = (DataTable)Session["Terms"];
            LoadDscription();

        }
    }

    protected void grdPyement_RowEditing(object sender, GridViewEditEventArgs e)
    {

        HtmlControl div = (HtmlControl)grdPyement.Rows[e.NewEditIndex].FindControl("dvCalender");
        DropDownList ddlPayTerm = (DropDownList)grdPyement.Rows[e.NewEditIndex].FindControl("ddlPayTerm");
        DropDownList ddlType = (DropDownList)grdPyement.Rows[e.NewEditIndex].FindControl("ddlType");
        TextBox txtReference = (TextBox)grdPyement.Rows[e.NewEditIndex].FindControl("txtReference");
        TextBox txtDate = (TextBox)grdPyement.Rows[e.NewEditIndex].FindControl("txtDate");
        TextBox txtAmount = (TextBox)grdPyement.Rows[e.NewEditIndex].FindControl("txtAmount");

        Label lblReference = (Label)grdPyement.Rows[e.NewEditIndex].FindControl("lblReference");
        Label lblDate = (Label)grdPyement.Rows[e.NewEditIndex].FindControl("lblDate");
        Label lblAmount = (Label)grdPyement.Rows[e.NewEditIndex].FindControl("lblAmount");

        txtReference.Visible = true;
        lblReference.Visible = false;
        // txtDate.Visible = true;
        div.Visible = true;
        lblDate.Visible = false;
        txtAmount.Visible = true;
        lblAmount.Visible = false;

        ddlPayTerm.Enabled = true;
        ddlType.Enabled = true;

        LinkButton btn = (LinkButton)grdPyement.Rows[e.NewEditIndex].Cells[5].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdPyement_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        DropDownList ddlPayTerm = (DropDownList)grdPyement.Rows[e.RowIndex].FindControl("ddlPayTerm");
        
        DropDownList ddlType = (DropDownList)grdPyement.Rows[e.RowIndex].FindControl("ddlType");
        TextBox txtReference = (TextBox)grdPyement.Rows[e.RowIndex].FindControl("txtReference");
        TextBox txtDate = (TextBox)grdPyement.Rows[e.RowIndex].FindControl("txtDate");
        TextBox txtAmount = (TextBox)grdPyement.Rows[e.RowIndex].FindControl("txtAmount");

        Label lblReference = (Label)grdPyement.Rows[e.RowIndex].FindControl("lblReference");
        Label lblDate = (Label)grdPyement.Rows[e.RowIndex].FindControl("lblDate");
        Label lblAmount = (Label)grdPyement.Rows[e.RowIndex].FindControl("lblAmount");

        int nvendor_cost_id = Convert.ToInt32(grdPyement.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
        string strQ = "UPDATE partial_payment_info SET pay_type_id=" + Convert.ToInt32(ddlType.SelectedValue) + " , pay_term_id=" + Convert.ToInt32(ddlPayTerm.SelectedValue) + " ,reference='" + txtReference.Text.Replace("'", "''") + "',pay_date='" + Convert.ToDateTime(txtDate.Text.Replace("'", "''")) + "' ,pay_amount=" + Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) + "  WHERE payment_id =" + nvendor_cost_id + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        dtTerms = (DataTable)Session["Terms"];
        LoadDscription();
        lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
        

    }

    protected void grdPyement_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HtmlControl div = (HtmlControl)e.Row.FindControl("dvCalender");

            dtTerms = (DataTable)Session["Terms"];
            DropDownList ddlPayTerm = (DropDownList)e.Row.FindControl("ddlPayTerm");
            DropDownList ddlType = (DropDownList)e.Row.FindControl("ddlType");
            TextBox txtReference = (TextBox)e.Row.FindControl("txtReference");
            TextBox txtDate = (TextBox)e.Row.FindControl("txtDate");
            TextBox txtAmount = (TextBox)e.Row.FindControl("txtAmount");

            Label lblReference = (Label)e.Row.FindControl("lblReference");
            Label lblDate = (Label)e.Row.FindControl("lblDate");
            Label lblAmount = (Label)e.Row.FindControl("lblAmount");


            //int nVendor = Convert.ToInt32(ddlPayTerm.SelectedValue);
            //if (nVendor == 0 && nSection == 0)
            //{
            //string str = lblReference.Text.Replace("&nbsp;", "");
            //if (str == "")
            if (Convert.ToDecimal(lblAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) == 0)
            {
                txtReference.Visible = true;
                lblReference.Visible = false;
                //txtDate.Visible = true;
                div.Visible = true;
                lblDate.Visible = false;
                txtAmount.Visible = true;
                lblAmount.Visible = false;
                ddlPayTerm.Enabled = true;
                
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
                DropDownList ddlPayTerm = (DropDownList)di.FindControl("ddlPayTerm");
                DropDownList ddlType = (DropDownList)di.FindControl("ddlType");
                TextBox txtReference = (TextBox)di.FindControl("txtReference");
                TextBox txtDate = (TextBox)di.FindControl("txtDate");
                TextBox txtAmount = (TextBox)di.FindControl("txtAmount");

                Label lblReference = (Label)di.FindControl("lblReference");
                Label lblDate = (Label)di.FindControl("lblDate");
                Label lblAmount = (Label)di.FindControl("lblAmount");
                DataRow dr = table.Rows[di.RowIndex];

                dr["payment_id"] = Convert.ToInt32(grdPyement.DataKeys[di.RowIndex].Values[0]);
                dr["pay_term_id"] = Convert.ToInt32(ddlPayTerm.SelectedValue);
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
        drNew["pay_term_id"] = -1;
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
        grdPyement.DataKeyNames = new string[] { "payment_id" };
        grdPyement.DataBind();
        lblResult.Text = "";

    }

    protected void btnAddnewCOPay_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddnewCOPay.ID, btnAddnewCOPay.GetType().Name, "Click"); 
        DataTable table = (DataTable)Session["co_part_payment"];
        dtCO_NAME = (DataTable)Session["CO_NAME"];
        for (int i = 0; i < table.Rows.Count; i++)
        {
            if (Convert.ToInt32(table.Rows[i]["co_payment_id"]) == 0)
            {
                return;
            }
        }
        foreach (GridViewRow di in grdCOPayment.Rows)
        {
            {
                DropDownList ddlCOTerm = (DropDownList)di.FindControl("ddlCOTerm");
                DropDownList ddlCO = (DropDownList)di.FindControl("ddlCO");
                DropDownList ddlCOType = (DropDownList)di.FindControl("ddlCOType");
                TextBox txtCOReference = (TextBox)di.FindControl("txtCOReference");
                TextBox txtCoDate = (TextBox)di.FindControl("txtCoDate");
                TextBox txtCOAmount = (TextBox)di.FindControl("txtCOAmount");

                Label lblCOReference = (Label)di.FindControl("lblCOReference");
                Label lblCoDate = (Label)di.FindControl("lblCoDate");
                Label lblCOAmount = (Label)di.FindControl("lblCOAmount");
                DataRow dr = table.Rows[di.RowIndex];

                dr["co_payment_id"] = Convert.ToInt32(grdCOPayment.DataKeys[di.RowIndex].Values[0]);
                dr["co_pay_term_id"] = Convert.ToInt32(ddlCOTerm.SelectedValue);
                dr["change_order_id"] = Convert.ToInt32(ddlCO.SelectedValue);
                dr["client_id"] = 1;
                dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                dr["co_reference"] = txtCOReference.Text;
                dr["co_pay_type_id"] = Convert.ToInt32(ddlCOType.SelectedValue);
                dr["co_pay_amount"] = Convert.ToDecimal(txtCOAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                dr["co_pay_date"] = Convert.ToDateTime(txtCoDate.Text);
                dr["create_date"] = DateTime.Now;

            }

        }
        DataRow drNew = table.NewRow();
        drNew["co_payment_id"] = 0;
        drNew["co_pay_term_id"] = 2;
        drNew["change_order_id"] = -1;
        drNew["client_id"] = 1;
        drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
        drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
        drNew["co_reference"] = "";
        drNew["co_pay_type_id"] = 1;
        drNew["co_pay_amount"] = 0;
        drNew["co_pay_date"] = DateTime.Now;
        drNew["create_date"] = DateTime.Now;
        table.Rows.Add(drNew);

        Session.Add("co_part_payment", table);
        grdCOPayment.DataSource = table;
        grdCOPayment.DataKeyNames = new string[] { "co_payment_id" };
        grdCOPayment.DataBind();
        lblResult.Text = "";


    }
    private void LoadCODscription()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpTable = LoadCODataTable();

        var item = from it in _db.co_partial_payments
                   where it.customer_id == Convert.ToInt32(hdnCustomerId.Value) && it.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && it.client_id == 1
                   select new COPartialPayment()
                   {
                       co_payment_id = (int)it.co_payment_id,
                       co_pay_term_id = (int)it.co_pay_term_id,
                       co_pay_type_id = (int)it.co_pay_type_id,
                       change_order_id = (int)it.change_order_id,
                       client_id = (int)it.client_id,
                       customer_id = (int)it.customer_id,
                       estimate_id = (int)it.estimate_id,
                       co_reference = it.co_reference,
                       co_pay_amount = (decimal)it.co_pay_amount,
                       co_pay_date = (DateTime)it.co_pay_date,
                       create_date = (DateTime)it.create_date
                   };
        foreach (COPartialPayment pp in item)
        {
            DataRow drNew = tmpTable.NewRow();
            drNew["co_payment_id"] = pp.co_payment_id;
            drNew["co_pay_term_id"] = pp.co_pay_term_id;
            drNew["co_pay_type_id"] = pp.co_pay_type_id;
            drNew["change_order_id"] = pp.change_order_id;
            drNew["client_id"] = pp.client_id;
            drNew["customer_id"] = pp.customer_id;
            drNew["estimate_id"] = pp.estimate_id;
            drNew["co_reference"] = pp.co_reference;
            drNew["co_pay_amount"] = pp.co_pay_amount;
            drNew["co_pay_date"] = pp.co_pay_date;
            drNew["create_date"] = pp.create_date;
            tmpTable.Rows.Add(drNew);
        }
        if (tmpTable.Rows.Count == 0)
        {
            DataRow drNew = tmpTable.NewRow();
            drNew["co_payment_id"] = 0;
            drNew["co_pay_term_id"] = 2;
            drNew["co_pay_type_id"] = 1;
            drNew["change_order_id"] = -1;
            drNew["client_id"] = 1;
            drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew["co_reference"] = "";
            drNew["co_pay_amount"] = 0;
            drNew["co_pay_date"] = DateTime.Now;
            drNew["create_date"] = DateTime.Now;
            tmpTable.Rows.Add(drNew);
        }
        Session.Add("co_part_payment", tmpTable);
        grdCOPayment.DataSource = tmpTable;
        grdCOPayment.DataKeyNames = new string[] { "co_payment_id" };
        grdCOPayment.DataBind();

    }
    private DataTable LoadCODataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("co_payment_id", typeof(int));
        table.Columns.Add("co_pay_term_id", typeof(int));
        table.Columns.Add("co_pay_type_id", typeof(int));
        table.Columns.Add("change_order_id", typeof(int));
        table.Columns.Add("client_id", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("co_reference", typeof(string));
        table.Columns.Add("co_pay_amount", typeof(decimal));
        table.Columns.Add("co_pay_date", typeof(DateTime));
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
    protected void grdCOPayment_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable table = (DataTable)Session["co_part_payment"];
            dtCO_NAME = (DataTable)Session["CO_NAME"];

            foreach (GridViewRow di in grdCOPayment.Rows)
            {
                {
                    DropDownList ddlCOTerm = (DropDownList)di.FindControl("ddlCOTerm");
                    DropDownList ddlCO = (DropDownList)di.FindControl("ddlCO");
                    DropDownList ddlCOType = (DropDownList)di.FindControl("ddlCOType");
                    TextBox txtCOReference = (TextBox)di.FindControl("txtCOReference");
                    TextBox txtCoDate = (TextBox)di.FindControl("txtCoDate");
                    TextBox txtCOAmount = (TextBox)di.FindControl("txtCOAmount");

                    Label lblCOReference = (Label)di.FindControl("lblCOReference");
                    Label lblCoDate = (Label)di.FindControl("lblCoDate");
                    Label lblCOAmount = (Label)di.FindControl("lblCOAmount");
                    DataRow dr = table.Rows[di.RowIndex];

                    dr["co_payment_id"] = Convert.ToInt32(grdCOPayment.DataKeys[di.RowIndex].Values[0]);
                    dr["co_pay_term_id"] = Convert.ToInt32(ddlCOTerm.SelectedValue);
                    dr["change_order_id"] = Convert.ToInt32(ddlCO.SelectedValue);
                    dr["client_id"] = 1;
                    dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                    dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                    dr["co_reference"] = txtCOReference.Text;
                    dr["co_pay_type_id"] = Convert.ToInt32(ddlCOType.SelectedValue);
                    dr["co_pay_amount"] = Convert.ToDecimal(txtCOAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                    dr["co_pay_date"] = Convert.ToDateTime(txtCoDate.Text);
                    dr["create_date"] = DateTime.Now;

                }

            }
            foreach (DataRow dr in table.Rows)
            {
                co_partial_payment pay_cost = new co_partial_payment();
                if (Convert.ToInt32(dr["co_payment_id"]) > 0)
                    pay_cost = _db.co_partial_payments.Single(l => l.co_payment_id == Convert.ToInt32(dr["co_payment_id"]));

                if (Convert.ToDecimal(dr["co_pay_amount"]) != 0)
                {
                    if (Convert.ToInt32(dr["change_order_id"]) == -1)
                    {
                        lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Please Select Change Order");
                        
                        return;
                    }
                    pay_cost.co_payment_id = Convert.ToInt32(dr["co_payment_id"]);
                    pay_cost.co_pay_term_id = Convert.ToInt32(dr["co_pay_term_id"]);
                    pay_cost.change_order_id = Convert.ToInt32(dr["change_order_id"]);
                    pay_cost.client_id = Convert.ToInt32(dr["client_id"]);
                    pay_cost.customer_id = Convert.ToInt32(dr["customer_id"]);
                    pay_cost.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                    pay_cost.co_pay_type_id = Convert.ToInt32(dr["co_pay_type_id"]);
                    pay_cost.co_reference = dr["co_reference"].ToString();
                    pay_cost.co_pay_amount = Convert.ToDecimal(dr["co_pay_amount"]);
                    pay_cost.co_pay_date = Convert.ToDateTime(dr["co_pay_date"]);
                    pay_cost.create_date = Convert.ToDateTime(dr["create_date"]);

		if (Convert.ToInt32(dr["co_payment_id"]) == 0)
                {
                    _db.co_partial_payments.InsertOnSubmit(pay_cost);
                }

                }
                
            }

            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            
            _db.SubmitChanges();
            dtCO_NAME = (DataTable)Session["CO_NAME"];
            LoadCODscription();

        }
    }

    protected void grdCOPayment_RowEditing(object sender, GridViewEditEventArgs e)
    {

        HtmlControl div = (HtmlControl)grdCOPayment.Rows[e.NewEditIndex].FindControl("dvCoCalender");
        DropDownList ddlCOTerm = (DropDownList)grdCOPayment.Rows[e.NewEditIndex].FindControl("ddlCOTerm");
        DropDownList ddlCO = (DropDownList)grdCOPayment.Rows[e.NewEditIndex].FindControl("ddlCO");
        DropDownList ddlCOType = (DropDownList)grdCOPayment.Rows[e.NewEditIndex].FindControl("ddlCOType");
        TextBox txtCOReference = (TextBox)grdCOPayment.Rows[e.NewEditIndex].FindControl("txtCOReference");
        TextBox txtCoDate = (TextBox)grdCOPayment.Rows[e.NewEditIndex].FindControl("txtCoDate");
        TextBox txtCOAmount = (TextBox)grdCOPayment.Rows[e.NewEditIndex].FindControl("txtCOAmount");

        Label lblCOReference = (Label)grdCOPayment.Rows[e.NewEditIndex].FindControl("lblCOReference");
        Label lblCoDate = (Label)grdCOPayment.Rows[e.NewEditIndex].FindControl("lblCoDate");
        Label lblCOAmount = (Label)grdCOPayment.Rows[e.NewEditIndex].FindControl("lblCOAmount");

        txtCOReference.Visible = true;
        lblCOReference.Visible = false;
        // txtDate.Visible = true;
        div.Visible = true;
        lblCoDate.Visible = false;
        txtCOAmount.Visible = true;
        lblCOAmount.Visible = false;

        ddlCOTerm.Enabled = true;
        ddlCOType.Enabled = true;
        ddlCO.Enabled = true;

        LinkButton btn = (LinkButton)grdCOPayment.Rows[e.NewEditIndex].Cells[6].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdCOPayment_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        DropDownList ddlCOTerm = (DropDownList)grdCOPayment.Rows[e.RowIndex].FindControl("ddlCOTerm");
        DropDownList ddlCO = (DropDownList)grdCOPayment.Rows[e.RowIndex].FindControl("ddlCO");
        DropDownList ddlCOType = (DropDownList)grdCOPayment.Rows[e.RowIndex].FindControl("ddlCOType");
        TextBox txtCOReference = (TextBox)grdCOPayment.Rows[e.RowIndex].FindControl("txtCOReference");
        TextBox txtCoDate = (TextBox)grdCOPayment.Rows[e.RowIndex].FindControl("txtCoDate");
        TextBox txtCOAmount = (TextBox)grdCOPayment.Rows[e.RowIndex].FindControl("txtCOAmount");

        Label lblCOReference = (Label)grdCOPayment.Rows[e.RowIndex].FindControl("lblCOReference");
        Label lblCoDate = (Label)grdCOPayment.Rows[e.RowIndex].FindControl("lblCoDate");
        Label lblCOAmount = (Label)grdCOPayment.Rows[e.RowIndex].FindControl("lblCOAmount");

        int nco_pay_id = Convert.ToInt32(grdPyement.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
        string strQ = "UPDATE co_partial_payment SET co_pay_type_id=" + Convert.ToInt32(ddlCOType.SelectedValue) + " , co_pay_term_id=" + Convert.ToInt32(ddlCOTerm.SelectedValue) + " ,co_reference='" + txtCOReference.Text.Replace("'", "''") + "',co_pay_date='" + Convert.ToDateTime(txtCoDate.Text.Replace("'", "''")) + "' ,co_pay_amount=" + Convert.ToDecimal(txtCOAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) + "  WHERE co_payment_id =" + nco_pay_id + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND change_order_id=" + Convert.ToInt32(ddlCO.SelectedValue) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        dtCO_NAME = (DataTable)Session["CO_NAME"];
        LoadCODscription();
        lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
        

    }

    protected void grdCOPayment_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            dtCO_NAME = (DataTable)Session["CO_NAME"];

            HtmlControl div = (HtmlControl)e.Row.FindControl("dvCoCalender");
            DropDownList ddlCOTerm = (DropDownList)e.Row.FindControl("ddlCOTerm");
            DropDownList ddlCO = (DropDownList)e.Row.FindControl("ddlCO");
            DropDownList ddlCOType = (DropDownList)e.Row.FindControl("ddlCOType");
            TextBox txtCOReference = (TextBox)e.Row.FindControl("txtCOReference");
            TextBox txtCoDate = (TextBox)e.Row.FindControl("txtCoDate");
            TextBox txtCOAmount = (TextBox)e.Row.FindControl("txtCOAmount");

            Label lblCOReference = (Label)e.Row.FindControl("lblCOReference");
            Label lblCoDate = (Label)e.Row.FindControl("lblCoDate");
            Label lblCOAmount = (Label)e.Row.FindControl("lblCOAmount");

            //int nVendor = Convert.ToInt32(ddlPayTerm.SelectedValue);
            //if (nVendor == 0 && nSection == 0)
            //{
            //string str = lblReference.Text.Replace("&nbsp;", "");
            //if (str == "")
            if (Convert.ToDecimal(lblCOAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) == 0)
            {
                txtCOReference.Visible = true;
                lblCOReference.Visible = false;
                //txtDate.Visible = true;
                div.Visible = true;
                lblCoDate.Visible = false;
                txtCOAmount.Visible = true;
                lblCOAmount.Visible = false;
                ddlCOTerm.Enabled = true;
                ddlCO.Enabled = true;
                ddlCOType.Enabled = true;
                LinkButton btn = (LinkButton)e.Row.Cells[6].Controls[0];
                btn.Text = "Save";
                btn.CommandName = "Save";

            }


        }

    }
}
