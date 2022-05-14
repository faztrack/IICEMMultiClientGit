using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class upload : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        HttpPostedFile uploads = Request.Files["FileData"];
        string strDes = "";
        string filename = "";
        string sCid = "";
        if (Request.QueryString.Get("description") != null)
            strDes = Request.QueryString.Get("description").ToString();
        if (Request.QueryString.Get("filename") != null)
            filename = Request.QueryString.Get("filename").ToString();
        if (Request.QueryString.Get("cid") != null)
            sCid = Request.QueryString.Get("cid").ToString();
        DataClassesDataContext _db = new DataClassesDataContext();
        if (uploads != null)
        {
            string file = System.IO.Path.GetFileName(uploads.FileName);
            string sSavePath = Server.MapPath("Document");
            string custId = Request.QueryString.Get("custId").ToString();
            string strExt="";

            try
            {
                string strFilePath = sSavePath + "\\" + custId;

                if (!System.IO.Directory.Exists(strFilePath))
                {
                    System.IO.Directory.CreateDirectory(strFilePath);
                }
                strExt = file.Substring(file.IndexOf("."));

                if (strExt.ToLower() == ".png" || strExt.ToLower() == ".jpg" || strExt.ToLower() == ".jpeg" || strExt.ToLower() == ".jif" || strExt.ToLower() == ".doc" || strExt.ToLower() == ".docx" || strExt.ToLower() == ".xls" || strExt.ToLower() == ".xlsx" || strExt.ToLower() == ".rtf" || strExt.ToLower() == ".txt" || strExt.ToLower() == ".htm" || strExt.ToLower() == ".html" || strExt.ToLower() == ".pdf")
                {

                    uploads.SaveAs(strFilePath + "\\" + file);
                    if (file.Length > 0)
                    {
                        file_upload_info fui = new file_upload_info();
                        if (_db.file_upload_infos.Where(l => l.client_id == Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]) && l.CustomerId == Convert.ToInt32(custId) && l.ImageName == file.ToString()).SingleOrDefault() == null)
                        {
                            fui.client_id = Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
                            fui.CustomerId = Convert.ToInt32(custId);
                            fui.Desccription = "";
                            fui.ImageName = file.ToString();
                            fui.is_design = false;
                            fui.estimate_id = 0;
                            fui.type = 0;
                            fui.vendor_cost_id = 0;
                            fui.IsSiteProgress = false;
                            fui.dms_dirid = 0;
                            fui.dms_fileid = 0;
                            _db.file_upload_infos.InsertOnSubmit(fui);
                            _db.SubmitChanges();
                        }

                    }
                }

            }
            catch
            {

            }

        }
        else if (strDes.Length > 2)
        {
            string StrQ = "UPDATE file_upload_info SET Desccription='" + strDes + "' WHERE CustomerId=" + Convert.ToInt32(sCid) + " AND ImageName='" + filename + "' AND client_id =" + Convert.ToInt32(ConfigurationManager.AppSettings["client_id"]);
            _db.ExecuteCommand(StrQ, string.Empty);

        }
    }

}
