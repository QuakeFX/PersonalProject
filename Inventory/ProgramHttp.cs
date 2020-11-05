using System;
using System.IO;
using System.Linq;
using System.Net;
using Geevers.Infrastructure;

namespace Inventory
{
    class Program
    {
        static void Main()
        {
            //var csv = new ReadWriteCSV("/Users/axlve/OneDrive/Desktop/inventory.txt");
            //var read = new ReadWriteCSV("/Users/axlve/OneDrive/Desktop/inventory-test.csv");
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

        private Response<string> AddLine(string line)
        {
            // returns Created if record was succesfully created
            // returns BadRequest if string contains dangerous characters
            // returns InternalServerError if for some reason record could not be created

            //if (line == null || ContainsDangerousCharacters(line))
            if (ContainsDangerousCharacters(line))
                {
                return HttpStatusCode.BadRequest;
            }
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepath, true))
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

            if (input.Contains("\n") || input.Contains("\t"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ContainsDangerousCharacters(string[] input)
        {
            // returns True if string contains CR or Tab
            // returns False if string is safe

            if (input.Contains("\n") || input.Contains("\t"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string RemoveUnnecessarySeperators(string input)
        {
            // returns a string without seperator characters for empty fields
            // input;a;b;;;; -> input;a;b

            if (input == null)
            {
                throw new ArgumentNullException("input", "Your input cannot be null");
            }
            if (input.Length == 0)
            {
                return "";
            }
            string output = "";
            int i = input.Length;
            while (i > 0)
            {
                i--;
                char c = input[i];
                if (c != seperator)
                {
                    output = input.Substring(0, i + 1);
                    break;
                }
            }
            return output;
        }

        public string ParseStringArrayToString(string[] input)
        {
            // returns a string from a string array
            // { "input", "to", "output" } -> "input;to;output"

            if (input == null || input.Length == 0 || input.All(item => item == null))
            {
                throw new ArgumentNullException("input", "Your input array cannot be null");
            }
            string line = "";
            for (int i = 0; i < input.Length; i++)
            {
                line += input[i];
                if (i < input.Length - 1) { line += seperator; }
            }
            return line;
        }

        public Response<string> AddRecord(string line)
        {
            // returns Created if record was succesfully created
            // returns BadRequest if input contains dangerous characters
            // returns InternalServerError if for some reason record could not be created

            if (line == null)
            {
                throw new ArgumentNullException("line", "Your input cannot be null");
            }
            return AddLine(line);
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
            return AddLine(line);
        }

        public Response<string> AddRecord(string[] input)
        {
            // returns Created if record was succesfully created
            // returns BadRequest if input array contains danagerous characters
            // returns InternalServerError if for some reason record could not be created

            if (input == null || input.Length == 0 || input.All(item => item == null))
            {
                throw new ArgumentNullException("input", "Your input array cannot be null");
            }
            string line = ParseStringArrayToString(input);
            return AddLine(line);
        }

        private Response<string> CopyLine(string line)
        {
            // returns Created if record was succesfully created
            // returns InternalServerError if for some reason record could not be created

            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@tempFile, true))
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

        private Response<string> EditRecord(string key, int column, string newRecord)
        {
            // returns OK if record was edited
            // returns NotFound if key was not found
            // returns InternalServerError if for some reason the record could not be edited

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
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] fields = lines[i].Split(seperator);

                    if (fields.Length < column)
                    {
                        if (!CopyLine(lines[i]).IsSuccessStatusCode) { error = true; }
                    }
                    else
                    {
                        if (fields[column - 1] != key)
                        {
                            if (!CopyLine(lines[i]).IsSuccessStatusCode) { error = true; }
                        }
                        else
                        {
                            found = true;
                            if (newRecord != null)
                            {
                                if (!CopyLine(newRecord).IsSuccessStatusCode) { error = true; }
                            }
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
            return EditRecord(key, column, RemoveUnnecessarySeperators(line));
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
            return EditRecord(key, column, RemoveUnnecessarySeperators(ParseStringArrayToString(newRecord)));
        }

        public Response<string> DeleteRecord(string key, int column)
        {
            // returns OK if record was edited
            // returns NotFound if key was not found
            // returns InternalServerError if for some reason the record could not be edited

            string newRecord = null;
            return EditRecord(key, column, newRecord);
        }

        public Response<bool> RecordExists(string key, int column)
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
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] fields = lines[i].Split(seperator);

                    if (fields.Length >= column)
                    {
                        if (fields[column - 1] == key)
                        {
                            return lines[i];
                        }
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
    }
}