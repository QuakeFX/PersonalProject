using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Inventory;
using System.Linq;

namespace Retrogram
{
    static class Database
    {
        private static ReadWriteCSV csv = new ReadWriteCSV("C:/Users/axlve/OneDrive/Desktop/Retrogram_tables_withType.csv", ';');
        const int userName = 0;
        const int itemID = 1;
        const int itemTitle = 2;
        const int itemCaption = 3;
        const int liked = 4;
        const int commentID = 5;
        const int text = 6;
        const int tags = 7;
        const int type = 8;
        const int date = 9;

        public static void Initialize()
        {
            Console.WriteLine("Database.Initialize");
            Console.WriteLine(csv.Filepath);
        }

        public static void QueryTable()
        {
            var data = csv.ReadFile().Result;
            foreach (var item in data)
            {
                Console.Write(item[userName] + "\t");
                Console.Write(item[itemID] + "\t");
                Console.Write(item[itemTitle] + "\t");
                Console.Write(item[itemCaption] + "\t");
                Console.Write(item[liked] + "\t");
                Console.Write(item[commentID] + "\t");
                Console.Write(item[text] + "\t");
                Console.Write(item[tags] + "\t");
                Console.Write(item[type] + "\t");
                Console.Write(item[date] + "\t");
                Console.WriteLine();
            }
        }

        public static List<Item> QueryItems()
        {
            var query = csv.ReadFile().Result.Skip(1)
                .Where(item => item[type] == "post")
                .Select(item => new Item 
                {
                    ItemID = Convert.ToInt32(item[itemID]),
                    ItemTitle = item[itemTitle],
                    ItemCaption = item[itemCaption],
                    UserName = item[userName],
                    Tags = item[tags].Split('#'),
                    Date = Convert.ToDateTime(item[date])
                })
                .ToList();
            return query;
        }

        public static List<Comment> QueryComments()
        {
            var query = csv.ReadFile().Result.Skip(1)
                .Where(item => item[type] == "comment")
                .Select(item => new Comment
                {
                    CommentID = Convert.ToInt32(item[commentID]),
                    ItemID = Convert.ToInt32(item[itemID]),
                    UserName = item[userName],
                    Text = item[text],
                    Tags = item[tags].Split('#'),
                    Date = Convert.ToDateTime(item[date])
                })
                .ToList();
            return query;
        }

        public static void QueryLikes()
        {

        }

        public static void QueryTags()
        {

        }
    }

    class Item
    {
        public int ItemID { get; set; }
        public string ItemTitle { get; set; }
        public string ItemCaption { get; set; }
        public string UserName { get; set; }
        public string[] Tags { get; set; }
        public DateTime Date { get; set; }

        public Item()
        {
            // empty constructor for property mapping
        }

        public Item(string itemID, string itemTitle, string itemCaption, string userName, string tags, string date)
        {
            ItemID = Convert.ToInt32(itemID);
            ItemTitle = itemTitle;
            ItemCaption = itemCaption;
            UserName = userName;
            Tags = tags.Split('#');
            Date = Convert.ToDateTime(date);
        }
    }

    class Comment
    {
        public int CommentID { get; set; }
        public int ItemID { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public string[] Tags { get; set; }
        public DateTime Date { get; set; }

    }


}
