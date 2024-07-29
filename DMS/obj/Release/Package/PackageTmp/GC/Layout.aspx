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
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <!-- Content specific to Layout.aspx -->
    <div class="container">
        <h1>Download Layout Document</h1>
        <div>
            <h1>Download Word File</h1>
            <asp:Button ID="btnDownload" runat="server" Text="Download Word File" OnClick="btnDownload_Click" />
        </div>
    </div>
</asp:Content>
