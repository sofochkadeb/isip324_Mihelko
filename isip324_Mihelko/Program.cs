using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace isip324_Mihelko
{
    internal class Program
    {
        enum Genre //создается перечисление жанров книг
        {
            Fiction,
            Fantasy,
            Detective,
            ScienceFiction
        };
        class Book //класс книга
        {
            private int _id; //приватное поле и свойство id книги, только геттер, чтобы нельзя было менять
            public int Id { get => _id; } //_id не меняется после создания книги, создали 3 книги — есть 3 разных поля _id (в каждом объекте своё)
            private string _title; // поле и свойство названия книги, с выбросом исключения чтобы не было пустым поле названия
            public string Title 
            { 
              get { return _title; }
              set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("У книги должно быть название");
                    }
                    else
                    {
                        _title = value;
                    }
                }         
            }
            private string _author; // поле и свойство автора книги, с выбросом исключения чтобы не было пустым поле автора
            public string Author
            {
                get { return _author; }
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("У книги должен быть автор. ");
                    }
                    else
                    {
                        _author = value;
                    }
                }
            }
            public Genre BookGenre { get; set; } //свойство для перечисления
            private int _year; // поле и свойство года выпуска книги, с выбросом исключения чтобы год не был меньше чем 1000
            public int Year
            {
                get
                { return _year; }
                set
                {
                    if (value >= 1000 && value <= 2026)
                    {
                        _year = value;
                    }
                    else
                    {
                        throw new ArgumentException("Год издания книги должен быть в диапазоне от 1000 г. до 2026 г."); 
                    }
                }
            }
            private decimal _price; //поле и свойство цены книги, с выбросом исключения о том, что цена книги не может быть меньше 0
            public decimal Price
            {
                get {  return _price; }
                set
                {
                    if (value < 0)
                    {
                        throw new ArgumentException("Цена книги не может быть отрицательной");
                    }
                    else
                    {
                        _price = value;
                    }
                }
            }
            public Book(Genre genre, int year, decimal price, string author, string title, int id) //конструктор класса Book
            {
                Price = price;
                Author = author;
                Title= title;
                Year = year;
                BookGenre = genre;
                _id = id;
            }
            public override string ToString() => $"[{Id}] \"{Title}\" — {Author} ({BookGenre}, {Year}) — {Price} руб."; 
            //метод ToString, чтобы одной строчкой выводилась сущ информация о книге, override потому что по иерархии класс Book уже наследуется от класса Object
        }
        class LibraryService //класс для методов команд, которые можно делать с книгами(типа добавить, удалить)
        {
            private readonly List<Book> _books = new List<Book>(); //создается список _books на основе класса Book, но он приватный, только для чтения
            private int _nextId = 1; // поле экземпляра, «счётчик для выдачи номеров», он меняется каждый раз, когда добавляешь книгу
            public Book AddBook(string title,string author, decimal price, int year, Genre genre) //метод AddBook, который принимает от польз параметры класса Book
            {
                Book book = new Book(genre, year, price, author, title, _nextId); // создается экземпляр класса Book, берём значение счётчика и передаём его как Id новой книги
                _books.Add(book);
                _nextId++;
                //_nextId — «какой Id дать следующей книге».
                //_id — «Id, который уже присвоен этой конкретной книге».
                return book;
            }
            public List<Book> GetAll() //метод для возвращения всего списка книг
            {
                return _books;
            }
            public bool DeleteById(int id) //метод удаления через перебор параметра id
            {
                Book book = _books.FirstOrDefault(b => b.Id == id); //LINQ FirstOrDefault если нашел то ок, не нашел то null
                if (book == null)
                {
                    return false;
                }
                else
                {
                    _books.Remove(book); //удалить книгу
                    return true;
                }
            }
            public List<Book> SearchByTitle(string part) //после public ставится тот тип данных, который возвращает метод (return)
            {
                return _books.Where(b => b.Title.ToLower().Contains(part.ToLower())).ToList(); //список книг возьми, проверь где название в нижнем ргеистре содержит параметр часть в нижнем регистре, и запиши в список
            }
            public List<Book> SearchByAuthor(string part)
            {
                return _books.Where(b => b.Author.ToLower().Contains(part.ToLower())).ToList(); //аналогично с методом поиска по части названия для части автора
            }
            public List<Book> SearchByGenre(Genre genre) //возьми перечисление Genre, с параметром genre
            {
                return _books.Where(b => b.BookGenre == genre).ToList(); //найди в списке _books одинаковые элементы свойства BookGenre с параметром genre(взятый из перечисления) 
            }
            public List<Book> SortByTitle() //сортировка по названию по возрастанию (чтобы вернулся список)
            {
                return _books.OrderBy(b => b.Title).ToList();
            }
            public List<Book> SortByYear() //сортировка по году по возрастанию (чтобы вернулся список)
            {
                return _books.OrderBy(b => b.Year).ToList();
            }
            public Book GetMostExpensive() //вернуть первую самую дорогую книжку по убыванию (экземпляр одной книги)
            {
                return _books.OrderByDescending(b => b.Price).FirstOrDefault();
            }
            public Book GetMostCheapest() //вернуть первую самую дешевую книжку по возрастанию (экземпляр одной книги)
            {
                return _books.OrderBy(b => b.Price).FirstOrDefault();
            }
            public List<IGrouping<string, Book>> GroupByAuthor() //группа (интерфейс), у которой ключ — строка (имя автора), а содержимое — книги, показанная в виде списка
            {
                return _books.GroupBy(b => b.Author).ToList(); 
            }
        }

        static void Main(string[] args)
        {
            foreach (IGrouping<string, Book> in service.GroupByAuthor())
            {
                Console.WriteLine($"Автор: {group.Key} — {group.Count()} книг");
            }
        }
    }
}
