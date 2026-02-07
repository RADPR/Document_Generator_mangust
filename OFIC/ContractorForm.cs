using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace OFIC
{
    public partial class ContractorForm : Form
    {
        private int contractorId = -1;
        private string currentContractorType = "";
        private bool isFormLoading = true; // Флаг загрузки формы

        // Цветовая схема
        private readonly Color PrimaryColor = Color.FromArgb(58, 88, 177);
        private readonly Color SecondaryColor = Color.FromArgb(240, 245, 255);
        private readonly Color SuccessColor = Color.FromArgb(46, 204, 113);

        public ContractorForm()
        {
            InitializeComponent();
            InitializeModernDesign();
            SetupEventHandlers();
            InitializeDynamicPanels();
            LoadFormData();
            SubscribePanelPaintEvents();

            lblTitle.Text = contractorId == -1 ? "НОВЫЙ КОНТРАГЕНТ" : "РЕДАКТИРОВАНИЕ КОНТРАГЕНТА";
            SetFormIcon();

            PositionButtons(); // Позиционируем кнопки сразу
        }

        public ContractorForm(int id) : this()
        {
            contractorId = id;
            LoadContractorData(id);
        }

        #region Инициализация и настройка формы

        private void InitializeModernDesign()
        {
            // Увеличиваем размер формы для лучшего размещения элементов
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(1200, 800);
            this.MinimumSize = new Size(1200, 800);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Стилизация заголовка
            panelHeader.BackColor = PrimaryColor;
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Segoe UI Semibold", 14);

            // Стилизация кнопок - делаем их видимыми сразу
            StyleButton(btnSave, SuccessColor, "Сохранить", true);
            StyleButton(btnCancel, Color.FromArgb(108, 117, 125), "Отмена", false);

            // Устанавливаем видимость кнопок сразу
            btnSave.Visible = true;
            btnCancel.Visible = true;

            // Стилизация панелей
            StylePanel(panelMainFields);
            StylePanel(panelOOO);
            StylePanel(panelIP);
            StylePanel(panelFL);
        }

        private void StyleButton(Button button, Color backColor, string text, bool isBold)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.BackColor = backColor;
            button.ForeColor = Color.White;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 10, isBold ? FontStyle.Bold : FontStyle.Regular);
            button.Height = 40;
            button.Width = 120;
            button.Text = text;
            button.Cursor = Cursors.Hand;
        }

        private void StylePanel(Panel panel)
        {
            panel.BackColor = SecondaryColor;
            panel.BorderStyle = BorderStyle.None;
        }

        private void SubscribePanelPaintEvents()
        {
            panelMainFields.Paint += Panel_Paint;
            panelOOO.Paint += Panel_Paint;
            panelIP.Paint += Panel_Paint;
            panelFL.Paint += Panel_Paint;
        }

        private void SetFormIcon()
        {
            this.Icon = contractorId == -1
                ? SystemIcons.Information
                : SystemIcons.Application;
        }

        private void SetupEventHandlers()
        {
            cmbContractorType.SelectedIndexChanged += (s, e) =>
            {
                currentContractorType = cmbContractorType.SelectedItem?.ToString() ?? "";
                ShowRelevantPanels();
                PositionButtons();
            };

            // УБИРАЕМ вызов AutoFillAddress при изменении типа здания
            // cmbBuildingType.SelectedIndexChanged += (s, e) => AutoFillAddress();

            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
        }

        #endregion

        #region Динамические панели и поля

        private void InitializeDynamicPanels()
        {
            CreateOOOFields();
            CreateIPFields();
            CreateFLFields();
            HideTypeSpecificPanels();
        }

        private void CreateOOOFields()
        {
            int x = 20, y = 40;
            int labelWidth = 100;
            int controlWidth = 280;
            int spacing = 35;
            int columnSpacing = 40;

            panelOOO.Height = 4 * spacing + 70;

            string[,] fields =
            {
                { "ФИО отв.:", "txtOOO_ResponsiblePerson", "ФИО" },
                { "ОГРН:", "txtOOO_OGRN", "13 цифр" },
                { "ИНН/КПП:", "txtOOO_INN_KPP", "10/9 цифр" },
                { "Рас. счет:", "txtOOO_PaymentAccount", "20 цифр" },
                { "Банк:", "txtOOO_BankName", "Наименование банка" },
                { "БИК:", "txtOOO_BIK", "9 цифр" },
                { "Корр. счет:", "txtOOO_CorrespondentAccount", "20 цифр" }
            };

            for (int i = 0; i < fields.GetLength(0); i++)
            {
                int currentRow, currentColumn;

                if (i < 6)
                {
                    currentRow = i / 2;
                    currentColumn = i % 2;
                }
                else
                {
                    currentRow = 3;
                    currentColumn = 0;
                }

                int currentX = x + (currentColumn * (labelWidth + controlWidth + columnSpacing));
                int currentY = y + (currentRow * spacing);

                if (i == 6)
                {
                    CreateLabel(panelOOO, currentX, currentY, fields[i, 0], labelWidth);
                    int wideControlWidth = (labelWidth + controlWidth + columnSpacing) + controlWidth - 10;
                    CreateTextBox(panelOOO, currentX + labelWidth + 10, currentY,
                        fields[i, 1], wideControlWidth, fields[i, 2]);
                }
                else
                {
                    CreateLabel(panelOOO, currentX, currentY, fields[i, 0], labelWidth);
                    CreateTextBox(panelOOO, currentX + labelWidth + 10, currentY,
                        fields[i, 1], controlWidth, fields[i, 2]);
                }
            }
        }

        private void CreateIPFields()
        {
            int x = 20, y = 40;
            int labelWidth = 100;
            int controlWidth = 280;
            int spacing = 35;
            int columnSpacing = 40;

            panelIP.Height = 3 * spacing + 70;

            string[,] fields =
            {
                { "Паспорт:", "txtIP_Passport", "Серия номер" },
                { "Кем выдан:", "txtIP_PassportIssuedBy", "Название органа" },
                { "Дата выдачи:", "txtIP_PassportIssueDate", "ДД.ММ.ГГГГ" },
                { "Код подразделения:", "txtIP_PassportDepartmentCode", "XXX-XXX" },
                { "ИНН:", "txtIP_PersonINN", "12 цифр" }
            };

            for (int i = 0; i < fields.GetLength(0); i++)
            {
                int column, row;

                if (i < 2)
                {
                    column = i;
                    row = 0;
                }
                else if (i < 4)
                {
                    column = i - 2;
                    row = 1;
                }
                else
                {
                    column = 0;
                    row = 2;
                }

                int currentX = x + (column * (labelWidth + controlWidth + columnSpacing));
                int currentY = y + (row * spacing);

                CreateLabel(panelIP, currentX, currentY, fields[i, 0], labelWidth);
                CreateTextBox(panelIP, currentX + labelWidth + 10, currentY,
                    fields[i, 1], controlWidth, fields[i, 2]);
            }
        }

        private void CreateFLFields()
        {
            int x = 20, y = 40;
            int labelWidth = 100;
            int controlWidth = 280;
            int spacing = 35;
            int columnSpacing = 40;

            panelFL.Height = 3 * spacing + 70;

            string[,] fields =
            {
                { "Паспорт:", "txtFL_Passport", "Серия номер" },
                { "Кем выдан:", "txtFL_PassportIssuedBy", "Название органа" },
                { "Дата выдачи:", "txtFL_PassportIssueDate", "ДД.ММ.ГГГГ" },
                { "Код подразделения:", "txtFL_PassportDepartmentCode", "XXX-XXX" },
                { "ИНН:", "txtFL_PersonINN", "12 цифр" }
            };

            for (int i = 0; i < fields.GetLength(0); i++)
            {
                int column, row;

                if (i < 2)
                {
                    column = i;
                    row = 0;
                }
                else if (i < 4)
                {
                    column = i - 2;
                    row = 1;
                }
                else
                {
                    column = 0;
                    row = 2;
                }

                int currentX = x + (column * (labelWidth + controlWidth + columnSpacing));
                int currentY = y + (row * spacing);

                CreateLabel(panelFL, currentX, currentY, fields[i, 0], labelWidth);
                CreateTextBox(panelFL, currentX + labelWidth + 10, currentY,
                    fields[i, 1], controlWidth, fields[i, 2]);
            }
        }

        private void CreateLabel(Panel panel, int x, int y, string text, int width)
        {
            Label label = new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 28),
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            panel.Controls.Add(label);
        }

        private TextBox CreateTextBox(Panel panel, int x, int y, string name, int width, string placeholder)
        {
            TextBox textBox = new TextBox
            {
                Name = name,
                Location = new Point(x, y),
                Size = new Size(width, 35),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Tag = placeholder
            };

            if (!string.IsNullOrEmpty(placeholder))
            {
                textBox.Text = placeholder;
                textBox.ForeColor = Color.Gray;

                textBox.Enter += (s, e) =>
                {
                    if (textBox.Text == placeholder.ToString())
                    {
                        textBox.Text = "";
                        textBox.ForeColor = Color.Black;
                    }
                };

                textBox.Leave += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        textBox.Text = placeholder.ToString();
                        textBox.ForeColor = Color.Gray;
                    }
                };
            }

            panel.Controls.Add(textBox);
            return textBox;
        }

        #endregion

        #region Управление отображением

        private void LoadFormData()
        {
            cmbContractorType.Items.AddRange(new[] { "ООО", "ИП", "ФЛ" });
            cmbPropertyType.Items.AddRange(new[] { "Аренда", "Собственность" });
            cmbSignalType.Items.AddRange(new[] { "ОС", "КТС", "ОС+КТС" });
            cmbBuildingType.Items.AddRange(new[] {
                "нежилое помещение", "дом", "коттедж", "квартира",
                "офис", "магазин", "склад", "производственное здание"
            });

            if (cmbContractorType.Items.Count > 0) cmbContractorType.SelectedIndex = 0;
            if (cmbPropertyType.Items.Count > 0) cmbPropertyType.SelectedIndex = 0;
            if (cmbSignalType.Items.Count > 0) cmbSignalType.SelectedIndex = 0;
            // НЕ УСТАНАВЛИВАЕМ выбранный индекс для cmbBuildingType при создании новой формы

            // Если это новая запись (не редактирование), то поле адреса должно быть пустым
            if (contractorId == -1)
            {
                txtAddress.Text = ""; // Очищаем поле адреса
                txtAddress.ForeColor = Color.Black; // Устанавливаем черный цвет
            }
        }

        private void ShowRelevantPanels()
        {
            HideTypeSpecificPanels();

            int panelY = panelMainFields.Location.Y + panelMainFields.Height + 30;

            switch (currentContractorType)
            {
                case "ООО":
                    panelOOO.Location = new Point(20, panelY);
                    panelOOO.Visible = true;
                    break;
                case "ИП":
                    panelIP.Location = new Point(20, panelY);
                    panelIP.Visible = true;
                    break;
                case "ФЛ":
                    panelFL.Location = new Point(20, panelY);
                    panelFL.Visible = true;
                    break;
            }

            PositionButtons();
        }

        private void HideTypeSpecificPanels()
        {
            panelOOO.Visible = false;
            panelIP.Visible = false;
            panelFL.Visible = false;
        }

        /// <summary>
        /// Правильное позиционирование кнопок
        /// </summary>
        private void PositionButtons()
        {
            // Определяем Y-позицию для кнопок
            int yPosition;

            if (panelOOO.Visible)
            {
                yPosition = panelOOO.Location.Y + panelOOO.Height + 30;
            }
            else if (panelIP.Visible)
            {
                yPosition = panelIP.Location.Y + panelIP.Height + 30;
            }
            else if (panelFL.Visible)
            {
                yPosition = panelFL.Location.Y + panelFL.Height + 30;
            }
            else
            {
                yPosition = panelOOO.Location.Y + panelOOO.Height + 30;
            }

            // Выравниваем кнопки по правому краю основной панели
            int panelRightEdge = panelMainFields.Location.X + panelMainFields.Width;

            btnCancel.Location = new Point(panelRightEdge - btnCancel.Width - 20, yPosition);
            btnSave.Location = new Point(panelRightEdge - btnCancel.Width - btnSave.Width - 40, yPosition);

            // Гарантируем видимость кнопок
            btnSave.Visible = true;
            btnCancel.Visible = true;

            // Обновляем форму, чтобы кнопки отобразились
            this.Refresh();
        }

        #endregion

        #region Вспомогательные методы

        // УБИРАЕМ метод AutoFillAddress или делаем его условным
        private void AutoFillAddress()
        {
            // Только если форма не в процессе загрузки и адрес действительно пустой
            if (!isFormLoading && string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                if (cmbBuildingType.SelectedItem != null)
                {
                    string buildingType = cmbBuildingType.SelectedItem.ToString();
                    txtAddress.Text = $"Адрес {buildingType.ToLower()}";
                    txtAddress.ForeColor = Color.Gray; // Сделаем серым, чтобы было видно, что это подсказка
                }
            }
        }

        private Control FindControl(string name)
        {
            Panel[] typePanels = { panelOOO, panelIP, panelFL };

            foreach (var panel in typePanels)
            {
                foreach (Control control in panel.Controls)
                {
                    if (control.Name == name)
                        return control;
                }
            }

            foreach (Control control in panelMainFields.Controls)
            {
                if (control.Name == name)
                    return control;
            }

            return null;
        }

        private string GetFieldValue(string controlName)
        {
            var control = FindControl(controlName);
            if (control == null) return string.Empty;

            if (control is TextBox textBox)
            {
                if (textBox.Tag != null && textBox.Text == textBox.Tag.ToString())
                    return string.Empty;

                return textBox.Text.Trim();
            }
            else if (control is ComboBox comboBox)
            {
                return comboBox.SelectedItem?.ToString() ?? string.Empty;
            }

            return string.Empty;
        }

        private void SetFieldValue(string controlName, object value)
        {
            var control = FindControl(controlName);
            if (control == null) return;

            string stringValue = value?.ToString() ?? string.Empty;

            if (control is TextBox textBox)
            {
                if (textBox.Tag != null && string.IsNullOrEmpty(stringValue))
                {
                    textBox.Text = textBox.Tag.ToString();
                    textBox.ForeColor = Color.Gray;
                }
                else
                {
                    textBox.Text = stringValue;
                    textBox.ForeColor = Color.Black;
                }
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.SelectedItem = stringValue;
            }
        }

        #endregion

        #region Загрузка и сохранение данных

        private void LoadContractorData(int id)
        {
            try
            {
                string query = "SELECT * FROM Contractors WHERE Id = @Id;";

                using (var conn = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LoadBasicFields(reader);
                            LoadSpecificFields(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBasicFields(SQLiteDataReader reader)
        {
            txtContractNumber.Text = reader["ContractNumber"]?.ToString() ?? "";
            cmbContractorType.Text = reader["ContractorType"]?.ToString() ?? "";
            txtFullName.Text = reader["FullName"]?.ToString() ?? "";
            cmbPropertyType.Text = reader["PropertyType"]?.ToString() ?? "";

            // Загружаем адрес из БД (может быть пустым)
            string address = reader["Address"]?.ToString() ?? "";
            txtAddress.Text = address;
            txtAddress.ForeColor = string.IsNullOrEmpty(address) ? Color.Gray : Color.Black;

            txtCadNumber.Text = reader["CadNumber"]?.ToString() ?? "";
            cmbSignalType.Text = reader["SignalType"]?.ToString() ?? "";

            if (reader["MonthlyAmount"] != DBNull.Value)
                txtMonthlyAmount.Text = reader["MonthlyAmount"].ToString();

            cmbBuildingType.Text = reader["BuildingType"]?.ToString() ?? "";
            txtEmail.Text = reader["Email"]?.ToString() ?? "";
            txtPhone.Text = reader["Phone"]?.ToString() ?? "";

            currentContractorType = reader["ContractorType"]?.ToString() ?? "";
            cmbContractorType.Text = currentContractorType;
        }

        private void LoadSpecificFields(SQLiteDataReader reader)
        {
            ShowRelevantPanels();

            switch (currentContractorType)
            {
                case "ООО":
                    LoadOOOFields(reader);
                    break;
                case "ИП":
                    LoadIPFields(reader);
                    break;
                case "ФЛ":
                    LoadFLFields(reader);
                    break;
            }
        }

        private void LoadOOOFields(SQLiteDataReader reader)
        {
            SetFieldValue("txtOOO_ResponsiblePerson", reader["ResponsiblePerson"]);
            SetFieldValue("txtOOO_OGRN", reader["OGRN"]);
            SetFieldValue("txtOOO_INN_KPP", reader["INN_KPP"]);
            SetFieldValue("txtOOO_PaymentAccount", reader["PaymentAccount"]);
            SetFieldValue("txtOOO_BankName", reader["BankName"]);
            SetFieldValue("txtOOO_BIK", reader["BIK"]);
            SetFieldValue("txtOOO_CorrespondentAccount", reader["CorrespondentAccount"]);
        }

        private void LoadIPFields(SQLiteDataReader reader)
        {
            SetFieldValue("txtIP_Passport", reader["Passport"]);
            SetFieldValue("txtIP_PassportIssuedBy", reader["PassportIssuedBy"]);
            SetFieldValue("txtIP_PassportIssueDate", reader["PassportIssueDate"]);
            SetFieldValue("txtIP_PassportDepartmentCode", reader["PassportDepartmentCode"]);
            SetFieldValue("txtIP_PersonINN", reader["PersonINN"]);
        }

        private void LoadFLFields(SQLiteDataReader reader)
        {
            SetFieldValue("txtFL_Passport", reader["Passport"]);
            SetFieldValue("txtFL_PassportIssuedBy", reader["PassportIssuedBy"]);
            SetFieldValue("txtFL_PassportIssueDate", reader["PassportIssueDate"]);
            SetFieldValue("txtFL_PassportDepartmentCode", reader["PassportDepartmentCode"]);
            SetFieldValue("txtFL_PersonINN", reader["PersonINN"]);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string query = contractorId == -1 ? GetInsertQuery() : GetUpdateQuery();

                using (var conn = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    AddParameters(cmd);

                    if (contractorId != -1)
                        cmd.Parameters.AddWithValue("@Id", contractorId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Данные успешно сохранены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetInsertQuery()
        {
            return @"
                INSERT INTO Contractors (
                    ContractNumber, ContractorType, FullName, PropertyType, 
                    Address, CadNumber, SignalType, MonthlyAmount, BuildingType,
                    Email, Phone, ResponsiblePerson, OGRN, INN_KPP, 
                    PaymentAccount, BankName, BIK, CorrespondentAccount,
                    Passport, PassportIssuedBy, PassportIssueDate, 
                    PassportDepartmentCode, PersonINN
                ) VALUES (
                    @ContractNumber, @ContractorType, @FullName, @PropertyType,
                    @Address, @CadNumber, @SignalType, @MonthlyAmount, @BuildingType,
                    @Email, @Phone, @ResponsiblePerson, @OGRN, @INN_KPP,
                    @PaymentAccount, @BankName, @BIK, @CorrespondentAccount,
                    @Passport, @PassportIssuedBy, @PassportIssueDate,
                    @PassportDepartmentCode, @PersonINN
                );";
        }

        private string GetUpdateQuery()
        {
            return @"
                UPDATE Contractors SET
                    ContractNumber = @ContractNumber,
                    ContractorType = @ContractorType,
                    FullName = @FullName,
                    PropertyType = @PropertyType,
                    Address = @Address,
                    CadNumber = @CadNumber,
                    SignalType = @SignalType,
                    MonthlyAmount = @MonthlyAmount,
                    BuildingType = @BuildingType,
                    Email = @Email,
                    Phone = @Phone,
                    ResponsiblePerson = @ResponsiblePerson,
                    OGRN = @OGRN,
                    INN_KPP = @INN_KPP,
                    PaymentAccount = @PaymentAccount,
                    BankName = @BankName,
                    BIK = @BIK,
                    CorrespondentAccount = @CorrespondentAccount,
                    Passport = @Passport,
                    PassportIssuedBy = @PassportIssuedBy,
                    PassportIssueDate = @PassportIssueDate,
                    PassportDepartmentCode = @PassportDepartmentCode,
                    PersonINN = @PersonINN
                WHERE Id = @Id;";
        }

        private void AddParameters(SQLiteCommand cmd)
        {
            AddBasicParameters(cmd);

            switch (currentContractorType)
            {
                case "ООО":
                    AddOOOParameters(cmd);
                    break;
                case "ИП":
                    AddIPParameters(cmd);
                    break;
                case "ФЛ":
                    AddFLParameters(cmd);
                    break;
            }
        }

        private void AddBasicParameters(SQLiteCommand cmd)
        {
            cmd.Parameters.AddWithValue("@ContractNumber", txtContractNumber.Text.Trim());
            cmd.Parameters.AddWithValue("@ContractorType", cmbContractorType.Text);
            cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
            cmd.Parameters.AddWithValue("@PropertyType", cmbPropertyType.Text);
            cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
            cmd.Parameters.AddWithValue("@CadNumber", txtCadNumber.Text.Trim());
            cmd.Parameters.AddWithValue("@SignalType", cmbSignalType.Text);

            if (decimal.TryParse(txtMonthlyAmount.Text.Replace(" ", ""), out decimal amount))
            {
                cmd.Parameters.AddWithValue("@MonthlyAmount", amount);
            }
            else
            {
                cmd.Parameters.AddWithValue("@MonthlyAmount", 0);
            }

            cmd.Parameters.AddWithValue("@BuildingType", cmbBuildingType.Text);
            cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
            cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
        }

        private void AddOOOParameters(SQLiteCommand cmd)
        {
            cmd.Parameters.AddWithValue("@ResponsiblePerson", GetFieldValue("txtOOO_ResponsiblePerson"));
            cmd.Parameters.AddWithValue("@OGRN", GetFieldValue("txtOOO_OGRN"));
            cmd.Parameters.AddWithValue("@INN_KPP", GetFieldValue("txtOOO_INN_KPP"));
            cmd.Parameters.AddWithValue("@PaymentAccount", GetFieldValue("txtOOO_PaymentAccount"));
            cmd.Parameters.AddWithValue("@BankName", GetFieldValue("txtOOO_BankName"));
            cmd.Parameters.AddWithValue("@BIK", GetFieldValue("txtOOO_BIK"));
            cmd.Parameters.AddWithValue("@CorrespondentAccount", GetFieldValue("txtOOO_CorrespondentAccount"));

            cmd.Parameters.AddWithValue("@Passport", DBNull.Value);
            cmd.Parameters.AddWithValue("@PassportIssuedBy", DBNull.Value);
            cmd.Parameters.AddWithValue("@PassportIssueDate", DBNull.Value);
            cmd.Parameters.AddWithValue("@PassportDepartmentCode", DBNull.Value);
            cmd.Parameters.AddWithValue("@PersonINN", DBNull.Value);
        }

        private void AddIPParameters(SQLiteCommand cmd)
        {
            cmd.Parameters.AddWithValue("@ResponsiblePerson", DBNull.Value);
            cmd.Parameters.AddWithValue("@OGRN", DBNull.Value);
            cmd.Parameters.AddWithValue("@INN_KPP", DBNull.Value);
            cmd.Parameters.AddWithValue("@PaymentAccount", DBNull.Value);
            cmd.Parameters.AddWithValue("@BankName", DBNull.Value);
            cmd.Parameters.AddWithValue("@BIK", DBNull.Value);
            cmd.Parameters.AddWithValue("@CorrespondentAccount", DBNull.Value);

            cmd.Parameters.AddWithValue("@Passport", GetFieldValue("txtIP_Passport"));
            cmd.Parameters.AddWithValue("@PassportIssuedBy", GetFieldValue("txtIP_PassportIssuedBy"));
            cmd.Parameters.AddWithValue("@PassportIssueDate", GetFieldValue("txtIP_PassportIssueDate"));
            cmd.Parameters.AddWithValue("@PassportDepartmentCode", GetFieldValue("txtIP_PassportDepartmentCode"));
            cmd.Parameters.AddWithValue("@PersonINN", GetFieldValue("txtIP_PersonINN"));
        }

        private void AddFLParameters(SQLiteCommand cmd)
        {
            cmd.Parameters.AddWithValue("@ResponsiblePerson", DBNull.Value);
            cmd.Parameters.AddWithValue("@OGRN", DBNull.Value);
            cmd.Parameters.AddWithValue("@INN_KPP", DBNull.Value);
            cmd.Parameters.AddWithValue("@PaymentAccount", DBNull.Value);
            cmd.Parameters.AddWithValue("@BankName", DBNull.Value);
            cmd.Parameters.AddWithValue("@BIK", DBNull.Value);
            cmd.Parameters.AddWithValue("@CorrespondentAccount", DBNull.Value);

            cmd.Parameters.AddWithValue("@Passport", GetFieldValue("txtFL_Passport"));
            cmd.Parameters.AddWithValue("@PassportIssuedBy", GetFieldValue("txtFL_PassportIssuedBy"));
            cmd.Parameters.AddWithValue("@PassportIssueDate", GetFieldValue("txtFL_PassportIssueDate"));
            cmd.Parameters.AddWithValue("@PassportDepartmentCode", GetFieldValue("txtFL_PassportDepartmentCode"));
            cmd.Parameters.AddWithValue("@PersonINN", GetFieldValue("txtFL_PersonINN"));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Графика и обработка событий

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                panelHeader.ClientRectangle,
                Color.FromArgb(58, 88, 177),
                Color.FromArgb(78, 108, 217),
                90F))
            {
                e.Graphics.FillRectangle(brush, panelHeader.ClientRectangle);
            }

            using (var pen = new Pen(Color.FromArgb(200, 220, 220, 220), 2))
            {
                e.Graphics.DrawLine(pen, 0, panelHeader.Height,
                    this.Width, panelHeader.Height);
            }
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel != null)
            {
                ControlPaint.DrawBorder(e.Graphics, panel.ClientRectangle,
                    Color.FromArgb(220, 220, 220), ButtonBorderStyle.Solid);
            }
        }

        private void ContractorForm_Load(object sender, EventArgs e)
        {

            // Плавное появление формы
            this.Opacity = 0;
            Timer fadeInTimer = new Timer { Interval = 10 };
            fadeInTimer.Tick += (s, args) =>
            {
                if (this.Opacity < 1)
                    this.Opacity += 0.05;
                else
                {
                    fadeInTimer.Stop();
                    isFormLoading = false; // Загрузка формы завершена
                }
            };
            fadeInTimer.Start();
        }

        #endregion
    }
}