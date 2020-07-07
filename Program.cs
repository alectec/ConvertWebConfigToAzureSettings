  
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConvertConfigToAzureSettings
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter the path for the web.config file:");

            string inputPath = @"d:\temp\web.config";
            if (!Debugger.IsAttached)
            {
                inputPath = Console.ReadLine();
            }            
            
            if (!(File.Exists(inputPath)))
            {
               Console.WriteLine("Cannot find input config file.");               
            }
            
            var localAppSettings = ReadAppSettingsFromConfigFile(inputPath);

            Console.WriteLine("Enter the output path for the JSON file:");
            string outputPath = @"d:\temp\appSettings.json";
             if (!Debugger.IsAttached)
            {
                outputPath = Console.ReadLine();
            }    

            await WriteAzureAppSettings(localAppSettings, outputPath);

            Console.WriteLine("Processing complete!");
            Console.WriteLine("Output located at: " + outputPath);
        }

        private static List<AzureAppSetting> ReadAppSettingsFromConfigFile(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);
            
            var settings = doc.Root
			   .Descendants("appSettings")
			   .Descendants()
			   .Select(r => new AzureAppSetting { Name = r.Attribute("key").Value,
                      Value = r.Attribute("value").Value }).ToList();			   				

            return settings;
        }

        private static async Task WriteAzureAppSettings(List<AzureAppSetting> settings, string outFilePath)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            await using (FileStream fileStream = new FileStream(
                outFilePath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true))
            {
                await JsonSerializer.SerializeAsync<List<AzureAppSetting>>(fileStream,settings, options);
            };
        }
    }
}
