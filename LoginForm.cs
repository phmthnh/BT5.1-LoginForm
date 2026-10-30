using System;
using System.Windows.Forms;
using BT5_1_LoginForm.Data;
using Microsoft.Data.SqlClient;

namespace BT5_1_LoginForm
{
    public partial class LoginForm : Form
    {
        private readonly UserRepository users = new();
        private readonly ErrorProvider errorProvider;
        private bool databaseReady;

        public LoginForm()
        {
            InitializeComponent();
            components ??= new System.ComponentModel.Container();
            // Khởi tạo lúc chạy để tránh lỗi thuộc tính mở rộng của ErrorProvider trong Designer.
            errorProvider = new ErrorProvider(components) { ContainerControl = this };
            txtUsername.MaxLength = 100;
            btnLogin.Enabled = false;
            Shown += LoginForm_Shown;
        }

        private async void LoginForm_Shown(object? sender, EventArgs e)
        {
            try
            {
                await users.InitializeAsync();
                databaseReady = true;
                if (!IsDisposed) btnLogin.Enabled = true;
            }
            catch (Exception ex) when (ex is SqlException or IOException or UnauthorizedAccessException or ArgumentException)
            {
                if (!IsDisposed)
                    MessageBox.Show(this, "Không thể mở cơ sở dữ liệu đăng nhập. Kiểm tra SQL Server và chuỗi kết nối rồi mở lại ứng dụng.",
                        "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sự kiện click nút Đăng Nhập
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (!databaseReady || !btnLogin.Enabled) return;
            bool isValid = true;

            // Validate Tên đăng nhập
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                errorProvider.SetError(txtUsername, "Vui lòng nhập tên đăng nhập!");
                isValid = false;
            }
            else
            {
                errorProvider.SetError(txtUsername, "");
            }

            // Validate Mật khẩu
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                errorProvider.SetError(txtPassword, "Vui lòng nhập mật khẩu!");
                isValid = false;
            }
            else
            {
                errorProvider.SetError(txtPassword, "");
            }

            if (!isValid) return;

            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;
            btnLogin.Enabled = false;
            txtUsername.Enabled = txtPassword.Enabled = false;
            try
            {
                var authenticated = await users.AuthenticateAsync(username, password);
                if (IsDisposed) return;
                if (!authenticated)
                {
                    errorProvider.SetError(txtPassword, "Tên đăng nhập hoặc mật khẩu không đúng!");
                    txtPassword.Clear();
                    return;
                }

                MessageBox.Show(this,
                    $"Đăng nhập thành công!\nTài khoản: {username}",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex) when (ex is SqlException or IOException or UnauthorizedAccessException or ArgumentException)
            {
                if (!IsDisposed)
                    MessageBox.Show(this, "Không thể đọc cơ sở dữ liệu đăng nhập. Vui lòng thử lại.",
                        "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed)
                {
                    btnLogin.Enabled = databaseReady;
                    txtUsername.Enabled = txtPassword.Enabled = true;
                    txtPassword.Focus();
                }
            }
        }

        // Sự kiện click nút Thoát
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Checkbox hiện/ẩn mật khẩu
        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            // Tick → hiện mật khẩu, bỏ tick → ẩn (*)
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }
    }
}
