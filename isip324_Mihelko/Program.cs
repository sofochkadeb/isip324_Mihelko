using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace isip324_Mihelko
{
    internal class Program
    {
    class Programs
        {
            static void Main(string[] args)
            {
                var names = new List<string>();
                var prices = new List<double>();
                int n;
                do
                {
                    Console.WriteLine("Введите количество операций (2-40): ");
                } while (!int.TryParse(Console.ReadLine(), out n) || n < 2 || n > 40);
                for (int i = 0; i < n; i++)
                {
                    Console.WriteLine($"Трата #{i + 1} (Название; Сумма): ");
                    string line = Console.ReadLine();
                    string[] parts = line.Split(';');
                    if (parts.Length != 2)
                    {
                        Console.WriteLine("Неверный формат. Повторите. ");
                        i--;
                        continue;
                    }
                    double price;
                    if (!double.TryParse(parts[1].Trim(), out price))
                    {
                        Console.WriteLine("Сумма должна быть числом. Повторите. ");
                        i--;
                        continue;
                    }
                    names.Add(parts[0].Trim());
                    prices.Add(price);
                    int choice;
                    do
                    {
                        Console.WriteLine("1. Вывод данных");
                        Console.WriteLine("2. Статистика");
                        Console.WriteLine("3. Сортировка по цене");
                        Console.WriteLine("4. Конвертация валюты");
                        Console.WriteLine("5. Поиск по названию");
                        Console.WriteLine("0. Выход");
                        Console.Write("Выбор: ");
                        if (!int.TryParse(Console.ReadLine(), out choice))
                            choice = -1;
                        switch (choice)
                        {
                            case 1: ShowData(names, prices); break;
                            case 2: ShowStats(prices); break;
                            case 3: BubbleSort(names, prices); break;
                            case 4: ConvertCurrency(names, prices); break;
                            case 5: SearchByName(names, prices); break;
                            case 0: Console.WriteLine("Выход..."); break;
                            default: Console.WriteLine("Неверный пункт. "); break;
                        }
                    } while (choice != 0);
                }
            }
            static void ShowData(List<string> names, List<double> prices)
            {
                if (names.Count == 0)
                {
                    Console.WriteLine("Список пуст.");
                    return;
                }
                for (int i = 0; i < names.Count; i++)
                {
                    Console.WriteLine((i + 1) + ". " + names[i] + " - " + prices[i] + "руб.");
                }
            }
            static void ShowStats(List<double> prices)
            {
                if (prices.Count == 0)
                {
                    Console.WriteLine("Список пуст.");
                    return;
                }
                double sum = 0;
                double min = prices[0];
                double max = prices[0];
                for (int i = 0; i < prices.Count; i++)
                {
                    sum = sum + prices[i];
                    if (prices[i] > max) max = prices[i];
                    if (prices[i] < min) min = prices[i];
                }
                Console.WriteLine("Сумма: ", sum, "руб.");
                Console.WriteLine("Среднее: ", (sum / prices.Count), "руб.");
                Console.WriteLine("Максимум: ", max, "руб.");
                Console.WriteLine("Минимум: ", min, "руб.");
            }
            static void BubbleSort(List<string> names, List<double> prices)
            {
                for (int i=0; i<prices.Count-1; i++)
                {
                    for (int j=0; i<prices.Count-1-i;j++)
                    {
                        if (prices[j] > prices[j+1])
                        {
                            double tmpP = prices[j];
                            prices[j] = prices[j+1];
                            prices[j+1] = tmpP;

                            string tmpN = names[j];
                            names[j] = names[j+1];
                            names[j+1] = tmpN;
                        }
                    }
                }
                Console.WriteLine("Отсортировано. ");
                ShowData(names, prices);
            }
            static void ConvertCurrency(List<string> names, List<double> prices)
            {
                Console.WriteLine("1. USD (курс 90)");
                Console.WriteLine("2. EUR (курс 100");
                Console.WriteLine("3. Свой курс");
                Console.WriteLine("Выбор: ");

                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                    return;

                double rate=1;
                if (choice == 1)
                    rate = 90;
                else if (choice == 2)
                    rate = 100;
                else if (choice == 3)
                {
                    Console.Write("Введите свой курс: ");
                    if (!double.TryParse(Console.ReadLine(), out rate))
                        return;
                }
                else
                {
                    Console.WriteLine("Неверно. ");
                    return;
                }
                for (int i = 0; i<prices.Count; i++)
                {
                    prices[i] = prices[i]/rate;
                }
                Console.WriteLine("Готово. ");
                ShowData(names, prices);
            }
            static void SearchByName(List<string> names, List<double> prices)
            {
                Console.WriteLine("Что ищем: ");
                string query = Console.ReadLine();
                bool found = false;
                for (int i = 0; i < names.Count; i++)
                {
                    if (names[i].ToLower().Contains(query.ToLower()))
                    {
                        Console.WriteLine(names[i], " - ", prices[i], "руб.");
                        found = true;
                    }
                    else if (found == false)
                    {
                        Console.WriteLine("Ничего не найдено. ");
                    }
                }
            }

          }
       }
    }
           
        
    

