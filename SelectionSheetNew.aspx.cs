using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class SelectionSheetNew : System.Web.UI.Page
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
           
            DataClassesDataContext _db = new DataClassesDataContext();
            int nEstimateId = Convert.ToInt32(Request.QueryString.Get("eid"));
            hdnEstimateId.Value = nEstimateId.ToString();
            int nCustomerId = Convert.ToInt32(Request.QueryString.Get("cid"));
            hdnCustomerId.Value = nCustomerId.ToString();

            if (Convert.ToInt32(hdnCustomerId.Value) > 0)
            {
                customer cust = new customer();
                cust = _db.customers.Single(c => c.customer_id == Convert.ToInt32(hdnCustomerId.Value));
                lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1;
                // Get Estimate Info
                if (Convert.ToInt32(hdnEstimateId.Value) > 0)
                {
                    customer_estimate cus_est = new customer_estimate();
                    cus_est = _db.customer_estimates.Single(ce => ce.customer_id == Convert.ToInt32(hdnCustomerId.Value) && ce.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
                    lblEstimateName.Text = cus_est.estimate_name;
                    lblCustomerName.Text = cust.first_name1 + " " + cust.last_name1 + " (" + cus_est.job_number+")";

                }
            }
            
            // LoadSelectionMaster();

            GetCabinetSheet();
            GetBathroomSheet();
            GetKitchen2Sheet();
            GetKitchenTile();
            GetTubTile();
            GetShowerTile();
            //Kitchen/Shower/Tub
            // loadKitchenSheetSelection();
            //loadShowerSheetSelection();
            // loadTUBSheetSelection();

        }
    }

    #region Cabinet
    protected void GetCabinetSheet()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable tmpTable = LoadSectionTable();

        var objCabSSList = _db.CabinetSheetSelections.Where(cp => cp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cp.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).ToList();

        foreach (CabinetSheetSelection Cabinfo in objCabSSList)
        {

            DataRow drNew = tmpTable.NewRow();

            drNew["CabinetSheetID"] = Cabinfo.CabinetSheetID;
            drNew["customer_id"] = Cabinfo.customer_id;
            drNew["estimate_id"] = Cabinfo.estimate_id;
            drNew["CabinetSheetName"] = Cabinfo.CabinetSheetName;
            drNew["UpperWallDoor"] = Cabinfo.UpperWallDoor;
            drNew["UpperWallWood"] = Cabinfo.UpperWallWood;
            drNew["UpperWallStain"] = Cabinfo.UpperWallStain;
            drNew["UpperWallExterior"] = Cabinfo.UpperWallExterior;
            drNew["UpperWallInterior"] = Cabinfo.UpperWallInterior;
            drNew["UpperWallOther"] = Cabinfo.UpperWallOther;
            drNew["BaseDoor"] = Cabinfo.BaseDoor;
            drNew["BaseWood"] = Cabinfo.BaseWood;
            drNew["BaseStain"] = Cabinfo.BaseStain;
            drNew["BaseExterior"] = Cabinfo.BaseExterior;
            drNew["BaseInterior"] = Cabinfo.BaseInterior;
            drNew["BaseOther"] = Cabinfo.BaseOther;
            drNew["MiscDoor"] = Cabinfo.MiscDoor;
            drNew["MiscWood"] = Cabinfo.MiscWood;
            drNew["MiscStain"] = Cabinfo.MiscStain;
            drNew["MiscExterior"] = Cabinfo.MiscExterior;
            drNew["MiscInterior"] = Cabinfo.MiscInterior;
            drNew["MiscOther"] = Cabinfo.MiscOther;
            drNew["LastUpdateDate"] = Cabinfo.LastUpdateDate;
            drNew["UpdateBy"] = Cabinfo.UpdateBy;

            tmpTable.Rows.Add(drNew);
        }

        if (objCabSSList.Count() == 0)
        {
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["CabinetSheetID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["CabinetSheetName"] = "";
            drNew1["UpperWallDoor"] = "";
            drNew1["UpperWallWood"] = "";
            drNew1["UpperWallStain"] = "";
            drNew1["UpperWallExterior"] = "";
            drNew1["UpperWallInterior"] = "";
            drNew1["UpperWallOther"] = "";
            drNew1["BaseDoor"] = "";
            drNew1["BaseWood"] = "";
            drNew1["BaseStain"] = "";
            drNew1["BaseExterior"] = "";
            drNew1["BaseInterior"] = "";
            drNew1["BaseOther"] = "";
            drNew1["MiscDoor"] = "";
            drNew1["MiscWood"] = "";
            drNew1["MiscStain"] = "";
            drNew1["MiscExterior"] = "";
            drNew1["MiscInterior"] = "";
            drNew1["MiscOther"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
        }

        Session.Add("sCabinetSection", tmpTable);

        grdCabinetSelectionSheet.DataSource = tmpTable;
        grdCabinetSelectionSheet.DataKeyNames = new string[] { "CabinetSheetID", "customer_id", "estimate_id" };
        grdCabinetSelectionSheet.DataBind();



    }

    protected void grdCabinetSelectionSheet_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nCabinetSheetID = Convert.ToInt32(grdCabinetSelectionSheet.DataKeys[e.Row.RowIndex].Values[0].ToString());

            LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");

            lnkDelete.Attributes["CommandArgument"] = string.Format("{0}", nCabinetSheetID);

            if (nCabinetSheetID > 0)
                lnkDelete.Visible = true;

        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, grdCabinetSelectionSheet.ID, grdCabinetSelectionSheet.GetType().Name, "Click"); 
        lblResult.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        string strRequired = string.Empty;
        try
        {
            DataTable table = (DataTable)Session["sCabinetSection"];



            foreach (GridViewRow row in grdCabinetSelectionSheet.Rows)
            {
                TextBox txtCabinetSheetName = (TextBox)row.FindControl("txtCabinetSheetName");

                TextBox txtUpperWallDoor = (TextBox)row.FindControl("txtUpperWallDoor");
                TextBox txtUpperWallWood = (TextBox)row.FindControl("txtUpperWallWood");
                TextBox txtUpperWallStain = (TextBox)row.FindControl("txtUpperWallStain");
                TextBox txtUpperWallExterior = (TextBox)row.FindControl("txtUpperWallExterior");
                TextBox txtUpperWallInterior = (TextBox)row.FindControl("txtUpperWallInterior");
                TextBox txtUpperWallOther = (TextBox)row.FindControl("txtUpperWallOther");

                TextBox txtBaseDoor = (TextBox)row.FindControl("txtBaseDoor");
                TextBox txtBaseWood = (TextBox)row.FindControl("txtBaseWood");
                TextBox txtBaseStain = (TextBox)row.FindControl("txtBaseStain");
                TextBox txtBaseExterior = (TextBox)row.FindControl("txtBaseExterior");
                TextBox txtBaseInterior = (TextBox)row.FindControl("txtBaseInterior");
                TextBox txtBaseOther = (TextBox)row.FindControl("txtBaseOther");

                TextBox txtMiscDoor = (TextBox)row.FindControl("txtMiscDoor");
                TextBox txtMiscWood = (TextBox)row.FindControl("txtMiscWood");
                TextBox txtMiscStain = (TextBox)row.FindControl("txtMiscStain");
                TextBox txtMiscExterior = (TextBox)row.FindControl("txtMiscExterior");
                TextBox txtMiscInterior = (TextBox)row.FindControl("txtMiscInterior");
                TextBox txtMiscOther = (TextBox)row.FindControl("txtMiscOther");

                Label lblCabinetSheetName = (Label)row.FindControl("lblCabinetSheetName");

                Label lblUpperWallDoor = (Label)row.FindControl("lblUpperWallDoor");
                Label lblUpperWallWood = (Label)row.FindControl("lblUpperWallWood");
                Label lblUpperWallStain = (Label)row.FindControl("lblUpperWallStain");
                Label lblUpperWallExterior = (Label)row.FindControl("lblUpperWallExterior");
                Label lblUpperWallInterior = (Label)row.FindControl("lblUpperWallInterior");
                Label lblUpperWallOther = (Label)row.FindControl("lblUpperWallOther");

                Label lblBaseDoor = (Label)row.FindControl("lblBaseDoor");
                Label lblBaseWood = (Label)row.FindControl("lblBaseWood");
                Label lblBaseStain = (Label)row.FindControl("lblBaseStain");
                Label lblBaseExterior = (Label)row.FindControl("lblBaseExterior");
                Label lblBaseInterior = (Label)row.FindControl("lblBaseInterior");
                Label lblBaseOther = (Label)row.FindControl("lblBaseOther");

                Label lblMiscDoor = (Label)row.FindControl("lblMiscDoor");
                Label lblMiscWood = (Label)row.FindControl("lblMiscWood");
                Label lblMiscStain = (Label)row.FindControl("lblMiscStain");
                Label lblMiscExterior = (Label)row.FindControl("lblMiscExterior");
                Label lblMiscInterior = (Label)row.FindControl("lblMiscInterior");
                Label lblMiscOther = (Label)row.FindControl("lblMiscOther");

                if (txtCabinetSheetName.Text.Trim() == "")
                {
                    lblCabinetSheetName.Text = csCommonUtility.GetSystemRequiredMessage2("Title/Location of Cabinet is required.");
                    strRequired = "required";
                }
                else
                    lblCabinetSheetName.Text = "";

                if (txtUpperWallDoor.Text.Trim() == "")
                {
                    lblUpperWallDoor.Text = csCommonUtility.GetSystemRequiredMessage2("Upper Wall Door is required.");
                    strRequired = "required";
                }
                else
                    lblUpperWallDoor.Text = "";

                if (txtUpperWallWood.Text.Trim() == "")
                {
                    lblUpperWallWood.Text = csCommonUtility.GetSystemRequiredMessage2("Upper Wall Wood is required.");
                    strRequired = "required";
                }
                else
                    lblUpperWallWood.Text = "";

                if (txtUpperWallStain.Text.Trim() == "")
                {
                    lblUpperWallStain.Text = csCommonUtility.GetSystemRequiredMessage2("Upper Wall Stain is required.");
                    strRequired = "required";
                }
                else
                    lblUpperWallStain.Text = "";

                if (txtUpperWallExterior.Text.Trim() == "")
                {
                    lblUpperWallExterior.Text = csCommonUtility.GetSystemRequiredMessage2("Upper Wall Exterior is required.");
                    strRequired = "required";
                }
                else
                    lblUpperWallExterior.Text = "";

                if (txtUpperWallInterior.Text.Trim() == "")
                {
                    lblUpperWallInterior.Text = csCommonUtility.GetSystemRequiredMessage2("Upper Wall Interior is required.");
                    strRequired = "required";
                }
                else
                    lblUpperWallInterior.Text = "";




                if (txtBaseDoor.Text.Trim() == "")
                {
                    lblBaseDoor.Text = csCommonUtility.GetSystemRequiredMessage2("Base Door is required.");
                    strRequired = "required";
                }
                else
                    lblBaseDoor.Text = "";

                if (txtBaseWood.Text.Trim() == "")
                {
                    lblBaseWood.Text = csCommonUtility.GetSystemRequiredMessage2("Base Wood is required.");
                    strRequired = "required";
                }
                else
                    lblBaseWood.Text = "";

                if (txtBaseStain.Text.Trim() == "")
                {
                    lblBaseStain.Text = csCommonUtility.GetSystemRequiredMessage2("Base Stain is required.");
                    strRequired = "required";
                }
                else
                    lblBaseStain.Text = "";

                if (txtBaseExterior.Text.Trim() == "")
                {
                    lblBaseExterior.Text = csCommonUtility.GetSystemRequiredMessage2("Base Exterior is required.");
                    strRequired = "required";
                }
                else
                    lblBaseExterior.Text = "";

                if (txtBaseInterior.Text.Trim() == "")
                {
                    lblBaseInterior.Text = csCommonUtility.GetSystemRequiredMessage2("Base Interior is required.");
                    strRequired = "required";
                }
                else
                    lblBaseInterior.Text = "";


                if (txtMiscDoor.Text.Trim() == "")
                {
                    lblMiscDoor.Text = csCommonUtility.GetSystemRequiredMessage2("Misc Door is required.");
                    strRequired = "required";
                }
                else
                    lblMiscDoor.Text = "";

                if (txtMiscWood.Text.Trim() == "")
                {
                    lblMiscWood.Text = csCommonUtility.GetSystemRequiredMessage2("Misc Wood is required.");
                    strRequired = "required";
                }
                else
                    lblMiscWood.Text = "";

                if (txtMiscStain.Text.Trim() == "")
                {
                    lblMiscStain.Text = csCommonUtility.GetSystemRequiredMessage2("Misc Stain is required.");
                    strRequired = "required";
                }
                else
                    lblMiscStain.Text = "";

                if (txtMiscExterior.Text.Trim() == "")
                {
                    lblMiscExterior.Text = csCommonUtility.GetSystemRequiredMessage2("Misc Exterior is required.");
                    strRequired = "required";
                }
                else
                    lblMiscExterior.Text = "";

                if (txtMiscInterior.Text.Trim() == "")
                {
                    lblMiscInterior.Text = csCommonUtility.GetSystemRequiredMessage2("Misc Interior is required.");
                    strRequired = "required";
                }
                else
                    lblMiscInterior.Text = "";




                if (strRequired.Length == 0)
                {
                    DataRow dr = table.Rows[row.RowIndex];

                    dr["CabinetSheetID"] = Convert.ToInt32(grdCabinetSelectionSheet.DataKeys[row.RowIndex].Values[0]);
                    dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                    dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                    dr["CabinetSheetName"] = txtCabinetSheetName.Text.Trim();
                    dr["UpperWallDoor"] = txtUpperWallDoor.Text.Trim();
                    dr["UpperWallWood"] = txtUpperWallWood.Text.Trim();
                    dr["UpperWallStain"] = txtUpperWallStain.Text.Trim();
                    dr["UpperWallExterior"] = txtUpperWallExterior.Text.Trim();
                    dr["UpperWallInterior"] = txtUpperWallInterior.Text.Trim();
                    dr["UpperWallOther"] = txtUpperWallOther.Text.Trim();
                    dr["BaseDoor"] = txtBaseDoor.Text.Trim();
                    dr["BaseWood"] = txtBaseWood.Text.Trim();
                    dr["BaseStain"] = txtBaseStain.Text.Trim();
                    dr["BaseExterior"] = txtBaseExterior.Text.Trim();
                    dr["BaseInterior"] = txtBaseInterior.Text.Trim();
                    dr["BaseOther"] = txtBaseOther.Text.Trim();
                    dr["MiscDoor"] = txtMiscDoor.Text.Trim();
                    dr["MiscWood"] = txtMiscWood.Text.Trim();
                    dr["MiscStain"] = txtMiscStain.Text.Trim();
                    dr["MiscExterior"] = txtMiscExterior.Text.Trim();
                    dr["MiscInterior"] = txtMiscInterior.Text.Trim();
                    dr["MiscOther"] = txtMiscOther.Text.Trim();
                    dr["LastUpdateDate"] = DateTime.Now;
                    dr["UpdateBy"] = User.Identity.Name;
                }
            }
            if (strRequired.Length == 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    bool bFlagNew = false;

                    CabinetSheetSelection objCabSS = _db.CabinetSheetSelections.SingleOrDefault(l => l.CabinetSheetID == Convert.ToInt32(dr["CabinetSheetID"]));
                    if (objCabSS == null)
                    {
                        objCabSS = new CabinetSheetSelection();
                        bFlagNew = true;

                    }


                    objCabSS.CabinetSheetID = Convert.ToInt32(dr["CabinetSheetID"]);
                    objCabSS.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    objCabSS.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                    objCabSS.CabinetSheetName = dr["CabinetSheetName"].ToString();
                    objCabSS.UpperWallDoor = dr["UpperWallDoor"].ToString();
                    objCabSS.UpperWallWood = dr["UpperWallWood"].ToString();
                    objCabSS.UpperWallStain = dr["UpperWallStain"].ToString();
                    objCabSS.UpperWallExterior = dr["UpperWallExterior"].ToString();
                    objCabSS.UpperWallInterior = dr["UpperWallInterior"].ToString();
                    objCabSS.UpperWallOther = dr["UpperWallOther"].ToString();
                    objCabSS.BaseDoor = dr["BaseDoor"].ToString();
                    objCabSS.BaseWood = dr["BaseWood"].ToString();
                    objCabSS.BaseStain = dr["BaseStain"].ToString();
                    objCabSS.BaseExterior = dr["BaseExterior"].ToString();
                    objCabSS.BaseInterior = dr["BaseInterior"].ToString();
                    objCabSS.BaseOther = dr["BaseOther"].ToString();
                    objCabSS.MiscDoor = dr["MiscDoor"].ToString();
                    objCabSS.MiscWood = dr["MiscWood"].ToString();
                    objCabSS.MiscStain = dr["MiscStain"].ToString();
                    objCabSS.MiscExterior = dr["MiscExterior"].ToString();
                    objCabSS.MiscInterior = dr["MiscInterior"].ToString();
                    objCabSS.MiscOther = dr["MiscOther"].ToString();
                    objCabSS.LastUpdateDate = DateTime.Now;
                    objCabSS.UpdateBy = User.Identity.Name;


                    if (bFlagNew)
                    {
                        _db.CabinetSheetSelections.InsertOnSubmit(objCabSS);
                    }
                }


                lblResult.Text = csCommonUtility.GetSystemMessage("Data has been saved successfully");

                _db.SubmitChanges();
                GetCabinetSheet();
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void lnkDelete_Click(object sender, EventArgs e)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            LinkButton lnkDelete = (LinkButton)sender;
            int nCabinetSheetID = Convert.ToInt32(lnkDelete.Attributes["CommandArgument"]);
            string strQ = string.Empty;

            strQ = "Delete FROM CabinetSheetSelection  WHERE CabinetSheetID =" + nCabinetSheetID;
            _db.ExecuteCommand(strQ, string.Empty);
            GetCabinetSheet();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }


    }

    protected void btnAddItem_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddItem.ID, btnAddItem.GetType().Name, "Click"); 
        CabinetSheetSelection objCabSS = new CabinetSheetSelection();
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable table = (DataTable)Session["sCabinetSection"];

        int nCabinetSheetID = Convert.ToInt32(hdnCabinetSheetID.Value);
        bool contains = table.AsEnumerable().Any(row => nCabinetSheetID == row.Field<int>("CabinetSheetID"));
        if (contains)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("You have already a pending item to save.");
            return;
        }

        foreach (GridViewRow row in grdCabinetSelectionSheet.Rows)
        {

            DataRow dr = table.Rows[row.RowIndex];

            TextBox txtCabinetSheetName = (TextBox)row.FindControl("txtCabinetSheetName");

            TextBox txtUpperWallDoor = (TextBox)row.FindControl("txtUpperWallDoor");
            TextBox txtUpperWallWood = (TextBox)row.FindControl("txtUpperWallWood");
            TextBox txtUpperWallStain = (TextBox)row.FindControl("txtUpperWallStain");
            TextBox txtUpperWallExterior = (TextBox)row.FindControl("txtUpperWallExterior");
            TextBox txtUpperWallInterior = (TextBox)row.FindControl("txtUpperWallInterior");
            TextBox txtUpperWallOther = (TextBox)row.FindControl("txtUpperWallOther");

            TextBox txtBaseDoor = (TextBox)row.FindControl("txtBaseDoor");
            TextBox txtBaseWood = (TextBox)row.FindControl("txtBaseWood");
            TextBox txtBaseStain = (TextBox)row.FindControl("txtBaseStain");
            TextBox txtBaseExterior = (TextBox)row.FindControl("txtBaseExterior");
            TextBox txtBaseInterior = (TextBox)row.FindControl("txtBaseInterior");
            TextBox txtBaseOther = (TextBox)row.FindControl("txtBaseOther");

            TextBox txtMiscDoor = (TextBox)row.FindControl("txtMiscDoor");
            TextBox txtMiscWood = (TextBox)row.FindControl("txtMiscWood");
            TextBox txtMiscStain = (TextBox)row.FindControl("txtMiscStain");
            TextBox txtMiscExterior = (TextBox)row.FindControl("txtMiscExterior");
            TextBox txtMiscInterior = (TextBox)row.FindControl("txtMiscInterior");
            TextBox txtMiscOther = (TextBox)row.FindControl("txtMiscOther");

            dr["CabinetSheetID"] = Convert.ToInt32(grdCabinetSelectionSheet.DataKeys[row.RowIndex].Values[0]);
            dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            dr["CabinetSheetName"] = txtCabinetSheetName.Text.Trim();
            dr["UpperWallDoor"] = txtUpperWallDoor.Text.Trim();
            dr["UpperWallWood"] = txtUpperWallWood.Text.Trim();
            dr["UpperWallStain"] = txtUpperWallStain.Text.Trim();
            dr["UpperWallExterior"] = txtUpperWallExterior.Text.Trim();
            dr["UpperWallInterior"] = txtUpperWallInterior.Text.Trim();
            dr["UpperWallOther"] = txtUpperWallOther.Text.Trim();
            dr["BaseDoor"] = txtBaseDoor.Text.Trim();
            dr["BaseWood"] = txtBaseWood.Text.Trim();
            dr["BaseStain"] = txtBaseStain.Text.Trim();
            dr["BaseExterior"] = txtBaseExterior.Text.Trim();
            dr["BaseInterior"] = txtBaseInterior.Text.Trim();
            dr["BaseOther"] = txtBaseOther.Text.Trim();
            dr["MiscDoor"] = txtMiscDoor.Text.Trim();
            dr["MiscWood"] = txtMiscWood.Text.Trim();
            dr["MiscStain"] = txtMiscStain.Text.Trim();
            dr["MiscExterior"] = txtMiscExterior.Text.Trim();
            dr["MiscInterior"] = txtMiscInterior.Text.Trim();
            dr["MiscOther"] = txtMiscOther.Text.Trim();
            dr["LastUpdateDate"] = DateTime.Now;
            dr["UpdateBy"] = User.Identity.Name;
        }

        DataRow drNew = table.NewRow();

        drNew["CabinetSheetID"] = Convert.ToInt32(hdnCabinetSheetID.Value);
        drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
        drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
        drNew["CabinetSheetName"] = "";
        drNew["UpperWallDoor"] = "";
        drNew["UpperWallWood"] = "";
        drNew["UpperWallStain"] = "";
        drNew["UpperWallExterior"] = "";
        drNew["UpperWallInterior"] = "";
        drNew["UpperWallOther"] = "";
        drNew["BaseDoor"] = "";
        drNew["BaseWood"] = "";
        drNew["BaseStain"] = "";
        drNew["BaseExterior"] = "";
        drNew["BaseInterior"] = "";
        drNew["BaseOther"] = "";
        drNew["MiscDoor"] = "";
        drNew["MiscWood"] = "";
        drNew["MiscStain"] = "";
        drNew["MiscExterior"] = "";
        drNew["MiscInterior"] = "";
        drNew["MiscOther"] = "";
        drNew["LastUpdateDate"] = DateTime.Now;
        drNew["UpdateBy"] = User.Identity.Name;

        table.Rows.InsertAt(drNew, 0);

        Session.Add("sCabinetSection", table);
        grdCabinetSelectionSheet.DataSource = table;
        grdCabinetSelectionSheet.DataKeyNames = new string[] { "CabinetSheetID", "customer_id", "estimate_id" };
        grdCabinetSelectionSheet.DataBind();
        lblResult.Text = "";

    }

    private DataTable LoadSectionTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("CabinetSheetID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("CabinetSheetName", typeof(string));
        table.Columns.Add("UpperWallDoor", typeof(string));
        table.Columns.Add("UpperWallWood", typeof(string));
        table.Columns.Add("UpperWallStain", typeof(string));
        table.Columns.Add("UpperWallExterior", typeof(string));
        table.Columns.Add("UpperWallInterior", typeof(string));
        table.Columns.Add("UpperWallOther", typeof(string));
        table.Columns.Add("BaseDoor", typeof(string));
        table.Columns.Add("BaseWood", typeof(string));
        table.Columns.Add("BaseStain", typeof(string));
        table.Columns.Add("BaseExterior", typeof(string));
        table.Columns.Add("BaseInterior", typeof(string));
        table.Columns.Add("BaseOther", typeof(string));
        table.Columns.Add("MiscDoor", typeof(string));
        table.Columns.Add("MiscWood", typeof(string));
        table.Columns.Add("MiscStain", typeof(string));
        table.Columns.Add("MiscExterior", typeof(string));
        table.Columns.Add("MiscInterior", typeof(string));
        table.Columns.Add("MiscOther", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));
        table.Columns.Add("UpdateBy", typeof(string));

        return table;
    }

    #endregion

    #region Bathroom

    protected void lnkDeleteBath_Click(object sender, EventArgs e)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            LinkButton lnkDeleteBath = (LinkButton)sender;
            int nBathSheetID = Convert.ToInt32(lnkDeleteBath.Attributes["CommandArgument"]);
            string strQ = string.Empty;

            strQ = "Delete FROM BathroomSheetSelections  WHERE BathroomID =" + nBathSheetID;
            _db.ExecuteCommand(strQ, string.Empty);
            GetBathroomSheet();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }


    }

    protected void btnSaveBathroom_Click(object sender, EventArgs e)
    {
        lblBathroomResult.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        string strRequired = string.Empty;
        try
        {
            DataTable table = (DataTable)Session["sBathroomSection"];



            foreach (GridViewRow row in grdBathroomSelectionSheet.Rows)
            {

                TextBox txtBathSheetName = (TextBox)row.FindControl("txtBathSheetName");
                Label lblBathSheetName = (Label)row.FindControl("lblBathSheetName");

                TextBox txtSinkQty = (TextBox)row.FindControl("txtSinkQty");
                TextBox txtSinkStyle = (TextBox)row.FindControl("txtSinkStyle");
                TextBox txtSinkOrder = (TextBox)row.FindControl("txtSinkOrder");

                TextBox txtSinkFaucentQty = (TextBox)row.FindControl("txtSinkFaucentQty");
                TextBox txtSinkFaucentStyle = (TextBox)row.FindControl("txtSinkFaucentStyle");
                TextBox txtSinkFaucentOrder = (TextBox)row.FindControl("txtSinkFaucentOrder");

                TextBox txtSinkDrainQty = (TextBox)row.FindControl("txtSinkDrainQty");
                TextBox txtSinkDrainStyle = (TextBox)row.FindControl("txtSinkDrainStyle");
                TextBox txtSinkdrainOrder = (TextBox)row.FindControl("txtSinkdrainOrder");

                TextBox txtSinkValveQty = (TextBox)row.FindControl("txtSinkValveQty");
                TextBox txtSinkValveStyle = (TextBox)row.FindControl("txtSinkValveStyle");
                TextBox txtSinkValveOrder = (TextBox)row.FindControl("txtSinkValveOrder");

                TextBox txtBathTubQty = (TextBox)row.FindControl("txtBathTubQty");
                TextBox txtBathTubStyle = (TextBox)row.FindControl("txtBathTubStyle");
                TextBox txtBathTubOrder = (TextBox)row.FindControl("txtBathTubOrder");

                TextBox txtTubFaucentQty = (TextBox)row.FindControl("txtTubFaucentQty");
                TextBox txtTubFaucentStyle = (TextBox)row.FindControl("txtTubFaucentStyle");
                TextBox txtTubFaucentOrder = (TextBox)row.FindControl("txtTubFaucentOrder");

                TextBox txtTubValveQty = (TextBox)row.FindControl("txtTubValveQty");
                TextBox txtTubValveStyle = (TextBox)row.FindControl("txtTubValveStyle");
                TextBox txtTubValveOrder = (TextBox)row.FindControl("txtTubValveOrder");

                TextBox txtTubDrainQty = (TextBox)row.FindControl("txtTubDrainQty");
                TextBox txtTubDrainStyle = (TextBox)row.FindControl("txtTubDrainStyle");
                TextBox txtTubDrainOrder = (TextBox)row.FindControl("txtTubDrainOrder");

                TextBox txtToiletQty = (TextBox)row.FindControl("txtToiletQty");
                TextBox txtToiletStyle = (TextBox)row.FindControl("txtToiletStyle");
                TextBox txtToiletOrder = (TextBox)row.FindControl("txtToiletOrder");

                TextBox txtShower_TubSystemQty = (TextBox)row.FindControl("txtShower_TubSystemQty");
                TextBox txtShower_TubSystemStyle = (TextBox)row.FindControl("txtShower_TubSystemStyle");
                TextBox txtShower_TubSystemOrder = (TextBox)row.FindControl("txtShower_TubSystemOrder");

                TextBox txtShowerValveQty = (TextBox)row.FindControl("txtShowerValveQty");
                TextBox txtShowerValveStyle = (TextBox)row.FindControl("txtShowerValveStyle");
                TextBox txtShowerValveOrder = (TextBox)row.FindControl("txtShowerValveOrder");

                TextBox txtHandheldShowerQty = (TextBox)row.FindControl("txtHandheldShowerQty");
                TextBox txtHandheldShowerStyle = (TextBox)row.FindControl("txtHandheldShowerStyle");
                TextBox txtHandheldShowerOrder = (TextBox)row.FindControl("txtHandheldShowerOrder");

                TextBox txtBodySprayQty = (TextBox)row.FindControl("txtBodySprayQty");
                TextBox txtBodySprayStyle = (TextBox)row.FindControl("txtBodySprayStyle");
                TextBox txtBodySprayOrder = (TextBox)row.FindControl("txtBodySprayOrder");

                TextBox txtBodySprayValveQty = (TextBox)row.FindControl("txtBodySprayValveQty");
                TextBox txtBodySprayValveStyle = (TextBox)row.FindControl("txtBodySprayValveStyle");
                TextBox txtBodySprayValveOrder = (TextBox)row.FindControl("txtBodySprayValveOrder");

                TextBox txtShowerDrainQty = (TextBox)row.FindControl("txtShowerDrainQty");
                TextBox txtShowerDrainStyle = (TextBox)row.FindControl("txtShowerDrainStyle");
                TextBox txtShowerDrainOrder = (TextBox)row.FindControl("txtShowerDrainOrder");

                TextBox txtShowerDrainBody_PlugQty = (TextBox)row.FindControl("txtShowerDrainBody_PlugQty");
                TextBox txtShowerDrainBody_PlugStyle = (TextBox)row.FindControl("txtShowerDrainBody_PlugStyle");
                TextBox txtShowerDrainBody_PlugOrder = (TextBox)row.FindControl("txtShowerDrainBody_PlugOrder");

                TextBox txtShowerDrainCoverQty = (TextBox)row.FindControl("txtShowerDrainCoverQty");
                TextBox txtShowerDrainCoverStyle = (TextBox)row.FindControl("txtShowerDrainCoverStyle");
                TextBox txtShowerDrainCoverOrder = (TextBox)row.FindControl("txtShowerDrainCoverOrder");

                TextBox txtCounterTopQty = (TextBox)row.FindControl("txtCounterTopQty");
                TextBox txtCounterTopStyle = (TextBox)row.FindControl("txtCounterTopStyle");
                TextBox txtCounterTopOrder = (TextBox)row.FindControl("txtCounterTopOrder");

                TextBox txtCounterTopEdgeQty = (TextBox)row.FindControl("txtCounterTopEdgeQty");
                TextBox txtCounterTopEdgeStyle = (TextBox)row.FindControl("txtCounterTopEdgeStyle");
                TextBox txtCounterTopEdgeOrder = (TextBox)row.FindControl("txtCounterTopEdgeOrder");

                TextBox txtCounterTop_OverhangQty = (TextBox)row.FindControl("txtCounterTop_OverhangQty");
                TextBox txtCounterTop_OverhangStyle = (TextBox)row.FindControl("txtCounterTop_OverhangStyle");
                TextBox txtCounterTop_OverhangOrder = (TextBox)row.FindControl("txtCounterTop_OverhangOrder");

                TextBox txtAdditionalplacesgettingcountertopQty = (TextBox)row.FindControl("txtAdditionalplacesgettingcountertopQty");
                TextBox txtAdditionalplacesgettingcountertopStyle = (TextBox)row.FindControl("txtAdditionalplacesgettingcountertopStyle");
                TextBox txtAdditionalplacesgettingcountertopOrder = (TextBox)row.FindControl("txtAdditionalplacesgettingcountertopOrder");

                TextBox txtGranite_Quartz_BacksplashQty = (TextBox)row.FindControl("txtGranite_Quartz_BacksplashQty");
                TextBox txtGranite_Quartz_BacksplashStyle = (TextBox)row.FindControl("txtGranite_Quartz_BacksplashStyle");
                TextBox txtGranite_Quartz_BacksplashOrder = (TextBox)row.FindControl("txtGranite_Quartz_BacksplashOrder");

                TextBox txtTubwalltileQty = (TextBox)row.FindControl("txtTubwalltileQty");
                TextBox txtTubwalltileStyle = (TextBox)row.FindControl("txtTubwalltileStyle");
                TextBox txtTubwalltileOrder = (TextBox)row.FindControl("txtTubwalltileOrder");

                TextBox txtWallTilelayoutQty = (TextBox)row.FindControl("txtWallTilelayoutQty");
                TextBox txtWallTilelayoutStyle = (TextBox)row.FindControl("txtWallTilelayoutStyle");
                TextBox txtWallTilelayoutOrder = (TextBox)row.FindControl("txtWallTilelayoutOrder");

                TextBox txtTubskirttileQty = (TextBox)row.FindControl("txtTubskirttileQty");
                TextBox txtTubskirttileStyle = (TextBox)row.FindControl("txtTubskirttileStyle");
                TextBox txtTubskirttileOrder = (TextBox)row.FindControl("txtTubskirttileOrder");

                TextBox txtShowerWallTileQty = (TextBox)row.FindControl("txtShowerWallTileQty");
                TextBox txtShowerWallTileStyle = (TextBox)row.FindControl("txtShowerWallTileStyle");
                TextBox txtShowerWallTileOrder = (TextBox)row.FindControl("txtShowerWallTileOrder");

                TextBox txtWall_Tile_layoutQty = (TextBox)row.FindControl("txtWall_Tile_layoutQty");
                TextBox txtWall_Tile_layoutStyle = (TextBox)row.FindControl("txtWall_Tile_layoutStyle");
                TextBox txtWall_Tile_layoutOrder = (TextBox)row.FindControl("txtWall_Tile_layoutOrder");

                TextBox txtShowerFloorTileQty = (TextBox)row.FindControl("txtShowerFloorTileQty");
                TextBox txtShowerFloorTileStyle = (TextBox)row.FindControl("txtShowerFloorTileStyle");
                TextBox txtShowerFloorTileOrder = (TextBox)row.FindControl("txtShowerFloorTileOrder");

                TextBox txtShowerTubTileHeightQty = (TextBox)row.FindControl("txtShowerTubTileHeightQty");
                TextBox txtShowerTubTileHeightStyle = (TextBox)row.FindControl("txtShowerTubTileHeightStyle");
                TextBox txtShowerTubTileHeightOrder = (TextBox)row.FindControl("txtShowerTubTileHeightOrder");

                TextBox txtFloorTiletQty = (TextBox)row.FindControl("txtFloorTiletQty");
                TextBox txtFloorTiletstyle = (TextBox)row.FindControl("txtFloorTiletstyle");
                TextBox txtFloorTiletOrder = (TextBox)row.FindControl("txtFloorTiletOrder");

                TextBox txtFloorTilelayoutQty = (TextBox)row.FindControl("txtFloorTilelayoutQty");
                TextBox txtFloorTilelayoutStyle = (TextBox)row.FindControl("txtFloorTilelayoutStyle");
                TextBox txtFloorTilelayoutOrder = (TextBox)row.FindControl("txtFloorTilelayoutOrder");

                TextBox txtBullnoseTileQty = (TextBox)row.FindControl("txtBullnoseTileQty");
                TextBox txtBullnoseTileStyle = (TextBox)row.FindControl("txtBullnoseTileStyle");
                TextBox txtBullnoseTileOrder = (TextBox)row.FindControl("txtBullnoseTileOrder");

                TextBox txtDecobandQty = (TextBox)row.FindControl("txtDecobandQty");
                TextBox txtDecobandStyle = (TextBox)row.FindControl("txtDecobandStyle");
                TextBox txtDecobandOrder = (TextBox)row.FindControl("txtDecobandOrder");

                TextBox txtDecobandHeightQty = (TextBox)row.FindControl("txtDecobandHeightQty");
                TextBox txtDecobandHeightStyle = (TextBox)row.FindControl("txtDecobandHeightStyle");
                TextBox txtDecobandHeightOrder = (TextBox)row.FindControl("txtDecobandHeightOrder");


                TextBox txtTileBaseboardQty = (TextBox)row.FindControl("txtTileBaseboardQty");
                TextBox txtTileBaseboardStyle = (TextBox)row.FindControl("txtTileBaseboardStyle");
                TextBox txtTileBaseboardOrder = (TextBox)row.FindControl("txtTileBaseboardOrder");

                TextBox txtGroutSelectionQty = (TextBox)row.FindControl("txtGroutSelectionQty");
                TextBox txtGroutSelectionStyle = (TextBox)row.FindControl("txtGroutSelectionStyle");
                TextBox txtGroutSelectionOrder = (TextBox)row.FindControl("txtGroutSelectionOrder");

                TextBox txtNicheLocationQty = (TextBox)row.FindControl("txtNicheLocationQty");
                TextBox txtNicheLocationStyle = (TextBox)row.FindControl("txtNicheLocationStyle");
                TextBox txtNicheLocationOrder = (TextBox)row.FindControl("txtNicheLocationOrder");

                TextBox txtNicheSizeQty = (TextBox)row.FindControl("txtNicheSizeQty");
                TextBox txtNicheSizeStyle = (TextBox)row.FindControl("txtNicheSizeStyle");
                TextBox txtNicheSizeOrder = (TextBox)row.FindControl("txtNicheSizeOrder");

                TextBox txtGlassQty = (TextBox)row.FindControl("txtGlassQty");
                TextBox txtGlassStyle = (TextBox)row.FindControl("txtGlassStyle");
                TextBox txtGlassOrder = (TextBox)row.FindControl("txtGlassOrder");

                TextBox txtWindowQty = (TextBox)row.FindControl("txtWindowQty");
                TextBox txtWindowStyle = (TextBox)row.FindControl("txtWindowStyle");
                TextBox txtWindowOrder = (TextBox)row.FindControl("txtWindowOrder");

                TextBox txtDoorQty = (TextBox)row.FindControl("txtDoorQty");
                TextBox txtDoorStyle = (TextBox)row.FindControl("txtDoorStyle");
                TextBox txtDoorOrder = (TextBox)row.FindControl("txtDoorOrder");

                TextBox txtGrabBarQty = (TextBox)row.FindControl("txtGrabBarQty");
                TextBox txtGrabBarStyle = (TextBox)row.FindControl("txtGrabBarStyle");
                TextBox txtGrabBarOrder = (TextBox)row.FindControl("txtGrabBarOrder");


                TextBox txtCabinetDoorStyleColorQty = (TextBox)row.FindControl("txtCabinetDoorStyleColorQty");
                TextBox txtCabinetDoorStyleColorStyle = (TextBox)row.FindControl("txtCabinetDoorStyleColorStyle");
                TextBox txtCabinetDoorStyleColorOrder = (TextBox)row.FindControl("txtCabinetDoorStyleColorOrder");

                TextBox txtMedicineCabinetQty = (TextBox)row.FindControl("txtMedicineCabinetQty");
                TextBox txtMedicineCabinetStyle = (TextBox)row.FindControl("txtMedicineCabinetStyle");
                TextBox txtMedicineCabinetOrder = (TextBox)row.FindControl("txtMedicineCabinetOrder");

                TextBox txtMirrorQty = (TextBox)row.FindControl("txtMirrorQty");
                TextBox txtMirrorStyle = (TextBox)row.FindControl("txtMirrorStyle");
                TextBox txtMirrorOrder = (TextBox)row.FindControl("txtMirrorOrder");

                TextBox txtWoodBaseboardQty = (TextBox)row.FindControl("txtWoodBaseboardQty");
                TextBox txtWoodBaseboardStyle = (TextBox)row.FindControl("txtWoodBaseboardStyle");
                TextBox txtWoodBaseboardOrder = (TextBox)row.FindControl("txtWoodBaseboardOrder");

                TextBox txtPaintColorQty = (TextBox)row.FindControl("txtPaintColorQty");
                TextBox txtPaintColorStyle = (TextBox)row.FindControl("txtPaintColorStyle");
                TextBox txtPaintColorOrder = (TextBox)row.FindControl("txtPaintColorOrder");

                TextBox txtLightingQty = (TextBox)row.FindControl("txtLightingQty");
                TextBox txtLightingStyle = (TextBox)row.FindControl("txtLightingStyle");
                TextBox txtLightingOrder = (TextBox)row.FindControl("txtLightingOrder");

                TextBox txtHardwareQty = (TextBox)row.FindControl("txtHardwareQty");
                TextBox txtHardwareStyle = (TextBox)row.FindControl("txtHardwareStyle");
                TextBox txtHardwareOrder = (TextBox)row.FindControl("txtHardwareOrder");

                TextBox txtTowelRingQty = (TextBox)row.FindControl("txtTowelRingQty");
                TextBox txtTowelRingStyle = (TextBox)row.FindControl("txtTowelRingStyle");
                TextBox txtTowelRingOrder = (TextBox)row.FindControl("txtTowelRingOrder");

                TextBox txtTowelBarQty = (TextBox)row.FindControl("txtTowelBarQty");
                TextBox txtTowelBarStyle = (TextBox)row.FindControl("txtTowelBarStyle");
                TextBox txtTowelBarOrder = (TextBox)row.FindControl("txtTowelBarOrder");

                TextBox txtTissueHolderQty = (TextBox)row.FindControl("txtTissueHolderQty");
                TextBox txtTissueHolderStyle = (TextBox)row.FindControl("txtTissueHolderStyle");
                TextBox txtTissueHolderOrder = (TextBox)row.FindControl("txtTissueHolderOrder");

                TextBox txtClosetDoorSeries = (TextBox)row.FindControl("txtClosetDoorSeries");
                TextBox txtClosetDoorOpeningSize = (TextBox)row.FindControl("txtClosetDoorOpeningSize");
                TextBox txtClosetDoorNumberOfPanels = (TextBox)row.FindControl("txtClosetDoorNumberOfPanels");
                TextBox txtClosetDoorFinish = (TextBox)row.FindControl("txtClosetDoorFinish");
                TextBox txtClosetDoorInsert = (TextBox)row.FindControl("txtClosetDoorInsert");

                TextBox txtBathpecialNotes = (TextBox)row.FindControl("txtBathpecialNotes");


                if (txtBathSheetName.Text.Trim() == "")
                {
                    lblBathSheetName.Text = csCommonUtility.GetSystemRequiredMessage2("Title/Location of Bathroom sheet is required.");
                    strRequired = "required";
                }
                else
                    lblBathSheetName.Text = "";



                if (strRequired.Length == 0)
                {
                    DataRow dr = table.Rows[row.RowIndex];

                    dr["BathroomID"] = Convert.ToInt32(grdBathroomSelectionSheet.DataKeys[row.RowIndex].Values[0]);
                    dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                    dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                    dr["BathroomSheetName"] = txtBathSheetName.Text;
                    dr["Sink_Qty"] = txtSinkQty.Text;
                    dr["Sink_Style"] = txtSinkStyle.Text;
                    dr["Sink_WhereToOrder"] = txtSinkOrder.Text;
                    dr["Sink_Fuacet_Qty"] = txtSinkFaucentQty.Text;
                    dr["Sink_Fuacet_Style"] = txtSinkFaucentStyle.Text;
                    dr["Sink_Fuacet_WhereToOrder"] = txtSinkFaucentOrder.Text;
                    dr["Sink_Drain_Qty"] = txtSinkDrainQty.Text;
                    dr["Sink_Drain_Style"] = txtSinkDrainStyle.Text;
                    dr["Sink_Drain_WhereToOrder"] = txtSinkdrainOrder.Text;
                    dr["Sink_Valve_Qty"] = txtSinkValveQty.Text;
                    dr["Sink_Valve_Style"] = txtSinkValveStyle.Text;
                    dr["Sink_Valve_WhereToOrder"] = txtSinkValveOrder.Text;
                    dr["Bathtub_Qty"] = txtBathTubQty.Text;
                    dr["Bathtub_Style"] = txtBathTubStyle.Text;
                    dr["Bathtub_WhereToOrder"] = txtBathTubOrder.Text;
                    dr["Tub_Faucet_Qty"] = txtTubFaucentQty.Text;
                    dr["Tub_Faucet_Style"] = txtTubFaucentStyle.Text;
                    dr["Tub_Faucet_WhereToOrder"] = txtTubFaucentOrder.Text;
                    dr["Tub_Valve_Qty"] = txtTubValveQty.Text;
                    dr["Tub_Valve_Style"] = txtTubValveStyle.Text;
                    dr["Tub_Valve_WhereToOrder"] = txtTubValveOrder.Text;
                    dr["Tub_Drain_Qty"] = txtTubDrainQty.Text;
                    dr["Tub_Drain_Style"] = txtTubDrainStyle.Text;
                    dr["Tub_Drain_WhereToOrder"] = txtTubDrainOrder.Text;
                    dr["Tollet_Qty"] = txtToiletQty.Text;
                    dr["Tollet_Style"] = txtToiletStyle.Text;
                    dr["Tollet_WhereToOrder"] = txtToiletOrder.Text;
                    dr["Shower_TubSystem_Qty"] = txtShower_TubSystemQty.Text;
                    dr["Shower_TubSystem_Style"] = txtShower_TubSystemStyle.Text;
                    dr["Shower_TubSystem_WhereToOrder"] = txtShower_TubSystemOrder.Text;
                    dr["Shower_Value_Qty"] = txtShowerValveQty.Text;
                    dr["Shower_Value_Style"] = txtShowerValveStyle.Text;
                    dr["Shower_Value_WhereToOrder"] = txtShowerValveOrder.Text;
                    dr["Handheld_Shower_Qty"] = txtHandheldShowerQty.Text;
                    dr["Handheld_Shower_Style"] = txtHandheldShowerStyle.Text;
                    dr["Handheld_Shower_WhereToOrder"] = txtHandheldShowerOrder.Text;
                    dr["Body_Spray_Qty"] = txtBodySprayQty.Text;
                    dr["Body_Spray_Style"] = txtBodySprayStyle.Text;
                    dr["Body_Spray_WhereToOrder"] = txtBodySprayOrder.Text;
                    dr["Body_Spray_Valve_Qty"] = txtBodySprayValveQty.Text;
                    dr["Body_Spray_Valve_Style"] = txtBodySprayValveStyle.Text;
                    dr["Body_Spray_Valve_WhereToOrder"] = txtBodySprayValveOrder.Text;
                    dr["Shower_Drain_Qty"] = txtShowerDrainQty.Text;
                    dr["Shower_Drain_Style"] = txtShowerDrainStyle.Text;
                    dr["Shower_Drain_WhereToOrder"] = txtShowerDrainOrder.Text;
                    dr["Shower_Drain_Body_Plug_Qty"] = txtShowerDrainBody_PlugQty.Text;
                    dr["Shower_Drain_Body_Plug_Style"] = txtShowerDrainBody_PlugStyle.Text;
                    dr["Shower_Drain_Body_Plug_WhereToOrder"] = txtShowerDrainBody_PlugOrder.Text;
                    dr["Shower_Drain_Cover_Qty"] = txtShowerDrainCoverQty.Text;
                    dr["Shower_Drain_Cover_Style"] = txtShowerDrainCoverStyle.Text;
                    dr["Shower_Drain_Cover_WhereToOrder"] = txtShowerDrainCoverOrder.Text;
                    dr["Counter_Top_Qty"] = txtCounterTopQty.Text;
                    dr["Counter_Top_Style"] = txtCounterTopStyle.Text;
                    dr["Counter_Top_WhereToOrder"] = txtCounterTopOrder.Text;
                    dr["Counter_To_Edge_Qty"] = txtCounterTopEdgeQty.Text;
                    dr["Counter_To_Edge_Style"] = txtCounterTopEdgeStyle.Text;
                    dr["Counter_To_Edge_WhereToOrder"] = txtCounterTopEdgeOrder.Text;
                    dr["Counter_Top_Overhang_Qty"] = txtCounterTop_OverhangQty.Text;
                    dr["Counter_Top_Overhang_Style"] = txtCounterTop_OverhangStyle.Text;
                    dr["Counter_Top_Overhang_WhereToOrder"] = txtCounterTop_OverhangOrder.Text;
                    dr["AdditionalPlacesGettingCountertop_Qty"] = txtAdditionalplacesgettingcountertopQty.Text;
                    dr["AdditionalPlacesGettingCountertop_Style"] = txtAdditionalplacesgettingcountertopStyle.Text;
                    dr["AdditionalPlacesGettingCountertop_WhereToOrder"] = txtAdditionalplacesgettingcountertopOrder.Text;
                    dr["Granite_Quartz_Backsplash_Qty"] = txtGranite_Quartz_BacksplashQty.Text;
                    dr["Granite_Quartz_Backsplash_Style"] = txtGranite_Quartz_BacksplashStyle.Text;
                    dr["Granite_Quartz_Backsplash_WhereToOrder"] = txtGranite_Quartz_BacksplashOrder.Text;
                    dr["Tub_Wall_Tile_Qty"] = txtTubwalltileQty.Text;
                    dr["Tub_Wall_Tile_Style"] = txtTubwalltileStyle.Text;
                    dr["Tub_Wall_Tile_WhereToOrder"] = txtTubwalltileOrder.Text;
                    dr["Wall_Tile_Layout_Qty"] = txtWallTilelayoutQty.Text;
                    dr["Wall_Tile_Layout_Style"] = txtWallTilelayoutStyle.Text;
                    dr["Wall_Tile_Layout_WhereToOrder"] = txtWallTilelayoutOrder.Text;
                    dr["Tub_skirt_tile_Qty"] = txtTubskirttileQty.Text;
                    dr["Tub_skirt_tile_Style"] = txtTubskirttileStyle.Text;
                    dr["Tub_skirt_tile_WhereToOrder"] = txtTubskirttileOrder.Text;
                    dr["Shower_Wall_Tile_Qty"] = txtShowerWallTileQty.Text;
                    dr["Shower_Wall_Tile_Style"] = txtShowerWallTileStyle.Text;
                    dr["Shower_Wall_Tile_WhereToOrder"] = txtShowerWallTileOrder.Text;
                    dr["Wall_Tile_Layout_Qty"] = txtWall_Tile_layoutQty.Text;
                    dr["Wall_Tile_Layout_Style"] = txtWall_Tile_layoutStyle.Text;
                    dr["Wall_Tile_Layout_WhereToOrder"] = txtWall_Tile_layoutOrder.Text;
                    dr["Shower_Floor_Tile_Qty"] = txtShowerFloorTileQty.Text;
                    dr["Shower_Floor_Tile_Style"] = txtShowerFloorTileStyle.Text;
                    dr["Shower_Floor_Tile_WhereToOrder"] = txtShowerFloorTileOrder.Text;
                    dr["Shower_Tub_Tile_Height_Qty"] = txtShowerTubTileHeightQty.Text;
                    dr["Shower_Tub_Tile_Height_Style"] = txtShowerTubTileHeightStyle.Text;
                    dr["Shower_Tub_Tile_Height_WhereToOrder"] = txtShowerTubTileHeightOrder.Text;
                    dr["Floor_Tile_Qty"] = txtFloorTiletQty.Text;
                    dr["Floor_Tile_Style"] = txtFloorTiletstyle.Text;
                    dr["Floor_Tile_WhereToOrder"] = txtFloorTiletOrder.Text;
                    dr["Floor_Tile_layout_Qty"] = txtFloorTilelayoutQty.Text;
                    dr["Floor_Tile_layout_Style"] = txtFloorTilelayoutStyle.Text;
                    dr["Floor_Tile_layout_WhereToOrder"] = txtFloorTilelayoutOrder.Text;
                    dr["BullnoseTile_Qty"] = txtBullnoseTileQty.Text;
                    dr["BullnoseTile_Style"] = txtBullnoseTileStyle.Text;
                    dr["BullnoseTile_WhereToOrder"] = txtBullnoseTileOrder.Text;
                    dr["Deco_Band_Qty"] = txtDecobandQty.Text;
                    dr["Deco_Band_Style"] = txtDecobandStyle.Text;
                    dr["Deco_Band_WhereToOrder"] = txtDecobandOrder.Text;
                    dr["Deco_Band_Height_Qty"] = txtDecobandHeightQty.Text;
                    dr["Deco_Band_Height_Style"] = txtDecobandHeightStyle.Text;
                    dr["Deco_Band_Height_WhereToOrder"] = txtDecobandHeightOrder.Text;
                    dr["Tile_Baseboard_Qty"] = txtTileBaseboardQty.Text;
                    dr["Tile_Baseboard_Style"] = txtTileBaseboardStyle.Text;
                    dr["Tile_Baseboard_WhereToOrder"] = txtTileBaseboardOrder.Text;
                    dr["Grout_Selection_Qty"] = txtGroutSelectionQty.Text;
                    dr["Grout_Selection_Style"] = txtGroutSelectionStyle.Text;
                    dr["Grout_Selection_WhereToOrder"] = txtGroutSelectionOrder.Text;
                    dr["Niche_Location_Qty"] = txtNicheLocationQty.Text;
                    dr["Niche_Location_Style"] = txtNicheLocationStyle.Text;
                    dr["Niche_Location_WhereToOrder"] = txtNicheLocationOrder.Text;
                    dr["Niche_Size_Qty"] = txtNicheSizeQty.Text;
                    dr["Niche_Size_Style"] = txtNicheSizeStyle.Text;
                    dr["Niche_Size_WhereToOrder"] = txtNicheSizeOrder.Text;
                    dr["Glass_Qty"] = txtGlassQty.Text;
                    dr["Glass_Style"] = txtGlassStyle.Text;
                    dr["Glass_WhereToOrder"] = txtGlassOrder.Text;
                    dr["Window_Qty"] = txtWindowQty.Text;
                    dr["Window_Style"] = txtWindowStyle.Text;
                    dr["Window_WhereToOrder"] = txtWindowOrder.Text;
                    dr["Door_Qty"] = txtDoorQty.Text;
                    dr["Door_Style"] = txtDoorStyle.Text;
                    dr["Door_WhereToOrder"] = txtDoorOrder.Text;
                    dr["Grab_Bar_Qty"] = txtGrabBarQty.Text;
                    dr["Grab_Bar_Style"] = txtGrabBarStyle.Text;
                    dr["Grab_Bar_WhereToOrder"] = txtGrabBarOrder.Text;
                    dr["Cabinet_Door_Style_Color_Qty"] = txtCabinetDoorStyleColorQty.Text;
                    dr["Cabinet_Door_Style_Color_Style"] = txtCabinetDoorStyleColorStyle.Text;
                    dr["Cabinet_Door_Style_Color_WhereToOrder"] = txtCabinetDoorStyleColorOrder.Text;
                    dr["Medicine_Cabinet_Qty"] = txtMedicineCabinetQty.Text;
                    dr["Medicine_Cabinet_Style"] = txtMedicineCabinetStyle.Text;
                    dr["Medicine_Cabinet_WhereToOrder"] = txtMedicineCabinetOrder.Text;
                    dr["Mirror_Qty"] = txtMirrorQty.Text;
                    dr["Mirror_Style"] = txtMirrorStyle.Text;
                    dr["Mirror_WhereToOrder"] = txtMirrorOrder.Text;
                    dr["Wood_Baseboard_Qty"] = txtWoodBaseboardQty.Text;
                    dr["Wood_Baseboard_Style"] = txtWoodBaseboardStyle.Text;
                    dr["Wood_Baseboard_WhereToOrder"] = txtWoodBaseboardOrder.Text;
                    dr["Paint_Color_Qty"] = txtPaintColorQty.Text;
                    dr["Paint_Color_Style"] = txtPaintColorStyle.Text;
                    dr["Paint_Color_WhereToOrder"] = txtPaintColorOrder.Text;
                    dr["Lighting_Qty"] = txtLightingQty.Text;
                    dr["Lighting_Style"] = txtLightingStyle.Text;
                    dr["Lighting_WhereToOrder"] = txtLightingOrder.Text;
                    dr["Hardware_Qty"] = txtHardwareQty.Text;
                    dr["Hardware_Style"] = txtHardwareStyle.Text;
                    dr["Hardware_WhereToOrder"] = txtHardwareOrder.Text;

                    dr["TowelRing_Qty"] = txtTowelRingQty.Text;
                    dr["TowelRing_Style"] = txtTowelRingStyle.Text;
                    dr["TowelRing_WhereToOrder"] = txtTowelRingOrder.Text;
                    dr["TowelBar_Qty"] = txtTowelBarQty.Text;
                    dr["TowelBar_Style"] = txtTowelBarStyle.Text;
                    dr["TowelBar_WhereToOrder"] = txtTowelBarOrder.Text;
                    dr["TissueHolder_Qty"] = txtTissueHolderQty.Text;
                    dr["TissueHolder_Style"] = txtTissueHolderOrder.Text;
                    dr["TissueHolder_WhereToOrder"] = txtTissueHolderOrder.Text;
                    dr["ClosetDoorSeries"] = txtClosetDoorSeries.Text;
                    dr["ClosetDoorOpeningSize"] = txtClosetDoorOpeningSize.Text;
                    dr["ClosetDoorNumberOfPanels"] = txtClosetDoorNumberOfPanels.Text;
                    dr["ClosetDoorFinish"] = txtClosetDoorFinish.Text;
                    dr["ClosetDoorInsert"] = txtClosetDoorInsert.Text;

                    dr["Special_Notes"] = txtBathpecialNotes.Text;
                    dr["UpdateBy"] = User.Identity.Name;
                    dr["LastUpdatedDate"] = DateTime.Now;
                }
            }
            if (strRequired.Length == 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    bool bFlagNew = false;

                    BathroomSheetSelection objBathSS = _db.BathroomSheetSelections.SingleOrDefault(l => l.BathroomID == Convert.ToInt32(dr["BathroomID"]));
                    if (objBathSS == null)
                    {
                        objBathSS = new BathroomSheetSelection();
                        bFlagNew = true;

                    }

                    objBathSS.BathroomID = Convert.ToInt32(dr["BathroomID"]);
                    objBathSS.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    objBathSS.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                    objBathSS.BathroomSheetName = dr["BathroomSheetName"].ToString();
                    objBathSS.Sink_Qty = dr["Sink_Qty"].ToString();
                    objBathSS.Sink_Style = dr["Sink_Style"].ToString();
                    objBathSS.Sink_WhereToOrder = dr["Sink_WhereToOrder"].ToString();
                    objBathSS.Sink_Fuacet_Qty = dr["Sink_Fuacet_Qty"].ToString();
                    objBathSS.Sink_Fuacet_Style = dr["Sink_Fuacet_Style"].ToString();
                    objBathSS.Sink_Fuacet_WhereToOrder = dr["Sink_Fuacet_WhereToOrder"].ToString();
                    objBathSS.Sink_Drain_Qty = dr["Sink_Drain_Qty"].ToString();
                    objBathSS.Sink_Drain_Style = dr["Sink_Drain_Style"].ToString();
                    objBathSS.Sink_Drain_WhereToOrder = dr["Sink_Drain_WhereToOrder"].ToString();
                    objBathSS.Sink_Valve_Qty = dr["Sink_Valve_Qty"].ToString();
                    objBathSS.Sink_Valve_Style = dr["Sink_Valve_Style"].ToString();
                    objBathSS.Sink_Valve_WhereToOrder = dr["Sink_Valve_WhereToOrder"].ToString();
                    objBathSS.Bathtub_Qty = dr["Bathtub_Qty"].ToString();
                    objBathSS.Bathtub_Style = dr["Bathtub_Style"].ToString();
                    objBathSS.Bathtub_WhereToOrder = dr["Bathtub_WhereToOrder"].ToString();
                    objBathSS.Tub_Faucet_Qty = dr["Tub_Faucet_Qty"].ToString();
                    objBathSS.Tub_Faucet_Style = dr["Tub_Faucet_Style"].ToString();
                    objBathSS.Tub_Faucet_WhereToOrder = dr["Tub_Faucet_WhereToOrder"].ToString();
                    objBathSS.Tub_Valve_Qty = dr["Tub_Valve_Qty"].ToString();
                    objBathSS.Tub_Valve_Style = dr["Tub_Valve_Style"].ToString();
                    objBathSS.Tub_Valve_WhereToOrder = dr["Tub_Valve_WhereToOrder"].ToString();
                    objBathSS.Tub_Drain_Qty = dr["Tub_Drain_Qty"].ToString();
                    objBathSS.Tub_Drain_Style = dr["Tub_Drain_Style"].ToString();
                    objBathSS.Tub_Drain_WhereToOrder = dr["Tub_Drain_WhereToOrder"].ToString();
                    objBathSS.Tollet_Qty = dr["Tollet_Qty"].ToString();
                    objBathSS.Tollet_Style = dr["Tollet_Style"].ToString();
                    objBathSS.Tollet_WhereToOrder = dr["Tollet_WhereToOrder"].ToString();
                    objBathSS.Shower_TubSystem_Qty = dr["Shower_TubSystem_Qty"].ToString();
                    objBathSS.Shower_TubSystem_Style = dr["Shower_TubSystem_Style"].ToString();
                    objBathSS.Shower_TubSystem_WhereToOrder = dr["Shower_TubSystem_WhereToOrder"].ToString();
                    objBathSS.Shower_Value_Qty = dr["Shower_Value_Qty"].ToString();
                    objBathSS.Shower_Value_Style = dr["Shower_Value_Style"].ToString();
                    objBathSS.Shower_Value_WhereToOrder = dr["Shower_Value_WhereToOrder"].ToString();
                    objBathSS.Handheld_Shower_Qty = dr["Handheld_Shower_Qty"].ToString();
                    objBathSS.Handheld_Shower_Style = dr["Handheld_Shower_Style"].ToString();
                    objBathSS.Handheld_Shower_WhereToOrder = dr["Handheld_Shower_WhereToOrder"].ToString();
                    objBathSS.Body_Spray_Qty = dr["Body_Spray_Qty"].ToString();
                    objBathSS.Body_Spray_Style = dr["Body_Spray_Style"].ToString();
                    objBathSS.Body_Spray_WhereToOrder = dr["Body_Spray_WhereToOrder"].ToString();
                    objBathSS.Body_Spray_Valve_Qty = dr["Body_Spray_Valve_Qty"].ToString();
                    objBathSS.Body_Spray_Valve_Style = dr["Body_Spray_Valve_Style"].ToString();
                    objBathSS.Body_Spray_Valve_WhereToOrder = dr["Body_Spray_Valve_WhereToOrder"].ToString();
                    objBathSS.Shower_Drain_Qty = dr["Shower_Drain_Qty"].ToString();
                    objBathSS.Shower_Drain_Style = dr["Shower_Drain_Style"].ToString();
                    objBathSS.Shower_Drain_WhereToOrder = dr["Shower_Drain_WhereToOrder"].ToString();
                    objBathSS.Shower_Drain_Body_Plug_Qty = dr["Shower_Drain_Body_Plug_Qty"].ToString();
                    objBathSS.Shower_Drain_Body_Plug_Style = dr["Shower_Drain_Body_Plug_Style"].ToString();
                    objBathSS.Shower_Drain_Body_Plug_WhereToOrder = dr["Shower_Drain_Body_Plug_WhereToOrder"].ToString();
                    objBathSS.Shower_Drain_Cover_Qty = dr["Shower_Drain_Cover_Qty"].ToString();
                    objBathSS.Shower_Drain_Cover_Style = dr["Shower_Drain_Cover_Style"].ToString();
                    objBathSS.Shower_Drain_Cover_WhereToOrder = dr["Shower_Drain_Cover_WhereToOrder"].ToString();
                    objBathSS.Counter_Top_Qty = dr["Counter_Top_Qty"].ToString();
                    objBathSS.Counter_Top_Style = dr["Counter_Top_Style"].ToString();
                    objBathSS.Counter_Top_WhereToOrder = dr["Counter_Top_WhereToOrder"].ToString();
                    objBathSS.Counter_To_Edge_Qty = dr["Counter_To_Edge_Qty"].ToString();
                    objBathSS.Counter_To_Edge_Style = dr["Counter_To_Edge_Style"].ToString();
                    objBathSS.Counter_To_Edge_WhereToOrder = dr["Counter_To_Edge_WhereToOrder"].ToString();
                    objBathSS.Counter_Top_Overhang_Qty = dr["Counter_Top_Overhang_Qty"].ToString();
                    objBathSS.Counter_Top_Overhang_Style = dr["Counter_Top_Overhang_Style"].ToString();
                    objBathSS.Counter_Top_Overhang_WhereToOrder = dr["Counter_Top_Overhang_WhereToOrder"].ToString();
                    objBathSS.AdditionalPlacesGettingCountertop_Qty = dr["AdditionalPlacesGettingCountertop_Qty"].ToString();
                    objBathSS.AdditionalPlacesGettingCountertop_Style = dr["AdditionalPlacesGettingCountertop_Style"].ToString();
                    objBathSS.AdditionalPlacesGettingCountertop_WhereToOrder = dr["AdditionalPlacesGettingCountertop_WhereToOrder"].ToString();
                    objBathSS.Granite_Quartz_Backsplash_Qty = dr["Granite_Quartz_Backsplash_Qty"].ToString();
                    objBathSS.Granite_Quartz_Backsplash_Style = dr["Granite_Quartz_Backsplash_Style"].ToString();
                    objBathSS.Granite_Quartz_Backsplash_WhereToOrder = dr["Granite_Quartz_Backsplash_WhereToOrder"].ToString();
                    objBathSS.Tub_Wall_Tile_Qty = dr["Tub_Wall_Tile_Qty"].ToString();
                    objBathSS.Tub_Wall_Tile_Style = dr["Tub_Wall_Tile_Style"].ToString();
                    objBathSS.Tub_Wall_Tile_WhereToOrder = dr["Tub_Wall_Tile_WhereToOrder"].ToString();
                    objBathSS.Wall_Tile_Layout_Qty = dr["Wall_Tile_Layout_Qty"].ToString();
                    objBathSS.Wall_Tile_Layout_Style = dr["Wall_Tile_Layout_Style"].ToString();
                    objBathSS.Wall_Tile_Layout_WhereToOrder = dr["Wall_Tile_Layout_WhereToOrder"].ToString();
                    objBathSS.Tub_skirt_tile_Qty = dr["Tub_skirt_tile_Qty"].ToString();
                    objBathSS.Tub_skirt_tile_Style = dr["Tub_skirt_tile_Style"].ToString();
                    objBathSS.Tub_skirt_tile_WhereToOrder = dr["Tub_skirt_tile_WhereToOrder"].ToString();
                    objBathSS.Shower_Wall_Tile_Qty = dr["Shower_Wall_Tile_Qty"].ToString();
                    objBathSS.Shower_Wall_Tile_Style = dr["Shower_Wall_Tile_Style"].ToString();
                    objBathSS.Shower_Wall_Tile_WhereToOrder = dr["Shower_Wall_Tile_WhereToOrder"].ToString();
                    objBathSS.Wall_Tile_Layout_Qty = dr["Wall_Tile_Layout_Qty"].ToString();
                    objBathSS.Wall_Tile_Layout_Style = dr["Wall_Tile_Layout_Style"].ToString();
                    objBathSS.Wall_Tile_Layout_WhereToOrder = dr["Wall_Tile_Layout_WhereToOrder"].ToString();
                    objBathSS.Shower_Floor_Tile_Qty = dr["Shower_Floor_Tile_Qty"].ToString();
                    objBathSS.Shower_Floor_Tile_Style = dr["Shower_Floor_Tile_Style"].ToString();
                    objBathSS.Shower_Floor_Tile_WhereToOrder = dr["Shower_Floor_Tile_WhereToOrder"].ToString();
                    objBathSS.Shower_Tub_Tile_Height_Qty = dr["Shower_Tub_Tile_Height_Qty"].ToString();
                    objBathSS.Shower_Tub_Tile_Height_Style = dr["Shower_Tub_Tile_Height_Style"].ToString();
                    objBathSS.Shower_Tub_Tile_Height_WhereToOrder = dr["Shower_Tub_Tile_Height_WhereToOrder"].ToString();
                    objBathSS.Floor_Tile_Qty = dr["Floor_Tile_Qty"].ToString();
                    objBathSS.Floor_Tile_Style = dr["Floor_Tile_Style"].ToString();
                    objBathSS.Floor_Tile_WhereToOrder = dr["Floor_Tile_WhereToOrder"].ToString();
                    objBathSS.Floor_Tile_layout_Qty = dr["Floor_Tile_layout_Qty"].ToString();
                    objBathSS.Floor_Tile_layout_Style = dr["Floor_Tile_layout_Style"].ToString();
                    objBathSS.Floor_Tile_layout_WhereToOrder = dr["Floor_Tile_layout_WhereToOrder"].ToString();
                    objBathSS.BullnoseTile_Qty = dr["BullnoseTile_Qty"].ToString();
                    objBathSS.BullnoseTile_Style = dr["BullnoseTile_Style"].ToString();
                    objBathSS.BullnoseTile_WhereToOrder = dr["BullnoseTile_WhereToOrder"].ToString();
                    objBathSS.Deco_Band_Qty = dr["Deco_Band_Qty"].ToString();
                    objBathSS.Deco_Band_Style = dr["Deco_Band_Style"].ToString();
                    objBathSS.Deco_Band_WhereToOrder = dr["Deco_Band_WhereToOrder"].ToString();
                    objBathSS.Deco_Band_Height_Qty = dr["Deco_Band_Height_Qty"].ToString();
                    objBathSS.Deco_Band_Height_Style = dr["Deco_Band_Height_Style"].ToString();
                    objBathSS.Deco_Band_Height_WhereToOrder = dr["Deco_Band_Height_WhereToOrder"].ToString();
                    objBathSS.Tile_Baseboard_Qty = dr["Tile_Baseboard_Qty"].ToString();
                    objBathSS.Tile_Baseboard_Style = dr["Tile_Baseboard_Style"].ToString();
                    objBathSS.Tile_Baseboard_WhereToOrder = dr["Tile_Baseboard_WhereToOrder"].ToString();
                    objBathSS.Grout_Selection_Qty = dr["Grout_Selection_Qty"].ToString();
                    objBathSS.Grout_Selection_Style = dr["Grout_Selection_Style"].ToString();
                    objBathSS.Grout_Selection_WhereToOrder = dr["Grout_Selection_WhereToOrder"].ToString();
                    objBathSS.Niche_Location_Qty = dr["Niche_Location_Qty"].ToString();
                    objBathSS.Niche_Location_Style = dr["Niche_Location_Style"].ToString();
                    objBathSS.Niche_Location_WhereToOrder = dr["Niche_Location_WhereToOrder"].ToString();
                    objBathSS.Niche_Size_Qty = dr["Niche_Size_Qty"].ToString();
                    objBathSS.Niche_Size_Style = dr["Niche_Size_Style"].ToString();
                    objBathSS.Niche_Size_WhereToOrder = dr["Niche_Size_WhereToOrder"].ToString();
                    objBathSS.Glass_Qty = dr["Glass_Qty"].ToString();
                    objBathSS.Glass_Style = dr["Glass_Style"].ToString();
                    objBathSS.Glass_WhereToOrder = dr["Glass_WhereToOrder"].ToString();
                    objBathSS.Window_Qty = dr["Window_Qty"].ToString();
                    objBathSS.Window_Style = dr["Window_Style"].ToString();
                    objBathSS.Window_WhereToOrder = dr["Window_WhereToOrder"].ToString();
                    objBathSS.Door_Qty = dr["Door_Qty"].ToString();
                    objBathSS.Door_Style = dr["Door_Style"].ToString();
                    objBathSS.Door_WhereToOrder = dr["Door_WhereToOrder"].ToString();
                    objBathSS.Grab_Bar_Qty = dr["Grab_Bar_Qty"].ToString();
                    objBathSS.Grab_Bar_Style = dr["Grab_Bar_Style"].ToString();
                    objBathSS.Grab_Bar_WhereToOrder = dr["Grab_Bar_WhereToOrder"].ToString();
                    objBathSS.Cabinet_Door_Style_Color_Qty = dr["Cabinet_Door_Style_Color_Qty"].ToString();
                    objBathSS.Cabinet_Door_Style_Color_Style = dr["Cabinet_Door_Style_Color_Style"].ToString();
                    objBathSS.Cabinet_Door_Style_Color_WhereToOrder = dr["Cabinet_Door_Style_Color_WhereToOrder"].ToString();
                    objBathSS.Medicine_Cabinet_Qty = dr["Medicine_Cabinet_Qty"].ToString();
                    objBathSS.Medicine_Cabinet_Style = dr["Medicine_Cabinet_Style"].ToString();
                    objBathSS.Medicine_Cabinet_WhereToOrder = dr["Medicine_Cabinet_WhereToOrder"].ToString();
                    objBathSS.Mirror_Qty = dr["Mirror_Qty"].ToString();
                    objBathSS.Mirror_Style = dr["Mirror_Style"].ToString();
                    objBathSS.Mirror_WhereToOrder = dr["Mirror_WhereToOrder"].ToString();
                    objBathSS.Wood_Baseboard_Qty = dr["Wood_Baseboard_Qty"].ToString();
                    objBathSS.Wood_Baseboard_Style = dr["Wood_Baseboard_Style"].ToString();
                    objBathSS.Wood_Baseboard_WhereToOrder = dr["Wood_Baseboard_WhereToOrder"].ToString();
                    objBathSS.Paint_Color_Qty = dr["Paint_Color_Qty"].ToString();
                    objBathSS.Paint_Color_Style = dr["Paint_Color_Style"].ToString();
                    objBathSS.Paint_Color_WhereToOrder = dr["Paint_Color_WhereToOrder"].ToString();
                    objBathSS.Lighting_Qty = dr["Lighting_Qty"].ToString();
                    objBathSS.Lighting_Style = dr["Lighting_Style"].ToString();
                    objBathSS.Lighting_WhereToOrder = dr["Lighting_WhereToOrder"].ToString();
                    objBathSS.Hardware_Qty = dr["Hardware_Qty"].ToString();
                    objBathSS.Hardware_Style = dr["Hardware_Style"].ToString();
                    objBathSS.Hardware_WhereToOrder = dr["Hardware_WhereToOrder"].ToString();

                    objBathSS.TowelRing_Qty = dr["TowelRing_Qty"].ToString();
                    objBathSS.TowelRing_Style = dr["TowelRing_Style"].ToString();
                    objBathSS.TowelRing_WhereToOrder = dr["TowelRing_WhereToOrder"].ToString();
                    objBathSS.TowelBar_Qty = dr["TowelBar_Qty"].ToString();
                    objBathSS.TowelBar_Style = dr["TowelBar_Style"].ToString();
                    objBathSS.TowelBar_WhereToOrder = dr["TowelBar_WhereToOrder"].ToString();
                    objBathSS.TissueHolder_Qty = dr["TissueHolder_Qty"].ToString();
                    objBathSS.TissueHolder_Style = dr["TissueHolder_Style"].ToString();
                    objBathSS.TissueHolder_WhereToOrder = dr["TissueHolder_WhereToOrder"].ToString();
                    objBathSS.ClosetDoorSeries = dr["ClosetDoorSeries"].ToString();
                    objBathSS.ClosetDoorOpeningSize = dr["ClosetDoorOpeningSize"].ToString();
                    objBathSS.ClosetDoorNumberOfPanels = dr["ClosetDoorNumberOfPanels"].ToString();
                    objBathSS.ClosetDoorFinish = dr["ClosetDoorFinish"].ToString();
                    objBathSS.ClosetDoorInsert = dr["ClosetDoorInsert"].ToString();

                    objBathSS.Special_Notes = dr["Special_Notes"].ToString();
                    objBathSS.UpdateBy = User.Identity.Name;
                    objBathSS.LastUpdatedDate = DateTime.Now;


                    if (bFlagNew)
                    {
                        _db.BathroomSheetSelections.InsertOnSubmit(objBathSS);
                    }
                }


                lblBathroomResult.Text = csCommonUtility.GetSystemMessage("Data has been saved successfully");

                _db.SubmitChanges();
                GetBathroomSheet();
            }
        }
        catch (Exception ex)
        {
            lblBathroomResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void grdBathroomSelectionSheet_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nBathSheetID = Convert.ToInt32(grdBathroomSelectionSheet.DataKeys[e.Row.RowIndex].Values[0].ToString());

            LinkButton lnkDeleteBath = (LinkButton)e.Row.FindControl("lnkDeleteBath");

            lnkDeleteBath.Attributes["CommandArgument"] = string.Format("{0}", nBathSheetID);

            if (nBathSheetID > 0)
                lnkDeleteBath.Visible = true;

        }
    }

    private DataTable LoadBathRoomTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("BathroomID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("BathroomSheetName", typeof(string));
        table.Columns.Add("Sink_Qty", typeof(string));
        table.Columns.Add("Sink_Style", typeof(string));
        table.Columns.Add("Sink_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Fuacet_Qty", typeof(string));
        table.Columns.Add("Sink_Fuacet_Style", typeof(string));
        table.Columns.Add("Sink_Fuacet_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Drain_Qty", typeof(string));
        table.Columns.Add("Sink_Drain_Style", typeof(string));
        table.Columns.Add("Sink_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Valve_Qty", typeof(string));
        table.Columns.Add("Sink_Valve_Style", typeof(string));
        table.Columns.Add("Sink_Valve_WhereToOrder", typeof(string));
        table.Columns.Add("Bathtub_Qty", typeof(string));
        table.Columns.Add("Bathtub_Style", typeof(string));
        table.Columns.Add("Bathtub_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Faucet_Qty", typeof(string));
        table.Columns.Add("Tub_Faucet_Style", typeof(string));
        table.Columns.Add("Tub_Faucet_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Valve_Qty", typeof(string));
        table.Columns.Add("Tub_Valve_Style", typeof(string));
        table.Columns.Add("Tub_Valve_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Drain_Qty", typeof(string));
        table.Columns.Add("Tub_Drain_Style", typeof(string));
        table.Columns.Add("Tub_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Tollet_Qty", typeof(string));
        table.Columns.Add("Tollet_Style", typeof(string));
        table.Columns.Add("Tollet_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_TubSystem_Qty", typeof(string));
        table.Columns.Add("Shower_TubSystem_Style", typeof(string));
        table.Columns.Add("Shower_TubSystem_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Value_Qty", typeof(string));
        table.Columns.Add("Shower_Value_Style", typeof(string));
        table.Columns.Add("Shower_Value_WhereToOrder", typeof(string));
        table.Columns.Add("Handheld_Shower_Qty", typeof(string));
        table.Columns.Add("Handheld_Shower_Style", typeof(string));
        table.Columns.Add("Handheld_Shower_WhereToOrder", typeof(string));
        table.Columns.Add("Body_Spray_Qty", typeof(string));
        table.Columns.Add("Body_Spray_Style", typeof(string));
        table.Columns.Add("Body_Spray_WhereToOrder", typeof(string));
        table.Columns.Add("Body_Spray_Valve_Qty", typeof(string));
        table.Columns.Add("Body_Spray_Valve_Style", typeof(string));
        table.Columns.Add("Body_Spray_Valve_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Drain_Qty", typeof(string));
        table.Columns.Add("Shower_Drain_Style", typeof(string));
        table.Columns.Add("Shower_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Drain_Body_Plug_Qty", typeof(string));
        table.Columns.Add("Shower_Drain_Body_Plug_Style", typeof(string));
        table.Columns.Add("Shower_Drain_Body_Plug_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Drain_Cover_Qty", typeof(string));
        table.Columns.Add("Shower_Drain_Cover_Style", typeof(string));
        table.Columns.Add("Shower_Drain_Cover_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Style", typeof(string));
        table.Columns.Add("Counter_Top_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_To_Edge_Qty", typeof(string));
        table.Columns.Add("Counter_To_Edge_Style", typeof(string));
        table.Columns.Add("Counter_To_Edge_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Style", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_WhereToOrder", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Qty", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Style", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_WhereToOrder", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Qty", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Style", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_Wall_Tile_Qty", typeof(string));
        table.Columns.Add("Tub_Wall_Tile_Style", typeof(string));
        table.Columns.Add("Tub_Wall_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Wall_Tile_Layout_Qty", typeof(string));
        table.Columns.Add("Wall_Tile_Layout_Style", typeof(string));
        table.Columns.Add("Wall_Tile_Layout_WhereToOrder", typeof(string));
        table.Columns.Add("Tub_skirt_tile_Qty", typeof(string));
        table.Columns.Add("Tub_skirt_tile_Style", typeof(string));
        table.Columns.Add("Tub_skirt_tile_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Wall_Tile_Qty", typeof(string));
        table.Columns.Add("Shower_Wall_Tile_Style", typeof(string));
        table.Columns.Add("Shower_Wall_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Floor_Tile_Qty", typeof(string));
        table.Columns.Add("Shower_Floor_Tile_Style", typeof(string));
        table.Columns.Add("Shower_Floor_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Shower_Tub_Tile_Height_Qty", typeof(string));
        table.Columns.Add("Shower_Tub_Tile_Height_Style", typeof(string));
        table.Columns.Add("Shower_Tub_Tile_Height_WhereToOrder", typeof(string));
        table.Columns.Add("Floor_Tile_Qty", typeof(string));
        table.Columns.Add("Floor_Tile_Style", typeof(string));
        table.Columns.Add("Floor_Tile_WhereToOrder", typeof(string));
        table.Columns.Add("Floor_Tile_layout_Qty", typeof(string));
        table.Columns.Add("Floor_Tile_layout_Style", typeof(string));
        table.Columns.Add("Floor_Tile_layout_WhereToOrder", typeof(string));
        table.Columns.Add("BullnoseTile_Qty", typeof(string));
        table.Columns.Add("BullnoseTile_Style", typeof(string));
        table.Columns.Add("BullnoseTile_WhereToOrder", typeof(string));
        table.Columns.Add("Deco_Band_Qty", typeof(string));
        table.Columns.Add("Deco_Band_Style", typeof(string));
        table.Columns.Add("Deco_Band_WhereToOrder", typeof(string));
        table.Columns.Add("Deco_Band_Height_Qty", typeof(string));
        table.Columns.Add("Deco_Band_Height_Style", typeof(string));
        table.Columns.Add("Deco_Band_Height_WhereToOrder", typeof(string));
        table.Columns.Add("Tile_Baseboard_Qty", typeof(string));
        table.Columns.Add("Tile_Baseboard_Style", typeof(string));
        table.Columns.Add("Tile_Baseboard_WhereToOrder", typeof(string));
        table.Columns.Add("Grout_Selection_Qty", typeof(string));
        table.Columns.Add("Grout_Selection_Style", typeof(string));
        table.Columns.Add("Grout_Selection_WhereToOrder", typeof(string));
        table.Columns.Add("Niche_Location_Qty", typeof(string));
        table.Columns.Add("Niche_Location_Style", typeof(string));
        table.Columns.Add("Niche_Location_WhereToOrder", typeof(string));
        table.Columns.Add("Niche_Size_Qty", typeof(string));
        table.Columns.Add("Niche_Size_Style", typeof(string));
        table.Columns.Add("Niche_Size_WhereToOrder", typeof(string));
        table.Columns.Add("Glass_Qty", typeof(string));
        table.Columns.Add("Glass_Style", typeof(string));
        table.Columns.Add("Glass_WhereToOrder", typeof(string));
        table.Columns.Add("Window_Qty", typeof(string));
        table.Columns.Add("Window_Style", typeof(string));
        table.Columns.Add("Window_WhereToOrder", typeof(string));
        table.Columns.Add("Door_Qty", typeof(string));
        table.Columns.Add("Door_Style", typeof(string));
        table.Columns.Add("Door_WhereToOrder", typeof(string));
        table.Columns.Add("Grab_Bar_Qty", typeof(string));
        table.Columns.Add("Grab_Bar_Style", typeof(string));
        table.Columns.Add("Grab_Bar_WhereToOrder", typeof(string));
        table.Columns.Add("Cabinet_Door_Style_Color_Qty", typeof(string));
        table.Columns.Add("Cabinet_Door_Style_Color_Style", typeof(string));
        table.Columns.Add("Cabinet_Door_Style_Color_WhereToOrder", typeof(string));
        table.Columns.Add("Medicine_Cabinet_Qty", typeof(string));
        table.Columns.Add("Medicine_Cabinet_Style", typeof(string));
        table.Columns.Add("Medicine_Cabinet_WhereToOrder", typeof(string));
        table.Columns.Add("Mirror_Qty", typeof(string));
        table.Columns.Add("Mirror_Style", typeof(string));
        table.Columns.Add("Mirror_WhereToOrder", typeof(string));
        table.Columns.Add("Wood_Baseboard_Qty", typeof(string));
        table.Columns.Add("Wood_Baseboard_Style", typeof(string));
        table.Columns.Add("Wood_Baseboard_WhereToOrder", typeof(string));
        table.Columns.Add("Paint_Color_Qty", typeof(string));
        table.Columns.Add("Paint_Color_Style", typeof(string));
        table.Columns.Add("Paint_Color_WhereToOrder", typeof(string));
        table.Columns.Add("Lighting_Qty", typeof(string));
        table.Columns.Add("Lighting_Style", typeof(string));
        table.Columns.Add("Lighting_WhereToOrder", typeof(string));
        table.Columns.Add("Hardware_Qty", typeof(string));
        table.Columns.Add("Hardware_Style", typeof(string));
        table.Columns.Add("Hardware_WhereToOrder", typeof(string));

        table.Columns.Add("TowelRing_Qty", typeof(string));
        table.Columns.Add("TowelRing_Style", typeof(string));
        table.Columns.Add("TowelRing_WhereToOrder", typeof(string));
        table.Columns.Add("TowelBar_Qty", typeof(string));
        table.Columns.Add("TowelBar_Style", typeof(string));
        table.Columns.Add("TowelBar_WhereToOrder", typeof(string));
        table.Columns.Add("TissueHolder_Qty", typeof(string));
        table.Columns.Add("TissueHolder_Style", typeof(string));
        table.Columns.Add("TissueHolder_WhereToOrder", typeof(string));
        table.Columns.Add("ClosetDoorSeries", typeof(string));
        table.Columns.Add("ClosetDoorOpeningSize", typeof(string));
        table.Columns.Add("ClosetDoorNumberOfPanels", typeof(string));
        table.Columns.Add("ClosetDoorFinish", typeof(string));
        table.Columns.Add("ClosetDoorInsert", typeof(string));

        table.Columns.Add("Special_Notes", typeof(string));
        table.Columns.Add("LastUpdatedDate", typeof(DateTime));
        table.Columns.Add("UpdateBy", typeof(string));

        return table;
    }

    protected void btnAddBathItem_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddBathItem.ID, btnAddBathItem.GetType().Name, "Click"); 
        BathroomSheetSelection objBathSS = new BathroomSheetSelection();
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable table = (DataTable)Session["sBathroomSection"];

        int nBathSheetID = Convert.ToInt32(hdnBathroomSheetID.Value);
        bool contains = table.AsEnumerable().Any(row => nBathSheetID == row.Field<int>("BathroomID"));
        if (contains)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("You have already a pending item to save.");
            return;
        }

        foreach (GridViewRow row in grdBathroomSelectionSheet.Rows)
        {
            DataRow dr = table.Rows[row.RowIndex];

            TextBox txtBathSheetName = (TextBox)row.FindControl("txtBathSheetName");
            Label lblBathSheetName = (Label)row.FindControl("lblBathSheetName");

            TextBox txtSinkQty = (TextBox)row.FindControl("txtSinkQty");
            TextBox txtSinkStyle = (TextBox)row.FindControl("txtSinkStyle");
            TextBox txtSinkOrder = (TextBox)row.FindControl("txtSinkOrder");

            TextBox txtSinkFaucentQty = (TextBox)row.FindControl("txtSinkFaucentQty");
            TextBox txtSinkFaucentStyle = (TextBox)row.FindControl("txtSinkFaucentStyle");
            TextBox txtSinkFaucentOrder = (TextBox)row.FindControl("txtSinkFaucentOrder");

            TextBox txtSinkDrainQty = (TextBox)row.FindControl("txtSinkDrainQty");
            TextBox txtSinkDrainStyle = (TextBox)row.FindControl("txtSinkDrainStyle");
            TextBox txtSinkdrainOrder = (TextBox)row.FindControl("txtSinkdrainOrder");

            TextBox txtSinkValveQty = (TextBox)row.FindControl("txtSinkValveQty");
            TextBox txtSinkValveStyle = (TextBox)row.FindControl("txtSinkValveStyle");
            TextBox txtSinkValveOrder = (TextBox)row.FindControl("txtSinkValveOrder");

            TextBox txtBathTubQty = (TextBox)row.FindControl("txtBathTubQty");
            TextBox txtBathTubStyle = (TextBox)row.FindControl("txtBathTubStyle");
            TextBox txtBathTubOrder = (TextBox)row.FindControl("txtBathTubOrder");

            TextBox txtTubFaucentQty = (TextBox)row.FindControl("txtTubFaucentQty");
            TextBox txtTubFaucentStyle = (TextBox)row.FindControl("txtTubFaucentStyle");
            TextBox txtTubFaucentOrder = (TextBox)row.FindControl("txtTubFaucentOrder");

            TextBox txtTubValveQty = (TextBox)row.FindControl("txtTubValveQty");
            TextBox txtTubValveStyle = (TextBox)row.FindControl("txtTubValveStyle");
            TextBox txtTubValveOrder = (TextBox)row.FindControl("txtTubValveOrder");

            TextBox txtTubDrainQty = (TextBox)row.FindControl("txtTubDrainQty");
            TextBox txtTubDrainStyle = (TextBox)row.FindControl("txtTubDrainStyle");
            TextBox txtTubDrainOrder = (TextBox)row.FindControl("txtTubDrainOrder");

            TextBox txtToiletQty = (TextBox)row.FindControl("txtToiletQty");
            TextBox txtToiletStyle = (TextBox)row.FindControl("txtToiletStyle");
            TextBox txtToiletOrder = (TextBox)row.FindControl("txtToiletOrder");

            TextBox txtShower_TubSystemQty = (TextBox)row.FindControl("txtShower_TubSystemQty");
            TextBox txtShower_TubSystemStyle = (TextBox)row.FindControl("txtShower_TubSystemStyle");
            TextBox txtShower_TubSystemOrder = (TextBox)row.FindControl("txtShower_TubSystemOrder");

            TextBox txtShowerValveQty = (TextBox)row.FindControl("txtShowerValveQty");
            TextBox txtShowerValveStyle = (TextBox)row.FindControl("txtShowerValveStyle");
            TextBox txtShowerValveOrder = (TextBox)row.FindControl("txtShowerValveOrder");

            TextBox txtHandheldShowerQty = (TextBox)row.FindControl("txtHandheldShowerQty");
            TextBox txtHandheldShowerStyle = (TextBox)row.FindControl("txtHandheldShowerStyle");
            TextBox txtHandheldShowerOrder = (TextBox)row.FindControl("txtHandheldShowerOrder");

            TextBox txtBodySprayQty = (TextBox)row.FindControl("txtBodySprayQty");
            TextBox txtBodySprayStyle = (TextBox)row.FindControl("txtBodySprayStyle");
            TextBox txtBodySprayOrder = (TextBox)row.FindControl("txtBodySprayOrder");

            TextBox txtBodySprayValveQty = (TextBox)row.FindControl("txtBodySprayValveQty");
            TextBox txtBodySprayValveStyle = (TextBox)row.FindControl("txtBodySprayValveStyle");
            TextBox txtBodySprayValveOrder = (TextBox)row.FindControl("txtBodySprayValveOrder");

            TextBox txtShowerDrainQty = (TextBox)row.FindControl("txtShowerDrainQty");
            TextBox txtShowerDrainStyle = (TextBox)row.FindControl("txtShowerDrainStyle");
            TextBox txtShowerDrainOrder = (TextBox)row.FindControl("txtShowerDrainOrder");

            TextBox txtShowerDrainBody_PlugQty = (TextBox)row.FindControl("txtShowerDrainBody_PlugQty");
            TextBox txtShowerDrainBody_PlugStyle = (TextBox)row.FindControl("txtShowerDrainBody_PlugStyle");
            TextBox txtShowerDrainBody_PlugOrder = (TextBox)row.FindControl("txtShowerDrainBody_PlugOrder");

            TextBox txtShowerDrainCoverQty = (TextBox)row.FindControl("txtShowerDrainCoverQty");
            TextBox txtShowerDrainCoverStyle = (TextBox)row.FindControl("txtShowerDrainCoverStyle");
            TextBox txtShowerDrainCoverOrder = (TextBox)row.FindControl("txtShowerDrainCoverOrder");

            TextBox txtCounterTopQty = (TextBox)row.FindControl("txtCounterTopQty");
            TextBox txtCounterTopStyle = (TextBox)row.FindControl("txtCounterTopStyle");
            TextBox txtCounterTopOrder = (TextBox)row.FindControl("txtCounterTopOrder");

            TextBox txtCounterTopEdgeQty = (TextBox)row.FindControl("txtCounterTopEdgeQty");
            TextBox txtCounterTopEdgeStyle = (TextBox)row.FindControl("txtCounterTopEdgeStyle");
            TextBox txtCounterTopEdgeOrder = (TextBox)row.FindControl("txtCounterTopEdgeOrder");

            TextBox txtCounterTop_OverhangQty = (TextBox)row.FindControl("txtCounterTop_OverhangQty");
            TextBox txtCounterTop_OverhangStyle = (TextBox)row.FindControl("txtCounterTop_OverhangStyle");
            TextBox txtCounterTop_OverhangOrder = (TextBox)row.FindControl("txtCounterTop_OverhangOrder");

            TextBox txtAdditionalplacesgettingcountertopQty = (TextBox)row.FindControl("txtAdditionalplacesgettingcountertopQty");
            TextBox txtAdditionalplacesgettingcountertopStyle = (TextBox)row.FindControl("txtAdditionalplacesgettingcountertopStyle");
            TextBox txtAdditionalplacesgettingcountertopOrder = (TextBox)row.FindControl("txtAdditionalplacesgettingcountertopOrder");

            TextBox txtGranite_Quartz_BacksplashQty = (TextBox)row.FindControl("txtGranite_Quartz_BacksplashQty");
            TextBox txtGranite_Quartz_BacksplashStyle = (TextBox)row.FindControl("txtGranite_Quartz_BacksplashStyle");
            TextBox txtGranite_Quartz_BacksplashOrder = (TextBox)row.FindControl("txtGranite_Quartz_BacksplashOrder");

            TextBox txtTubwalltileQty = (TextBox)row.FindControl("txtTubwalltileQty");
            TextBox txtTubwalltileStyle = (TextBox)row.FindControl("txtTubwalltileStyle");
            TextBox txtTubwalltileOrder = (TextBox)row.FindControl("txtTubwalltileOrder");

            TextBox txtWallTilelayoutQty = (TextBox)row.FindControl("txtWallTilelayoutQty");
            TextBox txtWallTilelayoutStyle = (TextBox)row.FindControl("txtWallTilelayoutStyle");
            TextBox txtWallTilelayoutOrder = (TextBox)row.FindControl("txtWallTilelayoutOrder");

            TextBox txtTubskirttileQty = (TextBox)row.FindControl("txtTubskirttileQty");
            TextBox txtTubskirttileStyle = (TextBox)row.FindControl("txtTubskirttileStyle");
            TextBox txtTubskirttileOrder = (TextBox)row.FindControl("txtTubskirttileOrder");

            TextBox txtShowerWallTileQty = (TextBox)row.FindControl("txtShowerWallTileQty");
            TextBox txtShowerWallTileStyle = (TextBox)row.FindControl("txtShowerWallTileStyle");
            TextBox txtShowerWallTileOrder = (TextBox)row.FindControl("txtShowerWallTileOrder");

            TextBox txtWall_Tile_layoutQty = (TextBox)row.FindControl("txtWall_Tile_layoutQty");
            TextBox txtWall_Tile_layoutStyle = (TextBox)row.FindControl("txtWall_Tile_layoutStyle");
            TextBox txtWall_Tile_layoutOrder = (TextBox)row.FindControl("txtWall_Tile_layoutOrder");

            TextBox txtShowerFloorTileQty = (TextBox)row.FindControl("txtShowerFloorTileQty");
            TextBox txtShowerFloorTileStyle = (TextBox)row.FindControl("txtShowerFloorTileStyle");
            TextBox txtShowerFloorTileOrder = (TextBox)row.FindControl("txtShowerFloorTileOrder");

            TextBox txtShowerTubTileHeightQty = (TextBox)row.FindControl("txtShowerTubTileHeightQty");
            TextBox txtShowerTubTileHeightStyle = (TextBox)row.FindControl("txtShowerTubTileHeightStyle");
            TextBox txtShowerTubTileHeightOrder = (TextBox)row.FindControl("txtShowerTubTileHeightOrder");

            TextBox txtFloorTiletQty = (TextBox)row.FindControl("txtFloorTiletQty");
            TextBox txtFloorTiletstyle = (TextBox)row.FindControl("txtFloorTiletstyle");
            TextBox txtFloorTiletOrder = (TextBox)row.FindControl("txtFloorTiletOrder");

            TextBox txtFloorTilelayoutQty = (TextBox)row.FindControl("txtFloorTilelayoutQty");
            TextBox txtFloorTilelayoutStyle = (TextBox)row.FindControl("txtFloorTilelayoutStyle");
            TextBox txtFloorTilelayoutOrder = (TextBox)row.FindControl("txtFloorTilelayoutOrder");

            TextBox txtBullnoseTileQty = (TextBox)row.FindControl("txtBullnoseTileQty");
            TextBox txtBullnoseTileStyle = (TextBox)row.FindControl("txtBullnoseTileStyle");
            TextBox txtBullnoseTileOrder = (TextBox)row.FindControl("txtBullnoseTileOrder");

            TextBox txtDecobandQty = (TextBox)row.FindControl("txtDecobandQty");
            TextBox txtDecobandStyle = (TextBox)row.FindControl("txtDecobandStyle");
            TextBox txtDecobandOrder = (TextBox)row.FindControl("txtDecobandOrder");

            TextBox txtDecobandHeightQty = (TextBox)row.FindControl("txtDecobandHeightQty");
            TextBox txtDecobandHeightStyle = (TextBox)row.FindControl("txtDecobandHeightStyle");
            TextBox txtDecobandHeightOrder = (TextBox)row.FindControl("txtDecobandHeightOrder");


            TextBox txtTileBaseboardQty = (TextBox)row.FindControl("txtTileBaseboardQty");
            TextBox txtTileBaseboardStyle = (TextBox)row.FindControl("txtTileBaseboardStyle");
            TextBox txtTileBaseboardOrder = (TextBox)row.FindControl("txtTileBaseboardOrder");

            TextBox txtGroutSelectionQty = (TextBox)row.FindControl("txtGroutSelectionQty");
            TextBox txtGroutSelectionStyle = (TextBox)row.FindControl("txtGroutSelectionStyle");
            TextBox txtGroutSelectionOrder = (TextBox)row.FindControl("txtGroutSelectionOrder");

            TextBox txtNicheLocationQty = (TextBox)row.FindControl("txtNicheLocationQty");
            TextBox txtNicheLocationStyle = (TextBox)row.FindControl("txtNicheLocationStyle");
            TextBox txtNicheLocationOrder = (TextBox)row.FindControl("txtNicheLocationOrder");

            TextBox txtNicheSizeQty = (TextBox)row.FindControl("txtNicheSizeQty");
            TextBox txtNicheSizeStyle = (TextBox)row.FindControl("txtNicheSizeStyle");
            TextBox txtNicheSizeOrder = (TextBox)row.FindControl("txtNicheSizeOrder");

            TextBox txtGlassQty = (TextBox)row.FindControl("txtGlassQty");
            TextBox txtGlassStyle = (TextBox)row.FindControl("txtGlassStyle");
            TextBox txtGlassOrder = (TextBox)row.FindControl("txtGlassOrder");

            TextBox txtWindowQty = (TextBox)row.FindControl("txtWindowQty");
            TextBox txtWindowStyle = (TextBox)row.FindControl("txtWindowStyle");
            TextBox txtWindowOrder = (TextBox)row.FindControl("txtWindowOrder");

            TextBox txtDoorQty = (TextBox)row.FindControl("txtDoorQty");
            TextBox txtDoorStyle = (TextBox)row.FindControl("txtDoorStyle");
            TextBox txtDoorOrder = (TextBox)row.FindControl("txtDoorOrder");

            TextBox txtGrabBarQty = (TextBox)row.FindControl("txtGrabBarQty");
            TextBox txtGrabBarStyle = (TextBox)row.FindControl("txtGrabBarStyle");
            TextBox txtGrabBarOrder = (TextBox)row.FindControl("txtGrabBarOrder");


            TextBox txtCabinetDoorStyleColorQty = (TextBox)row.FindControl("txtCabinetDoorStyleColorQty");
            TextBox txtCabinetDoorStyleColorStyle = (TextBox)row.FindControl("txtCabinetDoorStyleColorStyle");
            TextBox txtCabinetDoorStyleColorOrder = (TextBox)row.FindControl("txtCabinetDoorStyleColorOrder");

            TextBox txtMedicineCabinetQty = (TextBox)row.FindControl("txtMedicineCabinetQty");
            TextBox txtMedicineCabinetStyle = (TextBox)row.FindControl("txtMedicineCabinetStyle");
            TextBox txtMedicineCabinetOrder = (TextBox)row.FindControl("txtMedicineCabinetOrder");

            TextBox txtMirrorQty = (TextBox)row.FindControl("txtMirrorQty");
            TextBox txtMirrorStyle = (TextBox)row.FindControl("txtMirrorStyle");
            TextBox txtMirrorOrder = (TextBox)row.FindControl("txtMirrorOrder");

            TextBox txtWoodBaseboardQty = (TextBox)row.FindControl("txtWoodBaseboardQty");
            TextBox txtWoodBaseboardStyle = (TextBox)row.FindControl("txtWoodBaseboardStyle");
            TextBox txtWoodBaseboardOrder = (TextBox)row.FindControl("txtWoodBaseboardOrder");

            TextBox txtPaintColorQty = (TextBox)row.FindControl("txtPaintColorQty");
            TextBox txtPaintColorStyle = (TextBox)row.FindControl("txtPaintColorStyle");
            TextBox txtPaintColorOrder = (TextBox)row.FindControl("txtPaintColorOrder");

            TextBox txtLightingQty = (TextBox)row.FindControl("txtLightingQty");
            TextBox txtLightingStyle = (TextBox)row.FindControl("txtLightingStyle");
            TextBox txtLightingOrder = (TextBox)row.FindControl("txtLightingOrder");

            TextBox txtHardwareQty = (TextBox)row.FindControl("txtHardwareQty");
            TextBox txtHardwareStyle = (TextBox)row.FindControl("txtHardwareStyle");
            TextBox txtHardwareOrder = (TextBox)row.FindControl("txtHardwareOrder");

            TextBox txtTowelRingQty = (TextBox)row.FindControl("txtTowelRingQty");
            TextBox txtTowelRingStyle = (TextBox)row.FindControl("txtTowelRingStyle");
            TextBox txtTowelRingOrder = (TextBox)row.FindControl("txtTowelRingOrder");

            TextBox txtTowelBarQty = (TextBox)row.FindControl("txtTowelBarQty");
            TextBox txtTowelBarStyle = (TextBox)row.FindControl("txtTowelBarStyle");
            TextBox txtTowelBarOrder = (TextBox)row.FindControl("txtTowelBarOrder");

            TextBox txtTissueHolderQty = (TextBox)row.FindControl("txtTissueHolderQty");
            TextBox txtTissueHolderStyle = (TextBox)row.FindControl("txtTissueHolderStyle");
            TextBox txtTissueHolderOrder = (TextBox)row.FindControl("txtTissueHolderOrder");

            TextBox txtClosetDoorSeries = (TextBox)row.FindControl("txtClosetDoorSeries");
            TextBox txtClosetDoorOpeningSize = (TextBox)row.FindControl("txtClosetDoorOpeningSize");
            TextBox txtClosetDoorNumberOfPanels = (TextBox)row.FindControl("txtClosetDoorNumberOfPanels");
            TextBox txtClosetDoorFinish = (TextBox)row.FindControl("txtClosetDoorFinish");
            TextBox txtClosetDoorInsert = (TextBox)row.FindControl("txtClosetDoorInsert");


            TextBox txtBathpecialNotes = (TextBox)row.FindControl("txtBathpecialNotes");

            dr["BathroomID"] = Convert.ToInt32(grdBathroomSelectionSheet.DataKeys[row.RowIndex].Values[0]);
            dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            dr["BathroomSheetName"] = txtBathSheetName.Text;
            dr["Sink_Qty"] = txtSinkQty.Text;
            dr["Sink_Style"] = txtSinkStyle.Text;
            dr["Sink_WhereToOrder"] = txtSinkOrder.Text;
            dr["Sink_Fuacet_Qty"] = txtSinkFaucentQty.Text;
            dr["Sink_Fuacet_Style"] = txtSinkFaucentStyle.Text;
            dr["Sink_Fuacet_WhereToOrder"] = txtSinkFaucentOrder.Text;
            dr["Sink_Drain_Qty"] = txtSinkDrainQty.Text;
            dr["Sink_Drain_Style"] = txtSinkDrainStyle.Text;
            dr["Sink_Drain_WhereToOrder"] = txtSinkdrainOrder.Text;
            dr["Sink_Valve_Qty"] = txtSinkValveQty.Text;
            dr["Sink_Valve_Style"] = txtSinkValveStyle.Text;
            dr["Sink_Valve_WhereToOrder"] = txtSinkValveOrder.Text;
            dr["Bathtub_Qty"] = txtBathTubQty.Text;
            dr["Bathtub_Style"] = txtBathTubStyle.Text;
            dr["Bathtub_WhereToOrder"] = txtBathTubOrder.Text;
            dr["Tub_Faucet_Qty"] = txtTubFaucentQty.Text;
            dr["Tub_Faucet_Style"] = txtTubFaucentStyle.Text;
            dr["Tub_Faucet_WhereToOrder"] = txtTubFaucentOrder.Text;
            dr["Tub_Valve_Qty"] = txtTubValveQty.Text;
            dr["Tub_Valve_Style"] = txtTubValveStyle.Text;
            dr["Tub_Valve_WhereToOrder"] = txtTubValveOrder.Text;
            dr["Tub_Drain_Qty"] = txtTubDrainQty.Text;
            dr["Tub_Drain_Style"] = txtTubDrainStyle.Text;
            dr["Tub_Drain_WhereToOrder"] = txtTubDrainOrder.Text;
            dr["Tollet_Qty"] = txtToiletQty.Text;
            dr["Tollet_Style"] = txtToiletStyle.Text;
            dr["Tollet_WhereToOrder"] = txtToiletOrder.Text;
            dr["Shower_TubSystem_Qty"] = txtShower_TubSystemQty.Text;
            dr["Shower_TubSystem_Style"] = txtShower_TubSystemStyle.Text;
            dr["Shower_TubSystem_WhereToOrder"] = txtShower_TubSystemOrder.Text;
            dr["Shower_Value_Qty"] = txtShowerValveQty.Text;
            dr["Shower_Value_Style"] = txtShowerValveStyle.Text;
            dr["Shower_Value_WhereToOrder"] = txtShowerValveOrder.Text;
            dr["Handheld_Shower_Qty"] = txtHandheldShowerQty.Text;
            dr["Handheld_Shower_Style"] = txtHandheldShowerStyle.Text;
            dr["Handheld_Shower_WhereToOrder"] = txtHandheldShowerOrder.Text;
            dr["Body_Spray_Qty"] = txtBodySprayQty.Text;
            dr["Body_Spray_Style"] = txtBodySprayStyle.Text;
            dr["Body_Spray_WhereToOrder"] = txtBodySprayOrder.Text;
            dr["Body_Spray_Valve_Qty"] = txtBodySprayValveQty.Text;
            dr["Body_Spray_Valve_Style"] = txtBodySprayValveStyle.Text;
            dr["Body_Spray_Valve_WhereToOrder"] = txtBodySprayValveOrder.Text;
            dr["Shower_Drain_Qty"] = txtShowerDrainQty.Text;
            dr["Shower_Drain_Style"] = txtShowerDrainStyle.Text;
            dr["Shower_Drain_WhereToOrder"] = txtShowerDrainOrder.Text;
            dr["Shower_Drain_Body_Plug_Qty"] = txtShowerDrainBody_PlugQty.Text;
            dr["Shower_Drain_Body_Plug_Style"] = txtShowerDrainBody_PlugStyle.Text;
            dr["Shower_Drain_Body_Plug_WhereToOrder"] = txtShowerDrainBody_PlugOrder.Text;
            dr["Shower_Drain_Cover_Qty"] = txtShowerDrainCoverQty.Text;
            dr["Shower_Drain_Cover_Style"] = txtShowerDrainCoverStyle.Text;
            dr["Shower_Drain_Cover_WhereToOrder"] = txtShowerDrainCoverOrder.Text;
            dr["Counter_Top_Qty"] = txtCounterTopQty.Text;
            dr["Counter_Top_Style"] = txtCounterTopStyle.Text;
            dr["Counter_Top_WhereToOrder"] = txtCounterTopOrder.Text;
            dr["Counter_To_Edge_Qty"] = txtCounterTopEdgeQty.Text;
            dr["Counter_To_Edge_Style"] = txtCounterTopEdgeStyle.Text;
            dr["Counter_To_Edge_WhereToOrder"] = txtCounterTopEdgeOrder.Text;
            dr["Counter_Top_Overhang_Qty"] = txtCounterTop_OverhangQty.Text;
            dr["Counter_Top_Overhang_Style"] = txtCounterTop_OverhangStyle.Text;
            dr["Counter_Top_Overhang_WhereToOrder"] = txtCounterTop_OverhangOrder.Text;
            dr["AdditionalPlacesGettingCountertop_Qty"] = txtAdditionalplacesgettingcountertopQty.Text;
            dr["AdditionalPlacesGettingCountertop_Style"] = txtAdditionalplacesgettingcountertopStyle.Text;
            dr["AdditionalPlacesGettingCountertop_WhereToOrder"] = txtAdditionalplacesgettingcountertopOrder.Text;
            dr["Granite_Quartz_Backsplash_Qty"] = txtGranite_Quartz_BacksplashQty.Text;
            dr["Granite_Quartz_Backsplash_Style"] = txtGranite_Quartz_BacksplashStyle.Text;
            dr["Granite_Quartz_Backsplash_WhereToOrder"] = txtGranite_Quartz_BacksplashOrder.Text;
            dr["Tub_Wall_Tile_Qty"] = txtTubwalltileQty.Text;
            dr["Tub_Wall_Tile_Style"] = txtTubwalltileStyle.Text;
            dr["Tub_Wall_Tile_WhereToOrder"] = txtTubwalltileOrder.Text;
            dr["Wall_Tile_Layout_Qty"] = txtWallTilelayoutQty.Text;
            dr["Wall_Tile_Layout_Style"] = txtWallTilelayoutStyle.Text;
            dr["Wall_Tile_Layout_WhereToOrder"] = txtWallTilelayoutOrder.Text;
            dr["Tub_skirt_tile_Qty"] = txtTubskirttileQty.Text;
            dr["Tub_skirt_tile_Style"] = txtTubskirttileStyle.Text;
            dr["Tub_skirt_tile_WhereToOrder"] = txtTubskirttileOrder.Text;
            dr["Shower_Wall_Tile_Qty"] = txtShowerWallTileQty.Text;
            dr["Shower_Wall_Tile_Style"] = txtShowerWallTileStyle.Text;
            dr["Shower_Wall_Tile_WhereToOrder"] = txtShowerWallTileOrder.Text;
            dr["Wall_Tile_Layout_Qty"] = txtWall_Tile_layoutQty.Text;
            dr["Wall_Tile_Layout_Style"] = txtWall_Tile_layoutStyle.Text;
            dr["Wall_Tile_Layout_WhereToOrder"] = txtWall_Tile_layoutOrder.Text;
            dr["Shower_Floor_Tile_Qty"] = txtShowerFloorTileQty.Text;
            dr["Shower_Floor_Tile_Style"] = txtShowerFloorTileStyle.Text;
            dr["Shower_Floor_Tile_WhereToOrder"] = txtShowerFloorTileOrder.Text;
            dr["Shower_Tub_Tile_Height_Qty"] = txtShowerTubTileHeightQty.Text;
            dr["Shower_Tub_Tile_Height_Style"] = txtShowerTubTileHeightStyle.Text;
            dr["Shower_Tub_Tile_Height_WhereToOrder"] = txtShowerTubTileHeightOrder.Text;
            dr["Floor_Tile_Qty"] = txtFloorTiletQty.Text;
            dr["Floor_Tile_Style"] = txtFloorTiletstyle.Text;
            dr["Floor_Tile_WhereToOrder"] = txtFloorTiletOrder.Text;
            dr["Floor_Tile_layout_Qty"] = txtFloorTilelayoutQty.Text;
            dr["Floor_Tile_layout_Style"] = txtFloorTilelayoutStyle.Text;
            dr["Floor_Tile_layout_WhereToOrder"] = txtFloorTilelayoutOrder.Text;
            dr["BullnoseTile_Qty"] = txtBullnoseTileQty.Text;
            dr["BullnoseTile_Style"] = txtBullnoseTileStyle.Text;
            dr["BullnoseTile_WhereToOrder"] = txtBullnoseTileOrder.Text;
            dr["Deco_Band_Qty"] = txtDecobandQty.Text;
            dr["Deco_Band_Style"] = txtDecobandStyle.Text;
            dr["Deco_Band_WhereToOrder"] = txtDecobandOrder.Text;
            dr["Deco_Band_Height_Qty"] = txtDecobandHeightQty.Text;
            dr["Deco_Band_Height_Style"] = txtDecobandHeightStyle.Text;
            dr["Deco_Band_Height_WhereToOrder"] = txtDecobandHeightOrder.Text;
            dr["Tile_Baseboard_Qty"] = txtTileBaseboardQty.Text;
            dr["Tile_Baseboard_Style"] = txtTileBaseboardStyle.Text;
            dr["Tile_Baseboard_WhereToOrder"] = txtTileBaseboardOrder.Text;
            dr["Grout_Selection_Qty"] = txtGroutSelectionQty.Text;
            dr["Grout_Selection_Style"] = txtGroutSelectionStyle.Text;
            dr["Grout_Selection_WhereToOrder"] = txtGroutSelectionOrder.Text;
            dr["Niche_Location_Qty"] = txtNicheLocationQty.Text;
            dr["Niche_Location_Style"] = txtNicheLocationStyle.Text;
            dr["Niche_Location_WhereToOrder"] = txtNicheLocationOrder.Text;
            dr["Niche_Size_Qty"] = txtNicheSizeQty.Text;
            dr["Niche_Size_Style"] = txtNicheSizeStyle.Text;
            dr["Niche_Size_WhereToOrder"] = txtNicheSizeOrder.Text;
            dr["Glass_Qty"] = txtGlassQty.Text;
            dr["Glass_Style"] = txtGlassStyle.Text;
            dr["Glass_WhereToOrder"] = txtGlassOrder.Text;
            dr["Window_Qty"] = txtWindowQty.Text;
            dr["Window_Style"] = txtWindowStyle.Text;
            dr["Window_WhereToOrder"] = txtWindowOrder.Text;
            dr["Door_Qty"] = txtDoorQty.Text;
            dr["Door_Style"] = txtDoorStyle.Text;
            dr["Door_WhereToOrder"] = txtDoorOrder.Text;
            dr["Grab_Bar_Qty"] = txtGrabBarQty.Text;
            dr["Grab_Bar_Style"] = txtGrabBarStyle.Text;
            dr["Grab_Bar_WhereToOrder"] = txtGrabBarOrder.Text;
            dr["Cabinet_Door_Style_Color_Qty"] = txtCabinetDoorStyleColorQty.Text;
            dr["Cabinet_Door_Style_Color_Style"] = txtCabinetDoorStyleColorStyle.Text;
            dr["Cabinet_Door_Style_Color_WhereToOrder"] = txtCabinetDoorStyleColorOrder.Text;
            dr["Medicine_Cabinet_Qty"] = txtMedicineCabinetQty.Text;
            dr["Medicine_Cabinet_Style"] = txtMedicineCabinetStyle.Text;
            dr["Medicine_Cabinet_WhereToOrder"] = txtMedicineCabinetOrder.Text;
            dr["Mirror_Qty"] = txtMirrorQty.Text;
            dr["Mirror_Style"] = txtMirrorStyle.Text;
            dr["Mirror_WhereToOrder"] = txtMirrorOrder.Text;
            dr["Wood_Baseboard_Qty"] = txtWoodBaseboardQty.Text;
            dr["Wood_Baseboard_Style"] = txtWoodBaseboardStyle.Text;
            dr["Wood_Baseboard_WhereToOrder"] = txtWoodBaseboardOrder.Text;
            dr["Paint_Color_Qty"] = txtPaintColorQty.Text;
            dr["Paint_Color_Style"] = txtPaintColorStyle.Text;
            dr["Paint_Color_WhereToOrder"] = txtPaintColorOrder.Text;
            dr["Lighting_Qty"] = txtLightingQty.Text;
            dr["Lighting_Style"] = txtLightingStyle.Text;
            dr["Lighting_WhereToOrder"] = txtLightingOrder.Text;
            dr["Hardware_Qty"] = txtHardwareQty.Text;
            dr["Hardware_Style"] = txtHardwareStyle.Text;
            dr["Hardware_WhereToOrder"] = txtHardwareOrder.Text;

            dr["TowelRing_Qty"] = txtTowelRingQty.Text;
            dr["TowelRing_Style"] = txtTowelRingStyle.Text;
            dr["TowelRing_WhereToOrder"] = txtTowelRingOrder.Text;
            dr["TowelBar_Qty"] = txtTowelBarQty.Text;
            dr["TowelBar_Style"] = txtTowelBarStyle.Text;
            dr["TowelBar_WhereToOrder"] = txtTowelBarOrder.Text;
            dr["TissueHolder_Qty"] = txtTissueHolderQty.Text;
            dr["TissueHolder_Style"] = txtTissueHolderOrder.Text;
            dr["TissueHolder_WhereToOrder"] = txtTissueHolderOrder.Text;
            dr["ClosetDoorSeries"] = txtClosetDoorSeries.Text;
            dr["ClosetDoorOpeningSize"] = txtClosetDoorOpeningSize.Text;
            dr["ClosetDoorNumberOfPanels"] = txtClosetDoorNumberOfPanels.Text;
            dr["ClosetDoorFinish"] = txtClosetDoorFinish.Text;
            dr["ClosetDoorInsert"] = txtClosetDoorInsert.Text;

            dr["Special_Notes"] = txtBathpecialNotes.Text;
            dr["UpdateBy"] = User.Identity.Name;
            dr["LastUpdatedDate"] = DateTime.Now;
        }

        DataRow drNew = table.NewRow();

        drNew["BathroomID"] = Convert.ToInt32(hdnBathroomSheetID.Value);
        drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
        drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
        drNew["BathroomSheetName"] = "";
        drNew["Sink_Qty"] = "";
        drNew["Sink_Style"] = "";
        drNew["Sink_WhereToOrder"] = "";
        drNew["Sink_Fuacet_Qty"] = "";
        drNew["Sink_Fuacet_Style"] = "";
        drNew["Sink_Fuacet_WhereToOrder"] = "";
        drNew["Sink_Drain_Qty"] = "";
        drNew["Sink_Drain_Style"] = "";
        drNew["Sink_Drain_WhereToOrder"] = "";
        drNew["Sink_Valve_Qty"] = "";
        drNew["Sink_Valve_Style"] = "";
        drNew["Sink_Valve_WhereToOrder"] = "";
        drNew["Bathtub_Qty"] = "";
        drNew["Bathtub_Style"] = "";
        drNew["Bathtub_WhereToOrder"] = "";
        drNew["Tub_Faucet_Qty"] = "";
        drNew["Tub_Faucet_Style"] = "";
        drNew["Tub_Faucet_WhereToOrder"] = "";
        drNew["Tub_Valve_Qty"] = "";
        drNew["Tub_Valve_Style"] = "";
        drNew["Tub_Valve_WhereToOrder"] = "";
        drNew["Tub_Drain_Qty"] = "";
        drNew["Tub_Drain_Style"] = "";
        drNew["Tub_Drain_WhereToOrder"] = "";
        drNew["Tollet_Qty"] = "";
        drNew["Tollet_Style"] = "";
        drNew["Tollet_WhereToOrder"] = "";
        drNew["Shower_TubSystem_Qty"] = "";
        drNew["Shower_TubSystem_Style"] = "";
        drNew["Shower_TubSystem_WhereToOrder"] = "";
        drNew["Shower_Value_Qty"] = "";
        drNew["Shower_Value_Style"] = "";
        drNew["Shower_Value_WhereToOrder"] = "";
        drNew["Handheld_Shower_Qty"] = "";
        drNew["Handheld_Shower_Style"] = "";
        drNew["Handheld_Shower_WhereToOrder"] = "";
        drNew["Body_Spray_Qty"] = "";
        drNew["Body_Spray_Style"] = "";
        drNew["Body_Spray_WhereToOrder"] = "";
        drNew["Body_Spray_Valve_Qty"] = "";
        drNew["Body_Spray_Valve_Style"] = "";
        drNew["Body_Spray_Valve_WhereToOrder"] = "";
        drNew["Shower_Drain_Qty"] = "";
        drNew["Shower_Drain_Style"] = "";
        drNew["Shower_Drain_WhereToOrder"] = "";
        drNew["Shower_Drain_Body_Plug_Qty"] = "";
        drNew["Shower_Drain_Body_Plug_Style"] = "";
        drNew["Shower_Drain_Body_Plug_WhereToOrder"] = "";
        drNew["Shower_Drain_Cover_Qty"] = "";
        drNew["Shower_Drain_Cover_Style"] = "";
        drNew["Shower_Drain_Cover_WhereToOrder"] = "";
        drNew["Counter_Top_Qty"] = "";
        drNew["Counter_Top_Style"] = "";
        drNew["Counter_Top_WhereToOrder"] = "";
        drNew["Counter_To_Edge_Qty"] = "";
        drNew["Counter_To_Edge_Style"] = "";
        drNew["Counter_To_Edge_WhereToOrder"] = "";
        drNew["Counter_Top_Overhang_Qty"] = "";
        drNew["Counter_Top_Overhang_Style"] = "";
        drNew["Counter_Top_Overhang_WhereToOrder"] = "";
        drNew["AdditionalPlacesGettingCountertop_Qty"] = "";
        drNew["AdditionalPlacesGettingCountertop_Style"] = "";
        drNew["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
        drNew["Granite_Quartz_Backsplash_Qty"] = "";
        drNew["Granite_Quartz_Backsplash_Style"] = "";
        drNew["Granite_Quartz_Backsplash_WhereToOrder"] = "";
        drNew["Tub_Wall_Tile_Qty"] = "";
        drNew["Tub_Wall_Tile_Style"] = "";
        drNew["Tub_Wall_Tile_WhereToOrder"] = "";
        drNew["Wall_Tile_Layout_Qty"] = "";
        drNew["Wall_Tile_Layout_Style"] = "";
        drNew["Wall_Tile_Layout_WhereToOrder"] = "";
        drNew["Tub_skirt_tile_Qty"] = "";
        drNew["Tub_skirt_tile_Style"] = "";
        drNew["Tub_skirt_tile_WhereToOrder"] = "";
        drNew["Shower_Wall_Tile_Qty"] = "";
        drNew["Shower_Wall_Tile_Style"] = "";
        drNew["Shower_Wall_Tile_WhereToOrder"] = "";
        drNew["Wall_Tile_Layout_Qty"] = "";
        drNew["Wall_Tile_Layout_Style"] = "";
        drNew["Wall_Tile_Layout_WhereToOrder"] = "";
        drNew["Shower_Floor_Tile_Qty"] = "";
        drNew["Shower_Floor_Tile_Style"] = "";
        drNew["Shower_Floor_Tile_WhereToOrder"] = "";
        drNew["Shower_Tub_Tile_Height_Qty"] = "";
        drNew["Shower_Tub_Tile_Height_Style"] = "";
        drNew["Shower_Tub_Tile_Height_WhereToOrder"] = "";
        drNew["Floor_Tile_Qty"] = "";
        drNew["Floor_Tile_Style"] = "";
        drNew["Floor_Tile_WhereToOrder"] = "";
        drNew["Floor_Tile_layout_Qty"] = "";
        drNew["Floor_Tile_layout_Style"] = "";
        drNew["Floor_Tile_layout_WhereToOrder"] = "";
        drNew["BullnoseTile_Qty"] = "";
        drNew["BullnoseTile_Style"] = "";
        drNew["BullnoseTile_WhereToOrder"] = "";
        drNew["Deco_Band_Qty"] = "";
        drNew["Deco_Band_Style"] = "";
        drNew["Deco_Band_WhereToOrder"] = "";
        drNew["Deco_Band_Height_Qty"] = "";
        drNew["Deco_Band_Height_Style"] = "";
        drNew["Deco_Band_Height_WhereToOrder"] = "";
        drNew["Tile_Baseboard_Qty"] = "";
        drNew["Tile_Baseboard_Style"] = "";
        drNew["Tile_Baseboard_WhereToOrder"] = "";
        drNew["Grout_Selection_Qty"] = "";
        drNew["Grout_Selection_Style"] = "";
        drNew["Grout_Selection_WhereToOrder"] = "";
        drNew["Niche_Location_Qty"] = "";
        drNew["Niche_Location_Style"] = "";
        drNew["Niche_Location_WhereToOrder"] = "";
        drNew["Niche_Size_Qty"] = "";
        drNew["Niche_Size_Style"] = "";
        drNew["Niche_Size_WhereToOrder"] = "";
        drNew["Glass_Qty"] = "";
        drNew["Glass_Style"] = "";
        drNew["Glass_WhereToOrder"] = "";
        drNew["Window_Qty"] = "";
        drNew["Window_Style"] = "";
        drNew["Window_WhereToOrder"] = "";
        drNew["Door_Qty"] = "";
        drNew["Door_Style"] = "";
        drNew["Door_WhereToOrder"] = "";
        drNew["Grab_Bar_Qty"] = "";
        drNew["Grab_Bar_Style"] = "";
        drNew["Grab_Bar_WhereToOrder"] = "";
        drNew["Cabinet_Door_Style_Color_Qty"] = "";
        drNew["Cabinet_Door_Style_Color_Style"] = "";
        drNew["Cabinet_Door_Style_Color_WhereToOrder"] = "";
        drNew["Medicine_Cabinet_Qty"] = "";
        drNew["Medicine_Cabinet_Style"] = "";
        drNew["Medicine_Cabinet_WhereToOrder"] = "";
        drNew["Mirror_Qty"] = "";
        drNew["Mirror_Style"] = "";
        drNew["Mirror_WhereToOrder"] = "";
        drNew["Wood_Baseboard_Qty"] = "";
        drNew["Wood_Baseboard_Style"] = "";
        drNew["Wood_Baseboard_WhereToOrder"] = "";
        drNew["Paint_Color_Qty"] = "";
        drNew["Paint_Color_Style"] = "";
        drNew["Paint_Color_WhereToOrder"] = "";
        drNew["Lighting_Qty"] = "";
        drNew["Lighting_Style"] = "";
        drNew["Lighting_WhereToOrder"] = "";
        drNew["Hardware_Qty"] = "";
        drNew["Hardware_Style"] = "";
        drNew["Hardware_WhereToOrder"] = "";

        drNew["TowelRing_Qty"] = "";
        drNew["TowelRing_Style"] = "";
        drNew["TowelRing_WhereToOrder"] = "";
        drNew["TowelBar_Qty"] = "";
        drNew["TowelBar_Style"] = "";
        drNew["TowelBar_WhereToOrder"] = "";
        drNew["TissueHolder_Qty"] = "";
        drNew["TissueHolder_Style"] = "";
        drNew["TissueHolder_WhereToOrder"] = "";
        drNew["ClosetDoorSeries"] = "";
        drNew["ClosetDoorOpeningSize"] = "";
        drNew["ClosetDoorNumberOfPanels"] = "";
        drNew["ClosetDoorFinish"] = "";
        drNew["ClosetDoorInsert"] = "";

        drNew["Special_Notes"] = "";
        drNew["UpdateBy"] = User.Identity.Name;
        drNew["LastUpdatedDate"] = DateTime.Now;

        table.Rows.InsertAt(drNew, 0);

        Session.Add("sBathroomSection", table);
        grdBathroomSelectionSheet.DataSource = table;
        grdBathroomSelectionSheet.DataKeyNames = new string[] { "BathroomID", "customer_id", "estimate_id" };
        grdBathroomSelectionSheet.DataBind();
        lblBathroomResult.Text = "";

    }

    protected void GetBathroomSheet()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable tmpTable = LoadBathRoomTable();

        var objBathSSList = _db.BathroomSheetSelections.Where(cp => cp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cp.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).ToList();

        foreach (BathroomSheetSelection Bathinfo in objBathSSList)
        {

            DataRow drNew = tmpTable.NewRow();
            drNew["BathroomID"] = Convert.ToInt32(Bathinfo.BathroomID);
            drNew["customer_id"] = Convert.ToInt32(Bathinfo.customer_id);
            drNew["estimate_id"] = Convert.ToInt32(Bathinfo.estimate_id);
            drNew["BathroomSheetName"] = Bathinfo.BathroomSheetName;
            drNew["Sink_Qty"] = Bathinfo.Sink_Qty;
            drNew["Sink_Style"] = Bathinfo.Sink_Style;
            drNew["Sink_WhereToOrder"] = Bathinfo.Sink_WhereToOrder;
            drNew["Sink_Fuacet_Qty"] = Bathinfo.Sink_Fuacet_Qty;
            drNew["Sink_Fuacet_Style"] = Bathinfo.Sink_Fuacet_Style;
            drNew["Sink_Fuacet_WhereToOrder"] = Bathinfo.Sink_Fuacet_WhereToOrder;
            drNew["Sink_Drain_Qty"] = Bathinfo.Sink_Drain_Qty;
            drNew["Sink_Drain_Style"] = Bathinfo.Sink_Drain_Style;
            drNew["Sink_Drain_WhereToOrder"] = Bathinfo.Sink_Drain_WhereToOrder;
            drNew["Sink_Valve_Qty"] = Bathinfo.Sink_Valve_Qty;
            drNew["Sink_Valve_Style"] = Bathinfo.Sink_Valve_Style;
            drNew["Sink_Valve_WhereToOrder"] = Bathinfo.Sink_Valve_WhereToOrder;
            drNew["Bathtub_Qty"] = Bathinfo.Bathtub_Qty;
            drNew["Bathtub_Style"] = Bathinfo.Bathtub_Style;
            drNew["Bathtub_WhereToOrder"] = Bathinfo.Bathtub_WhereToOrder;
            drNew["Tub_Faucet_Qty"] = Bathinfo.Tub_Faucet_Qty;
            drNew["Tub_Faucet_Style"] = Bathinfo.Tub_Faucet_Style;
            drNew["Tub_Faucet_WhereToOrder"] = Bathinfo.Tub_Faucet_WhereToOrder;
            drNew["Tub_Valve_Qty"] = Bathinfo.Tub_Valve_Qty;
            drNew["Tub_Valve_Style"] = Bathinfo.Tub_Valve_Style;
            drNew["Tub_Valve_WhereToOrder"] = Bathinfo.Tub_Valve_WhereToOrder;
            drNew["Tub_Drain_Qty"] = Bathinfo.Tub_Drain_Qty;
            drNew["Tub_Drain_Style"] = Bathinfo.Tub_Drain_Style;
            drNew["Tub_Drain_WhereToOrder"] = Bathinfo.Tub_Drain_WhereToOrder;
            drNew["Tollet_Qty"] = Bathinfo.Tollet_Qty;
            drNew["Tollet_Style"] = Bathinfo.Tollet_Style;
            drNew["Tollet_WhereToOrder"] = Bathinfo.Tollet_WhereToOrder;
            drNew["Shower_TubSystem_Qty"] = Bathinfo.Shower_TubSystem_Qty;
            drNew["Shower_TubSystem_Style"] = Bathinfo.Shower_TubSystem_Style;
            drNew["Shower_TubSystem_WhereToOrder"] = Bathinfo.Shower_TubSystem_WhereToOrder;
            drNew["Shower_Value_Qty"] = Bathinfo.Shower_Value_Qty;
            drNew["Shower_Value_Style"] = Bathinfo.Shower_Value_Style;
            drNew["Shower_Value_WhereToOrder"] = Bathinfo.Shower_Value_WhereToOrder;
            drNew["Handheld_Shower_Qty"] = Bathinfo.Handheld_Shower_Qty;
            drNew["Handheld_Shower_Style"] = Bathinfo.Handheld_Shower_Style;
            drNew["Handheld_Shower_WhereToOrder"] = Bathinfo.Handheld_Shower_WhereToOrder;
            drNew["Body_Spray_Qty"] = Bathinfo.Body_Spray_Qty;
            drNew["Body_Spray_Style"] = Bathinfo.Body_Spray_Style;
            drNew["Body_Spray_WhereToOrder"] = Bathinfo.Body_Spray_WhereToOrder;
            drNew["Body_Spray_Valve_Qty"] = Bathinfo.Body_Spray_Valve_Qty;
            drNew["Body_Spray_Valve_Style"] = Bathinfo.Body_Spray_Valve_Style;
            drNew["Body_Spray_Valve_WhereToOrder"] = Bathinfo.Body_Spray_Valve_WhereToOrder;
            drNew["Shower_Drain_Qty"] = Bathinfo.Shower_Drain_Qty;
            drNew["Shower_Drain_Style"] = Bathinfo.Shower_Drain_Style;
            drNew["Shower_Drain_WhereToOrder"] = Bathinfo.Shower_Drain_WhereToOrder;
            drNew["Shower_Drain_Body_Plug_Qty"] = Bathinfo.Shower_Drain_Body_Plug_Qty;
            drNew["Shower_Drain_Body_Plug_Style"] = Bathinfo.Shower_Drain_Body_Plug_Style;
            drNew["Shower_Drain_Body_Plug_WhereToOrder"] = Bathinfo.Shower_Drain_Body_Plug_WhereToOrder;
            drNew["Shower_Drain_Cover_Qty"] = Bathinfo.Shower_Drain_Cover_Qty;
            drNew["Shower_Drain_Cover_Style"] = Bathinfo.Shower_Drain_Cover_Style;
            drNew["Shower_Drain_Cover_WhereToOrder"] = Bathinfo.Shower_Drain_Cover_WhereToOrder;
            drNew["Counter_Top_Qty"] = Bathinfo.Counter_Top_Qty;
            drNew["Counter_Top_Style"] = Bathinfo.Counter_Top_Style;
            drNew["Counter_Top_WhereToOrder"] = Bathinfo.Counter_Top_WhereToOrder;
            drNew["Counter_To_Edge_Qty"] = Bathinfo.Counter_To_Edge_Qty;
            drNew["Counter_To_Edge_Style"] = Bathinfo.Counter_To_Edge_Style;
            drNew["Counter_To_Edge_WhereToOrder"] = Bathinfo.Counter_To_Edge_WhereToOrder;
            drNew["Counter_Top_Overhang_Qty"] = Bathinfo.Counter_Top_Overhang_Qty;
            drNew["Counter_Top_Overhang_Style"] = Bathinfo.Counter_Top_Overhang_Style;
            drNew["Counter_Top_Overhang_WhereToOrder"] = Bathinfo.Counter_Top_Overhang_WhereToOrder;
            drNew["AdditionalPlacesGettingCountertop_Qty"] = Bathinfo.AdditionalPlacesGettingCountertop_Qty;
            drNew["AdditionalPlacesGettingCountertop_Style"] = Bathinfo.AdditionalPlacesGettingCountertop_Style;
            drNew["AdditionalPlacesGettingCountertop_WhereToOrder"] = Bathinfo.AdditionalPlacesGettingCountertop_WhereToOrder;
            drNew["Granite_Quartz_Backsplash_Qty"] = Bathinfo.Granite_Quartz_Backsplash_Qty;
            drNew["Granite_Quartz_Backsplash_Style"] = Bathinfo.Granite_Quartz_Backsplash_Style;
            drNew["Granite_Quartz_Backsplash_WhereToOrder"] = Bathinfo.Granite_Quartz_Backsplash_WhereToOrder;
            drNew["Tub_Wall_Tile_Qty"] = Bathinfo.Tub_Wall_Tile_Qty;
            drNew["Tub_Wall_Tile_Style"] = Bathinfo.Tub_Wall_Tile_Style;
            drNew["Tub_Wall_Tile_WhereToOrder"] = Bathinfo.Tub_Wall_Tile_WhereToOrder;
            drNew["Wall_Tile_Layout_Qty"] = Bathinfo.Wall_Tile_Layout_Qty;
            drNew["Wall_Tile_Layout_Style"] = Bathinfo.Wall_Tile_Layout_Style;
            drNew["Wall_Tile_Layout_WhereToOrder"] = Bathinfo.Wall_Tile_Layout_WhereToOrder;
            drNew["Tub_skirt_tile_Qty"] = Bathinfo.Tub_skirt_tile_Qty;
            drNew["Tub_skirt_tile_Style"] = Bathinfo.Tub_skirt_tile_Style;
            drNew["Tub_skirt_tile_WhereToOrder"] = Bathinfo.Tub_skirt_tile_WhereToOrder;
            drNew["Shower_Wall_Tile_Qty"] = Bathinfo.Shower_Wall_Tile_Qty;
            drNew["Shower_Wall_Tile_Style"] = Bathinfo.Shower_Wall_Tile_Style;
            drNew["Shower_Wall_Tile_WhereToOrder"] = Bathinfo.Shower_Wall_Tile_WhereToOrder;
            drNew["Wall_Tile_Layout_Qty"] = Bathinfo.Wall_Tile_Layout_Qty;
            drNew["Wall_Tile_Layout_Style"] = Bathinfo.Wall_Tile_Layout_Style;
            drNew["Wall_Tile_Layout_WhereToOrder"] = Bathinfo.Wall_Tile_Layout_WhereToOrder;
            drNew["Shower_Floor_Tile_Qty"] = Bathinfo.Shower_Floor_Tile_Qty;
            drNew["Shower_Floor_Tile_Style"] = Bathinfo.Shower_Floor_Tile_Style;
            drNew["Shower_Floor_Tile_WhereToOrder"] = Bathinfo.Shower_Floor_Tile_WhereToOrder;
            drNew["Shower_Tub_Tile_Height_Qty"] = Bathinfo.Shower_Tub_Tile_Height_Qty;
            drNew["Shower_Tub_Tile_Height_Style"] = Bathinfo.Shower_Tub_Tile_Height_Style;
            drNew["Shower_Tub_Tile_Height_WhereToOrder"] = Bathinfo.Shower_Tub_Tile_Height_WhereToOrder;
            drNew["Floor_Tile_Qty"] = Bathinfo.Floor_Tile_Qty;
            drNew["Floor_Tile_Style"] = Bathinfo.Floor_Tile_Style;
            drNew["Floor_Tile_WhereToOrder"] = Bathinfo.Floor_Tile_WhereToOrder;
            drNew["Floor_Tile_layout_Qty"] = Bathinfo.Floor_Tile_layout_Qty;
            drNew["Floor_Tile_layout_Style"] = Bathinfo.Floor_Tile_layout_Style;
            drNew["Floor_Tile_layout_WhereToOrder"] = Bathinfo.Floor_Tile_layout_WhereToOrder;
            drNew["BullnoseTile_Qty"] = Bathinfo.BullnoseTile_Qty;
            drNew["BullnoseTile_Style"] = Bathinfo.BullnoseTile_Style;
            drNew["BullnoseTile_WhereToOrder"] = Bathinfo.BullnoseTile_WhereToOrder;
            drNew["Deco_Band_Qty"] = Bathinfo.Deco_Band_Qty;
            drNew["Deco_Band_Style"] = Bathinfo.Deco_Band_Style;
            drNew["Deco_Band_WhereToOrder"] = Bathinfo.Deco_Band_WhereToOrder;
            drNew["Deco_Band_Height_Qty"] = Bathinfo.Deco_Band_Height_Qty;
            drNew["Deco_Band_Height_Style"] = Bathinfo.Deco_Band_Height_Style;
            drNew["Deco_Band_Height_WhereToOrder"] = Bathinfo.Deco_Band_Height_WhereToOrder;
            drNew["Tile_Baseboard_Qty"] = Bathinfo.Tile_Baseboard_Qty;
            drNew["Tile_Baseboard_Style"] = Bathinfo.Tile_Baseboard_Style;
            drNew["Tile_Baseboard_WhereToOrder"] = Bathinfo.Tile_Baseboard_WhereToOrder;
            drNew["Grout_Selection_Qty"] = Bathinfo.Grout_Selection_Qty;
            drNew["Grout_Selection_Style"] = Bathinfo.Grout_Selection_Style;
            drNew["Grout_Selection_WhereToOrder"] = Bathinfo.Grout_Selection_WhereToOrder;
            drNew["Niche_Location_Qty"] = Bathinfo.Niche_Location_Qty;
            drNew["Niche_Location_Style"] = Bathinfo.Niche_Location_Style;
            drNew["Niche_Location_WhereToOrder"] = Bathinfo.Niche_Location_WhereToOrder;
            drNew["Niche_Size_Qty"] = Bathinfo.Niche_Size_Qty;
            drNew["Niche_Size_Style"] = Bathinfo.Niche_Size_Style;
            drNew["Niche_Size_WhereToOrder"] = Bathinfo.Niche_Size_WhereToOrder;
            drNew["Glass_Qty"] = Bathinfo.Glass_Qty;
            drNew["Glass_Style"] = Bathinfo.Glass_Style;
            drNew["Glass_WhereToOrder"] = Bathinfo.Glass_WhereToOrder;
            drNew["Window_Qty"] = Bathinfo.Window_Qty;
            drNew["Window_Style"] = Bathinfo.Window_Style;
            drNew["Window_WhereToOrder"] = Bathinfo.Window_WhereToOrder;
            drNew["Door_Qty"] = Bathinfo.Door_Qty;
            drNew["Door_Style"] = Bathinfo.Door_Style;
            drNew["Door_WhereToOrder"] = Bathinfo.Door_WhereToOrder;
            drNew["Grab_Bar_Qty"] = Bathinfo.Grab_Bar_Qty;
            drNew["Grab_Bar_Style"] = Bathinfo.Grab_Bar_Style;
            drNew["Grab_Bar_WhereToOrder"] = Bathinfo.Grab_Bar_WhereToOrder;
            drNew["Cabinet_Door_Style_Color_Qty"] = Bathinfo.Cabinet_Door_Style_Color_Qty;
            drNew["Cabinet_Door_Style_Color_Style"] = Bathinfo.Cabinet_Door_Style_Color_Style;
            drNew["Cabinet_Door_Style_Color_WhereToOrder"] = Bathinfo.Cabinet_Door_Style_Color_WhereToOrder;
            drNew["Medicine_Cabinet_Qty"] = Bathinfo.Medicine_Cabinet_Qty;
            drNew["Medicine_Cabinet_Style"] = Bathinfo.Medicine_Cabinet_Style;
            drNew["Medicine_Cabinet_WhereToOrder"] = Bathinfo.Medicine_Cabinet_WhereToOrder;
            drNew["Mirror_Qty"] = Bathinfo.Mirror_Qty;
            drNew["Mirror_Style"] = Bathinfo.Mirror_Style;
            drNew["Mirror_WhereToOrder"] = Bathinfo.Mirror_WhereToOrder;
            drNew["Wood_Baseboard_Qty"] = Bathinfo.Wood_Baseboard_Qty;
            drNew["Wood_Baseboard_Style"] = Bathinfo.Wood_Baseboard_Style;
            drNew["Wood_Baseboard_WhereToOrder"] = Bathinfo.Wood_Baseboard_WhereToOrder;
            drNew["Paint_Color_Qty"] = Bathinfo.Paint_Color_Qty;
            drNew["Paint_Color_Style"] = Bathinfo.Paint_Color_Style;
            drNew["Paint_Color_WhereToOrder"] = Bathinfo.Paint_Color_WhereToOrder;
            drNew["Lighting_Qty"] = Bathinfo.Lighting_Qty;
            drNew["Lighting_Style"] = Bathinfo.Lighting_Style;
            drNew["Lighting_WhereToOrder"] = Bathinfo.Lighting_WhereToOrder;
            drNew["Hardware_Qty"] = Bathinfo.Hardware_Qty;
            drNew["Hardware_Style"] = Bathinfo.Hardware_Style;
            drNew["Hardware_WhereToOrder"] = Bathinfo.Hardware_WhereToOrder;

            drNew["TowelRing_Qty"] = Bathinfo.TowelRing_Qty;
            drNew["TowelRing_Style"] = Bathinfo.TowelRing_Style;
            drNew["TowelRing_WhereToOrder"] = Bathinfo.TowelRing_WhereToOrder;
            drNew["TowelBar_Qty"] = Bathinfo.TowelBar_Qty;
            drNew["TowelBar_Style"] = Bathinfo.TowelBar_Style;
            drNew["TowelBar_WhereToOrder"] = Bathinfo.TowelBar_WhereToOrder;
            drNew["TissueHolder_Qty"] = Bathinfo.TissueHolder_Qty;
            drNew["TissueHolder_Style"] = Bathinfo.TissueHolder_Style;
            drNew["TissueHolder_WhereToOrder"] = Bathinfo.TissueHolder_WhereToOrder;
            drNew["ClosetDoorSeries"] = Bathinfo.ClosetDoorSeries;
            drNew["ClosetDoorOpeningSize"] = Bathinfo.ClosetDoorOpeningSize;
            drNew["ClosetDoorNumberOfPanels"] = Bathinfo.ClosetDoorNumberOfPanels;
            drNew["ClosetDoorFinish"] = Bathinfo.ClosetDoorFinish;
            drNew["ClosetDoorInsert"] = Bathinfo.ClosetDoorInsert;

            drNew["Special_Notes"] = Bathinfo.Special_Notes;
            drNew["LastUpdatedDate"] = Bathinfo.LastUpdatedDate;
            drNew["UpdateBy"] = Bathinfo.UpdateBy;

            tmpTable.Rows.Add(drNew);
        }

        if (objBathSSList.Count() == 0)
        {
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["BathroomID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["BathroomSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Sink_Valve_Qty"] = "";
            drNew1["Sink_Valve_Style"] = "";
            drNew1["Sink_Valve_WhereToOrder"] = "";
            drNew1["Bathtub_Qty"] = "";
            drNew1["Bathtub_Style"] = "";
            drNew1["Bathtub_WhereToOrder"] = "";
            drNew1["Tub_Faucet_Qty"] = "";
            drNew1["Tub_Faucet_Style"] = "";
            drNew1["Tub_Faucet_WhereToOrder"] = "";
            drNew1["Tub_Valve_Qty"] = "";
            drNew1["Tub_Valve_Style"] = "";
            drNew1["Tub_Valve_WhereToOrder"] = "";
            drNew1["Tub_Drain_Qty"] = "";
            drNew1["Tub_Drain_Style"] = "";
            drNew1["Tub_Drain_WhereToOrder"] = "";
            drNew1["Tollet_Qty"] = "";
            drNew1["Tollet_Style"] = "";
            drNew1["Tollet_WhereToOrder"] = "";
            drNew1["Shower_TubSystem_Qty"] = "";
            drNew1["Shower_TubSystem_Style"] = "";
            drNew1["Shower_TubSystem_WhereToOrder"] = "";
            drNew1["Shower_Value_Qty"] = "";
            drNew1["Shower_Value_Style"] = "";
            drNew1["Shower_Value_WhereToOrder"] = "";
            drNew1["Handheld_Shower_Qty"] = "";
            drNew1["Handheld_Shower_Style"] = "";
            drNew1["Handheld_Shower_WhereToOrder"] = "";
            drNew1["Body_Spray_Qty"] = "";
            drNew1["Body_Spray_Style"] = "";
            drNew1["Body_Spray_WhereToOrder"] = "";
            drNew1["Body_Spray_Valve_Qty"] = "";
            drNew1["Body_Spray_Valve_Style"] = "";
            drNew1["Body_Spray_Valve_WhereToOrder"] = "";
            drNew1["Shower_Drain_Qty"] = "";
            drNew1["Shower_Drain_Style"] = "";
            drNew1["Shower_Drain_WhereToOrder"] = "";
            drNew1["Shower_Drain_Body_Plug_Qty"] = "";
            drNew1["Shower_Drain_Body_Plug_Style"] = "";
            drNew1["Shower_Drain_Body_Plug_WhereToOrder"] = "";
            drNew1["Shower_Drain_Cover_Qty"] = "";
            drNew1["Shower_Drain_Cover_Style"] = "";
            drNew1["Shower_Drain_Cover_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Tub_Wall_Tile_Qty"] = "";
            drNew1["Tub_Wall_Tile_Style"] = "";
            drNew1["Tub_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Tub_skirt_tile_Qty"] = "";
            drNew1["Tub_skirt_tile_Style"] = "";
            drNew1["Tub_skirt_tile_WhereToOrder"] = "";
            drNew1["Shower_Wall_Tile_Qty"] = "";
            drNew1["Shower_Wall_Tile_Style"] = "";
            drNew1["Shower_Wall_Tile_WhereToOrder"] = "";
            drNew1["Wall_Tile_Layout_Qty"] = "";
            drNew1["Wall_Tile_Layout_Style"] = "";
            drNew1["Wall_Tile_Layout_WhereToOrder"] = "";
            drNew1["Shower_Floor_Tile_Qty"] = "";
            drNew1["Shower_Floor_Tile_Style"] = "";
            drNew1["Shower_Floor_Tile_WhereToOrder"] = "";
            drNew1["Shower_Tub_Tile_Height_Qty"] = "";
            drNew1["Shower_Tub_Tile_Height_Style"] = "";
            drNew1["Shower_Tub_Tile_Height_WhereToOrder"] = "";
            drNew1["Floor_Tile_Qty"] = "";
            drNew1["Floor_Tile_Style"] = "";
            drNew1["Floor_Tile_WhereToOrder"] = "";
            drNew1["Floor_Tile_layout_Qty"] = "";
            drNew1["Floor_Tile_layout_Style"] = "";
            drNew1["Floor_Tile_layout_WhereToOrder"] = "";
            drNew1["BullnoseTile_Qty"] = "";
            drNew1["BullnoseTile_Style"] = "";
            drNew1["BullnoseTile_WhereToOrder"] = "";
            drNew1["Deco_Band_Qty"] = "";
            drNew1["Deco_Band_Style"] = "";
            drNew1["Deco_Band_WhereToOrder"] = "";
            drNew1["Deco_Band_Height_Qty"] = "";
            drNew1["Deco_Band_Height_Style"] = "";
            drNew1["Deco_Band_Height_WhereToOrder"] = "";
            drNew1["Tile_Baseboard_Qty"] = "";
            drNew1["Tile_Baseboard_Style"] = "";
            drNew1["Tile_Baseboard_WhereToOrder"] = "";
            drNew1["Grout_Selection_Qty"] = "";
            drNew1["Grout_Selection_Style"] = "";
            drNew1["Grout_Selection_WhereToOrder"] = "";
            drNew1["Niche_Location_Qty"] = "";
            drNew1["Niche_Location_Style"] = "";
            drNew1["Niche_Location_WhereToOrder"] = "";
            drNew1["Niche_Size_Qty"] = "";
            drNew1["Niche_Size_Style"] = "";
            drNew1["Niche_Size_WhereToOrder"] = "";
            drNew1["Glass_Qty"] = "";
            drNew1["Glass_Style"] = "";
            drNew1["Glass_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Grab_Bar_Qty"] = "";
            drNew1["Grab_Bar_Style"] = "";
            drNew1["Grab_Bar_WhereToOrder"] = "";
            drNew1["Cabinet_Door_Style_Color_Qty"] = "";
            drNew1["Cabinet_Door_Style_Color_Style"] = "";
            drNew1["Cabinet_Door_Style_Color_WhereToOrder"] = "";
            drNew1["Medicine_Cabinet_Qty"] = "";
            drNew1["Medicine_Cabinet_Style"] = "";
            drNew1["Medicine_Cabinet_WhereToOrder"] = "";
            drNew1["Mirror_Qty"] = "";
            drNew1["Mirror_Style"] = "";
            drNew1["Mirror_WhereToOrder"] = "";
            drNew1["Wood_Baseboard_Qty"] = "";
            drNew1["Wood_Baseboard_Style"] = "";
            drNew1["Wood_Baseboard_WhereToOrder"] = "";
            drNew1["Paint_Color_Qty"] = "";
            drNew1["Paint_Color_Style"] = "";
            drNew1["Paint_Color_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";

            drNew1["TowelRing_Qty"] = "";
            drNew1["TowelRing_Style"] = "";
            drNew1["TowelRing_WhereToOrder"] = "";
            drNew1["TowelBar_Qty"] = "";
            drNew1["TowelBar_Style"] = "";
            drNew1["TowelBar_WhereToOrder"] = "";
            drNew1["TissueHolder_Qty"] = "";
            drNew1["TissueHolder_Style"] = "";
            drNew1["TissueHolder_WhereToOrder"] = "";
            drNew1["ClosetDoorSeries"] = "";
            drNew1["ClosetDoorOpeningSize"] = "";
            drNew1["ClosetDoorNumberOfPanels"] = "";
            drNew1["ClosetDoorFinish"] = "";
            drNew1["ClosetDoorInsert"] = "";

            drNew1["Special_Notes"] = "";
            drNew1["UpdateBy"] = User.Identity.Name;
            drNew1["LastUpdatedDate"] = DateTime.Now;


            tmpTable.Rows.InsertAt(drNew1, 0);
        }

        Session.Add("sBathroomSection", tmpTable);

        grdBathroomSelectionSheet.DataSource = tmpTable;
        grdBathroomSelectionSheet.DataKeyNames = new string[] { "BathroomID", "customer_id", "estimate_id" };
        grdBathroomSelectionSheet.DataBind();



    }

    #endregion

    #region KitchenSelection
    protected void lnkDeleteKitchen_Click(object sender, EventArgs e)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            LinkButton lnkDeleteKitchen = (LinkButton)sender;
            int nKitchenSheetID = Convert.ToInt32(lnkDeleteKitchen.Attributes["CommandArgument"]);
            string strQ = string.Empty;

            strQ = "Delete FROM KitchenSelections  WHERE KitchenID =" + nKitchenSheetID;
            _db.ExecuteCommand(strQ, string.Empty);
            GetKitchen2Sheet();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }


    }
    protected void btnSaveKitchen_Click(object sender, EventArgs e)
    {
        lblKitchenResult2.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        string strRequired = string.Empty;
        try
        {
            DataTable table = (DataTable)Session["sKitchenSection"];

            foreach (GridViewRow row in grdKitchenSelectionSheet.Rows)
            {


                TextBox txtKitchenSheetName = (TextBox)row.FindControl("txtKitchenSheetName");
                Label lblKitchenSheetName = (Label)row.FindControl("lblKitchenSheetName");

                TextBox txtKitchenSinkQty = (TextBox)row.FindControl("txtKitchenSinkQty");
                TextBox txtKitchenSinkStyle = (TextBox)row.FindControl("txtKitchenSinkStyle");
                TextBox txtKitchenSinkOrder = (TextBox)row.FindControl("txtKitchenSinkOrder");

                TextBox txtKitchenSinkFaucetQty = (TextBox)row.FindControl("txtKitchenSinkFaucetQty");
                TextBox txtKitchenSinkFaucetStyle = (TextBox)row.FindControl("txtKitchenSinkFaucetStyle");
                TextBox txtKitchenSinkFaucetOrder = (TextBox)row.FindControl("txtKitchenSinkFaucetOrder");
                TextBox txtKitchenSinkDrainQty = (TextBox)row.FindControl("txtKitchenSinkDrainQty");

                TextBox txtKitchenSinkDrainStyle = (TextBox)row.FindControl("txtKitchenSinkDrainStyle");
                TextBox txtKitchenSinkDrainOrder = (TextBox)row.FindControl("txtKitchenSinkDrainOrder");
                TextBox txtKitchenCounterTopQty = (TextBox)row.FindControl("txtKitchenCounterTopQty");
                TextBox txtKitchenCounterTopStyle = (TextBox)row.FindControl("txtKitchenCounterTopStyle");
                TextBox txtKitchenCounterTopOrder = (TextBox)row.FindControl("txtKitchenCounterTopOrder");
                TextBox txtKitchenGraniteQuartzBacksplashQty = (TextBox)row.FindControl("txtKitchenGraniteQuartzBacksplashQty");
                TextBox txtKitchenGraniteQuartzBacksplashStyle = (TextBox)row.FindControl("txtKitchenGraniteQuartzBacksplashStyle");
                TextBox txtKitchenGraniteQuartzBacksplashOrder = (TextBox)row.FindControl("txtKitchenGraniteQuartzBacksplashOrder");
                TextBox txtKitchenCounterTopOverhangQty = (TextBox)row.FindControl("txtKitchenCounterTopOverhangQty");
                TextBox txtKitchenCounterTopOverhangStyle = (TextBox)row.FindControl("txtKitchenCounterTopOverhangStyle");
                TextBox txtKitchenCounterTopOverhangOrder = (TextBox)row.FindControl("txtKitchenCounterTopOverhangOrder");
                TextBox txtKitchenAdditionalplacesgettingcountertopQty = (TextBox)row.FindControl("txtKitchenAdditionalplacesgettingcountertopQty");
                TextBox txtKitchenAdditionalplacesgettingcountertopStyle = (TextBox)row.FindControl("txtKitchenAdditionalplacesgettingcountertopStyle");
                TextBox txtKitchenAdditionalplacesgettingcountertopOrder = (TextBox)row.FindControl("txtKitchenAdditionalplacesgettingcountertopOrder");
                TextBox txtKitchenCounterTopEdgeQty = (TextBox)row.FindControl("txtKitchenCounterTopEdgeQty");
                TextBox txtKitchenCounterTopEdgeStyle = (TextBox)row.FindControl("txtKitchenCounterTopEdgeStyle");
                TextBox txtKitchenCounterTopEdgeOrder = (TextBox)row.FindControl("txtKitchenCounterTopEdgeOrder");
                TextBox txtKitchenCabinetsQty = (TextBox)row.FindControl("txtKitchenCabinetsQty");
                TextBox txtKitchenCabinetsStyle = (TextBox)row.FindControl("txtKitchenCabinetsStyle");
                TextBox txtKitchenCabinetsOrder = (TextBox)row.FindControl("txtKitchenCabinetsOrder");
                TextBox txtKitchenDisposalQty = (TextBox)row.FindControl("txtKitchenDisposalQty");
                TextBox txtKitchenDisposalStyle = (TextBox)row.FindControl("txtKitchenDisposalStyle");
                TextBox txtKitchenDisposalOrder = (TextBox)row.FindControl("txtKitchenDisposalOrder");
                TextBox txtKitchenBaseboardQty = (TextBox)row.FindControl("txtKitchenBaseboardQty");
                TextBox txtKitchenBaseboardStyle = (TextBox)row.FindControl("txtKitchenBaseboardStyle");
                TextBox txtKitchenBaseboardOrder = (TextBox)row.FindControl("txtKitchenBaseboardOrder");
                TextBox txtKitchenWindowsQty = (TextBox)row.FindControl("txtKitchenWindowsQty");
                TextBox txtKitchenWindowsStyle = (TextBox)row.FindControl("txtKitchenWindowsStyle");
                TextBox txtKitchenWindowsOrder = (TextBox)row.FindControl("txtKitchenWindowsOrder");
                TextBox txtKitchenDoorsQty = (TextBox)row.FindControl("txtKitchenDoorsQty");
                TextBox txtKitchenDoorsStyle = (TextBox)row.FindControl("txtKitchenDoorsStyle");
                TextBox txtKitchenDoorsOrder = (TextBox)row.FindControl("txtKitchenDoorsOrder");

                TextBox txtKitchenLightingQty = (TextBox)row.FindControl("txtKitchenLightingQty");
                TextBox txtKitchenLightingStyle = (TextBox)row.FindControl("txtKitchenLightingStyle");
                TextBox txtKitchenLightingOrder = (TextBox)row.FindControl("txtKitchenLightingOrder");

                TextBox txtKitchenHardwareQty = (TextBox)row.FindControl("txtKitchenHardwareQty");
                TextBox txtKitchenHardwareStyle = (TextBox)row.FindControl("txtKitchenHardwareStyle");
                TextBox txtKitchenHardwareOrder = (TextBox)row.FindControl("txtKitchenHardwareOrder");

                TextBox txtKitchenSpecialNotes = (TextBox)row.FindControl("txtKitchenSpecialNotes");



                if (txtKitchenSheetName.Text.Trim() == "")
                {
                    lblKitchenSheetName.Text = csCommonUtility.GetSystemRequiredMessage2("Title/Location of Kitchen sheet is required.");
                    strRequired = "required";
                }
                else
                    lblKitchenSheetName.Text = "";


                if (strRequired.Length == 0)
                {
                    DataRow dr = table.Rows[row.RowIndex];

                    dr["KitchenID"] = Convert.ToInt32(grdKitchenSelectionSheet.DataKeys[row.RowIndex].Values[0]);
                    dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                    dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                    dr["KitchenSheetName"] = txtKitchenSheetName.Text.Trim();
                    dr["Sink_Qty"] = txtKitchenSinkQty.Text;
                    dr["Sink_Style"] = txtKitchenSinkStyle.Text;
                    dr["Sink_WhereToOrder"] = txtKitchenSinkOrder.Text;
                    dr["Sink_Fuacet_Qty"] = txtKitchenSinkFaucetQty.Text;
                    dr["Sink_Fuacet_Style"] = txtKitchenSinkFaucetStyle.Text;
                    dr["Sink_Fuacet_WhereToOrder"] = txtKitchenSinkFaucetOrder.Text;
                    dr["Sink_Drain_Qty"] = txtKitchenSinkDrainQty.Text;
                    dr["Sink_Drain_Style"] = txtKitchenSinkDrainStyle.Text;
                    dr["Sink_Drain_WhereToOrder"] = txtKitchenSinkDrainOrder.Text;
                    dr["Counter_Top_Qty"] = txtKitchenCounterTopQty.Text;
                    dr["Counter_Top_Style"] = txtKitchenCounterTopStyle.Text;
                    dr["Counter_Top_WhereToOrder"] = txtKitchenCounterTopOrder.Text;
                    dr["Granite_Quartz_Backsplash_Qty"] = txtKitchenGraniteQuartzBacksplashQty.Text;
                    dr["Granite_Quartz_Backsplash_Style"] = txtKitchenGraniteQuartzBacksplashStyle.Text;
                    dr["Granite_Quartz_Backsplash_WhereToOrder"] = txtKitchenGraniteQuartzBacksplashOrder.Text;
                    dr["Counter_Top_Overhang_Qty"] = txtKitchenCounterTopOverhangQty.Text;
                    dr["Counter_Top_Overhang_Style"] = txtKitchenCounterTopOverhangStyle.Text;
                    dr["Counter_Top_Overhang_WhereToOrder"] = txtKitchenCounterTopOverhangOrder.Text;
                    dr["AdditionalPlacesGettingCountertop_Qty"] = txtKitchenAdditionalplacesgettingcountertopQty.Text;
                    dr["AdditionalPlacesGettingCountertop_Style"] = txtKitchenAdditionalplacesgettingcountertopStyle.Text;
                    dr["AdditionalPlacesGettingCountertop_WhereToOrder"] = txtKitchenAdditionalplacesgettingcountertopOrder.Text;
                    dr["Counter_To_Edge_Qty"] = txtKitchenCounterTopEdgeQty.Text;
                    dr["Counter_To_Edge_Style"] = txtKitchenCounterTopEdgeStyle.Text;
                    dr["Counter_To_Edge_WhereToOrder"] = txtKitchenCounterTopEdgeOrder.Text;
                    dr["Cabinets_Qty"] = txtKitchenCabinetsQty.Text;
                    dr["Cabinets_Style"] = txtKitchenCabinetsStyle.Text;
                    dr["Cabinets_WhereToOrder"] = txtKitchenCabinetsOrder.Text;
                    dr["Disposal_Qty"] = txtKitchenDisposalQty.Text;
                    dr["Disposal_Style"] = txtKitchenDisposalStyle.Text;
                    dr["Disposal_WhereToOrder"] = txtKitchenDisposalOrder.Text;
                    dr["Baseboard_Qty"] = txtKitchenBaseboardQty.Text;
                    dr["Baseboard_Style"] = txtKitchenBaseboardStyle.Text;
                    dr["Baseboard_WhereToOrder"] = txtKitchenBaseboardOrder.Text;
                    dr["Window_Qty"] = txtKitchenWindowsQty.Text;
                    dr["Window_Style"] = txtKitchenWindowsStyle.Text;
                    dr["Window_WhereToOrder"] = txtKitchenWindowsOrder.Text;
                    dr["Door_Qty"] = txtKitchenDoorsQty.Text;
                    dr["Door_Style"] = txtKitchenDoorsStyle.Text;
                    dr["Door_WhereToOrder"] = txtKitchenDoorsOrder.Text;
                    dr["Lighting_Qty"] = txtKitchenLightingQty.Text;
                    dr["Lighting_Style"] = txtKitchenLightingStyle.Text;
                    dr["Lighting_WhereToOrder"] = txtKitchenLightingOrder.Text;
                    dr["Hardware_Qty"] = txtKitchenHardwareQty.Text;
                    dr["Hardware_Style"] = txtKitchenHardwareStyle.Text;
                    dr["Hardware_WhereToOrder"] = txtKitchenHardwareOrder.Text;
                    dr["Special_Notes"] = txtKitchenSpecialNotes.Text;
                    dr["LastUpdatedDate"] = DateTime.Now;
                    dr["UpdateBy"] = User.Identity.Name;
                }
            }
            if (strRequired.Length == 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    bool bFlagNew = false;

                    KitchenSelection objkss = _db.KitchenSelections.SingleOrDefault(l => l.KitchenID == Convert.ToInt32(dr["KitchenID"]));
                    if (objkss == null)
                    {
                        objkss = new KitchenSelection();
                        bFlagNew = true;

                    }

                    objkss.KitchenID = Convert.ToInt32(dr["KitchenID"]);
                    objkss.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    objkss.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                    objkss.KitchenSheetName = dr["KitchenSheetName"].ToString();
                    objkss.Sink_Qty = dr["Sink_Qty"].ToString();
                    objkss.Sink_Style = dr["Sink_Style"].ToString();
                    objkss.Sink_WhereToOrder = dr["Sink_WhereToOrder"].ToString();
                    objkss.Sink_Fuacet_Qty = dr["Sink_Fuacet_Qty"].ToString();
                    objkss.Sink_Fuacet_Style = dr["Sink_Fuacet_Style"].ToString();
                    objkss.Sink_Fuacet_WhereToOrder = dr["Sink_Fuacet_WhereToOrder"].ToString();
                    objkss.Sink_Drain_Qty = dr["Sink_Drain_Qty"].ToString();
                    objkss.Sink_Drain_Style = dr["Sink_Drain_Style"].ToString();
                    objkss.Sink_Drain_WhereToOrder = dr["Sink_Drain_WhereToOrder"].ToString();
                    objkss.Counter_Top_Qty = dr["Counter_Top_Qty"].ToString();
                    objkss.Counter_Top_Style = dr["Counter_Top_Style"].ToString();
                    objkss.Counter_Top_WhereToOrder = dr["Counter_Top_WhereToOrder"].ToString();
                    objkss.Granite_Quartz_Backsplash_Qty = dr["Granite_Quartz_Backsplash_Qty"].ToString();
                    objkss.Granite_Quartz_Backsplash_Style = dr["Granite_Quartz_Backsplash_Style"].ToString();
                    objkss.Granite_Quartz_Backsplash_WhereToOrder = dr["Granite_Quartz_Backsplash_WhereToOrder"].ToString();
                    objkss.Counter_Top_Overhang_Qty = dr["Counter_Top_Overhang_Qty"].ToString();
                    objkss.Counter_Top_Overhang_Style = dr["Counter_Top_Overhang_Style"].ToString();
                    objkss.Counter_Top_Overhang_WhereToOrder = dr["Counter_Top_Overhang_WhereToOrder"].ToString();
                    objkss.AdditionalPlacesGettingCountertop_Qty = dr["AdditionalPlacesGettingCountertop_Qty"].ToString();
                    objkss.AdditionalPlacesGettingCountertop_Style = dr["AdditionalPlacesGettingCountertop_Style"].ToString();
                    objkss.AdditionalPlacesGettingCountertop_WhereToOrder = dr["AdditionalPlacesGettingCountertop_WhereToOrder"].ToString();
                    objkss.Counter_To_Edge_Qty = dr["Counter_To_Edge_Qty"].ToString();
                    objkss.Counter_To_Edge_Style = dr["Counter_To_Edge_Style"].ToString();
                    objkss.Counter_To_Edge_WhereToOrder = dr["Counter_To_Edge_WhereToOrder"].ToString();
                    objkss.Cabinets_Qty = dr["Cabinets_Qty"].ToString();
                    objkss.Cabinets_Style = dr["Cabinets_Style"].ToString();
                    objkss.Cabinets_WhereToOrder = dr["Cabinets_WhereToOrder"].ToString();
                    objkss.Disposal_Qty = dr["Disposal_Qty"].ToString();
                    objkss.Disposal_Style = dr["Disposal_Style"].ToString();
                    objkss.Disposal_WhereToOrder = dr["Disposal_WhereToOrder"].ToString();
                    objkss.Baseboard_Qty = dr["Baseboard_Qty"].ToString();
                    objkss.Baseboard_Style = dr["Baseboard_Style"].ToString();
                    objkss.Baseboard_WhereToOrder = dr["Baseboard_WhereToOrder"].ToString();
                    objkss.Window_Qty = dr["Window_Qty"].ToString();
                    objkss.Window_Style = dr["Window_Style"].ToString();
                    objkss.Window_WhereToOrder = dr["Window_WhereToOrder"].ToString();
                    objkss.Door_Qty = dr["Door_Qty"].ToString();
                    objkss.Door_Style = dr["Door_Style"].ToString();
                    objkss.Door_WhereToOrder = dr["Door_WhereToOrder"].ToString();
                    objkss.Lighting_Qty = dr["Lighting_Qty"].ToString();
                    objkss.Lighting_Style = dr["Lighting_Style"].ToString();
                    objkss.Lighting_WhereToOrder = dr["Lighting_WhereToOrder"].ToString();
                    objkss.Hardware_Qty = dr["Hardware_Qty"].ToString();
                    objkss.Hardware_Style = dr["Hardware_Style"].ToString();
                    objkss.Hardware_WhereToOrder = dr["Hardware_WhereToOrder"].ToString();
                    objkss.Special_Notes = dr["Special_Notes"].ToString();
                    objkss.LastUpdatedDate = DateTime.Now;
                    objkss.UpdateBy = User.Identity.Name;

                    if (bFlagNew)
                    {
                        _db.KitchenSelections.InsertOnSubmit(objkss);
                    }
                }

                lblKitchenResult2.Text = csCommonUtility.GetSystemMessage("Data has been saved successfully");

                _db.SubmitChanges();
                GetKitchen2Sheet();
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void grdKitchenSelectionSheet_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nKitchenSheetID = Convert.ToInt32(grdKitchenSelectionSheet.DataKeys[e.Row.RowIndex].Values[0].ToString());

            LinkButton lnkDeleteKitchen = (LinkButton)e.Row.FindControl("lnkDeleteKitchen");

            lnkDeleteKitchen.Attributes["CommandArgument"] = string.Format("{0}", nKitchenSheetID);

            if (nKitchenSheetID > 0)
                lnkDeleteKitchen.Visible = true;

        }
    }

    private DataTable LoadKitchenTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("KitchenID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("KitchenSheetName", typeof(string));
        table.Columns.Add("Sink_Qty", typeof(string));
        table.Columns.Add("Sink_Style", typeof(string));
        table.Columns.Add("Sink_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Fuacet_Qty", typeof(string));
        table.Columns.Add("Sink_Fuacet_Style", typeof(string));
        table.Columns.Add("Sink_Fuacet_WhereToOrder", typeof(string));
        table.Columns.Add("Sink_Drain_Qty", typeof(string));
        table.Columns.Add("Sink_Drain_Style", typeof(string));
        table.Columns.Add("Sink_Drain_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Style", typeof(string));
        table.Columns.Add("Counter_Top_WhereToOrder", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Qty", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_Style", typeof(string));
        table.Columns.Add("Granite_Quartz_Backsplash_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Qty", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_Style", typeof(string));
        table.Columns.Add("Counter_Top_Overhang_WhereToOrder", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Qty", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_Style", typeof(string));
        table.Columns.Add("AdditionalPlacesGettingCountertop_WhereToOrder", typeof(string));
        table.Columns.Add("Counter_To_Edge_Qty", typeof(string));
        table.Columns.Add("Counter_To_Edge_Style", typeof(string));
        table.Columns.Add("Counter_To_Edge_WhereToOrder", typeof(string));
        table.Columns.Add("Cabinets_Qty", typeof(string));
        table.Columns.Add("Cabinets_Style", typeof(string));
        table.Columns.Add("Cabinets_WhereToOrder", typeof(string));
        table.Columns.Add("Disposal_Qty", typeof(string));
        table.Columns.Add("Disposal_Style", typeof(string));
        table.Columns.Add("Disposal_WhereToOrder", typeof(string));
        table.Columns.Add("Baseboard_Qty", typeof(string));
        table.Columns.Add("Baseboard_Style", typeof(string));
        table.Columns.Add("Baseboard_WhereToOrder", typeof(string));
        table.Columns.Add("Window_Qty", typeof(string));
        table.Columns.Add("Window_Style", typeof(string));
        table.Columns.Add("Window_WhereToOrder", typeof(string));
        table.Columns.Add("Door_Qty", typeof(string));
        table.Columns.Add("Door_Style", typeof(string));
        table.Columns.Add("Door_WhereToOrder", typeof(string));
        table.Columns.Add("Lighting_Qty", typeof(string));
        table.Columns.Add("Lighting_Style", typeof(string));
        table.Columns.Add("Lighting_WhereToOrder", typeof(string));
        table.Columns.Add("Hardware_Qty", typeof(string));
        table.Columns.Add("Hardware_Style", typeof(string));
        table.Columns.Add("Hardware_WhereToOrder", typeof(string));
        table.Columns.Add("Special_Notes", typeof(string));
        table.Columns.Add("LastUpdatedDate", typeof(DateTime));
        table.Columns.Add("UpdateBy", typeof(string));

        return table;
    }

    protected void btnAddItemKitchen_Click(object sender, EventArgs e)
    {
        KitchenSelection objKitchenSS = new KitchenSelection();
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable table = (DataTable)Session["sKitchenSection"];

        int nKitchenSheetID = Convert.ToInt32(hdnKitchenSheetID.Value);
        bool contains = table.AsEnumerable().Any(row => nKitchenSheetID == row.Field<int>("KitchenID"));
        if (contains)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("You have already a pending item to save.");
            return;
        }

        foreach (GridViewRow row in grdKitchenSelectionSheet.Rows)
        {
            DataRow dr = table.Rows[row.RowIndex];

            TextBox txtKitchenSheetName = (TextBox)row.FindControl("txtKitchenSheetName");
            Label lblKitchenSheetName = (Label)row.FindControl("lblKitchenSheetName");

            TextBox txtKitchenSinkQty = (TextBox)row.FindControl("txtKitchenSinkQty");
            TextBox txtKitchenSinkStyle = (TextBox)row.FindControl("txtKitchenSinkStyle");
            TextBox txtKitchenSinkOrder = (TextBox)row.FindControl("txtKitchenSinkOrder");

            TextBox txtKitchenSinkFaucetQty = (TextBox)row.FindControl("txtKitchenSinkFaucetQty");
            TextBox txtKitchenSinkFaucetStyle = (TextBox)row.FindControl("txtKitchenSinkFaucetStyle");
            TextBox txtKitchenSinkFaucetOrder = (TextBox)row.FindControl("txtKitchenSinkFaucetOrder");
            TextBox txtKitchenSinkDrainQty = (TextBox)row.FindControl("txtKitchenSinkDrainQty");

            TextBox txtKitchenSinkDrainStyle = (TextBox)row.FindControl("txtKitchenSinkDrainStyle");
            TextBox txtKitchenSinkDrainOrder = (TextBox)row.FindControl("txtKitchenSinkDrainOrder");
            TextBox txtKitchenCounterTopQty = (TextBox)row.FindControl("txtKitchenCounterTopQty");
            TextBox txtKitchenCounterTopStyle = (TextBox)row.FindControl("txtKitchenCounterTopStyle");
            TextBox txtKitchenCounterTopOrder = (TextBox)row.FindControl("txtKitchenCounterTopOrder");
            TextBox txtKitchenGraniteQuartzBacksplashQty = (TextBox)row.FindControl("txtKitchenGraniteQuartzBacksplashQty");
            TextBox txtKitchenGraniteQuartzBacksplashStyle = (TextBox)row.FindControl("txtKitchenGraniteQuartzBacksplashStyle");
            TextBox txtKitchenGraniteQuartzBacksplashOrder = (TextBox)row.FindControl("txtKitchenGraniteQuartzBacksplashOrder");
            TextBox txtKitchenCounterTopOverhangQty = (TextBox)row.FindControl("txtKitchenCounterTopOverhangQty");
            TextBox txtKitchenCounterTopOverhangStyle = (TextBox)row.FindControl("txtKitchenCounterTopOverhangStyle");
            TextBox txtKitchenCounterTopOverhangOrder = (TextBox)row.FindControl("txtKitchenCounterTopOverhangOrder");
            TextBox txtKitchenAdditionalplacesgettingcountertopQty = (TextBox)row.FindControl("txtKitchenAdditionalplacesgettingcountertopQty");
            TextBox txtKitchenAdditionalplacesgettingcountertopStyle = (TextBox)row.FindControl("txtKitchenAdditionalplacesgettingcountertopStyle");
            TextBox txtKitchenAdditionalplacesgettingcountertopOrder = (TextBox)row.FindControl("txtKitchenAdditionalplacesgettingcountertopOrder");
            TextBox txtKitchenCounterTopEdgeQty = (TextBox)row.FindControl("txtKitchenCounterTopEdgeQty");
            TextBox txtKitchenCounterTopEdgeStyle = (TextBox)row.FindControl("txtKitchenCounterTopEdgeStyle");
            TextBox txtKitchenCounterTopEdgeOrder = (TextBox)row.FindControl("txtKitchenCounterTopEdgeOrder");
            TextBox txtKitchenCabinetsQty = (TextBox)row.FindControl("txtKitchenCabinetsQty");
            TextBox txtKitchenCabinetsStyle = (TextBox)row.FindControl("txtKitchenCabinetsStyle");
            TextBox txtKitchenCabinetsOrder = (TextBox)row.FindControl("txtKitchenCabinetsOrder");
            TextBox txtKitchenDisposalQty = (TextBox)row.FindControl("txtKitchenDisposalQty");
            TextBox txtKitchenDisposalStyle = (TextBox)row.FindControl("txtKitchenDisposalStyle");
            TextBox txtKitchenDisposalOrder = (TextBox)row.FindControl("txtKitchenDisposalOrder");
            TextBox txtKitchenBaseboardQty = (TextBox)row.FindControl("txtKitchenBaseboardQty");
            TextBox txtKitchenBaseboardStyle = (TextBox)row.FindControl("txtKitchenBaseboardStyle");
            TextBox txtKitchenBaseboardOrder = (TextBox)row.FindControl("txtKitchenBaseboardOrder");
            TextBox txtKitchenWindowsQty = (TextBox)row.FindControl("txtKitchenWindowsQty");
            TextBox txtKitchenWindowsStyle = (TextBox)row.FindControl("txtKitchenWindowsStyle");
            TextBox txtKitchenWindowsOrder = (TextBox)row.FindControl("txtKitchenWindowsOrder");
            TextBox txtKitchenDoorsQty = (TextBox)row.FindControl("txtKitchenDoorsQty");
            TextBox txtKitchenDoorsStyle = (TextBox)row.FindControl("txtKitchenDoorsStyle");
            TextBox txtKitchenDoorsOrder = (TextBox)row.FindControl("txtKitchenDoorsOrder");

            TextBox txtKitchenLightingQty = (TextBox)row.FindControl("txtKitchenLightingQty");
            TextBox txtKitchenLightingStyle = (TextBox)row.FindControl("txtKitchenLightingStyle");
            TextBox txtKitchenLightingOrder = (TextBox)row.FindControl("txtKitchenLightingOrder");

            TextBox txtKitchenHardwareQty = (TextBox)row.FindControl("txtKitchenHardwareQty");
            TextBox txtKitchenHardwareStyle = (TextBox)row.FindControl("txtKitchenHardwareStyle");
            TextBox txtKitchenHardwareOrder = (TextBox)row.FindControl("txtKitchenHardwareOrder");

            TextBox txtKitchenSpecialNotes = (TextBox)row.FindControl("txtKitchenSpecialNotes");


            dr["KitchenID"] = Convert.ToInt32(grdKitchenSelectionSheet.DataKeys[row.RowIndex].Values[0]);
            dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            dr["KitchenSheetName"] = txtKitchenSheetName.Text.Trim();
            dr["Sink_Qty"] = txtKitchenSinkQty.Text;
            dr["Sink_Style"] = txtKitchenSinkStyle.Text;
            dr["Sink_WhereToOrder"] = txtKitchenSinkOrder.Text;
            dr["Sink_Fuacet_Qty"] = txtKitchenSinkFaucetQty.Text;
            dr["Sink_Fuacet_Style"] = txtKitchenSinkFaucetStyle.Text;
            dr["Sink_Fuacet_WhereToOrder"] = txtKitchenSinkFaucetOrder.Text;
            dr["Sink_Drain_Qty"] = txtKitchenSinkDrainQty.Text;
            dr["Sink_Drain_Style"] = txtKitchenSinkDrainStyle.Text;
            dr["Sink_Drain_WhereToOrder"] = txtKitchenSinkDrainOrder.Text;
            dr["Counter_Top_Qty"] = txtKitchenCounterTopQty.Text;
            dr["Counter_Top_Style"] = txtKitchenCounterTopStyle.Text;
            dr["Counter_Top_WhereToOrder"] = txtKitchenCounterTopOrder.Text;
            dr["Granite_Quartz_Backsplash_Qty"] = txtKitchenGraniteQuartzBacksplashQty.Text;
            dr["Granite_Quartz_Backsplash_Style"] = txtKitchenGraniteQuartzBacksplashStyle.Text;
            dr["Granite_Quartz_Backsplash_WhereToOrder"] = txtKitchenGraniteQuartzBacksplashOrder.Text;
            dr["Counter_Top_Overhang_Qty"] = txtKitchenCounterTopOverhangQty.Text;
            dr["Counter_Top_Overhang_Style"] = txtKitchenCounterTopOverhangStyle.Text;
            dr["Counter_Top_Overhang_WhereToOrder"] = txtKitchenCounterTopOverhangOrder.Text;
            dr["AdditionalPlacesGettingCountertop_Qty"] = txtKitchenAdditionalplacesgettingcountertopQty.Text;
            dr["AdditionalPlacesGettingCountertop_Style"] = txtKitchenAdditionalplacesgettingcountertopStyle.Text;
            dr["AdditionalPlacesGettingCountertop_WhereToOrder"] = txtKitchenAdditionalplacesgettingcountertopOrder.Text;
            dr["Counter_To_Edge_Qty"] = txtKitchenCounterTopEdgeQty.Text;
            dr["Counter_To_Edge_Style"] = txtKitchenCounterTopEdgeStyle.Text;
            dr["Counter_To_Edge_WhereToOrder"] = txtKitchenCounterTopEdgeOrder.Text;
            dr["Cabinets_Qty"] = txtKitchenCabinetsQty.Text;
            dr["Cabinets_Style"] = txtKitchenCabinetsStyle.Text;
            dr["Cabinets_WhereToOrder"] = txtKitchenCabinetsOrder.Text;
            dr["Disposal_Qty"] = txtKitchenDisposalQty.Text;
            dr["Disposal_Style"] = txtKitchenDisposalStyle.Text;
            dr["Disposal_WhereToOrder"] = txtKitchenDisposalOrder.Text;
            dr["Baseboard_Qty"] = txtKitchenBaseboardQty.Text;
            dr["Baseboard_Style"] = txtKitchenBaseboardStyle.Text;
            dr["Baseboard_WhereToOrder"] = txtKitchenBaseboardOrder.Text;
            dr["Window_Qty"] = txtKitchenWindowsQty.Text;
            dr["Window_Style"] = txtKitchenWindowsStyle.Text;
            dr["Window_WhereToOrder"] = txtKitchenWindowsOrder.Text;
            dr["Door_Qty"] = txtKitchenDoorsQty.Text;
            dr["Door_Style"] = txtKitchenDoorsStyle.Text;
            dr["Door_WhereToOrder"] = txtKitchenDoorsOrder.Text;
            dr["Lighting_Qty"] = txtKitchenLightingQty.Text;
            dr["Lighting_Style"] = txtKitchenLightingStyle.Text;
            dr["Lighting_WhereToOrder"] = txtKitchenLightingOrder.Text;
            dr["Hardware_Qty"] = txtKitchenHardwareQty.Text;
            dr["Hardware_Style"] = txtKitchenHardwareStyle.Text;
            dr["Hardware_WhereToOrder"] = txtKitchenHardwareOrder.Text;
            dr["Special_Notes"] = txtKitchenSpecialNotes.Text;
            dr["LastUpdatedDate"] = DateTime.Now;
            dr["UpdateBy"] = User.Identity.Name;
        }

        DataRow drNew = table.NewRow();

        drNew["KitchenID"] = Convert.ToInt32(hdnKitchenSheetID.Value);
        drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
        drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
        drNew["KitchenSheetName"] = "";
        drNew["Sink_Qty"] = "";
        drNew["Sink_Style"] = "";
        drNew["Sink_WhereToOrder"] = "";
        drNew["Sink_Fuacet_Qty"] = "";
        drNew["Sink_Fuacet_Style"] = "";
        drNew["Sink_Fuacet_WhereToOrder"] = "";
        drNew["Sink_Drain_Qty"] = "";
        drNew["Sink_Drain_Style"] = "";
        drNew["Sink_Drain_WhereToOrder"] = "";
        drNew["Counter_Top_Qty"] = "";
        drNew["Counter_Top_Style"] = "";
        drNew["Counter_Top_WhereToOrder"] = "";
        drNew["Granite_Quartz_Backsplash_Qty"] = "";
        drNew["Granite_Quartz_Backsplash_Style"] = "";
        drNew["Granite_Quartz_Backsplash_WhereToOrder"] = "";
        drNew["Counter_Top_Overhang_Qty"] = "";
        drNew["Counter_Top_Overhang_Style"] = "";
        drNew["Counter_Top_Overhang_WhereToOrder"] = "";
        drNew["AdditionalPlacesGettingCountertop_Qty"] = "";
        drNew["AdditionalPlacesGettingCountertop_Style"] = "";
        drNew["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
        drNew["Counter_To_Edge_Qty"] = "";
        drNew["Counter_To_Edge_Style"] = "";
        drNew["Counter_To_Edge_WhereToOrder"] = "";
        drNew["Cabinets_Qty"] = "";
        drNew["Cabinets_Style"] = "";
        drNew["Cabinets_WhereToOrder"] = "";
        drNew["Disposal_Qty"] = "";
        drNew["Disposal_Style"] = "";
        drNew["Disposal_WhereToOrder"] = "";
        drNew["Baseboard_Qty"] = "";
        drNew["Baseboard_Style"] = "";
        drNew["Baseboard_WhereToOrder"] = "";
        drNew["Window_Qty"] = "";
        drNew["Window_Style"] = "";
        drNew["Window_WhereToOrder"] = "";
        drNew["Door_Qty"] = "";
        drNew["Door_Style"] = "";
        drNew["Door_WhereToOrder"] = "";
        drNew["Lighting_Qty"] = "";
        drNew["Lighting_Style"] = "";
        drNew["Lighting_WhereToOrder"] = "";
        drNew["Hardware_Qty"] = "";
        drNew["Hardware_Style"] = "";
        drNew["Hardware_WhereToOrder"] = "";
        drNew["Special_Notes"] = "";
        drNew["LastUpdatedDate"] = DateTime.Now;
        drNew["UpdateBy"] = User.Identity.Name;

        table.Rows.InsertAt(drNew, 0);

        Session.Add("sKitchenSection", table);
        grdKitchenSelectionSheet.DataSource = table;
        grdKitchenSelectionSheet.DataKeyNames = new string[] { "KitchenID", "customer_id", "estimate_id" };
        grdKitchenSelectionSheet.DataBind();
        lblKitchenResult2.Text = "";

    }
    protected void GetKitchen2Sheet()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable tmpTable = LoadKitchenTable();

        var objKitchenSSList = _db.KitchenSelections.Where(cp => cp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cp.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).ToList();

        foreach (KitchenSelection Kitheninfo in objKitchenSSList)
        {

            DataRow drNew = tmpTable.NewRow();
            drNew["KitchenID"] = Convert.ToInt32(Kitheninfo.KitchenID);
            drNew["customer_id"] = Convert.ToInt32(Kitheninfo.customer_id);
            drNew["estimate_id"] = Convert.ToInt32(Kitheninfo.estimate_id);
            drNew["KitchenSheetName"] = Kitheninfo.KitchenSheetName;
            drNew["Sink_Qty"] = Kitheninfo.Sink_Qty;
            drNew["Sink_Style"] = Kitheninfo.Sink_Style;
            drNew["Sink_WhereToOrder"] = Kitheninfo.Sink_WhereToOrder;
            drNew["Sink_Fuacet_Qty"] = Kitheninfo.Sink_Fuacet_Qty;
            drNew["Sink_Fuacet_Style"] = Kitheninfo.Sink_Fuacet_Style;
            drNew["Sink_Fuacet_WhereToOrder"] = Kitheninfo.Sink_Fuacet_WhereToOrder;
            drNew["Sink_Drain_Qty"] = Kitheninfo.Sink_Drain_Qty;
            drNew["Sink_Drain_Style"] = Kitheninfo.Sink_Drain_Style;
            drNew["Sink_Drain_WhereToOrder"] = Kitheninfo.Sink_Drain_WhereToOrder;
            drNew["Counter_Top_Qty"] = Kitheninfo.Counter_Top_Qty;
            drNew["Counter_Top_Style"] = Kitheninfo.Counter_Top_Style;
            drNew["Counter_Top_WhereToOrder"] = Kitheninfo.Counter_Top_WhereToOrder;
            drNew["Granite_Quartz_Backsplash_Qty"] = Kitheninfo.Granite_Quartz_Backsplash_Qty;
            drNew["Granite_Quartz_Backsplash_Style"] = Kitheninfo.Granite_Quartz_Backsplash_Style;
            drNew["Granite_Quartz_Backsplash_WhereToOrder"] = Kitheninfo.Granite_Quartz_Backsplash_WhereToOrder;
            drNew["Counter_Top_Overhang_Qty"] = Kitheninfo.Counter_Top_Overhang_Qty;
            drNew["Counter_Top_Overhang_Style"] = Kitheninfo.Counter_Top_Overhang_Style;
            drNew["Counter_Top_Overhang_WhereToOrder"] = Kitheninfo.Counter_Top_Overhang_WhereToOrder;
            drNew["AdditionalPlacesGettingCountertop_Qty"] = Kitheninfo.AdditionalPlacesGettingCountertop_Qty;
            drNew["AdditionalPlacesGettingCountertop_Style"] = Kitheninfo.AdditionalPlacesGettingCountertop_Style;
            drNew["AdditionalPlacesGettingCountertop_WhereToOrder"] = Kitheninfo.AdditionalPlacesGettingCountertop_WhereToOrder;
            drNew["Counter_To_Edge_Qty"] = Kitheninfo.Counter_To_Edge_Qty;
            drNew["Counter_To_Edge_Style"] = Kitheninfo.Counter_To_Edge_Style;
            drNew["Counter_To_Edge_WhereToOrder"] = Kitheninfo.Counter_To_Edge_WhereToOrder;
            drNew["Cabinets_Qty"] = Kitheninfo.Cabinets_Qty;
            drNew["Cabinets_Style"] = Kitheninfo.Cabinets_Style;
            drNew["Cabinets_WhereToOrder"] = Kitheninfo.Cabinets_WhereToOrder;
            drNew["Disposal_Qty"] = Kitheninfo.Disposal_Qty;
            drNew["Disposal_Style"] = Kitheninfo.Disposal_Style;
            drNew["Disposal_WhereToOrder"] = Kitheninfo.Disposal_WhereToOrder;
            drNew["Baseboard_Qty"] = Kitheninfo.Baseboard_Qty;
            drNew["Baseboard_Style"] = Kitheninfo.Baseboard_Style;
            drNew["Baseboard_WhereToOrder"] = Kitheninfo.Baseboard_WhereToOrder;
            drNew["Window_Qty"] = Kitheninfo.Window_Qty;
            drNew["Window_Style"] = Kitheninfo.Window_Style;
            drNew["Window_WhereToOrder"] = Kitheninfo.Window_WhereToOrder;
            drNew["Door_Qty"] = Kitheninfo.Door_Qty;
            drNew["Door_Style"] = Kitheninfo.Door_Style;
            drNew["Door_WhereToOrder"] = Kitheninfo.Door_WhereToOrder;
            drNew["Lighting_Qty"] = Kitheninfo.Lighting_Qty;
            drNew["Lighting_Style"] = Kitheninfo.Lighting_Style;
            drNew["Lighting_WhereToOrder"] = Kitheninfo.Lighting_WhereToOrder;
            drNew["Hardware_Qty"] = Kitheninfo.Hardware_Qty;
            drNew["Hardware_Style"] = Kitheninfo.Hardware_Style;
            drNew["Hardware_WhereToOrder"] = Kitheninfo.Hardware_WhereToOrder;
            drNew["Special_Notes"] = Kitheninfo.Special_Notes;
            drNew["LastUpdatedDate"] = Kitheninfo.LastUpdatedDate;
            drNew["UpdateBy"] = Kitheninfo.UpdateBy;

            tmpTable.Rows.Add(drNew);
        }

        if (objKitchenSSList.Count() == 0)
        {
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["KitchenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["KitchenSheetName"] = "";
            drNew1["Sink_Qty"] = "";
            drNew1["Sink_Style"] = "";
            drNew1["Sink_WhereToOrder"] = "";
            drNew1["Sink_Fuacet_Qty"] = "";
            drNew1["Sink_Fuacet_Style"] = "";
            drNew1["Sink_Fuacet_WhereToOrder"] = "";
            drNew1["Sink_Drain_Qty"] = "";
            drNew1["Sink_Drain_Style"] = "";
            drNew1["Sink_Drain_WhereToOrder"] = "";
            drNew1["Counter_Top_Qty"] = "";
            drNew1["Counter_Top_Style"] = "";
            drNew1["Counter_Top_WhereToOrder"] = "";
            drNew1["Granite_Quartz_Backsplash_Qty"] = "";
            drNew1["Granite_Quartz_Backsplash_Style"] = "";
            drNew1["Granite_Quartz_Backsplash_WhereToOrder"] = "";
            drNew1["Counter_Top_Overhang_Qty"] = "";
            drNew1["Counter_Top_Overhang_Style"] = "";
            drNew1["Counter_Top_Overhang_WhereToOrder"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Qty"] = "";
            drNew1["AdditionalPlacesGettingCountertop_Style"] = "";
            drNew1["AdditionalPlacesGettingCountertop_WhereToOrder"] = "";
            drNew1["Counter_To_Edge_Qty"] = "";
            drNew1["Counter_To_Edge_Style"] = "";
            drNew1["Counter_To_Edge_WhereToOrder"] = "";
            drNew1["Cabinets_Qty"] = "";
            drNew1["Cabinets_Style"] = "";
            drNew1["Cabinets_WhereToOrder"] = "";
            drNew1["Disposal_Qty"] = "";
            drNew1["Disposal_Style"] = "";
            drNew1["Disposal_WhereToOrder"] = "";
            drNew1["Baseboard_Qty"] = "";
            drNew1["Baseboard_Style"] = "";
            drNew1["Baseboard_WhereToOrder"] = "";
            drNew1["Window_Qty"] = "";
            drNew1["Window_Style"] = "";
            drNew1["Window_WhereToOrder"] = "";
            drNew1["Door_Qty"] = "";
            drNew1["Door_Style"] = "";
            drNew1["Door_WhereToOrder"] = "";
            drNew1["Lighting_Qty"] = "";
            drNew1["Lighting_Style"] = "";
            drNew1["Lighting_WhereToOrder"] = "";
            drNew1["Hardware_Qty"] = "";
            drNew1["Hardware_Style"] = "";
            drNew1["Hardware_WhereToOrder"] = "";
            drNew1["Special_Notes"] = "";
            drNew1["LastUpdatedDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;

            tmpTable.Rows.InsertAt(drNew1, 0);
        }

        Session.Add("sKitchenSection", tmpTable);

        grdKitchenSelectionSheet.DataSource = tmpTable;
        grdKitchenSelectionSheet.DataKeyNames = new string[] { "KitchenID", "customer_id", "estimate_id" };
        grdKitchenSelectionSheet.DataBind();



    }

    #endregion

    #region KitchenTile
    protected void GetKitchenTile()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable tmpTable = LoadKitchenTileTable();

        var objKitTileSSList = _db.KitchenSheetSelections.Where(cp => cp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cp.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).ToList();

        foreach (KitchenSheetSelection objKitTileSS in objKitTileSSList)
        {

            DataRow drNew = tmpTable.NewRow();

            drNew["AutoKithenID"] = objKitTileSS.AutoKithenID;
            drNew["customer_id"] = objKitTileSS.customer_id;
            drNew["estimate_id"] = objKitTileSS.estimate_id;
            drNew["KitchenTileSheetName"] = objKitTileSS.KitchenTileSheetName;
            drNew["BacksplashQTY"] = objKitTileSS.BacksplashQTY;
            drNew["BacksplashMOU"] = objKitTileSS.BacksplashMOU;
            drNew["BacksplashStyle"] = objKitTileSS.BacksplashStyle;
            drNew["BacksplashColor"] = objKitTileSS.BacksplashColor;
            drNew["BacksplashSize"] = objKitTileSS.BacksplashSize;
            drNew["BacksplashVendor"] = objKitTileSS.BacksplashVendor;
            drNew["BacksplashPattern"] = objKitTileSS.BacksplashPattern;
            drNew["BacksplashGroutColor"] = objKitTileSS.BacksplashGroutColor;
            drNew["BBullnoseQTY"] = objKitTileSS.BBullnoseQTY;
            drNew["BBullnoseMOU"] = objKitTileSS.BBullnoseMOU;
            drNew["BBullnoseStyle"] = objKitTileSS.BBullnoseStyle;
            drNew["BBullnoseColor"] = objKitTileSS.BBullnoseColor;
            drNew["BBullnoseSize"] = objKitTileSS.BBullnoseSize;
            drNew["BBullnoseVendor"] = objKitTileSS.BBullnoseVendor;
            drNew["SchluterNOSticks"] = objKitTileSS.SchluterNOSticks;
            drNew["SchluterColor"] = objKitTileSS.SchluterColor;
            drNew["SchluterProfile"] = objKitTileSS.SchluterProfile;
            drNew["SchluterThickness"] = objKitTileSS.SchluterThickness;
            drNew["FloorQTY"] = objKitTileSS.FloorQTY;
            drNew["FloorMOU"] = objKitTileSS.FloorMOU;
            drNew["FloorStyle"] = objKitTileSS.FloorStyle;
            drNew["FloorColor"] = objKitTileSS.FloorColor;
            drNew["FloorSize"] = objKitTileSS.FloorSize;
            drNew["FloorVendor"] = objKitTileSS.FloorVendor;
            drNew["FloorPattern"] = objKitTileSS.FloorPattern;
            drNew["FloorDirection"] = objKitTileSS.FloorDirection;
            drNew["BaseboardQTY"] = objKitTileSS.BaseboardQTY;
            drNew["BaseboardMOU"] = objKitTileSS.BaseboardMOU;
            drNew["BaseboardStyle"] = objKitTileSS.BaseboardStyle;
            drNew["BaseboardColor"] = objKitTileSS.BaseboardColor;
            drNew["BaseboardSize"] = objKitTileSS.BaseboardSize;
            drNew["BaseboardVendor"] = objKitTileSS.BaseboardVendor;
            drNew["FloorGroutColor"] = objKitTileSS.FloorGroutColor;
            drNew["LastUpdateDate"] = objKitTileSS.LastUpdateDate;
            drNew["UpdateBy"] = objKitTileSS.UpdateBy;

            tmpTable.Rows.Add(drNew);
        }

        if (objKitTileSSList.Count() == 0)
        {
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["AutoKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["KitchenTileSheetName"] = "";
            drNew1["BacksplashQTY"] = "";
            drNew1["BacksplashMOU"] = "";
            drNew1["BacksplashStyle"] = "";
            drNew1["BacksplashColor"] = "";
            drNew1["BacksplashSize"] = "";
            drNew1["BacksplashVendor"] = "";
            drNew1["BacksplashPattern"] = "";
            drNew1["BacksplashGroutColor"] = "";
            drNew1["BBullnoseQTY"] = "";
            drNew1["BBullnoseMOU"] = "";
            drNew1["BBullnoseStyle"] = "";
            drNew1["BBullnoseColor"] = "";
            drNew1["BBullnoseSize"] = "";
            drNew1["BBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
        }

        Session.Add("sKitchenTileSection", tmpTable);

        grdKitchenTileSelectionSheet.DataSource = tmpTable;
        grdKitchenTileSelectionSheet.DataKeyNames = new string[] { "AutoKithenID", "customer_id", "estimate_id" };
        grdKitchenTileSelectionSheet.DataBind();



    }

    protected void grdKitchenTileSelectionSheet_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nAutoKithenID = Convert.ToInt32(grdKitchenTileSelectionSheet.DataKeys[e.Row.RowIndex].Values[0].ToString());

            LinkButton lnkKitchenTileDelete = (LinkButton)e.Row.FindControl("lnkKitchenTileDelete");

            lnkKitchenTileDelete.Attributes["CommandArgument"] = string.Format("{0}", nAutoKithenID);

            if (nAutoKithenID > 0)
                lnkKitchenTileDelete.Visible = true;

        }
    }

    protected void btnKitchenTileSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnKitchenTileSave.ID, btnKitchenTileSave.GetType().Name, "Click"); 
        lblResult.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        string strRequired = string.Empty;
        try
        {
            DataTable table = (DataTable)Session["sKitchenTileSection"];



            foreach (GridViewRow row in grdKitchenTileSelectionSheet.Rows)
            {
                Label lblKitchenTileSheetName = (Label)row.FindControl("lblKitchenTileSheetName");
                TextBox txKitchenTileSheetName = (TextBox)row.FindControl("txKitchenTileSheetName");

                TextBox txtBacksplashQTY1 = (TextBox)row.FindControl("txtBacksplashQTY1");
                TextBox txtBacksplashMOU1 = (TextBox)row.FindControl("txtBacksplashMOU1");
                TextBox txtBacksplashStyle1 = (TextBox)row.FindControl("txtBacksplashStyle1");
                TextBox txtBacksplashColor1 = (TextBox)row.FindControl("txtBacksplashColor1");
                TextBox txtBacksplashSize1 = (TextBox)row.FindControl("txtBacksplashSize1");
                TextBox txtBacksplashVendor1 = (TextBox)row.FindControl("txtBacksplashVendor1");
                TextBox txtBacksplashPattern1 = (TextBox)row.FindControl("txtBacksplashPattern1");
                TextBox txtBacksplashGroutColor1 = (TextBox)row.FindControl("txtBacksplashGroutColor1");
                TextBox txtBBullnoseQTY1 = (TextBox)row.FindControl("txtBBullnoseQTY1");
                TextBox txtBBullnoseMOU1 = (TextBox)row.FindControl("txtBBullnoseMOU1");
                TextBox txtBBullnoseStyle1 = (TextBox)row.FindControl("txtBBullnoseStyle1");
                TextBox txtBBullnoseColor1 = (TextBox)row.FindControl("txtBBullnoseColor1");
                TextBox txtBBullnoseSize1 = (TextBox)row.FindControl("txtBBullnoseSize1");
                TextBox txtBBullnoseVendor1 = (TextBox)row.FindControl("txtBBullnoseVendor1");
                TextBox txtSchluterNOSticks1 = (TextBox)row.FindControl("txtSchluterNOSticks1");
                TextBox txtSchluterColor1 = (TextBox)row.FindControl("txtSchluterColor1");
                TextBox txtSchluterProfile1 = (TextBox)row.FindControl("txtSchluterProfile1");
                TextBox txtSchluterThickness1 = (TextBox)row.FindControl("txtSchluterThickness1");
                TextBox txtFloorQTY1 = (TextBox)row.FindControl("txtFloorQTY1");
                TextBox txtFloorMOU1 = (TextBox)row.FindControl("txtFloorMOU1");
                TextBox txtFloorStyle1 = (TextBox)row.FindControl("txtFloorStyle1");
                TextBox txtFloorColor1 = (TextBox)row.FindControl("txtFloorColor1");
                TextBox txtFloorSize1 = (TextBox)row.FindControl("txtFloorSize1");
                TextBox txtFloorVendor1 = (TextBox)row.FindControl("txtFloorVendor1");
                TextBox txtFloorPattern1 = (TextBox)row.FindControl("txtFloorPattern1");
                TextBox txtFloorDirection1 = (TextBox)row.FindControl("txtFloorDirection1");
                TextBox txtBaseboardQTY1 = (TextBox)row.FindControl("txtBaseboardQTY1");
                TextBox txtBaseboardMOU1 = (TextBox)row.FindControl("txtBaseboardMOU1");
                TextBox txtBaseboardStyle1 = (TextBox)row.FindControl("txtBaseboardStyle1");
                TextBox txtBaseboardColor1 = (TextBox)row.FindControl("txtBaseboardColor1");
                TextBox txtBaseboardSize1 = (TextBox)row.FindControl("txtBaseboardSize1");
                TextBox txtBaseboardVendor1 = (TextBox)row.FindControl("txtBaseboardVendor1");
                TextBox txtFloorGroutColor1 = (TextBox)row.FindControl("txtFloorGroutColor1");


                if (txKitchenTileSheetName.Text.Trim() == "")
                {
                    lblKitchenTileSheetName.Text = csCommonUtility.GetSystemRequiredMessage2("Title/Location of Kitchen Tile is required.");
                    strRequired = "required";
                }
                else
                    lblKitchenTileSheetName.Text = "";


                if (strRequired.Length == 0)
                {
                    DataRow dr = table.Rows[row.RowIndex];

                    dr["AutoKithenID"] = Convert.ToInt32(grdKitchenTileSelectionSheet.DataKeys[row.RowIndex].Values[0]);
                    dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                    dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                    dr["KitchenTileSheetName"] = txKitchenTileSheetName.Text.Trim();
                    dr["BacksplashQTY"] = txtBacksplashQTY1.Text;
                    dr["BacksplashMOU"] = txtBacksplashMOU1.Text;
                    dr["BacksplashStyle"] = txtBacksplashStyle1.Text;
                    dr["BacksplashColor"] = txtBacksplashColor1.Text;
                    dr["BacksplashSize"] = txtBacksplashSize1.Text;
                    dr["BacksplashVendor"] = txtBacksplashVendor1.Text;
                    dr["BacksplashPattern"] = txtBacksplashPattern1.Text;
                    dr["BacksplashGroutColor"] = txtBacksplashGroutColor1.Text;
                    dr["BBullnoseQTY"] = txtBBullnoseQTY1.Text;
                    dr["BBullnoseMOU"] = txtBBullnoseMOU1.Text;
                    dr["BBullnoseStyle"] = txtBBullnoseStyle1.Text;
                    dr["BBullnoseColor"] = txtBBullnoseColor1.Text;
                    dr["BBullnoseSize"] = txtBBullnoseSize1.Text;
                    dr["BBullnoseVendor"] = txtBBullnoseVendor1.Text;
                    dr["SchluterNOSticks"] = txtSchluterNOSticks1.Text;
                    dr["SchluterColor"] = txtSchluterColor1.Text;
                    dr["SchluterProfile"] = txtSchluterProfile1.Text;
                    dr["SchluterThickness"] = txtSchluterThickness1.Text;
                    dr["FloorQTY"] = txtFloorQTY1.Text;
                    dr["FloorMOU"] = txtFloorMOU1.Text;
                    dr["FloorStyle"] = txtFloorStyle1.Text;
                    dr["FloorColor"] = txtFloorColor1.Text;
                    dr["FloorSize"] = txtFloorSize1.Text;
                    dr["FloorVendor"] = txtFloorVendor1.Text;
                    dr["FloorPattern"] = txtFloorPattern1.Text;
                    dr["FloorDirection"] = txtFloorDirection1.Text;
                    dr["BaseboardQTY"] = txtBaseboardQTY1.Text;
                    dr["BaseboardMOU"] = txtBaseboardMOU1.Text;
                    dr["BaseboardStyle"] = txtBaseboardStyle1.Text;
                    dr["BaseboardColor"] = txtBaseboardColor1.Text;
                    dr["BaseboardSize"] = txtBaseboardSize1.Text;
                    dr["BaseboardVendor"] = txtBaseboardVendor1.Text;
                    dr["FloorGroutColor"] = txtFloorGroutColor1.Text;
                    dr["LastUpdateDate"] = DateTime.Now;
                    dr["UpdateBy"] = User.Identity.Name;
                }
            }
            if (strRequired.Length == 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    bool bFlagNew = false;

                    KitchenSheetSelection objKitTileSS = _db.KitchenSheetSelections.SingleOrDefault(l => l.AutoKithenID == Convert.ToInt32(dr["AutoKithenID"]));
                    if (objKitTileSS == null)
                    {
                        objKitTileSS = new KitchenSheetSelection();
                        bFlagNew = true;

                    }


                    objKitTileSS.AutoKithenID = Convert.ToInt32(dr["AutoKithenID"]);
                    objKitTileSS.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    objKitTileSS.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                    objKitTileSS.KitchenTileSheetName = dr["KitchenTileSheetName"].ToString();
                    objKitTileSS.BacksplashQTY = dr["BacksplashQTY"].ToString();
                    objKitTileSS.BacksplashMOU = dr["BacksplashMOU"].ToString();
                    objKitTileSS.BacksplashStyle = dr["BacksplashStyle"].ToString();
                    objKitTileSS.BacksplashColor = dr["BacksplashColor"].ToString();
                    objKitTileSS.BacksplashSize = dr["BacksplashSize"].ToString();
                    objKitTileSS.BacksplashVendor = dr["BacksplashVendor"].ToString();
                    objKitTileSS.BacksplashPattern = dr["BacksplashPattern"].ToString();
                    objKitTileSS.BacksplashGroutColor = dr["BacksplashGroutColor"].ToString();
                    objKitTileSS.BBullnoseQTY = dr["BBullnoseQTY"].ToString();
                    objKitTileSS.BBullnoseMOU = dr["BBullnoseMOU"].ToString();
                    objKitTileSS.BBullnoseStyle = dr["BBullnoseStyle"].ToString();
                    objKitTileSS.BBullnoseColor = dr["BBullnoseColor"].ToString();
                    objKitTileSS.BBullnoseSize = dr["BBullnoseSize"].ToString();
                    objKitTileSS.BBullnoseVendor = dr["BBullnoseVendor"].ToString();
                    objKitTileSS.SchluterNOSticks = dr["SchluterNOSticks"].ToString();
                    objKitTileSS.SchluterColor = dr["SchluterColor"].ToString();
                    objKitTileSS.SchluterProfile = dr["SchluterProfile"].ToString();
                    objKitTileSS.SchluterThickness = dr["SchluterThickness"].ToString();
                    objKitTileSS.FloorQTY = dr["FloorQTY"].ToString();
                    objKitTileSS.FloorMOU = dr["FloorMOU"].ToString();
                    objKitTileSS.FloorStyle = dr["FloorStyle"].ToString();
                    objKitTileSS.FloorColor = dr["FloorColor"].ToString();
                    objKitTileSS.FloorSize = dr["FloorSize"].ToString();
                    objKitTileSS.FloorVendor = dr["FloorVendor"].ToString();
                    objKitTileSS.FloorPattern = dr["FloorPattern"].ToString();
                    objKitTileSS.FloorDirection = dr["FloorDirection"].ToString();
                    objKitTileSS.BaseboardQTY = dr["BaseboardQTY"].ToString();
                    objKitTileSS.BaseboardMOU = dr["BaseboardMOU"].ToString();
                    objKitTileSS.BaseboardStyle = dr["BaseboardStyle"].ToString();
                    objKitTileSS.BaseboardColor = dr["BaseboardColor"].ToString();
                    objKitTileSS.BaseboardSize = dr["BaseboardSize"].ToString();
                    objKitTileSS.BaseboardVendor = dr["BaseboardVendor"].ToString();
                    objKitTileSS.FloorGroutColor = dr["FloorGroutColor"].ToString();

                    objKitTileSS.LastUpdateDate = DateTime.Now;
                    objKitTileSS.UpdateBy = User.Identity.Name;


                    if (bFlagNew)
                    {
                        _db.KitchenSheetSelections.InsertOnSubmit(objKitTileSS);
                    }
                }


                lblKitchenTileResult.Text = csCommonUtility.GetSystemMessage("Data has been saved successfully");

                _db.SubmitChanges();
                GetKitchenTile();
            }
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void lnkKitchenTileDelete_Click(object sender, EventArgs e)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            LinkButton lnkKitchenTileDelete = (LinkButton)sender;
            int nKitchenTileSheetID = Convert.ToInt32(lnkKitchenTileDelete.Attributes["CommandArgument"]);
            string strQ = string.Empty;

            strQ = "Delete FROM KitchenSheetSelection  WHERE AutoKithenID =" + nKitchenTileSheetID;
            _db.ExecuteCommand(strQ, string.Empty);
            GetKitchenTile();
        }
        catch (Exception ex)
        {
            lblResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }


    }

    protected void btnAddKitchenTileItem_Click(object sender, EventArgs e)
    {
        KitchenSheetSelection objKitTileSS = new KitchenSheetSelection();
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable table = (DataTable)Session["sKitchenTileSection"];

        int nAutoKithenID = Convert.ToInt32(hdnCabinetSheetID.Value);
        bool contains = table.AsEnumerable().Any(row => nAutoKithenID == row.Field<int>("AutoKithenID"));
        if (contains)
        {
            lblResult.Text = csCommonUtility.GetSystemRequiredMessage("You have already a pending item to save.");
            return;
        }

        foreach (GridViewRow row in grdKitchenTileSelectionSheet.Rows)
        {

            DataRow dr = table.Rows[row.RowIndex];

            Label lblKitchenTileSheetName = (Label)row.FindControl("lblKitchenTileSheetName");
            TextBox txKitchenTileSheetName = (TextBox)row.FindControl("txKitchenTileSheetName");

            TextBox txtBacksplashQTY1 = (TextBox)row.FindControl("txtBacksplashQTY1");
            TextBox txtBacksplashMOU1 = (TextBox)row.FindControl("txtBacksplashMOU1");
            TextBox txtBacksplashStyle1 = (TextBox)row.FindControl("txtBacksplashStyle1");
            TextBox txtBacksplashColor1 = (TextBox)row.FindControl("txtBacksplashColor1");
            TextBox txtBacksplashSize1 = (TextBox)row.FindControl("txtBacksplashSize1");
            TextBox txtBacksplashVendor1 = (TextBox)row.FindControl("txtBacksplashVendor1");
            TextBox txtBacksplashPattern1 = (TextBox)row.FindControl("txtBacksplashPattern1");
            TextBox txtBacksplashGroutColor1 = (TextBox)row.FindControl("txtBacksplashGroutColor1");
            TextBox txtBBullnoseQTY1 = (TextBox)row.FindControl("txtBBullnoseQTY1");
            TextBox txtBBullnoseMOU1 = (TextBox)row.FindControl("txtBBullnoseMOU1");
            TextBox txtBBullnoseStyle1 = (TextBox)row.FindControl("txtBBullnoseStyle1");
            TextBox txtBBullnoseColor1 = (TextBox)row.FindControl("txtBBullnoseColor1");
            TextBox txtBBullnoseSize1 = (TextBox)row.FindControl("txtBBullnoseSize1");
            TextBox txtBBullnoseVendor1 = (TextBox)row.FindControl("txtBBullnoseVendor1");
            TextBox txtSchluterNOSticks1 = (TextBox)row.FindControl("txtSchluterNOSticks1");
            TextBox txtSchluterColor1 = (TextBox)row.FindControl("txtSchluterColor1");
            TextBox txtSchluterProfile1 = (TextBox)row.FindControl("txtSchluterProfile1");
            TextBox txtSchluterThickness1 = (TextBox)row.FindControl("txtSchluterThickness1");
            TextBox txtFloorQTY1 = (TextBox)row.FindControl("txtFloorQTY1");
            TextBox txtFloorMOU1 = (TextBox)row.FindControl("txtFloorMOU1");
            TextBox txtFloorStyle1 = (TextBox)row.FindControl("txtFloorStyle1");
            TextBox txtFloorColor1 = (TextBox)row.FindControl("txtFloorColor1");
            TextBox txtFloorSize1 = (TextBox)row.FindControl("txtFloorSize1");
            TextBox txtFloorVendor1 = (TextBox)row.FindControl("txtFloorVendor1");
            TextBox txtFloorPattern1 = (TextBox)row.FindControl("txtFloorPattern1");
            TextBox txtFloorDirection1 = (TextBox)row.FindControl("txtFloorDirection1");
            TextBox txtBaseboardQTY1 = (TextBox)row.FindControl("txtBaseboardQTY1");
            TextBox txtBaseboardMOU1 = (TextBox)row.FindControl("txtBaseboardMOU1");
            TextBox txtBaseboardStyle1 = (TextBox)row.FindControl("txtBaseboardStyle1");
            TextBox txtBaseboardColor1 = (TextBox)row.FindControl("txtBaseboardColor1");
            TextBox txtBaseboardSize1 = (TextBox)row.FindControl("txtBaseboardSize1");
            TextBox txtBaseboardVendor1 = (TextBox)row.FindControl("txtBaseboardVendor1");
            TextBox txtFloorGroutColor1 = (TextBox)row.FindControl("txtFloorGroutColor1");


            dr["AutoKithenID"] = Convert.ToInt32(grdKitchenTileSelectionSheet.DataKeys[row.RowIndex].Values[0]);
            dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            dr["KitchenTileSheetName"] = txKitchenTileSheetName.Text.Trim();
            dr["BacksplashQTY"] = txtBacksplashQTY1.Text;
            dr["BacksplashMOU"] = txtBacksplashMOU1.Text;
            dr["BacksplashStyle"] = txtBacksplashStyle1.Text;
            dr["BacksplashColor"] = txtBacksplashColor1.Text;
            dr["BacksplashSize"] = txtBacksplashSize1.Text;
            dr["BacksplashVendor"] = txtBacksplashVendor1.Text;
            dr["BacksplashPattern"] = txtBacksplashPattern1.Text;
            dr["BacksplashGroutColor"] = txtBacksplashGroutColor1.Text;
            dr["BBullnoseQTY"] = txtBBullnoseQTY1.Text;
            dr["BBullnoseMOU"] = txtBBullnoseMOU1.Text;
            dr["BBullnoseStyle"] = txtBBullnoseStyle1.Text;
            dr["BBullnoseColor"] = txtBBullnoseColor1.Text;
            dr["BBullnoseSize"] = txtBBullnoseSize1.Text;
            dr["BBullnoseVendor"] = txtBBullnoseVendor1.Text;
            dr["SchluterNOSticks"] = txtSchluterNOSticks1.Text;
            dr["SchluterColor"] = txtSchluterColor1.Text;
            dr["SchluterProfile"] = txtSchluterProfile1.Text;
            dr["SchluterThickness"] = txtSchluterThickness1.Text;
            dr["FloorQTY"] = txtFloorQTY1.Text;
            dr["FloorMOU"] = txtFloorMOU1.Text;
            dr["FloorStyle"] = txtFloorStyle1.Text;
            dr["FloorColor"] = txtFloorColor1.Text;
            dr["FloorSize"] = txtFloorSize1.Text;
            dr["FloorVendor"] = txtFloorVendor1.Text;
            dr["FloorPattern"] = txtFloorPattern1.Text;
            dr["FloorDirection"] = txtFloorDirection1.Text;
            dr["BaseboardQTY"] = txtBaseboardQTY1.Text;
            dr["BaseboardMOU"] = txtBaseboardMOU1.Text;
            dr["BaseboardStyle"] = txtBaseboardStyle1.Text;
            dr["BaseboardColor"] = txtBaseboardColor1.Text;
            dr["BaseboardSize"] = txtBaseboardSize1.Text;
            dr["BaseboardVendor"] = txtBaseboardVendor1.Text;
            dr["FloorGroutColor"] = txtFloorGroutColor1.Text;
            dr["LastUpdateDate"] = DateTime.Now;
            dr["UpdateBy"] = User.Identity.Name;
        }

        DataRow drNew = table.NewRow();

        drNew["AutoKithenID"] = Convert.ToInt32(hdnKitchenTileSheetID.Value);
        drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
        drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
        drNew["KitchenTileSheetName"] = "";
        drNew["BacksplashQTY"] = "";
        drNew["BacksplashMOU"] = "";
        drNew["BacksplashStyle"] = "";
        drNew["BacksplashColor"] = "";
        drNew["BacksplashSize"] = "";
        drNew["BacksplashVendor"] = "";
        drNew["BacksplashPattern"] = "";
        drNew["BacksplashGroutColor"] = "";
        drNew["BBullnoseQTY"] = "";
        drNew["BBullnoseMOU"] = "";
        drNew["BBullnoseStyle"] = "";
        drNew["BBullnoseColor"] = "";
        drNew["BBullnoseSize"] = "";
        drNew["BBullnoseVendor"] = "";
        drNew["SchluterNOSticks"] = "";
        drNew["SchluterColor"] = "";
        drNew["SchluterProfile"] = "";
        drNew["SchluterThickness"] = "";
        drNew["FloorQTY"] = "";
        drNew["FloorMOU"] = "";
        drNew["FloorStyle"] = "";
        drNew["FloorColor"] = "";
        drNew["FloorSize"] = "";
        drNew["FloorVendor"] = "";
        drNew["FloorPattern"] = "";
        drNew["FloorDirection"] = "";
        drNew["BaseboardQTY"] = "";
        drNew["BaseboardMOU"] = "";
        drNew["BaseboardStyle"] = "";
        drNew["BaseboardColor"] = "";
        drNew["BaseboardSize"] = "";
        drNew["BaseboardVendor"] = "";
        drNew["FloorGroutColor"] = "";
        drNew["LastUpdateDate"] = DateTime.Now;
        drNew["UpdateBy"] = User.Identity.Name;

        table.Rows.InsertAt(drNew, 0);

        Session.Add("sKitchenTileSection", table);
        grdKitchenTileSelectionSheet.DataSource = table;
        grdKitchenTileSelectionSheet.DataKeyNames = new string[] { "AutoKithenID", "customer_id", "estimate_id" };
        grdKitchenTileSelectionSheet.DataBind();
        lblKitchenTileResult.Text = "";

    }

    private DataTable LoadKitchenTileTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("AutoKithenID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("KitchenTileSheetName", typeof(string));
        table.Columns.Add("BacksplashQTY", typeof(string));
        table.Columns.Add("BacksplashMOU", typeof(string));
        table.Columns.Add("BacksplashStyle", typeof(string));
        table.Columns.Add("BacksplashColor", typeof(string));
        table.Columns.Add("BacksplashSize", typeof(string));
        table.Columns.Add("BacksplashVendor", typeof(string));
        table.Columns.Add("BacksplashPattern", typeof(string));
        table.Columns.Add("BacksplashGroutColor", typeof(string));
        table.Columns.Add("BBullnoseQTY", typeof(string));
        table.Columns.Add("BBullnoseMOU", typeof(string));
        table.Columns.Add("BBullnoseStyle", typeof(string));
        table.Columns.Add("BBullnoseColor", typeof(string));
        table.Columns.Add("BBullnoseSize", typeof(string));
        table.Columns.Add("BBullnoseVendor", typeof(string));
        table.Columns.Add("SchluterNOSticks", typeof(string));
        table.Columns.Add("SchluterColor", typeof(string));
        table.Columns.Add("SchluterProfile", typeof(string));
        table.Columns.Add("SchluterThickness", typeof(string));
        table.Columns.Add("FloorQTY", typeof(string));
        table.Columns.Add("FloorMOU", typeof(string));
        table.Columns.Add("FloorStyle", typeof(string));
        table.Columns.Add("FloorColor", typeof(string));
        table.Columns.Add("FloorSize", typeof(string));
        table.Columns.Add("FloorVendor", typeof(string));
        table.Columns.Add("FloorPattern", typeof(string));
        table.Columns.Add("FloorDirection", typeof(string));
        table.Columns.Add("BaseboardQTY", typeof(string));
        table.Columns.Add("BaseboardMOU", typeof(string));
        table.Columns.Add("BaseboardStyle", typeof(string));
        table.Columns.Add("BaseboardColor", typeof(string));
        table.Columns.Add("BaseboardSize", typeof(string));
        table.Columns.Add("BaseboardVendor", typeof(string));
        table.Columns.Add("FloorGroutColor", typeof(string));
        table.Columns.Add("UpdateBy", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));


        return table;
    }

    #endregion

    #region TubTile
    protected void GetTubTile()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable tmpTable = LoadTubTileTable();

        var objTubTileSSList = _db.TubSheetSelections.Where(cp => cp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cp.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).ToList();

        foreach (TubSheetSelection objTubTileSS in objTubTileSSList)
        {

            DataRow drNew = tmpTable.NewRow();

            drNew["TubID"] = objTubTileSS.TubID;
            drNew["customer_id"] = objTubTileSS.customer_id;
            drNew["estimate_id"] = objTubTileSS.estimate_id;
            drNew["TubTileSheetName"] = objTubTileSS.TubTileSheetName;
            drNew["WallTileQTY"] = objTubTileSS.WallTileQTY;
            drNew["WallTileMOU"] = objTubTileSS.WallTileMOU;
            drNew["WallTileStyle"] = objTubTileSS.WallTileStyle;
            drNew["WallTileColor"] = objTubTileSS.WallTileColor;
            drNew["WallTileSize"] = objTubTileSS.WallTileSize;
            drNew["WallTileVendor"] = objTubTileSS.WallTileVendor;
            drNew["WallTilePattern"] = objTubTileSS.WallTilePattern;
            drNew["WallTileGroutColor"] = objTubTileSS.WallTileGroutColor;
            drNew["WBullnoseQTY"] = objTubTileSS.WBullnoseQTY;
            drNew["WBullnoseMOU"] = objTubTileSS.WBullnoseMOU;
            drNew["WBullnoseStyle"] = objTubTileSS.WBullnoseStyle;
            drNew["WBullnoseColor"] = objTubTileSS.WBullnoseColor;
            drNew["WBullnoseSize"] = objTubTileSS.WBullnoseSize;
            drNew["WBullnoseVendor"] = objTubTileSS.WBullnoseVendor;
            drNew["SchluterNOSticks"] = objTubTileSS.SchluterNOSticks;
            drNew["SchluterColor"] = objTubTileSS.SchluterColor;
            drNew["SchluterProfile"] = objTubTileSS.SchluterProfile;
            drNew["SchluterThickness"] = objTubTileSS.SchluterThickness;
            drNew["DecobandQTY"] = objTubTileSS.DecobandQTY;
            drNew["DecobandMOU"] = objTubTileSS.DecobandMOU;
            drNew["DecobandStyle"] = objTubTileSS.DecobandStyle;
            drNew["DecobandColor"] = objTubTileSS.DecobandColor;
            drNew["DecobandSize"] = objTubTileSS.DecobandSize;
            drNew["DecobandVendor"] = objTubTileSS.DecobandVendor;
            drNew["DecobandHeight"] = objTubTileSS.DecobandHeight;
            drNew["NicheTileQTY"] = objTubTileSS.NicheTileQTY;
            drNew["NicheTileMOU"] = objTubTileSS.NicheTileMOU;
            drNew["NicheTileStyle"] = objTubTileSS.NicheTileStyle;
            drNew["NicheTileColor"] = objTubTileSS.NicheTileColor;
            drNew["NicheTileSize"] = objTubTileSS.NicheTileSize;
            drNew["NicheTileVendor"] = objTubTileSS.NicheTileVendor;
            drNew["NicheLocation"] = objTubTileSS.NicheLocation;
            drNew["NicheSize"] = objTubTileSS.NicheSize;
            drNew["ShelfLocation"] = objTubTileSS.ShelfLocation;
            drNew["FloorQTY"] = objTubTileSS.FloorQTY;
            drNew["FloorMOU"] = objTubTileSS.FloorMOU;
            drNew["FloorStyle"] = objTubTileSS.FloorStyle;
            drNew["FloorColor"] = objTubTileSS.FloorColor;
            drNew["FloorSize"] = objTubTileSS.FloorSize;
            drNew["FloorVendor"] = objTubTileSS.FloorVendor;
            drNew["FloorPattern"] = objTubTileSS.FloorPattern;
            drNew["FloorDirection"] = objTubTileSS.FloorDirection;
            drNew["BaseboardQTY"] = objTubTileSS.BaseboardQTY;
            drNew["BaseboardMOU"] = objTubTileSS.BaseboardMOU;
            drNew["BaseboardStyle"] = objTubTileSS.BaseboardStyle;
            drNew["BaseboardColor"] = objTubTileSS.BaseboardColor;
            drNew["BaseboardSize"] = objTubTileSS.BaseboardSize;
            drNew["BaseboardVendor"] = objTubTileSS.BaseboardVendor;
            drNew["FloorGroutColor"] = objTubTileSS.FloorGroutColor;
            drNew["TileTo"] = objTubTileSS.TileTo;
            drNew["LastUpdateDate"] = objTubTileSS.LastUpdateDate;
            drNew["UpdateBy"] = objTubTileSS.UpdateBy;

            tmpTable.Rows.Add(drNew);
        }

        if (objTubTileSSList.Count() == 0)
        {
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["TubID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["TubTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["ShelfLocation"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
        }

        Session.Add("sTubTileSection", tmpTable);

        grdTubTileSelectionSheet.DataSource = tmpTable;
        grdTubTileSelectionSheet.DataKeyNames = new string[] { "TubID", "customer_id", "estimate_id" };
        grdTubTileSelectionSheet.DataBind();



    }

    protected void grdTubTileSelectionSheet_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nTubID = Convert.ToInt32(grdTubTileSelectionSheet.DataKeys[e.Row.RowIndex].Values[0].ToString());

            LinkButton lnkTubTileDelete = (LinkButton)e.Row.FindControl("lnkTubTileDelete");

            lnkTubTileDelete.Attributes["CommandArgument"] = string.Format("{0}", nTubID);

            if (nTubID > 0)
                lnkTubTileDelete.Visible = true;

        }
    }

    protected void btnTubTileSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnTubTileSave.ID, btnTubTileSave.GetType().Name, "Click"); 
        lblTubTileResult.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        string strRequired = string.Empty;
        try
        {
            DataTable table = (DataTable)Session["sTubTileSection"];



            foreach (GridViewRow row in grdTubTileSelectionSheet.Rows)
            {
                Label lblTubTileSheetName = (Label)row.FindControl("lblTubTileSheetName");
                TextBox txTubTileSheetName = (TextBox)row.FindControl("txTubTileSheetName");

                TextBox txtWallQTY3 = (TextBox)row.FindControl("txtWallQTY3");
                TextBox txtWallMOU3 = (TextBox)row.FindControl("txtWallMOU3");
                TextBox txtWallStyle3 = (TextBox)row.FindControl("txtWallStyle3");
                TextBox txtWallColor3 = (TextBox)row.FindControl("txtWallColor3");
                TextBox txtWallSize3 = (TextBox)row.FindControl("txtWallSize3");
                TextBox txtWallVendor3 = (TextBox)row.FindControl("txtWallVendor3");
                TextBox txtWallPattern3 = (TextBox)row.FindControl("txtWallPattern3");
                TextBox txtWallGroutColor3 = (TextBox)row.FindControl("txtWallGroutColor3");
                TextBox txtWBullnoseQTY3 = (TextBox)row.FindControl("txtWBullnoseQTY3");
                TextBox txtWBullnoseMOU3 = (TextBox)row.FindControl("txtWBullnoseMOU3");
                TextBox txtWBullnoseStyle3 = (TextBox)row.FindControl("txtWBullnoseStyle3");
                TextBox txtWBullnoseColor3 = (TextBox)row.FindControl("txtWBullnoseColor3");
                TextBox txtWBullnoseSize3 = (TextBox)row.FindControl("txtWBullnoseSize3");
                TextBox txtWBullnoseVendor3 = (TextBox)row.FindControl("txtWBullnoseVendor3");
                TextBox txtSchluterNOSticks3 = (TextBox)row.FindControl("txtSchluterNOSticks3");
                TextBox txtSchluterColor3 = (TextBox)row.FindControl("txtSchluterColor3");
                TextBox txtSchluterProfile3 = (TextBox)row.FindControl("txtSchluterProfile3");
                TextBox txtSchluterThicknes3 = (TextBox)row.FindControl("txtSchluterThicknes3");
                TextBox txtDecobandQTY3 = (TextBox)row.FindControl("txtDecobandQTY3");
                TextBox txtDecobandMOU3 = (TextBox)row.FindControl("txtDecobandMOU3");
                TextBox txtDecobandStyle3 = (TextBox)row.FindControl("txtDecobandStyle3");
                TextBox txtDecobandColor3 = (TextBox)row.FindControl("txtDecobandColor3");
                TextBox txtDecobandSize3 = (TextBox)row.FindControl("txtDecobandSize3");
                TextBox txtDecobandVendor3 = (TextBox)row.FindControl("txtDecobandVendor3");
                TextBox txtDecobandHeight3 = (TextBox)row.FindControl("txtDecobandHeight3");
                TextBox txtNicheTileQTY3 = (TextBox)row.FindControl("txtNicheTileQTY3");
                TextBox txtNicheTileMOU3 = (TextBox)row.FindControl("txtNicheTileMOU3");
                TextBox txtNicheTileStyle3 = (TextBox)row.FindControl("txtNicheTileStyle3");
                TextBox txtNicheTileColor3 = (TextBox)row.FindControl("txtNicheTileColor3");
                TextBox txtNicheTileSize3 = (TextBox)row.FindControl("txtNicheTileSize3");
                TextBox txtNicheTileVendor3 = (TextBox)row.FindControl("txtNicheTileVendor3");
                TextBox txtNicheLocation3 = (TextBox)row.FindControl("txtNicheLocation3");
                TextBox txtNicheSize3 = (TextBox)row.FindControl("txtNicheSize3");
                TextBox txtShelfLocation3 = (TextBox)row.FindControl("txtShelfLocation3");
                TextBox txtFloorQTY3 = (TextBox)row.FindControl("txtFloorQTY3");
                TextBox txtFloorMOU3 = (TextBox)row.FindControl("txtFloorMOU3");
                TextBox txtFloorStyle3 = (TextBox)row.FindControl("txtFloorStyle3");
                TextBox txtFloorColor3 = (TextBox)row.FindControl("txtFloorColor3");
                TextBox txtFloorSize3 = (TextBox)row.FindControl("txtFloorSize3");
                TextBox txtFloorVendor3 = (TextBox)row.FindControl("txtFloorVendor3");
                TextBox txtFloorPattern3 = (TextBox)row.FindControl("txtFloorPattern3");
                TextBox txtFloorDirection3 = (TextBox)row.FindControl("txtFloorDirection3");
                TextBox txtBaseboardQTY3 = (TextBox)row.FindControl("txtBaseboardQTY3");
                TextBox txtBaseboardMOU3 = (TextBox)row.FindControl("txtBaseboardMOU3");
                TextBox txtBaseboardStyle3 = (TextBox)row.FindControl("txtBaseboardStyle3");
                TextBox txtBaseboardColor3 = (TextBox)row.FindControl("txtBaseboardColor3");
                TextBox txtBaseboardSize3 = (TextBox)row.FindControl("txtBaseboardSize3");
                TextBox txtBaseboardVendor3 = (TextBox)row.FindControl("txtBaseboardVendor3");
                TextBox txtFloorGroutColor3 = (TextBox)row.FindControl("txtFloorGroutColor3");
                TextBox txtTileto3 = (TextBox)row.FindControl("txtTileto3");





                if (txTubTileSheetName.Text.Trim() == "")
                {
                    lblTubTileSheetName.Text = csCommonUtility.GetSystemRequiredMessage2("Title/Location of Tub Tile is required.");
                    strRequired = "required";
                }
                else
                    lblTubTileSheetName.Text = "";


                if (strRequired.Length == 0)
                {
                    DataRow dr = table.Rows[row.RowIndex];

                    dr["TubID"] = Convert.ToInt32(grdTubTileSelectionSheet.DataKeys[row.RowIndex].Values[0]);
                    dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                    dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                    dr["TubTileSheetName"] = txTubTileSheetName.Text.Trim();
                    dr["WallTileQTY"] = txtWallQTY3.Text;
                    dr["WallTileMOU"] = txtWallMOU3.Text;
                    dr["WallTileStyle"] = txtWallStyle3.Text;
                    dr["WallTileColor"] = txtWallColor3.Text;
                    dr["WallTileSize"] = txtWallSize3.Text;
                    dr["WallTileVendor"] = txtWallVendor3.Text;
                    dr["WallTilePattern"] = txtWallPattern3.Text;
                    dr["WallTileGroutColor"] = txtWallGroutColor3.Text;
                    dr["WBullnoseQTY"] = txtWBullnoseQTY3.Text;
                    dr["WBullnoseMOU"] = txtWBullnoseMOU3.Text;
                    dr["WBullnoseStyle"] = txtWBullnoseStyle3.Text;
                    dr["WBullnoseColor"] = txtWBullnoseColor3.Text;
                    dr["WBullnoseSize"] = txtWBullnoseSize3.Text;
                    dr["WBullnoseVendor"] = txtWBullnoseVendor3.Text;
                    dr["SchluterNOSticks"] = txtSchluterNOSticks3.Text;
                    dr["SchluterColor"] = txtSchluterColor3.Text;
                    dr["SchluterProfile"] = txtSchluterProfile3.Text;
                    dr["SchluterThickness"] = txtSchluterThicknes3.Text;
                    dr["DecobandQTY"] = txtDecobandQTY3.Text;
                    dr["DecobandMOU"] = txtDecobandMOU3.Text;
                    dr["DecobandStyle"] = txtDecobandStyle3.Text;
                    dr["DecobandColor"] = txtDecobandColor3.Text;
                    dr["DecobandSize"] = txtDecobandSize3.Text;
                    dr["DecobandVendor"] = txtDecobandVendor3.Text;
                    dr["DecobandHeight"] = txtDecobandHeight3.Text;
                    dr["NicheTileQTY"] = txtNicheTileQTY3.Text;
                    dr["NicheTileMOU"] = txtNicheTileMOU3.Text;
                    dr["NicheTileStyle"] = txtNicheTileStyle3.Text;
                    dr["NicheTileColor"] = txtNicheTileColor3.Text;
                    dr["NicheTileSize"] = txtNicheTileSize3.Text;
                    dr["NicheTileVendor"] = txtNicheTileVendor3.Text;
                    dr["NicheLocation"] = txtNicheLocation3.Text;
                    dr["NicheSize"] = txtNicheSize3.Text;
                    dr["ShelfLocation"] = txtShelfLocation3.Text;
                    dr["FloorQTY"] = txtFloorQTY3.Text;
                    dr["FloorMOU"] = txtFloorMOU3.Text;
                    dr["FloorStyle"] = txtFloorStyle3.Text;
                    dr["FloorColor"] = txtFloorColor3.Text;
                    dr["FloorSize"] = txtFloorSize3.Text;
                    dr["FloorVendor"] = txtFloorVendor3.Text;
                    dr["FloorPattern"] = txtFloorPattern3.Text;
                    dr["FloorDirection"] = txtFloorDirection3.Text;
                    dr["BaseboardQTY"] = txtBaseboardQTY3.Text;
                    dr["BaseboardMOU"] = txtBaseboardMOU3.Text;
                    dr["BaseboardStyle"] = txtBaseboardStyle3.Text;
                    dr["BaseboardColor"] = txtBaseboardColor3.Text;
                    dr["BaseboardSize"] = txtBaseboardSize3.Text;
                    dr["BaseboardVendor"] = txtBaseboardVendor3.Text;
                    dr["FloorGroutColor"] = txtFloorGroutColor3.Text;
                    dr["TileTo"] = txtTileto3.Text;
                    dr["LastUpdateDate"] = DateTime.Now;
                    dr["UpdateBy"] = User.Identity.Name;
                }
            }
            if (strRequired.Length == 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    bool bFlagNew = false;

                    TubSheetSelection objTubTileSS = _db.TubSheetSelections.SingleOrDefault(l => l.TubID == Convert.ToInt32(dr["TubID"]));
                    if (objTubTileSS == null)
                    {
                        objTubTileSS = new TubSheetSelection();
                        bFlagNew = true;

                    }


                    objTubTileSS.TubID = Convert.ToInt32(dr["TubID"]);
                    objTubTileSS.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    objTubTileSS.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                    objTubTileSS.TubTileSheetName = dr["TubTileSheetName"].ToString();
                    objTubTileSS.WallTileQTY = dr["WallTileQTY"].ToString();
                    objTubTileSS.WallTileMOU = dr["WallTileMOU"].ToString();
                    objTubTileSS.WallTileStyle = dr["WallTileStyle"].ToString();
                    objTubTileSS.WallTileColor = dr["WallTileColor"].ToString();
                    objTubTileSS.WallTileSize = dr["WallTileSize"].ToString();
                    objTubTileSS.WallTileVendor = dr["WallTileVendor"].ToString();
                    objTubTileSS.WallTilePattern = dr["WallTilePattern"].ToString();
                    objTubTileSS.WallTileGroutColor = dr["WallTileGroutColor"].ToString();
                    objTubTileSS.WBullnoseQTY = dr["WBullnoseQTY"].ToString();
                    objTubTileSS.WBullnoseMOU = dr["WBullnoseMOU"].ToString();
                    objTubTileSS.WBullnoseStyle = dr["WBullnoseStyle"].ToString();
                    objTubTileSS.WBullnoseColor = dr["WBullnoseColor"].ToString();
                    objTubTileSS.WBullnoseSize = dr["WBullnoseSize"].ToString();
                    objTubTileSS.WBullnoseVendor = dr["WBullnoseVendor"].ToString();
                    objTubTileSS.SchluterNOSticks = dr["SchluterNOSticks"].ToString();
                    objTubTileSS.SchluterColor = dr["SchluterColor"].ToString();
                    objTubTileSS.SchluterProfile = dr["SchluterProfile"].ToString();
                    objTubTileSS.SchluterThickness = dr["SchluterThickness"].ToString();
                    objTubTileSS.DecobandQTY = dr["DecobandQTY"].ToString();
                    objTubTileSS.DecobandMOU = dr["DecobandMOU"].ToString();
                    objTubTileSS.DecobandStyle = dr["DecobandStyle"].ToString();
                    objTubTileSS.DecobandColor = dr["DecobandColor"].ToString();
                    objTubTileSS.DecobandSize = dr["DecobandSize"].ToString();
                    objTubTileSS.DecobandVendor = dr["DecobandVendor"].ToString();
                    objTubTileSS.DecobandHeight = dr["DecobandHeight"].ToString();
                    objTubTileSS.NicheTileQTY = dr["NicheTileQTY"].ToString();
                    objTubTileSS.NicheTileMOU = dr["NicheTileMOU"].ToString();
                    objTubTileSS.NicheTileStyle = dr["NicheTileStyle"].ToString();
                    objTubTileSS.NicheTileColor = dr["NicheTileColor"].ToString();
                    objTubTileSS.NicheTileSize = dr["NicheTileSize"].ToString();
                    objTubTileSS.NicheTileVendor = dr["NicheTileVendor"].ToString();
                    objTubTileSS.NicheLocation = dr["NicheLocation"].ToString();
                    objTubTileSS.NicheSize = dr["NicheSize"].ToString();
                    objTubTileSS.ShelfLocation = dr["ShelfLocation"].ToString();
                    objTubTileSS.FloorQTY = dr["FloorQTY"].ToString();
                    objTubTileSS.FloorMOU = dr["FloorMOU"].ToString();
                    objTubTileSS.FloorStyle = dr["FloorStyle"].ToString();
                    objTubTileSS.FloorColor = dr["FloorColor"].ToString();
                    objTubTileSS.FloorSize = dr["FloorSize"].ToString();
                    objTubTileSS.FloorVendor = dr["FloorVendor"].ToString();
                    objTubTileSS.FloorPattern = dr["FloorPattern"].ToString();
                    objTubTileSS.FloorDirection = dr["FloorDirection"].ToString();
                    objTubTileSS.BaseboardQTY = dr["BaseboardQTY"].ToString();
                    objTubTileSS.BaseboardMOU = dr["BaseboardMOU"].ToString();
                    objTubTileSS.BaseboardStyle = dr["BaseboardStyle"].ToString();
                    objTubTileSS.BaseboardColor = dr["BaseboardColor"].ToString();
                    objTubTileSS.BaseboardSize = dr["BaseboardSize"].ToString();
                    objTubTileSS.BaseboardVendor = dr["BaseboardVendor"].ToString();
                    objTubTileSS.FloorGroutColor = dr["FloorGroutColor"].ToString();
                    objTubTileSS.TileTo = dr["TileTo"].ToString();
                    objTubTileSS.LastUpdateDate = DateTime.Now;
                    objTubTileSS.UpdateBy = User.Identity.Name;


                    if (bFlagNew)
                    {
                        _db.TubSheetSelections.InsertOnSubmit(objTubTileSS);
                    }
                }


                lblTubTileResult.Text = csCommonUtility.GetSystemMessage("Data has been saved successfully");

                _db.SubmitChanges();
                GetTubTile();
            }
        }
        catch (Exception ex)
        {
            lblTubTileResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void lnkTubTileDelete_Click(object sender, EventArgs e)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            LinkButton lnkTubTileDelete = (LinkButton)sender;
            int nTubTileSheetID = Convert.ToInt32(lnkTubTileDelete.Attributes["CommandArgument"]);
            string strQ = string.Empty;

            strQ = "Delete FROM TubSheetSelection  WHERE TubID =" + nTubTileSheetID;
            _db.ExecuteCommand(strQ, string.Empty);
            GetTubTile();
        }
        catch (Exception ex)
        {
            lblTubTileResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }


    }

    protected void btnAddTubTileItem_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddTubTileItem.ID, btnAddTubTileItem.GetType().Name, "Click"); 
        TubSheetSelection objKitTileSS = new TubSheetSelection();
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable table = (DataTable)Session["sTubTileSection"];

        int nTubID = Convert.ToInt32(hdnCabinetSheetID.Value);
        bool contains = table.AsEnumerable().Any(row => nTubID == row.Field<int>("TubID"));
        if (contains)
        {
            lblTubTileResult.Text = csCommonUtility.GetSystemRequiredMessage("You have already a pending item to save.");
            return;
        }

        foreach (GridViewRow row in grdTubTileSelectionSheet.Rows)
        {

            DataRow dr = table.Rows[row.RowIndex];

            Label lblTubTileSheetName = (Label)row.FindControl("lblTubTileSheetName");
            TextBox txTubTileSheetName = (TextBox)row.FindControl("txTubTileSheetName");
            TextBox txtWallQTY3 = (TextBox)row.FindControl("txtWallQTY3");
            TextBox txtWallMOU3 = (TextBox)row.FindControl("txtWallMOU3");
            TextBox txtWallStyle3 = (TextBox)row.FindControl("txtWallStyle3");
            TextBox txtWallColor3 = (TextBox)row.FindControl("txtWallColor3");
            TextBox txtWallSize3 = (TextBox)row.FindControl("txtWallSize3");
            TextBox txtWallVendor3 = (TextBox)row.FindControl("txtWallVendor3");
            TextBox txtWallPattern3 = (TextBox)row.FindControl("txtWallPattern3");
            TextBox txtWallGroutColor3 = (TextBox)row.FindControl("txtWallGroutColor3");
            TextBox txtWBullnoseQTY3 = (TextBox)row.FindControl("txtWBullnoseQTY3");
            TextBox txtWBullnoseMOU3 = (TextBox)row.FindControl("txtWBullnoseMOU3");
            TextBox txtWBullnoseStyle3 = (TextBox)row.FindControl("txtWBullnoseStyle3");
            TextBox txtWBullnoseColor3 = (TextBox)row.FindControl("txtWBullnoseColor3");
            TextBox txtWBullnoseSize3 = (TextBox)row.FindControl("txtWBullnoseSize3");
            TextBox txtWBullnoseVendor3 = (TextBox)row.FindControl("txtWBullnoseVendor3");
            TextBox txtSchluterNOSticks3 = (TextBox)row.FindControl("txtSchluterNOSticks3");
            TextBox txtSchluterColor3 = (TextBox)row.FindControl("txtSchluterColor3");
            TextBox txtSchluterProfile3 = (TextBox)row.FindControl("txtSchluterProfile3");
            TextBox txtSchluterThicknes3 = (TextBox)row.FindControl("txtSchluterThicknes3");
            TextBox txtDecobandQTY3 = (TextBox)row.FindControl("txtDecobandQTY3");
            TextBox txtDecobandMOU3 = (TextBox)row.FindControl("txtDecobandMOU3");
            TextBox txtDecobandStyle3 = (TextBox)row.FindControl("txtDecobandStyle3");
            TextBox txtDecobandColor3 = (TextBox)row.FindControl("txtDecobandColor3");
            TextBox txtDecobandSize3 = (TextBox)row.FindControl("txtDecobandSize3");
            TextBox txtDecobandVendor3 = (TextBox)row.FindControl("txtDecobandVendor3");
            TextBox txtDecobandHeight3 = (TextBox)row.FindControl("txtDecobandHeight3");
            TextBox txtNicheTileQTY3 = (TextBox)row.FindControl("txtNicheTileQTY3");
            TextBox txtNicheTileMOU3 = (TextBox)row.FindControl("txtNicheTileMOU3");
            TextBox txtNicheTileStyle3 = (TextBox)row.FindControl("txtNicheTileStyle3");
            TextBox txtNicheTileColor3 = (TextBox)row.FindControl("txtNicheTileColor3");
            TextBox txtNicheTileSize3 = (TextBox)row.FindControl("txtNicheTileSize3");
            TextBox txtNicheTileVendor3 = (TextBox)row.FindControl("txtNicheTileVendor3");
            TextBox txtNicheLocation3 = (TextBox)row.FindControl("txtNicheLocation3");
            TextBox txtNicheSize3 = (TextBox)row.FindControl("txtNicheSize3");
            TextBox txtShelfLocation3 = (TextBox)row.FindControl("txtShelfLocation3");
            TextBox txtFloorQTY3 = (TextBox)row.FindControl("txtFloorQTY3");
            TextBox txtFloorMOU3 = (TextBox)row.FindControl("txtFloorMOU3");
            TextBox txtFloorStyle3 = (TextBox)row.FindControl("txtFloorStyle3");
            TextBox txtFloorColor3 = (TextBox)row.FindControl("txtFloorColor3");
            TextBox txtFloorSize3 = (TextBox)row.FindControl("txtFloorSize3");
            TextBox txtFloorVendor3 = (TextBox)row.FindControl("txtFloorVendor3");
            TextBox txtFloorPattern3 = (TextBox)row.FindControl("txtFloorPattern3");
            TextBox txtFloorDirection3 = (TextBox)row.FindControl("txtFloorDirection3");
            TextBox txtBaseboardQTY3 = (TextBox)row.FindControl("txtBaseboardQTY3");
            TextBox txtBaseboardMOU3 = (TextBox)row.FindControl("txtBaseboardMOU3");
            TextBox txtBaseboardStyle3 = (TextBox)row.FindControl("txtBaseboardStyle3");
            TextBox txtBaseboardColor3 = (TextBox)row.FindControl("txtBaseboardColor3");
            TextBox txtBaseboardSize3 = (TextBox)row.FindControl("txtBaseboardSize3");
            TextBox txtBaseboardVendor3 = (TextBox)row.FindControl("txtBaseboardVendor3");
            TextBox txtFloorGroutColor3 = (TextBox)row.FindControl("txtFloorGroutColor3");
            TextBox txtTileto3 = (TextBox)row.FindControl("txtTileto3");


            dr["TubID"] = Convert.ToInt32(grdTubTileSelectionSheet.DataKeys[row.RowIndex].Values[0]);
            dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            dr["TubTileSheetName"] = txTubTileSheetName.Text.Trim();
            dr["WallTileQTY"] = txtWallQTY3.Text;
            dr["WallTileMOU"] = txtWallMOU3.Text;
            dr["WallTileStyle"] = txtWallStyle3.Text;
            dr["WallTileColor"] = txtWallColor3.Text;
            dr["WallTileSize"] = txtWallSize3.Text;
            dr["WallTileVendor"] = txtWallVendor3.Text;
            dr["WallTilePattern"] = txtWallPattern3.Text;
            dr["WallTileGroutColor"] = txtWallGroutColor3.Text;
            dr["WBullnoseQTY"] = txtWBullnoseQTY3.Text;
            dr["WBullnoseMOU"] = txtWBullnoseMOU3.Text;
            dr["WBullnoseStyle"] = txtWBullnoseStyle3.Text;
            dr["WBullnoseColor"] = txtWBullnoseColor3.Text;
            dr["WBullnoseSize"] = txtWBullnoseSize3.Text;
            dr["WBullnoseVendor"] = txtWBullnoseVendor3.Text;
            dr["SchluterNOSticks"] = txtSchluterNOSticks3.Text;
            dr["SchluterColor"] = txtSchluterColor3.Text;
            dr["SchluterProfile"] = txtSchluterProfile3.Text;
            dr["SchluterThickness"] = txtSchluterThicknes3.Text;
            dr["DecobandQTY"] = txtDecobandQTY3.Text;
            dr["DecobandMOU"] = txtDecobandMOU3.Text;
            dr["DecobandStyle"] = txtDecobandStyle3.Text;
            dr["DecobandColor"] = txtDecobandColor3.Text;
            dr["DecobandSize"] = txtDecobandSize3.Text;
            dr["DecobandVendor"] = txtDecobandVendor3.Text;
            dr["DecobandHeight"] = txtDecobandHeight3.Text;
            dr["NicheTileQTY"] = txtNicheTileQTY3.Text;
            dr["NicheTileMOU"] = txtNicheTileMOU3.Text;
            dr["NicheTileStyle"] = txtNicheTileStyle3.Text;
            dr["NicheTileColor"] = txtNicheTileColor3.Text;
            dr["NicheTileSize"] = txtNicheTileSize3.Text;
            dr["NicheTileVendor"] = txtNicheTileVendor3.Text;
            dr["NicheLocation"] = txtNicheLocation3.Text;
            dr["NicheSize"] = txtNicheSize3.Text;
            dr["ShelfLocation"] = txtShelfLocation3.Text;
            dr["FloorQTY"] = txtFloorQTY3.Text;
            dr["FloorMOU"] = txtFloorMOU3.Text;
            dr["FloorStyle"] = txtFloorStyle3.Text;
            dr["FloorColor"] = txtFloorColor3.Text;
            dr["FloorSize"] = txtFloorSize3.Text;
            dr["FloorVendor"] = txtFloorVendor3.Text;
            dr["FloorPattern"] = txtFloorPattern3.Text;
            dr["FloorDirection"] = txtFloorDirection3.Text;
            dr["BaseboardQTY"] = txtBaseboardQTY3.Text;
            dr["BaseboardMOU"] = txtBaseboardMOU3.Text;
            dr["BaseboardStyle"] = txtBaseboardStyle3.Text;
            dr["BaseboardColor"] = txtBaseboardColor3.Text;
            dr["BaseboardSize"] = txtBaseboardSize3.Text;
            dr["BaseboardVendor"] = txtBaseboardVendor3.Text;
            dr["FloorGroutColor"] = txtFloorGroutColor3.Text;
            dr["TileTo"] = txtTileto3.Text;
            dr["LastUpdateDate"] = DateTime.Now;
            dr["UpdateBy"] = User.Identity.Name;
        }

        DataRow drNew = table.NewRow();

        drNew["TubID"] = Convert.ToInt32(hdnTubTileSheetID.Value);
        drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
        drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
        drNew["TubTileSheetName"] = "";
        drNew["WallTileQTY"] = "";
        drNew["WallTileMOU"] = "";
        drNew["WallTileStyle"] = "";
        drNew["WallTileColor"] = "";
        drNew["WallTileSize"] = "";
        drNew["WallTileVendor"] = "";
        drNew["WallTilePattern"] = "";
        drNew["WallTileGroutColor"] = "";
        drNew["WBullnoseQTY"] = "";
        drNew["WBullnoseMOU"] = "";
        drNew["WBullnoseStyle"] = "";
        drNew["WBullnoseColor"] = "";
        drNew["WBullnoseSize"] = "";
        drNew["WBullnoseVendor"] = "";
        drNew["SchluterNOSticks"] = "";
        drNew["SchluterColor"] = "";
        drNew["SchluterProfile"] = "";
        drNew["SchluterThickness"] = "";
        drNew["DecobandQTY"] = "";
        drNew["DecobandMOU"] = "";
        drNew["DecobandStyle"] = "";
        drNew["DecobandColor"] = "";
        drNew["DecobandSize"] = "";
        drNew["DecobandVendor"] = "";
        drNew["DecobandHeight"] = "";
        drNew["NicheTileQTY"] = "";
        drNew["NicheTileMOU"] = "";
        drNew["NicheTileStyle"] = "";
        drNew["NicheTileColor"] = "";
        drNew["NicheTileSize"] = "";
        drNew["NicheTileVendor"] = "";
        drNew["NicheLocation"] = "";
        drNew["NicheSize"] = "";
        drNew["ShelfLocation"] = "";
        drNew["FloorQTY"] = "";
        drNew["FloorMOU"] = "";
        drNew["FloorStyle"] = "";
        drNew["FloorColor"] = "";
        drNew["FloorSize"] = "";
        drNew["FloorVendor"] = "";
        drNew["FloorPattern"] = "";
        drNew["FloorDirection"] = "";
        drNew["BaseboardQTY"] = "";
        drNew["BaseboardMOU"] = "";
        drNew["BaseboardStyle"] = "";
        drNew["BaseboardColor"] = "";
        drNew["BaseboardSize"] = "";
        drNew["BaseboardVendor"] = "";
        drNew["FloorGroutColor"] = "";
        drNew["TileTo"] = "";
        drNew["LastUpdateDate"] = DateTime.Now;
        drNew["UpdateBy"] = User.Identity.Name;

        table.Rows.InsertAt(drNew, 0);

        Session.Add("sTubTileSection", table);
        grdTubTileSelectionSheet.DataSource = table;
        grdTubTileSelectionSheet.DataKeyNames = new string[] { "TubID", "customer_id", "estimate_id" };
        grdTubTileSelectionSheet.DataBind();
        lblTubTileResult.Text = "";

    }

    private DataTable LoadTubTileTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("TubID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("TubTileSheetName", typeof(string));
        table.Columns.Add("WallTileQTY", typeof(string));
        table.Columns.Add("WallTileMOU", typeof(string));
        table.Columns.Add("WallTileStyle", typeof(string));
        table.Columns.Add("WallTileColor", typeof(string));
        table.Columns.Add("WallTileSize", typeof(string));
        table.Columns.Add("WallTileVendor", typeof(string));
        table.Columns.Add("WallTilePattern", typeof(string));
        table.Columns.Add("WallTileGroutColor", typeof(string));
        table.Columns.Add("WBullnoseQTY", typeof(string));
        table.Columns.Add("WBullnoseMOU", typeof(string));
        table.Columns.Add("WBullnoseStyle", typeof(string));
        table.Columns.Add("WBullnoseColor", typeof(string));
        table.Columns.Add("WBullnoseSize", typeof(string));
        table.Columns.Add("WBullnoseVendor", typeof(string));
        table.Columns.Add("SchluterNOSticks", typeof(string));
        table.Columns.Add("SchluterColor", typeof(string));
        table.Columns.Add("SchluterProfile", typeof(string));
        table.Columns.Add("SchluterThickness", typeof(string));
        table.Columns.Add("DecobandQTY", typeof(string));
        table.Columns.Add("DecobandMOU", typeof(string));
        table.Columns.Add("DecobandStyle", typeof(string));
        table.Columns.Add("DecobandColor", typeof(string));
        table.Columns.Add("DecobandSize", typeof(string));
        table.Columns.Add("DecobandVendor", typeof(string));
        table.Columns.Add("DecobandHeight", typeof(string));
        table.Columns.Add("NicheTileQTY", typeof(string));
        table.Columns.Add("NicheTileMOU", typeof(string));
        table.Columns.Add("NicheTileStyle", typeof(string));
        table.Columns.Add("NicheTileColor", typeof(string));
        table.Columns.Add("NicheTileSize", typeof(string));
        table.Columns.Add("NicheTileVendor", typeof(string));
        table.Columns.Add("NicheLocation", typeof(string));
        table.Columns.Add("NicheSize", typeof(string));
        table.Columns.Add("ShelfLocation", typeof(string));
        table.Columns.Add("FloorQTY", typeof(string));
        table.Columns.Add("FloorMOU", typeof(string));
        table.Columns.Add("FloorStyle", typeof(string));
        table.Columns.Add("FloorColor", typeof(string));
        table.Columns.Add("FloorSize", typeof(string));
        table.Columns.Add("FloorVendor", typeof(string));
        table.Columns.Add("FloorPattern", typeof(string));
        table.Columns.Add("FloorDirection", typeof(string));
        table.Columns.Add("BaseboardQTY", typeof(string));
        table.Columns.Add("BaseboardMOU", typeof(string));
        table.Columns.Add("BaseboardStyle", typeof(string));
        table.Columns.Add("BaseboardColor", typeof(string));
        table.Columns.Add("BaseboardSize", typeof(string));
        table.Columns.Add("BaseboardVendor", typeof(string));
        table.Columns.Add("FloorGroutColor", typeof(string));
        table.Columns.Add("TileTo", typeof(string));
        table.Columns.Add("UpdateBy", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));

        return table;
    }

    #endregion

    #region ShowerTile
    protected void GetShowerTile()
    {
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable tmpTable = LoadShowerTileTable();

        var objShowerTileSSList = _db.ShowerSheetSelections.Where(cp => cp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cp.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).ToList();

        foreach (ShowerSheetSelection objShowerTileSS in objShowerTileSSList)
        {

            DataRow drNew = tmpTable.NewRow();

            drNew["ShowerKithenID"] = objShowerTileSS.ShowerKithenID;
            drNew["customer_id"] = objShowerTileSS.customer_id;
            drNew["estimate_id"] = objShowerTileSS.estimate_id;
            drNew["ShowerTileSheetName"] = objShowerTileSS.ShowerTileSheetName;
            drNew["WallTileQTY"] = objShowerTileSS.WallTileQTY;
            drNew["WallTileMOU"] = objShowerTileSS.WallTileMOU;
            drNew["WallTileStyle"] = objShowerTileSS.WallTileStyle;
            drNew["WallTileColor"] = objShowerTileSS.WallTileColor;
            drNew["WallTileSize"] = objShowerTileSS.WallTileSize;
            drNew["WallTileVendor"] = objShowerTileSS.WallTileVendor;
            drNew["WallTilePattern"] = objShowerTileSS.WallTilePattern;
            drNew["WallTileGroutColor"] = objShowerTileSS.WallTileGroutColor;
            drNew["WBullnoseQTY"] = objShowerTileSS.WBullnoseQTY;
            drNew["WBullnoseMOU"] = objShowerTileSS.WBullnoseMOU;
            drNew["WBullnoseStyle"] = objShowerTileSS.WBullnoseStyle;
            drNew["WBullnoseColor"] = objShowerTileSS.WBullnoseColor;
            drNew["WBullnoseSize"] = objShowerTileSS.WBullnoseSize;
            drNew["WBullnoseVendor"] = objShowerTileSS.WBullnoseVendor;
            drNew["SchluterNOSticks"] = objShowerTileSS.SchluterNOSticks;
            drNew["SchluterColor"] = objShowerTileSS.SchluterColor;
            drNew["SchluterProfile"] = objShowerTileSS.SchluterProfile;
            drNew["SchluterThickness"] = objShowerTileSS.SchluterThickness;
            drNew["ShowerPanQTY"] = objShowerTileSS.ShowerPanQTY;
            drNew["ShowerPanMOU"] = objShowerTileSS.ShowerPanMOU;
            drNew["ShowerPanStyle"] = objShowerTileSS.ShowerPanStyle;
            drNew["ShowerPanColor"] = objShowerTileSS.ShowerPanColor;
            drNew["ShowerPanSize"] = objShowerTileSS.ShowerPanSize;
            drNew["ShowerPanVendor"] = objShowerTileSS.ShowerPanVendor;
            drNew["ShowerPanGroutColor"] = objShowerTileSS.ShowerPanGroutColor;
            drNew["DecobandQTY"] = objShowerTileSS.DecobandQTY;
            drNew["DecobandMOU"] = objShowerTileSS.DecobandMOU;
            drNew["DecobandStyle"] = objShowerTileSS.DecobandStyle;
            drNew["DecobandColor"] = objShowerTileSS.DecobandColor;
            drNew["DecobandSize"] = objShowerTileSS.DecobandSize;
            drNew["DecobandVendor"] = objShowerTileSS.DecobandVendor;
            drNew["DecobandHeight"] = objShowerTileSS.DecobandHeight;
            drNew["NicheTileQTY"] = objShowerTileSS.NicheTileQTY;
            drNew["NicheTileMOU"] = objShowerTileSS.NicheTileMOU;
            drNew["NicheTileStyle"] = objShowerTileSS.NicheTileStyle;
            drNew["NicheTileColor"] = objShowerTileSS.NicheTileColor;
            drNew["NicheTileSize"] = objShowerTileSS.NicheTileSize;
            drNew["NicheTileVendor"] = objShowerTileSS.NicheTileVendor;
            drNew["NicheLocation"] = objShowerTileSS.NicheLocation;
            drNew["NicheSize"] = objShowerTileSS.NicheSize;
            drNew["BenchTileQTY"] = objShowerTileSS.BenchTileQTY;
            drNew["BenchTileMOU"] = objShowerTileSS.BenchTileMOU;
            drNew["BenchTileStyle"] = objShowerTileSS.BenchTileStyle;
            drNew["BenchTileColor"] = objShowerTileSS.BenchTileColor;
            drNew["BenchTileSize"] = objShowerTileSS.BenchTileSize;
            drNew["BenchTileVendor"] = objShowerTileSS.BenchTileVendor;
            drNew["BenchLocation"] = objShowerTileSS.BenchLocation;
            drNew["BenchSize"] = objShowerTileSS.BenchSize;
            drNew["FloorQTY"] = objShowerTileSS.FloorQTY;
            drNew["FloorMOU"] = objShowerTileSS.FloorMOU;
            drNew["FloorStyle"] = objShowerTileSS.FloorStyle;
            drNew["FloorColor"] = objShowerTileSS.FloorColor;
            drNew["FloorSize"] = objShowerTileSS.FloorSize;
            drNew["FloorVendor"] = objShowerTileSS.FloorVendor;
            drNew["FloorPattern"] = objShowerTileSS.FloorPattern;
            drNew["FloorDirection"] = objShowerTileSS.FloorDirection;
            drNew["BaseboardQTY"] = objShowerTileSS.BaseboardQTY;
            drNew["BaseboardMOU"] = objShowerTileSS.BaseboardMOU;
            drNew["BaseboardStyle"] = objShowerTileSS.BaseboardStyle;
            drNew["BaseboardColor"] = objShowerTileSS.BaseboardColor;
            drNew["BaseboardSize"] = objShowerTileSS.BaseboardSize;
            drNew["BaseboardVendor"] = objShowerTileSS.BaseboardVendor;
            drNew["FloorGroutColor"] = objShowerTileSS.FloorGroutColor;
            drNew["TileTo"] = objShowerTileSS.TileTo;
            drNew["LastUpdateDate"] = objShowerTileSS.LastUpdateDate;
            drNew["UpdateBy"] = objShowerTileSS.UpdateBy;


            tmpTable.Rows.Add(drNew);
        }

        if (objShowerTileSSList.Count() == 0)
        {
            DataRow drNew1 = tmpTable.NewRow();

            drNew1["ShowerKithenID"] = 0;
            drNew1["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            drNew1["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            drNew1["ShowerTileSheetName"] = "";
            drNew1["WallTileQTY"] = "";
            drNew1["WallTileMOU"] = "";
            drNew1["WallTileStyle"] = "";
            drNew1["WallTileColor"] = "";
            drNew1["WallTileSize"] = "";
            drNew1["WallTileVendor"] = "";
            drNew1["WallTilePattern"] = "";
            drNew1["WallTileGroutColor"] = "";
            drNew1["WBullnoseQTY"] = "";
            drNew1["WBullnoseMOU"] = "";
            drNew1["WBullnoseStyle"] = "";
            drNew1["WBullnoseColor"] = "";
            drNew1["WBullnoseSize"] = "";
            drNew1["WBullnoseVendor"] = "";
            drNew1["SchluterNOSticks"] = "";
            drNew1["SchluterColor"] = "";
            drNew1["SchluterProfile"] = "";
            drNew1["SchluterThickness"] = "";
            drNew1["ShowerPanQTY"] = "";
            drNew1["ShowerPanMOU"] = "";
            drNew1["ShowerPanStyle"] = "";
            drNew1["ShowerPanColor"] = "";
            drNew1["ShowerPanSize"] = "";
            drNew1["ShowerPanVendor"] = "";
            drNew1["ShowerPanGroutColor"] = "";
            drNew1["DecobandQTY"] = "";
            drNew1["DecobandMOU"] = "";
            drNew1["DecobandStyle"] = "";
            drNew1["DecobandColor"] = "";
            drNew1["DecobandSize"] = "";
            drNew1["DecobandVendor"] = "";
            drNew1["DecobandHeight"] = "";
            drNew1["NicheTileQTY"] = "";
            drNew1["NicheTileMOU"] = "";
            drNew1["NicheTileStyle"] = "";
            drNew1["NicheTileColor"] = "";
            drNew1["NicheTileSize"] = "";
            drNew1["NicheTileVendor"] = "";
            drNew1["NicheLocation"] = "";
            drNew1["NicheSize"] = "";
            drNew1["BenchTileQTY"] = "";
            drNew1["BenchTileMOU"] = "";
            drNew1["BenchTileStyle"] = "";
            drNew1["BenchTileColor"] = "";
            drNew1["BenchTileSize"] = "";
            drNew1["BenchTileVendor"] = "";
            drNew1["BenchLocation"] = "";
            drNew1["BenchSize"] = "";
            drNew1["FloorQTY"] = "";
            drNew1["FloorMOU"] = "";
            drNew1["FloorStyle"] = "";
            drNew1["FloorColor"] = "";
            drNew1["FloorSize"] = "";
            drNew1["FloorVendor"] = "";
            drNew1["FloorPattern"] = "";
            drNew1["FloorDirection"] = "";
            drNew1["BaseboardQTY"] = "";
            drNew1["BaseboardMOU"] = "";
            drNew1["BaseboardStyle"] = "";
            drNew1["BaseboardColor"] = "";
            drNew1["BaseboardSize"] = "";
            drNew1["BaseboardVendor"] = "";
            drNew1["FloorGroutColor"] = "";
            drNew1["TileTo"] = "";
            drNew1["LastUpdateDate"] = DateTime.Now;
            drNew1["UpdateBy"] = User.Identity.Name;


            tmpTable.Rows.InsertAt(drNew1, 0);
        }

        Session.Add("sShowerTileSection", tmpTable);

        grdShowerTileSelectionSheet.DataSource = tmpTable;
        grdShowerTileSelectionSheet.DataKeyNames = new string[] { "ShowerKithenID", "customer_id", "estimate_id" };
        grdShowerTileSelectionSheet.DataBind();



    }

    protected void grdShowerTileSelectionSheet_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int nShowerID = Convert.ToInt32(grdShowerTileSelectionSheet.DataKeys[e.Row.RowIndex].Values[0].ToString());

            LinkButton lnkShowerTileDelete = (LinkButton)e.Row.FindControl("lnkShowerTileDelete");

            lnkShowerTileDelete.Attributes["CommandArgument"] = string.Format("{0}", nShowerID);

            if (nShowerID > 0)
                lnkShowerTileDelete.Visible = true;

        }
    }

    protected void btnShowerTileSave_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnShowerTileSave.ID, btnShowerTileSave.GetType().Name, "Click"); 
        lblShowerTileResult.Text = "";
        DataClassesDataContext _db = new DataClassesDataContext();
        string strRequired = string.Empty;
        try
        {
            DataTable table = (DataTable)Session["sShowerTileSection"];



            foreach (GridViewRow row in grdShowerTileSelectionSheet.Rows)
            {
                Label lblShowerTileSheetName = (Label)row.FindControl("lblShowerTileSheetName");
                TextBox txShowerTileSheetName = (TextBox)row.FindControl("txShowerTileSheetName");

                TextBox txtWallQTY1 = (TextBox)row.FindControl("txtWallQTY1");
                TextBox txtWallMOU1 = (TextBox)row.FindControl("txtWallMOU1");
                TextBox txtWallStyle1 = (TextBox)row.FindControl("txtWallStyle1");
                TextBox txtWallColor1 = (TextBox)row.FindControl("txtWallColor1");
                TextBox txtWallSize1 = (TextBox)row.FindControl("txtWallSize1");
                TextBox txtWallVendor1 = (TextBox)row.FindControl("txtWallVendor1");
                TextBox txtWallPattern1 = (TextBox)row.FindControl("txtWallPattern1");
                TextBox txtWallGroutColor1 = (TextBox)row.FindControl("txtWallGroutColor1");
                TextBox txtSWBullnoseQTY1 = (TextBox)row.FindControl("txtSWBullnoseQTY1");
                TextBox txtSWBullnoseMOU1 = (TextBox)row.FindControl("txtSWBullnoseMOU1");
                TextBox txtSWBullnoseStyle1 = (TextBox)row.FindControl("txtSWBullnoseStyle1");
                TextBox txtSWBullnoseColor1 = (TextBox)row.FindControl("txtSWBullnoseColor1");
                TextBox txtSWBullnoseSize1 = (TextBox)row.FindControl("txtSWBullnoseSize1");
                TextBox txtSWBullnoseVendor1 = (TextBox)row.FindControl("txtSWBullnoseVendor1");
                TextBox txtSchluterNOSticks2 = (TextBox)row.FindControl("txtSchluterNOSticks2");
                TextBox txtSchluterColor2 = (TextBox)row.FindControl("txtSchluterColor2");
                TextBox txtSchluterProfile2 = (TextBox)row.FindControl("txtSchluterProfile2");
                TextBox txtSchluterThicknes2 = (TextBox)row.FindControl("txtSchluterThicknes2");
                TextBox txtShowerPanQTY1 = (TextBox)row.FindControl("txtShowerPanQTY1");
                TextBox txtShowerPanMOU1 = (TextBox)row.FindControl("txtShowerPanMOU1");
                TextBox txtShowerPanStyle1 = (TextBox)row.FindControl("txtShowerPanStyle1");
                TextBox txtShowerPanColor1 = (TextBox)row.FindControl("txtShowerPanColor1");
                TextBox txtShowerPanSize1 = (TextBox)row.FindControl("txtShowerPanSize1");
                TextBox txtShowerPanVendor1 = (TextBox)row.FindControl("txtShowerPanVendor1");
                TextBox txtShowerPanGroutColor1 = (TextBox)row.FindControl("txtShowerPanGroutColor1");
                TextBox txtDecobandQTY1 = (TextBox)row.FindControl("txtDecobandQTY1");
                TextBox txtDecobandMOU1 = (TextBox)row.FindControl("txtDecobandMOU1");
                TextBox txtDecobandStyle1 = (TextBox)row.FindControl("txtDecobandStyle1");
                TextBox txtDecobandColor1 = (TextBox)row.FindControl("txtDecobandColor1");
                TextBox txtDecobandSize1 = (TextBox)row.FindControl("txtDecobandSize1");
                TextBox txtDecobandVendor1 = (TextBox)row.FindControl("txtDecobandVendor1");
                TextBox txtDecobandHeight1 = (TextBox)row.FindControl("txtDecobandHeight1");
                TextBox txtNicheTileQTY1 = (TextBox)row.FindControl("txtNicheTileQTY1");
                TextBox txtNicheTileMOU1 = (TextBox)row.FindControl("txtNicheTileMOU1");
                TextBox txtNicheTileStyle1 = (TextBox)row.FindControl("txtNicheTileStyle1");
                TextBox txtNicheTileColor1 = (TextBox)row.FindControl("txtNicheTileColor1");
                TextBox txtNicheTileSize1 = (TextBox)row.FindControl("txtNicheTileSize1");
                TextBox txtNicheTileVendor1 = (TextBox)row.FindControl("txtNicheTileVendor1");
                TextBox txtNicheLocation1 = (TextBox)row.FindControl("txtNicheLocation1");
                TextBox txtNicheSize1 = (TextBox)row.FindControl("txtNicheSize1");
                TextBox txtBenchTileQTY1 = (TextBox)row.FindControl("txtBenchTileQTY1");
                TextBox txtBenchTileMOU1 = (TextBox)row.FindControl("txtBenchTileMOU1");
                TextBox txtBenchTileStyle1 = (TextBox)row.FindControl("txtBenchTileStyle1");
                TextBox txtBenchTileColor1 = (TextBox)row.FindControl("txtBenchTileColor1");
                TextBox txtBenchTileSize1 = (TextBox)row.FindControl("txtBenchTileSize1");
                TextBox txtBenchTileVendor1 = (TextBox)row.FindControl("txtBenchTileVendor1");
                TextBox txtBenchLocation1 = (TextBox)row.FindControl("txtBenchLocation1");
                TextBox txtBenchSize1 = (TextBox)row.FindControl("txtBenchSize1");
                TextBox txtFloorQTY2 = (TextBox)row.FindControl("txtFloorQTY2");
                TextBox txtFloorMOU2 = (TextBox)row.FindControl("txtFloorMOU2");
                TextBox txtFloorStyle2 = (TextBox)row.FindControl("txtFloorStyle2");
                TextBox txtFloorColor2 = (TextBox)row.FindControl("txtFloorColor2");
                TextBox txtFloorSize2 = (TextBox)row.FindControl("txtFloorSize2");
                TextBox txtFloorVendor2 = (TextBox)row.FindControl("txtFloorVendor2");
                TextBox txtFloorPattern2 = (TextBox)row.FindControl("txtFloorPattern2");
                TextBox txtFloorDirection2 = (TextBox)row.FindControl("txtFloorDirection2");
                TextBox txtBaseboardQTY2 = (TextBox)row.FindControl("txtBaseboardQTY2");
                TextBox txtBaseboardMOU2 = (TextBox)row.FindControl("txtBaseboardMOU2");
                TextBox txtBaseboardStyle2 = (TextBox)row.FindControl("txtBaseboardStyle2");
                TextBox txtBaseboardColor2 = (TextBox)row.FindControl("txtBaseboardColor2");
                TextBox txtBaseboardSize2 = (TextBox)row.FindControl("txtBaseboardSize2");
                TextBox txtBaseboardVendor2 = (TextBox)row.FindControl("txtBaseboardVendor2");
                TextBox txtFloorGroutColor2 = (TextBox)row.FindControl("txtFloorGroutColor2");
                TextBox txtTileto2 = (TextBox)row.FindControl("txtTileto2");

                if (txShowerTileSheetName.Text.Trim() == "")
                {
                    lblShowerTileSheetName.Text = csCommonUtility.GetSystemRequiredMessage2("Title/Location of Shower Tile is required.");
                    strRequired = "required";
                }
                else
                    lblShowerTileSheetName.Text = "";


                if (strRequired.Length == 0)
                {
                    DataRow dr = table.Rows[row.RowIndex];

                    dr["ShowerKithenID"] = Convert.ToInt32(grdShowerTileSelectionSheet.DataKeys[row.RowIndex].Values[0]);
                    dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
                    dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
                    dr["ShowerTileSheetName"] = txShowerTileSheetName.Text.Trim();
                    dr["WallTileQTY"] = txtWallQTY1.Text;
                    dr["WallTileMOU"] = txtWallMOU1.Text;
                    dr["WallTileStyle"] = txtWallStyle1.Text;
                    dr["WallTileColor"] = txtWallColor1.Text;
                    dr["WallTileSize"] = txtWallSize1.Text;
                    dr["WallTileVendor"] = txtWallVendor1.Text;
                    dr["WallTilePattern"] = txtWallPattern1.Text;
                    dr["WallTileGroutColor"] = txtWallGroutColor1.Text;
                    dr["WBullnoseQTY"] = txtSWBullnoseQTY1.Text;
                    dr["WBullnoseMOU"] = txtSWBullnoseMOU1.Text;
                    dr["WBullnoseStyle"] = txtSWBullnoseStyle1.Text;
                    dr["WBullnoseColor"] = txtSWBullnoseColor1.Text;
                    dr["WBullnoseSize"] = txtSWBullnoseSize1.Text;
                    dr["WBullnoseVendor"] = txtSWBullnoseVendor1.Text;
                    dr["SchluterNOSticks"] = txtSchluterNOSticks2.Text;
                    dr["SchluterColor"] = txtSchluterColor2.Text;
                    dr["SchluterProfile"] = txtSchluterProfile2.Text;
                    dr["SchluterThickness"] = txtSchluterThicknes2.Text;
                    dr["ShowerPanQTY"] = txtShowerPanQTY1.Text;
                    dr["ShowerPanMOU"] = txtShowerPanMOU1.Text;
                    dr["ShowerPanStyle"] = txtShowerPanStyle1.Text;
                    dr["ShowerPanColor"] = txtShowerPanColor1.Text;
                    dr["ShowerPanSize"] = txtShowerPanSize1.Text;
                    dr["ShowerPanVendor"] = txtShowerPanVendor1.Text;
                    dr["ShowerPanGroutColor"] = txtShowerPanGroutColor1.Text;
                    dr["DecobandQTY"] = txtDecobandQTY1.Text;
                    dr["DecobandMOU"] = txtDecobandMOU1.Text;
                    dr["DecobandStyle"] = txtDecobandStyle1.Text;
                    dr["DecobandColor"] = txtDecobandColor1.Text;
                    dr["DecobandSize"] = txtDecobandSize1.Text;
                    dr["DecobandVendor"] = txtDecobandVendor1.Text;
                    dr["DecobandHeight"] = txtDecobandHeight1.Text;
                    dr["NicheTileQTY"] = txtNicheTileQTY1.Text;
                    dr["NicheTileMOU"] = txtNicheTileMOU1.Text;
                    dr["NicheTileStyle"] = txtNicheTileStyle1.Text;
                    dr["NicheTileColor"] = txtNicheTileColor1.Text;
                    dr["NicheTileSize"] = txtNicheTileSize1.Text;
                    dr["NicheTileVendor"] = txtNicheTileVendor1.Text;
                    dr["NicheLocation"] = txtNicheLocation1.Text;
                    dr["NicheSize"] = txtNicheSize1.Text;
                    dr["BenchTileQTY"] = txtBenchTileQTY1.Text;
                    dr["BenchTileMOU"] = txtBenchTileMOU1.Text;
                    dr["BenchTileStyle"] = txtBenchTileStyle1.Text;
                    dr["BenchTileColor"] = txtBenchTileColor1.Text;
                    dr["BenchTileSize"] = txtBenchTileSize1.Text;
                    dr["BenchTileVendor"] = txtBenchTileVendor1.Text;
                    dr["BenchLocation"] = txtBenchLocation1.Text;
                    dr["BenchSize"] = txtBenchSize1.Text;
                    dr["FloorQTY"] = txtFloorQTY2.Text;
                    dr["FloorMOU"] = txtFloorMOU2.Text;
                    dr["FloorStyle"] = txtFloorStyle2.Text;
                    dr["FloorColor"] = txtFloorColor2.Text;
                    dr["FloorSize"] = txtFloorSize2.Text;
                    dr["FloorVendor"] = txtFloorVendor2.Text;
                    dr["FloorPattern"] = txtFloorPattern2.Text;
                    dr["FloorDirection"] = txtFloorDirection2.Text;
                    dr["BaseboardQTY"] = txtBaseboardQTY2.Text;
                    dr["BaseboardMOU"] = txtBaseboardMOU2.Text;
                    dr["BaseboardStyle"] = txtBaseboardStyle2.Text;
                    dr["BaseboardColor"] = txtBaseboardColor2.Text;
                    dr["BaseboardSize"] = txtBaseboardSize2.Text;
                    dr["BaseboardVendor"] = txtBaseboardVendor2.Text;
                    dr["FloorGroutColor"] = txtFloorGroutColor2.Text;
                    dr["TileTo"] = txtTileto2.Text;
                    dr["LastUpdateDate"] = DateTime.Now;
                    dr["UpdateBy"] = User.Identity.Name;
                }
            }
            if (strRequired.Length == 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    bool bFlagNew = false;

                    ShowerSheetSelection objShowerTileSS = _db.ShowerSheetSelections.SingleOrDefault(l => l.ShowerKithenID == Convert.ToInt32(dr["ShowerKithenID"]));
                    if (objShowerTileSS == null)
                    {
                        objShowerTileSS = new ShowerSheetSelection();
                        bFlagNew = true;

                    }

                    objShowerTileSS.ShowerKithenID = Convert.ToInt32(dr["ShowerKithenID"]);
                    objShowerTileSS.customer_id = Convert.ToInt32(hdnCustomerId.Value);
                    objShowerTileSS.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
                    objShowerTileSS.ShowerTileSheetName = dr["ShowerTileSheetName"].ToString();
                    objShowerTileSS.WallTileQTY = dr["WallTileQTY"].ToString();
                    objShowerTileSS.WallTileMOU = dr["WallTileMOU"].ToString();
                    objShowerTileSS.WallTileStyle = dr["WallTileStyle"].ToString();
                    objShowerTileSS.WallTileColor = dr["WallTileColor"].ToString();
                    objShowerTileSS.WallTileSize = dr["WallTileSize"].ToString();
                    objShowerTileSS.WallTileVendor = dr["WallTileVendor"].ToString();
                    objShowerTileSS.WallTilePattern = dr["WallTilePattern"].ToString();
                    objShowerTileSS.WallTileGroutColor = dr["WallTileGroutColor"].ToString();
                    objShowerTileSS.WBullnoseQTY = dr["WBullnoseQTY"].ToString();
                    objShowerTileSS.WBullnoseMOU = dr["WBullnoseMOU"].ToString();
                    objShowerTileSS.WBullnoseStyle = dr["WBullnoseStyle"].ToString();
                    objShowerTileSS.WBullnoseColor = dr["WBullnoseColor"].ToString();
                    objShowerTileSS.WBullnoseSize = dr["WBullnoseSize"].ToString();
                    objShowerTileSS.WBullnoseVendor = dr["WBullnoseVendor"].ToString();
                    objShowerTileSS.SchluterNOSticks = dr["SchluterNOSticks"].ToString();
                    objShowerTileSS.SchluterColor = dr["SchluterColor"].ToString();
                    objShowerTileSS.SchluterProfile = dr["SchluterProfile"].ToString();
                    objShowerTileSS.SchluterThickness = dr["SchluterThickness"].ToString();
                    objShowerTileSS.ShowerPanQTY = dr["ShowerPanQTY"].ToString();
                    objShowerTileSS.ShowerPanMOU = dr["ShowerPanMOU"].ToString();
                    objShowerTileSS.ShowerPanStyle = dr["ShowerPanStyle"].ToString();
                    objShowerTileSS.ShowerPanColor = dr["ShowerPanColor"].ToString();
                    objShowerTileSS.ShowerPanSize = dr["ShowerPanSize"].ToString();
                    objShowerTileSS.ShowerPanVendor = dr["ShowerPanVendor"].ToString();
                    objShowerTileSS.ShowerPanGroutColor = dr["ShowerPanGroutColor"].ToString();
                    objShowerTileSS.DecobandQTY = dr["DecobandQTY"].ToString();
                    objShowerTileSS.DecobandMOU = dr["DecobandMOU"].ToString();
                    objShowerTileSS.DecobandStyle = dr["DecobandStyle"].ToString();
                    objShowerTileSS.DecobandColor = dr["DecobandColor"].ToString();
                    objShowerTileSS.DecobandSize = dr["DecobandSize"].ToString();
                    objShowerTileSS.DecobandVendor = dr["DecobandVendor"].ToString();
                    objShowerTileSS.DecobandHeight = dr["DecobandHeight"].ToString();
                    objShowerTileSS.NicheTileQTY = dr["NicheTileQTY"].ToString();
                    objShowerTileSS.NicheTileMOU = dr["NicheTileMOU"].ToString();
                    objShowerTileSS.NicheTileStyle = dr["NicheTileStyle"].ToString();
                    objShowerTileSS.NicheTileColor = dr["NicheTileColor"].ToString();
                    objShowerTileSS.NicheTileSize = dr["NicheTileSize"].ToString();
                    objShowerTileSS.NicheTileVendor = dr["NicheTileVendor"].ToString();
                    objShowerTileSS.NicheLocation = dr["NicheLocation"].ToString();
                    objShowerTileSS.NicheSize = dr["NicheSize"].ToString();
                    objShowerTileSS.BenchTileQTY = dr["BenchTileQTY"].ToString();
                    objShowerTileSS.BenchTileMOU = dr["BenchTileMOU"].ToString();
                    objShowerTileSS.BenchTileStyle = dr["BenchTileStyle"].ToString();
                    objShowerTileSS.BenchTileColor = dr["BenchTileColor"].ToString();
                    objShowerTileSS.BenchTileSize = dr["BenchTileSize"].ToString();
                    objShowerTileSS.BenchTileVendor = dr["BenchTileVendor"].ToString();
                    objShowerTileSS.BenchLocation = dr["BenchLocation"].ToString();
                    objShowerTileSS.BenchSize = dr["BenchSize"].ToString();
                    objShowerTileSS.FloorQTY = dr["FloorQTY"].ToString();
                    objShowerTileSS.FloorMOU = dr["FloorMOU"].ToString();
                    objShowerTileSS.FloorStyle = dr["FloorStyle"].ToString();
                    objShowerTileSS.FloorColor = dr["FloorColor"].ToString();
                    objShowerTileSS.FloorSize = dr["FloorSize"].ToString();
                    objShowerTileSS.FloorVendor = dr["FloorVendor"].ToString();
                    objShowerTileSS.FloorPattern = dr["FloorPattern"].ToString();
                    objShowerTileSS.FloorDirection = dr["FloorDirection"].ToString();
                    objShowerTileSS.BaseboardQTY = dr["BaseboardQTY"].ToString();
                    objShowerTileSS.BaseboardMOU = dr["BaseboardMOU"].ToString();
                    objShowerTileSS.BaseboardStyle = dr["BaseboardStyle"].ToString();
                    objShowerTileSS.BaseboardColor = dr["BaseboardColor"].ToString();
                    objShowerTileSS.BaseboardSize = dr["BaseboardSize"].ToString();
                    objShowerTileSS.BaseboardVendor = dr["BaseboardVendor"].ToString();
                    objShowerTileSS.FloorGroutColor = dr["FloorGroutColor"].ToString();
                    objShowerTileSS.TileTo = dr["TileTo"].ToString();
                    objShowerTileSS.LastUpdateDate = DateTime.Now;
                    objShowerTileSS.UpdateBy = User.Identity.Name;


                    if (bFlagNew)
                    {
                        _db.ShowerSheetSelections.InsertOnSubmit(objShowerTileSS);
                    }
                }


                lblShowerTileResult.Text = csCommonUtility.GetSystemMessage("Data has been saved successfully");

                _db.SubmitChanges();
                GetShowerTile();
            }
        }
        catch (Exception ex)
        {
            lblShowerTileResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }
    }

    protected void lnkShowerTileDelete_Click(object sender, EventArgs e)
    {
        try
        {
            DataClassesDataContext _db = new DataClassesDataContext();
            LinkButton lnkShowerTileDelete = (LinkButton)sender;
            int nShowerTileSheetID = Convert.ToInt32(lnkShowerTileDelete.Attributes["CommandArgument"]);
            string strQ = string.Empty;

            strQ = "Delete FROM ShowerSheetSelection  WHERE ShowerKithenID =" + nShowerTileSheetID;
            _db.ExecuteCommand(strQ, string.Empty);
            GetShowerTile();
        }
        catch (Exception ex)
        {
            lblShowerTileResult.Text = csCommonUtility.GetSystemErrorMessage(ex.Message);
        }


    }

    protected void btnAddShowerTileItem_Click(object sender, EventArgs e)
    {
        KPIUtility.SaveEvent(this.Page.AppRelativeVirtualPath, btnAddShowerTileItem.ID, btnAddShowerTileItem.GetType().Name, "Click"); 
        ShowerSheetSelection objKitTileSS = new ShowerSheetSelection();
        DataClassesDataContext _db = new DataClassesDataContext();

        DataTable table = (DataTable)Session["sShowerTileSection"];

        int nShowerID = Convert.ToInt32(hdnCabinetSheetID.Value);
        bool contains = table.AsEnumerable().Any(row => nShowerID == row.Field<int>("ShowerKithenID"));
        if (contains)
        {
            lblShowerTileResult.Text = csCommonUtility.GetSystemRequiredMessage("You have already a pending item to save.");
            return;
        }

        foreach (GridViewRow row in grdShowerTileSelectionSheet.Rows)
        {

            DataRow dr = table.Rows[row.RowIndex];

            Label lblShowerTileSheetName = (Label)row.FindControl("lblShowerTileSheetName");
            TextBox txShowerTileSheetName = (TextBox)row.FindControl("txShowerTileSheetName");

            TextBox txtWallQTY1 = (TextBox)row.FindControl("txtWallQTY1");
            TextBox txtWallMOU1 = (TextBox)row.FindControl("txtWallMOU1");
            TextBox txtWallStyle1 = (TextBox)row.FindControl("txtWallStyle1");
            TextBox txtWallColor1 = (TextBox)row.FindControl("txtWallColor1");
            TextBox txtWallSize1 = (TextBox)row.FindControl("txtWallSize1");
            TextBox txtWallVendor1 = (TextBox)row.FindControl("txtWallVendor1");
            TextBox txtWallPattern1 = (TextBox)row.FindControl("txtWallPattern1");
            TextBox txtWallGroutColor1 = (TextBox)row.FindControl("txtWallGroutColor1");
            TextBox txtSWBullnoseQTY1 = (TextBox)row.FindControl("txtSWBullnoseQTY1");
            TextBox txtSWBullnoseMOU1 = (TextBox)row.FindControl("txtSWBullnoseMOU1");
            TextBox txtSWBullnoseStyle1 = (TextBox)row.FindControl("txtSWBullnoseStyle1");
            TextBox txtSWBullnoseColor1 = (TextBox)row.FindControl("txtSWBullnoseColor1");
            TextBox txtSWBullnoseSize1 = (TextBox)row.FindControl("txtSWBullnoseSize1");
            TextBox txtSWBullnoseVendor1 = (TextBox)row.FindControl("txtSWBullnoseVendor1");
            TextBox txtSchluterNOSticks2 = (TextBox)row.FindControl("txtSchluterNOSticks2");
            TextBox txtSchluterColor2 = (TextBox)row.FindControl("txtSchluterColor2");
            TextBox txtSchluterProfile2 = (TextBox)row.FindControl("txtSchluterProfile2");
            TextBox txtSchluterThicknes2 = (TextBox)row.FindControl("txtSchluterThicknes2");
            TextBox txtShowerPanQTY1 = (TextBox)row.FindControl("txtShowerPanQTY1");
            TextBox txtShowerPanMOU1 = (TextBox)row.FindControl("txtShowerPanMOU1");
            TextBox txtShowerPanStyle1 = (TextBox)row.FindControl("txtShowerPanStyle1");
            TextBox txtShowerPanColor1 = (TextBox)row.FindControl("txtShowerPanColor1");
            TextBox txtShowerPanSize1 = (TextBox)row.FindControl("txtShowerPanSize1");
            TextBox txtShowerPanVendor1 = (TextBox)row.FindControl("txtShowerPanVendor1");
            TextBox txtShowerPanGroutColor1 = (TextBox)row.FindControl("txtShowerPanGroutColor1");
            TextBox txtDecobandQTY1 = (TextBox)row.FindControl("txtDecobandQTY1");
            TextBox txtDecobandMOU1 = (TextBox)row.FindControl("txtDecobandMOU1");
            TextBox txtDecobandStyle1 = (TextBox)row.FindControl("txtDecobandStyle1");
            TextBox txtDecobandColor1 = (TextBox)row.FindControl("txtDecobandColor1");
            TextBox txtDecobandSize1 = (TextBox)row.FindControl("txtDecobandSize1");
            TextBox txtDecobandVendor1 = (TextBox)row.FindControl("txtDecobandVendor1");
            TextBox txtDecobandHeight1 = (TextBox)row.FindControl("txtDecobandHeight1");
            TextBox txtNicheTileQTY1 = (TextBox)row.FindControl("txtNicheTileQTY1");
            TextBox txtNicheTileMOU1 = (TextBox)row.FindControl("txtNicheTileMOU1");
            TextBox txtNicheTileStyle1 = (TextBox)row.FindControl("txtNicheTileStyle1");
            TextBox txtNicheTileColor1 = (TextBox)row.FindControl("txtNicheTileColor1");
            TextBox txtNicheTileSize1 = (TextBox)row.FindControl("txtNicheTileSize1");
            TextBox txtNicheTileVendor1 = (TextBox)row.FindControl("txtNicheTileVendor1");
            TextBox txtNicheLocation1 = (TextBox)row.FindControl("txtNicheLocation1");
            TextBox txtNicheSize1 = (TextBox)row.FindControl("txtNicheSize1");
            TextBox txtBenchTileQTY1 = (TextBox)row.FindControl("txtBenchTileQTY1");
            TextBox txtBenchTileMOU1 = (TextBox)row.FindControl("txtBenchTileMOU1");
            TextBox txtBenchTileStyle1 = (TextBox)row.FindControl("txtBenchTileStyle1");
            TextBox txtBenchTileColor1 = (TextBox)row.FindControl("txtBenchTileColor1");
            TextBox txtBenchTileSize1 = (TextBox)row.FindControl("txtBenchTileSize1");
            TextBox txtBenchTileVendor1 = (TextBox)row.FindControl("txtBenchTileVendor1");
            TextBox txtBenchLocation1 = (TextBox)row.FindControl("txtBenchLocation1");
            TextBox txtBenchSize1 = (TextBox)row.FindControl("txtBenchSize1");
            TextBox txtFloorQTY2 = (TextBox)row.FindControl("txtFloorQTY2");
            TextBox txtFloorMOU2 = (TextBox)row.FindControl("txtFloorMOU2");
            TextBox txtFloorStyle2 = (TextBox)row.FindControl("txtFloorStyle2");
            TextBox txtFloorColor2 = (TextBox)row.FindControl("txtFloorColor2");
            TextBox txtFloorSize2 = (TextBox)row.FindControl("txtFloorSize2");
            TextBox txtFloorVendor2 = (TextBox)row.FindControl("txtFloorVendor2");
            TextBox txtFloorPattern2 = (TextBox)row.FindControl("txtFloorPattern2");
            TextBox txtFloorDirection2 = (TextBox)row.FindControl("txtFloorDirection2");
            TextBox txtBaseboardQTY2 = (TextBox)row.FindControl("txtBaseboardQTY2");
            TextBox txtBaseboardMOU2 = (TextBox)row.FindControl("txtBaseboardMOU2");
            TextBox txtBaseboardStyle2 = (TextBox)row.FindControl("txtBaseboardStyle2");
            TextBox txtBaseboardColor2 = (TextBox)row.FindControl("txtBaseboardColor2");
            TextBox txtBaseboardSize2 = (TextBox)row.FindControl("txtBaseboardSize2");
            TextBox txtBaseboardVendor2 = (TextBox)row.FindControl("txtBaseboardVendor2");
            TextBox txtFloorGroutColor2 = (TextBox)row.FindControl("txtFloorGroutColor2");
            TextBox txtTileto2 = (TextBox)row.FindControl("txtTileto2");


            dr["ShowerKithenID"] = Convert.ToInt32(grdShowerTileSelectionSheet.DataKeys[row.RowIndex].Values[0]);
            dr["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
            dr["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
            dr["ShowerTileSheetName"] = txShowerTileSheetName.Text.Trim();
            dr["WallTileQTY"] = txtWallQTY1.Text;
            dr["WallTileMOU"] = txtWallMOU1.Text;
            dr["WallTileStyle"] = txtWallStyle1.Text;
            dr["WallTileColor"] = txtWallColor1.Text;
            dr["WallTileSize"] = txtWallSize1.Text;
            dr["WallTileVendor"] = txtWallVendor1.Text;
            dr["WallTilePattern"] = txtWallPattern1.Text;
            dr["WallTileGroutColor"] = txtWallGroutColor1.Text;
            dr["WBullnoseQTY"] = txtSWBullnoseQTY1.Text;
            dr["WBullnoseMOU"] = txtSWBullnoseMOU1.Text;
            dr["WBullnoseStyle"] = txtSWBullnoseStyle1.Text;
            dr["WBullnoseColor"] = txtSWBullnoseColor1.Text;
            dr["WBullnoseSize"] = txtSWBullnoseSize1.Text;
            dr["WBullnoseVendor"] = txtSWBullnoseVendor1.Text;
            dr["SchluterNOSticks"] = txtSchluterNOSticks2.Text;
            dr["SchluterColor"] = txtSchluterColor2.Text;
            dr["SchluterProfile"] = txtSchluterProfile2.Text;
            dr["SchluterThickness"] = txtSchluterThicknes2.Text;
            dr["ShowerPanQTY"] = txtShowerPanQTY1.Text;
            dr["ShowerPanMOU"] = txtShowerPanMOU1.Text;
            dr["ShowerPanStyle"] = txtShowerPanStyle1.Text;
            dr["ShowerPanColor"] = txtShowerPanColor1.Text;
            dr["ShowerPanSize"] = txtShowerPanSize1.Text;
            dr["ShowerPanVendor"] = txtShowerPanVendor1.Text;
            dr["ShowerPanGroutColor"] = txtShowerPanGroutColor1.Text;
            dr["DecobandQTY"] = txtDecobandQTY1.Text;
            dr["DecobandMOU"] = txtDecobandMOU1.Text;
            dr["DecobandStyle"] = txtDecobandStyle1.Text;
            dr["DecobandColor"] = txtDecobandColor1.Text;
            dr["DecobandSize"] = txtDecobandSize1.Text;
            dr["DecobandVendor"] = txtDecobandVendor1.Text;
            dr["DecobandHeight"] = txtDecobandHeight1.Text;
            dr["NicheTileQTY"] = txtNicheTileQTY1.Text;
            dr["NicheTileMOU"] = txtNicheTileMOU1.Text;
            dr["NicheTileStyle"] = txtNicheTileStyle1.Text;
            dr["NicheTileColor"] = txtNicheTileColor1.Text;
            dr["NicheTileSize"] = txtNicheTileSize1.Text;
            dr["NicheTileVendor"] = txtNicheTileVendor1.Text;
            dr["NicheLocation"] = txtNicheLocation1.Text;
            dr["NicheSize"] = txtNicheSize1.Text;
            dr["BenchTileQTY"] = txtBenchTileQTY1.Text;
            dr["BenchTileMOU"] = txtBenchTileMOU1.Text;
            dr["BenchTileStyle"] = txtBenchTileStyle1.Text;
            dr["BenchTileColor"] = txtBenchTileColor1.Text;
            dr["BenchTileSize"] = txtBenchTileSize1.Text;
            dr["BenchTileVendor"] = txtBenchTileVendor1.Text;
            dr["BenchLocation"] = txtBenchLocation1.Text;
            dr["BenchSize"] = txtBenchSize1.Text;
            dr["FloorQTY"] = txtFloorQTY2.Text;
            dr["FloorMOU"] = txtFloorMOU2.Text;
            dr["FloorStyle"] = txtFloorStyle2.Text;
            dr["FloorColor"] = txtFloorColor2.Text;
            dr["FloorSize"] = txtFloorSize2.Text;
            dr["FloorVendor"] = txtFloorVendor2.Text;
            dr["FloorPattern"] = txtFloorPattern2.Text;
            dr["FloorDirection"] = txtFloorDirection2.Text;
            dr["BaseboardQTY"] = txtBaseboardQTY2.Text;
            dr["BaseboardMOU"] = txtBaseboardMOU2.Text;
            dr["BaseboardStyle"] = txtBaseboardStyle2.Text;
            dr["BaseboardColor"] = txtBaseboardColor2.Text;
            dr["BaseboardSize"] = txtBaseboardSize2.Text;
            dr["BaseboardVendor"] = txtBaseboardVendor2.Text;
            dr["FloorGroutColor"] = txtFloorGroutColor2.Text;
            dr["TileTo"] = txtTileto2.Text;
            dr["LastUpdateDate"] = DateTime.Now;
            dr["UpdateBy"] = User.Identity.Name;
        }

        DataRow drNew = table.NewRow();

        drNew["ShowerKithenID"] = Convert.ToInt32(hdnShowerTileSheetID.Value);
        drNew["customer_id"] = Convert.ToInt32(hdnCustomerId.Value);
        drNew["estimate_id"] = Convert.ToInt32(hdnEstimateId.Value);
        drNew["ShowerTileSheetName"] = "";
        drNew["WallTileQTY"] = "";
        drNew["WallTileMOU"] = "";
        drNew["WallTileStyle"] = "";
        drNew["WallTileColor"] = "";
        drNew["WallTileSize"] = "";
        drNew["WallTileVendor"] = "";
        drNew["WallTilePattern"] = "";
        drNew["WallTileGroutColor"] = "";
        drNew["WBullnoseQTY"] = "";
        drNew["WBullnoseMOU"] = "";
        drNew["WBullnoseStyle"] = "";
        drNew["WBullnoseColor"] = "";
        drNew["WBullnoseSize"] = "";
        drNew["WBullnoseVendor"] = "";
        drNew["SchluterNOSticks"] = "";
        drNew["SchluterColor"] = "";
        drNew["SchluterProfile"] = "";
        drNew["SchluterThickness"] = "";
        drNew["ShowerPanQTY"] = "";
        drNew["ShowerPanMOU"] = "";
        drNew["ShowerPanStyle"] = "";
        drNew["ShowerPanColor"] = "";
        drNew["ShowerPanSize"] = "";
        drNew["ShowerPanVendor"] = "";
        drNew["ShowerPanGroutColor"] = "";
        drNew["DecobandQTY"] = "";
        drNew["DecobandMOU"] = "";
        drNew["DecobandStyle"] = "";
        drNew["DecobandColor"] = "";
        drNew["DecobandSize"] = "";
        drNew["DecobandVendor"] = "";
        drNew["DecobandHeight"] = "";
        drNew["NicheTileQTY"] = "";
        drNew["NicheTileMOU"] = "";
        drNew["NicheTileStyle"] = "";
        drNew["NicheTileColor"] = "";
        drNew["NicheTileSize"] = "";
        drNew["NicheTileVendor"] = "";
        drNew["NicheLocation"] = "";
        drNew["NicheSize"] = "";
        drNew["BenchTileQTY"] = "";
        drNew["BenchTileMOU"] = "";
        drNew["BenchTileStyle"] = "";
        drNew["BenchTileColor"] = "";
        drNew["BenchTileSize"] = "";
        drNew["BenchTileVendor"] = "";
        drNew["BenchLocation"] = "";
        drNew["BenchSize"] = "";
        drNew["FloorQTY"] = "";
        drNew["FloorMOU"] = "";
        drNew["FloorStyle"] = "";
        drNew["FloorColor"] = "";
        drNew["FloorSize"] = "";
        drNew["FloorVendor"] = "";
        drNew["FloorPattern"] = "";
        drNew["FloorDirection"] = "";
        drNew["BaseboardQTY"] = "";
        drNew["BaseboardMOU"] = "";
        drNew["BaseboardStyle"] = "";
        drNew["BaseboardColor"] = "";
        drNew["BaseboardSize"] = "";
        drNew["BaseboardVendor"] = "";
        drNew["FloorGroutColor"] = "";
        drNew["TileTo"] = "";
        drNew["LastUpdateDate"] = DateTime.Now;
        drNew["UpdateBy"] = User.Identity.Name;

        table.Rows.InsertAt(drNew, 0);

        Session.Add("sShowerTileSection", table);
        grdShowerTileSelectionSheet.DataSource = table;
        grdShowerTileSelectionSheet.DataKeyNames = new string[] { "ShowerKithenID", "customer_id", "estimate_id" };
        grdShowerTileSelectionSheet.DataBind();
        lblShowerTileResult.Text = "";

    }

    private DataTable LoadShowerTileTable()
    {
        DataTable table = new DataTable();

        table.Columns.Add("ShowerKithenID", typeof(int));
        table.Columns.Add("customer_id", typeof(int));
        table.Columns.Add("estimate_id", typeof(int));
        table.Columns.Add("ShowerTileSheetName", typeof(string));
        table.Columns.Add("WallTileQTY", typeof(string));
        table.Columns.Add("WallTileMOU", typeof(string));
        table.Columns.Add("WallTileStyle", typeof(string));
        table.Columns.Add("WallTileColor", typeof(string));
        table.Columns.Add("WallTileSize", typeof(string));
        table.Columns.Add("WallTileVendor", typeof(string));
        table.Columns.Add("WallTilePattern", typeof(string));
        table.Columns.Add("WallTileGroutColor", typeof(string));
        table.Columns.Add("WBullnoseQTY", typeof(string));
        table.Columns.Add("WBullnoseMOU", typeof(string));
        table.Columns.Add("WBullnoseStyle", typeof(string));
        table.Columns.Add("WBullnoseColor", typeof(string));
        table.Columns.Add("WBullnoseSize", typeof(string));
        table.Columns.Add("WBullnoseVendor", typeof(string));
        table.Columns.Add("SchluterNOSticks", typeof(string));
        table.Columns.Add("SchluterColor", typeof(string));
        table.Columns.Add("SchluterProfile", typeof(string));
        table.Columns.Add("SchluterThickness", typeof(string));
        table.Columns.Add("ShowerPanQTY", typeof(string));
        table.Columns.Add("ShowerPanMOU", typeof(string));
        table.Columns.Add("ShowerPanStyle", typeof(string));
        table.Columns.Add("ShowerPanColor", typeof(string));
        table.Columns.Add("ShowerPanSize", typeof(string));
        table.Columns.Add("ShowerPanVendor", typeof(string));
        table.Columns.Add("ShowerPanPattern", typeof(string));
        table.Columns.Add("ShowerPanGroutColor", typeof(string));
        table.Columns.Add("DecobandQTY", typeof(string));
        table.Columns.Add("DecobandMOU", typeof(string));
        table.Columns.Add("DecobandStyle", typeof(string));
        table.Columns.Add("DecobandColor", typeof(string));
        table.Columns.Add("DecobandSize", typeof(string));
        table.Columns.Add("DecobandVendor", typeof(string));
        table.Columns.Add("DecobandHeight", typeof(string));
        table.Columns.Add("NicheTileQTY", typeof(string));
        table.Columns.Add("NicheTileMOU", typeof(string));
        table.Columns.Add("NicheTileStyle", typeof(string));
        table.Columns.Add("NicheTileColor", typeof(string));
        table.Columns.Add("NicheTileSize", typeof(string));
        table.Columns.Add("NicheTileVendor", typeof(string));
        table.Columns.Add("NicheLocation", typeof(string));
        table.Columns.Add("NicheSize", typeof(string));
        table.Columns.Add("BenchTileQTY", typeof(string));
        table.Columns.Add("BenchTileMOU", typeof(string));
        table.Columns.Add("BenchTileStyle", typeof(string));
        table.Columns.Add("BenchTileColor", typeof(string));
        table.Columns.Add("BenchTileSize", typeof(string));
        table.Columns.Add("BenchTileVendor", typeof(string));
        table.Columns.Add("BenchLocation", typeof(string));
        table.Columns.Add("BenchSize", typeof(string));
        table.Columns.Add("FloorQTY", typeof(string));
        table.Columns.Add("FloorMOU", typeof(string));
        table.Columns.Add("FloorStyle", typeof(string));
        table.Columns.Add("FloorColor", typeof(string));
        table.Columns.Add("FloorSize", typeof(string));
        table.Columns.Add("FloorVendor", typeof(string));
        table.Columns.Add("FloorPattern", typeof(string));
        table.Columns.Add("FloorDirection", typeof(string));
        table.Columns.Add("BaseboardQTY", typeof(string));
        table.Columns.Add("BaseboardMOU", typeof(string));
        table.Columns.Add("BaseboardStyle", typeof(string));
        table.Columns.Add("BaseboardColor", typeof(string));
        table.Columns.Add("BaseboardSize", typeof(string));
        table.Columns.Add("BaseboardVendor", typeof(string));
        table.Columns.Add("FloorGroutColor", typeof(string));
        table.Columns.Add("TileTo", typeof(string));
        table.Columns.Add("UpdateBy", typeof(string));
        table.Columns.Add("LastUpdateDate", typeof(DateTime));

        return table;
    }

    #endregion


    #region KITCHEN/TUB/SHOWER


    //private void loadShowerSheetSelection()
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();

    //    if (_db.ShowerSheetSelections.Where(cp => cp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cp.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).SingleOrDefault() != null)
    //    {
    //        ShowerSheetSelection sss = _db.ShowerSheetSelections.SingleOrDefault(cp => cp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cp.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
    //        txtWallQTY1.Text = sss.WallTileQTY;
    //        txtWallMOU1.Text = sss.WallTileMOU;
    //        txtWallStyle1.Text = sss.WallTileStyle;
    //        txtWallColor1.Text = sss.WallTileColor;
    //        txtWallSize1.Text = sss.WallTileSize;
    //        txtWallVendor1.Text = sss.WallTileVendor;
    //        txtWallPattern1.Text = sss.WallTilePattern;
    //        txtWallGroutColor1.Text = sss.WallTileGroutColor;
    //        txtSWBullnoseQTY1.Text = sss.WBullnoseQTY;
    //        txtSWBullnoseMOU1.Text = sss.WBullnoseMOU;
    //        txtSWBullnoseStyle1.Text = sss.WBullnoseStyle;
    //        txtSWBullnoseColor1.Text = sss.WBullnoseColor;
    //        txtSWBullnoseSize1.Text = sss.WBullnoseSize;
    //        txtSWBullnoseVendor1.Text = sss.WBullnoseVendor;
    //        txtSchluterNOSticks2.Text = sss.SchluterNOSticks;
    //        txtSchluterColor2.Text = sss.SchluterColor;
    //        txtSchluterProfile2.Text = sss.SchluterProfile;
    //        txtSchluterThicknes2.Text = sss.SchluterThickness;
    //        txtShowerPanQTY1.Text = sss.ShowerPanQTY;
    //        txtShowerPanMOU1.Text = sss.ShowerPanMOU;
    //        txtShowerPanStyle1.Text = sss.ShowerPanStyle;
    //        txtShowerPanColor1.Text = sss.ShowerPanColor;
    //        txtShowerPanSize1.Text = sss.ShowerPanSize;
    //        txtShowerPanVendor1.Text = sss.ShowerPanVendor;
    //        //sss.ShowerPanPattern = "";
    //        txtShowerPanGroutColor1.Text = sss.ShowerPanGroutColor;
    //        txtDecobandQTY1.Text = sss.DecobandQTY;
    //        txtDecobandMOU1.Text = sss.DecobandMOU;
    //        txtDecobandStyle1.Text = sss.DecobandStyle;
    //        txtDecobandColor1.Text = sss.DecobandColor;
    //        txtDecobandSize1.Text = sss.DecobandSize;
    //        txtDecobandVendor1.Text = sss.DecobandVendor;
    //        txtDecobandHeight1.Text = sss.DecobandHeight;
    //        txtNicheTileQTY1.Text = sss.NicheTileQTY;
    //        txtNicheTileMOU1.Text = sss.NicheTileMOU;
    //        txtNicheTileStyle1.Text = sss.NicheTileStyle;
    //        txtNicheTileColor1.Text = sss.NicheTileColor;
    //        txtNicheTileSize1.Text = sss.NicheTileSize;
    //        txtNicheTileVendor1.Text = sss.NicheTileVendor;
    //        txtNicheLocation1.Text = sss.NicheLocation;
    //        txtNicheSize1.Text = sss.NicheSize;
    //        txtBenchTileQTY1.Text = sss.BenchTileQTY;
    //        txtBenchTileMOU1.Text = sss.BenchTileMOU;
    //        txtBenchTileStyle1.Text = sss.BenchTileStyle;
    //        txtBenchTileColor1.Text = sss.BenchTileColor;
    //        txtBenchTileSize1.Text = sss.BenchTileSize;
    //        txtBenchTileVendor1.Text = sss.BenchTileVendor;
    //        txtBenchLocation1.Text = sss.BenchLocation;
    //        txtBenchSize1.Text = sss.BenchSize;
    //        txtFloorQTY2.Text = sss.FloorQTY;
    //        txtFloorMOU2.Text = sss.FloorMOU;
    //        txtFloorStyle2.Text = sss.FloorStyle;
    //        txtFloorColor2.Text = sss.FloorColor;
    //        txtFloorSize2.Text = sss.FloorSize;
    //        txtFloorVendor2.Text = sss.FloorVendor;
    //        txtFloorPattern2.Text = sss.FloorPattern;
    //        txtFloorDirection2.Text = sss.FloorDirection;
    //        txtBaseboardQTY2.Text = sss.BaseboardQTY;
    //        txtBaseboardMOU2.Text = sss.BaseboardMOU;
    //        txtBaseboardStyle2.Text = sss.BaseboardStyle;
    //        txtBaseboardColor2.Text = sss.BaseboardColor;
    //        txtBaseboardSize2.Text = sss.BaseboardSize;
    //        txtBaseboardVendor2.Text = sss.BaseboardVendor;
    //        txtFloorGroutColor2.Text = sss.FloorGroutColor;
    //        txtTileto2.Text = sss.TileTo;
    //    }
    //}

    //protected void btnSaveShowerSheet_Click(object sender, EventArgs e)
    //{
    //    DataClassesDataContext _db = new DataClassesDataContext();

    //    ShowerSheetSelection sss = new ShowerSheetSelection();
    //    //string strQ = "DELETE ShowerSheetSelection WHERE customer_id=" + Convert.ToInt32(hdnCustomerId.Value) + " AND estimate_id=" + Convert.ToInt32(hdnEstimateId.Value);
    //    //_db.ExecuteCommand(strQ, string.Empty);
    //    //_db.SubmitChanges();
    //    if (_db.ShowerSheetSelections.Where(cp => cp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cp.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).SingleOrDefault() != null)
    //    {
    //        sss = _db.ShowerSheetSelections.SingleOrDefault(cp => cp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cp.estimate_id == Convert.ToInt32(hdnEstimateId.Value));
    //    }

    //    sss.customer_id = Convert.ToInt32(hdnCustomerId.Value);
    //    sss.estimate_id = Convert.ToInt32(hdnEstimateId.Value);
    //    sss.WallTileQTY = txtWallQTY1.Text;
    //    sss.WallTileMOU = txtWallMOU1.Text;
    //    sss.WallTileStyle = txtWallStyle1.Text;
    //    sss.WallTileColor = txtWallColor1.Text;
    //    sss.WallTileSize = txtWallSize1.Text;
    //    sss.WallTileVendor = txtWallVendor1.Text;
    //    sss.WallTilePattern = txtWallPattern1.Text;
    //    sss.WallTileGroutColor = txtWallGroutColor1.Text;
    //    sss.WBullnoseQTY = txtSWBullnoseQTY1.Text;
    //    sss.WBullnoseMOU = txtSWBullnoseMOU1.Text;
    //    sss.WBullnoseStyle = txtSWBullnoseStyle1.Text;
    //    sss.WBullnoseColor = txtSWBullnoseColor1.Text;
    //    sss.WBullnoseSize = txtSWBullnoseSize1.Text;
    //    sss.WBullnoseVendor = txtSWBullnoseVendor1.Text;
    //    sss.SchluterNOSticks = txtSchluterNOSticks2.Text;
    //    sss.SchluterColor = txtSchluterColor2.Text;
    //    sss.SchluterProfile = txtSchluterProfile2.Text;
    //    sss.SchluterThickness = txtSchluterThicknes2.Text;
    //    sss.ShowerPanQTY = txtShowerPanQTY1.Text;
    //    sss.ShowerPanMOU = txtShowerPanMOU1.Text;
    //    sss.ShowerPanStyle = txtShowerPanStyle1.Text;
    //    sss.ShowerPanColor = txtShowerPanColor1.Text;
    //    sss.ShowerPanSize = txtShowerPanSize1.Text;
    //    sss.ShowerPanVendor = txtShowerPanVendor1.Text;
    //    sss.ShowerPanPattern = "";
    //    sss.ShowerPanGroutColor = txtShowerPanGroutColor1.Text;
    //    sss.DecobandQTY = txtDecobandQTY1.Text;
    //    sss.DecobandMOU = txtDecobandMOU1.Text;
    //    sss.DecobandStyle = txtDecobandStyle1.Text;
    //    sss.DecobandColor = txtDecobandColor1.Text;
    //    sss.DecobandSize = txtDecobandSize1.Text;
    //    sss.DecobandVendor = txtDecobandVendor1.Text;
    //    sss.DecobandHeight = txtDecobandHeight1.Text;
    //    sss.NicheTileQTY = txtNicheTileQTY1.Text;
    //    sss.NicheTileMOU = txtNicheTileMOU1.Text;
    //    sss.NicheTileStyle = txtNicheTileStyle1.Text;
    //    sss.NicheTileColor = txtNicheTileColor1.Text;
    //    sss.NicheTileSize = txtNicheTileSize1.Text;
    //    sss.NicheTileVendor = txtNicheTileVendor1.Text;
    //    sss.NicheLocation = txtNicheLocation1.Text;
    //    sss.NicheSize = txtNicheSize1.Text;
    //    sss.BenchTileQTY = txtBenchTileQTY1.Text;
    //    sss.BenchTileMOU = txtBenchTileMOU1.Text;
    //    sss.BenchTileStyle = txtBenchTileStyle1.Text;
    //    sss.BenchTileColor = txtBenchTileColor1.Text;
    //    sss.BenchTileSize = txtBenchTileSize1.Text;
    //    sss.BenchTileVendor = txtBenchTileVendor1.Text;
    //    sss.BenchLocation = txtBenchLocation1.Text;
    //    sss.BenchSize = txtBenchSize1.Text;
    //    sss.FloorQTY = txtFloorQTY2.Text;
    //    sss.FloorMOU = txtFloorMOU2.Text;
    //    sss.FloorStyle = txtFloorStyle2.Text;
    //    sss.FloorColor = txtFloorColor2.Text;
    //    sss.FloorSize = txtFloorSize2.Text;
    //    sss.FloorVendor = txtFloorVendor2.Text;
    //    sss.FloorPattern = txtFloorPattern2.Text;
    //    sss.FloorDirection = txtFloorDirection2.Text;
    //    sss.BaseboardQTY = txtBaseboardQTY2.Text;
    //    sss.BaseboardMOU = txtBaseboardMOU2.Text;
    //    sss.BaseboardStyle = txtBaseboardStyle2.Text;
    //    sss.BaseboardColor = txtBaseboardColor2.Text;
    //    sss.BaseboardSize = txtBaseboardSize2.Text;
    //    sss.BaseboardVendor = txtBaseboardVendor2.Text;
    //    sss.FloorGroutColor = txtFloorGroutColor2.Text;
    //    sss.TileTo = txtTileto2.Text;
    //    sss.UpdateBy = User.Identity.Name;
    //    sss.LastUpdateDate = DateTime.Now;
    //    if (_db.ShowerSheetSelections.Where(cp => cp.customer_id == Convert.ToInt32(hdnCustomerId.Value) && cp.estimate_id == Convert.ToInt32(hdnEstimateId.Value)).SingleOrDefault() == null)
    //    {
    //        _db.ShowerSheetSelections.InsertOnSubmit(sss);
    //    }
    //    _db.SubmitChanges();
    //    lblShower.Text = csCommonUtility.GetSystemMessage("Data saved successfully.");


    //}

    #endregion




    protected void customerMenuTab_MenuItemClick(object sender, MenuEventArgs e)
    {
        multiViewMenuTab.ActiveViewIndex = Convert.ToInt32(e.Item.Value);
    }

}