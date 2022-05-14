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

public partial class salesperson : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Page.User.IsInRole("sales002") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            int nsid = Convert.ToInt32(Request.QueryString.Get("sid"));
            hdnSalesPersonId.Value = nsid.ToString();

            if (Convert.ToInt32(hdnSalesPersonId.Value) > 0)
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                sales_person sap = new sales_person();
                sap = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));

                lblSalesPersonName.Text = sap.first_name + " " + sap.last_name;

                string strAddress = "";
                strAddress = sap.address + " </br>" + sap.city + ", " + sap.state + " " + sap.zip;
                lblAddress.Text = strAddress;
                lblPhone.Text = sap.phone;
                lblEmail.Text = sap.email;
                
            }
            else
            {
                hdnSalesPersonId.Value = "0";                
            }
            this.Validate();
            GetCustomers();
        }

    }

   
    protected void GetCustomers()
    {
         DataClassesDataContext _db = new DataClassesDataContext();
        var item = from cus in _db.customers
                   where cus.status_id != 5 && cus.status_id != 4 && cus.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && cus.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                       orderby cus.registration_date descending, cus.last_name1 ascending
                       select cus;

           
            grdCustomerList.DataSource = item;
            grdCustomerList.DataBind();
    }
    
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("salespersonlist.aspx");
    }
    protected void grdCustomerList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int ncid = Convert.ToInt32(grdCustomerList.DataKeys[e.Row.RowIndex].Value.ToString());

            DataClassesDataContext _db = new DataClassesDataContext();

            // Customer Address
            customer cust = new customer();
            cust = _db.customers.Single(c => c.customer_id == ncid);
            string strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
            //e.Row.Cells[2].Text = strAddress;

            // Customer Email
            HyperLink hypEmail = (HyperLink)e.Row.Cells[3].FindControl("hypEmail");
            hypEmail.Text = cust.email;
            hypEmail.NavigateUrl = "mailto:" + cust.email + "?subject=Contact";

            // Customer Address in Google Map
            HyperLink hypAddress = (HyperLink)e.Row.FindControl("hypAddress");
            hypAddress.Text = strAddress;
            //hypAddress.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
            string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
            hypAddress.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;


            // Customer Status
            if (e.Row.Cells[6].Text.Trim() != "")
            {
                int nStatusId = Convert.ToInt32(e.Row.Cells[6].Text.Trim());
                if (nStatusId == 1)
                    e.Row.Cells[6].Text = "New";
                else if (nStatusId == 2)
                    e.Row.Cells[6].Text = "Follow-up";
                else if (nStatusId == 3)
                    e.Row.Cells[6].Text = "In-Design";
                else if (nStatusId == 4)
                    e.Row.Cells[6].Text = "Archive";
                else if (nStatusId == 5)
                    e.Row.Cells[6].Text = "Dead";
            }

            // Customer Estimates
            Table tdLink = (Table)e.Row.FindControl("tdLink");
            string strQ = "select * from customer_estimate where customer_id=" + ncid + " and client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            IEnumerable<customer_estimate_model> list = _db.ExecuteQuery<customer_estimate_model>(strQ, string.Empty);

            foreach (customer_estimate_model cus_est in list)
            {
                string strestimateName = cus_est.estimate_name;
                int nestid = Convert.ToInt32(cus_est.estimate_id);
                int nest_status_id = Convert.ToInt32(cus_est.status_id);

                TableRow row = new TableRow();
                TableCell cell = new TableCell();
                HyperLink hyp = new HyperLink();
                if (nest_status_id == 3)
                {
                    hyp.Text = strestimateName + " (SOLD) ";
                    //e.Row.Attributes.CssStyle.Add("text-decoration", "line-through");
                    //e.Row.Attributes.CssStyle.Add("color", "red");
                    //e.Row.BackColor = System.Drawing.Color.Green;

                    // Estimate Change Order
                    Table tblChangeOrder = (Table)e.Row.FindControl("tblChangeOrder");
                    string strQuery = "select * from changeorder_estimate where customer_id=" + ncid + " AND estimate_id = " + nestid + " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                    IEnumerable<changeorder_estimate> listItem = _db.ExecuteQuery<changeorder_estimate>(strQuery, string.Empty);
                    bool bFlag = true;
                    foreach (changeorder_estimate cho in listItem)
                    {
                        string strChangeOrderName = cho.changeorder_name;
                        int nChangeOrderId = Convert.ToInt32(cho.chage_order_id);
                        int est_status_id = Convert.ToInt32(cho.change_order_status_id);

                        TableRow row_ch = new TableRow();
                        TableCell cell_ch = new TableCell();
                        HyperLink hyp_ch = new HyperLink();

                        if (est_status_id == 3)
                        {
                            hyp_ch.Text = strChangeOrderName + "( Executed )";

                        }
                        else if (est_status_id == 4)
                        {
                            hyp_ch.Text = strChangeOrderName + "( Declined )";
                        }
                        else
                        {
                            hyp_ch.Text = strChangeOrderName;
                            bFlag = false;
                        }

                        hyp_ch.NavigateUrl = "change_order_locations.aspx?coestid=" + nChangeOrderId + "&eid=" + nestid + "&cid=" + ncid;
                        cell_ch.Controls.Add(hyp_ch);
                        cell_ch.HorizontalAlign = HorizontalAlign.Left;
                        row_ch.Cells.Add(cell_ch);
                        tblChangeOrder.Rows.Add(row_ch);
                    }
                    if (bFlag)
                    {
                        TableRow common_row = new TableRow();
                        TableCell common_cell = new TableCell();
                        HyperLink hyp_Common = new HyperLink();
                        hyp_Common.Text = "New Change Order";
                        common_cell.HorizontalAlign = HorizontalAlign.Left;
                        hyp_Common.NavigateUrl = "change_order_locations.aspx?eid=" + nestid + "&cid=" + ncid;
                        common_cell.Controls.Add(hyp_Common);
                        common_row.Cells.Add(common_cell);
                        tblChangeOrder.Rows.Add(common_row);
                    }
                }
                else
                    hyp.Text = strestimateName;

                hyp.NavigateUrl = "customer_locations.aspx?eid=" + nestid + "&cid=" + ncid;
                cell.Controls.Add(hyp);
                cell.HorizontalAlign = HorizontalAlign.Left;
                row.Cells.Add(cell);
                tdLink.Rows.Add(row);
            }
            TableRow commonrow = new TableRow();
            TableCell commoncell = new TableCell();
            HyperLink hypCommon = new HyperLink();
            hypCommon.Text = "New Estimate";
            commoncell.HorizontalAlign = HorizontalAlign.Left;
            hypCommon.NavigateUrl = "customer_locations.aspx?cid=" + ncid;
            commoncell.Controls.Add(hypCommon);
            commonrow.Cells.Add(commoncell);
            tdLink.Rows.Add(commonrow);
        }
    }
}
