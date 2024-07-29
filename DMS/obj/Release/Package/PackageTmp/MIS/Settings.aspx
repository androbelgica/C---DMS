<%@ Page Title="" Language="C#" MasterPageFile="~/PAGES/topnav.Master" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="DMS.MIS.Settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="../CSS/settings.css">
    <link href='https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css' rel='stylesheet'>
    <link href="https://cdn.lineicons.com/4.0/lineicons.css" rel="stylesheet">

    <!-- jQuery and Bootstrap JS -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.16.0/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <!-- Page Heading -->
        <div class="d-sm-flex align-items-center justify-content-between mb-4">
            <h1 class="h1 mb-0 text-gray-800">Settings</h1>
        </div>
        <div class="card">
            <div class="card-header">
                <ul class="nav nav-tabs card-header-tabs" id="myTab" role="tablist">
                    <li class="nav-item" role="presentation">
                        <a class="nav-link active" id="profile-tab" data-bs-toggle="tab" href="#profile" role="tab" aria-controls="profile" aria-selected="true">Profile</a>
                    </li>

                    <% if (Session["Access"]?.ToString() != "Basic")
                        { %>
                    <li class="nav-item" role="presentation">
                        <a class="nav-link" id="manage-user-tab" data-bs-toggle="tab" href="#user" role="tab" aria-controls="user" aria-selected="false">Accounts</a>
                    </li>
                    <li class="nav-item" role="presentation">
                        <a class="nav-link" id="manage-dept-tab" data-bs-toggle="tab" href="#dept" role="tab" aria-controls="dept" aria-selected="false">Department</a>
                    </li>
                    <% } %>
                </ul>
            </div>
            <div class="card-body">
                <div class="tab-content" id="myTabContent">
                    <!-- Manage PROFILE -->
                    <div class="tab-pane fade show active" id="profile" role="tabpanel" aria-labelledby="profile-tab">
                        <div class="row">
                            <!-- User Information -->
                            <div class="col-xl-6 col-md-6 mb-4">
                                <div class="card h-100">
                                    <div class="card-header">
                                        User Information
                                    </div>
                                    <div class="card-body">
                                        <!-- Name -->
                                        <div class="mb-3">
                                            <asp:Label runat="server" ID="name" class="col-sm-2 col-form-label txtbox-label" Visible="true">Name</asp:Label>
                                            <div class="col-sm-10">
                                                <asp:TextBox ID="nameTxtbox" runat="server" class="form-control form-control-sm" ReadOnly="true" Visible="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Username -->
                                        <div class="mb-3">
                                            <asp:Label runat="server" ID="username" class="col-sm-2 col-form-label" Visible="true">Username</asp:Label>
                                            <div class="col-sm-10">
                                                <asp:TextBox ID="usernameTxtbox" runat="server" class="form-control form-control-sm" ReadOnly="true" Visible="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Contact -->
                                        <div class="mb-3">
                                            <asp:Label runat="server" ID="contact" class="col-sm-2 col-form-label" Visible="true">Contact</asp:Label>
                                            <div class="col-sm-10">
                                                <asp:TextBox ID="contactTxtbox" runat="server" class="form-control form-control-sm" ReadOnly="true" Visible="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Email -->
                                        <div>
                                            <asp:Label runat="server" ID="email" class="col-sm-2 col-form-label" Visible="true">Email</asp:Label>
                                            <div class="col-sm-10">
                                                <asp:TextBox ID="emailTxtbox" runat="server" class="form-control form-control-sm" ReadOnly="true" Visible="true"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Change Password -->
                            <div class="col-xl-6 col-md-6 mb-4">
                                <div class="card h-100">
                                    <div class="card-header">
                                        Change Password
                                    </div>
                                    <div class="card-body">
                                        <!-- Enter Current PW -->
                                        <div class="mb-3">
                                            <asp:Label runat="server" ID="currentpword" class="col-sm-2 col-form-label">Current Password</asp:Label>
                                            <div class="col-sm-10">
                                                <asp:TextBox ID="currentpwordTxtbox" runat="server" class="form-control form-control-sm" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Enter New PW -->
                                        <div class="mb-3">
                                            <asp:Label runat="server" ID="newpword" class="col-sm-2 col-form-label">New Password</asp:Label>
                                            <div class="col-sm-10">
                                                <asp:TextBox ID="newpwordTxtbox" runat="server" class="form-control form-control-sm" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <!-- Re-enter New PW -->
                                        <div class="mb-3">
                                            <asp:Label runat="server" ID="confirmpword" class="col-sm-2 col-form-label">Re-enter New Password</asp:Label>
                                            <div class="col-sm-10">
                                                <asp:TextBox ID="confirmpwordTxtbox" runat="server" class="form-control form-control-sm" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-sm-10">
                                            <div class="alert alert-success d-none mb-2" id="successAlert_ChangePW" role="alert">
                                            </div>
                                        </div>
                                        <div class="col-sm-10">
                                            <div class="alert alert-danger d-none mb-2" id="errorAlert_ChangePW" role="alert">
                                            </div>
                                        </div>
                                        <div class="d-flex justify-content-end mt-3">
                                            <asp:Button ID="ChangePwBtn" runat="server" EnableViewState="true" Text="Change Password" class="btn btn-sm edit-btn" OnClick="ChangePwBtn_Click" Visible="true"></asp:Button>
                                        </div>
                                        <div class="d-flex justify-content-end">
                                            <asp:Button ID="CancelBtn" EnableViewState="true" runat="server" Text="Cancel" class="btn btn-sm cancel-btn" OnClick="CancelBtn_Click" Visible="false"></asp:Button>
                                            <asp:Button ID="SubmitBtn" EnableViewState="true" runat="server" Text="Submit" class="btn btn-sm submit-btn" OnClick="SubmitBtn_Click" Visible="false"></asp:Button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <% if (Session["Access"]?.ToString() != "Basic")
                        { %>
                    <!-- Manage ACCOUNTS-->
                    <div class="tab-pane fade" id="user" role="tabpanel" aria-labelledby="manage-user-tab">
                        <div class="search-and-filter d-flex justify-content-between align-items-center">
                            <div class="d-flex align-items-center flex-grow-1">
                                <div class="input-group input-group-sm search-bar flex-grow-1">
                                    <div class="input-group flex-grow-1">
                                        <asp:TextBox ID="searchtxtbox" runat="server" class="form-control form-control-sm search-textbox" EnableViewState="true"
                                            placeholder="Search" aria-label="search" aria-describedby="search"
                                            OnTextChanged="searchtxtbox_TextChanged">
                                        </asp:TextBox>
                                        <asp:LinkButton ID="searchBtn" runat="server" CssClass="btn btn-sm icon-search" OnClick="searchBtn_Click">
                            <i class='bx bx-search-alt'></i>
                                        </asp:LinkButton>
                                    </div>
                                </div>

                                <div class="btn-container ml-2">
                                    <button id="FilterBtn" class="btn btn-sm filter-btn" data-toggle="modal" data-target="#filterModal">
                                        <i class='bx bx-filter'></i><span>Filter</span>
                                    </button>
                                </div>
                            </div>

                            <div class="btn-container">
                                <button id="AddUserBtn" class="btn btn-sm add-user-btn" data-toggle="modal" data-target="#addUserModal">
                                    <i class='bx bx-user-plus'></i><span>Add User</span>
                                </button>
                            </div>
                        </div>


                        <br />
                        <div class="row">
                            <div class="mb-3">
                                <div class="alert alert-danger d-none mb-2" id="errorAlert_Accounts" role="alert">
                                </div>
                                <div class="alert alert-success d-none mb-2" id="successAlert_Accounts" role="alert">
                                </div>
                            </div>
                            <!-- Table Card for Accounts -->
                            <div class="table-responsive">
                                <asp:GridView ID="GridView1" runat="server" EnableViewState="true" AutoGenerateColumns="False"
                                    OnPageIndexChanging="GridView1_PageIndexChanging" class="custom-gridview" PageSize="7"
                                    AllowPaging="true" OnRowDataBound="GridView1_RowDataBound"
                                    OnRowEditing="GridView1_RowEditing"
                                    OnRowUpdating="GridView1_RowUpdating"
                                    OnRowCancelingEdit="GridView1_RowCancelingEdit" DataKeyNames="UserID">
                                    <Columns>
                                        <%-- Name --%>
                                        <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtName" runat="server" Text='<%# Bind("Name") %>' Width="120px" class="form-control form-control-sm"></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <%-- Username --%>
                                        <asp:TemplateField HeaderText="Username" SortExpression="Username">
                                            <ItemTemplate>
                                                <asp:Label ID="lblUsername" runat="server" Text='<%# Eval("Username") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtUsername" runat="server" Text='<%# Bind("Username") %>' Width="100px" class="form-control form-control-sm"></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <%-- Department --%>
                                        <asp:TemplateField HeaderText="Department" SortExpression="Department">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDepartment" runat="server" Text='<%# Eval("Department") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtDepartment" runat="server" Text='<%# Bind("Department") %>' Width="100px" class="form-control form-control-sm"></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <%-- Email --%>
                                        <asp:TemplateField HeaderText="Email" SortExpression="Email">
                                            <ItemTemplate>
                                                <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("Email") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtEmail" runat="server" Text='<%# Bind("Email") %>' Width="200px" class="form-control form-control-sm"></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <%-- Contact --%>
                                        <asp:TemplateField HeaderText="Contact" SortExpression="Contact">
                                            <ItemTemplate>
                                                <asp:Label ID="lblContact" runat="server" Text='<%# Eval("Contact") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtContact" runat="server" Text='<%# Bind("Contact") %>' Width="110px" class="form-control form-control-sm"></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <%-- Status --%>
                                        <asp:TemplateField HeaderText="Status" SortExpression="Status">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlAccStatus" runat="server" class="btn btn-primary btn-sm dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                                                    <asp:ListItem Text="Active" Value="Active"></asp:ListItem>
                                                    <asp:ListItem Text="Inactive" Value="Inactive"></asp:ListItem>
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <%-- Position --%>
                                        <asp:TemplateField HeaderText="Position" SortExpression="Position">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPosition" runat="server" Text='<%# Eval("Position") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtPosition" runat="server" Text='<%# Bind("Position") %>' Width="100px" class="form-control form-control-sm"></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <%-- Access --%>
                                        <asp:TemplateField HeaderText="Access" SortExpression="Access">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAccess" runat="server" Text='<%# Eval("Access") %>'></asp:Label>
                                            </ItemTemplate>
                                                <EditItemTemplate>
                                                <asp:DropDownList ID="ddlAccAccess" runat="server" class="btn btn-primary btn-sm dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                                                    <asp:ListItem Text="Basic" Value="Basic"></asp:ListItem>
                                                    <asp:ListItem Text="Advanced" Value="Advanced"></asp:ListItem>
                                                    <asp:ListItem Text="Master" Value="Master"></asp:ListItem>
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <%-- Actions --%>
                                        <asp:TemplateField HeaderText="Actions">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" class="edit-icon">
                                    <i class='bx bx-edit-alt' ></i>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" Text="Update" CssClass="badge rounded-pill update-linkbtn" />
                                                <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" Text="Cancel" CssClass="badge rounded-pill cancel-linkbtn" />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <%-- No Results --%>
                                    <EmptyDataTemplate>
                                        <div class="alert alert-info" role="alert">
                                            No results found.
                                        </div>
                                    </EmptyDataTemplate>
                                    <RowStyle HorizontalAlign="Center" />
                                    <PagerSettings Mode="NumericFirstLast"
                                        FirstPageText="&le;"
                                        LastPageText="&ge;"
                                        PageButtonCount="10"
                                        Position="Bottom" />
                                    <PagerStyle CssClass="pagination justify-content-start font-weight-bold" />
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                    <!-- Manage DEPT -->
                    <div class="tab-pane fade" id="dept" role="tabpanel" aria-labelledby="manage-dept-tab">
                        <div class="btn-container">
                            <button id="deptAddBtn" class="btn btn-sm dept-acc-btn" data-toggle="modal" data-target="#addDeptModal">
                                <i class='bx bx-plus-circle'></i><span>Add Department</span>
                            </button>

                            <button id="deptFilterBtn" class="btn btn-sm dept-filter-btn" data-toggle="modal" data-target="#filterModal2">
                                <i class='bx bx-filter'></i><span>Filter</span>
                            </button>
                        </div>
                        <br />

                        <!-- Alert messages for department updates -->
                        <div class="alert alert-danger d-none mb-2" id="errorAlert_Department" role="alert"></div>
                        <div class="alert alert-success d-none mb-2" id="successAlert_Department" role="alert"></div>

                        <div class="row">
                            <div class="mb-3">
                                <!-- Other content, if any -->
                            </div>
                            <!-- Table Card for Department -->
                            <div class="table-responsive">
                                <asp:GridView ID="GridView2" runat="server" EnableViewState="true" AutoGenerateColumns="False"
                                    OnPageIndexChanging="GridView2_PageIndexChanging" class="custom-gridview" PageSize="7"
                                    AllowPaging="true" OnRowEditing="GridView2_RowEditing"
                                    OnRowUpdating="GridView2_RowUpdating" OnRowDataBound="GridView2_RowDataBound"
                                    OnRowCancelingEdit="GridView2_RowCancelingEdit" DataKeyNames="DepartmentID">
                                    <Columns>
                                        <%-- Dept Name --%>
                                        <asp:TemplateField HeaderText="Department Name" SortExpression="DepartmentName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDepartmentName" runat="server" Text='<%# Eval("DepartmentName") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtDepartmentName" runat="server" Text='<%# Bind("DepartmentName") %>' Width="200px" class="form-control form-control-sm"></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <%-- Short Acro --%>
                                        <asp:TemplateField HeaderText="Short Acronym" SortExpression="ShortAcronym">
                                            <ItemTemplate>
                                                <asp:Label ID="lblShortAcronym" runat="server" Text='<%# Eval("ShortAcronym") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtShortAcronym" runat="server" Text='<%# Bind("ShortAcronym") %>' Width="150px" class="form-control form-control-sm"></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <%-- Status --%>
                                        <asp:TemplateField HeaderText="Status" SortExpression="Status">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlDeptStatus" runat="server" class="btn btn-primary btn-sm dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                                                    <asp:ListItem Text="Active" Value="Active"></asp:ListItem>
                                                    <asp:ListItem Text="Inactive" Value="Inactive"></asp:ListItem>
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <%-- Actions --%>
                                        <asp:TemplateField HeaderText="Actions">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" class="edit-icon">
                                                    <i class='bx bx-edit-alt'></i>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" Text="Update" CssClass="badge rounded-pill update-linkbtn" />
                                                <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" Text="Cancel" CssClass="badge rounded-pill cancel-linkbtn" />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <%-- No Results --%>
                                    <EmptyDataTemplate>
                                        <div class="alert alert-info" id="infoAlert_Department" role="alert">
                                            No results found.
                                        </div>
                                    </EmptyDataTemplate>
                                    <RowStyle HorizontalAlign="Center" />
                                    <PagerSettings Mode="NumericFirstLast"
                                        FirstPageText="&le;"
                                        LastPageText="&ge;"
                                        PageButtonCount="10"
                                        Position="Bottom" />
                                    <PagerStyle CssClass="pagination justify-content-start font-weight-bold" />
                                </asp:GridView>
                            </div>


                        </div>
                    </div>




                    <% } %>
                </div>
            </div>
        </div>
    </div>


    <!--START MODALS-->

    <!-- Add User Modal Structure for Account -->
    <div class="modal fade" id="addUserModal" tabindex="-1" aria-labelledby="addUserModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="addUserModalLabel">Add User</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                        <!-- Name -->
                        <div class="form-group">
                            <asp:Label ID="lblAddName" runat="server" for="username" Text="Name" class="col-form-label col-form-label-sm"></asp:Label>
                            <asp:TextBox ID="txtboxAddName" runat="server" class="form-control form-control-sm"></asp:TextBox>
                        </div>
                        <!-- Username -->
                        <div class="form-group">
                            <asp:Label ID="lblAddUsername" runat="server" for="username" Text="Username" class="col-form-label col-form-label-sm"></asp:Label>
                            <asp:TextBox ID="txtboxAddUsername" runat="server" class="form-control form-control-sm"></asp:TextBox>
                        </div>
                        <!-- Department -->
                        <div class="form-group">
                            <asp:Label ID="lblAddDept" runat="server" Text="Department" class="col-form-label col-form-label-sm"></asp:Label>
                            <div class="dropdown">
                                <asp:DropDownList ID="ddlAddDept" runat="server" CssClass="btn btn-sm dropdown-toggle setting-drop" data-bs-toggle="dropdown" aria-expanded="false">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <!-- Email -->
                        <div class="form-group">
                            <asp:Label ID="lblAddEmail" runat="server" for="email" Text="Email" class="col-form-label col-form-label-sm"></asp:Label>
                            <asp:TextBox ID="txtboxAddEmail" runat="server" class="form-control form-control-sm"></asp:TextBox>
                        </div>
                        <!-- Contact -->
                        <div class="form-group">
                            <asp:Label ID="lblAddContact" runat="server" for="contact" Text="Contact" class="col-form-label col-form-label-sm"></asp:Label>
                            <asp:TextBox ID="txtboxAddContact" runat="server" class="form-control form-control-sm"></asp:TextBox>
                        </div>
                        <!-- Position -->
                        <div class="form-group">
                            <asp:Label ID="lblAddPosition" runat="server" for="position" Text="Position" class="col-form-label col-form-label-sm"></asp:Label>
                            <asp:TextBox ID="txtboxAddPosition" runat="server" class="form-control form-control-sm"></asp:TextBox>
                        </div>
                        <!-- Access -->
                        <div class="form-group">
                            <asp:Label ID="lblAddAccess" runat="server" for="access" Text="Access" class="col-form-label col-form-label-sm"></asp:Label>
                            <div class="dropdown">
                                <asp:DropDownList ID="ddlAddAccess" runat="server" CssClass="btn btn-sm dropdown-toggle setting-drop" data-bs-toggle="dropdown" aria-expanded="false">
                                    <asp:ListItem Value="" Text="Select Access"></asp:ListItem>
                                    <asp:ListItem Value="Master" Text="Master"></asp:ListItem>
                                    <asp:ListItem Value="Advanced" Text="Advanced"></asp:ListItem>
                                    <asp:ListItem Value="Basic" Text="Basic"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="addUserbtn" runat="server" Text="Add User" class="btn modal-add-user-btn" Onclick="addUserbtn_Click"/>

                </div>
            </div>
        </div>
    </div>


    <!-- Filter Modal for Users/Accounts -->
    <div class="modal fade" id="filterModal" tabindex="-1" role="dialog" aria-labelledby="filterModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" id="filterModalModalLabel">Filter</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" id="close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="row mb-3">
                        <%-- Dropdown Department --%>
                        <asp:Label ID="deptDropDown" runat="server" Text="Department"></asp:Label>
                        <div class="dropdown">
                            <asp:DropDownList ID="deptDropDownList" runat="server" CssClass="btn dropdown-toggle setting-drop" data-bs-toggle="dropdown" aria-expanded="false">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <%-- Dropdown Status --%>
                        <asp:Label ID="statusDropDown" runat="server" Text="Status"></asp:Label>
                        <div class="dropdown">
                            <asp:DropDownList ID="statusDropDownList" runat="server" CssClass="btn dropdown-toggle setting-drop" data-bs-toggle="dropdown" aria-expanded="false">
                                <asp:ListItem Value="">Select Status</asp:ListItem>
                                <asp:ListItem Value="Active">Active</asp:ListItem>
                                <asp:ListItem Value="Inactive">Inactive</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <asp:Button ID="resetFilter" runat="server" class="btn resetBtn" Text="Reset Filter" OnClick="resetfilterBtn_Click"/>
                    <asp:Button ID="addFilter" runat="server" class="btn addBtn" Text="Add Filter" OnClick="filterBtn_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- Add Department Modal for Departments -->
    <div class="modal fade" id="addDeptModal" tabindex="-1" role="dialog" aria-labelledby="addDeptModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" id="addDeptModalLabel">Add Department</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                        <!-- Dept Name -->
                        <div class="form-group">
                            <asp:Label ID="lblDeptName" runat="server" for="username" Text="Department Name" class="col-form-label col-form-label-sm"></asp:Label>
                            <asp:TextBox ID="deptNametxtbox" runat="server" class="form-control form-control-sm"></asp:TextBox>
                        </div>
                        <!-- Short Acronym -->
                        <div class="form-group">
                            <asp:Label ID="lblShortAcro" runat="server" for="username" Text="Short Acronym" class="col-form-label col-form-label-sm"></asp:Label>
                            <asp:TextBox ID="deptShortAcrotxtbox" runat="server" class="form-control form-control-sm"></asp:TextBox>
                        </div>

                        <div class="col-sm-10">
                            <div class="alert alert-success d-none mb-2" id="successAlert_AddDept"  role="alert">
                            </div>
                        </div>
                        <div class="col-sm-10">
                            <div class="alert alert-danger d-none mb-2" id="errorAlert_AddDept" role="alert">
                            </div>
                        </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="addDeptbtn" runat="server" Text="Add Department" class="btn modal-add-user-btn" OnClick="addDeptbtn_Click"/>
                </div>
            </div>
        </div>
    </div>

    <!-- Filter Modal for Departments -->
    <div class="modal fade" id="filterModal2" tabindex="-1" role="dialog" aria-labelledby="filterModal2Label" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" id="filterModal2ModalLabel">Filter</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="row mb-3">
                        <%-- Dropdown Status --%>
                        <asp:Label ID="deptStatus" runat="server" Text="Status"></asp:Label>
                        <div class="dropdown">
                            <asp:DropDownList ID="deptStatusDropDownList" runat="server" CssClass="btn dropdown-toggle setting-drop" data-bs-toggle="dropdown" aria-expanded="false">
                                <asp:ListItem Value="">Select Status</asp:ListItem>
                                <asp:ListItem Value="Active">Active</asp:ListItem>
                                <asp:ListItem Value="Inactive">Inactive</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <asp:Button ID="deptResetFilterbtn" runat="server" class="btn resetBtn" Text="Reset Filter" Onclick="deptResetfilterBtn_Click"/>
                    <asp:Button ID="deptAddFilterbtn" runat="server" class="btn addBtn" Text="Add Filter" OnClick="deptfilterBtn_Click" />
                </div>
            </div>
        </div>
    </div>




    <!--END MODALs-->




    <script>
        /*ALERTS*/

        /*For Profile*/
        function showErrorAlert(message) {
            $('#errorAlert_ChangePW').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_ChangePW').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }
        function showSuccessAlert(message) {
            $('#successAlert_ChangePW').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert_ChangePW').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 2 seconds
        }

        /*For Accounts*/
        function showErrorAlertAcc(message) {
            $('#errorAlert_Accounts').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_Accounts').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }
        function showSuccessAlertAcc(message) {
            $('#successAlert_Accounts').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert_Accounts').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 2 seconds
        }

        /*For Department*/
        function showErrorAlertDept(message) {
            $('#errorAlert_Department').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_Department').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }
        function showSuccessAlertDept(message) {
            $('#successAlert_Department').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert_Department').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 2 seconds
        }

        /*For Department - Add*/
        function showErrorAlertAddDept(message) {
            $('#errorAlert_AddDept').text(message).removeClass('d-none').addClass('show');
            // Do not hide the modal after showing the error alert
        }
        function showSuccessAddAlertDept(message) {
            $('#successAlert_AddDept').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert_AddDept').removeClass('show').addClass('d-none');
                $('#addDeptModal').modal('hide'); // Hide the modal after success
            }, 3000); // Hide alert after 2 seconds
        }
        
        /*TABS*/
        $(document).ready(function () {
            // Ensure the tab is shown based on ViewState
            var activeTab = '<%= ViewState["ActiveTab"] %>';
            if (activeTab) {
                $('#myTab a[href="#' + activeTab + '"]').tab('show');
            }
        });

        /*MODALS*/
        /*for accounts filter*/
        $(document).ready(function () {
            // When any button with the class "filter-btn" is clicked
            $('.filter-btn').click(function (e) {
                e.preventDefault();
                $('#filterModal').modal('show');
            });
        });
        $(document).ready(function () {
            // When any button with the class "filter-btn" is clicked
            $('.close').click(function (e) {
                e.preventDefault();
                $('#filterModal').modal('hide');
            });
        });

        /*for department filter*/
        $(document).ready(function () {
            // When any button with the class "filter-btn" is clicked
            $('.dept-filter-btn').click(function (e) {
                e.preventDefault();
                $('#filterModal2').modal('show');
            });
        });
        $(document).ready(function () {
            // When any button with the class "filter-btn" is clicked
            $('.close').click(function (e) {
                e.preventDefault();
                $('#filterModal2').modal('hide');
            });
        });


        /*For departments add dept*/
        $(document).ready(function () {
            // When any button with the class "dept-acc-btn" is clicked
            $('.dept-acc-btn').click(function (e) {
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

        /*for accounts add user*/
        $(document).ready(function () {
            // When any button with the class "filter-btn" is clicked
            $('.add-user-btn').click(function (e) {
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

        

       
    </script>
</asp:Content>

