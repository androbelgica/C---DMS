<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="DMS.MIS.WebForm2" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Text Recognition with Tesseract.js</title>
    <script src="https://cdn.jsdelivr.net/npm/tesseract.js@2.1.0/dist/tesseract.min.js"></script>
</head>
<body>
    <button id="open-camera-button">Open Camera</button>
    <button id="close-camera-button" style="display: none;">Close Camera</button>
    <video id="video" width="640" height="480" autoplay style="display: none;"></video>
    <canvas id="canvas" width="640" height="480" style="display: none;"></canvas>
    <button id="btnCaptureFileScan" style="display: none;" runat="server">Capture and Recognize Text</button>
    <div class="full-document-section <%--d-none--%>" id="ocrSection">
        <h2>Full document</h2>
        <textarea class="full-document" id="ocrTextArea" style="width: 100%; height: 500px;" runat="server"></textarea>
    </div>

    <script>
        let stream = null;

        // Function to open the camera
        document.getElementById('open-camera-button').addEventListener('click', function () {
            navigator.mediaDevices.getUserMedia({ video: true })
                .then(function (mediaStream) {
                    stream = mediaStream;
                    var video = document.getElementById('video');
                    video.srcObject = stream;
                    video.style.display = 'block';
                    document.getElementById('btnCaptureFileScan').style.display = 'block';
                    document.getElementById('close-camera-button').style.display = 'block';
                    document.getElementById('open-camera-button').style.display = 'none';
                    video.play();
                })
                .catch(function (err) {
                    console.error('Error accessing the camera: ', err);
                });
        });

        // Function to close the camera
        document.getElementById('close-camera-button').addEventListener('click', function () {
            if (stream) {
                let tracks = stream.getTracks();
                tracks.forEach(track => track.stop());
                stream = null;
            }
            document.getElementById('video').style.display = 'none';
            document.getElementById('btnCaptureFileScan').style.display = 'none';
            document.getElementById('close-camera-button').style.display = 'none';
            document.getElementById('open-camera-button').style.display = 'block';
        });

        // Capture the current frame from the video and perform OCR
        document.getElementById('btnCaptureFileScan').addEventListener('click', function () {
            var video = document.getElementById('video');
            var canvas = document.getElementById('canvas');
            var context = canvas.getContext('2d');

            context.drawImage(video, 0, 0, canvas.width, canvas.height);

            var imageData = canvas.toDataURL('image/png');

            Tesseract.recognize(
                imageData,
                'eng',
                { logger: m => console.log(m) }
            ).then(({ data: { text } }) => {
                // Display recognized text in the textarea
                var ocrTextArea = document.getElementById('<%= ocrTextArea.ClientID %>');
                if (ocrTextArea) {
                    ocrTextArea.value = text;
                } else {
                    alert('OCR text area not found.');
                }
            }).catch(error => {
                console.error('Error during OCR:', error);
            });
        });
    </script>
</body>
</html>
