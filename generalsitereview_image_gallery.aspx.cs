using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class generalsitereview_image_gallery : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            lnkClose.Attributes.Add("onClick", "CloseWindow();");
            lnkPrint.Attributes.Add("onClick", "PrintWindow();");

            if (Request.QueryString.Get("gsid") != null)
            {
                int nCustomerId = 0;
                int nEstimateId = 0;
                if (Request.QueryString.Get("cid") != null)
                    nCustomerId = Convert.ToInt32(Request.QueryString.Get("cid"));
                if (Request.QueryString.Get("eid") != null)
                    nEstimateId = Convert.ToInt32(Request.QueryString.Get("eid"));
                int nSiteReviewsId = Convert.ToInt32(Request.QueryString.Get("gsid"));
                hdnCustomerId.Value = nCustomerId.ToString();
                hdnEstimateId.Value = nEstimateId.ToString();
                DataClassesDataContext _db = new DataClassesDataContext();

                customer objCust = new customer();
                objCust = _db.customers.Single(c => c.customer_id == nCustomerId);

                lblCustomerName.Text = objCust.first_name1 + " " + objCust.last_name1;



                lblJobStatusFor.Text = "Image Gallery";




                GetCustomerImageInfo(Convert.ToInt32(hdnCustomerId.Value), nSiteReviewsId);




            }
        }


    }



    protected void grdCustomersImage_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            try
            {

                int SiteReview_attach_id = Convert.ToInt32(grdCustomersImage.DataKeys[e.Row.RowIndex].Values[0].ToString());
                int SiteReviewsId = Convert.ToInt32(grdCustomersImage.DataKeys[e.Row.RowIndex].Values[1].ToString());
                int customer_id = Convert.ToInt32(grdCustomersImage.DataKeys[e.Row.RowIndex].Values[2].ToString());
                int estimate_id = Convert.ToInt32(grdCustomersImage.DataKeys[e.Row.RowIndex].Values[3].ToString());
                string SiteReview_file_name = grdCustomersImage.DataKeys[e.Row.RowIndex].Values[4].ToString();
                string Desccription = grdCustomersImage.DataKeys[e.Row.RowIndex].Values[4].ToString();
              
                HyperLink hypImage = (HyperLink)e.Row.FindControl("hypImage");
                Image img = (Image)e.Row.FindControl("img");



                hypImage.NavigateUrl = "UploadedFiles/" + customer_id + "/IMAGES/" + SiteReview_file_name;
                hypImage.Attributes.Add("data-ilb2-caption", "");
                hypImage.Attributes.Add("data-ilb3-gellarytitle", "Site Photos for: " + lblCustomerName.Text);

                img.ImageUrl = "UploadedFiles/" + customer_id + "/IMAGES/" + SiteReview_file_name;
                img.Attributes.Add("data-zoom-image", "UploadedFiles/" + customer_id + "/IMAGES/" + SiteReview_file_name);

               

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }



    private void GetCustomerImageInfo(int nCustId, int nSiteReviewsId)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();

          

            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                var itemImage = from f in _db.SiteReview_upolad_infos
                                where f.customer_id == nCustId                               
                                && f.SiteReviewsId == nSiteReviewsId
                                && (f.SiteReview_file_name.ToString().ToLower().Contains("jpg") || f.SiteReview_file_name.ToString().ToLower().Contains("png") || f.SiteReview_file_name.ToString().ToLower().Contains("jpeg"))
                                orderby f.SiteReview_attach_id ascending
                                select f;

                //  DataTable dt = csCommonUtility.LINQToDataTable(itemImage);

                //if (itemImage.Count() > 0)
                //{
                //    lblImageGalleryTitle.Visible = true;
                //}

                grdCustomersImage.DataSource = itemImage;
                grdCustomersImage.DataKeyNames = new string[] { "SiteReview_attach_id", "SiteReviewsId", "customer_id", "estimate_id", "SiteReview_file_name" };
                grdCustomersImage.DataBind();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}