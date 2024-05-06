<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="AsoftDemoWeb.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
<head runat="server">
    <title>Demo</title>
</head>
<body>
    <script>
        function showModal() {
            $('#myModal').show();
        }

        function hideModal() {
            // Lấy danh sách tất cả các thẻ input trong modal và xóa giá trị của chúng
            $('#myModal input[type="text"]').val('');
            $('#myModal input[type="number"]').val('');
            $('#myModal input[type="password"]').val('');
            $('#myModal').hide();
        }

        function onChangeConfirmPassword() {
            let password = document.getElementById("pwd")?.value;
            let confirmPassword = document.getElementById("pwd2")?.value;
            let label = document.getElementById('outputLabelPwdNV2');
            if (password !== confirmPassword && label) {
                label.innerText = "Mật khẩu không khớp!";
            } else {
                label.innerText = '';
            }
        }

    </script>
    <div>
        <nav class="navbar navbar-expand-sm navbar-dark bg-dark">
            <div class="container-fluid">
                <a class="navbar-brand" href="javascript:void(0)">Asoft</a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#mynavbar">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="collapse navbar-collapse" id="mynavbar">
                    <ul class="navbar-nav me-auto">
                        <li class="nav-item">
                            <a class="nav-link" href="javascript:void(0)">Home</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="javascript:void(0)">Fetures</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="javascript:void(0)">Pricing</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="javascript:void(0)">About</a>
                        </li>
                        <li>
                            <div class="btn-group">
                              <button type="button" class="nav-link btn dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                                Dropdown
                              </button>
                              <ul class="dropdown-menu">
                                <li><a class="dropdown-item" href="#">Action</a></li>
                                <li><a class="dropdown-item" href="#">Another action</a></li>
                                <li><a class="dropdown-item" href="#">Something else here</a></li>
                                <li><hr class="dropdown-divider"></li>
                                <li><a class="dropdown-item" href="#">Separated link</a></li>
                              </ul>
                            </div>
                        </li>
                    </ul>
                    <form class="d-flex">
                        <input class="form-control me-2" type="text" placeholder="Search" />
                        <button class="btn btn-primary" type="button">Search</button>
                    </form>
                </div>
            </div>
        </nav>
        <br />
        <br />
        <form id="form2" runat="server">

            <asp:Button ID="openModalAdd" CssClass="btn btn-primary" runat="server" OnCommand="onCommandOpenModalAdd" Text="Thêm nhân viên" />

            <br />
            <br />

            <asp:Table ID="Table1"
                GridLines="None"
                BorderStyle="None"
                HorizontalAlign="Center"
                CellPadding="15"
                Width="100%"
                runat="server" />

            <div class="modal" id="myModal" runat="server">
                <div class="modal-dialog">
                    <div class="modal-content" style="max-height: 90vh; overflow: auto; box-shadow: 0 0 0 50vmax rgba(0,0,0,.5);">
                        <div class="modal-header">
                            <% if (isModalUpdate)
                             {%>
                            <h4 class="modal-title">Cập nhật nhân viên</h4>
                            <% } else {%>
                            <h4 class="modal-title">Thêm mới nhân viên</h4>
                            <% }%>
                            <asp:Button ID="closeModal1" CssClass="btn-close" runat="server" OnClick="onClickCloseModal" />

                        </div>
                        <div class="modal-body">
                            <div class="mb-3">
                                <label for="maNV" class="form-label">Mã nhân viên:</label>
                                <asp:TextBox ID="maNV" runat="server" class="form-control" placeholder="Mã nhân viên"></asp:TextBox>
                                <asp:Label ID="outputLabelMaNV" runat="server" Text="" class="text-danger"></asp:Label>
                            </div>
                            <div class="mb-3">
                                <label for="hoNV" class="form-label">Họ tên:</label>
                                <asp:TextBox ID="hoNV" runat="server" class="form-control" placeholder="Nhập họ"></asp:TextBox>
                                <asp:Label ID="outputLabelHoNV" runat="server" Text="" class="text-danger"></asp:Label>
                            </div>
                            <div class="mb-3">
                                <label for="emailNV" class="form-label">Email:</label>
                                <asp:TextBox ID="emailNV" runat="server" class="form-control" placeholder="Nhập email"></asp:TextBox>
                                <asp:Label ID="outputLabelemailNV" runat="server" Text="" class="text-danger"></asp:Label>
                            </div>
                            <div class="mb-3">
                                <label for="pwd" class="form-label">Mật khẩu:</label>
                                <asp:TextBox TextMode="Password" ID="pwd" runat="server" class="form-control" placeholder="Nhập mật khẩu"></asp:TextBox>
                            </div>
                            <div class="mb-3">
                                <label for="pwd2" class="form-label">Nhập lại mật khẩu:</label>
                                <asp:TextBox
                                    TextMode="Password"
                                    ID="pwd2"
                                    runat="server"
                                    class="form-control"
                                    placeholder="Nhập lại mật khẩu"
                                    OnTextChanged="onChangeConfirmPassword"
                                    AutoPostBack="false"
                                    onkeyup="onChangeConfirmPassword();"></asp:TextBox>
                                
                                <asp:Label ID="outputLabelPwdNV2" runat="server" Text="" class="text-danger"></asp:Label>
                            </div>
                            <div class="mb-3">
                                <label for="phone" class="form-label">Số điện thoại:</label>
                                <asp:TextBox TextMode="Number" ID="phone" runat="server" class="form-control" placeholder="Nhập số điện thoại"></asp:TextBox>
                                <asp:Label ID="outputLabelPhoneNV" runat="server" Text="" class="text-danger"></asp:Label>
                            </div>
                            <% if (isModalUpdate)
                                {%>
                            <asp:Button ID="updateButton" CssClass="btn btn-primary" runat="server" OnCommand="BtnCapNhat_Command" Text="Cập nhật" />
                            <% } else {%>
                            <asp:Button ID="submitButton" CssClass="btn btn-primary" runat="server" OnClick="createNguoiDung" Text="Thêm" />
                            <asp:Button ID="reCreate" CssClass="btn btn-outline-primary d-none" runat="server" OnClick="onCommandOpenModalAdd" Text="Nhập lại" />
                            <% }%>
                            <asp:Button ID="closeModal2" CssClass="btn btn-danger" runat="server" OnClick="onClickCloseModal" Text="Đóng" />
                        </div>
                    </div>
                </div>
            </div>
        </form>
    </div>
</body>
</html>
