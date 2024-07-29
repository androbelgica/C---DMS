using System;
using System.Web.UI;

namespace DMS.PAGES
{
    public partial class AccessDenied : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // No need to store the previous URL
        }

        protected void GoBackButton_Click(object sender, EventArgs e)
        {
            // Redirect to the dashboard page
            Response.Redirect("/load.aspx?url=../PAGES/Dashboard.aspx");
        }
    }
}
