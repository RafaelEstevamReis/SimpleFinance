// See https://aka.ms/new-console-template for more information
using System;

Console.WriteLine("Simple.Finance TEST");

var mngr = new Simple.Finance.Manager("data.db");
mngr.Initialize(createBackup: true, backupName: $"data_{DateTime.Now:yyyyMMddHH}.db");

Tests.SampleFunctions.Run(mngr);
Console.WriteLine("\nEND");
