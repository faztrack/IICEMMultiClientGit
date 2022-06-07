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
using System.Drawing;

public partial class incentivedetails : System.Web.UI.Page
{
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
            string divisionName = "";
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            
            if (Page.User.IsInRole("admin006") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
            int nId = Convert.ToInt32(Request.QueryString.Get("iid"));
            hdnIncentiveId.Value = nId.ToString();

            pnlDiscount.Visible = true;
            pnlAmount.Visible = false;

            BindDivision();

            

            if (Convert.ToInt32(hdnIncentiveId.Value) > 0)
            {
                lblHeaderTitle.Text = "Incentive Details";
                DataClassesDataContext _db = new DataClassesDataContext();
                incentive inc = new incentive();
                inc = _db.incentives.Single(i => i.incentive_id == Convert.ToInt32(hdnIncentiveId.Value));
                txtIncentiveName.Text = inc.incentive_name;
                txtDescription.Text = inc.incentive_desc;
                txtAmount.Text = inc.amount.ToString();
                txtDiscount.Text = inc.discount.ToString();
                chkActive.Checked = Convert.ToBoolean(inc.is_active);
                txtStartDate.Text = Convert.ToDateTime(inc.start_date).ToShortDateString();
                txtEndDate.Text = Convert.ToDateTime(inc.end_date).ToShortDateString();               


                if (inc.incentive_type == 1)
                {
                    pnlDiscount.Visible = true;
                    pnlAmount.Visible = false;
                    rdbDiscountType.SelectedValue = inc.incentive_type.ToString();
                  
                  
                }
                else
                {
                    pnlDiscount.Visible = false;
                    pnlAmount.Visible = true;
                    rdbDiscountType.SelectedValue = inc.incentive_type.ToString();
                   
                    
                }
                //ddlDivision.SelectedValue = inc.client_id.ToString();

                if (inc.client_id.ToString().Contains(','))
                {
                    string[] ary = inc.client_id.Split(',');
                    foreach (ListItem item in lstDivision.Items)
                    {
                        foreach (var a in ary)
                        {
                            if (a == item.Value)
                            {
                                item.Selected = true;

                            }
                        }

                    }
                }
                else
                {
                    lstDivision.SelectedValue = inc.client_id;
                }


            }
            else
            {
                lblHeaderTitle.Text = "Add New Incentive";
                hdnIncentiveId.Value = "0";
            }


            csCommonUtility.SetPagePermission(this.Page, new string[] { "btnSubmit", "btnAddNew" });
        }
    }

    private void BindDivision()
    {
        try
        {
            string sql = "select id, division_name from division order by division_name";
            DataTable dt = csCommonUtility.GetDataTable(sql);
            lstDivision.DataSource = dt;
            lstDivision.DataTextField = "division_name";
            lstDivision.DataValueField = "id";
            lstDivision.DataBind();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.ToString());
        }
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("incentive_list.aspx");
    }
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        hdnIncentiveId.Value = "0";
        Response.Redirect("incentivedetails.aspx");
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 
        lblResult.Text = "";

        string selectedvalue = "";
        string selectDivisionName = "";
        foreach (ListItem item in lstDivision.Items)
        {
            if (item.Selected)
            {
                selectedvalue += item.Value + ",";
                selectDivisionName += item.Text + ", ";
            }
        }
        if (selectedvalue == "")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Division.");
            lblResult.Focus();
            return;
        }



        if (txtIncentiveName.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Incentive Name.");
            
            return;
        }
        if (rdbDiscountType.SelectedValue == "1")
        {
            if (txtDiscount.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Discount.");

                return;
            }
            else
            {
                try
                {
                    Convert.ToDecimal(txtDiscount.Text.Trim());
                }
                catch (Exception ex)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid discount.");

                    return;
                }
            }
        }
        else
        {
            if (txtAmount.Text.Trim() == "")
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Amount.");

                return;
            }
            else
            {
                try
                {
                    Convert.ToDecimal(txtAmount.Text.Trim());
                }
                catch (Exception ex)
                {
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid amount.");

                    return;
                }
            }
        }

        
       
        if (txtStartDate.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: Start Date.");
            
            return;
        }
        else
        {
            try
            {
                Convert.ToDateTime(txtStartDate.Text);
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid start date.");
                
                return;
            }
        }
        if (txtEndDate.Text.Trim() == "")
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("Missing required field: End Date.");
            
            return;
        }
        else
        {
            try
            {
                Convert.ToDateTime(txtEndDate.Text);
            }
            catch (Exception ex)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid end date.");
                
                return;
            }
        }

        DataClassesDataContext _db = new DataClassesDataContext();

        incentive inc = new incentive();
        if (Convert.ToInt32(hdnIncentiveId.Value) > 0)
            inc = _db.incentives.Single(i => i.incentive_id == Convert.ToInt32(hdnIncentiveId.Value));
        else
            if (_db.incentives.Where(i => i.incentive_name == txtIncentiveName.Text).SingleOrDefault() != null)
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Incentive already exist. Please try another.");
                
                return;
            }

        if (rdbDiscountType.SelectedValue == "1")
        {
            inc.discount = Convert.ToDecimal(txtDiscount.Text);
            inc.incentive_type =Convert.ToInt32(rdbDiscountType.SelectedValue);
            if (Convert.ToInt32(hdnIncentiveId.Value)==0)
            inc.amount = 0;
        }
        else
        {
            if (Convert.ToInt32(hdnIncentiveId.Value)==0)
            inc.discount = 0;
            inc.amount = Convert.ToDecimal(txtAmount.Text);
            inc.incentive_type = Convert.ToInt32(rdbDiscountType.SelectedValue);
        }
        inc.incentive_name = txtIncentiveName.Text;
        inc.incentive_desc = txtDescription.Text;

        inc.division_name = selectDivisionName.Trim().TrimEnd(',');
        inc.client_id = selectedvalue.Trim().TrimEnd(',');

        inc.is_active = Convert.ToBoolean(chkActive.Checked);
        inc.start_date = Convert.ToDateTime(txtStartDate.Text);
        inc.end_date = Convert.ToDateTime(txtEndDate.Text);
        //inc.client_id = Convert.ToInt32(ddlDivision.SelectedValue);

        if (Convert.ToInt32(hdnIncentiveId.Value) == 0)
        {
            _db.incentives.InsertOnSubmit(inc);

            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");
             
        }
        else
        {
            lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");
             
        }
        _db.SubmitChanges();
    }
    protected void rdbDiscountType_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdbDiscountType.ID, rdbDiscountType.GetType().Name, "SelectedIndexChanged"); 
        if (rdbDiscountType.SelectedValue =="1")
        {
            pnlDiscount.Visible = true;
            pnlAmount.Visible = false;
            

        }
        else
        {
            pnlDiscount.Visible = false;
            pnlAmount.Visible = true;
           
        }
    }
}
