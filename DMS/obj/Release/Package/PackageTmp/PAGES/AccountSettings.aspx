<%@ Page Title="" Language="C#" MasterPageFile="~/PAGES/topnav.Master" AutoEventWireup="true" CodeBehind="AccountSettings.aspx.cs" Inherits="DMS.PAGES.AccountSettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="../CSS/settings.css" />
    <link href='https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css' rel='stylesheet'>
    <link href="https://cdn.lineicons.com/4.0/lineicons.css" rel="stylesheet" />
    <script rel="stylesheet" src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script rel="stylesheet" src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <!-- Page Heading -->
        <div class="heading">
            <div class="page-heading">Accounts</div>
            <div class="btn-container">
                <asp:Button ID="addUserBtn" class="btn add-btn" runat="server" Text="+ Add User" data-toggle="modal" data-target="#addUserModal" />
            </div>
        </div>

        <div class="custom-gridview-container shadow-sm">

            <div class="filterz">
                <%-- Search --%>
                <div class="search-bar">
                    <asp:TextBox ID="searchtxtbox" runat="server" class="form-control form-control-sm acc-search-textbox" placeholder="Search"
                        aria-label="search" aria-describedby="search" AutoPostBack="true" OnTextChanged="searchtxtbox_TextChanged"></asp:TextBox>
                </div>
                <div class="table-filters">
                    <div class="date-activity">
                        <%-- Dept Filter --%>
                        <div class="btn-container">
                            <asp:DropDownList ID="deptFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="deptFilter_SelectedIndexChanged" CssClass="btn btn-sm dropdown-toggle custom-dropdown">
                                <asp:ListItem Enabled="true" Text="Filter by Department" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="status-reset">
                        <%-- Status Filter --%>
                        <div class="btn-container">
                            <asp:DropDownList ID="statusFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="statusFilter_SelectedIndexChanged" CssClass="btn btn-sm dropdown-toggle custom-dropdown">
                                <asp:ListItem Enabled="true" Text="Filter by Status" Value=""></asp:ListItem>
                                <asp:ListItem Enabled="true" Text="Active" Value="Active"></asp:ListItem>
                                <asp:ListItem Enabled="true" Text="Inactive" Value="Inactive"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <%-- RESET --%>
                        <div>
                            <asp:Button ID="Button1" runat="server" Text="Reset Filters" CssClass="btn reset-btn" OnClick="resetfilterBtn_Click" />
                        </div>
                    </div>
                </div>
            </div>
            <!-- Table Card for Accounts -->
            <div class="table-responsive">
                <div class="col-sm-13">
                    <div class="alert alert-success d-none mb-2" id="successAlert_EditAccount" role="alert">
                    </div>
                </div>
                <div class="col-sm-13">
                    <div class="alert alert-danger d-none mb-2" id="errorAlert_EditAccount" role="alert">
                    </div>
                </div>
                <div class="col-sm-13">
                    <div class="alert alert-success d-none mb-2" id="successAlert_AddUserModal" role="alert">
                    </div>
                </div>
                <asp:GridView ID="GridView1" runat="server" CssClass="custom-gridview" AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="20"
                    DataKeyNames="UserID">
                    <Columns>
                        <%-- Name --%>
                        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                        <%-- Username --%>
                        <asp:BoundField DataField="Username" HeaderText="Username" SortExpression="Username" />
                        <%-- Department --%>
                        <asp:BoundField DataField="Department" HeaderText="Department" SortExpression="Department" />
                        <%-- Email --%>
                        <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                        <%-- Contact --%>
                        <asp:BoundField DataField="Contact" HeaderText="Contact" SortExpression="Contact" />
                        <%-- Status --%>
                        <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <%-- Position --%>
                        <asp:BoundField DataField="Position" HeaderText="Position" SortExpression="Position" />
                        <%-- Actions --%>
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <ul class="navbar-nav">
                                    <li class="nav-item dropdown no-arrow mr-3">
                                        <a class="nav-link" href="#" id="userDropdown" role="button"
                                            data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                            <i class='bx bx-dots-vertical-rounded'></i>
                                        </a>
                                        <div class="dropdown-menu" aria-labelledby="userDropdown">
                                            <asp:LinkButton ID="lnkEdit" runat="server" CssClass="dropdown-item edit-btn" data-toggle="modal" data-target="#editUserModal"
                                                data-edituserid='<%# Eval("UserID") %>' Visible='<%# ShouldShowEditButton(Eval("UserID")) %>'>
                                                <i class='bx bx-edit-alt'></i>&nbsp; Edit
                                            </asp:LinkButton>



                                            <asp:LinkButton ID="lnkEditAccess" runat="server" CssClass="dropdown-item" OnClick="lnkEditAccess_Click"
                                                CommandArgument='<%# Eval("UserID") %>'>
                                                    <i class="lni lni-eye"></i>&nbsp; Edit Access
                                            </asp:LinkButton>
                                        </div>
                                    </li>
                                </ul>
                            </ItemTemplate>
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
    </div>




    <%-- ADD MODAL --%>
    <div class="modal fade" id="addUserModal" tabindex="-1" aria-labelledby="addUserModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-body">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>

                    <%-- CHECKLIST --%>
                    <div class="checkbox d-none">
                        <h5>User Rights</h5>
                        <div class="user-rights">
                            <div class="rights mr-3">
                                <p>Basic Access</p>
                                <asp:CheckBoxList runat="server" ID="CheckBoxListBasic" CssClass="check"></asp:CheckBoxList>
                            </div>
                            <div class="rights mr-3">
                                <p>Advanced Access</p>
                                <asp:CheckBoxList runat="server" ID="CheckBoxListAdvanced" CssClass="check"></asp:CheckBoxList>
                            </div>
                            <div class="rights">
                                <p>Master Access</p>
                                <asp:CheckBoxList runat="server" ID="CheckBoxListMaster" CssClass="check"></asp:CheckBoxList>
                            </div>
                        </div>
                        <div class="mt-3">
                            <input type="checkbox" id="selectAll" onchange="handleSelectAllChange(this)">
                            <label for="selectAll">Select all</label>
                            <input type="checkbox" id="unselectAll" onchange="handleUnselectAllChange(this)">
                            <label for="unselectAll">Unselect all</label>
                        </div>
                    </div>



                    <div class="user-info">
                        <h5>Add User</h5>
                        <%-- NAME --%>
                        <div class="row mb-2">
                            <label id="nameTextboxLabel" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Name</label>
                            <div class="col-sm-10">
                                <asp:TextBox ID="nameTextbox" runat="server" class="form-control form-control-sm"></asp:TextBox>
                            </div>
                        </div>
                        <%-- USERNAME --%>
                        <div class="row mb-2">
                            <label id="unameTextBoxLabel" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Username</label>
                            <div class="col-sm-10">
                                <asp:TextBox ID="unameTextBox" runat="server" class="form-control form-control-sm"></asp:TextBox>
                            </div>
                        </div>
                        <%-- EMAIL --%>
                        <div class="row mb-2">
                            <label id="emailTextboxLabel" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Email</label>
                            <div class="col-sm-10">
                                <asp:TextBox ID="emailTextbox" runat="server" class="form-control form-control-sm"></asp:TextBox>
                            </div>
                        </div>
                        <%-- CONTACT --%>
                        <div class="row mb-2">
                            <label id="contactTextBoxLabel" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Contact Number</label>
                            <div class="col-sm-10">
                                <asp:TextBox ID="contactTextBox" runat="server" class="form-control form-control-sm"></asp:TextBox>
                            </div>
                        </div>
                        <%-- DEPARTMENT --%>
                        <div class="row mb-2">
                            <label id="deptDropdownLabel" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Department</label>
                            <div class="col-sm-10">
                                <div class="dropdownModal">
                                    <asp:DropDownList ID="deptDropdown" runat="server" CssClass="btn btn-sm dropdown-toggle custom-dropdown">
                                        <asp:ListItem Enabled="true" Text="Select Department" Value="-1"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <%-- Position --%>
                        <div class="row mb-2">
                            <label id="positionTextBoxLabel" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Position</label>
                            <div class="col-sm-10">
                                <asp:TextBox ID="positionTextBox" runat="server" class="form-control form-control-sm"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm-13">
                            <div class="alert alert-danger d-none mb-2" id="errorAlert_AddUserModal" role="alert">
                            </div>
                        </div>
                    </div>

                    <div class="d-flex flex-row justify-content-end mt-3">
                        <asp:Button ID="nextBtn" runat="server" class="btn btn-sm next" Text="Next" OnClientClick="hideTextboxesAndLabels(); return false;" />
                        <asp:Button ID="backBtn" runat="server" class="btn btn-sm mr-2 next" Text="Back" Style="display: none;" OnClientClick="showTextboxesAndLabels(); return false;" />
                        <asp:Button ID="submitBtn" runat="server" class="btn btn-sm submit" Text="Submit" Style="display: none;" OnClick="submitBtn_Click" />
                    </div>


                </div>
            </div>
        </div>
    </div>

    <%-- EDIT MODAL --%>
    <div class="modal fade" id="editUserModal" tabindex="-1" aria-labelledby="editUserModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-body">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <asp:HiddenField ID="getUserID" runat="server" />
                    <div class="user-info">
                        <h5>Edit User Information</h5>
                        <%-- NAME --%>
                        <div class="row mb-2">
                            <label id="lblEditName" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Name</label>
                            <div class="col-sm-10">
                                <asp:TextBox ID="txtboxEditName" runat="server" class="form-control form-control-sm"></asp:TextBox>
                            </div>
                        </div>
                        <%-- EMAIL --%>
                        <div class="row mb-2">
                            <label id="lblEditEmail" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Email</label>
                            <div class="col-sm-10">
                                <asp:TextBox ID="txtboxEditEmail" runat="server" class="form-control form-control-sm"></asp:TextBox>
                            </div>
                        </div>
                        <%-- CONTACT --%>
                        <div class="row mb-2">
                            <label id="lblEditContact" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Contact Number</label>
                            <div class="col-sm-10">
                                <asp:TextBox ID="txtboxEditContact" runat="server" class="form-control form-control-sm"></asp:TextBox>
                            </div>
                        </div>
                        <%-- DEPARTMENT --%>
                        <div class="row mb-2">
                            <label id="lblEditDept" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Department</label>
                            <div class="col-sm-10">
                                <div class="dropdownModal">
                                    <asp:DropDownList ID="ddlEditDept" runat="server" CssClass="btn btn-sm dropdown-toggle custom-dropdown">
                                        <asp:ListItem Enabled="true" Text="Select Department" Value="-1"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <%-- STATUS --%>
                        <div class="row mb-2">
                            <label id="lblEditStatus" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Status</label>
                            <div class="col-sm-10">
                                <div class="dropdownModal">
                                    <asp:DropDownList ID="ddlEditStatus" runat="server" CssClass="btn btn-sm dropdown-toggle custom-dropdown">
                                        <asp:ListItem Enabled="true" Text="Select Status" Value=""></asp:ListItem>
                                        <asp:ListItem Enabled="true" Text="Active" Value="Active"></asp:ListItem>
                                        <asp:ListItem Enabled="true" Text="Inactive" Value="Inactive"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <%-- Position --%>
                        <div class="row mb-2">
                            <label id="lblEditPosition" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Position</label>
                            <div class="col-sm-10">
                                <asp:TextBox ID="txtboxEditPosition" runat="server" class="form-control form-control-sm"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm-13">
                            <div class="alert alert-danger d-none mb-2" id="errorAlert_EditModal" role="alert">
                            </div>
                        </div>
                    </div>
                    <div class="d-flex flex-row justify-content-end mt-3">
                        <asp:Button ID="btnEditUpdate" runat="server" class="btn btn-sm submit" Text="Update" OnClick="btnSaveChanges_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%-- EDIT GRANT ACCESS MODAL --%>
    <div class="modal fade" id="grantAccessModal" tabindex="-1" aria-labelledby="grantAccessModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-body">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <asp:HiddenField ID="hiddenUserID" runat="server" />
                    <asp:HiddenField ID="hiddenUserName" runat="server" />
                    <div class="access-info">
                        <h5>Edit Access for
                        <asp:Label ID="lblName" runat="server"></asp:Label></h5>

                        <div class="d-flex flex-row justify-content-between">
                            <div class="rights mr-3">
                                <p>Basic Access</p>
                                <asp:CheckBoxList runat="server" ID="CheckBoxListEditBasic" CssClass="check"></asp:CheckBoxList>
                            </div>
                            <div class="rights mr-3">
                                <p>Advanced Access</p>
                                <asp:CheckBoxList runat="server" ID="CheckBoxListEditAdvanced" CssClass="check"></asp:CheckBoxList>
                            </div>
                            <div class="rights">
                                <p>Master Access</p>
                                <asp:CheckBoxList runat="server" ID="CheckBoxListEditMaster" CssClass="check"></asp:CheckBoxList>
                            </div>
                        </div>
                        <div class="mt-3">
                            <input type="checkbox" id="editSelectAll" onchange="handleEditSelectAllChange(this)">
                            <label for="editSelectAll">Select all</label>
                            <input type="checkbox" id="editUnselectAll" onchange="handleEditUnselectAllChange(this)">
                            <label for="editUnselectAll">Unselect all</label>
                        </div>
                        <div class="col-sm-13">
                            <div class="alert alert-success d-none mb-2" id="successAlert_GrantAccess" role="alert">
                            </div>
                        </div>
                        <div class="col-sm-13">
                            <div class="alert alert-danger d-none mb-2" id="errorAlert_GrantAccess" role="alert">
                            </div>
                        </div>
                        <asp:Button ID="submitEditAccess" runat="server" Text="Save" class="btn btn-sm submit" OnClick="submitEditAccess_Click" />
                    </div>

                </div>
            </div>
        </div>
    </div>

    <script>
        // SELECT OR UNSELECT (ADD MODAL)
        function handleSelectAllChange(selectAllCheckbox) {
            if (selectAllCheckbox.checked) {
                // Check all checkboxes in Advanced and Master access levels
                checkAllCheckBoxes(true);
                // Uncheck the Unselect all checkbox
                document.getElementById("unselectAll").checked = false;
            }
            else {
                // Uncheck all checkboxes in Advanced and Master access levels
                checkAllCheckBoxes(false);
            }
        }

        function handleUnselectAllChange(unselectAllCheckbox) {
            if (unselectAllCheckbox.checked) {
                // Uncheck all checkboxes in Advanced and Master access levels
                checkAllCheckBoxes(false);
                // Uncheck the Select all checkbox
                document.getElementById("selectAll").checked = false;
            }
            else {
                // Check all checkboxes in Basic access level
                checkBasicCheckBoxes();
            }
        }

        function checkAllCheckBoxes(checked) {
            var advancedCheckBoxes = document.querySelectorAll("#<%= CheckBoxListAdvanced.ClientID %> input[type=checkbox]");
            var masterCheckBoxes = document.querySelectorAll("#<%= CheckBoxListMaster.ClientID %> input[type=checkbox]");
            var basicCheckBoxes = document.querySelectorAll("#<%= CheckBoxListBasic.ClientID %> input[type=checkbox]");
            basicCheckBoxes.forEach(function (checkbox) {
                checkbox.checked = true;
            });
            advancedCheckBoxes.forEach(function (checkbox) {
                checkbox.checked = checked;
            });
            masterCheckBoxes.forEach(function (checkbox) {
                checkbox.checked = checked;
            });
        }

        function checkBasicCheckBoxes() {
            var basicCheckBoxes = document.querySelectorAll("#<%= CheckBoxListBasic.ClientID %> input[type=checkbox]");
            basicCheckBoxes.forEach(function (checkbox) {
                checkbox.checked = true;
            });
        }
        // SELECT OR UNSELECT (EDIT ACCESS MODAL)
        function handleEditSelectAllChange(editSelectAllCheckbox) {
            if (editSelectAllCheckbox.checked) {
                // Check all checkboxes in Edit Basic, Edit Advanced, and Edit Master access levels
                checkAllEditCheckBoxes(true);
                // Uncheck the Edit Unselect all checkbox
                document.getElementById("editUnselectAll").checked = false;
            }
            else {
                // Uncheck all checkboxes in Edit Advanced and Edit Master access levels
                checkAllEditCheckBoxes(false);
            }
        }

        function handleEditUnselectAllChange(editUnselectAllCheckbox) {
            if (editUnselectAllCheckbox.checked) {
                // Uncheck all checkboxes in Edit Basic, Edit Advanced, and Edit Master access levels
                checkAllEditCheckBoxes(false);
                // Uncheck the Edit Select all checkbox
                document.getElementById("editSelectAll").checked = false;
            }
            else {
                // Uncheck all checkboxes in Edit Advanced and Edit Master access levels
                checkAllEditCheckBoxes(false);
            }
        }
        function checkAllEditCheckBoxes(checked) {
            var editBasicCheckBoxes = document.querySelectorAll("#<%= CheckBoxListEditBasic.ClientID %> input[type=checkbox]");
            var editAdvancedCheckBoxes = document.querySelectorAll("#<%= CheckBoxListEditAdvanced.ClientID %> input[type=checkbox]");
            var editMasterCheckBoxes = document.querySelectorAll("#<%= CheckBoxListEditMaster.ClientID %> input[type=checkbox]");

            editBasicCheckBoxes.forEach(function (checkbox) {
                checkbox.checked = checked;
            });

            editAdvancedCheckBoxes.forEach(function (checkbox) {
                checkbox.checked = checked;
            });

            editMasterCheckBoxes.forEach(function (checkbox) {
                checkbox.checked = checked;
            });
        }
        /*ALERT*/
        function showErrorEditAlert(message) {
            $('#errorAlert_EditAccount').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_EditAccount').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }
        function showSuccessEditAlert(message) {
            $('#successAlert_EditAccount').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert_EditAccount').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }

        /*EDIT MODAL ALERT*/
        function showErrorAlertModal(message) {
            $('#errorAlert_EditModal').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_EditModal').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds

            $('#editUserModal').modal('show');
        }

        /*ADD USER MODAL ALERT*/
        function showErrorAlertAddUserModal(message) {
            $('#errorAlert_AddUserModal').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_AddUserModal').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds

            $('#addUserModal').modal('show');
        }
        function showSuccessAlertAddUserModal(message) {
            $('#successAlert_AddUserModal').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert_AddUserModal').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }

        function changePage(pageIndex) {
            // Ensure page index is within valid range
            var pageCount = <%= GridView1.PageCount %>;
            if (pageIndex >= 0 && pageIndex < pageCount) {
                __doPostBack('<%= GridView1.ClientID %>', 'Page$' + pageIndex);
            }
        }
        /*GRANT ACCESS MODAL*/
        function showEditAccessModal() {
            $('#grantAccessModal').modal('show');
        }

        function closeEditAccessModal() {
            $('#grantAccessModal').modal('hide');
        }
        /*ALERT*/
        function showErrorAlert(message) {
            $('#errorAlert_GrantAccess').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_GrantAccess').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds

            $('#grantAccessModal').modal('show');
        }
        function showSuccessAlert(message) {
            $('#successAlert_GrantAccess').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert_GrantAccess').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds

            $('#grantAccessModal').modal('show');
        }


        /*ADD MODAL*/
        $(document).ready(function () {
            // When any button with the class "filter-btn" is clicked
            $('.add-btn').click(function (e) {
                e.preventDefault();
                $('#addUserModal').modal('show');
            });
        });
        $(document).ready(function () {
            // When any button with the class "filter-btn" is clicked
            $('.close').click(function (e) {
                e.preventDefault();
                $('#addUserModal').modal('hide');
            });
        });

        /*EDIT MODAL*/
        $(document).ready(function () {
            $('.edit-btn').click(function (e) {
                e.preventDefault();
                var userID = $(this).data('edituserid');
                var editName = $(this).closest('tr').find('td:eq(0)').text(); // Get the value from the first column (Name)
                var editEmail = $(this).closest('tr').find('td:eq(3)').text(); // Get the value from the fourth column (Email)
                var editContact = $(this).closest('tr').find('td:eq(4)').text(); // Get the value from the fifth column (Contact)
                var editDept = $(this).closest('tr').find('td:eq(2)').text(); // Get the value from the third column (Department)          
                var editStatus = $(this).closest('tr').find('td:eq(5)').text(); // Get the value from the sixth column (Status)
                var editPosition = $(this).closest('tr').find('td:eq(6)').text(); // Get the value from the seventh column (Position)

                // Set the values to the corresponding textboxes in the edit modal
                $('#<%= getUserID.ClientID %>').val(userID);
                $('#<%= txtboxEditName.ClientID %>').val(editName);
                $('#<%= txtboxEditEmail.ClientID %>').val(editEmail);
                $('#<%= txtboxEditContact.ClientID %>').val(editContact);
                $('#<%= ddlEditDept.ClientID %>').val(editDept);
                $('#<%= ddlEditStatus.ClientID %>').val(editStatus);
                $('#<%= txtboxEditPosition.ClientID %>').val(editPosition);

                $('#editUserModal').modal('show');
            });

            $('.close').click(function (e) {
                e.preventDefault();
                $('#editUserModal').modal('hide');
            });
        });


        function hideTextboxesAndLabels() {
            // Get the modal element
            var modal = document.getElementById('addUserModal');

            var checkboxList = modal.querySelector('.checkbox');
            checkboxList.classList.remove('d-none');

            var userInfo = modal.querySelector('.user-info');
            userInfo.classList.add('d-none');

            var backbtn = modal.querySelector('#<%= backBtn.ClientID %>');
            backbtn.style.display = 'block'

            var submitbtn = modal.querySelector('#<%= submitBtn.ClientID %>');
            submitbtn.style.display = 'block'

            var nextbtn = modal.querySelector('#<%= nextBtn.ClientID %>');
            nextbtn.style.display = 'none'

        }
        function showTextboxesAndLabels() {
            // Get the modal element
            var modal = document.getElementById('addUserModal');

            var checkboxList = modal.querySelector('.checkbox');
            checkboxList.classList.add('d-none');

            var userInfo = modal.querySelector('.user-info');
            userInfo.classList.remove('d-none');

            var backbtn = modal.querySelector('#<%= backBtn.ClientID %>');
            backbtn.style.display = 'none'

            var submitbtn = modal.querySelector('#<%= submitBtn.ClientID %>');
            submitbtn.style.display = 'none'

            var nextbtn = modal.querySelector('#<%= nextBtn.ClientID %>');
            nextbtn.style.display = 'block'
        }
    </script>
</asp:Content>
