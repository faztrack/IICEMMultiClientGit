using Saplin.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class rc_customer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            //if (Session["oUser"] == null)
            //{
            //    Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            //}
            //if (Page.User.IsInRole("sales001") == false)
            //{
            //    // No Permission Page.
            //    Response.Redirect("nopermission.aspx");
            //}

            GetTblColumns();
            string nColmns = "";
            string strQ = string.Empty;
            DataClassesDataContext _db = new DataClassesDataContext();
            tbl_rc_savedcolumn objRC = new tbl_rc_savedcolumn();

            strQ = " SELECT * FROM tbl_rc_savedcolumn WHERE user_name = '" + User.Identity.Name + "'";
            List<csRC_SavedColumn> colmnList = _db.ExecuteQuery<csRC_SavedColumn>(strQ, string.Empty).ToList();

            if (colmnList.Count() > 0)
            {
                nColmns = colmnList.Select(cl => cl.rc_tbl_columns).FirstOrDefault();

                // Set Saved Column Item as 'Selected'------------------------------------------------
                string[] strColmns = nColmns.Split(',');

                for (int i = 0; i < strColmns.Length; i++)
                {
                    foreach (ListItem item in checkBoxes1.Items)
                    {
                        if (strColmns[i].Trim() == item.Value.Trim())
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
            else
            {
                foreach (ListItem item in checkBoxes1.Items)
                {
                    item.Selected = true;
                }
            }


            GetCustomerReports(0, nColmns.Trim().TrimEnd(','));
        }
    }
    private void GetTblColumns()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        List<customer> CustomerList = _db.customers.ToList();
        DropDownCheckBoxes jj = new DropDownCheckBoxes();

        try
        {
            var item = from v in _db.tbl_rc_customers
                       orderby v.rc_column_id
                       select v;
            checkBoxes1.DataSource = item.ToList();
            checkBoxes1.DataTextField = "rc_column_name";
            checkBoxes1.DataValueField = "rc_db_tbl_column";
            checkBoxes1.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void GetCustomerReports(int nPageNo, string nColmns)
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        grdCustomerReport.PageIndex = nPageNo;

        string strQ = string.Empty;

        if (nColmns != "")
        {
            strQ = " SELECT " + nColmns + " FROM customers " +
                            " INNER JOIN lead_source ON customers.lead_source_id = lead_source.lead_source_id " +
                            " LEFT OUTER JOIN sales_person ON  customers.sales_person_id = sales_person.sales_person_id " +
                            " LEFT OUTER JOIN user_info ON customers.SuperintendentId = user_info.user_id";
        }
        else
        {
            strQ = " SELECT first_name1 +' '+ last_name1 as 'CustomerName',customers.address as 'Address',customers.City as 'City', " +
                            " customers.state as 'State', customers.zip_code as 'Zip',customers.phone as 'Phone', customers.mobile as 'Mobile',customers.fax as 'Fax', " +
                            " sales_person.first_name+' '+sales_person.last_name AS SalesPerson,notes as 'Notes',lead_name AS 'LeadSource'," +
                            " customers.email as 'Email', company as 'Company',user_info.first_name+' '+user_info.last_name AS 'Superintendent' " +
                            " FROM customers " +
                            " INNER JOIN lead_source ON customers.lead_source_id = lead_source.lead_source_id " +
                            " LEFT OUTER JOIN sales_person ON  customers.sales_person_id = sales_person.sales_person_id " +
                            " LEFT OUTER JOIN user_info ON customers.SuperintendentId = user_info.user_id";
        }


        List<csRC_Customer> custList = _db.ExecuteQuery<csRC_Customer>(strQ, string.Empty).ToList();

        DataTable tblCustList = SessionInfo.LINQToDataTable(custList);

        tblCustList.Columns.Remove("customer_id");      
        foreach (var column in tblCustList.Columns.Cast<DataColumn>().ToArray())
        {
            if (tblCustList.AsEnumerable().All(dr => dr.IsNull(column)))
                tblCustList.Columns.Remove(column);
        }

        grdCustomerReport.DataSource = tblCustList;
        grdCustomerReport.DataBind();

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

        if (grdCustomerReport.PageCount == nPageNo + 1)
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

    protected void checkBoxes_SelcetedIndexChanged(object sender, EventArgs e)
    {
        string nColmns = "";
        int count = 0;
        foreach (ListItem item in (sender as ListControl).Items)
        {
            if (item.Selected)
            {
                var Text = item.Text;
                var value = item.Value;
                nColmns += item.Value + ", ";
                count += 1;

                lblMessage.Text = "";
            }
        }
        if (count == 0)
        {
            lblMessage.Text = "Please Select Column(s)";
            lblMessage.ForeColor = System.Drawing.Color.Red;
        }
        GetCustomerReports(0, nColmns.Trim().TrimEnd(','));
    }
    protected void grdCustomerReport_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        string nColmns = "";
        GetCustomerReports(e.NewPageIndex, nColmns);
    }
    protected void grdCustomerReport_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        string nColmns = "";
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);

        GetCustomerReports(nCurrentPage - 2, nColmns);
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        string nColmns = "";
        int nCurrentPage = 0;
        nCurrentPage = Convert.ToInt32(lblCurrentPageNo.Text);

        GetCustomerReports(nCurrentPage, nColmns);
    }
    protected void ddlItemPerPage_SelectedIndexChanged(object sender, EventArgs e)
    {
        string nColmns = "";

        GetCustomerReports(0, nColmns);
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSave.ID, btnSave.GetType().Name, "Click"); 
        DataClassesDataContext _db = new DataClassesDataContext();
        string nColmns = "";
        int count = 0;
        foreach (ListItem item in checkBoxes1.Items)
        {
            if (item.Selected)
            {
                var Text = item.Text;
                var value = item.Value;
                nColmns += item.Value + ", ";
                count += 1;

                lblMessage.Text = "";
            }
        }
        if (count == 0)
        {
            lblMessage.Text = "Please Select Column(s)";
            lblMessage.ForeColor = System.Drawing.Color.Red;
            return;
        }

        // Save Column----------------------------------------------------------------------
        tbl_rc_savedcolumn objRC = new tbl_rc_savedcolumn();

        if (_db.tbl_rc_savedcolumns.Where(c => c.user_name == User.Identity.Name).Count() == 0)
        {
            objRC.user_name = User.Identity.Name;
            objRC.rc_tbl_columns = nColmns.Trim().TrimEnd(',');
            _db.tbl_rc_savedcolumns.InsertOnSubmit(objRC);
            _db.SubmitChanges();
        }
        else
        {
            objRC = _db.tbl_rc_savedcolumns.FirstOrDefault(c => c.user_name == User.Identity.Name);
            objRC.rc_tbl_columns = nColmns.Trim().TrimEnd(',');
            _db.SubmitChanges();
        }
    }
}