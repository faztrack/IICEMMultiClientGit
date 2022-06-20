using DataStreams.Csv;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class ProjectNotesReport : System.Web.UI.Page
{
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
    public static string[] GetAddress(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.address.ToLower().StartsWith(prefixText.ToLower())
                    select c.address).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.address.StartsWith(prefixText)
                    select c.address).Take<String>(count).ToArray();
        }
    }
    [WebMethod]
    public static string[] GetEmail(String prefixText, Int32 count)
    {
        if (HttpContext.Current.Session["cSearch"] != null)
        {
            List<customer> cList = (List<customer>)HttpContext.Current.Session["cSearch"];
            return (from c in cList
                    where c.email.ToLower().StartsWith(prefixText.ToLower())
                    select c.email).Take<String>(count).ToArray();
        }
        else
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            return (from c in _db.customers
                    where c.email.StartsWith(prefixText)
                    select c.email).Take<String>(count).ToArray();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string divisionName = "";
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            else
            {
                userinfo oUser = (userinfo)Session["oUser"];
                hdnClientId.Value = oUser.client_id.ToString();
                hdnPrimaryDivision.Value = oUser.primaryDivision.ToString();
                divisionName = oUser.divisionName;
            }
            if (Page.User.IsInRole("rpt009") == false)
            {
                // No Permission Page.
                Response.Redirect("nopermission.aspx");
            }

            // Get Customers
            # region Get Customers
            DataClassesDataContext _db = new DataClassesDataContext();
            List<customer> CustomerList = _db.customers.ToList();
            Session.Add("cSearch", CustomerList);

            # endregion
            //   DynamicQuery();

            BindDivision();
            LoadProjectNoteInfo(0, "WHERE pn.is_complete = 0");


            if (divisionName != "" && divisionName.Contains(","))
            {
                ddlDivision.Enabled = true;
            }
            else
            {
                ddlDivision.Enabled = false;
            }



        }

    }

    private void LoadProjectNoteInfo(int nPageNo, string strCondition)
    {

        DataClassesDataContext _db = new DataClassesDataContext();       

        string serchkey = txtSearch.Text.Trim();
      
        string strQ = "";
        

     
        if (serchkey.Length > 0)
        {         

            if (ddlSearchBy.SelectedValue == "1") // First Name
            {
                strCondition += " WHERE cus.first_name1 LIKE '%" + serchkey.Replace("'", "''") + "%' ";

            }

            else if (ddlSearchBy.SelectedValue == "2") // Last Name
            {
                strCondition += " WHERE cus.last_name1 like '%" + serchkey.Replace("'", "''") + "%' ";

            }
        }

        if (Convert.ToInt32(ddlStatus.SelectedValue) != 3) // Is complete? or All
        {
       
            if (strCondition.Length > 0)
            {
                strCondition += " AND pn.is_complete = " + ddlStatus.SelectedValue;
            }
            else
            {
                strCondition += "WHERE pn.is_complete = " + ddlStatus.SelectedValue;
            }

        }

        if (txtProjectStartDate.Text.Trim() != "" && txtProjectEndDate.Text.Trim() != "")
        {
            try
            {
                DateTime dt1 = Convert.ToDateTime(txtProjectStartDate.Text.Trim());
                DateTime dt2 = Convert.ToDateTime(txtProjectEndDate.Text.Trim());
                if (Convert.ToDateTime(txtProjectEndDate.Text.Trim()) >= Convert.ToDateTime(txtProjectStartDate.Text.Trim()))
                {
                    if (strCondition.Length == 0)
                    {

                        strCondition = " WHERE pn.ProjectDate between '" + dt1 + "' AND '" + dt2 + "'";
                    }
                    else
                    {
                        strCondition += " AND pn.ProjectDate between '" + dt1 + "' AND '" + dt2 + "'";
                    }
                }
            }
            catch
            {
            }
        }

        if (strCondition.Length > 0)
            strCondition += "AND cus.status_id NOT IN(4,5) AND cus.customer_id NOT IN (SELECT DISTINCT customer_id FROM customer_estimate WHERE IsEstimateActive = 0)";
        else
            strCondition += "Where cus.status_id NOT IN(4,5) AND cus.customer_id NOT IN (SELECT DISTINCT customer_id FROM customer_estimate WHERE IsEstimateActive = 0)";


        if (ddlDivision.SelectedItem.Text != "All")
        {
            if (strCondition.Length > 2)
                strCondition += " AND pn.client_id =" + Convert.ToInt32(ddlDivision.SelectedValue);
            else
                strCondition += " WHERE pn.client_id =" + Convert.ToInt32(ddlDivision.SelectedValue);
        }


        strQ = " SELECT pn.ProjectNoteId, pn.customer_id, cus.first_name1, pn.client_id as clientID,  cus.last_name1, cus.last_name1 + ', ' + cus.first_name1 as customer_name, pn.NoteDetails, pn.is_complete, pn.ProjectDate, pn.CreateDate, " +
            " pn.SectionName,pn.MaterialTrack,pn.DesignUpdates,pn.SuperintendentNotes,pn.isSOW " +
            " FROM ProjectNoteInfo as pn " +
            " INNER JOIN customers as cus ON pn.customer_id = cus.customer_id " +
            " " + strCondition + " " +
            " ORDER BY pn.ProjectDate desc ";

       // IEnumerable<csPrjectNoteReport> item = _db.ExecuteQuery<csPrjectNoteReport>(strQ, string.Empty).ToList();
        DataTable dt = csCommonUtility.GetDataTable(strQ);
        Session.Add("ProjectNote", dt);

        grdProjectNote.PageIndex = nPageNo;
        if (ddlItemPerPage.SelectedValue != "4")
        {
            grdProjectNote.PageSize = Convert.ToInt32(ddlItemPerPage.SelectedValue);
        }
        else
        {
            grdProjectNote.PageSize = 200;
        }
        grdProjectNote.DataSource = dt;
        grdProjectNote.DataKeyNames = new string[] { "customer_id", "customer_name", "clientID" };
        grdProjectNote.DataBind();
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

        if (grdProjectNote.PageCount == nPageNo + 1)
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

    protected void grdProjectNote_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DataClassesDataContext _db = new DataClassesDataContext();

            int ncid = Convert.ToInt32(grdProjectNote.DataKeys[e.Row.RowIndex].Values[0].ToString());
            string customerName = grdProjectNote.DataKeys[e.Row.RowIndex].Values[1].ToString();
            string nClientId = grdProjectNote.DataKeys[e.Row.RowIndex].Values[2].ToString();
            Label lblDivisionName = e.Row.FindControl("lblDivisionName") as Label;
            lblDivisionName.Text = csCommonUtility.GetDivisionName(nClientId);

            Label lblIsSOW = (Label)e.Row.FindControl("lblSOW");

            Label lblDescription = (Label)e.Row.FindControl("lblDescription");
            Label lblIsComplete = (Label)e.Row.FindControl("lblIsComplete");
            HyperLink hypCustomerName = (HyperLink)e.Row.FindControl("hypCustomerName");


            Label lblMaterialTrack = (Label)e.Row.FindControl("lblMaterialTrack");
            Label lblDesignUpdates = (Label)e.Row.FindControl("lblDesignUpdates");
            Label lblSuperintendentNotes = (Label)e.Row.FindControl("lblSuperintendentNotes");
        
         

            LinkButton lnkOpen = (LinkButton)e.Row.FindControl("lnkOpen");
            LinkButton lnkOpenMaterialTrack = (LinkButton)e.Row.FindControl("lnkOpenMaterialTrack");
            LinkButton lnkOpenDesignUpdates = (LinkButton)e.Row.FindControl("lnkOpenDesignUpdates");
            LinkButton lnkOpenSuperintendentNotes = (LinkButton)e.Row.FindControl("lnkOpenSuperintendentNotes");


            TextBox txtDate = (TextBox)e.Row.FindControl("txtDate");
            Label lblDate = (Label)e.Row.FindControl("lblDate");
          
            if (Convert.ToBoolean(lblIsComplete.Text))
            {
                lblIsComplete.Text = "Yes";
                e.Row.Attributes.CssStyle.Add("color", "green");

            }
            else
            {
                lblIsComplete.Text = "No";

            }
            if (Convert.ToBoolean(lblIsSOW.Text))
            {
                lblIsSOW.Text = "Yes";
          

            }
            else
            {
                lblIsSOW.Text = "No";

            }
            string str = lblDescription.Text.Replace("&nbsp;", "");



            if (str != "" && str.Length > 90)
            {
                lblDescription.Text = str.Substring(0, 90) + " ...";
                lblDescription.ToolTip = str;
                lnkOpen.Visible = true;
            }
            else
            {
                lblDescription.Text = str;
                lnkOpen.Visible = false;
            }

            if (lblMaterialTrack.Text != "" && lblMaterialTrack.Text.Length > 90)
            {
                lblMaterialTrack.Text = lblMaterialTrack.Text.Substring(0, 90) + " ...";
                lblMaterialTrack.ToolTip = lblMaterialTrack.Text;
                lnkOpenMaterialTrack.Visible = true;
            }
            else
            {
                lblMaterialTrack.Text = lblMaterialTrack.Text;
                lnkOpenMaterialTrack.Visible = false;
            }

            if (lblDesignUpdates.Text != "" && lblDesignUpdates.Text.Length > 90)
            {
                lblDesignUpdates.Text = lblDesignUpdates.Text.Substring(0, 90) + " ...";
                lblDesignUpdates.ToolTip = lblDesignUpdates.Text;
                lnkOpenDesignUpdates.Visible = true;
            }
            else
            {
                lblDesignUpdates.Text = lblDesignUpdates.Text;
                lnkOpenDesignUpdates.Visible = false;
            }

            if (lblSuperintendentNotes.Text != "" && lblSuperintendentNotes.Text.Length > 90)
            {
                lblSuperintendentNotes.Text = lblSuperintendentNotes.Text.Substring(0, 90) + " ...";
                lblSuperintendentNotes.ToolTip = lblSuperintendentNotes.Text;
                lnkOpenSuperintendentNotes.Visible = true;
            }
            else
            {
                lblSuperintendentNotes.Text = lblSuperintendentNotes.Text;
                lnkOpenSuperintendentNotes.Visible = false;
            }

            int nEstId = 0;

            if (_db.customer_estimates.Where(ce => ce.customer_id == ncid && ce.client_id.ToString().Contains(hdnClientId.Value) && ce.status_id == 3 && ce.IsEstimateActive == true).ToList().Count > 0)
            {

                var result = (from ce in _db.customer_estimates
                              where ce.customer_id == ncid && ce.client_id.ToString().Contains(hdnClientId.Value) && ce.status_id == 3 && ce.IsEstimateActive == true
                              select ce.estimate_id);

                int n = result.Count();
                if (result != null && n > 0)
                    nEstId = result.Max();
            }
            else
            {

                var result = (from ce in _db.customer_estimates
                              where ce.customer_id == ncid && ce.client_id.ToString().Contains(hdnClientId.Value) && ce.IsEstimateActive == true
                              select ce.estimate_id);

                int n = result.Count();
                if (result != null && n > 0)
                    nEstId = result.Max();
                else
                    nEstId = 1;
            }

            hypCustomerName.Text = customerName;
            hypCustomerName.NavigateUrl = "ProjectNotes.aspx?TypeId=4&cid="+ncid + "&eid=" +nEstId;

        }

    }

    protected void lnkOpen_Click(object sender, EventArgs e)
    {
        LinkButton btnsubmit = sender as LinkButton;

        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblDescription = gRow.Cells[2].Controls[0].FindControl("lblDescription") as Label;
        Label lblDescription_r = gRow.Cells[2].Controls[1].FindControl("lblDescription_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkOpen") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblDescription.Visible = false;
            lblDescription_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblDescription.Visible = true;
            lblDescription_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }
    protected void lnkOpenMaterialTrack_Click(object sender, EventArgs e)
    {

        LinkButton btnsubmit = sender as LinkButton;
    
        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblMaterialTrack = gRow.Cells[2].Controls[0].FindControl("lblMaterialTrack") as Label;
        Label lblMaterialTrack_r = gRow.Cells[2].Controls[1].FindControl("lblMaterialTrack_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkOpenMaterialTrack") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblMaterialTrack.Visible = false;
            lblMaterialTrack_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblMaterialTrack.Visible = true;
            lblMaterialTrack_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }
    protected void lnkOpenDesignUpdates_Click(object sender, EventArgs e)
    {

        LinkButton btnsubmit = sender as LinkButton;
    
        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblDesignUpdates = gRow.Cells[2].Controls[0].FindControl("lblDesignUpdates") as Label;
        Label lblDesignUpdates_r = gRow.Cells[2].Controls[1].FindControl("lblDesignUpdates_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkOpenDesignUpdates") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblDesignUpdates.Visible = false;
            lblDesignUpdates_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblDesignUpdates.Visible = true;
            lblDesignUpdates_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }
    protected void lnkOpenSuperintendentNotes_Click(object sender, EventArgs e)
    {

        LinkButton btnsubmit = sender as LinkButton;
      
        GridViewRow gRow = (GridViewRow)btnsubmit.NamingContainer;
        Label lblSuperintendentNotes = gRow.Cells[2].Controls[0].FindControl("lblSuperintendentNotes_r") as Label;
        Label lblSuperintendentNotes_r = gRow.Cells[2].Controls[1].FindControl("lblSuperintendentNotes_r") as Label;
        LinkButton lnkOpen = gRow.Cells[2].Controls[2].FindControl("lnkOpenSuperintendentNotes") as LinkButton;

        if (lnkOpen.Text == "More")
        {
            lblSuperintendentNotes.Visible = false;
            lblSuperintendentNotes_r.Visible = true;
            lnkOpen.Text = " Less";
            lnkOpen.ToolTip = "Click here to view less";
        }
        else
        {
            lblSuperintendentNotes.Visible = true;
            lblSuperintendentNotes_r.Visible = false;
            lnkOpen.Text = "More";
            lnkOpen.ToolTip = "Click here to view more";
        }
    }
    protected void grdProjectNote_Sorting(object sender, GridViewSortEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdProjectNote.ID, grdProjectNote.GetType().Name, "Sorting"); 
        DataTable dtProjectNote = (DataTable)Session["ProjectNote"];


        string strShort = e.SortExpression + " " + hdnOrder.Value;
        DataView dv = dtProjectNote.DefaultView;

        dv.Sort = strShort;

        if (hdnOrder.Value == "ASC")
            hdnOrder.Value = "DESC";
        else
            hdnOrder.Value = "ASC";

        Session["ProjectNote"] = dv.ToTable();

        dtProjectNote = (DataTable)Session["ProjectNote"];

        Session.Add("ProjectNote", dtProjectNote);
        grdProjectNote.DataSource = dtProjectNote;
        grdProjectNote.DataKeyNames = new string[] { "customer_id", "customer_name",  "clientID" };
        grdProjectNote.DataBind();

    }

    protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, ddlSearchBy.ID, ddlSearchBy.GetType().Name, "SelectedIndexChanged"); 
        txtSearch.Text = "";
        wtmFileNumber.WatermarkText = "Search by " + ddlSearchBy.SelectedItem.Text;

        if (ddlSearchBy.SelectedValue == "1")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetFirstName";
        }
        else if (ddlSearchBy.SelectedValue == "2")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetLastName";
        }
        else if (ddlSearchBy.SelectedValue == "4")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetAddress";
        }
        else if (ddlSearchBy.SelectedValue == "3")
        {
            txtSearch_AutoCompleteExtender.ServiceMethod = "GetEmail";
        }
        LoadProjectNoteInfo(0, "");
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        LoadProjectNoteInfo(0, "");;
    }
    protected void grdProjectNote_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        LoadProjectNoteInfo(e.NewPageIndex, "");
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        LoadProjectNoteInfo(nCurrentPage, "");
    }
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);
        LoadProjectNoteInfo(nCurrentPage - 2, "");
    }
    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
       
        LoadProjectNoteInfo(0, "");;
    }
    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
       
        LoadProjectNoteInfo(0, "");;
    }
    protected void lnkViewAll_Click(object sender, EventArgs e)
    {
        txtSearch.Text = "";
        ddlStatus.SelectedValue = "3";
        txtProjectEndDate.Text = "";
        txtProjectStartDate.Text = "";
        ddlDivision.SelectedValue = hdnPrimaryDivision.Value;
      
        LoadProjectNoteInfo(0, "");;

    }
    protected void txtProjectStartDate_TextChanged(object sender, EventArgs e)
    {
        LoadProjectNoteInfo(0, "");;
    }
    protected void txtProjectEndDate_TextChanged(object sender, EventArgs e)
    {
        LoadProjectNoteInfo(0, "");;
    }

  

    protected void btnExpCustList_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnExpCustList.ID, btnExpCustList.GetType().Name, "Click"); 
        DataTable tmpTable = LoadTmpDataTable();
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable tblCustList = (DataTable)Session["ProjectNote"];
        foreach (DataRow dr in tblCustList.Rows)
        {            
            DataRow drNew = tmpTable.NewRow();
           
            drNew["Customer Name"] = dr["customer_name"];
            drNew["Date"] = Convert.ToDateTime(dr["ProjectDate"]).ToShortDateString();
            drNew["Section Name"] = dr["SectionName"];
            drNew["Material Track"] = dr["MaterialTrack"];
            drNew["Design Updates"] = dr["DesignUpdates"];
            drNew["Superintendent Notes"] = dr["SuperintendentNotes"];
            drNew["General Notes"] = dr["NoteDetails"];
          
            if (Convert.ToBoolean(dr["is_complete"]))
            {
                drNew["Completed"] = "Yes";
            }
            else
            {
                drNew["Completed"] = "No";
            }
            if (Convert.ToBoolean(dr["isSOW"]))
            {
                drNew["Include In SOW"] = "Yes";
            }
            else
            {
                drNew["Include In SOW"] = "No";
            }  
           
            tmpTable.Rows.Add(drNew);
        }

        Response.Clear();
        Response.ClearHeaders();

        using (CsvWriter writer = new CsvWriter(Response.OutputStream, ',', System.Text.Encoding.Default))
        {
            writer.WriteAll(tmpTable, true);
        }
        Response.ContentType = "application/vnd.ms-excel";
        Response.AddHeader("Content-Disposition", "attachment; filename=ProjectNotesReport.csv");
        Response.End();
    }
    private DataTable LoadTmpDataTable()
    {
        DataTable table = new DataTable();
        table.Columns.Add("Customer Name", typeof(string));
        table.Columns.Add("Date", typeof(string));
        table.Columns.Add("Section Name", typeof(string));
        table.Columns.Add("Material Track", typeof(string));
        table.Columns.Add("Design Updates", typeof(string));
        table.Columns.Add("Superintendent Notes", typeof(string));
        table.Columns.Add("General Notes", typeof(string));
        table.Columns.Add("Completed", typeof(string));
        table.Columns.Add("Include In SOW", typeof(string));

        return table;
    }
    public static void DynamicQuery()
    {

        List<string> myFilterTargets = new List<string>();

        myFilterTargets.Add("first_name1");

        myFilterTargets.Add("customer_id");



        string myFilter = "Phil";



        DataClassesDataContext _db = new DataClassesDataContext();



        var q = _db.customers.Where(GetFilter(myFilterTargets, myFilter));



        Console.WriteLine("--Query--");

        Console.WriteLine(_db.GetCommand(q).CommandText);

        Console.WriteLine("--Results--");

        foreach (var c in q)
        {

            Console.WriteLine(c.first_name1);

        }

    }

    public static System.Linq.Expressions.Expression<Func<customer, bool>> GetFilter(IEnumerable<string> FilterTargets, string Filter)
    {

        ParameterExpression c = Expression.Parameter(typeof(customer), "c");

        Type[] ContainsTypes = new Type[1];

        ContainsTypes[0] = typeof(string);

        System.Reflection.MethodInfo myContainsInfo = typeof(string).GetMethod("Contains", ContainsTypes);



        List<Expression> myFilterExpressions =

      FilterTargets.Select<string, Expression>(s =>

        Expression.Call(

          Expression.Call(

            Expression.Property(c, typeof(customer).GetProperty(s)),

            "ToString",

            null,

            null

          ),

          myContainsInfo,

          Expression.Constant(Filter)

        )

      ).ToList();



        //build a one-sided tree of Or's from this collection.

        Expression OrExpression = null;

        foreach (Expression myFilterExpression in myFilterExpressions)
        {

            if (OrExpression == null)
            {

                OrExpression = myFilterExpression;

            }

            else
            {

                OrExpression = Expression.Or(myFilterExpression, OrExpression);

            }

        }

        //lambda is where the magic happens.

        Expression<Func<customer, bool>> predicate =

          Expression.Lambda<Func<customer, bool>>(OrExpression, c);



        Console.WriteLine("--predicate--");

        Console.WriteLine(predicate.ToString());



        return predicate;

    }
    private void BindDivision()
    {
        string sql = "select id, division_name from division order by division_name ";
        DataTable dt = csCommonUtility.GetDataTable(sql);
        ddlDivision.DataSource = dt;
        ddlDivision.DataTextField = "division_name";
        ddlDivision.DataValueField = "id";
        ddlDivision.DataBind();
        ddlDivision.Items.Insert(0, "All");
        ddlDivision.SelectedValue = hdnPrimaryDivision.Value;

    }

    protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadProjectNoteInfo(0, "");
    }
}