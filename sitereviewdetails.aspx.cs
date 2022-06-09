using System;
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
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Exchange.WebServices.Data;

public partial class sitereviewdetails : System.Web.UI.Page
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
          
           
          
            txtSiteReviewDate.Text = DateTime.Now.ToString("MM-dd-yyyy");
            txtSiteReviewNote.Focus();

            if (Request.QueryString.Get("cid") != null)
            {
                hdnCustomerId.Value = Convert.ToInt32(Request.QueryString.Get("cid")).ToString();
                hdnEstimateId.Value = Convert.ToInt32(Request.QueryString.Get("nestid")).ToString();
                hdnBackId.Value = Convert.ToInt32(Request.QueryString.Get("nbackid")).ToString();

                DataClassesDataContext _db = new DataClassesDataContext();
                customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));

                if (objCust != null)
                {
                    sales_person sap = new sales_person();
                    sap = _db.sales_persons.SingleOrDefault(c => c.sales_person_id == Convert.ToInt32(objCust.sales_person_id) && c.is_active==true);
                    if (sap != null)
                    {
                        hdnSalesEmail.Value = sap.email;
                    }

                    hdnClientId.Value = objCust.client_id.ToString();

                    string strSuperintendent = string.Empty;
                    user_info uinfo = _db.user_infos.SingleOrDefault(u => u.user_id == objCust.SuperintendentId && u.is_active==true);
                    if (uinfo != null)
                    {
                        strSuperintendent = uinfo.first_name + " " + uinfo.last_name;
                        hdnSuperandentEmail.Value = uinfo.email;
                    }
                    hdnLastName.Value = objCust.last_name1;
                }
                DeleteTemporaryFiles();
            }
            if (Request.QueryString.Get("sid") != null)
            {
                hdnSiteReviewId.Value = Convert.ToInt32(Request.QueryString.Get("sid")).ToString();
                int nSiteReviewId = Convert.ToInt32(hdnSiteReviewId.Value);
                BindImageUploadedDetails(Convert.ToInt32(hdnSiteReviewId.Value));
                GetSitereviewDetails(nSiteReviewId);
                lblHeaderTitle.Text = "Site Review Details " + "(" + hdnLastName.Value + ")";
                // btnSubmit.Text = "Update";
                btnAddNew.Visible = false;
                btnSubmit.Text = "Update";
            }
            else
            {
                lblHeaderTitle.Text = "Add New Site Review " + "(" + hdnLastName.Value + ")";
                btnSubmit.Text = "Save";
                btnAddNew.Visible = true;
            }

        }
    }


    private void GetSitereviewDetails(int nSiteReviewId)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            SiteReviewNote objSRN = _db.SiteReviewNotes.SingleOrDefault(s => s.SiteReviewsId == nSiteReviewId);
            txtSiteReviewDate.Text = Convert.ToDateTime(objSRN.SiteReviewsDate).ToString("MM-dd-yyyy");
            txtSiteReviewNote.Text = objSRN.SiteReviewsNotes;
            if (objSRN.IsCustomerView == true)
                chkCustomer.Checked = true;
            else
                chkCustomer.Checked = false;
            hdnCustomerId.Value = objSRN.customer_id.ToString();
            hdnEstimateId.Value = objSRN.estimate_id.ToString();

            if (objSRN.StateOfMindID == 0)
            {
                pnlCustomerStateofMind.Visible = false;
                chkCustomerMind.Checked = false;
            }
            else
            {
                chkCustomerMind.Checked = true;
                pnlCustomerStateofMind.Visible = true;
                if (objSRN.StateOfMindID == 1)
                {
                    hdnCustomerMind.Value = "1";
                    imgbtnAngry.Attributes.Add("class", "opacityimage");
                    imgbtnFrustrated.Attributes.Add("class", "");
                    imgbtnConfused.Attributes.Add("class", "");
                    imgbtnIndifferent.Attributes.Add("class", "");
                    imgbtnHappy.Attributes.Add("class", "");
                }
                else if (objSRN.StateOfMindID == 2)
                {
                    hdnCustomerMind.Value = "2";
                    imgbtnAngry.Attributes.Add("class", "");
                    imgbtnFrustrated.Attributes.Add("class", "opacityimage");
                    imgbtnConfused.Attributes.Add("class", "");
                    imgbtnIndifferent.Attributes.Add("class", "");
                    imgbtnHappy.Attributes.Add("class", "");
                }
                else if (objSRN.StateOfMindID == 3)
                {
                    hdnCustomerMind.Value = "3";
                    imgbtnAngry.Attributes.Add("class", "");
                    imgbtnFrustrated.Attributes.Add("class", "");
                    imgbtnConfused.Attributes.Add("class", "opacityimage");
                    imgbtnIndifferent.Attributes.Add("class", "");
                    imgbtnHappy.Attributes.Add("class", "");
                }
                else if (objSRN.StateOfMindID == 4)
                {
                    hdnCustomerMind.Value = "4";
                    imgbtnAngry.Attributes.Add("class", "");
                    imgbtnFrustrated.Attributes.Add("class", "");
                    imgbtnConfused.Attributes.Add("class", "");
                    imgbtnIndifferent.Attributes.Add("class", "opacityimage");
                    imgbtnHappy.Attributes.Add("class", "");
                }
                else if (objSRN.StateOfMindID == 5)
                {
                    hdnCustomerMind.Value = "5";
                    imgbtnAngry.Attributes.Add("class", "");
                    imgbtnFrustrated.Attributes.Add("class", "");
                    imgbtnConfused.Attributes.Add("class", "");
                    imgbtnIndifferent.Attributes.Add("class", "");
                    imgbtnHappy.Attributes.Add("class", "opacityimage");
                }


            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private void DeleteTemporaryFiles()
    {
        string DestinationPath = Server.MapPath("~/Uploads//Temp//")+hdnCustomerId.Value+"//";
        if (Directory.Exists(DestinationPath))
        {
            string[] fileEntries = Directory.GetFiles(DestinationPath);
            foreach (string file in fileEntries)
            {
                File.Delete(file);
            }

        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click"); 

        lblResult.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        SiteReviewNote objSRN = new SiteReviewNote();

        if (txtSiteReviewDate.Text.Trim() != "")
        {
            try
            {
                Convert.ToDateTime(txtSiteReviewDate.Text.Trim());
            }
            catch
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Invalid Date.<br />");
                return;
            }
        }
        else
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing required field: Date.<br />");
            return;
        }
        string DestinationPath = null;
        DestinationPath = Server.MapPath("~/Uploads//Temp//") + hdnCustomerId.Value + "//";
        if (!System.IO.Directory.Exists(DestinationPath))
        {
            System.IO.Directory.CreateDirectory(DestinationPath);
        }
        string sourceFile = Server.MapPath("Uploads\\Temp\\")+hdnCustomerId.Value + @"\";
        string[] fileEntries = Directory.GetFiles(sourceFile);
        if (fileEntries.Length == 0 && txtSiteReviewNote.Text == "")
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage("Missing Notes or Attached files .<br />");

            return;
        }

        if (Convert.ToInt32(hdnSiteReviewId.Value) > 0)
            objSRN = _db.SiteReviewNotes.Single(c => c.SiteReviewsId == Convert.ToInt32(hdnSiteReviewId.Value));

        objSRN.customer_id = Convert.ToInt32(hdnCustomerId.Value);
        objSRN.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
        objSRN.SiteReviewsDate = Convert.ToDateTime(txtSiteReviewDate.Text);
        objSRN.SiteReviewsNotes = txtSiteReviewNote.Text;
        objSRN.IsCrew = false;

        // Customer State of Mind

        if (chkCustomerMind.Checked)
        {

            objSRN.StateOfMindID = Convert.ToInt32(hdnCustomerMind.Value);
        }
        else
        {
            objSRN.StateOfMindID = 0;
        }

        // Customer Check

        if (chkCustomer.Checked == true)
            objSRN.IsCustomerView = true;
        else
            objSRN.IsCustomerView = false;

        objSRN.IsUserView = false;
        objSRN.IsVendorView = false;


        #region comment
        //if (chkUser.Checked)
        //    objSRN.IsUserView = true;
        //else
        //    objSRN.IsUserView = false;
        //if (chkCustomer.Checked)
        //    objSRN.IsCustomerView = true;
        //else
        //    objSRN.IsCustomerView = false;
        //if (chkVendor.Checked)
        //    objSRN.IsVendorView = true;
        //else
        //    objSRN.IsVendorView = false;

        //if (chkCustomerMind.Checked)
        //{
        //    objSRN.StateOfMindID = Convert.ToInt32(ddlcCstomerMind.SelectedValue);
        //}
        //else
        //{
        //    objSRN.StateOfMindID = 0;
        //}
        //if (chkUser.Checked)
        //    objSRN.IsUserView = true;
        //else
        //    objSRN.IsUserView = false;
        //if (chkCustomer.Checked)
        //    objSRN.IsCustomerView = true;
        //else
        //    objSRN.IsCustomerView = false;
        //if (chkVendor.Checked)
        //    objSRN.IsVendorView = true;
        //else
        //    objSRN.IsVendorView = false;

        //if (chkUserNotify.Checked)
        //    objSRN.IsUserNotify = true;
        //else
        //    objSRN.IsUserNotify = false;

        //if (chkCustomerNotify.Checked)
        //    objSRN.IsCustomerNotify = true;
        //else
        //    objSRN.IsCustomerNotify = false;

        //if (chkVendorNotify.Checked)
        //    objSRN.IsVendorNotify = true;
        //else
        //    objSRN.IsVendorNotify = false;

        #endregion

        // Customer Notify
        objSRN.IsUserNotify = false;
        objSRN.IsCustomerNotify = false;
        objSRN.IsVendorNotify = false;
        objSRN.client_id = Convert.ToInt32(hdnClientId.Value);

        if (Convert.ToInt32(hdnSiteReviewId.Value) == 0)
        {
            objSRN.HasAttachments = false;
            objSRN.LastUpdateDate = DateTime.Now;
            objSRN.CreateDate = DateTime.Now;
            if (Session["oUser"] != null)
            {
                userinfo objUser = (userinfo)Session["oUser"];
                objSRN.CreatedBy = objUser.first_name + " " + objUser.last_name;
            }

            _db.SiteReviewNotes.InsertOnSubmit(objSRN);
            _db.SubmitChanges();
            lblResult.Text = csCommonUtility.GetSystemMessage("Data has been saved successfully.");
            hdnSiteReviewId.Value = objSRN.SiteReviewsId.ToString();
            MoveFile();

            lblUpload.Visible = false;

            grdTemp.DataSource = null;
            grdTemp.DataBind();
            pnlTemporaryImageUpload.Visible = false;
            btnSubmit.Text = "Update";
        }
        else
        {
            
            objSRN.LastUpdateDate = DateTime.Now;
            if (Session["oUser"] != null)
            {
                userinfo objUser = (userinfo)Session["oUser"];
                objSRN.LastUpdatedBy = objUser.first_name + " " + objUser.last_name;
            }

            lblResult.Text = csCommonUtility.GetSystemMessage("Data has been updated successfully.");
            _db.SubmitChanges();

            UpdateFile();
            lblUpload.Visible = false;
            grdTemp.DataSource = null;
            grdTemp.DataBind();
            BindTempGrid();
            btnSubmit.Text = "Update";

        }


        if (chkEmail.Checked)
        {

            string strBody = CreateHtml();
            string strFrom = string.Empty;
            string strTO = string.Empty;
            ProjectNotesEmailInfo ObjPei = _db.ProjectNotesEmailInfos.FirstOrDefault(p => p.customer_id == Convert.ToInt32(hdnCustomerId.Value));
            string AddtionalEmail = string.Empty;
            //string SalesPersonEmail = string.Empty;
            //string SuperintendentEmail = string.Empty;

            if (ObjPei != null)
            {
                AddtionalEmail = ObjPei.AddtionalEmail;
                //SalesPersonEmail = ObjPei.SalesPersonEmail;
                //SuperintendentEmail = ObjPei.SuperintendentEmail;

            }


            strTO = hdnSalesEmail.Value.ToString();

            if (strTO.Length == 0)
            {
                strTO = hdnSuperandentEmail.Value.ToString();
            }
            else
            {
                if (hdnSuperandentEmail.Value.ToString() != "")
                {
                    strTO = hdnSalesEmail.Value.ToString() + ", " + hdnSuperandentEmail.Value.ToString();
                }

            }
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (AddtionalEmail.Length > 3)
            {
                string[] strIds = AddtionalEmail.Split(',');
                foreach (string strId in strIds)
                {
                    Match match1 = regex.Match(strId.Trim());
                    if (match1.Success)
                    {
                        if (strTO.Length == 0)
                        {
                            strTO = strId;
                        }
                        else
                        {
                            strTO = ", " + strId;
                        }

                    }
                }

            }

            string strBCCEmail = "avijit019@gmail.com";

            if (strTO.Length > 4)
            {


                try
                {
                    userinfo obj = new userinfo();
                    if ((userinfo)Session["oUser"] != null)
                    {
                        obj = (userinfo)Session["oUser"];
                        hdnEmailType.Value = obj.EmailIntegrationType.ToString();
                        strFrom = obj.company_email;

                    }
                    if (Convert.ToInt32(hdnEmailType.Value) == 1) // outlook email
                    {
                        if (strFrom.Trim().Length > 4)
                        {
                            ExchangeService service = EWSAPI.GetEWSService(strFrom.Trim());

                            EmailMessage message = new EmailMessage(service);
                            message.Subject = "Site Review for " + "(" + hdnLastName.Value + ")";
                            message.Body = strBody;

                            if (strTO.Length > 4)
                            {
                                string[] strIds = strTO.Split(',');
                                foreach (string strId in strIds)
                                {
                                    message.ToRecipients.Add(strId.Trim());
                                }
                            }

                            if (strBCCEmail.Length > 4)
                            {
                                string[] strBCCIds = strBCCEmail.Split(',');
                                foreach (string strBCCId in strBCCIds)
                                {
                                    message.BccRecipients.Add(strBCCId.Trim());
                                }
                            }

                            // Create a custom extended property and add it to the message.

                            Guid PropertySetId = Guid.NewGuid();
                            ExtendedPropertyDefinition fazExtendedPropertyDefinition = new ExtendedPropertyDefinition(PropertySetId, "FaztrackPropertyName", MapiPropertyType.String);
                            message.SetExtendedProperty(fazExtendedPropertyDefinition, "FaztrackPropertyName");

                            message.SendAndSaveCopy();

                        }
                    }
                    else
                    {
                        MailMessage msg = new MailMessage();
                        msg.From = new MailAddress("info@interiorinnovations.biz");

                        if (strTO != "")
                            msg.To.Add(strTO);
                        else
                        {
                            return;
                        }
                        if (strBCCEmail.Length > 3)
                        {
                            msg.Bcc.Add(strBCCEmail);
                        }

                        msg.Subject = "Site Review for " + "(" + hdnLastName.Value + ")";

                        msg.IsBodyHtml = true;

                        msg.Body = strBody;
                        msg.Priority = MailPriority.High;

                        csCommonUtility.SendByLocalhost(msg);

                        //SmtpClient smtp = new SmtpClient();
                        //smtp.Host = System.Configuration.ConfigurationManager.AppSettings["smtpserver"];
                        //smtp.Send(msg);

                        msg.Dispose();
                    }
                }
                catch (Exception ex) 
                { 
                    lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message); 
                }

            }

        }


    }

    private void UpdateFile()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            SiteReview_upolad_info objSRU = null;
            SiteReviewNote objSRN = new SiteReviewNote();
            string sFileName = "";

            string DestinationPath = null;
            DestinationPath = Server.MapPath("~/Uploads//Temp//") + hdnCustomerId.Value + "//";
            if (!System.IO.Directory.Exists(DestinationPath))
            {
                System.IO.Directory.CreateDirectory(DestinationPath);
            }
            string sourceFile = Server.MapPath("Uploads\\Temp\\")+hdnCustomerId.Value + @"\";
            string sFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//UPLOAD//";
            string sIMAGESFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//IMAGES//";

            if (!System.IO.Directory.Exists(sIMAGESFolderPath))
            {
                System.IO.Directory.CreateDirectory(sIMAGESFolderPath);
            }
            if (!System.IO.Directory.Exists(sFolderPath))
            {
                System.IO.Directory.CreateDirectory(sFolderPath);
            }

            if (Convert.ToInt32(hdnSiteReviewId.Value) > 0)
            {


                sourceFile = Server.MapPath("Uploads\\Temp\\")+hdnCustomerId.Value + @"\";
                string[] fileEntries1 = Directory.GetFiles(sourceFile);
                foreach (string file in fileEntries1)
                {
                    sFileName = Path.GetFileName(file);
                    objSRU = new SiteReview_upolad_info();
                    objSRU.SiteReviewsId = Convert.ToInt32(hdnSiteReviewId.Value);
                    objSRU.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    objSRU.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                    objSRU.SiteReview_file_name = sFileName;
                    objSRU.create_date = DateTime.Now;
                    if (Session["oUser"] != null)
                    {
                        userinfo objUser = (userinfo)Session["oUser"];
                        objSRN.CreatedBy = objUser.first_name + " " + objUser.last_name;
                    }

                    _db.SiteReview_upolad_infos.InsertOnSubmit(objSRU);
                    _db.SubmitChanges();


                }

                // Move Temp File to Destinition
                string[] fileEntries = Directory.GetFiles(sourceFile);
                if (fileEntries.Count() > 0)
                {
                    if (Convert.ToInt32(hdnSiteReviewId.Value) > 0)
                    {
                        objSRN = _db.SiteReviewNotes.Single(c => c.SiteReviewsId == Convert.ToInt32(hdnSiteReviewId.Value));
                        objSRN.HasAttachments = true;
                        _db.SubmitChanges();
                    }

                    foreach (string file in fileEntries)
                    {
                        sFileName = Path.GetFileName(file);
                        sourceFile = Server.MapPath("Uploads\\Temp\\")+hdnCustomerId.Value + @"\" + sFileName;
                        if (sFileName.Contains(".jpg") || sFileName.Contains(".jpeg") || sFileName.Contains(".png") || sFileName.Contains(".gif"))
                        {
                            File.Move(sourceFile, sIMAGESFolderPath + sFileName);
                        }
                        else
                        {
                            File.Move(sourceFile, sFolderPath + sFileName);
                        }
                    }
                }
                BindImageUploadedDetails(Convert.ToInt32(hdnSiteReviewId.Value));
            }
        }
        catch (Exception ex)
        {
            string s = ex.Message;
        }
    }


    private void MoveFile()
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            SiteReview_upolad_info objSRU = null;
            SiteReviewNote objSRN = new SiteReviewNote();
            string sFileName = "";

            string DestinationPath = null;
            DestinationPath = Server.MapPath("~/Uploads//Temp//") + hdnCustomerId.Value + "//";
            if (!System.IO.Directory.Exists(DestinationPath))
            {
                System.IO.Directory.CreateDirectory(DestinationPath);
            }
            string sourceFile = Server.MapPath("Uploads\\Temp\\")+hdnCustomerId.Value + @"\";
            string sFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//UPLOAD//";
            string sIMAGESFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//IMAGES//";

            if (!System.IO.Directory.Exists(sIMAGESFolderPath))
            {
                System.IO.Directory.CreateDirectory(sIMAGESFolderPath);
            }
            if (!System.IO.Directory.Exists(sFolderPath))
            {
                System.IO.Directory.CreateDirectory(sFolderPath);
            }
            string[] fileEntries1 = Directory.GetFiles(sourceFile);
            foreach (string file in fileEntries1)
            {
                sFileName = Path.GetFileName(file);
                objSRU = new SiteReview_upolad_info();
                objSRU.SiteReviewsId = Convert.ToInt32(hdnSiteReviewId.Value);
                objSRU.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                objSRU.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                objSRU.SiteReview_file_name = sFileName;
                objSRU.create_date = DateTime.Now;
                objSRU.CreatedBy = User.Identity.Name;
                _db.SiteReview_upolad_infos.InsertOnSubmit(objSRU);
                _db.SubmitChanges();


            }
            BindImageUploadedDetails(Convert.ToInt32(hdnSiteReviewId.Value));

            // Move Temp File to Destinition
            string[] fileEntries = Directory.GetFiles(sourceFile);
            if (fileEntries.Count() > 0)
            {
                if (Convert.ToInt32(hdnSiteReviewId.Value) > 0)
                {
                    objSRN = _db.SiteReviewNotes.Single(c => c.SiteReviewsId == Convert.ToInt32(hdnSiteReviewId.Value));
                    objSRN.HasAttachments = true;
                    _db.SubmitChanges();
                }

                foreach (string file in fileEntries)
                {
                    sFileName = Path.GetFileName(file);
                    sourceFile = Server.MapPath("Uploads\\Temp\\")+hdnCustomerId.Value + @"\" + sFileName;
                    if (sFileName.Contains(".jpg") || sFileName.Contains(".jpeg") || sFileName.Contains(".png") || sFileName.Contains(".gif"))
                    {
                        File.Move(sourceFile, sIMAGESFolderPath + sFileName);
                    }
                    else
                    {
                        File.Move(sourceFile, sFolderPath + sFileName);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            string s = ex.Message;
        }
    }

    private void BindImageUploadedDetails(int nSiteReviewId)
    {

        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = string.Empty;
        if (Convert.ToInt32(hdnSiteReviewId.Value) > 0)
        {
            //int nCustomerId = Convert.ToInt32(hdnCustomerId.Value);
            strQ = " select [SiteReview_attach_id], [SiteReviewsId] ,[customer_id],[estimate_id],[SiteReview_file_name] " +
                   " from [SiteReview_upolad_info] where SiteReviewsId=" + nSiteReviewId;
        }
        IEnumerable<csSiteReviewUpload> mList = _db.ExecuteQuery<csSiteReviewUpload>(strQ, string.Empty).ToList();

        if (mList.Count() > 0)
        {
            pnlDetailImageUpload.Visible = true;
            pnlTemporaryImageUpload.Visible = false;
            grdImageDetails.DataSource = mList;
            grdImageDetails.DataKeyNames = new string[] { "SiteReview_attach_id", "SiteReviewsId", "customer_id", "estimate_id", "SiteReview_file_name" };
            grdImageDetails.DataBind();
        }
        else
        {
            grdImageDetails.DataSource = null;
            grdImageDetails.DataBind();
            pnlDetailImageUpload.Visible = false;
        }
    }

    protected void grdImageDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {



                int SiteReview_attach_id = Convert.ToInt32(grdImageDetails.DataKeys[e.Row.RowIndex].Values[0].ToString());
                int SiteReviewsId = Convert.ToInt32(grdImageDetails.DataKeys[e.Row.RowIndex].Values[1].ToString());
                int customer_id = Convert.ToInt32(grdImageDetails.DataKeys[e.Row.RowIndex].Values[2].ToString());
                int estimate_id = Convert.ToInt32(grdImageDetails.DataKeys[e.Row.RowIndex].Values[3].ToString());
                string SiteReview_file_name = grdImageDetails.DataKeys[e.Row.RowIndex].Values[4].ToString();


                string fileName = SiteReview_file_name.Substring(0, 10);
                Label lblFileName1 = (Label)e.Row.FindControl("lblFileName1");
                lblFileName1.Text = fileName;



                ImageButton imgDelete1 = (ImageButton)e.Row.FindControl("imgDelete1");
                imgDelete1.CommandArgument = SiteReview_attach_id.ToString();


                if (SiteReview_file_name.Contains(".jpg") || SiteReview_file_name.Contains(".jpeg") || SiteReview_file_name.Contains(".png") || SiteReview_file_name.Contains(".gif"))
                {
                    HyperLink hypImg1 = (HyperLink)e.Row.FindControl("hypImg1");
                    hypImg1.Visible = true;

                    //hypImg1.NavigateUrl = "UploadedFiles/" + customer_id + "/IMAGES/" + SiteReview_file_name;
                    //hypImg1.Target = "_blank";
                    hypImg1.Attributes.Add("onclick", "window.open('generalsitereview_image_gallery.aspx?gsid=" + SiteReviewsId + "&cid=" + customer_id + "', 'MyWindow', 'left=150,top=100,width=900,height=650,status=0,toolbar=0,resizable=0,scrollbars=1');");
                    Image img1 = (Image)e.Row.FindControl("img1");

                    HyperLink hypPDF1 = (HyperLink)e.Row.FindControl("hypPDF1");
                    hypPDF1.Visible = false;
                    HyperLink hypExcel1 = (HyperLink)e.Row.FindControl("hypExcel1");
                    hypExcel1.Visible = false;
                    HyperLink hypDoc1 = (HyperLink)e.Row.FindControl("hypDoc1");
                    hypDoc1.Visible = false;
                    HyperLink hypTXT1 = (HyperLink)e.Row.FindControl("hypTXT1");
                    hypTXT1.Visible = false;

                    img1.ImageUrl = "UploadedFiles/" + customer_id + "/IMAGES/" + SiteReview_file_name;
                    //img.Attributes.Add("data-zoom-image", "Document/" + SiteReviewsId + "/" + SiteReview_file_name);
                }


                if (SiteReview_file_name.Contains(".pdf"))
                {
                    HyperLink hypImg1 = (HyperLink)e.Row.FindControl("hypImg1");
                    hypImg1.Visible = false;
                    HyperLink hypPDF1 = (HyperLink)e.Row.FindControl("hypPDF1");
                    hypPDF1.Visible = true;

                    HyperLink hypExcel1 = (HyperLink)e.Row.FindControl("hypExcel1");
                    hypExcel1.Visible = false;
                    HyperLink hypDoc1 = (HyperLink)e.Row.FindControl("hypDoc1");
                    hypDoc1.Visible = false;
                    HyperLink hypTXT1 = (HyperLink)e.Row.FindControl("hypTXT1");
                    hypTXT1.Visible = false;

                    Image imgPDF1 = (Image)e.Row.FindControl("imgPDF1");
                    imgPDF1.ImageUrl = "~/images/icon_pdf.png";
                    hypPDF1.NavigateUrl = "UploadedFiles/" + customer_id + "/UPLOAD/" + SiteReview_file_name;
                    hypPDF1.Target = "_blank";
                }
                if (SiteReview_file_name.Contains(".doc") || SiteReview_file_name.Contains(".docx"))
                {
                    HyperLink hypImg1 = (HyperLink)e.Row.FindControl("hypImg1");
                    hypImg1.Visible = false;

                    HyperLink hypPDF1 = (HyperLink)e.Row.FindControl("hypPDF1");
                    hypPDF1.Visible = false;
                    HyperLink hypExcel1 = (HyperLink)e.Row.FindControl("hypExcel1");
                    hypExcel1.Visible = false;
                    HyperLink hypDoc1 = (HyperLink)e.Row.FindControl("hypDoc1");
                    hypDoc1.Visible = true;
                    HyperLink hypTXT1 = (HyperLink)e.Row.FindControl("hypTXT1");
                    hypTXT1.Visible = false;

                    Image imgDoc1 = (Image)e.Row.FindControl("imgDoc1");
                    imgDoc1.ImageUrl = "~/images/icon_docs.png";
                    hypDoc1.NavigateUrl = "UploadedFiles/" + customer_id + "/UPLOAD/" + SiteReview_file_name;
                    hypDoc1.Target = "_blank";

                }
                if (SiteReview_file_name.Contains(".xls") || SiteReview_file_name.Contains(".xlsx") || SiteReview_file_name.Contains(".csv"))
                {
                    HyperLink hypImg1 = (HyperLink)e.Row.FindControl("hypImg1");
                    hypImg1.Visible = false;
                    HyperLink hypPDF1 = (HyperLink)e.Row.FindControl("hypPDF1");
                    hypPDF1.Visible = false;
                    HyperLink hypExcel1 = (HyperLink)e.Row.FindControl("hypExcel1");
                    hypExcel1.Visible = true;
                    HyperLink hypDoc1 = (HyperLink)e.Row.FindControl("hypDoc1");
                    hypDoc1.Visible = false;
                    HyperLink hypTXT1 = (HyperLink)e.Row.FindControl("hypTXT1");
                    hypTXT1.Visible = false;

                    Image imgExcel1 = (Image)e.Row.FindControl("imgExcel1");
                    imgExcel1.ImageUrl = "~/images/icon_excel.png";
                    hypExcel1.NavigateUrl = "UploadedFiles/" + customer_id + "/UPLOAD/" + SiteReview_file_name;
                    hypExcel1.Target = "_blank";
                }
                if (SiteReview_file_name.Contains(".txt") || SiteReview_file_name.Contains(".TXT"))
                {
                    HyperLink hypImg1 = (HyperLink)e.Row.FindControl("hypImg1");
                    hypImg1.Visible = false;
                    HyperLink hypPDF1 = (HyperLink)e.Row.FindControl("hypPDF1");
                    hypPDF1.Visible = false;
                    HyperLink hypExcel1 = (HyperLink)e.Row.FindControl("hypExcel1");
                    hypExcel1.Visible = false;
                    HyperLink hypDoc1 = (HyperLink)e.Row.FindControl("hypDoc1");
                    hypDoc1.Visible = false;
                    HyperLink hypTXT1 = (HyperLink)e.Row.FindControl("hypTXT1");
                    hypTXT1.Visible = true;

                    Image imgTXT1 = (Image)e.Row.FindControl("imgTXT1");
                    imgTXT1.ImageUrl = "~/images/icon_txt.png";
                    hypTXT1.NavigateUrl = "UploadedFiles/" + customer_id + "/UPLOAD/" + SiteReview_file_name;
                    hypTXT1.Target = "_blank";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


    protected void DeleteViewDetailImage(object sender, EventArgs e)
    {

        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string strQ = "";
            lblResult.Text = "";

            string nSiteReview_attach_id = (sender as ImageButton).CommandArgument;
            SiteReview_upolad_info objSRU = _db.SiteReview_upolad_infos.SingleOrDefault(sr => sr.SiteReview_attach_id == Convert.ToInt32(nSiteReview_attach_id));


            if (objSRU.SiteReview_file_name.Contains(".jpg") || objSRU.SiteReview_file_name.Contains(".jpeg") || objSRU.SiteReview_file_name.Contains(".png") || objSRU.SiteReview_file_name.Contains(".gif"))
            {
                string sIMAGESFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//IMAGES//" + objSRU.SiteReview_file_name;
                File.Delete(sIMAGESFolderPath);
            }
            else
            {
                string sFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value + "//UPLOAD//" + objSRU.SiteReview_file_name;
                File.Delete(sFolderPath);
            }
            strQ = "delete from SiteReview_upolad_info where SiteReview_attach_id=" + Convert.ToInt32(nSiteReview_attach_id);
            _db.ExecuteCommand(strQ, string.Empty);


            BindImageUploadedDetails(Convert.ToInt32(hdnSiteReviewId.Value));
        }
        catch (Exception ex)
        {
            throw ex;
        }
      
    }



    protected void btnCancel_Click(object sender, EventArgs e)
    {
        //  Response.Redirect("sitereviewlist.aspx");
        Response.Redirect("sitereviewlist.aspx?cid=" + hdnCustomerId.Value + "&nbackid=" + hdnBackId.Value + "&nestid=" + hdnEstimateId.Value, false);
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnUpload.ID, btnUpload.GetType().Name, "Click"); 
        try
        {
            HttpFileCollection fileCollection = Request.Files;
            string DestinationPath = null;

            DestinationPath = Server.MapPath("~/Uploads//Temp//")+hdnCustomerId.Value+"//";
            if (!System.IO.Directory.Exists(DestinationPath))
            {
                System.IO.Directory.CreateDirectory(DestinationPath);
            }

            for (int i = 0; i < fileCollection.Count; i++)
            {

                HttpPostedFile uploadfile = fileCollection[i];
                string fileName = "";
                string fileExt = Path.GetExtension(uploadfile.FileName);
                if (uploadfile.ContentLength > 0)
                {
                    int iFileSize = uploadfile.ContentLength;
                    if ((iFileSize / (1024 * 2014)) > 2)  // 2MB approx (actually less though)
                    {
                        lblResult.Text = csCommonUtility.GetSystemErrorMessage("File size maximum allowed 2MB");
                        return;
                    }
                    if (uploadfile.FileName.Contains(".jpg") || uploadfile.FileName.Contains(".jpeg") || uploadfile.FileName.Contains(".png") || uploadfile.FileName.Contains(".PNG") || uploadfile.FileName.Contains(".gif") || uploadfile.FileName.Contains(".GIF"))
                    {


                        fileName = uploadfile.FileName.Replace(fileExt, "") + "_" + DateTime.Now.Ticks.ToString() + fileExt;
                        string outputFileName = Path.Combine(DestinationPath, fileName);
                        System.Drawing.Bitmap bmpImage = new System.Drawing.Bitmap(uploadfile.InputStream);
                        System.Drawing.Imaging.ImageFormat format = bmpImage.RawFormat;
                        int newWidth = 1000;
                        int newHeight = 800;
                        System.Drawing.Bitmap bmpOut = null;
                        bmpOut = new System.Drawing.Bitmap(newWidth, newHeight);
                        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmpOut);
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.FillRectangle(System.Drawing.Brushes.White, 0, 0, newWidth, newHeight);
                        g.DrawImage(bmpImage, 0, 0, newWidth, newHeight);
                        bmpImage.Dispose();
                        bmpOut.Save(outputFileName, format);
                        bmpOut.Dispose();

                    }
                    else
                    {
                        fileName = uploadfile.FileName.Replace(fileExt, "") + "_" + DateTime.Now.Ticks.ToString() + fileExt;
                        uploadfile.SaveAs(DestinationPath + fileName);
                    }

                }

            }
            BindTempGrid();
        }
        catch (Exception ex)
        {
            throw ex;
        }

     

    }




    private void BindTempGrid()
    {
        try
        {
            int count = 0;
            DataTable tmpSTable = LoadtmpTable();
            DataRow dr = tmpSTable.NewRow();

            string DestinationPath = Server.MapPath("~/Uploads//Temp//")+hdnCustomerId.Value+"//";
            string[] fileEntries = Directory.GetFiles(DestinationPath);

            foreach (string file in fileEntries)
            {
                string FileName = Path.GetFileName(file);

                DataRow drNew = tmpSTable.NewRow();
                drNew["file_name"] = FileName;

                tmpSTable.Rows.Add(drNew);
                count++;
            }

            grdTemp.DataSource = tmpSTable;
            grdTemp.DataKeyNames = new string[] { "file_name" };
            grdTemp.DataBind();
            if (grdTemp.Rows.Count > 0)
            {
                lblUpload.Visible = true;
                pnlTemporaryImageUpload.Visible = true;

            }
            else
            {
                pnlTemporaryImageUpload.Visible = false;


            }

        }
        catch (Exception ex)
        {
        }
    }
    private DataTable LoadtmpTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("file_name", typeof(string));
        return table;
    }


    // Temporary Image Load 
    protected void grdTemp_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        try
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {


                string file = grdTemp.DataKeys[e.Row.RowIndex].Value.ToString();
                string fileName = file.Substring(0, 10);
                Label lblFileName = (Label)e.Row.FindControl("lblFileName");
                lblFileName.Text = fileName;

                string DestinationPath = Server.MapPath("~/Uploads//Temp//")+hdnCustomerId.Value+"//" + file;
                ImageButton imgDelete = (ImageButton)e.Row.FindControl("imgDelete");
                imgDelete.CommandArgument = DestinationPath;

                if (file.Contains(".jpg") || file.Contains(".jpeg") || file.Contains(".png") || file.Contains(".gif"))
                {
                    Image img = (Image)e.Row.FindControl("img");
                    img.Visible = true;
                    HyperLink hypPDF = (HyperLink)e.Row.FindControl("hypPDF");
                    hypPDF.Visible = false;
                    HyperLink hypExcel = (HyperLink)e.Row.FindControl("hypExcel");
                    hypExcel.Visible = false;
                    HyperLink hypDoc = (HyperLink)e.Row.FindControl("hypDoc");
                    hypDoc.Visible = false;
                    HyperLink hypTXT = (HyperLink)e.Row.FindControl("hypTXT");
                    hypTXT.Visible = false;

                    img.ImageUrl = "Uploads/Temp/"+hdnCustomerId.Value+"/" + file;
                    //img.Attributes.Add("data-zoom-image", "Document/" + SiteReviewsId + "/" + SiteReview_file_name);
                }


                if (file.Contains(".pdf"))
                {
                    Image img = (Image)e.Row.FindControl("img");
                    img.Visible = false;
                    HyperLink hypPDF = (HyperLink)e.Row.FindControl("hypPDF");
                    hypPDF.Visible = true;

                    HyperLink hypExcel = (HyperLink)e.Row.FindControl("hypExcel");
                    hypExcel.Visible = false;
                    HyperLink hypDoc = (HyperLink)e.Row.FindControl("hypDoc");
                    hypDoc.Visible = false;
                    HyperLink hypTXT = (HyperLink)e.Row.FindControl("hypTXT");
                    hypTXT.Visible = false;

                    Image imgPDF = (Image)e.Row.FindControl("imgPDF");
                    imgPDF.ImageUrl = "~/images/icon_pdf.png";
                    hypPDF.NavigateUrl = "Uploads/Temp/" + hdnCustomerId.Value + "/" + file;
                    hypPDF.Target = "_blank";
                }
                if (file.Contains(".doc") || file.Contains(".docx"))
                {
                    Image img = (Image)e.Row.FindControl("img");
                    img.Visible = false;

                    HyperLink hypPDF = (HyperLink)e.Row.FindControl("hypPDF");
                    hypPDF.Visible = false;
                    HyperLink hypExcel = (HyperLink)e.Row.FindControl("hypExcel");
                    hypExcel.Visible = false;
                    HyperLink hypDoc = (HyperLink)e.Row.FindControl("hypDoc");
                    hypDoc.Visible = true;
                    HyperLink hypTXT = (HyperLink)e.Row.FindControl("hypTXT");
                    hypTXT.Visible = false;

                    Image imgDoc = (Image)e.Row.FindControl("imgDoc");
                    imgDoc.ImageUrl = "~/images/icon_docs.png";
                    hypDoc.NavigateUrl = "Uploads/Temp/" + hdnCustomerId.Value + "/" + file;
                    hypDoc.Target = "_blank";

                }
                if (file.Contains(".xls") || file.Contains(".xlsx") || file.Contains(".csv"))
                {
                    Image img = (Image)e.Row.FindControl("img");
                    img.Visible = false;

                    HyperLink hypPDF = (HyperLink)e.Row.FindControl("hypPDF");
                    hypPDF.Visible = false;
                    HyperLink hypExcel = (HyperLink)e.Row.FindControl("hypExcel");
                    hypExcel.Visible = true;
                    HyperLink hypDoc = (HyperLink)e.Row.FindControl("hypDoc");
                    hypDoc.Visible = false;
                    HyperLink hypTXT = (HyperLink)e.Row.FindControl("hypTXT");
                    hypTXT.Visible = false;

                    Image imgExcel = (Image)e.Row.FindControl("imgExcel");
                    imgExcel.ImageUrl = "~/images/icon_excel.png";
                    hypExcel.NavigateUrl = "Uploads/Temp/" + hdnCustomerId.Value + "/" + file;
                    hypExcel.Target = "_blank";
                }
                if (file.Contains(".txt") || file.Contains(".TXT"))
                {
                    Image img = (Image)e.Row.FindControl("img");
                    img.Visible = false;

                    HyperLink hypPDF = (HyperLink)e.Row.FindControl("hypPDF");
                    hypPDF.Visible = false;
                    HyperLink hypExcel = (HyperLink)e.Row.FindControl("hypExcel");
                    hypExcel.Visible = false;
                    HyperLink hypDoc = (HyperLink)e.Row.FindControl("hypDoc");
                    hypDoc.Visible = false;
                    HyperLink hypTXT = (HyperLink)e.Row.FindControl("hypTXT");
                    hypTXT.Visible = true;

                    Image imgTXT = (Image)e.Row.FindControl("imgTXT");
                    imgTXT.ImageUrl = "~/images/icon_txt.png";
                    hypTXT.NavigateUrl = "Uploads/Temp/" + hdnCustomerId.Value + "/" + file;
                    hypTXT.Target = "_blank";
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    protected void DeleteFile(object sender, EventArgs e)
    {
        try
        {
            lblResult.Text = "";

            string filePath = (sender as ImageButton).CommandArgument;
            File.Delete(filePath);
            BindTempGrid();
        }
        catch (Exception ex)
        {
            throw ex;

        }

    }

   
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddNew.ID, btnAddNew.GetType().Name, "Click"); 
        btnSubmit_Click(sender, e);
        ResetAddNewValue();


    }

    private void ResetAddNewValue()
    {
        pnlDetailImageUpload.Visible = false;
        lblResult.Text = "";
        hdnSiteReviewId.Value = "0";
        lblUpload.Visible = false;
        txtSiteReviewNote.Text = "";
        chkCustomer.Checked = false;
        btnSubmit.Text = "Save";
    }
    protected void chkCustomerMind_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkCustomerMind.ID, chkCustomerMind.GetType().Name, "Click"); 

        if (chkCustomerMind.Checked)
        {
            pnlCustomerStateofMind.Visible = true;
        }
        else
        {
            pnlCustomerStateofMind.Visible = false;
        }
    }

    protected void imgbtnAngry_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgbtnAngry.ID, imgbtnAngry.GetType().Name, "Click"); 
        hdnCustomerMind.Value = "1";
        imgbtnAngry.Attributes.Add("class", "opacityimage");
        imgbtnFrustrated.Attributes.Add("class", "");
        imgbtnConfused.Attributes.Add("class", "");
        imgbtnIndifferent.Attributes.Add("class", "");
        imgbtnHappy.Attributes.Add("class", "");

    }
    protected void imgbtnFrustrate_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgbtnFrustrated.ID, imgbtnFrustrated.GetType().Name, "Click"); 

        hdnCustomerMind.Value = "2";
        imgbtnAngry.Attributes.Add("class", "");
        imgbtnFrustrated.Attributes.Add("class", "opacityimage");
        imgbtnConfused.Attributes.Add("class", "");
        imgbtnIndifferent.Attributes.Add("class", "");
        imgbtnHappy.Attributes.Add("class", "");
    }
    protected void imgbtnConfused_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgbtnConfused.ID, imgbtnConfused.GetType().Name, "Click"); 
        hdnCustomerMind.Value = "3";
        imgbtnAngry.Attributes.Add("class", "");
        imgbtnFrustrated.Attributes.Add("class", "");
        imgbtnConfused.Attributes.Add("class", "opacityimage");
        imgbtnIndifferent.Attributes.Add("class", "");
        imgbtnHappy.Attributes.Add("class", "");
    }
    protected void imgbtnIndifferent_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgbtnIndifferent.ID, imgbtnIndifferent.GetType().Name, "Click"); 
        hdnCustomerMind.Value = "4";
        imgbtnAngry.Attributes.Add("class", "");
        imgbtnFrustrated.Attributes.Add("class", "");
        imgbtnConfused.Attributes.Add("class", "");
        imgbtnIndifferent.Attributes.Add("class", "opacityimage");
        imgbtnHappy.Attributes.Add("class", "");
    }
    protected void imgbtnHappay_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgbtnHappy.ID, imgbtnHappy.GetType().Name, "Click"); 
        hdnCustomerMind.Value = "5";
        imgbtnAngry.Attributes.Add("class", "");
        imgbtnFrustrated.Attributes.Add("class", "");
        imgbtnConfused.Attributes.Add("class", "");
        imgbtnIndifferent.Attributes.Add("class", "");
        imgbtnHappy.Attributes.Add("class", "opacityimage");
    }
    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {

        Response.Redirect("sitereviewlist.aspx?cid=" + hdnCustomerId.Value + "&nbackid=" + hdnBackId.Value + "&nestid=" + hdnEstimateId.Value, false);

    }

    string CreateHtml()
    {
        string strHead = "Site Review for " + "(" + hdnLastName.Value + ")";
        string strQ = "SELECT * FROM SiteReviewNotes WHERE SiteReviewsId =" + Convert.ToInt32(hdnSiteReviewId.Value) + " AND customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
        DataTable dtFinal = csCommonUtility.GetDataTable(strQ);

        DataView dvFinal = dtFinal.DefaultView;
        string strFileHTML = "";
        string strHTML = "<br/> <br/>";
        strHTML += "<table width='680' border='0' cellspacing='0'cellpadding='0' align='left'> <tr><td align='left' valign='top'><p style='color:#0066CC; font-size:16px; font-weight:bold; font-family:Georgia, 'Times New Roman', Times, serif; font-style:italic; padding:5px 0 0; margin:0 auto;'>" + strHead + "</p> <table width='100%' border='0' cellspacing='3' cellpadding='5' > <tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <tr style='background:#171f89;'></tr><tr style='background-color:#171f89; color:#FFFFFF; font-weight:bold; font-size:12px; font-family:Arial, Helvetica, sans-serif;'> <td width='10%'>DATE</td><td width='54%'>NOTES</td><td width='18%'>ADDED BY</td> <td width='18%'>STATE OF MIND</td></tr>";

        for (int i = 0; i < dvFinal.Count; i++)
        {
            DataRowView dr = dvFinal[i];

            string str = string.Empty;

            string strF = "SELECT * FROM SiteReview_upolad_info WHERE SiteReviewsId = " + Convert.ToInt32(dr["SiteReviewsId"]) + " AND  customer_id =" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
            DataTable dtFile = csCommonUtility.GetDataTable(strF);
            if (dtFile.Rows.Count > 0)
            {
                strFileHTML = "";
                strFileHTML = "<table> <tr> ";
                foreach (DataRow drf in dtFile.Rows)
                {
                    string imgUrl = "https://ii.faztrack.com/";
                    string strFileName = drf["SiteReview_file_name"].ToString().Replace(" ", "%20");
                    if (strFileName.Contains(".jpg") || strFileName.Contains(".jpeg") || strFileName.Contains(".png") || strFileName.Contains(".gif"))
                    {
                        imgUrl += "UploadedFiles/" + hdnCustomerId.Value + "/IMAGES/" + strFileName;
                        strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'></td>";
                    }
                    else if (strFileName.Contains(".pdf"))
                    {
                        imgUrl += "images/icon_pdf.png";
                        strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'></td>";
                    }
                    else if (strFileName.Contains(".doc") || strFileName.Contains(".docx"))
                    {
                        imgUrl += "images/icon_docs.png";
                        strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'></td>";

                    }
                    else if (strFileName.Contains(".xls") || strFileName.Contains(".xlsx") || strFileName.Contains(".csv"))
                    {
                        imgUrl += "images/icon_excel.png";
                        strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'></td>";

                    }
                    else if (strFileName.Contains(".txt") || strFileName.Contains(".TXT"))
                    {
                        imgUrl += "images/icon_txt.png";
                        strFileHTML += "<td><img width='80px' height='80px' src='" + imgUrl + "'></td>";
                    }
                }
                strFileHTML += "</tr> </table>";

            }
            else
            {
                strFileHTML = "";
            }

            string strTemp = strFileHTML;

            string imgStateUrl = "https://ii.faztrack.com/";
            string strColor = "";

            string StateOfMindID = "";

            if (Convert.ToInt32(dr["StateOfMindID"]) > 0)
            {
                if (Convert.ToInt32(dr["StateOfMindID"]) == 1)
                {
                    imgStateUrl += "/assets/customerstatemind/angry.png";
                    StateOfMindID = " <img width='80px' height='80px' src='" + imgStateUrl + "'> ";
                }
                else if (Convert.ToInt32(dr["StateOfMindID"]) == 2)
                {
                    imgStateUrl += "/assets/customerstatemind/frustrated.png";
                    StateOfMindID = " <img width='80px' height='80px' src='" + imgStateUrl + "'> ";

                }
                else if (Convert.ToInt32(dr["StateOfMindID"]) == 3)
                {
                    imgStateUrl += "/assets/customerstatemind/confused.png";
                    StateOfMindID = " <img width='80px' height='80px' src='" + imgStateUrl + "'> ";

                }
                else if (Convert.ToInt32(dr["StateOfMindID"]) == 4)
                {
                    imgStateUrl += "/assets/customerstatemind/indifferent.png";
                    StateOfMindID = " <img width='80px' height='80px' src='" + imgStateUrl + "'> ";

                }
                else if (Convert.ToInt32(dr["StateOfMindID"]) == 5)
                {
                    imgStateUrl += "/assets/customerstatemind/happy.png";
                    StateOfMindID = " <img width='80px' height='80px' src='" + imgStateUrl + "'> ";

                }
                else
                {
                    StateOfMindID = "";

                }


            }

            if (i % 2 == 0)
                strColor = "background-color:#eee; color:#7d766b; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
            else
                strColor = "background-color:#faf8f6; color:#7d766b; font-weight:normal; font-size:12px; font-family:Arial, Helvetica, sans-serif;";
            if (dr["SiteReviewsDate"].ToString().Length > 0)
            {
                strHTML += "<tr style='" + strColor + "'><td>" + Convert.ToDateTime(dr["SiteReviewsDate"]).ToShortDateString() + "</td><td>" + dr["SiteReviewsNotes"].ToString() + "</td><td>" + dr["CreatedBy"].ToString() + "</td> <td>" + StateOfMindID + "</td></tr>";
                if (strFileHTML.Length > 4)
                {
                    strHTML += "<tr style='" + strColor + "'><td align='left' valign='top' colspan='4'> FILE(S) </td></tr>";
                    strHTML += "<tr style='" + strColor + "'><td align='left' valign='top' colspan='4'>" + strFileHTML + "</td></tr>";
                }
                else
                {
                    strHTML += "<tr style='" + strColor + "'><td align='left' valign='top' colspan='4'>No file attached </td></tr>";

                }
            }

        }
        strHTML += "</table>";
        strHTML += "</table>";
        return strHTML;
    }
}