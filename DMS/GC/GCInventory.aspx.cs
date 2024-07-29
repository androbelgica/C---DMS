using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Imaging;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using GridView = System.Web.UI.WebControls.GridView;
using Button = System.Web.UI.WebControls.Button;
using System.Globalization;
using Publisher = Microsoft.Office.Interop.Publisher;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Publisher;
using System.IO;
using ClosedXML.Excel;
using System.Drawing;
using ClosedXML.Excel.Drawings;

namespace DMS.GC
{
    public partial class GCInventory : System.Web.UI.Page
    {
        private string batchIdentifier;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadGiftCertificates();
            }

            if (Session["SearchText"] != null)
            {
                BindGridViewWithSearch();
            }
            else if (!IsPostBack && ViewState["SelectedStatus"] != null)
            {
                // Only load data by status if it's not a postback and ViewState is set
                FilterDataByStatus(ViewState["SelectedStatus"].ToString());
            }

            // Initialize batchIdentifier if it's not a postback
            if (!IsPostBack)
            {
                batchIdentifier = hfBatchIdentifier.Value;
            }

            if (IsPostBack)
            {
                // Check if the postback is triggered by the grid row click
                string eventTarget = Request["__EVENTTARGET"];
                if (eventTarget == UpdatePanel1.ClientID)
                {
                    string batchId = hfBatchIdentifier.Value;
                    LoadBatchDetails(batchId);
                }
            }

