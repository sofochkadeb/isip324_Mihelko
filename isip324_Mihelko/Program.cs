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
            Science
        };
        class Book
        {
            private int _id;
            public int Id { get => _id; }
            private string _title;
            public string Title { get => _title; set => _title = value; }
            private string _author;
            public string Author { get => _author; set => _author = value; }
            public Genre genre { get; set; }
            private int _year;
            public int Year
            {
                get
                {
                    return _year;
                }
                set
                {
                    if (value >= 1000 && value <= 2026)
                    {
                        _year = value;
                    }
                    else
                    {
                        
                    }
                }
            }
        }


        static void Main(string[] args)
        {
           
        }
    }
}
