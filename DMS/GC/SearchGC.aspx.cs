using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using QRCoder;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;
using ZXing;
using PdfSharp.Pdf.IO;
using ZXing.QrCode.Internal;
using QRCode = QRCoder.QRCode;
using PdfReader = iTextSharp.text.pdf.PdfReader;

namespace DMS.GC
{
    public partial class SearchGC : System.Web.UI.Page
    {
        //protected TextBox txtGiftCode;
        //protected TextBox txtTo;
        //protected TextBox txtEntitlement;
        //protected TextBox txtDateOfIssue;
        //protected TextBox txtValidity;
        protected TextBox txtGiftCodeAutoFill;
        //protected System.Web.UI.WebControls.Image imgQRCode;
        //protected GridView gvGiftCertificates;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            }
            if (!IsPostBack && Request.QueryString["action"] == "downloadPDF")
            {
                GenerateAndDownloadPDF();
            }
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateQRCode();
            InsertDataIntoDatabase();
        }

        private void InsertDataIntoDatabase()
        {
            string connectionString = GetConnectionString();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "INSERT INTO GiftCertificates (GiftCode, ToRecipient, Entitlement, DateOfIssue, Validity) VALUES (@GiftCode, @ToRecipient, @Entitlement, @DateOfIssue, @Validity)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@GiftCode", txtGiftCode.Text.Trim());
                        command.Parameters.AddWithValue("@ToRecipient", txtTo.Text.Trim());
                        command.Parameters.AddWithValue("@Entitlement", txtEntitlement.Text.Trim());
                        command.Parameters.AddWithValue("@DateOfIssue", txtDateOfIssue.Text.Trim());
                        command.Parameters.AddWithValue("@Validity", txtValidity.Text.Trim());

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting data into database: {ex.Message}");
            }
        }

        protected void btnAutofill_Click(object sender, EventArgs e)
        {
            FillFormFromDatabase(txtGiftCodeAutoFill.Text);
        }

        protected void btnSearchGC_Click(object sender, EventArgs e)
        {
            FillFormFromDatabase(txtGiftCode.Text);
        }

        private void FillFormFromDatabase(string giftCode)
        {
            var data = GetDataFromDatabase(giftCode);
            if (data != null)
            {
                txtGiftCode.Text = data[0];
                txtTo.Text = data[1];
                txtEntitlement.Text = data[2];
                txtDateOfIssue.Text = data[3];
                txtValidity.Text = data[4];

                GenerateQRCode();
            }
            else
            {
                ClearFormFields();
                ClearQRCode();
            }
        }

        private void ClearQRCode()
        {
            imgQRCode.ImageUrl = "";
        }

        private void ClearFormFields()
        {
            txtGiftCode.Text = "";
            txtTo.Text = "";
            txtEntitlement.Text = "";
            txtDateOfIssue.Text = "";
            txtValidity.Text = "";
        }

        private void GenerateQRCode()
        {
            string qrContent = $"Gift Code: {txtGiftCode.Text.Trim()}\nTo: {txtTo.Text.Trim()}\nEntitlement: {txtEntitlement.Text.Trim()}\nDate of Issue: {txtDateOfIssue.Text.Trim()}\nValidity: {txtValidity.Text.Trim()}";
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);

                using (Bitmap qrCodeImage = qrCode.GetGraphic(8, System.Drawing.Color.FromArgb(45, 52, 54), System.Drawing.Color.FromArgb(240, 240, 240), true))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] byteImage = ms.ToArray();
                        imgQRCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                    }
                }
            }
        }
        protected void btnScanQRCode_Click(object sender, EventArgs e)
        {
            // Call a JavaScript function to open the device's camera for scanning QR codes
            ScriptManager.RegisterStartupScript(this, GetType(), "StartQRCodeScan", "StartQRCodeScan();", true);
        }
        protected void btnGeneratePDF_Click(object sender, EventArgs e)
        {
            GenerateAndDownloadPDF();
            qrDiv.Attributes["class"] = qrDiv.Attributes["class"].Replace("d-none", "");
        }

        private void GenerateAndDownloadPDF()
        {
            string giftCode = txtGiftCode.Text.Trim();
            string to = txtTo.Text.Trim();
            string entitlement = txtEntitlement.Text.Trim();
            string dateOfIssue = txtDateOfIssue.Text.Trim();
            string validity = txtValidity.Text.Trim();
            string qrCodeBase64 = imgQRCode.ImageUrl.Split(',')[1];

            byte[] pdfBytes = GeneratePDF(giftCode, to, entitlement, dateOfIssue, validity, qrCodeBase64);
            if (pdfBytes != null)
            {
                SendPDFToClient(pdfBytes, $"GiftCertificate_{giftCode}.pdf");
            }
        }

        private byte[] GeneratePDF(string giftCode, string to, string entitlement, string dateOfIssue, string validity, string qrCodeBase64)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Set custom page size to 7x3 inches landscape
                iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(7 * 72, 3 * 72); // 72 points per inch
                Document document = new Document(pageSize);

                try
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    document.Open();
                    string templatePath = Server.MapPath("~/templates/draft.pdf");
                    PdfReader reader = new PdfReader(templatePath);
                    PdfImportedPage firstPage = writer.GetImportedPage(reader, 1);
                    PdfImportedPage secondPage = writer.GetImportedPage(reader, 2);
                    PdfContentByte canvas = writer.DirectContent;

                    // Scale the template to fit the new page size
                    float scaleX = pageSize.Width / firstPage.Width;
                    float scaleY = pageSize.Height / firstPage.Height;
                    float scale = Math.Min(scaleX, scaleY); // Ensure the template fits within the page dimensions

                    // Add the first page
                    canvas.AddTemplate(firstPage, scale, 0, 0, scale, 0, 0);
                    AddTextAndQRCode(canvas, pageSize, giftCode, to, entitlement, dateOfIssue, validity, qrCodeBase64);

                    // Start a new page
                    document.NewPage();

                    // Add the second page without modification
                    canvas.AddTemplate(secondPage, scale, 0, 0, scale, 0, 0);

                    document.Close();
                    return ms.ToArray();
                }
                catch (Exception ex)
                {
                    Response.Write($"Error: {ex.Message}");
                    return null;
                }
                finally
                {
                    if (document.IsOpen())
                    {
                        document.Close();
                    }
                }
            }
        }


        private void AddTextAndQRCode(PdfContentByte canvas, iTextSharp.text.Rectangle pageSize, string giftCode, string to, string entitlement, string dateOfIssue, string validity, string qrCodeBase64)
        {
            // Adjust font size for smaller text
            iTextSharp.text.Font font = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            // Center coordinates for the text fields
            float centerX = pageSize.Width / 2;

            // Define Y positions (adjust these based on your template)
            float toY = pageSize.Height - 100f;
            float entitlementY = toY - 20f;
            float dateOfIssueY = entitlementY - 30f;
            float validityY = dateOfIssueY - 30f;


            // Draw the text aligned to the center
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase(to, font), centerX, toY, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase(entitlement, font), centerX, entitlementY, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase(dateOfIssue, font), centerX, dateOfIssueY, 0);
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase(validity, font), centerX, validityY, 0);

            // QR code coordinates
            float qrCodeX = pageSize.Width - 50; // Right align
            float qrCodeY = pageSize.Height - 50f; // Top align

            byte[] qrCodeBytes = Convert.FromBase64String(qrCodeBase64);
            iTextSharp.text.Image qrCodeImage = iTextSharp.text.Image.GetInstance(qrCodeBytes);
            qrCodeImage.SetAbsolutePosition(qrCodeX, qrCodeY);
            qrCodeImage.ScaleToFit(60f, 60f); // Adjust size as needed
            canvas.AddImage(qrCodeImage);
        }





        private static System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {
            var codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }



        private void SendPDFToClient(byte[] pdfBytes, string fileName)
        {
            try
            {
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
                Response.BinaryWrite(pdfBytes);
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                Response.Write($"Error sending PDF to client: {ex.Message}");
            }
        }

        private string[] GetDataFromDatabase(string giftCode)
        {
            string[] data = new string[5];
            string connectionString = GetConnectionString();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT * FROM GiftCertificates WHERE GiftCode = @GiftCode";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@GiftCode", giftCode);
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                data[0] = reader["GiftCode"].ToString();
                                data[1] = reader["ToRecipient"].ToString();
                                data[2] = reader["Entitlement"].ToString();
                                data[3] = reader["DateOfIssue"].ToString();
                                data[4] = reader["Validity"].ToString();
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"MySQL Error: {ex.Message}");
                return null;
            }

            return data;
        }
        protected void btnNavigateToDatabase_Click(object sender, EventArgs e)
        {
            Response.Redirect("StoredGiftCertificates.aspx");
        }

        private string GetConnectionString()
        {
            string server = "localhost";
            string database = "GiftCertificateDB";
            string username = "root";
            string password = "";
            return $"Server={server};Database={database};Uid={username};Pwd={password};";
        }
    }
}
