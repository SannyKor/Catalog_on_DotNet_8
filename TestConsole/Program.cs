using System;
using System.Text;

class TestEncoding
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Console.WriteLine("Введіть українське слово (наприклад, 'Привіт'):");
        string input = Console.ReadLine();

        Console.WriteLine($"Ви ввели: '{input}'");
        Console.WriteLine($"Довжина рядка: {input.Length}");
        Console.WriteLine($"Байтів у UTF-8: {Encoding.UTF8.GetBytes(input).Length}");

        // Додаткова перевірка: виведення кожного символу та його ASCII значення
        Console.WriteLine("Деталі символів:");
        foreach (char c in input)
        {
            Console.WriteLine($"Символ: '{c}', Unicode: {(int)c:X4}"); // Виведе Unicode значення символу
        }

        Console.ReadKey(); // Залишити консоль відкритою
    }
}
