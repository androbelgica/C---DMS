using System;
using System.Web;

namespace DMS
{
    public partial class load : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string targetUrl = Request.QueryString["url"];
            if (!string.IsNullOrEmpty(targetUrl))
            {
                // Delay the redirection by 1 second (1000 milliseconds)
                Response.AddHeader("REFRESH", "1;URL=" + targetUrl);
            }
        }
    }
}
