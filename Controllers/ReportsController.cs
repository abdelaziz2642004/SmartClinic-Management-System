using Clinic.Data;
using Clinic.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IReportExporter _exporter;

        public ReportsController(AppDbContext context, IReportExporter exporter)
        {
            _context = context;
            _exporter = exporter;
        }

        // GET: api/reports/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportReports()
        {
            var reports = await _context.Reports.ToListAsync();

            if (!reports.Any())
                return NotFound("No reports available to export");

            var fileBytes = _exporter.Export(reports);
            var fileName = $"Reports_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";

            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
    }
}