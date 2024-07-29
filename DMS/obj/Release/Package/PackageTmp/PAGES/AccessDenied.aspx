<%@ Page Title="" Language="C#" MasterPageFile="~/PAGES/topnav.Master" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="DMS.PAGES.AccessDenied" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            text-align: center;
            padding-top: 50px;
        }
        .container {
            background-color: #fff;
            border-radius: 5px;
            padding: 20px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            width: 400px;
            margin: 0 auto;
        }
        h2 {
            color: #ff6347;
        }
        .btn{
            background-color: #D4AF37;
            color: #fff;
                transition: background-color 0.3s ease, transform 0.3s ease;

        }
        .btn:hover{
            background-color: #b08d26;
            color: #fff;
            transform: scale(1.1);
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <div class="container">
        <h2>Access Denied</h2>
        <p>You don't have the permission to access this page or action.</p>
        <asp:Button ID="GoBackButton" runat="server" Text="Go Back to Dashboard" OnClick="GoBackButton_Click" CssClass="btn" />
    </div>
</asp:Content>