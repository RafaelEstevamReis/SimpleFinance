using Simple.Finance;
using System;
using System.Windows.Forms;

namespace DemoProject
{
    internal static class Program
    {
        public static Manager Manager { get; } = new Manager("data.db");

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Manager.Initialize(createBackup: true, backupName: $"data_{DateTime.Now:yyyyMMddHH}.db");

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new frmMain());
        }
    }
}