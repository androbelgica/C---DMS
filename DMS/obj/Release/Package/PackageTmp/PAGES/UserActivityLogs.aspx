<%@ Page Title="" Language="C#" MasterPageFile="~/PAGES/topnav.Master" AutoEventWireup="true" CodeBehind="UserActivityLogs.aspx.cs" Inherits="DMS.PAGES.UserActivityLogs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Bootstrap CSS -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="../CSS/audit.css" />
    <link href='https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css' rel='stylesheet'>
    <link href="https://cdn.lineicons.com/4.0/lineicons.css" rel="stylesheet" />

    <!-- jQuery and Bootstrap JS -->
    <script rel="stylesheet" src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script rel="stylesheet" src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.16.0/umd/popper.min.js"></script>
    <script rel="stylesheet" src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>


    <!-- Bootstrap Datepicker CSS -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/css/bootstrap-datepicker.min.css" rel="stylesheet">
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/js/bootstrap-datepicker.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="main">
        <!-- Page Heading -->
        <div class="heading">
            <div class="page-heading">User Activity Logs</div>
        </div>
        <%-- Table --%>
        <div class="custom-gridview-container shadow-sm">

            <div class="filterz">
                <div class="search-bar">
                    <asp:TextBox ID="searchtxtbox" runat="server" class="form-control form-control-sm search-textbox" placeholder="Search"
                        aria-label="search" aria-describedby="search" AutoPostBack="true" OnTextChanged="searchtxtbox_TextChanged"></asp:TextBox>
                </div>
                <div class="table-filters">
                    <div class="date-activity">
                        <%-- Calendar --%>
                        <div class="calendar ml-1">
                            <input type="text" class="form-control form-control-sm filter-date" id="datepicker" placeholder="Filter by Date">
                            <asp:HiddenField ID="hfSelectedDate" runat="server" />
                            <asp:HiddenField ID="hfIsDateFilterApplied" runat="server" />
                        </div>
                        <%-- Activity Filter --%>
                        <div class="btn-container">
                            <asp:DropDownList ID="ddlUserAct" runat="server" CssClass="btn dropdown-toggle custom-dropdown" AutoPostBack="true"
                                data-bs-toggle="dropdown" aria-expanded="false" OnSelectedIndexChanged="activityFilter_SelectedIndexChanged">
                                <asp:ListItem Value="">Filter by Activity</asp:ListItem>
                                <asp:ListItem Value="Login">Login</asp:ListItem>
                                <asp:ListItem Value="Reset Password">Reset Password</asp:ListItem>
                                <asp:ListItem Value="OTP Sent to Email">OTP</asp:ListItem>
                                <asp:ListItem Value="Account Added">Add Account</asp:ListItem>
                                <asp:ListItem Value="Updated Account">Update Account</asp:ListItem>
                                <asp:ListItem Value="Added Department">Add Department</asp:ListItem>
                                <asp:ListItem Value="Updated Department">Update Department</asp:ListItem>
                                <asp:ListItem Value="Updated Access">Grant Access</asp:ListItem>
                            </asp:DropDownList>

                        </div>
                    </div>
                    <div class="status-reset">
                        <%-- Status Filter --%>
                        <div class="btn-container">
                            <asp:DropDownList ID="ddlUserStatus" runat="server" CssClass="btn dropdown-toggle custom-dropdown" AutoPostBack="true"
                                data-bs-toggle="dropdown" aria-expanded="false" OnSelectedIndexChanged="statusFilter_SelectedIndexChanged">
                                <asp:ListItem Value="">Filter by Status</asp:ListItem>
                                <asp:ListItem Value="Successful">Successful</asp:ListItem>
                                <asp:ListItem Value="Unsuccessful">Unsuccessful</asp:ListItem>
                            </asp:DropDownList>

                        </div>

                        <div>
                            <asp:Button ID="Button1" runat="server" Text="Reset Filters" CssClass="btn reset-btn" OnClick="ResetFilterBtn_Click" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="table-responsive">
                <asp:GridView ID="GridView1" runat="server" EnableViewState="true" AutoGenerateColumns="False"
                    CssClass="custom-gridview" PageSize="20" AllowPaging="true" OnRowDataBound="GridView1_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                        <asp:BoundField DataField="UserLogDateTime" HeaderText="Date & Time" SortExpression="UserLogDateTime" />
                        <asp:BoundField DataField="Activity" HeaderText="Activity" SortExpression="Activity" />
                        <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                    </Columns>
                    <PagerSettings Visible="false" />
                    <EmptyDataTemplate>
                        <div class="alert alert-info" role="alert">
                            No results found.
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
            <!--PAGINATION-->
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

                        <%--<asp:Button ID="btnGoToPage" runat="server" CssClass="btn page-btn" Text="Go" OnClick="btnGoToPage_Click" />--%>



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
    <script type="text/javascript">
        $(document).ready(function () {
            $('#datepicker').datepicker({
                format: 'mm/dd/yyyy',
                todayHighlight: true,
                autoclose: true
            }).on('changeDate', function (e) {
                var selectedDate = $('#datepicker').datepicker('getFormattedDate', 'yyyy-mm-dd');
                $('#<%= hfSelectedDate.ClientID %>').val(selectedDate);
                __doPostBack();
            });
        });
    </script>
</asp:Content>
