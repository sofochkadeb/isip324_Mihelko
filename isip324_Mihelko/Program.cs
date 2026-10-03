using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace isip324_Mihelko
{
    internal class Program
    {
        static List<string> allStatistics;
        static void AnalyzeNewText()
        {
            Console.WriteLine("Введите текст: ");
            string text = Console.ReadLine();
            if (text.Length < 100)
            {
                Console.WriteLine("Пользователь должен ввести минимум 100 символов");
                return;
            }
            string[] words = text.SplitIntoWords(text)
            int WordCount = words.Length;
        }

        static void Main(string[] args)
        {
            bool work;
            work = true;
            while (work)
            {
                Console.WriteLine("---------МЕНЮ----------");
                Console.WriteLine("1. Новый текст");
                Console.WriteLine("2. Показать статистику");
                Console.WriteLine("0. Выход");
                Console.WriteLine("Выберите пункт меню: ");
                int choice;
                int.TryParse(Console.ReadLine(), out choice);
                if (choice == 1)
                {
                    AnalyzeNewText();
                }
                else if (choice == 2)
                {
                    ShowOldStatistics();
                }
                else if (choice == 0)
                {
                    work = false;
                }
                else
                {
                    Console.WriteLine("Такого выбора нет в меню.");
                }
            }
        }
    }
}
