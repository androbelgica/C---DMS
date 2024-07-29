using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DMS.PAGES
{
    public partial class folders : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Name"] == null)
            {
                // Redirect to login page if session variable is not set
                Response.Redirect("../LOGIN/Login.aspx");
            }

            string currentUserName = Session["Name"].ToString();
            bool canViewAllFolders = CheckUserPermission("View all folders from all departments");
            bool canCreateFolders = CheckUserPermission("Create Folders"); // New permission check

            // Control visibility of create folder button based on permission
            newFolderBTN.Visible = canCreateFolders;

            if (!canCreateFolders && canViewAllFolders)
            {
                // If user can view all folders but cannot create, hide the new folder button
                newFolderBTN.Visible = false;
            }
            else if (canViewAllFolders && Session["Department"].ToString() == "MIS")
            {
                // For users from "MIS" department who can view all folders,
                // hide the new folder button if they cannot create folders
                newFolderBTN.Visible = canCreateFolders;
            }

            // Check if user does not have both permissions
            else if (!canViewAllFolders && !canCreateFolders)
            {
                Response.Redirect("../PAGES/AccessDenied.aspx");
            }

            if (canViewAllFolders)
            {
                // User can view all departments and folders
                if (!IsPostBack)
                {
                    if (Request.UrlReferrer != null)
                    {
                        Session["PreviousUrl"] = Request.UrlReferrer.AbsoluteUri;
                    }
                    BindDepartments(); // Display all departments
                    newFolderBTN.Visible = false;
                }
            }
            else
            {
                // User can view only their department's folders
                string sessDept = Session["Department"].ToString();

                if (sessDept == "MIS")
                {
                    if (!IsPostBack)
                    {
                        if (Request.UrlReferrer != null)
                        {
                            Session["PreviousUrl"] = Request.UrlReferrer.AbsoluteUri;
                        }
                        BindDepartments(); // Display departments for MIS
                        newFolderBTN.Visible = false;
                    }
                }
                else
                {
                    if (!IsPostBack)
                    {
                        if (Request.UrlReferrer != null)
                        {
                            Session["PreviousUrl"] = Request.UrlReferrer.AbsoluteUri;
                        }
                        BindFoldersWithAcronym(sessDept); // Display folders for the user's department
                        BackFolderBTN.Visible = false;
                    }
                }
            }
        }
        protected void BackFolderBTN_Click(object sender, EventArgs e)
        {
            // Redirect back to the previous page if available
            if (Session["PreviousUrl"] != null)
            {
                Response.Redirect(Session["PreviousUrl"].ToString());
            }
            else
            {
                // Redirect to a default page if no previous URL is available
                Response.Redirect("../LOGIN/LandingPage.aspx");
            }
        }
        //CARDS
        private void BindDepartments()
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT DepartmentName FROM department;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {


                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            rptDepartment.DataSource = dataTable;
                            rptDepartment.DataBind();
                        }
                        else
                        {
                            rptDepartment.DataSource = null;
                            rptDepartment.DataBind();
                        }
                    }
                }
            }
        }
        // GETTING THE FOLDERS
        private void BindFolders(string departmentName)
        {
            int departmentID = GetDepartmentIDFromName(departmentName);
            BackFolderBTN.Visible = true;
            if (departmentID == -1)
            {
                return;
            }

            hfCurrentDepartmentID.Value = departmentID.ToString();

            // Get the current logged-in user's name
            string currentUserName = Session["Name"].ToString();

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT FolderID, FolderName, Privacy, CreatedBy FROM folders WHERE DepartmentID = @DepartmentID;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);

                            rptFolder.DataSource = FilterFoldersByPrivacy(dataTable, currentUserName);
                            rptFolder.DataBind();
                        }
                        else
                        {
                            rptFolder.DataSource = null;
                            rptFolder.DataBind();
                        }
                    }
                }
            }
        }
        private DataTable FilterFoldersByPrivacy(DataTable dataTable, string currentUserName)
        {
            // Clone the structure of the original DataTable
            DataTable filteredTable = dataTable.Clone();

            foreach (DataRow row in dataTable.Rows)
            {
                string privacy = row["Privacy"].ToString();
                string createdBy = row["CreatedBy"].ToString();

                // Check if the folder should be displayed based on privacy settings and creator
                if (privacy != "Only Me" || createdBy == currentUserName)
                {
                    // Copy the row to the filtered table
                    filteredTable.ImportRow(row);
                }
            }

            return filteredTable;
        }
        private int GetDepartmentIDFromName(string departmentName)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT DepartmentID FROM department WHERE DepartmentName = @DepartmentName;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentName", departmentName);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            // Return -1 if departmentName is not found
            return -1;
        }
        //GETTING THE FOLDERS BY ACRONYM
        private void BindFoldersWithAcronym(string departmentAcro)
        {
            newFolderBTN.Visible = true;
            int departmentID = GetDepartmentIDFromAcro(departmentAcro);

            if (departmentID == -1)
            {
                return;
            }

            hfCurrentDepartmentID.Value = departmentID.ToString();
            // Get the current logged-in user's name
            string currentUserName = Session["Name"].ToString();

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
                    SELECT FolderID, FolderName, Privacy, CreatedBy 
                    FROM folders 
                    WHERE DepartmentID = @DepartmentID 
                    AND (Privacy <> 'Only Me' OR CreatedBy = @CurrentUserName);
                ";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                    cmd.Parameters.AddWithValue("@CurrentUserName", currentUserName);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            rptFolder.DataSource = dataTable;
                            rptFolder.DataBind();
                        }
                        else
                        {
                            rptFolder.DataSource = null;
                            rptFolder.DataBind();
                        }
                    }
                }
            }
        }
        private int GetDepartmentIDFromAcro(string departmentAcro)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT DepartmentID FROM department WHERE ShortAcronym = @ShortAcronym;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ShortAcronym", departmentAcro);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            // Return -1 if departmentName is not found
            return -1;
        }
        protected void DepartmentLink_Click(object sender, EventArgs e)
        {
            LinkButton departmentLink = (LinkButton)sender;
            string departmentName = departmentLink.CommandArgument;
            BindFolders(departmentName);

            // Assuming you have session information available for Department and Name
            string currentDepartment = Session["Department"].ToString();
            string currentUserName = Session["Name"].ToString();

            bool canViewAllFolders = CheckUserPermission("View all folders from all departments");
            bool canCreateFolders = CheckUserPermission("Create Folders"); // New permission check

            // Control visibility of create folder button based on permission
            newFolderBTN.Visible = canCreateFolders;

            if (!canCreateFolders && canViewAllFolders && currentDepartment == "MIS")
            {
                // If user can view all folders but cannot create, hide the new folder button
                newFolderBTN.Visible = false;
            }

            rptDepartment.Visible = false;
        }

        //GETTING FILES
        private void BindFiles(string folderName)
        {
            // First, retrieve the DepartmentID based on the departmentName
            int departmentID = Convert.ToInt32(hfCurrentDepartmentID.Value);
            BackFolderBTN.Visible = true;
            if (departmentID == -1)
            {
                // Handle case where department ID is not valid
                return;
            }

            // Retrieve the FolderID for the given folderName and departmentID
            int folderID = GetFolderIDFromNameAndDepartment(folderName, departmentID);
            if (folderID == -1)
            {
                // Handle case where folderID is not found for the given folderName and departmentID
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT FileName FROM files WHERE FolderID = @FolderID AND FolderID IN (SELECT FolderID FROM folders WHERE DepartmentID = @DepartmentID);";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FolderID", folderID);
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentID);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            rptFiles.DataSource = dataTable;
                            rptFiles.DataBind();
                        }
                        else
                        {
                            rptFiles.DataSource = null;
                            rptFiles.DataBind();
                        }
                    }
                }
            }
        }
        private int GetFolderIDFromNameAndDepartment(string folderName, int departmentID)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT FolderID FROM folders WHERE FolderName = @FolderName AND DepartmentID = @DepartmentID;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FolderName", folderName);
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentID);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            // Return -1 if folderID is not found
            return -1;
        }
        protected void FolderLink_Click(object sender, EventArgs e)
        {
            LinkButton folderLink = (LinkButton)sender;
            string folderName = folderLink.CommandArgument;
            BindFiles(folderName);


            rptFolder.Visible = false;
            newFolderBTN.Visible = false;
        }      
        //LINK BTNS
        protected void deptLink_Click(object sender, EventArgs e)
        {
            rptDepartment.Visible = true;
            rptFolder.Visible = false;
            rptFiles.Visible = false;
        }
        protected void folderLink_Click(object sender, EventArgs e)
        {
            rptDepartment.Visible = false;
            rptFolder.Visible = true;
            rptFiles.Visible = false;
        }
        //ADD FOLDER
        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(hfCurrentDepartmentID.Value, out int departmentID))
            {
                // Handle the case where the department ID is not a valid integer
                return;
            }

            string folderName = txtboxFolderName.Text.Trim();
            string folderPrivacy = ddlFolderPrivacy.SelectedValue;

            // Get the current logged-in user's name
            string currentUserName = Session["Name"].ToString();

            if (!string.IsNullOrEmpty(folderName))
            {
                using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    string query = "INSERT INTO folders (FolderName, DepartmentID, Privacy, CreatedBy) VALUES (@FolderName, @DepartmentID, @Privacy, @CreatedBy);";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FolderName", folderName);
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                        cmd.Parameters.AddWithValue("@Privacy", folderPrivacy);
                        cmd.Parameters.AddWithValue("@CreatedBy", currentUserName);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Refresh the folders list for the current department
                BindFolders(GetDepartmentNameFromID(departmentID));
                txtboxFolderName.Text = string.Empty;

                // Hide the back button for non-MIS departments
                string currentDepartment = Session["Department"].ToString();
                if (currentDepartment != "MIS")
                {
                    BackFolderBTN.Visible = false;
                }
            }
        }
        private string GetDepartmentNameFromID(int departmentID)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT DepartmentName FROM department WHERE DepartmentID = @DepartmentID;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentID);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }

            return string.Empty;
        }
        // DELETE FOLDER
        protected void rptFolder_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ShowDeleteModal")
            {
                int folderID = Convert.ToInt32(e.CommandArgument);
                hfDeleteFolderID.Value = folderID.ToString();
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowDeleteModal", "$('#deleteConfirmationModal').modal('show');", true);
            }
        }
        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            int folderID = Convert.ToInt32(hfDeleteFolderID.Value);
            DeleteFolder(folderID);
        }
        private void DeleteFolder(int folderID)
        {
            // Check if the user has the permission "Delete their folders (Only Me)"
            bool canDeleteOnlyMeFolders = CheckUserPermission("Delete their folders (Only Me)");

            // Retrieve the creator of the folder
            string createdBy = GetFolderCreatedBy(folderID);

            // Get the current logged-in user's name
            string currentUserName = Session["Name"].ToString();

            // Check if the user can delete this folder
            if (canDeleteOnlyMeFolders && createdBy == currentUserName)
            {
                using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    // Delete the folder; this will also delete related files due to ON DELETE CASCADE
                    string query = "DELETE FROM folders WHERE FolderID = @FolderID;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FolderID", folderID);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Rebind the folders to reflect the deletion
                string currentDepartmentName = GetDepartmentNameFromID(Convert.ToInt32(hfCurrentDepartmentID.Value));
                BindFolders(currentDepartmentName);

                // Hide the back button for non-MIS departments
                string currentDepartment = Session["Department"].ToString();
                if (currentDepartment != "MIS")
                {
                    BackFolderBTN.Visible = false;
                }
            }
            else
            {
                // Display an error message or handle permissions not met
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('You do not have permission to delete this folder.');", true);
            }
        }
        private bool CheckUserPermission(string permissionName)
        {
            // Get the current logged-in user's ID
            string userID = Session["UserID"].ToString(); // Assuming UserID is stored in Session

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Query to check if the user has the specified permission
                string query = "SELECT COUNT(*) FROM User_Permissions WHERE UserID = @UserID AND PermissionID = (SELECT PermissionID FROM Permissions WHERE PermissionName = @PermissionName);";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@PermissionName", permissionName);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        protected bool CanDeleteFolder(int folderID)
        {
            // Check if the current logged-in user can delete this folder
            string currentUserName = Session["Name"].ToString();
            bool canDeleteOnlyMeFolders = CheckUserPermission("Delete their folders (Only Me)");
            bool canDeleteAllFolders = CheckUserPermission("Delete all folders");

            // Fetch the creator and privacy of the folder
            string createdBy = GetFolderCreatedBy(folderID);
            string privacy = GetFolderPrivacy(folderID);

            // Determine if the current user can delete this folder
            bool canDelete = false;

            if (canDeleteAllFolders)
            {
                // User has permission to delete all folders
                canDelete = true;
            }
            else if (canDeleteOnlyMeFolders && createdBy == currentUserName)
            {
                // User has permission to delete their own folders, check privacy restrictions
                if (privacy == "Only Me")
                {
                    canDelete = true;
                }
                else
                {
                    canDelete = false;
                }
            }
            else if (canDeleteOnlyMeFolders && canDeleteAllFolders)
            {
                // User has permission to delete "My Department" and "Public" folders and "Only Me" folders
                canDelete = true;
            }
            else
            {
                // User cannot delete this folder
                canDelete = false;
            }

            return canDelete;
        }
        private string GetFolderCreatedBy(int folderID)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Query to retrieve the creator of the folder
                string query = "SELECT CreatedBy FROM folders WHERE FolderID = @FolderID;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FolderID", folderID);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }

            return string.Empty;
        }
        private string GetFolderPrivacy(int folderID)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Query to retrieve the privacy setting of the folder
                string query = "SELECT Privacy FROM folders WHERE FolderID = @FolderID;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FolderID", folderID);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }

            return string.Empty;
        }
        // RENAME DEPT FOLDER
        protected void btnRename_Click(object sender, EventArgs e)
        {
            string folderID = hfCurrentFolderID.Value;
            string newFolderName = rnFolder.Text;

            if (string.IsNullOrEmpty(newFolderName))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlertRenameModal", "showErrorAlertRenameModal('Blank folder name is not allowed.');", true);
                return;
            }

            try
            {
                // Check if the user has permission to rename folders
                bool canRenameOnlyMeFolders = CheckUserPermission("Rename their folders only");
                bool canRenameAllFolders = CheckUserPermission("Rename all folders");

                // Fetch the creator and privacy of the folder
                string createdBy = GetFolderCreatedBy(Convert.ToInt32(folderID));
                string privacy = GetFolderPrivacy(Convert.ToInt32(folderID));

                if ((canRenameOnlyMeFolders && createdBy == Session["Name"].ToString() && privacy == "Only Me") ||
                    canRenameAllFolders)
                {
                    using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                    {
                        conn.Open();

                        string updateQuery = "UPDATE folders SET FolderName = @NewFolderName WHERE FolderID = @FolderID;";
                        using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@NewFolderName", newFolderName);
                            updateCmd.Parameters.AddWithValue("@FolderID", folderID);

                            int rowsAffected = updateCmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                // Folder was successfully renamed, rebind folders to reflect the changes
                                rnFolder.Text = string.Empty;
                                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Folder was successfully renamed!');", true);

                                // Rebind the folders to reflect the changes
                                BindFoldersForCurrentDepartment();

                                // Hide the back button for non-MIS departments
                                string currentDepartment = Session["Department"].ToString();
                                if (currentDepartment != "MIS")
                                {
                                    BackFolderBTN.Visible = false;
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Folder could not be renamed.');", true); //Folder ID not found or no rows affected.
                            }
                        }
                    }
                }
                else
                {
                    // User does not have permission to rename this folder
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('You do not have permission to rename this folder.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('{ex.Message}');", true);
            }
        }
        private void BindFoldersForCurrentDepartment()
        {
            int departmentID;
            if (!int.TryParse(hfCurrentDepartmentID.Value, out departmentID))
            {
                return;
            }

            string departmentName = GetDepartmentNameFromID(departmentID);
            if (!string.IsNullOrEmpty(departmentName))
            {
                BindFolders(departmentName);
            }
        }
        protected bool CanRenameFolder(int folderID)
        {
            // Check if the current logged-in user can rename this folder
            string currentUserName = Session["Name"].ToString();
            bool canRenameTheirFoldersOnly = CheckUserPermission("Rename their folders only");
            bool canRenameAllFolders = CheckUserPermission("Rename all folders");

            // Fetch the creator of the folder
            string createdBy = GetRenameFolderCreatedBy(folderID);
            string privacy = GetRenameFolderPrivacy(folderID);

            // Determine if the current user can rename this folder
            bool canRename = false;

            if (canRenameAllFolders)
            {
                // User has permission to rename all folders
                canRename = true;
            }
            else if (canRenameTheirFoldersOnly && createdBy == currentUserName)
            {
                // User has permission to rename their own folders only
                if (privacy == "Only Me")
                {
                    canRename = true;
                }
                else
                {
                    canRename = false;
                }
            }
            else if (canRenameTheirFoldersOnly && canRenameAllFolders)
            {
                // User has permission to rename all including their own folders
                canRename = true;
            }
            else
            {
                // User cannot rename this folder
                canRename = false;
            }

            return canRename;
        }
        private string GetRenameFolderCreatedBy(int folderID)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Query to retrieve the creator of the folder
                string query = "SELECT CreatedBy FROM folders WHERE FolderID = @FolderID;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FolderID", folderID);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }

            return string.Empty;
        }
        private string GetRenameFolderPrivacy(int folderID)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Query to retrieve the privacy setting of the folder
                string query = "SELECT Privacy FROM folders WHERE FolderID = @FolderID;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FolderID", folderID);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }

            return string.Empty;
        }
        //DELETE FILES INSIDE FOLDER
        public class FileDetails
        {
            public string ControlID { get; set; }
            public string FileName { get; set; }
            public string UploaderName { get; set; }
            public DateTime UploadDateTime { get; set; }
            public string Privacy { get; set; }
            public string Category { get; set; }
            public byte[] FileContent { get; set; }
            public string OCRText { get; set; }
        }
        protected void rptFiles_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                // Get the filename from the command argument
                string filename = e.CommandArgument.ToString();

                // Check user permissions
                bool canDelete = CheckUserDeletePermission(filename);

                // If user can delete, show delete confirmation modal
                if (canDelete)
                {
                    // Store the filename in a hidden field to pass to modal
                    hrFileName.Value = filename;
                    // Register script to show modal
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "deleteFileModal", $"showDeleteFileModal('{filename}');", true);
                }
                else
                {
                    // User does not have permission to delete
                    ScriptManager.RegisterStartupScript(this, GetType(), "hideDeleteOption", "$('.deleteButton').hide();", true);
                }

                // Ensure newFolderBTN visibility is updated
                UpdateNewFolderButtonVisibility();
            }
            else if (e.CommandName == "ConfirmDeleteFile")
            {
                // Get the filename from the hidden field
                string filename = hrFileName.Value;
                // Delete the file
                DeleteFile(filename);

                // Ensure newFolderBTN visibility is updated
                UpdateNewFolderButtonVisibility();
            }
            else if (e.CommandName == "Preview")
            {
                string fileName = e.CommandArgument.ToString();
                // Retrieve file details from the database
                DataTable fileDetails = GetFileDetails(fileName);

                if (fileDetails != null && fileDetails.Rows.Count > 0)
                {
                    DataRow row = fileDetails.Rows[0];

                    // Store necessary details in session variables
                    Session["PreviewFileName"] = row["FileName"].ToString();
                    Session["PreviewUploaderName"] = row["UploaderName"].ToString();
                    Session["PreviewDepartment"] = row["Department"].ToString();
                    Session["PreviewUploadDateTime"] = row["UploadDateTime"].ToString();
                    Session["PreviewPrivacy"] = row["Privacy"].ToString();
                    Session["PreviewCategory"] = row["Category"].ToString();
                    Session["PreviewControlID"] = row["ControlID"].ToString();
                    Session["PreviewOcrText"] = row["OCRText"].ToString();
                    Session["PreviewFileContent"] = Convert.ToBase64String((byte[])row["FileContent"]);
                    Session["FileExtension"] = GetFileExtension(row["FileName"].ToString());

                    // Redirect to the preview page
                    Response.Redirect("~/PAGES/Preview.aspx");
                }
                else
                {
                    // Handle case where file details are not found
                    ScriptManager.RegisterStartupScript(this, GetType(), "FileNotFound", "alert('File details not found.');", true);
                }
            }
            else
            {
                string errorMessage = "Failed to retrieve file details. Please try again later.";
                ScriptManager.RegisterStartupScript(this, GetType(), "ErrorMessage", $"alert('{errorMessage}');", true);
            }
        }
        private DataTable GetFileDetails(string fileName)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "SELECT f.FileName, f.UploaderName, d.DepartmentName AS Department, f.UploadDateTime, f.Privacy, f.Category, f.ControlID, f.OCRText, f.FileContent " +
                               "FROM files f " +
                               "INNER JOIN Accounts a ON f.UploaderName = a.Name " +
                               "INNER JOIN department d ON a.Department = d.ShortAcronym " +
                               "WHERE f.FileName = @FileName";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FileName", fileName);
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable fileDetails = new DataTable();
                        adapter.Fill(fileDetails);
                        return fileDetails;
                    }
                }
            }
        }
        private string GetFileExtension(string fileName)
        {
            return System.IO.Path.GetExtension(fileName);
        }



        private void UpdateNewFolderButtonVisibility()
        {
            string currentDepartment = Session["Department"].ToString();
            if (currentDepartment != "MIS")
            {
                newFolderBTN.Visible = false;
            }
            else
            {
                newFolderBTN.Visible = true; // Ensure it's explicitly set if needed
            }
        }
        protected bool CheckDeletePermission(string filename)
        {
            bool canDelete = CheckUserDeletePermission(filename);
            return canDelete;
        }
        private bool CheckUserDeletePermission(string filename)
        {
            // Get the uploader name for the file
            string uploaderName = GetUploaderName(filename);

            // Check if the user has permission to delete based on uploaderName and privacy setting
            bool canDelete = CanUserDeleteFile(uploaderName, filename);

            return canDelete;
        }
        private string GetUploaderName(string filename)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT UploaderName FROM files WHERE FileName = @filename;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@filename", filename);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }

            return string.Empty;
        }
        private bool CanUserDeleteFile(string uploaderName, string filename)
        {
            // Retrieve current logged-in user's name
            string userName = Session["Name"].ToString(); // Assuming this retrieves the current user's name

            // Check if the user has specific permissions
            bool canDeleteTheirDocumentsOnly = CheckPermission(userName, "Delete their documents only");

            // Retrieve privacy setting for the file
            string privacyOption = GetPrivacyOptionForFile(filename);

            // Determine delete permission based on conditions
            if (canDeleteTheirDocumentsOnly && uploaderName == userName && privacyOption == "Only Me")
            {
                // User has "Delete their documents only" and is the uploader and privacy is not "Only Me", allow delete
                return true;
            }
            else if (CheckPermission(userName, "Delete all documents"))
            {
                // User has "Delete all documents" permission, allow delete
                return true;
            }
            else
            {
                // User does not have both permissions or conditions not met, disallow delete
                return false;
            }
        }
        private string GetPrivacyOptionForFile(string filename)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT Privacy FROM files WHERE FileName = @filename;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@filename", filename);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }

            return string.Empty;
        }
        private bool CheckPermission(string userName, string permissionName)
        {
            // Query to check if the user has the specified permission
            string query = @"SELECT COUNT(*) 
                     FROM User_Permissions up
                     INNER JOIN Permissions p ON up.PermissionID = p.PermissionID
                     INNER JOIN Accounts a ON up.UserID = a.UserID
                     INNER JOIN department d ON up.DepartmentID = d.DepartmentID
                     WHERE a.Name = @UserName
                     AND p.PermissionName = @PermissionName";

            int count = 0;

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@PermissionName", permissionName);

                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return count > 0;
        }
        protected void btnConfirmDeleteFile_Click(object sender, EventArgs e)
        {
            // Triggered when the user confirms deletion in the modal
            string filename = hrFileName.Value;
            DeleteFile(filename);
        }
        private void DeleteFile(string filename)
        {
            // Retrieve the folder ID using the file name
            int folderID = GetFolderIDForFile(filename);

            if (folderID == -1)
            {
                // Handle the case where the folder ID is not found
                return;
            }

            // Retrieve the ControlID for the file
            string controlID = GetControlIDForFile(filename);

            // Retrieve additional information about the file
            string uploaderName = Session["Name"].ToString(); // Current logged-in user's name
            string activity = "Deleted";
            string status = "Successful";
            string privacyOption = GetPrivacyOptionForFile(filename);
            string category = GetCategoryForFile(filename);

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Delete the file inside the folder
                string query = "DELETE FROM files WHERE FileName = @filename AND FolderID = @folderID;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@filename", filename);
                    cmd.Parameters.AddWithValue("@folderID", folderID);
                    cmd.ExecuteNonQuery();
                }
            }

            // Log the file deletion activity
            LogAudit(controlID, filename, uploaderName, activity, status, privacyOption, category);

            // Retrieve the folder name for the given file name
            string folderName = GetFolderNameFromID(folderID);

            // Rebind the files to reflect the deletion
            BindFiles(folderName);

            // Hide the back button for non-MIS departments
            string currentDepartment = Session["Department"].ToString();
            if (currentDepartment != "MIS")
            {
                newFolderBTN.Visible = false;
            }
        }
        private string GetControlIDForFile(string filename)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT ControlID FROM files WHERE FileName = @filename;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@filename", filename);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }

            return string.Empty;
        }
        private string GetCategoryForFile(string filename)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT Category FROM files WHERE FileName = @filename;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@filename", filename);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }

            return string.Empty;
        }
        private int GetFolderIDForFile(string filename)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT FolderID FROM files WHERE FileName = @filename;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@filename", filename);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            // Return -1 if folder ID is not found
            return -1;
        }
        private string GetFolderNameFromID(int folderID)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT FolderName FROM folders WHERE FolderID = @folderID;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@folderID", folderID);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }

            // Return an empty string if the folder name is not found
            return string.Empty;
        }
        //FORDA ICONS
        public string GetFileIcon(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            string iconClass = "bx bxs-file";
            string colorClass = "default-color"; // Define a default color class

            switch (extension)
            {
                case ".pdf":
                    iconClass = "bx bxs-file-pdf";
                    colorClass = "pdf-color"; // Define a class for PDF color
                    break;
                case ".doc":
                case ".docx":
                    iconClass = "bx bxs-file-doc";
                    colorClass = "doc-color"; // Define a class for DOC color
                    break;
                case ".jpg":
                case ".jpeg":
                case ".png":
                    iconClass = "bx bxs-file-image";
                    colorClass = "image-color"; // Define a class for image color
                    break;
                case ".xlsx":
                case ".xls":
                case ".xlsb":
                case ".xlsa":
                    iconClass = "bx bxs-file";
                    colorClass = "excel-color"; // Define a class for image color
                    break;
                case ".txt":
                    iconClass = "bx bxs-file-txt";
                    colorClass = "txt-color";
                    break;
                default:
                    iconClass = "bx bxs-file";
                    colorClass = "default-color"; // Define a class for default color
                    break;
            }

            // Return a combination of icon class and color class
            return $"{iconClass} {colorClass}";
        }
        private string GetConnectionString()
        {
            string server = "localhost";
            string database = "documentsystem";
            string username = "root";
            string password = "";

            return $"server={server};database={database};uid={username};pwd={password};";
        }
        private void LogAudit(string controlID, string fileName, string uploaderName, string activity, string status, string privacyOption, string category)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO documentlogs (ControlID, UploaderName, FileName, LogDateTime, Activity, Status, Privacy, Category) VALUES (@ControlID, @UploaderName, @FileName, @LogDateTime, @Activity, @Status, @Privacy, @Category)";

                        // Add parameters
                        command.Parameters.AddWithValue("@ControlID", controlID);
                        command.Parameters.AddWithValue("@UploaderName", uploaderName);
                        command.Parameters.AddWithValue("@FileName", fileName);
                        command.Parameters.AddWithValue("@LogDateTime", DateTime.Now);
                        command.Parameters.AddWithValue("@Activity", activity);
                        command.Parameters.AddWithValue("@Status", status);
                        command.Parameters.AddWithValue("@Privacy", privacyOption);
                        command.Parameters.AddWithValue("@Category", string.IsNullOrEmpty(category) ? DBNull.Value : (object)category);

                        // Execute query
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("File not successful.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('{ex.Message}');", true);
            }
        }
    }
}