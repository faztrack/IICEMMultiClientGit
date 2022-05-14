using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for test
/// </summary>
public class test
{
	public test()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static void SaveText(string strQ)
    {
        System.IO.File.WriteAllText(@"C:\Users\arefin\Desktop\SaveText.txt", strQ);
    }
}