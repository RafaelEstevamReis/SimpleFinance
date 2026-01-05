using Simple.BotUtils.DI;
using Simple.Finance;
using Simple.Sqlite;
using System;
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
            // Set DI
            var manager = new Manager("data.db");
            manager.Initialize(createBackup: true, backupName: $"bkp/data_{DateTime.Now:yyyyMMddHH}.db");
            Injector.AddSingleton(manager);

            var config = new ConfigurationDB("preferences.db");
            Injector.AddSingleton(config);

            // App Boot
            ApplicationConfiguration.Initialize();
            Application.Run(new frmMain());
        }
    }
}