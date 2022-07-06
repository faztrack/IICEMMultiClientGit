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

public partial class vendor_cost_info : System.Web.UI.Page
{
    public DataTable dtVendor;
    public DataTable dtSection;
   
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("admin015") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            if (Request.QueryString.Get("cid") != null)
                  hdnCustomerId.Value = Convert.ToInt32(Request.QueryString.Get("cid")).ToString();
            if (Request.QueryString.Get("eid") != null)
                hdnEstimateId.Value = Convert.ToInt32(Request.QueryString.Get("eid")).ToString();

            DataClassesDataContext _db = new DataClassesDataContext();

            // Get Customer Information
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                hdnSalesPersonId.Value = cust.sales_person_id.ToString();
                hdnClientId.Value = cust.client_id.ToString();
            }
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
            estimate_payment esp = new estimate_payment();
            if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
            {
                esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(hdnClientId.Value));
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
                //lblProjectTotal.Text = totalwithtax.ToString("c"); //with Taxes
                lblProjectTotal.Text = project_subtotal.ToString("c");// Without Taxes
                hdnEstPaymentId.Value = esp.est_payment_id.ToString();

            }

            decimal payAmount = 0;
            var result = (from ppi in _db.partial_payment_infos
                          where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.client_id == Convert.ToInt32(hdnClientId.Value)
                          select ppi.pay_amount);
            int n = result.Count();
            if (result != null && n > 0)
                payAmount = result.Sum();
            lblContractRecive.Text = payAmount.ToString("c");

            decimal COAmount = 0;
            var Co_result = (from cpi in _db.change_order_pricing_lists
                             join cho in _db.changeorder_estimates on new { cpi.chage_order_id, cpi.customer_id, cpi.estimate_id } equals new {chage_order_id = cho.chage_order_id,customer_id = cho.customer_id,estimate_id=cho.estimate_id }
                             where cpi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cpi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cho.change_order_status_id == 3 && cho.change_order_type_id != 3 && cpi.client_id == Convert.ToInt32(hdnClientId.Value)
                             select  cpi.EconomicsCost);
            
            int co_AM = Co_result.Count();
            if (result != null && co_AM > 0)
                COAmount = Co_result.Sum();
            lblTotalCOAmount.Text = COAmount.ToString("c");

            decimal COpayAmount = 0;
            var Coresult = (from cpi in _db.co_partial_payments
                          where cpi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cpi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cpi.client_id == Convert.ToInt32(hdnClientId.Value)
                            select cpi.co_pay_amount);
            int co = Coresult.Count();
            if (result != null && co > 0)
                COpayAmount = Coresult.Sum();
            lblCORecive.Text = COpayAmount.ToString("c");

            decimal TotalReciveAmount = 0;
            TotalReciveAmount = payAmount + COpayAmount;
            lblTotalRecive.Text = TotalReciveAmount.ToString("c");
            
            // Get vendor from DB
            LoadVendor();
            LoadSection();
        
            LoadDscription();
            LoadCommission();
            LoadCO_Commission();
            Calculate();
        }

    }
    private void Calculate()
    {
        decimal nProjecttotal = Convert.ToDecimal(lblProjectTotal.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
        decimal nTotalRecive = Convert.ToDecimal(lblTotalRecive.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));
        
        decimal nLabor = 0;
        decimal nMatrial = 0;
        decimal nCommission = 0;
        decimal ncoCommission = 0;
        decimal nTotalCommission = 0;
        foreach (GridViewRow di in grdVendorCost.Rows)
        {
            {
                DropDownList ddlCategory = (DropDownList)di.FindControl("ddlCategory");
                TextBox txtAmount = (TextBox)di.FindControl("txtAmount");
                if(Convert.ToInt32(ddlCategory.SelectedValue)==1)
                    nMatrial += Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                else
                    nLabor += Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

            }

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


        decimal ntotalcost = nCommission + nLabor + nMatrial + ncoCommission;
        decimal nProfit = nTotalRecive - ntotalcost;

        lblTotalCom.Text = nTotalCommission.ToString("c");

        lblLabor.Text = nLabor.ToString("c");
        lblMaterial.Text = nMatrial.ToString("c");
       // lblComission.Text = nCom.ToString("c");
        lblTotalCost.Text = ntotalcost.ToString("c");
        lblProfit.Text = nProfit.ToString("c");
        if (nProfit > 0)
            lblProfit.ForeColor = System.Drawing.Color.Green;
        else
            lblProfit.ForeColor = System.Drawing.Color.Red;


        
    }
    private void LoadCommission()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpCTable = LoadComTable();
        decimal Comper=0;
        decimal sCommission = 0;
        if (Convert.ToInt32(hdnSalesPersonId.Value) > 0)
        {
            sales_person sp_info = new sales_person();
            sp_info = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
            if (sp_info.com_per != null)
                Comper = Convert.ToDecimal(sp_info.com_per);
            decimal nProjecttotal = Convert.ToDecimal(lblProjectTotal.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));

            if (Comper > 0 && nProjecttotal > 0)
            {
                sCommission = nProjecttotal * (Comper / 100);
            }


        }

        var item = from it in _db.estimate_commissions
                   where it.customer_id == Convert.ToInt32(hdnCustomerId.Value) && it.estimate_id == Convert.ToInt32(hdnEstimateId.Value) 
                   select new EstCom()
                   {
                       client_id = (int)it.client_id,
                       estimate_com_id = (int)it.estimate_com_id,
                       customer_id = (int)it.customer_id,
                       estimate_id = (int)it.estimate_id,
                       comission_amount=(decimal)it.comission_amount

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
            drNew["client_id"] = Convert.ToInt32(hdnClientId.Value);
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
        decimal Comper = 0;
        decimal sCOCommission = 0;
        if (Convert.ToInt32(hdnSalesPersonId.Value) > 0)
        {
            sales_person sp_info = new sales_person();
            sp_info = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
            if (sp_info.com_per != null)
                Comper = Convert.ToDecimal(sp_info.com_per);
            decimal nCOtotal = Convert.ToDecimal(lblTotalCOAmount.Text.Trim().Replace("$", "").Replace("(", "-").Replace(")", ""));

            if (Comper > 0 && nCOtotal > 0)
            {
                sCOCommission = nCOtotal * (Comper / 100);
            }


        }

        var item = from it in _db.co_estimate_commissions
                   where it.customer_id == Convert.ToInt32(hdnCustomerId.Value) && it.estimate_id == Convert.ToInt32(hdnEstimateId.Value)
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
                    where it.customer_id == Convert.ToInt32(hdnCustomerId.Value) && it.estimate_id == Convert.ToInt32(hdnEstimateId.Value)
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

        var item = from it in _db.vendor_infos
                   where it.client_id == Convert.ToInt32(hdnClientId.Value)
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
                   where it.customer_id == Convert.ToInt32(hdnCustomerId.Value) && it.estimate_id == Convert.ToInt32(hdnEstimateId.Value) 
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
            tmpTable.Rows.Add(drNew);
        }
        Session.Add("vendor_cost_desc", tmpTable);
        DataView dv = tmpTable.DefaultView;
        dv.Sort = "cost_date ASC";
        grdVendorCost.DataSource = dv;
        grdVendorCost.DataKeyNames = new string[] { "vendor_cost_id" };
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

        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            DataTable table = (DataTable)Session["vendor_cost_desc"];
            dtVendor = (DataTable)Session["Vendor"];

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
            foreach (DataRow dr in table.Rows)
            {
                vendor_cost ven_cost = new vendor_cost();
                if (Convert.ToInt32(dr["vendor_cost_id"]) > 0)
                    ven_cost = _db.vendor_costs.Single(l => l.vendor_cost_id == Convert.ToInt32(dr["vendor_cost_id"]));
                //int nVendorId = Convert.ToInt32(dr["vendor_id"]);
                //int nSectionId = Convert.ToInt32(dr["section_id"]);
                //if (nVendorId > 0 && nSectionId > 0)
               // string str = dr["cost_description"].ToString().Trim();
                if(Convert.ToDecimal(dr["cost_amount"])!=0)
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
                    _db.vendor_costs.InsertOnSubmit(ven_cost);
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

        txtDescription.Visible = true;
        lblDescription.Visible = false;
       // txtDate.Visible = true;
        div.Visible = true;
        lblDate.Visible = false;
        txtAmount.Visible = true;
        lblAmount.Visible = false;

        ddlVendor.Enabled = true;
        ddlTrade.Enabled = true;
        ddlCategory.Enabled = true;

        LinkButton btn = (LinkButton)grdVendorCost.Rows[e.NewEditIndex].Cells[6].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdVendorCost_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        DropDownList ddlVendor = (DropDownList)grdVendorCost.Rows[e.RowIndex].FindControl("ddlVendor");
        DropDownList ddlTrade = (DropDownList)grdVendorCost.Rows[e.RowIndex].FindControl("ddlTrade");
        DropDownList ddlCategory = (DropDownList)grdVendorCost.Rows[e.RowIndex].FindControl("ddlCategory");
        TextBox txtDescription = (TextBox)grdVendorCost.Rows[e.RowIndex].FindControl("txtDescription");
        TextBox txtDate = (TextBox)grdVendorCost.Rows[e.RowIndex].FindControl("txtDate");
        TextBox txtAmount = (TextBox)grdVendorCost.Rows[e.RowIndex].FindControl("txtAmount");

        Label lblDescription = (Label)grdVendorCost.Rows[e.RowIndex].FindControl("lblDescription");
        Label lblDate = (Label)grdVendorCost.Rows[e.RowIndex].FindControl("lblDate");
        Label lblAmount = (Label)grdVendorCost.Rows[e.RowIndex].FindControl("lblAmount");

        int nvendor_cost_id = Convert.ToInt32(grdVendorCost.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
        string strQ = "UPDATE vendor_cost SET category_id=" + Convert.ToInt32(ddlCategory.SelectedValue) + " , section_id=" + Convert.ToInt32(ddlTrade.SelectedValue) + " , vendor_id=" + Convert.ToInt32(ddlVendor.SelectedValue) + " ,cost_description='" + txtDescription.Text.Replace("'", "''") + "',cost_date='" + Convert.ToDateTime(txtDate.Text.Replace("'", "''")) + "' ,cost_amount=" + Convert.ToDecimal(txtAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", "")) + "  WHERE vendor_cost_id =" + nvendor_cost_id + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        dtVendor = (DataTable)Session["Vendor"];
        dtSection = (DataTable)Session["Section"];
        LoadDscription();
        Calculate();
        lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
       

    }

    protected void grdVendorCost_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HtmlControl div = (HtmlControl)e.Row.FindControl("dvCalender");

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


            //int nVendor = Convert.ToInt32(ddlVendor.SelectedValue);
            //int nSection = Convert.ToInt32(ddlTrade.SelectedValue);
            //if (nVendor == 0 && nSection == 0)
            //{
            //string str = lblDescription.Text.Replace("&nbsp;", "");
            //if (str == "")
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
                LinkButton btn = (LinkButton)e.Row.Cells[6].Controls[0];
                btn.Text = "Save";
                btn.CommandName = "Save";

            }


        }

    }
    protected void btnAddnewRow_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddnewRow.ID, btnAddnewRow.GetType().Name, "Click"); 
        DataTable table = (DataTable)Session["vendor_cost_desc"];
        dtVendor = (DataTable)Session["Vendor"];
        dtSection = (DataTable)Session["Section"];
        for (int i = 0; i < table.Rows.Count; i++)
        {
            if (Convert.ToInt32(table.Rows[i]["vendor_cost_id"]) == 0)
            {
                return;
            }
        }
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
        grdVendorCost.DataKeyNames = new string[] { "vendor_cost_id" };
        grdVendorCost.DataBind();
        lblResult.Text = "";

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
        lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
      

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
        lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
       

    }
    protected void btnAcceptPayment_Click(object sender, EventArgs e)
    {
        if (Convert.ToInt32(hdnEstPaymentId.Value) > 0)
        {
            Response.Redirect("payment_withco.aspx?cid=" + hdnCustomerId.Value + "&epid=" + hdnEstPaymentId.Value + "&eid=" + hdnEstimateId.Value);
        }
        else
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Payment terms not yet saved Please click on Go To Payment to set payment terms.");
            return;

        }
    }
    protected void btnGoToPayment_Click(object sender, EventArgs e)
    {
        Response.Redirect("payment_info.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
    }
}


