namespace DemoProject.Reports;

using System;

// How to generate Datasets:
// 1. Update classes here
// 2. Add clesses to program.GenerateTypesXSDs()
// 3. Update project ReportItemSchemas.xsd with generated one

public class CategoriesOverviewModel
{
    public string CategoryName { get; set; }
    public string Description { get; set; }
    public decimal Value { get; set; }
    public DateTime Date { get; set; }
}

public class YearlySummaryModel
{
    public string CategoryName { get; set; }
    public decimal Month01 { get; set; }
    public decimal Month02 { get; set; }
    public decimal Month03 { get; set; }
    public decimal Month04 { get; set; }
    public decimal Month05 { get; set; }
    public decimal Month06 { get; set; }
    public decimal Month07 { get; set; }
    public decimal Month08 { get; set; }
    public decimal Month09 { get; set; }
    public decimal Month10 { get; set; }
    public decimal Month11 { get; set; }
    public decimal Month12 { get; set; }
    public decimal RowTotal { get; set; }
}