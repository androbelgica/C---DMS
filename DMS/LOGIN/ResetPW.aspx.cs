using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks; 
using MySql.Data.MySqlClient;
using System.Web.UI;
using System;
using System.Web.Services;
using System.Web;
using System.Configuration;
using MySqlX.XDevAPI;
using System.Data.SqlClient;


namespace DMS
{
    public partial class ResetPW : System.Web.UI.Page
    {
        private bool emailSent = false; // Variable to track email sending status

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected async void SendCodeBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(emailtxtbox.Text))
            {
                string enteredEmail = emailtxtbox.Text;

                // Check if the email exists in the database
                if (EmailExistsInDatabase(enteredEmail))
                {
                    // Check if the email is active
                    if (IsEmailActiveInDatabase(enteredEmail))
                    {
                        string otpCode = GenerateOtpCode(); // Generate a random OTP code

                        // Store the reset code in the database
                        StoreResetCodeInDatabase(enteredEmail, otpCode);

                        // Send OTP email
                        await SendOtpEmail(enteredEmail, otpCode);

                        if (emailSent)
                        {
                            // Redirect to the OTP code entry page with email in query string
                            Response.Redirect($"Code.aspx?Email={Server.UrlEncode(enteredEmail)}");
                        }
                        else
                        {
                            HandleError("Failed to send OTP email. Please try again later.", enteredEmail);
                        }
                    }
                    else
                    {
                        // Email is inactive in the database
                        error_alert.InnerText = "Your account is inactive. Please activate it first.";
                        error_alert.Visible = true;
                        ScriptManager.RegisterStartupScript(this, GetType(), "showAlert", "showAlertAndHide();", true);
                    }
                }
                else
                {
                    // Email does not exist in the database
                    error_alert.InnerText = "Email does not exist.";
                    error_alert.Visible = true;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showAlert", "showAlertAndHide();", true);
                }
            }
            else
            {
                // Empty textboxes
                error_alert.InnerText = "Please enter Email.";
                error_alert.Visible = true;
                ScriptManager.RegisterStartupScript(this, GetType(), "showAlert", "showAlertAndHide();", true);
            }
        }
        
        private string GenerateOtpCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // Generate a 6-digit OTP code
        }

        [WebMethod]
        public static string GetApiKeyandEmail()
        {
            string query = "SELECT ApiKey, ApiEmail FROM ApiKeys WHERE ApiID = 1"; // Assuming ApiID 1 always holds your key

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string apiKey = reader["ApiKey"].ToString();
                        string apiEmail = reader["ApiEmail"].ToString();

                        // Create an anonymous object
                        var result = new { ApiKey = apiKey, ApiEmail = apiEmail };

                        // Serialize object to JSON
                        string jsonResult = JsonConvert.SerializeObject(result);
                        return jsonResult;
                    }
                    return null; // Handle if no data found (though in your case, it should always find ApiID 1)
                }
            }
        }
        // Method to update API key and email
        [WebMethod]
        public static void UpdateApiKeyAndEmail(string newApiKey, string newApiEmail, string userID)
        {
            try
            {
                // Update the API key and email in the database
                UpdateApiKeyInDatabase(newApiKey, newApiEmail);

                // Log the update in the audit log
                LogAudit(userID, "Updated API Key and Email", "Successful");
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                throw new ApplicationException("Failed to update API key and email: " + ex.Message);
            }
        }

        // Method to update API key and email in database
        private static void UpdateApiKeyInDatabase(string newApiKey, string newApiEmail)
        {
            string query = "UPDATE ApiKeys SET ApiKey = @ApiKey, ApiEmail = @ApiEmail WHERE ApiID = 1";

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApiKey", newApiKey);
                    command.Parameters.AddWithValue("@ApiEmail", newApiEmail);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Update successful
                        System.Diagnostics.Debug.WriteLine("API Key and Email updated successfully in database.");
                    }
                    else
                    {
                        // Update failed
                        throw new Exception("Failed to update API Key and Email in database.");
                    }
                }
            }
        }
        private async Task SendOtpEmail(string recipientEmail, string otpCode)
        {
            try
            {
                // Fetch API key and email from the database based on ApiID
                var apiKeyInfo = GetApiKeyInfo(1); // Replace with the actual method to fetch from database

                if (apiKeyInfo == null)
                {
                    throw new Exception("API key information not found.");
                }

                string apiKey = apiKeyInfo.ApiKey;
                string apiEmail = apiKeyInfo.ApiEmail;

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("api-key", apiKey);

                    var emailData = new
                    {
                        sender = new { name = "Reset Password", email = apiEmail },
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

        // Method to fetch API key and email from database
        private ApiKeyInfo GetApiKeyInfo(int apiId)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                var command = new SqlCommand("SELECT ApiKey, ApiEmail FROM ApiKeys WHERE ApiID = @ApiID", connection);
                command.Parameters.AddWithValue("@ApiID", apiId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ApiKeyInfo
                        {
                            ApiKey = reader["ApiKey"].ToString(),
                            ApiEmail = reader["ApiEmail"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        // Define a class to hold API key information
        public class ApiKeyInfo
        {
            public string ApiKey { get; set; }
            public string ApiEmail { get; set; }
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
                        cmd.Parameters.AddWithValue("@ExpiryDate", DateTime.Now.AddMinutes(5)); // Code expires in 5 minutes

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
        private bool EmailExistsInDatabase(string email)
        {
            string connectionString = GetConnectionString();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM accounts WHERE Email = @Email";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Failed to check email existence: {ex.Message}");
                return false;
            }
        }
        private bool IsEmailActiveInDatabase(string email)
        {
            string connectionString = GetConnectionString();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Status FROM accounts WHERE Email = @Email";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        string status = cmd.ExecuteScalar()?.ToString();

                        // Check if the status is "Active"
                        return !string.IsNullOrEmpty(status) && status.Equals("Active", StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Failed to check email status: {ex.Message}");
                return false;
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
        private static void LogAudit(string userID, string activity, string status)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string auditQuery = "INSERT INTO AuditLogs (UserID, UserLogDateTime, Activity, Status) VALUES (@UserID, NOW(), @Activity, @Status)";
                    using (MySqlCommand auditCmd = new MySqlCommand(auditQuery, connection))
                    {
                        auditCmd.Parameters.AddWithValue("@UserID", userID);
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
                throw new ApplicationException("Audit log error: " + ex.Message);
            }
        }
        private static string GetConnectionString()
        {
            string server = "localhost";
            string database = "documentsystem";
            string username = "root";
            string password = "";

            return $"server={server};database={database};uid={username};pwd={password};";
        }
    }
}
