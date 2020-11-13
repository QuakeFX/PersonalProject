using System;
using System.Linq;
using System.Collections.Generic;
using Inventory;
using System.Data;

namespace Retrogram
{
    class Program
    {
        enum Input { Command, Dir, Comment, Tags }

        static void Main(string[] args)
        {
            Console.WriteLine("\n= = = R E T R O G R A M = = =\n");

            string input1 = "goto axl.verheul";
            string input2 = "goto 1006884";
            string input3 = "goto #hashtag";
            string input4 = "login";
            string input5 = "comment that's so cool! #";
            string input6 = "comment this is fire #fire #faka";
            var output1 = ConvertInput(input1);
            var output2 = ConvertInput(input2);
            var output3 = ConvertInput(input3);
            var output4 = ConvertInput(input4);
            var output5 = ConvertInput(input5);
            var output6 = ConvertInput(input6);

            //Dictionary<Enum, string> test = new Dictionary<Enum, string>();
            //test.Add(Input.Command, "goto");
            //test.Add(Input.Dir, "axl.verheul");
            //test.Add(Input.Comment, "that is fire!");
            //test.Add(Input.Tags, "fire, water");


            string[] input = input6.Trim().Split(' ');
            var dict = new Dictionary<Enum, string>();

            var query = input.Skip(1).Where(w => w.Substring(0, 1).Contains('#')).Select(w => w.Substring(1,w.Length-1));
            string abr = string.Join(",", query);
            Console.WriteLine(abr);
            var query2 = input.Skip(1).Where(w => !w.Substring(0, 1).Contains('#')).Select(w => w);
            string abd = string.Join(' ', query2);
            Console.WriteLine(abd);

            string[] tags = abr.Split(',');
            foreach (var item in abr)
            {
                Console.WriteLine("*" + item + "*");
            }


            string action = output1[Input.Command];

            switch (action)
            {
                case "goto":
                    {
                        // Input.Dir
                        // user: "axl.verheul" (opt: @)
                        // item: "1005884" (TryParseInt32)
                        // tag: "fire" (indicate with #)
                        Console.WriteLine("directory: " + output1[Input.Dir]);
                        break;
                    }

                case "comment":
                    {
                        // Input.Comment
                        // "that is fire!" (insert into db)
                        // Input.Tags
                        // "fire, water" (insert into db)
                        Console.WriteLine("comment: " + output1[Input.Comment]);
                        Console.WriteLine("tags: " + output1[Input.Tags]);
                        break;
                    }

                case "postnew":
                    {
                        //
                        break;
                    }

                case "login":
                    {
                        //
                        Console.WriteLine("login");
                        break;
                    }


                case "logout":
                    {
                        //
                        break;
                    }

                default:
                    {
                        Console.WriteLine("Command not recognized");
                        break;
                    }
            }

            //UI.Goto(112);

            /*
            var list = new List<string>();
            for (int i = 0; i < 6; i++)
            {
                list.Add(Console.ReadLine());
            }
            
            int itemID = 608823;
            
            var test = new ReadWriteCSV($"C:/Users/axlve/OneDrive/Desktop/RGRAM_{itemID}.csv", ';');
            test.SaveDrawing(list);

            Console.Clear();
            var render = test.LoadDrawing();
            foreach (string row in render)
            {
                Console.WriteLine(row);
            }
            */
        }

        static Dictionary<Enum,string> ConvertInput(string input) //WIP
        {
            var dict = new Dictionary<Enum, string>();

            input = input.Trim();
            string[] words = input.Split(' ');

            string command = words[0];
            dict.Add(Input.Command, command);
            if (words.Length == 1) 
            {
                return dict;
            }

            if (words.Length == 2)
            {
                dict.Add(Input.Dir, words[1]);
                return dict;
            }

            List<string> tagsList = new List<string>();
            //notes: use list for (comment) comparison, keep string clean for db

            string tags = "";
            foreach (string str in words)
            {
                if (str.Substring(0, 1) == "#")
                {
                    tagsList.Add(str);
                    if (str.Length > 1)
                    {
                        tags += str.Substring(1, str.Length - 1) + ", ";
                    }
                }
            }
            if (tags != "")
            {
                tags = tags.Substring(0, tags.Length - 2);
            }
            dict.Add(Input.Tags, tags);
            // now we have a list with (optional) tags

            string comment = "";
            for (int i = 1; i < words.Length; i++)
            {
                if (!tagsList.Contains(words[i]))
                {
                    comment += words[i] + " ";
                }
            }
            comment = comment.Trim();
            dict.Add(Input.Comment, comment);
            return dict;
            // now we have a comment

            // -----------------------------------
            // string > words (iterate)
            // ext: command
            // ext: dir
            // ext: tags
            // ext: comment
        }
    }
}