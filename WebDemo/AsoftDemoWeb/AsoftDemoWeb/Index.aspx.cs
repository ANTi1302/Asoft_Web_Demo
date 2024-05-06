using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;
using AsoftDemoWeb.AddData;
using System.Web.Services.Description;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Reflection.Emit;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.DynamicData;
using System.Xml.Linq;

namespace AsoftDemoWeb
{
    public partial class Index : System.Web.UI.Page
    {
        public List<NguoiDung> listNguoiDung;
        public bool isModalUpdate;
        public bool isShowModal;
        // Kết nối với database
        public string connectionString = "Server=DESKTOP;Database=Asoft_Demo;User Id=sa;Password=sapassword;";

        // Them NguoiDung
        public string insertQuery = "INSERT INTO NguoiDung (UserID, UserName, Password, Email, Tel, Disable) VALUES (@UserID, @UserName, @Password, @Email, @Tel, @Disable)";

        //////////////////

        protected void Page_Init(object sender, EventArgs e)
        {

            var db = new Asoft_DemoEntities1();
            var nguoiDung = db.NguoiDungs;
            listNguoiDung = new List<NguoiDung>();
            listNguoiDung = nguoiDung.ToList();

            // Tạo bảng và các cột tiêu đề (Table1 là id bên asp:Table
            Table1.Rows.Clear(); // Xóa bảng cũ (nếu có)
            TableHeaderRow headerRow = new TableHeaderRow();
            headerRow.Cells.Add(CreateHeaderCell("#"));
            headerRow.Cells.Add(CreateHeaderCell("Mã nhân viên"));
            headerRow.Cells.Add(CreateHeaderCell("Tên nhân viên"));
            headerRow.Cells.Add(CreateHeaderCell("Mật khẩu"));
            headerRow.Cells.Add(CreateHeaderCell("Email"));
            headerRow.Cells.Add(CreateHeaderCell("Số điện thoại"));
            headerRow.Cells.Add(CreateHeaderCell("Thực hiện"));
            Table1.Rows.Add(headerRow);

            // Thêm dữ liệu từ danh sách người dùng vào bảng
            int index = 0;
            foreach (NguoiDung ng in listNguoiDung)
            {
                TableRow row = new TableRow();
                row.Cells.Add(CreateCell((index + 1).ToString()));
                row.Cells.Add(CreateCell(ng.UserID));
                row.Cells.Add(CreateCell(ng.UserName));
                row.Cells.Add(CreateCell(ng.Password));
                row.Cells.Add(CreateCell(ng.Email));
                row.Cells.Add(CreateCell(ng.Tel));

                // Thêm ô cuối cùng với hai nút bấm
                TableCell actionCell = CreateCell("");
                Button btnEdit = new Button();
                btnEdit.Text = "Sửa";
                btnEdit.CommandArgument = ng.UserID; // Thêm Argument vào nút Sửa
                btnEdit.CommandName = "edit";
                btnEdit.CssClass = "btn btn-primary";
                btnEdit.Command += BtnEdit_Command; // Thiết lập sự kiện cho nút Sửa
                actionCell.Controls.Add(btnEdit);

                Button btnDelete = new Button();
                btnDelete.Text = "Xóa";
                btnDelete.CommandArgument = ng.UserID;
                btnDelete.CommandName = "delete";
                btnDelete.CssClass = "btn btn-danger";
                btnDelete.Command += BtnDelete_Command; // Thiết lập sự kiện cho nút Delete
                btnDelete.OnClientClick = "return confirm('Bạn có muốn xóa " + ng.UserName  + " ?')";
                actionCell.Controls.Add(btnDelete);

                row.Cells.Add(actionCell);

                Table1.Rows.Add(row);
                index++;
            }
        }
        // Phương thức tạo cột
        private TableCell CreateCell(string text)
        {
            TableCell cell = new TableCell();
            cell.Text = text;
            cell.BorderStyle = BorderStyle.Solid;
            cell.BorderWidth = Unit.Pixel(1);
            cell.BorderColor = System.Drawing.Color.FromArgb(222, 226, 230);
            return cell;
        }
        // Phương thức tạo tiêu đề
        private TableHeaderCell CreateHeaderCell(string text)
        {
            TableHeaderCell cell = new TableHeaderCell();
            cell.Text = text;
            cell.BorderStyle = BorderStyle.Solid;
            cell.BorderWidth = Unit.Pixel(1);
            cell.BorderColor = System.Drawing.Color.FromArgb(222, 226, 230);
            return cell;
        }
        // Sự kiện nút Sửa
        protected void BtnEdit_Command(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string userID = (String)btn.CommandArgument;
            if (userID == null) { return; }

            var nguoiDung = FindNguoiDungByID(userID);

            // gọi hàm setData vào modal
            setDataModal(nguoiDung);
            updateButton.Attributes["data-user-id"] = userID;


            // Show the modal
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showModal", "showModal();", true);
        }
        // Sự kiện nút xóa
        protected void BtnDelete_Command(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string userID = (String)btn.CommandArgument;
            if (userID == null) { return; }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Truy vấn SQL xóa người dùng
                    string deleteQuery = "DELETE FROM NguoiDung WHERE UserID = @UserID";

                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        // Thiết lập tham số
                        command.Parameters.AddWithValue("@UserID", userID);

                        // Thực thi truy vấn
                        int rowsAffected = command.ExecuteNonQuery();

                        // Kiểm tra số dòng bị ảnh hưởng để xác nhận xóa thành công
                        if (rowsAffected > 0)
                        {
                            // Xóa thành công
                            Response.Redirect("Index.aspx");
                        }
                        else
                        {
                            // Không có dòng nào bị ảnh hưởng, có thể UserID không tồn tại
                            // hoặc không có gì để xóa
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ, ví dụ: ghi log, hiển thị thông báo lỗi
                Console.WriteLine("Lỗi: " + ex.Message);
            }
        }

           
        // Sự kiện nút cập nhật
        protected void BtnCapNhat_Command(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            var dataId = btn.Attributes["data-user-id"];
            NguoiDung nguoiDung = CapNhatNguoiDungTheoUserId(dataId, hoNV.Text, pwd.Text, emailNV.Text, phone.Text);

            // Show the modal
            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideModal", "hideModal();", true);
        }

