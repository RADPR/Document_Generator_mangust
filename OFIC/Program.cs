using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace OFIC
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {

            ExcelPackage.License.SetNonCommercialPersonal("Антон");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. Инициализируем базу данных (создаём файл и таблицы)
            try
            {
                DatabaseHelper.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось инициализировать базу данных: {ex.Message}", "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Завершаем программу при ошибке
            }

            // 2. Показываем форму входа
            using (LoginForm loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // 3. Если пароль верный, запускаем главное окно
                    Application.Run(new Form1());
                }
                else
                {
                    // Пользователь нажал "Отмена" или закрыл форму
                    Application.Exit();
                }
            }
        }
    }
}
