using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DemoProject.Reports
{
    public partial class ReportViewerForm : Form
    {
        private readonly ReportViewer reportViewer;
        private object[] reportData = [];
        private Dictionary<string, string> parameters = [];
        private string reportPath = "Report.rdlc";

        public ReportViewerForm()
        {
            Text = "Report viewer";
            reportViewer = new ReportViewer();
            reportViewer.Dock = DockStyle.Fill;
            Controls.Add(reportViewer);

            this.Shown += ReportViewerForm_Shown;
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadReport(reportViewer.LocalReport, reportPath, reportData, parameters);
            reportViewer.RefreshReport();
            base.OnLoad(e);
        }
        private void ReportViewerForm_Shown(object? sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
        }

        public static void ShowReport(string reportPath, object[] reportData, Dictionary<string, string> reportParameters)
        {
            var frm = new ReportViewerForm();
            frm.reportData = reportData;
            frm.reportPath = reportPath;
            frm.parameters = reportParameters;
            frm.ShowDialog();
        }
        static void LoadReport(LocalReport report, string reportPath, object[] reportData, Dictionary<string, string> parameters)
        {
            using var fs = new FileStream(reportPath, FileMode.Open);
            report.LoadReportDefinition(fs);
            report.DataSources.Add(new ReportDataSource("Items", reportData));

            // Global params
            parameters["Print_Date"] = DateTime.Now.ToString("d"); // Local format
            // Ajust parameters
            List<ReportParameter> reportParameters = [];
            foreach (var p in report.GetParameters())
            {
                if (parameters.TryGetValue(p.Name, out string? value))
                {
                    reportParameters.Add(new ReportParameter(p.Name, value ?? ""));
                }
            }

            report.SetParameters(reportParameters);
        }
    }
}
