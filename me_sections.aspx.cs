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
using System.Drawing;

public partial class me_sections : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            KPIUtility.PageLoad(this.Page.AppRelativeVirtualPath);
            if (Session["oUser"] == null)
            {
                Response.Redirect(ConfigurationManager.AppSettings["LoginPage"].ToString());
            }
            if (Request.QueryString.Get("clid") != null)
                hdnClientId.Value = Request.QueryString.Get("clid");
            int nMeId = Convert.ToInt32(Request.QueryString.Get("meid"));
            hdnEstimateId.Value = nMeId.ToString();
            if (Request.QueryString.Get("spid") != null)
            {
                hdnSalesPersonId.Value = Convert.ToInt32(Request.QueryString.Get("spid")).ToString();
            }
            else
            {
                userinfo obj = (userinfo)Session["oUser"];
                hdnSalesPersonId.Value = obj.sales_person_id.ToString();
            }
            DataClassesDataContext _db = new DataClassesDataContext();
            sales_person sp = new sales_person();
            sp = _db.sales_persons.Single(s => s.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value));
            lblSalesPersonName.Text = sp.first_name + " " + sp.last_name;
            lblAddress.Text = sp.address;
            lblPhone.Text = sp.phone;
            lblEmail.Text = sp.email;

            model_estimate me = new model_estimate();
            me = _db.model_estimates.SingleOrDefault(mest => mest.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value) && mest.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mest.client_id == Convert.ToInt32(hdnClientId.Value)); 
            if(me != null)
            {
                lblModelEstimateName.Text = me.model_estimate_name;
            }
            


            chkSections.DataSource = _db.sectioninfos.Where(si => si.client_id == Convert.ToInt32(hdnClientId.Value) && si.parent_id == 0).ToList();
            chkSections.DataTextField = "section_name";
            chkSections.DataValueField = "section_id";
            chkSections.DataBind();

            if (Session["AddMoreMESection"] != null)
            {
                CheckExistingSections(Convert.ToInt32(hdnSalesPersonId.Value), Convert.ToInt32(hdnEstimateId.Value));
            }
            else
            {
                if (_db.model_estimate_sections.Where(mes => mes.sales_person_id == Convert.ToInt32(hdnSalesPersonId.Value) && mes.model_estimate_id == Convert.ToInt32(hdnEstimateId.Value)).ToList().Count > 0)
                {
                    Response.Redirect("me_pricing.aspx?meid=" + hdnEstimateId.Value + "&spid=" + hdnSalesPersonId.Value + "&clid=" + hdnClientId.Value);
                }
            }
        }
    }
    protected void CheckExistingSections(int nSalesPersonId, int nEstimateId)
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        var item = _db.model_estimate_sections.Where(mes => mes.sales_person_id == nSalesPersonId && mes.model_estimate_id == nEstimateId);
        foreach (ListItem li in chkSections.Items)
        {
            foreach (model_estimate_section me_sec in item)
            {
                if (me_sec.section_id == Convert.ToInt32(li.Value.ToString()))
                    li.Selected = true;
            }
        }
    }
    protected void btnContinue_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnContinue.ID, btnContinue.GetType().Name, "Click"); 
        lblMessage.Text = "";

        if (chkSections.SelectedItem == null)
        {
            lblMessage.Text = csCommonUtility.GetSystemErrorMessage("Please select estimate sections.");
         
            return;
        }
        DataClassesDataContext _db = new DataClassesDataContext();
        string strQ = "DELETE model_estimate_sections WHERE sales_person_id =" + Convert.ToInt32(hdnSalesPersonId.Value) + " AND client_id=" + Convert.ToInt32(hdnClientId.Value) + " AND model_estimate_id =" + Convert.ToInt32(hdnEstimateId.Value);
        _db.ExecuteCommand(strQ, string.Empty);
      
        for (int i = 0; i < chkSections.Items.Count; i++)
        {
            if (chkSections.Items[i].Selected == true)
            {
                int nSectionId = Convert.ToInt32(chkSections.Items[i].Value);
                // Add Model Estimate locations
                model_estimate_section me_sec = new model_estimate_section();
                me_sec.client_id = Convert.ToInt32(hdnClientId.Value);
                me_sec.sales_person_id = Convert.ToInt32(hdnSalesPersonId.Value);
                me_sec.section_id = nSectionId;
                me_sec.model_estimate_id = Convert.ToInt32(hdnEstimateId.Value);
               _db.model_estimate_sections.InsertOnSubmit(me_sec);
            }
        }
        _db.SubmitChanges();
        // Redirect to pricing page
        Response.Redirect("me_pricing.aspx?meid=" + hdnEstimateId.Value + "&spid=" + hdnSalesPersonId.Value + "&clid=" + hdnClientId.Value);
    }
    protected void btnBacktoLocations_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnBacktoLocations.ID, btnBacktoLocations.GetType().Name, "Click"); 
        Session.Add("AddMoreMELocation", "AddMoreMElocation");
        Response.Redirect("me_locations.aspx?meid=" + hdnEstimateId.Value + "&spid=" + hdnSalesPersonId.Value + "&clid=" + hdnClientId.Value);
    }
}
