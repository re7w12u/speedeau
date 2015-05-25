using ImportListeDeSuivi.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.SharePoint;
using System.Runtime.Serialization.Formatters.Binary;
using ImportListeDeSuivi.util;
using System.Threading;


namespace ImportListeDeSuivi
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            Logger.Subsribe(new Filelogger(String.Format("import-{0}.txt", DateTime.Now.Ticks)));

            string choice = PrintUsage();
            while (choice != "9")
            {
                switch (choice)
                {
                    case "1":
                        FileInfo file = p.ListFile();                        
                        Console.WriteLine("Data loaded. Processing...");
                        p.Load(file.FullName);
                        p.Import();
                        if (Logger.ErrorCount > 0) Console.WriteLine("check logs for for errors ({0})", Logger.ErrorCount);
                        break;
                    case "2":
                        p.EmptyList();
                        break;
                    case "3":
                        p.DeleteDatFile();
                        break;
                    default:
                        break;
                }
                choice = PrintUsage();
            }

            Console.WriteLine("bye bye");
        }


        private void GetOptions()
        {
            Console.WriteLine("Generate random codification if no value provided? y/n");
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.KeyChar == Char.Parse("y")) MakeRandomCodification = true;
        }

        private static string PrintUsage()
        {
            Console.WriteLine("*******************************");
            Console.WriteLine("Import liste de suivi - v0.3");
            Console.WriteLine("Commands:");
            Console.WriteLine(" 1    Fill in list");
            Console.WriteLine(" 2    Empty list");
            Console.WriteLine(" 3    Delete dat file");
            Console.WriteLine(" 9    Exit");
            Console.WriteLine("*******************************");

            return Console.ReadLine();
        }

        public List<Entry> Entries { get; set; }
        public bool MakeRandomCodification { get; set; }

        private void EmptyList()
        {
            string siteUrl = ConfigurationManager.AppSettings.Get("SiteUrl");
            string listName = ConfigurationManager.AppSettings.Get("ListName");

            Console.WriteLine("Opening SharePoint site : " + siteUrl);
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList list = web.Lists[listName];
                    //SPListItemCollection items = list.Items;
                    //IEnumerable<int> ids = items.Cast<SPListItem>().Select(i => i.ID);
                    //Progress.Total = ids.Count();
                    //for (int i = 0; i < ids.Count(); i++)
                    //{
                    //    Progress.PrintPercent();
                    //    items.DeleteItemById(ids.ElementAt(i));
                    //    Thread.Sleep(50);
                    //}                    
                    StringBuilder deletebuilder = new StringBuilder();
                    deletebuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Batch>");
                    string command = "<Method><SetList Scope=\"Request\">" + list.ID + "</SetList><SetVar Name=\"ID\">{0}</SetVar><SetVar Name=\"Cmd\">Delete</SetVar></Method>";

                    foreach (SPListItem item in list.Items)
                    {
                        deletebuilder.Append(string.Format(command, item.ID.ToString()));
                    }
                    deletebuilder.Append("</Batch>");

                    site.RootWeb.ProcessBatchData(deletebuilder.ToString());


                    Console.WriteLine("{0} is now empty", list.Title);
                }
            }
        }

        private void Import()
        {
            string siteUrl = ConfigurationManager.AppSettings.Get("SiteUrl");
            string listName = ConfigurationManager.AppSettings.Get("ListName");

            Progress.Total = Entries.Count;
            Console.WriteLine("Opening SharePoint site : " + siteUrl);
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList list = web.Lists[listName];
                    ItemBuilder builder = new ItemBuilder(site, MakeRandomCodification);
                    Console.WriteLine("importing data...");
                    foreach (Entry e in Entries)
                    {
                        Progress.PrintPercent();
                        SPListItem item = list.AddItem();
                        builder.Build(e, item);
                        item.Update();
                    }
                }
            }
        }

        private void Load(string path)
        {
            Console.WriteLine("Reading data from CSV file...");
            //string path = ConfigurationManager.AppSettings.Get("DataDirectory");
            string separator = ConfigurationManager.AppSettings.Get("Separator");
            string lineStart = ConfigurationManager.AppSettings.Get("LineStart");

            string data = File.ReadAllText(path, Encoding.Default);
            string[] lines = data.Split(new string[] { lineStart }, StringSplitOptions.RemoveEmptyEntries);

            Progress.Total = lines.Count();
            Entries = new List<Entry>();
            EntryBuilder builder = new EntryBuilder(lines.First(), separator);
            foreach (string line in lines.Skip(1))
            {
                Progress.PrintPercent();

                //if (!line.StartsWith("$$"))
                //{
                //    Logger.Err("line does not start with $$ - " + line);
                //    continue;
                //}

                Entry entry = builder.Build(line);
                Entries.Add(entry);
            }
        }

        public void Serialize()
        {
            string path = ConfigurationManager.AppSettings.Get("serializerPath");
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                Console.WriteLine("Serializing data");
                bformatter.Serialize(stream, Entries);
            }
        }

        public void Deserialize()
        {
            string path = ConfigurationManager.AppSettings.Get("serializerPath");

            if (!File.Exists(path))
            {
                Load(path);
                Serialize();
            }
            else
            {
                //Open the file written above and read values from it.
                using (Stream stream = File.Open(path, FileMode.Open))
                {
                    BinaryFormatter bformatter = new BinaryFormatter();

                    Console.WriteLine("Deserializing data");
                    Entries = (List<Entry>)bformatter.Deserialize(stream);
                }
            }
        }

        private void DeleteDatFile()
        {
            string path = ConfigurationManager.AppSettings.Get("serializerPath");
            if (File.Exists(path)) File.Delete(path);
            else Console.WriteLine("Nothing to delete @ " + path);
        }

        private FileInfo ListFile()
        {
            string path = ConfigurationManager.AppSettings.Get("DataDirectory");
            IEnumerable<FileInfo> files = new DirectoryInfo(path).EnumerateFiles();
            int count = 0;
            foreach (FileInfo f in files)
            {
                Console.WriteLine("{0} => {1}", count, f.Name);
                count++;
            }

            Console.WriteLine("choose import file by typing the number :");
            string key = Console.ReadLine();

            int index = Convert.ToInt32(key);

            return files.ElementAt(index);

        }
    }
}
