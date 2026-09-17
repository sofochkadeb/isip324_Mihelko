using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace isip324_Mihelko
{
    internal class Program
    {
        static void Main(string[] args)
        {
            namespace StoreApp
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
                    public bool InStock => Quantity > 0;

                }
             }
            

        }
    }
}
