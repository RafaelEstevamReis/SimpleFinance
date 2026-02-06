using Simple.BotUtils.DI;
using Simple.Finance;
using Simple.Finance.ExchangeRate;
using Simple.Sqlite;
using System;
using System.Text;
using System.Windows.Forms;

namespace DemoProject
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GenerateTypesXSDs();
            // Set DI
            var manager = new Manager("data.db");
            manager.Initialize(createBackup: true, backupName: $"bkp/data_{DateTime.Now:yyyyMMddHH}.db");
            Injector.AddSingleton(manager);

            var config = new KeyValueStorage("preferences.db");
            Injector.AddSingleton(config);

            var exchange = ExchangeRateConverter.CreateWithTemporalSeries();
            Injector.AddSingleton(exchange);

            // App Boot
            ApplicationConfiguration.Initialize();
            Application.Run(new frmMain());
        }

        private static void GenerateTypesXSDs()
        {
            Type[] types = [typeof(Reports.CategoriesOverviewModel)];
            var xri = new System.Xml.Serialization.XmlReflectionImporter();
            var xss = new System.Xml.Serialization.XmlSchemas();
            var xse = new System.Xml.Serialization.XmlSchemaExporter(xss);
            foreach (var type in types)
            {
                var xtm = xri.ImportTypeMapping(type);
                xse.ExportTypeMapping(xtm);
            }
            using var sw = new System.IO.StreamWriter("ReportItemSchemas.xsd", false, Encoding.UTF8);
            for (int i = 0; i < xss.Count; i++)
            {
                var xs = xss[i];
                xs.Id = "ReportItemSchemas";
                xs.Write(sw);
            }
        }

    }
}