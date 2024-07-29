<%@ Page Title="" Language="C#" MasterPageFile="~/PAGES/topnav.Master" AutoEventWireup="true" CodeBehind="DepartmentSettings.aspx.cs" Inherits="DMS.PAGES.DepartmentSettings" %>

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
            <div class="page-heading">Departments</div>
            <div class="btn-container">
                <asp:Button ID="addUserBtn" class="btn add-btn" runat="server" Text="+ Add Department" data-toggle="modal" data-target="#addUserModal" />
            </div>
        </div>

        <div class="custom-gridview-container shadow-sm">
            <div class="filterz">
                <div class="search-bar">
                    <asp:TextBox ID="searchtxtbox" runat="server" class="form-control form-control-sm dept search-textbox" placeholder="Search"
                        aria-label="search" aria-describedby="search" AutoPostBack="true" OnTextChanged="searchtxtbox_TextChanged"></asp:TextBox>
                </div>
                <div class="table-filters">
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
                            <asp:Button ID="resetFilter" runat="server" Text="Reset Filters" CssClass="btn reset-btn" OnClick="resetDeptfilterBtn_Click" />
                        </div>
                    </div>
                </div>
            </div>
            <!-- Table Card for Accounts -->
            <div class="table-responsive">
                <div class="col-sm-13">
                    <div class="alert alert-success d-none mb-2" id="successAlert_EditDept" role="alert">
                    </div>
                </div>
                <div class="col-sm-13">
                    <div class="alert alert-danger d-none mb-2" id="errorAlert_EditDept" role="alert">
                    </div>
                </div>
                <div class="col-sm-13">
                    <div class="alert alert-success d-none mb-2" id="successAlert_AddDeptModal" role="alert">
                    </div>
                </div>
                <asp:GridView ID="GridView2" runat="server" CssClass="custom-gridview" AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="20" DataKeyNames="DepartmentID">
                    <Columns>
                        <%-- Dept Name --%>
                        <asp:BoundField DataField="DepartmentName" HeaderText="Department Name" SortExpression="DepartmentName" />
                        <%-- Short Acro --%>
                        <asp:BoundField DataField="ShortAcronym" HeaderText="Short Acronym" SortExpression="ShortAcronym" />
                        <%-- Status --%>
                        <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <%-- Actions --%>
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" class="edit-btn" data-toggle="modal" data-target="#editDeptModal"
                                    data-editdeptid='<%# Eval("DepartmentID") %>'>
                                    <i class='bx bx-edit-alt'></i>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerSettings Visible="false" />
                    <%-- No Results --%>
                    <EmptyDataTemplate>
                        <div class="alert alert-info" id="infoAlert_Department" role="alert">
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
    <div class="modal fade" id="addDeptModal" tabindex="-1" aria-labelledby="addUserModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-body">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <div class="user-info">
                        <h5>Add Department</h5>
                        <%-- DEPARTMENT NAME --%>
                        <div class="row mb-2">
                            <label id="lblAddDeptName" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Department Name</label>
                            <div class="col-sm-10">
                                <asp:TextBox ID="txtboxAddDeptName" runat="server" class="form-control form-control-sm"></asp:TextBox>
                            </div>
                        </div>
                        <%-- SHORT ACRONYM --%>
                        <div class="row mb-2">
                            <label id="lblAddShortAcro" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Short Acronym</label>
                            <div class="col-sm-10">
                                <asp:TextBox ID="txtboxAddShortAcro" runat="server" class="form-control form-control-sm"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm-13">
                            <div class="alert alert-danger d-none mb-2" id="errorAlert_AddDeptModal" role="alert">
                            </div>
                        </div>

                        <div class="d-flex flex-row justify-content-end mt-3">
                            <asp:Button ID="btnAddSubmit" runat="server" class="btn btn-sm submit" Text="Submit" OnClick="btnAddSubmit_Click" />
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>

    <%-- EDIT MODAL --%>
    <div class="modal fade" id="editDeptModal" tabindex="-1" aria-labelledby="editDeptModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-body">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <asp:HiddenField ID="getDeptID" runat="server" />
                    <div class="user-info">
                        <h5>Edit Department Information</h5>
                        <%-- DEPARTMENT NAME --%>
                        <div class="row mb-3">
                            <label id="lblEditDeptName" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Department Name</label>
                            <div class="col-sm-10">
                                <asp:TextBox ID="txtboxEditDeptName" runat="server" class="form-control form-control-sm"></asp:TextBox>
                            </div>
                        </div>
                        <%-- SHORT ACRONYM --%>
                        <div class="row mb-3">
                            <label id="lblEditShortAcro" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Short Acronym</label>
                            <div class="col-sm-10">
                                <asp:TextBox ID="txtboxEditShortAcro" runat="server" class="form-control form-control-sm"></asp:TextBox>
                            </div>
                        </div>
                        <%-- STATUS --%>
                        <div class="row mb-3">
                            <label id="lblEditStatus" runat="server" class="col-sm-2 col-form-label col-form-label-sm">Status</label>
                            <div class="col-sm-10">
                                <asp:DropDownList ID="ddlEditStatus" runat="server" CssClass="btn btn-sm dropdown-toggle custom-dropdown">
                                    <asp:ListItem Text="Active" Value="Active"></asp:ListItem>
                                    <asp:ListItem Text="Inactive" Value="Inactive"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-sm-13">
                            <div class="alert alert-danger d-none mb-2" id="errorAlert_EditDeptModal" role="alert">
                            </div>
                        </div>

                        <div class="d-flex flex-row justify-content-end mt-3">
                            <asp:Button ID="btnEditSubmit" runat="server" class="btn btn-sm submit" Text="Submit" OnClick="btnEditSubmit_Click" />
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>



    <script>
        /*ADD DEPT MODAL ALERT*/
        function showErrorAlertAddDeptModal(message) {
            $('#errorAlert_AddDeptModal').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_AddDeptModal').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds

            $('#addDeptModal').modal('show');
        }
        function showSuccessAlertAddDeptModal(message) {
            $('#successAlert_AddDeptModal').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert_AddDeptModal').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }

        /*ALERT*/
        function showErrorAlert(message) {
            $('#errorAlert_EditDept').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_EditDept').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }
        function showSuccessAlert(message) {
            $('#successAlert_EditDept').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert_EditDept').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }

        /*EDIT MODAL ALERT*/
        function showErrorAlertDeptModal(message) {
            $('#errorAlert_EditDeptModal').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_EditDeptModal').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds

            $('#editDeptModal').modal('show');
        }

        function changePage(pageIndex) {
            // Ensure page index is within valid range
            var pageCount = <%= GridView2.PageCount %>;
            if (pageIndex >= 0 && pageIndex < pageCount) {
                __doPostBack('<%= GridView2.ClientID %>', 'Page$' + pageIndex);
            }
        }

        /*ADD MODAL*/
        $(document).ready(function () {
            // When any button with the class "filter-btn" is clicked
            $('.add-btn').click(function (e) {
                e.preventDefault();
                $('#addDeptModal').modal('show');
            });
        });
        $(document).ready(function () {
            // When any button with the class "filter-btn" is clicked
            $('.close').click(function (e) {
                e.preventDefault();
                $('#addDeptModal').modal('hide');
            });
        });

        /*EDIT MODAL*/
        $(document).ready(function () {
            $('.edit-btn').click(function (e) {
                e.preventDefault();

                var deptID = $(this).data('editdeptid');
                var editDeptName = $(this).closest('tr').find('td:eq(0)').text(); // Get the value from the first column (Dept Name)
                var editShortAcro = $(this).closest('tr').find('td:eq(1)').text(); // Get the value from the second column (Short Acronym)
                var editStatus = $(this).closest('tr').find('td:eq(2)').text(); // Get the value from the third column (Status)
                // Set values in the modal fields
                $('#<%= getDeptID.ClientID %>').val(deptID);
                $('#<%= txtboxEditDeptName.ClientID %>').val(editDeptName);
                $('#<%= txtboxEditShortAcro.ClientID %>').val(editShortAcro);
                $('#<%= ddlEditStatus.ClientID %>').val(editStatus);

                $('#editDeptModal').modal('show');
            });

            $('.close').click(function (e) {
                e.preventDefault();
                $('#editDeptModal').modal('hide');
            });
        });

        /*EDIT MODAL*/

    </script>
</asp:Content>
