using System;
using System.IO;
using System.Net;
using Geevers.Infrastructure;

namespace Inventory
{
    class Program
    {
        static void Main()
        {
            var csv = new ReadWriteCSV("/Users/axlve/OneDrive/Desktop/inventory.txt");
            csv.TestC();

        }
    }

    public class ReadWriteCSV
    {
        // FIELDS

        private string filepath;
        private char seperator;
        private string tempFile;

        // CONSTRUCTORS

        public ReadWriteCSV(string filepath, char seperator = ';')
        {
            Filepath = filepath;
            Seperator = seperator;
            //tempFile = "tempfile.txt";
            tempFile = "/Users/axlve/OneDrive/Desktop/temp.txt"; 

            if (filepath == Filepath && seperator == Seperator)
            {
                Console.WriteLine("Class succesfully initialized.");
            }
        }

        // PROPERTIES

        public string Filepath
        {
            get { return filepath; }
            set
            {
                if (value.IndexOf('.') <= 0)
                {
                    Console.WriteLine("Warning: invalid filepath.");
                }
                if (value.IndexOf('.') > 0)
                {
                    string extension = value.Substring(value.IndexOf('.'), value.Length - value.IndexOf('.'));

                    if (extension == ".csv" || extension == ".txt")
                    {
                        filepath = value;
                    }
                    else
                    {
                        Console.WriteLine("Warning: invalid filepath extension - only '.csv' and '.txt' are supported.");
                    }
                }
            }
        }

        public char Seperator
        {
            get { return seperator; }
            set
            {
                if (value == ';' || value == ',')
                {
                    seperator = value;
                }
                else
                {
                    Console.WriteLine("Warning: only ';' and ',' seperators are allowed.");
                }
            }
        }

        // METHODS

        public void Test()
        {
            Console.WriteLine("\n===testing ReadWriteCSV class===");
            
            // Instantiating
            Console.WriteLine("\n---testing ReadWriteCSV class: initialization---");

            Console.Write("InstanceA - correct : ");
            var instanceA = new ReadWriteCSV("/test.txt", ';');
            Console.Write("InstanceB - correct : ");
            var instanceB = new ReadWriteCSV("/test.csv", ',');
            Console.Write("InstanceC - invalid seperator : ");
            var instanceC = new ReadWriteCSV("/test.txt", ' ');
            Console.Write("InstanceD - missing file extension, invalid seperator : ");
            var instanceD = new ReadWriteCSV("/test", ' ');
            Console.Write("InstanceE - invalid file extension : ");
            var instanceE = new ReadWriteCSV("/test.png");
            Console.Write("InstanceF - missing file extension : ");
            var instanceF = new ReadWriteCSV("/test");

            // Method: AddRecord
            Console.WriteLine("\n---testing ReadWriteCSV class: AddRecord---");

            Console.Write("AddRecordA: ");
            var addRecordA = new ReadWriteCSV("test.txt");
            var responseA = addRecordA.AddRecord("a;b;c");
            Console.WriteLine(responseA.IsSuccessStatusCode);
            Console.WriteLine(responseA.Status);

            Console.Write("AddRecordB: ");
            var addRecordB = new ReadWriteCSV("test.bla");
            var responseB = addRecordB.AddRecord("a;b;c");
            Console.WriteLine(responseB.IsSuccessStatusCode);
            Console.WriteLine(responseB.Status);

            // Method: AddString
            Console.WriteLine("\n---testing ReadWriteCSV class: AddString---");

        }

        public void TestC()
        {
            var testC = new ReadWriteCSV("/Users/axlve/OneDrive/Desktop/test.txt");
            testC.AddRecord("header 1;header 2;header 3");
            string[] testC1 = new string[] { "field 1", "field 2", "field 3" };
            testC.AddString(testC1);
            Console.WriteLine("True : " + testC.RecordExists("field 3", 3).Result);
            Console.WriteLine("False : " + testC.RecordExists("field 3", 2).Result);
            Console.WriteLine("False : " + testC.RecordExists("field 3", 4).Result); //test for out of range
            Console.WriteLine("False : " + testC.RecordExists("field 4", 3).Result);

            testC.AddString(testC1); //duplicate row
            string[] testC2 = new string[] { "field A", "field B", "field C" };
            string[] testC3 = new string[] { "field 1A", "field 2B", "field 3C" };
            testC.EditRecord("field 3", 2, testC2);
            testC.EditRecord("field 3", 4, testC2); //test for out of range
            testC.EditRecord("field 3", 3, testC2); //correct
            testC.EditRecord("field 3", 3, testC3); //test for duplication
            
        }

        public Response<string> AddRecord(string line)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepath, true))
                {
                    file.WriteLine(line);
                    return HttpStatusCode.Created;
                }
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
                throw;
            }
        }

        public Response<string> AddTemp(string line)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@tempFile, true))
                {
                    file.WriteLine(line);
                }
                return HttpStatusCode.Created;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
                throw;
            }
        }

        public Response<string> AddString(string[] input, bool temp = false)
        {
            if (input.Length == 0)
            {
                return HttpStatusCode.BadRequest;
            }
            string line = "";
            for (int i = 0; i < input.Length; i++)
            {
                line += input[i];
                if (i < input.Length - 1) { line += seperator; }
            }

            if (temp == false)
            {
                return AddRecord(line);
            }
            else
            {
                return AddTemp(line);
            }
        }

        public Response<bool> RecordExists(string key, int column)
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filepath);
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] fields = lines[i].Split(seperator);

                    if (fields.Length >= column)
                    {
                        if (fields[column - 1] == key)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
                throw;
            }
        }

        public Response<string> EditRecord(string key, int column, string[] newRecord)
        {
            bool found = false;
            bool edited = false;
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filepath);
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] fields = lines[i].Split(seperator);

                    if (fields.Length < column)
                    {
                        AddTemp(lines[i]);
                    }
                    else
                    {
                        if (fields[column - 1] != key)
                        {
                            AddTemp(lines[i]);
                        }
                        else
                        {
                            found = true;
                            if (AddString(newRecord, true).Status == HttpStatusCode.Created)
                            {
                                edited = true;
                            }
                        }
                    }
                }
                if (edited)
                {
                    File.Delete(@filepath);
                    File.Move(tempFile, filepath);
                    return HttpStatusCode.OK;
                }
                else
                {
                    File.Delete(@tempFile);
                    if (found == false)
                    {
                        return HttpStatusCode.NotFound;
                    }
                    return HttpStatusCode.InternalServerError;
                }
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
                throw;
            }
        }

        public Response<string> DeleteRecord(string key, int column) //WIP
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filepath);
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] fields = lines[i].Split(seperator);

                    if (fields.Length < column)
                    {
                        AddTemp(lines[i]);
                    }
                    else
                    {
                        if (fields[column - 1] != key)
                        {
                            AddTemp(lines[i]);
                        }
                    }
                }
                File.Delete(@filepath);
                File.Move(tempFile, filepath);
                return HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
                throw;
            }

        }

    }
}