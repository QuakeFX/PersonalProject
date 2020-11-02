using System;
using System.Collections.Specialized;
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
            csv.Test();

            /*
            var a = SomeMethod("test");
            Console.WriteLine(a.IsSuccessStatusCode);
            Console.WriteLine(a.Result);
            Console.WriteLine(a.Status);
            */
        }

        public static Response<string> SomeMethod(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return HttpStatusCode.BadRequest;
            }
            //return input.ToUpper();
            return " ";
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
            int A = 1;
            int B = 1;
            int C = 1;
            int D = 0;

            Console.WriteLine("\n====== testing ReadWriteCSV.class ======");

            if (A == 1)
            {
                // Initializing
                Console.WriteLine("\n\n--- testing initialization ---");

                Console.Write("\nA1 correct :\t");
                var A1 = new ReadWriteCSV("/test.txt", ';');
                Console.Write("\nA2 correct :\t");
                var A2 = new ReadWriteCSV("/test.csv", ',');
                Console.Write("\nA3 invalid seperator :\t");
                var A3 = new ReadWriteCSV("/test.txt", ' ');
                Console.Write("\nA4 missing file extension, invalid seperator :\t");
                var A4 = new ReadWriteCSV("/test", ' ');
                Console.Write("\nA5 invalid file extension :\t");
                var A5 = new ReadWriteCSV("/test.png");
                Console.Write("\nA6 missing file extension :\t");
                var A6 = new ReadWriteCSV("/test");
            }

            if (B == 1)
            {
                // Method: AddRecord
                Console.WriteLine("\n\n--- testing AddRecord() ---");

                Console.Write("\nB1 correct :\t");
                var B1 = new ReadWriteCSV("test.txt").AddRecord("a;b;c");
                Console.WriteLine("Succes : " + B1.IsSuccessStatusCode);
                Console.WriteLine("Status : " + B1.Status);

                Console.Write("\nB2 invalid extension: ");
                var B2 = new ReadWriteCSV("test.bla").AddRecord("a;b;c");
                Console.WriteLine("Succes : " + B2.IsSuccessStatusCode);
                Console.WriteLine("Status : " + B2.Status);

                Console.Write("\nB3 null: ");
                var B3 = new ReadWriteCSV("test.txt").AddRecord(null);
                Console.WriteLine("Succes : " + B3.IsSuccessStatusCode);
                Console.WriteLine("Status : " + B3.Status);

                Console.Write("\nB4 empty: ");
                var B4 = new ReadWriteCSV("test.txt").AddRecord("");
                Console.WriteLine("Succes : " + B4.IsSuccessStatusCode);
                Console.WriteLine("Status : " + B4.Status);

                Console.Write("\nB5 whitespace: ");
                var B5 = new ReadWriteCSV("test.txt").AddRecord(" ");
                Console.WriteLine("Succes : " + B5.IsSuccessStatusCode);
                Console.WriteLine("Status : " + B5.Status);

            }

            if (C == 1)
            {
                // Method: AddString
                Console.WriteLine("\n\n--- AddString() ---");

                Console.Write("C1 correct :\t");
                string[] C12 = new string[] { "a", "b", "c" };
                var C1 = new ReadWriteCSV("test.txt").AddString(C12);
                Console.WriteLine("Succes : " + C1.IsSuccessStatusCode);
                Console.WriteLine("Status : " + C1.Status);

                Console.Write("\nC2 empty array :\t");
                string[] C22 = new string[] {};
                var C2 = new ReadWriteCSV("test.txt").AddString(C22);
                Console.WriteLine("Succes : " + C2.IsSuccessStatusCode);
                Console.WriteLine("Status : " + C2.Status);

                Console.Write("\nC3 array full of nothing \"\" :\t");
                string[] C32 = new string[] { "", "", "" };
                var C3 = new ReadWriteCSV("test.txt").AddString(C32);
                Console.WriteLine("Succes : " + C3.IsSuccessStatusCode);
                Console.WriteLine("Status : " + C3.Status);

                Console.Write("\nC4 missing filepath :\t");
                string[] C42 = new string[] { "1", "2", "3" };
                var C4 = new ReadWriteCSV("test").AddString(C42);
                Console.WriteLine("Succes : " + C4.IsSuccessStatusCode);
                Console.WriteLine("Status : " + C4.Status);

                Console.Write("\nC5 null :\t");
                string[] C52 = null;
                var C5 = new ReadWriteCSV("test.txt").AddString(C52);
                Console.WriteLine("Succes : " + C5.IsSuccessStatusCode);
                Console.WriteLine("Status : " + C5.Status);

            }

            if (D == 1)
            {
                var testD = new ReadWriteCSV("/Users/axlve/OneDrive/Desktop/test.txt");
                //testD.AddRecord("header 1;header 2;header 3");
                string[] testD1 = new string[] { "field 1", "field 2", "field 3" };
                //testC.AddString(testC1);
                Console.WriteLine("True : " + testD.RecordExists("field 3", 3).Result);
                Console.WriteLine("False : " + testD.RecordExists("field 3", 2).Result);
                Console.WriteLine("False : " + testD.RecordExists("field 3", 4).Result); //test for out of range
                Console.WriteLine("False : " + testD.RecordExists("field 4", 3).Result);

                //testC.AddString(testC1); //duplicate row
                string[] testD2 = new string[] { "field A", "field B", "field C" };
                string[] testD3 = new string[] { "field 1A", "field 2B", "field 3C" };
                //testC.EditRecord("field 3", 2, testC2);
                //testC.EditRecord("field 3", 4, testC2); //test for out of range
                testD.EditRecord("field 3", 3, testD2); //correct
                //testC.EditRecord("field 3", 3, testC3); //test for duplication

            }

        }

        

        public void TestC()
        {
            var testC = new ReadWriteCSV("/Users/axlve/OneDrive/Desktop/test.txt");
            //testC.AddRecord("header 1;header 2;header 3");
            string[] testC1 = new string[] { "field 1", "field 2", "field 3" };
            //testC.AddString(testC1);
            Console.WriteLine("True : " + testC.RecordExists("field 3", 3).Result);
            Console.WriteLine("False : " + testC.RecordExists("field 3", 2).Result);
            Console.WriteLine("False : " + testC.RecordExists("field 3", 4).Result); //test for out of range
            Console.WriteLine("False : " + testC.RecordExists("field 4", 3).Result);

            //testC.AddString(testC1); //duplicate row
            string[] testC2 = new string[] { "field A", "field B", "field C" };
            string[] testC3 = new string[] { "field 1A", "field 2B", "field 3C" };
            //testC.EditRecord("field 3", 2, testC2);
            //testC.EditRecord("field 3", 4, testC2); //test for out of range
            testC.EditRecord("field 3", 3, testC2); //correct
            //testC.EditRecord("field 3", 3, testC3); //test for duplication
            
        }

        public Response<string> AddRecord(string line)
        {
            // returns Created if record was succesfully created
            // returns BadRequest if string is null or empty
            // returns InternalServerError if for some reason record could not be created

            if (string.IsNullOrEmpty(line)) { return HttpStatusCode.BadRequest; }
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
            // returns Created if record was succesfully created
            // returns BadRequest if string is null or empty
            // returns InternalServerError if for some reason record could not be created

            if (string.IsNullOrEmpty(line)) { return HttpStatusCode.BadRequest; }
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@tempFile, true))
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

        public Response<string> AddString(string[] input, bool temp = false)
        {
            // returns Created if record was succesfully created
            // returns BadRequest if string is empty
            // returns InternalServerError if for some reason record could not be created

            if (input == null) { return HttpStatusCode.BadRequest; }
            //if (input.Length == 0) { return HttpStatusCode.BadRequest; }
            string line = "";
            for (int i = 0; i < input.Length; i++)
            {
                line += input[i];
                if (i < input.Length - 1) { line += seperator; }
            }
            if (temp) { return AddTemp(line); }
            return AddRecord(line);
        }

        public Response<bool> RecordExists(string key, int column)
        {
            // returns Result = true if key exists
            // returns Result = false if key could not be found
            // returns BadRequest if key is null or empty or if column is negative
            // returns InternalServerError if for some reason the file could not be read

            if (string.IsNullOrEmpty(key)) { return HttpStatusCode.BadRequest; }
            if (column < 0) { return HttpStatusCode.BadRequest; }
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
            // returns OK if record was edited
            // returns NotFound if key was not found
            // returns BadRequest if string[] is empty or column is negative
            // returns InternalServerError if for some reason the record could not be edited

            if (column < 0) { return HttpStatusCode.BadRequest; }
            if (newRecord.Length == 0) { return HttpStatusCode.BadRequest; }
            bool found = false;
            bool error = false;
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filepath);
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] fields = lines[i].Split(seperator);

                    if (fields.Length < column)
                    {
                        if (!AddTemp(lines[i]).IsSuccessStatusCode) { error = true; }
                    }
                    else
                    {
                        if (fields[column - 1] != key)
                        {
                            if (!AddTemp(lines[i]).IsSuccessStatusCode) { error = true; }
                        }
                        else
                        {
                            found = true;
                            if (!AddString(newRecord, true).IsSuccessStatusCode) { error = true; }
                        }
                    }
                }
                if (error) 
                {
                    File.Delete(@tempFile);
                    return HttpStatusCode.InternalServerError; 
                }
                if (found)
                {
                    File.Delete(@filepath);
                    File.Move(tempFile, filepath);
                    return HttpStatusCode.OK;
                }
                else
                {
                    File.Delete(@tempFile);
                    return HttpStatusCode.NotFound;
                }
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
                throw;
            }
        }

        public Response<string> DeleteRecord(string key, int column)
        {
            // returns OK if record was deleted
            // returns NotFound if key was not found
            // returns BadRequest if column is negative
            // returns InternalServerError if for some reason the record could not be deleted

            if (column < 0) { return HttpStatusCode.BadRequest; }
            bool deleted = false;
            bool error = false;
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filepath);
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] fields = lines[i].Split(seperator);

                    if (fields.Length < column)
                    {
                        if (!AddTemp(lines[i]).IsSuccessStatusCode) { error = true; }
                    }
                    else
                    {
                        if (fields[column - 1] != key)
                        {
                            if (!AddTemp(lines[i]).IsSuccessStatusCode) { error = true; }
                        }
                        else
                        {
                            deleted = true;
                        }
                    }
                }
                if (error)
                {
                    File.Delete(@tempFile);
                    return HttpStatusCode.InternalServerError;
                }
                if (deleted)
                {
                    File.Delete(@filepath);
                    File.Move(tempFile, filepath);
                    return HttpStatusCode.OK;
                }
                else
                {
                    File.Delete(@tempFile);
                    return HttpStatusCode.NotFound;
                }
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
                throw;
            }

        }

    }
}