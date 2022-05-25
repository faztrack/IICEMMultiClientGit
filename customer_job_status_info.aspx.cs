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
using CrystalDecisions.CrystalReports.Engine;
public partial class customer_job_status_info : System.Web.UI.Page
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
            else
            {
                userinfo oUser = (userinfo)Session["oUser"];
                hdnEmailType.Value = oUser.EmailIntegrationType.ToString();

            }

            if (Page.User.IsInRole("admin034") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }
          
            imgB.Attributes.Add("onClick", "DisplayWindow2();");
          
            imgD.Attributes.Add("onClick", "DisplayWindow4();");
            imgE.Attributes.Add("onClick", "DisplayWindow5();");
        
            int nEstid = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstid.ToString();
            if (Request.QueryString.Get("cid") != null)
            {
                int nCustomerId = Convert.ToInt32(Request.QueryString.Get("cid"));
                hdnCustomerId.Value = nCustomerId.ToString();
                Session.Add("CustomerId", hdnCustomerId.Value);
                DataClassesDataContext _db = new DataClassesDataContext();
                customer objCust = new customer();
                objCust = _db.customers.Single(c => c.customer_id == nCustomerId);

                rdoconfirm.SelectedValue = Convert.ToInt32(objCust.isJobSatusViewable).ToString();
                hdnClientId.Value = objCust.client_id.ToString();


                lblCustomerName.Text = objCust.first_name1 + " " + objCust.last_name1;
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.SingleOrDefault(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.client_id == Convert.ToInt32(hdnClientId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                    if (cus_est.alter_job_number == "" || cus_est.alter_job_number == null)
                        lblCustomerName.Text = objCust.first_name1 + " " + objCust.last_name1 + " ( Job Number: " + cus_est.job_number + " )";
                    else
                        lblCustomerName.Text = objCust.first_name1 + " " + objCust.last_name1 + " ( Job Number: " + cus_est.alter_job_number + " )";

                    //lblCustomerName.Text = objCust.first_name1 + " " + objCust.last_name1 + " ( Job Number: " + cus_est.job_number + " )";
                    // lblEstimateName.Text = cus_est.estimate_name;
                }

                customer_jobstatus objCJS = new customer_jobstatus();
                if (_db.customer_jobstatus.Where(c => c.customerid == nCustomerId && c.estimate_id == nEstid).SingleOrDefault() == null)
                {
                    objCJS.customerid = nCustomerId;
                    objCJS.jobstatusid = 0;
                    objCJS.estimate_id = nEstid;
                    _db.customer_jobstatus.InsertOnSubmit(objCJS);
                    _db.SubmitChanges();

                    hdnJobStatusId.Value = objCJS.jobstatusid.ToString();
                }
                else
                {
                    objCJS = _db.customer_jobstatus.Single(c => c.customerid == nCustomerId && c.estimate_id == nEstid);
                    hdnJobStatusId.Value = objCJS.jobstatusid.ToString();
                    objCJS.estimate_id = nEstid;
                }
                _db.SubmitChanges();
                BindImages(Convert.ToInt32(hdnJobStatusId.Value));
                LoadDscription();
            }

            csCommonUtility.SetPagePermission(this.Page, new string[] { "rdoconfirm", "chkA", "btnAddnewRow", "imgB" });
            csCommonUtility.SetPagePermissionForGrid(this.Page, new string[] { "Save", "grdStatusDesc_ddlStage"});
        }
    }
    private void LoadDscription()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DataTable tmpTable = LoadDataTable();

        var item = from it in _db.job_status_descs
                   where it.customer_id == Convert.ToInt32(hdnCustomerId.Value) && it.estimate_id == Convert.ToInt32(hdnEstimateId.Value)
                   select new StatusDescription()
                   {
                       jobstatus_desc_id = (int)it.jobstatus_desc_id,
                       customer_id = (int)it.customer_id,
                       jobstatusid = (int)it.jobstatusid,
                       status_description = it.status_description,
                       status_serial = (int)it.status_serial,
                       estimate_id = (int)it.estimate_id,
                   };
        foreach (StatusDescription jsc in item)
        {

            DataRow drNew = tmpTable.NewRow();
            drNew["jobstatus_desc_id"] = jsc.jobstatus_desc_id;
            drNew["customer_id"] = jsc.customer_id;
            drNew["jobstatusid"] = jsc.jobstatusid;
            drNew["status_description"] = jsc.status_description;
            drNew["status_serial"] = jsc.status_serial;
            drNew["estimate_id"] = jsc.estimate_id;
            tmpTable.Rows.Add(drNew);
        }
        if (tmpTable.Rows.Count == 0)
        {
            DataRow drNew = tmpTable.NewRow();
            drNew["jobstatus_desc_id"] = 0;
            drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew["jobstatusid"] = 1;
            drNew["status_description"] = "";
            drNew["status_serial"] = 1;
            drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            tmpTable.Rows.Add(drNew);
        }
        Session.Add("jobstatus_desc", tmpTable);
        grdStatusDesc.DataSource = tmpTable;
        grdStatusDesc.DataKeyNames = new string[] { "jobstatus_desc_id" };
        grdStatusDesc.DataBind();

    }
    private DataTable LoadDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("jobstatus_desc_id", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("jobstatusid", typeof(int));
        table.Columns.Add("status_serial", typeof(int));
        table.Columns.Add("status_description", typeof(string));
        table.Columns.Add("estimate_id", typeof(int));
        return table;
    }

    private void BindImages(int nJobStatusId)
    {

        if (nJobStatusId == 0)
        {
            imgA.ImageUrl = "JobImages/OrangeA.jpg";

            imgB.ImageUrl = "JobImages/WhiteB.jpg";
            imgC.ImageUrl = "JobImages/WhiteC.jpg";
            imgD.ImageUrl = "JobImages/WhiteD.jpg";
            imgE.ImageUrl = "JobImages/WhiteE.jpg";
            imgButtonF.ImageUrl = "JobImages/WhiteF.jpg";
            imgButtonF.ImageUrl = "JobImages/WhiteF.jpg";
            imgButtonG.ImageUrl = "JobImages/WhiteG.jpg";

            chkA.Enabled = true;
            chkB.Enabled = false;
            chkC.Enabled = false;
            chkD.Enabled = false;
            chkE.Enabled = false;
            chkF.Enabled = false;
            chkG.Enabled = false;

            chkA.Checked = false;
            chkB.Checked = false;
            chkC.Checked = false;
            chkD.Checked = false;
            chkE.Checked = false;
            chkF.Checked = false;
            chkG.Checked = false;

        }
        else if (nJobStatusId == 1)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/OrangeB.jpg";

            imgC.ImageUrl = "JobImages/WhiteC.jpg";
            imgD.ImageUrl = "JobImages/WhiteD.jpg";
            imgE.ImageUrl = "JobImages/WhiteE.jpg";
            imgButtonF.ImageUrl = "JobImages/WhiteF.jpg";
            imgButtonG.ImageUrl = "JobImages/WhiteG.jpg";
            
            chkA.Checked = true;
            chkB.Checked = false;
            chkC.Checked = false;
            chkD.Checked = false;
            chkE.Checked = false;
            chkF.Checked = false;
            chkG.Checked = false;

            chkA.Enabled = true;
            chkB.Enabled = true;
            chkC.Enabled = false;
            chkD.Enabled = false;
            chkE.Enabled = false;
            chkF.Enabled = false;
            chkG.Enabled = false;
        }
        else if (nJobStatusId == 2)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/OrangeC.jpg";

            imgD.ImageUrl = "JobImages/WhiteD.jpg";
            imgE.ImageUrl = "JobImages/WhiteE.jpg";
            imgButtonF.ImageUrl = "JobImages/WhiteF.jpg";
            imgButtonG.ImageUrl = "JobImages/WhiteG.jpg";

            chkA.Checked = true;
            chkB.Checked = true;
            chkC.Checked = false;
            chkD.Checked = false;
            chkE.Checked = false;
            chkF.Checked = false;
            chkG.Checked = false;

            chkA.Enabled = true;
            chkB.Enabled = true;
            chkC.Enabled = true;
            chkD.Enabled = false;
            chkE.Enabled = false;
            chkF.Enabled = false;
            chkG.Enabled = false;
        }
        else if (nJobStatusId == 3)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/OrangeD.jpg";

            imgE.ImageUrl = "JobImages/WhiteE.jpg";
            imgButtonF.ImageUrl = "JobImages/WhiteF.jpg";
            imgButtonG.ImageUrl = "JobImages/WhiteG.jpg";

            chkA.Checked = true;
            chkB.Checked = true;
            chkC.Checked = true;
            chkD.Checked = false;
            chkE.Checked = false;
            chkF.Checked = false;
            chkG.Checked = false;

            chkA.Enabled = true;
            chkB.Enabled = true;
            chkC.Enabled = true;
            chkD.Enabled = true;
            chkE.Enabled = false;
            chkF.Enabled = false;
            chkG.Enabled = false;
        }
        else if (nJobStatusId == 4)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/GreenD.jpg";
            imgE.ImageUrl = "JobImages/OrangeE.jpg";

            imgButtonF.ImageUrl = "JobImages/WhiteF.jpg";
            imgButtonG.ImageUrl = "JobImages/WhiteG.jpg";

            chkA.Checked = true;
            chkB.Checked = true;
            chkC.Checked = true;
            chkD.Checked = true;
            chkE.Checked = false;
            chkF.Checked = false;
            chkG.Checked = false;

            chkA.Enabled = true;
            chkB.Enabled = true;
            chkC.Enabled = true;
            chkD.Enabled = true;
            chkE.Enabled = true;
            chkF.Enabled = false;
            chkG.Enabled = false;
        }
        else if (nJobStatusId == 5)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/GreenD.jpg";
            imgE.ImageUrl = "JobImages/GreenE.jpg";
            imgButtonF.ImageUrl = "JobImages/OrangeF.jpg";

            imgButtonG.ImageUrl = "JobImages/WhiteG.jpg";

            chkA.Checked = true;
            chkB.Checked = true;
            chkC.Checked = true;
            chkD.Checked = true;
            chkE.Checked = true;
            chkF.Checked = false;
            chkG.Checked = false;

            chkA.Enabled = true;
            chkB.Enabled = true;
            chkC.Enabled = true;
            chkD.Enabled = true;
            chkE.Enabled = true;
            chkF.Enabled = false;
            chkG.Enabled = false;

        }
        else if (nJobStatusId == 6)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/GreenD.jpg";
            imgE.ImageUrl = "JobImages/GreenE.jpg";
            imgButtonF.ImageUrl = "JobImages/GreenF.jpg";
            imgButtonG.ImageUrl = "JobImages/OrangeG.jpg";

            chkA.Checked = true;
            chkB.Checked = true;
            chkC.Checked = true;
            chkD.Checked = true;
            chkE.Checked = true;
            chkF.Checked = true;
            chkG.Checked = false;

            chkA.Enabled = true;
            chkB.Enabled = true;
            chkC.Enabled = true;
            chkD.Enabled = true;
            chkE.Enabled = true;
            chkF.Enabled = false;
            chkG.Enabled = false;

        }
        else
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/GreenD.jpg";
            imgE.ImageUrl = "JobImages/GreenE.jpg";
            imgButtonF.ImageUrl = "JobImages/GreenF.jpg";
            imgButtonG.ImageUrl = "JobImages/GreenG.jpg";

            chkA.Checked = true;
            chkB.Checked = true;
            chkC.Checked = true;
            chkD.Checked = true;
            chkE.Checked = true;
            chkF.Checked = true;
            chkG.Checked = true;

            chkA.Enabled = true;
            chkB.Enabled = true;
            chkC.Enabled = true;
            chkD.Enabled = true;
            chkE.Enabled = true;
            chkF.Enabled = false;
            chkG.Enabled = false;

        }

    }
    protected void chkA_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkA.ID, chkA.GetType().Name, "CheckedChanged"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = string.Empty;
        if (chkA.Checked)
        {
            strQ = "UPDATE customer_jobstatus SET jobstatusid = 1 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            hdnJobStatusId.Value = "1";
        }
        else
        {
            strQ = "UPDATE customer_jobstatus SET jobstatusid = 0 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            hdnJobStatusId.Value = "0";
        }
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        BindImages(Convert.ToInt32(hdnJobStatusId.Value));
    }
    protected void chkB_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkB.ID, chkB.GetType().Name, "CheckedChanged"); 
        //DataClassesDataContext _db = new DataClassesDataContext();
        //string strQ = "UPDATE customer_jobstatus SET jobstatusid = 2 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        //_db.ExecuteCommand(strQ, string.Empty);
        //_db.SubmitChanges();

        //hdnJobStatusId.Value = "2";

        //BindImages(Convert.ToInt32(hdnJobStatusId.Value));
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = string.Empty;
        if (chkB.Checked)
        {
            strQ = "UPDATE customer_jobstatus SET jobstatusid = 2 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            hdnJobStatusId.Value = "2";
        }
        else
        {
            strQ = "UPDATE customer_jobstatus SET jobstatusid = 1 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            hdnJobStatusId.Value = "1";
        }
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        BindImages(Convert.ToInt32(hdnJobStatusId.Value));
    }
    protected void chkC_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkC.ID, chkC.GetType().Name, "CheckedChanged");
        //DataClassesDataContext _db = new DataClassesDataContext();
        //string strQ = "UPDATE customer_jobstatus SET jobstatusid = 3 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        //_db.ExecuteCommand(strQ, string.Empty);
        //_db.SubmitChanges();

        //hdnJobStatusId.Value = "3";

        //BindImages(Convert.ToInt32(hdnJobStatusId.Value));
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = string.Empty;
        if (chkC.Checked)
        {
            strQ = "UPDATE customer_jobstatus SET jobstatusid = 3 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            hdnJobStatusId.Value = "3";
        }
        else
        {
            strQ = "UPDATE customer_jobstatus SET jobstatusid = 2 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            hdnJobStatusId.Value = "2";
        }
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        BindImages(Convert.ToInt32(hdnJobStatusId.Value));
    }
    protected void chkD_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkD.ID, chkD.GetType().Name, "CheckedChanged"); 
        //DataClassesDataContext _db = new DataClassesDataContext();
        //string strQ = "UPDATE customer_jobstatus SET jobstatusid = 4 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        //_db.ExecuteCommand(strQ, string.Empty);
        //_db.SubmitChanges();

        //hdnJobStatusId.Value = "4";

        //BindImages(Convert.ToInt32(hdnJobStatusId.Value));
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = string.Empty;
        if (chkD.Checked)
        {
            strQ = "UPDATE customer_jobstatus SET jobstatusid = 4 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            hdnJobStatusId.Value = "4";
        }
        else
        {
            strQ = "UPDATE customer_jobstatus SET jobstatusid = 3 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            hdnJobStatusId.Value = "3";
        }
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        BindImages(Convert.ToInt32(hdnJobStatusId.Value));
    }
    protected void chkE_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkE.ID, chkE.GetType().Name, "CheckedChanged");
        //DataClassesDataContext _db = new DataClassesDataContext();
        //string strQ = "UPDATE customer_jobstatus SET jobstatusid = 5 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        //_db.ExecuteCommand(strQ, string.Empty);
        //_db.SubmitChanges();

        //hdnJobStatusId.Value = "5";

        //BindImages(Convert.ToInt32(hdnJobStatusId.Value));
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = string.Empty;
        if (chkE.Checked)
        {
            strQ = "UPDATE customer_jobstatus SET jobstatusid = 5 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            hdnJobStatusId.Value = "5";
        }
        else
        {
            strQ = "UPDATE customer_jobstatus SET jobstatusid = 4 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            hdnJobStatusId.Value = "4";
        }
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        BindImages(Convert.ToInt32(hdnJobStatusId.Value));
    }
    protected void chkF_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkF.ID, chkF.GetType().Name, "CheckedChanged");
        //DataClassesDataContext _db = new DataClassesDataContext();
        //string strQ = "UPDATE customer_jobstatus SET jobstatusid = 6 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        //_db.ExecuteCommand(strQ, string.Empty);
        //_db.SubmitChanges();

        //hdnJobStatusId.Value = "6";

        //BindImages(Convert.ToInt32(hdnJobStatusId.Value));
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = string.Empty;
        if (chkF.Checked)
        {
            strQ = "UPDATE customer_jobstatus SET jobstatusid = 6 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            hdnJobStatusId.Value = "6";
        }
        else
        {
            strQ = "UPDATE customer_jobstatus SET jobstatusid = 5 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            hdnJobStatusId.Value = "5";
        }
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        BindImages(Convert.ToInt32(hdnJobStatusId.Value));
    }
    protected void chkG_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkG.ID, chkG.GetType().Name, "CheckedChanged");
        //DataClassesDataContext _db = new DataClassesDataContext();
        //string strQ = "UPDATE customer_jobstatus SET jobstatusid = 7 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        //_db.ExecuteCommand(strQ, string.Empty);
        //_db.SubmitChanges();

        //hdnJobStatusId.Value = "7";

        //BindImages(Convert.ToInt32(hdnJobStatusId.Value));
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = string.Empty;
        if (chkG.Checked)
        {
            strQ = "UPDATE customer_jobstatus SET jobstatusid = 7 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            hdnJobStatusId.Value = "7";
        }
        else
        {
            strQ = "UPDATE customer_jobstatus SET jobstatusid = 6 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value) + " and estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            hdnJobStatusId.Value = "6";
        }
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        BindImages(Convert.ToInt32(hdnJobStatusId.Value));
    }
    protected void grdStatusDesc_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "Save")
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            //string strQ_Delete = "Delete job_status_desc WHERE  customer_id=" + Convert.ToInt32(hdnCustomerId.Value);
            //_db.ExecuteCommand(strQ_Delete, string.Empty);

            DataTable table = (DataTable)Session["jobstatus_desc"];

            foreach (GridViewRow di in grdStatusDesc.Rows)
            {
                {
                    DropDownList ddlStage = (DropDownList)di.FindControl("ddlStage");
                    TextBox txtDescription = (TextBox)di.FindControl("txtDescription");
                    TextBox txtStatusSerial = (TextBox)di.FindControl("txtStatusSerial");
                    DataRow dr = table.Rows[di.RowIndex];

                    dr["jobstatus_desc_id"] = Convert.ToInt32(grdStatusDesc.DataKeys[di.RowIndex].Values[0]);
                    dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                    dr["jobstatusid"] = Convert.ToInt32(ddlStage.SelectedValue);
                    dr["status_description"] = txtDescription.Text;
                    dr["status_serial"] = Convert.ToInt32(txtStatusSerial.Text);
                    dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                    

                }

            }
            foreach (DataRow dr in table.Rows)
            {
                job_status_desc job_status = new job_status_desc();
                if (Convert.ToInt32(dr["jobstatus_desc_id"]) > 0)
                    job_status = _db.job_status_descs.Single(l => l.jobstatus_desc_id == Convert.ToInt32(dr["jobstatus_desc_id"]));
                string str = dr["status_description"].ToString().Trim();
                if (str.Length > 0)
                {
                    job_status.customer_id = Convert.ToInt32(dr["customer_id"]);
                    job_status.jobstatusid = Convert.ToInt32(dr["jobstatusid"]);
                    job_status.status_serial = Convert.ToInt32(dr["status_serial"]);
                    job_status.status_description = dr["status_description"].ToString();
                    job_status.estimate_id = Convert.ToInt32(dr["estimate_id"]);
                   

                }
                if (Convert.ToInt32(dr["jobstatus_desc_id"]) == 0)
                {
                    _db.job_status_descs.InsertOnSubmit(job_status);
                }
            }

            lblResult.Text = csCommonUtility.GetSystemMessage("Data saved successfully");
            
            _db.SubmitChanges();
            LoadDscription();
           

        }
    }
    
    protected void grdStatusDesc_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DropDownList ddlStage = (DropDownList)grdStatusDesc.Rows[e.NewEditIndex].FindControl("ddlStage");
        TextBox txtDescription = (TextBox)grdStatusDesc.Rows[e.NewEditIndex].FindControl("txtDescription");
        TextBox txtStatusSerial = (TextBox)grdStatusDesc.Rows[e.NewEditIndex].FindControl("txtStatusSerial");
        Label lblDescription = (Label)grdStatusDesc.Rows[e.NewEditIndex].FindControl("lblDescription");
        Label lblStage = (Label)grdStatusDesc.Rows[e.NewEditIndex].FindControl("lblStage");
        Label lblStatusSerial = (Label)grdStatusDesc.Rows[e.NewEditIndex].FindControl("lblStatusSerial");
        txtDescription.Visible = true;
        lblDescription.Visible = false;
        txtStatusSerial.Visible = true;
        lblStatusSerial.Visible = false;
        ddlStage.Visible = true;
        lblStage.Visible = false;
        LinkButton btn = (LinkButton)grdStatusDesc.Rows[e.NewEditIndex].Cells[3].Controls[0];
        btn.Text = "Update";
        btn.CommandName = "Update";

    }
    protected void grdStatusDesc_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DropDownList ddlStage = (DropDownList)grdStatusDesc.Rows[e.RowIndex].FindControl("ddlStage");
        TextBox txtDescription = (TextBox)grdStatusDesc.Rows[e.RowIndex].FindControl("txtDescription");
        TextBox txtStatusSerial = (TextBox)grdStatusDesc.Rows[e.RowIndex].FindControl("txtStatusSerial");
        Label lblDescription = (Label)grdStatusDesc.Rows[e.RowIndex].FindControl("lblDescription");
        Label lblStage = (Label)grdStatusDesc.Rows[e.RowIndex].FindControl("lblStage");
        Label lblStatusSerial = (Label)grdStatusDesc.Rows[e.RowIndex].FindControl("lblStatusSerial");

        int njobstatus_desc_id = Convert.ToInt32(grdStatusDesc.DataKeys[Convert.ToInt32(e.RowIndex)].Values[0]);
        string strQ = "UPDATE job_status_desc SET  jobstatusid=" + Convert.ToInt32(ddlStage.SelectedValue) + " ,status_description='" + txtDescription.Text.Replace("'", "''") + "' ,status_serial=" + Convert.ToInt32(txtStatusSerial.Text.Replace("'", "''")) + "  WHERE jobstatus_desc_id =" + njobstatus_desc_id + " AND customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);
        _db.ExecuteCommand(strQ, string.Empty);

        LoadDscription();
        lblResult.Text = csCommonUtility.GetSystemMessage("Data updated successfully");
        

    }
    
    protected void grdStatusDesc_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
           

            DropDownList ddlStage = (DropDownList)e.Row.FindControl("ddlStage");
            TextBox txtDescription = (TextBox)e.Row.FindControl("txtDescription");
            TextBox txtStatusSerial = (TextBox)e.Row.FindControl("txtStatusSerial");
            Label lblDescription = (Label)e.Row.FindControl("lblDescription");
            Label lblStage = (Label)e.Row.FindControl("lblStage");
            Label lblStatusSerial = (Label)e.Row.FindControl("lblStatusSerial");
            lblStage.Text = ddlStage.SelectedItem.Text;

           
            string str = lblDescription.Text.Replace("&nbsp;", "");
            if (str == "")
            {
                txtDescription.Visible = true;
                lblDescription.Visible = false;
                txtStatusSerial.Visible = true;
                lblStatusSerial.Visible = false;
                ddlStage.Visible = true;
                lblStage.Visible = false;
                LinkButton btn = (LinkButton)e.Row.Cells[3].Controls[0];
                btn.Text = "Save";
                btn.CommandName = "Save";

            }
            

        }

    }
    protected void btnAddnewRow_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddnewRow.ID, btnAddnewRow.GetType().Name, "Click"); 
         DataTable table = (DataTable)Session["jobstatus_desc"];

            foreach (GridViewRow di in grdStatusDesc.Rows)
            {
                {
                    DropDownList ddlStage = (DropDownList)di.FindControl("ddlStage");
                    TextBox txtDescription = (TextBox)di.FindControl("txtDescription");
                    TextBox txtStatusSerial = (TextBox)di.FindControl("txtStatusSerial");
                    DataRow dr = table.Rows[di.RowIndex];

                    dr["jobstatus_desc_id"] = Convert.ToInt32(grdStatusDesc.DataKeys[di.RowIndex].Values[0]);
                    dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                    dr["jobstatusid"] = Convert.ToInt32(ddlStage.SelectedValue);
                    dr["status_description"] = txtDescription.Text;
                    dr["status_serial"] = Convert.ToInt32(txtStatusSerial.Text);
                    dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);

                }

            }

            DataRow drNew = table.NewRow();
            drNew["jobstatus_desc_id"] = 0;
            drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew["jobstatusid"] = 1;
            drNew["status_description"] = "";
            drNew["status_serial"] = 1;
            drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            table.Rows.Add(drNew);

            Session.Add("jobstatus_desc", table);
            grdStatusDesc.DataSource = table;
            grdStatusDesc.DataKeyNames = new string[] { "jobstatus_desc_id" };
            grdStatusDesc.DataBind();
            lblResult.Text = "";

    }
    protected void imgButtonF_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgButtonF.ID, imgButtonF.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        if (_db.customersurveys.Where(cs => cs.customerid == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).SingleOrDefault() != null)
        {
            customer objCust = new customer();
            objCust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
            string strCustName2 = "";
            string strCross = "";
            string strCustName = objCust.first_name1 + " " + objCust.last_name1;
            strCustName2 = objCust.first_name2 + " " + objCust.last_name2;
            string strAddress = objCust.address;
            strCross = objCust.cross_street;
            string strCityStaeZip = objCust.city + ", " + objCust.state + ", " + objCust.zip_code;
            ReportDocument rptFile = new ReportDocument();
            string strReportPath = Server.MapPath(@"Reports\rpt\rptCompletionCertificate.rpt");
            rptFile.Load(strReportPath);


            Hashtable ht = new Hashtable();
            ht.Add("p_CustomerName", strCustName);
            ht.Add("p_CustomerName2", strCustName2);
            ht.Add("p_address", strAddress);
            ht.Add("p_crossstreet", strCross);
            ht.Add("p_CityStaeZip", strCityStaeZip);

            Session.Add(SessionInfo.Report_File, rptFile);
            Session.Add(SessionInfo.Report_Param, ht);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
        }
    }
    protected void imgButtonG_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgButtonG.ID, imgButtonG.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        if (_db.customersurveys.Where(cs => cs.customerid == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).SingleOrDefault() != null)
        {
            customer objCust = new customer();
            objCust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
           // int nEstimateId = 1;
            string strCompletionDate = "";
            string strContractDate = "";
            string strMainCustName = "";
            string MainAddress = "";
            string strCustName2 = "";
            string strCross = "";
            string strCustName = objCust.first_name1 + " " + objCust.last_name1;
            strCustName2 = objCust.first_name2 + " " + objCust.last_name2;
            string strAddress = objCust.address;
            strCross = objCust.cross_street;
            string strCityStaeZip = objCust.city + ", " + objCust.state + ", " + objCust.zip_code;
            if (strCustName2.Length > 2)
                strMainCustName = strCustName + "&" + strCustName2;
            else
                strMainCustName = strCustName;
            if (strCross.Length > 2)
                MainAddress = strAddress + ", " + strCross;
            else
                MainAddress = strAddress;
            MainAddress += ", " + strCityStaeZip;
            if (_db.customersurveys.Where(cs => cs.customerid == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).SingleOrDefault() != null)
            {
                customersurvey csv = new customersurvey();
                csv = _db.customersurveys.Single(cs => cs.customerid == Convert.ToInt32(hdnCustomerId.Value) && cs.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                strCompletionDate = Convert.ToDateTime(csv.date).ToShortDateString();

            }
            //string strQ = "select * from customer_estimate where customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " and status_id=3 and client_id=1";
            //IEnumerable<customer_estimate_model> list = _db.ExecuteQuery<customer_estimate_model>(strQ, string.Empty);

            //foreach (customer_estimate_model cus_est in list)
            //{
            //    nEstimateId = Convert.ToInt32(cus_est.estimate_id);
            //}
            if (_db.estimate_payments.Where(pay => pay.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pay.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pay.client_id == Convert.ToInt32(hdnClientId.Value)).SingleOrDefault() != null)
            {
                estimate_payment objEstPay = new estimate_payment();
                objEstPay = _db.estimate_payments.Single(pay => pay.estimate_id == Convert.ToInt32(hdnEstimateId.Value) && pay.customer_id == Convert.ToInt32(hdnCustomerId.Value) && pay.client_id == Convert.ToInt32(hdnClientId.Value));
                strContractDate = objEstPay.contract_date;
            }


            ReportDocument rptFile = new ReportDocument();
            string strReportPath = Server.MapPath(@"Reports\rpt\rptWarranty.rpt");
            rptFile.Load(strReportPath);


            Hashtable ht = new Hashtable();
            ht.Add("p_CustomerName", strMainCustName);
            ht.Add("p_address", MainAddress);
            ht.Add("p_ContractDate", strContractDate);
            ht.Add("p_CompletionDate", strCompletionDate);

            Session.Add(SessionInfo.Report_File, rptFile);
            Session.Add(SessionInfo.Report_Param, ht);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Popup", "window.open('Reports/Common/ReportViewer.aspx');", true);
        }
    }
    //protected void Button1_Click(object sender, EventArgs e)
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();
    //    string strQ = "UPDATE job_status_desc SET estimate_id=" + Convert.ToInt32(hdnEstimateId.Value) + " where customer_id=" + Convert.ToInt32(hdnCustomerId.Value);
    //    _db.ExecuteCommand(strQ, string.Empty);
    //    lblResult.Text = "successfully";
    //    

    //}

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("customerlist.aspx");
    }

    protected void imgA_Click(object sender, EventArgs e)
    {
        Response.Redirect("DocumentManagement.aspx?cid=" + hdnCustomerId.Value);
    }

    protected void imgC_Click(object sender, EventArgs e)
    {
        Response.Redirect("DocumentManagement.aspx?cid=" + hdnCustomerId.Value);
    }

    protected void rdoconfirm_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdoconfirm.ID, rdoconfirm.GetType().Name, "SelectedIndexChanged"); 

        DataClassesDataContext _db = new DataClassesDataContext();

        customer objCust = _db.customers.Single(c => c.customer_id ==   Convert.ToInt32(hdnCustomerId.Value));

        
        objCust.isJobSatusViewable = Convert.ToBoolean(Convert.ToInt32(rdoconfirm.SelectedValue));

        _db.SubmitChanges();

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    }
}

