using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
        public static void Register() //регистрация в приложении
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
        public static void Login() //метод входа в приложение
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
        public static void ShowProducts() //метод для показа вообще всех сущ товаров
        {
            Console.Clear();
            Console.WriteLine("-------КАТАЛОГ ТОВАРОВ--------");
            var products = Core.Context.Product.Where(p => p.IsActive == true).ToList();
            foreach ( var product in products )
            {
                Console.WriteLine($"ID: {product.Id} | Название: {product.Name} | Цена: {product.Price} руб.");
            }
            if (CurrentUser != null)
            {
                Console.Write("\nВведите ID товара, чтобы добавить в корзину (или 0 для возврата): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int productId) && productId > 0)
                {
                    AddToCart(productId); //метод добавления в корзину
                }
            }
            else
            {
                Console.WriteLine("\nНажмите любую клавишу для возврата...");
                Console.ReadKey();
            }
        }
        public static void AddToCart(int productId)  //метод добавления товара в корзину
        {
            //поиск по корзине: этот юзер положил этот товар
            var cartItem = Core.Context.CartItem.FirstOrDefault(p => p.UserId == CurrentUser.Id 
                    && p.ProductId == productId);
            if (cartItem != null) // если товар был ранее, прибавляем количество
            {
                cartItem.Quantity++;
            }
            else //если товар добавляют в корзину впервые
            {
                CartItem newItem = new CartItem(); //новый экземпляр класса
                newItem.ProductId = productId; // передаем айди продукта экземпляру
                newItem.UserId = CurrentUser.Id; // передаем айди юзера экземпляру
                newItem.Quantity = 1;
                Core.Context.CartItem.Add(newItem);
            }
            Core.Context.SaveChanges();
            Console.WriteLine("Товар добавлен в корзину!");
            Console.ReadKey();
        }
        public static void ShowCart() //метод для показа товаров в корзине + перекидывает на метод оформ заказа
        {
            Console.Clear();
            Console.WriteLine("-------МОЯ КОРЗИНА--------");
            var items = Core.Context.CartItem.Where(p => p.UserId == CurrentUser.Id).ToList();
            if (items.Count == 0)
            {
                Console.WriteLine("Корзина пустая!");
                Console.ReadKey();
                return;
            }
            decimal total = 0;
            foreach (var item in items)
            {
                decimal priceItem = item.Product.Price * item.Quantity;
                total += priceItem;
                Console.WriteLine($"ID: {item.ProductId} | Название: {item.Product.Name} " +
                    $"| Количество: {item.Quantity} | Цена: {item.Product.Price} руб.");           
            }
            Console.WriteLine($"Всего к оплате: {total} руб.");
            Console.WriteLine("1. Оформить всю корзину");
            Console.WriteLine("2. Купить один товар");
            Console.WriteLine("0. Назад");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Checkout(items); //передается весь список товаров
                    break;
                case "2":
                    Console.WriteLine("Введите ID товара, который хотите купить: ");
                    string inputId = Console.ReadLine();
                    if (int.TryParse(inputId, out int productId))
                    {
                        //ищем товар в корзине
                        var singleItem = items.FirstOrDefault(p => p.ProductId == productId);
                        if (singleItem != null)
                        {
                            //передаем список из этого одного товара
                            Checkout(new List<CartItem> { singleItem });
                        }
                        else
                        {
                            Console.WriteLine("Такого товара нет в корзине!");
                            Console.ReadKey();
                        }
                    }
                    break;
                case "0":
                    return;
            }
        }

        public static void Checkout(List<CartItem> itemsToBuy) //метод оформления заказа
        {
            Console.Clear();
            Console.WriteLine("-------ОФОРМЛЕНИЕ ЗАКАЗА--------");
            var pickuppoints = Core.Context.PickupPoint.Where(u => u.IsActive == true).ToList();
            
            Console.WriteLine("Доступные пункты выдачи (ПВЗ): ");
            foreach (var pp in pickuppoints)
            {
                Console.WriteLine($"ID: {pp.Id} | Адрес: {pp.Address}");
            }

            Console.WriteLine("\nВведите ID нужного ПВЗ: ");
            string inputPvz = Console.ReadLine();
            int ppId = int.Parse(inputPvz); //превращаем текст в число

            decimal total = 0;
            foreach (var item in itemsToBuy)
            {
                total += item.Product.Price * item.Quantity;
            }

            Order newOrder = new Order();
            newOrder.UserId = CurrentUser.Id;
            newOrder.PickupPointId = ppId;
            newOrder.CreatedAt = DateTime.UtcNow;
            newOrder.TotalPrice = total;

            Core.Context.Order.Add(newOrder);
            Core.Context.SaveChanges();

            //перенос товаров из корзины в позиции заказа orderProduct
            foreach (var item in itemsToBuy)
            {
                OrderProduct op = new OrderProduct();
                op.OrderId = newOrder.Id; //привязываем к созданному заказу 
                op.ProductId = item.ProductId; //какой товар
                op.Quantity = item.Quantity; //сколько штук
                op.PriceAtPurchase = item.Product.Price; //запоминаем цену на момент покупки
                Core.Context.OrderProduct.Add(op);
            }
            Core.Context.CartItem.RemoveRange(itemsToBuy); //очищение корзины от купленных товаров
            Core.Context.SaveChanges();

            Console.WriteLine($"\nЗаказ успешно оформлен! Сумма: {total} руб.");
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        public static void ShowOrders()
        {
            Console.Clear();
            Console.WriteLine("-------МОИ ЗАКАЗЫ--------");
            //берем все заказы текущего пользователя и сортируем по дате (от старых к новым)
            var orders = Core.Context.Order
            .Where(o => o.UserId == CurrentUser.Id)
            .OrderBy(o => o.CreatedAt)
            .ToList();

            if (orders.Count == 0)
            {
                Console.WriteLine("\nУ вас пока нет заказов.");
                Console.ReadKey();
                return;
            }

            //выводится каждый заказ
            foreach (var order in orders)
            {
                Console.WriteLine($"\n------------------------------------------");
                Console.WriteLine($"Заказ №{order.Id} | Дата: {order.CreatedAt:dd.MM.yyyy HH:mm}");
                Console.WriteLine($"Пункт выдачи: {order.PickupPoint.Address}");
                Console.WriteLine($"Итоговая сумма: {order.TotalPrice} руб.");
                Console.WriteLine($"Товары в заказе:");
                //берем товары, которые были в этом заказе
                var orderProducts = Core.Context.OrderProduct.Where(op => op.OrderId == order.Id).ToList();
                foreach (var op in orderProducts)
                {
                    Console.WriteLine($"  - {op.Product.Name} | {op.Quantity} шт. | {op.PriceAtPurchase} руб.");
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
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
                    Console.Clear();
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
                            ShowCart();
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
