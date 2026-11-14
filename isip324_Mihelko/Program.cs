using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace isip324_Mihelko
{
    internal class Program
    {
        static User CurrentUser = null;
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
