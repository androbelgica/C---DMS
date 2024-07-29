using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Web.UI;
using WIA;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using PdfDocument = PdfSharp.Pdf.PdfDocument;
using PdfPage = PdfSharp.Pdf.PdfPage;
using PdfReader = PdfSharp.Pdf.IO.PdfReader;
using System.Collections.Generic;
using Image = System.Drawing.Image;
//using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;
using Control = System.Web.UI.Control;
using System.Web.UI.WebControls;
using System.Linq;


namespace DMS
{
    public partial class Upload : System.Web.UI.Page
    {
        private List<byte[]> scannedImages = new List<byte[]>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Name"] == null)
            {
                // Redirect to login page if session variable is not set
                Response.Redirect("../LOGIN/Login.aspx");
            }

            // Check if the user has the "Upload Files" permission
            List<string> userPermissions = Session["UserPermissions"] as List<string>;
            if (userPermissions == null || !userPermissions.Contains("Upload Files"))
            {
                // Redirect to Access Denied page if the user does not have the permission
                Response.Redirect("../PAGES/AccessDenied.aspx");
            }

            if (Session["ScannedImages"] != null)
            {
                scannedImages = (List<byte[]>)Session["ScannedImages"];
            }

            if (!IsPostBack)
            {
                PopulateFolderDropdown("Only Me", ddlOnlyMe);
                PopulateFolderDropdown("My Department", ddlMyDepartment);
                PopulateFolderDropdown("Public", ddlPublic);
                // Set the initial image index to -1 if there are no images
                currentImageIndex.Value = scannedImages.Count > 0 ? "0" : "-1";
                DisplayScannedImage(scannedImages.Count > 0 ? 0 : -1);
            }
        }
        
