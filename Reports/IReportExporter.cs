namespace Clinic.Reports
{
  
    public interface IReportExporter
    {
        byte[] Export(List<Clinic.Models.Report> reports);
    }
}