using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using QRCoder;

namespace DMS.GC
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }
        protected void btnGenerateQR_Click(object sender, EventArgs e)
        {
            // Retrieve form data
            string recipient = recipientTextBox.Text;
            string entitlement = entitlementDropDownList.SelectedValue;
            string description = descriptionTextBox.Text;
            string dateOfIssue = dateOfIssueTextBox.Text;
            string validity = validityTextBox.Text;
            string gcType = gcTypeDropDownList.SelectedValue;
            string chargeTo = chargeToTextBox.Text;
            string status = statusDropDownList.SelectedValue;
            string hotelName = hotelDropDownList.SelectedValue; // Retrieve selected hotel name
            int quantity = int.Parse(quantityTextBox.Text);

            generatedGCNumbersListBox.Items.Clear(); // Clear any previously generated GC numbers

            for (int i = 0; i < quantity; i++)
            {
                // Generate a unique GC number
                string gcNumber = GenerateUniqueGCNumber();

                // Concatenate form data
                string qrContent = $"GC Number: {gcNumber}\nRecipient: {recipient}\nEntitlement: {entitlement}\nDescription: {description}\nDate of Issue: {dateOfIssue}\nValidity: {validity}\nGC Type: {gcType}\nCharge To: {chargeTo}\nStatus: {status}";

                // Generate hash including the hotel name in the final hash string
                string hashInput = qrContent;
                string privacyHash = GenerateHash(hashInput, hotelName);

                // Append the hash to the QR content
                string finalQRContent = qrContent + "\nPrivacy Hash: " + privacyHash;

                // Generate QR code
                Bitmap qrCodeImage = GenerateQRCode(finalQRContent);

                // Save QR code to database and Excel
                SaveQRCodeToDatabaseAndExcel(qrCodeImage, gcNumber, recipient, entitlement, description, dateOfIssue, validity, gcType, chargeTo, status, quantity, privacyHash);

                // Display the generated GC number in the ListBox
                generatedGCNumbersListBox.Items.Add(gcNumber);

                // Optionally, display QR code (this will display the last generated QR code in the batch)
                if (i == quantity - 1)
                {
                    DisplayQRCode(qrCodeImage);
                }

                // Dispose of Bitmap to release resources
                qrCodeImage.Dispose();
            }
        }


        private string GenerateUniqueGCNumber()
        {
            // Generate a unique gift certificate number (12-character alphanumeric)
            return Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
        }

        private Bitmap GenerateQRCode(string qrContent)
        {
            // Generate QR code
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            // Adjust the size of the QR code image by changing the pixel size parameter
            int pixelSize = 5; // Smaller QR code
            return qrCode.GetGraphic(pixelSize); // Adjust the size of the QR code image as needed
        }

        private void SaveQRCodeToDatabaseAndExcel(Bitmap qrCodeImage, string gcNumber, string recipient, string entitlement, string description, string dateOfIssue, string validity, string gcType, string chargeTo, string status, int quantity, string privacyHash)
        {
            // Convert Bitmap to byte array
            byte[] imageBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                qrCodeImage.Save(ms, ImageFormat.Jpeg); // Save as JPG
                imageBytes = ms.ToArray();
            }

            // Insert QR code and related information into database
            string query = "INSERT INTO GiftCertificates (GCNumber, Recipient, Entitlement, Description, DateOfIssue, Validity, GCType, ChargeTo, Status, QRCodeImage, Quantity, PrivacyHash) " +
                           "VALUES (@GCNumber, @Recipient, @Entitlement, @Description, @DateOfIssue, @Validity, @GCType, @ChargeTo, @Status, @QRCodeImage, @Quantity, @PrivacyHash)";

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GCNumber", gcNumber);
                    command.Parameters.AddWithValue("@Recipient", recipient);
                    command.Parameters.AddWithValue("@Entitlement", entitlement);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@DateOfIssue", dateOfIssue);
                    command.Parameters.AddWithValue("@Validity", validity);
                    command.Parameters.AddWithValue("@GCType", gcType);
                    command.Parameters.AddWithValue("@ChargeTo", chargeTo);
                    command.Parameters.AddWithValue("@Status", status);
                    command.Parameters.AddWithValue("@QRCodeImage", imageBytes);
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@PrivacyHash", privacyHash);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            // Save QR code image to a local folder
            SaveQRCodeImageToLocalFolder(qrCodeImage, gcNumber);

            // Write data to Excel
            WriteDataToExcel(qrCodeImage, gcNumber, recipient, entitlement, description, dateOfIssue, validity, gcType, chargeTo, status, quantity, privacyHash);
        }
        private void SaveQRCodeImageToLocalFolder(Bitmap qrCodeImage, string gcNumber)
        {
            // Specify the directory where images will be saved
            string saveDirectory = Server.MapPath("~/App_Data/QRCodeImages");

            // Create directory if it doesn't exist
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            // Save the image with GC number as the file name
            string filePath = Path.Combine(saveDirectory, $"{gcNumber}.jpg");
            qrCodeImage.Save(filePath, ImageFormat.Jpeg);
        }
        private void WriteDataToExcel(Bitmap qrCodeImage, string gcNumber, string recipient, string entitlement, string description, string dateOfIssue, string validity, string gcType, string chargeTo, string status, int quantity, string privacyHash)
        {
            string qrImagesDirectory = Server.MapPath("~/App_Data/QRImages");
            if (!Directory.Exists(qrImagesDirectory))
            {
                Directory.CreateDirectory(qrImagesDirectory);
            }

            string tempImagePath = Path.Combine(qrImagesDirectory, $"{gcNumber}.jpg");

            try
            {
                // Save Bitmap to a file with the GC number as its name
                qrCodeImage.Save(tempImagePath, ImageFormat.Jpeg); // Save as JPG

                // Create or open existing Excel file using EPPlus
                string excelFilePath = Server.MapPath("~/App_Data/databasebellevue.xlsx");
                using (ExcelPackage package = new ExcelPackage(new FileInfo(excelFilePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"]; // Replace "Sheet1" with your sheet name

                    // Create header row if it doesn't exist
                    if (worksheet.Dimension == null)
                    {
                        worksheet.Cells[1, 1].Value = "GCNumber";
                        worksheet.Cells[1, 2].Value = "Recipient";
                        worksheet.Cells[1, 3].Value = "Entitlement";
                        worksheet.Cells[1, 4].Value = "Description";
                        worksheet.Cells[1, 5].Value = "DateOfIssue";
                        worksheet.Cells[1, 6].Value = "Validity";
                        worksheet.Cells[1, 7].Value = "GCType";
                        worksheet.Cells[1, 8].Value = "ChargeTo";
                        worksheet.Cells[1, 9].Value = "Status";
                        worksheet.Cells[1, 10].Value = "Quantity";
                        worksheet.Cells[1, 11].Value = "PrivacyHash";
                        worksheet.Cells[1, 12].Value = "QRCodeImagePath";
                    }

                    // Find the next empty row in the worksheet
                    int rowCount = worksheet.Dimension?.Rows + 1 ?? 2;

                    // Write data to Excel cells
                    worksheet.Cells[rowCount, 1].Value = gcNumber;
                    worksheet.Cells[rowCount, 2].Value = recipient;
                    worksheet.Cells[rowCount, 3].Value = entitlement;
                    worksheet.Cells[rowCount, 4].Value = description;
                    worksheet.Cells[rowCount, 5].Value = dateOfIssue;
                    worksheet.Cells[rowCount, 6].Value = validity;
                    worksheet.Cells[rowCount, 7].Value = gcType;
                    worksheet.Cells[rowCount, 8].Value = chargeTo;
                    worksheet.Cells[rowCount, 9].Value = status;
                    worksheet.Cells[rowCount, 10].Value = quantity;
                    worksheet.Cells[rowCount, 11].Value = privacyHash;
                    worksheet.Cells[rowCount, 12].Value = tempImagePath;

                    // Optionally, add QR code image as a drawing to the worksheet
                    ExcelPicture qrCodePicture = worksheet.Drawings.AddPicture(gcNumber, new FileInfo(tempImagePath));
                    qrCodePicture.SetPosition(rowCount - 1, 0, 11, 0); // Adjust the position as needed
                    qrCodePicture.SetSize(100, 100); // Set the size as needed

                    // Save the changes to the Excel file
                    package.Save();
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log, display message, etc.)
                Console.WriteLine($"Error saving Excel file: {ex.Message}");
            }
        }



        private void TryDeleteFile(string filePath)
        {
            int attempts = 0;
            while (attempts < 3)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    return; // Exit the method if deletion is successful
                }
                catch (IOException)
                {
                    // File is still in use, wait and retry
                    System.Threading.Thread.Sleep(500); // Wait for 0.5 seconds before retrying
                    attempts++;
                }
            }
        }


        private void DisplayQRCode(Bitmap qrCodeImage)
        {
            try
            {
                // Convert image to Base64 and display
                imgQRCode.ImageUrl = "data:image/jpeg;base64," + ImageToBase64(qrCodeImage, ImageFormat.Jpeg); // Display as JPG
                qrCodeContainer.Visible = true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error displaying QR code: {ex.Message}");
            }
        }

        // Convert image to Base64 string
        private string ImageToBase64(Bitmap image, ImageFormat format)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Convert Image to byte array
                    image.Save(ms, format);
                    byte[] imageBytes = ms.ToArray();

                    // Convert byte array to Base64 string
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error converting image to Base64: {ex.Message}");
                throw; // Re-throw the exception for further handling
            }
        }





        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Retrieve the GC number entered by the user
            string qrCodeNumber = gcNumberTextBox.Text;

            // Execute database query to fetch data based on GC number
            string query = "SELECT * FROM GiftCertificates WHERE GCNumber = @GCNumber";

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GCNumber", qrCodeNumber);

                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Autofill form fields with retrieved data
                            recipientTextBox.Text = reader["Recipient"].ToString();
                            entitlementDropDownList.SelectedValue = reader["Entitlement"].ToString();
                            descriptionTextBox.Text = reader["Description"].ToString();
                            dateOfIssueTextBox.Text = ((DateTime)reader["DateOfIssue"]).ToString("yyyy-MM-dd");
                            validityTextBox.Text = ((DateTime)reader["Validity"]).ToString("yyyy-MM-dd");
                            gcTypeDropDownList.SelectedValue = reader["GCType"].ToString();
                            chargeToTextBox.Text = reader["ChargeTo"].ToString();
                            statusDropDownList.SelectedValue = reader["Status"].ToString();

                            // Optionally, display QR code image
                            byte[] qrCodeImageBytes = (byte[])reader["QRCodeImage"];
                            Bitmap qrCodeImage = ImageFromByteArray(qrCodeImageBytes);
                            DisplayQRCode(qrCodeImage);

                            // Write data to Excel with quantity
                            WriteDataToExcel(qrCodeImage, qrCodeNumber, recipientTextBox.Text, entitlementDropDownList.SelectedValue, descriptionTextBox.Text, dateOfIssueTextBox.Text, validityTextBox.Text, gcTypeDropDownList.SelectedValue, chargeToTextBox.Text, statusDropDownList.SelectedValue, 1, reader["PrivacyHash"].ToString());
                        }
                        else
                        {
                            // Handle case when GC number is not found
                            // Display message or clear text fields
                            ClearFields();
                        }
                    }
                }
            }
        }





        private void ClearQRCodeDisplay()
        {
            imgQRCode.ImageUrl = "";
            qrCodeContainer.Visible = false;
        }



        protected void btnScanQRCode_Click(object sender, EventArgs e)
        {
            // Call a JavaScript function to open the device's camera for scanning QR codes
            ScriptManager.RegisterStartupScript(this, GetType(), "StartQRCodeScan", "StartQRCodeScan();", true);
        }

        private Bitmap ImageFromByteArray(byte[] byteArrayIn)
        {
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                return new Bitmap(ms);
            }
        }

        private void ClearFields()
        {
            // Clear text fields and QR code image
            recipientTextBox.Text = "";
            entitlementDropDownList.SelectedIndex = -1;
            descriptionTextBox.Text = "";
            dateOfIssueTextBox.Text = "";
            validityTextBox.Text = "";
            gcTypeDropDownList.SelectedIndex = -1;
            chargeToTextBox.Text = "";
            statusDropDownList.SelectedIndex = -1;
            ClearQRCodeDisplay();
        }



        private string GetConnectionString()
        {
            string server = "localhost";
            string database = "documentsystem";
            string username = "root";
            string password = "";

            return $"Server={server};Database={database};Uid={username};Pwd={password};";
        }

        private string GenerateHash(string input, string hotelName)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash as a byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convert byte array to a string and take first 24 characters
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length && builder.Length < 24; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                // Ensure the hash is exactly 24 characters long
                string hash = builder.ToString().Substring(0, 24);

                // Append the hotel name without spaces to the hash string
                string hotelNameNoSpaces = hotelName.Replace(" ", "");
                return hash + hotelNameNoSpaces;
            }
        }
    }
}