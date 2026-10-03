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
        static List<string> allStatistics = new List<string>(); //для статистики создается отдельный список

        static string[] SplitIntoWords(string text) //метод разделения текста на слова 
        {
            List<string> words = new List<string>(); //создается список строк для слов
            string currentWord = ""; //сюда по букве будет текущее слово собираться
            for (int i = 0;i<text.Length; i++)
            {
                char c = text[i]; //символ i взять
                if (char.IsLetterOrDigit(c)) //если этот символ буква или цифра 
                {
                    currentWord += c; //добавь этот символ в текущее слово
                }
                else if (currentWord.Length > 0) //иначе если символ не буква и не цифра и там уже что то есть
                {
                    words.Add(currentWord); //добавь текущее слово в список слов
                    currentWord = ""; //обнулить, начать собирать следующее
                }
            }
            if (currentWord.Length > 0) //если в тек слове ещё что-то осталось — сохранить
            {
                words.Add(currentWord); //добавить последнее слово в список
            }
            return words.ToArray(); //вернуть из метода массив строк, а не список
        }

        static int CountSentences(string text) //метод для подсчета предложений
        {
            int count = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == "." || c == "!" || c == "?")
                {
                    count++;
                }
            }
            if (count == 0 && text.Length > 0)
            {
                count = 1;
            }
            return count;
        }

        static void CountVowelsAndConsonants(string text, out int vowels,out int consonants) //метод подсчета гласных и согласных в тексте
        {
            vowels = 0;
            consonants = 0;
            string vowelLetters = "аеёиоуыэюяaeiouy";
            string consonantLetters = "бвгджзйклмнпрстфхцчшщbcdfghjklmnpqrstvwxz";
            for (int i = 0; i<text.Length; i++)
            {
                char c = char.ToLower(text[i]);
                if (vowelLetters.IndexOf(c) >= 0) //если символ с встречается в заданной строке, то он плюсуется к кол-ву 
                {
                    vowels++;
                }
                else if (consonantLetters.IndexOf(c) >= 0)
                {
                    consonants++;
                }
            }
        }

        static Dictionary<char, int> CountLetterFrequency(string text) //метод статистики по частоте каждой буквы(он вернёт словарь, где ключ — символ, значение — целое число)
        {
            Dictionary<char, int> frequency = new Dictionary<char, int>(); //пустой словарь с именем frequency
            for (int i = 0; i < text.Length; i++)
            {
                char c = char.ToLower(text[i]);
                if (char.IsLetter(c)) //если символ буква
                {
                    if (frequency.ContainsKey(c)) //если словарь уже содержит ключ с (метод возвращает true, если такая буква уже встречалась раньше)
                    {
                        frequency[c]= frequency[c]+1; //увеличить счетчик буквы на 1
                    }
                    else
                    {
                        frequency[c]= 1; //добавить новую букву со знач 1
                    }
                }
            }
            return frequency;
        }


            static void AnalyzeNewText() //метод анализа нового текста
            {
            Console.WriteLine("Введите текст: ");
            string text = Console.ReadLine();
            if (text.Length < 100)
            {
                Console.WriteLine("Пользователь должен ввести минимум 100 символов");
                return;
            }

            string[] words = SplitIntoWords(text); //применяется здесь метод деления текста на слова

            int WordCount = words.Length; //длина массива words (посчитать, сколько слов в массиве)

            string shortestWord = words[0]; //нахождение самого короткого и длинного слова через цикл for
            string longestWord = words[0];
            for (int i = 1; i < words.Length; i++) 
            {
                if (words[i].Length < shortestWord.Length)
                {
                    shortestWord = words[i];
                }
                if (words[i].Length > longestWord.Length)
                {
                    longestWord = words[i];
                }
            }

            int sentenceCount = CountSentences(text); //в переменную записывается кол-во предложений, через метод

            int vowels, consonants;
            CountVowelsAndConsonants(text, out vowels, out consonants); //в две разные переменные записываются кол-во гласных и согласных, через метод
        }


        static void Main(string[] args)
        {
            bool work;
            work = true;
            while (work)
            {
                Console.WriteLine("---------МЕНЮ----------"); //меню с проверкой выбора и самим выбором пользователя
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
