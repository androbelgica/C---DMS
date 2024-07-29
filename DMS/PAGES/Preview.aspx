<%@ Page Title="" Language="C#" MasterPageFile="~/PAGES/topnav.Master" AutoEventWireup="true" CodeBehind="Preview.aspx.cs" Inherits="DMS.PAGES.Preview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script defer data-domain="tools.simonwillison.net" src="https://plausible.io/js/script.js"></script>
    <script type="module">
        import pdfjsDist from 'https://cdn.jsdelivr.net/npm/pdfjs-dist@4.0.379/+esm';
        pdfjsLib.GlobalWorkerOptions.workerSrc =
            "https://cdn.jsdelivr.net/npm/pdfjs-dist@4.0.379/build/pdf.worker.min.mjs";
    </script>
    <script src="https://cdn.jsdelivr.net/npm/tesseract.js@5/dist/tesseract.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/mammoth/1.6.0/mammoth.browser.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.17.3/xlsx.full.min.js"></script>

    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="../CSS/preview.css" />
    <link href='https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css' rel='stylesheet'>
    <link href="https://cdn.lineicons.com/4.0/lineicons.css" rel="stylesheet" />
    <script rel="stylesheet" src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script rel="stylesheet" src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.16.0/umd/popper.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <!-- Page Heading -->
        <div class="d-sm-flex align-items-center justify-content-between">
            <h1 class="h1 mb-0 text-gray-800">Preview</h1>
        </div>
        <div class="row">
            <!-- Dropzone/Scan -->
            <div class="cards">
                <div class="card ocr-card">
                    <div class="card-body">
                        <div id="previewContainer" class="preview-container" runat="server"></div>
                        <asp:HiddenField ID="hiddenPreviewFileContent" runat="server" />
                        <asp:HiddenField ID="hiddenPreviewFileName" runat="server" />
                        <asp:HiddenField ID="hiddenPreviewOriginalFileName" runat="server" />
                        <asp:HiddenField ID="hiddenPreviewOriginalPrivacy" runat="server" />
                        <asp:HiddenField ID="hiddenPreviewControlID" runat="server" />
                        <asp:HiddenField ID="hiddenPreviewFileExtension" runat="server" />
                    </div>
                </div>
                <!-- Document Information -->
                <div class="card info-card">
                    <div class="card-body">
                        <div class="form-row mb-4">
                            <div class="row col-md-12 ">
                                <label class="form-label">Document Name</label>
                                <div class="col-sm">
                                    <asp:TextBox ID="previewdocnametxtbox" runat="server" CssClass="form-control form-control-sm doc-info-txtbox" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>

                        </div>

                        <div class="form-row mb-4">
                            <div class="row col-md-6 mr-3">
                                <label class="card-title">Uploader's Name</label>
                                <div class="col-sm">
                                    <asp:TextBox ID="previewuploadernametxtbox" runat="server" CssClass="form-control form-control-sm doc-info-txtbox" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row col-md-6">
                                <label class="card-title">Department</label>
                                <div class="col-sm">
                                    <asp:TextBox ID="previewdepttxtbox" runat="server" CssClass="form-control form-control-sm doc-info-txtbox" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>

                        </div>

                        <div class="form-row mb-4">
                            <div class="row col-md-6 mr-3">
                                <label class="card-title">Date & Time Uploaded</label>
                                <div class="col-sm">
                                    <asp:TextBox ID="datetimetxtbox" runat="server" CssClass="form-control form-control-sm doc-info-txtbox" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row col-md-6">
                                <label class="card-title">Category</label>
                                <div class="col-sm">
                                    <asp:TextBox ID="previewcategorytxtbox" runat="server" CssClass="form-control form-control-sm doc-info-txtbox" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row mb-3">
                            <label class="card-title">Privacy</label>
                            <div class="col-sm">
                                <div class="form-check-group d-flex flex-wrap align-items-center">
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
                        <div class="row mb-3" id="foldersRow" style="display: none;">
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
                        <div class="row mb-3">
                            <div class="alert alert-success d-none mb-2" id="successAlert_Preview" role="alert"></div>
                            <div class="alert alert-danger d-none mb-2" id="errorAlert_Preview" role="alert"></div>
                        </div>
                        <div class="row mb-3">
                            <div class="col-sm">
                                <%--<asp:Button ID="btnPreviewBack" runat="server" Text="Back" class="btn btn-sm prev-btn" OnClick="btnPreviewBack_Click" />--%>
                                <asp:Button ID="btnPreviewDownload" runat="server" Text="Download" class="btn btn-sm dl-btn" OnClick="btnPreviewDownload_Click" />
                                <asp:Button ID="btnPreviewPrint" runat="server" Text="Print" class="btn btn-sm print-btn" Visible="false" />
                                <asp:Button ID="btnPreviewEdit" runat="server" Text="Edit" class="btn btn-sm edit-btn" OnClick="btnPreviewEdit_Click" />
                            </div>
                        </div>
                        <div class="row mb-3">
                            <div class="col-sm">
                                <asp:Button ID="btnPreviewCancel" EnableViewState="true" runat="server" Text="Cancel" class="btn btn-sm cancel-btn" OnClick="btnPreviewCancel_Click" Visible="false"></asp:Button>
                                <asp:Button ID="btnPreviewSubmit" EnableViewState="true" runat="server" Text="Submit" class="btn btn-sm submit-btn" OnClick="btnPreviewSubmit_Click" Visible="false"></asp:Button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
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
        /*Alert*/
        function showErrorAlert(message) {
            $('#errorAlert_Preview').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_Preview').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }

        function showSuccessAlert(message) {
            $('#successAlert_Preview').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert_Preview').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }
        /*End Alert*/
    </script>
</asp:Content>
