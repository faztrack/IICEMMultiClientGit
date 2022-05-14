using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for LocationModel
/// </summary>
public class LocationModel
{
    public LocationModel() { }

    public int client_id { get; set; }
    public int location_id { get; set; }
    public string location_name { get; set; }
    public string loation_desc { get; set; }
    public int estimate_id { get; set; }
    public bool is_active { get; set; }
}