            BindPagination();
            UpdatePageIndexTextBox();
        }
        protected void LoadBatchDetails(string batchIdentifier)
        {
            string query = @"
        SELECT 
            GCNumber, 
            Recipient, 
            Entitlement, 
            Description, 
            DATE_FORMAT(DateOfIssue, '%Y-%m-%d') AS DateOfIssue, 
            DATE_FORMAT(Validity, '%Y-%m-%d') AS Validity, 
            GCType, 
            ChargeTo, 
            Status, 
            Quantity,
            QRCodeImage
        FROM 
            giftcertificates 
        WHERE 
            BatchIdentifier = @BatchIdentifier";

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BatchIdentifier", batchIdentifier);
                    connection.Open();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            gvBatchDetails.DataSource = dataSet.Tables[0];
                            gvBatchDetails.DataBind();

                            // Calculate total pages for modal pagination
                            int totalRows = dataSet.Tables[0].Rows.Count;
                            int pageSize = gvBatchDetails.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                            lblTotalPagesModal.Text = totalPages.ToString();
                            lblPageNumModal.Text = (gvBatchDetails.PageIndex + 1).ToString();
                        }
                        else
                        {
                            gvBatchDetails.DataSource = null;
                            gvBatchDetails.DataBind();
                            lblTotalPagesModal.Text = "0";
                            lblPageNumModal.Text = "0";
                        }

                        // Open the modal using JavaScript
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModal", "showModal();", true);
                    }
                }
            }
        }
        private void LoadGiftCertificates()
        {
            string query = @"
SELECT 
    GCNumber, 
    Recipient, 
    Entitlement, 
    Description, 
    DATE_FORMAT(DateOfIssue, '%Y-%m-%d') AS DateOfIssue, 
    DATE_FORMAT(Validity, '%Y-%m-%d') AS Validity, 
    GCType, 
    ChargeTo, 
    Status, 
    DATE_FORMAT(BookedFrom, '%Y-%m-%d') AS BookedFrom, 
    DATE_FORMAT(BookedTo, '%Y-%m-%d') AS BookedTo, 
    Quantity, 
    QRCodeImage,
    BatchIdentifier
FROM 
    (SELECT 
        GCNumber, 
        Recipient, 
        Entitlement, 
        Description, 
        DateOfIssue, 
        Validity, 
        GCType, 
        ChargeTo, 
        Status, 
        BookedFrom, 
        BookedTo, 
        Quantity, 
        QRCodeImage,
        BatchIdentifier,
        ROW_NUMBER() OVER (PARTITION BY BatchIdentifier ORDER BY ID DESC) AS RowNum,
        ID
    FROM 
        giftcertificates) AS Subquery
WHERE 
    Subquery.RowNum = 1
ORDER BY 
    ID DESC";

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            DataSet dataSet = new DataSet();
                            adapter.Fill(dataSet);

                            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                            {
                                gvGiftCertificates.DataSource = dataSet.Tables[0];
                                gvGiftCertificates.DataBind();
                                int totalRows = dataSet.Tables[0].Rows.Count;
                                int pageSize = gvGiftCertificates.PageSize;
                                int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                                lblTotalPages.Text = totalPages.ToString();
                                BindPagination();
                            }
                            else
                            {
                                // Handle case where no data is returned
                                gvGiftCertificates.DataSource = null;
                                gvGiftCertificates.DataBind();
                                BindPagination();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        Response.Write("Error: " + ex.Message);
                    }
                }
            }
        }



        protected void gvGiftCertificates_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ShowDetails")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvGiftCertificates.Rows[rowIndex];
                string batchIdentifier = row.Cells[10].Text; // Adjust the cell index as necessary

                // Set the hidden field value and trigger the modal
                hfBatchIdentifier.Value = batchIdentifier;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModal", "$('#batchModal').modal('show');", true);
            }
        }

        
        protected void gvGiftCertificates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string batchIdentifier = DataBinder.Eval(e.Row.DataItem, "BatchIdentifier").ToString();
                e.Row.CssClass = "grid-row";
                e.Row.Attributes["data-batch-identifier"] = batchIdentifier;
                e.Row.Attributes["style"] = "cursor:pointer;";
            }
        }


        protected void btnDownloadAll_Click(object sender, EventArgs e)
        {
            string query = @"
            SELECT 
                GCNumber, 
                Recipient, 
                Entitlement, 
                Description, 
                DATE_FORMAT(DateOfIssue, '%Y-%m-%d') AS DateOfIssue, 
                DATE_FORMAT(Validity, '%Y-%m-%d') AS Validity, 
                GCType, 
                ChargeTo, 
                Status, 
                Quantity,
                QRCodeImage,
                PrivacyHash
            FROM 
                giftcertificates";

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        using (XLWorkbook workbook = new XLWorkbook())
                        {
                            IXLWorksheet worksheet = workbook.Worksheets.Add("Gift Certificates");

                            worksheet.Cell(1, 1).Value = "GCNumber";
                            worksheet.Cell(1, 2).Value = "Recipient";
                            worksheet.Cell(1, 3).Value = "Entitlement";
                            worksheet.Cell(1, 4).Value = "Description";
                            worksheet.Cell(1, 5).Value = "DateOfIssue";
                            worksheet.Cell(1, 6).Value = "Validity";
                            worksheet.Cell(1, 7).Value = "GCType";
                            worksheet.Cell(1, 8).Value = "ChargeTo";
                            worksheet.Cell(1, 9).Value = "Status";
                            worksheet.Cell(1, 10).Value = "Quantity";
                            worksheet.Cell(1, 11).Value = "QRCodeImage";
                            worksheet.Cell(1, 12).Value = "PrivacyHash";

                            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                            {
                                worksheet.Cell(i + 2, 1).Value = dataSet.Tables[0].Rows[i]["GCNumber"].ToString();
                                worksheet.Cell(i + 2, 2).Value = dataSet.Tables[0].Rows[i]["Recipient"].ToString();
                                worksheet.Cell(i + 2, 3).Value = dataSet.Tables[0].Rows[i]["Entitlement"].ToString();
                                worksheet.Cell(i + 2, 4).Value = dataSet.Tables[0].Rows[i]["Description"].ToString();
                                worksheet.Cell(i + 2, 5).Value = dataSet.Tables[0].Rows[i]["DateOfIssue"].ToString();
                                worksheet.Cell(i + 2, 6).Value = dataSet.Tables[0].Rows[i]["Validity"].ToString();
                                worksheet.Cell(i + 2, 7).Value = dataSet.Tables[0].Rows[i]["GCType"].ToString();
                                worksheet.Cell(i + 2, 8).Value = dataSet.Tables[0].Rows[i]["ChargeTo"].ToString();
                                worksheet.Cell(i + 2, 9).Value = dataSet.Tables[0].Rows[i]["Status"].ToString();
                                worksheet.Cell(i + 2, 10).Value = dataSet.Tables[0].Rows[i]["Quantity"].ToString();
                                worksheet.Cell(i + 2, 12).Value = dataSet.Tables[0].Rows[i]["PrivacyHash"].ToString();

                                // Get and resize the QRCodeImage
                                byte[] qrCodeBytes = (byte[])dataSet.Tables[0].Rows[i]["QRCodeImage"];
                                using (MemoryStream ms = new MemoryStream(qrCodeBytes))
                                {
                                    using (Bitmap originalImage = new Bitmap(ms))
                                    {
                                        using (Bitmap resizedImage = new Bitmap(originalImage, new Size(320, 320)))
                                        {
                                            using (MemoryStream resizedMs = new MemoryStream())
                                            {
                                                resizedImage.Save(resizedMs, ImageFormat.Png);
                                                IXLPicture picture = worksheet.AddPicture(resizedMs).MoveTo(worksheet.Cell(i + 2, 11));
                                                picture.Scale(0.5); // Adjust scale as needed
                                            }
                                        }
                                    }
                                }
                            }

                            using (MemoryStream stream = new MemoryStream())
                            {
                                workbook.SaveAs(stream);
                                byte[] content = stream.ToArray();

                                Response.Clear();
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition", "attachment;filename=GiftCertificates.xlsx");
                                Response.BinaryWrite(content);
                                Response.End();
                            }
                        }
                    }
                }
            }
        }


        private DataTable GetBatchDetails(string batchIdentifier)
        {
            string query = @"
        SELECT 
            GCNumber, 
            Recipient, 
            Entitlement, 
            Description, 
            DATE_FORMAT(DateOfIssue, '%Y-%m-%d') AS DateOfIssue, 
            DATE_FORMAT(Validity, '%Y-%m-%d') AS Validity, 
            GCType, 
            ChargeTo, 
            Status, 
            QRCodeImage,
            PrivacyHash
        FROM 
            giftcertificates 
        WHERE 
            BatchIdentifier = @BatchIdentifier";

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BatchIdentifier", batchIdentifier);
                    connection.Open();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);
                        return dataSet.Tables[0];
                    }
                }
            }
        }

        private void ExportToExcel(DataTable dataTable)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                var worksheet = wb.Worksheets.Add("BatchDetails");

                // Load data table into worksheet
                worksheet.Cell(1, 1).InsertTable(dataTable);

                // Iterate over the rows to insert images
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i]["QRCodeImage"] != DBNull.Value)
                    {
                        byte[] imageBytes = (byte[])dataTable.Rows[i]["QRCodeImage"];
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            Bitmap image = new Bitmap(ms);
                            using (MemoryStream imageStream = new MemoryStream())
                            {
                                image.Save(imageStream, ImageFormat.Png);
                                var imageCell = worksheet.Cell(i + 2, dataTable.Columns["QRCodeImage"].Ordinal + 1); // Adjust cell index if needed
                                var imageXL = worksheet.AddPicture(imageStream)
                                                       .MoveTo(imageCell)
                                                       .Scale(0.5); // Adjust scale as needed
                            }
                        }
                    }
                }

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=BatchDetails.xlsx");

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }





        //FILTER BY STATUS
        protected void statusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset search text
            Session["SearchText"] = null;

            string selectedStatus = ddlUserStatus.SelectedValue;
            ViewState["SelectedStatus"] = selectedStatus;

            // Reset page index to 0 when a filter is applied
            gvGiftCertificates.PageIndex = 0;

            // Update page index text box
            UpdatePageIndexTextBox();

            if (!string.IsNullOrEmpty(selectedStatus))
            {
                FilterDataByStatus(selectedStatus);
            }
            else
            {
                LoadGiftCertificates();
            }

            gvGiftCertificates.PageIndex = 0;
        }
        protected void FilterDataByStatus(string status)
        {
            // Reset search text
            ViewState["SearchText"] = null;

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
            SELECT 
                GCNumber, 
                Recipient, 
                Entitlement, 
                Description, 
                DATE_FORMAT(DateOfIssue, '%Y-%m-%d') AS DateOfIssue, 
                DATE_FORMAT(Validity, '%Y-%m-%d') AS Validity, 
                GCType, 
                ChargeTo, 
                Status, 
                DATE_FORMAT(BookedFrom, '%Y-%m-%d') AS BookedFrom, 
                DATE_FORMAT(BookedTo, '%Y-%m-%d') AS BookedTo, 
                Quantity, 
                QRCodeImage,
                BatchIdentifier
            FROM 
                giftcertificates";

                if (!string.IsNullOrEmpty(status))
                {
                    query += " WHERE Status = @Status";
                }

                query += " ORDER BY ID DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(status))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            // Convert date/time strings to DateTime objects explicitly
                            foreach (DataRow row in dataSet.Tables[0].Rows)
                            {
                                row["DateOfIssue"] = ConvertToDateTime(row["DateOfIssue"]);
                                row["Validity"] = ConvertToDateTime(row["Validity"]);
                                row["BookedFrom"] = ConvertToDateTime(row["BookedFrom"]);
                                row["BookedTo"] = ConvertToDateTime(row["BookedTo"]);
                            }

                            gvGiftCertificates.DataSource = dataSet.Tables[0];
                            gvGiftCertificates.DataBind();
                            BindPagination();

                            // Calculate total number of pages for the filtered results
                            int totalRows = dataSet.Tables[0].Rows.Count;
                            int pageSize = gvGiftCertificates.PageSize;
                            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                            lblTotalPages.Text = totalPages.ToString();
                        }
                        else
                        {
                            gvGiftCertificates.DataSource = null;
                            gvGiftCertificates.DataBind();
                            BindPagination();

                            // Set total pages to 1 if no results found
                            lblTotalPages.Text = "1";
                        }
                    }
                }
            }
        }
        private DateTime? ConvertToDateTime(object value)
        {
            if (value == DBNull.Value || value == null)
            {
                return null;
            }

            string dateString = value.ToString();
            DateTime result;
            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
        protected void resetfilterBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("../GC/GCInventory.aspx");
        }
        // MODAL PAGE INDEX FUNCTION BELOW
        protected void gvBatchDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBatchDetails.PageIndex = e.NewPageIndex;
            string batchIdentifier = hfBatchIdentifier.Value;
            LoadBatchDetails(batchIdentifier);
        }
        protected void btnPrevModal_Click(object sender, EventArgs e)
        {
            if (gvBatchDetails.PageIndex > 0)
            {
                gvBatchDetails.PageIndex -= 1;
                string batchIdentifier = hfBatchIdentifier.Value;
                LoadBatchDetails(batchIdentifier);
            }
        }
        protected void btnNextModal_Click(object sender, EventArgs e)
        {
            if (gvBatchDetails.PageIndex < gvBatchDetails.PageCount - 1)
            {
                gvBatchDetails.PageIndex += 1;
                string batchIdentifier = hfBatchIdentifier.Value;
                LoadBatchDetails(batchIdentifier);
            }
        }


        protected void txtPageNumberModal_TextChanged(object sender, EventArgs e)
        {
            int pageNumber;
            if (int.TryParse(txtPageNumberModal.Text, out pageNumber))
            {
                if (pageNumber >= 1 && pageNumber <= gvBatchDetails.PageCount)
                {
                    gvBatchDetails.PageIndex = pageNumber - 1;
                    string batchIdentifier = hfBatchIdentifier.Value;
                    LoadBatchDetails(batchIdentifier);

                    txtPageNumberModal.Text = "";
                }
                else
                {
                    // Display error message for invalid page number
                    ScriptManager.RegisterStartupScript(this, GetType(), "InvalidPageNumberModal", $"alert('Invalid page number. Please enter a number between 1 and {gvBatchDetails.PageCount}.');", true);
                }
            }
            else
            {
                // Display error message for non-numeric input
                ScriptManager.RegisterStartupScript(this, GetType(), "InvalidInputModal", "alert('Please enter a valid number.');", true);
            }
        }
        //SEARCH
        protected void searchtxtbox_TextChanged(object sender, EventArgs e)
        {
            BindGridViewWithSearch();
        }
        protected void BindGridViewWithSearch()
        {
            string searchText = searchtxtbox.Text.Trim();
            Session["SearchText"] = searchText; // Store search text in session

            string connectionString = GetConnectionString(); // Replace with your connection string

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT 
                GCNumber, 
                Recipient, 
                Entitlement, 
                Description, 
                DATE_FORMAT(DateOfIssue, '%Y-%m-%d') AS DateOfIssue, 
                DATE_FORMAT(Validity, '%Y-%m-%d') AS Validity, 
                GCType, 
                ChargeTo, 
                Status, 
                DATE_FORMAT(BookedFrom, '%Y-%m-%d') AS BookedFrom, 
                DATE_FORMAT(BookedTo, '%Y-%m-%d') AS BookedTo, 
                Quantity, 
                QRCodeImage,
                BatchIdentifier
            FROM 
                giftcertificates
            WHERE
                GCNumber LIKE @searchText 
                OR Recipient LIKE @searchText 
                OR Entitlement LIKE @searchText 
                OR Description LIKE @searchText 
                OR DATE_FORMAT(DateOfIssue, '%Y-%m-%d') LIKE @searchText 
                OR DATE_FORMAT(Validity, '%Y-%m-%d') LIKE @searchText 
                OR GCType LIKE @searchText 
                OR ChargeTo LIKE @searchText 
                OR Status LIKE @searchText
            GROUP BY BatchIdentifier";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            // Data is available, bind the GridView
                            gvGiftCertificates.DataSource = dataSet.Tables[0];
                            gvGiftCertificates.DataBind();
                        }
                        else
                        {
                            // No data found, clear GridView
                            gvGiftCertificates.DataSource = null;
                            gvGiftCertificates.DataBind();
                        }

                        // Calculate and display pagination information
                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = gvGiftCertificates.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPages.Text = totalPages.ToString();
                        BindPagination(); // Ensure pagination is handled properly
                    }
                }
            }
        }
        // TABLE PAGE INDEX FUNCTION
        private void UpdatePageIndexTextBox()
        {
            // Update the txtboxPageNum with the current page index + 1 (to display 1-based index)
            lblPageNum.Text = (gvGiftCertificates.PageIndex + 1).ToString();
        }
        private void BindPagination()
        {
            int pageCount = gvGiftCertificates.PageCount;
            int currentPage = gvGiftCertificates.PageIndex;

            btnPrev.Visible = currentPage > 0;
            btnNext.Visible = currentPage < (pageCount - 1);

            // Update the txtboxPageNum with the current page index
            UpdatePageIndexTextBox();
        }
        protected void lnkPage_Click(object sender, EventArgs e)
        {
            LinkButton lnkPage = (LinkButton)sender;
            if (int.TryParse(lnkPage.Text, out int pageIndex))
            {
                gvGiftCertificates.PageIndex = pageIndex - 1;

                if (Session["SearchText"] != null)
                {
                    BindGridViewWithSearch();
                }
                else if (ViewState["SelectedStatus"] != null)
                {
                    FilterDataByStatus(ViewState["SelectedStatus"].ToString());
                    ViewState["SearchText"] = null;
                }
                else
                {
                    LoadGiftCertificates();

                }
                BindPagination();
            }
        }
        protected void btnPrev_Click(object sender, EventArgs e)
        {
            if (gvGiftCertificates.PageIndex > 0)
            {
                gvGiftCertificates.PageIndex -= 1;

                if (Session["SearchText"] != null)
                {
                    BindGridViewWithSearch();
                }
                else if (ViewState["SelectedStatus"] != null)
                {
                    FilterDataByStatus(ViewState["SelectedStatus"].ToString());
                    ViewState["SearchText"] = null;
                }
                else
                {
                    LoadGiftCertificates();

                }
                BindPagination();
            }
        }
        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (gvGiftCertificates.PageIndex < gvGiftCertificates.PageCount - 1)
            {
                gvGiftCertificates.PageIndex += 1;

                if (Session["SearchText"] != null)
                {
                    BindGridViewWithSearch();
                }
                else if (ViewState["SelectedStatus"] != null)
                {
                    FilterDataByStatus(ViewState["SelectedStatus"].ToString());
                    ViewState["SearchText"] = null;
                }
                else
                {
                    LoadGiftCertificates();

                }
                BindPagination();
            }
        }
        protected void btnGoToPage_Click(object sender, EventArgs e)
        {
            int pageNumber;
            if (int.TryParse(txtPageNumber.Text, out pageNumber))
            {
                if (pageNumber >= 1 && pageNumber <= gvGiftCertificates.PageCount)
                {
                    // Set the PageIndex of the GridView to the entered page number
                    gvGiftCertificates.PageIndex = pageNumber - 1;

                    if (Session["SearchText"] != null)
                    {
                        BindGridViewWithSearch();
                    }
                    else if (ViewState["SelectedStatus"] != null)
                    {
                        FilterDataByStatus(ViewState["SelectedStatus"].ToString());
                        ViewState["SearchText"] = null;
                    }
                    else
                    {
                        LoadGiftCertificates();

                    }
                    BindPagination();
                    txtPageNumber.Text = "";
                }
                else
                {
                    // The page index is out of range, so reset to the first page
                    gvGiftCertificates.PageIndex = 0;
                    LoadGiftCertificates();
                    BindPagination();
                    UpdatePageIndexTextBox();
                    txtPageNumber.Text = "";
                    // Display error message for invalid page number using JavaScript alert
                    ScriptManager.RegisterStartupScript(this, GetType(), "InvalidPageNumber", "alert('Invalid page number. Please enter a number between 1 and " + gvGiftCertificates.PageCount + ".');", true);
                }
            }
            else
            {
                // The page index is out of range, so reset to the first page
                gvGiftCertificates.PageIndex = 0;
                LoadGiftCertificates();
                BindPagination();
                UpdatePageIndexTextBox();
                txtPageNumber.Text = "";
                // Display error message for non-numeric input using JavaScript alert
                ScriptManager.RegisterStartupScript(this, GetType(), "InvalidInput", "alert('Please enter a valid number.');", true);
            }
        }
        protected void txtPageNumber_TextChanged(object sender, EventArgs e)
        {
            btnGoToPage_Click(sender, e);
        }
        public string GetImageUrl(object qrCodeImage)
        {
            if (qrCodeImage == DBNull.Value)
            {
                return ""; // Return a default image or an empty string if there's no QR code
            }
            else
            {
                return "data:image/png;base64," + Convert.ToBase64String((byte[])qrCodeImage);
            }
        }
        private static string GetConnectionString()
        {
            string server = "localhost";
            string database = "documentsystem";
            string username = "root";
            string password = "";
            return $"Server={server};Database={database};Uid={username};Pwd={password};";
        }

        protected void gvBatchDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "GeneratePublisher")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvBatchDetails.Rows[index];

                // Retrieve the text values from the cells directly
                string GCNumber = row.Cells[0].Text.Trim();
                string entitlement = row.Cells[1].Text.Trim();
                string description = row.Cells[2].Text.Trim();
                string dateOfIssue = row.Cells[3].Text.Trim();
                string validity = row.Cells[4].Text.Trim();
                string gcType = row.Cells[5].Text.Trim();
                string chargeTo = row.Cells[6].Text.Trim();
                string status = row.Cells[7].Text.Trim();
                string ReceiptTo = "Juan Dela Cruz";


                // Retrieve the QR code image file name from the GridView
                string qrCodeImageFileName = (row.FindControl("qrCodeImage") as System.Web.UI.WebControls.Image).ImageUrl;
              
                // Generate the Publisher file
                GeneratePublisherFile(ReceiptTo,GCNumber, entitlement, description, dateOfIssue, validity, gcType, chargeTo, status, qrCodeImageFileName);
            }
        }

        private void GeneratePublisherFile(string ReceiptTo , string GCNumber ,string entitlement, string description, string dateOfIssue, string validity, string gcType, string chargeTo, string status, string qrCodeImageFileName)
        {
            Publisher.Application publisherApp = new Publisher.Application();
            Publisher.Document doc = publisherApp.Documents.Add();

            Publisher.Shape GiftCertificate1 = doc.Pages[1].Shapes.AddTextbox(
                PbTextOrientation.pbTextOrientationHorizontal,
                -170,
                360,
                500,
                50
            );
            GiftCertificate1.TextFrame.TextRange.Text = "GIFT CERTIFICATE";
            GiftCertificate1.TextFrame.TextRange.ParagraphFormat.Alignment = PbParagraphAlignmentType.pbParagraphAlignmentCenter;
            GiftCertificate1.TextFrame.TextRange.Font.Size = 48;
            GiftCertificate1.TextFrame.TextRange.Font.Name = "Times New Roman";
            GiftCertificate1.Rotation = 270;


            Publisher.Shape TitleTo = doc.Pages[1].Shapes.AddTextbox(
                PbTextOrientation.pbTextOrientationHorizontal,
                130,
                600,
                50,
                50
            );
            TitleTo.TextFrame.TextRange.Text = "TO";
            TitleTo.TextFrame.TextRange.ParagraphFormat.Alignment = PbParagraphAlignmentType.pbParagraphAlignmentLeft;
            TitleTo.TextFrame.TextRange.Font.Size = 24;
            TitleTo.TextFrame.TextRange.Font.Name = "Times New Roman";
            TitleTo.Rotation = 270;



            Publisher.Shape TitleData = doc.Pages[1].Shapes.AddTextbox(
               PbTextOrientation.pbTextOrientationHorizontal,
               50,
               300,
               200,
               40
           );
            TitleData.TextFrame.TextRange.Text = ReceiptTo;
            TitleData.TextFrame.TextRange.ParagraphFormat.Alignment = PbParagraphAlignmentType.pbParagraphAlignmentCenter;
            TitleData.TextFrame.TextRange.Font.Size = 24;
            TitleData.TextFrame.TextRange.Font.Name = "Times New Roman";
            TitleData.Rotation = 270;

            Publisher.Shape EntitlementTitle = doc.Pages[1].Shapes.AddTextbox(
               PbTextOrientation.pbTextOrientationHorizontal,
               90,
               525,
               200,
               50
           );
            EntitlementTitle.TextFrame.TextRange.Text = "ENTITLEMENT";
            EntitlementTitle.TextFrame.TextRange.ParagraphFormat.Alignment = PbParagraphAlignmentType.pbParagraphAlignmentLeft;
            EntitlementTitle.TextFrame.TextRange.Font.Size = 24;
            EntitlementTitle.TextFrame.TextRange.Font.Name = "Times New Roman";
            EntitlementTitle.Rotation = 270;


           


            Publisher.Shape EntitlementData = doc.Pages[1].Shapes.AddTextbox(
              PbTextOrientation.pbTextOrientationHorizontal,
              50,
              250,
              300,
              40
             );
            EntitlementData.TextFrame.TextRange.Text = entitlement;
            EntitlementData.TextFrame.TextRange.ParagraphFormat.Alignment = PbParagraphAlignmentType.pbParagraphAlignmentCenter;
            EntitlementData.TextFrame.TextRange.Font.Size = 24;
            EntitlementData.TextFrame.TextRange.Font.Name = "Times New Roman";
            EntitlementData.Rotation = 270;


            Publisher.Shape DateofIssueTitle = doc.Pages[1].Shapes.AddTextbox(
             PbTextOrientation.pbTextOrientationHorizontal,
             120,
             505,
             240,
             50
            );

            DateofIssueTitle.TextFrame.TextRange.Text = "DATE OF ISSUE";
            DateofIssueTitle.TextFrame.TextRange.ParagraphFormat.Alignment = PbParagraphAlignmentType.pbParagraphAlignmentLeft;
            DateofIssueTitle.TextFrame.TextRange.Font.Size = 24;
            DateofIssueTitle.TextFrame.TextRange.Font.Name = "Times New Roman";
            DateofIssueTitle.Rotation = 270;


            Publisher.Shape DateofIssueData = doc.Pages[1].Shapes.AddTextbox(
             PbTextOrientation.pbTextOrientationHorizontal,
             140,
             300,
             200,
             40
            );
            DateofIssueData.TextFrame.TextRange.Text = dateOfIssue;
            DateofIssueData.TextFrame.TextRange.ParagraphFormat.Alignment = PbParagraphAlignmentType.pbParagraphAlignmentCenter;
            DateofIssueData.TextFrame.TextRange.Font.Size = 24;
            DateofIssueData.TextFrame.TextRange.Font.Name = "Times New Roman";
            DateofIssueData.Rotation = 270;


            Publisher.Shape ValidityTitle = doc.Pages[1].Shapes.AddTextbox(
              PbTextOrientation.pbTextOrientationHorizontal,
              160,
              505,
              240,
              50
          );
            ValidityTitle.TextFrame.TextRange.Text = "VALIDITY";
            ValidityTitle.TextFrame.TextRange.ParagraphFormat.Alignment = PbParagraphAlignmentType.pbParagraphAlignmentLeft;
            ValidityTitle.TextFrame.TextRange.Font.Size = 24;
            ValidityTitle.TextFrame.TextRange.Font.Name = "Times New Roman";
            ValidityTitle.Rotation = 270;


            Publisher.Shape ValidityData = doc.Pages[1].Shapes.AddTextbox(
            PbTextOrientation.pbTextOrientationHorizontal,
            180,
            300,
            200,
            40
           );
            ValidityData.TextFrame.TextRange.Text = validity;
            ValidityData.TextFrame.TextRange.ParagraphFormat.Alignment = PbParagraphAlignmentType.pbParagraphAlignmentCenter;
            ValidityData.TextFrame.TextRange.Font.Size = 24;
            ValidityData.TextFrame.TextRange.Font.Name = "Times New Roman";
            ValidityData.Rotation = 270;



            Publisher.Shape AUTHORIZEDBYTITLE = doc.Pages[1].Shapes.AddTextbox(
              PbTextOrientation.pbTextOrientationHorizontal,
              200,
              505,
              240,
              50
          );

            AUTHORIZEDBYTITLE.TextFrame.TextRange.Text = "AUTORIZED BY";
            AUTHORIZEDBYTITLE.TextFrame.TextRange.ParagraphFormat.Alignment = PbParagraphAlignmentType.pbParagraphAlignmentLeft;
            AUTHORIZEDBYTITLE.TextFrame.TextRange.Font.Size = 24;
            AUTHORIZEDBYTITLE.TextFrame.TextRange.Font.Name = "Times New Roman";
            AUTHORIZEDBYTITLE.Rotation = 270;



            Publisher.Page page1 = doc.Pages.Add(1, 1, -1, false);

            Publisher.Shape GiftCertificate = page1.Shapes.AddTextbox(
                 PbTextOrientation.pbTextOrientationHorizontal,
                 -180,
                 350,
                 600,
                 50
             );
            GiftCertificate.TextFrame.TextRange.Text = "TERMS AND CONDITION";
            GiftCertificate.TextFrame.TextRange.ParagraphFormat.Alignment = PbParagraphAlignmentType.pbParagraphAlignmentCenter;
            GiftCertificate.TextFrame.TextRange.Font.Size = 48;
            GiftCertificate.TextFrame.TextRange.Font.Name = "Times New Roman";
            GiftCertificate.Rotation = 270;


            Publisher.Shape GCASHNumber = page1.Shapes.AddTextbox(
            PbTextOrientation.pbTextOrientationHorizontal,
            80,
            300,
            500,
            200
           );
            GCASHNumber.TextFrame.TextRange.Text = $"GCNumber: {GCNumber}\nCharge To: {chargeTo}\nGC Type: {gcType}"; 
            GCASHNumber.TextFrame.TextRange.ParagraphFormat.Alignment = PbParagraphAlignmentType.pbParagraphAlignmentCenter;
            GCASHNumber.TextFrame.TextRange.Font.Size = 24;
            GCASHNumber.TextFrame.TextRange.Font.Name = "Times New Roman";
            GCASHNumber.Rotation = 270;

            string physicalPath = Server.MapPath(qrCodeImageFileName);
            Publisher.Shape qrCodeShape = doc.Pages[1].Shapes.AddPicture(physicalPath, MsoTriState.msoFalse, MsoTriState.msoCTrue, 50, 50, 100, 100);
            qrCodeShape.Rotation = 270;

            // Save the document
            string filePath = Server.MapPath("~/App_Data/GCGiftCertificates.pub");
            doc.SaveAs(filePath);

            //doc.Close();
            //publisherApp.Quit();

        }

    }
}