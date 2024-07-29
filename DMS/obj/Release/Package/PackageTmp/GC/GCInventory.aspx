<%@ Page Title="" Language="C#" MasterPageFile="~/GC/gc_topnav.Master" AutoEventWireup="true" CodeBehind="GCInventory.aspx.cs" Inherits="DMS.GC.GCInventory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="../CSS/stored_gc.css" />
    <link href='https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css' rel='stylesheet'>
    <link href="https://cdn.lineicons.com/4.0/lineicons.css" rel="stylesheet" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Add ScriptManager here -->
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:HiddenField ID="HiddenField1" runat="server" />
    <div class="main">
        <!-- Page Heading -->
        <div class="heading">
            <div class="page-heading">Stored Gift Certificates</div>
        </div>

        <!-- Table -->
        <div class="custom-gridview-container">
            <div class="filterz">
                <div class="search-bar">
                    <asp:TextBox ID="searchtxtbox" runat="server" class="form-control form-control-sm search-textbox" placeholder="Search"
                        aria-label="search" aria-describedby="search" AutoPostBack="true" OnTextChanged="searchtxtbox_TextChanged">
                    </asp:TextBox>
                </div>
                <div class="table-filters">
                    <div class="status-reset">
                        <!-- Status Filter -->
                        <div class="btn-container">
                            <asp:DropDownList ID="ddlUserStatus" runat="server" CssClass="btn dropdown-toggle custom-dropdown" AutoPostBack="true"
                                data-bs-toggle="dropdown" aria-expanded="false" OnSelectedIndexChanged="statusFilter_SelectedIndexChanged">
                                <asp:ListItem Value="">Filter by Status</asp:ListItem>
                                <asp:ListItem Value="Available">Available</asp:ListItem>
                                <asp:ListItem Value="Booked">Booked</asp:ListItem>
                                <asp:ListItem Value="Cancelled">Cancelled</asp:ListItem>
                                <asp:ListItem Value="Lost">Lost</asp:ListItem>
                                <asp:ListItem Value="Replaced">Replaced</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div>
                            <asp:Button ID="Button1" runat="server" Text="Reset Filters" CssClass="btn reset-btn" OnClick="resetfilterBtn_Click" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="table-responsive">
                <asp:GridView ID="gvGiftCertificates" runat="server" AutoGenerateColumns="False" CssClass="custom-gridview" AllowPaging="true"
                    PageSize="20" OnRowDataBound="gvGiftCertificates_RowDataBound" OnRowCommand="gvGiftCertificates_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="GCNumber" HeaderText="GC Number" />
                        <asp:BoundField DataField="Recipient" HeaderText="Recipient" />
                        <asp:BoundField DataField="Entitlement" HeaderText="Entitlement" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:BoundField DataField="DateOfIssue" HeaderText="Date of Issue" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="Validity" HeaderText="Validity" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="GCType" HeaderText="GC Type" />
                        <asp:BoundField DataField="ChargeTo" HeaderText="Charge To" />
                        <asp:BoundField DataField="Status" HeaderText="Status" />
                        <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                        <asp:TemplateField HeaderText="QR Code">
                            <ItemTemplate>
                                <asp:Image ID="qrCodeImage" runat="server" ImageUrl='<%# GetImageUrl(Eval("QRCodeImage")) %>' Alt="QR Code" Style="width: 20px; height: 20px;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="BatchIdentifier" HeaderText="Batch Identifier" Visible="False" />
                    </Columns>
                    <PagerSettings Visible="false" />
                    <EmptyDataTemplate>
                        <div class="alert alert-info" role="alert">
                            No results found.
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
                <asp:HiddenField ID="hfBatchIdentifier" runat="server" />
            </div>

            <!-- Pagination -->
            <nav aria-label="Page navigation" class="custom-pagination">
                <ul class="pagination">
                    <div class="page-cont">
                        <span>Go to page:</span>
                        <asp:TextBox ID="txtPageNumber" runat="server" CssClass="form-control search-num" Width="30px" AutoPostBack="true" OnTextChanged="txtPageNumber_TextChanged"></asp:TextBox>
                        &nbsp;
                    </div>
                </ul>
                <ul class="pagination">
                    <div class="page-cont">
                        <li class="page-item">
                            <asp:LinkButton ID="btnPrev" runat="server" CssClass="page-link" OnClick="btnPrev_Click" aria-label="Previous">
                            <i class='bx bx-chevron-left'></i>
                            </asp:LinkButton>
                        </li>
                        <span class="page-of">Page
                         <asp:Label ID="lblPageNum" runat="server"></asp:Label>
                            of
                        <asp:Label ID="lblTotalPages" runat="server"></asp:Label>
                        </span>
                        <li class="page-item">
                            <asp:LinkButton ID="btnNext" runat="server" CssClass="page-link" OnClick="btnNext_Click" aria-label="Next">
                            <i class='bx bx-chevron-right'></i>
                            </asp:LinkButton>
                        </li>
                    </div>
                </ul>
            </nav>
        </div>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="batchModal" tabindex="-1" role="dialog" aria-labelledby="batchModalLabel" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog" role="document">
            <div class="modal-content">

                <div class="modal-body">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <br />
                    <br />
                    <div class="custom-gridview-container">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <asp:GridView ID="gvBatchDetails" runat="server" AutoGenerateColumns="False" CssClass="custom-gridview-modal" 
                                    AllowPaging="true" PageSize="10">
                                    <Columns>
                                        <asp:BoundField DataField="GCNumber" HeaderText="GC Number" />
                                        <%--<asp:BoundField DataField="Recipient" HeaderText="Recipient"/>--%>
                                        <asp:BoundField DataField="Entitlement" HeaderText="Entitlement" />
                                        <asp:BoundField DataField="Description" HeaderText="Description" />
                                        <asp:BoundField DataField="DateOfIssue" HeaderText="Date of Issue" DataFormatString="{0:yyyy-MM-dd}" />
                                        <asp:BoundField DataField="Validity" HeaderText="Validity" DataFormatString="{0:yyyy-MM-dd}" />
                                        <asp:BoundField DataField="GCType" HeaderText="GC Type" />
                                        <asp:BoundField DataField="ChargeTo" HeaderText="Charge To" />
                                        <asp:BoundField DataField="Status" HeaderText="Status" />
                                        <asp:TemplateField HeaderText="QR Code">
                                            <ItemTemplate>
                                                <asp:Image ID="qrCodeImage" runat="server" ImageUrl='<%# GetImageUrl(Eval("QRCodeImage")) %>' Alt="QR Code" Style="width: 20px; height: 20px;" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerSettings Visible="false" />
                                    <EmptyDataTemplate>
                                        <div class="alert alert-info" role="alert">
                                            No results found.                                 
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                                <div>
                                    <!-- Modal Pagination -->
                                    <nav aria-label="Page navigation" class="custom-pagination">
                                        <ul class="pagination">
                                            <div class="page-cont">
                                                <span>Go to page:</span>
                                                <asp:TextBox ID="txtPageNumberModal" runat="server" CssClass="form-control search-num" Width="30px" 
                                                    OnTextChanged="txtPageNumberModal_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                &nbsp;
                                            </div>
                                        </ul>
                                        <ul class="pagination">
                                            <div class="page-cont">
                                                <li class="page-item">
                                                    <asp:LinkButton ID="btnPrevModal" runat="server" CssClass="page-link" OnClick="btnPrevModal_Click" aria-label="Previous">
    <i class='bx bx-chevron-left'></i>
                                                    </asp:LinkButton>
                                                </li>
                                                <span class="page-of">Page
 <asp:Label ID="lblPageNumModal" runat="server"></asp:Label>
                                                    of
                                                    <asp:Label ID="lblTotalPagesModal" runat="server"></asp:Label>
                                                </span>
                                                <li class="page-item">
                                                    <asp:LinkButton ID="btnNextModal" runat="server" CssClass="page-link" OnClick="btnNextModal_Click" aria-label="Next">
    <i class='bx bx-chevron-right'></i>
                                                    </asp:LinkButton>
                                                </li>
                                            </div>
                                        </ul>
                                    </nav>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                </div>
            </div>
        </div>
    </div>


    <script type="text/javascript">
        $(document).ready(function () {
            // Open modal when clicking on a grid row
            $('.grid-row').click(function (e) {
                e.preventDefault();
                var batchIdentifier = $(this).data('batch-identifier');
                $('#<%= hfBatchIdentifier.ClientID %>').val(batchIdentifier);
                __doPostBack('<%= UpdatePanel1.ClientID %>', '');
            });

            // Prevent modal from closing when clicking outside or pressing the Escape key
            $('#batchModal').modal({
                backdrop: 'static',
                keyboard: falses
            });
        });

        // Function to show the modal from server-side code
        function showModal() {
            $('#batchModal').modal('show');
        }

        // Add event listener to the close button
        $('.close').click(function () {
            $('#batchModal').modal('hide');
        });
    </script>
</asp:Content>