        // Sự kiện nút đóng
        protected void onClickCloseModal(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideModal", "hideModal();", true);
        }
        // Sự kiện nút Thêm nhân viên
        protected void onCommandOpenModalAdd(object sender, EventArgs e)
        {
            // gọi hàm reset data để xóa các input nhập liệu
            resetData();
            // Thiết lập hiện nút Lưu
            reCreate.CssClass = "btn btn-outline-primary d-none";
            // Thiết lập ẩn nút Nhập lại 
            submitButton.Attributes.Remove("disabled");
            // Thiết lập isModalUpdate false để hiện nút Thêm
            isModalUpdate = false;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showModal", "showModal();", true);
        }

        private void setDataModal(NguoiDung nguoiDungDetail)
        {
            maNV.Text = nguoiDungDetail.UserID;
            // Thiết lập ẩn input maNV
            maNV.Attributes["disabled"] = "true";
            hoNV.Text = nguoiDungDetail.UserName;
            emailNV.Text = nguoiDungDetail.Email;
            pwd.Attributes["value"] = nguoiDungDetail.Password;
            pwd2.Attributes["value"] = nguoiDungDetail.Password;
            phone.Text = nguoiDungDetail.Tel;
            outputLabelPwdNV2.Text = "";
            outputLabelemailNV.Text = "";
            outputLabelHoNV.Text = "";
            outputLabelMaNV.Text = "";
            // Thiết lập hiện nút Cập nhật
            isModalUpdate = true;

        }
        private void resetData()
        {
            maNV.Text = "";
            // thiết lập hiện maNV bằng cách xóa disabled
            maNV.Attributes.Remove("disabled");
            hoNV.Text = "";
            emailNV.Text = "";
            pwd.Attributes["value"] = "";
            pwd2.Attributes["value"] = "";
            phone.Text = "";
            outputLabelPwdNV2.Text = "";
            outputLabelemailNV.Text = "";
            outputLabelHoNV.Text = "";
            outputLabelMaNV.Text = "";
        }

        // Phương thức cập nhật
        private NguoiDung CapNhatNguoiDungTheoUserId(string userID, string newUserName, string newPassword, string newEmail, string newTel)
        {
            NguoiDung updatedUser = null; // Khởi tạo người dùng cập nhật

            try
            {
                // Kết nối CSDL
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Truy vấn SQL cập nhật
                    string updateQuery = "UPDATE NguoiDung SET UserName = @UserName, Password = @Password, Email = @Email, Tel = @Tel, Disable = 1 WHERE UserID = @UserID";

                    // Tạo đối tượng SqlCommand
                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        // Thiết lập các tham số
                        command.Parameters.AddWithValue("@UserID", userID);
                        command.Parameters.AddWithValue("@UserName", newUserName);
                        command.Parameters.AddWithValue("@Password", newPassword);
                        command.Parameters.AddWithValue("@Email", newEmail);
                        command.Parameters.AddWithValue("@Tel", newTel);

     
                        // Thực thi truy vấn
                        int rowsAffected = command.ExecuteNonQuery();

                        // Kiểm tra số dòng bị ảnh hưởng để xác nhận cập nhật thành công
                        if (rowsAffected > 0)
                        {
                            // Cập nhật thành công, tạo đối tượng NguoiDung mới
                            updatedUser = new NguoiDung
                            {
                                UserID = userID,
                                UserName = newUserName,
                                Password = newPassword,
                                Email = newEmail,
                                Tel = newTel
                            };
                        }
                        // Không cần xử lý trường hợp rowsAffected == 0 vì có thể người dùng không tồn tại hoặc không có gì thay đổi
                    }
                }

                // Chuyển hướng người dùng đến trang Index.aspx
                Response.Redirect("Index.aspx");
            }
            catch (Exception ex)
            {
               
                Console.WriteLine("Lỗi: " + ex.Message);
            }

