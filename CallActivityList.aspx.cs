using DataStreams.Csv;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CallActivityList : System.Web.UI.Page
{

    [WebMethod]
    public static string[] GetCompany(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.company.ToLower().StartsWith(prefixText.ToLower())
                    select c.company).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.company.StartsWith(prefixText)
                    select c.company).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.last_name1.ToLower().StartsWith(prefixText.ToLower())
                    select c.last_name1).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.last_name1.StartsWith(prefixText)
                    select c.last_name1).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetFirstName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.first_name1.ToLower().StartsWith(prefixText.ToLower())
                    select c.first_name1).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.first_name1.StartsWith(prefixText)
                    select c.first_name1).Take<String>(count).ToArray();
        }
    }

    [WebMethod]
    public static string[] GetPhone(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.phone.ToLower().StartsWith(prefixText.ToLower())
                    select c.phone).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.phone.StartsWith(prefixText)
                    select c.phone).Take<String>(count).ToArray();
        }
    }
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
                hdnClientId.Value = ((userinfo)Session["oUser"]).client_id.ToString();
            }
            if (Page.User.IsInRole("calllog001") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            // Get Leads
            # region Get Leads

            DataClassesDataContext _db = new DataClassesDataContext();
            List<customer> LeadList = _db.customers.ToList();
            Session.Add("cSearch", LeadList);

            # endregion
            BindLeadSource();
            ddlCallType.SelectedValue = "7";
            GetCustomerCallLog_FollowUp(0);


        }
    }

    private void BindLeadSource()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        var item = from l in _db.lead_sources
                   where l.client_id.ToString().Contains(hdnClientId.Value) && l.is_active == Convert.ToBoolean(1)
                   orderby l.lead_name
                   select l;
        ddlLeadSource.DataSource = item;
        ddlLeadSource.DataTextField = "lead_name";
        ddlLeadSource.DataValueField = "lead_source_id";

        ddlLeadSource.DataBind();
        ddlLeadSource.Items.Insert(0, "All");
        ddlLeadSource.SelectedIndex = 0;
    }

    protected void GetCustomerCallLog_FollowUp(int nPageNo)
    {
        if (Session["CallCustId"] != null)
        {
            int nCustomerId = Convert.ToInt32(Session["CallCustId"]);
            hdnLeadId.Value = nCustomerId.ToString();
        }
        DataClassesDataContext _db = new DataClassesDataContext();
        if (Convert.ToInt32(hdnLeadId.Value) > 0)
        {
            int nPage = 0;
            int nLeadSourceId = 0;
            int nCallType = 0;
            if (Session["sPage"] != null)
            {
                nPage = Convert.ToInt32(Session["sPage"]);
            }
            if (Session["sLeadSource"] != null)
            {
                nLeadSourceId = Convert.ToInt32(Session["sLeadSource"]);
            }
            if (Session["sCallType"] != null)
            {
                nCallType = Convert.ToInt32(Session["sCallType"]);
            }
            if (nLeadSourceId > 0)
                ddlLeadSource.SelectedValue = nLeadSourceId.ToString();
            else
                ddlLeadSource.SelectedIndex = -1;

            if (nCallType > 0)
                ddlCallType.SelectedValue = nCallType.ToString();
            else
                ddlCallType.SelectedValue = "7";

            int nCustomerId = Convert.ToInt32(hdnLeadId.Value);
            if (Session["sSearch"] != null)
            {
                txtSearch.Text = Session["sSearch"].ToString();
            }
           // txtSearch.Text = "";

            GetCustomerCallLog(nPage);
            hdnLeadId.Value = "0";
            Session.Remove("CallCustId");
          
        }
        else
        {
            Session.Remove("sPage");
            Session.Remove("sLeadSource");
            Session.Remove("sCallType");

            grdCustCallList.PageIndex = nPageNo;
            string strQ = " SELECT customers.customer_id, first_name1, last_name1, phone, mobile, ISNULL(appointment_date,'1900-01-01') AS appointment_date, customers.lead_source_id,lead_source.lead_name,company, lead_status_id ,Isnull (CallLogID,0) as CallLogID, CallDate, CallHour, CallMinutes, CallDuration, CallAMPM, DurationHour, DurationMinutes, t1.Description, CreatedByUser, ISNULL(CreateDate,'1900-01-01') AS  CreateDate,ISNULL(CallDateTime,'1900-01-01') AS CallDateTime, CallSubject, Isnull(CallTypeId,0) as CallTypeId, ISNULL(IsFollowUp,0) AS IsFollowUp, ISNULL(IsDoNotCall,0) AS IsDoNotCall, FollowDate, FollowHour, FollowMinutes, FollowAMPM, ISNULL(AppointmentDateTime,'1900-01-01') AS AppointmentDateTime,CASE WHEN YEAR(ISNULL(FollowDateTime,'2900-01-01')) = 1900  THEN '2900-01-01'  ELSE  ISNULL(FollowDateTime,'2900-01-01')  END  AS FollowDateTime " +
                    " FROM customers " +
                    " LEFT OUTER JOIN " +
                    " (SELECT     CallLogID, CustomerCallLog.customer_id, CallDate, CallHour, CallMinutes, CallDuration, CallAMPM, DurationHour, DurationMinutes, Description, CreatedByUser, CreateDate, CallDateTime, CallSubject, CallTypeId, IsFollowUp, FollowDate, FollowHour, FollowMinutes, FollowAMPM, AppointmentDateTime, FollowDateTime,IsDoNotCall FROM CustomerCallLog " +
                    " INNER JOIN " +
                    " ( SELECT    customer_id, Max(CallLogID) as ID FROM CustomerCallLog GROUP BY  customer_id ) AS x ON CustomerCallLog.customer_id = x.customer_id AND CustomerCallLog.CallLogID = x.ID) AS t1 ON t1.customer_id = customers.customer_id " +
                    "  INNER JOIN lead_source ON lead_source.lead_source_id = customers.lead_source_id where  IsFollowUp = 1  and IsDoNotCall = 0 Order By FollowDateTime asc ";

            IEnumerable<csCustomerCall> mList = _db.ExecuteQuery<csCustomerCall>(strQ, string.Empty).ToList();
            DataTable dt = csCommonUtility.LINQToDataTable(mList);
            Session.Add("sCustCallLog", dt);

            grdCustCallList.DataSource = mList;
            grdCustCallList.DataKeyNames = new string[] { "CallLogID", "customer_id", "lead_source_id", "CallTypeId", "IsFollowUp", "first_name1", "last_name1", "phone", "AppointmentDateTime", "FollowDateTime", "IsDoNotCall" };
            grdCustCallList.DataBind();
            lblCurrentPageNo.Text = Convert.ToString(nPageNo + 1);
            if (nPageNo == 0)
            {
                btnPrevious.Enabled = false;
                btnPrevious0.Enabled = false;
            }
            else
            {
                btnPrevious.Enabled = true;
                btnPrevious0.Enabled = true;
            }

            if (grdCustCallList.PageCount == nPageNo + 1)
            {
                btnNext.Enabled = false;
                btnNext0.Enabled = false;
            }
            else
            {
                btnNext.Enabled = true;
                btnNext0.Enabled = true;
            }

            if (grdCustCallList.Rows.Count == 0)
            {
                GetCustomerCallLog(0);
            }
        }



    }


    protected void GetCustomerCallLog(int nPageNo)
    {
        Session.Remove("LeadId");
        DataClassesDataContext _db = new DataClassesDataContext();

        grdCustCallList.PageIndex = nPageNo;
        string strCondition = "";

        if (txtSearch.Text.Trim() != "")
        {
            Session.Remove("sPage");
            Session.Remove("sLeadSource");
            Session.Remove("sCallType");

            string str = txtSearch.Text.Trim();
            if (ddlSearchBy.SelectedValue == "1")
            {
                strCondition = " customers.first_name1 LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "2")
            {
                strCondition = "  customers.last_name1 LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "3")
            {

                strCondition = "  customers.company LIKE '%" + str + "%'";
            }
            else if (ddlSearchBy.SelectedValue == "4")
            {
                strCondition = "  customers.phone LIKE '%" + str + "%'";
            }
        }

        if (Convert.ToInt32(ddlCallType.SelectedValue) == 7)
        {
            if (strCondition.Length > 0)
            {
                strCondition += " AND customers.status_id NOT IN(4,5)  ";
            }
            else
            {
                strCondition += " customers.status_id NOT IN(4,5)  ";
            }

        }
        else if (Convert.ToInt32(ddlCallType.SelectedValue) == 4)
        {
            if (strCondition.Length > 0)
            {
                strCondition += " AND IsFollowUp = 1  ";
            }
            else
            {
                strCondition += " IsFollowUp = 1  ";
            }
        }
        else if (Convert.ToInt32(ddlCallType.SelectedValue) == 5)
        {
            if (strCondition.Length > 0)
            {
                strCondition += " AND IsDoNotCall = 1    ";
            }
            else
            {
                strCondition += " IsDoNotCall = 1  ";
            }

        }
        else
        {
            if (strCondition.Length > 0)
            {
                strCondition += " AND CallTypeId = " + Convert.ToInt32(ddlCallType.SelectedValue);
            }
            else
            {
                strCondition += " CallTypeId = " + Convert.ToInt32(ddlCallType.SelectedValue);
            }

        }

        if (ddlLeadSource.SelectedItem.Text != "All")
        {
            if (strCondition.Length > 0)
            {
                strCondition += " AND customers.lead_source_id =" + Convert.ToInt32(ddlLeadSource.SelectedValue);
            }
            else
            {
                strCondition += " customers.lead_source_id =" + Convert.ToInt32(ddlLeadSource.SelectedValue);
            }

        }

        if (strCondition.Length > 0)
        {
            if (Convert.ToInt32(ddlCallType.SelectedValue) != 5)
            {
                strCondition = "Where customers.customer_id NOT IN(Select cc.customer_id from  CustomerCallLog cc INNER JOIN (SELECT customer_id, Max(CallLogID) as ID FROM CustomerCallLog GROUP BY  customer_id) as y on cc.customer_id =  y.customer_id and cc.CallLogID = y.ID WHERE cc.IsDoNotCall = 1 ) AND  " + strCondition;
            }
            else
            {
                strCondition = "Where  " + strCondition;
            }
        }
        else
        {
            strCondition = "Where customers.customer_id NOT IN(Select cc.customer_id from  CustomerCallLog cc INNER JOIN (SELECT customer_id, Max(CallLogID) as ID FROM CustomerCallLog GROUP BY  customer_id) as y on cc.customer_id =  y.customer_id and cc.CallLogID = y.ID WHERE cc.IsDoNotCall = 1 )";
        }

        string strQ = string.Empty;
        strQ = " SELECT customers.customer_id, first_name1, last_name1, phone, mobile, ISNULL(appointment_date,'1900-01-01') AS appointment_date, customers.lead_source_id,lead_source.lead_name,company, lead_status_id ,Isnull (CallLogID,0) as CallLogID, ISNULL(CallDate,'') as CallDate,  ISNULL(CallHour,'') as CallHour, ISNULL(CallMinutes, '') as CallMinutes, ISNULL(CallDuration, '') as CallDuration, ISNULL(CallAMPM, '') as CallAMPM, ISNULL(DurationHour, '') as DurationHour, ISNULL(DurationMinutes,'') as DurationMinutes, ISNULL(t1.Description, '') as Description, ISNULL(CreatedByUser,'') as CreatedByUser, ISNULL(CreateDate,'1900-01-01') AS  CreateDate,ISNULL(CallDateTime,'1900-01-01') AS CallDateTime, ISNULL(CallSubject,'') as CallSubject, Isnull(CallTypeId,0) as CallTypeId, ISNULL(IsFollowUp,0) AS IsFollowUp, ISNULL(IsDoNotCall,0) AS IsDoNotCall, ISNULL(FollowDate,'') as FollowDate, ISNULL(FollowHour,'') as FollowHour, ISNULL( FollowMinutes, '') as FollowMinutes, ISNULL(FollowAMPM,'') as FollowAMPM, ISNULL(AppointmentDateTime,'1900-01-01') AS AppointmentDateTime,CASE WHEN YEAR(ISNULL(FollowDateTime,'2900-01-01')) = 1900  THEN '2900-01-01'  ELSE  ISNULL(FollowDateTime,'2900-01-01')  END  AS FollowDateTime " +
                 " FROM customers " +
                 " LEFT OUTER JOIN " +
                 " (SELECT     CallLogID, CustomerCallLog.customer_id, Isnull(CallDate,'') as CallDate, Isnull (CallHour,'') as CallHour, Isnull (CallMinutes, '') as CallMinutes, Isnull (CallDuration, '') as CallDuration, Isnull (CallAMPM,'') as CallAMPM, Isnull (DurationHour, '') as DurationHour, Isnull (DurationMinutes, '') as DurationMinutes, Isnull (Description, '') as Description, Isnull (CreatedByUser,'') as CreatedByUser, CreateDate, CallDateTime, CallSubject, CallTypeId, IsFollowUp, FollowDate, FollowHour, FollowMinutes, FollowAMPM, AppointmentDateTime, FollowDateTime,IsDoNotCall FROM CustomerCallLog " +
                 " INNER JOIN " +
                 " ( SELECT    customer_id, Max(CallLogID) as ID FROM CustomerCallLog GROUP BY  customer_id ) AS x ON CustomerCallLog.customer_id = x.customer_id AND CustomerCallLog.CallLogID = x.ID) AS t1 ON t1.customer_id = customers.customer_id " +
                 "  INNER JOIN lead_source ON lead_source.lead_source_id = customers.lead_source_id " + strCondition + " Order By FollowDateTime asc, first_name1 asc ";

        IEnumerable<csCustomerCall> mList = _db.ExecuteQuery<csCustomerCall>(strQ, string.Empty).ToList();
        DataTable dt = csCommonUtility.LINQToDataTable(mList);
        Session.Add("sCustCallLog", dt);

        grdCustCallList.DataSource = mList;
        grdCustCallList.DataKeyNames = new string[] { "CallLogID", "customer_id", "lead_source_id", "CallTypeId", "IsFollowUp", "first_name1", "last_name1", "phone", "AppointmentDateTime", "FollowDateTime", "IsDoNotCall" };
        grdCustCallList.DataBind();
        lblCurrentPageNo.Text = Convert.ToString(nPageNo + 1);
        if (nPageNo == 0)
        {
            btnPrevious.Enabled = false;
            btnPrevious0.Enabled = false;
        }
        else
        {
            btnPrevious.Enabled = true;
            btnPrevious0.Enabled = true;
        }

        if (grdCustCallList.PageCount == nPageNo + 1)
        {
            btnNext.Enabled = false;
            btnNext0.Enabled = false;
        }
        else
        {
            btnNext.Enabled = true;
            btnNext0.Enabled = true;
        }
    }



    protected void grdCustCallList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nCallId = Convert.ToInt32(grdCustCallList.DataKeys[e.Row.RowIndex].Values[0]);
            int nCustId = Convert.ToInt32(grdCustCallList.DataKeys[e.Row.RowIndex].Values[1]);
            int nlead_source_Id = Convert.ToInt32(grdCustCallList.DataKeys[e.Row.RowIndex].Values[2]);
            int nCallTypeId = Convert.ToInt32(grdCustCallList.DataKeys[e.Row.RowIndex].Values[3]);
            bool nIsFollowUp = Convert.ToBoolean(grdCustCallList.DataKeys[e.Row.RowIndex].Values[4]);
            string strFirst = grdCustCallList.DataKeys[e.Row.RowIndex].Values[5].ToString();
            string strLast = grdCustCallList.DataKeys[e.Row.RowIndex].Values[6].ToString();
            string strphone = grdCustCallList.DataKeys[e.Row.RowIndex].Values[7].ToString();
            DateTime strAppointmentDateTime = Convert.ToDateTime(grdCustCallList.DataKeys[e.Row.RowIndex].Values[8]);
            DateTime strFollowDateTime = Convert.ToDateTime(grdCustCallList.DataKeys[e.Row.RowIndex].Values[9]);

            bool bIsDoNotCall = Convert.ToBoolean(grdCustCallList.DataKeys[e.Row.RowIndex].Values[10]);

            HyperLink hyp_Cust = (HyperLink)e.Row.FindControl("hyp_Cust");
            hyp_Cust.Text = strFirst + " " + strLast;
            hyp_Cust.NavigateUrl = "CallLogInfo.aspx?cid=" + nCustId+"&TypeId=1" ;

            //HyperLink hyp_company = (HyperLink)e.Row.FindControl("hyp_company");
            //hyp_company.NavigateUrl = "CallLogInfo.aspx?cid=" + nCustId;

            Label lblPhone = (Label)e.Row.FindControl("lblPhone");
            lblPhone.Text = strphone;
            lblPhone.Attributes.CssStyle.Add("padding", "5px 0 5px 0");

            Label lblCallDescriptionG = (Label)e.Row.FindControl("lblCallDescriptionG");
            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
            string str = lblCallDescriptionG.Text.Replace("&nbsp;", "");

            Label lblCallType = (Label)e.Row.FindControl("lblCallType");
            if (nCallTypeId == 1)
                lblCallType.Text = "Called";
            else if (nCallTypeId == 2)
                lblCallType.Text = "Pitched";
            else if (nCallTypeId == 6)
                lblCallType.Text = "Emailed";
            else if (nCallTypeId == 3)
            {

                DataClassesDataContext _db = new DataClassesDataContext();
                ScheduleCalendar objsc = new ScheduleCalendar();
                string apptDate = Convert.ToDateTime(strAppointmentDateTime).ToShortDateString();
                string startTime = Convert.ToDateTime(strAppointmentDateTime).ToShortTimeString();
                string endTime = "";
                if (startTime == "12:00 AM")
                {
                    startTime = "6:00 AM";
                    endTime = "7:00 AM";
                }
                else
                {
                    if (_db.ScheduleCalendars.Any(sc => sc.customer_id == nCustId && sc.type_id == 2 && sc.estimate_id == nCallId))
                    {
                        objsc = _db.ScheduleCalendars.Single(sc => sc.customer_id == nCustId && sc.type_id == 2 && sc.estimate_id == nCallId);
                        endTime = Convert.ToDateTime(objsc.event_end).ToShortTimeString();
                    }
                    else
                    {
                        endTime = startTime;
                    }
                }
                lblCallType.Text = "Booked Appt:" + apptDate + " </br>" + startTime + " to " + endTime;
            }
            else
            {
                lblCallType.Text = "";
                //lblCallType.Text = "Booked" + " </br>" + strAppointmentDateTime.ToString("MM/dd/yyyy  h:mm:ss tt");
            }
            Label lblFollowup = (Label)e.Row.FindControl("lblFollowup");
            string sFollowDate = string.Empty;
            if (nIsFollowUp)
            {
                sFollowDate = Convert.ToDateTime(strFollowDateTime).ToShortDateString();
                string sFollowTime = Convert.ToDateTime(strFollowDateTime).ToShortTimeString();
                //lblFollowup.Text = "Followup" + " </br>" + strFollowDateTime.ToString("MM/dd/yyyy  h:mm:ss tt");
                lblFollowup.Text = sFollowDate + " </br>" + sFollowTime;
            }
            else
                lblFollowup.Text = "";

            if (bIsDoNotCall)
            {
                lblCallType.Text = "Do Not Call";
            }


            Label lblCallStartDateTime = (Label)e.Row.FindControl("lblCallStartDateTime");

            DateTime dtCallDate = Convert.ToDateTime(lblCallStartDateTime.Text);
            string sCallDate = Convert.ToDateTime(dtCallDate).ToShortDateString();
            if (dtCallDate.Year == 1900)
            {
                lblCallStartDateTime.Text = "";
            }
            else
            {

                string SCallTime = Convert.ToDateTime(dtCallDate).ToShortTimeString();
                lblCallStartDateTime.Text = sCallDate + " </br>" + SCallTime;


            }
            string strTodayDate = DateTime.Now.ToShortDateString();
            if (strTodayDate == sCallDate)
            {
                if (nIsFollowUp)
                {
                    if (strTodayDate != sFollowDate)
                    {
                        e.Row.Attributes.CssStyle.Add("text-decoration", "line-through ; color: blue ;");
                        hyp_Cust.Attributes.CssStyle.Add("text-decoration", "line-through ; color: blue ;");
                        lblPhone.Attributes.CssStyle.Add("text-decoration", "line-through ; color: blue ;");
                        lblCallType.Attributes.CssStyle.Add("text-decoration", "line-through ; color: blue ;");
                        lblFollowup.Attributes.CssStyle.Add("text-decoration", "line-through ; color: blue ;");
                        lblCallStartDateTime.Attributes.CssStyle.Add("text-decoration", "line-through ; color: blue ;");
                    }

                }
                else
                {
                    e.Row.Attributes.CssStyle.Add("text-decoration", "line-through ; color: blue ;");
                    hyp_Cust.Attributes.CssStyle.Add("text-decoration", "line-through ; color: blue ;");
                    lblPhone.Attributes.CssStyle.Add("text-decoration", "line-through ; color: blue ;");
                    lblCallType.Attributes.CssStyle.Add("text-decoration", "line-through ; color: blue ;");
                    lblFollowup.Attributes.CssStyle.Add("text-decoration", "line-through ; color: blue ;");
                    lblCallStartDateTime.Attributes.CssStyle.Add("text-decoration", "line-through ; color: blue ;");
                }
            }
            if (sFollowDate.Length > 0)
            {
                if (Convert.ToDateTime(strTodayDate) >= Convert.ToDateTime(sFollowDate))
                {
                    e.Row.Attributes.CssStyle.Add("color", "red"); ;
                    hyp_Cust.Attributes.CssStyle.Add("color", "red"); ;
                    lblPhone.Attributes.CssStyle.Add("color", "red"); ;
                    lblCallType.Attributes.CssStyle.Add("color", "red"); ;
                    lblFollowup.Attributes.CssStyle.Add("color", "red"); ;
                    lblCallStartDateTime.Attributes.CssStyle.Add("color", "red"); ;
                }
            }
            if (str != "" && str.Length > 90)
            {
                lblCallDescriptionG.Text = str.Substring(0, 90) + " ...";
                lblCallDescriptionG.ToolTip = str;
                lnkOpen.Visible = true;
            }
            else
            {
                lblCallDescriptionG.Text = str;
                lnkOpen.Visible = false;
            }

        }
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        Session.Add("sSearch", txtSearch.Text);
        Session.Remove("LeadId");
        GetCustomerCallLog(0);
    }

    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
       
        Session.Remove("LeadId");
        GetCustomerCallLog(0);
    }

    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        
        Session.Remove("sPage");
        Session.Remove("sLeadSource");
        Session.Remove("sCallType");
        Session.Remove("sSearch");
        txtSearch.Text = "";
        ddlCallType.SelectedValue = "7";
        ddlLeadSource.SelectedIndex = -1;
        GetCustomerCallLog(0);

    }
    

    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "SelectedIndexChanged"); 
        txtSearch.Text = "";
        wtmFileNumber.WatermarkText = "Search by " + ddlSearchBy.SelectedItem.Text;
        if (ddlSearchBy.SelectedValue == "2")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetLastName";
        }
        else if (ddlSearchBy.SelectedValue == "1")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetFirstName";
        }
        else if (ddlSearchBy.SelectedValue == "3")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetCompany";
        }
        else if (ddlSearchBy.SelectedValue == "4")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetPhone";
        }

        GetCustomerCallLog(0);
    }
    protected void btnSalesCalendar_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("schedulecalendar.aspx?TypeID=2");
    }

    protected void grdCustCallList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCustCallList.ID, grdCustCallList.GetType().Name, "PageIndexChanging"); 
        Session.Add("sPage", e.NewPageIndex.ToString());
        GetCustomerCallLog(e.NewPageIndex);
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnNext.ID, btnNext.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        Session.Add("sPage", nCurrentPage.ToString());
        GetCustomerCallLog(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        Session.Add("sPage", (nCurrentPage - 2).ToString());
        GetCustomerCallLog(nCurrentPage - 2);
    }

    protected void ddlCallType_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlCallType.ID, ddlCallType.GetType().Name, "SelectedIndexChanged"); 
        if (ddlCallType.Text != "All")
            Session.Add("sCallType", ddlCallType.SelectedValue);
        GetCustomerCallLog(0);

    }
    protected void ddlLeadSource_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlLeadSource.ID, ddlLeadSource.GetType().Name, "SelectedIndexChanged"); 
        if (ddlLeadSource.Text != "All")
            Session.Add("sLeadSource", ddlLeadSource.SelectedValue);

        GetCustomerCallLog(0);
    }

    protected void grdCustCallList_Sorting(object sender, GridViewSortEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCustCallList.ID, grdCustCallList.GetType().Name, "Sorting"); 
        int nPageNo = 0;
        DataTable dtCallList = (DataTable)Session["sCustCallLog"];
        if (hdnOrder.Value == "ASC")
            hdnOrder.Value = "DESC";
        else
            hdnOrder.Value = "ASC";

        string strShort = e.SortExpression + " " + hdnOrder.Value;
        DataView dv = dtCallList.DefaultView;
        dv.Sort = strShort;
        Session["sCustCallLog"] = dv.ToTable();

        dtCallList = (DataTable)Session["sCustCallLog"];
        grdCustCallList.DataSource = dtCallList;
        grdCustCallList.DataKeyNames = new string[] { "CallLogID", "customer_id", "lead_source_id", "CallTypeId", "IsFollowUp", "first_name1", "last_name1", "phone", "AppointmentDateTime", "FollowDateTime", "IsDoNotCall" };
        grdCustCallList.DataBind();
        lblCurrentPageNo.Text = Convert.ToString(nPageNo + 1);
        if (nPageNo == 0)
        {
            btnPrevious.Enabled = false;
            btnPrevious0.Enabled = false;
        }
        else
        {
            btnPrevious.Enabled = true;
            btnPrevious0.Enabled = true;
        }

        if (grdCustCallList.PageCount == nPageNo + 1)
        {
            btnNext.Enabled = false;
            btnNext0.Enabled = false;
        }
        else
        {
            btnNext.Enabled = true;
            btnNext0.Enabled = true;
        }

    }
    protected void btnExpCustList_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnExpCustList.ID, btnExpCustList.GetType().Name, "Click"); 
        DataTable tmpTable = LoadTmpDataTable();
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable tblCustList = (DataTable)Session["sCustCallLog"];
        foreach (DataRow dr in tblCustList.Rows)
        {
            string strCallType = string.Empty;
            string strFollowup = string.Empty;
            string strCallDateTime = string.Empty;
            int nCallTypeId = Convert.ToInt32(dr["CallTypeId"]);
            int nCallLogID = Convert.ToInt32(dr["CallLogID"]);
            DateTime strAppointmentDateTime = Convert.ToDateTime(dr["AppointmentDateTime"]);
            DateTime strFollowDateTime = Convert.ToDateTime(dr["FollowDateTime"]);

            int nCustId = Convert.ToInt32(dr["customer_id"]);


            if (nCallTypeId == 1)
                strCallType = "Called";
            else if (nCallTypeId == 2)
                strCallType = "Pitched";
            else if (nCallTypeId == 3)
            {

                ScheduleCalendar objsc = new ScheduleCalendar();
                string apptDate = Convert.ToDateTime(strAppointmentDateTime).ToShortDateString();
                string startTime = Convert.ToDateTime(strAppointmentDateTime).ToShortTimeString();
                string endTime = "";
                if (startTime == "12:00 AM")
                {
                    startTime = "6:00 AM";
                    endTime = "7:00 AM";
                }
                else
                {
                    if (_db.ScheduleCalendars.Any(sc => sc.customer_id == nCustId && sc.type_id == 2 && sc.estimate_id == nCallLogID))
                    {
                        objsc = _db.ScheduleCalendars.Single(sc => sc.customer_id == nCustId && sc.type_id == 2 && sc.estimate_id == nCallLogID);
                        endTime = Convert.ToDateTime(objsc.event_end).ToShortTimeString();
                    }
                    else
                    {
                        endTime = startTime;
                    }
                }
                strCallType = "Booked Appt:" + apptDate + "  " + startTime + " to " + endTime;
            }
            else
            {
                strCallType = "";
            }

            string sFollowDate = string.Empty;
            if (Convert.ToBoolean(dr["IsFollowUp"]))
            {
                sFollowDate = Convert.ToDateTime(strFollowDateTime).ToShortDateString();
                string sFollowTime = Convert.ToDateTime(strFollowDateTime).ToShortTimeString();

                strFollowup = sFollowDate + " " + sFollowTime;
            }
            else
                strFollowup = "";




            DateTime dtCallDate = Convert.ToDateTime(dr["CallDateTime"]);
            string sCallDate = Convert.ToDateTime(dtCallDate).ToShortDateString();
            if (dtCallDate.Year == 1900)
            {
                strCallDateTime = "";
            }
            else
            {

                string SCallTime = Convert.ToDateTime(dtCallDate).ToShortTimeString();
                strCallDateTime = sCallDate + " " + SCallTime;


            }

            int nLeadSourceId = Convert.ToInt32(dr["lead_source_id"]);
            lead_source ls = new lead_source();
            ls = _db.lead_sources.Single(s => s.lead_source_id == nLeadSourceId);
            DataRow drNew = tmpTable.NewRow();

            //drNew["Company"] = dr["company"];
            drNew["First Name"] = dr["first_name1"];
            drNew["Last Name"] = dr["last_name1"];
            drNew["Phone"] = dr["phone"];
            drNew["Notes"] = dr["Description"];
            drNew["Call Date"] = strCallDateTime;
            drNew["Action"] = strCallType;
            drNew["Followup"] = strFollowup;
            // drNew["Address"] = dr["address"].ToString() + ' ' + dr["city"] + ',' + ' ' + dr["state"] + ' ' + dr["zip_code"];
            drNew["Lead Source"] = ls.lead_name.ToString();
            tmpTable.Rows.Add(drNew);
        }

        Response.Clear();
        Response.ClearHeaders();

        using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
        {
            writer.WriteAll(tmpTable, true);
        }
        Response.ContentType = "application/vnd.ms-excel";
        Response.AddHeader("Content-Disposition", "attachment; filename=ActivityLog.csv");
        Response.End();
    }
    private DataTable LoadTmpDataTable()
    {
        DataTable table = new DataTable();

        //table.Columns.Add("Company", typeof(string));
        table.Columns.Add("First Name", typeof(string));
        table.Columns.Add("Last Name", typeof(string));
        table.Columns.Add("Phone", typeof(string));
        //  table.Columns.Add("Email", typeof(string));
        table.Columns.Add("Notes", typeof(string));
        table.Columns.Add("Call Date", typeof(string));
        table.Columns.Add("Action", typeof(string));
        table.Columns.Add("Followup", typeof(string));
        // table.Columns.Add("Address", typeof(string));
        table.Columns.Add("Lead Source", typeof(string));

        return table;
    }
    protected void lnkOpen_Click(object sender, EventArgs e)
    {
        
        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblCallDescriptionG = gRow.Cells[2].Controls[0].FindControl("lblCallDescriptionG") as Label;
        Label lblCallDescriptionG_r = gRow.Cells[2].Controls[1].FindControl("lblCallDescriptionG_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkOpen") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblCallDescriptionG.Visible = false;
            lblCallDescriptionG_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblCallDescriptionG.Visible = true;
            lblCallDescriptionG_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }
}