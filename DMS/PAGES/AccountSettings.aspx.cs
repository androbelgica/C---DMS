using DocumentFormat.OpenXml.Bibliography;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Controls;
using ListItem = System.Web.UI.WebControls.ListItem;

namespace DMS.PAGES
{
    public partial class AccountSettings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Name"] == null)
            {
                // Redirect to login page if session variable is not set
                Response.Redirect("../LOGIN/Login.aspx");
            }

            string userId = Session["UserID"].ToString(); // Assuming you store UserID in session
            List<string> userPermissions = GetUserPermissionsPageLoad(userId);

            // Check if the user has the required permissions
            if (!userPermissions.Contains("Create new users for all departments") &&
                !userPermissions.Contains("Create new users in their department"))
            {
                // Redirect to access denied page if the user does not have the required permissions
                Response.Redirect("../PAGES/AccessDenied.aspx");
            }

            // Adjust visibility of deptFilter based on permissions
            if (!IsPostBack)
            {
                BindGridView();
                PopulateDepartmentDropdown();
                PopulateFilterDepartmentDropdown(); // This will handle visibility automatically
                PopulateUpdateDepartmentDropdown();
                BindCheckBoxLists();
            }
            if (perUser.Checked)
            {
                BindGridView();
                BindPagination();
            }
            else if (byPosition.Checked)
            {
                BindGridViewByPosition();
                BindPaginationByPosition();
            }
        }
        protected void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            GridView1.PageIndex = 0;
            GridViewByPosition.PageIndex = 0;

            if (perUser.Checked)
            {
                // clear filters if there is applied
                ViewState["SelectedDepartment"] = null; 
                ViewState["SelectedStatus"] = null;     
                ViewState["SearchText"] = null;

                statusFilter.SelectedValue = string.Empty;
                deptFilter.SelectedValue = string.Empty;
                searchtxtbox.Text = string.Empty;

                GridView1.Visible = true;
                paginationPerUser.Visible = true;

                GridViewByPosition.Visible = false;
                paginationByPosition.Visible = false;
                // Bind GridView1 and update pagination
                BindGridView();
                BindPagination();
            }
            else if (byPosition.Checked)
            {
                // clear filters if there is applied
                ViewState["SelectedDepartment"] = null; 
                ViewState["SelectedStatus"] = null;     
                ViewState["SearchText"] = null;

                statusFilter.SelectedValue = string.Empty;
                deptFilter.SelectedValue = string.Empty;
                searchtxtbox.Text = string.Empty;

                GridViewByPosition.Visible = true;
                paginationByPosition.Visible = true;

                GridView1.Visible = false;
                paginationPerUser.Visible = false;
                // Bind GridViewByPosition and update pagination
                BindGridViewByPosition();
                BindPaginationByPosition(); 
            }
        }
        private void BindGridViewByPosition()
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM Accounts";
                List<string> userPermissions = GetUserPermissionsPageLoad(Session["UserID"].ToString());

                // Check if user has permission to create users for all departments
                if (userPermissions.Contains("Create new users for all departments"))
                {
                    query += " ORDER BY UserID DESC";
                }
                else if (userPermissions.Contains("Create new users in their department"))
                {
                    string userDepartment = GetUserDepartment(Session["UserID"].ToString());
                    query += " WHERE Department = @Department ORDER BY UserID DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Department", userDepartment);
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataSet dataSet = new DataSet();
                            adapter.Fill(dataSet);

                            if (dataSet.Tables[0].Rows.Count > 0)
                            {
                                GridViewByPosition.DataSource = dataSet.Tables[0];
                                GridViewByPosition.DataBind();
                            }
                            else
                            {
                                GridViewByPosition.DataSource = null;
                                GridViewByPosition.DataBind();
                            }
                            BindPaginationByPosition();
                            int totalRows = dataSet.Tables[0].Rows.Count;
                            int pageSize = GridViewByPosition.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                            lblTotalPagesPosition.Text = totalPages.ToString();

                        }
                    }
                    return;
                }

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridViewByPosition.DataSource = dataSet.Tables[0];
                            GridViewByPosition.DataBind();
                        }
                        else
                        {
                            GridViewByPosition.DataSource = null;
                            GridViewByPosition.DataBind();
                        }
                        BindPaginationByPosition();
                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridViewByPosition.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPagesPosition.Text = totalPages.ToString();

                    }
                }
            }
        }
        private void BindGridView()
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM Accounts";
                List<string> userPermissions = GetUserPermissionsPageLoad(Session["UserID"].ToString());

                // Check if user has permission to create users for all departments
                if (userPermissions.Contains("Create new users for all departments"))
                {
                    query += " ORDER BY UserID DESC";
                }
                else if (userPermissions.Contains("Create new users in their department"))
                {
                    string userDepartment = GetUserDepartment(Session["UserID"].ToString());
                    query += " WHERE Department = @Department ORDER BY UserID DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Department", userDepartment);
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataSet dataSet = new DataSet();
                            adapter.Fill(dataSet);

                            if (dataSet.Tables[0].Rows.Count > 0)
                            {
                                GridView1.DataSource = dataSet.Tables[0];
                                GridView1.DataBind();
                            }
                            else
                            {
                                GridView1.DataSource = null;
                                GridView1.DataBind();
                            }
                            BindPagination();
                            int totalRows = dataSet.Tables[0].Rows.Count;
                            int pageSize = GridView1.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                            lblTotalPages.Text = totalPages.ToString();

                        }
                    }
                    return;
                }

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView1.DataSource = dataSet.Tables[0];
                            GridView1.DataBind();
                        }
                        else
                        {
                            GridView1.DataSource = null;
                            GridView1.DataBind();
                        }
                        BindPagination();
                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridView1.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPages.Text = totalPages.ToString();

                    }
                }
            }
        }
        private List<string> GetUserPermissionsPageLoad(string userId)
        {
            List<string> permissions = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                string query = "SELECT p.PermissionName " +
                               "FROM User_Permissions up " +
                               "JOIN Permissions p ON up.PermissionID = p.PermissionID " +
                               "WHERE up.UserID = @UserID";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        permissions.Add(reader["PermissionName"].ToString());
                    }
                }
            }
            return permissions;
        }

        // SEARCH FUNCTION BELOW
        protected void searchtxtbox_TextChanged(object sender, EventArgs e)
        {
            string searchText = searchtxtbox.Text.Trim();
            ViewState["SearchText"] = searchText;

            if (perUser.Checked)
            {
                if (!string.IsNullOrEmpty(searchText))
                {
                    SearchData(searchText);
                }
                else
                {
                    // Check if any filters are applied
                    string selectedDepartment = ViewState["SelectedDepartment"]?.ToString();
                    string selectedStatus = ViewState["SelectedStatus"]?.ToString();

                    if (!string.IsNullOrEmpty(selectedDepartment) || !string.IsNullOrEmpty(selectedStatus))
                    {
                        FilterDataByDepartmentAndStatus(selectedDepartment, selectedStatus);
                    }
                    else
                    {
                        BindGridView();
                    }
                }
            }
            else if (byPosition.Checked)
            {
                if (!string.IsNullOrEmpty(searchText))
                {
                    SearchDataByPosition(searchText);
                }
                else
                {
                    // Check if any filters are applied
                    string selectedDepartment = ViewState["SelectedDepartment"]?.ToString();
                    string selectedStatus = ViewState["SelectedStatus"]?.ToString();

                    if (!string.IsNullOrEmpty(selectedDepartment) || !string.IsNullOrEmpty(selectedStatus))
                    {
                        FilterDataByDepartmentAndStatusByPosition(selectedDepartment, selectedStatus);
                    }
                    else
                    {
                        BindGridViewByPosition();
                    }
                }
            }

            GridView1.PageIndex = 0;
            GridViewByPosition.PageIndex = 0;
        }
        private void SearchData(string searchText)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"SELECT * FROM Accounts 
                         WHERE (UserID LIKE @SearchText 
                                OR Name LIKE @SearchText 
                                OR Username LIKE @SearchText 
                                OR Department LIKE @SearchText 
                                OR Position LIKE @SearchText 
                                OR Email LIKE @SearchText 
                                OR Contact LIKE @SearchText 
                                OR Status LIKE @SearchText 
                                OR Access LIKE @SearchText)";

                List<string> userPermissions = GetUserPermissionsPageLoad(Session["UserID"].ToString());

                // If user has permission to create users for all departments, do not filter by department
                if (userPermissions.Contains("Create new users for all departments"))
                {
                    // No additional filtering needed
                }
                else if (userPermissions.Contains("Create new users in their department"))
                {
                    string userDepartment = GetUserDepartment(Session["UserID"].ToString());
                    query += " AND Department = @Department";
                }

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SearchText", $"%{searchText}%");

                    if (userPermissions.Contains("Create new users in their department"))
                    {
                        string userDepartment = GetUserDepartment(Session["UserID"].ToString());
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
                        }
                        else
                        {
                            GridView1.DataSource = null;
                            GridView1.DataBind();
                        }
                        BindPagination();
                        searchtxtbox.Text = "";
                        // Calculate total number of pages for the searched results
                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridView1.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPages.Text = totalPages.ToString();
                    }
                }
            }
        }
        private void SearchDataByPosition(string searchText)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"SELECT * FROM Accounts 
                         WHERE (UserID LIKE @SearchText 
                                OR Name LIKE @SearchText 
                                OR Username LIKE @SearchText 
                                OR Department LIKE @SearchText 
                                OR Position LIKE @SearchText 
                                OR Email LIKE @SearchText 
                                OR Contact LIKE @SearchText 
                                OR Status LIKE @SearchText 
                                OR Access LIKE @SearchText)";

                List<string> userPermissions = GetUserPermissionsPageLoad(Session["UserID"].ToString());

                // If user has permission to create users for all departments, do not filter by department
                if (userPermissions.Contains("Create new users for all departments"))
                {
                    // No additional filtering needed
                }
                else if (userPermissions.Contains("Create new users in their department"))
                {
                    string userDepartment = GetUserDepartment(Session["UserID"].ToString());
                    query += " AND Department = @Department";
                }

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SearchText", $"%{searchText}%");

                    if (userPermissions.Contains("Create new users in their department"))
                    {
                        string userDepartment = GetUserDepartment(Session["UserID"].ToString());
                        cmd.Parameters.AddWithValue("@Department", userDepartment);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridViewByPosition.DataSource = dataSet.Tables[0];
                            GridViewByPosition.DataBind();
                        }
                        else
                        {
                            GridViewByPosition.DataSource = null;
                            GridViewByPosition.DataBind();
                        }
                        BindPaginationByPosition();
                        searchtxtbox.Text = "";
                        // Calculate total number of pages for the searched results
                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridViewByPosition.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPagesPosition.Text = totalPages.ToString();
                    }
                }
            }
        }

        // DEPT AND STATUS FILTER FUNCTION BELOW - PER USER
        protected void PopulateFilterDepartmentDropdown()
        {
            try
            {
                List<string> userPermissions = GetUserPermissionsPageLoad(Session["UserID"].ToString());

                // Check if user has permission to create users for all departments
                if (userPermissions.Contains("Create new users for all departments"))
                {
                    using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                    {
                        conn.Open();

                        string query = "SELECT DepartmentName, ShortAcronym FROM department";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                deptFilter.Items.Clear();
                                deptFilter.Items.Add(new ListItem("Filter by Department", ""));

                                while (reader.Read())
                                {
                                    string departmentName = reader["DepartmentName"].ToString();
                                    string shortAcronym = reader["ShortAcronym"].ToString();
                                    deptFilter.Items.Add(new ListItem(departmentName, shortAcronym));
                                }
                            }
                        }
                    }
                    deptFilter.Visible = true; // Make the department filter dropdown visible
                }
                else if (userPermissions.Contains("Create new users in their department"))
                {
                    deptFilter.Visible = false; // Hide the department filter dropdown
                    ViewState["SelectedDepartment"] = null; // Reset selected department if hidden
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "deptDropdownError", $"alert('Error loading department dropdown: {ex.Message}');", true);
            }
        }
        private void FilterDataByDepartment(string department)
        {
            // Reset search text
            ViewState["SearchText"] = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    string query = "SELECT * FROM Accounts";

                    // Only apply department filter if user has permission to create users for all departments
                    List<string> userPermissions = GetUserPermissionsPageLoad(Session["UserID"].ToString());
                    if (!userPermissions.Contains("Create new users for all departments"))
                    {
                        department = ""; // Clear department filter if user doesn't have permission
                    }

                    if (!string.IsNullOrEmpty(department))
                    {
                        query += " WHERE Department = @Department";
                    }

                    query += " ORDER BY UserID DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(department))
                        {
                            cmd.Parameters.AddWithValue("@Department", department);
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
                            }
                            else
                            {
                                GridView1.DataSource = null;
                                GridView1.DataBind();
                                BindPagination();
                            }
                            int totalRows = dataSet.Tables[0].Rows.Count;
                            int pageSize = GridView1.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                            lblTotalPages.Text = totalPages.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "deptFilterError", $"alert('Error filtering data by department: {ex.Message}');", true);
            }
        }
        private void FilterDataByDepartmentAndStatus(string department, string status)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM Accounts WHERE 1=1";

                List<string> userPermissions = GetUserPermissionsPageLoad(Session["UserID"].ToString());

                // If user has permission to create users for all departments, do not filter by department
                if (userPermissions.Contains("Create new users for all departments"))
                {
                    if (!string.IsNullOrEmpty(department))
                    {
                        query += " AND Department = @Department";
                    }

                    if (!string.IsNullOrEmpty(status))
                    {
                        query += " AND Status = @Status";
                    }
                }

                query += " ORDER BY UserID DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(department))
                    {
                        cmd.Parameters.AddWithValue("@Department", department);
                    }

                    if (!string.IsNullOrEmpty(status))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView1.DataSource = dataSet.Tables[0];
                            GridView1.DataBind();
                        }
                        else
                        {
                            GridView1.DataSource = null;
                            GridView1.DataBind();
                        }
                        BindPagination();
                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridView1.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPages.Text = totalPages.ToString();
                    }
                }
            }
        }
        protected void deptFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string selectedDepartment = deptFilter.SelectedValue;
                ViewState["SelectedDepartment"] = selectedDepartment;

                // Check if status filter is set to default value
                string selectedStatus = ViewState["SelectedStatus"]?.ToString();

                List<string> userPermissions = GetUserPermissionsPageLoad(Session["UserID"].ToString());

                if (userPermissions.Contains("Create new users for all departments"))
                {
                    if (perUser.Checked)
                    {
                        // User has permission to create users for all departments
                        if (selectedDepartment == "" && selectedStatus == "")
                        {
                            // No filters applied, display all data
                            BindGridView();
                        }
                        else if (selectedDepartment != "")
                        {
                            // Department filter selected, check status filter
                            if (selectedStatus != "")
                            {
                                FilterDataByDepartmentAndStatus(selectedDepartment, selectedStatus);
                            }
                            else
                            {
                                // Only department filter applied
                                FilterDataByDepartment(selectedDepartment);
                            }
                        }
                        else
                        {
                            // Department filter set to default value, status filter may have a value
                            FilterDataByStatus(selectedStatus);
                        }
                    }
                    else if (byPosition.Checked)
                    {
                        // User has permission to create users for all departments
                        if (selectedDepartment == "" && selectedStatus == "")
                        {
                            // No filters applied, display all data
                            BindGridViewByPosition();
                        }
                        else if (selectedDepartment != "")
                        {
                            // Department filter selected, check status filter
                            if (selectedStatus != "")
                            {
                                FilterDataByDepartmentAndStatusByPosition(selectedDepartment, selectedStatus);
                            }
                            else
                            {
                                // Only department filter applied
                                FilterDataByDepartmentByPosition(selectedDepartment);
                            }
                        }
                        else
                        {
                            // Department filter set to default value, status filter may have a value
                            FilterDataByStatusByPosition(selectedStatus);
                        }
                    }
                    
                }
                else if (userPermissions.Contains("Create new users in their department"))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "deptFilterError", $"alert('Error applying filter. You do not have the permission to access this action.');", true);
                }

                GridView1.PageIndex = 0;
                GridViewByPosition.PageIndex = 0;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "deptFilterError", $"alert('Error applying department filter: {ex.Message}');", true);
            }
        }
        private void FilterDataByStatus(string status)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM Accounts WHERE 1=1";

                List<string> userPermissions = GetUserPermissionsPageLoad(Session["UserID"].ToString());

                // If user has permission to create users for all departments, do not filter by department
                if (userPermissions.Contains("Create new users for all departments"))
                {
                    // No additional filtering needed
                }
                else if (userPermissions.Contains("Create new users in their department"))
                {
                    string userDepartment = GetUserDepartment(Session["UserID"].ToString());
                    query += " AND Department = @Department";
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query += " AND Status = @Status";
                }

                query += " ORDER BY UserID DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (userPermissions.Contains("Create new users in their department"))
                    {
                        string userDepartment = GetUserDepartment(Session["UserID"].ToString());
                        cmd.Parameters.AddWithValue("@Department", userDepartment);
                    }

                    if (!string.IsNullOrEmpty(status))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView1.DataSource = dataSet.Tables[0];
                            GridView1.DataBind();
                        }
                        else
                        {
                            GridView1.DataSource = null;
                            GridView1.DataBind();
                        }
                        BindPagination();
                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridView1.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPages.Text = totalPages.ToString();
                    }
                }
            }
        }
        protected void statusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> userPermissions = GetUserPermissionsPageLoad(Session["UserID"].ToString());

            string selectedStatus = statusFilter.SelectedValue;
            ViewState["SelectedStatus"] = selectedStatus;

            // Check if department filter is set to default value
            string selectedDepartment = ViewState["SelectedDepartment"]?.ToString();

            if (userPermissions.Contains("Create new users for all departments"))
            {
                if (perUser.Checked)
                {
                    // User has permission to create users for all departments
                    if (selectedStatus == "" && selectedDepartment == "")
                    {
                        // No filters applied, display all data
                        BindGridView();
                    }
                    else if (selectedStatus != "")
                    {
                        // Status filter selected, check department filter
                        if (selectedDepartment != "")
                        {
                            FilterDataByDepartmentAndStatus(selectedDepartment, selectedStatus);
                        }
                        else
                        {
                            // Only status filter applied
                            FilterDataByStatus(selectedStatus);
                        }
                    }
                    else
                    {
                        // Status filter set to default value, department filter may have a value
                        FilterDataByDepartment(selectedDepartment);
                    }
                }
                else if (byPosition.Checked)
                {
                    // User has permission to create users for all departments
                    if (selectedStatus == "" && selectedDepartment == "")
                    {
                        // No filters applied, display all data
                        BindGridViewByPosition();
                    }
                    else if (selectedStatus != "")
                    {
                        // Status filter selected, check department filter
                        if (selectedDepartment != "")
                        {
                            FilterDataByDepartmentAndStatusByPosition(selectedDepartment, selectedStatus);
                        }
                        else
                        {
                            // Only status filter applied
                            FilterDataByStatusByPosition(selectedStatus);
                        }
                    }
                    else
                    {
                        // Status filter set to default value, department filter may have a value
                        FilterDataByDepartmentByPosition(selectedDepartment);
                    }
                }
                
            }
            else if (userPermissions.Contains("Create new users in their department"))
            {
                if (perUser.Checked)
                {
                    // User only has permission to create users in their department
                    FilterDataByStatusAndDepartment(selectedStatus);
                }
                else if (byPosition.Checked)
                {
                    // User only has permission to create users in their department
                    FilterDataByStatusAndDepartmentByPosition(selectedStatus);
                }
                
            }

            GridView1.PageIndex = 0;
            GridViewByPosition.PageIndex = 0;
        }   
        private void FilterDataByStatusAndDepartment(string status) // Ddl status function for users permission "Create new users in their department" only
        {
            string userDepartment = GetUserDepartment(Session["UserID"].ToString());

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM Accounts WHERE Department = @Department";

                if (!string.IsNullOrEmpty(status))
                {
                    query += " AND Status = @Status";
                }

                query += " ORDER BY UserID DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Department", userDepartment);

                    if (!string.IsNullOrEmpty(status))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView1.DataSource = dataSet.Tables[0];
                            GridView1.DataBind();
                        }
                        else
                        {
                            GridView1.DataSource = null;
                            GridView1.DataBind();
                        }
                        BindPagination();
                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridView1.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPages.Text = totalPages.ToString();
                    }
                }
            }
        }        
        protected void resetfilterBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("../PAGES/AccountSettings.aspx");
        }

        // DEPT AND STATUS FILTER FUNCTION BELOW - BY POSITION
        private void FilterDataByDepartmentByPosition(string department)
        {
            // Reset search text
            ViewState["SearchText"] = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    string query = "SELECT * FROM Accounts";

                    // Only apply department filter if user has permission to create users for all departments
                    List<string> userPermissions = GetUserPermissionsPageLoad(Session["UserID"].ToString());
                    if (!userPermissions.Contains("Create new users for all departments"))
                    {
                        department = ""; // Clear department filter if user doesn't have permission
                    }

                    if (!string.IsNullOrEmpty(department))
                    {
                        query += " WHERE Department = @Department";
                    }

                    query += " ORDER BY UserID DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(department))
                        {
                            cmd.Parameters.AddWithValue("@Department", department);
                        }

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataSet dataSet = new DataSet();
                            adapter.Fill(dataSet);

                            if (dataSet.Tables[0].Rows.Count > 0)
                            {
                                GridViewByPosition.DataSource = dataSet.Tables[0];
                                GridViewByPosition.DataBind();
                                BindPaginationByPosition();
                            }
                            else
                            {
                                GridViewByPosition.DataSource = null;
                                GridViewByPosition.DataBind();
                                BindPaginationByPosition();
                            }
                            int totalRows = dataSet.Tables[0].Rows.Count;
                            int pageSize = GridViewByPosition.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                            lblTotalPagesPosition.Text = totalPages.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "deptFilterError", $"alert('Error filtering data by department: {ex.Message}');", true);
            }
        }
        private void FilterDataByDepartmentAndStatusByPosition(string department, string status)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM Accounts WHERE 1=1";

                List<string> userPermissions = GetUserPermissionsPageLoad(Session["UserID"].ToString());

                // If user has permission to create users for all departments, do not filter by department
                if (userPermissions.Contains("Create new users for all departments"))
                {
                    if (!string.IsNullOrEmpty(department))
                    {
                        query += " AND Department = @Department";
                    }

                    if (!string.IsNullOrEmpty(status))
                    {
                        query += " AND Status = @Status";
                    }
                }

                query += " ORDER BY UserID DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(department))
                    {
                        cmd.Parameters.AddWithValue("@Department", department);
                    }

                    if (!string.IsNullOrEmpty(status))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridViewByPosition.DataSource = dataSet.Tables[0];
                            GridViewByPosition.DataBind();
                        }
                        else
                        {
                            GridViewByPosition.DataSource = null;
                            GridViewByPosition.DataBind();
                        }
                        BindPaginationByPosition();
                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridViewByPosition.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPagesPosition.Text = totalPages.ToString();
                    }
                }
            }
        }
        private void FilterDataByStatusByPosition(string status)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM Accounts WHERE 1=1";

                List<string> userPermissions = GetUserPermissionsPageLoad(Session["UserID"].ToString());

                // If user has permission to create users for all departments, do not filter by department
                if (userPermissions.Contains("Create new users for all departments"))
                {
                    // No additional filtering needed
                }
                else if (userPermissions.Contains("Create new users in their department"))
                {
                    string userDepartment = GetUserDepartment(Session["UserID"].ToString());
                    query += " AND Department = @Department";
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query += " AND Status = @Status";
                }

                query += " ORDER BY UserID DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (userPermissions.Contains("Create new users in their department"))
                    {
                        string userDepartment = GetUserDepartment(Session["UserID"].ToString());
                        cmd.Parameters.AddWithValue("@Department", userDepartment);
                    }

                    if (!string.IsNullOrEmpty(status))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridViewByPosition.DataSource = dataSet.Tables[0];
                            GridViewByPosition.DataBind();
                        }
                        else
                        {
                            GridViewByPosition.DataSource = null;
                            GridViewByPosition.DataBind();
                        }
                        BindPagination();
                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridViewByPosition.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPagesPosition.Text = totalPages.ToString();
                    }
                }
            }
        }
        private void FilterDataByStatusAndDepartmentByPosition(string status) // Ddl status function for users permission "Create new users in their department" only
        {
            string userDepartment = GetUserDepartment(Session["UserID"].ToString());

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM Accounts WHERE Department = @Department";

                if (!string.IsNullOrEmpty(status))
                {
                    query += " AND Status = @Status";
                }

                query += " ORDER BY UserID DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Department", userDepartment);

                    if (!string.IsNullOrEmpty(status))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridViewByPosition.DataSource = dataSet.Tables[0];
                            GridViewByPosition.DataBind();
                        }
                        else
                        {
                            GridViewByPosition.DataSource = null;
                            GridViewByPosition.DataBind();
                        }
                        BindPagination();
                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridViewByPosition.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPagesPosition.Text = totalPages.ToString();
                    }
                }
            }
        }
        
        // TABLE LNK FUNCTION (Per User & By Position
        protected void lnkPage_Click(object sender, EventArgs e)
        {
            LinkButton lnkPage = (LinkButton)sender;
            if (int.TryParse(lnkPage.Text, out int pageIndex))
            {
                GridView1.PageIndex = pageIndex - 1;
                GridViewByPosition.PageIndex = pageIndex - 1;

                // Check if any filters are applied
                string selectedDepartment = ViewState["SelectedDepartment"]?.ToString();
                string selectedStatus = ViewState["SelectedStatus"]?.ToString();
                string searchText = ViewState["SearchText"]?.ToString();

                if (!string.IsNullOrEmpty(searchText))
                {
                    if (perUser.Checked)
                    {
                        SearchData(searchText);
                    }
                    else if (byPosition.Checked)
                    {
                        SearchDataByPosition(searchText);
                    }
                }
                else if (!string.IsNullOrEmpty(selectedDepartment) || !string.IsNullOrEmpty(selectedStatus))
                {
                    if (perUser.Checked)
                    {
                        FilterDataByDepartmentAndStatus(selectedDepartment, selectedStatus);
                    }
                    else if (byPosition.Checked)
                    {
                        FilterDataByDepartmentAndStatusByPosition(selectedDepartment, selectedStatus);
                    }
                }
                else if (perUser.Checked)
                {
                    BindGridView();
                }
                else if (byPosition.Checked)
                {
                    BindGridViewByPosition();
                }

                BindPagination();
            }
            else
            {
                // Handle the case where parsing fails, perhaps with logging or error handling
            }
        }
        // TABLE PAGE FUNCTION BELOW - PER USER
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
        protected void btnPrev_Click(object sender, EventArgs e)
        {
            if (GridView1.PageIndex > 0)
            {
                GridView1.PageIndex -= 1;

                // Check if any filters are applied
                string selectedDepartment = ViewState["SelectedDepartment"]?.ToString();
                string selectedStatus = ViewState["SelectedStatus"]?.ToString();
                string searchText = ViewState["SearchText"]?.ToString();

                if (!string.IsNullOrEmpty(searchText))
                {
                    SearchData(searchText);
                }
                else if (!string.IsNullOrEmpty(selectedDepartment) || !string.IsNullOrEmpty(selectedStatus))
                {
                    FilterDataByDepartmentAndStatus(selectedDepartment, selectedStatus);
                }
                else
                {
                    BindGridView();
                }

                BindPagination();
            }
        }
        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (GridView1.PageIndex < GridView1.PageCount - 1)
            {
                GridView1.PageIndex += 1;

                // Check if any filters are applied
                string selectedDepartment = ViewState["SelectedDepartment"]?.ToString();
                string selectedStatus = ViewState["SelectedStatus"]?.ToString();
                string searchText = ViewState["SearchText"]?.ToString();

                if (!string.IsNullOrEmpty(searchText))
                {
                    SearchData(searchText);
                }
                else if (!string.IsNullOrEmpty(selectedDepartment) || !string.IsNullOrEmpty(selectedStatus))
                {
                    FilterDataByDepartmentAndStatus(selectedDepartment, selectedStatus);
                }
                else
                {
                    BindGridView();
                }

                BindPagination();
            }
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
                    BindGridView();

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
        private string GetUserDepartment(string userId)
        {
            string department = "";
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                string query = "SELECT Department FROM Accounts WHERE UserID = @UserID";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    conn.Open();
                    department = cmd.ExecuteScalar()?.ToString();
                }
            }
            return department;
        }

        // TABLE PAGE INDEX FUNCTION - BY POSITION
        private void UpdatePageIndexTextBoxPosition()
        {
            lblPageNumPosition.Text = (GridViewByPosition.PageIndex + 1).ToString();
        }
        private void BindPaginationByPosition()
        {
            int pageCountPosition = GridViewByPosition.PageCount;
            int currentPagePosition = GridViewByPosition.PageIndex;

            btnPrevPosition.Visible = currentPagePosition > 0;
            btnNextPosition.Visible = currentPagePosition < (pageCountPosition - 1);

            // Update the txtboxPageNum with the current page index
            UpdatePageIndexTextBoxPosition();
        }
        protected void btnPrevPosition_Click(object sender, EventArgs e)
        {
            if (GridViewByPosition.PageIndex > 0)
            {
                GridViewByPosition.PageIndex -= 1;

                // Check if any filters are applied
                string selectedDepartment = ViewState["SelectedDepartment"]?.ToString();
                string selectedStatus = ViewState["SelectedStatus"]?.ToString();
                string searchText = ViewState["SearchText"]?.ToString();

                if (!string.IsNullOrEmpty(searchText))
                {
                    SearchDataByPosition(searchText);
                }
                else if (!string.IsNullOrEmpty(selectedDepartment) || !string.IsNullOrEmpty(selectedStatus))
                {
                    FilterDataByDepartmentAndStatusByPosition(selectedDepartment, selectedStatus);
                }
                else
                {
                    BindGridViewByPosition();
                }

                BindPaginationByPosition();
            }
        }
        protected void btnNextPosition_Click(object sender, EventArgs e)
        {
            if (GridViewByPosition.PageIndex < GridViewByPosition.PageCount - 1)
            {
                GridViewByPosition.PageIndex += 1;

                // Check if any filters are applied
                string selectedDepartment = ViewState["SelectedDepartment"]?.ToString();
                string selectedStatus = ViewState["SelectedStatus"]?.ToString();
                string searchText = ViewState["SearchText"]?.ToString();

                if (!string.IsNullOrEmpty(searchText))
                {
                    SearchDataByPosition(searchText);
                }
                else if (!string.IsNullOrEmpty(selectedDepartment) || !string.IsNullOrEmpty(selectedStatus))
                {
                    FilterDataByDepartmentAndStatusByPosition(selectedDepartment, selectedStatus);
                }
                else
                {
                    BindGridViewByPosition();
                }

                BindPaginationByPosition();
            }
        }
        protected void txtPageNumberPosition_TextChanged(object sender, EventArgs e)
        {
            int pageNumber;
            if (int.TryParse(txtPageNumberPosition.Text, out pageNumber))
            {
                if (pageNumber >= 1 && pageNumber <= GridViewByPosition.PageCount)
                {
                    // Set the PageIndex of the GridView to the entered page number
                    GridViewByPosition.PageIndex = pageNumber - 1;

                    // Re-bind the GridView with the data for the specified page
                    BindGridViewByPosition();

                    // Update pagination controls
                    BindPaginationByPosition();

                    // Clear the entered page number
                    txtPageNumberPosition.Text = "";
                }
                else
                {
                    // The page index is out of range, so reset to the first page
                    GridViewByPosition.PageIndex = 0;
                    BindGridViewByPosition();
                    BindPaginationByPosition();
                    UpdatePageIndexTextBoxPosition();
                    txtPageNumberPosition.Text = "";
                    // Display error message for invalid page number using JavaScript alert
                    ScriptManager.RegisterStartupScript(this, GetType(), "InvalidPageNumber", "alert('Invalid page number. Please enter a number between 1 and " + GridView1.PageCount + ".');", true);
                }
            }
            else
            {
                // The page index is out of range, so reset to the first page
                GridViewByPosition.PageIndex = 0;
                BindGridViewByPosition();
                BindPaginationByPosition();
                UpdatePageIndexTextBoxPosition();
                txtPageNumberPosition.Text = "";
                // Display error message for non-numeric input using JavaScript alert
                ScriptManager.RegisterStartupScript(this, GetType(), "InvalidInput", "alert('Please enter a valid number.');", true);
            }
        }


        // EDIT USER INFO FUNCTION BELOW
        protected bool ShouldShowEditButton(object userIdObj)
        {
            if (userIdObj == null || Session["UserID"] == null)
            {
                return false;
            }

            string userId = userIdObj.ToString();
            string currentUserId = Session["UserID"].ToString();
            List<string> userPermissions = GetUserPermissionsPageLoad(currentUserId);

            bool CreateUsersALL = userPermissions.Contains("Create new users for all departments");
            bool EditUsersALL = userPermissions.Contains("Edit user information in all department");

            bool CreateUsersDEPT = userPermissions.Contains("Create new users in their department");
            bool EditUsersDEPT = userPermissions.Contains("Edit user information for users within their department");

            // Check if the current user can edit all users' info but not users in their department
            if (CreateUsersALL && EditUsersALL && CreateUsersDEPT && !EditUsersDEPT)
            {
                return true; // Allow user to edit their own info and others' info
            }

            // Check if the current user can NOT edit all users' info and edit users in their department
            if (CreateUsersALL && !EditUsersALL && CreateUsersDEPT && !EditUsersDEPT)
            {
                return false; // hide the edit button
            }

            // Check if the current user have all permissions
            if (CreateUsersALL && EditUsersALL && CreateUsersDEPT && EditUsersDEPT)
            {
                return true; // show the edit button
            }

            // Check if the current user have only
            if (!CreateUsersALL && !EditUsersALL && CreateUsersDEPT && !EditUsersDEPT)
            {
                return false; // hide the edit button
            }

            // Only show edit button for other users' info in the same department
            if (CreateUsersALL && CreateUsersDEPT && EditUsersDEPT)
            {
                // Check if the user whose edit button is being checked is in the same department
                if (IsUserInSameDepartment(currentUserId, userId))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true; // Show edit button for other cases
        }
        protected bool IsUserInSameDepartment(string currentUserId, string userId)
        {
            string departmentQuery = @"
        SELECT a.Department AS CurrentUserDepartment, b.Department AS UserDepartment
        FROM Accounts a
        JOIN Accounts b ON a.UserID = @currentUserId AND b.UserID = @userId";

            string currentUserIdDepartment = null;
            string userIdDepartment = null;

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(departmentQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@currentUserId", currentUserId);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            currentUserIdDepartment = reader["CurrentUserDepartment"].ToString();
                            userIdDepartment = reader["UserDepartment"].ToString();
                        }
                    }
                }
            }

            // Compare departments
            return currentUserIdDepartment == userIdDepartment;
        }
        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            string currentUser = Session["UserID"].ToString(); // current logged in user
            string userID = getUserID.Value;
            string updateName = txtboxEditName.Text;
            string updateEmail = txtboxEditEmail.Text;
            string updateContact = txtboxEditContact.Text;
            string updateDept = ddlEditDept.SelectedValue;
            string updateStatus = ddlEditStatus.SelectedValue;
            string updatePosition = txtboxEditPosition.Text;

            // Validate input fields
            if (string.IsNullOrWhiteSpace(updateName) || string.IsNullOrWhiteSpace(updateEmail) || string.IsNullOrWhiteSpace(updateContact) || updateDept == "-1" || string.IsNullOrWhiteSpace(updatePosition))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlertModal", "showErrorAlertModal('Please fill in all required fields.');", true);
                return; // Prevent further execution
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string selectQuery = "SELECT Username, Password, Access FROM accounts WHERE UserID = @userid";
                    MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);
                    selectCommand.Parameters.AddWithValue("@userid", userID);

                    using (MySqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string existingUsername = reader["Username"].ToString();
                            string existingPass = reader["Password"].ToString();
                            string existingAccess = reader["Access"].ToString();
                        }
                    }

                    string updateQuery = "UPDATE accounts SET " +
                        "Name = @name, " +
                        "Email = @email, " +
                        "Contact = @contact, " +
                        "Department = @dept, " +
                        "Status = @status, " +
                        "Position = @position " +
                        "WHERE UserID = @userid";

                    MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@userid", userID);
                    updateCommand.Parameters.AddWithValue("@name", updateName);
                    updateCommand.Parameters.AddWithValue("@email", updateEmail);
                    updateCommand.Parameters.AddWithValue("@contact", updateContact);
                    updateCommand.Parameters.AddWithValue("@dept", updateDept);
                    updateCommand.Parameters.AddWithValue("@status", updateStatus);
                    updateCommand.Parameters.AddWithValue("@position", updatePosition);

                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        LogAudit(currentUser, "Updated Account", "Successful");
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessEditAlert", "showSuccessEditAlert('Updated Account Successfully!');", true);
                    }
                    else
                    {
                        LogAudit(currentUser, "Updated Account", "Unsuccessful");
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorEditAlert", "showErrorEditAlert('Updated Account Failed');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorEditAlert", $"showErrorEditAlert('{ex.Message}');", true);
            }
            finally
            {
                BindGridView();
            }
        }
        protected void PopulateUpdateDepartmentDropdown()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    string query = "SELECT DepartmentName, ShortAcronym FROM department";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlEditDept.Items.Clear();
                            ddlEditDept.Items.Add(new ListItem("Select Department", ""));

                            while (reader.Read())
                            {
                                string departmentName = reader["DepartmentName"].ToString();
                                string shortAcronym = reader["ShortAcronym"].ToString();
                                ddlEditDept.Items.Add(new ListItem(departmentName, shortAcronym));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "deptDropdownError", $"alert('Error loading department dropdown: {ex.Message}');", true);
            }
        }


        // CHECKLIST - EDIT GRANT ACCESS FUNCTION BELOW - PER USER
        protected void lnkEditAccess_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string userID = btn.CommandArgument;

            // Ensure current user has permission to edit access
            string currentUserID = Session["UserID"].ToString();
            List<string> currentUserPermissions = GetUserPermissionsPageLoad(currentUserID);
            bool canEditUserAccess = currentUserPermissions.Contains("Edit access of other users");

            if (!canEditUserAccess)
            {
                // Handle access denied scenario (optional)
                Response.Redirect("../PAGES/AccessDenied.aspx");
                return;
            }

            // Continue with edit access functionality
            hiddenUserID.Value = userID;
            hiddenUserName.Value = GetUserName(userID);
            lblName.Text = hiddenUserName.Value;

            // Get the current permissions from the database
            List<int> currentPermissions = GetUserPermissions(userID);

            // Update the CheckBoxLists based on current permissions
            UpdateCheckBoxList(CheckBoxListEditBasic, currentPermissions);
            UpdateCheckBoxList(CheckBoxListEditAdvanced, currentPermissions);
            UpdateCheckBoxList(CheckBoxListEditMaster, currentPermissions);

            // Display the modal for editing access
            ScriptManager.RegisterStartupScript(this, GetType(), "grantAccessModal", "showEditAccessModal();", true);
        }
        private void UpdateCheckBoxList(CheckBoxList checkBoxList, List<int> currentPermissions)
        {
            foreach (ListItem item in checkBoxList.Items)
            {
                item.Selected = currentPermissions.Contains(int.Parse(item.Value));
            }
        }
        private List<int> GetUserPermissions(string userID)
        {
            List<int> permissions = new List<int>();
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT PermissionID FROM User_Permissions WHERE UserID = @UserID";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            permissions.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            return permissions;
        }
        protected void submitEditAccess_Click(object sender, EventArgs e)
        {
            string currentUser = Session["UserID"].ToString();
            string userID = hiddenUserID.Value;
            string userName = hiddenUserName.Value;

            // Retrieve the updated permissions from form inputs (checkboxes)
            string updatedPermissions = GetUpdatedPermissions();

            try
            {
                if (perUser.Checked)
                {
                    // Update the permissions in the database
                    UpdateUserPermissions(userID, userName, updatedPermissions);

                    // Refresh the grid view
                    BindGridView();

                    // Update session variable with new access level
                    Session["Access"] = updatedPermissions;
                    // Update session variable with new permissions
                    UpdateSessionUserPermissions(userID);

                    // Update filter dropdown visibility based on updated permissions
                    PopulateFilterDepartmentDropdown();

                    LogAudit(currentUser, "Updated Access", "Successful");
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Updated Access Successfully!');", true);
                }
                else if (byPosition.Checked)
                {
                    // Update the permissions for the selected user
                    UpdateUserPermissions(userID, userName, updatedPermissions);

                    // Get users with the same position
                    List<string> usersByPosition = GetUsersByPosition(userID);

                    // Update permissions for users with the same position
                    foreach (string otherUserID in usersByPosition)
                    {
                        string otherUserName = GetUserName(otherUserID);
                        UpdateUserPermissions(otherUserID, otherUserName, updatedPermissions);
                    }

                    // Refresh the grid view
                    BindGridViewByPosition();

                    // Update session variable with new access level
                    Session["Access"] = updatedPermissions;
                    // Update session variable with new permissions
                    UpdateSessionUserPermissions(userID);

                    // Update filter dropdown visibility based on updated permissions
                    PopulateFilterDepartmentDropdown();

                    LogAudit(currentUser, "Updated Access", "Successful");
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Updated Access By Position Successfully!');", true);
                }
            }
            catch (Exception ex)
            {
                LogAudit(currentUser, "Updated Access", "Unsuccessful");
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('{ex.Message}');", true);
            }
        }
        private List<string> GetUsersByPosition(string userID)
        {
            List<string> users = new List<string>();
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = @"
                SELECT a.UserID
                FROM Accounts a
                JOIN (
                    SELECT Position
                    FROM Accounts
                    WHERE UserID = @UserID
                ) p ON a.Position = p.Position
                WHERE a.UserID <> @UserID";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return users;
        }
        private void UpdateSessionUserPermissions(string userID)
        {
            // Get and update user permissions in session variable
            List<int> userPermissionIDs = GetUserPermissionIDs(userID);
            List<string> userPermissions = GetUserPermissions(userPermissionIDs);
            Session["UserPermissions"] = userPermissions;
        }
        private List<int> GetUserPermissionIDs(string userID)
        {
            List<int> permissions = new List<int>();
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT PermissionID FROM User_Permissions WHERE UserID = @UserID";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            permissions.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            return permissions;
        }
        private List<string> GetUserPermissions(List<int> permissionIDs)
        {
            List<string> permissions = new List<string>();
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT PermissionName FROM Permissions WHERE PermissionID IN (" + string.Join(",", permissionIDs) + ")";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            permissions.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return permissions;
        }
        private void UpdateUserPermissions(string userID, string userName, string updatedPermissions)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update the "Access" column in the "Accounts" table
                        string updateAccessQuery = "UPDATE Accounts SET Access = @Access WHERE UserID = @UserID";
                        using (MySqlCommand updateAccessCmd = new MySqlCommand(updateAccessQuery, connection, transaction))
                        {
                            updateAccessCmd.Parameters.AddWithValue("@Access", updatedPermissions);
                            updateAccessCmd.Parameters.AddWithValue("@UserID", userID);
                            updateAccessCmd.ExecuteNonQuery();
                        }

                        // Delete existing permissions for the user
                        string deleteQuery = "DELETE FROM User_Permissions WHERE UserID = @UserID";
                        using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, connection, transaction))
                        {
                            deleteCmd.Parameters.AddWithValue("@UserID", userID);
                            deleteCmd.ExecuteNonQuery();
                        }

                        // Insert new permissions for the user
                        if (!string.IsNullOrEmpty(updatedPermissions))
                        {
                            string[] permissionsArray = updatedPermissions.Split(',');

                            foreach (string permissionID in permissionsArray)
                            {
                                string insertQuery = "INSERT INTO User_Permissions (UserID, Name, PermissionID, DepartmentID) VALUES (@UserID, @Name, @PermissionID, @DepartmentID)";
                                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection, transaction))
                                {
                                    insertCmd.Parameters.AddWithValue("@UserID", userID);
                                    insertCmd.Parameters.AddWithValue("@Name", userName);
                                    insertCmd.Parameters.AddWithValue("@PermissionID", int.Parse(permissionID));
                                    insertCmd.Parameters.AddWithValue("@DepartmentID", GetDepartmentID(userID));
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if an exception occurs
                        transaction.Rollback();

                        // Throw the exception
                        throw new Exception("Error updating user permissions: " + ex.Message, ex);
                    }
                }
            }
        }
        private string GetUpdatedPermissions()
        {
            List<int> selectedPermissions = new List<int>();

            foreach (ListItem item in CheckBoxListEditBasic.Items)
            {
                if (item.Selected)
                    selectedPermissions.Add(int.Parse(item.Value));
            }

            foreach (ListItem item in CheckBoxListEditAdvanced.Items)
            {
                if (item.Selected)
                    selectedPermissions.Add(int.Parse(item.Value));
            }

            foreach (ListItem item in CheckBoxListEditMaster.Items)
            {
                if (item.Selected)
                    selectedPermissions.Add(int.Parse(item.Value));
            }

            // Convert the list of integers to a comma-separated string
            return string.Join(",", selectedPermissions);
        }
        private string GetDepartmentID(string userID)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = @"
    SELECT d.DepartmentID
    FROM Accounts a
    JOIN department d ON a.Department = d.ShortAcronym
    WHERE a.UserID = @UserID";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    return command.ExecuteScalar()?.ToString() ?? "";
                }
            }
        }
        private string GetUserName(string userID)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT Name FROM Accounts WHERE UserID = @UserID";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    return command.ExecuteScalar()?.ToString() ?? "";
                }
            }
        }

        //CHECKLIST - ADD USER FUNCTION BELOW
        protected void submitBtn_Click(object sender, EventArgs e)
        {
            string currentUserID = Session["UserID"].ToString();
            // Retrieve user information from form inputs
            string name = nameTextbox.Text;
            string username = unameTextBox.Text;
            string email = emailTextbox.Text;
            string contact = contactTextBox.Text;
            string department = deptDropdown.SelectedValue;
            string position = positionTextBox.Text;
            string access = GetAccessFromCheckboxes();

            if (string.IsNullOrWhiteSpace(nameTextbox.Text) ||
            string.IsNullOrWhiteSpace(unameTextBox.Text) ||
            string.IsNullOrWhiteSpace(emailTextbox.Text) ||
            string.IsNullOrWhiteSpace(contactTextBox.Text) ||
            deptDropdown.SelectedValue == "-1" ||
            string.IsNullOrWhiteSpace(positionTextBox.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlertAddUserModal", "showErrorAlertAddUserModal('Please fill in all required fields.');", true);
                return; // Prevent further execution
            }

            // Insert user into Accounts table
            string insertQuery = "INSERT INTO Accounts (Name, Username, Password, Department, Position, Email, Contact, Status, Access) " +
                                 "VALUES (@Name, @Username, @Password, @Department, @Position, @Email, @Contact, @Status, @Access)";

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", "changeme"); // default pass
                    command.Parameters.AddWithValue("@Department", department);
                    command.Parameters.AddWithValue("@Position", position);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Contact", contact);
                    command.Parameters.AddWithValue("@Status", "Active"); // Set auto active status
                    command.Parameters.AddWithValue("@Access", access);

                    command.ExecuteNonQuery();

                    // Get the new UserID
                    string newUserIDQuery = "SELECT UserID FROM Accounts WHERE Username = @Username";
                    using (MySqlCommand newUserIDCommand = new MySqlCommand(newUserIDQuery, connection))
                    {
                        newUserIDCommand.Parameters.AddWithValue("@Username", username);
                        string newUserID = newUserIDCommand.ExecuteScalar().ToString();

                        // Insert permissions for the new user
                        InsertUserPermissions(newUserID, name);
                    }

                    BindGridView();
                    LogAudit(currentUserID, "Account Added", "Successful");
                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlertAddUserModal", "showSuccessAlertAddUserModal('Account was Successfully Added!')", true);
                }
            }
        }
        // Function to get access rights based on selected checkboxes
        private string GetAccessFromCheckboxes()
        {
            string access = "";

            foreach (ListItem item in CheckBoxListBasic.Items)
            {
                if (item.Selected)
                    access += GetPermissionID(item.Text) + ",";
            }
            foreach (ListItem item in CheckBoxListAdvanced.Items)
            {
                if (item.Selected)
                    access += GetPermissionID(item.Text) + ",";
            }
            foreach (ListItem item in CheckBoxListMaster.Items)
            {
                if (item.Selected)
                    access += GetPermissionID(item.Text) + ",";
            }

            // Remove the trailing comma if access string is not empty
            if (!string.IsNullOrEmpty(access))
                access = access.Remove(access.Length - 1);

            return access;
        }
        // Method to get PermissionID based on PermissionName
        private string GetPermissionID(string permissionName)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "SELECT PermissionID FROM Permissions WHERE PermissionName = @PermissionName";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PermissionName", permissionName);
                    return command.ExecuteScalar().ToString();
                }
            }
        }
        // Function to insert user permissions into User_Permissions table
        private void InsertUserPermissions(string userID, string userName)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                connection.Open();

                foreach (ListItem item in CheckBoxListBasic.Items)
                {
                    if (item.Selected)
                        InsertUserPermission(connection, userID, userName, item.Text);
                }

                foreach (ListItem item in CheckBoxListAdvanced.Items)
                {
                    if (item.Selected)
                        InsertUserPermission(connection, userID, userName, item.Text);
                }

                foreach (ListItem item in CheckBoxListMaster.Items)
                {
                    if (item.Selected)
                        InsertUserPermission(connection, userID, userName, item.Text);
                }
            }
        }
        // Function to insert a single and multiple permission into User_Permissions table
        private void InsertUserPermission(MySqlConnection connection, string userID, string userName, string permissionName)
        {
            string insertPermissionQuery = "INSERT INTO User_Permissions (UserID, Name, PermissionID, DepartmentID) " +
                "VALUES (@UserID, @Name, @PermissionID, " +
                "(SELECT DepartmentID FROM department WHERE DepartmentName = @DepartmentName))";

            try
            {
                // Open the connection if it's not already open
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                // Create a command for executing SQL queries
                using (MySqlCommand command = new MySqlCommand(insertPermissionQuery, connection))
                {
                    // Split comma-separated permissions
                    string[] selectedPermissions = permissionName.Split(',');

                    // Loop through each permission
                    foreach (string permission in selectedPermissions)
                    {
                        // Trim to remove leading/trailing spaces
                        string trimmedPermission = permission.Trim();

                        // Get PermissionID based on PermissionName
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@UserID", userID);
                        command.Parameters.AddWithValue("@Name", userName);
                        command.Parameters.AddWithValue("@DepartmentName", deptDropdown.SelectedItem.Text);
                        command.Parameters.AddWithValue("@PermissionName", trimmedPermission);

                        // Retrieve PermissionID
                        string permissionIDQuery = "SELECT PermissionID FROM Permissions WHERE PermissionName = @PermissionName";
                        using (MySqlCommand permissionIDCommand = new MySqlCommand(permissionIDQuery, connection))
                        {
                            permissionIDCommand.Parameters.AddWithValue("@PermissionName", trimmedPermission);
                            object permissionIDObj = permissionIDCommand.ExecuteScalar();

                            if (permissionIDObj != null && permissionIDObj != DBNull.Value)
                            {
                                int permissionID = Convert.ToInt32(permissionIDObj);
                                command.Parameters.AddWithValue("@PermissionID", permissionID);

                                // Execute the command to insert into User_Permissions table
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                throw new Exception($"PermissionID is null for PermissionName: {trimmedPermission}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                throw new Exception("Error inserting user permissions: " + ex.Message, ex);
            }
            finally
            {
                // Always close the connection after use
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
        }
       


        // Populate CheckBoxLists for different access levels
        private void BindCheckBoxLists()
        {
            BindCheckBoxList("Basic", CheckBoxListBasic);
            BindCheckBoxList("Advanced", CheckBoxListAdvanced);
            BindCheckBoxList("Master", CheckBoxListMaster);

            BindCheckBoxList("Basic", CheckBoxListEditBasic);
            BindCheckBoxList("Advanced", CheckBoxListEditAdvanced);
            BindCheckBoxList("Master", CheckBoxListEditMaster);
        }
        private void BindCheckBoxList(string accessLevel, CheckBoxList checkBoxList)
        {
            string query = "SELECT PermissionID, PermissionName FROM Permissions WHERE AccessLevel = @AccessLevel";

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AccessLevel", accessLevel);
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string permissionName = reader["PermissionName"].ToString();
                            string permissionID = reader["PermissionID"].ToString();
                            ListItem listItem = new ListItem(permissionName, permissionID);

                            // Automatically check items if it's the Basic CheckBoxList
                            if (accessLevel == "Basic")
                            {
                                listItem.Selected = true;
                            }

                            checkBoxList.Items.Add(listItem);
                        }
                    }
                }
            }
        }
        protected void PopulateDepartmentDropdown()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    string query = "SELECT DepartmentName, ShortAcronym FROM department";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Clear existing items
                            deptDropdown.Items.Clear();

                            // Add the default option
                            deptDropdown.Items.Add(new ListItem("Filter by Department", ""));

                            // Add each department name to the dropdown list
                            while (reader.Read())
                            {
                                string departmentName = reader["DepartmentName"].ToString();
                                string shortAcronym = reader["ShortAcronym"].ToString();
                                deptDropdown.Items.Add(new ListItem(departmentName, shortAcronym));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "deptDropdownError", $"alert('Error loading department dropdown: {ex.Message}');", true);
            }
        }
        private string GetConnectionString()
        {
            string server = "localhost";
            string database = "documentsystem";
            string username = "root";
            string password = "";

            return $"server={server};database={database};uid={username};pwd={password};";
        }
        private void LogAudit(string userid, string activity, string status)
        {
            string connectionString = GetConnectionString();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string auditQuery = "INSERT INTO auditlogs (UserID, UserLogDateTime, Activity, Status) VALUES (@UserID, NOW(), @Activity, @Status)";
                    using (MySqlCommand auditCmd = new MySqlCommand(auditQuery, connection))
                    {
                        auditCmd.Parameters.AddWithValue("@UserID", userid);
                        auditCmd.Parameters.AddWithValue("@Activity", activity);
                        auditCmd.Parameters.AddWithValue("@Status", status);

                        auditCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "auditError", $"alert('Audit log error: {ex.Message}');", true);
            }
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Find the index of the "Status" column dynamically
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
                    // Apply color formatting based on status value
                    string status = e.Row.Cells[statusIndex].Text.Trim();
                    if (status.Equals("active", StringComparison.OrdinalIgnoreCase))
                    {
                        e.Row.Cells[statusIndex].ForeColor = System.Drawing.Color.Green;
                    }
                    else if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase))
                    {
                        e.Row.Cells[statusIndex].ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
        }
    }
}