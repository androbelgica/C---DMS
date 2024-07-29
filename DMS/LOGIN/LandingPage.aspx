<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LandingPage.aspx.cs" Inherits="DMS.LOGIN.LandingPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <title>Document Management System</title>
    <link rel="stylesheet" type="text/css" href="../CSS/landing.css" />
</head>


<body>
    <form id="form1" runat="server">
        <nav>
            <div class="logo">
                <img src="../Images/Logo 4.png" alt="LOgi" />
            </div>
            <div>
                <asp:Button ID="LogOutBtn" runat="server" Text="Logout" class="logout-btn" OnClick="LogOutBtn_Click" />
            </div>
        </nav>

        <div class="container">
            <div class="content">
                <h3>Welcome to The Hotels <br /> Document Management System!</h3>
                <p>Find, access, and manage hotel documents effortlessly, all from one convenient location.</p>
                <p>Simplify your document management process with our user-friendly system. Effortlessly organize, access, and collaborate on all your documents.</p>
                <asp:Button ID="DMSBtn" runat="server" Text="Dashboard" class="btn" OnClick="DMSBtn_Click" />
                <asp:Button ID="GCtBtn" runat="server" Text="Gift Certificate" class="btn btn-secondary" OnClick="GCBtn_Click" />
            </div>
        </div>

    </form>
</body>
<script>

    function redirectToPage(targetUrl) {
        window.location.href = '/load.aspx?url=' + encodeURIComponent(targetUrl);
    }
</script>

</html>
