using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using OfficeOpenXml;

namespace OFIC
{
    public partial class Form1 : Form
    {
        // Цвета для дизайна
        private readonly Color PrimaryColor = Color.FromArgb(58, 88, 177);
        private readonly Color SecondaryColor = Color.FromArgb(78, 108, 197);
        private readonly Color SuccessColor = Color.FromArgb(40, 167, 69);
        private readonly Color WarningColor = Color.FromArgb(255, 193, 7);
        private readonly Color DangerColor = Color.FromArgb(220, 53, 69);

        private Timer statusTimer;
        private int selectedContractorId = -1;
        private MenuStrip mainMenuStrip;

        public Form1()
        {
            InitializeComponent();
            InitializeMainMenu();
            InitializeModernDesign();
            SetupEventHandlers();

            // Настройка лицензии EPPlus (для некоммерческого использования)
        }

        private void InitializeMainMenu()
        {
            // Создание главного меню
            mainMenuStrip = new MenuStrip();
            mainMenuStrip.BackColor = PrimaryColor;
            mainMenuStrip.ForeColor = Color.White;
            mainMenuStrip.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            mainMenuStrip.Dock = DockStyle.Top;

            // Меню "Файл"
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("Файл");

            // Меню "База данных"
            ToolStripMenuItem databaseMenu = new ToolStripMenuItem("База данных");

            // Меню "Справка"
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Справка");

            // Добавление пунктов в меню "Файл"
            ToolStripMenuItem exportMenuItem = new ToolStripMenuItem("Экспорт в Excel", null, ExportToExcel_Click);
            ToolStripMenuItem importMenuItem = new ToolStripMenuItem("Импорт из Excel", null, ImportFromExcel_Click);
            ToolStripMenuItem clearMenuItem = new ToolStripMenuItem("Очистить базу данных", null, ClearDatabase_Click);
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Выход", null, ExitMenuItem_Click);

            fileMenu.DropDownItems.Add(exportMenuItem);
            fileMenu.DropDownItems.Add(importMenuItem);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(clearMenuItem);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(exitMenuItem);

            // Добавление пунктов в меню "База данных"
            ToolStripMenuItem refreshMenuItem = new ToolStripMenuItem("Обновить данные", null, btnRefresh_Click);
            ToolStripMenuItem addMenuItem = new ToolStripMenuItem("Добавить запись", null, btnAdd_Click);
            ToolStripMenuItem editMenuItem = new ToolStripMenuItem("Редактировать запись", null, btnEdit_Click);
            ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("Удалить запись", null, btnDelete_Click);
            ToolStripMenuItem generateMenuItem = new ToolStripMenuItem("Создать договоры", null, btnGenerate_Click);

            databaseMenu.DropDownItems.Add(refreshMenuItem);
            databaseMenu.DropDownItems.Add(new ToolStripSeparator());
            databaseMenu.DropDownItems.Add(addMenuItem);
            databaseMenu.DropDownItems.Add(editMenuItem);
            databaseMenu.DropDownItems.Add(deleteMenuItem);
            databaseMenu.DropDownItems.Add(new ToolStripSeparator());
            databaseMenu.DropDownItems.Add(generateMenuItem);

            // Добавление пунктов в меню "Справка"
            ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("О программе", null, AboutMenuItem_Click);
            helpMenu.DropDownItems.Add(aboutMenuItem);

            // Добавление меню в MenuStrip
            mainMenuStrip.Items.Add(fileMenu);
            mainMenuStrip.Items.Add(databaseMenu);
            mainMenuStrip.Items.Add(helpMenu);

            // Добавление MenuStrip на форму
            this.Controls.Add(mainMenuStrip);
            this.MainMenuStrip = mainMenuStrip;
        }

        private void InitializeModernDesign()
        {
            // Настройка формы - нормальный режим, не полный экран
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Normal;
            this.Size = new Size(1200, 700);
            this.MinimumSize = new Size(1000, 600);
            this.MaximumSize = new Size(1400, 800);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.BackColor = Color.White;
            this.Text = "Договорная система - Юридический отдел";

            // Настройка двойной буферизации для плавности
            this.DoubleBuffered = true;
            SetDoubleBuffered(dataGridView1);
        }

        private void SetupEventHandlers()
        {
            // События DataGridView
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;
            dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;

            // События кнопок
            btnAdd.MouseEnter += Button_MouseEnter;
            btnAdd.MouseLeave += Button_MouseLeave;
            btnEdit.MouseEnter += Button_MouseEnter;
            btnEdit.MouseLeave += Button_MouseLeave;
            btnDelete.MouseEnter += Button_MouseEnter;
            btnDelete.MouseLeave += Button_MouseLeave;
            btnGenerate.MouseEnter += Button_MouseEnter;
            btnGenerate.MouseLeave += Button_MouseLeave;
            btnRefresh.MouseEnter += Button_MouseEnter;
            btnRefresh.MouseLeave += Button_MouseLeave;

            // События поиска - теперь без таймера, сразу при вводе
            txtSearch.TextChanged += TxtSearch_TextChanged_Immediate;
            txtSearch.Enter += TxtSearch_Enter;
            txtSearch.Leave += TxtSearch_Leave;

            // Фильтры
            cmbTypeFilter.SelectedIndexChanged += Filter_Changed;
            cmbAmountFilter.SelectedIndexChanged += Filter_Changed;

            // Статус таймер
            statusTimer = new Timer { Interval = 5000 };
            statusTimer.Tick += StatusTimer_Tick;
        }

