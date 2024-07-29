using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Controls;
using Control = System.Web.UI.Control;
using Label = System.Web.UI.WebControls.Label;
using PdfiumViewer;
using System.Drawing;
using System.Windows.Forms;
using PrintDialog = System.Windows.Forms.PrintDialog;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;



namespace DMS
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Name"] == null)
            {
                // Redirect to login page if session variable is not set
                Response.Redirect("../LOGIN/Login.aspx");
            }
            if (!IsPostBack)
            {
                BindGridView();
                BindPagination();

                SetShowAllLink();
                // Check if total documents count needs to be updated
                if (Session["UpdateTotalDocuments"] != null && (bool)Session["UpdateTotalDocuments"])
                {
                    // Call the method to bind total documents
                    BindTotalDocuments();

                    // Reset the session variable
                    Session["UpdateTotalDocuments"] = false;
                }
                // Check if a button was previously clicked and bind GridView accordingly
                if (ViewState["SelectedButton"] != null)
                {
                    string selectedButton = ViewState["SelectedButton"].ToString();
                    switch (selectedButton)
                    {
                        case "allDocsBtn":
                            AllDocsBtn_Click(sender, e);
                            break;
                        case "myDocsBtn":
                            MyDocsBtn_Click(sender, e);
                            break;
                        case "deptDocsBtn":
                            DeptDocsBtn_Click(sender, e);
                            break;
                        case "publicDocsBtn":
                            PublicDocsBtn_Click(sender, e);
                            break;
                        default:
                            // Handle default case if needed
                            break;
                    }
                }

                PopulateFolderDropdown("Only Me", ddlOnlyMe);
                PopulateFolderDropdown("My Department", ddlMyDepartment);
                PopulateFolderDropdown("Public", ddlPublic);

                // Bind user activities based on current user's access level
                BindUserActivities();
            }
            else
            {
                // If it's a postback, check if there's a search text stored in session and re-bind GridView with search results
                if (Session["SearchText"] != null)
                {
                    BindGridViewWithSearch();
                }
            }

            BindPagination();
            UpdatePageIndexTextBox();

            string currentUserDepartment = Session["Department"].ToString();

            // Check if the user is from the 'MIS' department to show the button
            if (currentUserDepartment == "MIS")
            {
                EditAPIbtn.Visible = true;
            }
            else
            {
                EditAPIbtn.Visible = false;
            }
        }
        //DEFAULT BINDGRIDVIEW
        protected void BindGridView()
        {
            string currentUserName = Session["Name"] != null ? Session["Name"].ToString() : "";
            string currentUserDepartment = Session["Department"] != null ? Session["Department"].ToString() : "";

            // Fetch the uploaded files data based on privacy settings and permission
            string dataQuery = @"SELECT uf.ControlID, uf.UploaderName, acc.Department, uf.FileName, uf.UploadDateTime, uf.Privacy, uf.Category
                         FROM files uf
                         LEFT JOIN Accounts acc ON uf.UploaderName = acc.Name
                         WHERE uf.Privacy = 'Public'
                            OR uf.Privacy = 'My Department' AND (acc.Department = @UploaderDepartment OR acc.Department = @CurrentDepartment)
                            OR uf.Privacy = 'My Department' AND uf.UploaderName = @UploaderName
                            OR uf.Privacy <> 'Only Me' AND uf.UploaderName = @UploaderName";

            // Check if user has the 'View public and their department documents' permission
            bool hasViewPublicAndDeptDocsPermission = CheckPermission("View public and their department documents");

            // Check if user has the 'View all documents from all departments' permission
            bool hasViewAllDocsPermission = CheckPermission("View all documents from all departments");

            if (hasViewPublicAndDeptDocsPermission && hasViewAllDocsPermission || hasViewAllDocsPermission && !hasViewPublicAndDeptDocsPermission)
            {
                // If user has both permissions, show all documents with 'My Department' privacy
                dataQuery = @"SELECT uf.ControlID, uf.UploaderName, acc.Department, uf.FileName, uf.UploadDateTime, uf.Privacy, uf.Category
                     FROM files uf
                     LEFT JOIN Accounts acc ON uf.UploaderName = acc.Name
                     WHERE uf.Privacy = 'Public'
                        OR uf.Privacy = 'My Department'
                        OR uf.Privacy <> 'Only Me'";
            }
            else if (!hasViewPublicAndDeptDocsPermission)
            {
                // Restrict to only the current user's documents
                dataQuery = @"SELECT uf.ControlID, uf.UploaderName, acc.Department, uf.FileName, uf.UploadDateTime, uf.Privacy, uf.Category
                     FROM files uf
                     LEFT JOIN Accounts acc ON uf.UploaderName = acc.Name
                     WHERE uf.UploaderName = @UploaderName";
            }
            else if (!hasViewPublicAndDeptDocsPermission && !hasViewAllDocsPermission)
            {
                dataQuery += " AND 1=0 "; // This will effectively return no results
            }

            dataQuery += " ORDER BY uf.UploadDateTime DESC";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (MySqlCommand dataCmd = new MySqlCommand(dataQuery, conn))
                {
                    dataCmd.Parameters.AddWithValue("@UploaderDepartment", currentUserDepartment);
                    dataCmd.Parameters.AddWithValue("@CurrentDepartment", currentUserDepartment);
                    dataCmd.Parameters.AddWithValue("@UploaderName", currentUserName);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(dataCmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        GridView1.DataSource = dataSet.Tables[0];
                        GridView1.DataBind();

                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridView1.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPages.Text = totalPages.ToString();
                    }
                }
            }
            UpdateTotalDocumentsLabels();
        }
        //TABLE LINK BUTTONS FUNCTIONS
        private bool CheckPermission(string permissionName)
        {
            // Check if the current user has the specified permission
            string currentUserId = Session["UserID"].ToString();
            string query = @"SELECT COUNT(*) FROM User_Permissions up
                     INNER JOIN Permissions p ON up.PermissionID = p.PermissionID
                     WHERE up.UserID = @UserID AND p.PermissionName = @PermissionName";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", currentUserId);
                    cmd.Parameters.AddWithValue("@PermissionName", permissionName);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        protected void AllDocsBtn_Click(object sender, EventArgs e)
        {
            ViewState.Remove("publicDocsBtn");
            ViewState.Remove("deptDocsBtn");
            ViewState.Remove("myDocsBtn");
            Session["SearchText"] = null;
            ViewState["SelectedButton"] = "allDocsBtn";

            MyDocsBtn.CssClass = "link-btn";
            PublicDocsBtn.CssClass = "link-btn";
            AllDocsBtn.CssClass = "link-btn active";
            DeptDocsBtn.CssClass = "link-btn";

            string currentUserName = Session["Name"] != null ? Session["Name"].ToString() : "";
            string currentUserDepartment = Session["Department"] != null ? Session["Department"].ToString() : "";

            // Fetch the uploaded files data based on privacy settings and permission
            string dataQuery = @"SELECT uf.ControlID, uf.UploaderName, acc.Department, uf.FileName, uf.UploadDateTime, uf.Privacy, uf.Category
                         FROM files uf
                         LEFT JOIN Accounts acc ON uf.UploaderName = acc.Name
                         WHERE uf.Privacy = 'Public'
                            OR uf.Privacy = 'My Department' AND (acc.Department = @UploaderDepartment OR acc.Department = @CurrentDepartment)
                            OR uf.Privacy = 'My Department' AND uf.UploaderName = @UploaderName
                            OR uf.Privacy <> 'Only Me' AND uf.UploaderName = @UploaderName";

            // Check if user has the 'View public and their department documents' permission
            bool hasViewPublicAndDeptDocsPermission = CheckPermission("View public and their department documents");

            // Check if user has the 'View all documents from all departments' permission
            bool hasViewAllDocsPermission = CheckPermission("View all documents from all departments");

            if (hasViewPublicAndDeptDocsPermission && hasViewAllDocsPermission)
            {
                // If user has both permissions, show all documents with 'My Department' privacy
                dataQuery = @"SELECT uf.ControlID, uf.UploaderName, acc.Department, uf.FileName, uf.UploadDateTime, uf.Privacy, uf.Category
                     FROM files uf
                     LEFT JOIN Accounts acc ON uf.UploaderName = acc.Name
                     WHERE uf.Privacy = 'Public'
                        OR uf.Privacy = 'My Department'
                        OR uf.Privacy <> 'Only Me'";
            }
            else if (!hasViewPublicAndDeptDocsPermission)
            {
                // Restrict to only the current user's documents
                dataQuery = @"SELECT uf.ControlID, uf.UploaderName, acc.Department, uf.FileName, uf.UploadDateTime, uf.Privacy, uf.Category
                     FROM files uf
                     LEFT JOIN Accounts acc ON uf.UploaderName = acc.Name
                     WHERE uf.UploaderName = @UploaderName";
            }

            dataQuery += " ORDER BY uf.UploadDateTime DESC";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (MySqlCommand dataCmd = new MySqlCommand(dataQuery, conn))
                {
                    dataCmd.Parameters.AddWithValue("@UploaderDepartment", currentUserDepartment);
                    dataCmd.Parameters.AddWithValue("@CurrentDepartment", currentUserDepartment);
                    dataCmd.Parameters.AddWithValue("@UploaderName", currentUserName);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(dataCmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView1.DataSource = dataSet.Tables[0];
                            GridView1.DataBind();
                        }
                        else
                        {
                            GridView1.DataSource = null;
                            GridView1.DataBind();

                            lblTotalPages.Text = "1";
                        }

                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridView1.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPages.Text = totalPages.ToString();
                    }
                }
            }
        }
        protected void MyDocsBtn_Click(object sender, EventArgs e)
        {
            // Clear ViewState related to button selection
            ViewState.Remove("allDocsBtn");
            ViewState.Remove("publicDocsBtn");
            ViewState.Remove("deptDocsBtn");
            Session["SearchText"] = null;
            // Set ViewState for the current button
            ViewState["SelectedButton"] = "myDocsBtn";

            MyDocsBtn.CssClass = "link-btn active";
            PublicDocsBtn.CssClass = "link-btn";
            AllDocsBtn.CssClass = "link-btn";
            DeptDocsBtn.CssClass = "link-btn";

            string currentUserName = Session["Name"] != null ? Session["Name"].ToString() : "";
            string currentUserDepartment = Session["Department"] != null ? Session["Department"].ToString() : "";

            // Now, fetch the uploaded files data based on privacy settings
            string dataQuery = @"SELECT uf.ControlID, uf.UploaderName, acc.Department, uf.FileName, uf.UploadDateTime, uf.Privacy, uf.Category
             FROM files uf
             LEFT JOIN Accounts acc ON uf.UploaderName = acc.Name
             WHERE uf.UploaderName = @UploaderName";

            dataQuery += " ORDER BY uf.UploadDateTime DESC";
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (MySqlCommand dataCmd = new MySqlCommand(dataQuery, conn))
                {
                    dataCmd.Parameters.AddWithValue("@UploaderName", currentUserName);
                    dataCmd.Parameters.AddWithValue("@Department", currentUserDepartment);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(dataCmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView1.DataSource = dataSet.Tables[0];
                            GridView1.DataBind();
                            BindPagination();
                        }
                        else
                        {
                            GridView1.DataSource = null;
                            GridView1.DataBind();
                            BindPagination();

                            lblTotalPages.Text = "1";
                        }

                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridView1.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPages.Text = totalPages.ToString();
                        //GridView1.PageIndex = 0; this is not working
                    }
                }
            }
            //GridView1.PageIndex = 0; this not working
        }
        protected void DeptDocsBtn_Click(object sender, EventArgs e)
        {
            ViewState.Remove("publicDocsBtn");
            ViewState.Remove("allDocsBtn");
            ViewState.Remove("myDocsBtn");
            Session["SearchText"] = null;
            ViewState["SelectedButton"] = "deptDocsBtn";

            MyDocsBtn.CssClass = "link-btn";
            PublicDocsBtn.CssClass = "link-btn";
            AllDocsBtn.CssClass = "link-btn";
            DeptDocsBtn.CssClass = "link-btn active";

            string currentUserDepartment = Session["Department"] != null ? Session["Department"].ToString() : "";

            // Fetch the department documents data based on permissions
            string dataQuery = @"SELECT uf.ControlID, uf.UploaderName, acc.Department, uf.FileName, uf.UploadDateTime, uf.Privacy, uf.Category
                         FROM files uf
                         LEFT JOIN Accounts acc ON uf.UploaderName = acc.Name
                         WHERE uf.Privacy = 'My Department'";

            // Check if user has the 'View all documents from all departments' permission
            bool hasViewAllDocsPermission = CheckPermission("View all documents from all departments");

            if (hasViewAllDocsPermission)
            {
                // Display all documents with privacy 'My Department' regardless of current user's department
                dataQuery += " OR (uf.Privacy = 'My Department')";
            }
            else if (!hasViewAllDocsPermission)
            {
                // Filter by current user's department
                dataQuery += " AND acc.Department = @Department";
            }
            else
            {
                // display blank if current logged in user does not have the permission "View public and their department documents" permission
                dataQuery += "WHERE 1 = 0";
            }


            dataQuery += " ORDER BY uf.UploadDateTime DESC";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (MySqlCommand dataCmd = new MySqlCommand(dataQuery, conn))
                {
                    if (!hasViewAllDocsPermission)
                    {
                        dataCmd.Parameters.AddWithValue("@Department", currentUserDepartment);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(dataCmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView1.DataSource = dataSet.Tables[0];
                            GridView1.DataBind();
                            BindPagination();
                        }
                        else
                        {
                            GridView1.DataSource = null;
                            GridView1.DataBind();
                            BindPagination();

                            lblTotalPages.Text = "1";
                        }

                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridView1.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPages.Text = totalPages.ToString();
                    }
                }
            }
        }
        protected void PublicDocsBtn_Click(object sender, EventArgs e)
        {
            ViewState.Remove("allDocsBtn");
            ViewState.Remove("deptDocsBtn");
            ViewState.Remove("myDocsBtn");
            Session["SearchText"] = null;
            ViewState["SelectedButton"] = "publicDocsBtn";

            MyDocsBtn.CssClass = "link-btn";
            PublicDocsBtn.CssClass = "link-btn active";
            AllDocsBtn.CssClass = "link-btn";
            DeptDocsBtn.CssClass = "link-btn";

            // Fetch the uploaded public files data based on permission
            string dataQuery = @"SELECT uf.ControlID, uf.UploaderName, acc.Department, uf.FileName, uf.UploadDateTime, uf.Privacy, uf.Category
                         FROM files uf
                         LEFT JOIN Accounts acc ON uf.UploaderName = acc.Name
                         WHERE uf.Privacy = 'Public'";

            // Check if user has the 'View public and their department documents' permission
            dataQuery += @" AND EXISTS (
                        SELECT 1 FROM User_Permissions up
                        INNER JOIN Permissions p ON up.PermissionID = p.PermissionID
                        WHERE up.UserID = @UserID 
                        AND p.PermissionName = 'View public and their department documents'
                    )";

            dataQuery += " ORDER BY uf.UploadDateTime DESC";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (MySqlCommand dataCmd = new MySqlCommand(dataQuery, conn))
                {
                    dataCmd.Parameters.AddWithValue("@UserID", Session["UserID"].ToString());

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(dataCmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            GridView1.DataSource = dataSet.Tables[0];
                            GridView1.DataBind();
                            BindPagination();
                        }
                        else
                        {
                            GridView1.DataSource = null;
                            GridView1.DataBind();
                            BindPagination();

                            lblTotalPages.Text = "1";
                        }

                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridView1.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPages.Text = totalPages.ToString();
                    }
                }
            }
        }
        //SEARCH FUNCTION
        protected void Searchtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Set the search text
            string searchText = searchtxtbox.Text.Trim();
            Session["SearchText"] = searchText;

            // Bind the GridView with search results
            BindGridViewWithSearch();
        }
        protected void BindGridViewWithSearch()
        {
            string searchText = Session["SearchText"] != null ? Session["SearchText"].ToString() : "";
            string currentUserName = Session["Name"] != null ? Session["Name"].ToString() : "";
            string currentUserDepartment = Session["Department"] != null ? Session["Department"].ToString() : "";

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Check permissions
                bool canViewAllDocuments = CheckPermission("View all documents from all departments");
                bool canViewDepartmentDocuments = CheckPermission("View public and their department documents");

                string query = @"SELECT uf.*, acc.Department 
                         FROM files uf 
                         LEFT JOIN Accounts acc ON uf.UploaderName = acc.Name 
                         WHERE (uf.FileName LIKE @searchText 
                                OR uf.ControlID LIKE @searchText 
                                OR (DATE_FORMAT(uf.UploadDateTime, '%p') LIKE @searchText AND uf.UploadDateTime IS NOT NULL) 
                                OR (DATE_FORMAT(uf.UploadDateTime, '%l:') LIKE @searchText AND uf.UploadDateTime IS NOT NULL) 
                                OR (DATE_FORMAT(uf.UploadDateTime, '%l %p') LIKE @searchText AND uf.UploadDateTime IS NOT NULL) 
                                OR uf.UploaderName LIKE @searchText 
                                OR acc.Department LIKE @searchText 
                                OR uf.Privacy LIKE @searchText 
                                OR uf.Category LIKE @searchText
                                OR uf.OCRText LIKE @searchText) ";

                if (canViewAllDocuments || canViewAllDocuments && canViewDepartmentDocuments)
                {
                    // Users with this permission can search all files including their own "Only Me" files
                    query += " AND ((uf.Privacy <> 'Only Me') OR (uf.Privacy = 'Only Me' AND uf.UploaderName = @UploaderName)) ";
                }
                else if (canViewDepartmentDocuments && !canViewAllDocuments)
                {
                    // Users with permission "View public and their department documents" can only search files within their own department
                    query += " AND ((uf.Privacy <> 'Only Me' AND uf.UploaderName <> @UploaderName) OR (uf.Privacy = 'Only Me' AND uf.UploaderName = @UploaderName)) ";
                    query += " AND (uf.Privacy <> 'My Department' OR acc.Department = @Department) ";
                }
                else
                {
                    // If user doesn't have view permissions, they should not be able to see any files
                    query += " AND 1=0 "; // This will effectively return no results
                }

                query += " ORDER BY uf.UploadDateTime DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
                    cmd.Parameters.AddWithValue("@UploaderName", currentUserName);
                    cmd.Parameters.AddWithValue("@Department", currentUserDepartment);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count > 0)
                        {
                            // Data is available, bind the GridView
                            GridView1.DataSource = dataSet.Tables[0];
                            GridView1.DataBind();
                            BindPagination();
                        }
                        else
                        {
                            // No data found, display message in GridView
                            GridView1.DataSource = null;
                            GridView1.DataBind();
                            BindPagination();
                        }

                        int totalRows = dataSet.Tables[0].Rows.Count;
                        int pageSize = GridView1.PageSize;
                        int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                        lblTotalPages.Text = totalPages.ToString();

                        // Reset search text after binding GridView
                        Session["SearchText"] = null;
                        searchtxtbox.Text = ""; // Clear search text box
                    }
                }
            }
        }






        //TABLE PAGE INDEX FUNCTIONS
        private void UpdatePageIndexTextBox()
        {
            // Update the txtboxPageNum with the current page index + 1 (to display 1-based index)
            lblPageNum.Text = (GridView1.PageIndex + 1).ToString();
            //txtPageNumber.Text = (GridView1.PageIndex + 1).ToString();
        }
        protected void txtPageNumber_TextChanged(object sender, EventArgs e)
        {
            btnGoToPage_Click(sender, e);
        }
        protected void btnGoToPage_Click(object sender, EventArgs e)
        {
            int pageNumber;
            if (int.TryParse(txtPageNumber.Text, out pageNumber))
            {
                if (pageNumber >= 1 && pageNumber <= GridView1.PageCount)
                {
                    // Set the PageIndex of the GridView to the entered page number
                    GridView1.PageIndex = pageNumber - 1;

                    // Re-bind the GridView with the data for the specified page
                    BindGridView();

                    // Update pagination controls
                    BindPagination();

                    // Clear the entered page number
                    txtPageNumber.Text = "";
                }
                else
                {
                    // The page index is out of range, so reset to the first page
                    GridView1.PageIndex = 0;
                    BindGridView();
                    BindPagination();
                    UpdatePageIndexTextBox();
                    txtPageNumber.Text = "";
                    // Display error message for invalid page number using JavaScript alert
                    ScriptManager.RegisterStartupScript(this, GetType(), "InvalidPageNumber", "alert('Invalid page number. Please enter a number between 1 and " + GridView1.PageCount + ".');", true);
                }
            }
            else
            {
                // The page index is out of range, so reset to the first page
                GridView1.PageIndex = 0;
                BindGridView();
                BindPagination();
                UpdatePageIndexTextBox();
                txtPageNumber.Text = "";
                // Display error message for non-numeric input using JavaScript alert
                ScriptManager.RegisterStartupScript(this, GetType(), "InvalidInput", "alert('Please enter a valid number.');", true);
            }
        }
        private void BindPagination()
        {
            int pageCount = GridView1.PageCount;
            int currentPage = GridView1.PageIndex;

            // Enable or disable Previous and Next buttons based on current page
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
                // Set the GridView's PageIndex directly to the parsed pageIndex - 1
                GridView1.PageIndex = pageIndex - 1;

                // Re-bind the GridView based on the search or button previously clicked
                if (Session["SearchText"] != null)
                {
                    BindGridViewWithSearch();
                }
                else
                {
                    BindGridViewBasedOnButton();
                }

                // Update pagination controls
                BindPagination();
            }
            else
            {
                // Handle the case where parsing fails, perhaps with logging or error handling
            }
        }
        protected void btnPrev_Click(object sender, EventArgs e)
        {
            if (GridView1.PageIndex > 0)
            {
                GridView1.PageIndex -= 1;

                // Re-bind the GridView based on the search or button previously clicked
                if (Session["SearchText"] != null)
                {
                    BindGridViewWithSearch();
                }
                else
                {
                    BindGridViewBasedOnButton();
                }
                BindPagination();
            }
        }
        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (GridView1.PageIndex < GridView1.PageCount - 1)
            {
                GridView1.PageIndex += 1;

                // Re-bind the GridView based on the search or button previously clicked
                if (Session["SearchText"] != null)
                {
                    BindGridViewWithSearch();
                }
                else
                {
                    BindGridViewBasedOnButton();
                }
                BindPagination();
            }
        }
        private void BindGridViewBasedOnButton()
        {
            if (ViewState["SelectedButton"] != null)
            {
                string selectedButton = ViewState["SelectedButton"].ToString();
                switch (selectedButton)
                {
                    case "allDocsBtn":
                        AllDocsBtn_Click(null, null);
                        break;
                    case "myDocsBtn":
                        MyDocsBtn_Click(null, null);
                        break;
                    case "deptDocsBtn":
                        DeptDocsBtn_Click(null, null);
                        break;
                    case "publicDocsBtn":
                        PublicDocsBtn_Click(null, null);
                        break;
                    default:
                        // Handle default case if needed
                        break;
                }
            }
            else
            {
                // If no button was previously clicked, bind the default data
                BindGridView();
            }
        }
        //LABELS ABOVE CARDS
        private void UpdateTotalDocumentsLabels()
        {
            string currentUserName = Session["Name"] != null ? Session["Name"].ToString() : "";
            string currentUserDepartment = Session["Department"] != null ? Session["Department"].ToString() : "";
            string currentUserAccess = Session["Access"] != null ? Session["Access"].ToString() : "";
            bool canViewPublicAndDeptDocs = HasPermission(currentUserAccess, "View public and their department documents");
            bool canViewAllDocs = HasPermission(currentUserAccess, "View all documents from all departments");

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Determine which query to use based on permissions
                string countQuery = "";

                if (canViewAllDocs)
                {
                    // User can view all documents
                    countQuery = @"SELECT 
                COUNT(*) AS TotalUploadedFiles,
                COALESCE(SUM(CASE WHEN Category = 'Scanned' THEN 1 ELSE 0 END), 0) AS TotalScannedDocuments,
                COALESCE(SUM(CASE WHEN Category = 'Digital' THEN 1 ELSE 0 END), 0) AS TotalDigitalDocuments 
                FROM 
                files 
                WHERE 
                ((UploaderName = @UploaderName AND Privacy = 'Only Me') OR 
                (Privacy = 'My Department') OR 
                (Privacy = 'Public'))";
                }
                else
                {
                    // Default behavior: view public and their department documents
                    countQuery = @"SELECT 
                COUNT(*) AS TotalUploadedFiles,
                COALESCE(SUM(CASE WHEN Category = 'Scanned' THEN 1 ELSE 0 END), 0) AS TotalScannedDocuments,
                COALESCE(SUM(CASE WHEN Category = 'Digital' THEN 1 ELSE 0 END), 0) AS TotalDigitalDocuments 
                FROM 
                files 
                WHERE 
                ((UploaderName = @UploaderName AND Privacy = 'Only Me') OR 
                (Privacy = 'My Department' AND (UploaderName IN (SELECT Name FROM Accounts WHERE Department = @Department) OR UploaderName = @UploaderName)) OR 
                (Privacy = 'Public'))";
                }

                using (MySqlCommand countCmd = new MySqlCommand(countQuery, conn))
                {
                    countCmd.Parameters.AddWithValue("@UploaderName", currentUserName);
                    countCmd.Parameters.AddWithValue("@Department", currentUserDepartment);

                    using (MySqlDataReader reader = countCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblTotalDocu.Text = reader["TotalUploadedFiles"].ToString();
                            lblTotalScan.Text = reader["TotalScannedDocuments"].ToString();
                            lblTotalDigit.Text = reader["TotalDigitalDocuments"].ToString();
                        }
                    }
                }

                // Fetch total count of department files
                string countDeptQuery = canViewAllDocs
                    ? @"SELECT COUNT(*) AS TotalDeptFiles FROM files WHERE Privacy = 'My Department'"
                    : @"SELECT COUNT(*) AS TotalDeptFiles 
               FROM files 
               WHERE Privacy = 'My Department' 
               AND UploaderName IN (SELECT Name FROM Accounts WHERE Department = @Department AND Access NOT LIKE '%View all documents from all departments%')";

                int totalDeptFiles = 0;
                int totalMyDocs = 0;

                using (MySqlCommand countDeptCmd = new MySqlCommand(countDeptQuery, conn))
                {
                    countDeptCmd.Parameters.AddWithValue("@Department", currentUserDepartment);

                    using (MySqlDataReader reader = countDeptCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            totalDeptFiles = Convert.ToInt32(reader["TotalDeptFiles"]);
                        }
                    }
                }

                // Fetch total count of my documents
                string countMyDocsQuery = @"SELECT COUNT(*) AS TotalMyDocs 
                                    FROM files 
                                    WHERE UploaderName = @UploaderName";

                using (MySqlCommand countMyDocsCmd = new MySqlCommand(countMyDocsQuery, conn))
                {
                    countMyDocsCmd.Parameters.AddWithValue("@UploaderName", currentUserName);

                    using (MySqlDataReader reader = countMyDocsCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            totalMyDocs = Convert.ToInt32(reader["TotalMyDocs"]);
                        }
                    }
                }

                lblTotalDept.Text = totalDeptFiles.ToString();
                lblTotalMyDocs.Text = totalMyDocs.ToString();
            }
        }
        //TOTAL DOCUMENT
        protected void BindTotalDocuments()
        {
            // Get the current logged-in user's name and department
            string currentUserName = Session["Name"] != null ? Session["Name"].ToString() : "";
            string currentUserDepartment = Session["Department"] != null ? Session["Department"].ToString() : "";

            int totalMyDepartmentFiles = 0;
            int totalPublicFiles = 0;

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Count files for 'My Department'
                string queryMyDept = "SELECT COUNT(*) FROM files WHERE Privacy = 'My Department' AND (UploaderName IN (SELECT Name FROM Accounts WHERE Department = @Department) OR UploaderName = @UploaderName)";
                using (MySqlCommand cmdMyDept = new MySqlCommand(queryMyDept, conn))
                {
                    cmdMyDept.Parameters.AddWithValue("@UploaderName", currentUserName);
                    cmdMyDept.Parameters.AddWithValue("@Department", currentUserDepartment);
                    totalMyDepartmentFiles = Convert.ToInt32(cmdMyDept.ExecuteScalar());
                }

                // Count files for 'Public'
                string queryPublic = "SELECT COUNT(*) FROM files WHERE Privacy = 'Public'";
                using (MySqlCommand cmdPublic = new MySqlCommand(queryPublic, conn))
                {
                    totalPublicFiles = Convert.ToInt32(cmdPublic.ExecuteScalar());
                }
            }

            // Calculate total documents
            int totalDocuments = totalMyDepartmentFiles + totalPublicFiles;

            // Set the total count to the Label control
            lblTotalDocu.Text = totalDocuments.ToString();
        }

        //TABLE DELETE DOCUMENT FUNCTION
        // Event triggered when deleting a row
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // Get the ControlID of the row being deleted
            string controlID = GridView1.DataKeys[e.RowIndex].Value.ToString();

            // Log the controlID
            System.Diagnostics.Debug.WriteLine($"GridView1_RowDeleting: ControlID={controlID}");

            // Store the ControlID in a hidden field to be used in modal confirmation
            hfDeleteControlID.Value = controlID;

            // Show the delete confirmation modal using JavaScript
            ScriptManager.RegisterStartupScript(this, GetType(), "showDeleteModal", "$('#deleteModal').modal('show');", true);
        }

        // Event triggered when confirming deletion
        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            // Retrieve the ControlID from hidden field
            string controlID = hfDeleteControlID.Value;

            // Log the controlID
            System.Diagnostics.Debug.WriteLine($"btnConfirmDelete_Click: ControlID={controlID}");

            // Call method to delete the file
            DeleteFile(controlID);

            // Optionally, close the modal after deletion
            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "$('#deleteModal').modal('hide');", true);

            // Rebind the GridView and UserActivities after deletion
            BindGridView();
            BindUserActivities();
        }
        private void DeleteFile(string controlID)
        {
            // Log entry to DeleteFile method
            System.Diagnostics.Debug.WriteLine($"DeleteFile: Entry with ControlID={controlID}");

            // Check if the user has the necessary permissions
            List<string> userPermissions = Session["UserPermissions"] as List<string>;
            if (userPermissions == null)
            {
                Response.Redirect("../PAGES/AccessDenied.aspx");
                return;
            }

            string currentUser = Session["Name"].ToString();
            string currentDepartment = Session["Department"].ToString();

            bool canDeleteAny = userPermissions.Contains("Delete all documents");
            bool canDeleteOwn = userPermissions.Contains("Delete their documents only");
            bool canDeleteInDepartment = userPermissions.Contains("Delete documents within their department");

            if (!canDeleteAny && !canDeleteOwn && !canDeleteInDepartment)
            {
                Response.Redirect("../PAGES/AccessDenied.aspx");
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    // Get additional information about the file being deleted
                    string getInfoQuery = @"
                SELECT 
                    f.FileName, 
                    f.UploaderName, 
                    f.Privacy, 
                    f.Category, 
                    a.Department 
                FROM 
                    files f 
                JOIN 
                    Accounts a 
                ON 
                    f.UploaderName = a.Name 
                WHERE 
                    f.ControlID = @controlID";
                    MySqlCommand getInfoCommand = new MySqlCommand(getInfoQuery, connection);
                    getInfoCommand.Parameters.AddWithValue("@controlID", controlID);
                    MySqlDataReader reader = getInfoCommand.ExecuteReader();
                    string fileName = "";
                    string uploaderName = "";
                    string privacyOption = "";
                    string category = "";
                    string documentDepartment = "";
                    if (reader.Read())
                    {
                        fileName = reader.GetString("FileName");
                        uploaderName = reader.GetString("UploaderName");
                        privacyOption = reader.GetString("Privacy");
                        category = reader.IsDBNull(reader.GetOrdinal("Category")) ? "" : reader.GetString(reader.GetOrdinal("Category"));
                        documentDepartment = reader.GetString("Department");
                    }
                    reader.Close();

                    // Check if the user is allowed to delete this file
                    if (!canDeleteAny && !canDeleteOwn && documentDepartment != currentDepartment)
                    {
                        Response.Redirect("../PAGES/AccessDenied.aspx");
                        return;
                    }

                    // Delete the file
                    string deleteQuery = "DELETE FROM files WHERE ControlID = @controlID";
                    MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@controlID", controlID);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    // Log the result of the deletion
                    System.Diagnostics.Debug.WriteLine($"DeleteFile: rowsAffected={rowsAffected}");

                    if (rowsAffected > 0)
                    {
                        // Log the file deletion activity
                        LogAudit(controlID, fileName, currentUser, "Deleted", "Successful", privacyOption, category);
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Deleted File Successfully!');", true);
                    }
                    else
                    {
                        // Log the file deletion activity
                        LogAudit(controlID, fileName, currentUser, "Deleted", "Failed", privacyOption, category);
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Delete File Failed');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"DeleteFile: Exception={ex.Message}");
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('{ex.Message}');", true);
            }
        }





        // EDIT TABLE
        protected bool CanEditDocument(string uploaderName)
        {
            List<string> userPermissions = Session["UserPermissions"] as List<string>;
            string currentUser = Session["Name"].ToString();

            if (userPermissions != null)
            {
                if (userPermissions.Contains("Edit all documents"))
                {
                    return true; // User can edit all documents
                }
                else if (userPermissions.Contains("Edit their documents only") && uploaderName == currentUser)
                {
                    return true; // User can edit their own documents
                }
            }

            return false; // Default to false if no permissions or not allowed
        }
        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            // Get the ControlID
            string controlID = hfControlID.Value;
            string newFileName = txtboxEditFileName.Text;
            string fileExtension = hfFileExtension.Value; // Get the file extension from hidden field
            string newPrivacy = GetSelectedPrivacyOption();
            string currentUser = Session["Name"].ToString();

            string connectionString = GetConnectionString();
            string existingCategory = string.Empty;
            string originalFileName = string.Empty;
            string originalPrivacy = string.Empty;
            int originalFolderID = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve the original file name, category, and privacy
                    string selectQuery = "SELECT FileName, Category, Privacy, FolderID FROM files WHERE ControlID = @controlID";
                    MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);
                    selectCommand.Parameters.AddWithValue("@controlID", controlID);

                    using (MySqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            originalFileName = reader["FileName"].ToString();
                            existingCategory = reader["Category"].ToString();
                            originalPrivacy = reader["Privacy"].ToString();
                            originalFolderID = Convert.ToInt32(reader["FolderID"]);
                        }
                    }

                    // If no new privacy is selected, retain the original privacy
                    string updatedPrivacy = string.IsNullOrEmpty(newPrivacy) ? originalPrivacy : newPrivacy;

                    // Combine the new file name with the original file extension
                    string updatedFileName = newFileName + fileExtension;

                    string updateQuery = "UPDATE files SET FileName = @filename, Privacy = @privacy, FolderID = @folderID WHERE ControlID = @controlID";
                    MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@controlID", controlID);
                    updateCommand.Parameters.AddWithValue("@filename", updatedFileName);
                    updateCommand.Parameters.AddWithValue("@privacy", updatedPrivacy);
                    updateCommand.Parameters.AddWithValue("@folderID", GetSelectedFolderID(updatedPrivacy));

                    int rowsAffected = updateCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        LogAudit(controlID, updatedFileName, currentUser, "Edited", "Successful", updatedPrivacy, existingCategory);
                        ScriptManager.RegisterStartupScript(this, GetType(), "showSuccessAlert", "showSuccessAlert('Updated File Successfully!');", true);
                    }
                    else
                    {
                        LogAudit(controlID, updatedFileName, currentUser, "Edited", "Failed", updatedPrivacy, existingCategory);
                        ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", "showErrorAlert('Updated File Failed');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('{ex.Message}');", true);
            }
            finally
            {
                BindUserActivities();
                BindGridView();
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
            return "Only Me"; // Default privacy option if none is selected
        }
        protected string GetFileNameWithoutExtension(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }
        protected string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName);
        }
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

            // Query to fetch folders based on privacy and department
            string query = "SELECT FolderID, FolderName FROM Folders WHERE Privacy = @Privacy AND DepartmentID = @DepartmentID";

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Privacy", privacyOption);
                command.Parameters.AddWithValue("@DepartmentID", departmentID);

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


        //USER ACTIVITY FUNCTION BELOW
        private void BindUserActivities()
        {
            string userAccess = Session["Access"]?.ToString();
            string currentUserName = Session["Name"]?.ToString();
            string currentUserDepartment = Session["Department"]?.ToString();

            // Check if the user has any of the required permissions
            bool hasPermission = HasPermission(userAccess, "View user activity from all departments") ||
                                 HasPermission(userAccess, "View user activity within their department") ||
                                 HasPermission(userAccess, "View their own activity");

            if (!hasPermission)
            {
                // Display message indicating lack of permission
                rptUserActivities.Visible = false; // Hide the repeater
                lblNoPermissionMessage.Visible = true; // Show the permission message label
                lblNoPermissionMessage.Text = "You do not have the permission to view the user activity.<br /><br />" +
                                              "Please log-in again if you are already granted access to this action.";
                return; // Exit the function early
            }

            // Calculate the timestamp for ten days ago
            DateTime tenDaysAgo = DateTime.Now.AddDays(-10);

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "";

                if (HasPermission(userAccess, "View user activity from all departments"))
                {
                    query = "SELECT UploaderName, FileName, UploadDateTime AS ActivityDateTime, Activity FROM files " +
                    "WHERE (Privacy != 'Only Me') OR (UploaderName = @UploaderName AND Privacy = 'Only Me') " +
                    "AND UploadDateTime >= @TenDaysAgo " +
                    "UNION ALL " +
                    "SELECT UploaderName, FileName, LogDateTime AS ActivityDateTime, Activity FROM documentlogs " +
                    "WHERE LogDateTime >= @TenDaysAgo " +
                    "ORDER BY ActivityDateTime ASC";
                }
                else if (HasPermission(userAccess, "View user activity within their department"))
                {
                    query = "SELECT UploaderName, FileName, UploadDateTime AS ActivityDateTime, Activity FROM files " +
                    "WHERE (UploaderName IN (SELECT Name FROM Accounts WHERE Department = @Department) " +
                    "AND Privacy != 'Only Me') OR (UploaderName = @UploaderName AND Privacy = 'Only Me') " +
                    "AND UploadDateTime >= @TenDaysAgo " +
                    "UNION ALL " +
                    "SELECT UploaderName, FileName, LogDateTime AS ActivityDateTime, Activity FROM documentlogs " +
                    "WHERE (UploaderName IN (SELECT Name FROM Accounts WHERE Department = @Department) " +
                    "OR UploaderName = @UploaderName) " +
                    "AND LogDateTime >= @TenDaysAgo " +
                    "ORDER BY ActivityDateTime ASC";
                }
                else if (HasPermission(userAccess, "View their own activity"))
                {
                    query = "SELECT UploaderName, FileName, UploadDateTime AS ActivityDateTime, Activity FROM files " +
                    "WHERE UploaderName = @UploaderName AND UploadDateTime >= @TenDaysAgo " +
                    "UNION ALL " +
                    "SELECT UploaderName, FileName, LogDateTime AS ActivityDateTime, Activity FROM documentlogs " +
                    "WHERE UploaderName = @UploaderName AND LogDateTime >= @TenDaysAgo " +
                    "ORDER BY ActivityDateTime ASC";
                }

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UploaderName", currentUserName);
                    cmd.Parameters.AddWithValue("@TenDaysAgo", tenDaysAgo);

                    if (query.Contains("@Department"))
                    {
                        cmd.Parameters.AddWithValue("@Department", currentUserDepartment);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Filter out activities older than 10 days
                        DataRow[] filteredRows = dataTable.Select($"ActivityDateTime >= '{tenDaysAgo.ToString("yyyy-MM-dd HH:mm:ss")}'");

                        if (filteredRows.Length > 0)
                        {
                            rptUserActivities.Visible = true; // Show the repeater
                            lblNoPermissionMessage.Visible = false; // Hide the permission message label

                            DataTable filteredTable = filteredRows.CopyToDataTable();
                            // Sort the filtered table by ActivityDateTime descending (latest first)
                            filteredTable.DefaultView.Sort = "ActivityDateTime DESC";
                            rptUserActivities.DataSource = filteredTable;
                            rptUserActivities.DataBind();
                        }
                        else
                        {
                            rptUserActivities.Visible = false; // Hide the repeater
                            lblNoPermissionMessage.Visible = true; // Show the permission message label
                            lblNoPermissionMessage.Text = "No user activities found within the specified timeframe.";
                        }
                    }
                }
            }
        }
        private bool HasPermission(string userAccess, string permissionName)
        {
            string[] permissions = userAccess.Split(',');
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "SELECT PermissionID FROM Permissions WHERE PermissionName = @PermissionName";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PermissionName", permissionName);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        int permissionId = Convert.ToInt32(result);
                        return permissions.Contains(permissionId.ToString());
                    }
                }
            }
            return false;
        }
        private void SetShowAllLink()
        {
            string userAccessLevel = Session["Access"] as string;
            bool viewOwnActivityPermission = HasPermission(userAccessLevel, "View their own activity");
            bool viewDepartmentActivityPermission = HasPermission(userAccessLevel, "View user activity within their department");
            bool viewAllDepartmentsActivityPermission = HasPermission(userAccessLevel, "View user activity from all departments");

            // Check if the user has all permissions
            if (viewDepartmentActivityPermission && viewAllDepartmentsActivityPermission && viewOwnActivityPermission)
            {
                // If the user has all permissions, show the "Show all" link and set its URL
                showAllHyperLink.Visible = true;
                showAllHyperLink.NavigateUrl = "../PAGES/DocumentLogs.aspx";
            }
            else if (viewDepartmentActivityPermission || viewAllDepartmentsActivityPermission ||
                viewDepartmentActivityPermission && viewAllDepartmentsActivityPermission)
            {
                // If the user has either "View user activity within their department" or "View user activity from all departments" permissions,
                // show the "Show all" link and set its URL
                showAllHyperLink.Visible = true;
                showAllHyperLink.NavigateUrl = "../PAGES/DocumentLogs.aspx";
            }
            else
            {
                // Default behavior: hide the "Show all" link for Basic access level users or when no relevant permissions are found
                showAllHyperLink.Visible = false;
            }
        }
        // USER ACTIVITY INTERVAL
        protected double ConvertToUnixEpochMilliseconds(object activityDateTime)
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime activityTime;

            if (DateTime.TryParse(activityDateTime?.ToString(), out activityTime))
            {
                TimeSpan timeSpan = activityTime.ToUniversalTime() - unixEpoch;
                return timeSpan.TotalMilliseconds;
            }
            else
            {
                return 0; // or handle the case where the conversion fails
            }
        }
        protected string GetTimeAgo(object activityDateTime)
        {
            if (activityDateTime != null && activityDateTime != DBNull.Value)
            {
                DateTime activityTime = Convert.ToDateTime(activityDateTime);
                TimeSpan timeSinceActivity = DateTime.Now - activityTime;

                if (timeSinceActivity.TotalSeconds < 60)
                {
                    return $"{(int)timeSinceActivity.TotalSeconds} second{((int)timeSinceActivity.TotalSeconds != 1 ? "s" : "")} ago";
                }
                else if (timeSinceActivity.TotalMinutes < 60)
                {
                    return $"{(int)timeSinceActivity.TotalMinutes} minute{((int)timeSinceActivity.TotalMinutes != 1 ? "s" : "")} ago";
                }
                else if (timeSinceActivity.TotalHours < 24)
                {
                    return $"{(int)timeSinceActivity.TotalHours} hour{((int)timeSinceActivity.TotalHours != 1 ? "s" : "")} ago";
                }
                else if (timeSinceActivity.TotalDays < 10)
                {
                    return $"{(int)timeSinceActivity.TotalDays} day{((int)timeSinceActivity.TotalDays != 1 ? "s" : "")} ago";
                }
                else
                {
                    // If the activity is older than ten days, return the formatted date
                    return activityTime.ToString("MMM dd, yyyy");
                }
            }
            return string.Empty;
        }


       
        // PRINT FUNCTION BELOW
        protected void btnPrint_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string controlID = btn.CommandArgument;

            byte[] fileContent;
            string fileName;

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT FileName, FileContent FROM files WHERE ControlID = @ControlID", conn))
                {
                    cmd.Parameters.AddWithValue("@ControlID", controlID);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            fileName = reader["FileName"].ToString();
                            fileContent = (byte[])reader["FileContent"];
                            string contentType = GetContentType(fileName);
                            string base64Content = Convert.ToBase64String(fileContent);

                            if (contentType == "application/pdf")
                            {
                                string script = $"printPdf('{base64Content}', '{fileName}');";
                                ClientScript.RegisterStartupScript(this.GetType(), "PrintPdfScript", script, true);
                            }
                            else if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" || contentType == "application/vnd.ms-excel")
                            {
                                string script = $"printExcel('{base64Content}');";
                                ClientScript.RegisterStartupScript(this.GetType(), "PrintExcelScript", script, true);
                            }
                            else if (contentType == "text/plain" || contentType == "image/jpeg" || contentType == "image/png")
                            {
                                string script = $"printFile('{base64Content}', '{contentType}', '{fileName}');";
                                ClientScript.RegisterStartupScript(this.GetType(), "PrintScript", script, true);
                            }
                            else if (contentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                            {
                                string script = $"printWord('{base64Content}', '{fileName}');";
                                ClientScript.RegisterStartupScript(this.GetType(), "PrintWordScript", script, true);
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "UnsupportedFileType", $"alert('Unsupported file type for printing: {contentType}');", true);
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
        }
        private string GetContentType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            switch (extension)
            {
                case ".txt": return "text/plain";
                case ".jpg": case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".pdf": return "application/pdf";
                case ".xlsx": case ".xls": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                default: return "application/octet-stream";
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
        //FILE ICONS
        public string GetFileIcon(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            string iconClass = "bx bxs-file";
            string colorClass = "default-color"; // Define a default color class

            switch (extension)
            {
                case ".pdf":
                    iconClass = "bx bxs-file-pdf";
                    colorClass = "pdf-color"; // Define a class for PDF color
                    break;
                case ".doc":
                case ".docx":
                    iconClass = "bx bxs-file-doc";
                    colorClass = "doc-color"; // Define a class for DOC color
                    break;
                case ".jpg":
                case ".jpeg":
                case ".png":
                    iconClass = "bx bxs-file-image";
                    colorClass = "image-color"; // Define a class for image color
                    break;
                case ".xlsx":
                case ".xls":
                case ".xlsb":
                case ".xlsa":
                    iconClass = "bx bxs-file";
                    colorClass = "excel-color"; // Define a class for image color
                    break;
                case ".txt":
                    iconClass = "bx bxs-file-txt";
                    colorClass = "txt-color";
                    break;
                default:
                    iconClass = "bx bxs-file";
                    colorClass = "default-color"; // Define a class for default color
                    break;
            }

            // Return a combination of icon class and color class
            return $"{iconClass} {colorClass}";
        }
        private void LogAudit(string controlID, string fileName, string uploaderName, string activity, string status, string privacyOption, string category)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO documentlogs (ControlID, UploaderName, FileName, LogDateTime, Activity, Status, Privacy, Category) VALUES (@ControlID, @UploaderName, @FileName, @LogDateTime, @Activity, @Status, @Privacy, @Category)";

                        // Add parameters
                        command.Parameters.AddWithValue("@ControlID", controlID);
                        command.Parameters.AddWithValue("@UploaderName", uploaderName);
                        command.Parameters.AddWithValue("@FileName", fileName);
                        command.Parameters.AddWithValue("@LogDateTime", DateTime.Now);
                        command.Parameters.AddWithValue("@Activity", activity);
                        command.Parameters.AddWithValue("@Status", status);
                        command.Parameters.AddWithValue("@Privacy", privacyOption);
                        command.Parameters.AddWithValue("@Category", string.IsNullOrEmpty(category) ? DBNull.Value : (object)category);

                        // Execute query
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("File upload not successful.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showErrorAlert", $"showErrorAlert('{ex.Message}');", true);
            }
        }
        protected void UploadFilebtn_click(object sender, EventArgs e)
        {
            Response.Redirect("/load.aspx?url=../PAGES/Upload.aspx");
        }

        // PREVIEW FUNCTION
        protected void btnPreview_Click(object sender, EventArgs e)
        {
            // Get the GridView row corresponding to the clicked button
            GridViewRow clickedRow = ((Control)sender).NamingContainer as GridViewRow;

            // Get the ControlID from the CommandArgument
            string controlID = ((LinkButton)sender).CommandArgument;

            // Get the data from the selected row
            string fileName = (clickedRow.FindControl("lnkFileName") as LinkButton).Text;
            string uploaderName = clickedRow.Cells[2].Text; // Assuming uploader name is in the second cell
            string uploadDateTime = clickedRow.Cells[3].Text;
            string privacy = (clickedRow.FindControl("lblPrivacy") as Label).Text;

            // Get file extension from file name
            string fileExtension = Path.GetExtension(fileName);

            // Retrieve OCR text from the database
            string ocrText = GetOcrText(controlID);

            // Retrieve file content from the database
            byte[] fileContent = GetFileContentFromDatabase(controlID);
            string fileContentBase64 = Convert.ToBase64String(fileContent);

            // Store data in session variables
            Session["PreviewFileName"] = fileName;
            Session["PreviewUploaderName"] = uploaderName;
            Session["PreviewDepartment"] = GetDepartment(controlID); // Assuming you have implemented GetDepartment method correctly
            Session["PreviewUploadDateTime"] = uploadDateTime;
            Session["PreviewPrivacy"] = privacy;
            Session["PreviewCategory"] = GetCategory(controlID); // Assuming you have implemented GetCategory method correctly
            Session["PreviewControlID"] = controlID;
            Session["PreviewOcrText"] = ocrText;
            Session["PreviewFileContent"] = fileContentBase64; // Store the file content as Base64 string
            Session["FileExtension"] = fileExtension; // Store the file extension

            // Redirect to the Preview page
            Response.Redirect("~/PAGES/Preview.aspx");
        }


        private byte[] GetFileContentFromDatabase(string controlID)
        {
            byte[] fileContent = null;
            string connectionString = GetConnectionString(); // Replace with your connection string retrieval logic

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT FileContent FROM files WHERE ControlID = @ControlID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ControlID", controlID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            fileContent = (byte[])reader["FileContent"];
                        }
                    }
                }
            }

            return fileContent;
        }
        private string GetOcrText(string controlID)
        {
            string ocrText = string.Empty;
            string connectionString = GetConnectionString();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT OCRText FROM files WHERE ControlID = @ControlID";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@ControlID", controlID);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    ocrText = result.ToString();
                }
            }
            return ocrText;
        }
        private string GetDepartment(string controlID)
        {
            string department = string.Empty;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
                    SELECT d.DepartmentName 
                    FROM files f
                    JOIN Accounts a ON f.UploaderName = a.Name
                    JOIN department d ON a.Department = d.ShortAcronym
                    WHERE f.ControlID = @ControlID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ControlID", controlID);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        department = result.ToString();
                    }
                }
            }
            return department;
        }
        private string GetCategory(string controlID)
        {
            string category = string.Empty;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT Category FROM files WHERE ControlID = @ControlID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ControlID", controlID);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        category = result.ToString();
                    }
                }
            }

            return category;
        }

        //DOWNLOAD
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            // Get the file name from the button's CommandArgument
            string fileName = ((LinkButton)sender).CommandArgument;

            // Retrieve the file content from the database based on the file name
            byte[] fileContent = RetrieveFileContentFromDatabase(fileName);

            if (fileContent != null && fileContent.Length > 0)
            {
                try
                {
                    // Set the appropriate headers for the response to initiate download
                    Response.Clear();
                    Response.ContentType = "application/octet-stream";
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(fileContent);
                    Response.End();
                }
                catch (Exception ex)
                {
                    Response.Write("Error: " + ex.Message);
                }
            }
            else
            {
                Response.Write("Error: File not found or empty.");
            }
        }
        private byte[] RetrieveFileContentFromDatabase(string fileName)
        {
            byte[] fileContent = null;

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT FileContent FROM files WHERE FileName = @FileName";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FileName", fileName);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        fileContent = (byte[])result;
                    }
                }
            }

            return fileContent;
        }
    }
}