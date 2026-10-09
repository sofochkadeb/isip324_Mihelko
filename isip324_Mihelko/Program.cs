using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace isip324_Mihelko
{
    internal class Program
    {
        enum Genre
        {
            Fiction,
            Fantasy,
            Detective,
            ScienceFiction
        };
        class Book
        {
            private int _id;
            public int Id { get => _id; }
            private string _title;
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
            private string _author;
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
            public Genre BookGenre { get; set; }
            private int _year;
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
            private decimal _price;
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
            public Book(Genre genre, int year, decimal price, string author, string title, int id)
            {
                Price = price;
                Author = author;
                Title= title;
                Year = year;
                BookGenre = genre;
                _id = id;
            }
            public override string ToString() => $"[{Id}] \"{Title}\" — {Author} ({BookGenre}, {Year}) — {Price} руб.";

        }


        static void Main(string[] args)
        {
           
        }
    }
}
