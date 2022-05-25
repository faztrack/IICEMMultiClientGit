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
using System.Collections.Generic;

public partial class me_estimate_list : System.Web.UI.Page
{
    int nSalesPersonId = 0;
    int nUserId = 6;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["oUser"] == null)
        {
            Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
        }
        else
        {
            hdnClientId.Value = ((userinfo)Session["oUser"]).client_id.ToString();
        }
        userinfo obj = (userinfo)Session["oUser"];
        nSalesPersonId = obj.sales_person_id;
        nUserId = obj.user_id;
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            Session.Remove("CustomerId");

            MeEstimate();
            if (nUserId != 6)
            {
                tdlabelTitleGrn.Visible = true;
                lblTemplate1.Text = "Your Templates";
                lblEstimate2.Visible = true;
                grdPublicEstimationList.Visible = true;
                MeEstimatePublic();
                trPublicSearchAdd.Visible = true;
            }
            else
            {
                trPublicSearchAdd.Visible = false;
                tdlabelTitleGrn.Visible = false;
                lblTemplate1.Text = "All Templates";
                lblEstimate2.Visible = false;
                grdPublicEstimationList.Visible = false;
            }

           

        }
    }
    protected void MeEstimate()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        List<EstimateTemplateModel> EstTemp = new List<EstimateTemplateModel>();

        if (nSalesPersonId == 0)
        {
            var item = from me in _db.model_estimates
                       join sp in _db.sales_persons on me.sales_person_id equals sp.sales_person_id
                       where me.client_id.ToString().Contains(hdnClientId.Value)
                       orderby me.sales_person_id, me.model_estimate_id, me.model_estimate_name
                       select new EstimateTemplateModel()
                                       {
                                           template_id = (int)me.template_id,
                                           model_estimate_id = (int)me.model_estimate_id,
                                           sales_person_id = (int)me.sales_person_id,
                                           client_id = (int)me.client_id,
                                           status_id = (int)me.status_id,
                                           model_estimate_name = me.model_estimate_name,
                                           create_date = (DateTime)me.create_date,
                                           last_update_date = (DateTime)me.last_udated_date,
                                           estimate_comments = me.estimate_comments,
                                           sales_person_name = sp.first_name + " " + sp.last_name,
                                       };

            if (txtSearch.Text.Trim() != "")
            {
                string str = txtSearch.Text.Trim();
                item = from me in _db.model_estimates
                       join sp in _db.sales_persons on me.sales_person_id equals sp.sales_person_id
                       where me.client_id.ToString().Contains(hdnClientId.Value) && me.model_estimate_name.Contains(str)
                       orderby me.sales_person_id, me.model_estimate_id, me.model_estimate_name
                       select new EstimateTemplateModel()
                       {
                           template_id = (int)me.template_id,
                           model_estimate_id = (int)me.model_estimate_id,
                           sales_person_id = (int)me.sales_person_id,
                           client_id = (int)me.client_id,
                           status_id = (int)me.status_id,
                           model_estimate_name = me.model_estimate_name,
                           create_date = (DateTime)me.create_date,
                           last_update_date = (DateTime)me.last_udated_date,
                           estimate_comments = me.estimate_comments,
                           sales_person_name = sp.first_name + " " + sp.last_name,
                       };
            }
            EstTemp = item.ToList();
        }
        else
        {
            var item = from me in _db.model_estimates
                       join sp in _db.sales_persons on me.sales_person_id equals sp.sales_person_id
                       where me.client_id.ToString().Contains(hdnClientId.Value) && me.sales_person_id == nSalesPersonId
                       orderby me.sales_person_id, me.model_estimate_id, me.model_estimate_name
                       select new EstimateTemplateModel()
                       {
                           template_id = (int)me.template_id,
                           model_estimate_id = (int)me.model_estimate_id,
                           sales_person_id = (int)me.sales_person_id,
                           client_id = (int)me.client_id,
                           status_id = (int)me.status_id,
                           model_estimate_name = me.model_estimate_name,
                           create_date = (DateTime)me.create_date,
                           last_update_date = (DateTime)me.last_udated_date,
                           estimate_comments = me.estimate_comments,
                           sales_person_name = sp.first_name + " " + sp.last_name,
                       };

            if (txtSearch.Text.Trim() != "")
            {
                string str = txtSearch.Text.Trim();
                item = from me in _db.model_estimates
                       join sp in _db.sales_persons on me.sales_person_id equals sp.sales_person_id
                       where me.client_id.ToString().Contains(hdnClientId.Value) && me.model_estimate_name.Contains(str) && me.sales_person_id == nSalesPersonId
                       orderby me.sales_person_id, me.model_estimate_id, me.model_estimate_name
                       select new EstimateTemplateModel()
                       {
                           template_id = (int)me.template_id,
                           model_estimate_id = (int)me.model_estimate_id,
                           sales_person_id = (int)me.sales_person_id,
                           client_id = (int)me.client_id,
                           status_id = (int)me.status_id,
                           model_estimate_name = me.model_estimate_name,
                           create_date = (DateTime)me.create_date,
                           last_update_date = (DateTime)me.last_udated_date,
                           estimate_comments = me.estimate_comments,
                           sales_person_name = sp.first_name + " " + sp.last_name,
                       };
            }
            EstTemp = item.ToList();

        }


        grdEstimationList.DataSource = EstTemp;
        grdEstimationList.DataKeyNames = new string[] { "template_id", "model_estimate_id", "sales_person_id", "model_estimate_name" };
        grdEstimationList.DataBind();

        if (EstTemp.Count > 0)
        {
            tdlabelTitleBlu.Visible = true;
            trSearch.Visible = true;
        }
        else
        {
            tdlabelTitleBlu.Visible = false;
            trSearch.Visible = false;
        }

    }

    protected void MeEstimatePublic()
    {
        DataClassesDataContext _db = new DataClassesDataContext();
        List<EstimateTemplateModel> PublicEstTemp = new List<EstimateTemplateModel>();

        var item = from me in _db.model_estimates
                   join sp in _db.sales_persons on me.sales_person_id equals sp.sales_person_id
                   where me.client_id.ToString().Contains(hdnClientId.Value) && me.sales_person_id != nSalesPersonId && me.IsPublic == true
                   orderby me.sales_person_id, me.model_estimate_id, me.model_estimate_name
                   select new EstimateTemplateModel()
                   {
                       template_id = (int)me.template_id,
                       model_estimate_id = (int)me.model_estimate_id,
                       sales_person_id = (int)me.sales_person_id,
                       client_id = (int)me.client_id,
                       status_id = (int)me.status_id,
                       model_estimate_name = me.model_estimate_name,
                       create_date = (DateTime)me.create_date,
                       last_update_date = (DateTime)me.last_udated_date,
                       estimate_comments = me.estimate_comments,
                       sales_person_name = sp.first_name + " " + sp.last_name,
                   };

        if (txtPublicSearch.Text.Trim() != "")
        {
            string str = txtPublicSearch.Text.Trim();
            item = from me in _db.model_estimates
                   join sp in _db.sales_persons on me.sales_person_id equals sp.sales_person_id
                   where me.client_id.ToString().Contains(hdnClientId.Value) && me.model_estimate_name.Contains(str) && me.sales_person_id != nSalesPersonId && me.IsPublic == true
                   orderby me.sales_person_id, me.model_estimate_id, me.model_estimate_name
                   select new EstimateTemplateModel()
                   {
                       template_id = (int)me.template_id,
                       model_estimate_id = (int)me.model_estimate_id,
                       sales_person_id = (int)me.sales_person_id,
                       client_id = (int)me.client_id,
                       status_id = (int)me.status_id,
                       model_estimate_name = me.model_estimate_name,
                       create_date = (DateTime)me.create_date,
                       last_update_date = (DateTime)me.last_udated_date,
                       estimate_comments = me.estimate_comments,
                       sales_person_name = sp.first_name + " " + sp.last_name,
                   };
        }
        PublicEstTemp = item.ToList();


        grdPublicEstimationList.DataSource = PublicEstTemp;
        grdPublicEstimationList.DataKeyNames = new string[] { "template_id", "model_estimate_id", "sales_person_id", "model_estimate_name" };
        grdPublicEstimationList.DataBind();

        if (PublicEstTemp.Count > 0)
        {
            tdlabelTitleGrn.Visible = true;
            userinfo obj = (userinfo)Session["oUser"];
            if (obj.role_id != 1)
            {
                btnPublicDelete.Visible = false;
                grdPublicEstimationList.Columns[3].Visible = false;
               // btnAddNew.Visible = false;
            }
            else
            {
                btnPublicDelete.Visible = true;
                grdPublicEstimationList.Columns[3].Visible = true;
               // btnAddNew.Visible = true;
            }
        }
        else
        {
            tdlabelTitleGrn.Visible = false;
        }

    }
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        Response.Redirect("me_locations.aspx");
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnSearch.ID, btnSearch.GetType().Name, "Click"); 
        MeEstimate();
    }

    protected void btnPublicSearch_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPublicSearch.ID, btnPublicSearch.GetType().Name, "Click"); 
        MeEstimatePublic();
    }

   

    protected void grdEstimationList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nModel_estimate_id = Convert.ToInt32(grdEstimationList.DataKeys[e.Row.RowIndex].Values[1].ToString());
            int nsales_person_id = Convert.ToInt32(grdEstimationList.DataKeys[e.Row.RowIndex].Values[2].ToString());
            string strEstName = grdEstimationList.DataKeys[e.Row.RowIndex].Values[3].ToString();
            HyperLink hypEstName = (HyperLink)e.Row.Cells[0].FindControl("hypEstName");
            hypEstName.Text = strEstName;
            hypEstName.NavigateUrl = "me_locations.aspx?meid=" + nModel_estimate_id + "&spid=" + nsales_person_id;

        }

    }
    protected void grdPublicEstimationList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nModel_estimate_id = Convert.ToInt32(grdPublicEstimationList.DataKeys[e.Row.RowIndex].Values[1].ToString());
            int nsales_person_id = Convert.ToInt32(grdPublicEstimationList.DataKeys[e.Row.RowIndex].Values[2].ToString());
            string strEstName = grdPublicEstimationList.DataKeys[e.Row.RowIndex].Values[3].ToString();
            HyperLink hypEstName1 = (HyperLink)e.Row.Cells[0].FindControl("hypEstName1");
            hypEstName1.Text = strEstName;
            hypEstName1.NavigateUrl = "PublicMe_Pricing.aspx?meid=" + nModel_estimate_id + "&spid=" + nsales_person_id;

        }

    }

    protected void btnPublicDelete_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnPublicDelete.ID, btnPublicDelete.GetType().Name, "Click"); 
        lblResult.Text = "";
        bool isChecked = false;
        DataClassesDataContext _db = new DataClassesDataContext();

        string strQ = string.Empty;

        try
        {           
            foreach (GridViewRow row in grdPublicEstimationList.Rows)
            {
                CheckBox chkPublicDelete = (CheckBox)row.FindControl("chkPublicDelete");

                int nTemplateId = Convert.ToInt32(grdPublicEstimationList.DataKeys[row.RowIndex].Values[0].ToString());
                int nModelEstimateId = Convert.ToInt32(grdPublicEstimationList.DataKeys[row.RowIndex].Values[1].ToString());
                int nSalesPersonId = Convert.ToInt32(grdPublicEstimationList.DataKeys[row.RowIndex].Values[2].ToString());


                if (chkPublicDelete.Checked)
                {
                    isChecked = true;
                    strQ = "Delete FROM model_estimate  WHERE template_id = " + nTemplateId + " AND sales_person_id = " + nSalesPersonId + " AND model_estimate_id = " + nModelEstimateId;
                    _db.ExecuteCommand(strQ, string.Empty);

                    strQ = "DELETE FROM model_estimate_pricing WHERE sales_person_id = " + nSalesPersonId + " AND model_estimate_id = " + nModelEstimateId;
                    _db.ExecuteCommand(strQ, string.Empty);

                    strQ = "DELETE FROM model_estimate_locations WHERE sales_person_id = " + nSalesPersonId + " AND model_estimate_id = " + nModelEstimateId;
                    _db.ExecuteCommand(strQ, string.Empty);

                    strQ = "DELETE FROM model_estimate_sections WHERE sales_person_id = " + nSalesPersonId + " AND model_estimate_id = " + nModelEstimateId;
                    _db.ExecuteCommand(strQ, string.Empty);
                    strQ = "DELETE FROM model_estimate_payments WHERE sales_person_id = " + nSalesPersonId + " AND estimate_id = " + nModelEstimateId;
                    _db.ExecuteCommand(strQ, string.Empty);
                }


            }
            if (isChecked)
            {
                lblResult.Text = csCommonUtility.GetSystemMessage("Estimation Template(s) has been Deleted Successfully");
                MeEstimatePublic();
            }
            else
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please Select Estimation Template(s)");
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnDelete.ID, btnDelete.GetType().Name, "Click"); 
        lblResult.Text = "";
        bool isChecked = false;
        DataClassesDataContext _db = new DataClassesDataContext();

        string strQ = string.Empty;

        try
        {
            foreach (GridViewRow row in grdEstimationList.Rows)
            {
                CheckBox chkDelete = (CheckBox)row.FindControl("chkDelete");

                int nTemplateId = Convert.ToInt32(grdEstimationList.DataKeys[row.RowIndex].Values[0].ToString());
                int nModelEstimateId = Convert.ToInt32(grdEstimationList.DataKeys[row.RowIndex].Values[1].ToString());
                int nSalesPersonId = Convert.ToInt32(grdEstimationList.DataKeys[row.RowIndex].Values[2].ToString());


                if (chkDelete.Checked)
                {
                    isChecked = true;
                    strQ = "Delete FROM model_estimate  WHERE template_id = " + nTemplateId + " AND sales_person_id = " + nSalesPersonId + " AND model_estimate_id = " + nModelEstimateId;
                    _db.ExecuteCommand(strQ, string.Empty);

                    strQ = "DELETE FROM model_estimate_pricing WHERE sales_person_id = " + nSalesPersonId + " AND model_estimate_id = " + nModelEstimateId;
                    _db.ExecuteCommand(strQ, string.Empty);

                    strQ = "DELETE FROM model_estimate_locations WHERE sales_person_id = " + nSalesPersonId + " AND model_estimate_id = " + nModelEstimateId;
                    _db.ExecuteCommand(strQ, string.Empty);

                    strQ = "DELETE FROM model_estimate_sections WHERE sales_person_id = " + nSalesPersonId + " AND model_estimate_id = " + nModelEstimateId;
                    _db.ExecuteCommand(strQ, string.Empty);
                    strQ = "DELETE FROM model_estimate_payments WHERE sales_person_id = " + nSalesPersonId + " AND estimate_id = " + nModelEstimateId;
                    _db.ExecuteCommand(strQ, string.Empty);

                }


            }
            if (isChecked)
            {
                lblResult.Text = csCommonUtility.GetSystemMessage("Estimation Template(s) has been Deleted Successfully");
                MeEstimate();
            }
            else
            {
                lblResult.Text = csCommonUtility.GetSystemErrorMessage("Please Select Estimation Template(s)");
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }
}
