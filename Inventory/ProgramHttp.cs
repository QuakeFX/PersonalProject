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
            //var csv = new ReadWriteCSV("/Users/axlve/OneDrive/Desktop/inventory.txt");
            //var csv = new ReadWriteCSV("/Users/axlverheul/Desktop/inventory.txt");

            string root = "a";
            string root2 = "A";

            bool result = root.Equals(root2, StringComparison.OrdinalIgnoreCase);
            bool areEqual = String.Equals(root, root2, StringComparison.OrdinalIgnoreCase);

            Console.WriteLine("a equals A : " + result);
            Console.WriteLine("a equals A : " + areEqual);
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
            tempFile = "tempfile.txt";
            //tempFile = "/Users/axlve/OneDrive/Desktop/temp.txt"; 

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
                    //Console.WriteLine("Warning: invalid filepath.");
                    throw new ArgumentException("Invalid filepath", "filepath");
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
                        //Console.WriteLine("Warning: invalid filepath extension - only '.csv' and '.txt' are supported.");
                        throw new ArgumentException("Invalid filepath extension - only '.csv' and '.txt' are supported", "filepath");
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
                    //Console.WriteLine("Warning: only ';' and ',' seperators are allowed.");
                    throw new ArgumentException("Only ';' and ',' seperators are supported", "seperator");
                }
            }
        }

        // METHODS

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