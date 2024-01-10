﻿using Microsoft.AspNetCore.Mvc;
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
    }
}