        #region Вспомогательные методы

        private void SetDoubleBuffered(Control control)
        {
            System.Reflection.PropertyInfo propertyInfo = typeof(Control).GetProperty(
                "DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            propertyInfo.SetValue(control, true, null);
        }

        private void UpdateStatusBar()
        {
            int totalCount = dataGridView1.Rows.Count;
            int selectedCount = dataGridView1.SelectedRows.Count;

            lblRecordCount.Text = $"Записей: {totalCount}";
            lblSelectedCount.Text = $"Выбрано: {selectedCount}";
            lblLastUpdate.Text = $"Обновлено: {DateTime.Now:HH:mm:ss}";

            if (totalCount == 0)
            {
                lblRecordCount.ForeColor = WarningColor;
            }
            else
            {
                lblRecordCount.ForeColor = Color.Black;
            }
        }

        private void ShowStatusMessage(string message, bool isSuccess = true)
        {
            toolStripStatusLabel.Text = message;
            toolStripStatusLabel.ForeColor = isSuccess ? SuccessColor : DangerColor;

            statusTimer.Stop();
            statusTimer.Start();
        }

        private List<string> GetTableColumns()
        {
            var columns = new List<string>();
            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("PRAGMA table_info(Contractors);", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader.GetString(1); // name column
                            columns.Add(columnName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка получения структуры таблицы: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return columns;
        }

        private void LoadContractorsData()
        {
            try
            {
                // Получаем список всех столбцов в таблице
                List<string> availableColumns = GetTableColumns();

                if (availableColumns.Count == 0)
                {
                    // Если не удалось получить столбцы, используем базовый набор
                    availableColumns = new List<string>
                    {
                        "Id", "ContractNumber", "ContractorType", "FullName",
                        "PropertyType", "Address", "CadNumber", "SignalType",
                        "MonthlyAmount", "BuildingType", "ResponsiblePerson",
                        "OGRN", "INN_KPP", "PaymentAccount", "BankName", "BIK",
                        "CorrespondentAccount", "Passport", "PassportIssuedBy",
                        "PassportIssueDate", "PassportDepartmentCode", "PersonINN",
                        "Email", "Phone"
                    };
                }

                // Фильтруем только существующие столбцы
                var existingColumns = availableColumns.Where(col =>
                    !string.IsNullOrEmpty(col)).ToList();

                if (existingColumns.Count == 0)
                {
                    ShowStatusMessage("В таблице нет данных", false);
                    return;
                }

                // Формируем запрос для получения всех существующих столбцов
                StringBuilder queryBuilder = new StringBuilder();
                queryBuilder.Append("SELECT ");
                queryBuilder.Append(string.Join(", ", existingColumns));
                queryBuilder.Append(" FROM Contractors WHERE 1=1");

                List<SQLiteParameter> parameters = new List<SQLiteParameter>();

                // Фильтр по типу
                if (cmbTypeFilter.SelectedIndex > 0 && cmbTypeFilter.SelectedItem != null)
                {
                    string type = cmbTypeFilter.SelectedItem.ToString();
                    queryBuilder.Append(" AND ContractorType = @ContractorType");
                    parameters.Add(new SQLiteParameter("@ContractorType", type));
                }

                // Фильтр по сумме (ИСПРАВЛЕНО: новые диапазоны)
                if (cmbAmountFilter.SelectedIndex > 0 && cmbAmountFilter.SelectedItem != null)
                {
                    string amountFilter = cmbAmountFilter.SelectedItem.ToString();
                    if (amountFilter == "До 1,000")
                    {
                        queryBuilder.Append(" AND MonthlyAmount <= @MaxAmount");
                        parameters.Add(new SQLiteParameter("@MaxAmount", 1000m));
                    }
                    else if (amountFilter == "1,001-5,000")
                    {
                        queryBuilder.Append(" AND (MonthlyAmount >= @MinAmount AND MonthlyAmount <= @MaxAmount)");
                        parameters.Add(new SQLiteParameter("@MinAmount", 1001m));
                        parameters.Add(new SQLiteParameter("@MaxAmount", 5000m));
                    }
                    else if (amountFilter == "Свыше 5,000")
                    {
                        queryBuilder.Append(" AND MonthlyAmount > @MinAmount");
                        parameters.Add(new SQLiteParameter("@MinAmount", 5000m));
                    }
                }

                // Поиск по тексту (безопасный, с параметрами)
                string searchText = txtSearch.Text.Trim();
                if (!string.IsNullOrWhiteSpace(searchText) && searchText != "Поиск по имени, номеру или email...")
                {
                    queryBuilder.Append(" AND (FullName LIKE @SearchText OR ContractNumber LIKE @SearchText OR Email LIKE @SearchText)");
                    parameters.Add(new SQLiteParameter("@SearchText", "%" + searchText + "%"));
                }

                queryBuilder.Append(" ORDER BY Id DESC");

                using (var conn = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
                {
                    var command = new SQLiteCommand(queryBuilder.ToString(), conn);

                    // Добавляем параметры
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(param);
                    }

                    var adapter = new SQLiteDataAdapter(command);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Автоматически создаем столбцы на основе DataTable
                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dataTable;

                    // Настраиваем внешний вид и скрываем ненужные столбцы
                    ConfigureDataGridViewAppearance(dataTable);

                    UpdateStatusBar();
                    ShowStatusMessage($"Загружено {dataTable.Rows.Count} записей", true);
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Ошибка загрузки данных: {ex.Message}", false);
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureDataGridViewAppearance(DataTable dataTable)
        {
            try
            {
                // Сначала скрываем все столбцы
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.Visible = true; // Показываем все столбцы, которые есть в данных
                }

                // Настраиваем ширину и заголовки для известных столбцов
                if (dataGridView1.Columns.Contains("Id"))
                    dataGridView1.Columns["Id"].Visible = false;

                if (dataGridView1.Columns.Contains("ContractNumber"))
                {
                    dataGridView1.Columns["ContractNumber"].HeaderText = "Номер договора";
                    dataGridView1.Columns["ContractNumber"].Width = 120;
                }

                if (dataGridView1.Columns.Contains("ContractorType"))
                {
                    dataGridView1.Columns["ContractorType"].HeaderText = "Тип контрагента";
                    dataGridView1.Columns["ContractorType"].Width = 80;
                }

                if (dataGridView1.Columns.Contains("FullName"))
                {
                    dataGridView1.Columns["FullName"].HeaderText = "Контрагент";
                    dataGridView1.Columns["FullName"].Width = 200;
                }

                if (dataGridView1.Columns.Contains("PropertyType"))
                {
                    dataGridView1.Columns["PropertyType"].HeaderText = "Собственность/Аренда";
                    dataGridView1.Columns["PropertyType"].Width = 100;
                }

                if (dataGridView1.Columns.Contains("CadNumber"))
                {
                    dataGridView1.Columns["CadNumber"].HeaderText = "Кадастровый номер";
                    dataGridView1.Columns["CadNumber"].Width = 100;
                }

                if (dataGridView1.Columns.Contains("SignalType"))
                {
                    dataGridView1.Columns["SignalType"].HeaderText = "Тип оборудования";
                    dataGridView1.Columns["SignalType"].Width = 100;
                }

                if (dataGridView1.Columns.Contains("ResponsiblePerson"))
                {
                    dataGridView1.Columns["ResponsiblePerson"].HeaderText = "ФИО ответственного";
                    dataGridView1.Columns["ResponsiblePerson"].Width = 120;
                }

                if (dataGridView1.Columns.Contains("INN_KPP"))
                {
                    dataGridView1.Columns["INN_KPP"].HeaderText = "ИНН/КПП";
                    dataGridView1.Columns["INN_KPP"].Width = 80;
                }

                if (dataGridView1.Columns.Contains("PaymentAccount"))
                {
                    dataGridView1.Columns["PaymentAccount"].HeaderText = "Рассчетный счет";
                    dataGridView1.Columns["PaymentAccount"].Width = 80;
                }

                if (dataGridView1.Columns.Contains("BankName"))
                {
                    dataGridView1.Columns["BankName"].HeaderText = "Банк";
                    dataGridView1.Columns["BankName"].Width = 80;
                }

                if (dataGridView1.Columns.Contains("CorrespondentAccount"))
                {
                    dataGridView1.Columns["CorrespondentAccount"].HeaderText = "Корр. Счет";
                    dataGridView1.Columns["CorrespondentAccount"].Width = 100;
                }

                if (dataGridView1.Columns.Contains("Passport"))
                {
                    dataGridView1.Columns["Passport"].HeaderText = "Серия/номер паспорта";
                    dataGridView1.Columns["Passport"].Width = 100;
                }

                if (dataGridView1.Columns.Contains("PassportIssuedBy"))
                {
                    dataGridView1.Columns["PassportIssuedBy"].HeaderText = "Орган выдачи";
                    dataGridView1.Columns["PassportIssuedBy"].Width = 100;
                }

                if (dataGridView1.Columns.Contains("PassportIssueDate"))
                {
                    dataGridView1.Columns["PassportIssueDate"].HeaderText = "Дата выдачи";
                    dataGridView1.Columns["PassportIssueDate"].Width = 100;
                }

                if (dataGridView1.Columns.Contains("PassportDepartmentCode"))
                {
                    dataGridView1.Columns["PassportDepartmentCode"].HeaderText = "Код подразделения";
                    dataGridView1.Columns["PassportDepartmentCode"].Width = 100;
                }

                if (dataGridView1.Columns.Contains("PersonINN"))
                {
                    dataGridView1.Columns["PersonINN"].HeaderText = "ИНН";
                    dataGridView1.Columns["PersonINN"].Width = 100;
                }

                if (dataGridView1.Columns.Contains("BIK"))
                {
                    dataGridView1.Columns["BIK"].HeaderText = "БИК";
                    dataGridView1.Columns["BIK"].Width = 80;
                }

                if (dataGridView1.Columns.Contains("MonthlyAmount"))
                {
                    dataGridView1.Columns["MonthlyAmount"].HeaderText = "Сумма в месяц";
                    dataGridView1.Columns["MonthlyAmount"].Width = 120;
                    dataGridView1.Columns["MonthlyAmount"].DefaultCellStyle.Format = "N2";
                    dataGridView1.Columns["MonthlyAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if (dataGridView1.Columns.Contains("BuildingType"))
                {
                    dataGridView1.Columns["BuildingType"].HeaderText = "Тип здания";
                    dataGridView1.Columns["BuildingType"].Width = 100;
                }

                if (dataGridView1.Columns.Contains("Email"))
                {
                    dataGridView1.Columns["Email"].HeaderText = "Почта";
                    dataGridView1.Columns["Email"].Width = 150;
                }

                if (dataGridView1.Columns.Contains("Phone"))
                {
                    dataGridView1.Columns["Phone"].HeaderText = "Телефон";
                    dataGridView1.Columns["Phone"].Width = 100;
                }

                if (dataGridView1.Columns.Contains("INN"))
                {
                    dataGridView1.Columns["INN"].HeaderText = "ИНН";
                    dataGridView1.Columns["INN"].Width = 100;
                }

                if (dataGridView1.Columns.Contains("KPP"))
                {
                    dataGridView1.Columns["KPP"].HeaderText = "КПП";
                    dataGridView1.Columns["KPP"].Width = 80;
                }

                if (dataGridView1.Columns.Contains("OGRN"))
                {
                    dataGridView1.Columns["OGRN"].HeaderText = "ОГРН";
                    dataGridView1.Columns["OGRN"].Width = 120;
                }

                if (dataGridView1.Columns.Contains("Address"))
                {
                    dataGridView1.Columns["Address"].HeaderText = "Адрес";
                    dataGridView1.Columns["Address"].Width = 200;
                }

                // Настраиваем стиль таблицы
                dataGridView1.EnableHeadersVisualStyles = false;
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = true;
                dataGridView1.AllowUserToResizeColumns = true;
                dataGridView1.AllowUserToResizeRows = false;
                dataGridView1.ReadOnly = true;

                // Настраиваем заголовки для всех столбцов
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    column.HeaderCell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    column.HeaderCell.Style.BackColor = Color.FromArgb(240, 240, 240);

                    // Если заголовок не настроен, используем имя столбца
                    if (string.IsNullOrEmpty(column.HeaderText))
                    {
                        column.HeaderText = column.Name;
                    }
                }

                // Настраиваем перенос текста для длинных столбцов
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка настройки таблицы: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region Методы для работы с Excel

        private void ExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.RestoreDirectory = true;
                    saveFileDialog.FileName = $"Контрагенты_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        ExportDataToExcel(saveFileDialog.FileName);
                        ShowStatusMessage($"Данные успешно экспортированы в файл: {Path.GetFileName(saveFileDialog.FileName)}", true);

                        // Спрашиваем, открыть ли файл
                        if (MessageBox.Show("Экспорт завершен успешно.\nХотите открыть файл?", "Экспорт в Excel",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Ошибка при экспорте: {ex.Message}", false);
                MessageBox.Show($"Ошибка при экспорте в Excel:\n{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportDataToExcel(string filePath)
        {
            using (var excelPackage = new ExcelPackage())
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Контрагенты");

                // Получаем данные из базы данных
                using (var conn = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Contractors ORDER BY Id", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        // Создаем DataTable для хранения данных
                        var dataTable = new DataTable();
                        dataTable.Load(reader);

                        if (dataTable.Rows.Count == 0)
                        {
                            MessageBox.Show("Нет данных для экспорта.", "Информация",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        // Заполняем заголовки
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = dataTable.Columns[i].ColumnName;
                            worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                            worksheet.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                        }

                        // Заполняем данные
                        for (int row = 0; row < dataTable.Rows.Count; row++)
                        {
                            for (int col = 0; col < dataTable.Columns.Count; col++)
                            {
                                var value = dataTable.Rows[row][col];
                                if (value != DBNull.Value)
                                {
                                    // Проверяем, является ли значение числовым
                                    if (dataTable.Columns[col].DataType == typeof(decimal) ||
                                        dataTable.Columns[col].DataType == typeof(double) ||
                                        dataTable.Columns[col].DataType == typeof(int))
                                    {
                                        worksheet.Cells[row + 2, col + 1].Value = Convert.ToDouble(value);
                                        worksheet.Cells[row + 2, col + 1].Style.Numberformat.Format = "#,##0.00";
                                    }
                                    else
                                    {
                                        worksheet.Cells[row + 2, col + 1].Value = value.ToString();
                                    }
                                }
                            }
                        }

                        // Автонастройка ширины столбцов
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Добавляем форматирование границ
                        var allCells = worksheet.Cells[1, 1, dataTable.Rows.Count + 1, dataTable.Columns.Count];
                        allCells.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        allCells.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        allCells.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        allCells.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                }

                // Сохраняем файл
                excelPackage.SaveAs(new FileInfo(filePath));
            }
        }

        private void ImportFromExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Сначала проверяем файл
                        if (ValidateExcelFile(openFileDialog.FileName))
                        {
                            // Создаем резервную копию текущей базы данных
                            string backupFileName = CreateBackup();

                            // Предупреждение пользователя
                            DialogResult result = MessageBox.Show(
                                "Проверка файла прошла успешно.\n\n" +
                                "Текущая база данных будет сохранена в резервной копии.\n" +
                                "Все существующие данные будут заменены новыми.\n\n" +
                                "Продолжить импорт?",
                                "Подтверждение импорта",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning);

                            if (result == DialogResult.Yes)
                            {
                                // Импортируем данные
                                if (ImportDataFromExcel(openFileDialog.FileName))
                                {
                                    ShowStatusMessage("Данные успешно импортированы из Excel", true);
                                    LoadContractorsData();

                                    MessageBox.Show(
                                        $"Импорт завершен успешно!\n\n" +
                                        $"Резервная копия предыдущей базы данных сохранена в:\n{backupFileName}",
                                        "Импорт из Excel",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(
                                "Файл не прошел проверку.\n" +
                                "Убедитесь, что файл содержит правильные данные и структуру.",
                                "Ошибка проверки",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Ошибка при импорте: {ex.Message}", false);
                MessageBox.Show($"Ошибка при импорте из Excel:\n{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateExcelFile(string filePath)
        {
            try
            {
                using (var excelPackage = new ExcelPackage(new FileInfo(filePath)))
                {
                    if (excelPackage.Workbook.Worksheets.Count == 0)
                    {
                        MessageBox.Show("Файл не содержит рабочих листов.", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    var worksheet = excelPackage.Workbook.Worksheets[0];

                    // Проверяем, есть ли заголовки
                    if (worksheet.Dimension == null || worksheet.Dimension.Rows < 2 || worksheet.Dimension.Columns < 5)
                    {
                        MessageBox.Show("Файл содержит недостаточно данных.", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    // Получаем список столбцов из таблицы базы данных
                    var dbColumns = GetTableColumns();

                    // Проверяем основные обязательные столбцы
                    var requiredColumns = new List<string> { "ContractNumber", "ContractorType", "FullName", "MonthlyAmount" };
                    var excelHeaders = new List<string>();

                    // Читаем заголовки из Excel
                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                    {
                        var header = worksheet.Cells[1, col].Value?.ToString();
                        if (!string.IsNullOrEmpty(header))
                            excelHeaders.Add(header.Trim());
                    }

                    // Проверяем наличие обязательных столбцов
                    foreach (var requiredColumn in requiredColumns)
                    {
                        if (!excelHeaders.Contains(requiredColumn) && !dbColumns.Contains(requiredColumn))
                        {
                            MessageBox.Show($"В файле отсутствует обязательный столбец: {requiredColumn}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }

                    // Проверяем данные в строках
                    int errorCount = 0;
                    List<string> errorMessages = new List<string>();

                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        // Проверяем обязательные поля
                        var contractNumber = worksheet.Cells[row, GetColumnIndex(worksheet, "ContractNumber")]?.Value?.ToString();
                        var fullName = worksheet.Cells[row, GetColumnIndex(worksheet, "FullName")]?.Value?.ToString();
                        var monthlyAmountStr = worksheet.Cells[row, GetColumnIndex(worksheet, "MonthlyAmount")]?.Value?.ToString();

                        if (string.IsNullOrWhiteSpace(contractNumber))
                        {
                            errorMessages.Add($"Строка {row}: Отсутствует номер договора");
                            errorCount++;
                        }

                        if (string.IsNullOrWhiteSpace(fullName))
                        {
                            errorMessages.Add($"Строка {row}: Отсутствует имя контрагента");
                            errorCount++;
                        }

                        if (string.IsNullOrWhiteSpace(monthlyAmountStr) || !decimal.TryParse(monthlyAmountStr, out _))
                        {
                            errorMessages.Add($"Строка {row}: Неверный формат суммы");
                            errorCount++;
                        }

                        // Ограничиваем количество отображаемых ошибок
                        if (errorCount >= 10)
                        {
                            errorMessages.Add("... и другие ошибки");
                            break;
                        }
                    }

                    if (errorCount > 0)
                    {
                        string errorMessage = $"Найдено {errorCount} ошибок в файле:\n\n" +
                                             string.Join("\n", errorMessages.Take(5));

                        if (errorCount > 5)
                            errorMessage += $"\n\n... и еще {errorCount - 5} ошибок";

                        MessageBox.Show(errorMessage, "Ошибки в данных",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        DialogResult result = MessageBox.Show(
                            "В файле обнаружены ошибки. Продолжить импорт?",
                            "Подтверждение",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        return result == DialogResult.Yes;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке файла:\n{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private int GetColumnIndex(ExcelWorksheet worksheet, string columnName)
        {
            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            {
                if (worksheet.Cells[1, col].Value?.ToString() == columnName)
                    return col;
            }
            return -1;
        }

        private bool ImportDataFromExcel(string filePath)
        {
            using (var excelPackage = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = excelPackage.Workbook.Worksheets[0];

                using (var conn = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
                {
                    conn.Open();

                    // Начинаем транзакцию
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Очищаем таблицу
                            using (var cmd = new SQLiteCommand("DELETE FROM Contractors", conn, transaction))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            // Сбрасываем автоинкремент
                            using (var cmd = new SQLiteCommand("DELETE FROM sqlite_sequence WHERE name='Contractors'", conn, transaction))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            // Получаем список столбцов
                            var columnNames = new List<string>();
                            var columnIndexes = new Dictionary<string, int>();

                            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                            {
                                var columnName = worksheet.Cells[1, col].Value?.ToString();
                                if (!string.IsNullOrEmpty(columnName))
                                {
                                    columnNames.Add(columnName);
                                    columnIndexes[columnName] = col;
                                }
                            }

                            // Импортируем данные
                            int importedRows = 0;
                            int skippedRows = 0;

                            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                            {
                                try
                                {
                                    // Проверяем обязательные поля
                                    var contractNumber = worksheet.Cells[row, columnIndexes["ContractNumber"]]?.Value?.ToString();
                                    var fullName = worksheet.Cells[row, columnIndexes["FullName"]]?.Value?.ToString();

                                    if (string.IsNullOrWhiteSpace(contractNumber) || string.IsNullOrWhiteSpace(fullName))
                                    {
                                        skippedRows++;
                                        continue;
                                    }

                                    // Создаем команду для вставки
                                    string insertQuery = $"INSERT INTO Contractors ({string.Join(", ", columnNames)}) " +
                                                        $"VALUES ({string.Join(", ", columnNames.Select(c => $"@{c}"))})";

                                    using (var cmd = new SQLiteCommand(insertQuery, conn, transaction))
                                    {
                                        foreach (var columnName in columnNames)
                                        {
                                            var value = worksheet.Cells[row, columnIndexes[columnName]]?.Value;

                                            if (value == null)
                                            {
                                                cmd.Parameters.AddWithValue($"@{columnName}", DBNull.Value);
                                            }
                                            else
                                            {
                                                // Преобразуем значения в правильный формат
                                                if (columnName == "MonthlyAmount")
                                                {
                                                    if (decimal.TryParse(value.ToString(), out decimal decimalValue))
                                                        cmd.Parameters.AddWithValue($"@{columnName}", decimalValue);
                                                    else
                                                        cmd.Parameters.AddWithValue($"@{columnName}", DBNull.Value);
                                                }
                                                else
                                                {
                                                    cmd.Parameters.AddWithValue($"@{columnName}", value.ToString());
                                                }
                                            }
                                        }

                                        cmd.ExecuteNonQuery();
                                        importedRows++;
                                    }
                                }
                                catch
                                {
                                    skippedRows++;
                                    // Продолжаем импорт других строк
                                }
                            }

                            transaction.Commit();

                            MessageBox.Show(
                                $"Импорт завершен:\n" +
                                $"Успешно импортировано: {importedRows} строк\n" +
                                $"Пропущено: {skippedRows} строк",
                                "Результат импорта",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            return importedRows > 0;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Ошибка при импорте данных: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void ClearDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                // Создаем резервную копию перед очисткой
                string backupFileName = CreateBackup();

                DialogResult result = MessageBox.Show(
                    $"Перед очисткой базы данных будет создана резервная копия.\n\n" +
                    $"Резервная копия: {Path.GetFileName(backupFileName)}\n\n" +
                    $"Вы уверены, что хотите очистить базу данных?\n" +
                    $"Все данные будут удалены без возможности восстановления!",
                    "Подтверждение очистки",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    using (var conn = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
                    {
                        conn.Open();

                        // Удаляем все записи
                        using (var cmd = new SQLiteCommand("DELETE FROM Contractors", conn))
                        {
                            int rowsDeleted = cmd.ExecuteNonQuery();

                            // Сбрасываем автоинкремент
                            using (var resetCmd = new SQLiteCommand("DELETE FROM sqlite_sequence WHERE name='Contractors'", conn))
                            {
                                resetCmd.ExecuteNonQuery();
                            }

                            ShowStatusMessage($"База данных очищена. Удалено {rowsDeleted} записей", true);
                            LoadContractorsData();

                            MessageBox.Show(
                                $"База данных успешно очищена!\n\n" +
                                $"Резервная копия сохранена в:\n{backupFileName}",
                                "Очистка базы данных",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"Ошибка при очистке: {ex.Message}", false);
                MessageBox.Show($"Ошибка при очистке базы данных:\n{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string CreateBackup()
        {
            try
            {
                // Создаем папку для резервных копий, если её нет
                string backupFolder = Path.Combine(Application.StartupPath, "Backups");
                if (!Directory.Exists(backupFolder))
                    Directory.CreateDirectory(backupFolder);

                // Генерируем имя файла с timestamp
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFileName = Path.Combine(backupFolder, $"Backup_{timestamp}.xlsx");

                // Экспортируем текущие данные в Excel
                ExportDataToExcel(backupFileName);

                return backupFileName;
            }
            catch (Exception ex)
            {
                throw new Exception($"Не удалось создать резервную копию: {ex.Message}");
            }
        }

        #endregion

        #region События формы

        private void Form1_Load(object sender, EventArgs e)
        {
            // Настройка комбобоксов фильтров
            cmbTypeFilter.Items.AddRange(new string[] { "Все типы", "ООО", "ИП", "ФЛ" });
            cmbTypeFilter.SelectedIndex = 0;

            // ИСПРАВЛЕНО: новые диапазоны сумм
            cmbAmountFilter.Items.AddRange(new string[] { "Все суммы", "До 1,000", "1,001-5,000", "Свыше 5,000" });
            cmbAmountFilter.SelectedIndex = 0;

            // Настройка поиска - ИСПРАВЛЕНО: более подробная подсказка
            txtSearch.Text = "Поиск по имени, номеру или email...";
            txtSearch.ForeColor = Color.Gray;

            // Загружаем данные
            LoadContractorsData();

            // Плавное появление
            this.Opacity = 0;
            Timer fadeTimer = new Timer { Interval = 10 };
            fadeTimer.Tick += (s, args) =>
            {
                if (this.Opacity < 1.0)
                    this.Opacity += 0.05;
                else
                {
                    fadeTimer.Stop();
                    fadeTimer.Dispose();
                }
            };
            fadeTimer.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Градиент для панели заголовка
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                panelHeader.ClientRectangle,
                PrimaryColor,
                SecondaryColor,
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

        #region События DataGridView

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            UpdateStatusBar();

            // Включаем/выключаем кнопки в зависимости от выбора
            bool hasSelection = dataGridView1.SelectedRows.Count > 0;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
            btnGenerate.Enabled = dataGridView1.SelectedRows.Count >= 1;

            // Сохраняем ID выбранного контрагента
            if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].Cells["Id"] != null)
            {
                object idValue = dataGridView1.SelectedRows[0].Cells["Id"].Value;
                if (idValue != null && idValue != DBNull.Value)
                {
                    int.TryParse(idValue.ToString(), out selectedContractorId);
                }
            }
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count) return;

            // Форматирование суммы
            if (dataGridView1.Columns[e.ColumnIndex].Name == "MonthlyAmount" && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal amount))
                {
                    e.Value = amount.ToString("N2");
                    e.FormattingApplied = true;
                }
            }

            // Цвет строки в зависимости от типа
            if (dataGridView1.Columns.Contains("ContractorType") &&
                dataGridView1.Rows[e.RowIndex].Cells["ContractorType"] != null &&
                dataGridView1.Rows[e.RowIndex].Cells["ContractorType"].Value != null)
            {
                string type = dataGridView1.Rows[e.RowIndex].Cells["ContractorType"].Value.ToString();
                Color rowColor = Color.White;

                switch (type)
                {
                    case "ООО":
                        rowColor = Color.FromArgb(240, 249, 255);
                        break;
                    case "ИП":
                        rowColor = Color.FromArgb(255, 249, 240);
                        break;
                    case "ФЛ":
                        rowColor = Color.FromArgb(240, 255, 244);
                        break;
                }

                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = rowColor;
            }

            // Чередование цвета строк (поверх основного цвета)
            if (e.RowIndex % 2 == 0 && dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor == Color.White)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            }
        }

        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                btnEdit_Click(sender, e);
            }
        }

        #endregion

        #region События кнопок

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                Color hoverColor = button.BackColor;

                if (button == btnDelete)
                    hoverColor = Color.FromArgb(200, 35, 51);
                else if (button == btnGenerate)
                    hoverColor = Color.FromArgb(30, 147, 59);
                else if (button == btnAdd || button == btnEdit || button == btnRefresh)
                    hoverColor = Color.FromArgb(48, 78, 167);

                button.BackColor = hoverColor;
            }
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                Color normalColor;

                if (button == btnDelete)
                    normalColor = DangerColor;
                else if (button == btnGenerate)
                    normalColor = SuccessColor;
                else if (button == btnAdd || button == btnEdit || button == btnRefresh)
                    normalColor = PrimaryColor;
                else
                    normalColor = button.BackColor;

                button.BackColor = normalColor;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new ContractorForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadContractorsData();
                    ShowStatusMessage("Запись успешно добавлена", true);
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                ShowStatusMessage("Выберите запись для редактирования", false);
                return;
            }

            if (dataGridView1.SelectedRows[0].Cells["Id"] == null ||
                dataGridView1.SelectedRows[0].Cells["Id"].Value == null)
            {
                ShowStatusMessage("Не удалось получить ID записи", false);
                return;
            }

            int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
            using (var form = new ContractorForm(selectedId))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadContractorsData();
                    ShowStatusMessage("Запись успешно обновлена", true);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                ShowStatusMessage("Выберите запись для удаления", false);
                return;
            }

            if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                try
                {
                    string query = "DELETE FROM Contractors WHERE Id = @Id;";
                    using (var conn = new SQLiteConnection(DatabaseHelper.GetConnectionString()))
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", selectedId);
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            LoadContractorsData();
                            ShowStatusMessage("Запись успешно удалена", true);
                        }
                        else
                        {
                            ShowStatusMessage("Запись не найдена", false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowStatusMessage($"Ошибка удаления: {ex.Message}", false);
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                ShowStatusMessage("Выберите записи для генерации договоров", false);
                return;
            }

            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Выберите папку для сохранения файлов договоров";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    btnGenerate.Enabled = false;
                    btnGenerate.Text = "Генерация...";
                    Cursor = Cursors.WaitCursor;

                    int successCount = 0;
                    int errorCount = 0;
                    List<string> errorMessages = new List<string>();

                    try
                    {
                        // Собираем информацию о выбранных контрагентах
                        List<(int id, string contractNumber)> selectedContractors = new List<(int id, string contractNumber)>();

                        foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                        {
                            if (row.Cells["Id"] != null && row.Cells["Id"].Value != null)
                            {
                                int selectedId = Convert.ToInt32(row.Cells["Id"].Value);
                                string contractNumber = "Без номера";

                                if (row.Cells["ContractNumber"] != null && row.Cells["ContractNumber"].Value != null)
                                {
                                    contractNumber = row.Cells["ContractNumber"].Value.ToString();
                                }

                                selectedContractors.Add((selectedId, contractNumber));
                            }
                        }

                        // Генерируем документы последовательно
                        foreach (var contractor in selectedContractors)
                        {
                            try
                            {
                                // Передаем только ID и номер договора
                                bool isSuccess = await Task.Run(() =>
                                    DocumentGenerator.GenerateDocument(contractor.id, folderDialog.SelectedPath, contractor.contractNumber));

                                if (isSuccess)
                                {
                                    successCount++;
                                }
                                else
                                {
                                    errorCount++;
                                    errorMessages.Add($"Ошибка при создании договора №{contractor.contractNumber}");
                                }
                            }
                            catch (Exception ex)
                            {
                                errorCount++;
                                errorMessages.Add($"Ошибка для договора №{contractor.contractNumber}: {ex.Message}");
                            }
                        }

                        // Обновляем UI
                        btnGenerate.Enabled = true;
                        btnGenerate.Text = "Создать договоры";
                        Cursor = Cursors.Default;

                        // Показываем итоговое сообщение
                        if (successCount > 0)
                        {
                            string message = $"Успешно создано {successCount} договоров в папке: {folderDialog.SelectedPath}";

                            if (errorCount > 0)
                            {
                                message += $"\nОшибок: {errorCount}";
                                if (errorMessages.Any())
                                {
                                    message += $"\n\nСписок ошибок:\n{string.Join("\n", errorMessages.Take(5))}";
                                    if (errorMessages.Count > 5)
                                    {
                                        message += $"\n... и еще {errorMessages.Count - 5} ошибок";
                                    }
                                }
                            }

                            ShowStatusMessage(message, true);

                            // Подсвечиваем кнопку успеха
                            btnGenerate.BackColor = Color.FromArgb(30, 147, 59);
                            Timer successTimer = new Timer { Interval = 2000 };
                            successTimer.Tick += (s, args) =>
                            {
                                successTimer.Stop();
                                btnGenerate.BackColor = SuccessColor;
                            };
                            successTimer.Start();
                        }
                        else
                        {
                            ShowStatusMessage("Не удалось создать ни одного договора", false);
                        }
                    }
                    catch (Exception ex)
                    {
                        btnGenerate.Enabled = true;
                        btnGenerate.Text = "Создать договоры";
                        Cursor = Cursors.Default;
                        ShowStatusMessage($"Критическая ошибка: {ex.Message}", false);
                    }
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadContractorsData();
            ShowStatusMessage("Данные обновлены", true);
        }

        #endregion

        #region События поиска и фильтров

        // ИСПРАВЛЕНО: поиск сразу при вводе каждой буквы
        private void TxtSearch_TextChanged_Immediate(object sender, EventArgs e)
        {
            LoadContractorsData();
        }

        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Поиск по имени, номеру или email...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
            txtSearch.BackColor = Color.FromArgb(240, 245, 255);
        }

        private void TxtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Поиск по имени, номеру или email...";
                txtSearch.ForeColor = Color.Gray;
            }
            txtSearch.BackColor = Color.White;
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            LoadContractorsData();
        }

        #endregion

        #region События статусной строки

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel.Text = "Готово";
            toolStripStatusLabel.ForeColor = Color.Black;
            statusTimer.Stop();
        }

        #endregion

        #region Меню и дополнительные функции

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Договорная система для юридического отдела\n" +
                "Версия: 0.3\n" +
                "Создатель: Терехин Антон - RAD\n" +
                "Год: 2026\n\n" +
                "Добавлены функции:\n" +
                "- Экспорт базы данных в Excel\n" +
                "- Импорт базы данных из Excel\n" +
                "- Очистка базы данных с созданием резервной копии\n" +
                "- Улучшенное верхнее меню\n\n" +
                "Система предназначена для:\n" +
                "- Хранения данных контрагентов (ООО, ИП, ФЛ)\n" +
                "- Управления договорами\n" +
                "- Автоматической генерации документов\n" +
                "- Фильтрации и поиска записей\n" +
                "- Импорта/экспорта данных",
                "О программе",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Выход",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        #endregion

        private void panelHeader_Paint(object sender, PaintEventArgs e)
        {
            // Обработчик события рисования для заголовка
        }
    }
}