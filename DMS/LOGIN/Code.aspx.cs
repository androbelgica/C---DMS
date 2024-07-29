using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Web.UI;

namespace DMS
{
    public partial class Code : System.Web.UI.Page
    {
        public string enteredEmail;
        private bool emailSent = false; // Variable to track email sending status
        protected void Page_Load(object sender, EventArgs e)
        {
            // Retrieve the email from the query string
            enteredEmail = Request.QueryString["Email"];
        }

        protected void SubmitCodeBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(codetxtbox.Text))
            {
                string enteredCode = codetxtbox.Text;

                // Check if the entered OTP exists in the database and is valid
                if (IsValidOtp(enteredCode))
                {
                    // Add the following line to set the authentication cookie
                    FormsAuthentication.SetAuthCookie(enteredCode, false);
                    Response.Redirect($"../LOGIN/NewPW.aspx?Email={Server.UrlEncode(enteredEmail)}");
                }
                else
                {
                    // Display error alert for invalid OTP or expired OTP
                    error_alert.InnerText = "Invalid or expired OTP.";
                    error_alert.Visible = true;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showAlert", "showAlertAndHide();", true);
                }
            }
            else
            {
                // Empty textboxes
                error_alert.InnerText = "Please enter OTP.";
                error_alert.Visible = true;
                ScriptManager.RegisterStartupScript(this, GetType(), "showAlert", "showAlertAndHide();", true);
            }
        }

        private bool IsValidOtp(string otp)
        {
            string connectionString = GetConnectionString();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the OTP exists and is not expired
                    string query = "SELECT COUNT(*) FROM ResetOTP WHERE ResetCode = @otp AND ExpiryDate >= NOW()";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@otp", otp);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Failed to check OTP existence: {ex.Message}");
                return false;
            }
        }

        private bool OtpExistsInDatabase(string otp)
        {
            string connectionString = GetConnectionString();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM ResetOTP WHERE ResetCode = @otp";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@otp", otp);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Failed to check OTP existence: {ex.Message}");
                return false;
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
        protected async void ResendLink_Click(object sender, EventArgs e)
        {
            try
            {
                string otpCode = GenerateOtpCode(); // Generate a new OTP code

                // Store the reset code in the database (update the existing entry)
                StoreResetCodeInDatabase(enteredEmail, otpCode); // Use your existing StoreResetCodeInDatabase method

                // Send OTP email
                await SendOtpEmail(enteredEmail, otpCode);

                // Show success message
                success_alert.InnerText = "OTP has been resent successfully.";
                success_alert.Visible = true;

                // Register script to set success message in JavaScript variable
                ScriptManager.RegisterStartupScript(this, GetType(), "setSuccessMessage", $"var successMessage = 'OTP has been resent successfully.';", true);
                ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert();", true);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Failed to resend OTP: {ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('Failed to resend OTP: {ex.Message}');", true);
            }
        }
        private string GenerateOtpCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // Generate a 6-digit OTP code
        }
        private async Task SendOtpEmail(string recipientEmail, string otpCode)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("api-key", "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");

                    var emailData = new
                    {
                        sender = new { name = "Reset Password", email = "documentmanagementsystem@outlook.com" },
                        to = new List<object> { new { email = recipientEmail, name = "Recipient Name" } },
                        subject = "Your OTP Code",
                        htmlContent = $"<h3>Your OTP code is: {otpCode}</h3>"
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(emailData), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("https://api.sendinblue.com/v3/smtp/email", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Email sent successfully
                        emailSent = true;

                        // Log audit with UserID
                        string userId = GetUserIdByEmail(recipientEmail);
                        if (!string.IsNullOrEmpty(userId))
                        {
                            LogAudit(userId, "OTP Sent to Email", "Successful");
                        }
                    }
                    else
                    {
                        // Email sending failed
                        emailSent = false;
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        HandleError($"Failed to send OTP email. Sendinblue API response: {response.StatusCode} - {errorMessage}", recipientEmail);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                emailSent = false;
                HandleError($"Error sending OTP email: {ex.Message}", recipientEmail);
            }
        }
        private void StoreResetCodeInDatabase(string email, string resetCode)
        {
            string connectionString = GetConnectionString();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO ResetOTP (Email, ResetCode, ExpiryDate) VALUES (@Email, @ResetCode, @ExpiryDate)";
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@ResetCode", resetCode);
                        cmd.Parameters.AddWithValue("@ExpiryDate", DateTime.Now.AddMinutes(5)); // Code expires 5 minutes

                        // Execute the insert query
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Failed to store reset code: {ex.Message}");
            }
        }
        private void HandleError(string errorMessage, string email)
        {
            error_alert.InnerText = errorMessage;
            error_alert.Visible = true;
            ScriptManager.RegisterStartupScript(this, GetType(), "showAlert", "showAlertAndHide();", true);

            // Add to Audit
            string userId = GetUserIdByEmail(email);
            if (!string.IsNullOrEmpty(userId))
            {
                LogAudit(userId, "OTP Sent to Email", "Unsuccessful");
            }
        }
        private string GetUserIdByEmail(string email)
        {
            string connectionString = GetConnectionString();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT UserID FROM accounts WHERE Email = @Email";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        return cmd.ExecuteScalar()?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Failed to get user ID: {ex.Message}");
                return null;
            }
        }
        private void LogAudit(string userid, string activity, string status)
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
                        auditCmd.Parameters.AddWithValue("@UserID", userid);
                        auditCmd.Parameters.AddWithValue("@Activity", activity);
                        auditCmd.Parameters.AddWithValue("@Status", status);

                        // Execute the audit query
                        auditCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                ScriptManager.RegisterStartupScript(this, GetType(), "auditError", $"alert('Audit log error: {ex.Message}');", true);
            }
        }
    }
}