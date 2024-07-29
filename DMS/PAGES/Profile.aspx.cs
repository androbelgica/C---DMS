using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DMS.PAGES
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Name"] == null)
            {
                // Redirect to login page if session variable is not set
                Response.Redirect("../LOGIN/Login.aspx");
            }
            nameTxtbox.Text = Session["Name"].ToString();
            usernameTxtbox.Text = Session["Username"].ToString();
            contactTxtbox.Text = Session["Contact"].ToString();
            emailTxtbox.Text = Session["Email"].ToString();
            departmentTxtbox.Text = Session["Department"].ToString();
            positionTxtbox.Text = Session["Position"].ToString();

        }

        private string GetConnectionString()
        {
            string server = "localhost";
            string database = "documentsystem";
            string username = "root";
            string password = "";

            return $"server={server};database={database};uid={username};pwd={password};";
        }
        private void LogAudit(string activity, string status)
        {
            string userid = Session["UserID"].ToString(); // Get the current logged-in user ID

            string connectionString = GetConnectionString();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string auditQuery = "INSERT INTO AuditLogs (UserID, DateTime, Activity, Status) VALUES (@UserID, NOW(), @Activity, @Status)";
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


        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            string connectionString = GetConnectionString();
            try
            {
                string enteredCurrentPassword = currentpwordTxtbox.Text.Trim();
                string enteredNewPassword = newpwordTxtbox.Text.Trim();
                string confirmedPassword = confirmpwordTxtbox.Text.Trim();

                if (string.IsNullOrEmpty(enteredCurrentPassword) || string.IsNullOrEmpty(enteredNewPassword) || string.IsNullOrEmpty(confirmedPassword))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('All fields are required.');", true);
                    return;
                }

                if (enteredNewPassword != confirmedPassword)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('New password and confirm password do not match.');", true);
                    return;
                }

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string userID = Session["UserID"].ToString();

                    string query = "SELECT Password FROM Accounts WHERE UserID = @UserID";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        string currentPassword = cmd.ExecuteScalar()?.ToString();

                        if (currentPassword == enteredCurrentPassword)
                        {
                            query = "UPDATE Accounts SET Password = @Password WHERE UserID = @UserID";
                            using (MySqlCommand updateCmd = new MySqlCommand(query, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@Password", enteredNewPassword);
                                updateCmd.Parameters.AddWithValue("@UserID", userID);
                                int rowsAffected = updateCmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Password updated successfully!');", true);

                                    LogAudit("Password Update", "Successful");

                                    currentpwordTxtbox.Text = "";
                                    newpwordTxtbox.Text = "";
                                    confirmpwordTxtbox.Text = "";
                                }
                                else
                                {
                                    ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('No rows were updated');", true);
                                }
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Current password is incorrect.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "myalert", $"alert('An error occurred: {ex.Message}');", true);
            }
        }
    }
}