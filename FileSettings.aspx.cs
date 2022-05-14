using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class FileSettings : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetLastName(String prefixText, Int32 count)
    {
        List<csFilePath> cList = new List<csFilePath>();
        if (HttpContext.Current.Session["ssFilePath"] != null)
        {
            cList = (List<csFilePath>)HttpContext.Current.Session["ssFilePath"];
            return (from c in cList
                    where c.Name.ToLower().StartsWith(prefixText.ToLower())
                    select c.Name).Take<String>(count).ToArray();
        }
        return null;
    }

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
        var fbd = Request.PhysicalApplicationPath + "Document";

        var subdirs = Directory.GetDirectories(fbd)
                            .Select(p => new csFilePath
                            {
                                Path = p,
                                Name = Path.GetFileName(p)
                            })
                            .OrderBy(o=>o.Name).ToList();

        Session.Add("ssFilePath", subdirs);
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 

        DataClassesDataContext _db = new DataClassesDataContext();


        userinfo objuser = (userinfo)Session["oUser"];

        if (objuser.username.ToLower() != "faztrack")
        {
            Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
        }
        int nTotalCategorizeFile = 0;

        string strCondition = "";
        string strConditionFile = "";
        int nCustID = 0;
        int nEstID = 0;

        string strCustId = "";
        string strEstId = "";

        try
        {
            if (txtCustomerID.Text.Trim().Length > 0)
            {
                nCustID = Convert.ToInt32(txtCustomerID.Text.Trim());

                var objCust = from c in _db.customers

                              where c.customer_id == nCustID
                              select new
                              {
                                  client_id = c.client_id,
                                  customer_id = c.customer_id,
                                  first_name1 = c.first_name1,
                                  last_name1 = c.last_name1,
                                  is_active = c.is_active,
                                  registration_date = c.registration_date,
                                  sales_person_id = c.sales_person_id,
                                  update_date = c.update_date,
                                  status_id = c.status_id,
                                  islead = c.islead,
                                  isCustomer = c.isCustomer
                              };

                hyp_DocumentManagement.Text = "Go to Document Management";
                hyp_DocumentManagement.NavigateUrl = "DocumentManagement.aspx?cid=" + nCustID + "&nbackId=5";


                if (objCust != null)
                {
                    grdCustomer.DataSource = objCust.ToList();
                    grdCustomer.DataBind();
                }
            }
            else
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Customer ID is required");
                return;
            }

            if (txtEstimateID.Text.Trim().Length > 0)
                nEstID = Convert.ToInt32(txtEstimateID.Text.Trim());


            if (nCustID > 0)
            {
                strCondition = " WHERE CustomerId = " + nCustID;
                strCustId = " customer_id";
            }

            if (nEstID > 0)
            {
                strCondition = " WHERE CustomerId = " + nCustID + " AND estimate_id =" + nEstID;
                strEstId = ",estimate_id";
            }

            if (nCustID > 0)
            {
                strConditionFile = " CustomerId = " + nCustID + " AND";
            }

            if (nEstID > 0)
            {
                strConditionFile = " CustomerId = " + nCustID + " AND estimate_id =" + nEstID + " AND";
            }



            ////-- All File List
            string sql = "SELECT * " +
                " FROM file_upload_info" +
                " " + strCondition;

            lblFileCheck1.Text = "<b>-- All File List </b><br/><br/>" + sql;

            IEnumerable<file_upload_info> listfile = _db.ExecuteQuery<file_upload_info>(sql, string.Empty);

            grdFileCheck1.DataSource = listfile.ToList();
            grdFileCheck1.DataBind();

            lblFileCheckCount1.Text = "All File List (" + grdFileCheck1.Rows.Count + ")";

            ////--  File List NOT exist on Physical Path (Document folder) 
            sql = "SELECT * " +
               " FROM file_upload_info" +
               " " + strCondition;
            listfile = _db.ExecuteQuery<file_upload_info>(sql, string.Empty);

            lblFileCheck9.Text = "<b>-- File List NOT exist on Physical Path (Document folder)  </b><br/><br/>";
            List<csfile_upload_info> nlFile = new List<csfile_upload_info>();
            foreach (var f in listfile)
            {
                csfile_upload_info objFile = new csfile_upload_info();
                if (f.vendor_cost_id == 0)
                {
                    string sourceFile1 = Request.PhysicalApplicationPath + "Document\\" + f.CustomerId + "\\" + f.ImageName;
                    if (!System.IO.File.Exists(sourceFile1))
                    {
                        objFile.upload_fileId = f.upload_fileId;
                        objFile.CustomerId = (int)f.CustomerId;
                        objFile.Desccription = f.Desccription;
                        objFile.ImageName = f.ImageName;
                        objFile.client_id = (int)f.client_id;
                        objFile.estimate_id = (int)f.estimate_id;
                        objFile.is_design = (bool)f.is_design;
                        objFile.type = (int)f.type;
                        objFile.vendor_cost_id = (int)f.vendor_cost_id;
                        objFile.IsSiteProgress = (bool)f.IsSiteProgress;

                        nlFile.Add(objFile);
                    }
                }

            }

            grdFileCheck9.DataSource = nlFile.ToList();
            grdFileCheck9.DataBind();

            lblFileCheckCount9.Text = "File List NOT exist on Physical Path (Document folder) (" + grdFileCheck9.Rows.Count + ")";


            ////--  File List NOT exist on Physical Path (File folder)
            sql = "SELECT * " +
               " FROM file_upload_info" +
               " " + strCondition;
            listfile = _db.ExecuteQuery<file_upload_info>(sql, string.Empty);

            lblFileCheck9.Text = "<b>-- File List NOT exist on Physical Path (File folder)</b><br/><br/>";
            List<csfile_upload_info> nlFile2 = new List<csfile_upload_info>();
            foreach (var f in listfile)
            {
                csfile_upload_info objFile = new csfile_upload_info();
                if (f.vendor_cost_id != 0)
                {
                    string sourceFile2 = Request.PhysicalApplicationPath + "File\\" + f.ImageName;

                    if (!System.IO.File.Exists(sourceFile2))
                    {
                        objFile.upload_fileId = f.upload_fileId;
                        objFile.CustomerId = (int)f.CustomerId;
                        objFile.Desccription = f.Desccription;
                        objFile.ImageName = f.ImageName;
                        objFile.client_id = (int)f.client_id;
                        objFile.estimate_id = (int)f.estimate_id;
                        objFile.is_design = (bool)f.is_design;
                        objFile.type = (int)f.type;
                        objFile.vendor_cost_id = (int)f.vendor_cost_id;
                        objFile.IsSiteProgress = (bool)f.IsSiteProgress;

                        nlFile2.Add(objFile);
                    }
                }

            }

            grdFileCheck10.DataSource = nlFile2.ToList();
            grdFileCheck10.DataBind();

            lblFileCheckCount10.Text = "File List NOT exist on Physical Path (File folder) (" + grdFileCheck10.Rows.Count + ")";

            ////  ////  ////  ////  ////  ////
            ////-- Vendor
            sql = "Select * from Vendor where vendor_id in " +
                "(" +
                "Select vendor_id from vendor_cost " +
                "where vendor_cost_id in " +
                "(" +
                "Select vendor_cost_id from file_upload_info " + strCondition + ")" +
                ")" +
                "order by vendor_name";

            lblFileCheck2.Text = "<b>-- Vendor<b><br/><br/>" + sql;

            IEnumerable<Vendor> listVendor = _db.ExecuteQuery<Vendor>(sql, string.Empty);

            grdFileCheck2.DataSource = listVendor;
            grdFileCheck2.DataBind();

            lblFileCheckCount2.Text = "Vendor (Table) (" + grdFileCheck2.Rows.Count + ")";

            ////  ////  ////  ////  ////  ////
            ////-- vendor_cost
            sql = "Select * from vendor_cost " +
                    "where vendor_cost_id in " +
                    "(" +
                    "Select vendor_cost_id from file_upload_info " + strCondition + ")";

            lblFileCheck3.Text = "<b>--vendor_cost<b><br/><br/>" + sql;

            IEnumerable<vendor_cost> listvendor_cost = _db.ExecuteQuery<vendor_cost>(sql, string.Empty);

            grdFileCheck3.DataSource = listvendor_cost;
            grdFileCheck3.DataBind();

            lblFileCheckCount3.Text = "vendor_cost (Table) (" + grdFileCheck3.Rows.Count + ")";



            ////  ////  ////  ////  ////  ////
            ////  --is_design File List for Customer 
            sql = "SELECT   *  " +
                    "FROM         file_upload_info " +
                    "WHERE    " + strConditionFile + " (is_design = 1)";

            lblFileCheck4.Text = "<b>--is_design File List for " + nCustID + " <b><br/><br/>" + sql;

            listfile = _db.ExecuteQuery<file_upload_info>(sql, string.Empty);

            grdFileCheck4.DataSource = listfile;
            grdFileCheck4.DataBind();

            lblFileCheckCount4.Text = "DESIGN (" + grdFileCheck4.Rows.Count + ")";

            nTotalCategorizeFile += grdFileCheck4.Rows.Count;

            ////  ////  ////  ////  ////  ////
            ////-- IsSiteProgress  File List for Customer
            sql = "SELECT   *  " +
                    "FROM         file_upload_info " +
                    "WHERE    " + strConditionFile + " (IsSiteProgress = 1)";

            lblFileCheck5.Text = "<b>--IsSiteProgress File List for " + nCustID + " <b><br/><br/>" + sql;

            listfile = _db.ExecuteQuery<file_upload_info>(sql, string.Empty);

            grdFileCheck5.DataSource = listfile;
            grdFileCheck5.DataBind();

            lblFileCheckCount5.Text = "SITE PROGRESS (" + grdFileCheck5.Rows.Count + ")";

            nTotalCategorizeFile += grdFileCheck5.Rows.Count;

            ////  ////  ////  ////  ////  ////
            ////--type = 1 & vendor_cost_id  File List for Customer
            sql = "SELECT   *  " +
                   "FROM         file_upload_info " +
                   "WHERE    " + strConditionFile + " (type = 1) AND ( vendor_cost_id != 0)";

            lblFileCheck6.Text = "<b>--type = 1 & vendor_cost_id  File List for " + nCustID + " <b><br/><br/>" + sql;

            listfile = _db.ExecuteQuery<file_upload_info>(sql, string.Empty);

            grdFileCheck6.DataSource = listfile;
            grdFileCheck6.DataBind();

            lblFileCheckCount6.Text = "VENDOR (" + grdFileCheck6.Rows.Count + ")";

            nTotalCategorizeFile += grdFileCheck6.Rows.Count;

            ////  ////  ////  ////  ////  ////
            ////--type = 5   File List for Customer
            sql = "SELECT   *  " +
                    "FROM         file_upload_info " +
                    "WHERE    " + strConditionFile + " (type = 5)";

            lblFileCheck7.Text = "<b>--type = 5  File List for " + nCustID + " <b><br/><br/>" + sql;

            listfile = _db.ExecuteQuery<file_upload_info>(sql, string.Empty);

            grdFileCheck7.DataSource = listfile;
            grdFileCheck7.DataBind();

            lblFileCheckCount7.Text = "SELECTION (" + grdFileCheck7.Rows.Count + ")";

            nTotalCategorizeFile += grdFileCheck7.Rows.Count;

            ////  ////  ////  ////  ////  ////
            ////--type = 0   File List for Customer
            sql = "SELECT   *  " +
                    "FROM         file_upload_info " +
                    "WHERE    " + strConditionFile + " (type = 0 )AND ( vendor_cost_id = 0) AND (is_design = 0) AND (IsSiteProgress = 0)";

            lblFileCheck8.Text = "<b>--type = 0  File List for " + nCustID + " <b><br/><br/>" + sql;

            listfile = _db.ExecuteQuery<file_upload_info>(sql, string.Empty);

            grdFileCheck8.DataSource = listfile;
            grdFileCheck8.DataBind();

            lblFileCheckCount8.Text = "UPLOAD (" + grdFileCheck8.Rows.Count + ")";

            nTotalCategorizeFile += grdFileCheck8.Rows.Count;

            lblTotalCategorizeFile.Text = nTotalCategorizeFile.ToString();

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.StackTrace);
        }

    }

    public class csfile_upload_info
    {

        public int upload_fileId { get; set; }
        public int CustomerId { get; set; }
        public string Desccription { get; set; }
        public string ImageName { get; set; }
        public int client_id { get; set; }
        public int estimate_id { get; set; }
        public bool is_design { get; set; }
        public int type { get; set; }
        public int vendor_cost_id { get; set; }
        public bool IsSiteProgress { get; set; }

    }

    public class csFilePath
    {
        public string Path { get; set; }
        public string Name { get; set; }
    }
}