using System;

namespace Inventory
{
    class Class1
    {
        static void notMain()
        {
            Subclass.MethodA("monster");
            Subclass.MethodB(666);
        }
    }

    public class Baseclass
    {
        public void ToConsole(string input)
        {
            Console.WriteLine(input);
        }
    }

    public class Subclass : Baseclass
    {
        public void MethodA(string input)
        {
            ToConsole(input.ToUpper());
        }

        public void MethodB(int input)
        {
            ToConsole(input.ToString());
        }
    }
}