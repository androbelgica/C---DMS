﻿<%@ Page Title="" Language="C#" MasterPageFile="~/GC/gc_topnav.Master" AutoEventWireup="true" CodeBehind="Availment.aspx.cs" Inherits="DMS.GC.Availment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="../CSS/availment.css" />
    <link href='https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css' rel='stylesheet'>
    <link href="https://cdn.lineicons.com/4.0/lineicons.css" rel="stylesheet" />
    <script rel="stylesheet" src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script rel="stylesheet" src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <%-- SCANNER --%>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.5.1/jquery.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.9.3/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script src="https://rawgit.com/schmich/instascan-builds/master/instascan.min.js"></script>
    <script src="https://unpkg.com/html5-qrcode/minified/html5-qrcode.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/quagga@0.12.1/dist/quagga.min.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">

        <!-- Page Heading -->
        <div class="heading">
            <div class="page-heading">Availment</div>
        </div>
        <div class="containerz">
            <div class="card info-card">
                <div class="btn-group mb-3" role="group" aria-label="Basic radio toggle button group">
                    <asp:RadioButton ID="searchGC" runat="server" GroupName="btnGroupRadio" Text="Availment" AutoPostBack="true" OnCheckedChanged="RadioButton_CheckedChanged" Checked="true" />
                    <asp:RadioButton ID="createGC" runat="server" GroupName="btnGroupRadio" Text="Create" AutoPostBack="true" OnCheckedChanged="RadioButton_CheckedChanged" />
                </div>

                <%-- SCAN SECTION --%>
                <div id="scanCard" runat="server" visible="false">
                    <div class="row form-group">
                        <label for="bookedGC" class="col-sm-3 col-form-label">GC Number</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="scannedGC" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label for="bookedStatus" class="col-sm-3 col-form-label">Status</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="scannedStatus" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row form-group">
                        <label for="bookedRecepient" class="col-sm-3 col-form-label">Recipient</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="scannedRecipient" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row form-group">
                        <label for="bookedEntitlement" class="col-sm-3 col-form-label">Entitlement</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="scannedEntitlement" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row form-group">
                        <label for="bookedDescription" class="col-sm-3 col-form-label">Description</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="scannedDescription" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row form-group">
                        <label for="bookedDOI" class="col-sm-3 col-form-label">Date of Issue</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="scannedDOI" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row form-group">
                        <label for="bookedValidity" class="col-sm-3 col-form-label">Validity</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="scannedValidity" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div runat="server" id="Div1">
                        <div class="row form-group">
                            <label for="bookedfrom" class="col-sm-3 col-form-label" id="scanFromLabel">Booked From</label>
                            <div class="col-sm-9">
                                <asp:TextBox ID="scannedFrom" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>
                        <div runat="server" id="Div2">
                            <div class="row form-group">
                                <label for="bookedto" class="col-sm-3 col-form-label" id="scanToLabel">Booked To</label>
                                <div class="col-sm-9">
                                    <asp:TextBox ID="scannedTo" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <asp:HiddenField ID="hidScannedGC" runat="server" />
                    <asp:HiddenField ID="hidScannedRecipient" runat="server" />
                    <asp:HiddenField ID="hidScannedEntitlement" runat="server" />

                    <div class="row form-group">
                        <div class="col-sm-9">
                            <asp:Button ID="btnBackToSearch" runat="server" Text="Back to Search" CssClass="btn btn-sm btn-generate" OnClick="btnBackToSearch_Click" />
                            <asp:Button ID="btnAvailBook" runat="server" Text="Proceed to Booking" CssClass="btn btn-sm btn-generate ml-3" OnClick="btnAvailBook_Click" Style="display: none;" />
                        </div>
                    </div>
                </div>

                <%-- SEARCH SECTION --%>
                <div id="searchCard" runat="server">
                    <div class="form-group">
                        <label>Enter Gift Card Number</label>
                        <asp:TextBox ID="gcNumber" runat="server" CssClass="form-control" placeholder="Ex. 08-1111" AutoPostBack="True" OnTextChanged="gcNumber_TextChanged"></asp:TextBox>
                        <div runat="server" id="Div3">
                            <label for="btnScanQR" class="col-sm-3 col-form-label" id="OrLabel">OR</label><br />
                            <asp:Button ID="btnScanQR" runat="server" Text="Scan QR" CssClass="btn btn-sm btn-generate" OnClick="btnScanQR_Click" />
                        </div>
                    </div>
                </div>

                <div id="successMessage" class="alert alert-success d-none" role="alert">
                </div>
                <div id="errorMessage" class="alert alert-danger d-none" role="alert">
                </div>

                <%-- BOOKING SECTION --%>
                <div id="bookingSection" runat="server" visible="false">
                    <div class="row form-group">
                        <label for="status" class="col-sm-3 col-form-label">Status</label>
                        <div class="col-sm-9">
                            <asp:DropDownList ID="searchDDL" runat="server" CssClass="form-control" onchange="handleStatusChange()">
                                <asp:ListItem Value="" disabled>Select status</asp:ListItem>
                                <asp:ListItem Value="Available">Available</asp:ListItem>
                                <asp:ListItem Value="Cancelled">Cancelled</asp:ListItem>
                                <asp:ListItem Value="Lost">Lost</asp:ListItem>
                                <asp:ListItem Value="Replaced">Replaced</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label for="bookingRecipient" class="col-sm-3 col-form-label">Recipient</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="bookingRecipient" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label for="bookingEntitlement" class="col-sm-3 col-form-label">Entitlement</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="bookingEntitlement" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row form-group" id="bookingDatesRow" style="display: none;">
                        <label for="bookingFromDate" class="col-sm-3 col-form-label">Booked From:</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="bookingFromDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvBookingFromDate" runat="server" ControlToValidate="bookingFromDate" ErrorMessage="Booking from date is required." CssClass="text-danger"></asp:RequiredFieldValidator>
                            <br />
                            <asp:CompareValidator ID="cvBookingFromDate" runat="server"
                                ControlToValidate="bookingFromDate"
                                Operator="GreaterThanEqual"
                                Type="Date"
                                ErrorMessage="Booking from date must be today or a future date."
                                Display="Dynamic"
                                ValidationGroup="BookingValidationGroup"
                                SetFocusOnError="true"
                                CssClass="text-danger"
                                EnableClientScript="true">
                            </asp:CompareValidator>
                        </div>
                    </div>

                    <div class="row form-group" id="bookingToDateRow" style="display: none;">
                        <label for="bookingToDate" class="col-sm-3 col-form-label">Booked To:</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="bookingToDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvBookingToDate" runat="server" ControlToValidate="bookingToDate" ErrorMessage="Booking to date is required." CssClass="text-danger"></asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Button ID="BookNowButton" runat="server" Text="Book Now" OnClick="BookNowButton_Click" CssClass="btn btn-sm book-btn" ValidationGroup="BookingValidationGroup" />
                        <asp:Button ID="BackToScanQR" runat="server" Text="Back to Scan QR" CssClass="btn btn-sm book-btn ml-5" OnClick="btnScanQR_Click" Visible="false" CausesValidation="false" />
                    </div>
                </div>

                <%-- BOOKED SECTION --%>
                <div id="bookedSection" runat="server" visible="false">
                    <div class="row form-group">
                        <label for="bookedStatus" class="col-sm-3 col-form-label">Status</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="bookedStatus" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label for="bookedRecepient" class="col-sm-3 col-form-label">Recipient</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="bookedRecipient" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label for="bookedEntitlement" class="col-sm-3 col-form-label">Entitlement</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="bookedEntitlement" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label for="bookedDescription" class="col-sm-3 col-form-label">Description</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="bookedDescription" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label for="bookedDOI" class="col-sm-3 col-form-label">Date of Issue</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="bookedDOI" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group">
                        <label for="bookedValidity" class="col-sm-3 col-form-label">Validity</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="bookedValidity" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group" runat="server" id="bookedFromRow">
                        <label for="bookedfrom" class="col-sm-3 col-form-label" id="bookedFromLabel">Booked From</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="bookedFrom" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row form-group" runat="server" id="bookedToRow">
                        <label for="bookedto" class="col-sm-3 col-form-label" id="bookedToLabel">Booked To</label>
                        <div class="col-sm-9">
                            <asp:TextBox ID="bookedTo" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <%-- CREATE SECTION --%>
                <div id="createCard" runat="server" visible="false">
                    <div class="form-row">
                        <div class="form-group col-md-6">
                            <label for="quantity">Quantity</label>
                            <asp:TextBox ID="quantityTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvquantity" runat="server" ControlToValidate="quantityTextBox" ErrorMessage="Quantity is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="AllInputsGroup"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-6">
                            <label for="recipient">Recipient</label>
                            <asp:TextBox ID="recipientTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvRecipient" runat="server" ControlToValidate="recipientTextBox" ErrorMessage="Recipient is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="AllInputsGroup"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-6">
                            <label for="entitlement">Entitlement</label>
                            <asp:DropDownList ID="entitlementDropDownList" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Select entitlement</asp:ListItem>
                                <asp:ListItem Value="Free Accommodation">Free Accommodation</asp:ListItem>
                                <asp:ListItem Value="Free Dinner Buffet">Free Dinner Buffet</asp:ListItem>
                                <asp:ListItem Value="Free Lunch Buffet">Free Lunch Buffet</asp:ListItem>
                                <asp:ListItem Value="Free Breakfast Buffet">Free Breakfast Buffet</asp:ListItem>
                                <asp:ListItem Value="Free Spa">Free Spa</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvEntitlement" runat="server" ControlToValidate="entitlementDropDownList" InitialValue="" ErrorMessage="Entitlement is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="AllInputsGroup"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-6">
                            <label for="branch">Branch</label>
                            <asp:DropDownList ID="hotelDropDownList" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Select branch</asp:ListItem>
                                <asp:ListItem Value="Hotel 1">Hotel 1</asp:ListItem>
                                <asp:ListItem Value="Hotel 2">Hotel 2</asp:ListItem>
                                <asp:ListItem Value="Hotel 3">Hotel 3</asp:ListItem>
                                <asp:ListItem Value="The B Hotel">Hotel 1</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvBranch" runat="server" ControlToValidate="hotelDropDownList" InitialValue="" ErrorMessage="Branch is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="AllInputsGroup"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="description">Description</label>
                        <asp:TextBox ID="descriptionTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="descriptionTextBox" ErrorMessage="Description is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="AllInputsGroup"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-6">
                            <label for="dateOfIssue">Date of Issue</label>
                            <asp:TextBox ID="dateOfIssueTextBox" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDateOfIssue" runat="server" ControlToValidate="dateOfIssueTextBox" ErrorMessage="Date of Issue is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="AllInputsGroup"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="cvDateOfIssue" runat="server"
                                ControlToValidate="dateOfIssueTextBox"
                                Operator="GreaterThanEqual"
                                Type="Date"
                                ErrorMessage="Date of Issue must be today or a future date."
                                Display="Dynamic"
                                ValidationGroup="AllInputsGroup"
                                SetFocusOnError="true"
                                CssClass="invalid-feedback"
                                EnableClientScript="true">
                            </asp:CompareValidator>
                        </div>
                        <div class="form-group col-md-6">
                            <label for="validity">Validity</label>
                            <asp:TextBox ID="validityTextBox" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvValidity" runat="server" ControlToValidate="validityTextBox" ErrorMessage="Validity is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="AllInputsGroup"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="cvValidity" runat="server"
                                ControlToValidate="validityTextBox"
                                Operator="GreaterThanEqual"
                                Type="Date"
                                ErrorMessage="Validity must be today or a future date."
                                Display="Dynamic"
                                ValidationGroup="AllInputsGroup"
                                SetFocusOnError="true"
                                EnableClientScript="true"
                                CssClass="invalid-feedback">
                            </asp:CompareValidator>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="gcType">Type</label>
                        <asp:DropDownList ID="gcTypeDropDownList" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">Select type</asp:ListItem>
                            <asp:ListItem Value="Complimentary">Complimentary</asp:ListItem>
                            <asp:ListItem Value="Paid">Paid</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvGCType" runat="server" ControlToValidate="gcTypeDropDownList" InitialValue="" ErrorMessage="Type is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="AllInputsGroup"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-6">
                            <label for="chargeTo">Charge To</label>
                            <asp:TextBox ID="chargeToTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvChargeTo" runat="server" ControlToValidate="chargeToTextBox" ErrorMessage="Charge To is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="AllInputsGroup"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-6">
                            <label for="status">Status</label>
                            <asp:TextBox ID="statusTextBox" runat="server" CssClass="form-control" Text="Available" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="d-flex flex-row">
                        <asp:Button ID="btnGenerateQR" runat="server" Text="Create" OnClick="btnGenerateQR_Click" CssClass="btn btn-sm btn-generate" ValidationGroup="AllInputsGroup" />
                        <div class="scnqr-btn">
                        </div>
                    </div>
                </div>
            </div>

            <div class="card qr-scan">
                <div>
                    <asp:Image ID="imgQRCode" runat="server" CssClass="img-fluid" />
                    <asp:Panel ID="qrCodeContainer" runat="server" Visible="false">
                        <!-- QR code will be displayed here -->
                    </asp:Panel>
                </div>
                <div class="form-group" id="generatedGCNumbersDiv" runat="server" visible="false">
                    <label for="generatedGCNumbers">Gift Certificate Numbers</label>
                    <asp:ListBox ID="generatedGCNumbersListBox" runat="server" CssClass="form-control gc-list"></asp:ListBox>
                </div>
                <div id="scannerSection" runat="server" class="scanner-section" visible="false">
                    <div class="scanner">
                        <video id="video-preview" width="500" height="500" autoplay playsinline></video>
                    </div>
                    <!-- Retake button -->
                    <div class="row mb-3" id="retakeButtonRow" style="display: none;">
                        <div class="col-sm-9 offset-sm-3">
                            <asp:Button ID="btnRetake" runat="server" Text="Retake" CssClass="btn btn-secondary" OnClientClick="StartQRCodeScan(); return false;" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        let scanner;

        function StartQRCodeScan() {
            // Hide the retake button before starting scan
            document.getElementById('retakeButtonRow').style.display = 'none';

            // Check if it's an iOS device
            if (/iPhone|iPad|iPod/i.test(navigator.userAgent)) {
                StartIOSQRScan();
            } else {
                StartAndroidQRScan();
            }
        }
        function StartAndroidQRScan() {
            scanner = new Instascan.Scanner({ video: document.getElementById('video-preview') });

            // Define event handler for when QR codes are detected
            scanner.addListener('scan', function (content) {
                ProcessScannedData(content); // Call the function to process scanned data
                scanner.stop(); // Stop the scanner after successful scan
            });

            Instascan.Camera.getCameras().then(cameras => {
                if (cameras.length > 0) {
                    // Prefer rear camera if available
                    let rearCamera = cameras.find(camera => camera.name.includes('back'));
                    if (rearCamera) {
                        scanner.start(rearCamera);
                        // Apply CSS transformation to flip the video horizontally if needed
                        document.getElementById('video-preview').style.transform = 'rotateY(0deg)';
                        document.getElementById('retakeButtonRow').style.display = 'block';
                    } else {
                        // Use the first camera if no back camera found
                        scanner.start(cameras[0]);
                    }
                } else {
                    console.error('No cameras found');
                }
            }).catch(err => {
                console.error('Error retrieving cameras:', err);
            });
        }

        function StartIOSQRScan() {
            // Check if getUserMedia is supported
            if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
                alert('getUserMedia is not supported on this browser');
                return;
            }

            // Determine constraints based on iOS version and camera mode
            const iOSVersion = getIOSVersion();
            let constraints;

            if (iOSVersion && iOSVersion < 12) {
                // For iOS 11 and below, use 'environment' facingMode for back camera
                constraints = {
                    video: {
                        facingMode: {
                            exact: 'environment' // Prefer rear camera
                        }
                    }
                };
            } else {
                // For iOS 12 and above, simply use 'environment' without 'exact'
                constraints = {
                    video: {
                        facingMode: 'environment' // Prefer rear camera
                    }
                };
            }

            navigator.mediaDevices.getUserMedia(constraints)
                .then(function (stream) {
                    let videoElement = document.getElementById('video-preview');
                    videoElement.srcObject = stream;
                    videoElement.setAttribute('playsinline', true); // Required for playing video on iOS
                    videoElement.play();
                    document.getElementById('retakeButtonRow').style.display = 'block';

                    // Initialize QR code scanner based on iOS version
                    if (iOSVersion && iOSVersion < 12) {
                        // Use jsQR for older iOS versions
                        startJSQRScanner(videoElement, stream);
                    } else {
                        // Use QuaggaJS for iOS 12 and newer
                        startQuaggaScanner(videoElement, stream);
                    }
                })
                .catch(function (err) {
                    console.error('Error accessing camera:', err);
                });
        }

        function getIOSVersion() {
            const match = navigator.userAgent.match(/iPhone OS (\d+)/);
            return match ? parseInt(match[1], 10) : null;
        }

        function startQuaggaScanner(videoElement, stream) {
            // Initialize QuaggaJS scanner
            Quagga.init({
                inputStream: {
                    name: "Live",
                    type: "LiveStream",
                    target: videoElement
                },
                decoder: {
                    readers: [
                        "code_128_reader",
                        "ean_reader",
                        "ean_8_reader",
                        "code_39_reader",
                        "code_39_vin_reader",
                        "codabar_reader",
                        "upc_reader",
                        "upc_e_reader",
                        "i2of5_reader",
                        "2of5_reader",
                        "code_93_reader",
                        "code_32_reader",
                        "msi_reader",
                        "pzn_reader",
                        "qrcode_reader",
                        "datamatrix_reader",
                        "pdf417_reader"
                    ]
                }
            }, function (err) {
                if (err) {
                    console.error('Error initializing Quagga:', err);
                    return;
                }
                Quagga.start();

                // Define event handler for when QR codes are detected
                Quagga.onDetected(function (result) {
                    var code = result.codeResult.code;
                    alert('QR Code scanned: ' + code); // Alert with scanned content
                    ProcessScannedData(code); // Process scanned data
                    Quagga.stop(); // Stop the scanner after successful scan
                    stream.getTracks().forEach(track => track.stop()); // Stop the video stream
                });
            });
        }

        function startJSQRScanner(videoElement, stream) {
            // Use jsQR for QR code scanning on older iOS versions
            const canvasElement = document.createElement('canvas');
            const context = canvasElement.getContext('2d');
            canvasElement.width = videoElement.videoWidth;
            canvasElement.height = videoElement.videoHeight;

            const scanFrame = () => {
                context.drawImage(videoElement, 0, 0, canvasElement.width, canvasElement.height);
                const imageData = context.getImageData(0, 0, canvasElement.width, canvasElement.height);
                const code = jsQR(imageData.data, imageData.width, imageData.height);

                if (code) {
                    alert('QR Code scanned: ' + code.data); // Alert with scanned content
                    ProcessScannedData(code.data); // Process scanned data
                    stream.getTracks().forEach(track => track.stop()); // Stop the video stream
                } else {
                    requestAnimationFrame(scanFrame);
                }
            };

            scanFrame();
        }

        function ProcessScannedData(content) {
            const lines = content.split('\n');

            const giftCode = getValueFromLine(lines, 'GC Number');
            const recipient = getValueFromLine(lines, 'Recipient');
            const entitlement = getValueFromLine(lines, 'Entitlement');
            const description = getValueFromLine(lines, 'Description');
            const dateOfIssue = getValueFromLine(lines, 'Date of Issue');
            const validity = getValueFromLine(lines, 'Validity');
            var status = getStatusValue(lines);

            // Store scanned information in hidden fields
            document.getElementById('<%= hidScannedGC.ClientID %>').value = giftCode;
            document.getElementById('<%= hidScannedRecipient.ClientID %>').value = recipient;
            document.getElementById('<%= hidScannedEntitlement.ClientID %>').value = entitlement;

            document.getElementById('<%= scannedGC.ClientID %>').value = giftCode;
            document.getElementById('<%= scannedStatus.ClientID %>').value = status;
            document.getElementById('<%= scannedRecipient.ClientID %>').value = recipient;
            document.getElementById('<%= scannedEntitlement.ClientID %>').value = entitlement;
            document.getElementById('<%= scannedDescription.ClientID %>').value = description;
            document.getElementById('<%= scannedDOI.ClientID %>').value = dateOfIssue;
            document.getElementById('<%= scannedValidity.ClientID %>').value = validity;

            if (status === 'Booked') {
                const bookedFrom = getValueFromLine(lines, 'Booked From');
                const bookedTo = getValueFromLine(lines, 'Booked To');

                document.getElementById('<%= scannedFrom.ClientID %>').value = bookedFrom;
                document.getElementById('<%= scannedTo.ClientID %>').value = bookedTo;
                document.getElementById('<%= scannedFrom.ClientID %>').style.display = 'block';
                document.getElementById('<%= scannedTo.ClientID %>').style.display = 'block';
                document.getElementById('scanFromLabel').style.display = 'block';
                document.getElementById('scanToLabel').style.display = 'block';
            }
            else if (status === 'Available') {
                document.getElementById('<%= scannedFrom.ClientID %>').style.display = 'none';
                document.getElementById('<%= scannedTo.ClientID %>').style.display = 'none';
                document.getElementById('scanFromLabel').style.display = 'none';
                document.getElementById('scanToLabel').style.display = 'none';

                // Display btnAvailBook
                document.getElementById('<%= btnAvailBook.ClientID %>').style.display = "inline-block";
            }
            else {
                document.getElementById('<%= scannedFrom.ClientID %>').style.display = 'none';
                document.getElementById('<%= scannedTo.ClientID %>').style.display = 'none';
                document.getElementById('scanFromLabel').style.display = 'none';
                document.getElementById('scanToLabel').style.display = 'none';
            }
            document.getElementById('retakeButtonRow').style.display = 'block';
        }

        function getValueFromLine(lines, key) {
            for (let line of lines) {
                if (line.includes(key)) {
                    return line.split(':')[1].trim();
                }
            }
            return '';
        }
        function getStatusValue(lines) {
            for (let i = 0; i < lines.length; i++) {
                if (lines[i].includes('Status')) {
                    return lines[i].split(':')[1].trim();
                }
            }
            return '';
        }

        // handle change status in booking section
        function handleStatusChange() {
            var selectedStatus = $('#<%= searchDDL.ClientID %> option:selected').val();
            if (selectedStatus === 'Cancelled' || selectedStatus === 'Lost' || selectedStatus === 'Replaced') {
                $('#bookingDatesRow').hide();
                $('#bookingToDateRow').hide();
            } else {
                $('#bookingDatesRow').show();
                $('#bookingToDateRow').show();
            }
        }
        // Trigger initial check on page load
        $(document).ready(function () {
            handleStatusChange();
        });
        // Function to enable text selection and copying for ListBox options
        function enableListBoxCopy(listBoxId) {
            var listBox = document.getElementById(listBoxId);
            listBox.setAttribute('size', listBox.length); // Display all options
            listBox.setAttribute('multiple', ''); // Allow multiple selection

            listBox.addEventListener('click', function () {
                var selectedOptions = listBox.selectedOptions;
                var textToCopy = '';

                for (var i = 0; i < selectedOptions.length; i++) {
                    textToCopy += selectedOptions[i].text + '\n';
                }

                // Copy the selected text to clipboard
                navigator.clipboard.writeText(textToCopy).then(function () {
                    console.log('Text copied to clipboard');
                }, function (err) {
                    console.error('Unable to copy text to clipboard', err);
                });
            });
        }
        // Call the function to enable copy for your specific ListBox
        enableListBoxCopy('<%= generatedGCNumbersListBox.ClientID %>');
        function showMessage(type, message) {
            var elementId = type === 'success' ? 'successMessage' : 'errorMessage';
            var alertDiv = document.getElementById(elementId);
            alertDiv.textContent = message;
            alertDiv.classList.remove('d-none');

            setTimeout(function () {
                alertDiv.classList.add('d-none');
            }, 6000); // Hide alert after 3 seconds
        }
    </script>
</asp:Content>