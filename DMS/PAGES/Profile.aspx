<%@ Page Title="" Language="C#" MasterPageFile="~/PAGES/topnav.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="DMS.PAGES.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="../CSS/profile.css" />
    <link href='https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css' rel='stylesheet'>
    <link href="https://cdn.lineicons.com/4.0/lineicons.css" rel="stylesheet" />
    <script rel="stylesheet" src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script rel="stylesheet" src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <!-- Page Heading -->
        <div class="heading">
            <div class="page-heading">Profile</div>
        </div>


        <div class="text-muted mb-3">Your Basic Info</div>
        <div class="row g-3 mr-3">
            <div class="col-md-4 mb-2">
                <asp:Label ID="nameTxtboxLabel" runat="server" class="form-label" Text="Name"></asp:Label>
                <asp:TextBox ID="nameTxtbox" class="form-control form-control-sm txtbox" runat="server" ReadOnly="true"></asp:TextBox>
            </div>

            <div class="col-md-4">
                <asp:Label ID="emailTxtboxLabel" runat="server" class="form-label" Text="Email"></asp:Label>
                <asp:TextBox ID="emailTxtbox" class="form-control form-control-sm txtbox" runat="server" ReadOnly="true"></asp:TextBox>
            </div>
            <div class="col-md-4">
                <asp:Label ID="contactTxtboxLabel" runat="server" class="form-label" Text="Contact"></asp:Label>
                <asp:TextBox ID="contactTxtbox" class="form-control form-control-sm txtbox" runat="server" ReadOnly="true"></asp:TextBox>
            </div>
            <div class="col-md-4">
                <asp:Label ID="usernameTxtboxLabel" runat="server" class="form-label" Text="Username"></asp:Label>
                <asp:TextBox ID="usernameTxtbox" class="form-control form-control-sm txtbox" runat="server" ReadOnly="true"></asp:TextBox>
            </div>
            <div class="col-md-4">
                <asp:Label ID="departmentTxtboxLabel" runat="server" class="form-label" Text="Department"></asp:Label>
                <asp:TextBox ID="departmentTxtbox" class="form-control form-control-sm txtbox" runat="server" ReadOnly="true"></asp:TextBox>
            </div>
            <div class="col-md-4">
                <asp:Label ID="positionTxtboxLabel" runat="server" class="form-label" Text="Position"></asp:Label>
                <asp:TextBox ID="positionTxtbox" class="form-control form-control-sm txtbox" runat="server" ReadOnly="true"></asp:TextBox>
            </div>


            <div class="text-muted mt-5">Your Password</div>
            <div class="col-12">
                <button class="btn edit-btn">Change Password</button>
            </div>
        </div>

        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div class="modal fade" id="changePWModal" tabindex="-1" aria-labelledby="changePWModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-body">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        <asp:HiddenField ID="getDeptID" runat="server" />
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div class="user-info">
                                    <h5>Change Password</h5>
                                    <%-- Current Password --%>
                                    <div class="column mb-2">
                                        <label id="currentpwordTxtboxLabel" runat="server" class="col-sm-6 col-form-label col-form-label-sm">Current Password</label>
                                        <div class="col-sm-12">
                                            <asp:TextBox ID="currentpwordTxtbox" runat="server" TextMode="Password" class="form-control form-control-sm"></asp:TextBox>
                                        </div>
                                    </div>
                                    <%-- New Password --%>
                                    <div class="column mb-2">
                                        <label id="newpwordTxtboxLabel" runat="server" class="col-sm-5 col-form-label col-form-label-sm">New Password</label>
                                        <div class="col-sm-12">
                                            <asp:TextBox ID="newpwordTxtbox" runat="server" TextMode="Password" class="form-control form-control-sm"></asp:TextBox>
                                        </div>
                                    </div>
                                    <%-- Re-enter New Password --%>
                                    <div class="column mb-2">
                                        <label id="confirmpwordTxtboxLabel" runat="server" class="col-sm-9 col-form-label col-form-label-sm">Re-enter New Password</label>
                                        <div class="col-sm-12">
                                            <asp:TextBox ID="confirmpwordTxtbox" runat="server" TextMode="Password" class="form-control form-control-sm"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-sm-12">
                                        <div class="alert alert-success d-none mb-2" id="successAlert_ChangePW" role="alert"></div>
                                        <div class="alert alert-danger d-none mb-2" id="errorAlert_ChangePW" role="alert"></div>
                                    </div>

                                    <div class="d-flex flex-row justify-content-end mt-3">
                                        <asp:Button ID="btnEditSubmit" runat="server" class="btn btn-sm submit" Text="Submit" OnClick="SubmitBtn_Click" />
                                    </div>


                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>

        <script>
            $(document).ready(function () {
                $('.edit-btn').click(function (e) {
                    e.preventDefault();
                    $('#changePWModal').modal('show');
                });

                $('.close').click(function (e) {
                    e.preventDefault();
                    $('#changePWModal').modal('hide');
                });
            });

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
                }, 3000); // Hide alert after 3 seconds
            }
        </script>

    </div>
</asp:Content>