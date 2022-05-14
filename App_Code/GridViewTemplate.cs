using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

//A customized class for displaying the Template Column
public class GridViewTemplate : ITemplate
{
    //A variable to hold the type of ListItemType.
    ListItemType _templateType;

    //A variable to hold the column name.
    string _columnName;

    //Constructor where we define the template type and column name.
    public GridViewTemplate(ListItemType type, string colname)
    {
        //Stores the template type.
        _templateType = type;

        //Stores the column name.
        _columnName = colname;
    }

    void ITemplate.InstantiateIn(System.Web.UI.Control container)
    {
        switch (_templateType)
        {
            case ListItemType.Header:
                //Creates a new label control and add it to the container.
                Label lbl = new Label();            //Allocates the new label object.
                lbl.Text = _columnName;             //Assigns the name of the column in the lable.
                container.Controls.Add(lbl);        //Adds the newly created label control to the container.
                break;

            case ListItemType.Item:
                if (_columnName == "SalesPerson" || _columnName == "Customer")
                {
                    //Creates a new label control and add it to the container.
                    Label lbltext = new Label();            //Allocates the new label object.
                    lbltext.ID = _columnName;             //Assigns the ID of the column in the lable.
                    lbltext.Text = "";
                    container.Controls.Add(lbltext);        //Adds the newly created label control to the container.
                    break;
                }
                else
                {
                    //Creates a new text box control and add it to the container.
                    TextBox tb1 = new TextBox();                            //Allocates the new text box object.
                    tb1.DataBinding += new EventHandler(tb1_DataBinding);   //Attaches the data binding event.
                    tb1.Columns = 10;                                        //Creates a column with size 4.                    
                    container.Controls.Add(tb1);                            //Adds the newly created textbox to the container.

                    //Button bt1 = new Button();
                    //bt1.DataBinding += new EventHandler(bt1_DataBinding);
                    //container.Controls.Add(bt1);
                    break;                   
                }
                

            case ListItemType.EditItem:
                //As, I am not using any EditItem, I didnot added any code here.
                break;

            case ListItemType.Footer:
                CheckBox chkColumn = new CheckBox();
                chkColumn.ID = "Chk" + _columnName;
                container.Controls.Add(chkColumn);
                break;
        }
    }

    /// <summary>
    /// This is the event, which will be raised when the binding happens.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void tb1_DataBinding(object sender, EventArgs e)
    {
        TextBox txtdata = (TextBox)sender;
        GridViewRow container = (GridViewRow)txtdata.NamingContainer;
        object dataValue = DataBinder.Eval(container.DataItem, _columnName);
        if (dataValue != DBNull.Value)
        {
            txtdata.Text = dataValue.ToString();
            //txtdata.ID = dataValue.ToString();
            //txtdata.Attributes.Add("runat", "server");
        }
    }

    void bt1_DataBinding(object sender, EventArgs e)
    {
        Button btndata = (Button)sender;
        GridViewRow container = (GridViewRow)btndata.NamingContainer;
        object dataValue = DataBinder.Eval(container.DataItem, _columnName);
        if (dataValue != DBNull.Value)
        {
            btndata.Text = "Edit";// dataValue.ToString();
            //txtdata.ID = dataValue.ToString();
            //txtdata.Attributes.Add("runat", "server");
        }
    }
}