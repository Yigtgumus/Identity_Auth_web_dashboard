using Identity_Auth.Models;

public class CreateReportViewModel
{
    public string ReportName { get; set; }
    public string Query { get; set; }
    public List<UserDatabaseConnection> Connections { get; set; }
    public int SelectedConnectionId { get; set; }

}

