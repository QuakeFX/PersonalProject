using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Retrogram
{
    static class UI
    {
        // input query results
        // render different table styles
        // headers
        // instructions
        // handle layout
        // keep track of pages

        public static void GotoItem(int itemID)
        {
            RenderItem(itemID);
            Console.WriteLine();
            Console.WriteLine();
            RenderComments(itemID);
        }

        public static void GotoUser(string userName)
        {
            RenderUser(userName);

        }

        public static void GotoTag(string tag)
        {

        }

        static void RenderItem(int itemID)
        {
            var query = Database.QueryItems().Where(x => x.ItemID == itemID);
            foreach (var item in query)
            {
                Console.WriteLine("Title:\t\t" + item.ItemTitle + " ({0})", item.ItemID);
                Console.WriteLine("Caption:\t" + item.ItemCaption);
                Console.WriteLine("User:\t\t" + item.UserName);
                Console.WriteLine("Date:\t\t" + item.Date);
                Console.Write("Tags:\t\t");
                foreach (string tag in item.Tags)
                {
                    Console.Write("#" + tag + " ");
                }
            }

        }

        static void RenderUser(string userName)
        {/*
            var query = Database.QueryItems
                .Join(Database.QueryComments,
                userNameA => Item.UserName,
                userNameB => Comment.UserName,
                (userNameA, userNameB) => new { UserName = userNameA, UserName = userNameB });

            Console.WriteLine(query);

            Console.WriteLine("Profile:\t" + userName);
            Console.WriteLine();
            Console.WriteLine("Last seen:\t"); //add last date
        */}

        static void RenderItemsByUser(string userName)
        {
            var query = Database.QueryItems().Where(x => x.UserName == userName);


        }
        static void RenderItemsByTag(string tag)
        {
            var query = Database.QueryItems().Where(x => x.Tags.Contains(tag));

        }

        static void RenderComments(int itemID)
        {
            var query = Database.QueryComments().Where(x => x.ItemID == itemID);
            foreach (var comment in query)
            {
                Console.WriteLine("{0}\t{1}\t{2}", comment.UserName, comment.CommentID, comment.Text);
            }
        }
        
    }
}