using System;

namespace Inventory
{
    class Program
    {
        static void notMain()
        {
            DoSomething a = new DoSomething();
            BaseClass b = new BaseClass();
            SubClass c = new SubClass();

            //c.Test("capslock");
            //Console.WriteLine(c.Succes);
            //Console.WriteLine(c.Status);

            //c.Test2("Coca");
            //Console.WriteLine(c.Succes);
            //Console.WriteLine(c.Status);
            //Console.WriteLine(c.Result);

            //c.Test3("barcode");
            //Console.WriteLine(c.Succes);
            //Console.WriteLine(c.Status);
            //Console.WriteLine(c.Result);

            //c.Test4("Fritz");
            c.Test5("Pepsi");
            //c.Test6("Cherry");

        }
    }

    public class DoSomething
    {
        public void BroadcastString(string input)
        {
            Console.WriteLine(input.ToUpper());
        }

        public string AppendString(string input)
        {
            string output = input + " Cola";
            return output;
        }

        public void MatchString(string input, out bool match)
        {
            if (input == "barcode")
            {
                match = true;
            }
            else
            {
                match = false;
            }
        }

        public StateHandler AppendStringTurbo(string input)
        {
            StateHandler s = new StateHandler();
            string output = input + " Cola";
            s.Succes = true;
            s.Status = "string was appended";
            s.Result = output;
            return s;
        }

        public StateHandler AppendStringInterface<T>(string input) where T : IAmColaBrand
        {
            StateHandler s = new StateHandler();
            string output = input + " Cola";
            s.Succes = true;
            s.Status = "string was appended";
            s.Result = output;
            return s;
        }

        public T AppendStringGeneric<T>(string input) where T : IAmColaBrand, new()
        {
            T s = new T();
            string output = input + " Cola";
            s.Succes = true;
            s.Status = "string was appended";
            s.Result = output;
            return s;
        }

    }

    public class BaseClass : DoSomething
    {

        public string result;

        public string Result
        { get; set; }

        public BaseClass()
        {
            Result = "nothing";
        }

    }

    public class SubClass : BaseClass
    {
        public bool Succes
        { get; set; }

        public string Status
        { get; set; }

        public SubClass()
        {
            Succes = false;
            Status = "no status";
        }

        public void Test(string input)
        {
            BroadcastString(input);
            Succes = true;
            Status = "string was broadcasted";
        }

        public void Test2(string input)
        {
            Result = AppendString(input);
            Succes = true;
            Status = "string was appended";
        }

        public void Test3(string input)
        {
            MatchString(input, out bool match);
            Succes = match ? true : false;
            Status = match ? "The string was matched" : "The string was not matched";
            Result = match ? $"{input} == barcode" : $"{input} != barcode";
        }

        public void Test4(string input)
        {
            StateHandler s = AppendStringTurbo(input);
            Console.WriteLine(s.Succes);
            Console.WriteLine(s.Status);
            Console.WriteLine(s.Result);
        }

        public void Test5(string input) //where T : new()
        {
            StateHandler s = AppendStringGeneric<StateHandler>(input);
            Console.WriteLine(s.Succes);
            Console.WriteLine(s.Status);
            Console.WriteLine(s.Result);
        }

        public void Test6(string input) //where T : IAmColaBrand
        {
            StateHandler s = AppendStringInterface<StateHandler>(input);
            Console.WriteLine(s.Succes);
            Console.WriteLine(s.Status);
            Console.WriteLine(s.Result);
        }

}

    public class StateHandler : IAmColaBrand
    {
        public bool Succes
        { get; set; }
        public string Status
        { get; set; }
        public string Result
        { get; set; }

        public StateHandler()
        {
            Succes = false;
            Status = "no status";
            Result = null;
        }
    }

    public interface IAmColaBrand
    {
        bool Succes { get; set; }
        string Status { get; set; }
        string Result { get; set; }
    }
}