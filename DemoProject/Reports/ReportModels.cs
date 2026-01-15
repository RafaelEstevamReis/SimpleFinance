using System;

namespace DemoProject.Reports
{
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
}