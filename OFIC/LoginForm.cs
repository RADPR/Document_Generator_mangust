using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OFIC
{
    public partial class LoginForm : Form
    {
        // Переменные для анимации
        private bool passwordVisible = false;
        private Timer errorTimer;
        private int shakeCounter = 0;

        public LoginForm()
        {
            InitializeComponent();
            InitializeModernDesign();
            SetupEventHandlers();
        }

        private void InitializeModernDesign()
        {
            // Настройка формы
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(400, 300);
            this.BackColor = Color.White;
            this.Text = "Вход в договорную систему";
            // this.Icon = Properties.Resources.AppIcon; // Раскомментируйте если есть иконка

            // Устанавливаем AcceptButton и CancelButton программно
            this.AcceptButton = btnLogin;
            this.CancelButton = btnCancel;
        }

        private void SetupEventHandlers()
        {
            // Поле пароля
            textBoxPassword.Enter += TextBoxPassword_Enter;
            textBoxPassword.Leave += TextBoxPassword_Leave;
            textBoxPassword.KeyDown += TextBoxPassword_KeyDown;

            // Кнопки
            btnLogin.MouseEnter += BtnLogin_MouseEnter;
            btnLogin.MouseLeave += BtnLogin_MouseLeave;
            btnCancel.MouseEnter += BtnCancel_MouseEnter;
            btnCancel.MouseLeave += BtnCancel_MouseLeave;

            // Кнопка показа пароля
            btnTogglePassword.Click += BtnTogglePassword_Click;

            // Включаем двойную буферизацию для формы
            this.DoubleBuffered = true;

            // Включаем двойную буферизацию для панелей через Reflection
            SetDoubleBuffered(panelHeader);
            SetDoubleBuffered(panelBody);
        }

        // Метод для включения двойной буферизации для любого контрола
        private void SetDoubleBuffered(Control control)
        {
            // Используем Reflection для доступа к protected свойству
            System.Reflection.PropertyInfo propertyInfo = typeof(Control).GetProperty(
                "DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            propertyInfo.SetValue(control, true, null);
        }

        #region События формы

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Плавное появление формы
            this.Opacity = 0;
            Timer fadeTimer = new Timer();
            fadeTimer.Interval = 10;
            fadeTimer.Tick += (s, args) =>
            {
                if (this.Opacity < 1.0)
                {
                    this.Opacity += 0.05;
                }
                else
                {
                    fadeTimer.Stop();
                    fadeTimer.Dispose();
                }
            };
            fadeTimer.Start();

            // Устанавливаем фокус на поле пароля
            textBoxPassword.Focus();
        }

        private void LoginForm_Paint(object sender, PaintEventArgs e)
        {
            // Градиентный фон для заголовка
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                panelHeader.ClientRectangle,
                Color.FromArgb(58, 88, 177),
                Color.FromArgb(78, 108, 197),
                90F))
            {
                e.Graphics.FillRectangle(brush, panelHeader.ClientRectangle);
            }

            // Тень под заголовком
            using (var pen = new Pen(Color.FromArgb(200, 200, 200), 1))
            {
                e.Graphics.DrawLine(pen, 0, panelHeader.Height,
                    this.Width, panelHeader.Height);
            }
        }

        #endregion

        #region События поля пароля

        private void TextBoxPassword_Enter(object sender, EventArgs e)
        {
            textBoxPassword.BackColor = Color.FromArgb(240, 245, 255);
            textBoxPassword.BorderStyle = BorderStyle.FixedSingle;
            labelPasswordTitle.ForeColor = Color.FromArgb(58, 88, 177);
        }

        private void TextBoxPassword_Leave(object sender, EventArgs e)
        {
            textBoxPassword.BackColor = Color.White;
            textBoxPassword.BorderStyle = BorderStyle.FixedSingle;
            labelPasswordTitle.ForeColor = Color.Gray;
        }

        private void TextBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && btnLogin.Enabled)
            {
                btnLogin.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        #endregion

        #region События кнопок

        private void BtnLogin_MouseEnter(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.FromArgb(48, 78, 167);
            btnLogin.FlatAppearance.BorderColor = Color.FromArgb(38, 68, 157);
        }

        private void BtnLogin_MouseLeave(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.FromArgb(58, 88, 177);
            btnLogin.FlatAppearance.BorderColor = Color.FromArgb(58, 88, 177);
        }

        private void BtnCancel_MouseEnter(object sender, EventArgs e)
        {
            btnCancel.BackColor = Color.FromArgb(220, 220, 220);
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(180, 180, 180);
        }

        private void BtnCancel_MouseLeave(object sender, EventArgs e)
        {
            btnCancel.BackColor = Color.WhiteSmoke;
            btnCancel.FlatAppearance.BorderColor = Color.Gray;
        }

        private void BtnTogglePassword_Click(object sender, EventArgs e)
        {
            passwordVisible = !passwordVisible;

            if (passwordVisible)
            {
                // Показываем пароль
                textBoxPassword.PasswordChar = '\0'; // Пустой символ
                textBoxPassword.UseSystemPasswordChar = false;
                btnTogglePassword.Text = "👁‍🗨";
            }
            else
            {
                // Скрываем пароль
                textBoxPassword.PasswordChar = '●'; // Символ точки
                textBoxPassword.UseSystemPasswordChar = false;
                btnTogglePassword.Text = "👁";
            }

            // Анимация кнопки
            ButtonAnimation(btnTogglePassword);
        }

        #endregion

        #region Основная логика входа

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string password = textBoxPassword.Text.Trim();

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Введите пароль", true);
                return;
            }

            // Блокируем интерфейс на время проверки
            SetControlsEnabled(false);
            btnLogin.Text = "Подключение к серверу...";

            try
            {
                // Проверяем доступность сервера
                btnLogin.Text = "Проверка соединения...";
                bool serverAvailable = await DatabaseHelper.CheckServerAvailability();

                if (!serverAvailable)
                {
                    ShowError("Сервер авторизации недоступен. Проверьте интернет-соединение.", false);
                    return;
                }

                // Используем асинхронную проверку пароля
                btnLogin.Text = "Проверка пароля...";
                bool isValid = await DatabaseHelper.VerifyPasswordAsync(password);

                if (isValid)
                {
                    // Успешный вход
                    btnLogin.BackColor = Color.FromArgb(40, 167, 69);
                    btnLogin.Text = "Успешно!";

                    await Task.Delay(500);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // Неверный пароль
                    ShowError("Неверный пароль", true);
                    textBoxPassword.Clear();
                    textBoxPassword.Focus();
                    ShakeForm();
                }
            }
            catch (Exception ex)
            {
                // Показываем детальную ошибку при проблемах с подключением
                ShowError($"Ошибка авторизации: {ex.Message}", true);
                System.Media.SystemSounds.Hand.Play();

                // Можно добавить логирование ошибки
                Console.WriteLine($"Ошибка входа: {ex}");
            }
            finally
            {
                SetControlsEnabled(true);
                btnLogin.Text = "Войти";
                btnLogin.BackColor = Color.FromArgb(58, 88, 177);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Вспомогательные методы

        private void ShowError(string message, bool animate)
        {
            labelError.Text = message;
            labelError.Visible = true;

            if (animate)
            {
                // Анимация появления ошибки
                labelError.ForeColor = Color.FromArgb(220, 53, 69);

                if (errorTimer != null)
                {
                    errorTimer.Dispose();
                }

                errorTimer = new Timer();
                errorTimer.Interval = 100;
                int blinkCount = 0;

                errorTimer.Tick += (s, args) =>
                {
                    blinkCount++;
                    labelError.Visible = !labelError.Visible;

                    if (blinkCount >= 6)
                    {
                        errorTimer.Stop();
                        labelError.Visible = true;
                    }
                };

                errorTimer.Start();

                // Звуковой сигнал
                System.Media.SystemSounds.Exclamation.Play();
            }
        }

        private void ShakeForm()
        {
            Point originalLocation = this.Location;
            int shakeAmount = 10;
            shakeCounter = 0;

            Timer shakeTimer = new Timer();
            shakeTimer.Interval = 20;

            shakeTimer.Tick += (s, args) =>
            {
                shakeCounter++;
                int offsetX = (shakeCounter % 2 == 0) ? shakeAmount : -shakeAmount;
                int offsetY = (shakeCounter % 4 < 2) ? 0 : 2;

                this.Location = new Point(
                    originalLocation.X + offsetX,
                    originalLocation.Y + offsetY
                );

                if (shakeCounter >= 10)
                {
                    shakeTimer.Stop();
                    this.Location = originalLocation;
                }
            };

            shakeTimer.Start();
        }

        private void ButtonAnimation(Button button)
        {
            Timer timer = new Timer();
            timer.Interval = 50;
            int originalHeight = button.Height;
            int pressedHeight = originalHeight - 3;

            timer.Tick += (s, args) =>
            {
                if (button.Height > pressedHeight)
                {
                    button.Height--;
                }
                else
                {
                    timer.Stop();

                    // Возвращаем исходный размер
                    Timer restoreTimer = new Timer();
                    restoreTimer.Interval = 30;

                    restoreTimer.Tick += (s2, args2) =>
                    {
                        if (button.Height < originalHeight)
                        {
                            button.Height++;
                        }
                        else
                        {
                            restoreTimer.Stop();
                        }
                    };

                    restoreTimer.Start();
                }
            };

            timer.Start();
        }

        private void SetControlsEnabled(bool enabled)
        {
            textBoxPassword.Enabled = enabled;
            btnLogin.Enabled = enabled;
            btnCancel.Enabled = enabled;
            btnTogglePassword.Enabled = enabled;

            Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
        }

        #endregion

        // Обработчики событий для дизайнера (оставляем пустыми если не используются)
        private void label1_Click(object sender, EventArgs e) { }
        private void panelHeader_Paint(object sender, PaintEventArgs e) { }
    }
}