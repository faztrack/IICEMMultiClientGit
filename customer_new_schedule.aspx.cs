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

public partial class customer_new_schedule : System.Web.UI.Page
{
   
    private string strSection = "";
    private int i = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        btnPrev.Attributes.Add("onClick", "DisplayWindow4();");
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCid.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstid = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstid.ToString();

            DataClassesDataContext _db = new DataClassesDataContext();
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                string strAddress = "";
                strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                lblAddress.Text = strAddress;
                lblEmail.Text = cust.email;
                lblPhone.Text = cust.phone;
                lbltopHead.Text = "Schedule for " + cust.first_name1 + " " + cust.last_name1;

                // Get Estimate Info
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                    lblEstimateName.Text = cus_est.estimate_name;
                    ddlStatus.SelectedValue = cus_est.status_id.ToString();
                    ddlStatus.Enabled = false;
                    lblEstStatus.Text = ddlStatus.SelectedItem.Text;
                    if (ddlStatus.SelectedValue == "3")
                    {
                        lblSaleDate.Visible = true;
                        lblDateOfSale.Text = Convert.ToDateTime(cus_est.sale_date).ToShortDateString();
                        lblDateOfSale.Visible = true;

                    }
                    else
                    {
                        lblSaleDate.Visible = false;
                        lblDateOfSale.Visible = false;
                    }

                }
                GetData(1);
                GetData(2);


               
            }
        }
    }

    protected void btnGotoCustomerList_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }

   
  
    protected void grdSelectedItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            string str = grdSelectedItem1.DataKeys[e.Row.RowIndex].Values[1].ToString();
            if (str != strSection)
            {
                i = i + 1;
                strSection = str;
                GridViewRow objgridviewrow = new GridViewRow(e.Row.RowIndex + i, 0, DataControlRowType.Separator, DataControlRowState.Insert);

                //Creating a table cell object

                TableCell objtablecell = new TableCell();

                AddMergedCells(objgridviewrow, objtablecell, 3, "Section:", System.Drawing.Color.LightGreen.Name);

                AddMergedCells(objgridviewrow, objtablecell, 7, str, System.Drawing.Color.LightSkyBlue.Name);

                //Lastly add the gridrow object to the gridview object at the 0th position

                //Because,the header row position is 0.

                grdSelectedItem1.Controls[0].Controls.AddAt(e.Row.RowIndex + i, objgridviewrow);

            }
           string strNote = e.Row.Cells[9].Text.Replace("&nbsp;", "");
            if (strNote != "" && strNote.Length > 25)
            {
                e.Row.Cells[9].ToolTip = strNote;
                e.Row.Cells[9].Text = strNote.Substring(0, 25) + "...";
            }
            else
            {
                e.Row.Cells[9].ToolTip = strNote;
                e.Row.Cells[9].Text = strNote;

            }

        }

    }


    
    private void GetData(int nDirectId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var price_detail = from p in _db.pricing_details
                           join lc in _db.locations on p.location_id equals lc.location_id
                           where (from clc in _db.customer_locations
                                  where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                  select clc.location_id).Contains(p.location_id) &&
                                  (from cs in _db.customer_sections
                                   where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                   select cs.section_id).Contains(p.section_level)
                                   && p.is_direct == nDirectId && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && p.pricing_type == "A"
                           orderby p.execution_unit, lc.location_name ascending

                           select new PricingDetailModel()
                           {
                               pricing_id = (int)p.pricing_id,
                               item_id = (int)p.item_id,
                               labor_id = (int)p.labor_id,
                               section_serial = (decimal)p.section_serial,
                               section_name = p.section_name,
                               location_name = lc.location_name,
                               item_name = p.item_name,
                               measure_unit = p.measure_unit,
                               item_cost = (decimal)p.item_cost,
                               total_retail_price = (decimal)p.total_retail_price,
                               total_direct_price = (decimal)p.total_direct_price,
                               minimum_qty = (decimal)p.minimum_qty,
                               quantity = (decimal)p.quantity,
                               retail_multiplier = (decimal)p.retail_multiplier,
                               labor_rate = (decimal)p.labor_rate,
                               short_notes = p.short_notes,
                               tmpCol = string.Empty,
                               pricing_type = "A",
                               execution_unit = (decimal)p.execution_unit
                           };
        if (nDirectId == 1)
        {
            grdSelectedItem1.DataSource = price_detail.ToList();
            grdSelectedItem1.DataKeyNames = new string[] { "pricing_id","section_name" };
            grdSelectedItem1.DataBind();
        }
        else
        {
            grdSelectedItem2.DataSource = price_detail.ToList();
            grdSelectedItem2.DataKeyNames = new string[] { "pricing_id", "section_name" };
            grdSelectedItem2.DataBind();
        }

    }
    

    protected void grdSelectedItem2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TextBox txtUnitExe1 = (TextBox)e.Row.FindControl("txtUnitExe1");
            //Label lblUnitExe1 = (Label)e.Row.FindControl("lblUnitExe1");
            int serial = e.Row.RowIndex + 1;
            if (Convert.ToDecimal(txtUnitExe1.Text) == 0)
            {
                txtUnitExe1.Text = serial.ToString();
                //lblUnitExe1.Text = serial.ToString();
            }
            string str = e.Row.Cells[9].Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 25)
            {
                e.Row.Cells[9].ToolTip = str;
                e.Row.Cells[9].Text = str.Substring(0, 25) + "...";
            }
            else
            {
                e.Row.Cells[9].ToolTip = str;
                e.Row.Cells[9].Text = str;

            }

        }
    }
   
    protected void grdSelectedItem_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView grdSelecterdItem = (GridView)sender;
        TextBox txtUnitExe = (TextBox)grdSelecterdItem.Rows[e.NewEditIndex].FindControl("txtUnitExe");
        //Label lblUnitExe = (Label)grdSelecterdItem.Rows[e.NewEditIndex].FindControl("lblUnitExe");

        txtUnitExe.Visible = true;
        //lblUnitExe.Visible = false;
        LinkButton btn = (LinkButton)grdSelecterdItem.Rows[e.NewEditIndex].Cells[11].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";
    }
    protected void grdSelectedItem2_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridView grdSelectedItem2 = (GridView)sender;
        TextBox txtUnitExe1 = (TextBox)grdSelectedItem2.Rows[e.NewEditIndex].FindControl("txtUnitExe1");
        //Label lblUnitExe1 = (Label)grdSelectedItem2.Rows[e.NewEditIndex].FindControl("lblUnitExe1");
        txtUnitExe1.Visible = true;
        // lblUnitExe1.Visible = false;
        LinkButton btn = (LinkButton)grdSelectedItem2.Rows[e.NewEditIndex].Cells[11].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";
    }

    protected void grdSelectedItem1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        lblResult1.Text = "";

        GridView grdSelectedItem = (GridView)sender;
        DataClassesDataContext _db = new DataClassesDataContext();
        TextBox txtUnitExe = (TextBox)grdSelectedItem.Rows[e.RowIndex].FindControl("txtUnitExe");
        int nPricingId = Convert.ToInt32(grdSelectedItem.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
        string strQ = "UPDATE pricing_details SET execution_unit='" + txtUnitExe.Text.Replace("'", "''") + "' WHERE pricing_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        _db.ExecuteCommand(strQ, string.Empty);



    

        hdnPricingId.Value = "0";

        lblResult1.Text = "Item updated successfully";
        lblResult1.ForeColor = System.Drawing.Color.Green;

    }
    protected void grdSelectedItem2_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

        lblResult1.Text = "";

        GridView grdSelectedItem2 = (GridView)sender;
        DataClassesDataContext _db = new DataClassesDataContext();
        TextBox txtUnitExe1 = (TextBox)grdSelectedItem2.Rows[e.RowIndex].FindControl("txtUnitExe1");
        int nPricingId = Convert.ToInt32(grdSelectedItem2.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
        string strQ = "UPDATE pricing_details SET execution_unit='" + txtUnitExe1.Text.Replace("'", "''") + "' WHERE pricing_id =" + nPricingId + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        _db.ExecuteCommand(strQ, string.Empty);



        hdnPricingId.Value = "0";
        lblResult1.Text = "Item updated successfully";
        lblResult1.ForeColor = System.Drawing.Color.Green;
    }

    protected void btnSaveUnit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSaveUnit.ID, btnSaveUnit.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
       
            foreach (GridViewRow di in grdSelectedItem1.Rows)
            {
                pricing_detail pd = new pricing_detail();
                int nPricingId = Convert.ToInt32(grdSelectedItem1.DataKeys[Convert.ToInt32(di.RowIndex)].Values[0]);
                TextBox txtUnitExe = (TextBox)grdSelectedItem1.Rows[di.RowIndex].FindControl("txtUnitExe");

                if (Convert.ToInt32(hdnEstimateId.Value) > 0 && Convert.ToInt32(hdnCustomerId.Value) > 0)
                    pd = _db.pricing_details.Single(l => l.pricing_id == nPricingId && l.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && l.customer_id == Convert.ToInt32(hdnCustomerId.Value));


                pd.execution_unit = Convert.ToDecimal(txtUnitExe.Text);

            }
       
       
            foreach (GridViewRow di in grdSelectedItem2.Rows)
            {
                pricing_detail pd = new pricing_detail();
                int nPricingId = Convert.ToInt32(grdSelectedItem2.DataKeys[Convert.ToInt32(di.RowIndex)].Values[0]);
                TextBox txtUnitExe1 = (TextBox)grdSelectedItem2.Rows[di.RowIndex].FindControl("txtUnitExe1");

                if (Convert.ToInt32(hdnEstimateId.Value) > 0 && Convert.ToInt32(hdnCustomerId.Value) > 0)
                    pd = _db.pricing_details.Single(l => l.pricing_id == nPricingId && l.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && l.customer_id == Convert.ToInt32(hdnCustomerId.Value));


                pd.execution_unit = Convert.ToDecimal(txtUnitExe1.Text);

            }
      
        _db.SubmitChanges();
       
        lblResult1.Text = "Unit of Execution Saved successfully";
        lblResult1.ForeColor = System.Drawing.Color.Green;
        lblResult2.Text = "Unit of Execution Saved successfully";
        lblResult2.ForeColor = System.Drawing.Color.Green;


    }
    protected void btnGotoPricing_Click(object sender, EventArgs e)
    {
        Response.Redirect("sold_estimate.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value);
    }

    protected void AddMergedCells(GridViewRow objgridviewrow,TableCell objtablecell, int colspan, string celltext, string backcolor)
    {

        objtablecell = new TableCell();

        objtablecell.Text = celltext;

        objtablecell.ColumnSpan = colspan;

        objtablecell.Style.Add("background-color", backcolor);

        objtablecell.HorizontalAlign = HorizontalAlign.Center;

        objgridviewrow.Cells.Add(objtablecell);

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("sold_estimate.aspx");
    }
    
}