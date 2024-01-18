using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TableDependency.SqlClient.Base.Messages;
using TaskManagement.Helper;
using TaskManagement.Hubs;
using TaskManagement.Models;
using Task = TaskManagement.Models.Task;

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

        //[HttpPost]
        //public async Task<JsonResult> SentNoti()
        //{
        //    try
        //    {
        //            var username = "IT02";
        //        var hubConnections = _context.HubConnections.Where(con => con.Username == username).OrderByDescending(con => con.Id).FirstOrDefault();
        //        await _hubContext.Clients.Client(hubConnections.ConnectionId).SendAsync("ReceivedPersonalNotification", "Bạn có thông báo mới", username);
        //            return new JsonResult(new { status = true, message = "Gửi thông báo thành công" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new { status = false, message = "Error" + ex });
        //    }
        //}

        [HttpGet]
        public JsonResult GetListNoti()
        {
            try
            {
                var userCode = HttpContext.Session.GetString("user_code");
                var listNoti = _context.Notifications.Where(x => x.Username == userCode).OrderByDescending(x => x.NotificationDateTime).ToList();
                if (listNoti.Count > 0)
                {
                    //return new JsonResult(new { status = true, data = lstUser });
                    return new JsonResult(new { status = true, data = listNoti });
                }
                else
                {
                    return new JsonResult(new { status = false, message = "Dữ liệu trống" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "Error" + ex });
            }
        }

        [HttpPost]
        public JsonResult MarkNotificationAsRead(IFormCollection model)
        {
            try
            {
                var notification = _context.Notifications.Find(int.Parse(model["id"]));

                if (notification != null)
                {
                    notification.IsRead = true;
                    _context.Notifications.Update(notification);
                    _context.SaveChanges();

                    return Json(new { status = true });
                }
                else
                {
                    return Json(new { status = false, message = "Notification not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetChartData()
        {
            var tasks = _context.Tasks.ToList(); // Thay thế bằng danh sách thực tế của bạn
            var projects = _context.Projects.ToList(); // Thay thế bằng danh sách thực tế của bạn
            var departments = _context.Departments.ToList(); // Thay thế bằng danh sách thực tế của bạn

            var result = from task in tasks
                         join project in projects on task.ProjectId equals project.Id into projectGroup
                         from proj in projectGroup.DefaultIfEmpty()
                         join department in departments on proj.Department equals department.DepartmentCode into departmentGroup
                         from dept in departmentGroup.DefaultIfEmpty()
                         where task.Status != null && proj != null
                         group new { task, proj, dept } by new { task.Status, DepartmentName = dept?.DepartmentName } into groupedTasks
                         select new
                         {
                             Status = groupedTasks.Key.Status,
                             Department = groupedTasks.Key.DepartmentName,
                             TotalTasks = groupedTasks.Count()
                         };

            var labels = result.Select(r => r.Department).Distinct().ToArray();
            var totalTasks = result.Sum(r => r.TotalTasks);
            var totalTasksByStatus = result.GroupBy(r => r.Status)
                                       .Select(group => new
                                       {
                                           Status = group.Key,
                                           TotalTasks = group.Sum(g => g.TotalTasks),
                                           PercentTask = group.Sum(g => g.TotalTasks) > 0 ? Math.Round(((decimal)group.Sum(g => g.TotalTasks) / totalTasks) * 100 ,2) : 0
                                       })
                                       .ToArray();

            var datasets = result.GroupBy(r => r.Status)
                                 .Select(group =>
                                     new
                                     {
                                         label = GetStatusLabel(group.Key),
                                         backgroundColor = GetRandomColor(group.Key),
                                         borderColor = GetRandomColor(group.Key),
                                         //hoverBackgroundColor = GetRandomColor(),
                                         //hoverBorderColor = GetRandomColor(),
                                         data = labels.Select(label => group.FirstOrDefault(g => g.Department == label)?.TotalTasks ?? 0).ToArray()
                                     })
                                 .ToArray();

            return Json(new
            {
                labels,
                datasets,
                totalTasksByStatus
            });
        }

        private string GetStatusLabel(string status)
        {
            switch (status)
            {
                case "COMPLETE":
                    return "Hoàn thành";
                case "PROCESSING":
                    return "Đang thực hiện";
                case "EVALUATE":
                    return "Đã đánh giá";
                case "NEW":
                    return "Mới";
                default:
                    return status;
            }
        }

        private string GetRandomColor(string status)
        {
			switch (status)
			{
				case "COMPLETE":
					return "#5fc27e";
				case "PROCESSING":
					return "#5b7dff";
				case "EVALUATE":
					return "#fcc100";
				case "NEW":
					return "#47bac1";
				default:
					return status;
			}
		}
    }
}