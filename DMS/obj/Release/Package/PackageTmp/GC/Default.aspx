<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DMS.GC.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>The Bellevue Manila Gift Certificate Generator</title>
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap" rel="stylesheet" />
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />

    <style>
        body {
            font-family: 'Roboto', 'Arial', sans-serif;
            background-color: #f5f5f5;
            margin: 0;
            padding: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
        }

        .container {
            background-color: #fff;
            border-radius: 10px;
            padding: 40px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            text-align: center;
            width: 80%;
            max-width: 800px;
        }

        h1 {
            color: #b8860b; /* dark goldenrod */
            margin-bottom: 20px;
        }

        .form-group label {
            font-weight: bold;
        }

        .btn-gold {
            background-color: #b8860b;
            color: #fff;
            border: none;
        }

            .btn-gold:hover {
                background-color: #daa520;
            }
    </style>
</head>
<body>
    <nav class="navbar navbar-expand-lg navbar-dark fixed-top" style="background-color: #b8860b;">
        <a class="navbar-brand" href="Default.aspx">The Bellevue Manila Gift Certificate Generator</a>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
            <ul class="navbar-nav ml-auto">
                <li class="nav-item">
                    <a class="nav-link" href="Create.aspx">Create</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="Availment.aspx">Availment</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="GCInventory.aspx">GC Inventory</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="Layout.aspx">Layout</a>
                </li>
            </ul>
        </div>
    </nav>

    <form id="form1" runat="server">
        <div class="container" />
        <h1>The Bellevue Manila Gift Certificate Generator</h1>
        <div>
            <asp:DropDownList ID="hotelDropDownList" runat="server">
                <asp:ListItem Value="The Bellevue Hotel Manila">The Bellevue Hotel Manila</asp:ListItem>
                <asp:ListItem Value="The Bellevue Resort">The Bellevue Resort</asp:ListItem>
                <asp:ListItem Value="The B Hotel, Alabang">The B Hotel, Alabang</asp:ListItem>
                <asp:ListItem Value="The B Hotel, Quezon City">The B Hotel, Quezon City</asp:ListItem>
            </asp:DropDownList>
            <div class="form-group">
                <label for="gcNumber">Search Gift Certificate Number</label>
                <asp:TextBox ID="gcNumberTextBox" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="form-group">
                <label for="recipient">Recipient</label>
                <asp:TextBox ID="recipientTextBox" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="form-group">
                <label for="entitlement">Entitlement</label>
                <asp:DropDownList ID="entitlementDropDownList" runat="server" CssClass="form-control">
                    <asp:ListItem Value="">Select entitlement</asp:ListItem>
                    <asp:ListItem Value="Free Accommodation">Free Accommodation</asp:ListItem>
                    <asp:ListItem Value="Free Dinner Buffet">Free Dinner Buffet</asp:ListItem>
                    <asp:ListItem Value="Free Lunch Buffet">Free Lunch Buffet</asp:ListItem>
                    <asp:ListItem Value="Free Breakfast Buffet">Free Breakfast Buffet</asp:ListItem>
                    <asp:ListItem Value="Free Spa">Free Spa</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label for="description">Description</label>
                <asp:TextBox ID="descriptionTextBox" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="form-row">
                <div class="form-group col-md-6">
                    <label for="dateOfIssue">Date of Issue</label>
                    <asp:TextBox ID="dateOfIssueTextBox" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                </div>
                <div class="form-group col-md-6">
                    <label for="validity">Validity</label>
                    <asp:TextBox ID="validityTextBox" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                </div>
            </div>
            <div class="form-group">
                <label for="gcType">Type</label>
                <asp:DropDownList ID="gcTypeDropDownList" runat="server" CssClass="form-control">
                    <asp:ListItem Value="">Select type</asp:ListItem>
                    <asp:ListItem Value="Complimentary">Complimentary</asp:ListItem>
                    <asp:ListItem Value="Paid">Paid</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label for="chargeTo">Charge To</label>
                <asp:TextBox ID="chargeToTextBox" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="form-group">
                <label for="status">Status</label>
                <asp:DropDownList ID="statusDropDownList" runat="server" CssClass="form-control">
                    <asp:ListItem Value="">Select status</asp:ListItem>
                    <asp:ListItem Value="Available">Available</asp:ListItem>
                    <asp:ListItem Value="Used">Used</asp:ListItem>
                    <asp:ListItem Value="Cancelled">Cancelled</asp:ListItem>
                    <asp:ListItem Value="Loss">Loss</asp:ListItem>
                    <asp:ListItem Value="Replaced">Replaced</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label for="quantity">Quantity</label>
                <asp:TextBox ID="quantityTextBox" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <asp:Button ID="btnGenerateQR" runat="server" Text="Generate QR Code(s)" OnClick="btnGenerateQR_Click" CssClass="btn btn-gold" />
            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="btn btn-gold" />

            <div class="form-group">
                <label for="generatedGCNumbers">Gift Certificate Numbers</label>
                <asp:ListBox ID="generatedGCNumbersListBox" runat="server" CssClass="form-control"></asp:ListBox>
            </div>
            <div class="form-group text-center">
                <video id="video-preview" width="300" height="200" autoplay></video>
            </div>
            <div class="form-group text-center">
                <asp:Button ID="btnScanQRCode" runat="server" Text="Scan QR Code" CssClass="btn btn-info btn-scan" OnClientClick="StartQRCodeScan(); return false;" />
            </div>
            <asp:Image ID="imgQRCode" runat="server" />
            <asp:Panel ID="qrCodeContainer" runat="server" Visible="false">
                <!-- QR code will be displayed here -->
            </asp:Panel>
        </div>
        <div>
        </div>
    </form>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.5.1/jquery.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.9.3/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script src="https://rawgit.com/schmich/instascan-builds/master/instascan.min.js"></script>
    <script>
        let scanner;

        function StartQRCodeScan() {
            console.log('Scanning QR Code...');

            // Initialize InstaScan
            scanner = new Instascan.Scanner({ video: document.getElementById('video-preview') });

            // Define event handler for when QR codes are detected
            scanner.addListener('scan', function (content) {
                // Handle the detected QR code content
                alert('QR code detected: ' + content);
                ProcessScannedData(content); // Call the function to process scanned data
                scanner.stop(); // Stop the scanner after successful scan
            });

            // Start scanning
            Instascan.Camera.getCameras().then(function (cameras) {
                if (cameras.length > 0) {
                    scanner.start(cameras[0]);
                } else {
                    console.error('No cameras found.');
                }
            }).catch(function (error) {
                console.error('Error initializing camera:', error);
            });
        }

        function ProcessScannedData(content) {
            // Split the content by new lines
            const lines = content.split('\n');

            // Extract the values from each line
            const giftCode = lines[0].split(':')[1].trim();
            const recipient = lines[1].split(':')[1].trim();
            const entitlement = lines[2].split(':')[1].trim();
            const dateOfIssue = lines[3].split(':')[1].trim();
            const validity = lines[4].split(':')[1].trim();

            // Populate textboxes and dropdowns with extracted data
            // Adjust these IDs according to your markup
            document.getElementById('gcNumberTextBox').value = giftCode;
            document.getElementById('recipientTextBox').value = recipient;
            document.getElementById('entitlementDropDownList').value = entitlement;
            document.getElementById('dateOfIssueTextBox').value = dateOfIssue;
            document.getElementById('validityTextBox').value = validity;

            // Set GC Type dropdown
            const ddlGCType = document.getElementById('gcTypeDropDownList');
            if (entitlement.includes('Complimentary')) {
                ddlGCType.value = 'Complimentary'; // Adjust the value according to your DropDownList options
            } else {
                ddlGCType.value = 'Paid'; // Adjust the value according to your DropDownList options
            }

            // Optionally, you can also display an alert with the scanned QR code data
            alert('QR code scanned: ' + content);
        }
    </script>
</body>
</html>
