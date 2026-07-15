using ClosedXML.Excel;
using Clinic.Models;

namespace Clinic.Reports
{

    public class ExcelReportExporter : IReportExporter
    {
        public byte[] Export(List<Report> reports)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reports");

            // Header row
            worksheet.Cell(1, 1).Value = "Report ID";
            worksheet.Cell(1, 2).Value = "Appointment ID";
            worksheet.Cell(1, 3).Value = "Diagnosis";
            worksheet.Cell(1, 4).Value = "Message";
            worksheet.Cell(1, 5).Value = "Report Date";

            // Data rows
            int row = 2;
            foreach (var report in reports)
            {
                worksheet.Cell(row, 1).Value = report.ReportId;
                worksheet.Cell(row, 2).Value = report.AppointmentId;
                worksheet.Cell(row, 3).Value = report.Diagnosis;
                worksheet.Cell(row, 4).Value = report.Message;
                worksheet.Cell(row, 5).Value = report.ReportDate;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}