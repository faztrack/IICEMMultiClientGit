using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class projectfileupload : System.Web.UI.Page
{
    protected string custId = "0";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["oUser"] == null)
        {
            Response.Redirect("mobile.aspx");
        }
        if (Page.User.IsInRole("cus002") == false)
        {
            // No Permission Page.
            Response.Redirect("mobile.aspx");
        }
        //if (!Request.Browser.IsMobileDevice)
        //{
        //    Response.Redirect("customer_details.aspx");
        //}
        if (Request.QueryString.Get("cid") != null)
        {
            hdnCustomerId.Value = Request.QueryString.Get("cid").ToString();
            lblHearder.Text = "Test";
        }

        if (!IsPostBack)
        {

            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);

            DataClassesDataContext _db = new DataClassesDataContext();
            ListImages();
            dynamicCaptureImageAdd();


        }

    }




    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("mcustomerlist.aspx");
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSubmit.ID, btnSubmit.GetType().Name, "Click");
        lblResult.Text = "";

        DataClassesDataContext _db = new DataClassesDataContext();

        ProjectNoteImage objPrjImg = new ProjectNoteImage();


        string strRequired = string.Empty;



        try
        {
            string originalImageName = string.Empty;
            string imageExtName = string.Empty;
            string savingImageName = string.Empty;
            HttpPostedFile captureImage = imgCapture.PostedFile;
            if (!(captureImage.ContentLength > 0))
            {
                captureImage = imgFileUpload.PostedFile;
            }

            if (captureImage.ContentLength == 0)
            {
                lblResult.Text = "Please select image.<br/>";
                lblResult.ForeColor = System.Drawing.Color.Red;
                ListImages();
                dynamicCaptureImageAdd();
                return;
            }

            if (captureImage.ContentLength > 0)
            {
                originalImageName = captureImage.FileName;
                imageExtName = Path.GetExtension(originalImageName);
                if (originalImageName != "")
                {

                    imageExtName = imageExtName.ToLower();
                    string[] acceptedFileTypes = new string[4];
                    acceptedFileTypes[0] = ".jpg";
                    acceptedFileTypes[1] = ".jpeg";
                    acceptedFileTypes[2] = ".gif";
                    acceptedFileTypes[3] = ".png";
                    bool acceptFile = false;
                    for (int i = 0; i <= 3; i++)
                    {
                        if (imageExtName == acceptedFileTypes[i])
                        {
                            acceptFile = true;
                        }
                    }
                    if (!acceptFile)
                    {
                        strRequired += "Photo should be in jpg, jpeg, gif, png format.<br/>";
                    }
                }
            }
            if (strRequired.Length > 0)
            {
                lblResult.Text = csCommonUtility.GetSystemRequiredMessage(strRequired);
                return;
            }
            string imgFilePath = Server.MapPath("~/images/CaptureImages/");
            savingImageName = DateTime.Now.ToString("ddMMyyhhmmss").Replace("'", "").Replace(" ", "_") + imageExtName;

            string imgUrl = "~/images/CaptureImages/" + savingImageName;

            string outputFileName = Path.Combine(imgFilePath, savingImageName);
            if (!Directory.Exists(imgFilePath))
            {
                Directory.CreateDirectory(imgFilePath);
            }
            if (captureImage.ContentLength > 0)
            {
                System.Drawing.Bitmap bmpImage = new System.Drawing.Bitmap(captureImage.InputStream);
                System.Drawing.Imaging.ImageFormat format = bmpImage.RawFormat;
                int newWidth = 800;
                int newHeight = 500;
                System.Drawing.Bitmap bmpOut = null;
                bmpOut = new System.Drawing.Bitmap(newWidth, newHeight);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmpOut);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.FillRectangle(System.Drawing.Brushes.White, 0, 0, newWidth, newHeight);
                g.DrawImage(bmpImage, 0, 0, newWidth, newHeight);
                bmpImage.Dispose();
               // bmpOut.Save(outputFileName, format);
                bmpOut.Dispose();
                //imgPreview.ImageUrl = imgUrl;

                objPrjImg.ImageName = savingImageName;
                objPrjImg.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                objPrjImg.CreateDate = Convert.ToDateTime(DateTime.Now);
                objPrjImg.CreatedBy = User.Identity.Name;
                objPrjImg.ProjectNoteId = 0;
                _db.ProjectNoteImages.InsertOnSubmit(objPrjImg);
                _db.SubmitChanges();

            }

            lblResult.Text = csCommonUtility.GetSystemMessage("Image uploaded successfully.");

            ListImages();
            dynamicCaptureImageAdd();

        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }



    }
    private void ListImages()
    {
        //DirectoryInfo dir = new DirectoryInfo(MapPath("~/images/CaptureImages"));
        //FileInfo[] file = dir.GetFiles();
        //ArrayList list = new ArrayList();
        //foreach (FileInfo file2 in file)
        //{
        //    if (file2.Extension == ".jpg" || file2.Extension == ".jpeg" || file2.Extension == ".gif" || file2.Extension == ".png")
        //    {
        //        list.Add(file2);
        //    }
        //}


        DataClassesDataContext _db = new DataClassesDataContext();


        var mList = (from m in _db.ProjectNoteImages where m.customer_id == Convert.ToInt32(hdnCustomerId.Value) select m).ToList();

        if (mList.Count > 0)
        {
            DataTable dtPro = csCommonUtility.LINQToDataTable(mList);
            Session.Add("sImageList", dtPro);
        }
        else
        {
            Session["sImageList"] = null;
        }




        //Session.Add("sImageList", list);
        // dataListCaptureImage.DataSource = list;
        // dataListCaptureImage.DataBind();
    }

    protected void dynamicCaptureImageAdd()
    {

        try
        {



            if (Session["sImageList"] != null)
            {
                // var List = (List<string>)Session["sImageList"];
                DataTable dtPro = (DataTable)Session["sImageList"];

                ContentPlaceHolder content = (ContentPlaceHolder)this.Master.FindControl("head");

                HtmlGenericControl divRowMain = new HtmlGenericControl("div");
                divRowMain.Attributes["class"] = "row";

                int j = 0;

                foreach (DataRow dr in dtPro.Rows)
                {



                    HtmlGenericControl divColumncol = new HtmlGenericControl("div");
                    divColumncol.Attributes["class"] = "col-xs-6 col-sm-4 col-md-3 col-lg-2";

                    HtmlGenericControl divRowItem = new HtmlGenericControl("div");
                    divRowItem.Attributes["class"] = "row ";

                    //----------------- Image----------------------------
                    #region  Image
                    HtmlGenericControl divColumnImg = new HtmlGenericControl("div");
                    divColumnImg.Attributes["class"] = "col-xs-12 col-sm-12 col-md-12 col-lg-12";



                    HtmlGenericControl AnchorColumnImg = new HtmlGenericControl("a");
                    // AnchorColumnImg.Attributes["href"] = "productlist.aspx?brand=" + dr["Name"].ToString();






                    Image img = new Image();






                    img.Attributes["data-big"] = @"images/CaptureImages/" + dr["MobileImageName"].ToString();

                    img.ImageUrl = @"images/CaptureImages/" + dr["MobileImageName"].ToString();
                    img.Visible = true;
                    img.CssClass = "fancybox img-responsive center-block";


                    #endregion




                    //-----------------Grid Creating Start ----------------------------

                    ////---- Adding 4 columns in a row

                    divRowMain.Controls.Add(divColumncol);

                    divColumncol.Controls.Add(divRowItem);

                    divRowItem.Controls.Add(divColumnImg);


                    divColumnImg.Controls.Add(AnchorColumnImg);
                    AnchorColumnImg.Controls.Add(img);




                    ////---- Adding row in a div
                    j++;
                    if (j == 6)
                    {
                        divImageCapture.Controls.Add(divRowMain);
                        j = 0;
                        divRowMain = new HtmlGenericControl("div");
                        divRowMain.Attributes["class"] = "row";
                    }
                }

                ////---- Adding odd number row in a div
                if (j > 0)
                {
                    divImageCapture.Controls.Add(divRowMain);
                }
            }

        }

        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }




}