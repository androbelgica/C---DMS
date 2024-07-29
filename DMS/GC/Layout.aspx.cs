using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using OfficeOpenXml;
using System.Data.SqlClient;

namespace DMS.GC
{
    public partial class Layout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            string filePath = Server.MapPath("~/App_Data/layoutpub.pub");
            string fileName = "layoutpub.pub";

            if (File.Exists(filePath))
            {
                Response.ContentType = "application/vnd.ms-publisher";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.TransmitFile(filePath);
                Response.End();
            }
            else
            {
                // Handle the case where the file doesn't exist
                Response.Write("File not found.");
            }
        }
    }
}