<%@ Page Title="" Language="C#" MasterPageFile="~/PAGES/topnav.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="DMS.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="../CSS/dashboard.css" />
    <link href='https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css' rel='stylesheet'>
    <link href="https://cdn.lineicons.com/4.0/lineicons.css" rel="stylesheet" />
    <script rel="stylesheet" src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script rel="stylesheet" src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/boxicons/2.1.0/css/boxicons.min.css">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <!-- Print for ms word file -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/mammoth/1.4.2/mammoth.browser.min.js"></script>
    <!-- Include PDF.js scripts for pdf print -->
    <script src="https://mozilla.github.io/pdf.js/build/pdf.js"></script>
    <script src="https://mozilla.github.io/pdf.js/build/pdf.worker.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.11.338/pdf.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.9.359/pdf.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.8.335/pdf.min.js"></script>
    <!-- Include SheetJS/ Excel library for print -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.17.0/xlsx.full.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <div class="mbtn-container">
            <asp:Button runat="server" CssClass="m-upload-btn" OnClick="UploadFilebtn_click" Text="+" />

        </div>
        <!-- Page Heading -->
        <div class="heading">
            <div class="page-heading">Dashboard</div>

        </div>



        <!-- CARDS -->
        <div class="row cards content hidden" id="catalog">
            <!--TOTAL MY DOCS-->
            <div class="card mydocs">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs text-uppercase mb-1 ">
                                My Documents
                            </div>
                            <asp:Label ID="lblTotalMyDocs" runat="server" class="total-number" Text="####"></asp:Label>
                        </div>
                        <div class="col-auto icon">
                            <i class='bx bx-book-open my-docs'></i>
                        </div>
                    </div>
                </div>
            </div>
            <!--TOTAL SCANNED-->
            <div class="card scanned">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs text-uppercase mb-1">
                                Total Scanned
                            </div>
                            <asp:Label ID="lblTotalScan" runat="server" class="total-number" Text="####"></asp:Label>
                        </div>
                        <div class="col-auto icon">
                            <i class='bx bx-scan scan-docs'></i>
                        </div>
                    </div>
                </div>
            </div>
            <!--TOTAL DIGITAL-->
            <div class="card digital">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs text-uppercase mb-1 ">
                                Total Digital
                            </div>
                            <asp:Label ID="lblTotalDigit" runat="server" class="total-number" Text="####"></asp:Label>
                        </div>
                        <div class="col-auto icon">
                            <i class='bx bx-upload digi-docs'></i>
                        </div>
                    </div>
                </div>
            </div>
            <!--TOTAL DEP DOCS-->
            <div class="card dept">
                <div class="card-body">
                    <div class="row no-gutters align-items-center">
                        <div class="col mr-2">
                            <div class="text-xs text-uppercase mb-1 ">
                                Department Documents
                            </div>
                            <asp:Label ID="lblTotalDept" runat="server" class="total-number" Text="####"></asp:Label>
                        </div>
                        <div class="col-auto icon">
                            <i class='bx bx-folder dept-docs'></i>
                        </div>
                    </div>
                </div>
            </div>
            <!--TOTAL DOCS-->
            <div class="card docs">
                <div class="card-body">
                    <div class="row no-gutters">
                        <div class="col mr-2">
                            <div class="text-xs text-uppercase mb-1">
                                Total Documents
                            </div>
                            <asp:Label ID="lblTotalDocu" runat="server" class="total-number" Text="####"></asp:Label>
                        </div>
                        <div class="col-auto icon">
                            <i class='bx bx-file docus'></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>




        <div class="table-card-container">
            <!--TABLE-->

            <div class="custom-gridview-container content" id="table">

                <!--SEARCH & OTHER BUTTONS-->
                <div class="gview-subcontainer d-flex flex-row justify-content-between mb-3 mt-3 ml-3">
                    <div class="search-bar">
                        <asp:TextBox ID="searchtxtbox" runat="server" class="form-control form-control-sm search-textbox" placeholder="Search"
                            aria-label="search" aria-describedby="search" AutoPostBack="true" OnTextChanged="Searchtxtbox_TextChanged"></asp:TextBox>
                    </div>
                    <div class="btn-container mr-3">
                        <asp:Button ID="UploadFilebtn" class="btn btn-sm upload-btn" runat="server" Text="+ Upload File" OnClick="UploadFilebtn_click" />
                        <asp:Button ID="EditAPIbtn" class="btn btn-sm api-btn" runat="server" Text="Edit API" data-toggle="modal" data-target="#editApiKeyModal" />
                    </div>
                </div>

                <!-- Edit API Key Modal -->
                <div class="modal fade" id="editApiKeyModal" tabindex="-1" role="dialog" aria-labelledby="editApiKeyModalLabel" aria-hidden="true">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title" id="editApiKeyModalLabel">Edit API Key for Reset Pass</h5>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div class="modal-body">
                                <div class="form-group">
                                    <label for="currentApiKey">Current API Key:</label>
                                    <input type="text" class="form-control" id="currentApiKey" readonly>
                                    <label for="currentApiEmail">Current API Email:</label>
                                    <input type="text" class="form-control" id="currentApiEmail" readonly>
                                </div>
                                <div class="form-group">
                                    <label for="apiKeyInput">New API Key:</label>
                                    <input type="text" class="form-control" id="apiKeyInput" placeholder="Get new API Key from Brevo">
                                </div>
                                <div class="form-group">
                                    <label for="apiEmailInput">New Sender Email:</label>
                                    <input type="email" class="form-control" id="apiEmailInput" placeholder="Get new sender email address from Brevo">
                                </div>
                                <input type="hidden" id="currentUserID" value="<%= Session["UserID"] %>">
                                <div class="alert alert-success d-none mt-2" id="successAlert_EditAPI" role="alert"></div>
                                <div class="alert alert-danger d-none mt-2" id="errorAlert_EditAPI" role="alert"></div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-primary" id="saveApiKeyBtn">Save changes</button>
                            </div>
                        </div>
                    </div>
                </div>

                <!--LINK BUTTONS-->

                <div class="link-btn-container">
                    <asp:LinkButton ID="AllDocsBtn" class="link-btn active" runat="server" OnClick="AllDocsBtn_Click">All Documents</asp:LinkButton>
                    <asp:LinkButton ID="DeptDocsBtn" class="link-btn" runat="server" OnClick="DeptDocsBtn_Click">Department Documents</asp:LinkButton>
                    <asp:LinkButton ID="MyDocsBtn" class="link-btn" runat="server" OnClick="MyDocsBtn_Click">My Documents</asp:LinkButton>
                    <asp:LinkButton ID="PublicDocsBtn" class="link-btn" runat="server" OnClick="PublicDocsBtn_Click">Public Documents</asp:LinkButton>
                </div>

                <!--GRIDVIEW-->
                <div class="table-body">
                    <asp:GridView ID="GridView1" runat="server" CssClass="custom-gridview" AutoGenerateColumns="false"
                        AllowPaging="true" PageSize="20" OnRowDeleting="GridView1_RowDeleting"
                        DataKeyNames="ControlID">
                        <Columns>
                            <asp:BoundField DataField="ControlID" HeaderText="Control ID" SortExpression="ControlID" ReadOnly="true" />
                            <asp:TemplateField HeaderText="Document Name">
                                <ItemTemplate>
                                    <i class='<%# GetFileIcon(Eval("FileName").ToString()) %> gview-icon'></i>
                                    <asp:LinkButton ID="lnkFileName" runat="server" Text='<%# Eval("FileName") %>' OnClick="btnPreview_Click" CommandArgument='<%# Eval("ControlID") %>' CssClass="file-name-link"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="UploaderName" HeaderText="Uploader" SortExpression="UploaderName" ReadOnly="true" />

                            <%--<asp:BoundField DataField="Department" HeaderText="Department" SortExpression="Department" ReadOnly="true" />--%>
                            <asp:BoundField DataField="UploadDateTime" HeaderText="Date Uploaded" SortExpression="UploadDateTime" ReadOnly="true" />

                            <asp:TemplateField HeaderText="Privacy" SortExpression="Privacy">
                                <ItemTemplate>
                                    <asp:Label ID="lblPrivacy" runat="server" Text='<%# Eval("Privacy") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtPrivacy" runat="server" Text='<%# Bind("Privacy") %>' Width="80px"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="Category" HeaderText="Category" SortExpression="Category" ReadOnly="true" />

                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <ul class="navbar-nav">
                                        <li class="nav-item dropdown no-arrow mr-3">
                                            <a class="nav-link" href="#" id="userDropdown" role="button"
                                                data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                                <i class='bx bx-dots-vertical-rounded'></i>
                                            </a>
                                            <div class="dropdown-menu" aria-labelledby="userDropdown">
                                                <%-- Only show edit option if user has permission --%>
                                                <asp:LinkButton ID="lnkEdit" runat="server" CssClass="dropdown-item edit-btn" data-toggle="modal" data-target="#editModal"
                                                    data-controlid='<%# Eval("ControlID") %>' data-filename='<%# GetFileNameWithoutExtension(Eval("FileName").ToString()) %>' data-fileextension='<%# GetFileExtension(Eval("FileName").ToString()) %>' data-privacy='<%# Eval("Privacy") %>'
                                                    Visible='<%# CanEditDocument(Eval("UploaderName").ToString()) %>'>
                                                    <i class='bx bx-edit-alt'></i>&nbsp; Edit                 
                                                </asp:LinkButton>

                                                <asp:LinkButton ID="btnDownload" runat="server" CssClass="dropdown-item"
                                                    OnClick="btnDownload_Click" CommandArgument='<%# Eval("FileName") %>'>
                            <i class='bx bxs-download'></i>&nbsp; Download
                                                </asp:LinkButton>
                                                <%--                                                <asp:LinkButton runat="server" CssClass="dropdown-item" OnClick="btnPreview_Click">
                            <i class="lni lni-eye"></i>&nbsp Preview
                                                </asp:LinkButton>--%>
                                                <asp:LinkButton runat="server" CssClass="dropdown-item" OnClick="btnPrint_Click" CommandArgument='<%# Eval("ControlID") %>'>
                            <i class='bx bx-printer'></i>&nbsp; Print
                                                </asp:LinkButton>

                                                <%-- Only show delete option if user has permission --%>
                                                <asp:PlaceHolder ID="phDelete" runat="server"
                                                    Visible='<%# Session["UserPermissions"] != null && 
