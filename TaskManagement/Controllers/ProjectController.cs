using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Drawing;
using System.Globalization;
using TaskManagement.Helper;
using TaskManagement.Hubs;
using TaskManagement.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Task = TaskManagement.Models.Task;

namespace TaskManagement.Controllers
{
    public class ProjectController : Controller
    {
        private readonly TaskManagementContext _context;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ProjectController(TaskManagementContext context, IHubContext<NotificationHub> hubContext, IHttpContextAccessor contextAccessor)
        {
            this._context = context;
            this._hubContext = hubContext;
            this._contextAccessor = contextAccessor;
        }
        public IActionResult Index()
        {
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            return View();
        }
        public IActionResult ProjectCreate()
        {
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            return View();
        }
        [HttpPost]
        public JsonResult AddProject(IFormCollection model)
        {
            try
            {

                if (model != null)
                {

                    var project = new Project()
                    {
                        ProjectCode = model["code"],
                        ProjectName = model["name"],
                        StartTime = DateTime.Now,
                        EndTime = DateTime.Now,
                        Manager = model["manager"],
                        Department = model["department"],
                        Description = model["description"],
                        PriorityLevel = model["PriorityLevel"],
                        Point = int.Parse(model["point"]),
                        Status = model["status"],
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
                    _context.Add(project);
                    _context.SaveChanges();
                    return new JsonResult(new { status = true, message = "Tạo mới dự án thành công!" });
                }
                else
                {
                    return new JsonResult(new { status = false, message = "Tạo mới dự án thất bại!" });

                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false, message = "Error" + ex });
            }
        }

        public IActionResult ListProject()
        {
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            return View();
        }

