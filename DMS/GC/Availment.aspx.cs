using System;
using System.Drawing;
using QRCoder;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI;
using System.Configuration;
using MySql.Data.MySqlClient;
using DocumentFormat.OpenXml.Presentation;
using System.Security.Cryptography;
using System.Text;
using OfficeOpenXml.Drawing;
using OfficeOpenXml;
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using System.Linq;

namespace DMS.GC
{
    public partial class Availment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            if (!IsPostBack)
            {
                // validation of dates in CREATE section
                cvDateOfIssue.ValueToCompare = DateTime.Now.ToString("yyyy-MM-dd");
                cvValidity.ValueToCompare = DateTime.Now.ToString("yyyy-MM-dd");

                // validation of dates in BOOKING section
                cvBookingFromDate.ValueToCompare = DateTime.Now.ToString("yyyy-MM-dd");
                //cvBookingToDate.ValueToCompare = DateTime.Now.ToString("yyyy-MM-dd");

                scanCard.Visible = false;
            }
        }
        // CREATE SECTION FUNCTION BELOW
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
            string status = statusTextBox.Text;
            int quantity = int.Parse(quantityTextBox.Text);
            string hotelBranch = hotelDropDownList.SelectedValue;
            string batch = GenerateBatchIdentifier();
            generatedGCNumbersListBox.Items.Clear(); // Clear any previously generated GC numbers

            for (int i = 0; i < quantity; i++)
            {
                // Generate a unique GC number
                string gcNumber = GenerateUniqueGCNumber();

                // Generate sequence info
                string sequenceInfo = GenerateSequenceInfo(i + 1, quantity);

                // Concatenate form data
                string qrContent = $"GC Number: {gcNumber}\nRecipient: {recipient}\nEntitlement: {entitlement}\nDescription: {description}\nDate of Issue: {dateOfIssue}\nValidity: {validity}\nGC Type: {gcType}\nCharge To: {chargeTo}\nHotel Branch: {hotelBranch}\nStatus: {status}\nSequence: {sequenceInfo}";

                // Generate hash including the hotel name in the final hash string
                string hashInput = qrContent;
                string privacyHash = GenerateHash(hashInput, hotelBranch);

                // Append the hash to the QR content
                string finalQRContent = qrContent + "\nPrivacy Hash: " + privacyHash;

                // Generate QR code
                Bitmap qrCodeImage = GenerateQRCode(finalQRContent);

                // Save QR code to database and Excel
                SaveQRCodeToDatabaseAndExcel(qrCodeImage, gcNumber, recipient, entitlement, description, dateOfIssue, validity, gcType, chargeTo, status, quantity, privacyHash, hotelBranch, batch, sequenceInfo);

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

            // Make the div containing the ListBox visible
            generatedGCNumbersDiv.Visible = true;

            // Clear form fields after processing
            ClearFormFields();
        }
        private void DisplayQRCode(Bitmap qrCodeImage)
        {
            // Resize the QR code image to 10x10 pixels
            Bitmap resizedImage = ResizeImage(qrCodeImage, 300, 300);

            // Convert the resized image to a base64 string
            string base64Image = ImageToBase64(resizedImage, ImageFormat.Png);

            // Display QR code image
            imgQRCode.ImageUrl = "data:image/png;base64," + base64Image;

            // Make the QR code container visible
            qrCodeContainer.Visible = true;
        }
        private string GenerateSequenceInfo(int current, int total)
        {
            return $"{current} out of {total}";
        }
        public static string GenerateBatchIdentifier()
        {
            Random rand = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int length = 8; // Adjust the length of the identifier as needed

            char[] identifier = new char[length];
            for (int i = 0; i < length; i++)
            {
                identifier[i] = chars[rand.Next(chars.Length)];
            }

            return new string(identifier);
        }
        // Method to save QR code and related information to database and Excel
        private void SaveQRCodeToDatabaseAndExcel(Bitmap qrCodeImage, string gcNumber, string recipient, string entitlement, string description, string dateOfIssue, string validity, string gcType, string chargeTo, string status, int quantity, string privacyHash, string hotelBranch, string batchIdentifier, string sequenceInfo)
        {
            // Convert Bitmap to byte array
            byte[] imageBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                qrCodeImage.Save(ms, ImageFormat.Jpeg); // Save as JPG
                imageBytes = ms.ToArray();
            }

            // Insert QR code and related information into the database
            string query = "INSERT INTO GiftCertificates (GCNumber, Recipient, Entitlement, Description, DateOfIssue, Validity, GCType, ChargeTo, Status, QRCodeImage, Quantity, PrivacyHash, BatchIdentifier, hotelBranch, SequenceInfo) " +
                            "VALUES (@GCNumber, @Recipient, @Entitlement, @Description, @DateOfIssue, @Validity, @GCType, @ChargeTo, @Status, @QRCodeImage, @Quantity, @PrivacyHash, @BatchIdentifier, @hotelBranch, @SequenceInfo)";

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
                    command.Parameters.AddWithValue("@BatchIdentifier", batchIdentifier);
                    command.Parameters.AddWithValue("@hotelBranch", hotelBranch);
                    command.Parameters.AddWithValue("@SequenceInfo", sequenceInfo);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            // Save QR code image to a local folder and get the file path
            string qrCodeImagePath = SaveQRCodeImageToLocalFolder(qrCodeImage, gcNumber);

            // Write data to Excel with QR code image path
            WriteDataToExcel(gcNumber, recipient, entitlement, description, dateOfIssue, validity, gcType, chargeTo, status, quantity, privacyHash, batchIdentifier, hotelBranch, sequenceInfo, qrCodeImagePath);
        }
        private string SaveQRCodeImageToLocalFolder(Bitmap qrCodeImage, string gcNumber)
        {
            string folderPath = Server.MapPath("~/App_Data/QRCodeImages");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, $"{gcNumber}.jpg");
            qrCodeImage.Save(filePath, ImageFormat.Jpeg);

            return filePath;
        }
        private void WriteDataToExcel(string gcNumber, string recipient, string entitlement, string description, string dateOfIssue, string validity, string gcType, string chargeTo, string status, int quantity, string privacyHash, string batchIdentifier, string hotelBranch, string sequenceInfo, string qrCodeImagePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set this according to your licensing scenario

            string filePath = Path.Combine(Server.MapPath("~/App_Data"), "GiftCertificates.xlsx");

            // Check if the Excel file exists, create it if it doesn't
            FileInfo excelFile = new FileInfo(filePath);
            if (!excelFile.Exists)
            {
                using (ExcelPackage package = new ExcelPackage(excelFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("GiftCertificates");

                    // Header row style
                    var headerCells = worksheet.Cells["A1:O1"]; // Adjust "O1" to the last column where your data ends
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(Color.Black);
                    headerCells.Style.Font.Color.SetColor(Color.White);

                    // Header row content
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
                    worksheet.Cells[1, 12].Value = "BatchIdentifier";
                    worksheet.Cells[1, 13].Value = "HotelBranch";
                    worksheet.Cells[1, 14].Value = "SequenceInfo";
                    worksheet.Cells[1, 15].Value = "QRCodeImagePath"; // New column for QR Code Image Path

                    // Set column widths
                    worksheet.Column(1).Width = 15; // Adjust the width as needed
                    worksheet.Column(2).Width = 20; // Adjust the width as needed
                    worksheet.Column(3).Width = 20; // Adjust the width as needed
                    worksheet.Column(4).Width = 30; // Adjust the width as needed
                    worksheet.Column(5).Width = 15; // Adjust the width as needed
                    worksheet.Column(6).Width = 15; // Adjust the width as needed
                    worksheet.Column(7).Width = 15; // Adjust the width as needed
                    worksheet.Column(8).Width = 20; // Adjust the width as needed
                    worksheet.Column(9).Width = 15; // Adjust the width as needed
                    worksheet.Column(10).Width = 10; // Adjust the width as needed
                    worksheet.Column(11).Width = 60; // Adjust the width as needed
                    worksheet.Column(12).Width = 20; // Adjust the width as needed
                    worksheet.Column(13).Width = 35; // Adjust the width as needed
                    worksheet.Column(14).Width = 20; // Adjust the width as needed
                    worksheet.Column(15).Width = 40; // Adjust the width as needed

                    // Data row
                    worksheet.Cells[2, 1].Value = gcNumber;
                    worksheet.Cells[2, 2].Value = recipient;
                    worksheet.Cells[2, 3].Value = entitlement;
                    worksheet.Cells[2, 4].Value = description;
                    worksheet.Cells[2, 5].Value = dateOfIssue;
                    worksheet.Cells[2, 6].Value = validity;
                    worksheet.Cells[2, 7].Value = gcType;
                    worksheet.Cells[2, 8].Value = chargeTo;
                    worksheet.Cells[2, 9].Value = status;
                    worksheet.Cells[2, 10].Value = quantity;
                    worksheet.Cells[2, 11].Value = privacyHash;
                    worksheet.Cells[2, 12].Value = batchIdentifier;
                    worksheet.Cells[2, 13].Value = hotelBranch;
                    worksheet.Cells[2, 14].Value = sequenceInfo;
                    worksheet.Cells[2, 15].Value = qrCodeImagePath; // New cell for QR Code Image Path

                    // Add QR Code image
                    if (!string.IsNullOrEmpty(qrCodeImagePath))
                    {
                        var qrCode = worksheet.Drawings.AddPicture("QRCode_" + gcNumber, new FileInfo(qrCodeImagePath));
                        qrCode.From.Column = 15; // Column index where QR code image should be placed
                        qrCode.From.Row = 1; // Row index where QR code image should be placed
                        qrCode.SetSize(100, 100); // Set the size of the QR code image
                    }

                    package.Save();
                }
            }
            else
            {
                using (ExcelPackage package = new ExcelPackage(excelFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["GiftCertificates"];

                    int rowCount = worksheet.Dimension.Rows;

                    // Header row style (if file already exists, set style for existing header row)
                    var headerCells = worksheet.Cells["A1:O1"]; // Adjust "O1" to the last column where your data ends
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(Color.Black);
                    headerCells.Style.Font.Color.SetColor(Color.White);

                    // Set column widths
                    worksheet.Column(1).Width = 15; // Adjust the width as needed
                    worksheet.Column(2).Width = 20; // Adjust the width as needed
                    worksheet.Column(3).Width = 20; // Adjust the width as needed
                    worksheet.Column(4).Width = 30; // Adjust the width as needed
                    worksheet.Column(5).Width = 15; // Adjust the width as needed
                    worksheet.Column(6).Width = 15; // Adjust the width as needed
                    worksheet.Column(7).Width = 15; // Adjust the width as needed
                    worksheet.Column(8).Width = 20; // Adjust the width as needed
                    worksheet.Column(9).Width = 15; // Adjust the width as needed
                    worksheet.Column(10).Width = 10; // Adjust the width as needed
                    worksheet.Column(11).Width = 60; // Adjust the width as needed
                    worksheet.Column(12).Width = 20; // Adjust the width as needed
                    worksheet.Column(13).Width = 35; // Adjust the width as needed
                    worksheet.Column(14).Width = 20; // Adjust the width as needed
                    worksheet.Column(15).Width = 40; // Adjust the width as needed

                    // Data row
                    worksheet.Cells[rowCount + 1, 1].Value = gcNumber;
                    worksheet.Cells[rowCount + 1, 2].Value = recipient;
                    worksheet.Cells[rowCount + 1, 3].Value = entitlement;
                    worksheet.Cells[rowCount + 1, 4].Value = description;
                    worksheet.Cells[rowCount + 1, 5].Value = dateOfIssue;
                    worksheet.Cells[rowCount + 1, 6].Value = validity;
                    worksheet.Cells[rowCount + 1, 7].Value = gcType;
                    worksheet.Cells[rowCount + 1, 8].Value = chargeTo;
                    worksheet.Cells[rowCount + 1, 9].Value = status;
                    worksheet.Cells[rowCount + 1, 10].Value = quantity;
                    worksheet.Cells[rowCount + 1, 11].Value = privacyHash;
                    worksheet.Cells[rowCount + 1, 12].Value = batchIdentifier;
                    worksheet.Cells[rowCount + 1, 13].Value = hotelBranch;
                    worksheet.Cells[rowCount + 1, 14].Value = sequenceInfo;
                    worksheet.Cells[rowCount + 1, 15].Value = qrCodeImagePath; // New cell for QR Code Image Path

                    // Add QR Code image
                    if (!string.IsNullOrEmpty(qrCodeImagePath))
                    {
                        var qrCode = worksheet.Drawings.AddPicture("QRCode_" + gcNumber, new FileInfo(qrCodeImagePath));
                        qrCode.From.Column = 15; // Column index where QR code image should be placed
                        qrCode.From.Row = rowCount + 1; // Row index where QR code image should be placed
                        qrCode.SetSize(100, 100); // Set the size of the QR code image
                    }

                    package.Save();
                }
            }
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
            int pixelSize = 10; // Smaller QR code
            return qrCode.GetGraphic(pixelSize); // Adjust the size of the QR code image as needed
        }

        //AVAILMENT FUNCTION BELOW 
        protected void BookNowButton_Click(object sender, EventArgs e)
        {
            string message;
            string messageType;

            if (Page.IsValid)
            {
                string giftCardNumber = gcNumber.Text;
                string selectedStatus = searchDDL.SelectedValue;

                // Automatically set status to 'Booked' if selectedStatus is 'Available'
                if (selectedStatus == "Available")
                {
                    selectedStatus = "Booked";
                }

                // Retrieve DateOfIssue and Validity from the database
                DateTime dateOfIssue;
                DateTime dateOfValidity;
                string recipient;
                string entitlement;
                string description;
                string gcType;
                string chargeTo;

                // Declare bookingFromDateParsed and bookingToDateParsed variables
                DateTime? bookingFromDateParsed = null;
                DateTime? bookingToDateParsed = null;

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
                    {
                        string query = "SELECT DateOfIssue, Validity, Recipient, Entitlement, Description, GCType, ChargeTo FROM giftcertificates WHERE GCNumber = @GCNumber";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@GCNumber", giftCardNumber);

                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dateOfIssue = Convert.ToDateTime(reader["DateOfIssue"]);
                                dateOfValidity = Convert.ToDateTime(reader["Validity"]);
                                recipient = reader["Recipient"].ToString();
                                entitlement = reader["Entitlement"].ToString();
                                description = reader["Description"].ToString();
                                gcType = reader["GCType"].ToString();
                                chargeTo = reader["ChargeTo"].ToString();
                            }
                            else
                            {
                                message = "Gift certificate not found.";
                                messageType = "error";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowAlert", $"showMessage('{messageType}', '{message}');", true);
                                return;
                            }

                            // Close the reader after reading data
                            reader.Close();
                        }

                        // Update status and booking dates if applicable
                        string updateQuery;
                        MySqlCommand updateCommand;

                        // Update status only (for 'Lost', 'Replaced', 'Cancelled') or 'Booked'
                        if (selectedStatus == "Booked")
                        {
                            string bookingFrom = bookingFromDate.Text.Trim();
                            string bookingTo = bookingToDate.Text.Trim();
                            bool isValid = true;

                            // Validate booking dates against DateOfIssue and Validity
                            isValid = ValidateBookingDates(bookingFrom, bookingTo, dateOfIssue, dateOfValidity, out bookingFromDateParsed, out bookingToDateParsed);

                            if (!isValid)
                            {
                                return;
                            }

                            // Update status to 'Booked' and booking dates
                            updateQuery = "UPDATE giftcertificates SET BookedFrom = @BookingFrom, BookedTo = @BookingTo, Status = @Status WHERE GCNumber = @GCNumber";
                            updateCommand = new MySqlCommand(updateQuery, connection);
                            updateCommand.Parameters.AddWithValue("@BookingFrom", bookingFromDateParsed);
                            updateCommand.Parameters.AddWithValue("@BookingTo", bookingToDateParsed);
                        }
                        else
                        {
                            // Update status only (for 'Lost', 'Replaced', 'Cancelled')
                            updateQuery = "UPDATE giftcertificates SET Status = @Status WHERE GCNumber = @GCNumber";
                            updateCommand = new MySqlCommand(updateQuery, connection);
                        }

                        // Common parameters for both status updates
                        updateCommand.Parameters.AddWithValue("@GCNumber", giftCardNumber);
                        updateCommand.Parameters.AddWithValue("@Status", selectedStatus);

                        // Execute the update command
                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Regenerate QR code with updated information
                            UpdateQRCode(giftCardNumber, selectedStatus);

                            // Retrieve updated information for display in the scan section
                            string retrieveQuery = "SELECT * FROM giftcertificates WHERE GCNumber = @GCNumber";
                            MySqlCommand retrieveCommand = new MySqlCommand(retrieveQuery, connection);
                            retrieveCommand.Parameters.AddWithValue("@GCNumber", giftCardNumber);

                            using (MySqlDataReader readerUpdate = retrieveCommand.ExecuteReader())
                            {
                                if (readerUpdate.Read())
                                {
                                    // Update UI based on the selected status
                                    if (selectedStatus != "Booked")
                                    {
                                        // Hide booking dates section for statuses other than 'Booked'
                                        bookedFromRow.Visible = false;
                                        bookedToRow.Visible = false;
                                        bookedSection.Visible = true;
                                        bookingSection.Visible = false;
                                        scannerSection.Visible = false;

                                        // show scan qr button
                                        Div3.Visible = true;

                                        message = "Gift certificate updated. Status: " + selectedStatus;
                                        messageType = "success";
                                    }
                                    else
                                    {
                                        // Show booking dates section for 'Booked'
                                        DateTime bookedFromValue = Convert.ToDateTime(readerUpdate["BookedFrom"]);
                                        bookedFrom.Text = bookedFromValue.ToString("dd-MM-yyyy");
                                        DateTime bookedToValue = Convert.ToDateTime(readerUpdate["BookedTo"]);
                                        bookedTo.Text = bookedToValue.ToString("dd-MM-yyyy");

                                        bookedFromRow.Visible = true;
                                        bookedToRow.Visible = true;
                                        bookedSection.Visible = true;
                                        bookingSection.Visible = false;
                                        scannerSection.Visible = false;

                                        // show scan qr button
                                        Div3.Visible = true;

                                        message = "Welcome to the Hotel, enjoy your stay!";
                                        messageType = "success";
                                    }

                                    // Set other display fields
                                    bookedStatus.Text = readerUpdate["Status"].ToString();
                                    bookedRecipient.Text = readerUpdate["Recipient"].ToString();
                                    bookedEntitlement.Text = readerUpdate["Entitlement"].ToString();
                                    bookedDescription.Text = readerUpdate["Description"].ToString();
                                    bookedDOI.Text = Convert.ToDateTime(readerUpdate["DateOfIssue"]).ToString("dd-MM-yyyy");
                                    bookedValidity.Text = Convert.ToDateTime(readerUpdate["Validity"]).ToString("dd-MM-yyyy");
                                }
                                else
                                {
                                    message = "Error retrieving updated information after booking. Please try again.";
                                    messageType = "error";
                                }
                            }
                        }
                        else
                        {
                            message = "Error updating status. Please try again.";
                            messageType = "error";
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = "An error occurred while booking the stay: " + ex.Message;
                    messageType = "error";
                }

                // Show message to user based on booking result
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowAlert", $"showMessage('{messageType}', '{message}');", true);
            }
        }
        private void UpdateQRCode(string giftCardNumber, string status)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
                {
                    string retrieveQuery = "SELECT * FROM giftcertificates WHERE GCNumber = @GCNumber";
                    MySqlCommand retrieveCommand = new MySqlCommand(retrieveQuery, connection);
                    retrieveCommand.Parameters.AddWithValue("@GCNumber", giftCardNumber);

                    connection.Open();

                    // Execute the reader to fetch data
                    using (MySqlDataReader reader = retrieveCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string recipient = reader["Recipient"].ToString();
                            string entitlement = reader["Entitlement"].ToString();
                            string description = reader["Description"].ToString();
                            DateTime dateOfIssue = Convert.ToDateTime(reader["DateOfIssue"]);
                            DateTime dateOfValidity = Convert.ToDateTime(reader["Validity"]);
                            DateTime? bookedFrom = reader["BookedFrom"] as DateTime?;
                            DateTime? bookedTo = reader["BookedTo"] as DateTime?;
                            string gcType = reader["GCType"].ToString();
                            string chargeTo = reader["ChargeTo"].ToString();

                            // Construct QR code content based on status
                            string qrContent;
                            if (status == "Booked")
                            {
                                qrContent = $"GC Number: {giftCardNumber}\nRecipient: {recipient}\nEntitlement: {entitlement}\nDescription: {description}\nDate of Issue: {dateOfIssue.ToString("dd-MM-yyyy")}\nValidity: {dateOfValidity.ToString("dd-MM-yyyy")}\nBooked From: {bookedFrom?.ToString("dd-MM-yyyy")}\nBooked To: {bookedTo?.ToString("dd-MM-yyyy")}\nGC Type: {gcType}\nCharge To: {chargeTo}\nStatus: {status}";
                            }
                            else
                            {
                                qrContent = $"GC Number: {giftCardNumber}\nRecipient: {recipient}\nEntitlement: {entitlement}\nDescription: {description}\nDate of Issue: {dateOfIssue.ToString("dd-MM-yyyy")}\nValidity: {dateOfValidity.ToString("dd-MM-yyyy")}\nStatus: {status}";
                            }

                            // Generate privacy hash and update QR code image
                            string privacyHash = GenerateHash(qrContent, hotelDropDownList.SelectedValue);
                            string finalQRContent = qrContent + "\nPrivacy Hash: " + privacyHash;
                            Bitmap updatedQRCodeImage = GenerateQRCode(finalQRContent);

                            // Convert Bitmap to byte array
                            byte[] imageBytes;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                updatedQRCodeImage.Save(ms, ImageFormat.Jpeg); // Save as JPG
                                imageBytes = ms.ToArray();
                            }

                            // Close the reader before executing another command
                            reader.Close();

                            // Update QR code image in database
                            string updateQRQuery = "UPDATE giftcertificates SET QRCodeImage = @QRCodeImage WHERE GCNumber = @GCNumber";
                            using (MySqlCommand updateQRCommand = new MySqlCommand(updateQRQuery, connection))
                            {
                                updateQRCommand.Parameters.AddWithValue("@GCNumber", giftCardNumber);
                                updateQRCommand.Parameters.AddWithValue("@QRCodeImage", imageBytes);
                                updateQRCommand.ExecuteNonQuery();
                            }

                            // Dispose of Bitmap to release resources
                            updatedQRCodeImage.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating QR code image: " + ex.Message);
            }
        }
        private bool ValidateBookingDates(string bookingFrom, string bookingTo, DateTime dateOfIssue, DateTime dateOfValidity, out DateTime? bookingFromDateParsed, out DateTime? bookingToDateParsed)
        {
            bookingFromDateParsed = null;
            bookingToDateParsed = null;
            bool isValid = true;

            if (!string.IsNullOrEmpty(bookingFrom))
            {
                if (!DateTime.TryParseExact(bookingFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedFrom))
                {
                    rfvBookingFromDate.ErrorMessage = "Invalid date format. Please use yyyy-MM-dd.";
                    rfvBookingFromDate.IsValid = false;
                    isValid = false;
                }
                else
                {
                    bookingFromDateParsed = parsedFrom;

                    if (bookingFromDateParsed < dateOfIssue)
                    {
                        rfvBookingFromDate.ErrorMessage = $"Booking from date cannot be before Date of Issue ({dateOfIssue.ToString("yyyy-MM-dd")}).";
                        rfvBookingFromDate.IsValid = false;
                        isValid = false;
                    }
                }
            }
            else
            {
                rfvBookingFromDate.ErrorMessage = "Booking from date is required.";
                rfvBookingFromDate.IsValid = false;
                isValid = false;
            }

            if (!string.IsNullOrEmpty(bookingTo))
            {
                if (!DateTime.TryParseExact(bookingTo, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTo))
                {
                    rfvBookingToDate.ErrorMessage = "Invalid date format. Please use yyyy-MM-dd.";
                    rfvBookingToDate.IsValid = false;
                    isValid = false;
                }
                else
                {
                    bookingToDateParsed = parsedTo;

                    if (bookingToDateParsed >= dateOfValidity)
                    {
                        rfvBookingToDate.ErrorMessage = $"Booking to date cannot be on or after Validity date ({dateOfValidity.ToString("yyyy-MM-dd")}).";
                        rfvBookingToDate.IsValid = false;
                        isValid = false;
                    }
                    else if (bookingToDateParsed <= bookingFromDateParsed)
                    {
                        rfvBookingToDate.ErrorMessage = "Booking to date must be greater than booking from date.";
                        rfvBookingToDate.IsValid = false;
                        isValid = false;
                    }
                }
            }
            else
            {
                rfvBookingToDate.ErrorMessage = "Booking to date is required.";
                rfvBookingToDate.IsValid = false;
                isValid = false;
            }

            return isValid;
        }




        protected void btnScanQR_Click(object sender, EventArgs e)
        {
            // Register the script to start QR code scanning
            scanCard.Visible = true;
            searchCard.Visible = false;
            createCard.Visible = false;
            scannerSection.Visible = true;
            bookingSection.Visible = false;
            bookedSection.Visible = false;

            btnBackToSearch.Visible = true;
            btnRetake.Visible = true;
            ScriptManager.RegisterStartupScript(this, GetType(), "StartQRCodeScan", "StartQRCodeScan();", true);
        }

        protected void btnAvailBook_Click(object sender, EventArgs e)
        {
            // Retrieve scanned information from hidden fields
            string scannedGC = hidScannedGC.Value;
            string scannedRecipient = hidScannedRecipient.Value;
            string scannedEntitlement = hidScannedEntitlement.Value;

            // Display scanned information
            gcNumber.Text = scannedGC;
            bookingRecipient.Text = scannedRecipient;
            bookingEntitlement.Text = scannedEntitlement;

            // Optionally set the status dropdown to "Available" or any default value
            searchDDL.SelectedValue = "Available";

            // Hide scanCard and show bookingSection (assuming these are the correct IDs)
            scanCard.Visible = false;
            bookingSection.Visible = true;
            BackToScanQR.Visible = true;
        }

        protected void btnBackToSearch_Click(object sender, EventArgs e)
        {
            searchCard.Visible = true;
            createCard.Visible = false;
            scannerSection.Visible = false;
            scanCard.Visible = false;

            // Clear form fields on CREATE when user go to AVAILMENT
            ClearFormFields();
        }

        protected void gcNumber_TextChanged(object sender, EventArgs e)
        {
            string message = "";
            string messageType = "";
            scannerSection.Visible = false;
            string giftCardNumber = gcNumber.Text;
            Div3.Visible = false;

            if (!string.IsNullOrEmpty(giftCardNumber))
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
                    {
                        string query = "SELECT * FROM giftcertificates WHERE GCNumber = @GCNumber";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@GCNumber", giftCardNumber);
                            connection.Open();

                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string status = reader["status"].ToString();

                                    if (status == "Available")
                                    {
                                        bookingFromDate.Text = "";
                                        bookingToDate.Text = "";
                                        searchDDL.SelectedValue = reader["Status"].ToString();
                                        bookingRecipient.Text = reader["Recipient"].ToString();
                                        bookingEntitlement.Text = reader["Entitlement"].ToString();

                                        bookingSection.Visible = true;
                                        bookedSection.Visible = false;
                                        scannerSection.Visible = false;

                                    }
                                    else if (status == "Booked")
                                    {
                                        bookingSection.Visible = false;
                                        bookedSection.Visible = true;
                                        scannerSection.Visible = false;


                                        bookingFromDate.Text = "";
                                        bookingToDate.Text = "";

                                        bookedStatus.Text = reader["Status"].ToString();
                                        bookedRecipient.Text = reader["Recipient"].ToString();
                                        bookedEntitlement.Text = reader["Entitlement"].ToString();
                                        bookedDescription.Text = reader["Description"].ToString();
                                        DateTime dateOfIssue = Convert.ToDateTime(reader["DateOfIssue"]);
                                        bookedDOI.Text = dateOfIssue.ToString("dd-MM-yyyy");
                                        DateTime dateOfValidity = Convert.ToDateTime(reader["Validity"]);
                                        bookedValidity.Text = dateOfValidity.ToString("dd-MM-yyyy");

                                        // Display booking dates
                                        DateTime bookedfrom = Convert.ToDateTime(reader["BookedFrom"]);
                                        bookedFrom.Text = bookedfrom.ToString("dd-MM-yyyy");
                                        DateTime bookedto = Convert.ToDateTime(reader["BookedTo"]);
                                        bookedTo.Text = bookedto.ToString("dd-MM-yyyy");

                                        // show scan qr button
                                        Div3.Visible = true;

                                    }
                                    else if (status == "Lost")
                                    {
                                        bookingSection.Visible = false;
                                        bookedSection.Visible = true;
                                        scannerSection.Visible = false;

                                        bookingFromDate.Text = "";
                                        bookingToDate.Text = "";

                                        bookedStatus.Text = reader["Status"].ToString();
                                        bookedRecipient.Text = reader["Recipient"].ToString();
                                        bookedEntitlement.Text = reader["Entitlement"].ToString();
                                        bookedDescription.Text = reader["Description"].ToString();
                                        DateTime dateOfIssue = Convert.ToDateTime(reader["DateOfIssue"]);
                                        bookedDOI.Text = dateOfIssue.ToString("dd-MM-yyyy");
                                        DateTime dateOfValidity = Convert.ToDateTime(reader["Validity"]);
                                        bookedValidity.Text = dateOfValidity.ToString("dd-MM-yyyy");

                                        // hide booking dates
                                        bookedFromRow.Visible = false;
                                        bookedToRow.Visible = false;

                                        // show scan qr button
                                        Div3.Visible = true;

                                    }
                                    else if (status == "Cancelled")
                                    {
                                        bookingSection.Visible = false;
                                        bookedSection.Visible = true;
                                        scannerSection.Visible = false;


                                        bookingFromDate.Text = "";
                                        bookingToDate.Text = "";

                                        bookedStatus.Text = reader["Status"].ToString();
                                        bookedRecipient.Text = reader["Recipient"].ToString();
                                        bookedEntitlement.Text = reader["Entitlement"].ToString();
                                        bookedDescription.Text = reader["Description"].ToString();
                                        DateTime dateOfIssue = Convert.ToDateTime(reader["DateOfIssue"]);
                                        bookedDOI.Text = dateOfIssue.ToString("dd-MM-yyyy");
                                        DateTime dateOfValidity = Convert.ToDateTime(reader["Validity"]);
                                        bookedValidity.Text = dateOfValidity.ToString("dd-MM-yyyy");

                                        // hide booking dates
                                        bookedFromRow.Visible = false;
                                        bookedToRow.Visible = false;

                                        // show scan qr button
                                        Div3.Visible = true;
                                    }
                                    else if (status == "Replaced")
                                    {
                                        bookingSection.Visible = false;
                                        bookedSection.Visible = true;
                                        scannerSection.Visible = false;


                                        bookingFromDate.Text = "";
                                        bookingToDate.Text = "";

                                        bookedStatus.Text = reader["Status"].ToString();
                                        bookedRecipient.Text = reader["Recipient"].ToString();
                                        bookedEntitlement.Text = reader["Entitlement"].ToString();
                                        bookedDescription.Text = reader["Description"].ToString();
                                        DateTime dateOfIssue = Convert.ToDateTime(reader["DateOfIssue"]);
                                        bookedDOI.Text = dateOfIssue.ToString("dd-MM-yyyy");
                                        DateTime dateOfValidity = Convert.ToDateTime(reader["Validity"]);
                                        bookedValidity.Text = dateOfValidity.ToString("dd-MM-yyyy");

                                        // hide booking dates
                                        bookedFromRow.Visible = false;
                                        bookedToRow.Visible = false;

                                        // show scan qr button
                                        Div3.Visible = true;
                                    }
                                }
                                else
                                {
                                    message = "Gift card not found.";
                                    messageType = "error";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowAlert", $"showMessage('{messageType}', '{message}');", true);

                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = "An error occurred while checking the gift card status: " + ex.Message;
                    messageType = "error";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowAlert", $"showMessage('{messageType}', '{message}');", true);
                }
            }

            // Display the message using JavaScript
        }
        protected void ScanQRButton_Click(object sender, EventArgs e)
        {
            gcNumber.Text = "";
            //statusMessage.Text = "";
            scannerSection.Visible = true;
            bookingSection.Visible = false;
            bookedSection.Visible = false;
        }
        protected void btnScanQRCode_Click(object sender, EventArgs e)
        {
            // Call a JavaScript function to open the device's camera for scanning QR codes
            ScriptManager.RegisterStartupScript(this, GetType(), "StartQRCodeScan", "StartQRCodeScan();", true);
        }
        private void ClearFormFields()
        {
            // Clear TextBoxes
            recipientTextBox.Text = "";
            descriptionTextBox.Text = "";
            dateOfIssueTextBox.Text = "";
            validityTextBox.Text = "";
            chargeToTextBox.Text = "";
            quantityTextBox.Text = "";

            // Reset DropDownLists
            entitlementDropDownList.SelectedIndex = 0;
            gcTypeDropDownList.SelectedIndex = 0;
            hotelDropDownList.SelectedIndex = 0;
            //statusDropDownList.SelectedIndex = 0;
        }      
        private Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            // Create a new bitmap with the specified size
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                g.DrawImage(image, 0, 0, width, height);
            }
            return resizedImage;
        }
        private string ImageToBase64(Bitmap image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }       
        protected void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetButtonVisibility();
        }
        private void SetButtonVisibility()
        {

            if (searchGC.Checked)
            {
                searchCard.Visible = true;
                createCard.Visible = false;
                scannerSection.Visible = false;
                scanCard.Visible = false;

                // Clear form fields on CREATE when user go to AVAILMENT
                ClearFormFields();

            }
            else if (createGC.Checked)
            {
                scannerSection.Visible = false;
                searchCard.Visible = false;
                scanCard.Visible = false;
                bookingSection.Visible = false;
                bookedSection.Visible = false;
                createCard.Visible = true;
            }
            //else if (scanGC.Checked)
            //{
            //    scanCard.Visible = true;
            //    searchCard.Visible = false;
            //    createCard.Visible = false;
            //    scannerSection.Visible = true;

            //    ScriptManager.RegisterStartupScript(this, GetType(), "StartQRCodeScan", "StartQRCodeScan();", true);
            //}
        }
        private static string GetConnectionString()
        {
            string server = "localhost";
            string database = "documentsystem";
            string username = "root";
            string password = "";

            return $"Server={server};Database={database};Uid={username};Pwd={password};";
        }
    }
}