(((List<string>)Session["UserPermissions"]).Contains("Delete all documents") || 
(((List<string>)Session["UserPermissions"]).Contains("Delete their documents only") && 
Eval("UploaderName").ToString() == Session["Name"].ToString()) ||
(((List<string>)Session["UserPermissions"]).Contains("Delete documents within their department") && 
Eval("Department").ToString() == Session["Department"].ToString())) %>'>
                                                    <asp:LinkButton runat="server" CssClass="dropdown-item" CommandName="Delete"
                                                        CommandArgument='<%# Eval("ControlID") %>' OnClientClick='<%# "showDeleteModal(\"" + Eval("ControlID") + "\"); return false;" %>'>
                     <i class='bx bx-trash'></i>&nbsp; Delete
                       
                                                    </asp:LinkButton>
                                                </asp:PlaceHolder>

                                            </div>
                                        </li>
                                    </ul>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" Text="Update" CssClass="badge rounded-pill update-linkbtn" />
                                    <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" Text="Cancel" CssClass="badge rounded-pill cancel-linkbtn" />
                                </EditItemTemplate>

                                <FooterTemplate>
                                    <asp:Label ID="lblFooterFileName" runat="server" Text="Footer Text Here"></asp:Label>
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerSettings Visible="false" />
                        <EmptyDataTemplate>
                            <div class="alert alert-info" role="alert">
                                No results found.
               
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>

                <!-- Delete Confirmation Modal -->
                <asp:HiddenField ID="hfDeleteControlID" runat="server" />
                <div id="deleteModal" class="modal fade" role="dialog">
                    <div class="modal-dialog modal-dialog-centered" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Confirm Delete</h5>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div class="modal-body">
                                <p>Are you sure you want to delete this file?</p>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnConfirmDelete" runat="server" Text="Delete" CssClass="btn btn-danger" OnClick="btnConfirmDelete_Click" />
                                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                            </div>
                        </div>
                    </div>
                </div>

                <!--PAGINATION-->
                <nav aria-label="Page navigation" class="custom-pagination">
                    <ul class="pagination">
                        <div class="page-cont">
                            <span>Go to page:</span>
                            <asp:TextBox ID="txtPageNumber" runat="server" CssClass="form-control search-num" Width="30px" AutoPostBack="true" OnTextChanged="txtPageNumber_TextChanged"></asp:TextBox>
                            &nbsp;
                        </div>
                    </ul>
                    <ul class="pagination">
                        <div class="page-cont">
                            <li class="page-item">
                                <asp:LinkButton ID="btnPrev" runat="server" CssClass="page-link" OnClick="btnPrev_Click" aria-label="Previous">
                                <i class='bx bx-chevron-left'></i>
                                </asp:LinkButton>
                            </li>
                            <span class="page-of">Page
                             <asp:Label ID="lblPageNum" runat="server"></asp:Label>
                                of
                            <asp:Label ID="lblTotalPages" runat="server"></asp:Label>
                            </span>

                            <%--<asp:Button ID="btnGoToPage" runat="server" CssClass="btn page-btn" Text="Go" OnClick="btnGoToPage_Click" />--%>



                            <li class="page-item">
                                <asp:LinkButton ID="btnNext" runat="server" CssClass="page-link" OnClick="btnNext_Click" aria-label="Next">
                                <i class='bx bx-chevron-right'></i>
                                </asp:LinkButton>
                            </li>


                        </div>
                    </ul>
                </nav>



            </div>


            <!--USER ACTIVITY-->
            <div class="user-activity content hidden" id="user-activity">

                <div class="card-header">
                    User Activity
                </div>
                <div class="useract-card-body">

                    <asp:Repeater ID="rptUserActivities" runat="server">
                        <ItemTemplate>
                            <div class="user-activity-card">
                                <div class="user-activity-body">
                                    <h6><%# Eval("UploaderName") %></h6>
                                    <div class="d-flex justify-content-between">
                                        <p>
                                            <%# Eval("FileName") %>
                                        </p>
                                        <p>
                                            <%# Eval("Activity") %>
                                        </p>
                                    </div>
                                    <span>
                                        <p>
                                            <asp:Label ID="lblMinutesAgo" runat="server" CssClass="minutes-ago" data-activitydatetime='<%# ConvertToUnixEpochMilliseconds(Eval("ActivityDateTime")) %>'></asp:Label>
                                        </p>
                                    </span>

                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <!-- Label for displaying no permission message -->
                    <asp:Label ID="lblNoPermissionMessage" runat="server" Visible="false" Text="" Mode="MultiLine"></asp:Label>
                </div>
                <div class="card-footer">
                    <asp:HyperLink ID="showAllHyperLink" runat="server" class="show-all-btn" data-abc="true">Show all</asp:HyperLink>
                </div>
            </div>
        </div>
    </div>
    <!-- EDIT MODAL -->
    <div class="modal fade" id="editModal" tabindex="-1" role="dialog" aria-labelledby="editModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-body">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <h5>Edit file</h5>
                    <br />
                    <asp:HiddenField ID="hfControlID" runat="server" />
                    <asp:HiddenField ID="hfFileExtension" runat="server" />
                    <div class="column mb-3">
                        <label for="lblEditFileName" class="col-form-label">File Name</label>
                        <asp:TextBox ID="txtboxEditFileName" runat="server" class="form-control"></asp:TextBox>
                    </div>
                    <div class="column mb-3">
                        <label class="card-title">Select Privacy</label>
                        <div class="col-sm">
                            <div class="form-check-group">
                                <div class="form-check form-check-inline">
                                    <asp:RadioButton ID="rbOnlyMe" runat="server" GroupName="privacyOption" onclick="showFoldersDropdown()" />
                                    Only Me
                                </div>
                                <div class="form-check form-check-inline">
                                    <asp:RadioButton ID="rbMyDepartment" runat="server" GroupName="privacyOption" onclick="showFoldersDropdown()" />
                                    My Department
                                </div>
                                <div class="form-check form-check-inline">
                                    <asp:RadioButton ID="rbPublic" runat="server" GroupName="privacyOption" onclick="showFoldersDropdown()" />
                                    Public
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="column mb-3" id="foldersRow" style="display: none;">
                        <label class="card-title">Select Folder</label>
                        <div class="col-sm">
                            <asp:DropDownList ID="ddlOnlyMe" runat="server" CssClass="dropdown-toggle" Style="display: none;">
                                <asp:ListItem Text="Only Me" Value="" />
                            </asp:DropDownList>
                            <asp:DropDownList ID="ddlMyDepartment" runat="server" CssClass="dropdown-toggle" Style="display: none;">
                                <asp:ListItem Text="My Department" Value="" />
                            </asp:DropDownList>
                            <asp:DropDownList ID="ddlPublic" runat="server" CssClass="dropdown-toggle" Style="display: none;">
                                <asp:ListItem Text="Public" Value="" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="column mb-3">
                        <div class="alert alert-success d-none mb-2" id="successAlert_EditModal" role="alert"></div>
                        <div class="alert alert-danger d-none mb-2" id="errorAlert_EditModal" role="alert"></div>
                    </div>
                    <div class="d-flex flex-row justify-content-end mt-3">
                        <asp:Button ID="btnSaveChanges" runat="server" Text="Save Changes" class="btn btn-sm upload-btn" OnClick="btnSaveChanges_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- BOTTOM NAV -->
    <div class="nav">
        <div class="nav-item bot active" data-tab="table" onclick="showTab('table')">
            <i class='bx bx-table'></i>
            <span class="nav-text">Table</span>
        </div>
        <div class="nav-item bot" data-tab="catalog" onclick="showTab('catalog')">
            <i class='bx bx-library'></i>
            <span class="nav-text">Catalog</span>
        </div>
        <div class="nav-item bot" data-tab="user-activity" onclick="showTab('user-activity')">
            <i class='bx bx-user'></i>
            <span class="nav-text">User Activities</span>
        </div>
    </div>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/docx/6.0.3/docx.min.js"></script>

    <script type="text/javascript">
        // EDIT API ALERT MODAL
        function showErrorAlertAPI(message) {
            $('#errorAlert_EditAPI').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_EditAPI').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }
        function showSuccessAlertAPI(message) {
            $('#successAlert_EditAPI').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert_EditAPI').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }

        /*API MODAL*/
        $(document).ready(function () {
            $('.api-btn').click(function (e) {
                e.preventDefault();

                // Fetch the current API key and email
                $.ajax({
                    type: "POST",
                    url: "../LOGIN/ResetPW.aspx/GetApiKeyandEmail",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        // Parse JSON response
                        var apiData = JSON.parse(response.d);

                        // Set the current API key and email in the modal
                        $('#currentApiKey').val(apiData.ApiKey);
                        $('#currentApiEmail').val(apiData.ApiEmail);
                    },
                    error: function (error) {
                        console.error("Error fetching API key and email:", error);
                        showErrorAlertAPI("Failed to fetch API key and email.");
                    }
                });

                $('#editApiKeyModal').modal('show');
            });

            // Handle saving changes to API key and email
            $('#saveApiKeyBtn').click(function () {
                var newApiKey = $('#apiKeyInput').val();
                var newApiEmail = $('#apiEmailInput').val();
                var currentUserID = $('#currentUserID').val(); // Get current user ID

                // Validate inputs
                if (newApiKey === '' || newApiEmail === '') {
                    showErrorAlertAPI("API Key and Email cannot be empty.");
                    return; // Exit function if inputs are empty
                }

                // Update API key and email via AJAX call
                $.ajax({
                    type: "POST",
                    url: "../LOGIN/ResetPW.aspx/UpdateApiKeyAndEmail",
                    data: JSON.stringify({ newApiKey: newApiKey, newApiEmail: newApiEmail, userID: currentUserID }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        // Update current API key and email displayed in modal
                        $('#currentApiKey').val(newApiKey);
                        $('#currentApiEmail').val(newApiEmail);
                        // Clear the API key and email input fields
                        $('#apiKeyInput').val('');
                        $('#apiEmailInput').val('');
                        // Display success message
                        showSuccessAlertAPI("API Key and Email updated successfully!");
                    },
                    error: function (error) {
                        console.error("Error updating API key and email:", error);
                        showErrorAlertAPI("Failed to update API key and email.");
                    }
                });
            });

            $('.close').click(function (e) {
                e.preventDefault();
                $('#editApiKeyModal').modal('hide');
            });
        });




        /*DELETE MODAL*/
        function showDeleteModal(controlID) {
            document.getElementById('<%= hfDeleteControlID.ClientID %>').value = controlID;
            $('#deleteModal').modal('show');
        }

        // PRINT FUNCTION
        function printFile(base64Content, contentType, fileName) {
            const byteCharacters = atob(base64Content);
            const byteNumbers = new Array(byteCharacters.length);
            for (let i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }
            const byteArray = new Uint8Array(byteNumbers);
            const blob = new Blob([byteArray], { type: contentType });

            const url = URL.createObjectURL(blob);
            const iframe = document.createElement('iframe');
            iframe.style.display = 'none';
            iframe.src = url;

            document.body.appendChild(iframe);

            iframe.onload = function () {
                if (contentType === 'text/plain') {
                    const doc = iframe.contentDocument || iframe.contentWindow.document;
                    const pre = document.createElement('pre');
                    pre.style.margin = '0';
                    pre.style.padding = '0';
                    pre.style.fontFamily = 'monospace';
                    pre.style.whiteSpace = 'pre-wrap';
                    pre.style.overflowWrap = 'break-word';
                    pre.textContent = byteCharacters;

                    doc.body.innerHTML = '';
                    doc.body.appendChild(pre);
                } else if (contentType === 'image/jpeg' || contentType === 'image/png') {
                    const img = document.createElement('img');
                    img.src = url;
                    img.style.width = '100%';

                    const doc = iframe.contentDocument || iframe.contentWindow.document;
                    doc.body.innerHTML = '';
                    doc.body.appendChild(img);
                } else if (contentType === 'application/pdf') {
                    printPdf(base64Content, fileName);
                } else if (contentType === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet') {
                    printExcel(base64Content, fileName);
                } else if (contentType === 'application/vnd.openxmlformats-officedocument.wordprocessingml.document') {
                    printWord(base64Content, fileName);
                } else {
                    alert('Unsupported file type for printing: ' + contentType);
                }

                setTimeout(function () {
                    iframe.contentWindow.print();
                    URL.revokeObjectURL(url);
                    document.body.removeChild(iframe);
                }, 1);
            };
        }
        function printWord(base64Content, fileName) {
            const byteCharacters = atob(base64Content);
            const byteNumbers = new Array(byteCharacters.length);
            for (let i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }
            const byteArray = new Uint8Array(byteNumbers);
            const blob = new Blob([byteArray], { type: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' });

            const reader = new FileReader();
            reader.onload = function (event) {
                const arrayBuffer = event.target.result;

                mammoth.convertToHtml({ arrayBuffer: arrayBuffer })
                    .then(displayResult)
                    .catch(handleError);

                function displayResult(result) {
                    const iframe = document.createElement('iframe');
                    iframe.style.position = 'absolute';
                    iframe.style.width = '0px';
                    iframe.style.height = '0px';
                    iframe.style.border = 'none';
                    document.body.appendChild(iframe);

                    const docContent = iframe.contentWindow.document;
                    docContent.open();
                    docContent.write('<html><head><title>Print Word</title></head><body>');
                    docContent.write('<style>img { max-width: 100%; height: auto; }</style>'); // Ensure images fit within iframe
                    docContent.write(result.value);
                    docContent.write('</body></html>');
                    docContent.close();

                    // Adjust iframe height if needed for content visibility
                    iframe.style.height = docContent.body.scrollHeight + 'px';

                    // Wait for images to load
                    const images = docContent.querySelectorAll('img');
                    let imagesLoaded = 0;
                    images.forEach(img => {
                        img.onload = function () {
                            imagesLoaded++;
                            if (imagesLoaded === images.length) {
                                iframe.contentWindow.focus();
                                iframe.contentWindow.print();
                                document.body.removeChild(iframe);
                            }
                        };
                    });

                    // If no images, directly print
                    if (images.length === 0) {
                        iframe.contentWindow.focus();
                        iframe.contentWindow.print();
                        document.body.removeChild(iframe);
                    }
                }

                function handleError(err) {
                    console.log(err);
                    alert('Error processing Word document: ' + err);
                }
            };
            reader.readAsArrayBuffer(blob);
        }
        function printExcel(base64Content) {
            const binary = atob(base64Content);
            const array = new Uint8Array(binary.length);
            for (let i = 0; i < binary.length; i++) {
                array[i] = binary.charCodeAt(i);
            }
            const workbook = XLSX.read(array, { type: 'array' });
            const worksheet = workbook.Sheets[workbook.SheetNames[0]];
            const htmlString = XLSX.utils.sheet_to_html(worksheet);

            // Create an invisible iframe
            const iframe = document.createElement('iframe');
            iframe.style.position = 'absolute';
            iframe.style.width = '0px';
            iframe.style.height = '0px';
            iframe.style.border = 'none';
            document.body.appendChild(iframe);

            // Write the HTML string to the iframe
            const doc = iframe.contentWindow.document;
            doc.open();
            doc.write('<html><head><title>Print Excel</title></head><body>');
            doc.write(htmlString);
            doc.write('</body></html>');
            doc.close();

            // Wait for the content to load and then print
            iframe.contentWindow.focus();
            iframe.contentWindow.print();

            // Remove the iframe after printing
            document.body.removeChild(iframe);
        }
        function printPdf(base64Content, fileName) {
            // Create a Blob object from base64 content
            var byteCharacters = atob(base64Content);
            var byteNumbers = new Array(byteCharacters.length);
            for (var i = 0; i < byteCharacters.length; i++) {
                byteNumbers[i] = byteCharacters.charCodeAt(i);
            }
            var byteArray = new Uint8Array(byteNumbers);
            var blob = new Blob([byteArray], { type: 'application/pdf' });

            // Create a URL for the Blob object
            var url = URL.createObjectURL(blob);

            // Create an iframe element dynamically
            var iframe = document.createElement('iframe');
            iframe.style.display = 'none'; // Hide the iframe
            document.body.appendChild(iframe);

            // Load the PDF content into the iframe
            iframe.src = url;

            // Wait for the iframe to load and then trigger the print dialog
            iframe.onload = function () {
                iframe.contentWindow.print();
            };
        }

        function showTab(tabName) {
            // Hide all content divs
            var contents = document.querySelectorAll('.content');
            contents.forEach(content => {
                content.classList.add('hidden');
            });

            // Remove active class from all nav items
            var navItems = document.querySelectorAll('.nav-item');
            navItems.forEach(item => {
                item.classList.remove('active');
            });

            // Show the content related to the clicked nav item
            var activeContent = document.getElementById(tabName);
            if (activeContent) {
                activeContent.classList.remove('hidden');
            }

            // Set the clicked nav item as active
            var activeItem = document.querySelector(`.nav-item[data-tab="${tabName}"]`);
            if (activeItem) {
                activeItem.classList.add('active');
            }
        }


        window.onload = function () {
            updateTimeAgo(); // Update immediately on load
            setInterval(updateTimeAgo, 1000); // Update every second for countdown precision
        };
        function updateTimeAgo() {
            var labels = document.getElementsByClassName("minutes-ago");
            var tenDaysInMillis = 10 * 24 * 60 * 60 * 1000; // 10 days in milliseconds

            for (var i = 0; i < labels.length; i++) {
                var activityDateTime = new Date(parseInt(labels[i].getAttribute("data-activitydatetime")));
                var now = new Date();
                var timeDifference = now - activityDateTime;

                if (timeDifference < 60000) { // Less than a minute
                    var secondsAgo = Math.round(timeDifference / 1000);
                    labels[i].innerHTML = secondsAgo + (secondsAgo === 1 ? " second ago" : " seconds ago");
                } else if (timeDifference < 3600000) { // Less than an hour
                    var minutesAgo = Math.round(timeDifference / (1000 * 60));
                    labels[i].innerHTML = minutesAgo + (minutesAgo === 1 ? " minute ago" : " minutes ago");
                } else if (timeDifference < 86400000) { // Less than a day
                    var hoursAgo = Math.round(timeDifference / (1000 * 60 * 60));
                    labels[i].innerHTML = hoursAgo + (hoursAgo === 1 ? " hour ago" : " hours ago");
                } else if (timeDifference < tenDaysInMillis) { // Less than ten days
                    var daysAgo = Math.round(timeDifference / (1000 * 60 * 60 * 24));
                    labels[i].innerHTML = daysAgo + (daysAgo === 1 ? " day ago" : " days ago");
                } else { // More than ten days, hide the label
                    labels[i].style.display = "none";
                }
            }
        }

        function changePage(pageIndex) {
            // Ensure page index is within valid range
            var pageCount = <%= GridView1.PageCount %>;
            if (pageIndex >= 0 && pageIndex < pageCount) {
                __doPostBack('<%= GridView1.ClientID %>', 'Page$' + pageIndex);
            }
        }

        /*Dropdown*/
        function changeButtonText(text) {
            document.getElementById('dropdownMenuButton').innerText = text;
        }
        /*End Dropdown*/

        function showFoldersDropdown() {
            var rbOnlyMe = document.getElementById('<%= rbOnlyMe.ClientID %>');
            var rbMyDepartment = document.getElementById('<%= rbMyDepartment.ClientID %>');
            var rbPublic = document.getElementById('<%= rbPublic.ClientID %>');
            var ddlOnlyMe = document.getElementById('<%= ddlOnlyMe.ClientID %>');
            var ddlMyDepartment = document.getElementById('<%= ddlMyDepartment.ClientID %>');
            var ddlPublic = document.getElementById('<%= ddlPublic.ClientID %>');
            var foldersRow = document.getElementById('foldersRow');

            if (rbOnlyMe.checked) {
                foldersRow.style.display = 'block';
                ddlOnlyMe.style.display = 'block';
                ddlMyDepartment.style.display = 'none';
                ddlPublic.style.display = 'none';
            } else if (rbMyDepartment.checked) {
                foldersRow.style.display = 'block';
                ddlOnlyMe.style.display = 'none';
                ddlMyDepartment.style.display = 'block';
                ddlPublic.style.display = 'none';
            } else if (rbPublic.checked) {
                foldersRow.style.display = 'block';
                ddlOnlyMe.style.display = 'none';
                ddlMyDepartment.style.display = 'none';
                ddlPublic.style.display = 'block';
            } else {
                foldersRow.style.display = 'none';
                ddlOnlyMe.style.display = 'none';
                ddlMyDepartment.style.display = 'none';
                ddlPublic.style.display = 'none';
            }
        }

        // ALERT MODAL
        function showErrorAlert(message) {
            $('#errorAlert_EditModal').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_EditModal').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }

        function showSuccessAlert(message) {
            $('#successAlert_EditModal').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert_EditModal').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }
        /*EDIT MODAL*/
        $(document).ready(function () {
            $('.edit-btn').click(function (e) {
                e.preventDefault();
                var controlID = $(this).data('controlid');
                var filename = $(this).data('filename');
                var fileExtension = $(this).data('fileextension');
                var privacy = $(this).data('privacy'); // Get the privacy from the data attribute

                $('#<%= hfControlID.ClientID %>').val(controlID);
                $('#<%= hfFileExtension.ClientID %>').val(fileExtension);
                $('#<%= txtboxEditFileName.ClientID %>').val(filename);

                switch (privacy) {
                    case "Only Me":
                        $('#<%= rbOnlyMe.ClientID %>').prop('checked', true);
                        break;
                    case "My Department":
                        $('#<%= rbMyDepartment.ClientID %>').prop('checked', true);
                        break;
                    case "Public":
                        $('#<%= rbPublic.ClientID %>').prop('checked', true);
                        break;
                    default:
                        // Default to "Only Me" if no privacy is provided
                        $('#<%= rbOnlyMe.ClientID %>').prop('checked', true);
                        break;
                }


                $('#editModal').modal('show');
            });

            $('.close').click(function (e) {
                e.preventDefault();
                $('#editModal').modal('hide');
            });
        });

        /*LINK BUTTONS*/
        function setActive(buttonId) {
            var buttons = document.querySelectorAll('.link-btn');
            buttons.forEach(function (button) {
                button.classList.remove('active');
            });

            var selectedButton = document.querySelector('[onclick*="' + buttonId + '"]');
            selectedButton.classList.add('active');

            return false; // to prevent page scroll
        }
    </script>
</asp:Content>
