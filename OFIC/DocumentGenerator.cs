using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Nut; // Для преобразования чисел в пропись
using EasyDox; // Для работы с шаблонами Word
using Xceed.Words.NET;

namespace OFIC
{
    public static class DocumentGenerator
    {
        public enum Case
        {
            Nominative,  // Именительный (кто? что?) - "десять тысяч"
            Genitive     // Родительный (кого? чего?) - "десяти тысяч"
        }
        // Метод для генерации документа на основе ID записи
        public static bool GenerateDocument(int contractorId, string saveDirectory, string contractNumber)
        {
            try
            {
                // 1. Загружаем данные контрагента из БД
                var contractorData = LoadContractorData(contractorId);
                if (contractorData == null)
                {
                    return false;
                }

                // 2. Формируем словарь данных для подстановки в шаблон
                var dataForMerge = PrepareMergeData(contractorData);

                // 3. Определяем путь к шаблону (файл должен быть в папке с программой)
                string templatePath = Path.Combine(Application.StartupPath, "шаблон_договора.docx");
                if (!File.Exists(templatePath))
                {
                    return false;
                }

                // 4. Создаём имя выходного файла: используем переданный номер договора
                string outputFileName;
                if (!string.IsNullOrEmpty(contractNumber) && contractNumber != "Без номера")
                {
                    // Очищаем номер договора от недопустимых символов для имени файла
                    string cleanContractNumber = string.Concat(contractNumber.Split(Path.GetInvalidFileNameChars()));
                    outputFileName = $"{cleanContractNumber}.docx";
                }
                else
                {
                    // Если нет номера договора, используем ID
                    outputFileName = $"Договор_{contractorId}.docx";
                }

                string outputPath = Path.Combine(saveDirectory, outputFileName);

                // 5. Выполняем слияние с шаблоном через EasyDox
                try
                {
                    // Загружаем шаблон
                    var doc = DocX.Load(templatePath);

                    // Заменяем все метки в документе
                    foreach (var field in dataForMerge)
                    {
                        doc.ReplaceText("{{" + field.Key + "}}", field.Value);
                    }

                    // Сохраняем результат
                    doc.SaveAs(outputPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при создании документа: {ex.Message}");
                    return false;
                }

                // 6. Проверяем, что файл создан
                return File.Exists(outputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при генерации документа: {ex.Message}");
                return false;
            }
        }

        // Загрузка данных контрагента из БД
        private static ContractorData LoadContractorData(int id)
        {
            string query = "SELECT * FROM Contractors WHERE Id = @Id;";
            try
            {
                using (var conn = new System.Data.SQLite.SQLiteConnection(DatabaseHelper.GetConnectionString()))
                using (var cmd = new System.Data.SQLite.SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var data = new ContractorData
                            {
                                // Основные данные
                                Id = Convert.ToInt32(reader["Id"]),
                                ContractNumber = reader["ContractNumber"].ToString(),
                                ContractorType = reader["ContractorType"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                PropertyType = reader["PropertyType"].ToString(),
                                Address = reader["Address"].ToString(),
                                CadNumber = reader["CadNumber"].ToString(),
                                SignalType = reader["SignalType"].ToString(),
                                MonthlyAmount = Convert.ToDecimal(reader["MonthlyAmount"]),
                                BuildingType = reader["BuildingType"]?.ToString(),

                                // ООО данные
                                ResponsiblePerson = reader["ResponsiblePerson"]?.ToString(),
                                OGRN = reader["OGRN"]?.ToString(),
                                INN_KPP = reader["INN_KPP"]?.ToString(),
                                PaymentAccount = reader["PaymentAccount"]?.ToString(),
                                BankName = reader["BankName"]?.ToString(),
                                BIK = reader["BIK"]?.ToString(),
                                CorrespondentAccount = reader["CorrespondentAccount"]?.ToString(),

                                // ИП/ФЛ данные
                                Passport = reader["Passport"]?.ToString(),
                                PassportIssuedBy = reader["PassportIssuedBy"]?.ToString(),
                                PassportIssueDate = reader["PassportIssueDate"]?.ToString(),
                                PassportDepartmentCode = reader["PassportDepartmentCode"]?.ToString(),
                                PersonINN = reader["PersonINN"]?.ToString(),

                                // Контакты
                                Email = reader["Email"]?.ToString(),
                                Phone = reader["Phone"]?.ToString()
                            };
                            return data;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
                return null;
            }
            return null;
        }

        private static string GetSignatoryName(ContractorData data)
        {
            if (data.ContractorType == "ООО" && !string.IsNullOrWhiteSpace(data.ResponsiblePerson))
            {
                // Для ООО берем ответственное лицо
                return FormatShortName(data.ResponsiblePerson);
            }
            else
            {
                // Для ИП/ФЛ берем основное ФИО
                return FormatShortName(data.FullName);
            }
        }

        // Метод для сокращения ФИО до формата "Фамилия И.О."
        private static string FormatShortName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return string.Empty;

            // Исправленная строка: Split по пробелам с удалением пустых элементов
            var parts = fullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
                return fullName;

            string lastName = parts[0];
            string initials = "";

            for (int i = 1; i < Math.Min(parts.Length, 3); i++)
            {
                if (parts[i].Length > 0)
                    initials += parts[i][0] + ".";
            }

            return $"{lastName} {initials}".Trim();
        }

        // Подготовка словаря данных для подстановки в шаблон
        private static Dictionary<string, string> PrepareMergeData(ContractorData data)
        {
            var mergeData = new Dictionary<string, string>();

            // 1. Базовые поля (точно соответствуют меткам в шаблоне)
            mergeData["Номер_договора"] = data.ContractNumber;
            mergeData["ИП_ООО"] = data.ContractorType;
            

            // 2. Формируем поле ФИО/Название в зависимости от типа
            mergeData["ФИО_НК"] = FormatFullName(data);
            mergeData["Аренда"] = data.PropertyType == "Аренда" ? "аренды" : "собственности";
            mergeData["адрес"] = data.Address;
            mergeData["кад_номер"] = data.CadNumber;

            // 3. Тип сигнализации
            mergeData["ОС_КТС"] = FormatSignalType(data.SignalType);

            // 4. Дата в нужном формате
            mergeData["Дата"] = DateTime.Now.ToString("«dd» MMMM yyyy г.");

            // 5. Суммы (ежемесячная)
            decimal monthlyAmount = data.MonthlyAmount;
            mergeData["Сумма"] = FormatAmountNumeric(monthlyAmount);
            mergeData["Сумма_пропись"] = FormatAmountWords(monthlyAmount);

            // 6. Суммы (годовая)
            decimal yearlyAmount = monthlyAmount * 12;
            mergeData["Сумма_12"] = FormatAmountNumeric(yearlyAmount);
            mergeData["Сумма_пропись_12"] = FormatAmountWordsGenitive(yearlyAmount); // Родительный падеж!

            // 7. Динамические поля на основе типа контрагента
            FormatDynamicFields(mergeData, data);

            // 8. Контакты
            mergeData["емейл"] = data.Email ?? "";
            mergeData["телефон"] = data.Phone ?? "";

            mergeData["ФИО_Ответственный"] = GetSignatoryName(data);

            // Добавьте после других полей
            mergeData["Тип_здания"] = !string.IsNullOrWhiteSpace(data.BuildingType)? data.BuildingType: "нежилое помещение";

            return mergeData;
        }

        // Форматирование ФИО/Названия в зависимости от типа
        private static string FormatFullName(ContractorData data)
        {
            switch (data.ContractorType)
            {
                case "ИП":
                    return $"индивидуальный предприниматель {data.FullName}";
                case "ФЛ":
                    return $"физическое лицо {data.FullName}"; // ← ИЗМЕНЕНИЕ ЗДЕСЬ
                case "ООО":
                    return $"{data.FullName}";
                default:
                    return data.FullName;
            }
        }

        // Форматирование типа сигнализации
        private static string FormatSignalType(string signalType)
        {
            switch (signalType)
            {
                case "ОС":
                    return "(охранная сигнализация)";
                case "КТС":
                    return "(тревожная сигнализация)";
                case "ОС+КТС":
                    return "(охранная и тревожная сигнализация)";
                default:
                    return signalType;
            }
        }

        // Форматирование числовой суммы (с пробелами между тысячами)
        private static string FormatAmountNumeric(decimal amount)
        {
            return amount.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("ru-RU"));
        }

        // Форматирование суммы прописью (с помощью библиотеки Nut)
        private static string FormatAmountWords(decimal amount)
        {
            try
            {
                long wholeAmount = (long)amount;
                return SumToStringHelper.ConvertSumToWords(wholeAmount, Case.Nominative);
            }
            catch (Exception)
            {
                return "(сумма прописью)";
            }
        }
        private static string FormatAmountWordsGenitive(decimal amount)
        {
            try
            {
                long wholeAmount = (long)amount;
                return SumToStringHelper.ConvertSumToWords(wholeAmount, Case.Genitive);
            }
            catch (Exception)
            {
                return "(сумма прописью)";
            }
        }
        public static class SumToStringHelper
        {
            // Основные словари для преобразования
            private static readonly string[] units =
                { "", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };
            private static readonly string[] teens =
                { "десять", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать",
          "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать" };
            private static readonly string[] tens =
                { "", "", "двадцать", "тридцать", "сорок", "пятьдесят",
          "шестьдесят", "семьдесят", "восемьдесят", "девяносто" };
            private static readonly string[] hundreds =
                { "", "сто", "двести", "триста", "четыреста", "пятьсот",
          "шестьсот", "семьсот", "восемьсот", "девятьсот" };
            private static readonly string[] unitsGenitive =
    { "", "одного", "двух", "трех", "четырех", "пяти", "шести", "семи", "восьми", "девяти" };

            private static readonly string[] tensGenitive =
                { "", "", "двадцати", "тридцати", "сорока", "пятидесяти",
      "шестидесяти", "семидесяти", "восьмидесяти", "девяноста" };

            private static readonly string[] hundredsGenitive =
                { "", "ста", "двухсот", "трехсот", "четырехсот", "пятисот",
      "шестисот", "семисот", "восьмисот", "девятисот" };

            private static readonly string[] teensGenitive =
                { "десяти", "одиннадцати", "двенадцати", "тринадцати", "четырнадцати",
      "пятнадцати", "шестнадцати", "семнадцати", "восемнадцати", "девятнадцати" };

            // Разряды (тысячи, миллионы, миллиарды) в разных падежах
            private static readonly string[,] ranks =
            {
        { "", "", "", "" }, // не используется, для удобства индексации
        { "тысяча", "тысячи", "тысяч", "female" },       // rank 1: тысячи
        { "миллион", "миллиона", "миллионов", "male" },   // rank 2: миллионы
        { "миллиард", "миллиарда", "миллиардов", "male" } // rank 3: миллиарды
    };

            // Основной метод преобразования числа в пропись
            public static string ConvertSumToWords(long number, Case wordCase = Case.Nominative)
            {
                if (number == 0)
                    return wordCase == Case.Genitive ? "ноля" : "ноль";

                string result = "";
                int rank = 0;
                long tempNumber = number;

                while (tempNumber > 0)
                {
                    int part = (int)(tempNumber % 1000);
                    if (part > 0)
                    {
                        string partWords = ConvertThreeDigit(part, rank, wordCase);
                        if (!string.IsNullOrEmpty(partWords))
                        {
                            result = partWords + " " + result;
                        }
                    }
                    tempNumber /= 1000;
                    rank++;
                }

                return result.Trim();
            }

            // Преобразование трехзначного числа (0-999) с учетом разряда
            private static string ConvertThreeDigit(int number, int rank, Case wordCase)
            {
                if (number == 0)
                    return "";

                string result = "";
                int hundredsDigit = number / 100;
                int tensDigit = (number % 100) / 10;
                int unitsDigit = number % 10;

                // 1. Сотни
                if (hundredsDigit > 0)
                {
                    result += (wordCase == Case.Genitive ?
                              hundredsGenitive[hundredsDigit] : hundreds[hundredsDigit]) + " ";
                }

                // 2. Десятки и единицы
                int lastTwoDigits = number % 100;

                if (lastTwoDigits >= 10 && lastTwoDigits <= 19)
                {
                    // Числа 10-19
                    result += (wordCase == Case.Genitive ?
                              teensGenitive[lastTwoDigits - 10] : teens[lastTwoDigits - 10]) + " ";
                }
                else
                {
                    // Десятки (20-90)
                    if (tensDigit > 1)
                    {
                        result += (wordCase == Case.Genitive ?
                                  tensGenitive[tensDigit] : tens[tensDigit]) + " ";
                    }
                    else if (tensDigit == 1)
                    {
                        // Для чисел 01-09 (десяток = 1, но это не 10-19)
                        if (unitsDigit > 0)
                        {
                            result += (wordCase == Case.Genitive ? "десяти " : "десять ");
                        }
                    }

                    // Единицы
                    if (unitsDigit > 0)
                    {
                        if (rank == 1) // Для тысяч особые формы
                        {
                            string[] thousandUnits = { "", "одна", "две", "три", "четыре", "пять",
                                           "шесть", "семь", "восемь", "девять" };
                            string[] thousandUnitsGenitive = { "", "одной", "двух", "трех", "четырех",
                                                   "пяти", "шести", "семи", "восьми", "девяти" };

                            result += (wordCase == Case.Genitive ?
                                      thousandUnitsGenitive[unitsDigit] : thousandUnits[unitsDigit]) + " ";
                        }
                        else
                        {
                            result += (wordCase == Case.Genitive ?
                                      unitsGenitive[unitsDigit] : units[unitsDigit]) + " ";
                        }
                    }
                }

                // Добавляем название разряда (тысячи, миллионы)
                result += GetRankForm(number, rank, wordCase);

                return result.Trim();
            }

            // Получение правильной формы разряда (тысяча/тысячи/тысяч)
            private static string GetRankForm(int number, int rank, Case wordCase)
            {
                if (rank == 0)
                    return "";

                int lastTwo = number % 100;
                int lastOne = number % 10;

                // Формы для разрядов в разных падежах
                string[,] rankForms =
                {
        // Для rank=1 (тысячи): именительный, родительный
        { "тысяча", "тысячи", "тысяч", "тысячи", "тысяч", "тысяч" },
        // Для rank=2 (миллионы)
        { "миллион", "миллиона", "миллионов", "миллиона", "миллионов", "миллионов" },
        // Для rank=3 (миллиарды)
        { "миллиард", "миллиарда", "миллиардов", "миллиарда", "миллиардов", "миллиардов" }
    };

                int formIndex;
                if (lastTwo >= 11 && lastTwo <= 19)
                    formIndex = 2; // Множественное число (11-19)
                else
                {
                    switch (lastOne)
                    {
                        case 1: formIndex = 0; break; // 1
                        case 2:
                        case 3:
                        case 4: formIndex = 1; break; // 2-4
                        default: formIndex = 2; break; // 0, 5-9
                    }
                }

                // Смещение для родительного падежа
                if (wordCase == Case.Genitive)
                    formIndex += 3;

                // Проверяем, что индекс в пределах массива
                if (rank - 1 < rankForms.GetLength(0) && formIndex < rankForms.GetLength(1))
                    return rankForms[rank - 1, formIndex] + " ";

                return "";
            }

        }

        // Форматирование динамических полей (доп_поля_1-6 и связанные поля)
        private static void FormatDynamicFields(Dictionary<string, string> mergeData, ContractorData data)
        {
            if (data.ContractorType == "ООО")
            {
                // Для ООО
                mergeData["доп_поле_1"] = "ОГРН";
                mergeData["ОГРН_Паспорт"] = data.OGRN ?? "";
                mergeData["доп_поле_2"] = "ИНН/КПП";
                mergeData["ИНН_Паспорт"] = data.INN_KPP ?? "";
                mergeData["доп_поле_3"] = "Расчетный счёт";
                mergeData["счет_паспорт"] = data.PaymentAccount ?? "";
                mergeData["доп_поле_4"] = "Наименование банка";
                mergeData["Банк_паспорт"] = data.BankName ?? "";
                mergeData["доп_поле_5"] = "БИК";
                mergeData["БИК_ИНН"] = data.BIK ?? "";
                mergeData["доп_поле_6"] = "Кор/счет";
                mergeData["Корсчет"] = data.CorrespondentAccount ?? "";
            }
            else if (data.ContractorType == "ИП" || data.ContractorType == "ФЛ")
            {
                // Для ИП/ФЛ
                mergeData["доп_поле_1"] = "Паспорт серия и №";
                mergeData["ОГРН_Паспорт"] = data.Passport ?? "";
                mergeData["доп_поле_2"] = "Орган выдачи";
                mergeData["ИНН_Паспорт"] = data.PassportIssuedBy ?? "";
                mergeData["доп_поле_3"] = "Дата выдачи";
                mergeData["счет_паспорт"] = data.PassportIssueDate ?? "";
                mergeData["доп_поле_4"] = "Код подразделения";
                mergeData["Банк_паспорт"] = data.PassportDepartmentCode ?? "";
                mergeData["доп_поле_5"] = "ИНН";
                mergeData["БИК_ИНН"] = data.PersonINN ?? "";
                mergeData["доп_поле_6"] = "";
                mergeData["Корсчет"] = "";
            }
        }

        // Вспомогательный класс для хранения данных контрагента
        private class ContractorData
        {
            public int Id { get; set; }
            public string ContractNumber { get; set; }
            public string ContractorType { get; set; }
            public string FullName { get; set; }
            public string PropertyType { get; set; }
            public string Address { get; set; }
            public string CadNumber { get; set; }
            public string SignalType { get; set; }
            public decimal MonthlyAmount { get; set; }
            public string BuildingType { get; set; }

            // ООО
            public string ResponsiblePerson { get; set; }
            public string OGRN { get; set; }
            public string INN_KPP { get; set; }
            public string PaymentAccount { get; set; }
            public string BankName { get; set; }
            public string BIK { get; set; }
            public string CorrespondentAccount { get; set; }

            // ИП/ФЛ
            public string Passport { get; set; }
            public string PassportIssuedBy { get; set; }
            public string PassportIssueDate { get; set; }
            public string PassportDepartmentCode { get; set; }
            public string PersonINN { get; set; }

            // Контакты
            public string Email { get; set; }
            public string Phone { get; set; }
        }
    }
}
