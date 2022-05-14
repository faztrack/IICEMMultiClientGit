using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PreconstructionCheckList : System.Web.UI.Page
{
    private double subtotal = 0.0;
    private double grandtotal = 0.0;

    decimal tax_amount = Convert.ToDecimal(0.00);
    decimal nTaxRate = Convert.ToDecimal(0.00);

    private double subtotal_diect = 0.0;
    private double grandtotal_direct = 0.0;
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        DateTime end = (DateTime)Session["loadstarttime"];
        TimeSpan loadtime = DateTime.Now - end;
        lblLoadTime.Text = (Math.Round(Convert.ToDecimal(loadtime.TotalSeconds), 3).ToString()) + " Sec";
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Add("loadstarttime", DateTime.Now);
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
            if (Page.User.IsInRole("admin044") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            BindSuperintendent();
            btnUpdateSuperintendent.Visible = false;
            int nCid = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCid.ToString();

            Session.Add("CustomerId", hdnCustomerId.Value);

            int nEstid = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstid.ToString();

            DataClassesDataContext _db = new DataClassesDataContext();
            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                if (_db.change_order_pricing_lists.Any(ch => ch.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ch.estimate_id == Convert.ToInt32(hdnEstimateId.Value)))
                {
                    hdnCOOrderExist.Value = "1"; // Set value 1 
                }
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                string strAddress = "";
                strAddress = cust.address + " </br>" + cust.city + ", " + cust.state + " " + cust.zip_code;
                lblAddress.Text = strAddress;
                lblEmail.Text = cust.email;
                lblPhone.Text = cust.phone;
                if (cust.SuperintendentId != null && cust.SuperintendentId != 0)
                {
                    string strSuperintendent = "";
                    ListItem dditem = ddlSuperintendent.Items.FindByValue(cust.SuperintendentId.ToString());
                    if (dditem != null)
                        this.ddlSuperintendent.Items.FindByValue(cust.SuperintendentId.ToString()).Selected = true;
                    else
                    {
                        user_info uinfo = _db.user_infos.Single(u => u.user_id == cust.SuperintendentId);
                        if (uinfo != null)
                        {
                            strSuperintendent = uinfo.first_name + " " + uinfo.last_name;
                            ddlSuperintendent.Items.Insert(0, new ListItem(strSuperintendent, cust.SuperintendentId.ToString()));
                        }
                    }
                }

                int nSalesPersonId = Convert.ToInt32(cust.sales_person_id);

                if (_db.sales_persons.Any(c => c.sales_person_id == nSalesPersonId && c.sales_person_id > 0))
                {
                    sales_person sp_info = new sales_person();
                    sp_info = _db.sales_persons.Single(c => c.sales_person_id == Convert.ToInt32(nSalesPersonId));
                    lblSalesPerson.Text = sp_info.first_name + " " + sp_info.last_name;
                }

                //hypGoogleMap.NavigateUrl = "GoogleMap.aspx?strAdd=" + strAddress.Replace("</br>", "");
                string address = cust.address + ",+" + cust.city + ",+" + cust.state + ",+" + cust.zip_code;
                hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;

                estimate_payment est_pay = new estimate_payment();

                est_pay = _db.estimate_payments.Where(ep => ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).FirstOrDefault();
                if (est_pay != null)
                {
                    if (est_pay.tax_rate != null)
                    {
                        lblTax.Text = est_pay.tax_rate.ToString();
                    }
                }
                if (lblTax.Text != "")
                {
                    if (Convert.ToDecimal(lblTax.Text) == 0)
                    {
                        lblTax.Visible = false;
                        lblTax_label.Visible = false;

                    }
                    else
                    {
                        lblTax.Visible = true;
                        lblTax_label.Visible = true;
                    }
                }
                else
                {
                    lblTax.Visible = false;
                    lblTax_label.Visible = false;
                }


                // Get Estimate Info
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                   // hdnFinanceValue.Value = Convert.ToDecimal(cus_est.FinancePer).ToString();
                  //  hdnIsCash.Value = Convert.ToBoolean(cus_est.IsCashTerm).ToString();
                    //lblJobNumber.Text = cus_est.job_number;
                    if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                        lblJobNumber.Text = cus_est.job_number;
                    else
                        lblJobNumber.Text = cus_est.alter_job_number;
                    lblEstimateName.Text = cus_est.estimate_name;
                    ddlStatus.SelectedValue = cus_est.status_id.ToString();
                    ddlStatus.Enabled = false;
                    lblSaleDate.Visible = true;
                    txtSaleDate.Text = Convert.ToDateTime(cus_est.sale_date).ToShortDateString();
                    txtSaleDate.Visible = true;
                    txtSaleDate.ReadOnly = true;
                    if (cus_est.estimate_comments != null)
                        txtComments.Text = cus_est.estimate_comments.Replace("&nbsp;", "");
                    else
                        txtComments.Text = "";
                    txtComments.ReadOnly = true;

                    if (Convert.ToBoolean(cus_est.IsEstimateActive) == false)
                    {
                        rdbEstimateIsActive.SelectedValue = "0";
                    }
                    else
                    {
                        rdbEstimateIsActive.SelectedValue = "1";

                    }

                    if ((cus_est.job_number ?? "").Length > 0)
                    {
                       // lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                        if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                            lblTitelJobNumber.Text = " ( Job Number: " + cus_est.job_number + " )";
                        else
                            lblTitelJobNumber.Text = " ( Job Number: " + cus_est.alter_job_number + " )";
                    }
                }


                BindSelectedItemGrid();
                BindSelectedItemGrid_Direct();

                if (grdGroupingDirect.Rows.Count == 0)
                {
                    lblDirectPricingHeader.Visible = false;
                }
                else
                {
                    lblDirectPricingHeader.Visible = true;
                }
                Calculate_Total();
                if (grdGrouping.Rows.Count == 0 && grdGroupingDirect.Rows.Count == 0)
                {

                    tblTotalProjectPrice.Visible = false;

                    if (grdGroupingDirect.Rows.Count == 0)
                    {
                        lblDirectPricingHeader.Visible = false;
                    }
                    else
                    {
                        lblDirectPricingHeader.Visible = true;
                    }
                }
                else
                {
                    tblTotalProjectPrice.Visible = true;
                    if (grdGroupingDirect.Rows.Count == 0)
                    {
                        lblDirectPricingHeader.Visible = false;
                    }
                    else
                    {
                        lblDirectPricingHeader.Visible = true;
                    }
                }
            }


            csCommonUtility.SetPagePermission(this.Page, new string[] { "rdbEstimateIsActive", "ddlStatus", "ddlSuperintendent", "ddlSuperintendentTwo", "ddlDesigner", "ddlArchitect", "btnSave", "btnEmailFlagItem" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "grdGrouping_lnkAddLocNotes", "chkFlagItem" });
        }
    }

    protected void btnGotoCustomerList_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnGotoCustomerList.ID, btnGotoCustomerList.GetType().Name, "Click");
        Response.Redirect("customerlist.aspx");
    }
    private void BindSuperintendent()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "select first_name+' '+last_name AS Superintendent_name,user_id from user_info WHERE role_id = 4";
        List<userinfo> mList = _db.ExecuteQuery<userinfo>(strQ, string.Empty).ToList();
        ddlSuperintendent.DataSource = mList;
        ddlSuperintendent.DataTextField = "Superintendent_name";
        ddlSuperintendent.DataValueField = "user_id";
        ddlSuperintendent.DataBind();
        ddlSuperintendent.Items.Insert(0, "Select");
    }
    private decimal GetRetailTotal()
    {
        decimal dRetail = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        if (hdnCOOrderExist.Value == "1")
        {
            var result = (from pd in _db.co_pricing_masters
                          where (from clc in _db.changeorder_locations
                                 where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                 select clc.location_id).Contains(pd.location_id) &&
                                 (from cs in _db.changeorder_sections
                                  where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                  select cs.section_id).Contains(pd.section_level) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.item_status_id == 1 && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                          select pd.total_retail_price);
            int n = result.Count();
            if (result != null && n > 0)
                dRetail = (decimal)result.Sum();

            //if (!Convert.ToBoolean(hdnIsCash.Value))
            //{
            //    decimal nFinanceAmountRetail = 0;
            //    if (Convert.ToDecimal(hdnFinanceValue.Value) > 0)
            //    {
            //        nFinanceAmountRetail = Convert.ToDecimal(dRetail * Convert.ToDecimal(hdnFinanceValue.Value) / 100);
            //        dRetail = dRetail + nFinanceAmountRetail;
            //    }
            //}
        }
        else
        {
            var result = (from pd in _db.pricing_details
                          where (from clc in _db.customer_locations
                                 where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                 select clc.location_id).Contains(pd.location_id) &&
                                 (from cs in _db.customer_sections
                                  where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                  select cs.section_id).Contains(pd.section_level) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A"
                          select pd.total_retail_price);
            int n = result.Count();
            if (result != null && n > 0)
                dRetail = result.Sum();

            //if (!Convert.ToBoolean(hdnIsCash.Value))
            //{
            //    decimal nFinanceAmountRetail = 0;
            //    if (Convert.ToDecimal(hdnFinanceValue.Value) > 0)
            //    {
            //        nFinanceAmountRetail = Convert.ToDecimal(dRetail * Convert.ToDecimal(hdnFinanceValue.Value) / 100);
            //        dRetail = dRetail + nFinanceAmountRetail;
            //    }
            //}
        }

        return dRetail;
    }
    private decimal GetDirctTotal()
    {
        decimal dDirect = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        if (hdnCOOrderExist.Value == "1")
        {
            var result = (from pd in _db.co_pricing_masters
                          where (from clc in _db.changeorder_locations
                                 where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                 select clc.location_id).Contains(pd.location_id) &&
                                 (from cs in _db.changeorder_sections
                                  where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                  select cs.section_id).Contains(pd.section_level) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.item_status_id == 1 && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                          select pd.total_direct_price);
            int n = result.Count();
            if (result != null && n > 0)
                dDirect = result.Sum();

            //if (!Convert.ToBoolean(hdnIsCash.Value))
            //{
            //    decimal nFinanceAmountRetail = 0;
            //    if (Convert.ToDecimal(hdnFinanceValue.Value) > 0)
            //    {
            //        nFinanceAmountRetail = Convert.ToDecimal(dDirect * Convert.ToDecimal(hdnFinanceValue.Value) / 100);
            //        dDirect = dDirect + nFinanceAmountRetail;
            //    }
            //}
        }
        else
        {
            var result = (from pd in _db.pricing_details
                          where (from clc in _db.customer_locations
                                 where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                 select clc.location_id).Contains(pd.location_id) &&
                                 (from cs in _db.customer_sections
                                  where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                  select cs.section_id).Contains(pd.section_level) && pd.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pd.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pd.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && pd.pricing_type == "A"
                          select pd.total_direct_price);
            int n = result.Count();
            if (result != null && n > 0)
                dDirect = result.Sum();

            //if (!Convert.ToBoolean(hdnIsCash.Value))
            //{
            //    decimal nFinanceAmountRetail = 0;
            //    if (Convert.ToDecimal(hdnFinanceValue.Value) > 0)
            //    {
            //        nFinanceAmountRetail = Convert.ToDecimal(dDirect * Convert.ToDecimal(hdnFinanceValue.Value) / 100);
            //        dDirect = dDirect + nFinanceAmountRetail;
            //    }
            //}
        }

        return dDirect;
        
    }
    public void BindSelectedItemGrid()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        string strQ = string.Empty;
        if (hdnCOOrderExist.Value == "1")
        {
            strQ = "select DISTINCT co_pricing_master.location_id AS colId,'LOCATION: '+ location.location_name as colName,Max(ISNULL(sort_id,0)) AS sort_id, ISNULL(LocationNoteDetails.LocationNotes,'') AS LocationNotes, ISNULL(LocationNoteDetails.LocationNotesNew,'') AS LocationNotesNew  from co_pricing_master  " +
               " INNER JOIN location on location.location_id = co_pricing_master.location_id " +
              " LEFT OUTER JOIN LocationNoteDetails ON LocationNoteDetails.location_id = co_pricing_master.location_id AND LocationNoteDetails.estimate_id = co_pricing_master.estimate_id AND LocationNoteDetails.customer_id = co_pricing_master.customer_id " +
              " WHERE co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND co_pricing_master.item_status_id =1 AND co_pricing_master.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + "  GROUP BY co_pricing_master.location_id,location.location_name,LocationNoteDetails.LocationNotes,LocationNoteDetails.LocationNotesNew order by  sort_id asc";
        }
        else
        {
            strQ = "select DISTINCT pricing_details.location_id AS colId,'LOCATION: '+ location.location_name as colName,Max(ISNULL(sort_id,0)) AS sort_id, ISNULL(LocationNoteDetails.LocationNotes,'') AS LocationNotes, ISNULL(LocationNoteDetails.LocationNotesNew,'') AS LocationNotesNew  from pricing_details  " +
               " INNER JOIN location on location.location_id = pricing_details.location_id " +
              " LEFT OUTER JOIN LocationNoteDetails ON LocationNoteDetails.location_id = pricing_details.location_id AND LocationNoteDetails.estimate_id = pricing_details.estimate_id AND LocationNoteDetails.customer_id = pricing_details.customer_id " +
              " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
              " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=1 AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + "  GROUP BY pricing_details.location_id,location.location_name,LocationNoteDetails.LocationNotes,LocationNoteDetails.LocationNotesNew order by sort_id asc";
        }
               
        
        
        List<PricingMaster> mList = _db.ExecuteQuery<PricingMaster>(strQ, string.Empty).ToList();
                 grdGrouping.DataSource = mList;
                 grdGrouping.DataKeyNames = new string[] { "colId", "LocationNotes", "LocationNotesNew" };
                 grdGrouping.DataBind();

    }
    public void BindSelectedItemGrid_Direct()
    {
        DataClassesDataContext _db = new DataClassesDataContext();


     
        string strQ = string.Empty;
        if (hdnCOOrderExist.Value == "1")
        {
            strQ = "select DISTINCT co_pricing_master.location_id AS colId,'LOCATION: '+ location.location_name as colName,Max(ISNULL(sort_id,0)) AS sort_id, ISNULL(LocationNoteDetails.LocationNotes,'') AS LocationNotes,ISNULL(LocationNoteDetails.LocationNotesNew,'') AS LocationNotesNew  from co_pricing_master  " +
                    " INNER JOIN location on location.location_id = co_pricing_master.location_id " +
                   " LEFT OUTER JOIN LocationNoteDetails ON LocationNoteDetails.location_id = co_pricing_master.location_id AND LocationNoteDetails.estimate_id = co_pricing_master.estimate_id AND LocationNoteDetails.customer_id = co_pricing_master.customer_id " +
                       " where co_pricing_master.location_id IN (Select location_id from changeorder_locations WHERE changeorder_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                      " AND co_pricing_master.section_level IN (Select section_id from changeorder_sections WHERE changeorder_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND changeorder_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND changeorder_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                      " AND co_pricing_master.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND co_pricing_master.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND co_pricing_master.item_status_id=1 AND co_pricing_master.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + "  GROUP BY co_pricing_master.location_id,location.location_name,LocationNoteDetails.LocationNotes,LocationNoteDetails.LocationNotesNew order by sort_id asc";
        }
        else
        {
            strQ = "select DISTINCT pricing_details.location_id AS colId,'LOCATION: '+ location.location_name as colName,Max(ISNULL(sort_id,0)) AS sort_id, ISNULL(LocationNoteDetails.LocationNotes,'') AS LocationNotes,ISNULL(LocationNoteDetails.LocationNotesNew,'') AS LocationNotesNew  from pricing_details  " +
                   " INNER JOIN location on location.location_id = pricing_details.location_id " +
                  " LEFT OUTER JOIN LocationNoteDetails ON LocationNoteDetails.location_id = pricing_details.location_id AND LocationNoteDetails.estimate_id = pricing_details.estimate_id AND LocationNoteDetails.customer_id = pricing_details.customer_id " +
                      " where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                     " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                     " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct=2 AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " GROUP BY pricing_details.location_id,location.location_name,LocationNoteDetails.LocationNotes,LocationNoteDetails.LocationNotesNew order by sort_id asc";
        }
        List<PricingMaster> mList = _db.ExecuteQuery<PricingMaster>(strQ, string.Empty).ToList();
        grdGroupingDirect.DataSource = mList;
        grdGroupingDirect.DataKeyNames = new string[] { "colId", "LocationNotes", "LocationNotesNew" };
        grdGroupingDirect.DataBind();

    }
    public void Calculate_Total()
    {
        decimal direct = 0;
        decimal retail = 0;
        decimal grandtotal = 0;
        direct = GetDirctTotal();
        retail = GetRetailTotal();
        grandtotal = direct + retail;

        lblDirctTotalCost.Text = direct.ToString("c");
        lblRetailTotalCost.Text = retail.ToString("c");
        lblGrandTotalCost.Text = grandtotal.ToString("c");

    }

    protected void grdSelectedItem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdSelecterdItem = (GridView)sender;
            Label lblshort_notes = (Label)e.Row.FindControl("lblshort_notes");
            TextBox txtshort_notes = (TextBox)e.Row.FindControl("txtshort_notes");
            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
            string str = lblshort_notes.Text.Replace("&nbsp;", "");
            Label lblItemName = (Label)e.Row.FindControl("lblItemName");

            string strLocation = grdSelecterdItem.DataKeys[e.Row.RowIndex].Values[2].ToString();
            string strSection = grdSelecterdItem.DataKeys[e.Row.RowIndex].Values[3].ToString();

         

            int ncoPricingUd = Convert.ToInt32(grdSelecterdItem.DataKeys[e.Row.RowIndex].Values[0]);
            int nItemStatusId = Convert.ToInt32(grdSelecterdItem.DataKeys[e.Row.RowIndex].Values[6]);
            Label lblT_price1 = (Label)e.Row.Cells[7].FindControl("lblT_price1");
            Label lblTotal_price = (Label)e.Row.FindControl("lblTotal_price");
            Label lblDleted = (Label)e.Row.FindControl("lblDleted");
            
            decimal dTotalPrice = Convert.ToDecimal(grdSelecterdItem.DataKeys[e.Row.RowIndex].Values[4]);
            string IsItemFlag = grdSelecterdItem.DataKeys[e.Row.RowIndex].Values[7].ToString();
            CheckBox chkFlagItem = (CheckBox)e.Row.FindControl("chkFlagItem");
            if (IsItemFlag != null && IsItemFlag != "")
            {
                if (Convert.ToBoolean(IsItemFlag) == true)
                {
                    chkFlagItem.Checked = true;
                    // e.Row.BackColor = System.Drawing.Color.LightGreen;
                    e.Row.Attributes.CssStyle.Add("background", "#e6fff5");
                }

            }

            lblItemName.Text = strSection;
            if (str != "" && str.Length > 50)
            {
                txtshort_notes.Text = str;
                lblshort_notes.Text = str.Substring(0, 50) + "...";
                lblshort_notes.ToolTip = str;
                lnkOpen.Visible = true;

            }
            else
            {
                txtshort_notes.Text = str;
                lblshort_notes.Text = str;
                lnkOpen.Visible = false;

            }
           

            if (nItemStatusId == 2) //BABU
            {
                e.Row.Cells[0].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[1].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[2].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[3].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[4].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[5].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[6].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[7].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[8].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                lblT_price1.Text = "0.00";

            }
            else if (nItemStatusId == 3)
            {
                e.Row.Attributes.CssStyle.Add("color", "green");
                DataClassesDataContext _db = new DataClassesDataContext();
                decimal nAmount = 0;
                var result = (from ppi in _db.co_pricing_masters
                              where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.co_pricing_list_id == ncoPricingUd
                              select ppi.total_retail_price);
                int n = result.Count();
                if (result != null && n > 0)
                    nAmount = result.Sum();
                else
                {

                    lblT_price1.Text = "0.00";
                    lblDleted.Visible = true;
                    lblDleted.ForeColor = Color.Red;
                }
                
               
            }

        }

    }


    protected string GetTotalPrice()
    {
        decimal nGrandtotal = Convert.ToDecimal(grandtotal);

        if (lblTax.Text.ToString().Trim() == "")
            nTaxRate = Convert.ToDecimal(0.00);
        else
            nTaxRate = Convert.ToDecimal(lblTax.Text.ToString().Trim());

        tax_amount = Convert.ToDecimal(nGrandtotal * nTaxRate / 100);

        grandtotal = Convert.ToDouble(nGrandtotal + tax_amount);

        return "Total with Tax: " + grandtotal.ToString("c");
    }
    protected string GetTotalPriceDirect()
    {
        decimal nGrandtotal_direct = Convert.ToDecimal(grandtotal_direct);

        if (lblTax.Text.ToString().Trim() == "")
            nTaxRate = Convert.ToDecimal(0.00);
        else
            nTaxRate = Convert.ToDecimal(lblTax.Text.ToString().Trim());

        tax_amount = Convert.ToDecimal(nGrandtotal_direct * nTaxRate / 100);

        grandtotal_direct = Convert.ToDouble(nGrandtotal_direct + tax_amount);

        return "Total with Tax: " + grandtotal_direct.ToString("c");
    }
    protected void grdGrouping_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGrouping.DataKeys[e.Row.RowIndex].Values[0]);
            Label lblLocNote = (Label)e.Row.FindControl("lblLocNote");
            LinkButton lnkAddLocNotes = (LinkButton)e.Row.FindControl("lnkAddLocNotes");
            LinkButton lnkEditNotes = (LinkButton)e.Row.FindControl("lnkEditNotes");
            Label lblLocNoteAsSold = (Label)e.Row.FindControl("lblLocNoteAsSold");

            string strNewLocationNotes = grdGrouping.DataKeys[e.Row.RowIndex].Values[2].ToString();

            if (strNewLocationNotes.Length == 0)
            {
                lnkAddLocNotes.Text = "Add Notes";
                lnkEditNotes.Visible = false;
                lnkAddLocNotes.Visible = true;
                lblLocNote.Visible = false;

            }
            else
            {
                string str = strNewLocationNotes.Replace("&nbsp;", "");
                if (str.Length > 50)
                {
                    lblLocNote.ToolTip = str;
                    lblLocNote.Text = str.Substring(0, 50) + "...";
                }
                else
                {
                    lblLocNote.ToolTip = str;
                    lblLocNote.Text = str;
                }
                lnkAddLocNotes.Visible = false;
                lnkEditNotes.Text = "(Edit Notes)";
                lnkEditNotes.Visible = true;
                lblLocNote.Visible = true;
            }

            string strLocationNotes = grdGrouping.DataKeys[e.Row.RowIndex].Values[1].ToString();
            if (strLocationNotes.Length > 0)
            {
                lblLocNoteAsSold.Visible = true;
                string str = strLocationNotes.Replace("&nbsp;", "");
                if (str.Length > 100)
                {
                    lblLocNoteAsSold.ToolTip = str;
                    lblLocNoteAsSold.Text = str.Substring(0, 100) + "...";
                }
                else
                {
                    lblLocNoteAsSold.ToolTip = str;
                    lblLocNoteAsSold.Text = str;
                }

            }
            else
            {
                lblLocNoteAsSold.Visible = false;
            }

            GridView gv = e.Row.FindControl("grdSelectedItem1") as GridView;
            int nDirectId = 1;
            GetData(colId, gv, nDirectId);
            GridViewRow footerRow = gv.FooterRow;
            GridViewRow headerRow = gv.HeaderRow;
            foreach (GridViewRow row in gv.Rows)
            {
                Label labelTotal = footerRow.FindControl("lblSubTotal") as Label;
                Label lblSubTotalLabel = footerRow.FindControl("lblSubTotalLabel") as Label;
                Label lblHeader = headerRow.FindControl("lblHeader") as Label;
                subtotal += Double.Parse((row.FindControl("lblT_price1") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                labelTotal.Text = subtotal.ToString("c");

                lblHeader.Text = "Section";
                lblSubTotalLabel.Text = "Sub Total:";
            }
            grandtotal += subtotal;
            subtotal = 0.0;
        }
    }
    private void GetData(int colId, GridView grd, int nDirectId)
    {
        try
        {


            DataClassesDataContext _db = new DataClassesDataContext();
            if (hdnCOOrderExist.Value == "1")
            {
                string strP = " select pricing_details.pricing_id AS co_pricing_list_id, pricing_details.item_id,  pricing_details.labor_id, pricing_details.section_serial," +
                         " location.location_name, pricing_details.section_name,  pricing_details.item_name,pricing_details.measure_unit,pricing_details.item_cost," +
                         " pricing_details.total_retail_price, pricing_details.total_direct_price,  pricing_details.minimum_qty,pricing_details.quantity," +
                         " pricing_details.retail_multiplier, pricing_details.labor_rate,pricing_details.short_notes,  pricing_details.short_notes_new," +
                         " 1 AS item_status_id, pricing_details.last_update_date, pricing_details.is_mandatory, pricing_details.is_direct," +
                         " pricing_details.section_level, location.location_id,0 AS co_pricing_item_id,IsItemFlag" +
                         " FROM pricing_details " +
                         " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                         " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                         " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND " +
                         " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                         " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                         " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND PD.client_id = 1 ) " +
                         " UNION ALL " +
                         " SELECT change_order_pricing_list.co_pricing_list_id,  change_order_pricing_list.item_id, 1 AS labor_id, change_order_pricing_list.section_serial," +
                             " location.location_name,change_order_pricing_list.section_name, change_order_pricing_list.item_name,  change_order_pricing_list.measure_unit,1 AS item_cost," +
                             " change_order_pricing_list.total_retail_price, change_order_pricing_list.total_direct_price, 1 AS minimum_qty,change_order_pricing_list.quantity," +
                             " 1 AS retail_multiplier,1 AS labor_rate,change_order_pricing_list.short_notes,change_order_pricing_list.short_notes_new, change_order_pricing_list.item_status_id," +
                             " change_order_pricing_list.last_update_date, 0 AS is_mandatory, change_order_pricing_list.is_direct, change_order_pricing_list.section_level, " +
                             " location.location_id,change_order_pricing_list.co_pricing_item_id as co_pricing_item_id,IsItemFlag" +
                             " FROM change_order_pricing_list " +
                             " INNER JOIN location on location.location_id = change_order_pricing_list.location_id " +
                             " INNER JOIN changeorder_estimate on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
                             " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and is_direct = " + nDirectId + " AND changeorder_estimate.change_order_status_id = 3 order by section_level";
                DataTable dt = csCommonUtility.GetDataTable(strP);

                if (nDirectId == 1)
                    Session.Add("Item_list", dt);
                else
                    Session.Add("Item_list_Direct", dt);
                DataView dv = dt.DefaultView;
                dv.RowFilter = "location_id =" + colId;
                dv.Sort = "last_update_date asc";
                grd.DataSource = dv;
                grd.DataKeyNames = new string[] { "co_pricing_list_id", "is_mandatory", "location_name", "section_name", "total_retail_price", "total_direct_price", "item_status_id", "IsItemFlag" };
                grd.DataBind();
            }
            else
            {
                var price_detail = from p in _db.pricing_details
                                   join lc in _db.locations on p.location_id equals lc.location_id
                                   where (from clc in _db.customer_locations
                                          where clc.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && clc.customer_id == Convert.ToInt32(hdnCustomerId.Value) && clc.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                          select clc.location_id).Contains(p.location_id) &&
                                          (from cs in _db.customer_sections
                                           where cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && cs.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cs.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                                           select cs.section_id).Contains(p.section_level) && p.location_id == colId && p.is_direct == nDirectId && p.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && p.customer_id == Convert.ToInt32(hdnCustomerId.Value) && p.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && p.pricing_type == "A"
                                   orderby p.section_level ascending

                                   select new CO_PricingDeatilModel()
                                   {
                                       co_pricing_list_id = (int)p.pricing_id,
                                       item_id = (int)p.item_id,
                                       labor_id = (int)p.labor_id,
                                       section_serial = (decimal)p.section_serial,
                                       location_name = lc.location_name,
                                       section_name = p.section_name,
                                       item_name = p.item_name,
                                       measure_unit = p.measure_unit,
                                       item_cost = (decimal)p.item_cost,
                                       total_retail_price = Convert.ToDecimal(p.total_retail_price),
                                       total_direct_price = Convert.ToDecimal(p.total_direct_price),
                                       minimum_qty = (decimal)p.minimum_qty,
                                       quantity = (decimal)p.quantity,
                                       retail_multiplier = (decimal)p.retail_multiplier,
                                       labor_rate = (decimal)p.labor_rate,
                                       short_notes = p.short_notes,
                                       short_notes_new = p.short_notes_new,
                                       item_status_id = 0,
                                       IsItemFlag = p.IsItemFlag.ToString(),
                                       co_pricing_item_id = 0,
                                       is_mandatory = false
                                   };
                // DataTable dtCust = csCommonUtility.LINQToDataTable(price_detail);
                grd.DataSource = price_detail.ToList().OrderBy(c => c.section_level).ThenBy(c => c.item_id);
                grd.DataKeyNames = new string[] { "co_pricing_list_id", "is_mandatory", "location_name", "section_name", "total_retail_price", "total_direct_price", "item_status_id", "IsItemFlag" };
                grd.DataBind();
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    private void GetData_New(int colId, GridView grd, int nDirectId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

      
        string strP = " SELECT  pricing_id as co_pricing_list_id, item_id, labor_id, section_serial, " +
                  " location.location_name,section_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
                 " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,1 as item_status_id,last_update_date,short_notes_new, is_mandatory, 0 as co_pricing_item_id, " +
                 " is_direct,section_level,location.location_id,'' as tmpCo " +
                 " FROM pricing_details " +
                 " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                 " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                 " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND " +
                  " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                 " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                  " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND is_direct = " + nDirectId + " AND PD.client_id = 1 ) " +
                 " order by section_level";
        DataTable dt = csCommonUtility.GetDataTable(strP);
        DataRow drNew = null;
        string strQ = " SELECT  co_pricing_list_id, item_id,1 as labor_id, section_serial, " +
                       " location.location_name,section_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
                       " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,item_status_id,last_update_date, " +
                       " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date,short_notes_new, change_order_pricing_list.co_pricing_item_id as co_pricing_item_id " +
                       " FROM change_order_pricing_list " +
                       " INNER JOIN location on location.location_id = change_order_pricing_list.location_id " +
                       " INNER JOIN changeorder_estimate on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
                       " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and is_direct = " + nDirectId + " AND changeorder_estimate.change_order_status_id = 3 order by section_level";
        DataTable dtcol = csCommonUtility.GetDataTable(strQ);
      
        foreach (DataRow dr in dtcol.Rows)
        {
           drNew = dt.NewRow();
            drNew["co_pricing_list_id"] = dr["co_pricing_list_id"];
            drNew["item_id"] = dr["item_id"];
            drNew["labor_id"] = dr["labor_id"];
            drNew["section_serial"] = dr["section_serial"];
            drNew["location_name"] = dr["location_name"];
            drNew["section_name"] = dr["section_name"];
            drNew["item_name"] = dr["item_name"];
            drNew["measure_unit"] = dr["measure_unit"];
            drNew["item_cost"] = dr["item_cost"];
            drNew["total_retail_price"] = dr["total_retail_price"];
            drNew["total_direct_price"] = dr["total_direct_price"];
            drNew["minimum_qty"] = dr["minimum_qty"];
            drNew["quantity"] = dr["quantity"];
            drNew["retail_multiplier"] = dr["retail_multiplier"];
            drNew["labor_rate"] = dr["labor_rate"];
            drNew["short_notes"] = dr["short_notes"];
            drNew["item_status_id"] = dr["item_status_id"];
            drNew["last_update_date"] = dr["last_update_date"];
            drNew["is_direct"] = dr["is_direct"];
            drNew["section_level"] = dr["section_level"];
            drNew["location_id"] = dr["location_id"];
            drNew["short_notes_new"] = dr["short_notes_new"];
            drNew["co_pricing_item_id"] = dr["co_pricing_item_id"];
          
            dt.Rows.Add(drNew);
          
        }
        if (nDirectId == 1)
            Session.Add("Item_list", dt);
        else
            Session.Add("Item_list_Direct", dt);
        DataView dv = dt.DefaultView;
        dv.RowFilter = "location_id =" + colId;
     
        
        dv.Sort = "last_update_date asc";
       
        grd.DataSource = dv;
        grd.DataKeyNames = new string[] { "co_pricing_list_id", "is_mandatory", "location_name", "section_name", "total_retail_price", "total_direct_price", "item_status_id"};
        grd.DataBind();

    }


    protected void grdSelectedItem2_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdSelectedItem2 = (GridView)sender;
            Label lblshort_notes1 = (Label)e.Row.FindControl("lblshort_notes1");
            TextBox txtshort_notes1 = (TextBox)e.Row.FindControl("txtshort_notes1");
            LinkButton lnkOpen1 = (LinkButton)e.Row.FindControl("lnkOpen1");
            Label lblItemName2 = (Label)e.Row.FindControl("lblItemName2");

            string strLocation = grdSelectedItem2.DataKeys[e.Row.RowIndex].Values[2].ToString();
            string strSection = grdSelectedItem2.DataKeys[e.Row.RowIndex].Values[3].ToString();

            int nItemStatusId = Convert.ToInt32(grdSelectedItem2.DataKeys[e.Row.RowIndex].Values[6]);
            int ncoPricingUd = Convert.ToInt32(grdSelectedItem2.DataKeys[e.Row.RowIndex].Values[0]);
            Label lblT_price2 = (Label)e.Row.Cells[7].FindControl("lblT_price2");
            Label lblTotal_price2 = (Label)e.Row.FindControl("lblTotal_price2");
            Label lblDleted1 = (Label)e.Row.FindControl("lblDleted1");

            

            decimal dDirectPrice = Convert.ToDecimal(grdSelectedItem2.DataKeys[e.Row.RowIndex].Values[5]);
            //if (!Convert.ToBoolean(hdnIsCash.Value))
            //{
            //    decimal nFintRetail = 0;
            //    decimal nFinanceAmountRetail = 0;
            //    if (Convert.ToDecimal(hdnFinanceValue.Value) > 0)
            //    {
            //        nFinanceAmountRetail = Convert.ToDecimal(dDirectPrice * Convert.ToDecimal(hdnFinanceValue.Value) / 100);

            //    }
            //    nFintRetail = dDirectPrice + nFinanceAmountRetail;
            //    lblTotal_price2.Text = nFintRetail.ToString("c");
            //    lblT_price2.Text = nFintRetail.ToString("c");
                
            //}

            lblItemName2.Text = strSection;
            string str = lblshort_notes1.Text.Replace("&nbsp;", "");
            if (str != "" && str.Length > 50)
            {
                txtshort_notes1.Text = str;
                lblshort_notes1.Text = str.Substring(0, 50) + "...";
                lblshort_notes1.ToolTip = str;
                lnkOpen1.Visible = true;

            }
            else
            {
                txtshort_notes1.Text = str;
                lblshort_notes1.Text = str;
                lnkOpen1.Visible = false;

            }


            if (nItemStatusId == 2) //BABU
            {
                e.Row.Cells[0].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[1].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[2].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[3].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[4].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[5].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[6].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                e.Row.Cells[7].Attributes.CssStyle.Add("text-decoration", "line-through ; color: red ;");
                lblT_price2.Text = "0.00";

            }
            else if (nItemStatusId == 3)
            {
                e.Row.Attributes.CssStyle.Add("color", "green");
                DataClassesDataContext _db = new DataClassesDataContext();
                decimal nAmount = 0;
                var result = (from ppi in _db.co_pricing_masters
                              where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.co_pricing_list_id == ncoPricingUd
                              select ppi.total_direct_price);
                int n = result.Count();
                if (result != null && n > 0)
                    nAmount = result.Sum();
                else
                {
                    lblT_price2.Text = "0.00";
                    lblDleted1.Visible = true;
                    lblDleted1.ForeColor = Color.Red;
                }
               
            }
           

        }
    }
    protected void grdGroupingDirect_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            int colId = Convert.ToInt32(grdGroupingDirect.DataKeys[e.Row.RowIndex].Values[0]);
            int nDirectId = 2;

            GridView gv = e.Row.FindControl("grdSelectedItem2") as GridView;
            GetData(colId, gv, nDirectId);
            GridViewRow footerRow = gv.FooterRow;
            GridViewRow headerRow = gv.HeaderRow;
            foreach (GridViewRow row in gv.Rows)
            {
                Label labelTotal2 = footerRow.FindControl("lblSubTotal2") as Label;
                Label lblSubTotalLabel2 = footerRow.FindControl("lblSubTotalLabel2") as Label;
                Label lblHeader2 = headerRow.FindControl("lblHeader2") as Label;
               
                subtotal_diect += Double.Parse((row.FindControl("lblT_price2") as Label).Text.Replace("$", "").Replace("(", "-").Replace(")", ""));
                labelTotal2.Text = subtotal_diect.ToString("c");
                lblHeader2.Text = "Section";
                lblSubTotalLabel2.Text = "Sub Total:";
            }
            grandtotal_direct += subtotal_diect;
            subtotal_diect = 0.0;
        }
    }
    protected void btnSchedule_Click(object sender, EventArgs e)
    {
        
        Response.Redirect("schedulecalendar.aspx?eid=" + hdnEstimateId.Value + "&cid=" + hdnCustomerId.Value + "&TypeID=1");


    }
    protected void ddlSuperintendent_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSuperintendent.ID, ddlSuperintendent.GetType().Name, "Change");
        if (ddlSuperintendent.SelectedItem.Text == "Select")
        {
            lblResult1.Text = "Superintendent is a required field";
            lblResult1.ForeColor = Color.Red;
            btnUpdateSuperintendent.Visible = false;
        }
        else
        {
            btnUpdateSuperintendent.Visible = true;
        }

    }
    protected void btnUpdateSuperintendent_Click(object sender, EventArgs e)
    {

        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpdateSuperintendent.ID, btnUpdateSuperintendent.GetType().Name, "Click");
        DataClassesDataContext _db = new DataClassesDataContext();
        int nSuperintendentId = 0;
        if (ddlSuperintendent.SelectedItem.Text != "Select")
        {
            nSuperintendentId = Convert.ToInt32(ddlSuperintendent.SelectedValue);
        }
        string strCustQ = "UPDATE customers SET  SuperintendentId =" + nSuperintendentId + "  WHERE  customer_id=" + Convert.ToInt32(hdnCustomerId.Value);
        _db.ExecuteCommand(strCustQ, string.Empty);
        lblResult1.Text = "Superintendent updated successfully";
        lblResult1.ForeColor = Color.Green;
        btnUpdateSuperintendent.Visible = false;
    }
    protected void lnkOpen_Click(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnsubmit.ID, btnsubmit.GetType().Name, "Click");
        //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert(' " + btnsubmit.CommandArgument + "');", true);
        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblshort_notes = gRow.Cells[0].Controls[0].FindControl("lblshort_notes") as Label;
        Label lblshort_notes_r = gRow.Cells[0].Controls[1].FindControl("lblshort_notes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[0].Controls[2].FindControl("lnkOpen") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblshort_notes.Visible = false;
            lblshort_notes_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblshort_notes.Visible = true;
            lblshort_notes_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";

        }

    }
    protected void lnkOpen1_Click(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnsubmit.ID, btnsubmit.GetType().Name, "Click");
        //ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert(' " + btnsubmit.CommandArgument + "');", true);
        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblshort_notes1 = gRow.Cells[0].Controls[0].FindControl("lblshort_notes1") as Label;
        Label lblshort_notes1_r = gRow.Cells[0].Controls[1].FindControl("lblshort_notes1_r") as Label;
        LinkButton lnkOpen1 = gRow.Cells[0].Controls[2].FindControl("lnkOpen1") as LinkButton;

        if (lnkOpen1.Text == "More")
        {
            lblshort_notes1.Visible = false;
            lblshort_notes1_r.Visible = true;
            lnkOpen1.Text = " Less";
            lnkOpen1.ToolTip = "Click here to view less";
        }
        else
        {
            lblshort_notes1.Visible = true;
            lblshort_notes1_r.Visible = false;
            lnkOpen1.Text = "More";
            lnkOpen1.ToolTip = "Click here to view more";

        }

    }
    protected void rdbEstimateIsActive_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdbEstimateIsActive.ID, rdbEstimateIsActive.GetType().Name, "Change");
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            string strQ = "UPDATE customer_estimate SET IsEstimateActive = " + rdbEstimateIsActive.SelectedValue +
               " WHERE estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) +
               " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) +
               " AND client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            _db.ExecuteCommand(strQ, string.Empty);

            if (_db.ScheduleCalendars.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value)).Count() > 0)
            {
                string sSql = "UPDATE ScheduleCalendar SET IsEstimateActive=" + rdbEstimateIsActive.SelectedValue + " WHERE estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value);
                _db.ExecuteCommand(sSql, string.Empty);
            }
            if (_db.ScheduleCalendarTemps.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value)).Count() > 0)
            {
                string sSql = "UPDATE ScheduleCalendartemp SET IsEstimateActive=" + rdbEstimateIsActive.SelectedValue + " WHERE estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value);
                _db.ExecuteCommand(sSql, string.Empty);
            }

            if (rdbEstimateIsActive.SelectedValue == "1")
            {
                lblResult1.Text = csCommonUtility.GetSystemMessage("Project activated successfully.");
            }
            else
            {
                lblResult1.Text = csCommonUtility.GetSystemMessage("Project deactivated successfully.");
            }
        }
        catch (Exception ex)
        {
            lblResult1.Text = ex.Message;
            lblResult1.ForeColor = System.Drawing.Color.Red;
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click");
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            foreach (GridViewRow diMain in grdGrouping.Rows)
            {
                GridView grdSelectedItem = (GridView)diMain.FindControl("grdSelectedItem1");
                foreach (GridViewRow di in grdSelectedItem.Rows)
                {

                    int nPricingId = Convert.ToInt32(grdSelectedItem.DataKeys[Convert.ToInt32(di.RowIndex)].Values[0]);
                    int nStatusId = Convert.ToInt32(grdSelectedItem.DataKeys[Convert.ToInt32(di.RowIndex)].Values[6]);

                    Label lblCoItemId = (Label)grdSelectedItem.Rows[di.RowIndex].FindControl("lblCoTremId1");
                    string nco_pricing_item_id = lblCoItemId.Text;
                    TextBox txtshort_notes_New = (TextBox)grdSelectedItem.Rows[di.RowIndex].FindControl("txtshort_notes_New");
                    CheckBox chkFlagItem = (CheckBox)grdSelectedItem.Rows[di.RowIndex].FindControl("chkFlagItem");

                    if ((nStatusId == 2 || nStatusId == 3) && nco_pricing_item_id != null && nco_pricing_item_id != "0" && nco_pricing_item_id!="")
                    {
                       
                        change_order_pricing_list objCh = new change_order_pricing_list();
                        if (Convert.ToInt32(hdnEstimateId.Value) > 0 && Convert.ToInt32(hdnCustomerId.Value) > 0 && (nStatusId == 2 || nStatusId == 3))
                            objCh = _db.change_order_pricing_lists.Single(l => l.co_pricing_list_id == nPricingId && l.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && l.customer_id == Convert.ToInt32(hdnCustomerId.Value) && l.co_pricing_item_id == Convert.ToInt32(nco_pricing_item_id));
                           objCh.short_notes_new = txtshort_notes_New.Text;
                           if (chkFlagItem.Checked)
                           {
                               objCh.IsItemFlag = true;
                           }
                           else
                           {
                               objCh.IsItemFlag = false;
                           }
                           
                    }
                    else
                    {
                        pricing_detail pd = new pricing_detail();
                        if (Convert.ToInt32(hdnEstimateId.Value) > 0 && Convert.ToInt32(hdnCustomerId.Value) > 0)
                            pd = _db.pricing_details.Single(l => l.pricing_id == nPricingId && l.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && l.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                        pd.short_notes_new = txtshort_notes_New.Text;
                        if (chkFlagItem.Checked)
                        {
                            pd.IsItemFlag = true;
                        }
                        else
                        {
                            pd.IsItemFlag = false;
                        }
                       
                    }



                }
            }
            foreach (GridViewRow diDirect in grdGroupingDirect.Rows)
            {
                GridView grdSelectedItemDirect = (GridView)diDirect.FindControl("grdSelectedItem2");
                foreach (GridViewRow di in grdSelectedItemDirect.Rows)
                {

                    int nPricingId = Convert.ToInt32(grdSelectedItemDirect.DataKeys[Convert.ToInt32(di.RowIndex)].Values[0]);
                    int nStatusId = Convert.ToInt32(grdSelectedItemDirect.DataKeys[Convert.ToInt32(di.RowIndex)].Values[6]);
                    Label lblCoItemId = (Label)grdSelectedItemDirect.Rows[di.RowIndex].FindControl("lblCoTremId2");
                    string nco_pricing_item_id = lblCoItemId.Text;
                    TextBox txtshort_notes_New2 = (TextBox)grdSelectedItemDirect.Rows[di.RowIndex].FindControl("txtshort_notes_New2");


                    if ((nStatusId == 2 || nStatusId == 3) && nco_pricing_item_id != null && nco_pricing_item_id != "0" && nco_pricing_item_id!="")
                    {
                        
                        change_order_pricing_list objCh = new change_order_pricing_list();
                        if (Convert.ToInt32(hdnEstimateId.Value) > 0 && Convert.ToInt32(hdnCustomerId.Value) > 0 && (nStatusId == 2 || nStatusId == 3))

                            objCh = _db.change_order_pricing_lists.Single(l => l.co_pricing_list_id == nPricingId && l.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && l.customer_id == Convert.ToInt32(hdnCustomerId.Value) && l.co_pricing_item_id ==Convert.ToInt32(nco_pricing_item_id));
                           objCh.short_notes_new = txtshort_notes_New2.Text;
                           
                    }
                    else
                    {
                        pricing_detail pd = new pricing_detail();
                        if (Convert.ToInt32(hdnEstimateId.Value) > 0 && Convert.ToInt32(hdnCustomerId.Value) > 0)
                            pd = _db.pricing_details.Single(l => l.pricing_id == nPricingId && l.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && l.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                        pd.short_notes_new = txtshort_notes_New2.Text;
                        
                    }



                }
            }
            _db.SubmitChanges();
            BindSelectedItemGrid();
            BindSelectedItemGrid_Direct();

            lblResult1.Text = csCommonUtility.GetSystemMessage("Data Saved successfully");
            lblResult.Text = csCommonUtility.GetSystemMessage("Data Saved successfully");

        }
        catch(Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    protected void grdGrouping_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        if (e.CommandName == "AddNotes")
        {

            int index = Convert.ToInt32(e.CommandArgument);
            int nLocId = Convert.ToInt32(grdGrouping.DataKeys[index].Values[0]);
            hdnLocId.Value = nLocId.ToString();
            location loc = _db.locations.Single(l => l.location_id == nLocId && l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
            lblLocationName.Text = loc.location_name;
            if (_db.LocationNoteDetails.Where(ln => ln.location_id == nLocId && ln.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ln.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ln.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() == null)
            {
                txtLocationNotes.Text = "";
            }
            else
            {
                LocationNoteDetail lnd = new LocationNoteDetail();
                lnd = _db.LocationNoteDetails.Single(ln => ln.location_id == nLocId && ln.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ln.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ln.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

                txtLocationNotes.Text = lnd.LocationNotesNew.Replace("&nbsp;", "");
            }
            ModalPopupExtender5.Show();
        }
        else if (e.CommandName == "EditNotes")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            int nLocId = Convert.ToInt32(grdGrouping.DataKeys[index].Values[0]);
            hdnLocId.Value = nLocId.ToString();
            location loc = _db.locations.Single(l => l.location_id == nLocId && l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
            lblLocationName.Text = loc.location_name;
            if (_db.LocationNoteDetails.Where(ln => ln.location_id == nLocId && ln.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ln.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ln.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() == null)
            {
                txtLocationNotes.Text = "";
            }
            else
            {
                LocationNoteDetail lnd = new LocationNoteDetail();
                lnd = _db.LocationNoteDetails.Single(ln => ln.location_id == nLocId && ln.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ln.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ln.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

                txtLocationNotes.Text = lnd.LocationNotesNew.Replace("&nbsp;", "");
            }
            ModalPopupExtender5.Show();
        }




    }

    protected void btnLocMscSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnLocMscSubmit.ID, btnLocMscSubmit.GetType().Name, "Click");
        DataClassesDataContext _db = new DataClassesDataContext();
        int nLocNoteID = 0;

        if (_db.LocationNoteDetails.Where(ln => ln.location_id == Convert.ToInt32(hdnLocId.Value) && ln.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ln.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ln.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() == null)
        {
            nLocNoteID = 0;
        }
        else
        {
            LocationNoteDetail lnd = _db.LocationNoteDetails.Single(ln => ln.location_id == Convert.ToInt32(hdnLocId.Value) && ln.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ln.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ln.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
            nLocNoteID = lnd.LocationNotesId;
        }

        LocationNoteDetail lndNew = new LocationNoteDetail();
        if (nLocNoteID > 0)
            lndNew = _db.LocationNoteDetails.Single(ln => ln.LocationNotesId == nLocNoteID);
        lndNew.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
        lndNew.customer_id = Convert.ToInt32(hdnCustomerId.Value);
        lndNew.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
        lndNew.location_id = Convert.ToInt32(hdnLocId.Value);
        // lndNew.LocationNotes = txtLocationNotes.Text;
        lndNew.LocationNotesNew = txtLocationNotes.Text;
        lndNew.LastUpdateBy = User.Identity.Name;
        if (nLocNoteID == 0)
        {
            lndNew.CreateDate = DateTime.Now;
            _db.LocationNoteDetails.InsertOnSubmit(lndNew);

        }
        _db.SubmitChanges();

        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();

        if (grdGroupingDirect.Rows.Count == 0)
        {
            lblDirectPricingHeader.Visible = false;
        }
        else
        {
            lblDirectPricingHeader.Visible = true;
        }
        Calculate_Total();
    }
    protected void btnPrintCheckList_Click(object sender, EventArgs e)
    {
         KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrintCheckList.ID, btnPrintCheckList.GetType().Name, "Click");
        DataClassesDataContext _db = new DataClassesDataContext();
        decimal totalwithtax = 0;
        decimal project_subtotal = 0;
        decimal tax_amount = 0;
        string strLeadTime = "";
        string strContract_date = "";
        string strdate = "";
        if (_db.estimate_payments.Where(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])).SingleOrDefault() == null)
        {
            totalwithtax = 0;
        }
        else
        {
            estimate_payment esp = new estimate_payment();
            esp = _db.estimate_payments.Single(ep => ep.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ep.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ep.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));
            totalwithtax = Convert.ToDecimal(esp.new_total_with_tax);
            if (Convert.ToDecimal(esp.adjusted_price) > 0)
                project_subtotal = Convert.ToDecimal(esp.adjusted_price);
            else
                project_subtotal = Convert.ToDecimal(esp.project_subtotal);
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
        }

        //string strQ = " SELECT  pricing_id, pricing_details.client_id, pricing_details.customer_id, pricing_details.estimate_id, pricing_details.location_id, sales_person_id, section_level, item_id, section_name, item_name, measure_unit, item_cost, minimum_qty, quantity, retail_multiplier, labor_rate, labor_id, section_serial, item_cnt, total_direct_price, total_retail_price, is_direct, pricing_type, short_notes,location_name,short_notes_new,ISNULL(LocationNoteDetails.LocationNotes,'') AS LocationNotes, ISNULL(LocationNoteDetails.LocationNotesNew,'') AS LocationNotesNew  " +
        //            " FROM pricing_details  INNER JOIN location ON pricing_details.location_id=location.location_id AND pricing_details.client_id=location.client_id " +
        //            " LEFT OUTER JOIN LocationNoteDetails ON LocationNoteDetails.location_id = pricing_details.location_id AND LocationNoteDetails.estimate_id = pricing_details.estimate_id AND LocationNoteDetails.customer_id = pricing_details.customer_id " +
        //            " WHERE pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +

        //            " AND pricing_details.section_level IN (Select section_id from customer_sections  WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
        //            " AND pricing_details.estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND pricing_details.client_id=" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);

        //List<PricingDetailModel> CList = _db.ExecuteQuery<PricingDetailModel>(strQ, string.Empty).ToList();


        string strP = " SELECT  pricing_id, item_id, labor_id, section_serial,ISNULL(LocationNoteDetails.LocationNotes,'') AS LocationNotes, ISNULL(LocationNoteDetails.LocationNotesNew,'') AS LocationNotesNew, " +
                  " location.location_name,section_name,item_name,measure_unit,item_cost,total_retail_price,total_direct_price, " +
                 " minimum_qty,quantity,retail_multiplier,labor_rate,short_notes,1 as item_status_id,last_update_date,short_notes_new, is_mandatory, 0 as co_pricing_item_id, " +
                 " is_direct,section_level,location.location_id,'' as tmpCo " +
                 " FROM pricing_details " +
                 " LEFT OUTER JOIN LocationNoteDetails ON LocationNoteDetails.location_id = pricing_details.location_id AND LocationNoteDetails.estimate_id = pricing_details.estimate_id AND LocationNoteDetails.customer_id = pricing_details.customer_id " +
                 " INNER JOIN location on location.location_id = pricing_details.location_id where pricing_details.location_id IN (Select location_id from customer_locations WHERE customer_locations.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_locations.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_locations.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                 " AND pricing_details.section_level IN (Select section_id from customer_sections WHERE customer_sections.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND customer_sections.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND customer_sections.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " ) " +
                 " AND pricing_details.estimate_id =" + Convert.ToInt32(hdnEstimateId.Value) + " AND pricing_details.customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + "  AND pricing_details.client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) + " AND " +
                  " pricing_details.pricing_id  NOT IN (  SELECT PD.pricing_id FROM pricing_details PD INNER JOIN " +
                 " ( SELECT item_id,item_name,total_retail_price,short_notes,location_id FROM change_order_pricing_list cop INNER JOIN changeorder_estimate as ce   on ce.customer_id = cop.customer_id AND  ce.estimate_id = cop.estimate_id AND  ce.chage_order_id = cop.chage_order_id where ce.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + " AND ce.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " and ce.change_order_status_id = 3 AND cop.is_direct = 1 ) b ON b.item_id = PD.item_id AND b.item_name = PD.item_name AND b.total_retail_price = PD.total_retail_price AND b.short_notes = PD.short_notes AND b.location_id = PD.location_id " +
                  " where  PD.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + " AND PD.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND PD.client_id = 1 ) " +
                 " order by section_level";
        DataTable dtPRIC = csCommonUtility.GetDataTable(strP);
        DataRow drNew = null;
        string strQc = " SELECT  co_pricing_list_id as pricing_id, item_id,1 as labor_id, section_serial,ISNULL(LocationNoteDetails.LocationNotes,'') AS LocationNotes,ISNULL(LocationNoteDetails.LocationNotesNew,'') AS LocationNotesNew,  " +
                       " location.location_name,section_name,item_name,measure_unit,1 as item_cost,total_retail_price,total_direct_price, " +
                       " 1 as minimum_qty,quantity,1 as retail_multiplier,1 as labor_rate,short_notes,item_status_id,last_update_date, " +
                       " is_direct,section_level,location.location_id,changeorder_estimate.changeorder_name,changeorder_estimate.execute_date,short_notes_new, change_order_pricing_list.co_pricing_item_id as co_pricing_item_id " +
                       " FROM change_order_pricing_list " +
                       " INNER JOIN location on location.location_id = change_order_pricing_list.location_id " +
                       " LEFT OUTER JOIN LocationNoteDetails ON LocationNoteDetails.location_id = change_order_pricing_list.location_id AND LocationNoteDetails.estimate_id = change_order_pricing_list.estimate_id AND LocationNoteDetails.customer_id = change_order_pricing_list.customer_id " +
                       " INNER JOIN changeorder_estimate on changeorder_estimate.customer_id = change_order_pricing_list.customer_id AND  changeorder_estimate.estimate_id = change_order_pricing_list.estimate_id AND  changeorder_estimate.chage_order_id = change_order_pricing_list.chage_order_id " +
                       " where changeorder_estimate.customer_id = " + Convert.ToInt32(hdnCustomerId.Value) + "  AND changeorder_estimate.estimate_id = " + Convert.ToInt32(hdnEstimateId.Value) + "  AND changeorder_estimate.change_order_status_id = 3 order by section_level";
        DataTable dtcol = csCommonUtility.GetDataTable(strQc);

        foreach (DataRow dr in dtcol.Rows)
        {
            drNew = dtPRIC.NewRow();
            drNew["pricing_id"] = dr["pricing_id"];
            drNew["LocationNotes"] = dr["LocationNotes"];
            drNew["LocationNotesNew"] = dr["LocationNotesNew"];
            drNew["item_id"] = dr["item_id"];
            drNew["labor_id"] = dr["labor_id"];
            drNew["section_serial"] = dr["section_serial"];
            drNew["location_name"] = dr["location_name"];
            drNew["section_name"] = dr["section_name"];
            drNew["item_name"] = dr["item_name"];
            drNew["measure_unit"] = dr["measure_unit"];
            drNew["item_cost"] = dr["item_cost"];
            drNew["total_retail_price"] = dr["total_retail_price"];
            drNew["total_direct_price"] = dr["total_direct_price"];
            drNew["minimum_qty"] = dr["minimum_qty"];
            drNew["quantity"] = dr["quantity"];
            drNew["retail_multiplier"] = dr["retail_multiplier"];
            drNew["labor_rate"] = dr["labor_rate"];
            drNew["short_notes"] = dr["short_notes"];
            drNew["item_status_id"] = dr["item_status_id"];
            if(Convert.ToInt32(dr["item_status_id"]) == 3)
            {
                decimal nAmount = 0;
                var result = (from ppi in _db.co_pricing_masters
                              where ppi.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && ppi.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ppi.co_pricing_list_id == Convert.ToInt32(dr["pricing_id"])
                              select ppi.total_retail_price);
                int n = result.Count();
                if (result != null && n > 0)
                    nAmount = result.Sum();
                else
                {
                    drNew["item_status_id"] = 4;
                }
            }
            drNew["last_update_date"] = dr["last_update_date"];
            drNew["is_direct"] = dr["is_direct"];
            drNew["section_level"] = dr["section_level"];
            drNew["location_id"] = dr["location_id"];
            drNew["short_notes_new"] = dr["short_notes_new"];
            drNew["co_pricing_item_id"] = dr["co_pricing_item_id"];

            dtPRIC.Rows.Add(drNew);

        }


        if (dtPRIC.Rows.Count == 0)
        {
            return;
        }



        customer_estimate cus_est = new customer_estimate();
        cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));

        ReportDocument rptFile = new ReportDocument();

        string strReportPath = Server.MapPath(@"Reports\rpt\rptCheckListReport.rpt");

        rptFile.Load(strReportPath);
        rptFile.SetDataSource(dtPRIC);

        customer objCust = new customer();
        objCust = _db.customers.Single(cus => cus.customer_id == Convert.ToInt32(hdnCustomerId.Value));
        string strCustomerName2 = objCust.first_name2.ToString() + " " + objCust.last_name2.ToString();
        string strCustomerName = objCust.first_name1 + " " + objCust.last_name1;
        string strCustomerAddress = objCust.address;
        string strCustomerCity = objCust.city;
        string strCustomerState = objCust.state;
        string strCustomerZip = objCust.zip_code;
        string strCustomerPhone = objCust.phone;
        string strCustomerEmail = objCust.email;
        string strCustomerCompany = objCust.company;

        Hashtable ht = new Hashtable();
        ht.Add("p_CustomerName", strCustomerName);
        ht.Add("p_EstimateName", cus_est.estimate_name);
        ht.Add("p_status_id", cus_est.status_id);
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
        ht.Add("p_customername2", strCustomerName2);
        Session.Add(SessionInfo.Report_File, rptFile);
        Session.Add(SessionInfo.Report_Param, ht);

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
    }

    string CreateHtml()
    {


        int i = 0;
        string strHTML = "<br/> <br/>";
        strHTML += "<table width='1000' border='0' cellspacing='0'cellpadding='0' align='left'> <tr><td align='left' valign='top'><p style='color:#0066CC; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'>" + "" + "</p> <table width='100%' border='0' cellspacing='3' cellpadding='5' > <tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <tr style='background:#171f89;'></tr><tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <td width='15%'>As Sold Short Notes</td><td width='25%'>ITEM NAME</td><td width='15%'>LOCATION</td><td width='15%'>SECTION</td><td width='25%'>CHECKLIST NOTES</td><td width='10%'>UOM</td><td width='5%'>QTY</td><td width='10%'>EXT.PRICE</td></tr>";

        DataClassesDataContext _db = new DataClassesDataContext();
        foreach (GridViewRow diMain in grdGrouping.Rows)
        {
            GridView grdSelectedItem = (GridView)diMain.FindControl("grdSelectedItem1");
            Label lblLocationName = (Label)diMain.FindControl("Label1");
            string locationName = lblLocationName.Text.Substring(lblLocationName.Text.IndexOf(":")).Replace(":", "");
            foreach (GridViewRow di in grdSelectedItem.Rows)
            {

                int nPricingId = Convert.ToInt32(grdSelectedItem.DataKeys[Convert.ToInt32(di.RowIndex)].Values[0]);
                int nStatusId = Convert.ToInt32(grdSelectedItem.DataKeys[Convert.ToInt32(di.RowIndex)].Values[6]);

                Label lblCoItemId = (Label)grdSelectedItem.Rows[di.RowIndex].FindControl("lblCoTremId1");
                string nco_pricing_item_id = lblCoItemId.Text;
                TextBox txtshort_notes_New = (TextBox)grdSelectedItem.Rows[di.RowIndex].FindControl("txtshort_notes_New");
                CheckBox chkFlagItem = (CheckBox)grdSelectedItem.Rows[di.RowIndex].FindControl("chkFlagItem");

                Label lblshort_notes = (Label)grdSelectedItem.Rows[di.RowIndex].FindControl("lblshort_notes");

                string itemName = HttpUtility.HtmlDecode(di.Cells[1].Text).ToString();
                Label lblSectionName = (Label)grdSelectedItem.Rows[di.RowIndex].FindControl("lblItemName");
                string UOM = di.Cells[5].Text;

                string Qty = di.Cells[7].Text;
                Label lblTotal_price = (Label)grdSelectedItem.Rows[di.RowIndex].FindControl("lblTotal_price");


                if ((nStatusId == 2 || nStatusId == 3) && nco_pricing_item_id != null && nco_pricing_item_id != "0" && nco_pricing_item_id != "")
                {

                    change_order_pricing_list objCh = new change_order_pricing_list();
                    if (Convert.ToInt32(hdnEstimateId.Value) > 0 && Convert.ToInt32(hdnCustomerId.Value) > 0 && (nStatusId == 2 || nStatusId == 3))
                        objCh = _db.change_order_pricing_lists.Single(l => l.co_pricing_list_id == nPricingId && l.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && l.customer_id == Convert.ToInt32(hdnCustomerId.Value) && l.co_pricing_item_id == Convert.ToInt32(nco_pricing_item_id));
                    objCh.short_notes_new = txtshort_notes_New.Text;
                    if (chkFlagItem.Checked)
                    {
                        objCh.IsItemFlag = true;
                    }
                    else
                    {
                        objCh.IsItemFlag = false;
                    }

                }
                else
                {
                    pricing_detail pd = new pricing_detail();
                    if (Convert.ToInt32(hdnEstimateId.Value) > 0 && Convert.ToInt32(hdnCustomerId.Value) > 0)
                        pd = _db.pricing_details.Single(l => l.pricing_id == nPricingId && l.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && l.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                    pd.short_notes_new = txtshort_notes_New.Text;
                    if (chkFlagItem.Checked)
                    {
                        pd.IsItemFlag = true;
                    }
                    else
                    {
                        pd.IsItemFlag = false;
                    }

                }

                if (chkFlagItem.Checked)
                {

                    string strColor = "";
                    i++;
                    if (i % 2 == 0)
                        strColor = "background-color:#eee; color:#7d766b; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
                    else
                        strColor = "background-color:#faf8f6; color:#7d766b; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";

                    strHTML += "<tr style='" + strColor + "'><td>" + lblshort_notes.Text + "</td><td>" + itemName + "</td><td>" + locationName + "</td><td>" + lblSectionName.Text + "</td><td>" + txtshort_notes_New.Text + "</td><td>" + UOM + "</td><td>" + Qty + "</td><td>" + lblTotal_price.Text + "</td></tr>";


                }
            }



        }

        strHTML += "</table>";



        _db.SubmitChanges();
        BindSelectedItemGrid();
        BindSelectedItemGrid_Direct();


        if (i != 0)
            return strHTML;
        return "";


    }
    protected void btnEmailFlagItem_Click(object sender, EventArgs e)
    {

        string strMessage = CreateHtml();
        Session.Add("MessBody", strMessage);
        string url = "window.open('sendemailoutlook.aspx?custId=" + hdnCustomerId.Value + "&precon=a&eid=" + hdnEstimateId.Value + "', 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1')";
        ScriptManager.RegisterClientScriptBlock(this.Page, Page.GetType(), "MyWindow", url, true);

    }
}