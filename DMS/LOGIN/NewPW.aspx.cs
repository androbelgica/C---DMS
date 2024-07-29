using MySql.Data.MySqlClient;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DMS
{
    public partial class NewPW : System.Web.UI.Page
    {
        private string enteredEmail;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Retrieve the email from the query string
            enteredEmail = Request.QueryString["Email"];
        }

        private string GetConnectionString()
        {
            string server = "localhost";
            string database = "documentsystem";
            string username = "root";
            string password = "";

            return $"server={server};database={database};uid={username};pwd={password};";
        }

        protected void SubmitCodeBtn_Click(object sender, EventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                try
                {
                    connection.Open();

                    string newPass = newpwtxtbox.Text;
                    string confirmPass = newpwtxtbox2.Text;

                    if (newPass != confirmPass)
                    {
                        // Passwords don't match
                        error_alert.InnerText = "New password and confirm password do not match.";
                        error_alert.Visible = true;
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showAlertAndHide('error_alert');", true);
                    }
                    else if (string.IsNullOrWhiteSpace(newPass) || string.IsNullOrWhiteSpace(confirmPass))
                    {
                        // New password or confirm password is empty
                        error_alert.InnerText = "New password and confirm password must not be empty.";
                        error_alert.Visible = true;
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showAlertAndHide('error_alert');", true);
                    }
                    else
                    {
                        // Passwords match, retrieve UserID from the database
                        string userId;
                        using (MySqlCommand cmd = new MySqlCommand("SELECT UserID FROM Accounts WHERE Email = @Email", connection))
                        {
                            cmd.Parameters.AddWithValue("@Email", enteredEmail);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    userId = reader["UserID"].ToString();
                                }
                                else
                                {
                                    // User not found, handle accordingly
                                    error_alert.InnerText = "User not found.";
                                    error_alert.Visible = true;
                                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showAlertAndHide('error_alert');", true);
                                    return;
                                }
                            }
                        }

                        // Update the password in the database
                        UpdatePassword(connection, userId, newPass);

                        // Log the audit
                        LogAudit(userId, "Reset Password", "Successful");

                        // Provide feedback to the user
                        ScriptManager.RegisterStartupScript(this, GetType(), "passwordResetSuccess", "alert('Password reset successful.'); window.location.href = 'Login.aspx';", true);
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    ScriptManager.RegisterStartupScript(this, GetType(), "resetPasswordError", $"alert('Error resetting password: {ex.Message}');", true);
                }
            }
        }

        private void UpdatePassword(MySqlConnection connection, string userId, string newPassword)
        {
            try
            {
                string updateQuery = "UPDATE Accounts SET Password = @NewPassword WHERE UserID = @UserID";
                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection))
                {
                    updateCmd.Parameters.AddWithValue("@NewPassword", newPassword);
                    updateCmd.Parameters.AddWithValue("@UserID", userId);

                    // Execute the update query
                    updateCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                ScriptManager.RegisterStartupScript(this, GetType(), "updatePasswordError", $"alert('Error updating password: {ex.Message}');", true);
            }
        }

        private void LogAudit(string userId, string activity, string status)
        {
            string connectionString = GetConnectionString();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string auditQuery = "INSERT INTO AuditLogs (UserID, UserLogDateTime, Activity, Status) VALUES (@UserID, NOW(), @Activity, @Status)";
                    using (MySqlCommand auditCmd = new MySqlCommand(auditQuery, connection))
                    {
                        auditCmd.Parameters.AddWithValue("@UserID", userId);
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
