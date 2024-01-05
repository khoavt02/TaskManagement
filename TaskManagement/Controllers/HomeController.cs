using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TableDependency.SqlClient.Base.Messages;
using TaskManagement.Helper;
using TaskManagement.Hubs;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly TaskManagementContext _context;
        public HomeController(ILogger<HomeController> logger, IHubContext<NotificationHub> hubContext, TaskManagementContext context)
        {
            _logger = logger;
            _hubContext = hubContext;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Author()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<JsonResult> SentNoti()
        {
            try
            {
                    var username = "IT02";
                var hubConnections = _context.HubConnections.Where(con => con.Username == username).OrderByDescending(con => con.Id).FirstOrDefault();
                await _hubContext.Clients.Client(hubConnections.ConnectionId).SendAsync("ReceivedPersonalNotification", "Bạn có thông báo mới", username);
                    return new JsonResult(new { status = true, message = "Gửi thông báo thành công" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "Error" + ex });
            }
        }
    }
}