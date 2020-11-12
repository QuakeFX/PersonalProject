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
            string input4 = "back";
            string input5 = "comment that's so cool!";
            string input6 = "comment this is fire #fire #faka";
            var output1 = ConvertInput(input1);
            var output2 = ConvertInput(input2);
            var output3 = ConvertInput(input3);
            var output4 = ConvertInput(input4);
            var output5 = ConvertInput(input5);
            var output6 = ConvertInput(input6);

            Dictionary<Enum, string> test = new Dictionary<Enum, string>();
            test.Add(Input.Command, "goto");
            test.Add(Input.Dir, "axl.verheul");
            test.Add(Input.Comment, "that is fire!");
            test.Add(Input.Tags, "fire, water");

            string action = test[Input.Command];

            switch (action)
            {
                case "goto":
                    {
                        // Input.Dir
                        // user: "axl.verheul" (opt: @)
                        // item: "1005884" (TryParseInt32)
                        // tag: "fire" (indicate with #)
                        break;
                    }

                case "comment":
                    {
                        // Input.Comment
                        // "that is fire!" (insert into db)
                        // Input.Tags
                        // "fire, water" (insert into db)
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

        static List<string> ConvertInput(string input) //WIP
        {
            input = input.Trim();
            string[] words = input.Split(' ');
            string command = words[0];
            //if Length == 1, return
            //if (words.Length ==1) {return command; }

            string dir;
            if (words.Length == 2)
            {
                dir = words[1];
            }
            //if Length == 2, return
            //return dir;

            //words.Length = 3
            //comment cool! #like

            List<string> tags = new List<string>(); //turn this into string
            foreach (string str in words)
            {
                if (str.Substring(0, 1) == "#")
                {
                    tags.Add(str);
                }
            }
            // now we have tags;

            string comment = "";
            for (int i = 1; i < words.Length; i++)
            {
                if (!tags.Contains(words[i]))
                {
                    comment += words[i] + " ";
                }
            }
            comment = comment.Trim();
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