        [HttpGet]
        public JsonResult GetListProject(int offset, int limit, string name, string from_date, string to_date, string status, string priority_level, string department_s)
        {
            try
            {
                var projects = _context.Projects;
                var users = _context.Users;
                var query = from project in _context.Projects
                            join manager in _context.Users on project.Manager equals manager.UserCode into managerGroup
                            from manager in managerGroup.DefaultIfEmpty()
                            join creator in _context.Users on project.CreatedBy equals creator.UserCode into creatorGroup
                            from creator in creatorGroup.DefaultIfEmpty()
                            join department in _context.Departments on project.Department equals department.DepartmentCode into departmentGroup
                            from department in departmentGroup.DefaultIfEmpty()
                            select new
                            {
                                Id = project.Id,
                                UpdatedBy = project.UpdatedBy,
                                UpdatedDate = project.UpdatedDate,
                                CreatedBy = project.CreatedBy,
                                CreatedDate = project.CreatedDate,
                                Status = project.Status,
                                Point = project.Point,
                                PriorityLevel = project.PriorityLevel,
                                MembersQuantity = project.MembersQuantity,
                                Description = project.Description,
                                Users = project.Users,
                                Department = project.Department,
                                DepartmentName = department != null ? department.DepartmentName : string.Empty,
                                Manager = project.Manager,
                                Process = project.Process,
                                EndTime = project.EndTime,
                                StartTime = project.StartTime,
                                CompleteTime = project.CompleteTime,
                                ProjectName = project.ProjectName,
                                ProjectCode = project.ProjectCode,
                                LinkFiles = project.LinkFiles,
                                ManagerName = manager != null ? manager.UserName : string.Empty,
                                CreatedName = creator != null ? creator.UserName : string.Empty,
                            };

                //List<User> lstUser = _context.Users.ToList();
                if (name != null)
                {
                    query = query.Where(x => x.ProjectName.Contains(name) || x.ProjectCode.Contains(name));
                }

                if (status != null)
                {
                    query = query.Where(x => x.Status == status);
                }
                if (priority_level != null)
                {
                    query = query.Where(x => x.PriorityLevel == priority_level);
                }
                if (from_date != null && to_date != null)
                {
                    DateTime parsedFromDate = DateTime.ParseExact(from_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    DateTime parsedToDate = DateTime.ParseExact(to_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    query = query.Where(x => (x.StartTime >= parsedFromDate && x.StartTime <= parsedToDate) ||
                         (x.EndTime >= parsedFromDate && x.EndTime <= parsedToDate));
                }
                if (department_s != null)
                {
                    query = query.Where(x => x.Department == department_s);
                }
                var data = (from s in _context.Users select s);
                var lstProject = query.OrderByDescending(x => x.CreatedDate).Skip(offset).Take(limit).ToList();
                if (lstProject.Count > 0)
                {
                    //return new JsonResult(new { status = true, data = lstUser });
                    return new JsonResult(new { status = true, rows = lstProject, total = query.Count() });
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

        public IActionResult ProjectDetail(int id)
        {
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            ViewBag.lstProjects = _context.Projects.ToList();
            //Task taskDetail = _context.Tasks.Find(id);
            Project project = _context.Projects.Find(id);
            if(project != null)
            {
                ViewBag.projectName = project.ProjectName;
                ViewBag.department = project.Department;
                ViewBag.ProjectId = id;
            }
            return View(project);
        }

        [HttpGet]
        public PartialViewResult GetListProjectKaban()
        {
            try
            {
                var projects = _context.Projects;
                var users = _context.Users;

                var query = from project in _context.Projects
                            join manager in _context.Users on project.Manager equals manager.UserCode into managerGroup
                            from manager in managerGroup.DefaultIfEmpty()
                            join creator in _context.Users on project.CreatedBy equals creator.UserCode into creatorGroup
                            from creator in creatorGroup.DefaultIfEmpty()
                            join department in _context.Departments on project.Department equals department.DepartmentCode into departmentGroup
                            from department in departmentGroup.DefaultIfEmpty()
                            select new ProjectView
                            {
                                Id = project.Id,
                                UpdatedBy = project.UpdatedBy,
                                UpdatedDate = project.UpdatedDate,
                                CreatedBy = project.CreatedBy,
                                CreatedDate = project.CreatedDate,
                                Status = project.Status,
                                Point = project.Point,
                                PriorityLevel = project.PriorityLevel,
                                MembersQuantity = project.MembersQuantity,
                                Description = project.Description,
                                Users = project.Users,
                                Department = project.Department,
                                DepartmentName = department != null ? department.DepartmentName : string.Empty,
                                Manager = project.Manager,
                                Process = project.Process,
                                EndTime = (DateTime)project.EndTime,
                                StartTime = (DateTime)project.StartTime,
                                CompleteTime = project.CompleteTime,
                                ProjectName = project.ProjectName,
                                ProjectCode = project.ProjectCode,
                                LinkFiles = project.LinkFiles,
                                ManagerName = manager != null ? manager.UserName : string.Empty,
                                CreatedName = creator != null ? creator.UserName : string.Empty,
                            };
                var listProjectView = new List<ProjectView>();
                listProjectView = query.OrderByDescending(x => x.CreatedDate).ToList();
                return PartialView("~/Views/Project/Partial/_ListProject.cshtml", listProjectView);
            }
            catch (Exception ex)
            {
                return PartialView("~/Views/Project/Partial/_ListProject.cshtml");
            }
        }
        [HttpPost]
        public JsonResult UpdateProject(IFormCollection model)
        {
            try
            {
                bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Project", "Edit");
                if (hasPermission)
                {
                    if (model != null)
                    {
                       
                            //if (model["project_code"] == "" || model["project_name"] =="")
                            //{
                            //	return new JsonResult(new { status = false, message = "Vui lòng nhập mã dự án và tên dự án!" });
                            //}
                            //if (model["start_date"] == "" || model["start_date"] == "")
                            //{
                            //	return new JsonResult(new { status = false, message = "Vui lòng chọn ngày bắt đầu và ngày kết thúc!" });
                            //}
                            //if (DateTime.Parse(model["start_date"]) > DateTime.Parse(model["end_date"]))
                            //{
                            //	return new JsonResult(new { status = false, message = "Vui lòng chọn ngày bắt đầu nhỏ hơn ngày kết thúc!" });
                            //}
                            //if (model["manager"] == "" || model["department"] == "")
                            //{
                            //	return new JsonResult(new { status = false, message = "Vui lòng chọn quản lý và phòng ban!" });
                            //}
                            //if (int.Parse(model["point"]) < 0)
                            //{
                            //	return new JsonResult(new { status = false, message = "Điểm số phải lớn hơn 0!" });
                            //}
                            var project_detail = _context.Projects.Where(p => p.Id == int.Parse(model["id"])).FirstOrDefault();
                            if (project_detail != null)
                            {
                                project_detail.ProjectCode = model["project_code"];
                                project_detail.ProjectName = model["project_name"];
                                project_detail.StartTime = DateTime.Parse(model["start_date"]);
                                project_detail.EndTime = DateTime.Parse(model["end_date"]);
                                project_detail.Manager = model["manager"];
                                project_detail.Department = model["department"];
                                project_detail.Description = model["project_description"];
                                project_detail.PriorityLevel = model["priority_level"];
                                project_detail.Point = decimal.Parse(model["point"]);
                                project_detail.Users = model["users"];
                                project_detail.Status = model["status"];
                                project_detail.Process = decimal.Parse(model["process"]);
                                project_detail.UpdatedDate = DateTime.Now;
                                project_detail.UpdatedBy = HttpContext.Session.GetString("user_code");

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
                                    project_detail.LinkFiles = filePath;
                                }
                                Notification notificationMa = new Notification()
                                {
                                    Message = "Dự án bạn quản lý có cập nhật mới!",
                                    Username = model["manager"],
                                    Link = "/Project/ProjectDetail?id=" + project_detail.Id,
                                    NotificationDateTime = DateTime.Now,
                                    IsRead = false,
                                };
                                _context.Add(notificationMa);
                                _context.Update(project_detail);
                                _context.SaveChanges();
                            }

                        var hubConnectionsMa = _context.HubConnections.Where(con => con.Username == model["manager"].ToString()).OrderByDescending(con => con.Id).FirstOrDefault();
                        if (hubConnectionsMa != null && model["manager"].ToString() != HttpContext.Session.GetString("user_code"))
                        {
                             _hubContext.Clients.Client(hubConnectionsMa.ConnectionId).SendAsync("ReceivedPersonalNotification", "Thông báo", "Dự án bạn quản lý có cập nhật mới!");
                        }
                        return new JsonResult(new { status = true, message = "Cập nhật dự án thành công!" });
                    }
                    else
                    {
                        return new JsonResult(new { status = false, message = "Cập nhật dự án thất bại!" });

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

        #region Dự án của tôi

        public IActionResult ListProjectUser()
        {
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            return View();
        }

        [HttpGet]
        public JsonResult GetListProjectUser(int offset, int limit, string name, string from_date, string to_date, string status, string priority_level, string department_s)
        {
            try
            {
                var projects = _context.Projects;
                var users = _context.Users;
                var query = from project in _context.Projects
                            join manager in _context.Users on project.Manager equals manager.UserCode into managerGroup
                            from manager in managerGroup.DefaultIfEmpty()
                            join creator in _context.Users on project.CreatedBy equals creator.UserCode into creatorGroup
                            from creator in creatorGroup.DefaultIfEmpty()
                            join department in _context.Departments on project.Department equals department.DepartmentCode into departmentGroup
                            from department in departmentGroup.DefaultIfEmpty()
                            select new
                            {
                                Id = project.Id,
                                UpdatedBy = project.UpdatedBy,
                                UpdatedDate = project.UpdatedDate,
                                CreatedBy = project.CreatedBy,
                                CreatedDate = project.CreatedDate,
                                Status = project.Status,
                                Point = project.Point,
                                PriorityLevel = project.PriorityLevel,
                                MembersQuantity = project.MembersQuantity,
                                Description = project.Description,
                                Users = project.Users,
                                Department = project.Department,
                                DepartmentName = department != null ? department.DepartmentName : string.Empty,
                                Manager = project.Manager,
                                Process = project.Process,
                                EndTime = project.EndTime,
                                StartTime = project.StartTime,
                                CompleteTime = project.CompleteTime,
                                ProjectName = project.ProjectName,
                                ProjectCode = project.ProjectCode,
                                LinkFiles = project.LinkFiles,
                                ManagerName = manager != null ? manager.UserName : string.Empty,
                                CreatedName = creator != null ? creator.UserName : string.Empty,
                            };

                //List<User> lstUser = _context.Users.ToList();
                if (name != null)
                {
                    query = query.Where(x => x.ProjectName.Contains(name) || x.ProjectCode.Contains(name));
                }

                if (status != null)
                {
                    query = query.Where(x => x.Status == status);
                }
                if (priority_level != null)
                {
                    query = query.Where(x => x.PriorityLevel == priority_level);
                }
                if (from_date != null && to_date != null)
                {
                    DateTime parsedFromDate = DateTime.ParseExact(from_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    DateTime parsedToDate = DateTime.ParseExact(to_date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    query = query.Where(x => (x.StartTime >= parsedFromDate && x.StartTime <= parsedToDate) ||
                         (x.EndTime >= parsedFromDate && x.EndTime <= parsedToDate));
                }
                if (department_s != null)
                {
                    query = query.Where(x => x.Department == department_s);
                }
                var userCode = HttpContext.Session.GetString("user_code");
                query = query.Where(x => x.Users != null)
                              .ToList()
                              .Where(x => x.Users.Split(',').Any(user => user.Equals(userCode)))
                              .AsQueryable();

                var data = (from s in _context.Users select s);
                var lstProject = query.OrderByDescending(x => x.CreatedDate).Skip(offset).Take(limit).ToList();
                if (lstProject.Count > 0)
                {
                    //return new JsonResult(new { status = true, data = lstUser });
                    return new JsonResult(new { status = true, rows = lstProject, total = query.Count() });
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

        public IActionResult ProjectKabanUser()
        {
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            return View();
        }
        [HttpGet]
        public PartialViewResult GetListProjectKabanUser()
        {
            try
            {
                var projects = _context.Projects;
                var users = _context.Users;

                var query = from project in _context.Projects
                            join manager in _context.Users on project.Manager equals manager.UserCode into managerGroup
                            from manager in managerGroup.DefaultIfEmpty()
                            join creator in _context.Users on project.CreatedBy equals creator.UserCode into creatorGroup
                            from creator in creatorGroup.DefaultIfEmpty()
                            join department in _context.Departments on project.Department equals department.DepartmentCode into departmentGroup
                            from department in departmentGroup.DefaultIfEmpty()
                            select new ProjectView
                            {
                                Id = project.Id,
                                UpdatedBy = project.UpdatedBy,
                                UpdatedDate = project.UpdatedDate,
                                CreatedBy = project.CreatedBy,
                                CreatedDate = project.CreatedDate,
                                Status = project.Status,
                                Point = project.Point,
                                PriorityLevel = project.PriorityLevel,
                                MembersQuantity = project.MembersQuantity,
                                Description = project.Description,
                                Users = project.Users,
                                Department = project.Department,
                                DepartmentName = department != null ? department.DepartmentName : string.Empty,
                                Manager = project.Manager,
                                Process = project.Process,
                                EndTime = (DateTime)project.EndTime,
                                StartTime = (DateTime)project.StartTime,
                                CompleteTime = project.CompleteTime,
                                ProjectName = project.ProjectName,
                                ProjectCode = project.ProjectCode,
                                LinkFiles = project.LinkFiles,
                                ManagerName = manager != null ? manager.UserName : string.Empty,
                                CreatedName = creator != null ? creator.UserName : string.Empty,
                            };
                var listProjectView = new List<ProjectView>();
                var userCode = HttpContext.Session.GetString("user_code");
                query = query.Where(x => x.Users != null)
                              .ToList()
                              .Where(x => x.Users.Split(',').Any(user => user.Equals(userCode)))
                              .AsQueryable();
                listProjectView = query.OrderByDescending(x => x.CreatedDate).ToList();
                return PartialView("~/Views/Project/Partial/_ListProject.cshtml", listProjectView);
            }
            catch (Exception ex)
            {
                return PartialView("~/Views/Project/Partial/_ListProject.cshtml");
            }
        }
        #endregion
    }
}
