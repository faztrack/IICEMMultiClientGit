using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SchedulerConflictTest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
        if (Session["oUser"] == null)
        {
            Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
        }

        userinfo objuser = (userinfo)Session["oUser"];

        if (objuser.username.ToLower() != "faztrack")
        {
            Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
        }
    }

    protected void btnCheck_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnCheck.ID, btnCheck.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();


        userinfo objuser = (userinfo)Session["oUser"];

        if (objuser.username.ToLower() != "faztrack")
        {
            Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
        }

        string strCondition = "";
        string strConditionLink = "";
        int nCustID = 0;
        int nEstID = 0;

        string strCustId = "";
        string strEstId = "";

        try
        {
            if (txtCustomerID.Text.Trim().Length > 0)
                nCustID = Convert.ToInt32(txtCustomerID.Text.Trim());

            if (txtEstimateID.Text.Trim().Length > 0)
                nEstID = Convert.ToInt32(txtEstimateID.Text.Trim());


            if (nCustID > 0)
            {
                strCondition = " AND customer_id = " + nCustID;
                strCustId = ",customer_id";
            }

            if (nEstID > 0)
            {
                strCondition = " AND customer_id = " + nCustID + " AND estimate_id =" + nEstID;
                strEstId = ",estimate_id";
            }

            if (nCustID > 0)
            {
                strConditionLink = " WHERE customer_id = " + nCustID;
            }

            if (nEstID > 0)
            {
                strConditionLink = " WHERE customer_id = " + nCustID + " AND estimate_id =" + nEstID;
            }

           

            ////-- Event Duplicate Check with event id
            string sql = "SELECT event_id,COUNT(*) AS count " + strCustId + " " + strEstId + " " +
                " FROM ScheduleCalendar " +
                " WHERE type_id <> 5 " + strCondition + " " +
                " GROUP BY event_id " + strCustId + "" + strEstId + "" +
                " HAVING COUNT(*) > 1";

            lblDuplicateCheck1.Text = "<b>-- Event Duplicate Check with event id</b><br/><br/>" + sql;

            IEnumerable<csScheduleCalendarCheck> list = _db.ExecuteQuery<csScheduleCalendarCheck>(sql, string.Empty);

            grdDuplicateCheck1.DataSource = list;
            grdDuplicateCheck1.DataBind();

            ////-- Event Duplicate Check with title and customer_id
            sql = "SELECT title, customer_id,COUNT(*) AS count  " + strEstId + " " +
            " FROM ScheduleCalendar  " +
            " WHERE type_id <> 5 " + strCondition + " " +
            " GROUP BY title, customer_id " + strEstId + " " +
            " HAVING  COUNT(*) > 1";

            lblDuplicateCheck2.Text = "<b>-- Event Duplicate Check with title and customer_id<b><br/><br/>" + sql;

            list = _db.ExecuteQuery<csScheduleCalendarCheck>(sql, string.Empty);
            grdDuplicateCheck2.DataSource = list;
            grdDuplicateCheck2.DataBind();

            ////-- Event Duplicate Check with event id and customer_id
            sql = "SELECT event_id, customer_id,COUNT(*) AS count " + strEstId + " " +
            " FROM ScheduleCalendar " +
           " WHERE type_id <> 5 " + strCondition + " " +
            " GROUP BY event_id, customer_id " + strEstId + "" +
            " HAVING COUNT(*) > 1";

            lblDuplicateCheck3.Text = "<b>--Event Duplicate Check with event id and customer_id<b><br/><br/>" + sql;

            list = _db.ExecuteQuery<csScheduleCalendarCheck>(sql, string.Empty);
            grdDuplicateCheck3.DataSource = list;
            grdDuplicateCheck3.DataBind();

            ////  -- Link Duplicate Check 
            sql = "SELECT parent_event_id,child_event_id, COUNT(*) AS count " + strCustId + " " + strEstId + " " +
            " FROM ScheduleCalendarLink " +
            " " + strConditionLink + " " +
            " GROUP BY parent_event_id,child_event_id " + strCustId + " " + strEstId + " " +
            " HAVING COUNT(*) > 1";

            lblDuplicateCheck4.Text = "<b>--Link Duplicate Check <b><br/><br/>" + sql;

            list = _db.ExecuteQuery<csScheduleCalendarCheck>(sql, string.Empty);
            grdDuplicateCheck4.DataSource = list;
            grdDuplicateCheck4.DataBind();

            ////-- Child Duplicate Check 
            sql = "SELECT child_event_id, customer_id, COUNT(*) AS count " + strEstId + " " +
            " FROM ScheduleCalendarLink " +
           " " + strConditionLink + " " +
            " GROUP BY child_event_id, customer_id " + strEstId + " " +
            " HAVING COUNT(*) > 1";

            lblDuplicateCheck5.Text = "<b>--Child Duplicate Check <b><br/><br/>" + sql;

            list = _db.ExecuteQuery<csScheduleCalendarCheck>(sql, string.Empty);
            grdDuplicateCheck5.DataSource = list;
            grdDuplicateCheck5.DataBind();

            ////-- event link with thyself Check
            sql = "SELECT parent_event_id,child_event_id,customer_id,estimate_id FROM  ScheduleCalendarLink  " +
            " WHERE parent_event_id = child_event_id "+ strCondition;

            lblDuplicateCheck6.Text = "<b>--event link with thyself Check <b><br/><br/>" + sql;

            list = _db.ExecuteQuery<csScheduleCalendarCheck>(sql, string.Empty);
            grdDuplicateCheck6.DataSource = list;
            grdDuplicateCheck6.DataBind();

            ////-- Event Duplicate Check with event id 
            sql = "SELECT event_id,COUNT(*) AS count " + strCustId + " " + strEstId + " " +
            " FROM ScheduleCalendarTemp " +
            " WHERE type_id <> 5 " + strCondition + " " +
            " GROUP BY event_id " + strCustId + " " + strEstId + " " +
            " HAVING COUNT(*) > 1";

            lblDuplicateCheckTemp1.Text = "<b>--Event Duplicate Check with event id <b><br/><br/>" + sql;

            list = _db.ExecuteQuery<csScheduleCalendarCheck>(sql, string.Empty);
            grdDuplicateCheckTemp1.DataSource = list;
            grdDuplicateCheckTemp1.DataBind();

            //// -- Event Duplicate Check with title and Customer id 
            sql = "SELECT title, customer_id,COUNT(*) AS count " + strEstId + " " +
            " FROM ScheduleCalendarTemp  " +
            " WHERE type_id <> 5 " + strCondition + " " +
            " GROUP BY title, customer_id " + strEstId + " " +
            " HAVING COUNT(*) > 1";

            lblDuplicateCheckTemp2.Text = "<b>--Event Duplicate Check with title and Customer id <b><br/><br/>" + sql;

            list = _db.ExecuteQuery<csScheduleCalendarCheck>(sql, string.Empty);
            grdDuplicateCheckTemp2.DataSource = list;
            grdDuplicateCheckTemp2.DataBind();

            ////-- Event Duplicate Check with event id and Customer id 
            sql = "SELECT event_id, customer_id,COUNT(*) AS count " + strEstId + " " +
            " FROM ScheduleCalendarTemp " +
            " WHERE type_id <> 5 " + strCondition + " " +
            " GROUP BY event_id, customer_id " + strEstId + " " +
            " HAVING COUNT(*) > 1";

            lblDuplicateCheckTemp3.Text = "<b>--Event Duplicate Check with event id and Customer id <b><br/><br/>" + sql;

            list = _db.ExecuteQuery<csScheduleCalendarCheck>(sql, string.Empty);
            grdDuplicateCheckTemp3.DataSource = list;
            grdDuplicateCheckTemp3.DataBind();

            ////-- Link Duplicate Check 
            sql = "SELECT parent_event_id,child_event_id, COUNT(*)  AS count  " + strCustId + " " + strEstId + " " +
            " FROM ScheduleCalendarLinkTemp " +
            " " + strConditionLink + " " +
            " GROUP BY parent_event_id,child_event_id  " + strCustId + " " + strEstId + " " +
            " HAVING COUNT(*) > 1 ";

            lblDuplicateCheckTemp4.Text = "<b>--Link Duplicate Check <b><br/><br/>" + sql;

            list = _db.ExecuteQuery<csScheduleCalendarCheck>(sql, string.Empty);
            grdDuplicateCheckTemp4.DataSource = list;
            grdDuplicateCheckTemp4.DataBind();

            ////-- Child Duplicate Check 
            sql = "SELECT child_event_id, customer_id,COUNT(*)  AS count " + strEstId + " " +
            " FROM ScheduleCalendarLinkTemp " +
             " " + strConditionLink + " " +
            " GROUP BY child_event_id, customer_id " + strEstId + " " +
            " HAVING COUNT(*) > 1 ";

            lblDuplicateCheckTemp5.Text = "<b>--Child Duplicate Check <b><br/><br/>" + sql;

            list = _db.ExecuteQuery<csScheduleCalendarCheck>(sql, string.Empty);
            grdDuplicateCheckTemp5.DataSource = list;
            grdDuplicateCheckTemp5.DataBind();

            ////-- event link with thyself Check
            sql = "SELECT parent_event_id,child_event_id,customer_id,estimate_id FROM  ScheduleCalendarLinkTemp  " +
            " WHERE parent_event_id = child_event_id " + strCondition;

            lblDuplicateCheckTemp6.Text = "<b>--event link with thyself Check<b><br/><br/>" + sql;

            list = _db.ExecuteQuery<csScheduleCalendarCheck>(sql, string.Empty);
            grdDuplicateCheckTemp6.DataSource = list;
            grdDuplicateCheckTemp6.DataBind();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }

    }

    public class csScheduleCalendarCheck
    {
        public string title { get; set; }
        public int event_id { get; set; }     
        public int child_event_id { get; set; }
        public int parent_event_id { get; set; }
        public int customer_id { get; set; }
        public int estimate_id { get; set; }
        public int count { get; set; }  

    }
}