using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Controls;

namespace DMS.PAGES
{
    public partial class UserActivityLogs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Name"] == null)
            {
                // Redirect to login page if session variable is not set
                Response.Redirect("../LOGIN/Login.aspx");
            }

            // Add the ListItem conditionally based on the user's department
            string userDepartment = Session["UserDepartment"]?.ToString();
            if (userDepartment == "MIS")
            {
                // Add the ListItem
                ddlUserAct.Items.Add(new ListItem("API Key and Email", "Updated API Key and Email"));

            }
            // Check if the user has the required permissions to view user logs
            List<string> userPermissions = Session["UserPermissions"] as List<string>;
            if (userPermissions == null ||
                (!userPermissions.Contains("View all user logs from all departments") &&
                 !userPermissions.Contains("View all user logs within their department")))
            {
                // Redirect to Access Denied page if the user does not have the required permissions
                Response.Redirect("../PAGES/AccessDenied.aspx");
            }

            if (!IsPostBack)
            {
                // Bind the GridView only when the page is loaded for the first time
                BindGridView();
            }
            else
            {
                // Handle date filter
                string selectedDate = hfSelectedDate.Value;
                DateTime filterDate;
                if (DateTime.TryParse(selectedDate, out filterDate))
                {
                    BindGridView(filterDate);
                }
                else
                {
                    BindGridView();
                }
            }
            BindPagination();
            UpdatePageIndexTextBox();
        }
        private void BindGridView()
        {
            List<string> userPermissions = Session["UserPermissions"] as List<string>;
            string query = "SELECT * FROM auditlogs";

            // Check if user is not from 'MIS' department and remove 'Updated API Key and Email' activity
            string userDepartment = Session["Department"]?.ToString();
            bool excludeApiKeyEmailUpdate = userDepartment != "MIS";

            if (excludeApiKeyEmailUpdate)
            {
                query += " WHERE Activity != 'Updated API Key and Email'";
            }

            if (userPermissions.Contains("View all user logs within their department") &&
                !userPermissions.Contains("View all user logs from all departments"))
            {
                // Show logs only within the user's department
                query += " WHERE UserID IN (SELECT UserID FROM Accounts WHERE Department = @Department)";
            }

            query += " ORDER BY UserLogDateTime DESC";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (userPermissions.Contains("View all user logs within their department"))
                    {
                        cmd.Parameters.AddWithValue("@Department", Session["Department"].ToString());
                    }

                    try
                    {
                        conn.Open();
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            GridView1.DataSource = dt;
                            GridView1.DataBind();

                            // Calculate total number of pages and set lblTotalPages only during initial binding
                            if (!IsPostBack)
                            {
                                int totalRows = dt.Rows.Count;
                                int pageSize = GridView1.PageSize;
                                int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

                                lblTotalPages.Text = totalPages.ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle the error appropriately
                        Response.Write("Error fetching data: " + ex.Message);
                        // Optionally, log this error or display it on the UI
                    }
                }
            }
        }

        // FILTER DATE FUNCTION BELOW
        private void BindGridView(DateTime? filterDate = null)
        {
            List<string> userPermissions = Session["UserPermissions"] as List<string>;
            string query = "SELECT * FROM AuditLogs";

            // Check if user is not from 'MIS' department and exclude 'Updated API Key and Email' activity
            string userDepartment = Session["Department"]?.ToString();
            bool excludeApiKeyEmailUpdate = userDepartment != "MIS";

            // Modify query based on exclusion condition
            if (excludeApiKeyEmailUpdate)
            {
                query += " WHERE Activity != 'Updated API Key and Email'";
            }

            // Add additional conditions based on permissions and filterDate
            if (userPermissions.Contains("View all user logs within their department") &&
                !userPermissions.Contains("View all user logs from all departments"))
            {
                string department = Session["Department"].ToString();
                if (excludeApiKeyEmailUpdate)
                {
                    query += " AND UserID IN (SELECT UserID FROM Accounts WHERE Department = @Department)";
                }
                else
                {
                    query += " WHERE UserID IN (SELECT UserID FROM Accounts WHERE Department = @Department)";
                }

                if (filterDate.HasValue)
                {
                    query += " AND DATE(UserLogDateTime) = @FilterDate";
                }
            }
            else if (filterDate.HasValue)
            {
                if (excludeApiKeyEmailUpdate)
                {
                    query += " AND DATE(UserLogDateTime) = @FilterDate";
                }
                else
                {
                    query += " WHERE DATE(UserLogDateTime) = @FilterDate";
                }
            }

            query += " ORDER BY UserLogDateTime DESC";

            // Execute the query and bind data to GridView
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (userPermissions.Contains("View all user logs within their department"))
                    {
                        cmd.Parameters.AddWithValue("@Department", Session["Department"].ToString());
                    }
                    if (filterDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@FilterDate", filterDate.Value.Date);
                    }

                    try
                    {
                        conn.Open();
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            GridView1.DataSource = dt;
                            GridView1.DataBind();

                            // Calculate total number of pages and set lblTotalPages only during initial binding or when filtering by date
                            int totalRows = dt.Rows.Count;
                            int pageSize = GridView1.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

                            lblTotalPages.Text = totalPages.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle the error appropriately
                        Response.Write("Error fetching data: " + ex.Message);
                        // Optionally, log this error or display it on the UI
                    }
                }
            }
        }


        //----- SEARCH FUNCTION BELOW
        protected void searchtxtbox_TextChanged(object sender, EventArgs e)
        {
            string searchText = searchtxtbox.Text.Trim();
            ViewState["SearchText"] = searchText;

            if (!string.IsNullOrEmpty(searchText))
            {
                SearchData(searchText);
            }
            else
            {
                // Check if any filters are applied
                string selectedActivity = ViewState["SelectedActivity"]?.ToString();
                string selectedStatus = ViewState["SelectedStatus"]?.ToString();

                if (!string.IsNullOrEmpty(selectedActivity) || !string.IsNullOrEmpty(selectedStatus))
                {
                    FilterDataByActivityAndStatus(selectedActivity, selectedStatus);
                }
                else
                {
                    BindGridView();
                }
            }

            GridView1.PageIndex = 0;
            UpdatePageIndexTextBox();
        }
        private void SearchData(string searchText)
        {
            ViewState["SelectedActivity"] = null;
            ViewState["SelectedStatus"] = null;
            // Clear filter date value
            hfSelectedDate.Value = "";
            List<string> userPermissions = Session["UserPermissions"] as List<string>;
            string userDepartment = Session["Department"]?.ToString();
            bool excludeApiKeyEmailUpdate = userDepartment != "MIS";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"SELECT * FROM AuditLogs 
                         WHERE (Name LIKE @searchText 
                         OR (DATE_FORMAT(UserLogDateTime, '%p') LIKE @searchText AND UserLogDateTime IS NOT NULL) 
                         OR (DATE_FORMAT(UserLogDateTime, '%l:') LIKE @searchText AND UserLogDateTime IS NOT NULL) 
                         OR (DATE_FORMAT(UserLogDateTime, '%l %p') LIKE @searchText AND UserLogDateTime IS NOT NULL) 
                         OR Activity LIKE @searchText 
                         OR Status LIKE @searchText)";

                if (excludeApiKeyEmailUpdate)
                {
                    query += " AND Activity != 'Updated API Key and Email'";
                }

                if (userPermissions.Contains("View all user logs within their department") &&
                    !userPermissions.Contains("View all user logs from all departments"))
                {
                    query += " AND UserID IN (SELECT UserID FROM Accounts WHERE Department = @Department)";
                }

                query += " ORDER BY UserID DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SearchText", $"%{searchText}%");

                    if (excludeApiKeyEmailUpdate)
                    {
                        cmd.Parameters.AddWithValue("@Department", userDepartment);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView1.DataSource = dataSet.Tables[0];
                            GridView1.DataBind();
                            BindPagination();
                            searchtxtbox.Text = "";

                            // Calculate total number of pages for the searched results
                            int totalRows = dataSet.Tables[0].Rows.Count;
                            int pageSize = GridView1.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                            lblTotalPages.Text = totalPages.ToString();
                        }
                        else
                        {
                            // No matching records found
                            GridView1.DataSource = null;
                            GridView1.DataBind();
                            BindPagination();
                            searchtxtbox.Text = "";

                            // Set total pages to 1 if no results found
                            lblTotalPages.Text = "1";
                        }
                    }
                }
            }
        }

        // FILTER ACTIVITY AND STATUS FUNCTION BELOW
        private void FilterDataByActivityAndStatus(string activity, string status)
        {
            // Reset search text
            ViewState["SearchText"] = null;
            // Clear filter date value
            hfSelectedDate.Value = "";
            List<string> userPermissions = Session["UserPermissions"] as List<string>;
            string userDepartment = Session["Department"]?.ToString();
            bool excludeApiKeyEmailUpdate = userDepartment != "MIS";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM auditlogs WHERE 1=1";

                if (!string.IsNullOrEmpty(activity))
                {
                    query += " AND Activity = @Activity";
                }
                if (!string.IsNullOrEmpty(status))
                {
                    query += " AND Status = @Status";
                }

                if (excludeApiKeyEmailUpdate)
                {
                    query += " AND Activity != 'Updated API Key and Email'";
                }

                if (userPermissions.Contains("View all user logs within their department") &&
                    !userPermissions.Contains("View all user logs from all departments"))
                {
                    query += " AND UserID IN (SELECT UserID FROM Accounts WHERE Department = @Department)";
                }

                query += " ORDER BY UserLogDateTime DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(activity))
                    {
                        cmd.Parameters.AddWithValue("@Activity", activity);
                    }
                    if (!string.IsNullOrEmpty(status))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);
                    }
                    if (excludeApiKeyEmailUpdate)
                    {
                        cmd.Parameters.AddWithValue("@Department", userDepartment);
                    }
                    else if (userPermissions.Contains("View all user logs within their department") &&
                        !userPermissions.Contains("View all user logs from all departments"))
                    {
                        cmd.Parameters.AddWithValue("@Department", Session["Department"].ToString());
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView1.DataSource = dataSet.Tables[0];
                            GridView1.DataBind();
                            BindPagination();

                            // Calculate total number of pages for the filtered results
                            int totalRows = dataSet.Tables[0].Rows.Count;
                            int pageSize = GridView1.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                            lblTotalPages.Text = totalPages.ToString();
                        }
                        else
                        {
                            GridView1.DataSource = null;
                            GridView1.DataBind();
                            BindPagination();

                            // Set total pages to 1 if no results found
                            lblTotalPages.Text = "1";
                        }
                    }
                }
            }
        }
        protected void activityFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedActivity = ddlUserAct.SelectedValue;
            ViewState["SelectedActivity"] = selectedActivity;

            // Reset page index to 0 when a filter is applied
            GridView1.PageIndex = 0;

            // Update page index text box
            UpdatePageIndexTextBox();

            // Check if status filter is set to default value
            string selectedStatus = ViewState["SelectedStatus"]?.ToString();

            if (selectedActivity == "" && selectedStatus == "")
            {
                // No filters applied, display all data
                BindGridView();
            }
            else if (selectedActivity != "")
            {
                // activity filter selected, check status filter
                if (selectedStatus != "")
                {
                    FilterDataByActivityAndStatus(selectedActivity, selectedStatus);
                }
                else
                {
                    // Only activity filter applied
                    FilterDataByActivity(selectedActivity);
                }
            }
            else
            {
                // Department filter set to default value, status filter may have a value
                FilterDataByStatus(selectedStatus);
            }

            GridView1.PageIndex = 0;
        }
        private void FilterDataByActivity(string activity)
        {
            // Clear filter date value
            hfSelectedDate.Value = "";
            List<string> userPermissions = Session["UserPermissions"] as List<string>;
            string query = "SELECT * FROM auditlogs WHERE 1=1";

            if (userPermissions.Contains("View all user logs within their department") &&
                !userPermissions.Contains("View all user logs from all departments"))
            {
                // Show logs only within the user's department
                query += " AND UserID IN (SELECT UserID FROM Accounts WHERE Department = @Department)";
            }

            if (!string.IsNullOrEmpty(activity))
            {
                query += " AND Activity = @Activity";
            }

            query += " ORDER BY UserLogDateTime DESC";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (userPermissions.Contains("View all user logs within their department") &&
                !userPermissions.Contains("View all user logs from all departments"))
                    {
                        cmd.Parameters.AddWithValue("@Department", Session["Department"].ToString());
                    }
                    if (!string.IsNullOrEmpty(activity))
                    {
                        cmd.Parameters.AddWithValue("@Activity", activity);
                    }

                    try
                    {
                        conn.Open();
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataSet dataSet = new DataSet();
                            adapter.Fill(dataSet);

                            if (dataSet.Tables[0].Rows.Count > 0)
                            {
                                GridView1.DataSource = dataSet.Tables[0];
                                GridView1.DataBind();
                                BindPagination();

                                // Calculate total number of pages for the filtered results
                                int totalRows = dataSet.Tables[0].Rows.Count;
                                int pageSize = GridView1.PageSize;
                                int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                                lblTotalPages.Text = totalPages.ToString();
                            }
                            else
                            {
                                GridView1.DataSource = null;
                                GridView1.DataBind();
                                BindPagination();

                                // Set total pages to 0 if no results found
                                lblTotalPages.Text = "1";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle the error appropriately
                        Response.Write("Error fetching data: " + ex.Message);
                        // Optionally, log this error or display it on the UI
                    }
                }
            }
        }
        private void FilterDataByStatus(string status)
        {
            // Clear filter date value
            hfSelectedDate.Value = "";
            List<string> userPermissions = Session["UserPermissions"] as List<string>;
            string userDepartment = Session["Department"]?.ToString();
            bool excludeApiKeyEmailUpdate = userDepartment != "MIS";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM auditlogs WHERE 1=1";

                if (excludeApiKeyEmailUpdate)
                {
                    query += " AND Activity != 'Updated API Key and Email'";
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query += " AND Status = @Status";
                }

                if (userPermissions.Contains("View all user logs within their department") &&
                    !userPermissions.Contains("View all user logs from all departments"))
                {
                    query += " AND UserID IN (SELECT UserID FROM Accounts WHERE Department = @Department)";
                }

                query += " ORDER BY UserLogDateTime DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(status))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);
                    }
                    if (excludeApiKeyEmailUpdate)
                    {
                        cmd.Parameters.AddWithValue("@Department", userDepartment);
                    }
                    else if (userPermissions.Contains("View all user logs within their department") &&
                        !userPermissions.Contains("View all user logs from all departments"))
                    {
                        cmd.Parameters.AddWithValue("@Department", Session["Department"].ToString());
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView1.DataSource = dataSet.Tables[0];
                            GridView1.DataBind();
                            BindPagination();

                            // Calculate total number of pages for the filtered results
                            int totalRows = dataSet.Tables[0].Rows.Count;
                            int pageSize = GridView1.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                            lblTotalPages.Text = totalPages.ToString();
                        }
                        else
                        {
                            GridView1.DataSource = null;
                            GridView1.DataBind();
                            BindPagination();

                            // Set total pages to 1 if no results found
                            lblTotalPages.Text = "1";
                        }
                    }
                }
            }
        }

        protected void statusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedStatus = ddlUserStatus.SelectedValue;
            ViewState["SelectedStatus"] = selectedStatus;

            // Reset page index to 0 when a filter is applied
            GridView1.PageIndex = 0;

            // Update page index text box
            UpdatePageIndexTextBox();

            // Check if activity filter is set to default value
            string selectedActivity = ViewState["SelectedActivity"]?.ToString();

            if (selectedStatus == "" && selectedActivity == "")
            {
                // No filters applied, display all data
                BindGridView();
            }
            else if (selectedStatus != "")
            {
                // Status filter selected, check activity filter
                if (selectedActivity != "")
                {
                    FilterDataByActivityAndStatus(selectedActivity, selectedStatus);
                }
                else
                {
                    // Only status filter applied
                    FilterDataByStatus(selectedStatus);
                }
            }
            else
            {
                // Status filter set to default value, activity filter may have a value
                FilterDataByActivity(selectedActivity);
            }

            GridView1.PageIndex = 0;
        }




        

        // TABLE PAGE FUNCTION BELOW
        private void UpdatePageIndexTextBox()
        {
            // Update the txtboxPageNum with the current page index + 1 (to display 1-based index)
            lblPageNum.Text = (GridView1.PageIndex + 1).ToString();
        }
        private void BindPagination()
        {
            int pageCount = GridView1.PageCount;
            int currentPage = GridView1.PageIndex;

            btnPrev.Visible = currentPage > 0;
            btnNext.Visible = currentPage < (pageCount - 1);

            // Update the txtboxPageNum with the current page index
            UpdatePageIndexTextBox();
        }
        protected void lnkPage_Click(object sender, EventArgs e)
        {
            LinkButton lnkPage = (LinkButton)sender;
            if (int.TryParse(lnkPage.Text, out int pageIndex))
            {
                GridView1.PageIndex = pageIndex - 1;

                // Check if any filters are applied
                string selectedActivity = ViewState["SelectedActivity"]?.ToString();
                string selectedStatus = ViewState["SelectedStatus"]?.ToString();
                string searchText = ViewState["SearchText"]?.ToString();

                string selectedDate = hfSelectedDate.Value;
                DateTime filterDate;
                if (DateTime.TryParse(selectedDate, out filterDate))
                {
                    BindGridView(filterDate);
                }
                else if (!string.IsNullOrEmpty(searchText))
                {
                    SearchData(searchText);
                }
                else if (!string.IsNullOrEmpty(selectedActivity) || !string.IsNullOrEmpty(selectedStatus))
                {
                    FilterDataByActivityAndStatus(selectedActivity, selectedStatus);
                }
                else
                {
                    BindGridView();
                }
                BindPagination();
            }
            else
            {
            }
        }
        protected void btnPrev_Click(object sender, EventArgs e)
        {
            if (GridView1.PageIndex > 0)
            {
                GridView1.PageIndex -= 1;

                BindGridViewWithFilters();
                BindPagination();
            }
        }
        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (GridView1.PageIndex < GridView1.PageCount - 1)
            {
                GridView1.PageIndex += 1;

                BindGridViewWithFilters();
                BindPagination();
            }
        }
        private void BindGridViewWithFilters()
        {
            string selectedActivity = ViewState["SelectedActivity"]?.ToString();
            string selectedStatus = ViewState["SelectedStatus"]?.ToString();
            string searchText = ViewState["SearchText"]?.ToString();

            string selectedDate = hfSelectedDate.Value;
            DateTime filterDate;
            if (DateTime.TryParse(selectedDate, out filterDate))
            {
                BindGridView(filterDate);
            }
            else if (!string.IsNullOrEmpty(searchText))
            {
                SearchData(searchText);
            }
            else if (!string.IsNullOrEmpty(selectedActivity) || !string.IsNullOrEmpty(selectedStatus))
            {
                FilterDataByActivityAndStatus(selectedActivity, selectedStatus);
            }
            else
            {
                BindGridView();
            }
            BindPagination();
        }


        protected void txtPageNumber_TextChanged(object sender, EventArgs e)
        {
            int pageNumber;
            if (int.TryParse(txtPageNumber.Text, out pageNumber))
            {
                if (pageNumber >= 1 && pageNumber <= GridView1.PageCount)
                {
                    // Set the PageIndex of the GridView to the entered page number
                    GridView1.PageIndex = pageNumber - 1;

                    // Re-bind the GridView with the data for the specified page
                    BindGridViewWithFilters();

                    // Update pagination controls
                    BindPagination();

                    // Clear the entered page number
                    txtPageNumber.Text = "";
                }
                else
                {
                    // The page index is out of range, so reset to the first page
                    GridView1.PageIndex = 0;
                    BindGridView();
                    BindPagination();
                    UpdatePageIndexTextBox();
                    txtPageNumber.Text = "";
                    // Display error message for invalid page number using JavaScript alert
                    ScriptManager.RegisterStartupScript(this, GetType(), "InvalidPageNumber", "alert('Invalid page number. Please enter a number between 1 and " + GridView1.PageCount + ".');", true);
                }
            }
            else
            {
                // The page index is out of range, so reset to the first page
                GridView1.PageIndex = 0;
                BindGridView();
                BindPagination();
                UpdatePageIndexTextBox();
                txtPageNumber.Text = "";
                // Display error message for non-numeric input using JavaScript alert
                ScriptManager.RegisterStartupScript(this, GetType(), "InvalidInput", "alert('Please enter a valid number.');", true);
            }
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Check if User ID is blank or null
                string userID = DataBinder.Eval(e.Row.DataItem, "UserID") as string;
                if (string.IsNullOrEmpty(userID))
                {
                    e.Row.Cells[0].Text = "User not registered";
                }

                // Apply color formatting based on status value
                int statusIndex = -1;
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (GridView1.Columns[i].HeaderText.Equals("Status"))
                    {
                        statusIndex = i;
                        break;
                    }
                }

                if (statusIndex != -1)
                {
                    string status = e.Row.Cells[statusIndex].Text.Trim();
                    e.Row.Cells[statusIndex].ForeColor = status.Equals("successful", StringComparison.OrdinalIgnoreCase)
                        ? System.Drawing.Color.Green
                        : System.Drawing.Color.Red;
                }
            }
        }
        protected void ResetFilterBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl); // Reload the current page to reset filters
        }

        private string GetConnectionString()
        {
            string server = "localhost";
            string database = "documentsystem";
            string username = "root";
            string password = "";

            return $"server={server};database={database};uid={username};pwd={password};";
        }
    }
}