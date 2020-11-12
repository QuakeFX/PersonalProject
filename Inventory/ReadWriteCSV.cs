using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using Geevers.Infrastructure;

namespace Inventory
{
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
        }

        // PROPERTIES

        public string Filepath
        {
            get { return filepath; }
            set
            {
                if (value == null || value.IndexOf('.') <= 0)
                {
                    throw new ArgumentException("Invalid filepath", "filepath");
                }
                else
                {
                    //extract filepath
                    int p = value.IndexOf('.');
                    int q = value.LastIndexOf('/');
                    if (q == -1) { q = 0; }
                    string path = value.Substring(q, p - q);

                    if (string.IsNullOrWhiteSpace(path.Replace('/', ' ')))
                    {
                        throw new ArgumentException("Invalid filepath", "filepath");
                    }
                }

                string extension = value.Substring(value.IndexOf('.'), value.Length - value.IndexOf('.'));

                if (extension == ".csv" || extension == ".txt")
                {
                    filepath = value;
                }
                else
                {
                    throw new ArgumentException("Invalid filepath extension - only '.csv' and '.txt' are supported", "filepath");
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
                    throw new ArgumentException("Only ';' and ',' seperators are supported", "seperator");
                }
            }
        }

        // METHODS

        public enum WriteTo { Main, Temp }

        public Response<string> WriteLineToFile(string line, WriteTo mode = WriteTo.Main)
        {
            // returns Created if record was succesfully created
            // returns BadRequest if string contains dangerous characters or is null
            // returns InternalServerError if for some reason record could not be created

            if (line == null)
            {
                throw new ArgumentNullException("line", "The line to be written to file cannot be null");
            }
            if (ContainsDangerousCharacters(line))
            {
                return HttpStatusCode.BadRequest;
            }
            try
            {
                string destination = filepath;
                if (mode == WriteTo.Main) { destination = filepath; }
                if (mode == WriteTo.Temp) { destination = tempFile; }
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@destination, true))
                {
                    file.WriteLine(RemoveUnnecessarySeperators(line));
                    return HttpStatusCode.Created;
                }
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
                throw;
            }
        }

        public bool ContainsDangerousCharacters(string input)
        {
            // returns True if string contains CR or Tab
            // returns False if string is safe

            if (input == null)
            {
                throw new ArgumentNullException("input", "Your input cannot be null");
            }
            if (input.Contains("\n") || input.Contains("\t"))
            { return true; } else { return false; }
        }

        public string RemoveUnnecessarySeperators(string input)
        {
            // returns a string without right-hand seperator characters for empty fields
            // input;a;b;;;; -> input;a;b

            if (input == null)
            {
                throw new ArgumentNullException("input", "Your input cannot be null");
            }
            string output = "";
            int i = input.Length;
            while (i > 0)
            {
                i--;
                if (input[i] != seperator)
                {
                    output = input.Substring(0, i + 1); //make single operation
                    break;
                }
            }
            return output;
        }

        public string ParseStringArrayToString(string[] input)
        {
            // returns a [seperated] string from a string array
            // { "input", "to", "output" } -> "input;to;output"

            if (input == null || input.Length == 0 || input.All(item => item == null))
            {
                throw new ArgumentNullException("input", "Your input array cannot be null");
            }
            string output = "";
            foreach (string field in input)
            {
                output += field + seperator;
            }
            return output.Remove(output.Length - 1);
        }

        public Response<string> AddRecord(string fieldOne, string fieldTwo, string fieldThree)
        {
            // returns Created if record was succesfully created
            // returns BadRequest if input contains dangerous characters
            // returns InternalServerError if for some reason record could not be created

            if (fieldOne == null && fieldTwo == null && fieldThree == null)
            {
                throw new ArgumentNullException("fieldOne && fieldTwo && fieldThree", "Your fields cannot be null");
            }
            string line = fieldOne + seperator + fieldTwo + seperator + fieldThree;
            return WriteLineToFile(line);
        }

        public Response<string> AddRecord(string[] newRecord)
        {
            // returns Created if record was succesfully created
            // returns BadRequest if input array contains danagerous characters
            // returns InternalServerError if for some reason record could not be created

            if (newRecord == null || newRecord.Length == 0 || newRecord.All(item => item == null))
            {
                throw new ArgumentNullException("input", "Your input array cannot be null");
            }
            string line = ParseStringArrayToString(newRecord);
            return WriteLineToFile(line);
        }

        public Response<string> EditRecord(string key, int column, string fieldOne, string fieldTwo, string fieldThree)
        {
            // returns OK if record was edited
            // returns NotFound if key was not found
            // returns InternalServerError if for some reason the record could not be edited

            if (fieldOne == null && fieldTwo == null && fieldThree == null)
            {
                throw new ArgumentNullException("fieldOne && fieldTwo && fieldThree", "Your fields cannot be null");
            }
            string line = fieldOne + seperator + fieldTwo + seperator + fieldThree;
            return RewriteFile(key, column, RemoveUnnecessarySeperators(line));
        }

        public Response<string> EditRecord(string key, int column, string[] newRecord)
        {
            // returns OK if record was edited
            // returns NotFound if key was not found
            // returns InternalServerError if for some reason the record could not be edited

            if (newRecord == null || newRecord.Length == 0 || newRecord.All(item => item == null))
            {
                throw new ArgumentNullException("newRecord", "Your newRecord array cannot be null");
            }
            string line = ParseStringArrayToString(newRecord);
            return RewriteFile(key, column, RemoveUnnecessarySeperators(line));
        }

        public Response<string> DeleteRecord(string key, int column)
        {
            // returns OK if record was deleted
            // returns NotFound if key was not found
            // returns InternalServerError if for some reason the record could not be edited

            string newRecord = null;
            return RewriteFile(key, column, newRecord);
        }

        private Response<string> RewriteFile(string key, int column, string newRecord)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key", "Your key cannot be null");
            }
            if (column < 1)
            {
                throw new ArgumentOutOfRangeException("column", "CSV column range starts at 1! You cannot use zero or negative");
            }
            bool found = false;
            bool error = false;
            try
            {
                File.Delete(@tempFile);
                string[] lines = System.IO.File.ReadAllLines(filepath);
                foreach (var line in lines)
                {
                    string[] fields = line.Split(seperator);
                    if (fields.Length < column)
                    {
                        if (!WriteLineToFile(line, WriteTo.Temp).IsSuccessStatusCode) { error = true; }
                    }
                    else if (fields[column - 1] != key)
                    {
                        if (!WriteLineToFile(line, WriteTo.Temp).IsSuccessStatusCode) { error = true; }
                    }
                    else if (fields[column - 1] == key)
                    {
                        found = true;
                        if (newRecord != null)
                        {
                            if (!WriteLineToFile(newRecord, WriteTo.Temp).IsSuccessStatusCode) { error = true; }
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

        public Response<bool> RecordExists(string key, int column) //WIP
        {
            // returns True if key exists
            // returns False if key could not be found
            // returns InternalServerError if for some reason the file could not be read

            if (key == null)
            {
                throw new ArgumentNullException("key", "Your key cannot be null");
            }
            if (column < 1)
            {
                throw new ArgumentOutOfRangeException("column", "CSV column range starts at 1! You cannot use zero or negative");
            }
            return ReadRecord(key, column).Result != null ? true : false;
        }

        public Response<string> ReadRecord(string key, int column)
        {
            // returns String if key exists
            // returns NotFound if key could not be found
            // returns InternalServerError if for some reason the file could not be read

            if (key == null)
            {
                throw new ArgumentNullException("key", "Your key cannot be null");
            }
            if (column < 1)
            {
                throw new ArgumentOutOfRangeException("column", "CSV column range starts at 1! You cannot use zero or negative");
            }
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filepath);
                foreach (string line in lines)
                {
                    string[] fields = line.Split(seperator);
                    if ((fields.Length >= column) && (fields[column - 1] == key))
                    {
                        return line;
                    }
                }
                return HttpStatusCode.NotFound;
            }
            catch (Exception)
            {
                return HttpStatusCode.InternalServerError;
                throw;
            }
        }

        public Response<string> Replace(string key, int column, string value)
        {
            // returns OK if key was edited
            // returns NotFound if key could not be found
            // returns InternalServerError if for some reason the file could not be read

            if (key == null)
            {
                throw new ArgumentNullException("key", "Your key cannot be null");
            }
            if (column < 1)
            {
                throw new ArgumentOutOfRangeException("column", "CSV column range starts at 1! You cannot use zero or negative");
            }
            var readingRecord = ReadRecord(key, column);
            if (readingRecord.IsSuccessStatusCode)
            {
                string[] fields = readingRecord.Result.Split(seperator);
                fields[column - 1] = value;
                return EditRecord(key, column, fields);
            }
            return readingRecord.Status;
        }

        public Response<List<string[]>> ReadFile()
        {
            string[] lines = System.IO.File.ReadAllLines(filepath);
            var list = new List<string[]>();
            foreach (string line in lines)
            {
                list.Add(line.Split(seperator));
            }
            return list;
        }

        public void SaveDrawing(List<string> rows)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filepath, true))
            {
                foreach (string row in rows)
                {
                    file.WriteLine(row);
                }
            }
        }
        public List<string> LoadDrawing()
        {
            var list = new List<string>();
            string[] lines = System.IO.File.ReadAllLines(filepath);
            foreach (string row in lines)
            {
                list.Add(row);
            }
            return list;
        }
    }
}