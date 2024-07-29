<%@ Page Title="" Language="C#" MasterPageFile="~/PAGES/topnav.Master" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="DMS.Upload" %>

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
    <link rel="stylesheet" type="text/css" href="../CSS/upload.css" />
    <link href='https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css' rel='stylesheet'>
    <link href="https://cdn.lineicons.com/4.0/lineicons.css" rel="stylesheet" />
    <script rel="stylesheet" src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script rel="stylesheet" src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.16.0/umd/popper.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <!-- Camera -->
    <script src="https://cdn.jsdelivr.net/npm/tesseract.js@2.1.0/dist/tesseract.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">

        <!-- Page Heading -->
        <div class="d-sm-flex align-items-center justify-content-between">
            <h1 class="h1 mb-0 text-gray-800">Upload File</h1>
        </div>

        <div class="scan-upload-info">
            <!-- Dropzone/Scan -->
            <div class="file-upload-container" id="fileUploadContainer">
                <div class="upload-border">
                    <!-- Display selected, dropped or scanned files -->
                    <div id="filedisplayContainer" class="scanned-image-container" runat="server">
                        <video id="video-preview" class="scanned-image-container video" style="display: none;" autoplay></video>
                        <canvas id="canvas" style="display: none;"></canvas>
                    </div>
                    <asp:Button ID="btnClearScannedImage" runat="server" OnClick="btnClearScannedImage_Click" Style="display: none;" />

                    <asp:HiddenField ID="hiddenScannedImageContent" runat="server" />
                    <asp:HiddenField ID="hiddenCaptureImageData" runat="server" />

                    <asp:FileUpload ID="fileUploader" runat="server" class="dropzone-input" type="file"
                        accept=".pdf,.jpg,.jpeg,.png,.gif,.docx,.xlsx,.txt" onchange="handleFileUpload(this)" />

                    <asp:HiddenField ID="hiddenFileContent" runat="server" />
                    <asp:HiddenField ID="hiddenFileName" runat="server" />

                    <div class="upload-file-text" id="uploadFileText">
                        <div class="dropzone-icon" id="dropzone">
                            <span><i class='bx bx-cloud-upload'></i></span>
                        </div>
                        <div class="dropzone">
                            <span>Drop a file here</span>
                        </div>
                        <div class="dropzone">
                            <span>or</span>
                        </div>
                        <div class="dropzone">
                            <span>click here to select a file</span>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Document Information -->
            <div class="doc-infor">
                <div class="card shadow-sm">

                    <div class="card-body file-info-box">

                        <div class="btn-group" role="group" aria-label="Basic radio toggle button group">
                            <asp:RadioButton ID="selectFile" runat="server" GroupName="btnGroupRadio" Text="Choose File" AutoPostBack="true" OnCheckedChanged="RadioButton_CheckedChanged" Checked="true" />
                            <asp:RadioButton ID="scanFile" runat="server" GroupName="btnGroupRadio" Text="Scan" AutoPostBack="true" OnCheckedChanged="RadioButton_CheckedChanged" />
                            <asp:RadioButton ID="camera" runat="server" GroupName="btnGroupRadio" Text="Camera" AutoPostBack="true" OnCheckedChanged="RadioButton_CheckedChanged" />
                        </div>

                        <div class="row mb-3">
                            <label class="card-title">Document Name</label>
                            <div class="col-sm">
                                <asp:TextBox ID="docnametxtbox" runat="server" CssClass="form-control form-control-sm doc-info-txtbox" Placeholder=""></asp:TextBox>
                                <asp:HiddenField ID="hddnFileExtension" runat="server" />
                            </div>

                        </div>
                        <div class="row mb-3">
                            <label class="card-title">File Source/Source Type</label>
                            <div class="col-sm-10">
                                <div class="dropdown upload-drop">
                                    <input type="text" id="dropdownMenuButton" class="btn btn-sm dropdown-toggle" readonly />
                                </div>

                            </div>
                        </div>
                        <div class="row mb-3">
                            <label class="card-title">Select Privacy Option</label>
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
                        <br />
                        <div class="row mb-3">
                            <div class="alert alert-success d-none mb-2" id="successAlert_Upload" role="alert"></div>
                            <div class="alert alert-danger d-none mb-2" id="errorAlert_Upload" role="alert"></div>
                        </div>
                        <div class="row mb-3">
                            <div class="col-sm">
                                <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="btn btn-sm upload-file" OnClick="btnUpload_Click" />
                                <asp:Button ID="btnScan" runat="server" Text="Scan" CssClass="btn btn-sm scan-file" OnClick="btnScan_Click" Visible="false" />
                                <asp:Button ID="btnOpenCamera" runat="server" Text="Retake" CssClass="btn btn-sm camera-file" OnClick="btnOpenCamera_Click" Visible="false" />
                                <asp:Button ID="btnCloseScanCamera" runat="server" Text="Close Camera" CssClass="btn btn-sm camera-file" OnClick="btnCloseScanCamera_Click" Visible="false" />
                                <asp:Button ID="btnCaptureFileScan" runat="server" Text="Capture" CssClass="btn btn-sm camera-file" OnClick="btnCaptureFileScan_Click" Visible="false" />
                            </div>
                        </div>
                        <div class="row mb-3">
                            <div class="col-sm">
                                <asp:HiddenField ID="currentImageIndex" runat="server" Value="0" />
                                <asp:Button ID="btnNext" runat="server" Text="Next" class="btn btn-sm scan-file" OnClick="btnNext_Click" Visible="false" />
                                <asp:Button ID="btnPrevious" runat="server" Text="Previous" class="btn btn-sm scan-file" OnClick="btnPrevious_Click" Visible="false" />
                                <asp:Button ID="btnDelete" runat="server" Text="Delete" class="btn btn-sm scan-file" OnClick="btnDelete_Click" Visible="false" />
                                <div class="full-document-section d-none" id="ocrSection">
                                    <h2>OCR Text</h2>
                                    <textarea class="full-document" id="ocrTextArea" runat="server"></textarea>
                                </div>
                                <asp:HiddenField ID="hiddenOCRText" runat="server" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
        <asp:HiddenField ID="selectedCategory" runat="server" />
    </div>
    <script>
        let stream = null;

        // Function to open the camera
        function openCamera() {
            navigator.mediaDevices.getUserMedia({ video: true })
                .then(function (mediaStream) {
                    stream = mediaStream;
                    var video = document.getElementById('video-preview');
                    video.srcObject = stream;
                    video.style.display = 'block';
                    video.play();
                })
                .catch(function (err) {
                    console.error('Error accessing the camera: ', err);
                });
        }

        // Function to close the camera
        function closeCamera() {
            if (stream) {
                let tracks = stream.getTracks();
                tracks.forEach(track => track.stop());
                stream = null;
            }
            document.getElementById('video-preview').style.display = 'none';
            // Hide the ocrSection when closing camera
            document.getElementById('ocrSection').style.display = 'none';
        }

        function handleCaptureButtonClick(event) {
            event.preventDefault(); // Prevent form submission

            var video = document.getElementById('video-preview');
            var canvas = document.createElement('canvas'); // Create a new canvas element
            var context = canvas.getContext('2d');
            // Set canvas width and height to match the video dimensions
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            context.drawImage(video, 0, 0, canvas.width, canvas.height);

            var cameraImage = canvas.toDataURL('image/png');
            // Store the captured image data in a hidden field or input
            var hiddenCaptureImageData = document.getElementById('<%= hiddenCaptureImageData.ClientID %>');
            hiddenCaptureImageData.value = cameraImage;

            // Clear previous content in filedisplayContainer
            var filedisplayContainer = document.getElementById('<%= filedisplayContainer.ClientID %>');
            filedisplayContainer.innerHTML = '';

            // Create an image element and set its attributes
            var imgElement = document.createElement('img');
            imgElement.src = cameraImage;

            // Append the image element to filedisplayContainer
            filedisplayContainer.appendChild(imgElement);
            // Set to 'Camera'
            document.getElementById('dropdownMenuButton').value = 'Camera';
            // Display OCR text in ocr text area to store
            Tesseract.recognize(
                cameraImage,
                'eng',
                { logger: m => console.log(m) }
            ).then(({ data: { text } }) => {
                var ocrTextArea = document.getElementById('<%= ocrTextArea.ClientID %>');
                if (ocrTextArea) {
                    ocrTextArea.value = text;
                    closeCamera();
                    // Show the retake button to open camera
                    document.getElementById('<%= btnOpenCamera.ClientID %>').style.display = 'block';
                } else {
                    alert('OCR text area not found.');
                }
            }).catch(error => {
                console.error('Error during OCR:', error);
                // Handle OCR error
            });

            // Show the open camera button to open camera for retake
            
        }

        function showErrorAlert(message) {
            $('#errorAlert_Upload').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#errorAlert_Upload').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }

        function showSuccessAlert(message) {
            $('#successAlert_Upload').text(message).removeClass('d-none').addClass('show');
            setTimeout(function () {
                $('#successAlert_Upload').removeClass('show').addClass('d-none');
            }, 3000); // Hide alert after 3 seconds
        }

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

        function selectCategory(category) {
            document.getElementById('<%= selectedCategory.ClientID %>').value = category;
            document.getElementById('dropdownMenuButton').innerText = category;
        }

        function autoFillDocumentName() {
            document.getElementById('<%= docnametxtbox.ClientID %>').value = '';
        }

        function hideUploadFileText() {
            document.getElementById('uploadFileText').style.display = 'none';
        }

        function showUploadFileText() {
            document.getElementById('uploadFileText').style.display = 'block';
        }

        function clearScannedImage() {
            // Find the scanned image container
            var filedisplayContainer = document.getElementById('<%= filedisplayContainer.ClientID %>');

            // Check if the container exists
            if (filedisplayContainer) {
                // Clear the content of the container
                filedisplayContainer.innerHTML = '';

                // Hide the container
                filedisplayContainer.style.display = 'none';
            }
        }

        $(document).ready(function () {
            // File input change handler
            $('#<%= fileUploader.ClientID %>').on('change', function (event) {
                var file = event.target.files[0];
                if (file) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        $('#<%= hiddenFileContent.ClientID %>').val(e.target.result);
                        $('#<%= hiddenFileName.ClientID %>').val(file.name);
                        $('#<%= docnametxtbox.ClientID %>').val(file.name);
                    };
                    reader.readAsDataURL(file);
                }
            });

            // Dropzone click handler
            $('#dropzone').click(function () {
                $('#<%= fileUploader.ClientID %>').click();
            });

            // Drag and drop handlers
            $('#dropzone').on('dragover', function (event) {
                event.preventDefault();
                $(this).addClass('drag-over');
            }).on('dragleave', function (event) {
                event.preventDefault();
                $(this).removeClass('drag-over');
            }).on('drop', function (event) {
                event.preventDefault();
                $(this).removeClass('drag-over');
                var file = event.originalEvent.dataTransfer.files[0];
                if (file) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        $('#<%= hiddenFileContent.ClientID %>').val(e.target.result);
                        $('#<%= hiddenFileName.ClientID %>').val(file.name);
                        $('#<%= docnametxtbox.ClientID %>').val(file.name);
                    };
                    reader.readAsDataURL(file);
                }
            });

            // Add event listener for the capture button after document is fully loaded
            document.getElementById('<%= btnCaptureFileScan.ClientID %>').addEventListener('click', handleCaptureButtonClick);
        });

        /*Dropdown*/
        function changeButtonText(category) {
            document.getElementById('<%= selectedCategory.ClientID %>').value = category;
            document.getElementById('dropdownMenuButton').innerText = category;
        }
        /*End Dropdown*/

        /*Start Upload*/
        let dropzone, fileInput, uploadFileText;
        let tesseractWorker;
        let fileSelectionAllowed = true;
        let selectedFile = null;

        function handleFileInput(event) {
            const file = event.target.files[0];
            processFile(file);
        }

        window.onload = function () {
            const selectedCategory = document.getElementById('<%= selectedCategory.ClientID %>').value;
            if (selectedCategory) {
                document.getElementById('dropdownMenuButton').innerText = selectedCategory;
            }

            dropzone = document.getElementById('dropzone');
            fileInput = document.getElementById('<%= fileUploader.ClientID %>');
            uploadFileText = document.getElementById('uploadFileText');

            fileInput.addEventListener('change', handleFileInput);

            dropzone.addEventListener('click', () => {
                if (fileSelectionAllowed) fileInput.click();
            });

            // Set event listeners
            dropzone.addEventListener('dragover', (event) => {
                event.preventDefault();
                if (fileSelectionAllowed) {
                    dropzone.classList.add('drag-over');
                }
            });

            dropzone.addEventListener('dragleave', (event) => {
                event.preventDefault();
                if (fileSelectionAllowed) {
                    dropzone.classList.remove('drag-over');
                }
            });

            dropzone.addEventListener('drop', handleDrop);
            dropzone.addEventListener('click', handleClick);

            fileInput.addEventListener('change', (event) => {
                clearScannedImage(); // clear scanned document when user click or drop a file to the dropzone
                // Automatically set the value to "Digital" in the read-only textbox
                document.getElementById('dropdownMenuButton').value = 'Digital';
                const file = event.target.files[0];
                processFile(file);
            });

            initializeTesseract();
        };

        async function handleDrop(event) {
            event.preventDefault();
            if (fileSelectionAllowed) {
                dropzone.classList.remove('drag-over');
                const file = event.dataTransfer.files[0];
                fileInput.files = event.dataTransfer.files;
                processFile(file);
            }
        }

        async function handleClick() {
            if (fileSelectionAllowed) {
                fileInput.click();
            }
        }

        async function processFile(file) {
            if (!file) return;

            const reader = new FileReader();
            reader.onload = function (e) {
                document.getElementById('<%= hiddenFileContent.ClientID %>').value = e.target.result;
                document.getElementById('<%= hiddenFileName.ClientID %>').value = file.name;
            };
            reader.readAsDataURL(file);

            // Hide the upload instructions text
            uploadFileText.style.display = 'none';

            const originalText = dropzone.innerText;
            dropzone.innerText = 'Processing file...';
            dropzone.classList.add('disabled');
            fileSelectionAllowed = false;

            // Display file preview
            if (file.type === 'application/pdf') {
                await processPDF(file);
            } else if (file.type === 'image/jpeg' || file.type === 'image/png' || file.type === 'image/gif') {
                await processImage(file);
            } else if (file.type === 'application/vnd.openxmlformats-officedocument.wordprocessingml.document') {
                await processDocx(file);
            } else if (file.type === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet') {
                await processXlsx(file);
            } else if (file.type === 'text/plain') {
                await processTxt(file);
            }

            dropzone.innerText = originalText;
            dropzone.classList.remove('disabled');
            fileSelectionAllowed = true;
        }

        // PDF file display preview
        async function processPDF(file) {
            const reader = new FileReader();
            reader.onload = async function (e) {
                const pdfData = e.target.result;
                const { numPages, imageIterator } = await convertPDFToImages(pdfData);
                let text = '';

                for await (const image of imageIterator) {
                    const { data: { text: pageText } } = await Tesseract.recognize(image.imageURL);
                    text += pageText + '\n';

                    // Display the page image in the scannedImageContainer
                    displayPageImage(image.imageURL);
                }

                // Display extracted text in the textarea
                document.getElementById('<%= ocrTextArea.ClientID %>').innerText = text;
            };
            reader.readAsArrayBuffer(file);
        }

        async function convertPDFToImages(pdfData) {
            const pdf = await pdfjsLib.getDocument(pdfData).promise;
            const numPages = pdf.numPages;

            async function* images() {
                for (let i = 1; i <= numPages; i++) {
                    try {
                        const page = await pdf.getPage(i);
                        const viewport = page.getViewport({ scale: 1 });
                        const canvas = document.createElement('canvas');
                        const context = canvas.getContext('2d');
                        const desiredWidth = 800; // Adjust as needed
                        canvas.width = desiredWidth;
                        canvas.height = (desiredWidth / viewport.width) * viewport.height;
                        const renderContext = {
                            canvasContext: context,
                            viewport: page.getViewport({ scale: desiredWidth / viewport.width }),
                        };
                        await page.render(renderContext).promise;
                        const imageURL = canvas.toDataURL('image/jpeg', 0.8);
                        yield { imageURL };
                    } catch (error) {
                        console.error(`Error rendering page ${i}:`, error);
                    }
                }
            }

            return { numPages, imageIterator: images() };
        }

        // Image file display preview
        function displayPageImage(imageURL) {
            const filedisplayContainer = document.getElementById('<%= filedisplayContainer.ClientID %>');
            const img = document.createElement('img');
            img.src = imageURL;
            filedisplayContainer.appendChild(img);
            filedisplayContainer.style.display = 'block';
            uploadFileText.style.display = 'none';
        }

        async function processImage(file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                displayImage(e.target.result);
            };
            reader.readAsDataURL(file);
        }

        function displayImage(imageURL) {
            const filedisplayContainer = document.getElementById('<%= filedisplayContainer.ClientID %>');
            filedisplayContainer.innerHTML = `<img src="${imageURL}" alt="Preview Image" />`;
            filedisplayContainer.style.display = 'block';
            uploadFileText.style.display = 'none';
        }

        // MSWORD file file display preview
        async function processDocx(file) {
            const reader = new FileReader();
            reader.onload = async function (e) {
                const arrayBuffer = e.target.result;
                const result = await mammoth.convertToHtml({ arrayBuffer });
                const html = result.value;
                displayHtmlPreview(html);
            };
            reader.readAsArrayBuffer(file);
        }

        // EXCEL file display preview
        async function processXlsx(file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                const data = e.target.result;
                const workbook = XLSX.read(data, { type: 'binary' });
                const firstSheet = workbook.Sheets[workbook.SheetNames[0]];

                // Extract text from the first sheet
                const extractedText = extractTextFromSheet(firstSheet);

                // Display HTML preview
                const html = XLSX.utils.sheet_to_html(firstSheet, { editable: true });
                displayHtmlPreview(html);

                // Update OCR text area with extracted text
                document.getElementById('<%= ocrTextArea.ClientID %>').innerText = extractedText;
            };
            reader.readAsBinaryString(file);
        }

        function extractTextFromSheet(sheet) {
            let extractedText = '';
            for (let key in sheet) {
                if (sheet.hasOwnProperty(key)) {
                    if (key.startsWith('A') || key.startsWith('B') || key.startsWith('C')) { // Adjust column range as needed
                        const cell = sheet[key];
                        if (cell && cell.v) {
                            extractedText += cell.v + ' ';
                        }
                    }
                }
            }
            return extractedText;
        }

        function displayHtmlPreview(html) {
            const filedisplayContainer = document.getElementById('<%= filedisplayContainer.ClientID %>');
            filedisplayContainer.innerHTML = `<div class="html-preview">${html}</div>`;
            filedisplayContainer.style.display = 'block';
            uploadFileText.style.display = 'none';
        }

        // TXT file display preview
        async function processTxt(file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                const text = e.target.result;
                displayTextPreview(text);
            };
            reader.readAsText(file);
        }

        function displayTextPreview(text) {
            const filedisplayContainer = document.getElementById('<%= filedisplayContainer.ClientID %>');
            filedisplayContainer.innerHTML = `<pre>${text}</pre>`;
            filedisplayContainer.style.display = 'block';
            uploadFileText.style.display = 'none';
        }

        async function convertPDFToImages11(file) {
            const pdf = await pdfjsLib.getDocument(URL.createObjectURL(file)).promise;
            const numPages = pdf.numPages;
            async function* images() {
                for (let i = 1; i <= numPages; i++) {
                    try {
                        const page = await pdf.getPage(i);
                        const viewport = page.getViewport({ scale: 1 });
                        const canvas = document.createElement('canvas');
                        const context = canvas.getContext('2d');
                        const desiredWidth = 800; // Adjust as needed
                        canvas.width = desiredWidth;
                        canvas.height = (desiredWidth / viewport.width) * viewport.height;
                        const renderContext = {
                            canvasContext: context,
                            viewport: page.getViewport({ scale: desiredWidth / viewport.width }),
                        };
                        await page.render(renderContext).promise;
                        const imageURL = canvas.toDataURL('image/jpeg', 0.8);
                        yield { imageURL };
                    } catch (error) {
                        console.error(`Error rendering page ${i}:`, error);
                    }
                }
            }
            return { numPages, imageIterator: images() };
        }

        // Initialize Tesseract
        (async function initializeTesseract() {
            tesseractWorker = await Tesseract.createWorker();
        })();
        /*End Upload*/
    </script>
</asp:Content>
