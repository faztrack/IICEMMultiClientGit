using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class customerschedulecalendar : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            customeruserinfo objUName = new customeruserinfo();
            string strUName = "";
            //if (Session["oCustomerUser"] == null)
            //{
            //    //Page.ClientScript.RegisterOnSubmitStatement(typeof(Page), "closePage", "window.onunload = CloseWindow();");
            //    objUName = (customeruserinfo)Session["oCustomerUser"];
            //    strUName = objUName.customerusername;
            //}

            //Clear Search
            HttpContext.Current.Session.Add("CusId", 0);

            string strCustName = "";
            int nCustomerID = 0;
            int nEstimateID = 0;
            int nEmployeeID = 0;
            int nTypeId = 1;

            DataClassesDataContext _db = new DataClassesDataContext();
            customer objCust = new customer();
            customer_estimate cus_est = new customer_estimate();

            co_pricing_master objCOPM = new co_pricing_master();
            location objLocation = new location();


            string serviceColor = "fc-default";


            if (nTypeId == 1)
            {
                lbltopHead.Text = "Your Project Calendar";
            }

            if (Session["oCustomerUser"] != null) // Customer Schedule
            {
                customeruserinfo obj = (customeruserinfo)Session["oCustomerUser"];
                nCustomerID = Convert.ToInt32(obj.customerid);

                int CustomerCalendarWeeklyView = 1;
                if (_db.customers.Where(c => c.customer_id == nCustomerID).Count() > 0)
                {
                    objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCustomerID);
                    strCustName = objCust.first_name1 + " " + objCust.last_name1;
                    lbltopHead.Text = "Project Calendar for " + strCustName;
                    CustomerCalendarWeeklyView = (int)objCust.CustomerCalendarWeeklyView;
                }

                hdnEstIDSelected.Value = nEstimateID.ToString();
                hdnCustIDSelected.Value = nCustomerID.ToString();
                hdnCustCalWeeklyView.Value = CustomerCalendarWeeklyView.ToString();

                string strQ = "SELECT * FROM customer_estimate WHERE customer_id=" + nCustomerID + " AND status_id = 3 AND client_id=1";
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
                }

                IEnumerable<int> listCheckedEstId = chkEst.Items
                                       .Cast<ListItem>()
                                       .Where(item => item.Selected)
                                       .OrderBy(item => item.Selected)
                                       .Select(item => int.Parse(item.Value));

                HttpContext.Current.Session.Add("sSelectedCustEstIdList", listCheckedEstId);

                // BindEstimate(nCustomerID, listCheckedEstId);

                HttpContext.Current.Session.Add("uname", strUName);
                HttpContext.Current.Session.Add("CustSelected", nCustomerID);
                HttpContext.Current.Session.Add("EstSelected", nEstimateID);
                HttpContext.Current.Session.Add("sCustCalWeeklyView", CustomerCalendarWeeklyView);

                if (_db.ScheduleCalendars.Where(sc => sc.customer_id == nCustomerID && listCheckedEstId.Contains((int)sc.estimate_id)).Count() > 0)
                {
                    var date = _db.ScheduleCalendars.Where(sc => sc.customer_id == nCustomerID && listCheckedEstId.Contains((int)sc.estimate_id) && sc.event_start >= DateTime.Now).Min(x => x.event_start);


                    if (_db.customer_estimates.Where(sc => sc.customer_id == nCustomerID).Count() != 0)
                    {

                        var dtStartOfJob = _db.customer_estimates.FirstOrDefault(c => c.customer_id == nCustomerID).job_start_date ?? "";
                        if (dtStartOfJob != "")
                            StartofJob.Value = Convert.ToDateTime(dtStartOfJob).ToShortDateString();


                    }

                    HttpContext.Current.Session.Add("cid", nCustomerID);
                    HttpContext.Current.Session.Add("eid", nEstimateID);
                    HttpContext.Current.Session.Add("empid", nEmployeeID);
                    HttpContext.Current.Session.Add("TypeID", nTypeId);

                    hdnEstimateID.Value = nEstimateID.ToString();
                    hdnCustomerID.Value = nCustomerID.ToString();
                    hdnEmployeeID.Value = nEmployeeID.ToString();
                    hdnTypeID.Value = nTypeId.ToString();

                    hdnEventStartDate.Value = date.ToString();
                }
                else
                {
                    HttpContext.Current.Session.Add("cid", null);
                    HttpContext.Current.Session.Add("eid", null);
                    HttpContext.Current.Session.Add("empid", null);
                    //HttpContext.Current.Session.Add("TypeID", null);

                    hdnEstimateID.Value = "";
                    hdnCustomerID.Value = "";
                    hdnEmployeeID.Value = "";
                    hdnTypeID.Value = "";


                }
            }
            else
            {
                HttpContext.Current.Session.Add("cid", null);
                HttpContext.Current.Session.Add("eid", null);
                HttpContext.Current.Session.Add("empid", null);
                //HttpContext.Current.Session.Add("TypeID", null);
                HttpContext.Current.Session.Add("CustSelected", null);
                HttpContext.Current.Session.Add("EstSelected", null);

                hdnEstimateID.Value = "";
                hdnCustomerID.Value = "";
                hdnEmployeeID.Value = "";
                hdnTypeID.Value = "";
                Response.Redirect("customerlogin.aspx");
            }

        }
    }

    protected void chkEst_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, chkEst.ID, chkEst.GetType().Name, "SelectedIndexChanged"); 
        int nCustId = Convert.ToInt32(hdnCustomerID.Value);
        int nEstId = 0;// Convert.ToInt32(ddlEst.SelectedItem.Value);

        IEnumerable<int> listCheckedEstId = chkEst.Items
                                         .Cast<ListItem>()
                                         .Where(item => item.Selected)
                                         .OrderBy(item => item.Selected)
                                         .Select(item => int.Parse(item.Value));

        HttpContext.Current.Session.Add("sSelectedCustEstIdList", listCheckedEstId);

        // BindEstimate(nCustId, listCheckedEstId);
    }

    public void BindEstimate(int nCustId, IEnumerable<int> listCheckedEstId)
    {
        //DataClassesDataContext _db = new DataClassesDataContext();

        //var item = from e in _db.customer_estimates

        //           where e.customer_id == nCustId && listCheckedEstId.Contains((int)e.estimate_id)
        //           orderby e.estimate_name
        //           select new
        //           {
        //               estimate_name = e.estimate_name,
        //               estimate_id = e.estimate_id,
        //               customer_id = e.customer_id
        //           };

        //grdEstimates.DataSource = item.Distinct().OrderBy(o => o.estimate_id).ToList();
        //grdEstimates.DataKeyNames = new string[] { "customer_id", "estimate_id", "estimate_name" };
        //grdEstimates.DataBind();
    }
}