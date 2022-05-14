using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;


public partial class DocumentManagement : System.Web.UI.Page
{
    [WebMethod]
    public static string[] GetContextMenuData()
    {

        string[] sArry = new string[] { "", "" };
        csCommonUtility.setDMUserData cMenu = (csCommonUtility.setDMUserData)HttpContext.Current.Session["sDMUserData"];

        sArry[0] = cMenu.CustomerId.ToString();
        sArry[1] = cMenu.Role.ToString();

        return sArry;
    }

    [WebMethod]
    public static string GetColPermissionData()
    {


        string cMenu = HttpContext.Current.Session["sPermissionRole"].ToString();
        return cMenu;
    }




    private void CreatePreSetFolder()
    {

        string sFolderPath = ConfigurationManager.AppSettings["DocumentManager_Path"] + hdnCustomerId.Value;

        if (!Directory.Exists(sFolderPath))
            Directory.CreateDirectory(sFolderPath);

        // - IMAGES
        if (!Directory.Exists(sFolderPath + "\\IMAGES"))
        {
            Directory.CreateDirectory(sFolderPath + "\\IMAGES");


        }
        // - ESTIMATES & C/Os
        if (!Directory.Exists(sFolderPath + "\\ESTIMATES & COs"))
            Directory.CreateDirectory(sFolderPath + "\\ESTIMATES & COs");

        //- PERMITS
        if (!Directory.Exists(sFolderPath + "\\PERMITS"))
            Directory.CreateDirectory(sFolderPath + "\\PERMITS");

        //- DESIGN DOCS
        if (!Directory.Exists(sFolderPath + "\\DESIGN DOCS"))
            Directory.CreateDirectory(sFolderPath + "\\DESIGN DOCS");

        //- SITE PROGRESS
        if (!Directory.Exists(sFolderPath + "\\SITE PROGRESS"))
            Directory.CreateDirectory(sFolderPath + "\\SITE PROGRESS");

        //- VENDOR
        if (!Directory.Exists(sFolderPath + "\\VENDOR"))
            Directory.CreateDirectory(sFolderPath + "\\VENDOR");

        //- SELECTIONS
        if (!Directory.Exists(sFolderPath + "\\SELECTIONS"))
            Directory.CreateDirectory(sFolderPath + "\\SELECTIONS");

        //- OTHER
        if (!Directory.Exists(sFolderPath + "\\UPLOAD"))
            Directory.CreateDirectory(sFolderPath + "\\UPLOAD");


        //  //- CHECKS AND PAYMENTS
        if (!Directory.Exists(sFolderPath + "\\CHECKS AND PAYMENTS"))
         Directory.CreateDirectory(sFolderPath + "\\CHECKS AND PAYMENTS");



        //  //- BEFORE PHOTOS
        if (!Directory.Exists(sFolderPath + "\\BEFORE PHOTOS"))
            Directory.CreateDirectory(sFolderPath + "\\BEFORE PHOTOS");

        //  //- Signed Contracts
        if (!Directory.Exists(sFolderPath + "\\SIGNED CONTRACTS"))
            Directory.CreateDirectory(sFolderPath + "\\SIGNED CONTRACTS");

        //  //- Cabinet Layouts
        if (!Directory.Exists(sFolderPath + "\\CABINET LAYOUTS"))
            Directory.CreateDirectory(sFolderPath + "\\CABINET LAYOUTS");

        //  //- Elevations
        if (!Directory.Exists(sFolderPath + "\\ELEVATIONS"))
            Directory.CreateDirectory(sFolderPath + "\\ELEVATIONS");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["oUser"] == null)
        {
            Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
        }
        else
        {
            userinfo oUser = (userinfo)Session["oUser"];
            hdnEmailType.Value = oUser.EmailIntegrationType.ToString();

        }
        if (Request.QueryString.Get("eid") != null)
        {
            hdnEstimateId.Value = Convert.ToInt32(Request.QueryString.Get("eid")).ToString();
        }

        if (Request.QueryString.Get("cid") != null)
        {
            hdnCustomerId.Value = Request.QueryString.Get("cid").ToString();
            hdnBackId.Value = Convert.ToInt32(Request.QueryString.Get("nbackId")).ToString();
            Session.Add("CustomerId", hdnCustomerId.Value);
            DataClassesDataContext _db = new DataClassesDataContext();

            csCommonUtility.setDMUserData objDMU = new csCommonUtility.setDMUserData();

            objDMU.CustomerId = Convert.ToInt32(hdnCustomerId.Value);
            objDMU.Role = "Admin";

            Session.Add("sDMUserData", objDMU); //for Context menu

            //Session.Add("sPermissionRole", "admin"); // for table Column

            customer cust = _db.customers.Single(c => c.customer_id == objDMU.CustomerId);

            lblCustomer.Text = "(" + cust.first_name1 + " " + cust.last_name1 + ")";

            CreatePreSetFolder();


            // elFinder.Connector.Config.AppConnectorConfig.Instance.LocalFSRootDirectoryPath = sFolderPath;
            //   elFinder.Connector.Config.AppConnectorConfig.Instance.CustomerId = Convert.ToInt32(hdnCustomerId.Value);


            // elFinder.Connector.Config.AppConnectorConfig.Instance.BaseUrl = ConfigurationManager.AppSettings["DocumentManager_Base_URL"] + hdnCustomerId.Value;


            //if (Session["sRole"] != null)
            //{
            //    if (Session["sRole"] == "admin")
            //        elFinder.Connector.Config.AppConnectorConfig.Instance.Owner = "Admin";
            //    else if (Session["sRole"] == "customer")
            //        elFinder.Connector.Config.AppConnectorConfig.Instance.Owner = "Customer";
            //    else if (Session["sRole"] == "vendor")
            //        elFinder.Connector.Config.AppConnectorConfig.Instance.Owner = "Vendor";

            //}
        }
    }
    protected void imgBack_Click(object sender, ImageClickEventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, imgBack.ID, imgBack.GetType().Name, "Click"); 
        if (Convert.ToInt32(hdnBackId.Value) > 0)
            Response.Redirect("customerlist.aspx");
        else
            Response.Redirect("leadlist.aspx");
    }

}