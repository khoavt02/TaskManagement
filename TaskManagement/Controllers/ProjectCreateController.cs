using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.Data.Common;
using TableDependency.SqlClient.Base.Messages;
using TaskManagement.Helper;
using TaskManagement.Hubs;
using TaskManagement.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManagement.Controllers
{
	public class ProjectCreateController : Controller
	{
		private readonly TaskManagementContext _context;
		private readonly ILogger _logger;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IHubContext<NotificationHub> _hubContext;
		public ProjectCreateController(TaskManagementContext context, IHubContext<NotificationHub> hubContext, IHttpContextAccessor contextAccessor)
		{
			this._context = context;
			this._hubContext = hubContext;
			this._contextAccessor = contextAccessor;
		}
		public IActionResult Index()
		{
			bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Project", "Add");
			if (!hasPermission) return RedirectToAction("Author", "Home");
			ViewBag.lstDepartments = _context.Departments.ToList();
			ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.Where(x => x.PositionCode != "NV").ToList();
            return View();
		}
		
		[HttpPost]
		public async Task<JsonResult> AddProject(IFormCollection model)
		{
			try
			{
				bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Project", "Add");
				if (hasPermission) {
					if (model != null)
					{
                        if (model["project_code"] == "" || model["project_name"] == "")
                        {
                            return new JsonResult(new { status = false, message = "Vui lòng nhập mã dự án và tên dự án!" });
                        }
                        if (model["project_description"] == "" && model.Files["file"] == null)
                        {
                            return new JsonResult(new { status = false, message = "Vui lòng nhập mô tả hoặc chọn file đính kèm!" });
                        }
                        if (model["start_date"] == "" || model["end_date"] == "")
                        {
                            return new JsonResult(new { status = false, message = "Vui lòng chọn ngày bắt đầu và ngày kết thúc!" });
                        }
                        if (DateTime.Parse(model["start_date"]) > DateTime.Parse(model["end_date"]))
                        {
                            return new JsonResult(new { status = false, message = "Vui lòng chọn ngày bắt đầu nhỏ hơn ngày kết thúc!" });
                        }
                        if (model["manager"] == "" || model["department"] == "")
                        {
                            return new JsonResult(new { status = false, message = "Vui lòng chọn quản lý và phòng ban!" });
                        }
                        if (decimal.Parse(model["point"]) < 0 || model["point"] == "")
                        {
                            return new JsonResult(new { status = false, message = "Điểm số phải lớn hơn 0!" });
                        }
                        if (model["priority_level"] == "")
                        {
                            return new JsonResult(new { status = false, message = "Vui lòng chọn mức độ quan trọng của dự án!" });
                        }
                        var project = new Project()
						{
							ProjectCode = model["project_code"],
							ProjectName = model["project_name"],
							StartTime = DateTime.Parse(model["start_date"]),
							EndTime = DateTime.Parse(model["end_date"]),
							Manager = model["manager"],
							Department = model["department"],
							Description = model["project_description"],
							PriorityLevel = model["priority_level"],
							Point = int.Parse(model["point"]),
							Users = model["users"],
							Status = "NEW",
							Process = 0,
							CreatedDate = DateTime.Now,
							CreatedBy = Request.Cookies["user_code"],
						};
                        var file = model.Files["file"];
                        if (file != null && file.Length > 0)
                        {
                            // Lưu tệp vào một thư mục cụ thể
                            var filePath = Guid.NewGuid().ToString() + "_" + file.FileName;
                            var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", filePath);

                            using (var stream = new FileStream(physicalPath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            // Lưu đường dẫn tệp vào thuộc tính FilePath của đối tượng TaskProgress
                            project.LinkFiles = filePath;
                        }
                        Notification notificationMa = new Notification()
                        {
                            Message = "Bạn được giao làm quản lý một dự án mới!",
                            Username = model["manager"],
                            Link = "/Project/ProjectDetail?id=" + project.Id,
                            NotificationDateTime = DateTime.Now,
                            IsRead = false,
                        };
                        _context.Add(notificationMa);
                        _context.Add(project);
						//var userString = model["users"].ToString();
						//string[] arrUser = userString.Split(',');
						//foreach (var user in arrUser)
						//{
      //                      Notification notification = new Notification()
      //                      {
      //                          Message = "Bạn được thêm vào một dự án mới!",
      //                          Username = user,
      //                          Link = "/Project/ProjectDetail?id=" + project.Id,
      //                          NotificationDateTime = DateTime.Now,
      //                          IsRead = false,
      //                      };
      //                      _context.Add(notification);
      //                      var hubConnections = _context.HubConnections.Where(con => con.Username == user).OrderByDescending(con => con.Id).FirstOrDefault();
						//	if (hubConnections != null)
						//	{
						//		await _hubContext.Clients.Client(hubConnections.ConnectionId).SendAsync("ReceivedPersonalNotification", "Thông báo", "Bạn được thêm vào một dự án mới!");
						//	}
						//}
                        _context.SaveChanges();
                        var hubConnectionsMa = _context.HubConnections.Where(con => con.Username == model["manager"].ToString()).OrderByDescending(con => con.Id).FirstOrDefault();
						if(hubConnectionsMa != null)
						{
							await _hubContext.Clients.Client(hubConnectionsMa.ConnectionId).SendAsync("ReceivedPersonalNotification", "Thông báo", "Bạn được giao làm quản lý một dự án mới!");
						}
                        return new JsonResult(new { status = true, message = "Thêm mới dự án thành công!" });
					}
					else
					{
						return new JsonResult(new { status = false, message = "Tạo mới dự án thất bại!" });

					}
				}
				else
				{
					return new JsonResult(new { status = false, message = "Bạn không có quyền tạo dự án!" });
				}
				
			}
			catch (Exception ex)
			{
				return new JsonResult(new { status = false, message = "Error" + ex });
			}
		}
    }
}



