using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

public partial class ToolSettings : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            LoadMenuList();
        }


    }

    public void LoadMenuList()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var menuList = _db.menu_items.OrderBy(m => m.menu_id).ToList();

        grdMenu.DataSource = menuList;
        grdMenu.DataBind();

        ddlParent.DataSource = menuList.Where(m => m.parent_id == 0).ToList();
        ddlParent.DataTextField = "menu_name";
        ddlParent.DataValueField = "menu_id";
        ddlParent.DataBind();
    }

    protected void btnUpdateCustomerPhoneNumber_Click(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        lblMessage2.Text = "";
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            List<customer> CustomerList = _db.customers.ToList();


            foreach (customer c in CustomerList)
            {
                c.phone = csCommonUtility.GetPhoneFormat(c.phone ?? "");
                c.mobile = csCommonUtility.GetPhoneFormat(c.mobile ?? "");
                c.fax = csCommonUtility.GetPhoneFormat(c.fax ?? "");
            }
            _db.SubmitChanges();

            lblMessage2.Text = csCommonUtility.GetSystemMessage("Customer Data updated successfully.");
        }
        catch (Exception ex)
        {
            lblMessage2.Text = csCommonUtility.GetSystemErrorMessage("Customer: " + ex.Message);
        }
    }

    protected void btnUpdateUserPhoneNumber_Click(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        lblMessage2.Text = "";
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            List<user_info> userList = _db.user_infos.ToList();


            foreach (user_info u in userList)
            {
                u.phone = csCommonUtility.GetPhoneFormat(u.phone ?? "");
                u.fax = csCommonUtility.GetPhoneFormat(u.fax ?? "");
            }
            _db.SubmitChanges();

            lblMessage2.Text = csCommonUtility.GetSystemMessage("User Data updated successfully.");
        }
        catch (Exception ex)
        {
            lblMessage2.Text = csCommonUtility.GetSystemErrorMessage("User: " + ex.Message);
        }
    }

    protected void btnUpdateSalesPersonPhoneNumber_Click(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        lblMessage2.Text = "";
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            List<sales_person> spList = _db.sales_persons.ToList();


            foreach (sales_person s in spList)
            {
                s.phone = csCommonUtility.GetPhoneFormat(s.phone ?? "");
                s.fax = csCommonUtility.GetPhoneFormat(s.fax ?? "");
            }
            _db.SubmitChanges();

            lblMessage2.Text = csCommonUtility.GetSystemMessage("Sales Person Data updated successfully.");
        }
        catch (Exception ex)
        {
            lblMessage2.Text = csCommonUtility.GetSystemErrorMessage("Sales Person: " + ex.Message);
        }
    }


    protected void btnUpdateCustomerAppointment_Click(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        lblMessage2.Text = "";
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            List<ScheduleCalendar> scList = _db.ScheduleCalendars.Where(s => s.type_id == 2).ToList();


            foreach (ScheduleCalendar sc in scList)
            {
                CustomerCallLog custCall = new CustomerCallLog();

                int empID = Convert.ToInt32(_db.customers.SingleOrDefault(c => c.customer_id == sc.customer_id).sales_person_id);

                custCall.CallSubject = sc.title;
                custCall.Description = sc.description;
                custCall.AppointmentDateTime = Convert.ToDateTime(sc.event_start);

                custCall.customer_id = sc.customer_id;
                custCall.CallDate = Convert.ToDateTime(sc.event_start).ToShortDateString();
                custCall.CallHour = Convert.ToDateTime(sc.event_start).ToString("hh", CultureInfo.InvariantCulture);
                custCall.CallMinutes = Convert.ToDateTime(sc.event_start).ToString("mm", CultureInfo.InvariantCulture);
                custCall.CallAMPM = Convert.ToDateTime(sc.event_start).ToString("tt", CultureInfo.InvariantCulture);

                custCall.CallDuration = "0";
                custCall.DurationHour = "0";

                string strCallDateTime = custCall.CallDate + " " + custCall.CallHour + ":" + custCall.CallMinutes + " " + custCall.CallAMPM;

                custCall.DurationMinutes = "0";

                custCall.CreatedByUser = sc.last_updated_by;
                custCall.CreateDate = sc.create_date;
                custCall.CallDateTime = Convert.ToDateTime(strCallDateTime);

                custCall.CallTypeId = 3;
                custCall.IsFollowUp = false;
                custCall.FollowDate = Convert.ToDateTime(sc.event_start).ToShortDateString(); ;
                custCall.FollowHour = Convert.ToDateTime(sc.event_start).ToString("hh", CultureInfo.InvariantCulture);
                custCall.FollowMinutes = Convert.ToDateTime(sc.event_start).ToString("mm", CultureInfo.InvariantCulture);
                custCall.FollowAMPM = Convert.ToDateTime(sc.event_start).ToString("tt", CultureInfo.InvariantCulture);

                string strFollowupDate = custCall.FollowDate + " " + custCall.FollowHour + ":" + custCall.FollowMinutes + " " + custCall.FollowAMPM;

                custCall.FollowDateTime = Convert.ToDateTime(strFollowupDate);
                custCall.IsDoNotCall = false;
                custCall.sales_person_id = empID;


                _db.CustomerCallLogs.InsertOnSubmit(custCall);
            }
            _db.SubmitChanges();

            lblMessage3.Text = csCommonUtility.GetSystemMessage("Data updated successfully.");
        }
        catch (Exception ex)
        {
            lblMessage3.Text = csCommonUtility.GetSystemErrorMessage("Appointment: " + ex.Message);
        }
    }

    protected void btnSend_Click(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/calendar/v3/calendars/faztrackclient@gmail.com/events/watch");
            request.Method = "POST";


            request.ContentType = "application/json";
            string postData = "type=web_hook&id=01234567-89ab-cdef-0123456789ab&address=https://test.ii.faztrack.com/schedulecalendar.aspx";
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = bytes.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            var result = reader.ReadToEnd();
            stream.Dispose();
            reader.Dispose();
        }
        catch (Exception ex)
        {
            lblMessage.Text = ex.Message;
            lblMessage.ForeColor = Color.Red;
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        lblResultMenu.Text = "";

        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            menu_item objmList = new menu_item();

            objmList.menu_code = txtMenuCode.Text.Trim();
            objmList.menu_name = txtMenuName.Text.Trim();
            objmList.parent_id = Convert.ToInt32(ddlParent.SelectedValue);
            objmList.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            objmList.menu_url = txtMenuUrl.Text.Trim();
            objmList.isShow = Convert.ToInt32(ddlisShow.SelectedValue);
            objmList.serial = Convert.ToInt32(txtSerial.Text);

            _db.menu_items.InsertOnSubmit(objmList);
            _db.SubmitChanges();
            lblResultMenu.Text = csCommonUtility.GetSystemMessage("Menu Item Added Successfully");

            LoadMenuList();
        }
        catch (Exception ex)
        {
            lblResultMenu.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void btnFileSaveMove_Click(object sender, EventArgs e)
    {
        lblResultMenu.Text = "";

        try
        {
            int nCustId = 0;
            DataClassesDataContext _db = new DataClassesDataContext();
            string strFileNotFoundList = "";

            // string[] aryFileType = new string[] { "png", "jpg" };

            var objfiles = from f in _db.file_upload_infos
                           //where f.CustomerId == nCustId
                           //&& f.is_design == true
                           //&& f.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                           //&& f.type != 1
                           //&& (f.ImageName.ToString().ToLower().Contains("jpg") || f.ImageName.ToString().ToLower().Contains("png") || f.ImageName.ToString().ToLower().Contains("jpeg"))
                           orderby f.upload_fileId ascending
                           select f;
            if (!chkAll.Checked)
            {
                if (txtCustomerId.Text.Length > 0)
                {
                    objfiles = from f in _db.file_upload_infos
                               where f.CustomerId == Convert.ToInt32(txtCustomerId.Text)
                               //&& f.is_design == true
                               //&& f.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"])
                               //&& f.type != 1
                               //&& (f.ImageName.ToString().ToLower().Contains("jpg") || f.ImageName.ToString().ToLower().Contains("png") || f.ImageName.ToString().ToLower().Contains("jpeg"))
                               orderby f.upload_fileId ascending
                               select f;
                }
                else
                {
                    lblResultMove.Text = csCommonUtility.GetSystemErrorMessage("Customer ID is required");
                    return;
                }
            }
            bool isFileNotFound = false;

            bool isDESIGNDOCSTitleAdded = false;
            bool isSITEPROGRESSTitleAdded = false;
            bool isVENDORTitleAdded = false;
            bool isUPLOADTitleAdded = false;
            bool isSELECTIONSTitleAdded = false;

            int nCustomerId = 0;

            foreach (var file in objfiles)
            {
                if ((bool)file.is_design) ////----------------------------------------------------DESIGN DOCS---------------------------------------
                {

                    string sourceFile = Request.PhysicalApplicationPath + "Document\\" + file.CustomerId + "\\" + file.ImageName;
                    string destinationFile = Server.MapPath("~/UploadedFiles//" + file.CustomerId + "//DESIGN DOCS");

                    if (!System.IO.Directory.Exists(destinationFile))
                    {
                        System.IO.Directory.CreateDirectory(destinationFile);
                    }

                    if (System.IO.File.Exists(sourceFile))
                    {
                        File.Copy(sourceFile, Path.Combine(destinationFile, file.ImageName), true);
                    }
                    else
                    {
                        if (!isDESIGNDOCSTitleAdded)
                        {
                            strFileNotFoundList += Environment.NewLine + "-------------------------------------DESIGN DOCS-------------------------------------------------------------------------------------------------------------------------------------------------" + Environment.NewLine;
                            strFileNotFoundList += "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------" + Environment.NewLine;
                            isDESIGNDOCSTitleAdded = true;

                            isFileNotFound = true;
                        }
                        nCustomerId = (int)file.CustomerId;
                        var objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCustomerId);
                        strFileNotFoundList += "Customer Id: " + file.CustomerId + ", Customer Status: " + objCust.status_id + ", File Name: " + file.ImageName + ", is_design: " + file.is_design + ", IsSiteProgress: " + file.IsSiteProgress + ", type: " + file.type + ", vendor_cost_id: " + file.vendor_cost_id + Environment.NewLine;
                    }
                }
                else if ((bool)file.IsSiteProgress) ////----------------------------------------------SITE PROGRESS---------------------------------------
                {
                    string sourceFile = Request.PhysicalApplicationPath + "Document\\" + file.CustomerId + "\\" + file.ImageName;
                    string destinationFile = Server.MapPath("~/UploadedFiles//" + file.CustomerId + "//SITE PROGRESS");

                    if (!System.IO.Directory.Exists(destinationFile))
                    {
                        System.IO.Directory.CreateDirectory(destinationFile);
                    }

                    if (System.IO.File.Exists(sourceFile))
                    {
                        File.Copy(sourceFile, Path.Combine(destinationFile, file.ImageName), true);
                    }
                    else
                    {
                        if (!isSITEPROGRESSTitleAdded)
                        {
                            strFileNotFoundList += Environment.NewLine + "-------------------------------------SITE PROGRESS------------------------------------------------------------------------------------------------------------------------------------------------" + Environment.NewLine;
                            strFileNotFoundList += "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------" + Environment.NewLine;
                            isSITEPROGRESSTitleAdded = true;

                            isFileNotFound = true;
                        }
                        nCustomerId = (int)file.CustomerId;
                        var objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCustomerId);
                        strFileNotFoundList += "Customer Id: " + file.CustomerId + ", Customer Status: " + objCust.status_id + ", File Name: " + file.ImageName + ", is_design: " + file.is_design + ", IsSiteProgress: " + file.IsSiteProgress + ", type: " + file.type + ", vendor_cost_id: " + file.vendor_cost_id + Environment.NewLine;
                    }
                }

                else if (file.type == 1 && file.vendor_cost_id != 0)  ////----------------------------------VENDOR---------------------------------------
                {
                    vendor_cost objVC = _db.vendor_costs.SingleOrDefault(vc => vc.customer_id == file.CustomerId && vc.vendor_cost_id == file.vendor_cost_id);

                    if (file.upload_fileId == 809)
                    {
                        var filetest = file;
                    }
                    if (objVC != null)
                    {
                        Vendor objVI = _db.Vendors.SingleOrDefault(vi => vi.vendor_id == objVC.vendor_id);

                        if (objVI != null)
                        {
                            string sourceFile = Request.PhysicalApplicationPath + "File\\" + file.ImageName;
                            string destinationFile = Server.MapPath("~/UploadedFiles//" + file.CustomerId + "//VENDOR//" + objVI.vendor_name.Replace("&", "AND"));

                            if (!System.IO.Directory.Exists(destinationFile))
                            {
                                System.IO.Directory.CreateDirectory(destinationFile);
                            }

                            if (System.IO.File.Exists(sourceFile))
                            {
                                File.Copy(sourceFile, Path.Combine(destinationFile, file.ImageName), true);
                            }
                            else
                            {
                                if (!isVENDORTitleAdded)
                                {
                                    strFileNotFoundList += Environment.NewLine + "-------------------------------------VENDOR------------------------------------------------------------------------------------------------------------------------------------------------" + Environment.NewLine;
                                    strFileNotFoundList += "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------" + Environment.NewLine;
                                    isVENDORTitleAdded = true;

                                    isFileNotFound = true;
                                }
                                nCustomerId = (int)file.CustomerId;
                                var objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCustomerId);
                                strFileNotFoundList += "Customer Id: " + file.CustomerId + ", Customer Status: " + objCust.status_id + ", File Name: " + file.ImageName + ", is_design: " + file.is_design + ", IsSiteProgress: " + file.IsSiteProgress + ", type: " + file.type + ", vendor_cost_id: " + file.vendor_cost_id + Environment.NewLine;
                            }

                        }
                    }
                }
                else if (file.type == 5)  ////----------------------------------SELECTIONS---------------------------------------
                {
                    //string sourceFile = Request.PhysicalApplicationPath + "Document\\" + file.CustomerId + "\\" + file.ImageName;
                    //string destinationFile = Server.MapPath("~/UploadedFiles//" + file.CustomerId + "//SELECTIONS");

                    //if (!System.IO.Directory.Exists(destinationFile))
                    //{
                    //    System.IO.Directory.CreateDirectory(destinationFile);
                    //}

                    //if (System.IO.File.Exists(sourceFile))
                    //{
                    //    File.Copy(sourceFile, Path.Combine(destinationFile, file.ImageName), true);
                    //}
                    //else
                    //{
                    //    if (!isSELECTIONSTitleAdded)
                    //    {
                    //        strFileNotFoundList += Environment.NewLine + "-------------------------------------SELECTIONS------------------------------------------------------------------------------------------------------------------------------------------------" + Environment.NewLine;
                    //        strFileNotFoundList += "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------" + Environment.NewLine;
                    //        isSELECTIONSTitleAdded = true;

                    //        isFileNotFound = true;
                    //    }
                    //    nCustomerId = (int)file.CustomerId;
                    //    var objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCustomerId);
                    //    strFileNotFoundList += "Customer Id: " + file.CustomerId + ", Customer Status: " + objCust.status_id + ", File Name: " + file.ImageName + ", is_design: " + file.is_design + ", IsSiteProgress: " + file.IsSiteProgress + ", type: " + file.type + ", vendor_cost_id: " + file.vendor_cost_id + Environment.NewLine;
                    //}
                }
                else////------------------------------------------------------------------------- UPLOAD ---------------------------------------
                {
                    //  string sourceFile = Request.PhysicalApplicationPath + "File\\" + file.CustomerId + "\\" + file.ImageName;

                    string sourceFile = Request.PhysicalApplicationPath + "Document\\" + file.CustomerId + "\\" + file.ImageName;
                    string destinationFile = Server.MapPath("~/UploadedFiles//" + file.CustomerId + "//UPLOAD");

                    string sourceImgFile = Request.PhysicalApplicationPath + "Document\\" + file.CustomerId + "\\" + file.ImageName;
                    string destinationImgFile = Server.MapPath("~/UploadedFiles//" + file.CustomerId + "//IMAGES");

                    if (!System.IO.Directory.Exists(destinationFile))
                    {
                        System.IO.Directory.CreateDirectory(destinationFile);
                    }

                    if (!System.IO.Directory.Exists(destinationImgFile))
                    {
                        System.IO.Directory.CreateDirectory(destinationImgFile);
                    }

                    if (System.IO.File.Exists(sourceImgFile) && (file.ImageName.ToString().ToLower().Contains("jpg") || file.ImageName.ToString().ToLower().Contains("png") || file.ImageName.ToString().ToLower().Contains("jpeg")))
                    {
                        File.Copy(sourceImgFile, Path.Combine(destinationImgFile, file.ImageName), true);
                    }
                    else if (System.IO.File.Exists(sourceFile))
                    {
                        File.Copy(sourceFile, Path.Combine(destinationFile, file.ImageName), true);
                    }
                    else
                    {
                        if (!isUPLOADTitleAdded)
                        {
                            strFileNotFoundList += Environment.NewLine + "-------------------------------------UPLOAD------------------------------------------------------------------------------------------------------------------------------------------------" + Environment.NewLine;
                            strFileNotFoundList += "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------" + Environment.NewLine;
                            isUPLOADTitleAdded = true;

                            isFileNotFound = true;
                        }
                        nCustomerId = (int)file.CustomerId;
                        var objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCustomerId);
                        strFileNotFoundList += "Customer Id: " + file.CustomerId + ", Customer Status: " + objCust.status_id + ", File Name: " + file.ImageName + ", is_design: " + file.is_design + ", IsSiteProgress: " + file.IsSiteProgress + ", type: " + file.type + ", vendor_cost_id: " + file.vendor_cost_id + Environment.NewLine;
                    }


                }


            }
            if (strFileNotFoundList.Length > 0)
            {
                string path = ConfigurationManager.AppSettings["xmlPathToolSettings"].ToString() + "//FileNotFoundNotes_" + nCustomerId + "_" + DateTime.Now.Ticks.ToString() + ".txt";

                if (!File.Exists(path))
                {
                    File.Create(path).Dispose();

                    using (TextWriter tw = new StreamWriter(path))
                    {
                        tw.WriteLine(strFileNotFoundList);
                        tw.Close();
                    }

                }
                else if (File.Exists(path))
                {
                    using (TextWriter tw = new StreamWriter(path))
                    {
                        tw.WriteLine(strFileNotFoundList);
                        tw.Close();
                    }
                }
            }


            string strMessage = csCommonUtility.GetSystemMessage("File has Moved Successfully");

            if (isFileNotFound)
            {
                strMessage = csCommonUtility.GetSystemErrorMessage("Some Files has not Moved, check 'FileNotFoundNotes.txt' for Details in Project Folder");
            }
            lblResultMove.Text = strMessage;

        }
        catch (Exception ex)
        {
            lblResultMove.Text = csCommonUtility.GetSystemErrorMessage(ex.StackTrace);
        }
    }

    protected void btnFileThumbnil_Click(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        try
        {
            string strFileNotFoundList = "";
            string strCustomerId = "";
            bool isThumbnilTitleAdded = false;
            int nCustId = 0;



            string strMessage = csCommonUtility.GetSystemMessage("Thumbnil image saved Successfully");
            if (txtCustomerId.Text.Length > 0)
            {
                nCustId = Convert.ToInt32(txtCustomerId.Text);
            }

            // int[] ctrCustomerIDs = new int[] {0};
            //var objfiles = from f in _db.file_upload_infos
            //               where ctrCustomerIDs.Contains((int)f.CustomerId)
            //               orderby f.CustomerId ascending
            //               select f;

            var objfiles = from f in _db.file_upload_infos
                           orderby f.CustomerId ascending
                           select f;


            if (!chkAll.Checked)
            {
                if (txtCustomerId.Text.Length > 0)
                {
                    objfiles = from f in _db.file_upload_infos
                               where f.CustomerId == Convert.ToInt32(txtCustomerId.Text)
                               orderby f.CustomerId ascending
                               select f;
                }
                else
                {
                    lblResultMove.Text = csCommonUtility.GetSystemErrorMessage("Customer ID is required");
                    return;
                }
            }



            bool isFileNotFound = false;
            foreach (var file in objfiles)
            {
                if (file.type == 5)
                {
                    nCustId = (int)file.CustomerId;
                    if (file.ImageName.ToLower().Contains(".jpg") || file.ImageName.ToLower().Contains(".png") || file.ImageName.ToLower().Contains(".jpeg") || file.ImageName.ToLower().Contains(".gif"))
                    {
                        string sFileThumbnailPath = System.Configuration.ConfigurationManager.AppSettings["UploadDir"] + "\\" + nCustId + "\\SELECTIONS\\Thumbnail";
                        string sFilePath = System.Configuration.ConfigurationManager.AppSettings["DocumentManager_Path"] + nCustId + "\\SELECTIONS\\" + file.ImageName;
                        if (System.IO.File.Exists(sFilePath))
                        {

                            // Thumbnail Image Save

                            ImageUtility.SaveThumbnailImageFromTools(sFilePath, sFileThumbnailPath);


                        }
                        else
                        {
                            if (!isThumbnilTitleAdded)
                            {
                                strFileNotFoundList += Environment.NewLine + "-------------------------------------Thumbnil------------------------------------------------------------------------------------------------------------------------------------------------" + Environment.NewLine;
                                strFileNotFoundList += "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------" + Environment.NewLine;
                                isThumbnilTitleAdded = true;

                                isFileNotFound = true;
                            }
                            nCustId = (int)file.CustomerId;
                            strFileNotFoundList += "Customer Id: :" + file.CustomerId + ", File Name: " + file.ImageName + ", is_design: " + file.is_design + ", IsSiteProgress: " + file.IsSiteProgress + ", type: " + file.type + ", vendor_cost_id: " + file.vendor_cost_id + Environment.NewLine;
                            strCustomerId += file.CustomerId + "," + Environment.NewLine;
                        }
                    }
                }


            }

            if (strFileNotFoundList.Length > 0)
            {
                string path = ConfigurationManager.AppSettings["xmlPathToolSettings"].ToString() + "//FileNotFoundNotes_(Thumbnil)_" + DateTime.Now.Ticks.ToString() + ".txt";
                string pathcustid = ConfigurationManager.AppSettings["xmlPathToolSettings"].ToString() + "//FileNotFoundNotes_Custid_" + DateTime.Now.Ticks.ToString() + ".txt";

                if (!File.Exists(path))
                {
                    File.Create(path).Dispose();

                    using (TextWriter tw = new StreamWriter(path))
                    {
                        tw.WriteLine(strFileNotFoundList);
                        tw.Close();
                    }

                }
                else if (File.Exists(path))
                {
                    using (TextWriter tw = new StreamWriter(path))
                    {
                        tw.WriteLine(strFileNotFoundList);
                        tw.Close();
                    }
                }

                // Customer Id
                if (!File.Exists(pathcustid))
                {
                    File.Create(pathcustid).Dispose();

                    using (TextWriter tw = new StreamWriter(pathcustid))
                    {
                        tw.WriteLine(strCustomerId);
                        tw.Close();
                    }

                }
                else if (File.Exists(pathcustid))
                {
                    using (TextWriter tw = new StreamWriter(pathcustid))
                    {
                        tw.WriteLine(strCustomerId);
                        tw.Close();
                    }
                }
            }


            if (isFileNotFound)
            {
                strMessage = csCommonUtility.GetSystemErrorMessage("Some Thumbnil image has not Saved, check 'FileNotFoundNotes_(Thumbnil)_" + DateTime.Now.Ticks.ToString() + ".txt' for Details in Project Folder");
            }



            lblResultMove.Text = strMessage;
        }
        catch (Exception ex)
        {
            lblResultMove.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void btnSOWUpdate_Click(object sender, EventArgs e)
    {

        //lblSOWResult.Text = "";
        //DataClassesDataContext _db = new DataClassesDataContext();
        //try
        //{
        //    string strCondition = "";
        //    string strRequired = "";

        //    int nCustID = 0;
        //    int nEstID = 0;

        //    if (txtSOWCustomerId.Text.Trim().Length > 0 && txtSOWEstimateId.Text.Trim().Length > 0)
        //    {
        //        try
        //        {
        //            nCustID = Convert.ToInt32(txtSOWCustomerId.Text.Trim());
        //            nEstID = Convert.ToInt32(txtSOWEstimateId.Text.Trim());

        //            strCondition = " WHERE customer_id = " + nCustID + " AND estimate_id =" + nEstID;
        //        }
        //        catch
        //        {
        //            strRequired = "Invalid Customer Id or Estimate Id";
        //        }
        //    }

        //    if (txtCondition.Text.Trim().Length > 0)
        //    {
        //        if (strCondition.Length > 0)
        //            strCondition += " AND " + txtCondition.Text.Trim();
        //        else
        //            strCondition = " WHERE " + txtCondition.Text.Trim();
        //    }

        //    if (strRequired.Length > 0)
        //    {
        //        lblSOWResult.Text = csCommonUtility.GetSystemErrorMessage(strRequired);
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgressCircle", "HideProgressCircle('LoadingProgressCircle');", true);
        //        return;
        //    }


        //    string sqlCOPL = "SELECT * FROM change_order_pricing_list " + strCondition;

        //    List<change_order_pricing_list> listCOPL = _db.ExecuteQuery<change_order_pricing_list>(sqlCOPL, string.Empty).ToList();

        //    foreach (var l in listCOPL)
        //    {
        //        pricing_detail objPD = _db.pricing_details.FirstOrDefault(p => p.item_id == l.item_id && p.item_name == l.item_name
        //                                && p.total_retail_price == l.total_retail_price && p.short_notes == l.short_notes && p.location_id == l.location_id && p.customer_id == l.customer_id && p.estimate_id == l.estimate_id);

        //        co_pricing_master objCPM = _db.co_pricing_masters.FirstOrDefault(p => p.item_id == l.item_id && p.item_name == l.item_name && p.co_pricing_list_id == l.co_pricing_list_id
        //                                      && p.total_retail_price == l.total_retail_price && p.short_notes == l.short_notes && p.location_id == l.location_id && p.customer_id == l.customer_id && p.estimate_id == l.estimate_id);

        //        item_price objITP = _db.item_prices.FirstOrDefault(p => p.item_id == l.item_id);

        //        if (objPD != null) // from pricing_detail
        //        {
        //            l.item_cost = objPD.item_cost;
        //            l.labor_rate = objPD.labor_rate;
        //            l.retail_multiplier = objPD.retail_multiplier;

        //            l.labor_id = objPD.labor_id;
        //            l.minimum_qty = objPD.minimum_qty;
        //        }
        //        else if (objCPM != null) // from co_pricing_master
        //        {
        //            l.item_cost = objCPM.item_cost;
        //            l.labor_rate = objCPM.labor_rate;
        //            l.retail_multiplier = objCPM.retail_multiplier;

        //            l.labor_id = objCPM.labor_id;
        //            l.minimum_qty = objCPM.minimum_qty;
        //        }
        //        else if (objITP != null) // from item_price
        //        {
        //            l.item_cost = objITP.item_cost;
        //            l.labor_rate = objITP.labor_rate;
        //            l.retail_multiplier = objITP.retail_multiplier;

        //            l.labor_id = objITP.labor_id;
        //            l.minimum_qty = objITP.minimum_qty;
        //        }
        //        else if (l.item_name.Trim().ToLower().Contains(">>other")) // for other item
        //        {
        //            decimal dTPrice = 0;
        //            decimal nItem_Cost = 0;

        //            if (l.is_direct == 1)
        //                dTPrice = Convert.ToDecimal(l.total_retail_price);
        //            else
        //                dTPrice = Convert.ToDecimal(l.total_direct_price);

        //            if (dTPrice != 0 || l.quantity != 0)
        //            {
        //                nItem_Cost = (dTPrice / (decimal)l.quantity) / 2;
        //            }


        //            l.item_cost = nItem_Cost;
        //            l.labor_rate = 0;
        //            l.retail_multiplier = 0;

        //            l.labor_id = 0;
        //            l.minimum_qty = 0;
        //        }
        //        _db.SubmitChanges();
        //    }


        //    lblSOWResult.Text = csCommonUtility.GetSystemMessage("Data Updated Successfuly");
        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgressCircle", "HideProgressCircle('LoadingProgressCircle');", true);
        //}
        //catch (Exception ex)
        //{
        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgressCircle", "HideProgressCircle('LoadingProgressCircle');", true);
        //    lblSOWResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        //}
    }

    protected void chkSOWALL_CheckedChanged(object sender, EventArgs e)
    {
        txtSOWCustomerId.Text = "";
        txtSOWEstimateId.Text = "";
        txtCondition.Text = "";
    }

    protected void btnRunSql_Click(object sender, EventArgs e)
    {
        lblSOWResult.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        try
        {
            string strSql = "";
            string strRequired = "";


            if (txtSql.Text.Trim().Length > 0)
            {
                strSql = txtSql.Text.Trim();

                if (strSql.ToLower().Contains("delete") || strSql.ToLower().Contains("update") || strSql.ToLower().Contains("truncate")
                    || strSql.ToLower().Contains("drop") || strSql.ToLower().Contains("create") || strSql.ToLower().Contains("backup") || strSql.ToLower().Contains("alter"))
                {
                    strRequired = "delete, update, truncate, drop, create, backup, alter actions are blocked";
                }
            }
            else
            {
                strRequired = "sql query is required";
            }

            if (strRequired.Length > 0)
            {
                lblResultSql.Text = csCommonUtility.GetSystemErrorMessage(strRequired);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgressCircle", "HideProgressCircle('LoadingProgressCircle1');", true);
                return;
            }

            var list = DataReader.Complex_Read_DataTable(strSql);
            lblTotalSMS.Text = "Total : " + list.Rows.Count;
            lblResultSql.Text = csCommonUtility.GetSystemMessage("Sql Run Successfuly. " + list.Rows.Count + " Rows found");
            grdOutput.DataSource = list;
            grdOutput.DataBind();


            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgressCircle", "HideProgressCircle('LoadingProgressCircle1');", true);
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgressCircle", "HideProgressCircle('LoadingProgressCircle1');", true);
            lblResultSql.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void btnClearSql_Click(object sender, EventArgs e)
    {
        txtSql.Text = "";
        lblResultSql.Text = "";

        grdOutput.DataSource = null;
        grdOutput.DataBind();
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgressCircle", "HideProgressCircle('LoadingProgressCircle1');", true);
    }

    protected void btnUpdateLatLong_Click(object sender, EventArgs e)
    {
        lblSOWResult.Text = "";

        try
        {
            updateCustomersLatLng();
            lblLatLongResult.Text = csCommonUtility.GetSystemMessage("Customers Lat Long Updated Successfuly.");



            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgressCircle", "HideProgressCircle('LoadingProgressCircle2');", true);
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgressCircle", "HideProgressCircle('LoadingProgressCircle2');", true);
            lblLatLongResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    private void updateCustomersLatLng()
    {
        XDocument xdoc;
        WebResponse response = null;
        DataClassesDataContext _db = new DataClassesDataContext();
        List<customer> customresForUpdate = new List<customer>(); ;
        if (System.Web.HttpContext.Current.Session["cSearch"] != null)
        {
            customresForUpdate = (List<customer>)System.Web.HttpContext.Current.Session["cSearch"];
        }
        try
        {

            foreach (var c in customresForUpdate)
            {
                customer Cuustomer = _db.customers.SingleOrDefault(x => x.customer_id == c.customer_id);


                if (Cuustomer.customer_id != 1)
                {
                    string address = Cuustomer.address.Replace(".", "").Replace("#", "");
                    // string address = "2401 E Rio Salado PKWY  UNIT 1085";
                    string fulladdress = address + "," + Cuustomer.city.Trim() + "," + Cuustomer.state.Trim() + " " + Cuustomer.zip_code;
                    //  string fulladdress = address;

                    string url = String.Format("https://maps.googleapis.com/maps/api/geocode/xml?address=" +
                        fulladdress + "&key=AIzaSyA6TSREURsg3auq7IP_bxgrl0Q_yHJPHc0");

                    WebRequest request = WebRequest.Create(url);
                    request.UseDefaultCredentials = true;
                    try
                    {
                        response = request.GetResponse();

                        xdoc = XDocument.Load(response.GetResponseStream());

                        XElement result = xdoc.Element("GeocodeResponse").Element("result");
                        XElement locationElement = result.Element("geometry").Element("location");
                        XElement Latitude = locationElement.Element("lat");
                        XElement Longitude = locationElement.Element("lng");

                        string lat = (string)Latitude.Value;
                        string lang = (string)Longitude.Value;
                        Cuustomer.Latitude = lat;
                        Cuustomer.Longitude = lang;

                    }
                    catch
                    {
                        response = null;
                    }




                    if (response != null)
                    {
                        _db.SubmitChanges();
                    }

                }

            }





        }
        catch (Exception ex)
        {
            Console.WriteLine("There is an error . Because " + ex);

        }





    }


    protected void btnUpdateMasterUser_Click(object sender, EventArgs e)
    {
        try
        {
            MasterDataClassesDataContext _mdb = new MasterDataClassesDataContext();


            DataClassesDataContext _db = new DataClassesDataContext();
            List<user_info> uinfo = _db.user_infos.ToList();

            List<Crew_Detail> crinfo = _db.Crew_Details.ToList();


            //List<MasterUserInfo> lMuinfo = _mdb.MasterUserInfos.ToList();

            //foreach (var mu in lMuinfo)
            //{
            //    mu.phone = csCommonUtility.ExtractNumber(mu.phone);
            //    _mdb.SubmitChanges();
            //}

            // Master User Info Insert

            foreach (var u in uinfo)
            {
                var objMUInfo = _mdb.MasterUserInfos.Where(m => m.client_id == 2 && (m.email == u.email && m.password == u.password));

                if (objMUInfo.Count() == 0)
                {
                    MasterUserInfo muinfo = new MasterUserInfo();
                    muinfo.client_id = 2; // Client 2 for IICEM
                    muinfo.user_type = 1; // user
                    muinfo.first_name = u.first_name;
                    muinfo.last_name = u.last_name;
                    muinfo.email = u.email;
                    muinfo.phone = csCommonUtility.ExtractNumber(u.phone);
                    muinfo.password = u.password;
                    muinfo.is_active = u.is_active;
                    muinfo.create_date = u.create_date;
                    muinfo.user_id = u.user_id;
                    _mdb.MasterUserInfos.InsertOnSubmit(muinfo);
                    _mdb.SubmitChanges();
                }
            }

            foreach (var c in crinfo)
            {
                string CrewPhone = csCommonUtility.ExtractNumber(c.phone);
                var objMUInfo = _mdb.MasterUserInfos.Where(m => m.client_id == 2 && (m.password == c.password && m.phone == CrewPhone));

                if (objMUInfo.Count() == 0)
                {
                    MasterUserInfo muinfo = new MasterUserInfo();
                    muinfo.client_id = 2; // Client 2 for IICEM
                    muinfo.user_type = 2; // Crew
                    muinfo.first_name = c.first_name;
                    muinfo.last_name = c.last_name;
                    muinfo.email = c.email;
                    muinfo.phone = CrewPhone;
                    muinfo.password = c.password;
                    muinfo.is_active = c.is_active;
                    muinfo.create_date = c.CreatedDate;
                    muinfo.user_id = c.crew_id;
                    _mdb.MasterUserInfos.InsertOnSubmit(muinfo);
                    _mdb.SubmitChanges();
                }
            }


            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgressCircle", "HideProgressCircle('LoadingProgressCircle1');", true);
            lblMsgUpdateMasterUser.Text = csCommonUtility.GetSystemMessage("Data Updated Successfully");

        }
        catch (Exception ex)
        {
            lblMsgUpdateMasterUser.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgressCircle", "HideProgressCircle('LoadingProgressCircle1');", true);
        }
    }


}