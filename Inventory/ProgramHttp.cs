using System;
using System.Net;
using Geevers.Infrastructure;

namespace Inventory
{
    class ReadWriteCSVHTTP
    {
        static void notMain()
        {
            var c = SomeMethod("capslock");
            Console.WriteLine(c);
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
