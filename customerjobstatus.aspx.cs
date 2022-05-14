using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Configuration;

public partial class customerjobstatus : System.Web.UI.Page
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
            if (Request.QueryString.Get("cid") != null)
            {
                int nCustomerId = Convert.ToInt32(Request.QueryString.Get("cid"));
                hdnCustomerId.Value = nCustomerId.ToString();
                Session.Add("CustomerId", hdnCustomerId.Value);
                DataClassesDataContext _db = new DataClassesDataContext();
                customer objCust = new customer();
                objCust = _db.customers.Single(c => c.customer_id == nCustomerId);

                lblCustomerName.Text = objCust.first_name1 + " " + objCust.last_name1;

                customer_jobstatus objCJS = new customer_jobstatus();
                if (_db.customer_jobstatus.Where(c => c.customerid == nCustomerId).SingleOrDefault() == null)
                {
                    objCJS.customerid = nCustomerId;
                    objCJS.jobstatusid = 0;
                    _db.customer_jobstatus.InsertOnSubmit(objCJS);
                    _db.SubmitChanges();

                    hdnJobStatusId.Value = objCJS.jobstatusid.ToString();
                }
                else
                {
                    objCJS = _db.customer_jobstatus.Single(c => c.customerid == nCustomerId);
                    hdnJobStatusId.Value = objCJS.jobstatusid.ToString();
                }

                BindImages(Convert.ToInt32(hdnJobStatusId.Value));
            }
        }
    }

    private void BindImages(int nJobStatusId)
    {
        //for (int i = 1; i < 8; i++)
        //{
        //    // Green
        //    if (i < nJobStatusId)
        //    {
                
        //    }
        //        // Orange
        //    else if (i == nJobStatusId)
        //    {

        //    }
        //        // White
        //    else
        //    {
 
        //    }
        //}

        if (nJobStatusId == 0)
        {
            imgA.ImageUrl = "JobImages/OrangeA.jpg";
            imgB.ImageUrl = "JobImages/WhiteB.jpg";
            imgC.ImageUrl = "JobImages/WhiteC.jpg";
            imgD.ImageUrl = "JobImages/WhiteD.jpg";
            imgE.ImageUrl = "JobImages/WhiteE.jpg";
            imgF.ImageUrl = "JobImages/WhiteF.jpg";
            imgG.ImageUrl = "JobImages/WhiteG.jpg";

            chkA.Enabled = true;
        }
        else if (nJobStatusId == 1)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/OrangeB.jpg";
            imgC.ImageUrl = "JobImages/WhiteC.jpg";
            imgD.ImageUrl = "JobImages/WhiteD.jpg";
            imgE.ImageUrl = "JobImages/WhiteE.jpg";
            imgF.ImageUrl = "JobImages/WhiteF.jpg";
            imgG.ImageUrl = "JobImages/WhiteG.jpg";

            chkA.Checked = true;
            chkA.Enabled = false;
            chkB.Enabled = true;
        }
        else if (nJobStatusId == 2)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/OrangeC.jpg";
            imgD.ImageUrl = "JobImages/WhiteD.jpg";
            imgE.ImageUrl = "JobImages/WhiteE.jpg";
            imgF.ImageUrl = "JobImages/WhiteF.jpg";
            imgG.ImageUrl = "JobImages/WhiteG.jpg";

            chkA.Checked = true;
            chkB.Checked = true;
            chkB.Enabled = false;
            chkC.Enabled = true;
        }
        else if (nJobStatusId == 3)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/OrangeD.jpg";
            imgE.ImageUrl = "JobImages/WhiteE.jpg";
            imgF.ImageUrl = "JobImages/WhiteF.jpg";
            imgG.ImageUrl = "JobImages/WhiteG.jpg";

            chkA.Checked = true;
            chkB.Checked = true;
            chkC.Checked = true;
            chkC.Enabled = false;
            chkD.Enabled = true;
        }
        else if (nJobStatusId == 4)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/GreenD.jpg";
            imgE.ImageUrl = "JobImages/OrangeE.jpg";
            imgF.ImageUrl = "JobImages/WhiteF.jpg";
            imgG.ImageUrl = "JobImages/WhiteG.jpg";

            chkA.Checked = true;
            chkB.Checked = true;
            chkC.Checked = true;
            chkD.Checked = true;
            chkD.Enabled = false;
            chkE.Enabled = true;
        }
        else if (nJobStatusId == 5)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/GreenD.jpg";
            imgE.ImageUrl = "JobImages/GreenE.jpg";
            imgF.ImageUrl = "JobImages/OrangeF.jpg";
            imgG.ImageUrl = "JobImages/WhiteG.jpg";

            chkA.Checked = true;
            chkB.Checked = true;
            chkC.Checked = true;
            chkD.Checked = true;
            chkE.Checked = true;
            chkE.Enabled = false;
            chkF.Enabled = true;
        }
        else if (nJobStatusId == 6)
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/GreenD.jpg";
            imgE.ImageUrl = "JobImages/GreenE.jpg";
            imgF.ImageUrl = "JobImages/GreenF.jpg";
            imgG.ImageUrl = "JobImages/OrangeG.jpg";

            chkA.Checked = true;
            chkB.Checked = true;
            chkC.Checked = true;
            chkD.Checked = true;
            chkE.Checked = true;
            chkF.Checked = true;
            chkF.Enabled = false;
            chkG.Enabled = true;
        }
        else
        {
            imgA.ImageUrl = "JobImages/GreenA.jpg";
            imgB.ImageUrl = "JobImages/GreenB.jpg";
            imgC.ImageUrl = "JobImages/GreenC.jpg";
            imgD.ImageUrl = "JobImages/GreenD.jpg";
            imgE.ImageUrl = "JobImages/GreenE.jpg";
            imgF.ImageUrl = "JobImages/GreenF.jpg";
            imgG.ImageUrl = "JobImages/GreenG.jpg";

            chkA.Checked = true;
            chkB.Checked = true;
            chkC.Checked = true;
            chkD.Checked = true;
            chkE.Checked = true;
            chkF.Checked = true;
            chkG.Checked = true;
            chkG.Enabled = false;
        }
    }
    protected void chkA_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkA.ID, chkA.GetType().Name, "CheckedChanged"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "UPDATE customer_jobstatus SET jobstatusid = 1 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        hdnJobStatusId.Value = "1";

        BindImages(Convert.ToInt32(hdnJobStatusId.Value));
    }
    protected void chkB_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkB.ID, chkB.GetType().Name, "CheckedChanged"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "UPDATE customer_jobstatus SET jobstatusid = 2 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        hdnJobStatusId.Value = "2";

        BindImages(Convert.ToInt32(hdnJobStatusId.Value));
    }
    protected void chkC_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkC.ID, chkC.GetType().Name, "CheckedChanged"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "UPDATE customer_jobstatus SET jobstatusid = 3 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        hdnJobStatusId.Value = "3";

        BindImages(Convert.ToInt32(hdnJobStatusId.Value));
    }
    protected void chkD_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkD.ID, chkD.GetType().Name, "CheckedChanged"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "UPDATE customer_jobstatus SET jobstatusid = 4 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        hdnJobStatusId.Value = "4";

        BindImages(Convert.ToInt32(hdnJobStatusId.Value));
    }
    protected void chkE_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkE.ID, chkE.GetType().Name, "CheckedChanged"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "UPDATE customer_jobstatus SET jobstatusid = 5 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        hdnJobStatusId.Value = "5";

        BindImages(Convert.ToInt32(hdnJobStatusId.Value));
    }
    protected void chkF_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkF.ID, chkF.GetType().Name, "CheckedChanged"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "UPDATE customer_jobstatus SET jobstatusid = 6 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        hdnJobStatusId.Value = "6";

        BindImages(Convert.ToInt32(hdnJobStatusId.Value));
    }
    protected void chkG_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkG.ID, chkG.GetType().Name, "CheckedChanged"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "UPDATE customer_jobstatus SET jobstatusid = 7 WHERE customerid =" + Convert.ToInt32(hdnCustomerId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
        _db.SubmitChanges();

        hdnJobStatusId.Value = "7";

        BindImages(Convert.ToInt32(hdnJobStatusId.Value));
    }
}
