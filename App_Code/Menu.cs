using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Menu
/// </summary>
public class Menu
{
	public Menu()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public int menu_id { get; set; }
    public string menu_code { get; set; }
    public string menu_name { get; set; }
    public int parent_id { get; set; }
    public int? client_id { get; set; }
    public string menu_url { get; set; }
    public int isShow { get; set; }
    public int serial { get; set; }
}