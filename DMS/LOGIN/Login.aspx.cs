using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI;

namespace DMS
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private string GetConnectionString()
        {
            string server = "localhost";
            string database = "documentsystem";
            string username = "root";
            string password = "";

            return $"server={server};database={database};uid={username};pwd={password};";
        }
        protected void LoginBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(unametxtbox.Text) || !string.IsNullOrEmpty(pwordtxtbox.Text))
            {
                string connectionString = GetConnectionString();
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        string enteredUsername = unametxtbox.Text;
                        string enteredPassword = pwordtxtbox.Text;

                        string query = "SELECT * FROM accounts WHERE Username = @username";
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@username", enteredUsername);

                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string storedPassword = reader["Password"].ToString();
                                string status = reader["Status"].ToString();
                                string userID = reader["UserID"].ToString();
                                string accessLevel = reader["Access"].ToString();

                                if (enteredPassword == storedPassword)
                                {
                                    if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Add the following line to set the authentication cookie
                                        FormsAuthentication.SetAuthCookie(enteredUsername, false);

                                        // Set the user name and other details in the session
                                        Session["Name"] = reader["Name"].ToString();
                                        Session["Username"] = reader["Username"].ToString();
                                        Session["Contact"] = reader["Contact"].ToString();
                                        Session["Department"] = reader["Department"].ToString();
                                        Session["Position"] = reader["Position"].ToString();
                                        Session["Email"] = reader["Email"].ToString();
                                        Session["Access"] = accessLevel; // Store access level in session
                                        Session["UserID"] = userID;

                                        // Retrieve user permissions from the database
                                        List<string> userPermissions = GetUserPermissionsFromDatabase(userID);
                                        Session["UserPermissions"] = userPermissions;

                                        LogAudit(userID, "Login", "Successful");

                                        //Response.Redirect("/load.aspx?url=../LOGIN/LandingPage.aspx");
                                        //Response.Redirect("/load.aspx?url=../PAGES/Dashboard.aspx");
                                        //Response.Redirect("/load.aspx?url=../PAGES/Upload.aspx");
                                        //Response.Redirect("/load.aspx?url=../PAGES/AccountSettings.aspx");
                                        Response.Redirect("../GC/Availment.aspx");
                                        //Response.Redirect("../GC/GCInventory.aspx");
                                    }
                                    else
                                    {
                                        // Username is not active
                                        HandleError("Username is not active.", userID);
                                    }
                                }
                                else
                                {
                                    // Invalid password
                                    HandleError("Invalid password.", userID);
                                }
                            }
                        }
                        else
                        {
                            // Invalid username
                            HandleError("Invalid username.", enteredUsername);
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", $"alert('Error: {ex.Message}');", true);
                    }
                }
            }
            else
            {
                // Empty textboxes
                error_alert.InnerText = "Username or password is invalid.";
                error_alert.Visible = true;
                ScriptManager.RegisterStartupScript(this, GetType(), "showAlert", "showAlertAndHide();", true);
            }
        }
        private List<string> GetUserPermissionsFromDatabase(string userId)
        {
            List<string> permissions = new List<string>();

            try
            {
                string connectionString = GetConnectionString();
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT p.PermissionName " +
                                   "FROM Permissions p " +
                                   "INNER JOIN User_Permissions up ON p.PermissionID = up.PermissionID " +
                                   "WHERE up.UserID = @UserId";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            permissions.Add(reader["PermissionName"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "permissionsError", $"alert('Error retrieving permissions: {ex.Message}');", true);
            }

            return permissions;
        }
        private void HandleError(string errorMessage, string userID)
        {
            error_alert.InnerText = errorMessage;
            error_alert.Visible = true;
            ScriptManager.RegisterStartupScript(this, GetType(), "showAlert", "showAlertAndHide();", true);

            LogAudit(userID, "Login", "Unsuccessful");
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
    }
}
