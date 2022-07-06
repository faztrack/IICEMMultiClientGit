using Plivo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class mlandingpage : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetCustomerName(String prefixText, Int32 count)
    {


        List<csCustomer> cList = (List<csCustomer>)HttpContext.Current.Session["eSearch"];
        return (from c in cList
                where c.customer_name.ToLower().Contains(prefixText.ToLower())
                select c.customer_name).Take<String>(count).ToArray();


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

            if (Session["oCrew"] != null)
            {

                csCommonUtility.WriteLog("Crew Landing ok on page Load");

                PanelMap.Visible = true;
                pnlTimeClock.Visible = true;
                hyp_Leads.Visible = false;
            }
            if (Session["oUser"] != null)
            {
                hyp_Leads.Visible = true;
                userinfo obj = (userinfo)Session["oUser"];
                if (obj.IsTimeClock == true)
                    pnlTimeClock.Visible = true;
                else
                    pnlTimeClock.Visible = false;
                if (obj.role_id == 1 || obj.role_id == 4)
                    hypVendorList.Visible = true;
                else
                    hypVendorList.Visible = false;
                if (obj.role_id == 4)
                    hypConfirmTomorrow.Visible = true;
                else
                    hypConfirmTomorrow.Visible = false;

            }
            BindCustomer();
            GetCustomersNew(0);
            GetSearchCustomer(0);

            csCommonUtility.WriteLog("Crew Lading page get data successfully.");
        }
    }

    [System.Web.Services.WebMethod(true)]
    public static string SetCrewLocation(string latitude, string longitude)
    {
        string result = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        Crew_Detail crwDetailsobj = new Crew_Detail();
        Crew_Location obj = new Crew_Location();

        if (System.Web.HttpContext.Current.Session["oCrew"] != null)
        {
            crwDetailsobj = (Crew_Detail)System.Web.HttpContext.Current.Session["oCrew"];
        }
        try
        {

            obj.CrewId = crwDetailsobj.crew_id;
            obj.Latitude = latitude;
            obj.Longitude = longitude;
            obj.CreatedDate = DateTime.Now;

            _db.Crew_Locations.InsertOnSubmit(obj);
            _db.SubmitChanges();
            result = "ok";
        }
        catch (Exception ex)
        {
            result = ex.Message;
            Console.WriteLine("There is an error . Because " + ex);
        }

        return result;
    }

    private void BindCustomer()
    {

        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            string StrCondition = string.Empty;
            if (Session["oUser"] != null)
            {
                userinfo obj = (userinfo)Session["oUser"];
                string clientId = obj.client_id;
                if (obj.role_id!=3)
                {
                    StrCondition = " where  c.status_id NOT IN(4,5) and ce.status_id=3 and ce.IsEstimateActive=1 and c.client_id in (" + clientId + ") ";
                }
                else
                {
                    StrCondition = " where c.status_id NOT IN(4,5) AND  ce.status_id=3 and ce.IsEstimateActive=1 and c.sales_person_id=" + obj.sales_person_id;
                }
                string strQ = " select distinct case when ce.alter_job_number!='' then c.last_name1 + ',' + c.first_name1 + ' (' + ce.alter_job_number + ')' " +
                             " else  c.last_name1 + ',' + c.first_name1 + ' (' + ce.job_number + ')' end as customer_name,ce.customer_estimate_id," +
                             " c.customer_id,c.last_name1 "+
                             " from customers as c  "+
                             " INNER JOIN "+
                             " customer_estimate AS ce on ce.customer_id=c.customer_id "+ StrCondition + " order by customer_name ";
 
             
                List<csCustomer> mList = _db.ExecuteQuery<csCustomer>(strQ, string.Empty).ToList();
                Session.Add("eSearch", mList);
                ddCustomer.DataSource = mList;
                ddCustomer.DataTextField = "customer_name";
                ddCustomer.DataValueField = "customer_estimate_id";
                ddCustomer.DataBind();
                ddCustomer.Items.Insert(0, "Select Customer");

            }
            if (Session["oCrew"] != null)
            {
                Crew_Detail objC = (Crew_Detail)Session["oCrew"];
                StrCondition = " where  c.status_id NOT IN(4,5) and ce.status_id=3 and ce.IsEstimateActive=1 AND ce.client_id = " + objC.client_id;
                
                
                string strQ = " select distinct case when ce.alter_job_number!='' then c.last_name1 + ',' + c.first_name1 + ' (' + ce.alter_job_number + ')' " +
                             " else  c.last_name1 + ',' + c.first_name1 + ' (' + ce.job_number + ')' end as customer_name,ce.customer_estimate_id," +
                             " c.customer_id,c.last_name1 " +
                             " from customers as c  " +
                             " INNER JOIN " +
                             " customer_estimate AS ce on ce.customer_id=c.customer_id " + StrCondition + " order by customer_name ";

                List<csCustomer> mList = _db.ExecuteQuery<csCustomer>(strQ, string.Empty).ToList();
                Session.Add("eSearch", mList);
                ddCustomer.DataSource = mList;
                ddCustomer.DataTextField = "customer_name";
                ddCustomer.DataValueField = "customer_estimate_id";
                ddCustomer.DataBind();
                ddCustomer.Items.Insert(0, "Select Customer");

            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private void GetSearchCustomer(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        grdCustomerList.PageIndex = nPageNo;
        string strQ = string.Empty;
        // Initial Load Data
        string strWhere = "";
        if (Session["oUser"] != null)
        {
            userinfo obj = (userinfo)Session["oUser"];
            strWhere = " WHERE ce.job_number!='' and  userId =" + obj.user_id + " AND ce.client_id in ("+ obj.client_id +") AND IsCrew=0 order by searchDate DESC";
        }
        if (Session["oCrew"] != null)
        {
            Crew_Detail objC = (Crew_Detail)Session["oCrew"];
            strWhere = " WHERE ce.job_number!='' and  userId =" + objC.crew_id + " AND ce.client_id = "+ objC.client_id +" AND IsCrew=1 order by searchDate DESC";
        }
        strQ = " SELECT  searchCustomerId,ce.client_id, ce.customer_id, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date,mobile, " +
                      " ce.sales_person_id, update_date, ce.status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId, " +
                      " case when ce.alter_job_number!='' then searchcustomers.last_name1 + ',' + searchcustomers.first_name1 + ' (' + ce.alter_job_number + ')' " +
                       " else  searchcustomers.last_name1 + ',' + searchcustomers.first_name1 + ' (' + ce.job_number + ')' end as jobNumber,ce.customer_estimate_id " +
                      " FROM searchcustomers " +
                      " INNER JOIN  customer_estimate  AS ce on ce.customer_id=searchcustomers.customer_id  and ce.customer_estimate_id=searchcustomers.customer_estimate_id " +
                      "" + strWhere;


     

        IEnumerable<csSearchcustomer> mList = _db.ExecuteQuery<csSearchcustomer>(strQ, string.Empty).ToList();
        if (mList.Count() > 0)
        {
            lblInitial.Visible = false;
            pnlSearchCustomer.Visible = true;
            grdCustomerList.DataSource = mList;
            grdCustomerList.DataKeyNames = new string[] { "customer_id", "searchCustomerId", "jobNumber", "customer_estimate_id" };
            grdCustomerList.DataBind();
        }
        else
        {
            lblInitial.Visible = true;
            pnlSearchCustomer.Visible = false;
        }

        hdnCurrentPageNo.Value = Convert.ToString(nPageNo + 1);
        if (nPageNo == 0)
        {
            btnPrevious.Enabled = false;

        }
        else
        {
            btnPrevious.Enabled = true;

        }

        if (grdCustomerList.PageCount == nPageNo + 1)
        {
            btnNext.Enabled = false;

        }
        else
        {
            btnNext.Enabled = true;

        }
    }
    protected void GetCustomersNew(int nPageNo)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        grdIcon.PageIndex = nPageNo;
        string strQ = string.Empty;
        string strWhere = "";
        if (Session["oUser"] != null)
        {
            userinfo obj = (userinfo)Session["oUser"];
                strWhere =" WHERE userId =" + obj.user_id + " AND IsCrew=0 order by searchDate DESC";
        }
        if (Session["oCrew"] != null)
        {
            Crew_Detail objC = (Crew_Detail)Session["oCrew"];
            strWhere =" WHERE userId =" + objC.crew_id + " AND IsCrew=1 order by searchDate DESC";
        }

        strQ = " SELECT top 1 searchCustomerId,ce.client_id, ce.customer_id, first_name1, last_name1, first_name2, last_name2, address, cross_street, city, state, zip_code, fax, email, phone, is_active, registration_date,mobile, " +
                       " ce.sales_person_id, update_date, ce.status_id, notes, appointment_date, lead_source_id, status_note, company, email2, SuperintendentId, " +
                       " case when ce.alter_job_number!='' then searchcustomers.last_name1 + ',' + searchcustomers.first_name1 + ' (' + ce.alter_job_number + ')' " +
                        " else  searchcustomers.last_name1 + ',' + searchcustomers.first_name1 + ' (' + ce.job_number + ')' end as jobNumber,ce.customer_estimate_id " +
                       " FROM searchcustomers " +
                       " INNER JOIN  customer_estimate  AS ce on ce.customer_id=searchcustomers.customer_id and ce.customer_estimate_id=searchcustomers.customer_estimate_id  " +
                       "" + strWhere;
                       
        
        IEnumerable<csSearchcustomer> mList = _db.ExecuteQuery<csSearchcustomer>(strQ, string.Empty).ToList();
        if (mList.Count() > 0)
        {
            foreach (var li in mList)
            {

                if (li.notes.Length > 70)
                {
                    pnlLeadNotes.Visible = true;
                    lblCustomerLeadNotes.Text = li.notes.Substring(0, 70) + "...";
                }
                else
                {
                    if (li.notes != "" && li.notes != null)
                    {
                        lblCustomerLeadNotes.Text = li.notes;
                        inkLeadnoteView.Visible = false;
                    }
                    else
                        pnlLeadNotes.Visible = false;

                }
                hdnCustomerId.Value = li.customer_id.ToString();
                lblSearchCustomerName.Text = li.jobNumber; //li.first_name1 + " " + li.last_name1+" ("+li.jobNumber+")";

                if (li.phone != "" && li.phone != null)
                {
                    lblCustomerMobile.Visible = true;
                    lblCustomerPhone.Text = li.phone + ", ";
                    hrfCustomerPhone.HRef = "tel:" + li.phone;
                    lblCustomerMobile.Text = li.mobile + " m";
                    hrfCustomerMobile.HRef = "tel:" + li.mobile;
                }
                else
                {
                    lblCustomerMobile.Visible = false;
                    lblCustomerPhone.Text = li.phone;
                    hrfCustomerPhone.HRef = "tel:" + li.phone;
                }

                lblAddress.Text = li.address + " " + li.city;
                string address = li.address + ",+" + li.city + ",+" + li.state + ",+" + li.zip_code;
                hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;
                if (li.SuperintendentId != 0)
                {

                    user_info objUser = _db.user_infos.SingleOrDefault(u => u.user_id == li.SuperintendentId);
                    if (objUser != null)
                    {
                        lblSuperintendent.Text = objUser.first_name + " " + objUser.last_name;
                        lblPhone.Text = objUser.phone;
                        hyPhone.HRef = "tel:" + objUser.phone;
                    }
                }
                else
                {
                    lblSuperintendent.Text = "";
                    lblPhone.Text = "";

                }

            }
            lblSearhText.Visible = false;
            pnlCustomerFullName.Visible = true;
            DataTable dt = csCommonUtility.LINQToDataTable(mList);
            Session.Add("customerList", dt);
            grdIcon.DataSource = mList;
            grdIcon.DataKeyNames = new string[] { "customer_id", "sales_person_id", "customer_estimate_id" };
            grdIcon.DataBind();
        }
        else
        {
            lblSearhText.Visible = true;
            pnlCustomerFullName.Visible = false;
        }
    }

    protected void grdCustomerList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            LinkButton InkCustomerName = (LinkButton)e.Row.FindControl("InkCustomerName");
            if (e.Row.RowIndex == 0)
            {
                InkCustomerName.CssClass = "custName";
            }
            int ncid = Convert.ToInt32(grdCustomerList.DataKeys[e.Row.RowIndex].Values[0]);
            string jobNumber =grdCustomerList.DataKeys[e.Row.RowIndex].Values[2].ToString();
            int ncustomer_estimate_id = Convert.ToInt32(grdCustomerList.DataKeys[e.Row.RowIndex].Values[3]);
            // Customer Address
          //  customer cust = new customer();
           // cust = _db.customers.Single(c => c.customer_id == ncid);

            InkCustomerName.Text = jobNumber;// cust.last_name1;
            InkCustomerName.CommandArgument = ncustomer_estimate_id.ToString();
        }
    }

    protected void GetCustomer(object sender, EventArgs e)
    {
        try
        {

            LinkButton InkCustomerName = (LinkButton)sender;
            KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, InkCustomerName.ID, InkCustomerName.GetType().Name, "Gridview click Select Customer Landing Page");

            int ncustomer_estimate_id = Convert.ToInt32(InkCustomerName.CommandArgument);
            GetCustomerByCustomerID(ncustomer_estimate_id);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private void GetCustomerByCustomerID(int ncustomer_estimate_id)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        customer_estimate objES = _db.customer_estimates.SingleOrDefault(c => c.customer_estimate_id == ncustomer_estimate_id);
        customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == objES.customer_id);
         hdnCustomerId.Value = objCust.customer_id.ToString();
        if (objCust != null)
        {
            if (objCust.notes.Length > 70)
            {
                pnlLeadNotes.Visible = true;
                lblCustomerLeadNotes.Text = objCust.notes.Substring(0, 70) + "...";
            }
            else
            {
                if (objCust.notes != "" && objCust.notes != null)
                {
                    lblCustomerLeadNotes.Text = objCust.notes;
                    inkLeadnoteView.Visible = false;
                }
                else
                    pnlLeadNotes.Visible = false;
            }

            if (objCust.mobile != "" && objCust.mobile != null)
            {
                lblCustomerMobile.Visible = true;
                lblCustomerPhone.Text = objCust.phone + ", ";
                hrfCustomerPhone.HRef = "tel:" + objCust.phone;
                lblCustomerMobile.Text = objCust.mobile + " m";
                hrfCustomerMobile.HRef = "tel:" + objCust.mobile;
            }
            else
            {
                lblCustomerMobile.Visible = false;
                lblCustomerPhone.Text = objCust.phone;
                hrfCustomerPhone.HRef = "tel:" + objCust.phone;
            }

         
            pnlCustomerFullName.Visible = true;
            lblSearchCustomerName.Text =ddCustomer.SelectedItem.Text; //objCust.first_name1 + " " + objCust.last_name1;
            string address = objCust.address + ",+" + objCust.city + ",+" + objCust.state + ",+" + objCust.zip_code;
            hypGoogleMap.NavigateUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=" + address;
            lblAddress.Text = objCust.address + " " + objCust.city;

            if (objCust.SuperintendentId != 0)
            {
                user_info objUser = _db.user_infos.SingleOrDefault(u => u.user_id == objCust.SuperintendentId);
                if (objUser != null)
                {
                    lblSuperintendent.Text = objUser.first_name + " " + objUser.last_name;
                    lblPhone.Text = objUser.phone;
                    hyPhone.HRef = "tel:" + objUser.phone;
                }

            }
            else
            {
                lblSuperintendent.Text = "";
                lblPhone.Text = "";
            }
        }
        string strQ = string.Empty;

        if (Session["oUser"] != null)
        {
            userinfo obj = (userinfo)Session["oUser"];

            strQ = "delete from searchcustomers WHERE customer_id ='" + objCust.customer_id + "' AND userId=" + obj.user_id + "  AND IsCrew=0 and customer_estimate_id=" + ncustomer_estimate_id;
            _db.ExecuteCommand(strQ, string.Empty);

            strQ = "insert into searchcustomers " +
                         " SELECT  [client_id],[customer_id],[first_name1],[last_name1] ,[first_name2],[last_name2],[address] ,[cross_street], " +
                         " [city] ,[state] ,[zip_code] ,[fax] ,[email] ,[phone] ,[is_active],[registration_date],[sales_person_id],[update_date], " +
                         " [status_id],[notes] ,[appointment_date],[lead_source_id] ,[status_note],[company] ,[email2],[SuperintendentId], " +
                         " [mobile],[lead_status_id],[islead],[isCustomer],[website],[isCalendarOnline], " + obj.user_id + ",getdate(),0," + ncustomer_estimate_id + " " +
                         " FROM customers " +
                         " WHERE status_id NOT IN(4,5) AND  customer_id ='" + objCust.customer_id + "' ";

            _db.ExecuteCommand(strQ, string.Empty);
        }

        if (Session["oCrew"] != null)
        {
            Crew_Detail objC = (Crew_Detail)Session["oCrew"];

            strQ = "delete from searchcustomers WHERE customer_id ='" + objCust.customer_id + "' AND userId=" + objC.crew_id + "  AND IsCrew=1  and customer_estimate_id=" + ncustomer_estimate_id;
            _db.ExecuteCommand(strQ, string.Empty);

            strQ = "insert into searchcustomers " +
                         " SELECT  [client_id],[customer_id],[first_name1],[last_name1] ,[first_name2],[last_name2],[address] ,[cross_street], " +
                         " [city] ,[state] ,[zip_code] ,[fax] ,[email] ,[phone] ,[is_active],[registration_date],[sales_person_id],[update_date], " +
                         " [status_id],[notes] ,[appointment_date],[lead_source_id] ,[status_note],[company] ,[email2],[SuperintendentId], " +
                         " [mobile],[lead_status_id],[islead],[isCustomer],[website],[isCalendarOnline], " + objC.crew_id + ",getdate(),1 ," + ncustomer_estimate_id +
                         " FROM customers " +
                         " WHERE status_id NOT IN(4,5) AND  customer_id ='" + objCust.customer_id + "' ";

            _db.ExecuteCommand(strQ, string.Empty);
        }
      

        GetSearchCustomer(0);
        GetCustomersNew(0);
    }
    protected void grdCustomerList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCustomerList.ID, grdCustomerList.GetType().Name, "PageIndexChanging");
        GetSearchCustomer(e.NewPageIndex);
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(hdnCurrentPageNo.Value);
        GetSearchCustomer(nCurrentPage);
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPrevious.ID, btnPrevious.GetType().Name, "Click"); 
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(hdnCurrentPageNo.Value);
        GetSearchCustomer(nCurrentPage - 2);
    }

    protected void grdIcon_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {
                int nEstId = 1;
                int ncid = Convert.ToInt32(grdIcon.DataKeys[e.Row.RowIndex].Values[0]);
                int ncustomer_estimate_id = Convert.ToInt32(grdIcon.DataKeys[e.Row.RowIndex].Values[2]);
                customer_estimate objES = _db.customer_estimates.SingleOrDefault(c => c.customer_estimate_id == ncustomer_estimate_id);
                if(objES!=null)
                {
                    nEstId = objES.estimate_id;
                }
               

                //if (_db.customer_estimates.Where(ce => ce.customer_id == ncid && ce.client_id == 1 && ce.status_id == 3 && ce.IsEstimateActive == true).ToList().Count > 0)
                //{

                //    var result = (from ce in _db.customer_estimates
                //                  where ce.customer_id == ncid && ce.client_id == 1 && ce.status_id == 3 && ce.IsEstimateActive == true
                //                  select ce.estimate_id);

                //    int n = result.Count();
                //    if (result != null && n > 0)
                //        nEstId = result.Max();
                //}
                //else
                //{

                //    var result = (from ce in _db.customer_estimates
                //                  where ce.customer_id == ncid && ce.client_id == 1 && ce.IsEstimateActive == true
                //                  select ce.estimate_id);

                //    int n = result.Count();
                //    if (result != null && n > 0)
                //        nEstId = result.Max();
                //    else
                //        nEstId = 1;
                //}

                //Panel pnlLead = (Panel)e.Row.FindControl("pnlLead");
                Panel pnlCompoSiteCrew = (Panel)e.Row.FindControl("pnlCompoSiteCrew");
                //Panel pnlCompositUser = (Panel)e.Row.FindControl("pnlCompositUser");
                //if (Session["oCrew"] != null)
                //{
                //    pnlLead.Visible = false;
                //    pnlCompositUser.Visible = false;
                //    pnlCompoSiteCrew.Visible = true;

                //}
                //else
                //{
                //    pnlCompoSiteCrew.Visible = false;
                //    pnlLead.Visible = true;
                //    pnlCompositUser.Visible = true;
                //}

                Image imgSiteReview = (Image)e.Row.FindControl("imgSiteReview");
                imgSiteReview.ImageUrl = "~/assets/mobileicons/12-Site-review-List.png";

                HyperLink hypSiteReview = (HyperLink)e.Row.FindControl("hypSiteReview");
                hypSiteReview.NavigateUrl = "msiteviewlist.aspx?cid=" + ncid + "&nestid=" + nEstId;
                //Crew

                HyperLink hyp_DocumentManagement = (HyperLink)e.Row.FindControl("hyp_DocumentManagement");
                hyp_DocumentManagement.NavigateUrl = "mDocumentManagement.aspx?cid=" + ncid;

                HyperLink hypCompositrSowCrew = (HyperLink)e.Row.FindControl("hypCompositrSowCrew");
                hypCompositrSowCrew.NavigateUrl = "mcomposite_sow.aspx?cid=" + ncid + "&nestid=" + nEstId;

                HyperLink hypProjectNotes = (HyperLink)e.Row.FindControl("hypProjectNotes");
                hypProjectNotes.NavigateUrl = "mProjectNotes.aspx?cid=" + ncid + "&nestid=" + nEstId;
                HyperLink hypSelection = (HyperLink)e.Row.FindControl("hypSelection");
                hypSelection.NavigateUrl = "mselectionlist.aspx?cid=" + ncid + "&nestid=" + nEstId;
                Panel PnlMessage = (Panel)e.Row.FindControl("PnlMessage");

                HyperLink hypPmNotes = (HyperLink)e.Row.FindControl("hypPmNotes");
                if (Session["oCrew"] != null)
                {
                    hypPmNotes.Visible = false;
                    PnlMessage.Visible = true;

                }
                else
                {

                    hypPmNotes.Visible = true;
                    PnlMessage.Visible = false;
                }
                HyperLink hypPMNotes = (HyperLink)e.Row.FindControl("hypPmNotes");
                hypPMNotes.NavigateUrl = "PMNotes.aspx?cid=" + ncid + "&nestid=" + nEstId;

                LinkButton lnkMessage = (LinkButton)e.Row.FindControl("lnkMessage");
                 string nId = ncid + "," + nEstId;
                lnkMessage.CommandArgument = nId.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    protected void lnkMessage_Click(object sender, EventArgs e)
    {
        try
        {

            DataClassesDataContext _db = new DataClassesDataContext();
            Crew_Detail objC = (Crew_Detail)Session["oCrew"];
            LinkButton lnkMessage = sender as LinkButton;
            int nCid = 0;
            int nEstId = 0;
            string nId = lnkMessage.CommandArgument;
            if (nId.Contains(','))
            {
                Session.Add("nId", nId);
                var nCustomerId = nId.Split(',');
                nCid = Convert.ToInt32(nCustomerId[0]);
                nEstId = Convert.ToInt32(nCustomerId[1]);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openEnroutModal();", true);

                customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCid);
                lblMessage.Text = "Are you sure you want to  start for the next job?";
                lblCustomerFullName.Text = "<font style='font-weight:bold'>Name:</font> " + objCust.first_name1 + " " + objCust.last_name1;
                lblCustomerAddress.Text = "<font style='font-weight:bold'>Address:</font> " + objCust.address + " " + objCust.city + "," + objCust.state + " " + objCust.zip_code;
                GetCustomersNew(0);


            }




        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (Session["oCrew"] != null)
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                Crew_Detail objC = (Crew_Detail)Session["oCrew"];

                int nCid = 0;
                int nEstId = 0;

                if (Session["nId"] != null)
                {

                    string nId = Session["nId"].ToString();
                    if (nId.Contains(','))
                    {
                        var nCustomerId = nId.Split(',');

                        nCid = Convert.ToInt32(nCustomerId[0]);
                        nEstId = Convert.ToInt32(nCustomerId[1]);
                        CrewCustomerRoute objCCR = new CrewCustomerRoute();
                        objCCR.customer_id = nCid;
                        objCCR.estimate_id = nEstId;
                        objCCR.CrewId = objC.crew_id;
                        objCCR.IsStart = true;
                        objCCR.IsEnd = false;
                        objCCR.CreatedDate = DateTime.Now;
                        _db.CrewCustomerRoutes.InsertOnSubmit(objCCR);
                        _db.SubmitChanges();


                        string auth_id = ConfigurationManager.AppSettings["AuthId"];
                        string auth_token = ConfigurationManager.AppSettings["AuthToken"];
                        string from_mobile = ConfigurationManager.AppSettings["FromNumber"];
                        string AppType = ConfigurationManager.AppSettings["AppType"];


                        customer objCust = _db.customers.SingleOrDefault(c => c.customer_id == nCid);


                        string strToMobile = objCust.phone.Trim();

                        string NoFormat = csCommonUtility.ExtractNumber(strToMobile.Trim());

                        if (NoFormat.Length == 10)
                        {
                            strToMobile = "1" + strToMobile;
                        }

                        //Session["Address"].ToString()

                        if (System.Configuration.ConfigurationManager.AppSettings["SMSSEND"].ToLower() == "true")
                        {
                            string SMSBody = "Hi, your technician " + objC.full_name + " from Arizona's Interior Innovations is on the way to " + objCust.address + " " + objCust.city + ", " + objCust.state + " " + objCust.zip_code + ".";// + Environment.NewLine;
                            SMSBody += "Track now: https://ii.faztrack.com/crewmap.aspx?c=" + nCid + "&cid=" + objC.crew_id;
                            //string SMSBody = "Test";
                            ScheduleSM sms = new ScheduleSM();

                            var api = new PlivoApi(auth_id, auth_token);// Main Account


                            var response = api.Message.Create(
                                             src: from_mobile,
                                              dst: new List<String> { strToMobile },
                                              text: SMSBody

                                          );




                            sms.title = SMSBody.Trim();
                            sms.reference_id = nCid;
                            sms.reference_type = 1; // Customer
                            sms.estimate_id = 0;
                            sms.mobile = strToMobile;
                            //obj.reponse = "";
                            sms.event_id = 0;
                            sms.schedule_date = DateTime.Now;
                            sms.create_date = DateTime.Now;
                            sms.created_by = User.Identity.Name;
                            sms.reponse = response.ToString();
                            sms.is_success = true;
                            sms.send_date = DateTime.Now;
                            sms.error = "";
                            sms.Uuid = response.MessageUuid[0].ToString();
                            sms.Status = response.Message;
                            sms.units = 0;
                            sms.total_amount = 0;
                            sms.sms_rate = 0;

                            _db.ScheduleSMs.InsertOnSubmit(sms);

                            _db.SubmitChanges();
                        }



                        Response.Redirect("mcrewlocationmap.aspx?cid=" + nCid);

                        //  GetCustomersNew(0);
                    }
                }

            }

        }
        catch (Exception ex)
        {
            lblResultMSG.Text = csCommonUtility.GetSystemErrorMessage("Unable to send text. Please try again in 30 mins.");
        }
    }
    protected void lnkTimeClock_Click(object sender, EventArgs e)
    {
        Response.Redirect("mtimeclock.aspx",false);
    }
    protected void ddCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddCustomer.ID, ddCustomer.GetType().Name, "SelectedIndexChanged"); 
        try
        {


            GetCustomerByCustomerID(Convert.ToInt32(ddCustomer.SelectedValue));
           
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    protected void InkSchedule_Click(object sender, EventArgs e)
    {
        Response.Redirect("mcrewschedulecalendar.aspx");
    }
    protected void inkMyJobs_Click(object sender, EventArgs e)
    {
        Response.Redirect("mmyjobs.aspx");

    }


    protected void inkLeadnoteView_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, inkLeadnoteView.ID, inkLeadnoteView.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();

        try
        {
            customer cust = _db.customers.SingleOrDefault(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
            if (cust != null)
            {
                lblCustomerName.Text = "(" + cust.last_name1 + ")";
                lblLeadsNote.Text = cust.notes;
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 

        string StrSearch = txtSearch.Text.Trim().Replace("'", "");
        lblResultMSG.Text = "";
        if (StrSearch.Contains(","))
        {
            try
            {
                DataClassesDataContext _db = new DataClassesDataContext();
                string StrCondition = string.Empty;
                string job = "";
                string[] fullName = StrSearch.Split(',');

                string lastName = fullName[0].Trim();
                string firstName = fullName[1].Trim();


                var JNum = firstName.Split('(');
                if (StrSearch.IndexOf("(") != -1)
                {
                    job = JNum[1];

                }
                job = job.Replace(")", "");
               
                
             


                if (Session["oUser"] != null)
                {
                    userinfo obj = (userinfo)Session["oUser"];
                    if (obj.role_id != 3)
                    {
                        StrCondition = " where  c.status_id NOT IN(4,5) AND ce.alter_job_number ='" + job + "' or ce.job_number='" + job + "'";
                    }
                    else
                    {
                        StrCondition = " where c.status_id NOT IN(4,5) AND  ce.alter_job_number ='" + job + "' or ce.job_number='" + job + "' AND c.sales_person_id=" + obj.sales_person_id;
                    }


                    string strQ = " select distinct case when ce.alter_job_number!='' then c.last_name1 + ',' + c.first_name1 + ' (' + ce.alter_job_number + ')' " +
                              " else  c.last_name1 + ',' + c.first_name1 + ' (' + ce.job_number + ')' end as customer_name," +
                              " c.customer_id,c.last_name1 " +
                              " from customers as c  " +
                              " INNER JOIN " +
                              " customer_estimate AS ce on ce.customer_id=c.customer_id " + StrCondition + " order by customer_name ";
                    
                    List<csCustomer> mList = _db.ExecuteQuery<csCustomer>(strQ, string.Empty).ToList();
                    if (mList.Count == 1)
                    {
                        if (_db.customer_estimates.Where(ce => ce.job_number == job.Trim() && ce.status_id == 3).ToList().Count > 0)
                        {
                            customer_estimate objE = _db.customer_estimates.FirstOrDefault(ce => ce.job_number == job.Trim()  && ce.status_id == 3);

                            ddCustomer.SelectedValue = objE.customer_estimate_id.ToString();
                            GetCustomerByCustomerID(objE.customer_estimate_id);
                        }
                        else
                        {
                            customer_estimate objE = _db.customer_estimates.FirstOrDefault(ce => ce.alter_job_number == job.Trim() && ce.status_id == 3);
                            if (objE != null)
                            {
                                ddCustomer.SelectedValue = objE.customer_estimate_id.ToString();
                                GetCustomerByCustomerID(objE.customer_estimate_id);
                            }
                        }

                      
                    }
                   



                }
                if (Session["oCrew"] != null)
                {
                    Crew_Detail objC = (Crew_Detail)Session["oCrew"];
                    string CrewName = objC.first_name.Trim() + " " + objC.last_name.Trim();
                 

                    StrCondition = " where c.IsCustomer=1 AND c.status_id NOT IN(4,5) and ce.status_id=3 and ce.IsEstimateActive=1 AND s.IsEstimateActive = 1 AND s.employee_name  LIKE '%" + CrewName + "%' AND ce.alter_job_number ='" + job + "' or ce.job_number='" + job + "'";
                    string strQ = " select distinct case when ce.alter_job_number!='' then c.last_name1 + ',' + c.first_name1 + ' (' + ce.alter_job_number + ')' " +
                                 " else  c.last_name1 + ',' + c.first_name1 + ' (' + ce.job_number + ')' end as customer_name," +
                                 " c.customer_id,c.last_name1 " +
                                 " from customers as c  " +
                                 " Right join ScheduleCalendar as s ON c.customer_id = s.customer_id " +
                                 " INNER JOIN  customer_estimate  AS ce on ce.customer_id=s.customer_id and ce.estimate_id=s.estimate_id " +
                                 " " + StrCondition + " ";

                    List<csCustomer> mList = _db.ExecuteQuery<csCustomer>(strQ, string.Empty).ToList();

                    if (mList.Count == 1)
                    {
                        if (_db.customer_estimates.Where(ce => ce.job_number == job.Trim() &&  ce.status_id == 3).ToList().Count > 0)
                        {
                            customer_estimate objE = _db.customer_estimates.FirstOrDefault(ce => ce.job_number == job.Trim() && ce.status_id == 3);

                            ddCustomer.SelectedValue = objE.customer_estimate_id.ToString();
                            GetCustomerByCustomerID(objE.customer_estimate_id);
                        }
                        else
                        {
                            customer_estimate objE = _db.customer_estimates.FirstOrDefault(ce => ce.alter_job_number == job.Trim() && ce.status_id == 3);
                            if (objE != null)
                            {
                                ddCustomer.SelectedValue = objE.customer_estimate_id.ToString();
                                GetCustomerByCustomerID(objE.customer_estimate_id);
                            }
                        }
                    }
                   

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        else
        {
            if (StrSearch == "")
                BindCustomer();
            else
                lblResultMSG.Text = csCommonUtility.GetSystemErrorMessage("No record found.");
        }

    }
}