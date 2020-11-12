using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Inventory
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Start!");
            var db = new ReadWriteCSV("C:/Users/axlve/OneDrive/Desktop/Retrogram_tables_withType.txt", ';');

            string[] lines = System.IO.File.ReadAllLines(db.Filepath);
            
            var list = new List<Comment>();

            int user = 0;
            int imageID = 1;
            int imageTitle = 2;
            int liked = 4;
            int type = 8;

            foreach (string line in lines)
            {
                //list.Add(row.Split(';'));
                string[] fields = line.Split(';');
                list.Add(new Comment (fields[user], fields[imageID], fields[imageTitle]));
            }

            var query = list.Where(c => c.ImageTitle != "");
            int caption = query.Count();

            foreach (var item in query)
            {
                Console.WriteLine(item.ImageTitle);
            }
            Console.WriteLine("count: " + caption);
            Console.ReadKey();

            // = = = = = =

            var anotherList = new List<string[]>();
            foreach (string line in lines)
            {
                anotherList.Add(line.Split(';'));
            }

            var anotherQuery = anotherList.Where(item => item[imageID] == "334")
                .Where(item => item[liked] == "TRUE")
                .Count();

            Console.WriteLine("Post #334 has {0} like(s)", anotherQuery);

            // = = = = = =

            var masterQuery = anotherList.Where(item => item[user] == "julie0815")
                .Select(item => new Comment(
                    item[user],
                    item[imageID],
                    item[imageTitle]
                ))
                .ToList();

            foreach(var item in masterQuery)
            {
                Console.WriteLine(item.ImageTitle);
            }


        }
        class FileToMemory
        {

        }

        class Comment
        {
            public string User { get; set; }
            public string ImageID { get; set; }
            public string ImageTitle {get; set;}

            public Comment(string user, string imageID, string imageTitle)
            {
                User = user;
                ImageID = imageID;
                ImageTitle = imageTitle;

            }

        }
    }
}