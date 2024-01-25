using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using TaskManagement.Helper;
using TaskManagement.Hubs;
using TaskManagement.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Task = TaskManagement.Models.Task;
using System.Collections.Generic;

namespace TaskManagement.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskManagementContext _context;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<NotificationHub> _hubContext;

        public TaskController(TaskManagementContext context, IHubContext<NotificationHub> hubContext, IHttpContextAccessor contextAccessor)
        {
            this._context = context;
            this._contextAccessor = contextAccessor;
            this._hubContext = hubContext;
        }
        public IActionResult Index()
        {
            bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Task", "View");
            if (!hasPermission) return RedirectToAction("Author", "Home");
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            return View();
        }
        public IActionResult TaskCreate()
        {
            bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Task", "Add");
            bool hasPermission2 = AuthorizationHelper.CheckRole(this._contextAccessor, "TaskUser", "Add");
            if (!hasPermission || !hasPermission2) return RedirectToAction("Author", "Home");
            var user_code = HttpContext.Session.GetString("user_code");
            User user = _context.Users.Where(x => x.UserCode == user_code).FirstOrDefault();
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.Where(x => x.DepartmentCode == user.DepartmentCode).ToList();
            //ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.Where(x => x.DepartmentCode == user.DepartmentCode).ToList();
            ViewBag.lstProjects = _context.Projects.Where(x => x.Department.Contains(user.DepartmentCode)).ToList();
            return View();
        }
        public IActionResult TaskChildCreate(int id_parent)
        {
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstProjects = _context.Projects.ToList();
            Task task = _context.Tasks.Where(x => x.Id == id_parent).FirstOrDefault();
            ViewBag.taskParent = task;
            Project project = _context.Projects.Where(p => p.Id == task.ProjectId).FirstOrDefault();
            ViewBag.project = project;
            ViewBag.department = _context.Departments.Where(x => x.DepartmentCode == project.Department).FirstOrDefault();
            string[] assignedUserCodes = task.AssignedUser.Split(',');
            ViewBag.lstUsers = _context.Users.Where(user => assignedUserCodes.Contains(user.UserCode)).ToList();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AddTask(IFormCollection model)
        {
            try
            {
                bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Task", "Add");
                bool hasPermission2 = AuthorizationHelper.CheckRole(this._contextAccessor, "TaskUser", "Add");
                if (hasPermission || hasPermission2)
                {
                    if (model != null)
                    {
                        if (model["project_id"] == "")
                        {
                            return new JsonResult(new { status = false, message = "Vui lòng chọn dự án!" });
                        }
                        if (model["task_name"] == "")
                        {
                            return new JsonResult(new { status = false, message = "Vui lòng nhập tên công việc!" });
                        }
                        if (model["task_description"] == "" && model.Files["file"] == null)
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
                        if (model["department"] == "" || model["assigned_user"] == "")
                        {
                            return new JsonResult(new { status = false, message = "Vui lòng chọn phòng ban và nhân viên thực hiện!" });
                        }
                        if (decimal.Parse(model["point"]) < 0 || model["point"] == "")
                        {
                            return new JsonResult(new { status = false, message = "Điểm số phải lớn hơn 0!" });
                        }
                        if (model["priority_level"] == "")
                        {
                            return new JsonResult(new { status = false, message = "Vui lòng chọn mức độ quan trọng của dự án!" });
                        }
                        var TaskMaxId = _context.Tasks.Where(e => e.ProjectId == int.Parse(model["project_id"])).OrderByDescending(e => e.Id).FirstOrDefault();
                        Project project = _context.Projects.Where(e => e.Id == int.Parse(model["project_id"])).FirstOrDefault();
                        // Tính tổng số điểm cho dự án cụ thể
                        decimal totalPointsForProject = _context.Tasks
                            .Where(task => task.ProjectId == project.Id)
                            .Sum(task => task.Points ?? 0);
                        if ((totalPointsForProject + decimal.Parse(model["point"])) > project.Point)
                        {
                            return new JsonResult(new { status = false, message = "Điểm số của dự án chỉ còn"+(project.Point - totalPointsForProject) });
                        }
                        string id_task = "1";
                        if (TaskMaxId != null)
                        {
                            id_task = TaskMaxId.Id.ToString();
                        }
                        var ProjectCode = project.ProjectCode;
                        var task = new Task()
                        {
                            ProjectId = int.Parse(model["project_id"]),
                            TaskCode = ProjectCode + "/" + id_task,
                            TaskName = model["task_name"],
                            StartTime = DateTime.Parse(model["start_date"]),
                            EndTime = DateTime.Parse(model["end_date"]),
                            EstimateTime = Decimal.Parse(model["estimate_time"]),
                            AssignedUser = model["assigned_user"],
                            Description = model["task_description"],
                            Level = model["priority_level"],
                            Points = int.Parse(model["point"]),
                            Status = "NEW",
                            ProcessPercent = 0,
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
                            task.LinkFiles = filePath;
                        }
                        _context.Add(task);
                        _context.SaveChanges();
                        var userString = model["assigned_user"].ToString();
                        string[] arrUser = userString.Split(',');
                        foreach (var user in arrUser)
                        {
                            Notification notification = new Notification()
                            {
                                Message = "Bạn được giao một công việc mới!",
                                Username = user,
                                Link = "/Task/TaskDetail?id=" + task.Id,
                                NotificationDateTime = DateTime.Now,
                                IsRead = false,
                            };
                            _context.Add(notification);
                            var hubConnections = _context.HubConnections.Where(con => con.Username == user).OrderByDescending(con => con.Id).FirstOrDefault();
                            if (hubConnections != null)
                            {
                                await _hubContext.Clients.Client(hubConnections.ConnectionId).SendAsync("ReceivedPersonalNotification", "Thông báo", "Bạn được giao một công việc mới!");
                            }
                        }
                        UpdateProjectProgress(task.ProjectId);
                        _context.SaveChanges();
                        return new JsonResult(new { status = true, message = "Thêm mới công việc thành công!" });
                    }
                    else
                    {
                        return new JsonResult(new { status = false, message = "Tạo mới công việc thất bại!" });

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

        [HttpPost]
        public JsonResult AddTaskV2(string point, string priority_level, string department,
    string manager, string users, string end_date, string start_date,
    string task_description, string estimate_time, string task_name, string project_id)
        {
            try
            {
                var TaskMaxId = _context.Tasks.Where(e => e.ProjectId == int.Parse(project_id)).OrderByDescending(e => e.Id).FirstOrDefault();
                Project project = _context.Projects.Where(e => e.Id == int.Parse(project_id)).FirstOrDefault();
                string id_task = "1";
                if (TaskMaxId != null)
                {
                    id_task = TaskMaxId.Id.ToString();
                }
                var ProjectCode = project.ProjectCode;
                var task = new Task()
                {
                    ProjectId = int.Parse(project_id),
                    TaskCode = ProjectCode + "/" + id_task,
                    TaskName = task_name,
                    StartTime = DateTime.Parse(start_date),
                    EndTime = DateTime.Parse(end_date),
                    EstimateTime = Decimal.Parse(estimate_time),
                    AssignedUser = users,
                    Description = task_description,
                    Level = priority_level,
                    Points = int.Parse(point),
                    Status = "NEW",
                    ProcessPercent = 0,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Request.Cookies["user_code"],

                };
                _context.Add(task);
                _context.SaveChanges();
                return new JsonResult(new { status = true, message = "Thêm mới công việc thành công!" });
                
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "Error" + ex });
            }
        }

        [HttpPost]
        public JsonResult AddTaskChild(IFormCollection model)
        {
            try
            {
                if (model != null)
                {
                    if (model["project_id"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng chọn dự án!" });
                    }
                    if (model["task_name"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng nhập tên công việc!" });
                    }
                    if (model["task_description"] == "" && model.Files["file"] == null)
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
                    if (int.Parse(model["point"]) < 0 || model["point"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Điểm số phải lớn hơn 0!" });
                    }
                    if (model["priority_level"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng chọn mức độ quan trọng của dự án!" });
                    }
                    Task taskParent = _context.Tasks.Where(e => e.Id == int.Parse(model["task_parent_id"])).FirstOrDefault();
                    // Tính tổng số điểm cho dự án cụ thể
                    decimal totalPointsForProject = _context.Tasks
                        .Where(task => task.TaskParent == int.Parse(model["task_parent_id"]))
                        .Sum(task => task.Points ?? 0);
                    if ((totalPointsForProject + decimal.Parse(model["point"])) > taskParent.Points)
                    {
                        return new JsonResult(new { status = false, message = "Điểm số của công việc chỉ còn" + (taskParent.Points - totalPointsForProject) });
                    }
                    var TaskMaxId = _context.Tasks.Where(e => e.TaskParent == int.Parse(model["task_parent_id"])).OrderByDescending(e => e.Id).FirstOrDefault();
                    string id_task = "1";
                    if (TaskMaxId != null)
                    {
                        id_task = TaskMaxId.Id.ToString();
                    }
                    var task = new Task()
                    {
                        ProjectId = int.Parse(model["project_id"]),
                        TaskCode = model["task_parent_code"] + "/" + id_task,
                        TaskName = model["task_name"],
                        TaskParent = int.Parse(model["task_parent_id"]),
                        StartTime = DateTime.Parse(model["start_date"]),
                        EndTime = DateTime.Parse(model["end_date"]),
                        EstimateTime = Decimal.Parse(model["estimate_time"]),
                        AssignedUser = model["assigned_user"],
                        Description = model["task_description"],
                        Level = model["priority_level"],
                        Points = int.Parse(model["point"]),
                        Status = "NEW",
                        ProcessPercent = 0,
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
						task.LinkFiles = filePath;
					}
                    _context.Add(task);
                    UpdateParentTaskProgress(task);
                    UpdateProjectProgress(task.ProjectId);
                    _context.SaveChanges();
                    return new JsonResult(new { status = true, message = "Thêm mới công việc thành công!" });
                }
                else
                {
                    return new JsonResult(new { status = false, message = "Tạo mới công việc thất bại!" });

                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "Error" + ex });
            }
        }
        [HttpPost]
        public async Task<JsonResult> UpdateTaskAsync(IFormCollection model)
        {
            try
            {
                bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Task", "Edit");
                bool hasPermission2 = AuthorizationHelper.CheckRole(this._contextAccessor, "TaskUser", "Edit");
                if (hasPermission || hasPermission2)
                {
                    if (model != null)
                    {
                        if (model["project_id"] == "")
                        {
                            return new JsonResult(new { status = false, message = "Vui lòng chọn dự án!" });
                        }
                        if (model["task_name"] == "")
                        {
                            return new JsonResult(new { status = false, message = "Vui lòng nhập tên công việc!" });
                        }
                        if (model["task_description"] == "" && model.Files["file"] == null)
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
                            return new JsonResult(new { status = false, message = "Vui lòng chọn mức độ quan trọng của công việc!" });
                        }
                        if(decimal.Parse(model["proccess"])>100 || decimal.Parse(model["proccess"]) < 0)
                        {
                            return new JsonResult(new { status = false, message = "Phần trăm tiến độ phải lớn hơn 0 và nhỏ hơn 100!" });
                        }
                        var taskUpdate = _context.Tasks.Where(x => x.Id == int.Parse(model["task_id"])).FirstOrDefault();
                        if (taskUpdate.TaskParent == null)
                        {
                            Project project = _context.Projects.Where(e => e.Id == int.Parse(model["project_id"])).FirstOrDefault();
                            decimal totalPointsForProject = _context.Tasks
                                .Where(task => task.ProjectId == project.Id)
                                .Sum(task => task.Points ?? 0);
                            if ((totalPointsForProject + decimal.Parse(model["point"])) > project.Point)
                            {
                                return new JsonResult(new { status = false, message = "Điểm số của dự án chỉ còn" + (project.Point - totalPointsForProject) });
                            }
                        }
                        else {
                            Task taskParent = _context.Tasks.Where(e => e.Id == int.Parse(model["task_parent_id"])).FirstOrDefault();
                            // Tính tổng số điểm cho dự án cụ thể
                            decimal totalPointsForProject = _context.Tasks
                                .Where(task => task.TaskParent == int.Parse(model["task_parent_id"]))
                                .Sum(task => task.Points ?? 0);
                            if ((totalPointsForProject + decimal.Parse(model["point"])) > taskParent.Points)
                            {
                                return new JsonResult(new { status = false, message = "Điểm số của công việc chỉ còn" + (taskParent.Points - totalPointsForProject) });
                            }
                        }
                        if (taskUpdate != null) {

                            taskUpdate.TaskName = model["task_name"];
                            taskUpdate.StartTime = DateTime.Parse(model["start_date"]);
                            taskUpdate.EndTime = DateTime.Parse(model["end_date"]);
                            taskUpdate.EstimateTime = Decimal.Parse(model["estimate_time"]);
                            taskUpdate.AssignedUser = model["assigned_user"];
                            taskUpdate.Description = model["task_description"];
                            taskUpdate.Level = model["priority_level"];
                            taskUpdate.Points = decimal.Parse(model["point"]);
                            taskUpdate.UpdateDate = DateTime.Now;
                            taskUpdate.UpdateBy = Request.Cookies["user_code"];
                            taskUpdate.Status = model["status"];
                            taskUpdate.ProcessPercent = decimal.Parse(model["proccess"]);
                            if(taskUpdate.ProcessPercent == 100 || taskUpdate.Status == "COMPLETE")
                            {
                                taskUpdate.CompleteTime = DateTime.Now;
                            }
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
                                taskUpdate.LinkFiles = filePath;
                            }
                            _context.Update(taskUpdate);
                            UpdateParentTaskProgress(taskUpdate);
                            UpdateProjectProgress(taskUpdate.ProjectId);
                            Notification notification = new Notification()
                            {
                                Message = "Công việc bạn quản lý có cập nhật mới!",
                                Username = taskUpdate.CreatedBy,
                                Link = "/Task/TaskDetail?id=" + taskUpdate.Id,
                                NotificationDateTime = DateTime.Now,
                                IsRead = false,
                            };
                            _context.Add(notification);
                            var hubConnections = _context.HubConnections.Where(con => con.Username == taskUpdate.CreatedBy).OrderByDescending(con => con.Id).FirstOrDefault();
                            if (hubConnections != null)
                            {
                                await _hubContext.Clients.Client(hubConnections.ConnectionId).SendAsync("ReceivedPersonalNotification", "Thông báo", "Công việc bạn quản lý có cập nhật mới!");
                            }
                            _context.SaveChanges();
                            return new JsonResult(new { status = true, message = "Cập nhật công việc thành công!" });
                        }
                        else {
                            return new JsonResult(new { status = false, message = "Cập nhật công việc thất bại!" });
                        }
                        
                    }
                    else
                    {
                        return new JsonResult(new { status = false, message = "Cập nhật công việc thất bại!" });

                    }
                }
                else
                {
                    return new JsonResult(new { status = false, message = "Bạn không có quyền cập nhật công việc!" });
                }
                
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "Error" + ex });
            }
        }


        #region TaskDetail
        public IActionResult TaskDetail(int id)
        {
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            ViewBag.lstProjects = _context.Projects.ToList();
            Task taskDetail = _context.Tasks.Find(id);
            Project project = _context.Projects.Where(x => x.Id == taskDetail.ProjectId).FirstOrDefault();
            ViewBag.projectName = project.ProjectName;
            ViewBag.department = project.Department;

            return View(taskDetail);
        }

        [HttpGet]
        public JsonResult GetListProccessOfTask(int id, int offset, int limit)
        {
            try
            {
                var lstProcess = _context.TaskProgresses.Where(x => x.TaskId == id).ToList();

                if (lstProcess.Count > 0)
                {
                    var users = _context.Users;
                    var data = lstProcess
                        .GroupJoin(users,
                            process => process.CreatedBy,
                            user => user.UserCode,
                            (process, userGroup) => new { Process = process, Users = userGroup })
                        .SelectMany(
                            x => x.Users.DefaultIfEmpty(),
                            (x, user) => new
                            {
                                ProcessId = x.Process.Id,
                                ProcessName = x.Process.Description,
                                CreatedName = user?.UserName,
                                TaskId = x.Process.TaskId,
                                ProjectId = x.Process.ProjectId,
                                ProcessPercent = x.Process.ProcessPercent,
                                Estimate = x.Process.Estimate,
                                FileAttach = x.Process.FileAttach,
                                Description = x.Process.Description,
                                CreatedDate = x.Process.CreatedDate
                            });

                    return new JsonResult(new { status = true, rows = data, total = data.Count() });
                }
                else
                {
                    return new JsonResult(new { status = false, message = "Dữ liệu trống" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "Error: " + ex.Message });
            }

        }

        [HttpPost]
        public async Task<JsonResult> AddProccessTaskAsync(IFormCollection model)
        {
            try
            {
                if (model != null)
                {
                    if (decimal.Parse(model["proccess"]) > 100 || decimal.Parse(model["proccess"]) < 0)
                    {
                        return new JsonResult(new { status = false, message = "Phần trăm tiến độ phải lớn hơn 0 và nhỏ hơn 100!" });
                    }
                    if (model["description"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng nhập mô tả tiến độ!" });
                    }
                    if (decimal.Parse(model["estimate"]) <= 0)
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng nhập thời gian thực hiện!" });
                    }
                    var taskProccess = new TaskProgress()
                    {
                        TaskId = int.Parse(model["id"]),
                        ProjectId = int.Parse(model["project_id"]),
                        ProcessPercent = decimal.Parse(model["proccess"]),
                        Description = model["description"],
                        Estimate = decimal.Parse(model["estimate"]),
                        CreatedBy = Request.Cookies["user_code"],
                        CreatedDate = DateTime.Now,
                    };
					var file = model.Files["file"]; // "file" nên phù hợp với thuộc tính name của input file trong form
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
						taskProccess.FileAttach = filePath;
					}
					Task task = _context.Tasks.Where(x => x.Id == taskProccess.TaskId).FirstOrDefault();
                    if (task != null)
                    {
                        task.ProcessPercent = taskProccess.ProcessPercent;
                        if (task.Status == "NEW" && taskProccess.ProcessPercent < 100)
                        {
                            task.Status = "PROCESSING";
                        }
                        if (taskProccess.ProcessPercent == 100)
                        {
                            task.Status = "COMPLETE";
                            task.CompleteTime = DateTime.Now;
                        }
                    }
                    UpdateParentTaskProgress(task);
                    UpdateProjectProgress(task.ProjectId);
                    _context.Add(taskProccess);
                    Notification notification = new Notification()
                    {
                        Message = "Công việc bạn quản lý có cập nhật tiến độ mới!",
                        Username = task.CreatedBy,
                        Link = "/Task/TaskDetail?id=" + task.Id,
                        NotificationDateTime = DateTime.Now,
                        IsRead = false,
                    };
                    _context.Add(notification);
                    var hubConnections = _context.HubConnections.Where(con => con.Username == task.CreatedBy).OrderByDescending(con => con.Id).FirstOrDefault();
                    if (hubConnections != null)
                    {
                        await _hubContext.Clients.Client(hubConnections.ConnectionId).SendAsync("ReceivedPersonalNotification", "Thông báo", "Công việc bạn quản lý có cập nhật tiến độ mới!");
                    }
                    _context.SaveChanges();
                    return new JsonResult(new { status = true, message = "Thêm mới tiến độ thành công!" });
                }
                else
                {
                    return new JsonResult(new { status = false, message = "Thêm mới tiến độ thất bại!" });

                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "Error" + ex });
            }
        }
		[HttpGet]
		public IActionResult DownloadFile(string fileName)
		{
			// Kiểm tra xem tệp có tồn tại không
			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

			if (!System.IO.File.Exists(filePath))
			{
				return NotFound();
			}

			// Đọc dữ liệu từ tệp
			var fileBytes = System.IO.File.ReadAllBytes(filePath);

			// Trả về tệp dưới dạng phản hồi
			return File(fileBytes, "text/plain", fileName);
		}
		[HttpPost]
        public async Task<JsonResult> AddReviewTaskAsync(IFormCollection model)
        {
            try
            {
                if (model != null)
                {

                    var taskEvualate = new TaskEvaluate()
                    {
                        TaskId = int.Parse(model["task_id_review"]),
                        ProjectId = int.Parse(model["project_id_review"]),
                        Content = model["complete_level"],
                        Description = model["review_description"],
                        Points = decimal.Parse(model["point_review"]),
                        CreatedDate = DateTime.Now,
                        CreatedBy = HttpContext.Session.GetString("user_code")
                    };
                    var task = _context.Tasks.Where(x => x.Id == taskEvualate.TaskId).FirstOrDefault();
                    task.Status = "EVALUATE";
                    _context.Update(task);
                    _context.Add(taskEvualate);
                    _context.SaveChanges();
                    var userString = task.AssignedUser;
                    string[] arrUser = userString.Split(',');
                    foreach (var user in arrUser)
                    {
                        Notification notification = new Notification()
                        {
                            Message = "Bạn có một công việc được đánh giá!",
                            Username = user,
                            Link = "/Task/ListTaskUser",
                            NotificationDateTime = DateTime.Now,
                            IsRead = false,
                        };
                        _context.Add(notification);
                        var hubConnections = _context.HubConnections.Where(con => con.Username == user).OrderByDescending(con => con.Id).FirstOrDefault();
                        if (hubConnections != null)
                        {
                            await _hubContext.Clients.Client(hubConnections.ConnectionId).SendAsync("ReceivedPersonalNotification", "Thông báo", "Bạn có một công việc được đánh giá!");
                        }
                    }
                    _context.SaveChanges();
                    return new JsonResult(new { status = true, message = "Đánh giá thành công!" });
                }
                else
                {
                    return new JsonResult(new { status = false, message = "Đánh giá thất bại!" });

                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "Error" + ex });
            }
        }
        #endregion

        #region Uploadfile
        [HttpPost]
        public JsonResult Upload(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var fileName = Path.Combine(uploadPath, Path.GetRandomFileName());

                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        file.CopyToAsync(fileStream);
                    }

                    return new JsonResult(new { status = true, message = "Thêm file thành công!" });
                }
                else
                {
                    return new JsonResult(new { status = false, message = "Thêm file thất bại!" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "Error" + ex });
            }


        }
        #endregion
        public IActionResult ListTask()
        {
            bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Task", "View");
            if (!hasPermission) return RedirectToAction("Author", "Home");
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            ViewBag.lstProjects = _context.Projects.ToList();   
            return View();
        }
        [HttpGet]
        public JsonResult GetListTask(int offset, int limit, string name, string from_date, string to_date, string status, string priority_level,  int project_id, string department)
        {
            try
            {
                var tasks = _context.Tasks;
                var users = _context.Users;
                var projects = _context.Projects;
                var departments = _context.Departments;
                var task_evaluate = _context.TaskEvaluates;
                var intermediateResult = tasks
                .GroupJoin(users,
                           task => task.CreatedBy,
                           user => user.UserCode,
                           (task, userGroup) => new { Task = task, Users = userGroup })
                .SelectMany(
                           x => x.Users.DefaultIfEmpty(),
                           (x, user) => new
                           {
                               Task = x.Task,
                               Manager = user,
                           })
                .ToList(); // Force execution

                var query = intermediateResult
                    .GroupJoin(task_evaluate,
                               p => p.Task.Id,
                               e => e.TaskId,
                               (p, evaluations) => new { p.Task, p.Manager, Evaluations = evaluations })
                    .SelectMany(
                               x => x.Evaluations.DefaultIfEmpty(),
                               (x, evaluation) => new
                               {
                                   Id = x.Task.Id,
                                   TaskCode = x.Task.TaskCode,
                                   TaskName = x.Task.TaskName,
                                   Description = x.Task.Description,
                                   TaskParent = x.Task.TaskParent,
                                   ProjectId = x.Task.ProjectId,
                                   AssignedUser = x.Task.AssignedUser,
                                   Status = x.Task.Status,
                                   EstimateTime = x.Task.EstimateTime,
                                   Level = x.Task.Level,
                                   Points = x.Task.Points,
                                   ProcessPercent = x.Task.ProcessPercent,
                                   StartTime = x.Task.StartTime,
                                   EndTime = x.Task.EndTime,
                                   CompleteTime = x.Task.CompleteTime,
                                   CreatedDate = x.Task.CreatedDate,
                                   CreateBy = x.Task.CreatedBy,
                                   UpdateDate = x.Task.UpdateDate,
                                   UpdateBy = x.Task.UpdateBy,
                                   CreatedName = x.Manager?.UserName,
                                   IsEvaluated = x.Evaluations != null && x.Evaluations.Any() ? x.Evaluations.First().Id > 0 : false,
                               });

                if (name != null)
                {
                    query = query.Where(x => x.TaskName.Contains(name) || x.TaskCode.Contains(name));
                }

                if (status != null)
                {
                    query = query.Where(x => x.Status == status);
                }
                if (priority_level != null)
                {
                    query = query.Where(x => x.Level == priority_level);
                }
                if (project_id > 0)
                {
                    query = query.Where(x => x.ProjectId == project_id);
                }
                if (from_date != null && to_date != null)
                {
                    DateTime parsedFromDate = DateTime.ParseExact(from_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    DateTime parsedToDate = DateTime.ParseExact(to_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    query = query.Where(x => (x.StartTime >= parsedFromDate && x.StartTime <= parsedToDate) ||
                         (x.EndTime >= parsedFromDate && x.EndTime <= parsedToDate));
                }
                //if (review != 0)
                //{
                //    if (review == 1)
                //    {
                //        query = query.Where(x => x.IsEvaluated == true);
                //    }
                //    else
                //    {
                //        query = query.Where(x => x.IsEvaluated == false);
                //    }
                //}
                
                var lstTaskWithDepartment = query
    .Join(projects,
          t => t.ProjectId,
          p => p.Id,
          (t, p) => new { Task = t, Project = p }) // Join tasks and projects
    .Join(departments,
          tp => tp.Project.Department,
          d => d.DepartmentCode,
          (tp, d) => new
          {
              tp.Task.Id,
              tp.Task.TaskCode,
              tp.Task.TaskName,
              tp.Task.Description,
              tp.Task.TaskParent,
              tp.Task.ProjectId,
              tp.Task.AssignedUser,
              tp.Task.Status,
              tp.Task.EstimateTime,
              tp.Task.Level,
              tp.Task.Points,
              tp.Task.ProcessPercent,
              tp.Task.StartTime,
              tp.Task.EndTime,
              tp.Task.CompleteTime,
              tp.Task.CreatedDate,
              tp.Task.CreateBy,
              tp.Task.UpdateDate,
              tp.Task.UpdateBy,
              tp.Task.CreatedName,
              ProjectName = tp.Project.ProjectName,
              DepartmentName = d.DepartmentName,
              DepartmentCode = d.DepartmentCode,
              IsEvaluated = tp.Task.IsEvaluated
          })
    .Where(x => department == null || x.DepartmentCode == department)
    
    .ToList();
                var lstTask = lstTaskWithDepartment.OrderBy(x => x.TaskCode).Skip(offset).Take(limit).ToList();
                if (lstTask.Count > 0)
                {
                    return new JsonResult(new { status = true, rows = lstTask, total = lstTaskWithDepartment.Count() });
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
        [HttpGet]
        public IActionResult ExcelListTask(string name, string from_date, string to_date, string status, string priority_level,  int project_id, string department)
        {
            try
            {
                bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Task", "Export");
                if (!hasPermission) return new JsonResult(new { status = false, message = "Bạn không có quyền xuất excel công việc!" });
                var tasks = _context.Tasks;
                var users = _context.Users;
                var projects = _context.Projects;
                var departments = _context.Departments;
                var task_evaluate = _context.TaskEvaluates;
                var intermediateResult = tasks
                .GroupJoin(users,
                           task => task.CreatedBy,
                           user => user.UserCode,
                           (task, userGroup) => new { Task = task, Users = userGroup })
                .SelectMany(
                           x => x.Users.DefaultIfEmpty(),
                           (x, user) => new
                           {
                               Task = x.Task,
                               Manager = user,
                           })
                .ToList(); // Force execution

                var query = intermediateResult
                    .GroupJoin(task_evaluate,
                               p => p.Task.Id,
                               e => e.TaskId,
                               (p, evaluations) => new { p.Task, p.Manager, Evaluations = evaluations })
                    .SelectMany(
                               x => x.Evaluations.DefaultIfEmpty(),
                               (x, evaluation) => new
                               {
                                   Id = x.Task.Id,
                                   TaskCode = x.Task.TaskCode,
                                   TaskName = x.Task.TaskName,
                                   Description = x.Task.Description,
                                   TaskParent = x.Task.TaskParent,
                                   ProjectId = x.Task.ProjectId,
                                   AssignedUser = x.Task.AssignedUser,
                                   Status = x.Task.Status,
                                   EstimateTime = x.Task.EstimateTime,
                                   Level = x.Task.Level,
                                   Points = x.Task.Points,
                                   ProcessPercent = x.Task.ProcessPercent,
                                   StartTime = x.Task.StartTime,
                                   EndTime = x.Task.EndTime,
                                   CompleteTime = x.Task.CompleteTime,
                                   CreatedDate = x.Task.CreatedDate,
                                   CreateBy = x.Task.CreatedBy,
                                   UpdateDate = x.Task.UpdateDate,
                                   UpdateBy = x.Task.UpdateBy,
                                   CreatedName = x.Manager?.UserName,
                                   IsEvaluated = x.Evaluations != null && x.Evaluations.Any() ? x.Evaluations.First().Id > 0 : false,
                               });

                if (name != null)
                {
                    query = query.Where(x => x.TaskName.Contains(name) || x.TaskCode.Contains(name));
                }

                if (status != null)
                {
                    query = query.Where(x => x.Status == status);
                }
                if (priority_level != null)
                {
                    query = query.Where(x => x.Level == priority_level);
                }
                if (project_id > 0)
                {
                    query = query.Where(x => x.ProjectId == project_id);
                }
                if (from_date != null && to_date != null)
                {
                    DateTime parsedFromDate = DateTime.ParseExact(from_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    DateTime parsedToDate = DateTime.ParseExact(to_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    query = query.Where(x => (x.StartTime >= parsedFromDate && x.StartTime <= parsedToDate) ||
                         (x.EndTime >= parsedFromDate && x.EndTime <= parsedToDate));
                }
                //if (review != 0)
                //{
                //    if (review == 1)
                //    {
                //        query = query.Where(x => x.IsEvaluated == true);
                //    }
                //    else
                //    {
                //        query = query.Where(x => x.IsEvaluated == false);
                //    }
                //}
                var lstTask = query.ToList();
                var lstTaskWithDepartment = lstTask
    .Join(projects,
          t => t.ProjectId,
          p => p.Id,
          (t, p) => new { Task = t, Project = p }) // Join tasks and projects
    .Join(departments,
          tp => tp.Project.Department,
          d => d.DepartmentCode,
          (tp, d) => new
          {
              tp.Task.Id,
              tp.Task.TaskCode,
              tp.Task.TaskName,
              tp.Task.Description,
              tp.Task.TaskParent,
              tp.Task.ProjectId,
              tp.Task.AssignedUser,
              tp.Task.Status,
              tp.Task.EstimateTime,
              tp.Task.Level,
              tp.Task.Points,
              tp.Task.ProcessPercent,
              tp.Task.StartTime,
              tp.Task.EndTime,
              tp.Task.CompleteTime,
              tp.Task.CreatedDate,
              tp.Task.CreateBy,
              tp.Task.UpdateDate,
              tp.Task.UpdateBy,
              tp.Task.CreatedName,
              ProjectName = tp.Project.ProjectName,
              DepartmentName = d.DepartmentName,
              DepartmentCode = d.DepartmentCode,
              IsEvaluated = tp.Task.IsEvaluated
          })
    .Where(x => department == null || x.DepartmentCode == department)
    .ToList();
                if (lstTaskWithDepartment.Count > 0)
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Tasks");

                        // Set custom headers
                        worksheet.Cells[1, 1].Value = "Mã công việc";
                        worksheet.Cells[1, 2].Value = "Tên công việc";
                        worksheet.Cells[1, 3].Value = "Ngày bắt đầu";
                        worksheet.Cells[1, 4].Value = "Ngày kết thúc";
                        worksheet.Cells[1, 5].Value = "Thời gian";
                        worksheet.Cells[1, 6].Value = "Tiến độ";
                        worksheet.Cells[1, 7].Value = "Ngày hoàn thành";
                        worksheet.Cells[1, 8].Value = "Mức độ";
                        worksheet.Cells[1, 9].Value = "Điểm số";
                        worksheet.Cells[1, 10].Value = "Ngày tạo";
                        worksheet.Cells[1, 11].Value = "Người tạo";
                        worksheet.Cells[1, 12].Value = "Phòng ban thực hiện";
                        worksheet.Cells[1, 13].Value = "Thuộc dự án";
                        // Add more headers as needed...

                        // Populate data
                        for (int i = 0; i < lstTask.Count; i++)
                        {
                            worksheet.Cells[i + 2, 1].Value = lstTaskWithDepartment[i].TaskCode;
                            worksheet.Cells[i + 2, 2].Value = lstTaskWithDepartment[i].TaskName;
                            worksheet.Cells[i + 2, 3].Value = lstTaskWithDepartment[i].StartTime?.ToString("dd/MM/yyyy");
                            worksheet.Cells[i + 2, 4].Value = lstTaskWithDepartment[i].EndTime?.ToString("dd/MM/yyyy");
                            worksheet.Cells[i + 2, 5].Value = lstTaskWithDepartment[i].EstimateTime;
                            worksheet.Cells[i + 2, 6].Value = lstTaskWithDepartment[i].ProcessPercent;
                            worksheet.Cells[i + 2, 7].Value = lstTaskWithDepartment[i].CompleteTime?.ToString("dd/MM/yyyy");
                            worksheet.Cells[i + 2, 8].Value = lstTaskWithDepartment[i].Level;
                            worksheet.Cells[i + 2, 9].Value = lstTaskWithDepartment[i].Points;
                            worksheet.Cells[i + 2, 10].Value = lstTaskWithDepartment[i].CreatedDate?.ToString("dd/MM/yyyy");
                            worksheet.Cells[i + 2, 11].Value = lstTaskWithDepartment[i].CreatedName;
                            worksheet.Cells[i + 2, 12].Value = lstTaskWithDepartment[i].DepartmentName;
                            worksheet.Cells[i + 2, 13].Value = lstTaskWithDepartment[i].ProjectName;
                            // Add more data columns as needed...
                        }

                        // Return the Excel file
                        var stream = new MemoryStream(package.GetAsByteArray());
                        DateTime currentDate = DateTime.Now;

                        // Định dạng ngày giờ thành chuỗi để thêm vào tên tệp
                        string formattedDate = currentDate.ToString("yyyyMMdd_HHmmss");

                        // Thêm chuỗi định dạng vào tên tệp
                        string fileName = $"Danh_sách_công_việc_{formattedDate}.xlsx";
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
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
        [HttpGet]
        public PartialViewResult GetListTaskKaban( string name, string from_date, string to_date)
        {
            try
            {
                var tasks = _context.Tasks;
                var users = _context.Users;
                var task_evaluate = _context.TaskEvaluates;
                var intermediateResult = tasks
                .GroupJoin(users,
                           task => task.CreatedBy,
                           user => user.UserCode,
                           (task, userGroup) => new { Task = task, Users = userGroup })
                .SelectMany(
                           x => x.Users.DefaultIfEmpty(),
                           (x, user) => new
                           {
                               Task = x.Task,
                               Manager = user,
                           })
                .ToList(); 

                var query = intermediateResult
                    .GroupJoin(task_evaluate,
                               p => p.Task.Id,
                               e => e.TaskId,
                               (p, evaluations) => new { p.Task, p.Manager, Evaluations = evaluations })
                    .SelectMany(
                               x => x.Evaluations.DefaultIfEmpty(),
                               (x, evaluation) => new
                               {
                                   Id = x.Task.Id,
                                   TaskCode = x.Task.TaskCode,
                                   TaskName = x.Task.TaskName,
                                   Description = x.Task.Description,
                                   TaskParent = x.Task.TaskParent,
                                   ProjectId = x.Task.ProjectId,
                                   AssignedUser = x.Task.AssignedUser,
                                   Status = x.Task.Status,
                                   EstimateTime = x.Task.EstimateTime,
                                   Level = x.Task.Level,
                                   Points = x.Task.Points,
                                   ProcessPercent = x.Task.ProcessPercent,
                                   StartTime = x.Task.StartTime,
                                   EndTime = x.Task.EndTime,
                                   CompleteTime = x.Task.CompleteTime,
                                   CreatedDate = x.Task.CreatedDate,
                                   CreateBy = x.Task.CreatedBy,
                                   UpdateDate = x.Task.UpdateDate,
                                   UpdateBy = x.Task.UpdateBy,
                                   CreatedName = x.Manager?.UserName,
                                   IsEvaluated = x.Evaluations != null && x.Evaluations.Any() ? x.Evaluations.First().Id > 0 : false,
                               });

                if (name != null)
                {
                    query = query.Where(x => x.TaskName.Contains(name) || x.TaskCode.Contains(name));
                }
                if (from_date != null && to_date != null)
                {
                    DateTime parsedFromDate = DateTime.ParseExact(from_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    DateTime parsedToDate = DateTime.ParseExact(to_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    query = query.Where(x => (x.StartTime >= parsedFromDate && x.StartTime <= parsedToDate) ||
                         (x.EndTime >= parsedFromDate && x.EndTime <= parsedToDate));
                }
                var lstTask = query.OrderBy(x => x.TaskCode).ToList();
                var listTaskView = lstTask.Select(task => new TaskView
                {
                    Id = task.Id,
                    TaskCode = task.TaskCode,
                    TaskName = task.TaskName,
                    Description = task.Description,
                    TaskParent = task.TaskParent,
                    ProjectId = task.ProjectId,
                    AssignedUser = task.AssignedUser,
                    Status = task.Status,
                    EstimateTime = task.EstimateTime,
                    Level = task.Level,
                    Points = task.Points,
                    ProcessPercent = task.ProcessPercent,
                    StartTime = task.StartTime,
                    EndTime = task.EndTime,
                    CompleteTime = task.CompleteTime,
                    CreatedDate = task.CreatedDate,
                    CreatedBy = task.CreateBy,
                    UpdateDate = task.UpdateDate,
                    UpdateBy = task.UpdateBy,
                    CreatedName = task.CreatedName, 
                    IsEvaluated = task.IsEvaluated
                }).ToList();
                return PartialView("~/Views/Task/Partial/_ListTask.cshtml", listTaskView);
            }
            catch (Exception ex)
            {
                return PartialView("~/Views/Task/Partial/_ListTask.cshtml");
            }
        }

        [HttpGet]
        public JsonResult GetDetailReviewById(int id)
        {
            try
            {
                var detailR = _context.TaskEvaluates
                .Where(x => x.TaskId == id)
                .GroupJoin(
                    _context.Users,
                    te => te.CreatedBy,
                    u => u.UserCode,
                    (te, userGroup) => new { TaskEvaluate = te, Users = userGroup })
                .SelectMany(
                    x => x.Users.DefaultIfEmpty(),
                    (x, user) => new
                    {
                        TaskEvaluate = x.TaskEvaluate,
                        CreatedByName = user != null ? user.UserName : null,
                        // Add other properties you need from TaskEvaluate and Users
                    })
                .FirstOrDefault();
                if (detailR != null)
                {
                    return new JsonResult(new { status = true, data = detailR });
                }
                else
                {
                    return new JsonResult(new { status = false, message = "Không tìm thấy bản ghi" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "Error" + ex });
            }
        }

        #region Công việc cá nhân
        public IActionResult ListTaskUser()
        {
            bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "TaskUser", "View");
            if (!hasPermission) return RedirectToAction("Author", "Home");
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            ViewBag.lstProjects = _context.Projects.ToList();
            return View();
        }
        public IActionResult TaskKabanUser()
        {
            bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "TaskUser", "View");
            if (!hasPermission) return RedirectToAction("Author", "Home");
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            return View();
        }
        [HttpGet]
        public JsonResult GetListTaskUser(int offset, int limit, string name, string from_date, string to_date, string status, string priority_level, int review, int project_id)
        {
            try
            {
                var tasks = _context.Tasks;
                var users = _context.Users;
                var departments = _context.Departments;
                var projects = _context.Projects;
                var task_evaluate = _context.TaskEvaluates;
                var intermediateResult = tasks
    .GroupJoin(users,
               task => task.CreatedBy,
               user => user.UserCode,
               (task, userGroup) => new { Task = task, Users = userGroup })
    .SelectMany(
               x => x.Users.DefaultIfEmpty(),
               (x, user) => new
               {
                   Task = x.Task,
                   Manager = user,
               })
    .ToList(); // Force execution

                var query = intermediateResult
                    .GroupJoin(task_evaluate,
                               p => p.Task.Id,
                               e => e.TaskId,
                               (p, evaluations) => new { p.Task, p.Manager, Evaluations = evaluations })
                    .SelectMany(
                               x => x.Evaluations.DefaultIfEmpty(),
                               (x, evaluation) => new
                               {
                                   Id = x.Task.Id,
                                   TaskCode = x.Task.TaskCode,
                                   TaskName = x.Task.TaskName,
                                   Description = x.Task.Description,
                                   TaskParent = x.Task.TaskParent,
                                   ProjectId = x.Task.ProjectId,
                                   AssignedUser = x.Task.AssignedUser,
                                   Status = x.Task.Status,
                                   EstimateTime = x.Task.EstimateTime,
                                   Level = x.Task.Level,
                                   Points = x.Task.Points,
                                   ProcessPercent = x.Task.ProcessPercent,
                                   StartTime = x.Task.StartTime,
                                   EndTime = x.Task.EndTime,
                                   CompleteTime = x.Task.CompleteTime,
                                   CreatedDate = x.Task.CreatedDate,
                                   CreateBy = x.Task.CreatedBy,
                                   UpdateDate = x.Task.UpdateDate,
                                   UpdateBy = x.Task.UpdateBy,
                                   CreatedName = x.Manager?.UserName,
                                   IsEvaluated = x.Evaluations != null && x.Evaluations.Any() ? x.Evaluations.First().Id > 0 : false,
                               });



                if (name != null)
                {
                    query = query.Where(x => x.TaskName.Contains(name) || x.TaskCode.Contains(name));
                }
                if (project_id > 0)
                {
                    query = query.Where(x => x.ProjectId == project_id);
                }

                if (status != null)
                {
                    query = query.Where(x => x.Status == status);
                }
                if (priority_level != null)
                {
                    query = query.Where(x => x.Level == priority_level);
                }
                if (from_date != null && to_date != null)
                {
                    DateTime parsedFromDate = DateTime.ParseExact(from_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    DateTime parsedToDate = DateTime.ParseExact(to_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    query = query.Where(x => (x.StartTime >= parsedFromDate && x.StartTime <= parsedToDate) ||
                         (x.EndTime >= parsedFromDate && x.EndTime <= parsedToDate));
                }
                if (review != 0)
                {
                    if (review == 1)
                    {
                        query = query.Where(x => x.IsEvaluated == true);
                    }
                    else
                    {
                        query = query.Where(x => x.IsEvaluated == false);
                    }
                }
                var userCode = HttpContext.Session.GetString("user_code");

                var lstTaskWithDepartment = query
    .GroupJoin(projects,
               t => t.ProjectId,
               p => p.Id,
               (t, projectGroup) => new { Task = t, Projects = projectGroup })
    .SelectMany(
               x => x.Projects.DefaultIfEmpty(),
               (x, project) => new { Task = x.Task, Project = project })
    .GroupJoin(tasks,
               tp => tp.Task.TaskParent,
               d => d.Id,
               (tp, taskGroup) => new { tp.Task, tp.Project, Tasks = taskGroup })
    .SelectMany(
               x => x.Tasks.DefaultIfEmpty(),
               (x, task) => new
               {
                   x.Task.Id,
                   x.Task.TaskCode,
                   x.Task.TaskName,
                   x.Task.Description,
                   x.Task.TaskParent,
                   x.Task.ProjectId,
                   x.Task.AssignedUser,
                   x.Task.Status,
                   x.Task.EstimateTime,
                   x.Task.Level,
                   x.Task.Points,
                   x.Task.ProcessPercent,
                   x.Task.StartTime,
                   x.Task.EndTime,
                   x.Task.CompleteTime,
                   x.Task.CreatedDate,
                   x.Task.CreateBy,
                   x.Task.UpdateDate,
                   x.Task.UpdateBy,
                   x.Task.CreatedName,
                   ProjectName = x.Project?.ProjectName, // Thêm dấu "?" để tránh lỗi khi Project là null
                   TaskParentCode = task?.TaskCode,
                   TaskParentName = task?.TaskName,
                   IsEvaluated = x.Task.IsEvaluated
               })
    .ToList();
                var query2 = lstTaskWithDepartment.Where(x => x.AssignedUser != null)
                              .ToList()
                              .Where(x => x.AssignedUser.Split(',').Any(user => user.Equals(userCode)))
                              .AsQueryable();
                var lstTask = query2.OrderBy(x => x.TaskCode).Skip(offset).Take(limit).ToList();
                if (lstTask.Count > 0)
                {
                    //return new JsonResult(new { status = true, data = lstUser });
                    return new JsonResult(new { status = true, rows = lstTask, total = query2.Count() });
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

        [HttpGet]
        public IActionResult ExcelListTaskUser(string name, string from_date, string to_date, string status, string priority_level, int review, int project_id)
        {
            try
            {
                bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "TaskUser", "Export");
                if (!hasPermission) return new JsonResult(new { status = false, message = "Bạn không có quyền xuất excel công việc!" });
                var tasks = _context.Tasks;
                var users = _context.Users;
                var departments = _context.Departments;
                var projects = _context.Projects;
                var task_evaluate = _context.TaskEvaluates;
                var intermediateResult = tasks
    .GroupJoin(users,
               task => task.CreatedBy,
               user => user.UserCode,
               (task, userGroup) => new { Task = task, Users = userGroup })
    .SelectMany(
               x => x.Users.DefaultIfEmpty(),
               (x, user) => new
               {
                   Task = x.Task,
                   Manager = user,
               })
    .ToList(); // Force execution

                var query = intermediateResult
                    .GroupJoin(task_evaluate,
                               p => p.Task.Id,
                               e => e.TaskId,
                               (p, evaluations) => new { p.Task, p.Manager, Evaluations = evaluations })
                    .SelectMany(
                               x => x.Evaluations.DefaultIfEmpty(),
                               (x, evaluation) => new
                               {
                                   Id = x.Task.Id,
                                   TaskCode = x.Task.TaskCode,
                                   TaskName = x.Task.TaskName,
                                   Description = x.Task.Description,
                                   TaskParent = x.Task.TaskParent,
                                   ProjectId = x.Task.ProjectId,
                                   AssignedUser = x.Task.AssignedUser,
                                   Status = x.Task.Status,
                                   EstimateTime = x.Task.EstimateTime,
                                   Level = x.Task.Level,
                                   Points = x.Task.Points,
                                   ProcessPercent = x.Task.ProcessPercent,
                                   StartTime = x.Task.StartTime,
                                   EndTime = x.Task.EndTime,
                                   CompleteTime = x.Task.CompleteTime,
                                   CreatedDate = x.Task.CreatedDate,
                                   CreateBy = x.Task.CreatedBy,
                                   UpdateDate = x.Task.UpdateDate,
                                   UpdateBy = x.Task.UpdateBy,
                                   CreatedName = x.Manager?.UserName,
                                   IsEvaluated = x.Evaluations != null && x.Evaluations.Any() ? x.Evaluations.First().Id > 0 : false,
                               });



                if (name != null)
                {
                    query = query.Where(x => x.TaskName.Contains(name) || x.TaskCode.Contains(name));
                }
                if (project_id > 0)
                {
                    query = query.Where(x => x.ProjectId == project_id);
                }

                if (status != null)
                {
                    query = query.Where(x => x.Status == status);
                }
                if (priority_level != null)
                {
                    query = query.Where(x => x.Level == priority_level);
                }
                if (from_date != null && to_date != null)
                {
                    DateTime parsedFromDate = DateTime.ParseExact(from_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    DateTime parsedToDate = DateTime.ParseExact(to_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    query = query.Where(x => (x.StartTime >= parsedFromDate && x.StartTime <= parsedToDate) ||
                         (x.EndTime >= parsedFromDate && x.EndTime <= parsedToDate));
                }
                if (review != 0)
                {
                    if (review == 1)
                    {
                        query = query.Where(x => x.IsEvaluated == true);
                    }
                    else
                    {
                        query = query.Where(x => x.IsEvaluated == false);
                    }
                }
                var userCode = HttpContext.Session.GetString("user_code");

                var lstTaskWithDepartment = query
    .GroupJoin(projects,
               t => t.ProjectId,
               p => p.Id,
               (t, projectGroup) => new { Task = t, Projects = projectGroup })
    .SelectMany(
               x => x.Projects.DefaultIfEmpty(),
               (x, project) => new { Task = x.Task, Project = project })
    .GroupJoin(tasks,
               tp => tp.Task.TaskParent,
               d => d.Id,
               (tp, taskGroup) => new { tp.Task, tp.Project, Tasks = taskGroup })
    .SelectMany(
               x => x.Tasks.DefaultIfEmpty(),
               (x, task) => new
               {
                   x.Task.Id,
                   x.Task.TaskCode,
                   x.Task.TaskName,
                   x.Task.Description,
                   x.Task.TaskParent,
                   x.Task.ProjectId,
                   x.Task.AssignedUser,
                   x.Task.Status,
                   x.Task.EstimateTime,
                   x.Task.Level,
                   x.Task.Points,
                   x.Task.ProcessPercent,
                   x.Task.StartTime,
                   x.Task.EndTime,
                   x.Task.CompleteTime,
                   x.Task.CreatedDate,
                   x.Task.CreateBy,
                   x.Task.UpdateDate,
                   x.Task.UpdateBy,
                   x.Task.CreatedName,
                   ProjectName = x.Project?.ProjectName, // Thêm dấu "?" để tránh lỗi khi Project là null
                   TaskParentCode = task?.TaskCode,
                   TaskParentName = task?.TaskName,
                   IsEvaluated = x.Task.IsEvaluated
               })
    .ToList();
                var query2 = lstTaskWithDepartment.Where(x => x.AssignedUser != null)
                              .ToList()
                              .Where(x => x.AssignedUser.Split(',').Any(user => user.Equals(userCode)))
                              .AsQueryable();
                var lstTask = query2.ToList();
                if (lstTask.Count > 0)
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Tasks");

                        // Set custom headers
                        worksheet.Cells[1, 1].Value = "Mã công việc";
                        worksheet.Cells[1, 2].Value = "Tên công việc";
                        worksheet.Cells[1, 3].Value = "Công việc cha";
                        worksheet.Cells[1, 4].Value = "Thuộc dự án";
                        worksheet.Cells[1, 5].Value = "Ngày bắt đầu";
                        worksheet.Cells[1, 6].Value = "Ngày kết thúc";
                        worksheet.Cells[1, 7].Value = "Thời gian";
                        worksheet.Cells[1, 8].Value = "Tiến độ";
                        worksheet.Cells[1, 9].Value = "Ngày hoàn thành";
                        worksheet.Cells[1, 10].Value = "Mức độ";
                        worksheet.Cells[1, 11].Value = "Điểm số";
                        worksheet.Cells[1, 12].Value = "Ngày tạo";
                        worksheet.Cells[1, 13].Value = "Người tạo";
                        
                        // Add more headers as needed...

                        // Populate data
                        for (int i = 0; i < lstTask.Count; i++)
                        {
                            worksheet.Cells[i + 2, 1].Value = lstTaskWithDepartment[i].TaskCode;
                            worksheet.Cells[i + 2, 2].Value = lstTaskWithDepartment[i].TaskName;
                            worksheet.Cells[i + 2, 3].Value = lstTaskWithDepartment[i].TaskParentName;
                            worksheet.Cells[i + 2, 4].Value = lstTaskWithDepartment[i].ProjectName;
                            worksheet.Cells[i + 2, 5].Value = lstTaskWithDepartment[i].StartTime?.ToString("dd/MM/yyyy");
                            worksheet.Cells[i + 2, 6].Value = lstTaskWithDepartment[i].EndTime?.ToString("dd/MM/yyyy");
                            worksheet.Cells[i + 2, 7].Value = lstTaskWithDepartment[i].EstimateTime;
                            worksheet.Cells[i + 2, 8].Value = lstTaskWithDepartment[i].ProcessPercent;
                            worksheet.Cells[i + 2, 9].Value = lstTaskWithDepartment[i].CompleteTime?.ToString("dd/MM/yyyy");
                            worksheet.Cells[i + 2, 10].Value = lstTaskWithDepartment[i].Level;
                            worksheet.Cells[i + 2, 11].Value = lstTaskWithDepartment[i].Points;
                            worksheet.Cells[i + 2, 12].Value = lstTaskWithDepartment[i].CreatedDate?.ToString("dd/MM/yyyy");
                            worksheet.Cells[i + 2, 13].Value = lstTaskWithDepartment[i].CreatedName;
                            
                            // Add more data columns as needed...
                        }

                        // Return the Excel file
                        var stream = new MemoryStream(package.GetAsByteArray());
                        DateTime currentDate = DateTime.Now;

                        // Định dạng ngày giờ thành chuỗi để thêm vào tên tệp
                        string formattedDate = currentDate.ToString("yyyyMMdd_HHmmss");

                        // Thêm chuỗi định dạng vào tên tệp
                        string fileName = $"Danh_sách_công_việc_{formattedDate}.xlsx";
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
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
        [HttpGet]
        public PartialViewResult GetListTaskKabanUser(string name, string from_date, string to_date)
        {
            try
            {
                var tasks = _context.Tasks;
                var users = _context.Users;
                var task_evaluate = _context.TaskEvaluates;
                var intermediateResult = tasks
    .GroupJoin(users,
               task => task.CreatedBy,
               user => user.UserCode,
               (task, userGroup) => new { Task = task, Users = userGroup })
    .SelectMany(
               x => x.Users.DefaultIfEmpty(),
               (x, user) => new
               {
                   Task = x.Task,
                   Manager = user,
               })
    .ToList(); // Force execution

                var query = intermediateResult
                    .GroupJoin(task_evaluate,
                               p => p.Task.Id,
                               e => e.TaskId,
                               (p, evaluations) => new { p.Task, p.Manager, Evaluations = evaluations })
                    .SelectMany(
                               x => x.Evaluations.DefaultIfEmpty(),
                               (x, evaluation) => new
                               {
                                   Id = x.Task.Id,
                                   TaskCode = x.Task.TaskCode,
                                   TaskName = x.Task.TaskName,
                                   Description = x.Task.Description,
                                   TaskParent = x.Task.TaskParent,
                                   ProjectId = x.Task.ProjectId,
                                   AssignedUser = x.Task.AssignedUser,
                                   Status = x.Task.Status,
                                   EstimateTime = x.Task.EstimateTime,
                                   Level = x.Task.Level,
                                   Points = x.Task.Points,
                                   ProcessPercent = x.Task.ProcessPercent,
                                   StartTime = x.Task.StartTime,
                                   EndTime = x.Task.EndTime,
                                   CompleteTime = x.Task.CompleteTime,
                                   CreatedDate = x.Task.CreatedDate,
                                   CreateBy = x.Task.CreatedBy,
                                   UpdateDate = x.Task.UpdateDate,
                                   UpdateBy = x.Task.UpdateBy,
                                   CreatedName = x.Manager?.UserName,
                                   IsEvaluated = x.Evaluations != null && x.Evaluations.Any() ? x.Evaluations.First().Id > 0 : false,
                               });

                if (name != null)
                {
                    query = query.Where(x => x.TaskName.Contains(name) || x.TaskCode.Contains(name));
                }
                if (from_date != null && to_date != null)
                {
                    DateTime parsedFromDate = DateTime.ParseExact(from_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    DateTime parsedToDate = DateTime.ParseExact(to_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    query = query.Where(x => (x.StartTime >= parsedFromDate && x.StartTime <= parsedToDate) ||
                         (x.EndTime >= parsedFromDate && x.EndTime <= parsedToDate));
                }
                var userCode = HttpContext.Session.GetString("user_code");
                query = query.Where(x => x.AssignedUser != null)
                              .ToList()
                              .Where(x => x.AssignedUser.Split(',').Any(user => user.Equals(userCode)))
                              .AsQueryable();
                //var listTaskView = new List<TaskView>();
                var lstTask = query.OrderBy(x => x.TaskCode).ToList();
                var listTaskView = lstTask.Select(task => new TaskView
                {
                    Id = task.Id,
                    TaskCode = task.TaskCode,
                    TaskName = task.TaskName,
                    Description = task.Description,
                    TaskParent = task.TaskParent,
                    ProjectId = task.ProjectId,
                    AssignedUser = task.AssignedUser,
                    Status = task.Status,
                    EstimateTime = task.EstimateTime,
                    Level = task.Level,
                    Points = task.Points,
                    ProcessPercent = task.ProcessPercent,
                    StartTime = task.StartTime,
                    EndTime = task.EndTime,
                    CompleteTime = task.CompleteTime,
                    CreatedDate = task.CreatedDate,
                    CreatedBy = task.CreateBy,
                    UpdateDate = task.UpdateDate,
                    UpdateBy = task.UpdateBy,
                    CreatedName = task.CreatedName, // Assuming Manager is a navigation property in your Task entity
                    IsEvaluated = task.IsEvaluated
                }).ToList();
                return PartialView("~/Views/Task/Partial/_ListTask.cshtml", listTaskView);
            }
            catch (Exception ex)
            {
                return PartialView("~/Views/Task/Partial/_ListTask.cshtml");
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateTaskStatusAsync(IFormCollection model)
        {
            try
            {
                if (model != null)
                {
                    var taskId = int.Parse(model["id"]);
                    var task = _context.Tasks.FirstOrDefault(x => x.Id == taskId);
                    var taskProccess = new TaskProgress()
                    {
                        TaskId = int.Parse(model["id"]),
                        ProjectId = task.ProjectId,
                        ProcessPercent = (model["status"]) == "COMPLETE" ? 100 : task.ProcessPercent,
                        Description = "Cập nhật trạng thái công việc",
                        //Estimate = decimal.Parse(model["estimate"]),
                        CreatedBy = Request.Cookies["user_code"],
                        CreatedDate = DateTime.Now,
                    };
                    if (task != null)
                    {
                        task.Status = model["status"];
                        if(model["status"] == "COMPLETE")
                        {
                            task.ProcessPercent = 100;
                        }
                        _context.Update(task);
                        UpdateParentTaskProgress(task);
                        UpdateProjectProgress(task.ProjectId);
                            Notification notification = new Notification()
                            {
                                Message = "Công việc bạn quản lý có cập nhật tiến độ mới!",
                                Username = task.CreatedBy,
                                Link = "/Task/TaskDetail?id=" + task.Id,
                                NotificationDateTime = DateTime.Now,
                                IsRead = false,
                            };
                            _context.Add(notification);
                            var hubConnections = _context.HubConnections.Where(con => con.Username == task.CreatedBy).OrderByDescending(con => con.Id).FirstOrDefault();
                            if (hubConnections != null)
                            {
                                await _hubContext.Clients.Client(hubConnections.ConnectionId).SendAsync("ReceivedPersonalNotification", "Thông báo", "Công việc bạn quản lý có cập nhật tiến độ mới!");
                            }
                        _context.SaveChanges();
                        var userString = model["assigned_user"].ToString();
                        string[] arrUser = userString.Split(',');
                        
                        return new JsonResult(new { status = true, message = "Cập nhật trạng thái công việc thành công!" });
                    }
                    else
                    {
                        return new JsonResult(new { status = false, message = "Không tìm thấy công việc!" });
                    }
                }
                else
                {
                    return new JsonResult(new { status = false, message = "Dữ liệu không hợp lệ!" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "Error: " + ex.Message });
            }
        }

        private void UpdateParentTaskProgress(Task task)
        {
            if (task.TaskParent != null)
            {
                Task taskParent = _context.Tasks.Find(task.TaskParent);
                if (taskParent != null)
                {
                    UpdateTaskProgress(taskParent);

                    // Recursive call to update progress for the grandparent tasks
                    UpdateParentTaskProgress(taskParent);
                }
            }
        }

        private void UpdateTaskProgress(Task task)
        {
            List<Task> lstTaskChild = _context.Tasks.Where(x => x.TaskParent == task.Id).ToList();
            int countTaskChild = lstTaskChild.Count;
            decimal totalProcess = 0;

            if (countTaskChild > 0)
            {
                foreach (Task taskChild in lstTaskChild)
                {
                    totalProcess += (decimal)taskChild.ProcessPercent;
                }
            }

            task.ProcessPercent = countTaskChild > 0 ? totalProcess / countTaskChild : 0;

            if (task.Status == "NEW")
            {
                task.Status = "PROCESSING";
            }
            else if (task.ProcessPercent == 100)
            {
                task.Status = "COMPLETE";
                task.CompleteTime = DateTime.Now;
            }

            _context.Update(task);
        }

        private void UpdateProjectProgress(int? project_id)
        {
            List<Task> lstTaskChild = _context.Tasks.Where(x => x.ProjectId == project_id && x.TaskParent == null).ToList();
            int countTaskChild = lstTaskChild.Count;
            decimal totalProcess = 0;

            if (countTaskChild > 0)
            {
                foreach (Task taskChild in lstTaskChild)
                {
                    totalProcess += (decimal)taskChild.ProcessPercent;
                }
            }
            var project = _context.Projects.FirstOrDefault(x => x.Id == project_id);

            project.Process = countTaskChild > 0 ? totalProcess / countTaskChild : 0;
            if (project.Status == "NEW" && project.Process > 0)
            {
                project.Status = "PROCESSING";
            }else if(project.Process == 100)
            {
                project.Status = "COMPLETE";
                project.CompleteTime = DateTime.Now;    
            }
            _context.Update(project);
        }

        public JsonResult GetDepartmentByProjectId(int id)
        {
            try
            {
                var project = _context.Projects.Where(x => x.Equals(id)).FirstOrDefault();
                return new JsonResult(new { status = true, departmentCode = project.Department });

            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "Error: " + ex.Message });
            }


        }
        #endregion

        public IActionResult TaskStatistic()
        {
            bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Statistic", "View");
            if (!hasPermission) return RedirectToAction("Author", "Home");
            return View();
        }
        [HttpGet]
        public JsonResult GetListTaskStatistic(int offset, int limit, string from_date, string to_date)
        {
            try
            {
                var query = _context.Tasks;

                // Lọc dữ liệu theo thời gian (nếu có) và thực thi truy vấn
                var filteredTasks = query.ToList();
                if (from_date != null && to_date != null)
                {
                    DateTime parsedFromDate = DateTime.ParseExact(from_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    DateTime parsedToDate = DateTime.ParseExact(to_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    filteredTasks = filteredTasks.Where(x => (x.StartTime >= parsedFromDate && x.StartTime <= parsedToDate) ||
                                                         (x.EndTime >= parsedFromDate && x.EndTime <= parsedToDate)).ToList();
                }
                filteredTasks = filteredTasks.ToList(); // Thực thi truy vấn và lấy dữ liệu

                // Thực hiện thống kê và phân trang
                var projectsByDepartment = filteredTasks
                    .Join(
                        _context.Projects,
                        task => task.ProjectId, // Replace with the actual foreign key property in Project entity
                        project => project.Id,      // Replace with the actual primary key property in Department entity
                        (task, project) => new { Task = task, Project = project }
                    )
                    .GroupBy(joinResult => joinResult.Project)
                    .Select(group => new
                    {
                        Project = group.Key.ProjectName,
                        Department = group.Key.Department,
                        TaskCount = group.Count(),
                        ImportantPriorityTask = group.Count(p => p.Task.Level == "IMPORTANT"),
                        HighPriorityTask = group.Count(p => p.Task.Level == "HIGH"),
                        MediumPriorityTask = group.Count(p => p.Task.Level == "NORMAL"),
                        LowPriorityTask = group.Count(p => p.Task.Level == "LOW"),
                        NewTask = group.Count(p => p.Task.Status == "NEW"),
                        ProcessTask = group.Count(p => p.Task.Status == "PROCESSING"),
                        CompleteTask = group.Count(p => p.Task.Status == "COMPLETE"),
                        EvaluateTask = group.Count(p => p.Task.Status == "EVALUATE"),
                        TotalPoint = group.Sum(x => x.Task.Points)
                    })
                    .ToList(); // Thực thi truy vấn và lấy kết quả

                var lstProject = projectsByDepartment.Skip(offset).Take(limit).ToList();
                if (lstProject.Count > 0)
                {
                    //return new JsonResult(new { status = true, data = lstUser });
                    return new JsonResult(new { status = true, rows = lstProject, total = projectsByDepartment.Count() });
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
        [HttpGet]
        public IActionResult ExcelTaskStatistic(string from_date, string to_date)
        {
            try
            {
                bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Statistic", "Export");
                if (!hasPermission) return new JsonResult(new { status = false, message = "Bạn không có quyền xuất excel báo cáo thống kê!" });
                var query = _context.Tasks;

                // Lọc dữ liệu theo thời gian (nếu có) và thực thi truy vấn
                var filteredTasks = query.ToList();
                if (from_date != null && to_date != null)
                {
                    DateTime parsedFromDate = DateTime.ParseExact(from_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    DateTime parsedToDate = DateTime.ParseExact(to_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    filteredTasks = filteredTasks.Where(x => (x.StartTime >= parsedFromDate && x.StartTime <= parsedToDate) ||
                                                         (x.EndTime >= parsedFromDate && x.EndTime <= parsedToDate)).ToList();
                }
                filteredTasks = filteredTasks.ToList(); // Thực thi truy vấn và lấy dữ liệu

                // Thực hiện thống kê và phân trang
                var projectsByDepartment = filteredTasks
                    .Join(
                        _context.Projects,
                        task => task.ProjectId, // Replace with the actual foreign key property in Project entity
                        project => project.Id,      // Replace with the actual primary key property in Department entity
                        (task, project) => new { Task = task, Project = project }
                    )
                    .GroupBy(joinResult => joinResult.Project)
                    .Select(group => new
                    {
                        Project = group.Key.ProjectName,
                        Department = group.Key.Department,
                        TaskCount = group.Count(),
                        ImportantPriorityTask = group.Count(p => p.Task.Level == "IMPORTANT"),
                        HighPriorityTask = group.Count(p => p.Task.Level == "HIGH"),
                        MediumPriorityTask = group.Count(p => p.Task.Level == "NORMAL"),
                        LowPriorityTask = group.Count(p => p.Task.Level == "LOW"),
                        NewTask = group.Count(p => p.Task.Status == "NEW"),
                        ProcessTask = group.Count(p => p.Task.Status == "PROCESSING"),
                        CompleteTask = group.Count(p => p.Task.Status == "COMPLETE"),
                        EvaluateTask = group.Count(p => p.Task.Status == "EVALUATE"),
                        TotalPoint = group.Sum(x => x.Task.Points)
                    })
                    .ToList(); // Thực thi truy vấn và lấy kết quả

                var lstProject = projectsByDepartment.ToList();
                if (lstProject.Count > 0)
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Tasks");

                        // Set custom headers
                        worksheet.Cells[1, 1].Value = "Dự án";
                        worksheet.Cells[1, 2].Value = "Thuộc phòng ban";
                        worksheet.Cells[1, 3].Value = "Tổng công việc";
                        worksheet.Cells[1, 4].Value = "Mức quan trọng";
                        worksheet.Cells[1, 5].Value = "Mức cao";
                        worksheet.Cells[1, 6].Value = "Mức bình thường";
                        worksheet.Cells[1, 7].Value = "Mức thấp";
                        worksheet.Cells[1, 8].Value = "Công việc mới";
                        worksheet.Cells[1, 9].Value = "Đang thực hiện";
                        worksheet.Cells[1, 10].Value = "Đã hoàn thành";
                        worksheet.Cells[1, 11].Value = "Đã đánh giá";
                        worksheet.Cells[1, 12].Value = "Tổng điểm";

                        // Populate data
                        for (int i = 0; i < lstProject.Count; i++)
                        {
                            worksheet.Cells[i + 2, 1].Value = lstProject[i].Project;
                            worksheet.Cells[i + 2, 2].Value = lstProject[i].Department;
                            worksheet.Cells[i + 2, 3].Value = lstProject[i].TaskCount;
                            worksheet.Cells[i + 2, 4].Value = lstProject[i].ImportantPriorityTask;
                            worksheet.Cells[i + 2, 5].Value = lstProject[i].HighPriorityTask;
                            worksheet.Cells[i + 2, 6].Value = lstProject[i].MediumPriorityTask;
                            worksheet.Cells[i + 2, 7].Value = lstProject[i].LowPriorityTask;
                            worksheet.Cells[i + 2, 8].Value = lstProject[i].NewTask;
                            worksheet.Cells[i + 2, 9].Value = lstProject[i].ProcessTask;
                            worksheet.Cells[i + 2, 10].Value = lstProject[i].CompleteTask;
                            worksheet.Cells[i + 2, 11].Value = lstProject[i].EvaluateTask;
                            worksheet.Cells[i + 2, 12].Value = lstProject[i].TotalPoint;
                            // Add more data columns as needed...
                        }

                        // Return the Excel file
                        var stream = new MemoryStream(package.GetAsByteArray());
                        DateTime currentDate = DateTime.Now;

                        // Định dạng ngày giờ thành chuỗi để thêm vào tên tệp
                        string formattedDate = currentDate.ToString("yyyyMMdd_HHmmss");

                        // Thêm chuỗi định dạng vào tên tệp
                        string fileName = $"Báo cáo_thống_kê_công_việc_{formattedDate}.xlsx";
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
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
    }
}
