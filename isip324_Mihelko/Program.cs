using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace isip324_Mihelko
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var name = new List<string>();
            var price = new List<double>();
            int n;
            do
            {
                Console.WriteLine("Введите количество операций (2-40): ");
            } while (!int.TryParse(Console.ReadLine(), out n) || n < 2 || n > 40);
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"Трата #{i + 1} (Название; Сумма): ");
                string line = Console.ReadLine();
                string[] parts = line.Split(';');
            }
        }
    }
}
