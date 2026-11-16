using System.Globalization;
using System.Net;
using System.Text.Json;
using HttpClient client = new()
{
    Timeout = TimeSpan.FromSeconds(10)
};
Console.WriteLine("Конвертер валют");
Console.WriteLine("Источник курсов: https://www.exchangerate-api.com (обновление раз в сутки).");
decimal currency = 0;
string v1 = "RUB";
string v2 = "USD";
int a = 1;
string menu = """
    1. Конвертировать валюту (1 -> 2)
    2. Выбрать валюту 1
    3. Выбрать валюту 2
    4. Ввести сумму
    0. Выход из конвертера
    """;
do
{
    Console.WriteLine();
    Console.WriteLine($"Выбрано: {v1} -> {v2}. Сумма: {currency}");
    Console.WriteLine(menu);
    Console.Write("Выберите опцию: ");
    string? input = Console.ReadLine();
    if (input is null)
    {
        break;
    }
    if (!int.TryParse(input, out int option))
    {
        Console.WriteLine("Введите номер пункта числом.");
        continue;
    }
    a = option;
    switch (a)
    {
        case 1:
        {
            if (currency <= 0)
            {
                Console.WriteLine("Сначала введите сумму через пункт 4.");
                break;
            }
            if (v1 == v2)
            {
                Console.WriteLine($"{currency} {v1} = {currency:F2} {v2}");
                break;
            }
            try
            {
                string url = $"https://open.er-api.com/v6/latest/{v1}";
                string json = await client.GetStringAsync(url);
                using JsonDocument data = JsonDocument.Parse(json);
                JsonElement root = data.RootElement;
                if (root.ValueKind != JsonValueKind.Object ||
                    !root.TryGetProperty("result", out JsonElement apiResult) ||
                    apiResult.ValueKind != JsonValueKind.String ||
                    apiResult.GetString() != "success")
                {
                    Console.WriteLine("API не смогло предоставить курсы валют.");
                    break;
                }
                if (!root.TryGetProperty("base_code", out JsonElement baseCode) ||
                    baseCode.ValueKind != JsonValueKind.String ||
                    baseCode.GetString() != v1)
                {
                    Console.WriteLine("API вернуло курсы для другой исходной валюты.");
                    break;
                }
                if (!root.TryGetProperty("rates", out JsonElement rates) ||
                    rates.ValueKind != JsonValueKind.Object ||
                    !rates.TryGetProperty(v2, out JsonElement targetRate) ||
                    targetRate.ValueKind != JsonValueKind.Number ||
                    !targetRate.TryGetDecimal(out decimal rate) ||
                    rate <= 0)
                {
                    Console.WriteLine($"API не вернуло корректный курс для {v2}.");
                    break;
                }
                decimal result = currency * rate;
                Console.WriteLine($"Курс: 1 {v1} = {rate} {v2}");
                Console.WriteLine($"Результат: {currency} {v1} = {result:F2} {v2}");
                if (root.TryGetProperty("time_last_update_utc", out JsonElement updateTime) &&
                    updateTime.ValueKind == JsonValueKind.String)
                {
                    Console.WriteLine($"Дата курса: {updateTime.GetString()}");
                }
            }
            catch (HttpRequestException error)
            {
                if (error.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    Console.WriteLine("Слишком много запросов к API. Попробуйте позже.");
                }
                else
                {
                    Console.WriteLine("Не удалось получить курс: проверьте интернет или попробуйте позже.");
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("API не ответило за 10 секунд. Попробуйте позже.");
            }
            catch (JsonException)
            {
                Console.WriteLine("API вернуло ответ, который не удалось прочитать.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Сумма слишком велика для расчёта. Введите меньшую сумму.");
            }
            break;
        }
        case 2:
        {
            Console.Write("Введите исходную валюту (RUB, USD, EUR): ");
            string? code = Console.ReadLine();
            if (code is null)
            {
                a = 0;
                break;
            }
            code = code.Trim().ToUpperInvariant();
            if (code != "RUB" && code != "USD" && code != "EUR")
            {
                Console.WriteLine("Выберите RUB, USD или EUR.");
                break;
            }
            v1 = code;
            break;
        }
        case 3:
        {
            Console.Write("Введите целевую валюту (RUB, USD, EUR): ");
            string? code = Console.ReadLine();
            if (code is null)
            {
                a = 0;
                break;
            }
            code = code.Trim().ToUpperInvariant();
            if (code != "RUB" && code != "USD" && code != "EUR")
            {
                Console.WriteLine("Выберите RUB, USD или EUR.");
                break;
            }
            v2 = code;
            break;
        }
        case 4:
        {
            Console.Write("Введите сумму (например, 1000.50 или 1000,50): ");
            string? amountInput = Console.ReadLine();
            if (amountInput is null)
            {
                a = 0;
                break;
            }
            amountInput = amountInput.Trim().Replace(',', '.');
            if (!decimal.TryParse(amountInput, NumberStyles.Number,
                    CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Введите положительную сумму числом.");
                break;
            }
            currency = amount;
            break;
        }
        case 0:
            break;
        default:
            Console.WriteLine("Выберите пункт от 0 до 4.");
            break;
    }
}
while (a != 0);
Console.WriteLine("Конвертер завершён.");
