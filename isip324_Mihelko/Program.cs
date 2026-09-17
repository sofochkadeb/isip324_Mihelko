using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace isip324_Mihelko
{
    internal class Program
    {
        public enum Category
        {
            Food,
            Electronics,
            Clothes,
            Household
        }
        public class Product
        {
            private static int _counter = 1000;
            public int Code { get; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public Category Category { get; set; }

            public Product(string name, decimal price, int quantity, Category category)
            {
                //проверка названия
                if (name == null || name.Trim() == "")
                {
                    throw new Exception("Ошибка: название не может быть пустым.");
                }
                //проверка цены
                if (price < 0)
                {
                    throw new Exception("Ошибка: цена не может быть отрицательной.");
                }
                //проверка количества
                if (quantity < 0)
                {
                    throw new Exception("Ошибка: количество не может быть отрицательным.");
                }

                //присваиваем значения
                Code = _counter;
                _counter = _counter + 1;
                Name = name.Trim();
                Price = price;
                Quantity = quantity;
                Category = category;
            }

            //есть ли товар на складе
            public bool InStock()
            {
                if (Quantity > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public void Print()
            {
                string stock;
                if (InStock() == true)
                {
                    stock = "Да (" + Quantity + " шт.)";
                }
                else
                {
                    stock = "Нет";
                }

                string line = "Код: " + Code
                    + " | Название: " + Name
                    + " | Цена: " + Price.ToString("F2") + " руб."
                    + " | Кол-во: " + Quantity
                    + " | Категория: " + Category
                    + " | В наличии: " + stock;

                Console.WriteLine(line);
            }
        }
        public class Store
        {
            //список товаров
            public List<Product> _products = new List<Product>();

            //получить список (для поиска)
            public List<Product> GetProducts()
            {
                return _products;
            }

            //ДОБАВЛЕНИЕ ПРОДУКТА
            public void AddProduct(string name, decimal price, int quantity, Category category)
            {
                try
                {
                    Product p = new Product(name, price, quantity, category);
                    _products.Add(p);
                    Console.WriteLine("Товар добавлен. Код: " + p.Code);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            //ПОИСК ПО КОДУ ТОВАРА
            public Product FindByCode(int code)
            {
                for (int i = 0; i < _products.Count; i++)
                {
                    if (_products[i].Code == code)
                    {
                        return _products[i];
                    }
                }
                return null;
            }

            //УДАЛЕНИЕ ПО КОДУ ТОВАРА
            public void DeleteProduct(int code)
            {
                Product p = FindByCode(code);
                if (p == null)
                {
                    Console.WriteLine("Товар с таким кодом не найден.");
                    return;
                }
                _products.Remove(p);
                Console.WriteLine("Товар удален.");
            }

            //ПОСТАВКА ТОВАРА
            public void OrderSupply(int code, int amount)
            {
                if (amount <= 0)
                {
                    Console.WriteLine("Количество товара должно быть больше 0");
                    return;
                }
                Product p = FindByCode(code);
                if (p == null)
                {
                    Console.WriteLine("Товар с таким кодом не найден.");
                    return;
                }
                p.Quantity = p.Quantity + amount;
                Console.WriteLine("Поставка оформлена. Теперь на складе" + p.Quantity + " шт.");
            }

            //ПРОДАЖА ТОВАРА
            public void SellProduct(int code, int amount)
            {
                if (amount <= 0)
                {
                    Console.WriteLine("Количество товара должно быть больше 0");
                    return;
                }
                Product p = FindByCode(code);
                if (p == null)
                {
                    Console.WriteLine("Товар с таким кодом не найден.");
                    return;
                }
                if (p.Quantity < amount)
                {
                    Console.WriteLine("Недостаточно товара. В наличии: " + p.Quantity + " шт.");
                    return;
                }
                p.Quantity = p.Quantity - amount;
                Console.WriteLine("Продано " + amount + " шт. Остаток: " + p.Quantity + " шт.");
            }

            //ПОИСК ПО НАЗВАНИЮ ТОВАРА
            public void FindByName(string part)
            {
                bool found = false;
                for (int i = 0; i < _products.Count; i++)
                {
                    Product p = _products[i];
                    if (p.Name.ToLower().Contains(part.ToLower()))
                    {
                        p.Print();
                        found = true;
                    }
                }
                if (found == false)
                {
                    Console.WriteLine("Ничего не найдено.");
                }
            }

            //ПОИСК ПО КАТЕГОРИИ ТОВАРА
            public void FindByCategory(Category category)
            {
                bool found = false;
                for (int i = 0; i < _products.Count; i++)
                {
                    Product p = _products[i];
                    if (p.Category == category)
                    {
                        p.Print();
                        found = true;
                    }
                }
                if (found == false)
                {
                    Console.WriteLine("Ничего не найдено");
                }
            }

            //ПЕЧАТЬ ВСЕХ ТОВАРОВ
            public void PrintAll()
            {
                if (_products.Count == 0)
                {
                    Console.WriteLine("Список товаров пуст.");
                    return;
                }
                for (int i = 0; i < _products.Count; i++)
                {
                    _products[i].Print();
                }
            }
        }
    }
}
