using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class CustomerDocumentManagement : System.Web.UI.Page
{
   

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["oCustomerUser"] == null)
        {
            Response.Redirect("customerlogin.aspx");
        }
        else
        {
            customeruserinfo obj = (customeruserinfo)Session["oCustomerUser"];
            int nCustomerId = Convert.ToInt32(obj.customerid);
            hdnCustomerId.Value = nCustomerId.ToString();

            csCommonUtility.setDMUserData objDMU = new csCommonUtility.setDMUserData();

            objDMU.CustomerId = Convert.ToInt32(hdnCustomerId.Value);
            objDMU.Role = "Customer";

            Session.Add("sDMUserData", objDMU); //for Context menu

            //Session.Add("sRole", "admin"); //for Context menu

            //Session.Add("sPermissionRole", "admin"); // for table Column

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
        if (!Directory.Exists(sFolderPath + "\\OTHER"))
            Directory.CreateDirectory(sFolderPath + "\\OTHER");

            //elFinder.Connector.Config.AppConnectorConfig.Instance.LocalFSRootDirectoryPath = sFolderPath;

              }
    }

  
}