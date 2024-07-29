<%@ Page Title="" Language="C#" MasterPageFile="~/GC/gc_topnav.Master" AutoEventWireup="true" CodeBehind="Layout.aspx.cs" Inherits="DMS.GC.Layout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Download Layout Page</title>
    <!-- Bootstrap CSS -->
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Custom CSS -->
    <style>
        body {
            background-color: #f8f9fa;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }

        .container {
            margin-top: 50px;
            display: flex;
            flex-direction: column;
            align-items: center;
        }

        .table thead th {
            background-color: #b8860b;
            color: white;
        }

        .table tbody tr:nth-child(even) {
            background-color: #f2f2f2;
        }

        .table tbody tr:hover {
            background-color: #d4af37;
        }

        h1 {
            color: #b8860b;
            text-align: center;
            margin-bottom: 20px;
        }

        .download-button {
            background-color: #B48C2C;
            color: #fff;
            border-radius: 3px;
            width: 100%;
            font-size: 15px;
        }

        .download-button:hover {
            background-color: #242424;
            color: #B48C2C;
        }

        .button-container {
            display: flex;
            justify-content: center;
            align-items: center;
            margin-bottom: 30px;
        }

        .steps {
            background-color: #fff;
            padding: 20px;
            border: 1px solid #ddd;
            border-radius: 5px;
            width: 80%;
            max-width: 600px;
        }

        .steps ol {
            padding-left: 20px;
        }

        .steps li {
            margin-bottom: 10px;
        }

        .steps h2 {
            color: #b8860b;
            text-align: center;
            margin-bottom: 20px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <!-- Content specific to Layout.aspx -->
    <div class="container">
        <h1>Download Layout Document and Word File</h1>
        <br />
        <div class="button-container">
            <asp:Button ID="btnDownload" runat="server" Text="Download Word File" class="btn btn-sm download-button" OnClick="btnDownload_Click" />
        </div>
        <br />
        <div class="steps">
            <h2>Steps to Follow</h2>
            <ol>
                <li>Click the "Download Word File" button.</li>
                <li>Wait for it to finish downloading.</li>
                <li>Open the 'layoutpub' file.</li>
                <li>Click 'Yes' to open the publication and access the data.</li>
                <li>Choose the 'Try to reconnect to the data source'.</li>
                <li>Find the 'GiftCertificates' excel file and click 'Open'.</li>
                <li>You can now edit the layout.</li>
            </ol>
        </div>
    </div>
</asp:Content>
