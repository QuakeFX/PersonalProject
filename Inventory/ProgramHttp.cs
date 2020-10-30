using System;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using Geevers.Infrastructure;

namespace Inventory
{
    class Program
    {
        static void notMain()
        {

        }
    }



    class ReadWriteCSV
    {
        static void Main()
        {
            Console.WriteLine("test 123");
            var c = SomeMethod("capslock");
            if (c.IsSuccessStatusCode)
            {
                Console.WriteLine("check");
                Console.WriteLine(c.Result);
            }
            else
            {
                Console.WriteLine(c.Status);
            }
        }

        public static Response<string> SomeMethod(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return HttpStatusCode.BadRequest;
            }
            return input.ToUpper();
        }
    }
}