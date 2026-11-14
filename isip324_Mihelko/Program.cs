using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace isip324_Mihelko
{
    internal class Program
    {
        static User CurrentUser = null;
        public static void Register()
        {
            Console.WriteLine("Введите логин: ");
            string login = Console.ReadLine();
            User existingUser = Core.Context.User.FirstOrDefault(u => u.Username == login);
            if (existingUser != null)
            {
                Console.WriteLine("Этот логин уже занят! Попробуйте другой.");
                Console.ReadKey(); 
                return; 
            }

            Console.WriteLine("Введите пароль: ");
            string parol1 = Console.ReadLine();
            Console.WriteLine("Повторите введенный пароль: ");
            string parol2 = Console.ReadLine();
            if (parol1 != parol2)
            {
                Console.WriteLine("Пароли не совпадают! Попробуйте снова...");
                Console.ReadKey();
                return;
            }
            else
            {
                User newUser = new User();
                newUser.Username = login;
                newUser.PasswordHash = parol1;
                newUser.CreatedAt = DateTime.UtcNow;

                Core.Context.User.Add(newUser);
                Core.Context.SaveChanges();
                Console.WriteLine("Успешно зарегистрированы! Теперь можете войти.");
                Console.ReadKey();
            }
        }
        public static void Login()
        {
            Console.WriteLine("Введите логин: ");
            string login = Console.ReadLine();
            Console.WriteLine("Введите пароль: ");
            string parol = Console.ReadLine();
            User existingUser = Core.Context.User.FirstOrDefault(u => u.Username == login && u.PasswordHash == parol);
            if (existingUser != null)
            {
                CurrentUser = existingUser;
                Console.WriteLine($"Успешный вход. Привет, {login}!");
                Console.ReadKey();
                return;
            }
            else
            {
                Console.WriteLine("Неверный логин или пароль. Попробуйте снова...");
                Console.ReadKey();                
            }
            
        }
        static void Main(string[] args)
        {
            while (true)
            {
                if (CurrentUser == null)
                {
                    Console.WriteLine("---------МЕНЮ---------");
                    Console.WriteLine("1. Войти в аккаунт");
                    Console.WriteLine("2. Зарегистрироваться");
                    Console.WriteLine("3. Посмотреть товары");
                    Console.WriteLine("0. Выход");
                    Console.WriteLine("Выберите действие: ");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            Login();
                            break;
                        case "2":
                            Register();
                            break;
                        case "3":
                            ShowProducts();
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Неверный ввод. Нажмите любую клавишу...");
                            Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine($"-------МЕНЮ ({CurrentUser.Username})-------");
                    Console.WriteLine("1. Посмотреть товары");
                    Console.WriteLine("2. Корзина");
                    Console.WriteLine("3. Заказы");
                    Console.WriteLine("4. Выйти из аккаунта");
                    Console.WriteLine("0. Выход");
                    Console.WriteLine("Выберите действие: ");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            ShowProducts();
                            break;
                        case "2":
                            ShowKorzina();
                            break;
                        case "3":
                            ShowOrders();
                            break;
                        case "4":
                            CurrentUser = null;
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Неверный ввод. Нажмите любую клавишу...");
                            Console.ReadKey();
                            break;
                    }
                }                               
            }
        }
    }
}
