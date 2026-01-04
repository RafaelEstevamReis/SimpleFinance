using Simple.Finance;
using Simple.Sqlite;
using System;
using System.Windows.Forms;

namespace DemoProject
{
    internal static class Program
    {
        // TODO: Refactor with DI
        public static Manager Manager { get; } = new Manager("data.db");
        public static ConfigurationDB Config = new ConfigurationDB("preferences.db");

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Manager.Initialize(createBackup: true, backupName: $"bkp/data_{DateTime.Now:yyyyMMddHH}.db");
            ApplicationConfiguration.Initialize();
            Application.Run(new frmMain());
        }
    }
}