            return updatedUser; // Trả về đối tượng NguoiDung cập nhật, hoặc null nếu không có gì thay đổi hoặc có lỗi xảy ra
        }

        protected void onChangeConfirmPassword(object sender, EventArgs e)
        {
            string password = pwd.Text;
            string confirmPassword = pwd2.Text;
            if (password != confirmPassword)
            {
                outputLabelPwdNV2.Text = "Mật khẩu không khớp!";
                // Show the modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showModal", "showModal();", true);

            }
            else
            {
                outputLabelPwdNV2.Text = "";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showModal", "showModal();", true);

            }
        }


        /////////////////

        // phương thức thêm người dùng
        protected void createNguoiDung(object sender, EventArgs e)
        {
            string txtMaNV = maNV.Text.Trim().ToUpper();
            string txtHo = hoNV.Text;
            string txtEmail = emailNV.Text;
            string txtPass = pwd.Text;
            string txtTel = phone.Text;
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            bool isError = false;

            // Kiểm tra xem địa chỉ email khớp với biểu thức chính quy không
            Match match = Regex.Match(txtEmail, pattern);



            try
            {
               // Kiểm tra mã nhân viên rỗng

                if (string.IsNullOrWhiteSpace(txtMaNV))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showModal", "showModal();", true);
                    outputLabelMaNV.Text = "Mã nhân viên không được để trống.";
                    isError = true;


                }
                else
                {
                    // Kiểm tra mã người dùng tồn tại hay chưa
                    using (var db = new Asoft_DemoEntities1())
                    {
                        // trả số lượng người dùng được tìm thấy
                        var existingUser = db.NguoiDungs.FirstOrDefault(u => u.UserID == txtMaNV);

                        if (existingUser != null)
                        {
                            outputLabelMaNV.Text = "Mã người dùng đã tồn tại.";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "showModal", "showModal();", true);
                            isError = true;
                        }
                        else
                        {
                            outputLabelMaNV.Text = "";
                        }
                    }


                }
                // kiểm tra tên rỗng
                if (string.IsNullOrWhiteSpace(txtHo))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showModal", "showModal();", true);

                    outputLabelHoNV.Text = "Tên nhân viên không được để trống.";
                    isError = true;

                }
                else
                {
                    outputLabelHoNV.Text = "";
                }
                // kiểm tra email hợp lệ
                if (!match.Success)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showModal", "showModal();", true);
                    outputLabelemailNV.Text = "Email nhân viên không hợp lệ.";
                    isError = true;
                }
                else
                {
                    outputLabelemailNV.Text = "";
                }


                if (!isError)
                {
                    // Thêm người dùng mới
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand(insertQuery, connection))
                        {
                            // Thiết lập các parameters
                            command.Parameters.AddWithValue("@UserID", txtMaNV);
                            command.Parameters.AddWithValue("@UserName", txtHo);
                            command.Parameters.AddWithValue("@Password", txtPass);
                            command.Parameters.AddWithValue("@Email", txtEmail);
                            command.Parameters.AddWithValue("@Tel", txtTel);
                            command.Parameters.AddWithValue("@Disable", 1);

                            // Kết nối CSDL
                            connection.Open();
                            command.ExecuteNonQuery();
                            
                            // thiết lập mở nút Nhập lại
                            reCreate.CssClass = "btn btn-outline-primary";
                            // thiết lập ẩn nút Lưu
                            submitButton.Attributes["disabled"] = "true";
                        }

                    }
                }


            }
            catch (Exception ex)
            {

            }

        }
        

        protected void showUserDetails(object sender, EventArgs e)
        {
            // Access the user details passed from the button
            var userID = Request.Form["userID"];
            var userName = Request.Form["userName"];
            var password = Request.Form["password"];
            var email = Request.Form["email"];
            var tel = Request.Form["tel"];

            // Set the values of the modal fields with the user details
            maNV.Text = userID;
            hoNV.Text = userName;
            emailNV.Text = email;
            pwd.Text = password;

            phone.Text = tel;

            // Show the modal
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "$('#myModal').modal('show');", true);
        }

        // SQL tìm NguoiDung theo UserID
        public NguoiDung FindNguoiDungByID(string userID)
        {
            NguoiDung nguoiDung = null;

            // Query to find the nguoiDung by ID
            string query = "SELECT * FROM NguoiDung WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Set the parameter value
                    command.Parameters.AddWithValue("@UserID", userID);

                    // Open the connection
                    connection.Open();

                    // Execute the query
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Check if any rows were returned
                        if (reader.Read())
                        {
                            // Create a new nguoiDung object and populate its properties
                            nguoiDung = new NguoiDung();
                            nguoiDung.UserID = reader["UserID"].ToString();
                            nguoiDung.UserName = reader["UserName"].ToString();
                            nguoiDung.Password = reader["Password"].ToString();
                            nguoiDung.Email = reader["Email"].ToString();
                            nguoiDung.Tel = reader["Tel"].ToString();
                        }
                    }
                }
            }

            return nguoiDung;
        }

       
    }

        
}