Console.WriteLine("Конвертер валют");
decimal currency =0;
string v1 ="rub";
string v2 ="usd";
int a = 1;
string menu = ("1. Конвертировать валюту(1->2)\n2. Выбрать валюту 1\n3. Выбрать валюту 2\n0. Выход из конвертора");
do
{
    Console.WriteLine($"{menu}\nВыберите опцию: ");
    if (!int.TryParse(Console.ReadLine(), out int option))
    {
        Console.WriteLine($"{menu}\nВыберите опцию: ");
        continue;
    }
    a = option;
    switch(a)
    {
        case 1:

        case 2:
            Console.Write("Введите конвертируемый код валюты (rub,usd,eur): ");
            v1 = Console.ReadLine();
            break;
        case 3:
            Console.Write("Введите код валюты в которую хотите конвертировать (rub,usd,eur): ");
            v2 = Console.ReadLine();
            break;
        case 0:
        break;
    }
}
while(a > 0);