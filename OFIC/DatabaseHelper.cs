using System;
using System.Data.SQLite;
using System.Net.Http;
using System.Threading.Tasks;

namespace OFIC
{
    public static class DatabaseHelper
    {
        // Путь к файлу базы данных. Он будет создан в папке с программой.
        private static string dbPath = "contracts.db";
        private static string connectionString = $"Data Source={dbPath};Version=3;";

        // URL вашей веб-страницы с паролем (ОБЯЗАТЕЛЬНО ИЗМЕНИТЕ НА СВОЙ)
        private static string passwordServiceUrl = "https://sanexpert24.ru/ofpaskol.php";

        // Метод для инициализации базы данных и создания таблицы, если её нет
        public static void InitializeDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // SQL-скрипт для создания таблицы 'Contractors' со всеми полями
                    string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Contractors (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ContractNumber TEXT NOT NULL,
                        ContractorType TEXT NOT NULL, -- 'ООО', 'ИП', 'ФЛ'
                        FullName TEXT NOT NULL,
                        PropertyType TEXT NOT NULL, -- 'Аренда' или 'Собственность'
                        Address TEXT NOT NULL,
                        CadNumber TEXT NOT NULL,
                        SignalType TEXT NOT NULL, -- 'ОС', 'КТС', 'ОС+КТС'
                        MonthlyAmount DECIMAL NOT NULL,
                        -- Тип здания (универсальное для всех)
                        BuildingType TEXT,

                        -- Реквизиты для ООО
                        ResponsiblePerson TEXT,
                        OGRN TEXT,
                        INN_KPP TEXT,
                        PaymentAccount TEXT,
                        BankName TEXT,
                        BIK TEXT,
                        CorrespondentAccount TEXT,
                        

                        -- Реквизиты для ИП/ФЛ
                        Passport TEXT,
                        PassportIssuedBy TEXT,
                        PassportIssueDate TEXT,
                        PassportDepartmentCode TEXT,
                        PersonINN TEXT,

                        -- Контакты
                        Email TEXT,
                        Phone TEXT
                    );";

                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // УДАЛЕНО СОЗДАНИЕ ТАБЛИЦЫ ДЛЯ ПАРОЛЯ (AppConfig)
                    // УДАЛЕНО ИНИЦИАЛИЗАЦИЯ СТАНДАРТНОГО ПАРОЛЯ
                }
                Console.WriteLine("База данных успешно инициализирована.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при инициализации базы данных: {ex.Message}");
                throw;
            }
        }

        // Метод для получения строки подключения (используется в других местах)
        public static string GetConnectionString()
        {
            return connectionString;
        }

        // Функция хэширования пароля
        public static string SimpleHash(string input)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Метод для получения хеша пароля с веб-сайта (ВЫБРАСЫВАЕТ ИСКЛЮЧЕНИЕ ПРИ ОШИБКЕ)
        public static async Task<string> GetPasswordHashFromWebAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                // Устанавливаем таймаут для запроса
                client.Timeout = TimeSpan.FromSeconds(15);

                try
                {
                    // Получаем хеш пароля с сайта
                    HttpResponseMessage response = await client.GetAsync(passwordServiceUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        return (await response.Content.ReadAsStringAsync()).Trim();
                    }
                    else
                    {
                        throw new Exception($"Ошибка сервера: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (TaskCanceledException)
                {
                    throw new Exception("Таймаут подключения. Проверьте интернет-соединение.");
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception($"Ошибка сети: {ex.Message}. Проверьте интернет-соединение и URL.");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка при получении пароля: {ex.Message}");
                }
            }
        }

        // Основной метод для проверки пароля (асинхронный)
        public static async Task<bool> VerifyPasswordAsync(string inputPassword)
        {
            // Получаем хеш с веб-сайта (если нет интернета - будет исключение)
            string storedHash = await GetPasswordHashFromWebAsync();

            if (string.IsNullOrEmpty(storedHash))
            {
                throw new Exception("Не удалось получить хеш пароля с сервера. Сервер вернул пустой ответ.");
            }

            // Вычисляем хеш введенного пароля
            var inputHash = SimpleHash(inputPassword);

            // Сравниваем
            return storedHash == inputHash;
        }

        // Метод для проверки доступности сервера с паролем
        public static async Task<bool> CheckServerAvailability()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    var response = await client.GetAsync(passwordServiceUrl);
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        // Метод для обновления URL веб-службы пароля
        public static void SetPasswordServiceUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                passwordServiceUrl = url;
            }
        }
    }
}