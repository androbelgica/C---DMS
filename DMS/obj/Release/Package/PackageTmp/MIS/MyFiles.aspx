<%@ Page Title="" Language="C#" MasterPageFile="~/PAGES/topnav.Master" AutoEventWireup="true" CodeBehind="MyFiles.aspx.cs" Inherits="DMS.MIS.MyFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="../CSS/myfiles.css" />
    <link href='https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css' rel='stylesheet'>
    <link href="https://cdn.lineicons.com/4.0/lineicons.css" rel="stylesheet" />

    <!-- jQuery and Bootstrap JS -->
    <script rel="stylesheet" src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script rel="stylesheet" src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.16.0/umd/popper.min.js"></script>
    <script rel="stylesheet" src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <!-- Page Heading -->
        <div class="d-sm-flex align-items-left justify-content-between heading">
            <h1 class="h1 mb-0 text-gray-800">My Files</h1>


            <div class="input-group input-group-sm search-bar p-2">
                <asp:TextBox ID="searchtxtbox" runat="server" class="form-control search-textbox" placeholder="Search" aria-label="search" aria-describedby="search" AutoPostBack="true"></asp:TextBox>
                <div class="input-group-append">
                    <span class="input-group-text bg-transparent" id="search2"><i class='bx bx-search-alt'></i></span>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-xl-12 col-md-6">
                    <div class="mb-3">
                        <div class="alert alert-danger d-none mb-2" id="errorAlert" role="alert">
                        </div>
                        <div class="alert alert-success d-none mb-2" id="successAlert" role="alert">
                        </div>
                    </div>
                    <asp:GridView ID="GridView1" runat="server" EnableViewState="true" AutoGenerateColumns="False" OnPageIndexChanging="GridView1_PageIndexChanging"
                        CssClass="custom-gridview" PageSize="9" AllowPaging="true"
                        OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating"
                        OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowDeleting="GridView1_RowDeleting"
                        DataKeyNames="ControlID">
                        <Columns>
                            <asp:BoundField DataField="ControlID" HeaderText="Control ID" SortExpression="ControlID" ReadOnly="true" />

                            <asp:TemplateField HeaderText="File Name" SortExpression="FileName">
                                <ItemTemplate>
                                    <asp:Label ID="lblFileName" runat="server" Text='<%# Eval("FileName") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtFileName" runat="server" Text='<%# Bind("FileName") %>' Width="150px"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="UploadDateTime" HeaderText="Date & Time" SortExpression="UploadDateTime" ReadOnly="true" />

                            <%-- Privacy --%>
                            <asp:TemplateField HeaderText="Privacy" SortExpression="Privacy">
                                <ItemTemplate>
                                    <asp:Label ID="lblMyFilesPrivacy" runat="server" Text='<%# Eval("Privacy") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlMyFilesPrivacy" runat="server" class="btn btn-primary btn-sm dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                                        <asp:ListItem Text="Only Me" Value="Only Me"></asp:ListItem>
                                        <asp:ListItem Text="My Department" Value="My Department"></asp:ListItem>
                                        <asp:ListItem Text="Public" Value="Public"></asp:ListItem>
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="Category" HeaderText="Category" SortExpression="Category" ReadOnly="true" />

                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <ul class="navbar-nav">
                                        <!-- Nav Item - User Information -->
                                        <li class="nav-item dropdown no-arrow mr-3">
                                            <a class="nav-link" href="#" id="userDropdown" role="button"
                                                data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                                <i class='bx bx-dots-vertical-rounded'></i>
                                                <!-- Kebab icon -->
                                            </a>
                                            <div class="dropdown-menu" aria-labelledby="userDropdown">
                                                <asp:LinkButton runat="server" CssClass="dropdown-item" CommandName="Edit">
                <i class='bx bx-edit-alt'></i>&nbsp Edit
                                                </asp:LinkButton>
                                                <asp:LinkButton runat="server" CssClass="dropdown-item">
                <i class="lni lni-eye"></i>&nbsp Preview
                                                </asp:LinkButton>
                                                <asp:LinkButton runat="server" CssClass="dropdown-item" CommandName="Delete">
                <i class='bx bx-trash'></i>&nbsp Delete
                                                </asp:LinkButton>
                                            </div>
                                        </li>
                                    </ul>
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
                            PageButtonCount="5"
                            Position="Bottom" />
                        <PagerStyle CssClass="pagination justify-content-start font-weight-bold" />
                    </asp:GridView>
                </div>
        </div>
    </div>
    <script>
        /*Alert*/

        function showErrorAlert(message) {
            $('#errorAlert').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }
        function showSuccessAlert(message) {
            $('#successAlert').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 2 seconds
        }

        /*End Alert*/
    </script>
</asp:Content>

