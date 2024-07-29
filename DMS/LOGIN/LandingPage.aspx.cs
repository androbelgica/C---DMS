using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DMS.LOGIN
{
    public partial class LandingPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Name"] == null)
            {
                // Redirect to login page if session variable is not set
                Response.Redirect("../LOGIN/Login.aspx");
            }
        }

        protected void DMSBtn_Click(object sender, EventArgs e)
        {
            // Redirect to the desired page
            Response.Redirect("/load.aspx?url=../PAGES/Dashboard.aspx");
        }
        protected void GCBtn_Click(object sender, EventArgs e)
        {
            // Redirect to the desired page
            Response.Redirect("/load.aspx?url=../GC/Availment.aspx");
        }
        protected void LogOutBtn_Click(object sender, EventArgs e)
        {
            // Clear the session
            Session.Clear();
            Session.Abandon();
            // Redirect to login page
            Response.Redirect("../LOGIN/Login.aspx");
        }

    }
}