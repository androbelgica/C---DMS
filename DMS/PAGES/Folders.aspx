<%@ Page Title="" Language="C#" MasterPageFile="~/PAGES/topnav.Master" AutoEventWireup="true" CodeBehind="Folders.aspx.cs" Inherits="DMS.PAGES.folders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="../CSS/folders.css" />
    <link href='https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css' rel='stylesheet'>
    <link href="https://cdn.lineicons.com/4.0/lineicons.css" rel="stylesheet" />
    <script rel="stylesheet" src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script rel="stylesheet" src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <!-- Page Heading -->
        <div class="heading">
            <div class="page-heading">Folders</div>

        </div>
        <div class="buttons">


            <asp:Button ID="BackFolderBTN" runat="server" Visible="false" class="btn back-btn" Text="Back" OnClick="BackFolderBTN_Click" />
            <div class="new-btn">
                <asp:Button ID="newFolderBTN" runat="server" class="btn back-btn" Text="+ Add Folder" Visible="false" />
            </div>
        </div>

        <div class="useract-card-body">
            <%-- DEPARTMENT --%>
            <asp:Repeater ID="rptDepartment" runat="server">
                <HeaderTemplate>
                    <div class="row">
                </HeaderTemplate>
                <ItemTemplate>
                    <%-- Open a new div row every 5 items --%>
                    <%# (Container.ItemIndex % 6 == 0) ? "<div class='dept-cards col-11 d-flex'>" : "" %>
                    <div class="col-md-2 user-activity-card-dept">
                        <asp:LinkButton runat="server" CssClass="user-activity-body" CommandArgument='<%# Eval("DepartmentName") %>'
                            OnClick="DepartmentLink_Click">

                        <div class="user-activity-left d-flex align-items-center justify-content-between">
                            <div class="d-flex align-items-center">
                                <i class='bx bxs-folder'></i>
                                <h6 class="mb-0"><%# Eval("DepartmentName") %></h6>
                            </div>
                            
                        </div>
                        </asp:LinkButton>
                    </div>
                    <%-- Close the div row every 5 items --%>
                    <%# ((Container.ItemIndex + 1) % 6 == 0) ? "</div>" : "" %>
                </ItemTemplate>
                <FooterTemplate>
                    <%-- Ensure to close the last row if it wasn't closed in the ItemTemplate --%>
                    <%# (rptDepartment.Items.Count % 6 != 0) ? "</div>" : "" %>
                </FooterTemplate>
            </asp:Repeater>


            <%-- FOLDERS --%>
            <asp:Repeater ID="rptFolder" runat="server" OnItemCommand="rptFolder_ItemCommand">
                <HeaderTemplate>
                    <div class="row">
                        <div class="col-sm-13">
                            <div class="alert alert-success d-none mb-2" id="successAlert_RenameFolder" role="alert"></div>
                        </div>
                        <div class="col-sm-13">
                            <div class="alert alert-danger d-none mb-2" id="errorAlert_RenameFolder" role="alert"></div>
                        </div>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <%-- Open a new div row every 5 items --%>
                    <%# (Container.ItemIndex % 6 == 0) ? "<div class='folder-cards col-11 d-flex'>" : "" %>
                    <div class="col-md-2 user-activity-card">
                        <asp:LinkButton runat="server" CssClass="user-activity-body" CommandArgument='<%# Eval("FolderName") %>' OnClick="FolderLink_Click">                    
                <div class="user-activity-left d-flex align-items-center justify-content-between">
                    <div class="d-flex align-items-center">
                        <i class='bx bxs-folder'></i>
                        <h6 class="mb-0"><%# Eval("FolderName") %></h6>
                    </div>
                    <ul class="navbar-nav">
                        <li class="nav-item dropdown no-arrow mr-3">
                            <a class="nav-link" href="#" id="userDropdown<%# Container.ItemIndex %>" role="button" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <i class='bx bx-dots-vertical-rounded'></i>
                            </a>
                            <div class="dropdown-menu dropdown-menu-end" aria-labelledby="userDropdown<%# Container.ItemIndex %>">
                                <%-- RENAME --%>
                                <asp:LinkButton runat="server" CssClass="dropdown-item rn-dept" data-toggle="modal" data-target="#renameDeptFolderModal" data-renamefolderid='<%# Eval("FolderID") %>'
                                    Visible='<%# CanRenameFolder(Convert.ToInt32(Eval("FolderID"))) %>'>
                                    <i class='bx bx-rename'></i>&nbsp; Rename
                                </asp:LinkButton>
                                <%-- DELETE --%>
                                <asp:LinkButton runat="server" CssClass="dropdown-item" CommandName="ShowDeleteModal" CommandArgument='<%# Eval("FolderID") %>'
                                    Visible='<%# CanDeleteFolder(Convert.ToInt32(Eval("FolderID"))) %>'>
                                    <i class='bx bx-trash'></i>&nbsp; Delete
                                </asp:LinkButton>
                            </div>
                        </li>
                    </ul>
                </div>
                        </asp:LinkButton>
                    </div>
                    <%-- Close the div row every 5 items --%>
                    <%# ((Container.ItemIndex + 1) % 6 == 0) ? "</div>" : "" %>
                </ItemTemplate>
                <FooterTemplate>
                    <%-- Ensure to close the last row if it wasn't closed in the ItemTemplate --%>
                    <%# (rptFolder.Items.Count % 6 != 0) ? "</div>" : "" %>
                </FooterTemplate>
            </asp:Repeater>
            <asp:HiddenField ID="hfDeleteFolderID" runat="server" />


            <!-- DELETE FOLDER Confirmation Modal -->
            <div class="modal fade" id="deleteConfirmationModal" tabindex="-1" role="dialog" aria-labelledby="deleteConfirmationModalLabel" aria-hidden="true">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="deleteConfirmationModalLabel">Confirm Delete</h5>
                        </div>
                        <div class="modal-body">
                            Are you sure you want to delete this folder?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnConfirmDelete" runat="server" CssClass="btn btn-danger" OnClick="btnConfirmDelete_Click" Text="Delete" />
                        </div>
                    </div>
                </div>
            </div>



            <%-- FILES --%>
            <asp:Repeater ID="rptFiles" runat="server" OnItemCommand="rptFiles_ItemCommand">
                <HeaderTemplate>
                    <div class="row">
                </HeaderTemplate>
                <ItemTemplate>
                    <%# (Container.ItemIndex % 6 == 0) ? "<div class='file-cards col-11 d-flex'>" : "" %>
                    <div class="col-md-2 user-activity-card">
                        <asp:LinkButton runat="server" CssClass="user-activity-body" CommandName="Preview" CommandArgument='<%# Eval("FileName") %>'>
                            <div class="user-activity-left d-flex align-items-center justify-content-between">
                                <div class="d-flex align-items-center">
                                    <i class='<%# GetFileIcon(Eval("FileName").ToString()) %> gview-icon'></i>
                                    <h6 class="mb-0"><%# Eval("FileName") %></h6>
                                </div>
                                <ul class="navbar-nav">
                                    <li class="nav-item dropdown no-arrow mr-3">
                                        <a class="nav-link" href="#" id="userDropdown<%# Container.ItemIndex %>" role="button"
                                            data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                            <i class='bx bx-dots-vertical-rounded'></i>
                                        </a>
                                        <div class="dropdown-menu dropdown-menu-end" aria-labelledby="userDropdown<%# Container.ItemIndex %>">
                            <asp:LinkButton runat="server" CssClass="dropdown-item deleteButton" CommandName="Delete" CommandArgument='<%# Eval("FileName") %>'
                                Visible='<%# CheckDeletePermission(Eval("FileName").ToString()) %>'>
                                <i class='bx bx-trash'></i>&nbsp; Delete
                            </asp:LinkButton>
                        </div>
                                    </li>
                                </ul>
                            </div>
                        </asp:LinkButton>
                    </div>
                    <%# ((Container.ItemIndex + 1) % 6 == 0) ? "</div>" : "" %>
                </ItemTemplate>
                <FooterTemplate>
                    <%-- Ensure to close the last row if it wasn't closed in the ItemTemplate --%>
                    <%# (rptFiles.Items.Count % 6 != 0) ? "</div>" : "" %>
                </FooterTemplate>
            </asp:Repeater>
            <asp:HiddenField ID="hrFileName" runat="server" />

            <!-- DELETE FILE Confirmation Modal -->
            <div class="modal fade" id="deleteFileConfirmationModal" tabindex="-1" role="dialog" aria-labelledby="deleteFileConfirmationModalLabel" aria-hidden="true">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="deleteFileConfirmationModalLabel">Confirm Delete File</h5>
                        </div>
                        <div class="modal-body">
                            Are you sure you want to delete this file?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                            <asp:Button ID="btnConfirmDeleteFile" runat="server" CssClass="btn btn-danger" OnClick="btnConfirmDeleteFile_Click" Text="Delete" />
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <!-- ADD FOLDER MODAL -->
        <div class="modal fade" id="addFolderModal" tabindex="-1" role="dialog" aria-labelledby="addFolderModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-body">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        <h5>Add Folder</h5>
                        <br />
                        <asp:HiddenField ID="hfCurrentDepartmentID" runat="server" />
                        <div class="column mb-3">
                            <label for="lbltxtboxFolderName" class="col-form-label">Folder Name</label>
                            <asp:TextBox ID="txtboxFolderName" runat="server" class="form-control"></asp:TextBox>
                        </div>

                        <div class="column mb-3">
                            <div class="dropdown privacy-btn">
                                <label for="lblFolderPrivacy" class="col-form-label">Folder Privacy</label>
                                <br />
                                <asp:DropDownList ID="ddlFolderPrivacy" runat="server" class="btn dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                                    <asp:ListItem Value="" Text="Select Privacy"></asp:ListItem>
                                    <asp:ListItem Value="Only Me" Text="Only Me"></asp:ListItem>
                                    <asp:ListItem Value="My Department" Text="My Department"></asp:ListItem>
                                    <asp:ListItem Value="Public" Text="Public"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="d-flex flex-row justify-content-end">
                            <asp:Button ID="btnSaveChanges" runat="server" Text="Save" class="btn btn-primary" OnClick="btnSaveChanges_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <!-- RENAME FOLDER MODAL -->
        <div class="modal fade" id="renameDeptFolderModal" tabindex="-1" role="dialog" aria-labelledby="renameDeptFolderModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-body">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        <h5>Rename Folder</h5>
                        <br />
                        <asp:HiddenField ID="hfCurrentFolderID" runat="server" />
                        <asp:HiddenField ID="hfDepartmentID" runat="server" />
                        <div class="column mb-3">
                            <label for="lblrnFolder" class="col-form-label">Folder Name</label>
                            <asp:TextBox ID="rnFolder" runat="server" class="form-control"></asp:TextBox>
                        </div>

                        <div class="col-sm-13">
                            <div class="alert alert-danger d-none mb-2" id="errorAlert_RenameFolderModal" role="alert">
                            </div>
                        </div>

                        <div class="d-flex flex-row">
                            <asp:Button ID="btnRename" runat="server" Text="Rename" class="btn btn-primary" OnClick="btnRename_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <script>
        function showDeleteFileModal(filename) {
            // Set the filename in the hidden field
            document.getElementById('<%= hrFileName.ClientID %>').value = filename;
            // Show the delete file confirmation modal
            $('#deleteFileConfirmationModal').modal('show');
        }
        /*ALERT*/
        function showErrorAlert(message) {
            $('#errorAlert_RenameFolder').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_RenameFolder').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }
        function showSuccessAlert(message) {
            $('#successAlert_RenameFolder').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert_RenameFolder').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }

        /*RENAME MODAL ALERT*/
        function showErrorAlertRenameModal(message) {
            $('#errorAlert_RenameFolderModal').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_RenameFolderModal').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds

            $('#renameDeptFolderModal').modal('show');
        }

        /*ADD MODAL*/
        $(document).ready(function () {
            // When any button with the class "filter-btn" is clicked
            $('.new-btn').click(function (e) {
                e.preventDefault();
                $('#addFolderModal').modal('show');
            });
        });
        $(document).ready(function () {
            // When any button with the class "filter-btn" is clicked
            $('.close').click(function (e) {
                e.preventDefault();
                $('#addFolderModal').modal('hide');
            });
        });

        // RENAME MODAL
        $(document).ready(function () {
            $('.rn-dept').click(function (e) {
                e.preventDefault();
                var folderID = $(this).data('renamefolderid');
                var editFolderName = $(this).closest('.user-activity-card').find('h6').text();

                $('#<%= hfCurrentFolderID.ClientID %>').val(folderID);
                $('#<%= rnFolder.ClientID %>').val(editFolderName);

                $('#renameDeptFolderModal').modal('show');
            });

            $('.close').click(function (e) {
                e.preventDefault();
                $('#renameDeptFolderModal').modal('hide');
            });
        });
    </script>

</asp:Content>
