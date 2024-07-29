using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Controls;

namespace DMS.PAGES
{
    public partial class DepartmentSettings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Name"] == null)
            {
                // Redirect to login page if session variable is not set
                Response.Redirect("../LOGIN/Login.aspx");
            }

            // Check if the user has the "Create new department" permission
            List<string> userPermissions = Session["UserPermissions"] as List<string>;
            if (userPermissions == null || !userPermissions.Contains("Create new department"))
            {
                // Redirect to Access Denied page if the user does not have the permission
                Response.Redirect("../PAGES/AccessDenied.aspx");
            }

            if (!IsPostBack)
            {
                BindGridView2();
                // Store userPermissions in ViewState to make it accessible
                ViewState["UserPermissions"] = userPermissions;
            }
            BindPagination();
            UpdatePageIndexTextBox();
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

        // SEARCH FUNCTION BELOW
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
                string selectedStatus = ViewState["SelectedStatus"]?.ToString();

                if (!string.IsNullOrEmpty(selectedStatus))
                {
                    FilterDataByStatus(selectedStatus);
                }
                else
                {
                    BindGridView2();
                }
            }

            GridView2.PageIndex = 0;
        }
        private void SearchData(string searchText)
        {
            ViewState["SelectedStatus"] = null;

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"SELECT * FROM department 
                        WHERE DepartmentName LIKE @SearchText 
                        OR ShortAcronym LIKE @SearchText 
                        OR Status LIKE @SearchText";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SearchText", $"%{searchText}%");

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
                        }
                        
                    }
                }
            }
        }
        // FILTER FUNCTION BELOW
        protected void statusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedStatus = statusFilter.SelectedValue;
            ViewState["SelectedStatus"] = selectedStatus;

            if (selectedStatus == "")
            {
                // No filters applied, display all data
                BindGridView2();
            }
            else
            {
                // Apply status filter
                FilterDataByStatus(selectedStatus);
            }

            GridView2.PageIndex = 0;
        }
        private void FilterDataByStatus(string status)
        {
            // Reset search text
            ViewState["SearchText"] = null;

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM department";

                if (!string.IsNullOrEmpty(status))
                {
                    query += " WHERE Status = @Status";
                }

                query += " ORDER BY DepartmentID DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
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
                            GridView2.DataSource = dataSet.Tables[0];
                            GridView2.DataBind();
                            BindPagination();
                        }
                        else
                        {
                            GridView2.DataSource = null;
                            GridView2.DataBind();
                            BindPagination();
                        }
                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridView2.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPages.Text = totalPages.ToString();
                    }
                }
            }
        }
        protected void resetDeptfilterBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("../PAGES/DepartmentSettings.aspx");
        }
        protected void BindGridView2()
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM department ORDER BY DepartmentID DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            // Data is available, bind the GridView
                            GridView2.DataSource = dataSet.Tables[0];
                            GridView2.DataBind();
                        }
                        else
                        {
                            // No data found
                            GridView2.DataSource = null;
                            GridView2.DataBind();
                        }
                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridView2.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPages.Text = totalPages.ToString();
                    }
                }
            }
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

        //TABLE PAGE
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
                // Set the GridView's PageIndex directly to the parsed pageIndex - 1
                GridView2.PageIndex = pageIndex - 1;

                string selectedStatus = ViewState["SelectedStatus"]?.ToString();
                string searchText = ViewState["SearchText"]?.ToString();

                if (!string.IsNullOrEmpty(searchText))
                {
                    SearchData(searchText);
                }
                else if (!string.IsNullOrEmpty(selectedStatus))
                {
                    FilterDataByStatus(selectedStatus);
                }
                else
                {
                    BindGridView2();
                }

                BindPagination();
            }
            else
            {
                // Handle the case where parsing fails, perhaps with logging or error handling
            }
        }
        protected void btnPrev_Click(object sender, EventArgs e)
        {
            if (GridView2.PageIndex > 0)
            {
                GridView2.PageIndex -= 1;
                string selectedStatus = ViewState["SelectedStatus"]?.ToString();
                string searchText = ViewState["SearchText"]?.ToString();

                if (!string.IsNullOrEmpty(searchText))
                {
                    SearchData(searchText);
                }
                else if (!string.IsNullOrEmpty(selectedStatus))
                {
                    FilterDataByStatus(selectedStatus);
                }
                else
                {
                    BindGridView2();
                }

                BindPagination();
            }
        }
        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (GridView2.PageIndex < GridView2.PageCount - 1)
            {
                GridView2.PageIndex += 1;
                string selectedStatus = ViewState["SelectedStatus"]?.ToString();
                string searchText = ViewState["SearchText"]?.ToString();

                if (!string.IsNullOrEmpty(searchText))
                {
                    SearchData(searchText);
                }
                else if (!string.IsNullOrEmpty(selectedStatus))
                {
                    FilterDataByStatus(selectedStatus);
                }
                else
                {
                    BindGridView2();
                }

                BindPagination();
            }
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
                    BindGridView2();

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
        protected void txtPageNumber_TextChanged(object sender, EventArgs e)
        {
            btnGoToPage_Click(sender, e);
        }
        // ADD DEPT FUNCTION BELOW
        private bool AddDepartment(string deptName, string shortAcro)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "INSERT INTO Department (DepartmentName, ShortAcronym, Status) VALUES (@deptName, @shortAcro, @status)";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@deptName", deptName);
                    cmd.Parameters.AddWithValue("@shortAcro", shortAcro);
                    cmd.Parameters.AddWithValue("@status", "Active");
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        protected void btnAddSubmit_Click(object sender, EventArgs e)
        {
            string currentUserID = Session["UserID"].ToString();
            string deptName = txtboxAddDeptName.Text.Trim();
            string shortAcro = txtboxAddShortAcro.Text.Trim();

            if (!string.IsNullOrEmpty(deptName) && !string.IsNullOrEmpty(shortAcro))
            {
                if (AddDepartment(deptName, shortAcro))
                {
                    // Department added successfully, refresh the GridView
                    BindGridView2();

                    // Log the audit activity
                    LogAudit(currentUserID, "Account Added", "Successful");

                    // Clear the textboxes
                    txtboxAddDeptName.Text = "";
                    txtboxAddShortAcro.Text = "";

                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlertAddDeptModal", "showSuccessAlertAddDeptModal('Department was Successfully Added!');", true);
                }
                else
                {
                    // Failed to add department, show error message
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlertAddDeptModal", "showErrorAlertAddDeptModal('Failed to add department. Please try again.');", true);
                }
            }
            else
            {
                // Show validation message if department name or acronym is empty
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlertAddDeptModal", "showErrorAlertAddDeptModal('Please enter department name and short acronym.');", true);
            }
        }

        //EDIT DEPT FUNCTION BELOW
        protected void btnEditSubmit_Click(object sender, EventArgs e)
        {
            // Retrieve userPermissions from ViewState
            List<string> userPermissions = ViewState["UserPermissions"] as List<string>;
            // Check if user has permission to edit departments
            if (!userPermissions.Contains("Edit department information"))
            {
                Response.Redirect("../PAGES/AccessDenied.aspx"); // Or handle access denial appropriately
                return;
            }

            string currentUser = Session["UserID"].ToString(); // current logged in user
            string deptID = getDeptID.Value;
            string editDeptName = txtboxEditDeptName.Text;
            string editShortAcro = txtboxEditShortAcro.Text;
            string editStatus = ddlEditStatus.SelectedValue;

            // Validate input fields
            if (string.IsNullOrWhiteSpace(editDeptName) || string.IsNullOrWhiteSpace(editShortAcro))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlertDeptModal", "showErrorAlertDeptModal('Please fill in all required fields.');", true);
                return; // Prevent further execution
            }
            try
            {
                using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string updateQuery = "UPDATE department SET DepartmentName = @deptname, ShortAcronym = @shortacro, Status = @status WHERE DepartmentID = @deptid";

                    MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@deptid", deptID);
                    updateCommand.Parameters.AddWithValue("@deptname", editDeptName);
                    updateCommand.Parameters.AddWithValue("@shortacro", editShortAcro);
                    updateCommand.Parameters.AddWithValue("@status", editStatus);

                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        // Update related accounts based on new department status
                        string updateAccountsQuery = "UPDATE Accounts SET Status = @accountStatus WHERE Department = @shortacro";
                        MySqlCommand updateAccountsCommand = new MySqlCommand(updateAccountsQuery, connection);

                        // Determine the status to set for accounts based on department status
                        string accountStatus = (editStatus == "Active") ? "Active" : "Inactive";

                        updateAccountsCommand.Parameters.AddWithValue("@accountStatus", accountStatus);
                        updateAccountsCommand.Parameters.AddWithValue("@shortacro", editShortAcro);

                        int accountsAffected = updateAccountsCommand.ExecuteNonQuery();

                        LogAudit(currentUser, "Updated Department", "Successful");
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Updated Department Successfully!');", true);
                    }
                    else
                    {
                        LogAudit(currentUser, "Updated Department", "Unsuccessful");
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Failed to update Department');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), $"showErrorAlert{DateTime.Now.Ticks}", $"showErrorAlert('{ex.Message}');", true);
            }
            finally
            {
                BindGridView2(); // Assuming this method binds the updated data to your GridView
            }

        }

    }
}