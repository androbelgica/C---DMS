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
    public partial class DocumentLogs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Name"] == null)
            {
                // Redirect to login page if session variable is not set
                Response.Redirect("../LOGIN/Login.aspx");
            }
            // Check if the user has the required permissions to view user logs
            List<string> userPermissions = Session["UserPermissions"] as List<string>;
            if (userPermissions == null ||
                (!userPermissions.Contains("View all document logs from all departments") &&
                 !userPermissions.Contains("View all document logs within their department")))
            {
                // Redirect to Access Denied page if the user does not have the required permissions
                Response.Redirect("../PAGES/AccessDenied.aspx");
            }
            if (!IsPostBack)
            {
                // Bind the GridView only when the page is loaded for the first time
                BindGridView2();
                // Store userPermissions in ViewState to make it accessible
                ViewState["UserPermissions"] = userPermissions;
            }
            else
            {
                // Handle date filter
                string selectedDate = hfSelectedDate.Value;
                DateTime filterDate;
                if (DateTime.TryParse(selectedDate, out filterDate))
                {
                    BindGridView2(filterDate);
                }
                else
                {
                    BindGridView2();
                }
            }
            BindPagination();
            UpdatePageIndexTextBox();
        }
        protected void BindGridView2()
        {
            // Get the current logged-in user's name and department from the session
            string currentUser = Session["Name"] != null ? Session["Name"].ToString() : "Unknown";
            string currentDepartment = Session["Department"] != null ? Session["Department"].ToString() : "UnknownDepartment";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Construct the base queries for uploadedfiles and documentlogs
                string uploadedFilesQuery = @"
            SELECT f.ControlID, f.UploaderName, f.FileName, f.UploadDateTime AS LogDateTime, 'Uploaded' AS Activity, f.Status, f.Privacy, f.Category
            FROM files f
            JOIN Accounts a ON f.UploaderName = a.Name
            WHERE f.Activity = 'Uploaded' AND f.Status = 'Successful'
                AND (f.Privacy != 'Only Me' OR (f.Privacy = 'Only Me' AND f.UploaderName = @uploaderName))
                AND (a.Department = @currentDepartment OR @viewAllDepartmentPermission = 1)";

                string documentLogsQuery = @"
            SELECT d.ControlID, d.UploaderName, d.FileName, d.LogDateTime, d.Activity, d.Status, d.Privacy, d.Category
            FROM documentlogs d
            JOIN Accounts a ON d.UploaderName = a.Name
            WHERE (d.Privacy != 'Only Me' OR (d.Privacy = 'Only Me' AND d.UploaderName = @uploaderName))
                AND (a.Department = @currentDepartment OR @viewAllDepartmentPermission = 1)";

                // Combine the queries using UNION ALL
                string combinedQuery = $@"
            ({uploadedFilesQuery})
            UNION ALL
            ({documentLogsQuery})
            ORDER BY LogDateTime DESC";

                using (MySqlCommand cmd = new MySqlCommand(combinedQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@uploaderName", currentUser);
                    cmd.Parameters.AddWithValue("@currentDepartment", currentDepartment);
                    cmd.Parameters.AddWithValue("@viewAllDepartmentPermission", IsViewAllDepartmentPermission());

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                        {
                            // Data is available, bind the GridView
                            GridView2.DataSource = dataSet.Tables[0];
                            GridView2.DataBind();

                            // Calculate total number of pages and set lblTotalPages only during initial binding
                            if (!IsPostBack)
                            {
                                int totalRows = dataSet.Tables[0].Rows.Count;
                                int pageSize = GridView2.PageSize;
                                int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

                                lblTotalPages.Text = totalPages.ToString();
                            }
                        }
                        else
                        {
                            // No data found
                            GridView2.DataSource = null;
                            GridView2.DataBind();
                        }
                    }
                }
            }
        }
        private bool IsViewAllDepartmentPermission()
        {
            List<string> userPermissions = Session["UserPermissions"] as List<string>;
            return userPermissions != null && userPermissions.Contains("View all document logs from all departments");
        }
        protected void BindGridView2(DateTime? filterDate)
        {
            // Get the current logged-in user's name and department from the session
            string currentUser = Session["Name"] != null ? Session["Name"].ToString() : "Unknown";
            string currentDepartment = Session["Department"] != null ? Session["Department"].ToString() : "UnknownDepartment";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Construct the base queries for uploadedfiles and documentlogs
                string uploadedFilesQuery = @"
            SELECT f.ControlID, f.UploaderName, f.FileName, f.UploadDateTime AS LogDateTime, 'Uploaded' AS Activity, f.Status, f.Privacy, f.Category
            FROM files f
            JOIN Accounts a ON f.UploaderName = a.Name
            WHERE f.Activity = 'Uploaded' AND f.Status = 'Successful'
                AND (f.Privacy != 'Only Me' OR (f.Privacy = 'Only Me' AND f.UploaderName = @uploaderName))
                AND (a.Department = @currentDepartment OR @viewAllDepartmentPermission = 1)";

                string documentLogsQuery = @"
            SELECT d.ControlID, d.UploaderName, d.FileName, d.LogDateTime, d.Activity, d.Status, d.Privacy, d.Category
            FROM documentlogs d
            JOIN Accounts a ON d.UploaderName = a.Name
            WHERE (d.Privacy != 'Only Me' OR (d.Privacy = 'Only Me' AND d.UploaderName = @uploaderName))
                AND (a.Department = @currentDepartment OR @viewAllDepartmentPermission = 1)";

                // Add filter by date if filterDate is provided
                if (filterDate.HasValue)
                {
                    string filterDateString = filterDate.Value.ToString("yyyy-MM-dd");
                    uploadedFilesQuery += $" AND DATE(f.UploadDateTime) = '{filterDateString}'";
                    documentLogsQuery += $" AND DATE(d.LogDateTime) = '{filterDateString}'";
                }

                // Combine the queries using UNION ALL
                string combinedQuery = $@"
            ({uploadedFilesQuery})
            UNION ALL
            ({documentLogsQuery})
            ORDER BY LogDateTime DESC";

                using (MySqlCommand cmd = new MySqlCommand(combinedQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@uploaderName", currentUser);
                    cmd.Parameters.AddWithValue("@currentDepartment", currentDepartment);
                    cmd.Parameters.AddWithValue("@viewAllDepartmentPermission", IsViewAllDepartmentPermission());

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                        {
                            // Data is available, bind the GridView
                            GridView2.DataSource = dataSet.Tables[0];
                            GridView2.DataBind();

                            int totalRows = dataSet.Tables[0].Rows.Count;
                            int pageSize = GridView2.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

                            lblTotalPages.Text = totalPages.ToString();
                        }
                        else
                        {
                            // No data found
                            GridView2.DataSource = null;
                            GridView2.DataBind();
                        }
                    }
                }
            }
        }
        //----- SEARCH FUNCTION BELOW
        protected void BindGridViewWithSearch()
        {
            if (ViewState["SearchText"] != null)
            {
                string searchText = ViewState["SearchText"].ToString();

                using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    // Get the current logged-in user's department and permissions
                    string currentUser = Session["Name"] != null ? Session["Name"].ToString() : "Unknown";
                    string currentDepartment = Session["Department"] != null ? Session["Department"].ToString() : "UnknownDepartment";
                    bool viewAllDepartmentPermission = IsViewAllDepartmentPermission();

                    // Construct the query based on user permissions
                    string query = @"
                SELECT ControlID, UploaderName, FileName, Activity, Status, Privacy, Category, LogDateTime
                FROM documentlogs
                WHERE (UploaderName LIKE @SearchText 
                    OR ControlID LIKE @SearchText 
                    OR FileName LIKE @SearchText 
                    OR Privacy LIKE @SearchText 
                    OR Category LIKE @SearchText 
                    OR (DATE_FORMAT(LogDateTime, '%p') LIKE @SearchText AND LogDateTime IS NOT NULL) 
                    OR (DATE_FORMAT(LogDateTime, '%l:') LIKE @SearchText AND LogDateTime IS NOT NULL) 
                    OR (DATE_FORMAT(LogDateTime, '%l %p') LIKE @SearchText AND LogDateTime IS NOT NULL) 
                    OR Activity LIKE @SearchText 
                    OR Status LIKE @SearchText)
                    AND (@viewAllDepartmentPermission = 1 OR UploaderName IN (
                        SELECT Name 
                        FROM Accounts 
                        WHERE Department = @currentDepartment
                    ))
                UNION ALL
                SELECT ControlID, UploaderName, FileName, Activity, Status, Privacy, Category, UploadDateTime AS LogDateTime
                FROM files
                WHERE (UploaderName LIKE @SearchText 
                    OR ControlID LIKE @SearchText 
                    OR FileName LIKE @SearchText 
                    OR Privacy LIKE @SearchText 
                    OR Category LIKE @SearchText 
                    OR (DATE_FORMAT(UploadDateTime, '%p') LIKE @SearchText AND UploadDateTime IS NOT NULL) 
                    OR (DATE_FORMAT(UploadDateTime, '%l:') LIKE @SearchText AND UploadDateTime IS NOT NULL) 
                    OR (DATE_FORMAT(UploadDateTime, '%l %p') LIKE @SearchText AND UploadDateTime IS NOT NULL) 
                    OR Activity LIKE @SearchText 
                    OR Status LIKE @SearchText)
                    AND (@viewAllDepartmentPermission = 1 OR UploaderName IN (
                        SELECT Name 
                        FROM Accounts 
                        WHERE Department = @currentDepartment
                    ))
                ORDER BY LogDateTime DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SearchText", $"%{searchText}%");
                        cmd.Parameters.AddWithValue("@currentDepartment", currentDepartment);
                        cmd.Parameters.AddWithValue("@viewAllDepartmentPermission", viewAllDepartmentPermission ? 1 : 0);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataSet dataSet = new DataSet();
                            adapter.Fill(dataSet);

                            if (dataSet.Tables[0].Rows.Count > 0)
                            {
                                GridView2.DataSource = dataSet.Tables[0];
                                GridView2.DataBind();
                                BindPagination();
                                searchtxtbox.Text = "";

                                // Calculate total number of pages for the searched results
                                int totalRows = dataSet.Tables[0].Rows.Count;
                                int pageSize = GridView2.PageSize;
                                int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                                lblTotalPages.Text = totalPages.ToString();
                            }
                            else
                            {
                                // No matching records found
                                GridView2.DataSource = null;
                                GridView2.DataBind();
                                BindPagination();
                                searchtxtbox.Text = "";

                                // Set total pages to 1 if no results found
                                lblTotalPages.Text = "1";
                            }
                        }
                    }
                }
            }
            else
            {
                BindGridView2(); // Handle case where ViewState["SearchText"] is null
            }
        }
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
                    BindGridView2();
                }
            }

            GridView2.PageIndex = 0;
            UpdatePageIndexTextBox();
        }
        private void SearchData(string searchText)
        {
            ViewState["SelectedActivity"] = null;
            ViewState["SelectedStatus"] = null;
            // Clear filter date value
            hfSelectedDate.Value = "";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Get the current logged-in user's department and permissions
                string currentUser = Session["Name"] != null ? Session["Name"].ToString() : "Unknown";
                string currentDepartment = Session["Department"] != null ? Session["Department"].ToString() : "UnknownDepartment";
                bool viewAllDepartmentPermission = IsViewAllDepartmentPermission();

                // Construct the query based on user permissions
                string queryDocumentLogs = @"
            SELECT ControlID, UploaderName, FileName, Activity, Status, Privacy, Category, LogDateTime 
            FROM documentlogs 
            WHERE (UploaderName LIKE @SearchText 
                OR ControlID LIKE @SearchText 
                OR FileName LIKE @SearchText 
                OR Privacy LIKE @SearchText 
                OR Category LIKE @SearchText 
                OR (DATE_FORMAT(LogDateTime, '%p') LIKE @SearchText AND LogDateTime IS NOT NULL) 
                OR (DATE_FORMAT(LogDateTime, '%l:') LIKE @SearchText AND LogDateTime IS NOT NULL) 
                OR (DATE_FORMAT(LogDateTime, '%l %p') LIKE @SearchText AND LogDateTime IS NOT NULL) 
                OR Activity LIKE @SearchText 
                OR Status LIKE @SearchText)
                AND (@viewAllDepartmentPermission = 1 OR UploaderName IN (
                    SELECT Name 
                    FROM Accounts 
                    WHERE Department = @currentDepartment
                ))";

                string queryFiles = @"
            SELECT ControlID, UploaderName, FileName, Activity, Status, Privacy, Category, UploadDateTime AS LogDateTime 
            FROM files 
            WHERE (UploaderName LIKE @SearchText 
                OR ControlID LIKE @SearchText 
                OR FileName LIKE @SearchText 
                OR Privacy LIKE @SearchText 
                OR Category LIKE @SearchText 
                OR (DATE_FORMAT(UploadDateTime, '%p') LIKE @SearchText AND UploadDateTime IS NOT NULL) 
                OR (DATE_FORMAT(UploadDateTime, '%l:') LIKE @SearchText AND UploadDateTime IS NOT NULL) 
                OR (DATE_FORMAT(UploadDateTime, '%l %p') LIKE @SearchText AND UploadDateTime IS NOT NULL) 
                OR Activity LIKE @SearchText 
                OR Status LIKE @SearchText)
                AND (@viewAllDepartmentPermission = 1 OR UploaderName IN (
                    SELECT Name 
                    FROM Accounts 
                    WHERE Department = @currentDepartment
                ))";

                string query = queryDocumentLogs + " UNION ALL " + queryFiles + " ORDER BY LogDateTime DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SearchText", $"%{searchText}%");
                    cmd.Parameters.AddWithValue("@currentDepartment", currentDepartment);
                    cmd.Parameters.AddWithValue("@viewAllDepartmentPermission", viewAllDepartmentPermission ? 1 : 0);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView2.DataSource = dataSet.Tables[0];
                            GridView2.DataBind();
                            BindPagination();
                            searchtxtbox.Text = "";

                            // Calculate total number of pages for the searched results
                            int totalRows = dataSet.Tables[0].Rows.Count;
                            int pageSize = GridView2.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                            lblTotalPages.Text = totalPages.ToString();
                        }
                        else
                        {
                            // No matching records found
                            GridView2.DataSource = null;
                            GridView2.DataBind();
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
        private void FilterDataByActivity(string activity)
        {
            // Reset search text
            ViewState["SearchText"] = null;
            // Clear filter date value
            hfSelectedDate.Value = "";

            // Get the current logged-in user's name from the session
            string currentUser = Session["Name"] != null ? Session["Name"].ToString() : "Unknown";

            // Get the current logged-in user's department from the session
            string currentDepartment = Session["Department"] != null ? Session["Department"].ToString() : "";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Construct the base SELECT statement for documentlogs
                string queryDocumentLogs = @"
            SELECT ControlID, UploaderName, FileName, LogDateTime, Activity, Status, Privacy, Category
            FROM documentlogs";

                // Construct the base SELECT statement for uploadedfiles
                string queryUploadedFiles = @"
            SELECT ControlID, UploaderName, FileName, UploadDateTime AS LogDateTime, Activity, Status, Privacy, Category
            FROM files";

                // Construct the WHERE clause based on selected activity
                List<string> conditions = new List<string>();

                // Add conditions for specific activities
                if (activity == "Select Activity")
                {
                    // No specific activity selected, include all activities
                    conditions.Add($"1=1");
                }
                else if (activity == "Uploaded" || activity == "Edited" || activity == "Deleted")
                {
                    conditions.Add($"Activity = @Activity");
                }

                // Apply department-based filtering logic
                if (!IsViewAllDepartmentPermission())
                {
                    conditions.Add($"UploaderName IN (SELECT Name FROM Accounts WHERE Department = @currentDepartment)");
                }

                // Add condition for 'Only Me' privacy
                conditions.Add($"(Privacy != 'Only Me' OR (Privacy = 'Only Me' AND UploaderName = @currentUser))");

                string whereClause = "";
                if (conditions.Count > 0)
                {
                    whereClause = " WHERE " + string.Join(" AND ", conditions);
                }

                // Apply the WHERE clause to both queries
                queryDocumentLogs += whereClause;
                queryUploadedFiles += whereClause;

                // Combine the queries using UNION ALL
                string query = $@"
            ({queryDocumentLogs})
            UNION ALL
            ({queryUploadedFiles})
            ORDER BY LogDateTime DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Set parameters for activity, current user, and department
                    cmd.Parameters.AddWithValue("@Activity", activity);
                    cmd.Parameters.AddWithValue("@currentUser", currentUser);
                    cmd.Parameters.AddWithValue("@currentDepartment", currentDepartment);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView2.DataSource = dataSet.Tables[0];
                            GridView2.DataBind();
                            BindPagination();

                            // Calculate total number of pages for the filtered results
                            int totalRows = dataSet.Tables[0].Rows.Count;
                            int pageSize = GridView2.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                            lblTotalPages.Text = totalPages.ToString();
                        }
                        else
                        {
                            GridView2.DataSource = null;
                            GridView2.DataBind();
                            BindPagination();

                            // Set total pages to 1 if no results found
                            lblTotalPages.Text = "1";
                        }
                    }
                }
            }
        }
        private void FilterDataByStatus(string status)
        {
            // Reset search text
            ViewState["SearchText"] = null;
            // Clear filter date value
            hfSelectedDate.Value = "";

            // Get the current logged-in user's name from the session
            string currentUser = Session["Name"] != null ? Session["Name"].ToString() : "Unknown";

            // Get the current logged-in user's department from the session
            string currentDepartment = Session["Department"] != null ? Session["Department"].ToString() : "";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Construct the base SELECT statement for documentlogs
                string queryDocumentLogs = @"
            SELECT ControlID, UploaderName, FileName, LogDateTime, Activity, Status, Privacy, Category
            FROM documentlogs";

                // Construct the base SELECT statement for uploadedfiles
                string queryUploadedFiles = @"
            SELECT ControlID, UploaderName, FileName, UploadDateTime AS LogDateTime, Activity, Status, Privacy, Category
            FROM files";

                // Construct the WHERE clause based on selected status
                List<string> conditions = new List<string>();

                if (status == "Select Status")
                {
                    // No specific status selected, include all statuses
                    conditions.Add($"1=1");
                }
                else
                {
                    conditions.Add($"Status = @Status");
                }

                // Apply department-based filtering logic
                if (!IsViewAllDepartmentPermission())
                {
                    conditions.Add($"UploaderName IN (SELECT Name FROM Accounts WHERE Department = @currentDepartment)");
                }

                // Add condition for 'Only Me' privacy
                conditions.Add($"(Privacy != 'Only Me' OR (Privacy = 'Only Me' AND UploaderName = @currentUser))");

                string whereClause = "";
                if (conditions.Count > 0)
                {
                    whereClause = " WHERE " + string.Join(" AND ", conditions);
                }

                // Apply the WHERE clause to both queries
                queryDocumentLogs += whereClause;
                queryUploadedFiles += whereClause;

                // Combine the queries using UNION ALL
                string query = $@"
            ({queryDocumentLogs})
            UNION ALL
            ({queryUploadedFiles})
            ORDER BY LogDateTime DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Set parameters for status and current user
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@currentUser", currentUser);
                    cmd.Parameters.AddWithValue("@currentDepartment", currentDepartment);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView2.DataSource = dataSet.Tables[0];
                            GridView2.DataBind();
                            BindPagination();

                            // Calculate total number of pages for the filtered results
                            int totalRows = dataSet.Tables[0].Rows.Count;
                            int pageSize = GridView2.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                            lblTotalPages.Text = totalPages.ToString();
                        }
                        else
                        {
                            GridView2.DataSource = null;
                            GridView2.DataBind();
                            BindPagination();

                            // Set total pages to 1 if no results found
                            lblTotalPages.Text = "1";
                        }
                    }
                }
            }
        }
        private void FilterDataByActivityAndStatus(string activity, string status)
        {
            // Reset search text
            ViewState["SearchText"] = null;
            // Clear filter date value
            hfSelectedDate.Value = "";

            // Get the current logged-in user's name and department from the session
            string currentUser = Session["Name"] != null ? Session["Name"].ToString() : "Unknown";
            string currentDepartment = Session["Department"] != null ? Session["Department"].ToString() : "UnknownDepartment";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Construct the base SELECT statement for documentlogs
                string queryDocumentLogs = @"
            SELECT ControlID, UploaderName, FileName, LogDateTime, Activity, Status, Privacy, Category
            FROM documentlogs";

                // Construct the base SELECT statement for uploadedfiles
                string queryUploadedFiles = @"
            SELECT ControlID, UploaderName, FileName, UploadDateTime AS LogDateTime, Activity, Status, Privacy, Category
            FROM files";

                // Construct the WHERE clause based on selected activity and status
                List<string> conditions = new List<string>();

                if (activity == "Select Activity" && status == "Select Status")
                {
                    // No specific activity or status selected, include all data
                    conditions.Add($"1=1");
                }
                else
                {
                    if (activity == "Select Activity")
                    {
                        // No specific activity selected, include all activities
                        conditions.Add($"1=1");
                    }
                    else if (activity == "Uploaded")
                    {
                        conditions.Add($"Activity = 'Uploaded'");
                    }
                    else if (activity == "Edited" || activity == "Deleted")
                    {
                        conditions.Add($"Activity = '{activity}'");
                    }

                    if (status == "Select Status")
                    {
                        // No specific status selected, include all statuses
                        conditions.Add($"1=1");
                    }
                    else
                    {
                        conditions.Add($"Status = @Status");
                    }
                }
                // Apply department-based filtering logic
                if (!IsViewAllDepartmentPermission())
                {
                    conditions.Add($"UploaderName IN (SELECT Name FROM Accounts WHERE Department = @currentDepartment)");
                }

                // Add condition for 'Only Me' privacy
                conditions.Add($"(Privacy != 'Only Me' OR (Privacy = 'Only Me' AND UploaderName = @currentUser))");

                string whereClause = "";
                if (conditions.Count > 0)
                {
                    whereClause = " WHERE " + string.Join(" AND ", conditions);
                }

                // Apply the WHERE clause to both queries
                queryDocumentLogs += whereClause;
                queryUploadedFiles += whereClause;

                // Combine the queries using UNION ALL
                string query = $@"
            ({queryDocumentLogs})
            UNION ALL
            ({queryUploadedFiles})
            ORDER BY LogDateTime DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Set parameters for activity, status, current user, and department
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@currentUser", currentUser);
                    cmd.Parameters.AddWithValue("@currentDepartment", currentDepartment);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView2.DataSource = dataSet.Tables[0];
                            GridView2.DataBind();
                            BindPagination();

                            // Calculate total number of pages for the filtered results
                            int totalRows = dataSet.Tables[0].Rows.Count;
                            int pageSize = GridView2.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                            lblTotalPages.Text = totalPages.ToString();
                        }
                        else
                        {
                            GridView2.DataSource = null;
                            GridView2.DataBind();
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
            string selectedActivity = actDropDown.SelectedValue;
            ViewState["SelectedActivity"] = selectedActivity;

            // Reset page index to 0 when a filter is applied
            GridView2.PageIndex = 0;

            // Update page index text box
            UpdatePageIndexTextBox();

            // Check if status filter is set to default value
            string selectedStatus = ViewState["SelectedStatus"]?.ToString();

            if (selectedActivity == "" && selectedStatus == "")
            {
                // No filters applied, display all data
                BindGridView2();
            }
            else if (selectedActivity != "")
            {
                // Activity filter selected, check status filter
                if (selectedStatus != "")
                {
                    // Filter by both activity and status
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
                // Activity filter set to default value, status filter may have a value
                FilterDataByStatus(selectedStatus);
            }

            GridView2.PageIndex = 0;
        }
        protected void statusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedStatus = statusDropDown.SelectedValue;
            ViewState["SelectedStatus"] = selectedStatus;

            // Reset page index to 0 when a filter is applied
            GridView2.PageIndex = 0;

            // Update page index text box
            UpdatePageIndexTextBox();

            // Check if activity filter is set to default value
            string selectedActivity = ViewState["SelectedActivity"]?.ToString();

            if (selectedStatus == "" && selectedActivity == "")
            {
                // No filters applied, display all data
                BindGridView2();
            }
            else if (selectedStatus != "")
            {
                // Status filter selected, check activity filter
                if (selectedActivity != "")
                {
                    // Filter by both activity and status
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

            GridView2.PageIndex = 0;
        }


        // TABLE PAGE FUNCTIONS BELOW
        private void UpdatePageIndexTextBox()
        {
            // Update the txtboxPageNum with the current page index + 1 (to display 1-based index)
            lblPageNum.Text = (GridView2.PageIndex + 1).ToString();
        }
        private void BindPagination()
        {
            int pageCount = GridView2.PageCount;
            int currentPage = GridView2.PageIndex;

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
                GridView2.PageIndex = pageIndex - 1;

                // Check if any filters are applied
                string selectedActivity = ViewState["SelectedActivity"]?.ToString();
                string selectedStatus = ViewState["SelectedStatus"]?.ToString();
                string searchText = ViewState["SearchText"]?.ToString();

                string selectedDate = hfSelectedDate.Value;
                DateTime filterDate;
                if (DateTime.TryParse(selectedDate, out filterDate))
                {
                    BindGridView2(filterDate);
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
                    BindGridView2();
                }
                BindPagination();
            }
            else
            {
            }
        }
        protected void btnPrev_Click(object sender, EventArgs e)
        {
            if (GridView2.PageIndex > 0)
            {
                GridView2.PageIndex -= 1;

                BindGridViewWithFilters();
                BindPagination();
            }
        }
        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (GridView2.PageIndex < GridView2.PageCount - 1)
            {
                GridView2.PageIndex += 1;

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
                BindGridView2(filterDate);
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
                BindGridView2();
            }
        }
        protected void txtPageNumber_TextChanged(object sender, EventArgs e)
        {
            btnGoToPage_Click(sender, e);
        }
        protected void btnGoToPage_Click(object sender, EventArgs e)
        {
            int pageNumber;
            if (int.TryParse(txtPageNumber.Text, out pageNumber))
            {
                if (pageNumber >= 1 && pageNumber <= GridView2.PageCount)
                {
                    // Set the PageIndex of the GridView to the entered page number
                    GridView2.PageIndex = pageNumber - 1;

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
                    GridView2.PageIndex = 0;
                    BindGridView2();
                    BindPagination();
                    UpdatePageIndexTextBox();
                    txtPageNumber.Text = "";
                    // Display error message for invalid page number using JavaScript alert
                    ScriptManager.RegisterStartupScript(this, GetType(), "InvalidPageNumber", "alert('Invalid page number. Please enter a number between 1 and " + GridView2.PageCount + ".');", true);
                }
            }
            else
            {
                // The page index is out of range, so reset to the first page
                GridView2.PageIndex = 0;
                BindGridView2();
                BindPagination();
                UpdatePageIndexTextBox();
                txtPageNumber.Text = "";
                // Display error message for non-numeric input using JavaScript alert
                ScriptManager.RegisterStartupScript(this, GetType(), "InvalidInput", "alert('Please enter a valid number.');", true);
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
        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Find the index of the "Status" column dynamically
                int statusIndex = -1;
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (GridView2.Columns[i].HeaderText.Equals("Status"))
                    {
                        statusIndex = i;
                        break;
                    }
                }

                if (statusIndex != -1)
                {
                    // Apply color formatting based on status value
                    string status = e.Row.Cells[statusIndex].Text.Trim();
                    if (status.Equals("successful", StringComparison.OrdinalIgnoreCase))
                    {
                        e.Row.Cells[statusIndex].ForeColor = System.Drawing.Color.Green;
                    }
                    else if (status.Equals("failed", StringComparison.OrdinalIgnoreCase))
                    {
                        e.Row.Cells[statusIndex].ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
        }
        protected void btnDocsAddFilter_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string selectedDocsActivity = actDropDown.SelectedValue;
                string selectedDocsStatus = statusDropDown.SelectedValue;

                // Construct the base SELECT statement for documentlogs
                string queryDocumentLogs = "SELECT ControlID, UploaderName, FileName, Activity, Status, Privacy, Category, LogDateTime FROM documentlogs";
                // Construct the base SELECT statement for uploadedfiles
                string queryUploadedFiles = "SELECT ControlID, UploaderName, FileName, Activity, Status, Privacy, Category, UploadDateTime AS LogDateTime FROM files";

                // Construct the WHERE clause based on selected activity and status
                List<string> conditions = new List<string>();

                if (!string.IsNullOrEmpty(selectedDocsActivity) && selectedDocsActivity != "Select Activity")
                {
                    conditions.Add($"Activity = '{selectedDocsActivity}'");
                }

                if (!string.IsNullOrEmpty(selectedDocsStatus) && selectedDocsStatus != "Select Status")
                {
                    conditions.Add($"Status = '{selectedDocsStatus}'");
                }

                string whereClause = "";
                if (conditions.Count > 0)
                {
                    whereClause = " WHERE " + string.Join(" AND ", conditions);
                }

                // Apply the WHERE clause to both queries
                queryDocumentLogs += whereClause;
                queryUploadedFiles += whereClause;

                // Combine the queries using UNION ALL
                string query = $"{queryDocumentLogs} UNION ALL {queryUploadedFiles} ORDER BY LogDateTime DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        DataTable filteredData = dataSet.Tables[0];

                        // Store filtered data and criteria in ViewState
                        ViewState["FilteredData"] = filteredData;
                        ViewState["SearchText"] = null;
                        ViewState["StartDate"] = null;
                        ViewState["EndDate"] = null;

                        // Reset GridView page index to 0 when applying filters
                        GridView2.PageIndex = 0;

                        // Rebind the GridView with filtered data
                        GridView2.DataSource = filteredData;
                        GridView2.DataBind();
                    }
                }
            }
        }
    }
}