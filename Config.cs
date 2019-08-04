using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace XmlCutter
{
    class Config
    {

        public static string ConfigPath = AppDomain.CurrentDomain.BaseDirectory + "XmlCutter.xml";
        public static List<FileInfo> XmlFileList = new List<FileInfo>();
        public static List<XmlIndex> XmlIndexList = new List<XmlIndex>();
        public static DirectoryInfo ModDirectory;
        public static string TreeLogPath;
        public static string ConsoleTitle = "Xml Cutter";
        public static string XmlParentElementName;
        public static Dictionary<string,string> XmlParentElementAttr = new Dictionary<string, string>();
        public static List<string> XmlElementAppendList = new List<string>();


        public static bool LoadConfig()
        {

            Console.Write("Try to get config file:" + ConfigPath + "   Result:");

            if (File.Exists(ConfigPath))
            {

                Console.WriteLine("Is exists!");

            }
            else
            {
                Console.WriteLine("Is not exists! ");
                Program.Termination(-1, ConfigPath + " Is not exists! Please create a config!");
            }

            XmlDocument ConfigDocument = new XmlDocument();
            ConfigDocument.Load(ConfigPath);
            XmlNode IndexListNode = ConfigDocument.SelectSingleNode("Config/IndexList");

            if (IndexListNode == null)
            {
                Program.Termination(-1, ConfigPath + " doesn't have Config/IndexList!");
            }


            XmlNodeList XmlIndexList = IndexListNode.SelectNodes("XmlIndex");

            if (XmlIndexList.Count < 0)
            {

                Program.Termination(-1, ConfigPath + " doesn't have any XmlIndex data!");
            }

            System.Console.WriteLine("Read Parent Data...");
            try { 
            XmlNode ParentNode = ConfigDocument.SelectSingleNode("Config/Parent");
            XmlElement ParentElement = (XmlElement)ParentNode;
            XmlParentElementName = ParentElement.GetAttribute("element");
            System.Console.WriteLine("Find parent element:" + XmlParentElementName);
            foreach(XmlElement e in ParentElement.SelectSingleNode("attr").ChildNodes)
            {
                System.Console.WriteLine("Find parent attr:" + e.Name + "|" + e.GetAttribute("attr"));
                XmlParentElementAttr.Add(e.Name,e.GetAttribute("attr"));

            }
            }
            catch (Exception e)
            {
                Program.Error("Error Parent Data:"+e.Message);
                Program.Error("Set Parent Data NULL!");
                XmlParentElementName = null;
                XmlParentElementAttr = null;
            }

            System.Console.WriteLine("Read Append Data...");
            try
            {
                XmlNode ParentNode = ConfigDocument.SelectSingleNode("Config/Append");
                XmlElement ParentElement = (XmlElement)ParentNode;
                foreach (XmlElement e in ParentElement.ChildNodes)
                {
                    System.Console.WriteLine("Find append Element:" + e.Name);
                    Config.XmlElementAppendList.Add(e.Name);

                }
            }
            catch(Exception e)
            {
                Program.Error("Error Append Data:"+e.Message);
                Program.Error("Set Append Data NULL!");
                XmlElementAppendList = null;
            }

            if (XmlIndexList.Count <= 0)
            {
                Program.Termination(-1, " No XmlIndexs in config.");
            }

            foreach (XmlNode i in XmlIndexList)
            {

                XmlIndex Node = new XmlIndex();

                XmlElement iElement = (XmlElement)i;
                Node.Root = iElement.GetAttribute("root");
                Node.Attr = iElement.GetAttribute("attr");
                Node.Type = iElement.GetAttribute("type");

                Console.WriteLine("New index loaded:" + Node.ToString());

                if (Node.Root.Length <= 0)
                {
                    Program.Error("Where are the Root?:" + Node.ToString());
                }
                else
                {
                    Config.XmlIndexList.Add(Node);
                }



            }


            return true;
        }

        public static void SearchDirectory(int depth, DirectoryInfo directory, string extname)
        {
            Console.Title = Config.ConsoleTitle +String.Format(" {0} ",DateTime.Now.ToString("hh:mm:ss"))+ " Search Directory:"+directory.FullName;
            WriteDirectoryTree(depth,"D-" + directory.Name);
            Console.WriteLine("Search " + directory.FullName + " depth: " + depth + " extname:" + extname);
            DirectoryInfo[] SubDirectory = directory.GetDirectories();
            FileInfo[] Files = directory.GetFiles(extname);


            if (SubDirectory.Length > 0)
            {
                Console.WriteLine("Search subdirectory in " + directory.FullName);
                foreach (DirectoryInfo d in SubDirectory)
                {
                    Console.WriteLine("Get subdirectory, jump in:" + d.FullName);
                    SearchDirectory(depth + 1, d, extname);
                }
            }
            else
            {
                Console.WriteLine("No directory in " + directory.FullName);
            }

            if (Files.Length > 0)
            {
                Console.WriteLine(directory.FullName + " " + extname + " file number count:" + Files.Length);
                foreach (FileInfo f in Files)
                {

                    Console.WriteLine("Add " + f.FullName + " into the list.");
                    WriteDirectoryTree(depth, "  F-" + f.Name);
                    XmlFileList.Add(f);
                }
            }
            else
            {
                Console.WriteLine(directory.FullName + " doesn't have " + extname + " file");
            }
        }


        public static void WriteDirectoryTree(int depth,string filename)
        {
            
            FileStream fs = File.Open(TreeLogPath,FileMode.Append,FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            Console.WriteLine("Write directory tree:"+TreeLogPath);
            sw.WriteLine(GetDirectoryTreeString(depth,filename));
            sw.Close();
            fs.Close();
            
        }

        public static string GetDirectoryTreeString(int depth, string filename)
        {
            StringBuilder StrBuild = new StringBuilder();

            StrBuild.Append("   ");

            for(int i = 0; i < depth; i++)
            {
                StrBuild.Append("   ");
            }
            StrBuild.Append(filename);
            Console.WriteLine("Tree information:" + StrBuild.ToString());

            return StrBuild.ToString();
        }

    }
}
