using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConvertResxToConstant
{
    public class LanguageConstant
    {
        public string Key { get; set; }
    }

    public class LanguageConstantInput
    {
        public static string KEY = $"{nameof(KEY)}";
        public List<LanguageConstant> Texts { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Enter filename: ");
            //var fileName = Console.ReadLine();

            //Console.WriteLine("Enter filename: ");
            //var namespaceName = Console.ReadLine();

            var xml = File.ReadAllText("SharedResource.en.resx");
            var classKey = File.ReadAllText("file.txt");

            var obj = new
            {
                Texts = XElement.Parse(xml)
                    .Elements("data")
                    .Select(el => new LanguageConstant
                    {
                        Key = el.Attribute("name").Value,
                        //text = el.Element("value").Value.Trim()
                    })
                    .ToList()
            };

            string json = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            LanguageConstantInput keys = JsonConvert.DeserializeObject<LanguageConstantInput>(json);

            var content = "";
            foreach (var languageConstant in keys.Texts)
            {
                content += $"public static string {languageConstant.Key} = \"{languageConstant.Key}\";{Environment.NewLine}\t\t";
            }

            classKey = classKey.Replace("{content}", content);

            string fileName = @"LocalConstants.cs";
            FileInfo fi = new FileInfo(fileName);
            if (fi.Exists)
            {
                fi.Delete();
            }

            // Create a new file     
            using (StreamWriter sw = fi.CreateText())
            {
                sw.WriteLine(classKey);
            }

            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
