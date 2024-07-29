using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Image = System.Drawing.Image;


namespace DMS.PAGES
{
    public partial class Preview : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Name"] == null)
            {
                // Redirect to login page if session variable is not set
                Response.Redirect("../LOGIN/Login.aspx");
            }

            if (!IsPostBack)
            {
                // Check if the necessary data is available in session variables
                if (Session["PreviewFileName"] != null && Session["PreviewUploaderName"] != null &&
                    Session["PreviewPrivacy"] != null && Session["PreviewCategory"] != null &&
                    Session["PreviewControlID"] != null && Session["PreviewOcrText"] != null
                    && Session["PreviewFileContent"] != null && Session["FileExtension"] != null)
                {
                    // Retrieve and set the data from session variables
                    SetPreviewDetails();
                }
                else
                {
                    string errorMessage = "An error occurred while processing your request. Please try again later.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ErrorMessage", $"alert('{errorMessage}');", true);
                }
                PopulateFolderDropdown("Only Me", ddlOnlyMe);
                PopulateFolderDropdown("My Department", ddlMyDepartment);
                PopulateFolderDropdown("Public", ddlPublic);

                DisableRadioButtons();

                // Check permissions and enable edit button accordingly
                CheckAndSetEditPermissions();
            }
        }
        private void CheckAndSetEditPermissions()
        {
            string currentUser = Session["Name"].ToString();
            bool canEditAll = CheckUserPermission(currentUser, "Edit all documents");
            bool canEditOwn = CheckUserPermission(currentUser, "Edit their documents only");

            string uploaderName = Session["PreviewUploaderName"].ToString();
            bool isUploader = uploaderName.Equals(currentUser, StringComparison.OrdinalIgnoreCase);

            // Determine visibility of edit button based on permissions and uploader
            if (canEditAll || (canEditOwn && isUploader))
            {
                btnPreviewEdit.Visible = true;
            }
            else
            {
                btnPreviewEdit.Visible = false;
            }
        }
        private bool CheckUserPermission(string userName, string permissionName)
        {
            // Logic to query database and check if user has the specified permission
            bool hasPermission = false;

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM User_Permissions up " +
                               "INNER JOIN Permissions p ON up.PermissionID = p.PermissionID " +
                               "INNER JOIN Accounts a ON up.UserID = a.UserID " +
                               "WHERE a.Name = @UserName AND p.PermissionName = @PermissionName";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@PermissionName", permissionName);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    hasPermission = count > 0;
                }
            }

            return hasPermission;
        }
        private string GetConnectionString()
        {
            string server = "localhost";
            string database = "documentsystem";
            string username = "root";
            string password = "";

            return $"server={server};database={database};uid={username};pwd={password};";
        }
        // PREVIEW DISPLAY
        private void SetPreviewDetails()
        {
            string fileName = Session["PreviewFileName"].ToString();
            hiddenPreviewFileName.Value = fileName;

            string uploaderName = Session["PreviewUploaderName"].ToString();
            string uploaderDept = Session["PreviewDepartment"].ToString();
            string uploadDateTime = Session["PreviewUploadDateTime"].ToString();
            string privacy = Session["PreviewPrivacy"].ToString();
            string uploadCategory = Session["PreviewCategory"].ToString();
            string controlID = Session["PreviewControlID"].ToString();
            string ocrText = Session["PreviewOcrText"].ToString();

            string fileExtension = Session["FileExtension"].ToString();
            hiddenPreviewFileExtension.Value = fileExtension;

            string fileContent = Session["PreviewFileContent"].ToString();

            // Store original details in hidden fields
            hiddenPreviewOriginalPrivacy.Value = privacy;
            hiddenPreviewOriginalFileName.Value = fileName;
            hiddenPreviewControlID.Value = controlID;

            // Populate the corresponding fields in the preview page
            previewdocnametxtbox.Text = fileName;
            previewuploadernametxtbox.Text = uploaderName;
            previewdepttxtbox.Text = uploaderDept;
            datetimetxtbox.Text = uploadDateTime;
            previewcategorytxtbox.Text = uploadCategory;
            SetPrivacyRadioButtons(privacy);

            // Display OCR text in the preview container
            //previewContainer.InnerHtml = Server.HtmlEncode(ocrText);

            // Determine the file type and display accordingly
            if (fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") || fileName.EndsWith(".png"))
            {
                // Image file
                previewContainer.InnerHtml = $"<img src='data:image/jpeg;base64,{fileContent}' width='100%' height='auto;'/>";
            }
            else if (fileName.EndsWith(".pdf"))
            {
                // PDF file
                previewContainer.InnerHtml = $"<iframe src='data:application/pdf;base64,{fileContent}' width='100%' height='500px;'></iframe>";
                btnPreviewPrint.Visible = false;
            }
            else if (fileName.EndsWith(".xlsx") || fileName.EndsWith(".xls"))
            {
                // Excel file
                previewContainer.InnerHtml = $"<p>File type not supported for preview. Here is the OCR Text of the file:<br><br> {Server.HtmlEncode(ocrText)}</p>";
                btnPreviewPrint.Visible = false;
            }
            else if (fileName.EndsWith(".docx") || fileName.EndsWith(".doc"))
            {
                // Word document
                previewContainer.InnerHtml = $"<p>File type not supported for preview. Here is the OCR Text of the file:<br><br> {Server.HtmlEncode(ocrText)}</p>";
                btnPreviewPrint.Visible = false;
            }
            else if (fileName.EndsWith(".txt"))
            {
                // Text file
                previewContainer.InnerHtml = $"<p>File type not supported for preview. Here is the OCR Text of the file:<br><br> {Server.HtmlEncode(ocrText)}</p>";
                btnPreviewPrint.Visible = false;
            }
            else
            {
                // Handle unsupported file types or display a message
                if (!string.IsNullOrEmpty(ocrText))
                {
                    previewContainer.InnerHtml = $"<p>File type not supported for preview. Here is the OCR Text of the file:<br><br> {Server.HtmlEncode(ocrText)}</p>";
                    btnPreviewPrint.Visible = false;
                }
                else
                {
                    previewContainer.InnerHtml = "<p>File type not supported for preview.</p>";
                    btnPreviewPrint.Visible = false;
                }
            }
        }
        private void SetPrivacyRadioButtons(string privacy)
        {
            switch (privacy)
            {
                case "Only Me":
                    rbOnlyMe.Checked = true;
                    break;
                case "My Department":
                    rbMyDepartment.Checked = true;
                    break;
                case "Public":
                    rbPublic.Checked = true;
                    break;
            }
        }
        private void DisableRadioButtons()
        {
            rbOnlyMe.Enabled = false;
            rbMyDepartment.Enabled = false;
            rbPublic.Enabled = false;
        }
        private void EnableRadioButtons()
        {
            rbOnlyMe.Enabled = true;
            rbMyDepartment.Enabled = true;
            rbPublic.Enabled = true;
        }
        protected void btnPreviewEdit_Click(object sender, EventArgs e)
        {
            SetEditMode(true);
            RestoreOriginalPrivacy();
            EnableRadioButtons();
        }
        protected void btnPreviewCancel_Click(object sender, EventArgs e)
        {
            SetEditMode(false);
            RestoreOriginalPrivacy();
            DisableRadioButtons();
            RestoreOriginalFileName();
        }
        private void SetEditMode(bool isEditMode)
        {
            //btnPreviewBack.Visible = !isEditMode;
            btnPreviewDownload.Visible = !isEditMode;
            //btnPreviewPrint.Visible = !isEditMode;
            btnPreviewEdit.Visible = !isEditMode;
            previewdocnametxtbox.ReadOnly = !isEditMode;

            btnPreviewCancel.Visible = isEditMode;
            btnPreviewSubmit.Visible = isEditMode;
        }
        private void RestoreOriginalPrivacy()
        {
            string originalPrivacy = hiddenPreviewOriginalPrivacy.Value;
            SetPrivacyRadioButtons(originalPrivacy); // Set the radio buttons to the original privacy value
        }
        private void RestoreOriginalFileName()
        {
            string originalFileName = hiddenPreviewFileName.Value;
            previewdocnametxtbox.Text = originalFileName;
        }

        // DOWNLOAD
        protected void btnPreviewDownload_Click(object sender, EventArgs e)
        {
            string originalFileName = hiddenPreviewOriginalFileName.Value;
            string downloadFileName = previewdocnametxtbox.Text;

            byte[] fileContent = RetrieveFileContentFromDatabase(originalFileName);
            if (fileContent != null && fileContent.Length > 0)
            {
                InitiateFileDownload(fileContent, downloadFileName);
            }
            else
            {
                Response.Write("Error: File not found or empty.");
            }
        }
        private byte[] RetrieveFileContentFromDatabase(string fileName)
        {
            byte[] fileContent = null;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "SELECT FileContent FROM files WHERE FileName = @FileName";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FileName", fileName);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        fileContent = (byte[])result;
                    }
                }
            }
            return fileContent;
        }
        private void InitiateFileDownload(byte[] fileContent, string downloadFileName)
        {
            try
            {
                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + downloadFileName);
                Response.BinaryWrite(fileContent);
                Response.End();
            }
            catch (Exception ex)
            {
                Response.Write("Error: " + ex.Message);
            }
        }
        protected void btnPreviewBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PAGES/Dashboard.aspx");
        }

        // PRINT FUNCTION
        protected void btnPreviewPrint_Click(object sender, EventArgs e)
        {
            // Register script to print the content of the preview frame
            ScriptManager.RegisterStartupScript(this, GetType(), "PrintScript", "printPreviewContent();", true);
        }

        // SUBMIT
        private string GetSelectedPrivacyOption()
        {
            if (rbOnlyMe.Checked)
            {
                return "Only Me";
            }
            else if (rbMyDepartment.Checked)
            {
                return "My Department";
            }
            else if (rbPublic.Checked)
            {
                return "Public";
            }
            return "Only Me"; // Default privacy option if none is selected
        }
        protected void btnPreviewSubmit_Click(object sender, EventArgs e)
        {
            // Get necessary parameters
            string controlID = hiddenPreviewControlID.Value;
            string modifiedFileName = previewdocnametxtbox.Text;
            string fileExtension = hiddenPreviewFileExtension.Value;
            string modifiedPrivacy = GetSelectedPrivacyOption();
            string currentUser = Session["Name"].ToString();

            string originalFileName = string.Empty;
            string originalPrivacy = string.Empty;
            string existingCategory = string.Empty;
            int originalFolderID = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    // Retrieve original details
                    string selectQuery = "SELECT FileName, Category, Privacy, FolderID FROM files WHERE ControlID = @controlID";
                    MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);
                    selectCommand.Parameters.AddWithValue("@controlID", controlID);

                    using (MySqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            originalFileName = reader["FileName"].ToString();
                            originalPrivacy = reader["Privacy"].ToString();
                            existingCategory = reader["Category"].ToString();
                            originalFolderID = Convert.ToInt32(reader["FolderID"]);
                        }
                    }

                    // Combine the new file name with the original file extension
                    string updatedFileName = GetFileNameWithoutExtension(modifiedFileName) + fileExtension;

                    // Update filename, privacy, and folder
                    string updateQuery = "UPDATE files SET FileName = @filename, Privacy = @privacy, FolderID = @folderID WHERE ControlID = @controlID";
                    MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@filename", updatedFileName);
                    updateCommand.Parameters.AddWithValue("@privacy", modifiedPrivacy);
                    updateCommand.Parameters.AddWithValue("@folderID", GetSelectedFolderID(modifiedPrivacy));
                    updateCommand.Parameters.AddWithValue("@controlID", controlID);

                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        LogAudit(controlID, updatedFileName, currentUser, "Edited", "Successful", modifiedPrivacy, existingCategory);
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Updated File Successfully!');", true);
                    }
                    else
                    {
                        LogAudit(controlID, updatedFileName, currentUser, "Edited", "Unsuccessful", modifiedPrivacy, existingCategory);
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Updated File Failed');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('{ex.Message}');", true);
            }
            finally
            {
                // Restore read-only state of the previewdocnametxtbox and disable radio buttons
                //btnPreviewBack.Visible = true;
                btnPreviewDownload.Visible = true;
                //btnPreviewPrint.Visible = true;
                btnPreviewEdit.Visible = true;
                previewdocnametxtbox.ReadOnly = true;

                btnPreviewCancel.Visible = false;
                btnPreviewSubmit.Visible = false;
                DisableRadioButtons();
            }
        }
        private int GetSelectedFolderID(string privacyOption)
        {
            // Initialize folder ID
            int folderID = 0;

            // Determine which dropdown list corresponds to the selected privacy option
            DropDownList ddl;
            switch (privacyOption)
            {
                case "Only Me":
                    ddl = ddlOnlyMe;
                    break;
                case "My Department":
                    ddl = ddlMyDepartment;
                    break;
                case "Public":
                    ddl = ddlPublic;
                    break;
                default:
                    return folderID; // Return 0 if privacy option is invalid
            }

            // Get the selected folder ID from the dropdown list
            if (ddl != null && ddl.SelectedItem != null)
            {
                folderID = Convert.ToInt32(ddl.SelectedItem.Value);
            }

            return folderID;
        }
        protected void PopulateFolderDropdown(string privacyOption, DropDownList ddl)
        {
            // Retrieve the currently logged-in user's username
            string name = Session["Name"] != null ? Session["Name"].ToString() : string.Empty;

            // Get the department ID associated with the logged-in user
            int departmentID = GetLoggedInUserDepartmentID(name);

            // Clear existing items and add default items
            ddl.Items.Clear();

            // Query to fetch folders based on privacy and department
            string query = "SELECT FolderID, FolderName FROM Folders WHERE Privacy = @Privacy AND DepartmentID = @DepartmentID";

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Privacy", privacyOption);
                command.Parameters.AddWithValue("@DepartmentID", departmentID);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ListItem item = new ListItem();
                            item.Text = reader["FolderName"].ToString();
                            item.Value = reader["FolderID"].ToString();
                            ddl.Items.Add(item);
                        }
                    }
                    else
                    {
                        // Add a default text when no folders are available
                        ddl.Items.Add(new ListItem("No folders available", ""));
                        ddl.Items[ddl.Items.Count - 1].Attributes.Add("disabled", "disabled");
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    // Handle the exception (e.g., log or display error)
                    Console.WriteLine("Error fetching folders: " + ex.Message);
                }
            }
        }
        protected int GetLoggedInUserDepartmentID(string name)
        {
            int departmentID = 0;

            string query = "SELECT DepartmentID FROM department WHERE ShortAcronym = (SELECT Department FROM Accounts WHERE Name = @name)";

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", name);
                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    departmentID = Convert.ToInt32(result);
                }
            }

            return departmentID;
        }
        protected string GetFileNameWithoutExtension(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }
        protected string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName);
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
                            throw new Exception("File upload not successful.");
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