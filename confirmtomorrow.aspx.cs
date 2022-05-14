using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class confirmtomorrow : System.Web.UI.Page
{

    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.last_name1.ToLower().Trim().StartsWith(prefixText.ToLower().Trim())
                    select c.last_name1.Trim()).Distinct().Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.last_name1.ToLower().Trim().StartsWith(prefixText.ToLower().Trim())
                    select c.last_name1.Trim()).Distinct().Take<String>(count).ToArray();
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {


        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);

            if (Session["oUser"] == null && Session["oCrew"] == null)
            {
                Response.Redirect("mobile.aspx");
            }
            DataClassesDataContext _db = new DataClassesDataContext();
            if (Session["oUser"] != null)
            {
                userinfo objUName = (userinfo)Session["oUser"];
                List<customer> CustomerList = (from c in _db.customers where c.SuperintendentId == objUName.user_id && c.is_active == true && c.isCustomer == 1 select c).ToList();
                Session.Add("cSearch", CustomerList);
            }

            GetData();
            BindData();
          
        }
   
    }

    private void GetData()
    {
        try
        {
            DateTime today = DateTime.Now;
            DateTime start = DateTime.Now.AddDays(1);
            DateTime end = DateTime.Now.AddDays(1);
            DataClassesDataContext _db = new DataClassesDataContext();
            if (Session["oUser"] != null)
            {
                userinfo objUName = (userinfo)Session["oUser"];
                string sql = "select sc.event_id, c.customer_id, c.first_name1, c.last_name1, c.first_name1 +' '+ c.last_name1 AS customer_name, '('+c.last_name1+') '+sc.title as title, sc.employee_name, sc.estimate_id, " +
                           " CASE WHEN c.Phone = '' THEN c.mobile ELSE c.Phone END AS mobile, sc.event_start, sc.event_end, sc.section_name, sc.location_name, sc.IsScheduleDayException," +
                           " ISNULL(s.first_name,'') as superfirstname, ISNULL(s.last_name,'') as superlastname, ISNULL(s.phone,'') as supermobile, " +
                           " c.address + ', ' + c.city + ', ' + c.state + ', ' + c.zip_code as CustAddress,ce.estimate_name " +
                           " from [ScheduleCalendar] AS sc " +
                           " Inner join customers AS c on sc.customer_id = c.customer_id " +
                           " INNER JOIN  customer_estimate as ce on ce.customer_id=sc.customer_id and ce.estimate_id=sc.estimate_id " +
                           " LEFT OUTER JOIN user_info as s on c.SuperintendentId = s.user_id " +
                           " where (c.is_active=1 and sc.IsEstimateActive=1 and sc.employee_name != ''  and sc.employee_name != 'TBD TBD') and c.SuperintendentId=" + objUName.user_id + " and ((event_start>= '" + start.ToString("MM/dd/yyyy") + "'  and event_start<'" + end.AddDays(1).ToString("MM/dd/yyyy") + "') or ( event_start< '" + start.ToString("MM/dd/yyyy") + "'  and event_end>='" + start.AddDays(1).ToString("MM/dd/yyyy") + "')) " +
                           " order by  c.first_name1";

                DataTable dt = csCommonUtility.GetDataTable(sql);

                foreach (DataRow dr in dt.Rows)
                {
                    //  SuperConfirmTomorrow objS = _db.SuperConfirmTomorrows.SingleOrDefault(s => s.event_id == Convert.ToInt32(dr["event_id"].ToString()) && s.SuperintendentId == objUName.user_id );
                    string sSQl = " select * from SuperConfirmTomorrow where event_id =" + Convert.ToInt32(dr["event_id"].ToString()) + " and SuperintendentId = " + objUName.user_id + " and CreateDate>='" + today.ToString("MM/dd/yyyy") + "'";
                    DataTable dt2 = csCommonUtility.GetDataTable(sSQl);
                    if (dt2.Rows.Count == 0)
                    {
                        SuperConfirmTomorrow objST = new SuperConfirmTomorrow();
                        objST.event_id = Convert.ToInt32(dr["event_id"].ToString());
                        objST.customer_id = Convert.ToInt32(dr["customer_id"].ToString());
                        objST.estimate_id = Convert.ToInt32(dr["estimate_id"].ToString());
                        objST.estimate_name = dr["estimate_name"].ToString();
                        objST.SuperintendentId = objUName.user_id;
                        objST.superlastname = dr["superfirstname"].ToString() + " " + dr["superlastname"].ToString();
                        objST.first_name1 = dr["first_name1"].ToString();
                        objST.last_name1 = dr["last_name1"].ToString();
                        objST.section_name = dr["section_name"].ToString();
                        objST.location_name = dr["location_name"].ToString();
                        objST.employee_name = dr["employee_name"].ToString();
                        objST.event_start = Convert.ToDateTime(dr["event_start"].ToString());
                        objST.event_end = Convert.ToDateTime(dr["event_end"].ToString());
                        objST.StartYes = false;
                        objST.StartNo = false;
                        objST.CreateDate = DateTime.Now;
                        _db.SuperConfirmTomorrows.InsertOnSubmit(objST);
                        _db.SubmitChanges();

                    }
                }

            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private void BindData()
    {
        try
        {
            if (Session["oUser"] != null)
            {
                DateTime todays = DateTime.Now;
                userinfo objUName = (userinfo)Session["oUser"];
                string sql = " SELECT Id, SuperintendentId, superlastname, event_id, estimate_id, customer_id, first_name1, last_name1, section_name, location_name, estimate_name, event_start, event_end, CreateDate, StartYes, StartNo," +
                            " employee_name FROM SuperConfirmTomorrow " +
                            " where SuperintendentId=" + objUName.user_id + " and CreateDate>='" + todays.ToString("MM/dd/yyyy") + "'" +
                            " order by last_name1 ";
                DataTable dt = csCommonUtility.GetDataTable(sql);
                gridEvent.DataSource = dt;
                gridEvent.DataKeyNames = new string[] { "event_id", "employee_name", "customer_id", "estimate_id", "section_name", "location_name", "Id", "StartNo" };
                gridEvent.DataBind();

            
                if (dt.Rows.Count == 0)
                {
                    btnSave.Visible = false;
                    btnSave2.Visible = false;
                }
                else
                {
                    btnSave.Visible = true;
                    btnSave2.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("mlandingpage.aspx");
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {

    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {

    }

    protected void gridEvent_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            int nEvent_id = Convert.ToInt32(gridEvent.DataKeys[e.Row.RowIndex].Values[0]);
            string strEmployee_name = gridEvent.DataKeys[e.Row.RowIndex].Values[1].ToString();
            int nCustomer_id = Convert.ToInt32(gridEvent.DataKeys[e.Row.RowIndex].Values[2]);
            int nEstimate_id = Convert.ToInt32(gridEvent.DataKeys[e.Row.RowIndex].Values[3]);
            string section = gridEvent.DataKeys[e.Row.RowIndex].Values[4].ToString();
            string location = gridEvent.DataKeys[e.Row.RowIndex].Values[5].ToString();
            int nId = Convert.ToInt32(gridEvent.DataKeys[e.Row.RowIndex].Values[6]);
            Boolean StartNo = Convert.ToBoolean(gridEvent.DataKeys[e.Row.RowIndex].Values[7]);
            Label lblSection = e.Row.FindControl("lblSection") as Label;
            Label lblLocation = e.Row.FindControl("lblLocation") as Label;
            RadioButtonList rdbStart = e.Row.FindControl("rdbStart") as RadioButtonList;
            lblSection.Text = section;
            lblLocation.Text = location;

            if (StartNo)
                rdbStart.SelectedValue = "0";

        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {

            DataClassesDataContext _db = new DataClassesDataContext();
            string StartNoCheckedIDs = "";
            string StartNoUncheckIDs = "";
            StartNoCheckedIDs = "";
            lblResult.Text = "";
            string sSQL = "";
            if (Session["oUser"] != null)
            {

                userinfo objUName = (userinfo)Session["oUser"];
                for (int i = 0; i < gridEvent.Rows.Count; i++)
                {

                    int nId = Convert.ToInt32(gridEvent.DataKeys[i].Values[6].ToString());
                    GridViewRow row = gridEvent.Rows[i];

                    //CheckBox ChkBoxRows = (CheckBox)row.Cells[5].Controls[1];
                    RadioButtonList rdbStart = (RadioButtonList)row.FindControl("rdbStart");
                    if (rdbStart.SelectedValue == "0")
                    {

                        StartNoCheckedIDs += nId + ",";
                    }
                    else
                    {
                        StartNoUncheckIDs += nId + ",";
                    }

                }

                StartNoCheckedIDs = StartNoCheckedIDs.TrimEnd(',');
                StartNoUncheckIDs = StartNoUncheckIDs.TrimEnd(',');
                if (StartNoCheckedIDs.Length > 0)
                {
                    sSQL = " UPDATE SuperConfirmTomorrow set StartNo = 1 WHERE  Id in(" + StartNoCheckedIDs + ") and SuperintendentId=" + objUName.user_id;
                    _db.ExecuteCommand(sSQL, string.Empty);
                }
                if (StartNoUncheckIDs.Length > 0)
                {
                    sSQL = " UPDATE SuperConfirmTomorrow set StartNo = 0 WHERE  Id in(" + StartNoUncheckIDs + ") and SuperintendentId=" + objUName.user_id;
                    _db.ExecuteCommand(sSQL, string.Empty);
                }

                lblResult.Text = csCommonUtility.GetSystemMessage("Data has been saved successfully.");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
            }




        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void rdbStart_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (Session["oUser"] != null)
            {
                userinfo objUName = (userinfo)Session["oUser"];
                string sSQL = "";
                DataClassesDataContext _db = new DataClassesDataContext();
                GridViewRow row = (GridViewRow)(((RadioButtonList)sender).NamingContainer);
                int nId = Convert.ToInt32(gridEvent.DataKeys[row.RowIndex].Values[6].ToString());
                RadioButtonList rdbStart = (RadioButtonList)row.FindControl("rdbStart");
                if (rdbStart.SelectedValue == "0")
                {
                    sSQL = " UPDATE SuperConfirmTomorrow set StartNo = 1 WHERE  Id =" + nId + " and SuperintendentId=" + objUName.user_id;
                    _db.ExecuteCommand(sSQL, string.Empty);
                }
                else
                {
                    sSQL = " UPDATE SuperConfirmTomorrow set StartNo = 0 WHERE  Id =" + nId + " and SuperintendentId=" + objUName.user_id;
                    _db.ExecuteCommand(sSQL, string.Empty);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }


    }


}











