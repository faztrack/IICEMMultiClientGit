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
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Net.Mail;
using System.Web.Script.Serialization;

public partial class schedulecalendar : System.Web.UI.Page
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
                hdnPrimaryDivision.Value = oUser.primaryDivision.ToString();
                hdnClientId.Value = oUser.client_id.ToString();
                hdnDivisionName.Value = oUser.divisionName;
            }



            //if (Page.User.IsInRole("Call003") == false)
            //{
            //    // No Permission Page.
            //    Response.Redirect("nopermission.aspx");
            //}
            HttpContext.Current.Session.Add("sCreatedBy", User.Identity.Name);

            //ReSetDateDiff();

            //Clear Search

            if (HttpContext.Current.Session["sSetDivisionIdOnCahnge"] != null)
            {
                hdnPrimaryDivision.Value = HttpContext.Current.Session["sSetDivisionIdOnCahnge"].ToString();
            }

            HttpContext.Current.Session["sSetDivisionIdOnCahnge"] = null;

            HttpContext.Current.Session.Add("sClientId", Convert.ToInt32(hdnPrimaryDivision.Value));
            HttpContext.Current.Session.Add("CusId", 0);
            HttpContext.Current.Session.Add("sSecName", "");
            HttpContext.Current.Session.Add("sKeySearchUserName", "");
            HttpContext.Current.Session.Add("sKeySearchSuperintendentName", "");

            BindDivision(hdnDivisionName.Value);

            userinfo objUName = (userinfo)Session["oUser"];
            string strUName = objUName.first_name;

            int nCustomerID = 0;
            int nEstimateID = 0;
            int nEmployeeID = 0;
            int nTypeId = 0;

            DataClassesDataContext _db = new DataClassesDataContext();
            customer objCust = new customer();
            customer_estimate cus_est = new customer_estimate();
            ScheduleCalendar objSC = new ScheduleCalendar();
            co_pricing_master objCOPM = new co_pricing_master();
            location objLocation = new location();

            company_profile objComp = _db.company_profiles.SingleOrDefault(c => c.client_id == 1); //  Do not change it

            System.Web.HttpContext.Current.Session.Add("sUnassignedCheck", (bool)objComp.IsScheduleUnassignedCheck);

            string strEventName = "";
            string serviceColor = "fc-default";
            string strEstimateName = "";
            string strCustName = "";
            string strEmpInitial = "";
            string strNotes = "";
            nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeID"));
            HttpContext.Current.Session.Add("TypeID", nTypeId);
            if (nTypeId == 1)
            {
                lbltopHead.Text = "Operation Calendar";
                tblSearch.Visible = true;
                hdnCalStateAction.Value = "false";
                if (Request.QueryString.Get("cid") == null && Request.QueryString.Get("eid") == null)
                {
                    if (System.Web.HttpContext.Current.Session["cid"] != null)
                    {
                        hdnCustIDSelected.Value = System.Web.HttpContext.Current.Session["cid"].ToString();
                        nCustomerID = (int)System.Web.HttpContext.Current.Session["cid"];
                    }

                    if (System.Web.HttpContext.Current.Session["eid"] != null)
                    {
                        hdnEstIDSelected.Value = System.Web.HttpContext.Current.Session["eid"].ToString();
                        nEstimateID = (int)System.Web.HttpContext.Current.Session["eid"];
                    }



                }
            }
            if (nTypeId == 2)
            {
                lbltopHead.Text = "Sales Calendar";
                tblSearch.Visible = false;
                hdnCalStateAction.Value = "false";
                Session.Add("sIsCalendarOnline", false);
            }
            if (Request.QueryString.Get("cid") != null && Request.QueryString.Get("eid") != null) // Customer Schedule
            {
                trDivision.Visible = false;
                trSearchCal.Visible = false;
                trCalStateAction.Visible = true;
                trProjects.Visible = true;
                trSectionsList.Visible = true;
                tblSectionDragDrop.Visible = true;
                nCustomerID = Convert.ToInt32(Request.QueryString.Get("cid"));
                nEstimateID = Convert.ToInt32(Request.QueryString.Get("eid"));
                nEmployeeID = Convert.ToInt32(Request.QueryString.Get("empid"));
                nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeID"));

                hypSOW.NavigateUrl = "composite_workoerder.aspx?cid=" + nCustomerID + "&eid=" + nEstimateID;

                hdnEstIDSelected.Value = nEstimateID.ToString();
                hdnCustIDSelected.Value = nCustomerID.ToString();

                tdTitleHeaderTool.Visible = true;


                BindDragTemplateMainSectionList(nCustomerID, nEstimateID);

                HttpContext.Current.Session.Add("uname", strUName);
                HttpContext.Current.Session.Add("CustSelected", nCustomerID);
                HttpContext.Current.Session.Add("EstSelected", nEstimateID);

                string strQ = "SELECT * FROM customer_estimate WHERE customer_id=" + nCustomerID + " AND status_id = 3 " ;
                IEnumerable<customer_estimate> list = _db.ExecuteQuery<customer_estimate>(strQ, string.Empty);

                //ddlEst.DataSource = list;
                //ddlEst.DataTextField = "estimate_name";
                //ddlEst.DataValueField = "estimate_id";
                //ddlEst.DataBind();

                chkEst.DataSource = list;
                chkEst.DataTextField = "estimate_name";
                chkEst.DataValueField = "estimate_id";
                chkEst.DataBind();

                foreach (ListItem item in chkEst.Items)
                {
                    item.Selected = true;

                    SaveSectionCOPricingMaster(nCustomerID, Convert.ToInt32(item.Value));
                }

                IEnumerable<int> listCheckedEstId = SetSessionEstimate();

                BindEstimate(nCustomerID, listCheckedEstId);

                Session.Add("CustomerId", nCustomerID);



                btnBack.Text = "Return to Customer List";// Schedule (" + strCustName + " - " + strEstimateName + ")";
              


                if (_db.customers.Where(c => c.customer_id == nCustomerID).Count() > 0)
                {
                    objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCustomerID);
                    strCustName = objCust.first_name1 + " " + objCust.last_name1;

                    lbltopHead.Text = "Operation Calendar (" + strCustName + ")";

                    hdnCalStateAction.Value = objCust.isCalendarOnline.ToString().ToLower();
                    Session.Add("sIsCalendarOnline", objCust.isCalendarOnline);

                    rdoconfirm.SelectedValue = Convert.ToInt32(objCust.CustomerCalendarWeeklyView).ToString();

                    if ((bool)objCust.isCalendarOnline)
                    {
                        tblProjectStartDate.Visible = true;
                        lblCalState.Text = "Calendar is Online";
                        lblCalState.BackColor = Color.Green;
                        lblCalState.ForeColor = Color.White;

                        btnCalStateAction.Text = "Go Offline";

                        //btnCalStateAction.BorderColor = Color.Red;

                        if (_db.customer_estimates.Where(sc => sc.customer_id == nCustomerID).Count() != 0)
                        {

                            var dtStartOfJob = _db.customer_estimates.FirstOrDefault(c => c.customer_id == nCustomerID).job_start_date ?? "";
                            if (dtStartOfJob != "")
                                txtProjectStartDate.Text = Convert.ToDateTime(dtStartOfJob).ToShortDateString();

                        }

                        HttpContext.Current.Session.Add("cid", nCustomerID);
                        HttpContext.Current.Session.Add("eid", nEstimateID);
                        HttpContext.Current.Session.Add("empid", nEmployeeID);
                        HttpContext.Current.Session.Add("TypeID", nTypeId);


                        hdnAddEventName.Value = "";
                        hdnEventDesc.Value = "";// objCOPM.short_notes;
                        hdnEstimateID.Value = nEstimateID.ToString();
                        hdnCustomerID.Value = nCustomerID.ToString();
                        hdnEmployeeID.Value = nEmployeeID.ToString();
                        hdnTypeID.Value = nTypeId.ToString();

                        if (nTypeId == 1)
                            serviceColor = "fc-contract";
                        else if (nTypeId == 2)
                            serviceColor = "fc-ticket";
                        else if (nTypeId == 3)
                            serviceColor = "fc-sales";
                        else
                            serviceColor = "fc-default";

                        hdnServiceCssClass.Value = serviceColor;
                        btnBack.Text = "Return to Customer List";// Schedule (" + strCustName + " - " + strEstimateName + ")";

                    }
                    else
                    {
                        tblProjectStartDate.Visible = true;
                        lblCalState.Text = "Calendar is Offline";
                        lblCalState.BackColor = Color.Red;
                        lblCalState.ForeColor = Color.White;

                        btnCalStateAction.Text = "Go Online";

                        //btnCalStateAction.BorderColor = Color.Green;

                        string date = "";



                        if (_db.ScheduleCalendarTemps.Where(sc => sc.customer_id == nCustomerID && sc.estimate_id == nEstimateID).Count() != 0)
                        {
                            date = (_db.ScheduleCalendarTemps.Where(c => c.customer_id == nCustomerID && c.estimate_id == nEstimateID && c.event_start >= DateTime.Now).Min(x => x.event_start)).ToString();

                        }
                        if (_db.customer_estimates.Where(sc => sc.customer_id == nCustomerID).Count() != 0)
                        {

                            var dtStartOfJob = _db.customer_estimates.FirstOrDefault(c => c.customer_id == nCustomerID).job_start_date ?? "";
                            if (dtStartOfJob != "")
                                txtProjectStartDate.Text = Convert.ToDateTime(dtStartOfJob).ToShortDateString();

                        }


                        HttpContext.Current.Session.Add("cid", nCustomerID);
                        HttpContext.Current.Session.Add("eid", nEstimateID);
                        HttpContext.Current.Session.Add("empid", nEmployeeID);
                        HttpContext.Current.Session.Add("TypeID", nTypeId);

                        //  hdnAddEventName.Value = (strEmpInitial + " " + strEstimateName + " - " + strCustName).Trim();
                        hdnAddEventName.Value = "";
                        hdnEventDesc.Value = "";// objCOPM.short_notes;
                        hdnEstimateID.Value = nEstimateID.ToString();
                        hdnCustomerID.Value = nCustomerID.ToString();
                        hdnEmployeeID.Value = nEmployeeID.ToString();
                        hdnTypeID.Value = nTypeId.ToString();

                        if (nTypeId == 1)
                            serviceColor = "fc-contract";
                        else if (nTypeId == 2)
                            serviceColor = "fc-ticket";
                        else if (nTypeId == 3)
                            serviceColor = "fc-sales";
                        else
                            serviceColor = "fc-default";

                        hdnServiceCssClass.Value = serviceColor;



                        hdnEventStartDate.Value = date.ToString();
                        btnBack.Visible = true;
                        btnBack.Text = "Return to Customer List";// Schedule (" + strCustName + " - " + strEstimateName + ")";



                    }


                }


            }
            else if (Request.QueryString.Get("cid") != null) // Customer sales
            {
                trCalStateAction.Visible = false;
                nCustomerID = Convert.ToInt32(Request.QueryString.Get("cid"));
                nEmployeeID = Convert.ToInt32(Request.QueryString.Get("empid"));
                nTypeId = Convert.ToInt32(Request.QueryString.Get("TypeID"));

                bool IsNewCall = true;
                int nCallLogID = 0;

                if (Request.QueryString.Get("IsNewCall") != null)
                    IsNewCall = Convert.ToBoolean(Request.QueryString.Get("IsNewCall"));

                if (Request.QueryString.Get("CallLogID") != null)
                    nCallLogID = Convert.ToInt32(Request.QueryString.Get("CallLogID"));

                hdnEstIDSelected.Value = nEstimateID.ToString();
                hdnCustIDSelected.Value = nCustomerID.ToString();


                HttpContext.Current.Session.Add("uname", strUName);
                HttpContext.Current.Session.Add("CustSelected", nCustomerID);
                HttpContext.Current.Session.Add("EstSelected", nEstimateID);
                HttpContext.Current.Session.Add("eid", nCallLogID);

                HttpContext.Current.Session.Add("cid", nCustomerID);

                if (_db.customers.Where(c => c.customer_id == nCustomerID).Count() > 0)
                {
                    objCust = _db.customers.FirstOrDefault(c => c.customer_id == nCustomerID);
                    var emp = _db.sales_persons.FirstOrDefault(em => em.sales_person_id == objCust.sales_person_id);

                    strCustName = objCust.last_name1;

                    HttpContext.Current.Session.Add("empid", objCust.sales_person_id);
                    //strEventName = (objCust.last_name1 + " (" + emp.first_name + " " + emp.last_name + ")").Trim();
                    //strCustName = objCust.last_name1;
                    //strNotes = objCust.notes ?? "";


                    CustomerCallLog custCall = new CustomerCallLog();
                    if (nCallLogID != 0)
                    {

                        custCall = _db.CustomerCallLogs.Single(c => c.customer_id == nCustomerID && c.CallTypeId == 3 && c.CallLogID == nCallLogID);

                        strEventName = custCall.CallSubject.Trim();
                        strNotes = custCall.Description ?? "";
                    }
                }

                if (_db.ScheduleCalendars.Where(sc => sc.customer_id == nCustomerID && sc.type_id == nTypeId).Count() == 0)
                {
                    HttpContext.Current.Session.Add("cid", nCustomerID);
                    HttpContext.Current.Session.Add("eid", nCallLogID);
                    HttpContext.Current.Session.Add("empid", nEmployeeID);
                    HttpContext.Current.Session.Add("TypeID", nTypeId);

                    hdnAddEventName.Value = strEventName;
                    hdnEventDesc.Value = strNotes;
                    hdnEstimateID.Value = nCallLogID.ToString();
                    hdnCustomerID.Value = nCustomerID.ToString();
                    hdnEmployeeID.Value = nEmployeeID.ToString();
                    hdnTypeID.Value = nTypeId.ToString();

                    if (nTypeId == 1)
                        serviceColor = "fc-contract";
                    else if (nTypeId == 2)
                        serviceColor = "fc-ticket";
                    else if (nTypeId == 3)
                        serviceColor = "fc-sales";
                    else
                        serviceColor = "fc-default";

                    hdnServiceCssClass.Value = serviceColor;
                    btnBack.Visible = true;
                    btnBack.Text = "Return to  Customer List";//  (" + strCustName + ")";
                }
                else
                {
                    var date = _db.ScheduleCalendars.Where(c => c.customer_id == nCustomerID && c.type_id == 2).FirstOrDefault().event_start;
                    //HttpContext.Current.Session.Add("cid", null);
                    //HttpContext.Current.Session.Add("eid", null);
                    //HttpContext.Current.Session.Add("empid", null);
                    //HttpContext.Current.Session.Add("TypeID", null);
                    hdnAddEventName.Value = "";
                    hdnEventDesc.Value = "";
                    hdnEstimateID.Value = "";
                    hdnCustomerID.Value = "";
                    hdnEmployeeID.Value = "";
                    hdnTypeID.Value = "22";
                    hdnEventStartDate.Value = date.ToString();
                    btnBack.Visible = true;
                    btnBack.Text = "Return to Customer List";//  (" + strCustName + ")";
                }
            }
            else
            {
                trCalStateAction.Visible = false;
                HttpContext.Current.Session.Add("cid", null);
                HttpContext.Current.Session.Add("eid", null);
                HttpContext.Current.Session.Add("empid", null);
                //HttpContext.Current.Session.Add("TypeID", null);
                HttpContext.Current.Session.Add("CustSelected", null);
                HttpContext.Current.Session.Add("EstSelected", null);
                hdnAddEventName.Value = "";
                hdnEventDesc.Value = "";
                hdnEstimateID.Value = "";
                hdnCustomerID.Value = "";
                hdnEmployeeID.Value = "";
                hdnTypeID.Value = nTypeId.ToString();
            }


        }


    }

    private void BindDivision(string divisionName)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        List<division> listDiv = new List<division>();

        string[] aryDiv = divisionName.Trim().Split(',').Select(p => p.Trim().ToLower()).ToArray();

        var itemList = _db.divisions.Where(d => aryDiv.Contains(d.division_name.Trim().ToLower()));

        if (itemList.Any())
            listDiv = itemList.OrderBy(d => d.division_name).ToList();

        ddlDivision.DataSource = listDiv;
        ddlDivision.DataTextField = "division_name";
        ddlDivision.DataValueField = "id";
        ddlDivision.DataBind();
        ddlDivision.SelectedValue = hdnPrimaryDivision.Value;
    }

    //protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    //{

    //}

    protected void btnBack_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnBack.ID, btnBack.GetType().Name, "Click");

        Response.Redirect("customerlist.aspx");
    }

    protected void btnSaveProjectStartDate_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnBack.ID, btnBack.GetType().Name, "Click");

        DateTime dtProjectStartDate = new DateTime();
        if (txtProjectStartDate.Text != "")
        {
            try
            {
                dtProjectStartDate = Convert.ToDateTime(txtProjectStartDate.Text);
            }
            catch
            {
                return;
            }
        }
        else
        {
            return;
        }

        DataClassesDataContext _db = new DataClassesDataContext();
        if (_db.customer_estimates.Where(sc => sc.customer_id == Convert.ToInt32(hdnCustomerID.Value)).Count() != 0)
        {
            customer_estimate objcusest = _db.customer_estimates.FirstOrDefault(sc => sc.customer_id == Convert.ToInt32(hdnCustomerID.Value));

            objcusest.job_start_date = dtProjectStartDate.ToShortDateString();

            _db.SubmitChanges();
        }
    }

    protected void btnSalesCalendar_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("schedulecalendar.aspx?TypeID=2");
    }

    protected void btnOperationCalendar_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("schedulecalendar.aspx?TypeID=1");
    }

    protected void btnCalStateAction_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnCalStateAction.ID, btnCalStateAction.GetType().Name, "Click");
        DataClassesDataContext _db = new DataClassesDataContext();

        int nCustId = Convert.ToInt32(hdnCustIDSelected.Value);
        // int nEstid = Convert.ToInt32(hdnEstIDSelected.Value);

        customer objcpmList = _db.customers.SingleOrDefault(c => c.customer_id == nCustId);

        IEnumerable<int> listCheckedEstId = chkEst.Items
                                      .Cast<ListItem>()
                                      .Where(item => item.Selected)
                                      .OrderBy(item => item.Selected)
                                      .Select(item => int.Parse(item.Value));
        string EstimateIds = "";
        foreach (ListItem l in chkEst.Items)
        {
            if (l.Selected)
            {
                EstimateIds += l.Value + ",";
            }
        }

        if (btnCalStateAction.Text == "Go Offline") // if Calendar Offline
        {
            if (_db.ScheduleCalendars.Any(sc => sc.customer_id == nCustId && listCheckedEstId.Contains((int)sc.estimate_id))) // if Data available in Online (ScheduleCalendar) Table then  processed 
            {
                if (!_db.ScheduleCalendarTemps.Any(sc => sc.customer_id == nCustId && listCheckedEstId.Contains((int)sc.estimate_id)))  // if Data NOT available in OFFLINE (ScheduleCalendarTemp) Table then  processed 
                {
                    //Insert
                    string sSqlINSERT = "INSERT INTO [ScheduleCalendarTemp] " +
                                    " SELECT [title],[description],[event_start],[event_end],[customer_id],[estimate_id],[employee_id],[section_name], " +
                                    " [location_name],[create_date],[last_updated_date],[last_updated_by],[type_id],[parent_id],[job_start_date], " +
                                    " [co_pricing_list_id],[cssClassName],[google_event_id],[operation_notes],[is_complete],[IsEstimateActive],[employee_name], " +
                                    " [event_id], [duration], [IsScheduleDayException], [IsEWSCalendarSynch],[selectedweekends],[weekends], [client_id] " +
                                    " FROM [ScheduleCalendar] " +
                                    " WHERE type_id NOT IN (2,22,11,5)  AND [customer_id] = " + nCustId + " AND [estimate_id] in (" + EstimateIds.TrimEnd(',') + ")";

                    _db.ExecuteCommand(sSqlINSERT, string.Empty);

                    //Insert
                    string sSqlLinkINSERT = "INSERT INTO [ScheduleCalendarLinkTemp] " +
                                           " SELECT [parent_event_id], " +
                                           " [child_event_id], " +
                                           " [customer_id], " +
                                           " [estimate_id], " +
                                           " [link_id], " +
                                           " [dependencyType], " +
                                           " [lag] " +
                                           " FROM [ScheduleCalendarLink] " +
                                           " WHERE [customer_id] = " + nCustId + " AND [estimate_id] in (" + EstimateIds.TrimEnd(',') + ")";

                    _db.ExecuteCommand(sSqlLinkINSERT, string.Empty);
                }
            }

            objcpmList.isCalendarOnline = false;

            lblCalState.Text = "Calendar is Offline";
            lblCalState.BackColor = Color.Red;
            lblCalState.ForeColor = Color.White;
            btnCalStateAction.Text = "Go Online";

            //btnCalStateAction.BorderColor = Color.Green;

            hdnCalStateAction.Value = "false";
            Session.Add("sIsCalendarOnline", false);

        }
        else // if Calendar Online
        {
            if (_db.ScheduleCalendarTemps.Any(sc => sc.customer_id == nCustId && listCheckedEstId.Contains((int)sc.estimate_id)))// if Data available in OFFLINE (ScheduleCalendarTemp) Table then  processed 
            {
                //Delete  table
                string sqlDELETE = "DELETE ScheduleCalendar WHERE type_id NOT IN (2,22,11,5)  AND [customer_id] = " + nCustId + " AND [estimate_id] in (" + EstimateIds.TrimEnd(',') + ")";
                _db.ExecuteCommand(sqlDELETE, string.Empty);

                //Delete  table
                string sqlDELETELink = "DELETE ScheduleCalendarLink WHERE [customer_id] = " + nCustId + " AND [estimate_id] in (" + EstimateIds.TrimEnd(',') + ")";
                _db.ExecuteCommand(sqlDELETELink, string.Empty);

                //Insert
                string sSQLINSERT = "INSERT INTO [ScheduleCalendar] " +
                                " SELECT [title],[description],[event_start],[event_end],[customer_id],[estimate_id],[employee_id],[section_name], " +
                                " [location_name],[create_date],[last_updated_date],[last_updated_by],[type_id],[parent_id],[job_start_date], " +
                                " [co_pricing_list_id],[cssClassName],[google_event_id],[operation_notes],[is_complete],[IsEstimateActive],[employee_name], "+
                                " [event_id], [duration], [IsScheduleDayException], [IsEWSCalendarSynch],[selectedweekends],[weekends], [client_id] " +
                                " FROM [ScheduleCalendarTemp] " +
                                " WHERE type_id NOT IN (2,22,11,5)  AND [customer_id] = " + nCustId + " AND [estimate_id] in (" + EstimateIds.TrimEnd(',') + ")";

                _db.ExecuteCommand(sSQLINSERT, string.Empty);

                //Insert
                string sSqlLinkINSERT = "INSERT INTO [ScheduleCalendarLink] " +
                                       " SELECT [parent_event_id], " +
                                       " [child_event_id], " +
                                       " [customer_id], " +
                                       " [estimate_id], " +
                                       " [link_id], " +
                                       " [dependencyType], " +
                                       " [lag] " +
                                       " FROM [ScheduleCalendarLinkTemp] " +
                                       " WHERE [customer_id] = " + nCustId + " AND [estimate_id] in (" + EstimateIds.TrimEnd(',') + ")";

                _db.ExecuteCommand(sSqlLinkINSERT, string.Empty);

                //Delete Temp table
                string sqlDELETETemp = "DELETE ScheduleCalendarTemp WHERE type_id <>5 AND [customer_id] = " + nCustId + " AND [estimate_id] in (" + EstimateIds.TrimEnd(',') + ")";
                _db.ExecuteCommand(sqlDELETETemp, string.Empty);

                //Delete Temp table
                string sqlDELETELinkTemp = "DELETE ScheduleCalendarLinkTemp WHERE [customer_id] = " + nCustId + " AND [estimate_id] in (" + EstimateIds.TrimEnd(',') + ")";
                _db.ExecuteCommand(sqlDELETELinkTemp, string.Empty);
            }

            objcpmList.isCalendarOnline = true;

            lblCalState.Text = "Calendar is Online";
            lblCalState.BackColor = Color.Green;
            lblCalState.ForeColor = Color.White;

            btnCalStateAction.Text = "Go Offline";

            //btnCalStateAction.BorderColor = Color.Red;

            hdnCalStateAction.Value = "true";
            Session.Add("sIsCalendarOnline", true);
        }
        _db.SubmitChanges();




        hdnCalStateAction.Value = objcpmList.isCalendarOnline.ToString().ToLower();
    }

    protected void btnHdn_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnHdn.ID, btnHdn.GetType().Name, "Click");
        DataClassesDataContext _db = new DataClassesDataContext();

        if (System.Web.HttpContext.Current.Session["cid"] != null)
        {
            hdnCustIDSelected.Value = System.Web.HttpContext.Current.Session["cid"].ToString();
        }

        if (System.Web.HttpContext.Current.Session["eid"] != null)
        {
            hdnEstIDSelected.Value = System.Web.HttpContext.Current.Session["eid"].ToString();
        }


        int nCustomerID = Convert.ToInt32(hdnCustIDSelected.Value);
        int nEstimateID = Convert.ToInt32(hdnEstIDSelected.Value);
        int nEventId = Convert.ToInt32(hdnEventId.Value);

        IEnumerable<int> listCheckedMainSection = chkTemplateMainSection.Items
                                      .Cast<ListItem>()
                                      .Where(item => item.Selected)
                                      .OrderBy(item => item.Selected)
                                      .Select(item => int.Parse(item.Value));

        if (Request.QueryString.Get("cid") == null) // && Request.QueryString.Get("eid") == null) // When Calendar is in Master (All Schedule Data)
        {
            System.Web.HttpContext.Current.Session["cid"] = null;
            System.Web.HttpContext.Current.Session["eid"] = null;



        }

        if (Request.QueryString.Get("cid") != null) // && Request.QueryString.Get("eid") != null) // When Calendar is in Single (Schedule Data by Customer & Estimate)
        {
            IEnumerable<int> listCheckedEstId = chkEst.Items
                                        .Cast<ListItem>()
                                        .Where(item => item.Selected)
                                        .OrderBy(item => item.Selected)
                                        .Select(item => int.Parse(item.Value));

            BindEstimate(nCustomerID, listCheckedEstId);
            BindDragTemplateSubSectionList(nCustomerID, nEstimateID, listCheckedMainSection);
            BindEstimatePT(nCustomerID, listCheckedEstId);
        }

    }



    //protected void ddlEst_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    int nCustId = Convert.ToInt32(hdnCustomerID.Value);
    //    int nEstid = Convert.ToInt32(ddlEst.SelectedItem.Value);

    //    Response.Redirect("schedulecalendar.aspx?eid=" + nEstid + "&cid=" + nCustId + "&TypeID=1");
    //}

    protected void BindDragTemplateMainSectionList(int nCustId, int nEstId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = (from cal in _db.CalGroupInfos
                    where cal.Calparent_id == 0
                    orderby cal.serial
                    select new
                    {
                        section_id = cal.Calsection_id,
                        section_name = cal.Calsection_name.Trim()
                    });


        chkTemplateMainSection.DataSource = item.Distinct().ToList();
        chkTemplateMainSection.DataTextField = "section_name";
        chkTemplateMainSection.DataValueField = "section_id";
        chkTemplateMainSection.DataBind();

        int nIndex = 0;
        if (chkTemplateMainSection.Items.Count > 0)
        {
            nIndex = chkTemplateMainSection.Items.Count + 1;
        }

        chkTemplateMainSection.Items.Add(new ListItem("Payment Terms", nIndex.ToString(), true));
    }

    protected void chkTemplateMainSection_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkTemplateMainSection.ID, chkTemplateMainSection.GetType().Name, "SelectedIndexChanged");
        int nCustId = Convert.ToInt32(hdnCustomerID.Value);
        int nEstId = 0;// Convert.ToInt32(ddlEst.SelectedItem.Value);

        if (chkTemplateMainSection.SelectedItem != null)
        {
            if (chkTemplateMainSection.SelectedItem.Text == "Payment Terms")
            {
                IEnumerable<int> listCheckedEstId = chkEst.Items
                                       .Cast<ListItem>()
                                       .Where(item => item.Selected)
                                       .OrderBy(item => item.Selected)
                                       .Select(item => int.Parse(item.Value));

                BindEstimatePT(nCustId, listCheckedEstId);
            }
            else
            {

                IEnumerable<int> listCheckedMainSection = chkTemplateMainSection.Items
                                                 .Cast<ListItem>()
                                                 .Where(item => item.Selected)
                                                 .OrderBy(item => item.Selected)
                                                 .Select(item => int.Parse(item.Value));

                BindDragTemplateSubSectionList(nCustId, nEstId, listCheckedMainSection);
            }
        }
        else
        {
            grdDragTemplateSection.DataSource = null;
            grdDragTemplateSection.DataBind();
        }
    }


    protected void BindDragTemplateSubSectionList(int nCustId, int nEstId, IEnumerable<int> listCheckedMainSection)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = (from cal in _db.CalGroupInfos
                    where listCheckedMainSection.Contains((int)cal.Calparent_id) && !(from sc in _db.ScheduleCalendarTemps where sc.customer_id == nCustId && sc.estimate_id == nEstId select sc.section_name).Contains(cal.Calsection_name)
                    orderby cal.serial
                    select new
                    {
                        cssClassName = cal.cssClassName,
                        serial = cal.serial,
                        section_name = cal.Calsection_name.Trim()
                    });




        grdDragTemplateSection.DataSource = item.Distinct().OrderBy(s => s.serial).ToList();
        grdDragTemplateSection.DataKeyNames = new string[] { "cssClassName" };
        grdDragTemplateSection.DataBind();
    }

    public void BindEstimatePT(int nCustId, IEnumerable<int> listCheckedEstId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = from e in _db.customer_estimates

                   where e.customer_id == nCustId && listCheckedEstId.Contains((int)e.estimate_id)
                   orderby e.estimate_name
                   select new
                   {
                       estimate_name = e.estimate_name,
                       estimate_id = e.estimate_id,
                       customer_id = e.customer_id
                   };

        grdEstimatesPT.DataSource = item.Distinct().OrderBy(o => o.estimate_id).ToList();
        grdEstimatesPT.DataKeyNames = new string[] { "customer_id", "estimate_id", "estimate_name" };
        grdEstimatesPT.DataBind();
    }

    protected void grdEstimatesPT_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nCustomerId = Convert.ToInt32(grdEstimatesPT.DataKeys[e.Row.RowIndex].Values[0]);
            int nEstimateId = Convert.ToInt32(grdEstimatesPT.DataKeys[e.Row.RowIndex].Values[1]);

            GridView gv = e.Row.FindControl("grdDragPaymentTerms") as GridView;

            BindDragTemplatePaymentTermsList(gv, nCustomerId, nEstimateId);
        }
    }


    protected void BindDragTemplatePaymentTermsList(GridView gv, int nCustId, int nEstId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        estimate_payment objEstPay = new estimate_payment();



        if (_db.estimate_payments.Any(pay => pay.estimate_id == nEstId && pay.customer_id == nCustId && pay.client_id == Convert.ToInt32(hdnClientId.Value)))
        {
            objEstPay = _db.estimate_payments.Single(pay => pay.estimate_id == nEstId && pay.customer_id == nCustId && pay.client_id == Convert.ToInt32(hdnClientId.Value));

            List<csExternalEvents> list = new List<csExternalEvents>();
            csExternalEvents objPT = new csExternalEvents();

            if (Convert.ToDecimal(objEstPay.deposit_amount) > 0)
            {
                objPT.cssClassName = "fc-white fc-PaymentTerms";
                objPT.serial = 1;
                objPT.section_name = objEstPay.deposit_value.Trim() + " Payment";
                objPT.customer_id = (int)objEstPay.customer_id;
                objPT.estimate_id = (int)objEstPay.estimate_id;

                list.Add(objPT);
            }

            if (Convert.ToDecimal(objEstPay.start_job_amount) > 0)
            {
                objPT = new csExternalEvents();
                objPT.cssClassName = "fc-white fc-PaymentTerms";
                objPT.serial = 2;
                objPT.section_name = objEstPay.start_job_value.Trim() + " Payment";
                objPT.customer_id = (int)objEstPay.customer_id;
                objPT.estimate_id = (int)objEstPay.estimate_id;
                list.Add(objPT);
            }

            if (Convert.ToDecimal(objEstPay.due_completion_amount) > 0)
            {
                objPT = new csExternalEvents();
                objPT.cssClassName = "fc-white fc-PaymentTerms";
                objPT.serial = 3;
                objPT.section_name = objEstPay.due_completion_value.Trim() + " Payment";
                objPT.customer_id = (int)objEstPay.customer_id;
                objPT.estimate_id = (int)objEstPay.estimate_id;
                list.Add(objPT);
            }

            if (Convert.ToDecimal(objEstPay.flooring_amount) > 0)
            {
                objPT = new csExternalEvents();
                objPT.cssClassName = "fc-white fc-PaymentTerms";
                objPT.serial = 4;
                objPT.section_name = objEstPay.flooring_value.Trim() + " Payment";
                objPT.customer_id = (int)objEstPay.customer_id;
                objPT.estimate_id = (int)objEstPay.estimate_id;
                list.Add(objPT);
            }

            if (Convert.ToDecimal(objEstPay.countertop_amount) > 0)
            {
                objPT = new csExternalEvents();
                objPT.cssClassName = "fc-white fc-PaymentTerms";
                objPT.serial = 5;
                objPT.section_name = objEstPay.countertop_value.Trim() + " Payment";
                objPT.customer_id = (int)objEstPay.customer_id;
                objPT.estimate_id = (int)objEstPay.estimate_id;
                list.Add(objPT);
            }

            if (Convert.ToDecimal(objEstPay.deliver_cabinet_amount) > 0)
            {
                objPT = new csExternalEvents();
                objPT.cssClassName = "fc-white fc-PaymentTerms";
                objPT.serial = 6;
                objPT.section_name = objEstPay.deliver_caninet_value.Trim() + " Payment";
                objPT.customer_id = (int)objEstPay.customer_id;
                objPT.estimate_id = (int)objEstPay.estimate_id;
                list.Add(objPT);
            }

            if (Convert.ToDecimal(objEstPay.drywall_amount) > 0)
            {
                objPT = new csExternalEvents();
                objPT.cssClassName = "fc-white fc-PaymentTerms";
                objPT.serial = 7;
                objPT.section_name = objEstPay.drywall_value.Trim() + " Payment";
                objPT.customer_id = (int)objEstPay.customer_id;
                objPT.estimate_id = (int)objEstPay.estimate_id;
                list.Add(objPT);
            }



            if (Convert.ToDecimal(objEstPay.final_measure_amount) > 0)
            {
                objPT = new csExternalEvents();
                objPT.cssClassName = "fc-white fc-PaymentTerms";
                objPT.serial = 8;
                objPT.section_name = objEstPay.final_measure_value.Trim() + " Payment";
                objPT.customer_id = (int)objEstPay.customer_id;
                objPT.estimate_id = (int)objEstPay.estimate_id;
                list.Add(objPT);
            }



            if (Convert.ToDecimal(objEstPay.other_amount) > 0)
            {
                objPT = new csExternalEvents();
                objPT.cssClassName = "fc-white fc-PaymentTerms";
                objPT.serial = 9;
                objPT.section_name = objEstPay.other_value.Trim() + " Payment";
                objPT.customer_id = (int)objEstPay.customer_id;
                objPT.estimate_id = (int)objEstPay.estimate_id;
                list.Add(objPT);
            }

            if (Convert.ToDecimal(objEstPay.substantial_amount) > 0)
            {
                objPT = new csExternalEvents();
                objPT.cssClassName = "fc-white fc-PaymentTerms";
                objPT.serial = 10;
                objPT.section_name = objEstPay.substantial_value.Trim() + " Payment";
                objPT.customer_id = (int)objEstPay.customer_id;
                objPT.estimate_id = (int)objEstPay.estimate_id;
                list.Add(objPT);
            }

            var objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCustId);

            if ((bool)objCust.isCalendarOnline)
            {
                list = (from l in list
                        where !(from sc in _db.ScheduleCalendars where sc.customer_id == nCustId && sc.estimate_id == nEstId select sc.section_name).Contains(l.section_name)
                        select l).ToList();
            }
            else
            {
                list = (from l in list
                        where !(from sc in _db.ScheduleCalendarTemps where sc.customer_id == nCustId && sc.estimate_id == nEstId select sc.section_name).Contains(l.section_name)
                        select l).ToList();
            }

            gv.DataSource = list.Distinct().OrderBy(s => s.serial).ToList();
            gv.DataKeyNames = new string[] { "cssClassName", "customer_id", "estimate_id" };
            gv.DataBind();
        }
    }

    protected void grdDragTemplateSection_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string strCssClass = grdDragTemplateSection.DataKeys[e.Row.RowIndex].Values[0].ToString();
            Label lblSection = (Label)e.Row.FindControl("lblSection");
            lblSection.CssClass = "fc-event fc-default";// +strCssClass;
        }
    }

    protected void grdDragPaymentTerms_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdDragPaymentTerms = (GridView)sender;

            string strCssClass = grdDragPaymentTerms.DataKeys[e.Row.RowIndex].Values[0].ToString();
            Label lblSection = (Label)e.Row.FindControl("lblSection");
            lblSection.CssClass = "fc-event fc-default fc-drag";// +strCssClass;
        }
    }

    public void BindEstimate(int nCustId, IEnumerable<int> listCheckedEstId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = from e in _db.customer_estimates

                   where e.customer_id == nCustId && listCheckedEstId.Contains((int)e.estimate_id)
                   orderby e.estimate_name
                   select new
                   {
                       estimate_name = e.estimate_name,
                       estimate_id = e.estimate_id,
                       customer_id = e.customer_id
                   };

        grdEstimates.DataSource = item.Distinct().OrderBy(o => o.estimate_id).ToList();
        grdEstimates.DataKeyNames = new string[] { "customer_id", "estimate_id", "estimate_name" };
        grdEstimates.DataBind();
    }

    protected void grdEstimates_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nCustomerId = Convert.ToInt32(grdEstimates.DataKeys[e.Row.RowIndex].Values[0]);
            int nEstimateId = Convert.ToInt32(grdEstimates.DataKeys[e.Row.RowIndex].Values[1]);

            GridView gv = e.Row.FindControl("grdProjectSection") as GridView;

            BindProjectSectionList(gv, nCustomerId, nEstimateId);
        }
    }

    protected void chkEst_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkEst.ID, chkEst.GetType().Name, "SelectedIndexChanged");
        int nCustId = Convert.ToInt32(hdnCustIDSelected.Value);
        BindEstimate(nCustId, SetSessionEstimate());

        if (chkPMNotes.Checked)
            BindEstimatePM(nCustId, SetSessionEstimate());
    }

    protected void BindProjectSectionList(GridView grdProjectSection, int nCustomerId, int nEstimateId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCustomerId);

        IQueryable<csExternalEvents> item = Enumerable.Empty<csExternalEvents>().AsQueryable();

        if ((bool)objCust.isCalendarOnline)
        {
            item = from cpm in _db.co_pricing_masters
                   join l in _db.locations on cpm.location_id equals l.location_id
                   join sec in _db.sectioninfos on cpm.section_name equals sec.section_name
                   where cpm.customer_id == nCustomerId && cpm.estimate_id == nEstimateId && sec.parent_id == 0
                   && !(from sc in _db.ScheduleCalendars where sc.customer_id == nCustomerId && sc.estimate_id == nEstimateId select sc.title.Trim()).Contains(cpm.section_name.Trim() + " (" + l.location_name.Trim() + ")")
                   orderby cpm.section_name
                   select new csExternalEvents()
                   {
                       cssClassName = sec.cssClassName ?? "",
                       serial = (int)sec.serial,
                       section_name = cpm.section_name.Trim() + " - (" + l.location_name.Trim() + ")",
                       customer_id = (int)cpm.customer_id,
                       estimate_id = (int)cpm.estimate_id,
                       client_id = (int)cpm.client_id
                   };
        }
        else
        {
            item = from cpm in _db.co_pricing_masters
                   join l in _db.locations on cpm.location_id equals l.location_id
                   join sec in _db.sectioninfos on cpm.section_name equals sec.section_name
                   where cpm.customer_id == nCustomerId && cpm.estimate_id == nEstimateId && sec.parent_id == 0
                   && !(from sc in _db.ScheduleCalendarTemps where sc.customer_id == nCustomerId && sc.estimate_id == nEstimateId select sc.title.Trim()).Contains(cpm.section_name.Trim() + " (" + l.location_name.Trim() + ")")
                   orderby cpm.section_name
                   select new csExternalEvents()
                   {
                       cssClassName = sec.cssClassName ?? "",
                       serial = (int)sec.serial,
                       section_name = cpm.section_name.Trim() + " - (" + l.location_name.Trim() + ")",
                       customer_id = (int)cpm.customer_id,
                       estimate_id = (int)cpm.estimate_id,
                       client_id = (int)cpm.client_id
                   };
        }




        grdProjectSection.DataSource = item.Distinct().OrderBy(o => o.section_name).OrderBy(o => o.estimate_id).ToList();
        grdProjectSection.DataKeyNames = new string[] { "cssClassName", "customer_id", "estimate_id", "client_id" };
        grdProjectSection.DataBind();
    }

    protected void grdProjectSection_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdProjectSection = (GridView)sender;

            string strCssClass = grdProjectSection.DataKeys[e.Row.RowIndex].Values[0].ToString();
            Label lblSection = (Label)e.Row.FindControl("lblSection");
            lblSection.CssClass = "fc-event " + strCssClass;
        }
    }

    public void BindEstimatePM(int nCustId, IEnumerable<int> listCheckedEstId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = from e in _db.customer_estimates

                   where e.customer_id == nCustId && listCheckedEstId.Contains((int)e.estimate_id)
                   orderby e.estimate_name
                   select new
                   {
                       estimate_name = e.estimate_name,
                       estimate_id = e.estimate_id,
                       customer_id = e.customer_id
                   };

        grdEstimatesPM.DataSource = item.Distinct().OrderBy(o => o.estimate_id).ToList();
        grdEstimatesPM.DataKeyNames = new string[] { "customer_id", "estimate_id", "estimate_name" };
        grdEstimatesPM.DataBind();
    }

    protected void grdEstimatesPM_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nCustomerId = Convert.ToInt32(grdEstimatesPM.DataKeys[e.Row.RowIndex].Values[0]);
            int nEstimateId = Convert.ToInt32(grdEstimatesPM.DataKeys[e.Row.RowIndex].Values[1]);

            GridView gv = e.Row.FindControl("grdPMNotes") as GridView;

            BindDatatoGridView(gv, nEstimateId);
        }
    }
    protected void chkPMNotes_CheckedChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkPMNotes.ID, chkPMNotes.GetType().Name, "Changed");
        int nCustId = Convert.ToInt32(hdnCustIDSelected.Value);

        BindEstimatePM(nCustId, SetSessionEstimate());
    }

    private void BindDatatoGridView(GridView grdPMNotes, int nEstimateId)
    {
        try
        {
            lblResult.Text = "";
            DataClassesDataContext _db = new DataClassesDataContext();


            if (chkPMNotes.Checked)
            {

                IEnumerable<int> listCheckedEstId = SetSessionEstimate();

                string EstimateIds = String.Join(", ", listCheckedEstId);

                int CustomerId = Convert.ToInt32(hdnCustIDSelected.Value);


                string dataGridobj = " select distinct pii.IsComplete, si.section_name, pii.customer_id,pii.PMNoteId, pii.estimate_id,pii.section_id, pii.NoteDetails,pii.CreateDate,pii.CreatedBy,pii.vendor_id, ISNULL(reverse(stuff(reverse(vendor_name), 2, 1, '')),'')  as vendor_name from PMNoteInfo as pii "
                    + " left join sectioninfo as si on si.section_id=pii.section_id  where pii.customer_id=" + CustomerId + " and pii.estimate_id in (" + nEstimateId + ") order by CreateDate desc ";
                DataTable dt = csCommonUtility.GetDataTable(dataGridobj);
                grdPMNotes.DataSource = dt;
                grdPMNotes.DataKeyNames = new string[] { "PMNoteId", "customer_id", "NoteDetails", "estimate_id", "vendor_id", "vendor_name", "CreateDate", "CreatedBy", "section_id", "section_name", "IsComplete" };
                grdPMNotes.DataBind();

                PMNotesSections.Visible = true;
                trSectionsList.Visible = false;
                trGridSection.Visible = false;
            }
            else
            {
                grdEstimatesPM.DataSource = null;
                grdEstimatesPM.DataBind();

                grdPMNotes.DataSource = null;
                grdPMNotes.DataBind();

                PMNotesSections.Visible = false;
                trSectionsList.Visible = true;
                trGridSection.Visible = true;
            }

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    protected void grdPMNotes_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridView grdPMNotes = (GridView)sender;
            int nPMNoteId = Convert.ToInt32(grdPMNotes.DataKeys[e.Row.RowIndex].Values[0]);
            int nCustomerId = Convert.ToInt32(grdPMNotes.DataKeys[e.Row.RowIndex].Values[1]);
            bool IsComplete = Convert.ToBoolean(grdPMNotes.DataKeys[e.Row.RowIndex].Values[10]);

            CheckBox chkIsComplete = (CheckBox)e.Row.FindControl("chkIsComplete");
            chkIsComplete.Checked = IsComplete;
            chkIsComplete.Attributes["CommandArgument"] = string.Format("{0}", nPMNoteId);
        }
    }

    protected void chkIsComplete_CheckedChanged(object sender, EventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        CheckBox chkIsComplete = (CheckBox)sender;
        int nPMNoteId = Convert.ToInt32(chkIsComplete.Attributes["CommandArgument"]);
        int nCustomerId = Convert.ToInt32(hdnCustIDSelected.Value);

        string strQ = string.Empty;

        try
        {
            if (chkIsComplete.Checked)
            {
                strQ = "UPDATE PMNoteInfo SET IsComplete = 1  WHERE PMNoteId=" + nPMNoteId + "  AND customer_id=" + nCustomerId;
            }
            else
            {
                strQ = "UPDATE PMNoteInfo SET IsComplete = 0 WHERE PMNoteId=" + nPMNoteId + "  AND customer_id=" + nCustomerId;
            }
        }
        catch (Exception ex)
        {

            throw ex;
        }

        _db.ExecuteCommand(strQ, string.Empty);

        BindEstimatePM(nCustomerId, SetSessionEstimate());
    }
    public IEnumerable<int> SetSessionEstimate()
    {
        IEnumerable<int> listCheckedEstId = null;
        int nCustId = Convert.ToInt32(hdnCustIDSelected.Value);


        listCheckedEstId = chkEst.Items
                                  .Cast<ListItem>()
                                  .Where(item => item.Selected)
                                  .OrderBy(item => item.Selected)
                                  .Select(item => int.Parse(item.Value));

        HttpContext.Current.Session.Add("sSelectedEstIdList", listCheckedEstId);


        return listCheckedEstId;
    }

    protected void SaveSectionCOPricingMaster(int nCustId, int nEstId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (_db.co_pricing_masters.Where(cl => cl.customer_id == nCustId && cl.estimate_id == nEstId).ToList().Count == 0)
        {
            List<customer_location> Cust_LocList = _db.customer_locations.Where(cl => cl.estimate_id == nEstId).ToList();
            List<customer_section> Cust_SecList = _db.customer_sections.Where(cs => cs.estimate_id == nEstId && cs.customer_id == nCustId ).ToList();
            List<pricing_detail> Pm_List = _db.pricing_details.Where(pd => pd.estimate_id == nEstId && pd.customer_id == nCustId && pd.pricing_type == "A").ToList();

            foreach (customer_location objcl in Cust_LocList)
            {
                changeorder_location co_loc = new changeorder_location();
                co_loc.client_id = objcl.client_id;
                co_loc.customer_id = objcl.customer_id;
                co_loc.location_id = objcl.location_id;
                co_loc.estimate_id = nEstId;
                _db.changeorder_locations.InsertOnSubmit(co_loc);
            }
            foreach (customer_section objcs in Cust_SecList)
            {
                changeorder_section co_sec = new changeorder_section();
                co_sec.client_id = objcs.client_id;
                co_sec.customer_id = objcs.customer_id;
                co_sec.section_id = objcs.section_id;
                co_sec.estimate_id = nEstId;
                co_sec.sales_person_id = objcs.sales_person_id;
                _db.changeorder_sections.InsertOnSubmit(co_sec);
            }
            foreach (pricing_detail objCpm in Pm_List)
            {
                co_pricing_master cpm = new co_pricing_master();
                cpm.client_id = objCpm.client_id; ;
                cpm.customer_id = objCpm.customer_id;
                cpm.estimate_id = nEstId;
                cpm.location_id = objCpm.location_id; ;
                cpm.sales_person_id = objCpm.sales_person_id;
                cpm.section_level = objCpm.section_level;
                cpm.item_id = objCpm.item_id;
                cpm.section_name = objCpm.section_name;
                cpm.item_name = objCpm.item_name;
                cpm.measure_unit = objCpm.measure_unit;
                cpm.minimum_qty = objCpm.minimum_qty;
                cpm.quantity = objCpm.quantity;
                cpm.retail_multiplier = objCpm.retail_multiplier;
                cpm.labor_id = objCpm.labor_id;
                cpm.is_direct = objCpm.is_direct;
                cpm.item_cost = objCpm.item_cost;
                cpm.total_direct_price = objCpm.total_direct_price;
                cpm.total_retail_price = objCpm.total_retail_price;
                cpm.labor_rate = objCpm.labor_rate;
                cpm.section_serial = objCpm.section_serial;
                cpm.item_cnt = objCpm.item_cnt;
                cpm.item_status_id = 1;
                cpm.short_notes = objCpm.short_notes;
                cpm.create_date = DateTime.Today;
                cpm.last_update_date = DateTime.Today;
                cpm.prev_total_price = 0;
                cpm.execution_unit = 0;
                cpm.week_id = 1;
                cpm.is_complete = false;
                cpm.schedule_note = "";
                cpm.sort_id = objCpm.sort_id;
                cpm.CalEventId = 0;
                cpm.is_CommissionExclude = objCpm.is_CommissionExclude;

                _db.co_pricing_masters.InsertOnSubmit(cpm);
            }
        }

        _db.SubmitChanges();
    }

    protected void btnHdnSendCalEmail_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnHdnSendCalEmail.ID, btnHdnSendCalEmail.GetType().Name, "Click");
        btnCalStateAction_Click(sender, e);

        string url = "sendemailoutlook.aspx?calnotifyCustID=" + hdnCustomerID.Value + "&calnotifyEstID=" + hdnEstimateID.Value + "', 'MyWindow', 'left=200,top=100,width=900,height=600,status=0,toolbar=0,resizable=0,scrollbars=1";



        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "Popup", "window.open('" + url + "'); HideProgress();", true);
        // SendCalendarNotificationEmail();
    }

    //this method only updates title and description //this is called when a event is clicked on the calendar
    [System.Web.Services.WebMethod(true)]
    public static string UpdateEvent(ImproperCalendarEvent cevent)
    {
        int nCheck = 0;
        var date = cevent.start.ToString();

        //cevent.start = DateTime.Parse(cevent.start).ToString("dd-MM-yyyy hh:mm:ss tt");
        //cevent.end = DateTime.Parse(cevent.end).ToString("dd-MM-yyyy hh:mm:ss tt");

        List<int> idList = (List<int>)System.Web.HttpContext.Current.Session["idList"];
        if (idList != null && idList.Contains(cevent.id))
        {
            if (CheckAlphaNumeric(cevent.title) && CheckAlphaNumeric(cevent.description))
            {
                nCheck = EventDAO.updateEvent(cevent.id, cevent.title.Replace("\"", "''"), cevent.section_name.Replace("\"", "''"), cevent.location_name.Replace("\"", "''"), cevent.description.Replace("\"", "''"), cevent.cssClassName,
                     DateTime.ParseExact(cevent.start.ToString(), "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
                     DateTime.ParseExact(cevent.end.ToString(), "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
                     cevent.employee_id, cevent.employee_name, cevent.child_event_id, cevent.dependencyType, cevent.offsetDays,
                     cevent.parentDependencyType, cevent.parentOffsetDays, cevent.linkType, cevent.customer_id, cevent.estimate_id, cevent.IsScheduleDayException, cevent.is_complete, cevent.selectedweekends);

                //GridView gv = new GridView();
                //System.IO.StringWriter stringWriter = new System.IO.StringWriter();
                //HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
                //DataClassesDataContext _db = new DataClassesDataContext();
                //var item = from scTemp in _db.ScheduleCalendarTemps
                //           join linktemp in _db.ScheduleCalendarLinkTemps on scTemp.event_id equals linktemp.child_event_id
                //           where linktemp.customer_id == cevent.customer_id && linktemp.estimate_id == cevent.estimate_id && linktemp.parent_event_id == cevent.id
                //           select new
                //           {
                //               link_id = linktemp.link_id,
                //               title = scTemp.title,
                //               start = scTemp.event_start,
                //               end = scTemp.event_end
                //           };

                //gv.DataSource = item.ToList();
                //gv.DataKeyNames = new string[] { "link_id" };
                //gv.DataBind();

                //gv.RenderControl(htmlWriter);
                //return stringWriter.ToString();
                if (nCheck <= 0)
                {
                    return nCheck.ToString();
                }
                else
                {
                    return cevent.id.ToString();
                }
                //return "updated event with id:" + cevent.id + " update title to: " + cevent.title +
                //" update description to: " + cevent.description;
            }

        }

        return "unable to update event with id:" + cevent.id + " title : " + cevent.title +
            " description : " + cevent.description;
    }

    [System.Web.Services.WebMethod(true)]
    public static string CancelUpdateEventTime(ImproperCalendarEvent improperEvent)
    {
        try
        {

            String strdt = (String)System.Web.HttpContext.Current.Session["strdt"];
            DateTime dt = Convert.ToDateTime(strdt);
            string ndt = dt.ToString("dd-MM-yyyy hh:mm:ss tt");
            List<int> idList = (List<int>)System.Web.HttpContext.Current.Session["idList"];
            if (idList != null && idList.Contains(improperEvent.id))
            {
                EventDAO.updateEventTime(improperEvent.id,
                    DateTime.ParseExact(ndt.ToString(), "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
                    DateTime.ParseExact(ndt.ToString(), "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
                      improperEvent.customer_id, improperEvent.estimate_id, improperEvent.IsScheduleDayException, improperEvent.selectedweekends);

                return "updated event with id:" + improperEvent.id + "update start to: " + improperEvent.start +
                    " update end to: " + improperEvent.end;
            }
        }
        catch (Exception ex)
        {
        }
        return "unable to update event with id: " + improperEvent.id;
    }

    //this method only updates start and end time //this is called when a event is dragged or resized in the calendar
    [System.Web.Services.WebMethod(true)]
    public static string UpdateEventTime(ImproperCalendarEvent improperEvent)
    {
        int nCheck = 0;
        List<int> idList = (List<int>)System.Web.HttpContext.Current.Session["idList"];
        if (idList != null && idList.Contains(improperEvent.id))
        {
            nCheck = EventDAO.updateEventTime(improperEvent.id,
                  DateTime.ParseExact(improperEvent.start.ToString(), "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
                  DateTime.ParseExact(improperEvent.end.ToString(), "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
                    improperEvent.customer_id, improperEvent.estimate_id, improperEvent.IsScheduleDayException, improperEvent.selectedweekends);

            if (nCheck <= 0)
            {
                return nCheck.ToString();
            }
            else
            {
                return "updated event with id:" + improperEvent.id + "update start to: " + improperEvent.start +
                    " update end to: " + improperEvent.end;
            }
        }
        else
        {
            return "unable to update event with id: " + improperEvent.id;
        }
    }

    //this method only updates All events start and end time //this is called when a event is dragged or resized in the calendar
    [System.Web.Services.WebMethod(true)]
    public static string UpdateEventTimeAll(ImproperCalendarEvent improperEvent)
    {
        List<int> idList = (List<int>)System.Web.HttpContext.Current.Session["idList"];
        if (idList != null && idList.Contains(improperEvent.id))
        {
            //DataClassesDataContext _db = new DataClassesDataContext();
            //DateTime nStart = DateTime.ParseExact(improperEvent.start.ToString(), "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            //ScheduleCalendar objsc = _db.ScheduleCalendars.SingleOrDefault(sc => sc.event_id == improperEvent.id);
            //string strdt = objsc.event_start.ToString();
            //HttpContext.Current.Session.Add("strdt", strdt);

            EventDAO.updateEventTimeAll(improperEvent.id,
                DateTime.ParseExact(improperEvent.start.ToString(), "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
                DateTime.ParseExact(improperEvent.end.ToString(), "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
                  improperEvent.customer_id, improperEvent.estimate_id);

            return "updated event with id:" + improperEvent.id + "update start to: " + improperEvent.start +
                " update end to: " + improperEvent.end;
        }

        return "unable to update event with id: " + improperEvent.id;
    }

    [System.Web.Services.WebMethod(true)]
    public static string UpdateEventNotes(ImproperCalendarEvent improperEvent)
    {
        List<int> idList = (List<int>)System.Web.HttpContext.Current.Session["idList"];
        if (idList != null && idList.Contains(improperEvent.id))
        {
            EventDAO.UpdateEventNotes(improperEvent.id,
                 DateTime.ParseExact(improperEvent.start.ToString(), "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
                DateTime.ParseExact(improperEvent.end.ToString(), "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
                improperEvent.operation_notes.Replace("\"", "''"));

            return "updated event with id:" + improperEvent.id;
        }

        return "unable to update event with id: " + improperEvent.id;
    }
    //called when delete button is pressed
    [System.Web.Services.WebMethod(true)]
    public static String deleteEvent(int id)
    {
        //idList is stored in Session by JsonResponse.ashx for security reasons
        //whenever any event is update or deleted, the event id is checked
        //whether it is present in the idList, if it is not present in the idList
        //then it may be a malicious user trying to delete someone elses events
        //thus this checking prevents misuse
        List<int> idList = (List<int>)System.Web.HttpContext.Current.Session["idList"];
        if (idList != null && idList.Contains(id))
        {
            EventDAO.deleteEvent(id);
            return "deleted event with id:" + id;
        }

        return "unable to delete event with id: " + id;

    }

    //called when delete button is pressed
    [System.Web.Services.WebMethod(true)]
    public static String cancelEvent()
    {
        EventDAO.cancelEvent();
        return "cancel event";
    }

    //called when Add button is clicked //this is called when a mouse is clicked on open space of any day or dragged  //over mutliple days
    [System.Web.Services.WebMethod]
    public static int addEvent(ImproperCalendarEvent improperEvent)
    {

        CalendarEvent cevent = new CalendarEvent()
        {
            title = improperEvent.title.Replace("\"", "''"),
            section_name = improperEvent.section_name.Replace("\"", "''"),
            location_name = improperEvent.location_name.Replace("\"", "''"),
            description = improperEvent.description.Replace("\"", "''"),
            start = DateTime.ParseExact(improperEvent.start, "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
            end = DateTime.ParseExact(improperEvent.end, "dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture),
            customer_id = improperEvent.customer_id,
            estimate_id = improperEvent.estimate_id,
            employee_id = improperEvent.employee_id,
            employee_name = improperEvent.employee_name.Replace("\"", "''"),
            cssClassName = improperEvent.cssClassName,
            child_event_id = improperEvent.child_event_id,
            dependencyType = improperEvent.dependencyType,
            offsetDays = improperEvent.offsetDays,
            parentDependencyType = improperEvent.parentDependencyType,
            parentOffsetDays = improperEvent.parentOffsetDays,
            linkType = improperEvent.linkType,
            IsScheduleDayException = improperEvent.IsScheduleDayException,
            is_complete = improperEvent.is_complete,
            client_id = improperEvent.client_id
        };

        if (CheckAlphaNumeric(cevent.section_name) && CheckAlphaNumeric(cevent.description))
        {
            int key = EventDAO.addEvent(cevent);

            List<int> idList = (List<int>)System.Web.HttpContext.Current.Session["idList"];

            if (idList != null)
            {
                idList.Add(key);
            }

            return key;//return the primary key of the added cevent object

        }

        return -1;//return a negative number just to signify nothing has been added

    }
    [System.Web.Services.WebMethod]
    public static csProjectLink GetEvent(int nCustId, string SectionName, string UserName, string SuperintendentName)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        HttpContext.Current.Session.Add("CusId", nCustId);
        HttpContext.Current.Session.Add("sSecName", SectionName);
        HttpContext.Current.Session.Add("sKeySearchUserName", UserName);
        HttpContext.Current.Session.Add("sKeySearchSuperintendentName", SuperintendentName);


        int nEstimateID = 0;
        string date = "";

        if (nCustId != 0 && SectionName != "" && UserName != "")
        {
            var objSC = _db.ScheduleCalendars.Where(c => c.customer_id == nCustId && c.section_name == SectionName && c.employee_name.Contains(UserName));
            if (objSC.Any())
            {
                nEstimateID = Convert.ToInt32(objSC.Max(x => x.estimate_id));
                var tempDate = objSC.Where(c => c.event_start >= DateTime.Today).Min(x => x.event_start);
                if (tempDate != null)
                    date = ConvertToTimestamp((DateTime)tempDate);
            }

            HttpContext.Current.Session.Add("sEstSelectedByCustSearch", nEstimateID);

            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = nCustId;
            objPL.estimate_id = nEstimateID;
            objPL.date = date;

            return objPL;
        }
        else if (nCustId != 0 && UserName != "")
        {
            var objSC = _db.ScheduleCalendars.Where(c => c.customer_id == nCustId && c.employee_name.Contains(UserName));
            if (objSC.Any())
            {
                nEstimateID = Convert.ToInt32(objSC.Max(x => x.estimate_id));
                var tempDate = objSC.Where(c => c.event_start >= DateTime.Today).Min(x => x.event_start);
                if (tempDate != null)
                    date = ConvertToTimestamp((DateTime)tempDate);
            }

            HttpContext.Current.Session.Add("sEstSelectedByCustSearch", nEstimateID);

            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = nCustId;
            objPL.estimate_id = nEstimateID;
            objPL.date = date;

            return objPL;
        }
        else if (nCustId != 0 && SectionName != "")
        {
            var objSC = _db.ScheduleCalendars.Where(c => c.customer_id == nCustId && c.section_name == SectionName);
            if (objSC.Any())
            {
                nEstimateID = Convert.ToInt32(objSC.Max(x => x.estimate_id));
                var tempDate = objSC.Where(c => c.event_start >= DateTime.Today).Min(x => x.event_start);
                if (tempDate != null)
                    date = ConvertToTimestamp((DateTime)tempDate);
            }

            HttpContext.Current.Session.Add("sEstSelectedByCustSearch", nEstimateID);

            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = nCustId;
            objPL.estimate_id = nEstimateID;
            objPL.date = date;

            return objPL;
        }
        else if (SectionName != "" && UserName != "")
        {
            var objSC = _db.ScheduleCalendars.Where(c => c.section_name == SectionName && c.employee_name.Contains(UserName));
            if (objSC.Any())
            {
                nEstimateID = Convert.ToInt32(objSC.Max(x => x.estimate_id));
                var tempDate = objSC.Where(c => c.event_start >= DateTime.Today).Min(x => x.event_start);
                if (tempDate != null)
                    date = ConvertToTimestamp((DateTime)tempDate);
            }

            HttpContext.Current.Session.Add("sEstSelectedByCustSearch", nEstimateID);

            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = nCustId;
            objPL.estimate_id = nEstimateID;
            objPL.date = date;

            return objPL;
        }
        else if (nCustId != 0)
        {
            var objSC = _db.ScheduleCalendars.Where(c => c.customer_id == nCustId);
            if (objSC.Any())
            {
                nEstimateID = Convert.ToInt32(objSC.Max(x => x.estimate_id));
                var tempDate = objSC.Where(c => c.event_start >= DateTime.Today).Min(x => x.event_start);
                if (tempDate != null)
                    date = ConvertToTimestamp((DateTime)tempDate);
            }



            HttpContext.Current.Session.Add("sEstSelectedByCustSearch", nEstimateID);

            csProjectLink objPL = new csProjectLink();

            objPL.customer_id = nCustId;
            objPL.estimate_id = nEstimateID;
            objPL.date = date;

            return objPL;
        }
        else if (SectionName != "")
        {
            var dtDate = _db.ScheduleCalendars.Where(c => c.section_name == SectionName && c.event_start >= DateTime.Now).Min(x => x.event_start);

            HttpContext.Current.Session.Add("sEstSelectedByCustSearch", null);

            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = nCustId;
            objPL.estimate_id = 0;
            if (dtDate != null)
            {
                objPL.date = ConvertToTimestamp((DateTime)dtDate);
            }
            else
            {
                objPL.date = "";
            }

            return objPL;
        }
        else if (UserName != "")
        {
            var dtDate = _db.ScheduleCalendars.Where(c => c.employee_name.Contains(UserName) && c.event_start >= DateTime.Now).Min(x => x.event_start);

            HttpContext.Current.Session.Add("sEstSelectedByCustSearch", null);

            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = nCustId;
            objPL.estimate_id = 0;
            if (dtDate != null)
            {
                objPL.date = ConvertToTimestamp((DateTime)dtDate);
            }
            else
            {
                objPL.date = "";
            }

            return objPL;
        }
        else if (SuperintendentName != "")
        {
            var dtDate = (
                        from sc in _db.ScheduleCalendars
                        join c in _db.customers on sc.customer_id equals c.customer_id
                        join u in _db.user_infos on c.SuperintendentId equals u.user_id
                        where (u.first_name.Trim().ToLower() + ' ' + u.first_name.Trim().ToLower()).Contains(SuperintendentName.Trim().ToLower())
                        && sc.event_start >= DateTime.Now
                        select sc.event_start).Min();


            //_db.ScheduleCalendars.Where(c => c.employee_name.Contains(UserName) && c.event_start >= DateTime.Now).Min(x => x.event_start);

            HttpContext.Current.Session.Add("crsEstSelectedByCustSearch", null);

            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = nCustId;
            objPL.estimate_id = 0;
            if (dtDate != null)
            {
                objPL.date = ConvertToTimestamp((DateTime)dtDate);
            }
            else
            {
                objPL.date = "";
            }

            return objPL;
        }
        else
        {
            HttpContext.Current.Session.Add("sEstSelectedByCustSearch", null);
            csProjectLink objPL = new csProjectLink();
            objPL.customer_id = 0;
            objPL.estimate_id = 0;
            objPL.date = "";

            return objPL;
        }
    }

    private static bool CheckAlphaNumeric(string str)
    {
        return Regex.IsMatch(str, @"^[a-zA-Z0-9_\-.?""',;><:!@#$%&\[\]()/* ]*$");
    }

    [System.Web.Services.WebMethod]
    public static List<csCustomer> GetCustomer(string keyword, int divisionId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = (from c in _db.customers
                    join sc in _db.ScheduleCalendars on c.customer_id equals sc.customer_id
                    where c.last_name1.ToUpper().StartsWith(keyword.Trim().ToUpper()) && sc.estimate_id != 0 && c.client_id == divisionId
                    select new csCustomer
                    {
                        customer_id = c.customer_id,
                        customer_name = c.first_name1.Trim() + " " + c.last_name1.Trim()
                    }).Distinct().ToList();
        return item.OrderBy(c => c.customer_name).ToList();
    }

    [System.Web.Services.WebMethod]
    public static string GetEmployeeById(int empId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = (from sp in _db.sales_persons
                    where sp.sales_person_id == empId
                    select sp).SingleOrDefault();

        return item.first_name.ToString() + " " + item.last_name.ToString();
    }

    [System.Web.Services.WebMethod]
    public static List<SectionInfo> GetSection(string keyword, int divisionId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = from sc in _db.ScheduleCalendars
                   where sc.section_name.ToUpper().StartsWith(keyword.Trim().ToUpper()) && (sc.type_id == 1 || sc.type_id == 11) && sc.client_id == divisionId
                   select new SectionInfo
                   {
                       section_name = sc.section_name.Trim()
                   };

        //var item = from c in _db.ScheduleCalendars
        //           where c.section_name.ToUpper().Contains(keyword.Trim().ToUpper()) //&& c.parent_id == 0
        //           orderby c.section_name
        //           select new SectionInfo
        //           {
        //               section_name = c.section_name.Trim()
        //           };

        return item.Distinct().OrderBy(s => s.section_name).ToList();
    }

    [System.Web.Services.WebMethod]
    public static List<userinfo> GetUserName(string keyword, int divisionId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = (from cr in _db.Crew_Details
                    where (cr.first_name.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) || cr.last_name.Trim().ToUpper().Contains(keyword.Trim().ToUpper()))
                    && cr.is_active == true && cr.client_id == divisionId
                    orderby cr.first_name
                    select new userinfo
                    {
                        first_name = cr.first_name.Trim(),
                        last_name = cr.last_name.Trim(),
                        sales_person_id = cr.crew_id,
                        sales_person_name = cr.first_name.Trim() + " " + cr.last_name.Trim()
                    });

        if (item.ToList().Count() == 0 && keyword.ToLower().Contains("*"))
        {
            item = (from cr in _db.Crew_Details
                    where cr.is_active == true && cr.client_id == divisionId
                    orderby cr.first_name
                    select new userinfo
                    {
                        first_name = cr.first_name.Trim(),
                        last_name = cr.last_name.Trim(),
                        sales_person_id = cr.crew_id,
                        sales_person_name = cr.first_name.Trim() + " " + cr.last_name.Trim()
                    });
        }

        return item.Distinct().OrderBy(f => f.first_name).ToList();
    }

    [System.Web.Services.WebMethod]
    public static List<userinfo> GetSuperintendentName(string keyword, int divisionId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();



        var item = from u in _db.user_infos
                   join c in _db.customers on u.user_id equals c.SuperintendentId
                   where (u.first_name.Trim().ToUpper().Contains(keyword.Trim().ToUpper()) || u.last_name.Trim().ToUpper().Contains(keyword.Trim().ToUpper()))
                   && u.is_active == true && u.client_id.Contains(divisionId.ToString())
                   orderby u.first_name
                   select new userinfo
                   {
                       first_name = u.first_name.Trim(),
                       last_name = u.last_name.Trim(),
                       sales_person_id = u.user_id,
                       sales_person_name = u.first_name.Trim() + " " + u.last_name.Trim()
                   };


        if (item.ToList().Count() == 0 && keyword.ToLower().Contains("*"))
        {
            item = from u in _db.user_infos
                   join c in _db.customers on u.user_id equals c.SuperintendentId
                   where u.is_active == true && u.client_id.Contains(divisionId.ToString())
                   orderby u.first_name
                   select new userinfo
                   {
                       first_name = u.first_name.Trim(),
                       last_name = u.last_name.Trim(),
                       sales_person_id = u.user_id,
                       sales_person_name = u.first_name.Trim() + " " + u.last_name.Trim()
                   };

        }

        return item.Distinct().OrderBy(f => f.first_name).ToList();


    }


    [System.Web.Services.WebMethod]
    public static List<CO_PricingDeatilModel> GetSectionByCustomerId(string keyword, int nCustId, int nEstId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = from cpm in _db.co_pricing_masters
                   where cpm.section_name.ToUpper().StartsWith(keyword.Trim().ToUpper()) && cpm.customer_id == nCustId && cpm.estimate_id == nEstId
                   orderby cpm.section_name
                   select new CO_PricingDeatilModel
                   {
                       section_name = cpm.section_name.Trim()
                   };

        if (item.ToList().Count() == 0 && keyword.ToLower().Contains("*"))
        {
            item = from cpm in _db.co_pricing_masters
                   where cpm.customer_id == nCustId && cpm.estimate_id == nEstId
                   orderby cpm.section_name
                   select new CO_PricingDeatilModel
                   {
                       section_name = cpm.section_name.Trim()
                   };
        }

        return item.Distinct().OrderBy(l => l.section_name).ToList();
    }

    [System.Web.Services.WebMethod]
    public static List<CO_PricingDeatilModel> GetLocationByCustomerId(string keyword, int nCustId, int nEstId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = from cpm in _db.co_pricing_masters
                   join l in _db.locations on cpm.location_id equals l.location_id
                   where l.location_name.ToUpper().StartsWith(keyword.Trim().ToUpper()) && cpm.customer_id == nCustId && cpm.estimate_id == nEstId
                   orderby l.location_name
                   select new CO_PricingDeatilModel
                   {
                       location_name = l.location_name.Trim()
                   };

        if (item.ToList().Count() == 0 && keyword.ToLower().Contains("*"))
        {
            item = from cpm in _db.co_pricing_masters
                   join l in _db.locations on cpm.location_id equals l.location_id
                   where cpm.customer_id == nCustId && cpm.estimate_id == nEstId
                   orderby l.location_name
                   select new CO_PricingDeatilModel
                   {
                       location_name = l.location_name.Trim()
                   };
        }

        return item.Distinct().OrderBy(l => l.location_name).ToList();
    }

    [System.Web.Services.WebMethod]
    public static List<userinfo> GetSalesPerson(string keyword)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = (from cr in _db.Crew_Details
                    where cr.first_name.ToUpper().StartsWith(keyword.Trim().ToUpper()) && cr.is_active == true
                    orderby cr.first_name
                    select new userinfo
                    {
                        first_name = cr.first_name.Trim(),
                        last_name = cr.last_name.Trim(),
                        sales_person_id = cr.crew_id,
                        sales_person_name = cr.first_name.Trim() + " " + cr.last_name.Trim()
                    });

        if (item.ToList().Count() == 0 && keyword.ToLower().Contains("*"))
        {
            item = (from cr in _db.Crew_Details
                    where cr.is_active == true
                    orderby cr.first_name
                    select new userinfo
                    {
                        first_name = cr.first_name.Trim(),
                        last_name = cr.last_name.Trim(),
                        sales_person_id = cr.crew_id,
                        sales_person_name = cr.first_name.Trim() + " " + cr.last_name.Trim()
                    });
        }

        return item.Distinct().OrderBy(f => f.first_name).ToList();

    }

    [System.Web.Services.WebMethod]
    public static List<csScheduleCalendar> GetSubsequentSection(string keyword, int nCustId, int nEstId, string ParentEventName)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        List<csScheduleCalendar> listSC = new List<csScheduleCalendar>();
        string sSql = "";
        string strCondition = "";

        customer objcust = _db.customers.SingleOrDefault(c => c.customer_id == nCustId);
        if (objcust != null)
        {
            if ((bool)objcust.isCalendarOnline)
            {
                if (keyword.ToLower().Contains("*"))
                {
                    strCondition = " WHERE sc.IsScheduleDayException = 0 AND sc.[customer_id] = " + nCustId + " AND sc.[estimate_id] = " + nEstId + " AND  RTRIM(LTRIM(sc.[title])) <> '" + ParentEventName.Trim().Replace("&amp;", "&") + "' " +
                        " AND " +
                        " (event_id NOT IN " +
                        " (SELECT parent_event_id " +
                        " FROM allparent AS allparent_1)) " +
                        " AND " +
                        " sc.event_id not in (select child_event_id from ScheduleCalendarLink where  (customer_id = " + nCustId + ") AND (estimate_id = " + nEstId + ")) ";
                }
                else
                {
                    strCondition = " WHERE sc.IsScheduleDayException = 0 AND sc.[title] LIKE '" + keyword.Trim() + "%' AND sc.[customer_id] = " + nCustId + " AND sc.[estimate_id] = " + nEstId + " AND RTRIM(LTRIM(sc.[title])) <> '" + ParentEventName.Trim().Replace("&amp;", "&") + "' " +
                                " AND " +
                                " (event_id NOT IN " +
                                " (SELECT parent_event_id " +
                                " FROM allparent AS allparent_1)) " +
                                " AND " +
                                " sc.event_id not in (select child_event_id from ScheduleCalendarLink where  (customer_id = " + nCustId + ") AND (estimate_id = " + nEstId + ")) ";
                }

                sSql = "WITH allparent AS (SELECT parent_event_id " +
                    " FROM ScheduleCalendarLink AS scl " +
                    " WHERE (child_event_id = " +
                    " (SELECT event_id " +
                    " FROM ScheduleCalendar AS sc1 " +
                    " WHERE (RTRIM(LTRIM(title)) = '" + ParentEventName.Trim().Replace("&amp;", "&") + "') AND (customer_id = " + nCustId + ") AND (estimate_id = " + nEstId + "))) " +
                    " UNION ALL " +
                    " SELECT scl2.parent_event_id " +
                    " FROM ScheduleCalendarLink AS scl2 INNER JOIN " +
                    " allparent AS allparent_2 ON scl2.child_event_id = allparent_2.parent_event_id) " +
                    " " +
                    " SELECT sc.[event_id], sc.[title] + '...' + CONVERT(varchar, sc.[event_start], 101) + ' To ' +  CONVERT(varchar, sc.[event_end], 101) AS [section_name] " +
                     " FROM [dbo].[ScheduleCalendar] AS sc " +
                     " " + strCondition + " " +
                     " ORDER BY sc.[title]";
            }
            else
            {
                if (keyword.ToLower().Contains("*"))
                {
                    strCondition = " WHERE sc.IsScheduleDayException = 0 AND sc.[customer_id] = " + nCustId + " AND sc.[estimate_id] = " + nEstId + " AND RTRIM(LTRIM(sc.[title])) <> '" + ParentEventName.Trim().Replace("&amp;", "&") + "' " +
                        " AND " +
                        " (event_id NOT IN " +
                        " (SELECT parent_event_id " +
                        " FROM allparent AS allparent_1)) " +
                        " AND " +
                        " sc.event_id not in (select child_event_id from ScheduleCalendarLinkTemp where  (customer_id = " + nCustId + ") AND (estimate_id = " + nEstId + ")) ";
                }
                else
                {
                    strCondition = " WHERE sc.IsScheduleDayException = 0 AND sc.[title] LIKE '" + keyword.Trim() + "%' AND sc.[customer_id] = " + nCustId + " AND sc.[estimate_id] = " + nEstId + " AND RTRIM(LTRIM(sc.[title])) <> '" + ParentEventName.Trim().Replace("&amp;", "&") + "' " +
                                " AND " +
                                " (event_id NOT IN " +
                                " (SELECT parent_event_id " +
                                " FROM allparent AS allparent_1)) " +
                                " AND " +
                                " sc.event_id not in (select child_event_id from ScheduleCalendarLinkTemp where  (customer_id = " + nCustId + ") AND (estimate_id = " + nEstId + ")) ";
                }

                sSql = "WITH allparent AS (SELECT parent_event_id " +
                    " FROM ScheduleCalendarLinkTemp AS scl " +
                    " WHERE (child_event_id = " +
                    " (SELECT event_id " +
                    " FROM ScheduleCalendarTemp AS sc1 " +
                    " WHERE (RTRIM(LTRIM(title)) = '" + ParentEventName.Trim().Replace("&amp;", "&") + "') AND (customer_id = " + nCustId + ") AND (estimate_id = " + nEstId + "))) " +
                    " UNION ALL " +
                    " SELECT scl2.parent_event_id " +
                    " FROM ScheduleCalendarLinkTemp AS scl2 INNER JOIN " +
                    " allparent AS allparent_2 ON scl2.child_event_id = allparent_2.parent_event_id) " +
                    " " +
                    "SELECT sc.[event_id], sc.[title] + '...' + CONVERT(varchar, sc.[event_start], 101) + ' To ' +  CONVERT(varchar, sc.[event_end], 101) AS [section_name] " +
                     " FROM [dbo].[ScheduleCalendarTemp] AS sc " +
                     " " + strCondition + " " +
                     " ORDER BY sc.[title]";
            }
        }

        listSC = _db.ExecuteQuery<csScheduleCalendar>(sSql).ToList();


        return listSC.Distinct().ToList();
    }

    [System.Web.Services.WebMethod]
    public static List<csScheduleCalendar> GetParentSection(string keyword, int nCustId, int nEstId, string ParentEventName)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        List<csScheduleCalendar> listSC = new List<csScheduleCalendar>();
        string sSql = "";
        string strCondition = "";

        customer objcust = _db.customers.SingleOrDefault(c => c.customer_id == nCustId);
        if (objcust != null)
        {
            if ((bool)objcust.isCalendarOnline)
            {
                if (keyword.ToLower().Contains("*"))
                {
                    strCondition = " WHERE sc.IsScheduleDayException = 0 AND sc.[customer_id] = " + nCustId + " AND sc.[estimate_id] = " + nEstId + " AND RTRIM(LTRIM(sc.[title])) <> '" + ParentEventName.Trim().Replace("&amp;", "&") + "' " +
                                 " AND " +
                                 " event_id NOT IN ( " +
                                " SELECT [child_event_id] " +
                                " FROM allchild) ";
                }
                else
                {
                    strCondition = " WHERE sc.IsScheduleDayException = 0 AND sc.[title] LIKE '" + keyword.Trim() + "%' AND sc.[customer_id] = " + nCustId + " AND sc.[estimate_id] = " + nEstId + " AND RTRIM(LTRIM(sc.[title])) <> '" + ParentEventName.Trim().Replace("&amp;", "&") + "' " +
                                " AND " +
                                 " event_id NOT IN ( " +
                                " SELECT [child_event_id] " +
                                " FROM allchild) ";
                }

                sSql = " with allchild  " +
                        " AS (select scl.[child_event_id] " +
                        " FROM ScheduleCalendarLink as scl " +
                        " WHERE [parent_event_id] = (SELECT event_id FROM ScheduleCalendar  AS sc1 WHERE  RTRIM(LTRIM(sc1.title)) = '" + ParentEventName.Trim().Replace("&amp;", "&") + "' and (customer_id = " + nCustId + ") AND (estimate_id = " + nEstId + ")) " +
                        " UNION ALL " +
                        " select scl2.[child_event_id] " +
                        " FROM ScheduleCalendarLink as scl2 " +
                        " JOIN allchild  " +
                        " ON scl2.[parent_event_id] = allchild.[child_event_id]) " +
                        "  " +
                        " SELECT sc.[event_id], sc.[title] + '...' + CONVERT(varchar, sc.[event_start], 101) + ' To ' +  CONVERT(varchar, sc.[event_end], 101) AS [section_name] " +
                        " FROM [dbo].[ScheduleCalendar] AS sc " +
                        " " + strCondition + " " +
                        " ORDER BY sc.[title]";
            }
            else
            {

                if (keyword.ToLower().Contains("*"))
                {
                    strCondition = " WHERE sc.IsScheduleDayException = 0 AND sc.[customer_id] = " + nCustId + " AND sc.[estimate_id] = " + nEstId + " AND RTRIM(LTRIM(sc.[title])) <> '" + ParentEventName.Trim().Replace("&amp;", "&") + "' " +
                                " AND " +
                                " event_id NOT IN ( " +
                                " SELECT [child_event_id] " +
                                " FROM allchild) ";
                }
                else
                {
                    strCondition = " WHERE sc.IsScheduleDayException = 0 AND sc.[title] LIKE '" + keyword.Trim() + "%' AND sc.[customer_id] = " + nCustId + " AND sc.[estimate_id] = " + nEstId + " AND RTRIM(LTRIM(sc.[title])) <> '" + ParentEventName.Trim().Replace("&amp;", "&") + "' " +
                                 " AND " +
                                " event_id NOT IN ( " +
                                " SELECT [child_event_id] " +
                                " FROM allchild) ";
                }

                sSql = " with allchild  " +
                        " AS (select scl.[child_event_id] " +
                        " FROM ScheduleCalendarLinkTemp as scl " +
                        " WHERE [parent_event_id] = (SELECT event_id FROM ScheduleCalendarTemp  AS sc1 WHERE  RTRIM(LTRIM(sc1.title)) = '" + ParentEventName.Trim().Replace("&amp;", "&") + "' and (customer_id = " + nCustId + ") AND (estimate_id = " + nEstId + ")) " +
                        " UNION ALL " +
                        " select scl2.[child_event_id] " +
                        " FROM ScheduleCalendarLinkTemp as scl2 " +
                        " JOIN allchild  " +
                        " ON scl2.[parent_event_id] = allchild.[child_event_id]) " +
                        "  " +
                        " SELECT sc.[event_id], sc.[title] + '...' + CONVERT(varchar, sc.[event_start], 101) + ' To ' +  CONVERT(varchar, sc.[event_end], 101) AS [section_name] " +
                        " FROM [dbo].[ScheduleCalendarTemp] AS sc " +
                        " " + strCondition + " " +
                        " ORDER BY sc.[title]";
            }
        }


        listSC = _db.ExecuteQuery<csScheduleCalendar>(sSql).ToList();

        return listSC.Distinct().ToList();
    }

    [System.Web.Services.WebMethod]
    public static String GetSomeData()
    {
        int ncid = 0;
        int neid = 0;
        if (System.Web.HttpContext.Current.Session["cid"] != null)
        {
            ncid = (int)System.Web.HttpContext.Current.Session["cid"];
        }
        if (System.Web.HttpContext.Current.Session["eid"] != null)
        {
            neid = (int)System.Web.HttpContext.Current.Session["eid"];
        }
        GridView gv = new GridView();
        System.IO.StringWriter stringWriter = new System.IO.StringWriter();
        HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
        DataClassesDataContext _db = new DataClassesDataContext();
        var item = from scTemp in _db.ScheduleCalendarTemps
                   join linktemp in _db.ScheduleCalendarLinkTemps on scTemp.event_id equals linktemp.child_event_id
                   where scTemp.customer_id == ncid && scTemp.estimate_id == neid
                   select new
                   {
                       link_id = linktemp.link_id,
                       title = scTemp.title,
                       start = scTemp.event_start,
                       end = scTemp.event_end
                   };

        gv.DataSource = item.ToList();
        gv.DataKeyNames = new string[] { "link_id" };
        gv.DataBind();

        gv.RenderControl(htmlWriter);
        return stringWriter.ToString();
    }


    [System.Web.Services.WebMethod]
    public static string AddEventLink(string datascEventLinks)
    {
        System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        scEventLink scEventLinks = json.Deserialize<scEventLink>(datascEventLinks);
        int nDuration = 0;
        string result = "Ok";
        try
        {

            DataClassesDataContext _db = new DataClassesDataContext();


            ScheduleCalendarLinkTemp objSCLinkTmp = new ScheduleCalendarLinkTemp();

            ScheduleCalendarTemp objParentSCTmp = _db.ScheduleCalendarTemps.SingleOrDefault(sc => sc.event_id == scEventLinks.parent_event_id && sc.customer_id == scEventLinks.customer_id && sc.estimate_id == scEventLinks.estimate_id);
            ScheduleCalendarTemp objChildSCTmp = _db.ScheduleCalendarTemps.SingleOrDefault(sc => sc.event_id == scEventLinks.child_event_id && sc.customer_id == scEventLinks.customer_id && sc.estimate_id == scEventLinks.estimate_id);


            if (objParentSCTmp != null && objChildSCTmp != null)
            {

                int nLinkId = 1;

                if (_db.ScheduleCalendarLinkTemps.Any())
                {
                    int nMaxSCLink = Convert.ToInt32(_db.ScheduleCalendarLinks.DefaultIfEmpty().Max(e => e == null ? 0 : e.link_id));
                    int nMaxSCLinkTemp = Convert.ToInt32(_db.ScheduleCalendarLinkTemps.DefaultIfEmpty().Max(e => e == null ? 0 : e.link_id));

                    if (nMaxSCLinkTemp > nMaxSCLink)
                        nLinkId = nMaxSCLinkTemp + 1;
                    else
                        nLinkId = nMaxSCLink + 1;
                }
                objSCLinkTmp.link_id = nLinkId;
                objSCLinkTmp.parent_event_id = scEventLinks.parent_event_id;
                objSCLinkTmp.child_event_id = scEventLinks.child_event_id;
                objSCLinkTmp.customer_id = objParentSCTmp.customer_id;
                objSCLinkTmp.estimate_id = objParentSCTmp.estimate_id;
                objSCLinkTmp.dependencyType = scEventLinks.dependencyType;
                objSCLinkTmp.lag = scEventLinks.offsetdays;

                _db.ScheduleCalendarLinkTemps.InsertOnSubmit(objSCLinkTmp);
                _db.SubmitChanges();

                if (scEventLinks.dependencyType == 1) // Start Same Time
                {
                    nDuration = Convert.ToInt32(objChildSCTmp.duration) - 1;
                    DateTime dtOldStart = Convert.ToDateTime(objChildSCTmp.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(objChildSCTmp.event_end);
                    DateTime dtNewStart = Convert.ToDateTime(objParentSCTmp.event_start);

                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }

                    objChildSCTmp.event_start = dtNewStart;
                    objChildSCTmp.event_end = dtNewEnd;
                    objChildSCTmp.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }

                if (scEventLinks.dependencyType == 2) // Start After Finish
                {

                    nDuration = Convert.ToInt32(objChildSCTmp.duration) - 1;
                    DateTime dtOldStart = Convert.ToDateTime(objChildSCTmp.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(objChildSCTmp.event_end);


                    DateTime dtNewStart = GetWorkingDay(Convert.ToDateTime(objParentSCTmp.event_end).AddDays(1));

                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }



                    objChildSCTmp.event_start = dtNewStart;

                    objChildSCTmp.event_end = dtNewEnd;
                    objChildSCTmp.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }

                if (scEventLinks.dependencyType == 3) // Offset days
                {


                    nDuration = Convert.ToInt32(objChildSCTmp.duration) - 1;


                    DateTime dtOldStart = Convert.ToDateTime(objChildSCTmp.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(objChildSCTmp.event_end);


                    DateTime dtNewStart = Convert.ToDateTime(objParentSCTmp.event_end);

                    for (int i = 0; i <= scEventLinks.offsetdays; i++)
                    {
                        dtNewStart = GetWorkingDay(dtNewStart.AddDays(1));

                    }


                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }

                    objChildSCTmp.event_start = dtNewStart;

                    objChildSCTmp.event_end = dtNewEnd;
                    objChildSCTmp.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }


                updateAllLinkOnline(scEventLinks.child_event_id, scEventLinks.customer_id, scEventLinks.estimate_id);
            }


        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        return result;
    }

    [System.Web.Services.WebMethod]
    public static String DeleteEventLink(string datascEventLinks)
    {
        string result = "";
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<scEventLink> scEventLinks = json.Deserialize<List<scEventLink>>(datascEventLinks);

            foreach (scEventLink sclink in scEventLinks)
            {

                string sqlDELETETemp = "DELETE ScheduleCalendarLinkTemp WHERE parent_event_id = " + sclink.parent_event_id + " AND child_event_id = " + sclink.child_event_id + " AND customer_id = " + sclink.customer_id + " AND estimate_id = " + sclink.estimate_id + "";
                _db.ExecuteCommand(sqlDELETETemp, string.Empty);

                string sqlDELETE = "DELETE ScheduleCalendarLink WHERE parent_event_id = " + sclink.parent_event_id + " AND child_event_id = " + sclink.child_event_id + " AND customer_id = " + sclink.customer_id + " AND estimate_id = " + sclink.estimate_id + "";
                _db.ExecuteCommand(sqlDELETE, string.Empty);

                System.Web.HttpContext.Current.Session["cid"] = sclink.customer_id;
                System.Web.HttpContext.Current.Session["eid"] = sclink.estimate_id;

            }

            result = "Ok";
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        return result;
    }

    [System.Web.Services.WebMethod]
    public static String UpdateEventLink(string datascEventLinks, int id)
    {
        System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<scEventLink> scEventLinks = json.Deserialize<List<scEventLink>>(datascEventLinks);

        string result = "";
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int ncid = 0;
            int neid = 0;
            if (System.Web.HttpContext.Current.Session["cid"] != null)
            {
                ncid = (int)System.Web.HttpContext.Current.Session["cid"];
            }
            if (System.Web.HttpContext.Current.Session["eid"] != null)
            {
                neid = (int)System.Web.HttpContext.Current.Session["eid"];
            }

            foreach (scEventLink sclink in scEventLinks)
            {
                int nLinkId = sclink.link_id;
                int nDependencyType = sclink.dependencyType;

                int nParentEventId = sclink.parent_event_id;
                int nChildEventId = sclink.child_event_id;

                ncid = sclink.customer_id;
                neid = sclink.estimate_id;

                int nOffsetDays = 0;
                if (nDependencyType == 1)
                {
                    nOffsetDays = 0;
                }
                else if (nDependencyType == 2)
                {
                    nOffsetDays = 1;
                }
                else if (nDependencyType == 3)
                {
                    nOffsetDays = sclink.offsetdays;
                }


                if (_db.ScheduleCalendarTemps.Any(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid))
                {
                    ScheduleCalendarLinkTemp objSCLinkTmp = _db.ScheduleCalendarLinkTemps.FirstOrDefault(lnk => lnk.parent_event_id == nParentEventId && lnk.child_event_id == nChildEventId && lnk.customer_id == ncid && lnk.estimate_id == neid);

                    if (objSCLinkTmp != null)
                    {
                        objSCLinkTmp.dependencyType = nDependencyType;
                        objSCLinkTmp.lag = nOffsetDays;
                        _db.SubmitChanges();

                        ScheduleCalendarTemp ocjSCTempP = _db.ScheduleCalendarTemps.FirstOrDefault(sc => sc.event_id == objSCLinkTmp.parent_event_id && sc.customer_id == ncid && sc.estimate_id == neid);
                        ScheduleCalendarTemp ocjSCTempC = _db.ScheduleCalendarTemps.FirstOrDefault(s => s.event_id == objSCLinkTmp.child_event_id);


                        if (nDependencyType == 1) // Start Same Time
                        {
                            int nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;
                            DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                            DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);
                            DateTime dtNewStart = Convert.ToDateTime(ocjSCTempP.event_start);

                            dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                            DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                            for (int i = 1; i <= nDuration; i++)
                            {
                                dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                            }

                            ocjSCTempC.event_start = dtNewStart;
                            ocjSCTempC.event_end = dtNewEnd;
                            ocjSCTempC.IsEWSCalendarSynch = false;
                            _db.SubmitChanges();
                        }

                        if (nDependencyType == 2) // Start After Finish
                        {

                            int nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;
                            DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                            DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);


                            DateTime dtNewStart = GetWorkingDay(Convert.ToDateTime(ocjSCTempP.event_end).AddDays(1));

                            dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                            DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                            for (int i = 1; i <= nDuration; i++)
                            {
                                dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                            }

                            ocjSCTempC.event_start = dtNewStart;
                            ocjSCTempC.event_end = dtNewEnd;
                            ocjSCTempC.IsEWSCalendarSynch = false;
                            _db.SubmitChanges();
                        }

                        if (nDependencyType == 3) // Offset days
                        {
                            int nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;


                            DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                            DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);


                            DateTime dtNewStart = Convert.ToDateTime(ocjSCTempP.event_end);

                            for (int i = 0; i <= nOffsetDays; i++)
                            {
                                dtNewStart = GetWorkingDay(dtNewStart.AddDays(1));

                            }


                            dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                            DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                            for (int i = 1; i <= nDuration; i++)
                            {
                                dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                            }

                            ocjSCTempC.event_start = dtNewStart;
                            ocjSCTempC.event_end = dtNewEnd;
                            ocjSCTempC.IsEWSCalendarSynch = false;
                            _db.SubmitChanges();
                        }

                        updateAllLink((int)objSCLinkTmp.child_event_id, (int)objSCLinkTmp.customer_id, (int)objSCLinkTmp.estimate_id);
                    }

                }
                if (_db.ScheduleCalendars.Any(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid)) // Online Schedule Data
                {
                    ScheduleCalendarLink objSCLinkOnlin = _db.ScheduleCalendarLinks.FirstOrDefault(lnk => lnk.parent_event_id == nParentEventId && lnk.child_event_id == nChildEventId && lnk.customer_id == ncid && lnk.estimate_id == neid);
                    if (objSCLinkOnlin != null)
                    {
                        objSCLinkOnlin.dependencyType = nDependencyType;
                        objSCLinkOnlin.lag = nOffsetDays;
                        _db.SubmitChanges();

                        ScheduleCalendar ocjSCOnlinP = _db.ScheduleCalendars.FirstOrDefault(sc => sc.event_id == objSCLinkOnlin.parent_event_id && sc.customer_id == ncid && sc.estimate_id == neid);
                        ScheduleCalendar ocjSCOnlinC = _db.ScheduleCalendars.FirstOrDefault(s => s.event_id == objSCLinkOnlin.child_event_id);


                        if (nDependencyType == 1) // Start Same Time
                        {
                            int nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;
                            DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                            DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);
                            DateTime dtNewStart = Convert.ToDateTime(ocjSCOnlinP.event_start);

                            dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                            DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                            for (int i = 1; i <= nDuration; i++)
                            {
                                dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                            }

                            ocjSCOnlinC.event_start = dtNewStart;
                            ocjSCOnlinC.event_end = dtNewEnd;
                            ocjSCOnlinC.IsEWSCalendarSynch = false;
                            _db.SubmitChanges();
                        }

                        if (nDependencyType == 2) // Start After Finish
                        {

                            int nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;
                            DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                            DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);


                            DateTime dtNewStart = GetWorkingDay(Convert.ToDateTime(ocjSCOnlinP.event_end).AddDays(1));

                            dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                            DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                            for (int i = 1; i <= nDuration; i++)
                            {
                                dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                            }

                            ocjSCOnlinC.event_start = dtNewStart;
                            ocjSCOnlinC.event_end = dtNewEnd;
                            ocjSCOnlinC.IsEWSCalendarSynch = false;
                            _db.SubmitChanges();
                        }

                        if (nDependencyType == 3) // Offset days
                        {
                            int nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;


                            DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                            DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);


                            DateTime dtNewStart = Convert.ToDateTime(ocjSCOnlinP.event_end);

                            for (int i = 0; i <= nOffsetDays; i++)
                            {
                                dtNewStart = GetWorkingDay(dtNewStart.AddDays(1));

                            }


                            dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                            DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                            for (int i = 1; i <= nDuration; i++)
                            {
                                dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                            }

                            ocjSCOnlinC.event_start = dtNewStart;
                            ocjSCOnlinC.event_end = dtNewEnd;
                            ocjSCOnlinC.IsEWSCalendarSynch = false;
                            _db.SubmitChanges();
                        }

                        updateAllLinkOnline((int)objSCLinkOnlin.child_event_id, (int)objSCLinkOnlin.customer_id, (int)objSCLinkOnlin.estimate_id);  // Online Schedule Data
                    }
                }
                System.Web.HttpContext.Current.Session["cid"] = ncid;
                System.Web.HttpContext.Current.Session["eid"] = neid;
                result = "Ok";
            }

        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        return result;
    }

    [System.Web.Services.WebMethod]
    public static String DeleteParentEventLink(string datascEventLinks)
    {

        string result = "";
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<scEventLink> scEventLinks = json.Deserialize<List<scEventLink>>(datascEventLinks);

            foreach (scEventLink sclink in scEventLinks)
            {

                string sqlDELETETemp = "DELETE ScheduleCalendarLinkTemp WHERE parent_event_id = " + sclink.parent_event_id + " AND child_event_id = " + sclink.child_event_id + " AND customer_id = " + sclink.customer_id + " AND estimate_id = " + sclink.estimate_id + "";
                _db.ExecuteCommand(sqlDELETETemp, string.Empty);

                string sqlDELETE = "DELETE ScheduleCalendarLink WHERE parent_event_id = " + sclink.parent_event_id + " AND child_event_id = " + sclink.child_event_id + " AND customer_id = " + sclink.customer_id + " AND estimate_id = " + sclink.estimate_id + "";
                _db.ExecuteCommand(sqlDELETE, string.Empty);

                System.Web.HttpContext.Current.Session["cid"] = sclink.customer_id;
                System.Web.HttpContext.Current.Session["eid"] = sclink.estimate_id;
            }

            result = "Ok";
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        return result;
    }

    [System.Web.Services.WebMethod]
    public static String UpdateParentEventLink(string datascEventLinks, int id)
    {
        System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<scEventLink> scEventLinks = json.Deserialize<List<scEventLink>>(datascEventLinks);

        string result = "";
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int ncid = 0;
            int neid = 0;
            if (System.Web.HttpContext.Current.Session["cid"] != null)
            {
                ncid = (int)System.Web.HttpContext.Current.Session["cid"];
            }
            if (System.Web.HttpContext.Current.Session["eid"] != null)
            {
                neid = (int)System.Web.HttpContext.Current.Session["eid"];
            }


            foreach (scEventLink sclink in scEventLinks)
            {
                int nEventId = sclink.link_id;
                int nDependencyType = sclink.dependencyType;

                ncid = sclink.customer_id;
                neid = sclink.estimate_id;

                int nOffsetDays = 0;
                if (nDependencyType == 1)
                {
                    nOffsetDays = 0;
                }
                else if (nDependencyType == 2)
                {
                    nOffsetDays = 1;
                }
                else if (nDependencyType == 3)
                {
                    nOffsetDays = sclink.offsetdays;
                }

                if (_db.ScheduleCalendarTemps.Any(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid))
                {

                    ScheduleCalendarLinkTemp objSCLinkTmp = _db.ScheduleCalendarLinkTemps.FirstOrDefault(lnk => lnk.child_event_id == nEventId && lnk.customer_id == ncid && lnk.estimate_id == neid);
                    if (objSCLinkTmp != null)
                    {
                        ScheduleCalendarTemp ocjSCTempP = _db.ScheduleCalendarTemps.FirstOrDefault(s => s.event_id == objSCLinkTmp.parent_event_id);
                        ScheduleCalendarTemp ocjSCTempC = _db.ScheduleCalendarTemps.FirstOrDefault(s => s.event_id == objSCLinkTmp.child_event_id);

                        if (ocjSCTempP != null)
                        {
                            if (nDependencyType == 1) // Start Same Time
                            {
                                int nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;
                                DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                                DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);
                                DateTime dtNewStart = Convert.ToDateTime(ocjSCTempP.event_start);

                                dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                for (int i = 1; i <= nDuration; i++)
                                {
                                    dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                                }

                                ocjSCTempC.event_start = dtNewStart;
                                ocjSCTempC.event_end = dtNewEnd;
                                ocjSCTempC.IsEWSCalendarSynch = false;
                                _db.SubmitChanges();
                            }

                            if (nDependencyType == 2) // Start After Finish
                            {

                                int nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;
                                DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                                DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);


                                DateTime dtNewStart = GetWorkingDay(Convert.ToDateTime(ocjSCTempP.event_end).AddDays(1));

                                dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                for (int i = 1; i <= nDuration; i++)
                                {
                                    dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                                }

                                ocjSCTempC.event_start = dtNewStart;
                                ocjSCTempC.event_end = dtNewEnd;
                                ocjSCTempC.IsEWSCalendarSynch = false;
                                _db.SubmitChanges();
                            }

                            if (nDependencyType == 3) // Offset days
                            {
                                int nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;


                                DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                                DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);


                                DateTime dtNewStart = Convert.ToDateTime(ocjSCTempP.event_end);

                                for (int i = 0; i <= nOffsetDays; i++)
                                {
                                    dtNewStart = GetWorkingDay(dtNewStart.AddDays(1));

                                }


                                dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                for (int i = 1; i <= nDuration; i++)
                                {
                                    dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                                }

                                ocjSCTempC.event_start = dtNewStart;
                                ocjSCTempC.event_end = dtNewEnd;
                                ocjSCTempC.IsEWSCalendarSynch = false;
                                _db.SubmitChanges();
                            }


                            if (objSCLinkTmp != null)
                            {
                                objSCLinkTmp.dependencyType = nDependencyType;
                                objSCLinkTmp.lag = nOffsetDays;
                            }
                        }

                        _db.SubmitChanges();

                        updateAllLink((int)objSCLinkTmp.child_event_id, (int)objSCLinkTmp.customer_id, (int)objSCLinkTmp.estimate_id);
                    }
                }
                if (_db.ScheduleCalendars.Any(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid)) // Online Schedule Data
                {
                    ScheduleCalendarLink objSCLinkOnlin = _db.ScheduleCalendarLinks.FirstOrDefault(lnk => lnk.child_event_id == nEventId && lnk.customer_id == ncid && lnk.estimate_id == neid);
                    if (objSCLinkOnlin != null)
                    {
                        ScheduleCalendar ocjSCOnlinP = _db.ScheduleCalendars.FirstOrDefault(s => s.event_id == objSCLinkOnlin.parent_event_id);
                        ScheduleCalendar ocjSCOnlinC = _db.ScheduleCalendars.FirstOrDefault(s => s.event_id == objSCLinkOnlin.child_event_id);

                        if (ocjSCOnlinP != null)
                        {
                            if (nDependencyType == 1) // Start Same Time
                            {
                                int nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;
                                DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                                DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);
                                DateTime dtNewStart = Convert.ToDateTime(ocjSCOnlinP.event_start);

                                dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                for (int i = 1; i <= nDuration; i++)
                                {
                                    dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                                }

                                ocjSCOnlinC.event_start = dtNewStart;
                                ocjSCOnlinC.event_end = dtNewEnd;
                                ocjSCOnlinC.IsEWSCalendarSynch = false;
                                _db.SubmitChanges();
                            }

                            if (nDependencyType == 2) // Start After Finish
                            {

                                int nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;
                                DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                                DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);


                                DateTime dtNewStart = GetWorkingDay(Convert.ToDateTime(ocjSCOnlinP.event_end).AddDays(1));

                                dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                for (int i = 1; i <= nDuration; i++)
                                {
                                    dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                                }

                                ocjSCOnlinC.event_start = dtNewStart;
                                ocjSCOnlinC.event_end = dtNewEnd;
                                ocjSCOnlinC.IsEWSCalendarSynch = false;
                                _db.SubmitChanges();
                            }

                            if (nDependencyType == 3) // Offset days
                            {
                                int nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;


                                DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                                DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);


                                DateTime dtNewStart = Convert.ToDateTime(ocjSCOnlinP.event_end);

                                for (int i = 0; i <= nOffsetDays; i++)
                                {
                                    dtNewStart = GetWorkingDay(dtNewStart.AddDays(1));

                                }


                                dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                                DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                                for (int i = 1; i <= nDuration; i++)
                                {
                                    dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                                }

                                ocjSCOnlinC.event_start = dtNewStart;
                                ocjSCOnlinC.event_end = dtNewEnd;
                                ocjSCOnlinC.IsEWSCalendarSynch = false;
                                _db.SubmitChanges();
                            }


                            if (objSCLinkOnlin != null)
                            {
                                objSCLinkOnlin.dependencyType = nDependencyType;
                                objSCLinkOnlin.lag = nOffsetDays;
                            }
                        }

                        _db.SubmitChanges();

                        updateAllLinkOnline((int)objSCLinkOnlin.child_event_id, (int)objSCLinkOnlin.customer_id, (int)objSCLinkOnlin.estimate_id);
                    }
                }
            }

            System.Web.HttpContext.Current.Session["cid"] = ncid;
            System.Web.HttpContext.Current.Session["eid"] = neid;

            result = "Ok";

        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        return result;
    }

    public static void updateAllLink(int id, int ncid, int neid)
    {

        if (System.Web.HttpContext.Current.Session["cid"] != null)
        {
            ncid = (int)System.Web.HttpContext.Current.Session["cid"];
        }
        if (System.Web.HttpContext.Current.Session["eid"] != null)
        {
            neid = (int)System.Web.HttpContext.Current.Session["eid"];
        }
        DataClassesDataContext _db = new DataClassesDataContext();

        ScheduleCalendarTemp ocjSCTempUpdate = _db.ScheduleCalendarTemps.SingleOrDefault(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid);
        //If Event is Parent
        List<ScheduleCalendarLinkTemp> objSCLinkTmpP = new List<ScheduleCalendarLinkTemp>();

        if (_db.ScheduleCalendarLinkTemps.Any(sl => sl.parent_event_id == id))
        {
            objSCLinkTmpP = _db.ScheduleCalendarLinkTemps.Where(sl => sl.parent_event_id == id).ToList();

            foreach (ScheduleCalendarLinkTemp slT in objSCLinkTmpP)
            {
                ScheduleCalendarTemp ocjSCTempC = _db.ScheduleCalendarTemps.FirstOrDefault(s => s.event_id == slT.child_event_id && slT.customer_id == ncid && slT.estimate_id == neid);

                int nDays = (Convert.ToDateTime(ocjSCTempC.event_end) - Convert.ToDateTime(ocjSCTempC.event_start)).Days;

                if (slT.dependencyType == 1) // Start Same Time
                {
                    int nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;
                    DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);
                    DateTime dtNewStart = Convert.ToDateTime(ocjSCTempUpdate.event_start);

                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }

                    ocjSCTempC.event_start = dtNewStart;
                    ocjSCTempC.event_end = dtNewEnd;
                    ocjSCTempC.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }

                if (slT.dependencyType == 2) // Start After Finish
                {
                    int nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;
                    DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);


                    DateTime dtNewStart = GetWorkingDay(Convert.ToDateTime(ocjSCTempUpdate.event_end).AddDays(1));

                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }

                    ocjSCTempC.event_start = dtNewStart;
                    ocjSCTempC.event_end = dtNewEnd;
                    ocjSCTempC.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }

                if (slT.dependencyType == 3) // Offset days
                {
                    int nDuration = Convert.ToInt32(ocjSCTempC.duration) - 1;


                    DateTime dtOldStart = Convert.ToDateTime(ocjSCTempC.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCTempC.event_end);


                    DateTime dtNewStart = Convert.ToDateTime(ocjSCTempUpdate.event_end);

                    for (int i = 0; i <= (int)slT.lag; i++)
                    {
                        dtNewStart = GetWorkingDay(dtNewStart.AddDays(1));

                    }


                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }

                    ocjSCTempC.event_start = dtNewStart;
                    ocjSCTempC.event_end = dtNewEnd;
                    ocjSCTempC.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }

                updateAllLink((int)slT.child_event_id, (int)slT.customer_id, (int)slT.estimate_id);
            }
        }
    }

    public static void updateAllLinkOnline(int id, int ncid, int neid)  // Online Schedule Data
    {
        if (System.Web.HttpContext.Current.Session["cid"] != null)
        {
            ncid = (int)System.Web.HttpContext.Current.Session["cid"];
        }
        if (System.Web.HttpContext.Current.Session["eid"] != null)
        {
            neid = (int)System.Web.HttpContext.Current.Session["eid"];
        }
        DataClassesDataContext _db = new DataClassesDataContext();

        ScheduleCalendar ocjSCOnlinUpdate = _db.ScheduleCalendars.SingleOrDefault(sc => sc.event_id == id && sc.customer_id == ncid && sc.estimate_id == neid);
        //If Event is Parent
        List<ScheduleCalendarLink> objSCLinkOnlinP = new List<ScheduleCalendarLink>();

        if (_db.ScheduleCalendarLinks.Any(sl => sl.parent_event_id == id))
        {
            objSCLinkOnlinP = _db.ScheduleCalendarLinks.Where(sl => sl.parent_event_id == id).ToList();

            foreach (ScheduleCalendarLink slT in objSCLinkOnlinP)
            {
                ScheduleCalendar ocjSCOnlinC = _db.ScheduleCalendars.FirstOrDefault(s => s.event_id == slT.child_event_id && slT.customer_id == ncid && slT.estimate_id == neid);

                int nDays = (Convert.ToDateTime(ocjSCOnlinC.event_end) - Convert.ToDateTime(ocjSCOnlinC.event_start)).Days;

                if (slT.dependencyType == 1) // Start Same Time
                {
                    int nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;
                    DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);
                    DateTime dtNewStart = Convert.ToDateTime(ocjSCOnlinUpdate.event_start);

                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }

                    ocjSCOnlinC.event_start = dtNewStart;
                    ocjSCOnlinC.event_end = dtNewEnd;
                    ocjSCOnlinC.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }

                if (slT.dependencyType == 2) // Start After Finish
                {
                    int nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;
                    DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);


                    DateTime dtNewStart = GetWorkingDay(Convert.ToDateTime(ocjSCOnlinUpdate.event_end).AddDays(1));

                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }

                    ocjSCOnlinC.event_start = dtNewStart;
                    ocjSCOnlinC.event_end = dtNewEnd;
                    ocjSCOnlinC.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }

                if (slT.dependencyType == 3) // Offset days
                {
                    int nDuration = Convert.ToInt32(ocjSCOnlinC.duration) - 1;


                    DateTime dtOldStart = Convert.ToDateTime(ocjSCOnlinC.event_start);
                    DateTime dtOldEnd = Convert.ToDateTime(ocjSCOnlinC.event_end);


                    DateTime dtNewStart = Convert.ToDateTime(ocjSCOnlinUpdate.event_end);

                    for (int i = 0; i <= (int)slT.lag; i++)
                    {
                        dtNewStart = GetWorkingDay(dtNewStart.AddDays(1));

                    }


                    dtNewStart = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldStart.Hour, dtOldStart.Minute, dtOldStart.Second);
                    DateTime dtNewEnd = new DateTime(dtNewStart.Year, dtNewStart.Month, dtNewStart.Day, dtOldEnd.Hour, dtOldEnd.Minute, dtOldEnd.Second);
                    for (int i = 1; i <= nDuration; i++)
                    {
                        dtNewEnd = GetWorkingDay(dtNewEnd.AddDays(1));

                    }

                    ocjSCOnlinC.event_start = dtNewStart;
                    ocjSCOnlinC.event_end = dtNewEnd;
                    ocjSCOnlinC.IsEWSCalendarSynch = false;
                    _db.SubmitChanges();
                }

                updateAllLinkOnline((int)slT.child_event_id, (int)slT.customer_id, (int)slT.estimate_id);
            }
        }
    }

    [System.Web.Services.WebMethod]
    public static String GetDayOfWeek(string strdt)
    {
        int cnt = 0;
        DateTime dt = Convert.ToDateTime(strdt);

        if (dt.DayOfWeek == DayOfWeek.Saturday)
            cnt = +2;
        else if (dt.DayOfWeek == DayOfWeek.Sunday)
            cnt++;
        else if (IsHoliday(dt))
        {
            DateTime hdt = dt.AddDays(1);

            if (hdt.DayOfWeek == DayOfWeek.Saturday)
                cnt = +3;
            else if (hdt.DayOfWeek == DayOfWeek.Sunday)
                cnt = +2;
            else
                cnt++;
        }

        var dtTest = dt.AddDays(cnt).ToShortDateString();

        return dt.AddDays(cnt).ToShortDateString();
    }

    [System.Web.Services.WebMethod]
    public static bool IsHoliday(DateTime dt)
    {
        bool IsHoliday = false;
        DateTime date = DateTime.Parse(dt.ToShortDateString());
        HolidayCalculator hc = new HolidayCalculator(date, "Holidays.xml");
        foreach (HolidayCalculator.Holiday h in hc.OrderedHolidays)
        {
            if (h.Date.ToShortDateString() == date.ToShortDateString())
            {
                IsHoliday = true;
            }
        }
        return IsHoliday;
    }

    public void ReSetDateDiff()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        List<ScheduleCalendarTemp> listSCTemp = _db.ScheduleCalendarTemps.ToList();



        foreach (ScheduleCalendarTemp obiSCTemp in listSCTemp)
        {

            obiSCTemp.duration = GetDuration(Convert.ToDateTime(obiSCTemp.event_start), Convert.ToDateTime(obiSCTemp.event_end));
            obiSCTemp.IsEWSCalendarSynch = false;
            _db.SubmitChanges();
        }


        List<ScheduleCalendar> listSC = _db.ScheduleCalendars.ToList();

        foreach (ScheduleCalendar obiSC in listSC)
        {

            obiSC.duration = GetDuration(Convert.ToDateTime(obiSC.event_start), Convert.ToDateTime(obiSC.event_end));
            obiSC.IsEWSCalendarSynch = false;
            _db.SubmitChanges();
        }
    }

    private static int GetHolidayCount(DateTime startDate, DateTime endDate)
    {
        int count = 0;
        TimeSpan diff = endDate - startDate;
        int days = diff.Days;

        bool bFound = false;
        for (var i = 0; i <= days; i++)
        {
            bFound = false;
            var testDate = startDate.AddDays(i);
            switch (testDate.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    Console.WriteLine(testDate.ToShortDateString());
                    count++;
                    bFound = true;
                    break;
            }

            if (bFound == false && IsHoliday(testDate))
                count++;


        }

        return count;
    }

    private static int GetDuration(DateTime startDate, DateTime endDate)
    {
        startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
        endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

        int nOffDay = GetHolidayCount(startDate, endDate);

        int nDuration = 1;

        TimeSpan diff = endDate - startDate;
        nDuration += diff.Days;

        return (nDuration - nOffDay);
    }

    private static DateTime GetWorkingDay(DateTime Date)
    {
        if (IsHoliday(Date))
            Date = Date.AddDays(1);

        switch (Date.DayOfWeek)
        {
            case DayOfWeek.Saturday:
                Date = Date.AddDays(2);
                break;
            case DayOfWeek.Sunday:
                Date = Date.AddDays(1);
                break;

        }

        return Date;
    }

    [System.Web.Services.WebMethod]
    public static String IsValideEventLink(int id, int ncid, int neid, DateTime start, DateTime end)
    {
        int nCheck = 0;
        DataClassesDataContext _db = new DataClassesDataContext();
        if (_db.ScheduleCalendarLinkTemps.Any(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid))
        {
            var objChildLink = _db.ScheduleCalendarLinkTemps.SingleOrDefault(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid);

            var objP = _db.ScheduleCalendarTemps.SingleOrDefault(sc => sc.event_id == objChildLink.parent_event_id);

            DateTime dtNewStartC = new DateTime(start.Year, start.Month, start.Day);
            DateTime dtNewEndP = new DateTime(Convert.ToDateTime(objP.event_end).Year, Convert.ToDateTime(objP.event_end).Month, Convert.ToDateTime(objP.event_end).Day);

            DateTime dtNewStartP = new DateTime(Convert.ToDateTime(objP.event_start).Year, Convert.ToDateTime(objP.event_start).Month, Convert.ToDateTime(objP.event_start).Day);

            TimeSpan diffPrntEndToChldStart = dtNewStartC - dtNewEndP; // Parent End date to Child Start Date
            TimeSpan diffPrnttStartToChldStart = dtNewStartC - dtNewStartP; // Parent Start date to Child Start Date

            int nDaysPrntEndToChldStart = diffPrntEndToChldStart.Days;// Parent End date to Child Start Date
            int nDaysPrnttStartToChldStart = diffPrnttStartToChldStart.Days;// Parent Start date to Child Start Date

            if (nDaysPrntEndToChldStart > 0)// Parent End date to Child Start Date
            {
                nCheck = 0;

            }
            else if (nDaysPrntEndToChldStart < 0)
            {
                nCheck = nDaysPrntEndToChldStart;

                if (nDaysPrnttStartToChldStart == 0)// Parent Start date to Child Start Date
                {
                    nCheck = 0;
                }
                else if (nDaysPrnttStartToChldStart > 0)// Parent Start date to Child Start Date
                {
                    nCheck = -1;
                }
            }
            else
            {
                nCheck = -1;
            }


            if (nCheck < 0)
            {
                return nCheck.ToString();
            }
        }

        else if (_db.ScheduleCalendarLinks.Any(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid))
        {
            var objChildLink = _db.ScheduleCalendarLinks.SingleOrDefault(sl => sl.child_event_id == id && sl.customer_id == ncid && sl.estimate_id == neid);

            var objP = _db.ScheduleCalendars.SingleOrDefault(sc => sc.event_id == objChildLink.parent_event_id);

            DateTime dtNewStartC = new DateTime(start.Year, start.Month, start.Day);
            DateTime dtNewEndP = new DateTime(Convert.ToDateTime(objP.event_end).Year, Convert.ToDateTime(objP.event_end).Month, Convert.ToDateTime(objP.event_end).Day);

            DateTime dtNewStartP = new DateTime(Convert.ToDateTime(objP.event_start).Year, Convert.ToDateTime(objP.event_start).Month, Convert.ToDateTime(objP.event_start).Day);

            TimeSpan diffPrntEndToChldStart = dtNewStartC - dtNewEndP; // Parent End date to Child Start Date
            TimeSpan diffPrnttStartToChldStart = dtNewStartC - dtNewStartP; // Parent Start date to Child Start Date

            int nDaysPrntEndToChldStart = diffPrntEndToChldStart.Days;// Parent End date to Child Start Date
            int nDaysPrnttStartToChldStart = diffPrnttStartToChldStart.Days;// Parent Start date to Child Start Date

            if (nDaysPrntEndToChldStart > 0)// Parent End date to Child Start Date
            {
                nCheck = 0;

            }
            else if (nDaysPrntEndToChldStart < 0)
            {
                nCheck = nDaysPrntEndToChldStart;

                if (nDaysPrnttStartToChldStart == 0)// Parent Start date to Child Start Date
                {
                    nCheck = 0;
                }
                else if (nDaysPrnttStartToChldStart > 0)// Parent Start date to Child Start Date
                {
                    nCheck = -1;
                }
            }
            else
            {
                nCheck = -1;
            }


            if (nCheck < 0)
            {
                return nCheck.ToString();
            }
        }

        return nCheck.ToString();
    }

    [System.Web.Services.WebMethod]
    public static List<ImproperCalendarLinkEvent> GetChildEventTable(int parentEventId, int ncid, int neid)
    {
        List<ImproperCalendarLinkEvent> objlnkEvent = new List<ImproperCalendarLinkEvent>();
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            bool IsCalendarOnline = true;
            var objCust = _db.customers.Where(c => c.customer_id == ncid);

            if (objCust != null)
            {
                IsCalendarOnline = (bool)objCust.SingleOrDefault().isCalendarOnline;
            }

            if (IsCalendarOnline)
            {


                var results = (from sc in _db.ScheduleCalendars
                               join link in _db.ScheduleCalendarLinks on sc.event_id equals link.child_event_id
                               where link.customer_id == ncid && link.estimate_id == neid && link.parent_event_id == parentEventId
                               orderby sc.event_start
                               select new ImproperCalendarLinkEvent
                               {
                                   link_id = (int)link.link_id,
                                   customer_id = (int)link.customer_id,
                                   estimate_id = (int)link.estimate_id,
                                   title = sc.title,
                                   start = sc.event_start.ToString(),
                                   end = sc.event_end.ToString(),
                                   parent_event_id = (int)link.parent_event_id,
                                   child_event_id = (int)link.child_event_id,
                                   dependencyType = (int)link.dependencyType,
                                   offsetDays = (int)link.lag,
                                   IsSuccess = "Ok"
                               });
                var newresults = from item in results.ToList()
                                 select new ImproperCalendarLinkEvent
                                 {
                                     link_id = item.link_id,
                                     customer_id = item.customer_id,
                                     estimate_id = item.estimate_id,
                                     title = item.title,
                                     start = Convert.ToDateTime(item.start).ToString("MM/dd/yyyy hh:mm tt"),
                                     end = Convert.ToDateTime(item.end).ToString("MM/dd/yyyy hh:mm tt"),
                                     parent_event_id = item.parent_event_id,
                                     child_event_id = item.child_event_id,
                                     dependencyType = item.dependencyType,
                                     offsetDays = item.offsetDays,
                                     IsSuccess = item.IsSuccess
                                 };
                objlnkEvent = newresults.ToList();
            }
            else
            {
                var results = (from scTemp in _db.ScheduleCalendarTemps
                               join linktemp in _db.ScheduleCalendarLinkTemps on scTemp.event_id equals linktemp.child_event_id
                               where linktemp.customer_id == ncid && linktemp.estimate_id == neid && linktemp.parent_event_id == parentEventId
                               orderby scTemp.event_start
                               select new ImproperCalendarLinkEvent
                               {
                                   link_id = (int)linktemp.link_id,
                                   customer_id = (int)linktemp.customer_id,
                                   estimate_id = (int)linktemp.estimate_id,
                                   title = scTemp.title,
                                   start = scTemp.event_start.ToString(),
                                   end = scTemp.event_end.ToString(),
                                   parent_event_id = (int)linktemp.parent_event_id,
                                   child_event_id = (int)linktemp.child_event_id,
                                   dependencyType = (int)linktemp.dependencyType,
                                   offsetDays = (int)linktemp.lag,
                                   IsSuccess = "Ok"
                               });
                var newresults = from item in results.ToList()
                                 select new ImproperCalendarLinkEvent
                                 {
                                     link_id = item.link_id,
                                     customer_id = item.customer_id,
                                     estimate_id = item.estimate_id,
                                     title = item.title,
                                     start = Convert.ToDateTime(item.start).ToString("MM/dd/yyyy hh:mm tt"),
                                     end = Convert.ToDateTime(item.end).ToString("MM/dd/yyyy hh:mm tt"),
                                     parent_event_id = item.parent_event_id,
                                     child_event_id = item.child_event_id,
                                     dependencyType = item.dependencyType,
                                     offsetDays = item.offsetDays,
                                     IsSuccess = item.IsSuccess
                                 };
                objlnkEvent = newresults.ToList();
            }

        }
        catch (Exception ex)
        {
            //  objlnkEvent.IsSuccess = ex.Message;
        }

        return objlnkEvent;
    }

    [System.Web.Services.WebMethod]
    public static List<ImproperCalendarLinkEvent> GetParentEventTable(int childEventId, int ncid, int neid)
    {
        List<ImproperCalendarLinkEvent> objlnkEvent = new List<ImproperCalendarLinkEvent>();
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            bool IsCalendarOnline = true;
            var objCust = _db.customers.Where(c => c.customer_id == ncid);

            if (objCust != null)
            {
                IsCalendarOnline = (bool)objCust.SingleOrDefault().isCalendarOnline;
            }

            if (IsCalendarOnline)
            {
                var results = (from sc in _db.ScheduleCalendars
                               join link in _db.ScheduleCalendarLinks on sc.event_id equals link.parent_event_id
                               where link.customer_id == ncid && link.estimate_id == neid && link.child_event_id == childEventId
                               orderby sc.event_start
                               select new ImproperCalendarLinkEvent
                               {
                                   link_id = (int)link.link_id,
                                   customer_id = (int)link.customer_id,
                                   estimate_id = (int)link.estimate_id,
                                   title = sc.title,
                                   start = sc.event_start.ToString(),
                                   end = sc.event_end.ToString(),
                                   parent_event_id = (int)link.parent_event_id,
                                   child_event_id = (int)link.child_event_id,
                                   dependencyType = (int)link.dependencyType,
                                   offsetDays = (int)link.lag,
                                   IsSuccess = "Ok"
                               });
                var newresults = from item in results.ToList()
                                 select new ImproperCalendarLinkEvent
                                 {
                                     link_id = item.link_id,
                                     customer_id = item.customer_id,
                                     estimate_id = item.estimate_id,
                                     title = item.title,
                                     start = Convert.ToDateTime(item.start).ToString("MM/dd/yyyy hh:mm tt"),
                                     end = Convert.ToDateTime(item.end).ToString("MM/dd/yyyy hh:mm tt"),
                                     parent_event_id = item.parent_event_id,
                                     child_event_id = item.child_event_id,
                                     dependencyType = item.dependencyType,
                                     offsetDays = item.offsetDays,
                                     IsSuccess = item.IsSuccess
                                 };
                objlnkEvent = newresults.ToList();
            }
            else
            {
                var results = (from scTemp in _db.ScheduleCalendarTemps
                               join linktemp in _db.ScheduleCalendarLinkTemps on scTemp.event_id equals linktemp.parent_event_id
                               where linktemp.customer_id == ncid && linktemp.estimate_id == neid && linktemp.child_event_id == childEventId
                               orderby scTemp.event_start
                               select new ImproperCalendarLinkEvent
                               {
                                   link_id = (int)linktemp.link_id,
                                   customer_id = (int)linktemp.customer_id,
                                   estimate_id = (int)linktemp.estimate_id,
                                   title = scTemp.title,
                                   start = scTemp.event_start.ToString(),
                                   end = scTemp.event_end.ToString(),
                                   parent_event_id = (int)linktemp.parent_event_id,
                                   child_event_id = (int)linktemp.child_event_id,
                                   dependencyType = (int)linktemp.dependencyType,
                                   offsetDays = (int)linktemp.lag,
                                   IsSuccess = "Ok"
                               });
                var newresults = from item in results.ToList()
                                 select new ImproperCalendarLinkEvent
                                 {
                                     link_id = item.link_id,
                                     customer_id = item.customer_id,
                                     estimate_id = item.estimate_id,
                                     title = item.title,
                                     start = Convert.ToDateTime(item.start).ToString("MM/dd/yyyy hh:mm tt"),
                                     end = Convert.ToDateTime(item.end).ToString("MM/dd/yyyy hh:mm tt"),
                                     parent_event_id = item.parent_event_id,
                                     child_event_id = item.child_event_id,
                                     dependencyType = item.dependencyType,
                                     offsetDays = item.offsetDays,
                                     IsSuccess = item.IsSuccess
                                 };
                objlnkEvent = newresults.ToList();
            }

        }
        catch (Exception ex)
        {
            //  objlnkEvent.IsSuccess = ex.Message;
        }

        return objlnkEvent;
    }

    [System.Web.Services.WebMethod]
    public static bool GetUnassignedCheckboxState()
    {
        bool IsCheckedBox = true;
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            company_profile objComp = _db.company_profiles.SingleOrDefault(c => c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

            System.Web.HttpContext.Current.Session.Add("sUnassignedCheck", (bool)objComp.IsScheduleUnassignedCheck);

            IsCheckedBox = (bool)objComp.IsScheduleUnassignedCheck;


        }
        catch (Exception ex)
        {
            //  objlnkEvent.IsSuccess = ex.Message;
        }

        return IsCheckedBox;
    }

    [System.Web.Services.WebMethod]
    public static csProjectLink SetUnassignedCheckboxState(bool IsCheked)
    {
        csProjectLink objPL = new csProjectLink();
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            company_profile objComp = _db.company_profiles.SingleOrDefault(c => c.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]));

            objComp.IsScheduleUnassignedCheck = IsCheked;

            _db.SubmitChanges();

            System.Web.HttpContext.Current.Session.Add("sUnassignedCheck", IsCheked);

            if (System.Web.HttpContext.Current.Session["CustSelected"] != null)
            {
                objPL.customer_id = Convert.ToInt32(System.Web.HttpContext.Current.Session["CustSelected"]);
                objPL.estimate_id = Convert.ToInt32(System.Web.HttpContext.Current.Session["EstSelected"]);
                objPL.date = "";
            }
            else
            {
                objPL.customer_id = 0;
                objPL.estimate_id = 0;
                objPL.date = "";
            }



            return objPL;
        }
        catch (Exception ex)
        {
            //  objlnkEvent.IsSuccess = ex.Message;
        }

        return objPL;
    }

    //
    [System.Web.Services.WebMethod(true)]
    public static String SetDivisionIdOnCahnge(int divisionId)
    {
        HttpContext.Current.Session.Add("sSetDivisionIdOnCahnge", divisionId);
        return divisionId.ToString();
    }

    public class scEventLink
    {
        public int link_id { get; set; }
        public int dependencyType { get; set; }
        public DateTime event_start { get; set; }
        public DateTime event_end { get; set; }
        public int offsetdays { get; set; }
        public int child_event_id { get; set; }
        public int parent_event_id { get; set; }
        public int customer_id { get; set; }
        public int estimate_id { get; set; }
    }

    public class csProjectLink
    {
        public string date { get; set; }
        public int customer_id { get; set; }
        public int estimate_id { get; set; }
    }

    public class csExternalEvents
    {
        public string cssClassName { get; set; }
        public int serial { get; set; }
        public string section_name { get; set; }
        public int customer_id { get; set; }
        public int estimate_id { get; set; }
        public int client_id { get; set; }

    }

    public static string ConvertToTimestamp(DateTime value)
    {
        // var text = value.ToString("'\"'yyyy-MM-dd'T'HH:mm:ss'\"'", System.Globalization.CultureInfo.InvariantCulture); // 2014-11-10T10:00:00
        var text = value.ToString("yyyy-MM-dd'T'HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture); // 2014-11-10T10:00:00
        long epoch = (value.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        return text;
    }

    protected void rdoconfirm_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, rdoconfirm.ID, rdoconfirm.GetType().Name, "SelectedIndexChanged");
        DataClassesDataContext _db = new DataClassesDataContext();

        customer objCust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerID.Value));


        objCust.CustomerCalendarWeeklyView = Convert.ToInt32(rdoconfirm.SelectedValue);

        _db.SubmitChanges();

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "HideProgress", "HideProgress();", true);
    }

    [System.Web.Services.WebMethod]
    public static string SetImmediateWeekEnds(bool isSelected, int eventId, int ncid, string Start, string End)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        DateTime start = DateTime.ParseExact(Start, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        DateTime end = DateTime.ParseExact(End, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

        List<csWeekends> lweekends = new List<csWeekends>();
        JavaScriptSerializer ser = new JavaScriptSerializer();

        string weeekendList = "";
        string selectedweekends = "";
        bool IsCalendarOnline = true;


        DateTime dtStart = Convert.ToDateTime(start);
        DateTime dtEnd = Convert.ToDateTime(end);

        DateTime startDate = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day);
        DateTime endDate = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day);

        TimeSpan diff = endDate - startDate;
        try
        {

            for (int i = 0; i <= diff.Days; i++)
            {
                DateTime dtCurrentTime = start.AddDays(i);
                //if (dtCurrentTime.DayOfWeek == DayOfWeek.Sunday || dtCurrentTime.DayOfWeek == DayOfWeek.Saturday)

                //{
                //    weeekendList += dtCurrentTime.ToShortDateString() + ',';
                //}
                if (dtCurrentTime.DayOfWeek == DayOfWeek.Sunday)
                {
                    weeekendList += "Sun " + dtCurrentTime.ToShortDateString() + ',';
                }
                else if (dtCurrentTime.DayOfWeek == DayOfWeek.Saturday)
                {
                    weeekendList += "Sat " + dtCurrentTime.ToShortDateString() + ',';
                }


            }
            weeekendList.TrimEnd(',');


            string[] weekendsAry = weeekendList.Split(',').Select(p => p.Trim()).ToArray();
            weekendsAry = weekendsAry.Where(x => !string.IsNullOrEmpty(x)).ToArray();



            var objCust = _db.customers.Where(c => c.customer_id == ncid);

            if (objCust != null)
            {
                IsCalendarOnline = (bool)objCust.SingleOrDefault().isCalendarOnline;
            }

            if (IsCalendarOnline)
            {
                ScheduleCalendar scObj = _db.ScheduleCalendars.SingleOrDefault(c => c.event_id == eventId && c.customer_id == ncid);
                if (scObj != null)
                {
                    selectedweekends = scObj.selectedweekends;
                    //scObj.weekends = weeekendList;
                    //_db.SubmitChanges();
                }
            }
            else
            {
                ScheduleCalendarTemp objSchduleTimeTemp = _db.ScheduleCalendarTemps.SingleOrDefault(c => c.event_id == eventId);
                if (objSchduleTimeTemp != null)
                {
                    selectedweekends = objSchduleTimeTemp.selectedweekends;
                    //objSchduleTimeTemp.weekends = weeekendList;
                    //_db.SubmitChanges();
                }
            }

            if (isSelected && (weekendsAry.Length > 0))
            {
                foreach (var we in weekendsAry)
                {
                    csWeekends weekendss = new csWeekends();
                    weekendss.weekends = we.ToString();
                    if (selectedweekends.Contains(we))
                    {
                        weekendss.Checked = "checked";
                    }
                    weekendss.message = "";
                    lweekends.Add(weekendss);
                }

            }
            return ser.Serialize(lweekends);
        }
        catch (Exception ex)
        {
            lweekends = new List<csWeekends>() {
                new csWeekends() { weekends = "", Checked = "", message = "GetWeekEnds Error:"+ex.Message }
            };
            return ser.Serialize(lweekends);
        }
    }


    [System.Web.Services.WebMethod]
    public static string GetWeekEnds(bool isSelected, int eventId, int ncid)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        List<csWeekends> lweekends = new List<csWeekends>();
        JavaScriptSerializer ser = new JavaScriptSerializer();

        bool IsCalendarOnline = true;

        string weekendstotal = "";
        string selectedweekends = "";
        try
        {
            var objCust = _db.customers.Where(c => c.customer_id == ncid);

            if (objCust.Any())
            {
                IsCalendarOnline = (bool)objCust.SingleOrDefault().isCalendarOnline;
            }


            if (IsCalendarOnline)
            {
                ScheduleCalendar objSchduleTime = _db.ScheduleCalendars.SingleOrDefault(c => c.event_id == eventId);
                if (objSchduleTime != null)
                {
                    weekendstotal = objSchduleTime.weekends;
                    selectedweekends = objSchduleTime.selectedweekends;
                }
            }
            else
            {
                ScheduleCalendarTemp objSchduleTimeTemp = _db.ScheduleCalendarTemps.SingleOrDefault(c => c.event_id == eventId);
                if (objSchduleTimeTemp != null)
                {
                    weekendstotal = objSchduleTimeTemp.weekends;
                    selectedweekends = objSchduleTimeTemp.selectedweekends;
                }
            }


            string[] weekendsAry = weekendstotal.Split(',').Select(p => p.Trim()).ToArray();
            weekendsAry = weekendsAry.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            if (isSelected && (weekendsAry.Length > 0))
            {
                foreach (var we in weekendsAry)
                {
                    csWeekends weekendss = new csWeekends();
                    weekendss.weekends = we.ToString();
                    if (selectedweekends.Contains(we))
                    {
                        weekendss.Checked = "checked";
                    }
                    weekendss.message = "";
                    lweekends.Add(weekendss);
                }

            }

            return ser.Serialize(lweekends);
        }
        catch (Exception ex)
        {
            lweekends = new List<csWeekends>() {
                new csWeekends() { weekends = "", Checked = "", message = "GetWeekEnds Error:"+ex.Message }
            };
            return ser.Serialize(lweekends);
        }




    }

    public class csWeekends
    {
        public string weekends { get; set; }
        public string Checked { get; set; }
        public string message { get; set; }
    }
}
