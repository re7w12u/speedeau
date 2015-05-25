using ImportListeDeSuivi.model;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportTickets
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();

            string choice = PrintUsage();
            while (choice != "3")
            {
                switch (choice)
                {
                    case "1":
                        p.Import();
                        break;
                    case "2":
                        p.EmptyList();
                        break;
                    default:
                        break;
                }
                choice = PrintUsage();
            }

            Console.WriteLine("bye bye");
        }

        private static string PrintUsage()
        {
            Console.WriteLine("*******************************");
            Console.WriteLine("Import liste de suivi - v0.2");
            Console.WriteLine("Commands:");
            Console.WriteLine(" 1    Fill in list");
            Console.WriteLine(" 2    Empty list");
            Console.WriteLine(" 3    Exit");
            Console.WriteLine("*******************************");

            return Console.ReadLine();
        }



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
                    SPListItemCollection items = list.Items;
                    IEnumerable<int> ids = items.Cast<SPListItem>().Select(i => i.ID);
                    Progress.Total = ids.Count();
                    for (int i = 0; i < ids.Count(); i++)
                    {
                        Progress.PrintPercent();
                        items.DeleteItemById(ids.ElementAt(i));
                    }
                    list.Update();
                    Console.WriteLine("{0} is now empty", list.Title);
                }
            }
        }

        private void Import()
        {
            string siteUrl = ConfigurationManager.AppSettings.Get("SiteUrl");
            string listName = ConfigurationManager.AppSettings.Get("ListName");
            string fileName = ConfigurationManager.AppSettings.Get("fileName");

            Console.WriteLine("Opening SharePoint site : " + siteUrl);
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList list = web.Lists[listName];
                    Console.WriteLine("importing data...");
                    string[] lines = ReadFrom(fileName);
                    Progress.Total = lines.Count();

                    foreach (string e in lines)
                    {
                        Progress.PrintPercent();
                        SPListItem item = list.AddItem();
                        string[] data = e.Split(new char[] { '\t' });

                        item["Title"] = Ensure(data,0).CleanUp();
                        item["NumeroTFS"] = Ensure(data, 1).CleanUp();
                        EnsureDate(item, "Date", data, 2);
                        item["Criticite"] = Ensure(data, 3).CleanUp();
                        item["theme"] = Ensure(data, 4).CleanUp();
                        item["EtapesReproduction"] = Ensure(data, 5);
                        item["ResultatAttendu"] = Ensure(data, 6);
                        item["Statut"] = Ensure(data, 7).CleanUp();
                        //item["Assigné A"] = data,8];

                        item.Update();
                    }
                }
            }
        }

        private void EnsureDate(SPListItem item, string fieldName, string[] data, int i)
        {
            if (!String.IsNullOrWhiteSpace(data[i]))
            {
                item[fieldName] = Convert.ToDateTime(data[2]).ToString("yyyy-MM-ddThh:mm:ssZ");
            }
        }

        string[] ReadFrom(string file)
        {
            string[] lines = File.ReadAllText(file, Encoding.Default).Split(new string[]{"#;#"}, StringSplitOptions.RemoveEmptyEntries);
            return lines;
        }

        string Ensure(string[] obj, int index)
        {
             if (obj == null) return String.Empty;
            if (obj.Count() <= index) return String.Empty;
            if(String.IsNullOrWhiteSpace(obj[index])) return String.Empty;
            return obj[index].ToString();
        }

        

        //private void Load()
        //{
        //    Console.WriteLine("Reading data from CSV file...");
        //    string path = ConfigurationManager.AppSettings.Get("DataFile");
        //    string separator = ConfigurationManager.AppSettings.Get("Separator");
        //    string[] lines = File.ReadAllLines(path, Encoding.Default);
        //    Progress.Total = lines.Count();
        //    Entries = new List<Entry>();
        //    EntryBuilder builder = new EntryBuilder(lines.First(), separator);
        //    foreach (string line in lines.Skip(1))
        //    {
        //        Progress.PrintPercent();
        //        Entry entry = builder.Build(line);
        //        Entries.Add(entry);
        //    }
        //}

    }


    public static class Extend{

        public static string CleanUp(this string input)
        {
            return input.Replace("\r\n", "");
        }
    }
}
