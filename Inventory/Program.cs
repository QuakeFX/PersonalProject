using System;

namespace Inventory
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Start!");
            var csv = new ReadWriteCSV("C:/Users/axlve/OneDrive/Desktop/inventory.txt", ';');
            string a = "123";
            string b = "456";
            string c = "789";
            string[] abc = new string[] { a, b, c };
            //Console.WriteLine(csv.AddRecord(c,b,a).Status);
        }
    }
}
