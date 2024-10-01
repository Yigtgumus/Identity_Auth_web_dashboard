using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Identity_Auth.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Identity_Auth.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Identity_Auth.ViewModels;
using Identity_Auth.Helpers;
using System.Text.Json;
[Authorize]
public class UserDatabaseController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<UserDatabaseController> _logger;
    public UserDatabaseController(AppDbContext context, UserManager<AppUser> userManager, ILogger<UserDatabaseController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }
    private string GetCurrentUserId()
    {
        return _userManager.GetUserId(User);
    }

    [HttpGet]
    public IActionResult Index()
    {
        var userId = GetCurrentUserId();
        var connections = _context.UserDatabaseConnections
                                  .Where(c => c.UserId == userId)
                                  .ToList();
        return View(connections);
    }

    [HttpGet]
    public IActionResult AddConnection()
    {
        return View(new UserDatabaseConnection());
    }

    [HttpPost]
    public IActionResult AddConnection(UserDatabaseConnection model)
    {
        ModelState.Remove(nameof(UserDatabaseConnection.ConnectionString));
        ModelState.Remove(nameof(UserDatabaseConnection.UserId));
        ModelState.Remove(nameof(UserDatabaseConnection.User));
        if (ModelState.IsValid)
        {
            string connectionString = $"Server={model.ServerName};Database={model.DatabaseName};User Id={model.Username};Password={model.Password};TrustServerCertificate=True";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    model.ConnectionString = connectionString;
                    model.UserId = GetCurrentUserId(); 
                    _context.UserDatabaseConnections.Add(model);
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Veritabanına bağlanırken bir hata oluştu. Lütfen tekrar deneyin.");
            }
        }

        return View(model);
    }

    public async Task<IActionResult> ReportsList()
    {
        var userId = GetCurrentUserId();
        var reports = await _context.UserReports
                                    .Where(r => r.UserId == userId)
                                    .ToListAsync();

        var reportViewModels = new List<ReportListViewModel>();

        foreach (var report in reports)
        {
            var connectionAlias = await GetConnectionAliasAsync(report.ConnectionId);
            var reportViewModel = new ReportListViewModel
            {
                Id = report.Id,
                ReportName = report.Alias,
                ConnectionAlias = connectionAlias
            };
            reportViewModels.Add(reportViewModel);
        }

        return View(reportViewModels);
    }



    [HttpGet]
    public IActionResult TerstView()
    {
        return View();
    }

    [HttpPost]
    public IActionResult DeleteConnection(int id)
    {
        var userId = GetCurrentUserId();
        var connection = _context.UserDatabaseConnections
                                 .FirstOrDefault(c => c.Id == id && c.UserId == userId);

        if (connection != null)
        {
            // Raporları sil
            var reports = _context.UserReports.Where(r => r.ConnectionId == id).ToList();
            _context.UserReports.RemoveRange(reports);

            // Grafiklerin silinmesi
            var graphics = _context.Graphics.Where(g => g.ConnectionId == id).ToList();
            _context.Graphics.RemoveRange(graphics);

            // Bağlantıyı sil
            _context.UserDatabaseConnections.Remove(connection);

            // Değişiklikleri kaydet
            _context.SaveChanges();

            // "ReportsList" view'una yönlendir
            return RedirectToAction("Index");
        }

        return NotFound();
    }


    [HttpPost]
    public IActionResult DeleteReport(int id)
    {
        var userId = GetCurrentUserId();
        var report = _context.UserReports
                              .FirstOrDefault(r => r.Id == id && r.UserId == userId);

        if (report != null)
        {
            _context.UserReports.Remove(report);
            _context.SaveChanges();
            TempData["Message"] = "Rapor başarıyla silindi.";
        }
        else
        {
            TempData["Message"] = "Rapor bulunamadı veya yetkilendirme hatası.";
        }

        return RedirectToAction("ReportsList");
    }

    [HttpPost]
    public IActionResult RenameConnection(int id, string newAlias)
    {
        var userId = GetCurrentUserId();
        var connection = _context.UserDatabaseConnections
                                 .FirstOrDefault(c => c.Id == id && c.UserId == userId);
        if (connection != null)
        {
            connection.Alias = newAlias;
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult EditReportName(int id, string newName)
    {
        // Id'ye göre ilgili raporu bulun
        var report = _context.UserReports.Find(id);
        if (report != null)
        {
            report.Alias = newName;
            _context.SaveChanges();
            return RedirectToAction("ReportsList"); // Listeyi tekrar yüklemek için Index sayfasına yönlendirin
        }
        return BadRequest();
    }

    [HttpPost]
    public IActionResult TestConnection(int id)
    {
        var connection = _context.UserDatabaseConnections.Find(id);
        if (connection != null)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(connection.ConnectionString))
                {
                    sqlConnection.Open();
                    // Bağlantı başarılı
                    TempData["Message"] = "Connection Succesfull !";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Could not connect : " + ex.Message;
            }
        }
        return RedirectToAction("Index");
    }

    public IActionResult Dashboard()
    {
        return View();
    }
    [HttpGet]
    public IActionResult CreateReport()
    {
        var userId = GetCurrentUserId();
        var connections = _context.UserDatabaseConnections
                                  .Where(c => c.UserId == userId)
                                  .ToList();

        var model = new CreateReportViewModel
        {
            Connections = connections
        };

        return View(model);
    }

    [HttpPost]
    [HttpPost]
    public IActionResult CreateReport(CreateReportViewModel model)
    {
        var userId = GetCurrentUserId();
        model.Connections = _context.UserDatabaseConnections
                                    .Where(c => c.UserId == userId)
                                    .ToList();

        // ModelState'den bazı alanları çıkarabilirsiniz, örneğin:
        ModelState.Remove("Connections");
        ModelState.Remove("SelectedConnectionId");
        ModelState.Remove("ReportName");

        if (ModelState.IsValid)
        {
            var connection = _context.UserDatabaseConnections
                                     .FirstOrDefault(c => c.Id == model.SelectedConnectionId && c.UserId == userId);

            if (connection != null)
            {
                List<Dictionary<string, object>> queryResult = new List<Dictionary<string, object>>();

                try
                {
                    using (var sqlConnection = new SqlConnection(connection.ConnectionString))
                    {
                        sqlConnection.Open();
                        using (var command = new SqlCommand(model.Query, sqlConnection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var row = new Dictionary<string, object>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        row[reader.GetName(i)] = reader.GetValue(i);
                                    }
                                    queryResult.Add(row);
                                }
                            }
                        }
                    }
                    ViewData["QueryResult"] = queryResult; // Veriyi ViewData ile gönderiyoruz
                    TempData["Message"] = "Query executed successfully!";
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "Problem occurred while executing query: " + ex.Message;
                }
            }
            else
            {
                TempData["Message"] = "Connection not found.";
            }
        }
        else
        {
            TempData["Message"] = "Invalid model state.";
        }

        return View(model);
    }
    [HttpPost]
    public IActionResult SaveReport([FromBody] CreateReportViewModel model)
    {
        var userId = GetCurrentUserId();

        // İlk olarak, model durumunu kontrol edin
        ModelState.Remove("Id");
        ModelState.Remove("Connections");
        if (!ModelState.IsValid)
        {
            return Json(new { message = "Invalid Model State." });
        }

        try
        {
            // Veritabanı bağlantısını kontrol edin
            var connection = _context.UserDatabaseConnections
                                     .FirstOrDefault(c => c.Id == model.SelectedConnectionId && c.UserId == userId);

            // Eğer bağlantı bulunamazsa, kullanıcıya bilgi verin
            if (connection == null)
            {
                return Json(new { message = "Bağlantı bulunamadı." });
            }

            // Yeni raporu oluşturun
            var newReport = new UserReport
            {
                Alias = model.ReportName,
                Query = model.Query,
                ConnectionId = connection.Id,
                UserId = userId,
                DatabaseName = connection.DatabaseName,
                ConnectionAlias = connection.Alias
            };

            _context.UserReports.Add(newReport);
            _context.SaveChanges();

            return Json(new { message = "Rapor başarıyla kaydedildi." });
        }
        catch (Exception ex)
        {
            // Hata durumunda, hata mesajını döndürün
            return Json(new { message = "Bir hata oluştu: " + ex.Message });
        }
    }

    [HttpGet]
    public IActionResult ViewReport(int id)
    {
        var userId = GetCurrentUserId();
        var report = _context.UserReports.FirstOrDefault(r => r.Id == id && r.UserId == userId);

        if (report == null)
        {
            return NotFound();
        }

        var viewModel = new ViewReportViewModel
        {
            ReportId = report.Id,
            ReportName = report.Alias,
            Query = report.Query,
            ConnectionId = report.ConnectionId,
        };

        return View(viewModel);
    }
    [HttpGet]
    public IActionResult ExecuteReportQuery(int reportId)
    {
        var userId = GetCurrentUserId();
        var report = _context.UserReports.FirstOrDefault(r => r.Id == reportId && r.UserId == userId);

        if (report == null)
        {
            return NotFound();
        }

        // Bağlantı dizesini al
        var connectionString = GetConnectionString(report.ConnectionId);
        DataTable resultTable;

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (var command = new SqlCommand(report.Query, connection))
            {
                using (var adapter = new SqlDataAdapter(command))
                {
                    resultTable = new DataTable();
                    adapter.Fill(resultTable);
                }
            }
        }

        var columns = resultTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
        var rows = resultTable.Rows.Cast<DataRow>().Select(r => r.ItemArray.Select(i => i.ToString()).ToList()).ToList();

        return Json(new { columns, rows });
    }

    private string GetConnectionString(int connectionId)
    {
        var connection = _context.UserDatabaseConnections.FirstOrDefault(c => c.Id == connectionId);
        return $"Server={connection.ServerName};Database={connection.DatabaseName};User Id={connection.Username};Password={connection.Password};TrustServerCertificate=True;";
    }
    [HttpGet]
    public IActionResult GetDashboards()
    {
        var userId = GetCurrentUserId();
        var dashboards = _context.Dashboards
                                 .Where(d => d.UserId == userId)
                                 .Select(d => new { d.Id, d.Name })
                                 .ToList();
        return Json(dashboards);
    }
    [HttpPost]
    public IActionResult SaveGraphic([FromBody] Graphic graphic)
    {

        // Sorgu metnini ham biçimde kaydetmek için doğrulama yapabilirsiniz
        ModelState.Remove("Id");
        ModelState.Remove("ConnectionId");
        ModelState.Remove("Type");
        ModelState.Remove("Row");
        ModelState.Remove("Query");
        ModelState.Remove("Dashboard");
        ModelState.Remove("Connection");
        ModelState.Remove("Priorty");
        if (ModelState.IsValid)
        {
            graphic.Query = System.Net.WebUtility.HtmlDecode(graphic.Query);
            _context.Graphics.Add(graphic);
            _context.SaveChanges();
            return Json(new { message = "Graphic saved successfully." });
        }
        else
        {
            return Json(new { message = "Invalid Attempt. No Dashboard Found" });
        }
       
    }

    [HttpPost]
    public IActionResult AddDashboard([FromBody] Dashboard dashboard)
    {


        dashboard.UserId = GetCurrentUserId();
        _context.Dashboards.Add(dashboard);
        _context.SaveChanges();
        return Json(new { message = "Dashboard added successfully." });


    }
    public IActionResult DashboardList()
    {
        var userId = GetCurrentUserId();
        var dashboards = _context.Dashboards
                                 .Where(d => d.UserId == userId)
                                 .ToList(); // Model türü ile uyumlu hale getiriyoruz
        return View(dashboards); // Model türünü uyumlu hale getiriyoruz
    }

    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var dashboard = await _context.Dashboards
            .Include(d => d.Graphics)
            .ThenInclude(g => g.Connection)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dashboard == null)
        {
            return NotFound();
        }

        var graphicsData = new List<GraphicDataViewModel>();

        foreach (var graphic in dashboard.Graphics)
        {
            var data = await GetChartDataAsync(graphic.Id);
            graphicsData.Add(data);
        }

        var viewModel = new DashboardViewModel
        {
            Dashboard = dashboard,
            GraphicsData = graphicsData
        };

        // Serialize the GraphicsData to JSON and pass it to the View via ViewBag
        ViewBag.GraphicsDataJson = System.Text.Json.JsonSerializer.Serialize(graphicsData);

        return View(viewModel);
    }

    private async Task<GraphicDataViewModel> GetChartDataAsync(int graphicId)
    {
        var graphic = await _context.Graphics
            .Include(g => g.Connection)
            .FirstOrDefaultAsync(g => g.Id == graphicId);

        if (graphic == null)
        {
            return null;
        }

        var connectionString = GetConnectionString(graphic.ConnectionId);
        var query = graphic.Query;
        var alias = await GetConnectionAliasAsync(graphic.ConnectionId);


        var data = new GraphicDataViewModel
        {
            Id = graphic.Id,
            Type = graphic.Type,
            Row = graphic.Row,
            Query = query,
            Labels = new List<string>(),
            Data = new List<List<object>>(),
            Values = new List<double>(),
            Alias =  alias,
            YAxis=graphic.YAxis,
            Priority=graphic.Priorty,
        };

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var columns = reader.FieldCount;

                   
                    for (var i = 0; i < columns; i++)
                    {
                        data.Labels.Add(reader.GetName(i));
                    }

                   
                    while (await reader.ReadAsync())
                    {
                        var row = new List<object>();
                        for (var i = 0; i < columns; i++)
                        {
                            row.Add(reader.GetValue(i));
                        }
                        data.Data.Add(row);

                       
                        if (data.Values.Count == 0 && row.Count > 0)
                        {
                            if (double.TryParse(row[0]?.ToString(), out var value))
                            {
                                data.Values.Add(value);
                            }
                        }
                    }
                }
            }
        }

        return data;
    }
    [HttpPost]
    public IActionResult DeleteGraphic(int id)
    {
        var graphic = _context.Graphics.Find(id);
        if (graphic == null)
        {
            return NotFound();
        }

        _context.Graphics.Remove(graphic);
        _context.SaveChanges();

        return RedirectToAction("Details", new { id = graphic.DashboardId }); // Dashboard ID'yi burada kullanarak silme sonrası aynı sayfaya yönlendiriyoruz
    }
    [HttpGet]
    private async Task<string> GetConnectionAliasAsync(int connectionId)
    {
        var userId = GetCurrentUserId();
        var connection = await _context.UserDatabaseConnections
                                       .FirstOrDefaultAsync(c => c.Id == connectionId && c.UserId == userId);

        return connection?.Alias;
    }
    [HttpPost]
    public IActionResult DeleteDashboard(int id)
    {
        var userId = GetCurrentUserId();
        var dashboard = _context.Dashboards
                                .Include(d => d.Graphics)
                                .FirstOrDefault(d => d.Id == id && d.UserId == userId);

        if (dashboard == null)
        {
            return NotFound();
        }

        // Dashboard ve bağlı grafiklerin silinmesi
        _context.Graphics.RemoveRange(dashboard.Graphics);
        _context.Dashboards.Remove(dashboard);
        _context.SaveChanges();

        return Json(new { message = "Dashboard and its graphics deleted successfully." });
    }  
}










