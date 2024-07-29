<%@ Page Title="" Language="C#" MasterPageFile="~/GC/gc_topnav.Master" AutoEventWireup="true" CodeBehind="SearchGC.aspx.cs" Inherits="DMS.GC.SearchGC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.2/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <link rel="stylesheet" type="text/css" href="../CSS/myfiles.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="main">
        <!-- Page Heading -->
        <div class="d-sm-flex align-items-center justify-content-between heading">
            <h1 class="h1 mb-0 text-gray-800">Gift Cerificate</h1>
        </div>

        <div class="row">
            <div class="col-xl-6 mt-4">
                <div class="card">
                    <div class="card-body shadow">
                        <div class="form-group mb-2">
                            <label for="txtGiftCode">Gift Code:</label>
                            <asp:TextBox ID="txtGiftCode" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvGiftCode" runat="server" ControlToValidate="txtGiftCode" ErrorMessage="Gift Code is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revGiftCode" runat="server" ControlToValidate="txtGiftCode" ErrorMessage="Invalid Gift Code format." ValidationExpression="^[A-Z0-9-]+$" CssClass="text-danger" Display="Dynamic"></asp:RegularExpressionValidator>
                        </div>
                        <div class="form-group mb-2">
                            <label for="txtTo">To:</label>
                            <asp:TextBox ID="txtTo" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvTo" runat="server" ControlToValidate="txtTo" ErrorMessage="Recipient is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group mb-2">
                            <label for="txtEntitlement">Entitlement:</label>
                            <asp:TextBox ID="txtEntitlement" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEntitlement" runat="server" ControlToValidate="txtEntitlement" ErrorMessage="Entitlement is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group mb-2">
                            <label for="txtDateOfIssue">Date of Issue:</label>
                            <asp:TextBox ID="txtDateOfIssue" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDateOfIssue" runat="server" ControlToValidate="txtDateOfIssue" ErrorMessage="Date of Issue is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="cvDateOfIssue" runat="server" ControlToValidate="txtDateOfIssue" Operator="DataTypeCheck" Type="Date" ErrorMessage="Invalid date format." CssClass="text-danger" Display="Dynamic"></asp:CompareValidator>
                        </div>
                        <div class="form-group mb-2">
                            <label for="txtValidity">Validity:</label>
                            <asp:TextBox ID="txtValidity" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvValidity" runat="server" ControlToValidate="txtValidity" ErrorMessage="Validity is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="cvValidity" runat="server" ControlToValidate="txtValidity" Operator="DataTypeCheck" Type="Date" ErrorMessage="Invalid date format." CssClass="text-danger" Display="Dynamic"></asp:CompareValidator>
                        </div>


                        <div class="form-group text-center">
                            <asp:Button ID="btnScanQRCode" runat="server" Text="Scan QR Code" OnClick="btnScanQRCode_Click" CausesValidation="false" />
                            <asp:Button ID="btnGenerate" runat="server" Text="Generate QR Code" CssClass="btn btn-primary btn-generate" OnClick="btnGenerate_Click" />
                            <asp:Button ID="btnSearchGC" runat="server" Text="Search GC" CssClass="btn btn-info btn-search" OnClick="btnSearchGC_Click" ValidationGroup="searchGC" />
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="searchGC" CssClass="text-danger" />
                            
                            
                        </div>
                        <%--<div class="form-group text-center qr-code">
                        </div>--%>

                        <script src="https://cdn.jsdelivr.net/npm/@zxing/library@latest"></script>

                    </div>
                </div>
            </div>

            <div class="col-xl-6 col-md-6 mt-4" id="qrDiv" runat="server">
                <div class="card shadow">
                    <div class="form-group text-center qr-code">
                        <div id="scanner-container"></div>
                        <div class="qr-code-container">
                            <asp:Image ID="imgQRCode" runat="server" CssClass="qr-code-img" />
                            <%--<asp:Image ID="Image1" runat="server" CssClass="qr-code-img" />--%>
                            <%--<img src="image/logo.png" alt="Logo" class="qr-code-logo" />--%>
                            <div class="form-group text-center">
                                <asp:Button ID="btnGeneratePDF" runat="server" Text="Generate PDF" OnClick="btnGeneratePDF_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


    </div>
</asp:Content>
