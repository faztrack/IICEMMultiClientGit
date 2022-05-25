using DataStreams.Csv;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AllowanceReport : System.Web.UI.Page
{
    private double grandtotal = 0.0;
    private double Actgrandtotal = 0.0;
    private double Diffgrandtotal = 0.0;
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

            if (Page.User.IsInRole("rpt012") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            hdnEstimateId.Value = "0";

            int ncid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = ncid.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);

            DataClassesDataContext _db = new DataClassesDataContext();

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
                string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

                hdnClientId.Value = cust.client_id.ToString();

            }

            if (Request.QueryString.Get("eid") != null)
                hdnEstimateId.Value = Convert.ToInt32(Request.QueryString.Get("eid")).ToString();


            // Get Estimate Info
            if (Convert.ToInt32(hdnEstimateId.Value) > 0)
            {
                if (_db.co_pricing_masters.Any(ch => ch.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ch.estimate_id == Convert.ToInt32(hdnEstimateId.Value)))
                {
                    hdnAllowance.Value = "1"; // Set value 1 
                }
                customer_estimate cus_est = new customer_estimate();
                cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

                lblEstimateName.Text = cus_est.estimate_name;
                //lblJobNumber.Text = cus_est.job_number;
                if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                    lblJobNumber.Text = cus_est.job_number;
                else
                    lblJobNumber.Text = cus_est.alter_job_number;
               
                GetAllowncePrice();
                BindGrid();

                if ((cus_est.job_number ?? "").Length > 0)
                {
                   // lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                    if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                        lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                    else
                        lblTitelJobNumber.Text = " ( Job Number: " + cus_est.alter_job_number + " )";
                }
            }



        }
    }
    protected string GetTotalPrice()
    {
        return grandtotal.ToString("c");
    }
    protected string GetActualTotalPrice()
    {
        return Actgrandtotal.ToString("c");
    }
    protected string GetDiffTotalPrice()
    {
        return Diffgrandtotal.ToString("c");
    }


    public void GetAllowncePrice()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable dtAllowance = new DataTable();
        string StrQ = string.Empty;
        if (Convert.ToInt32(hdnAllowance.Value) > 0)
        {
            StrQ = " SELECT PD.customer_id,PD.estimate_id,PD.co_pricing_list_id as pricing_id,PD.item_id,l.location_name, PD.section_name,PD.item_name,PD.short_notes,PD.measure_unit,PD.total_retail_price, " +
                     " ISNULL(AD.actual_price,0) AS actual_price, PD.total_retail_price-ISNULL(AD.actual_price,0) AS price_difference,ISNULL(AD.allowance_id,0) AS allowance_id " +
                     " FROM co_pricing_master PD " +
                     " INNER JOIN location l ON l.location_id = PD.location_id " +

                     " LEFT OUTER JOIN allowance_details AD ON  PD.customer_id = AD.customer_id AND   PD.estimate_id = AD.estimate_id AND PD.co_pricing_list_id = AD.pricing_id AND  PD.item_id = AD.item_id " +
                     " WHERE PD.location_id IN (SELECT location_id FROM changeorder_locations WHERE estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + ")  " +
                      " AND PD.section_level IN (SELECT section_id  FROM  changeorder_sections  WHERE  estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + ") " +
                     " AND PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " and PD.item_status_id=1 and PD.item_name like '%Allowance%' ORDER BY PD.section_level ";
        }
        else
        {
          StrQ = " SELECT PD.customer_id,PD.estimate_id,PD.pricing_id,PD.item_id,l.location_name, PD.section_name,PD.item_name,PD.short_notes,PD.measure_unit,PD.total_retail_price, " +
                     " ISNULL(AD.actual_price,0) AS actual_price, PD.total_retail_price-ISNULL(AD.actual_price,0) AS price_difference,ISNULL(AD.allowance_id,0) AS allowance_id " +
                     " FROM pricing_details PD " +
                     " INNER JOIN location l ON l.location_id = PD.location_id " +

                     " LEFT OUTER JOIN allowance_details AD ON  PD.customer_id = AD.customer_id AND   PD.estimate_id = AD.estimate_id AND PD.pricing_id = AD.pricing_id AND  PD.item_id = AD.item_id " +
                     " WHERE PD.location_id IN (SELECT location_id FROM customer_locations WHERE estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + ")  " +
                      " AND PD.section_level IN (SELECT section_id  FROM  customer_sections  WHERE  estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + ") " +
                     " AND PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " and PD.item_name like '%Allowance%' ORDER BY PD.section_level ";
        }
       

        dtAllowance = csCommonUtility.GetDataTable(StrQ);

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
    }

    public void BindGrid()
    {
        DataTable dtAllowance = new DataTable();

        string StrQ = string.Empty;
        if (Convert.ToInt32(hdnAllowance.Value) > 0)
        {
            StrQ = " SELECT PD.customer_id,PD.estimate_id,PD.co_pricing_list_id as pricing_id,PD.item_id,l.location_name, PD.section_name,PD.item_name,PD.short_notes,PD.measure_unit,PD.quantity,PD.total_retail_price, " +
                     " ISNULL(AD.actual_price,0) AS actual_price, PD.total_retail_price-ISNULL(AD.actual_price,0) AS price_difference,ISNULL(AD.allowance_id,0) AS allowance_id " +
                     " FROM co_pricing_master PD " +
                     " INNER JOIN location l ON l.location_id = PD.location_id " +

                     " LEFT OUTER JOIN allowance_details AD ON  PD.customer_id = AD.customer_id AND   PD.estimate_id = AD.estimate_id AND PD.co_pricing_list_id = AD.pricing_id AND  PD.item_id = AD.item_id " +
                     " WHERE PD.location_id IN (SELECT location_id FROM changeorder_locations WHERE estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + ")  " +
                      " AND PD.section_level IN (SELECT section_id  FROM  changeorder_sections  WHERE  estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + ") " +
                     " AND PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " and PD.item_status_id=1 and PD.item_name like '%Allowance%' ORDER BY PD.section_level ";
        }
        else
        {
            StrQ = " SELECT PD.customer_id,PD.estimate_id,PD.pricing_id,PD.item_id,l.location_name, PD.section_name,PD.item_name,PD.short_notes,PD.measure_unit,PD.quantity,PD.total_retail_price, " +
                       " ISNULL(AD.actual_price,0) AS actual_price, PD.total_retail_price-ISNULL(AD.actual_price,0) AS price_difference,ISNULL(AD.allowance_id,0) AS allowance_id " +
                       " FROM pricing_details PD " +
                       " INNER JOIN location l ON l.location_id = PD.location_id " +

                       " LEFT OUTER JOIN allowance_details AD ON  PD.customer_id = AD.customer_id AND   PD.estimate_id = AD.estimate_id AND PD.pricing_id = AD.pricing_id AND  PD.item_id = AD.item_id " +
                       " WHERE PD.location_id IN (SELECT location_id FROM customer_locations WHERE estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + ")  " +
                        " AND PD.section_level IN (SELECT section_id  FROM  customer_sections  WHERE  estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + ") " +
                       " AND PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " and PD.item_name like '%Allowance%' ORDER BY PD.section_level ";
        }
       

        dtAllowance = csCommonUtility.GetDataTable(StrQ);
        Session.Add("Allowance", dtAllowance);

        grdAllowance.DataSource = dtAllowance;
        grdAllowance.DataKeyNames = new string[] { "allowance_id", "pricing_id", "customer_id", "estimate_id" };
        grdAllowance.DataBind();


    }

    private DataTable LoadDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("Location", typeof(string));
        table.Columns.Add("Section", typeof(string));
        table.Columns.Add("Item", typeof(string));
        table.Columns.Add("Short Note", typeof(string));
        table.Columns.Add("UOM", typeof(string));
        table.Columns.Add("Code", typeof(string));
        table.Columns.Add("Cost", typeof(string));
        table.Columns.Add("Actual Cost", typeof(string));
        table.Columns.Add("Difference", typeof(string));

        return table;
    }

    protected void btnExpList_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnExpList.ID, btnExpList.GetType().Name, "Click"); 
        DataTable dtTmp = (DataTable)Session["Allowance"];
        decimal Gtotal = 0;
        decimal GActualtotal = 0;
        decimal GDifftotal = 0;
        DataTable tmpTable = LoadDataTable();
        DataRow drNew = tmpTable.NewRow();
        //drNew["Location"] = "Allowance Report";
        //tmpTable.Rows.Add(drNew);
        foreach (DataRow dr in dtTmp.Rows)
        {
            Gtotal += Convert.ToDecimal(dr["total_retail_price"]);
            GActualtotal += Convert.ToDecimal(dr["actual_price"]);
            GDifftotal += Convert.ToDecimal(dr["price_difference"]);

            drNew = tmpTable.NewRow();
            drNew["Location"] = dr["location_name"];
            drNew["Section"] = dr["section_name"];
            drNew["Item"] = dr["item_name"];
            drNew["Short Note"] = dr["short_notes"];
            drNew["UOM"] = dr["measure_unit"];
            drNew["Code"] = dr["quantity"];
            drNew["Cost"] = Convert.ToDecimal(dr["total_retail_price"]).ToString("c");
            drNew["Actual Cost"] = Convert.ToDecimal(dr["actual_price"]).ToString("c");
            drNew["Difference"] = Convert.ToDecimal(dr["price_difference"]).ToString("c");
            tmpTable.Rows.Add(drNew);

        }
        drNew = tmpTable.NewRow();
        drNew["Location"] = "TOTAL:";
        drNew["Cost"] = Gtotal.ToString("c");
        drNew["Actual Cost"] = GActualtotal.ToString("c");
        drNew["Difference"] = GDifftotal.ToString("c");
        tmpTable.Rows.Add(drNew);



        Response.Clear();
        Response.ClearHeaders();

        using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
        {
            string[] str1 = { "Allowance Report" };
            string[] str2 = { " " };

            writer.WriteRecord(str1);
            writer.WriteRecord(str2, true);
            writer.WriteAll(tmpTable, true);
        }
        Response.ContentType = "application/vnd.ms-excel";
        Response.AddHeader("Content-Disposition", "attachment; filename=AllowanceReport.csv");
        Response.End();

    }
    protected void grdAllowance_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblAmount = (Label)e.Row.FindControl("lblAmount");

            double dTotal = Convert.ToDouble(lblAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

            grandtotal += dTotal;



            TextBox txtActualCost = (TextBox)e.Row.FindControl("txtActualCost");

            Label lblPriceDifference = (Label)e.Row.FindControl("lblPriceDifference");

            double nActualCost = Convert.ToDouble(txtActualCost.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

            Actgrandtotal += nActualCost;


            double dPriceDifference = 0;

            dPriceDifference = dTotal - nActualCost;
            Diffgrandtotal += dPriceDifference;

            lblPriceDifference.Text = dPriceDifference.ToString("c");

            if (dTotal < nActualCost)
                lblPriceDifference.ForeColor = Color.Red;
            else if (dTotal > nActualCost)
                lblPriceDifference.ForeColor = Color.Green;
            else if (dTotal == nActualCost)
                lblPriceDifference.ForeColor = Color.Black;

        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }

    protected void txtActualCost_TextChanged(object sender, EventArgs e)
    {
       
        string strSender = sender.ToString();
        int i = 0;
        if (strSender.IndexOf("TextBox") != -1)
        {
            TextBox txtActualCost1 = (TextBox)grdAllowance.FindControl("txtActualCost");
            txtActualCost1 = (TextBox)sender;
            GridViewRow gvr = (GridViewRow)txtActualCost1.NamingContainer;
            i = gvr.RowIndex;
        }


        Label lblAmount = (Label)grdAllowance.Rows[i].FindControl("lblAmount");
        TextBox txtActualCost = (TextBox)grdAllowance.Rows[i].FindControl("txtActualCost");

        Label lblPriceDifference = (Label)grdAllowance.Rows[i].FindControl("lblPriceDifference");

        int nAllowanceId = Convert.ToInt32(grdAllowance.DataKeys[i].Values[0].ToString());

        if (txtActualCost.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing  Quantity.");
            return;
        }
        else
        {
            try
            {
                Convert.ToDecimal(txtActualCost.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid  quantity.");

                return;
            }
        }

        decimal nActualCost = Convert.ToDecimal(txtActualCost.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
        decimal nCost = Convert.ToDecimal(lblAmount.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));

        decimal dPriceDifference = 0;

        dPriceDifference = nCost - nActualCost;


        lblPriceDifference.Text = dPriceDifference.ToString("c");

        if (nCost < nActualCost)
            lblPriceDifference.ForeColor = Color.Red;
        else if (nCost > nActualCost)
            lblPriceDifference.ForeColor = Color.Green;
        else if (nCost == nActualCost)
            lblPriceDifference.ForeColor = Color.Black;

        DataClassesDataContext _db = new DataClassesDataContext();
        allowance_detail objalwncdtl = new allowance_detail();

        objalwncdtl = _db.allowance_details.SingleOrDefault(al => al.allowance_id == nAllowanceId);

        if (objalwncdtl != null)
        {
            objalwncdtl.actual_price = Convert.ToDecimal(txtActualCost.Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
            objalwncdtl.price_difference = dPriceDifference;
        }


        _db.SubmitChanges();
        BindGrid();

    }
}