<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="DMS.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <!-- Bootstrap CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/css/bootstrap.min.css" rel="stylesheet" />

    <!-- Bootstrap JS (optional if you're using Bootstrap features that require JavaScript) -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/js/bootstrap.bundle.min.js"></script>


    <title>The Bellevue Manila - Document Management System</title>
    <link rel="stylesheet" type="text/css" href="../CSS/login.css" />

</head>


<body>
    <form id="form1" runat="server">
        <div class="cont">
            <div class="container">
                <div class="content">
                    <div class="card">
                        <div class="logo-container">
                            <img src="../Images/Logo 1.png" class="logo" alt="Logo" />

                        </div>
                        <a>Welcome to The Bellevue Manila!</a>
                        <div class="label">
                            Login to your account
                       
                        </div>

                        <div>
                            <asp:TextBox runat="server" class="form-control form-control-lg mb-3" type="text" placeholder="Email" ID="unametxtbox"></asp:TextBox>
                            <asp:TextBox runat="server" type="password" class="form-control form-control-lg mb-1" placeholder="Password" ID="pwordtxtbox"></asp:TextBox>
                            <div class="forgot mb-3">
                                <small><a href="ResetPW.aspx">Forgot Password?</a></small>
                            </div>

                            <asp:Button ID="LoginBtn" runat="server" Text="LOGIN" class="btn btn-sm login-button" OnClick="LoginBtn_Click" />
                        </div>

                        <div class="mt-2">
                            <div class="alert alert-danger mb-2" id="error_alert" runat="server" visible="false" role="alert">
                                <span id="errorMessage"></span>
                            </div>
                        </div>
                    </div>

                </div>
            </div>

        </div>
        <script>
            function showAlertAndHide() {
                // Make the alert visible
                document.getElementById('error_alert').style.display = "block";

                // Set a timeout to hide the alert after 5 seconds
                setTimeout(function () {
                    // Hide the alert after 5 seconds
                    document.getElementById('error_alert').style.display = "none";
                }, 5000); // 5000 milliseconds = 5 seconds
            }

            function redirectToPage(targetUrl) {
                window.location.href = '/load.aspx?url=' + encodeURIComponent(targetUrl);
            }

            window.history.forward();
            function noBack() {
                window.history.forward();
            }
        </script>
    </form>
</body>
</html>