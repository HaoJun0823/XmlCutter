using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XmlCutter
{
    class Program
    {

        public static int ErrorNumber = 0;
        static void Main(string[] args)
        {

            
            Console.Title = Config.ConsoleTitle;
            Banner.Print();
            Console.WriteLine("Program execution...");
            Console.WriteLine("https://github.com/HaoJun0823/XmlCutter");
            foreach (string i in args)
            {
                Console.WriteLine("Args:"+i);
            }
            if (args.Length<=0)
            {
                Termination(-1, "Please drag your mod directory into this programs!");
            }



            DirectoryInfo ModDirectory = new DirectoryInfo(args[0]);
            if (ModDirectory == null)
            {
                Termination(-1, "I don't think " + args[0] + " is a directory.");
            }
            if (!ModDirectory.Exists)
            {
                Termination(-1, "I don't think "+args[0]+" is alive.");
            }
            else
            {
                Config.ModDirectory = ModDirectory;
                Config.TreeLogPath = Config.ModDirectory.FullName + @"\XmlCutter" + ".directory.tree." + DateTime.Now.Date.ToString("yyyymmdd") + DateTime.Now.ToString("hhmmss") + ".log";
            }


            
            Config.LoadConfig();
            Console.Title = Config.ConsoleTitle + " Config Get!";
            Console.WriteLine("Get Xml files from " + args[0]);
            
            Config.SearchDirectory(0,Config.ModDirectory,"*.xml");



            Console.WriteLine("Xml files Number:" + Config.XmlFileList.Count);

            Console.WriteLine("Do cut!");
            XmlCutter.Action();
            Console.WriteLine("Error: {0} ",ErrorNumber);
            Console.Title = Config.ConsoleTitle;
            Pause();
        }

        public static void Termination(int status,String message)
        {
            Console.Title = Config.ConsoleTitle + " [FAILD]";
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine ("[STATUS "+ status + "]:"+ message);
           
            Console.WriteLine("[FAILD]:Xml Cutter Exiting...");
            Console.ForegroundColor = ConsoleColor.Gray;
            Pause();
            System.Environment.Exit(status);
            
        }

        public static void Error(String message)
        {
            ErrorNumber++;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[Error]:" + message);
            Console.ForegroundColor = ConsoleColor.Gray;

        }

        public static void Pause()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Press any key to continue...");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ReadKey(true);
        }


    }
}
