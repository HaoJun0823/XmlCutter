using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace XmlCutter
{
    class XmlCutter
    {

        public static DirectoryInfo TargetDirectory;
        public static XmlDocument XmlSet;
        public static bool NeedSave = false;

        public static bool Do(XmlIndex index, FileInfo file)
        {



            try
            {



                if (string.IsNullOrEmpty(index.Type))
                {
                    Console.WriteLine("No type, this is single xml, file name as:" + index.Attr);
                    XmlDocument SourceXml = new XmlDocument();
                    SourceXml.Load(file.FullName);
                    System.Console.WriteLine("Load Source:" + file.FullName);
                    XmlDocument TargetXml;
                    System.Console.WriteLine("Created Target Document!");



                    Console.WriteLine("Try Cut!");
                    XmlNodeList CutNodeList = SourceXml.GetElementsByTagName(index.Root);
                    Console.WriteLine("Cut Node List Size:" + CutNodeList.Count);
                    foreach (XmlNode node in CutNodeList)
                    {
                        Console.WriteLine("Try Append Single Xml!");
                        TargetXml = AppendXmlElementFromList(SourceXml, CreateXml());
                        Console.WriteLine("Xml Append Finished!");
                        Console.WriteLine("Try Cut Single Xml!");
                        XmlDocument SingleXml = AppendSingleNode(index, node, TargetXml);
                        Console.WriteLine("Xml Cut Finished!");
                        string Path = TargetDirectory + @"\" + index.Root + @"\" + ((XmlElement)node).GetAttribute(index.Attr) + ".xml";
                        Console.WriteLine("Xml Save As:" + Path);
                        SingleXml.Save(Path);
                        Console.WriteLine("Done!");
                    }







                }
                else
                {

                    Console.WriteLine("Get type, save as xml set, file name as:" + index.Type);
                    XmlDocument SourceXml = new XmlDocument();
                    SourceXml.Load(file.FullName);
                    XmlNode ParentNode = XmlSet.SelectSingleNode(Config.XmlParentElementName);
                    XmlNodeList SourceNode = SourceXml.GetElementsByTagName(index.Root);
                    Console.WriteLine("Cut SourceNode <{0}>, target size:{1}.", index.Root, SourceNode.Count);


                    if (SourceNode.Count > 0)
                    {
                        Console.WriteLine("Try Append Xml!");
                        Console.WriteLine("Do Append!");
                        foreach (string s in Config.XmlElementAppendList)
                        {
                            XmlNodeList SourceAppendNodeList = SourceXml.GetElementsByTagName(s);
                            Console.WriteLine("Target Node Name:" + Config.XmlParentElementName);
                            XmlNode TargetAppendNode = XmlSet.SelectSingleNode(Config.XmlParentElementName);
                            Console.WriteLine("Get {0} <{1}>, merge into target <{2}>.", SourceAppendNodeList.Count, s, TargetAppendNode.Name);
                            foreach (XmlNode i in SourceAppendNodeList)
                            {
                                XmlNode Node = i.CloneNode(true);
                                Console.WriteLine("Append:" + Node.OuterXml);

                                TargetAppendNode.AppendChild(XmlSet.ImportNode(Node, true));
                            }
                        }


                        Console.WriteLine("Xml Append Finished!");
                        Console.WriteLine("Try Cut Xml!");
                        Console.WriteLine("Do Cut!");

                        foreach (XmlNode i in SourceNode)
                        {
                            Console.WriteLine("Cut:" + i.OuterXml);
                            ParentNode.AppendChild(XmlSet.ImportNode(i, true));

                        }
                        Console.WriteLine("Cut Done!");
                    }
                    else
                    {
                        Console.WriteLine("No target node, pass.");
                    }






                }


            }
            catch (Exception e)
            {
                Program.Error(e.ToString());
                return false;
            }






            return true;
        }

        public static XmlDocument AppendSingleNode(XmlIndex index, XmlNode sourcenode, XmlDocument target)
        {
            Console.WriteLine("Do Cut!");
            XmlNode ParentNode = target.SelectSingleNode(Config.XmlParentElementName);
            Console.WriteLine("Cut:" + sourcenode);
            ParentNode.AppendChild(target.ImportNode(sourcenode, true));

            return target;
        }


        public static XmlDocument AppendXmlElementFromList(XmlDocument source, XmlDocument target)
        {
            Console.WriteLine("Do Append!");
            foreach (string s in Config.XmlElementAppendList)
            {
                XmlNodeList SourceAppendNodeList = source.GetElementsByTagName(s);
                Console.WriteLine("Target Node Name:" + Config.XmlParentElementName);
                XmlNode TargetAppendNode = target.SelectSingleNode(Config.XmlParentElementName);
                Console.WriteLine("Get {0} <{1}>, merge into target <{2}>.", SourceAppendNodeList.Count, s, TargetAppendNode.Name);
                foreach (XmlNode i in SourceAppendNodeList)
                {
                    XmlNode Node = i.CloneNode(true);
                    Console.WriteLine("Append:" + Node.OuterXml);

                    TargetAppendNode.AppendChild(target.ImportNode(Node, true));
                }
            }


            return target;
        }





        public static XmlDocument CreateXml()
        {

            XmlDocument Document = new XmlDocument();
            XmlDeclaration XmlDeclaration = Document.CreateXmlDeclaration("1.0", "utf-8", null);
            Document.AppendChild(XmlDeclaration);
            if (Config.XmlParentElementName != null)
            {

                XmlElement ParentElement = Document.CreateElement(Config.XmlParentElementName);

                Console.WriteLine("Create Parent:" + ParentElement.Name);

                foreach (KeyValuePair<string, string> i in Config.XmlParentElementAttr)
                {
                    Console.WriteLine("Create Parent Attr:" + i.Key + "|" + i.Value);
                    ParentElement.SetAttribute(i.Key, i.Value);
                }
                Document.AppendChild(ParentElement);


            }
            else
            {
                Console.WriteLine("No Parent Information!");
            }



            Console.WriteLine("Xml Outer:" + Document.OuterXml);
            return Document;
        }

        public static void Action()
        {

            if (BuildDirectory())
            {
                Console.WriteLine("Created target directory:" + TargetDirectory.FullName);
            }
            else
            {
                Program.Termination(-1, "Target directory create error:" + TargetDirectory.FullName);
            }

            if (BuildMetaData())
            {
                Console.WriteLine("Finished MetaData Building.");
            }
            else
            {
                Program.Termination(-1, "MetaData Building Error!");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Start Xml Cutter!");

            for (int x = 0; x < Config.XmlIndexList.Count; x++)
            {
                XmlIndex Index = Config.XmlIndexList[x];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("--------------------");
                Console.WriteLine("[{0}/{1}]Cut <{2}>", x + 1, Config.XmlIndexList.Count, Index.Root);
                Console.ForegroundColor = ConsoleColor.Green;

                if (!string.IsNullOrEmpty(Index.Type))
                {
                    XmlSet = CreateXml();
                    Console.WriteLine("Create xmlset with <{0}>", Index.Type);
                    NeedSave = true;

                }
                else
                {
                    NeedSave = false;
                }


                for (int i = 0; i < Config.XmlFileList.Count; i++)
                {
                    FileInfo File = Config.XmlFileList[i];
                    Console.WriteLine("====================");
                    Console.WriteLine("[{0}/{1}]Cut {2}", i + 1, Config.XmlFileList.Count, File.FullName);
                    Console.Title = Config.ConsoleTitle + String.Format(" Cooking   [{0}/{1}]-Step:<{2}>   [{3}/{4}]-File:{5}", x + 1, Config.XmlIndexList.Count, Index.Root, i + 1, Config.XmlFileList.Count, File.Name);
                    if (Do(Index, File))
                    {
                        Console.WriteLine("{0} {1}", File.FullName, "done!");
                    }
                    else
                    {
                        Program.Error(File.FullName + " not finished!");
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                }
                if (NeedSave)
                {
                    string Path = TargetDirectory + @"\" + Index.Root + @"\" + Index.Type + ".xml";
                    Console.WriteLine("Xml Save As:" + Path);
                    XmlSet.Save(Path);
                    NeedSave = false;
                }

            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static bool BuildMetaData()
        {

            foreach (XmlIndex i in Config.XmlIndexList)
            {
                string Path = TargetDirectory + @"\" + i.Root;
                Directory.CreateDirectory(Path);
                Console.WriteLine("Create Xml directory:" + Path);



            }



            return true;
        }


        public static bool BuildDirectory()
        {
            DirectoryInfo ParentDirectory = Config.ModDirectory.Parent;
            TargetDirectory = new DirectoryInfo(ParentDirectory.FullName + @"\" + "XmlCutter." + Config.ModDirectory.Name + DateTime.Now.Date.ToString("yyyymmdd") + DateTime.Now.ToString("hhmmss"));
            Console.WriteLine("Creating target direcotry:" + TargetDirectory.FullName);
            TargetDirectory.Create();

            if (!TargetDirectory.Exists)
            {
                return false;
            }


            return true;
        }



    }
}