        //BUTTONS
        protected void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetButtonVisibility();
        }
        private void SetButtonVisibility()
        {
            btnUpload.Visible = true;

            if (selectFile.Checked)
            {
                btnScan.Visible = false;
                btnOpenCamera.Visible = false;
                btnCloseScanCamera.Visible = false;
                btnCaptureFileScan.Visible = false;
            }
            else if (scanFile.Checked)
            {
                btnScan.Visible = true;
                btnOpenCamera.Visible = false;
                btnCloseScanCamera.Visible = false;
                btnCaptureFileScan.Visible = false;
            }
            else if (camera.Checked)
            {
                btnScan.Visible = false;
                btnOpenCamera.Visible = true;

                // open camera and hide text on the file display container
        //        string script = @"
        //    hideUploadFileText();
        //";

        //        ScriptManager.RegisterStartupScript(this, GetType(), "StartCameraAndHideText", script, true);

                btnCloseScanCamera.Visible = false;
                btnCaptureFileScan.Visible = false;
            }
        }
        private string GetConnectionString()
        {
            string server = "localhost";
            string database = "documentsysteM";
            string username = "root";
            string password = "";

            return $"Server={server};Database={database};Uid={username};Pwd={password};";
        }
        private byte[] ConvertToPdf(byte[] imageContent)
        {
            using (MemoryStream ms = new MemoryStream(imageContent))
            {
                // Create a PDF document
                PdfDocument pdfDocument = new PdfDocument();
                PdfPage page = pdfDocument.AddPage();

                // Get the graphics object for drawing on the PDF page
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Load the image directly from the memory stream
                XImage xImage = XImage.FromStream(ms);

                // Draw the image onto the PDF page
                gfx.DrawImage(xImage, 0, 0);

                // Save the PDF document to a memory stream
                using (MemoryStream pdfStream = new MemoryStream())
                {
                    pdfDocument.Save(pdfStream, false);
                    return pdfStream.ToArray();
                }
            }
        }
        // Function to determine if the provided image content represents a scanned document
        private bool IsScannedDocument(string base64ImageContent)
        {
            // Convert the base64 image content to byte array
            byte[] imageContent = Convert.FromBase64String(base64ImageContent.Split(',')[1]);

            // Load the image from the byte array
            using (MemoryStream ms = new MemoryStream(imageContent))
            {
                using (Image image = Image.FromStream(ms))
                {
                    return !IsPdf(imageContent);
                }
            }
        }
        // Function to check if the provided byte array represents a PDF file
        private bool IsPdf(byte[] fileContent)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(fileContent))
                {
                    using (PdfDocument pdfDocument = PdfReader.Open(ms, PdfDocumentOpenMode.ReadOnly))
                    {
                        // If the PDF document is opened without any exception, it's a valid PDF
                        return true;
                    }
                }
            }
            catch
            {
                // An exception occurred, indicating it's not a valid PDF
                return false;
            }
        }

        // OCR
        //private string PerformImageOCR(byte[] imageData)
        //{
        //    string extractedText = string.Empty;
        //    using (MemoryStream ms = new MemoryStream(imageData))
        //    {
        //        using (Image image = Image.FromStream(ms))
        //        {
        //            using (Bitmap bitmap = new Bitmap(image))
        //            {
        //                using (var engine = new TesseractEngine(Server.MapPath(@"~/Tesseract-OCR/tessdata"), "eng", EngineMode.Default))
        //                {
        //                    using (var page = engine.Process(bitmap))
        //                    {
        //                        extractedText = page.GetText();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    // Update the OCR text directly to the textarea
        //    ocrTextArea.InnerText = extractedText;
        //    return extractedText;
        //}
        //private string ExtractTextAndImagesFromWordDocument(byte[] fileContent)
        //{
        //    // Create a temporary file path to save the Word document
        //    string tempFilePath = Path.GetTempFileName();
        //    // Write the Word file content to the temporary file
        //    File.WriteAllBytes(tempFilePath, fileContent);

        //    // Create an instance of the Word application
        //    Application wordApp = new Application();
        //    Document doc = null;
        //    try
        //    {
        //        // Open the Word document
        //        doc = wordApp.Documents.Open(tempFilePath);

        //        // Extract text from the document
        //        string text = doc.Content.Text;

        //        // Extract images from the document and perform OCR on them
        //        string ocrTextFromImages = ExtractAndPerformOCROnImages(doc);

        //        // Combine the extracted text and OCR text from images
        //        return text + Environment.NewLine + ocrTextFromImages;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any exceptions
        //        throw ex;
        //    }
        //    finally
        //    {
        //        // Close the document and quit Word application
        //        if (doc != null)
        //        {
        //            doc.Close();
        //            Marshal.ReleaseComObject(doc);
        //        }
        //        wordApp.Quit();
        //        Marshal.ReleaseComObject(wordApp);

        //        // Delete the temporary file
        //        File.Delete(tempFilePath);
        //    }
        //}
        //private string ExtractAndPerformOCROnImages(Document doc)
        //{
        //    string ocrTextFromImages = string.Empty;
        //    int imageCounter = 1;

        //    foreach (InlineShape shape in doc.InlineShapes)
        //    {
        //        if (shape.Type == WdInlineShapeType.wdInlineShapePicture)
        //        {
        //            try
        //            {
        //                // Get the image bytes directly from the shape
        //                byte[] imageBytes = shape.Range.EnhMetaFileBits;

        //                // Perform OCR on the image bytes
        //                string ocrText = PerformImageOCR(imageBytes);

        //                // Append OCR text to the result
        //                ocrTextFromImages += ocrText + Environment.NewLine;

        //                // Increment the image counter
        //                imageCounter++;
        //            }
        //            catch (Exception ex)
        //            {
        //                // Log the exception or handle it as needed
        //                ocrTextFromImages += $"Error processing image {imageCounter}: {ex.Message}" + Environment.NewLine;
        //            }
        //        }
        //    }

        //    return ocrTextFromImages;
        //}

        //  CAMERA FUNCTIONS
        protected void btnCaptureFileScan_Click(object sender, EventArgs e)
        {
            // Store captured image data in hidden field
            hiddenCaptureImageData.Value = Request.Form["cameraImage"]; // Ensure this captures the image data correctly

            // Call the capture script code (if needed)
            string script = @"
        handleCaptureButtonClick(event);
        document.getElementById('" + btnOpenCamera.ClientID + @"').style.display = 'inline-block'; // Display the button
    ";

            ScriptManager.RegisterStartupScript(this, GetType(), "handleCaptureButtonClick", script, true);

            // Set visibility of btnOpenCamera to true (in case the script did not execute)
            btnOpenCamera.Visible = true;
        }

        protected void btnOpenCamera_Click(object sender, EventArgs e)
        {
            // open camera and hide text on the file display container
            string script = @"
                    openCamera();
                    hideUploadFileText();
                ";

            ScriptManager.RegisterStartupScript(this, GetType(), "StartCameraAndHideText", script, true);

            btnCloseScanCamera.Visible = false;
            btnCaptureFileScan.Visible = true;
        }
        protected void btnCloseScanCamera_Click(object sender, EventArgs e)
        {
            // close camera and show text on the file display container
            string script = @"
                    closeCamera();
                    showUploadFileText();
                ";

            ScriptManager.RegisterStartupScript(this, GetType(), "HideCameraAndShowText", script, true);

            btnCloseScanCamera.Visible = false;
            btnCaptureFileScan.Visible = false;
            ocrTextArea.InnerText = "";
        }
        private byte[] ConvertImageToPdf(byte[] imageContent)
        {
            using (MemoryStream ms = new MemoryStream(imageContent))
            {
                try
                {
                    // Reset stream position
                    ms.Position = 0;

                    // Create a PDF document
                    using (PdfDocument pdfDocument = new PdfDocument())
                    {
                        // Add a page to the document
                        PdfPage page = pdfDocument.AddPage();

                        // Get the graphics object for drawing on the PDF page
                        using (XGraphics gfx = XGraphics.FromPdfPage(page))
                        {
                            // Load the image directly from the memory stream
                            using (XImage xImage = XImage.FromStream(ms))
                            {
                                // Calculate the size to fit the image onto the PDF page
                                double pageWidth = page.Width.Point;
                                double pageHeight = page.Height.Point;

                                double imageWidth = xImage.PixelWidth;
                                double imageHeight = xImage.PixelHeight;

                                double ratioWidth = pageWidth / imageWidth;
                                double ratioHeight = pageHeight / imageHeight;
                                double ratio = Math.Min(ratioWidth, ratioHeight);

                                double scaledWidth = imageWidth * ratio;
                                double scaledHeight = imageHeight * ratio;

                                // Calculate position to center image on the page
                                double xPosition = (pageWidth - scaledWidth) / 2;
                                double yPosition = (pageHeight - scaledHeight) / 2;

                                // Draw the image onto the PDF page
                                gfx.DrawImage(xImage, xPosition, yPosition, scaledWidth, scaledHeight);
                            }
                        }

                        // Save the PDF document to a memory stream
                        using (MemoryStream pdfStream = new MemoryStream())
                        {
                            pdfDocument.Save(pdfStream, false);
                            return pdfStream.ToArray();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (log, throw, or handle as appropriate)
                    Console.WriteLine("Error converting image to PDF: " + ex.Message);
                    throw; // Propagate the exception or handle it as per your application's error handling strategy
                }
            }
        }
        // UPLOAD
        //protected void btnUpload_Click(object sender, EventArgs e)
        //{
        //    bool isFileUploaded = false;
        //    string fileNameToUse = string.Empty;
        //    string autofillFileExtension = string.Empty;

        //    // Retrieve captured image data from hidden field
        //    string captureImage = hiddenCaptureImageData.Value;

        //    // Check if scanned images are available in the session
        //    List<byte[]> scannedImages = Session["ScannedImages"] as List<byte[]>;

        //    if (!string.IsNullOrEmpty(hiddenFileContent.Value) && !string.IsNullOrEmpty(hiddenFileName.Value)) // DIGITAL upload function
        //    {
        //        isFileUploaded = true;
        //        // Get the file content from the hidden field
        //        string base64Content = hiddenFileContent.Value;
        //        byte[] fileContent = Convert.FromBase64String(base64Content.Split(',')[1]); 

        //        // Get the original file name from the hidden field
        //        string originalFileName = hiddenFileName.Value;

        //        // Get the current file name from the textbox
        //        string currentFileName = docnametxtbox.Text;

        //        // Use the currentFileName if it's not empty and different from the originalFileName
        //        fileNameToUse = string.IsNullOrEmpty(currentFileName) || currentFileName == originalFileName
        //            ? originalFileName
        //            : currentFileName;

        //        // Extract file extension from the original file name
        //        autofillFileExtension = Path.GetExtension(originalFileName);

        //        // If the document name does not end with the file extension, append it
        //        if (!fileNameToUse.EndsWith(autofillFileExtension, StringComparison.OrdinalIgnoreCase))
        //        {
        //            fileNameToUse += autofillFileExtension;
        //        }

        //        // Get the current logged-in user's name from the session
        //        string uploaderName = Session["Name"] != null ? Session["Name"].ToString() : "Unknown";

        //        // Determine the privacy level based on the selected radio option
        //        string privacyOption = GetSelectedPrivacyOption();
        //        if (!rbOnlyMe.Checked && !rbMyDepartment.Checked && !rbPublic.Checked)
        //        {
        //            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Please select a privacy option.');", true);
        //            return;
        //        }

        //        int folderID = GetSelectedFolderID(privacyOption);

        //        string category = "Digital";

        //        // Check if the uploaded file is a text file
        //        string fileExtension = System.IO.Path.GetExtension(originalFileName);
        //        if (fileExtension.Equals(".xls", StringComparison.OrdinalIgnoreCase) || fileExtension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        //        {
        //            // Perform text extraction for Excel file content
        //            string extractedText = ocrTextArea.InnerText; // Get extracted text from the hidden field

        //            RecordFileUpload(fileNameToUse, uploaderName, privacyOption, category, folderID, fileContent, extractedText);
        //        }
        //        else if (fileExtension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
        //        {
        //            // Perform OCR on the text file content
        //            string ocrText = Encoding.UTF8.GetString(fileContent); // Assuming text file content is encoded in UTF-8

        //            RecordFileUpload(fileNameToUse, uploaderName, privacyOption, category, folderID, fileContent, ocrText);
        //        }
        //        else if (fileExtension.Equals(".doc", StringComparison.OrdinalIgnoreCase) || fileExtension.Equals(".docx", StringComparison.OrdinalIgnoreCase))
        //        {
        //            // Extract text and images from Word file content
        //            string extractedText = ExtractTextAndImagesFromWordDocument(fileContent);

        //            RecordFileUpload(fileNameToUse, uploaderName, privacyOption, category, folderID, fileContent, extractedText);
        //        }
        //        else if (fileExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        //        {
        //            // Extract text from the PDF file using OCR
        //            string extractedText = ocrTextArea.InnerText;

        //            RecordFileUpload(fileNameToUse, uploaderName, privacyOption, category, folderID, fileContent, extractedText);
        //        }
        //        else
        //        {
        //            // Handle image OCR (existing logic)
        //            string ocrText = PerformImageOCR(fileContent); // Call your existing image OCR function

        //            RecordFileUpload(fileNameToUse, uploaderName, privacyOption, category, folderID, fileContent, ocrText);
        //        }

        //        // Clear form fields
        //        hiddenScannedImageContent.Value = "";
        //        docnametxtbox.Text = "";
        //        selectedCategory.Value = "";
        //        rbOnlyMe.Checked = false;
        //        rbMyDepartment.Checked = false;
        //        rbPublic.Checked = false;
        //    }

        //    if (!string.IsNullOrEmpty(hiddenScannedImageContent.Value)) //  SINGLE SCAN upload function
        //    {
        //        isFileUploaded = true;
        //        string base64ScannedContent = hiddenScannedImageContent.Value;
        //        byte[] scannedFileContent = Convert.FromBase64String(base64ScannedContent.Split(',')[1]);

        //        // Get the current file name from the textbox
        //        string currentFileName = docnametxtbox.Text;

        //        // Validate the currentFileName to ensure it is not empty
        //        if (string.IsNullOrEmpty(currentFileName))
        //        {
        //            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Please enter a document name.');", true);
        //            return;
        //        }

        //        // Use the currentFileName as the file name to use
        //        fileNameToUse = currentFileName;

        //        // Get the current logged-in user's name from the session
        //        string uploaderName = Session["Name"] != null ? Session["Name"].ToString() : "Unknown";

        //        // Determine the privacy level based on the selected radio option and folder
        //        string privacyOption = GetSelectedPrivacyOption();
        //        if (!rbOnlyMe.Checked && !rbMyDepartment.Checked && !rbPublic.Checked)
        //        {
        //            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Please select a privacy option.');", true);
        //            return;
        //        }

        //        int folderID = GetSelectedFolderID(privacyOption);

        //        string category = "Scanned";

        //        // Check if the uploaded file is a scanned document
        //        bool isScannedDocument = IsScannedDocument(base64ScannedContent);

        //        // Convert scanned documents to PDF
        //        if (isScannedDocument)
        //        {
        //            // Convert the scanned document to PDF
        //            scannedFileContent = ConvertToPdf(scannedFileContent);

        //            // Append .pdf to the filename
        //            if (!fileNameToUse.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        //            {
        //                fileNameToUse += ".pdf";
        //            }
        //        }

        //        // Perform OCR on the uploaded file
        //        string ocrText = ocrTextArea.InnerText;

        //        RecordFileUpload(fileNameToUse, uploaderName, privacyOption, category, folderID, scannedFileContent, ocrText);

        //        // Clear the hidden field for the scanned image content
        //        hiddenScannedImageContent.Value = "";
        //        docnametxtbox.Text = "";
        //        selectedCategory.Value = "";
        //        rbOnlyMe.Checked = false;
        //        rbMyDepartment.Checked = false;
        //        rbPublic.Checked = false;
        //        ocrTextArea.InnerText = "";
        //    }
        //    if (scannedImages != null && scannedImages.Count > 0) // MULTIPLE SCAN upload function
        //    {
        //        btnNext.Visible = false;
        //        btnPrevious.Visible = false;
        //        btnDelete.Visible = false;
        //        // If there are scanned images, handle the upload process for multiple scans
        //        isFileUploaded = true;

        //        // Get the document name from the textbox
        //        string currentFileName = docnametxtbox.Text;

        //        // Validate the document name to ensure it is not empty
        //        if (string.IsNullOrEmpty(currentFileName))
        //        {
        //            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Please enter a document name.');", true);
        //            return;
        //        }

        //        // Use the currentFileName as the file name to use
        //        fileNameToUse = currentFileName;

        //        // Append ".pdf" to the filename if it doesn't already have it
        //        if (!fileNameToUse.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        //        {
        //            fileNameToUse += ".pdf";
        //        }

        //        // Get the current logged-in user's name from the session
        //        string uploaderName = Session["Name"] != null ? Session["Name"].ToString() : "Unknown";

        //        // Determine the privacy level based on the selected radio option
        //        string privacyOption = GetSelectedPrivacyOption();
        //        if (!rbOnlyMe.Checked && !rbMyDepartment.Checked && !rbPublic.Checked)
        //        {
        //            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Please select a privacy option.');", true);
        //            return;
        //        }

        //        int folderID = GetSelectedFolderID(privacyOption);

        //        string category = "Scanned";

        //        // Combine the scanned images into a single PDF
        //        byte[] combinedPdf = CombineImagesToPdf(scannedImages);

        //        string ocrText = ocrTextArea.InnerText;

        //        RecordFileUpload(fileNameToUse, uploaderName, privacyOption, category, folderID, combinedPdf, ocrText);

        //        // Clear form fields
        //        docnametxtbox.Text = "";
        //        selectedCategory.Value = "";
        //        rbOnlyMe.Checked = false;
        //        rbMyDepartment.Checked = false;
        //        rbPublic.Checked = false;
        //        ocrTextArea.InnerText = "";

        //        // Clear the scanned images list
        //        scannedImages.Clear();

        //        // Clear the session variable
        //        Session["ScannedImages"] = null;
        //    }
        //    if (!string.IsNullOrEmpty(captureImage)) // CAMERA upload function
        //    {
        //        isFileUploaded = true;

        //        // Convert captured image data to byte array
        //        byte[] imageBytes = Convert.FromBase64String(captureImage.Split(',')[1]);

        //        // Convert captured image to PDF (if necessary)
        //        byte[] pdfBytes = ConvertImageToPdf(imageBytes); // Implement ConvertImageToPdf method

        //        // Get the document name from the textbox
        //        string currentFileName = docnametxtbox.Text;

        //        // Validate the document name to ensure it is not empty
        //        if (string.IsNullOrEmpty(currentFileName))
        //        {
        //            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Please enter a document name.');", true);
        //            return;
        //        }

        //        // Use the currentFileName as the file name to use
        //        fileNameToUse = currentFileName;

        //        // Append ".pdf" to the filename if it doesn't already have it
        //        if (!fileNameToUse.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        //        {
        //            fileNameToUse += ".pdf";
        //        }

        //        // Get uploader name and privacy option
        //        string uploaderName = Session["Name"] != null ? Session["Name"].ToString() : "Unknown";
        //        string privacyOption = GetSelectedPrivacyOption();
        //        int folderID = GetSelectedFolderID(privacyOption);
        //        string category = "Camera";

        //        // Get OCR text (assuming it's stored in ocrTextArea.InnerText)
        //        string ocrText = ocrTextArea.InnerText; // Implement this as per your logic

        //        // Record file upload to the database
        //        RecordFileUpload(fileNameToUse, uploaderName, privacyOption, category, folderID, pdfBytes, ocrText);

        //        // Clear hidden field for captured image data
        //        hiddenCaptureImageData.Value = "";

        //        // Clear form fields
        //        docnametxtbox.Text = "";
        //        selectedCategory.Value = "";
        //        rbOnlyMe.Checked = false;
        //        rbMyDepartment.Checked = false;
        //        rbPublic.Checked = false;

        //        btnCloseScanCamera.Visible = false;
        //        btnCaptureFileScan.Visible = false;
        //        ocrTextArea.InnerText = "";
        //    }

        //    if (!isFileUploaded)
        //    {
        //        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Please select a file or scan a document.');", true);
        //    }
        //    else
        //    {
        //        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('File uploaded successfully!');", true);
        //    }
        //}
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

            // Base query to fetch folders
            string query = @"
        SELECT FolderID, FolderName 
        FROM Folders 
        WHERE Privacy = @Privacy 
        AND (
            Privacy = 'Public' 
            OR (Privacy = 'My Department' AND DepartmentID = @DepartmentID)
            OR (@Privacy = 'Only Me' AND CreatedBy = @name)
        )";

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Privacy", privacyOption);
                command.Parameters.AddWithValue("@DepartmentID", departmentID);
                command.Parameters.AddWithValue("@name", name);

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
        private void RecordFileUpload(string fileName, string uploaderName, string privacyOption, string category, int folderID, byte[] fileContent, string ocrText)
        {
            // Connect to the database and record the file upload information
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = "INSERT INTO files (UploaderName, FolderID, FileName, UploadDateTime, Activity, Status, Privacy, Category, FileContent, OCRText) " +
                               "VALUES (@UploaderName, @FolderID, @FileName, @UploadDateTime, @Activity, @Status, @Privacy, @Category, @FileContent, @OCRText)";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UploaderName", uploaderName);
                    command.Parameters.AddWithValue("@FolderID", folderID);
                    command.Parameters.AddWithValue("@FileName", fileName);
                    command.Parameters.AddWithValue("@UploadDateTime", DateTime.Now);
                    command.Parameters.AddWithValue("@Activity", "Uploaded");
                    command.Parameters.AddWithValue("@Status", "Successful");
                    command.Parameters.AddWithValue("@Privacy", privacyOption);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@FileContent", fileContent);
                    command.Parameters.AddWithValue("@OCRText", ocrText);

                    command.ExecuteNonQuery();
                }
            }
        }
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
            else
            {
                // Default 
                return "Only Me";
            }
        }

        // CLEAR
        protected void btnClearScannedImage_Click(object sender, EventArgs e)
        {
            // Clear the scanned image container
            ClearScannedImage();
        }
        private void ClearScannedImage()
        {
            // Clear the container
            filedisplayContainer.Controls.Clear();
        }


        /// ------ SCANNER
    //    protected void btnScan_Click(object sender, EventArgs e)
    //    {
    //        Image scannedImage = ScanUsingWIA();

    //        if (scannedImage != null)
    //        {
    //            byte[] imageData = ImageToByteArray(scannedImage);

    //            // Perform OCR on the scanned image data
    //            string ocrText = PerformImageOCR(imageData);

    //            // Update the OCR text in the textarea
    //            // Append the new OCR text to the existing content of ocrTextArea
    //            ocrTextArea.InnerText += Environment.NewLine + ocrText;

    //            // Add the new scanned image to the list
    //            scannedImages.Add(imageData);

    //            // Update the session variable with the new list
    //            Session["ScannedImages"] = scannedImages;

    //            // Display the latest scanned image
    //            DisplayScannedImage(scannedImages.Count - 1);

    //            // Update the current image index
    //            currentImageIndex.Value = (scannedImages.Count - 1).ToString();

    //            ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Scanned image added successfully!');", true);

    //            ScriptManager.RegisterStartupScript(this, GetType(), "AutoFillCategoryAndNameAndHideText",
    //"$('#dropdownMenuButton').val('Scanned'); hideUploadFileText();", true);

    //            btnNext.Visible = true;
    //            btnPrevious.Visible = true;
    //            btnDelete.Visible = true;
    //        }
    //        else
    //        {
    //            ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Scanning failed. Please make sure only the scanner is connected.');", true);
    //        }
    //    }
        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (scannedImages == null || scannedImages.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('No scanned images available.');", true);
                DisplayScannedImage(-1);
                return;
            }

            int currentIndex = int.Parse(currentImageIndex.Value);

            if (currentIndex < scannedImages.Count - 1)
            {
                currentIndex++;
                currentImageIndex.Value = currentIndex.ToString();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('You are already viewing the LATEST scanned image.');", true);
            }

            DisplayScannedImage(currentIndex);
            ScriptManager.RegisterStartupScript(this, GetType(), "hideUploadFileText", "hideUploadFileText();", true);
        }
        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            if (scannedImages == null || scannedImages.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('No scanned images available.');", true);
                DisplayScannedImage(-1);
                return;
            }

            int currentIndex = int.Parse(currentImageIndex.Value);

            if (currentIndex > 0)
            {
                currentIndex--;
                currentImageIndex.Value = currentIndex.ToString();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('You are already viewing the FIRST scanned image.');", true);
            }

            DisplayScannedImage(currentIndex);
            ScriptManager.RegisterStartupScript(this, GetType(), "hideUploadFileText", "hideUploadFileText();", true);
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (scannedImages == null || scannedImages.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('No scanned images available.');", true);
                DisplayScannedImage(-1);
                return;
            }

            // Remove the last scanned image
            scannedImages.RemoveAt(scannedImages.Count - 1);

            // Remove the corresponding OCR text from ocrTextArea
            RemoveLastOCRText();

            // Update the session variable with the updated list
            Session["ScannedImages"] = scannedImages;

            // Display the new last scanned image if there are remaining images
            if (scannedImages.Count > 0)
            {
                DisplayScannedImage(scannedImages.Count - 1);
            }
            else
            {
                // No images left to display
                DisplayScannedImage(-1);
                btnNext.Visible = false;
                btnPrevious.Visible = false;
                btnDelete.Visible = false;
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Last scanned image deleted successfully.');", true);
        }
        private void RemoveLastOCRText()
        {
            // Split the current text in ocrTextArea by new line
            string[] lines = ocrTextArea.InnerText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            // Remove the last line (which corresponds to the last OCR text)
            if (lines.Length > 0)
            {
                ocrTextArea.InnerText = string.Join(Environment.NewLine, lines.Take(lines.Length - 1));
            }
            else
            {
                ocrTextArea.InnerText = string.Empty;
            }
        }
        private void DisplayScannedImage(int index)
        {
            filedisplayContainer.Controls.Clear();

            if (index >= 0 && index < scannedImages.Count)
            {
                byte[] imageData = scannedImages[index];
                System.Web.UI.WebControls.Image webImage = new System.Web.UI.WebControls.Image();
                webImage.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(imageData);
                filedisplayContainer.Controls.Add(webImage);
            }
        }
        private Image ScanUsingWIA()
        {
            try
            {
                var deviceManager = new DeviceManager();
                var device = deviceManager.DeviceInfos[1].Connect();

                if (device != null)
                {
                    var item = device.Items[1];
                    var imageFile = (ImageFile)item.Transfer();
                    if (imageFile != null)
                    {
                        return Image.FromStream(new MemoryStream((byte[])imageFile.FileData.get_BinaryData()));
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }
        private void SendScannedImageToClient(byte[] imageData)
        {
            // Clear the container before adding the new scanned image
            Control container = FindControlRecursive(Page, "filedisplayContainer");
            if (container != null && container is System.Web.UI.HtmlControls.HtmlGenericControl)
            {
                ((System.Web.UI.HtmlControls.HtmlGenericControl)container).Controls.Clear();

                // Create an image control to display the scanned image
                System.Web.UI.WebControls.Image scannedImage = new System.Web.UI.WebControls.Image();
                scannedImage.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(imageData);

                // Add the image control to the scannedImageContainer
                ((System.Web.UI.HtmlControls.HtmlGenericControl)container).Controls.Add(scannedImage);

                // Store the scanned image data in the hidden field
                hiddenScannedImageContent.Value = scannedImage.ImageUrl;
            }
            else
            {
                // If the container is not found or not of the expected type, show an error message
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Failed to find or cast the container for displaying the scanned image.');", true);
            }
        }
        // Recursive method to find a control within naming containers
        private Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
            {
                return root;
            }

            foreach (Control control in root.Controls)
            {
                Control foundControl = FindControlRecursive(control, id);
                if (foundControl != null)
                {
                    return foundControl;
                }
            }

            return null;
        }
        private byte[] CombineImagesToPdf(List<byte[]> imageContents)
        {
            using (PdfDocument pdfDocument = new PdfDocument())
            {
                foreach (byte[] imageContent in imageContents)
                {
                    using (MemoryStream ms = new MemoryStream(imageContent))
                    {
                        Image image = Image.FromStream(ms);
                        PdfPage page = pdfDocument.AddPage();
                        XGraphics gfx = XGraphics.FromPdfPage(page);

                        // Reset the memory stream position to the beginning
                        ms.Position = 0;

                        // Load the image directly from the memory stream
                        XImage xImage = XImage.FromStream(ms);

                        // Calculate the size to fit the image on the page while maintaining aspect ratio
                        double width = page.Width;
                        double height = page.Height;
                        double imageRatio = (double)image.Width / (double)image.Height;
                        double pageRatio = width / height;

                        if (imageRatio > pageRatio)
                        {
                            // Image is wider than the page
                            width = page.Width;
                            height = page.Width / imageRatio;
                        }
                        else
                        {
                            // Image is taller than or equal to the page
                            height = page.Height;
                            width = page.Height * imageRatio;
                        }

                        // Calculate the position to center the image on the page
                        double xPos = (page.Width - width) / 2;
                        double yPos = (page.Height - height) / 2;

                        gfx.DrawImage(xImage, xPos, yPos, width, height);
                    }
                }

                using (MemoryStream pdfStream = new MemoryStream())
                {
                    pdfDocument.Save(pdfStream, false);
                    return pdfStream.ToArray();
                }
            }
        }
    }
}
