using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Drawing;
using TaskManagement.Helper;
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
		public ProjectController(TaskManagementContext context)
		{
			this._context = context;
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
					_context.Add(project);
					_context.SaveChanges();
					return new JsonResult(new { status = true, message = "Thêm mới phòng ban thành công!" });
				}
				else
				{
					return new JsonResult(new { status = false, message = "Tạo mới phòng ban thất bại!" });

				}
			}
			catch (Exception ex)
			{
				return new JsonResult(new { status = false, message = "Error" + ex });
			}
		}
		[HttpPost]
		public JsonResult UpdateDepartment(IFormCollection model)
		{
			try
			{

				if (model != null)
				{
					var department = _context.Departments.Where(x => x.Id == int.Parse(model["id"])).FirstOrDefault();
					department.DepartmentCode = model["code"];
					department.DepartmentName = model["name"];
					department.Status = model["status"] == "1" ? true : false;
					department.Mannager = model["management"];
					department.UpdatedBy = Request.Cookies["user_code"];
					department.UpdatedDate = DateTime.Now;
					_context.Update(department);
					_context.SaveChanges();
					return new JsonResult(new { status = true, message = "Cập nhật thành công!" });
				}
				else
				{
					return new JsonResult(new { status = false, message = "Cập nhật thất bại!" });

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
		public JsonResult GetListProject( int offset, int limit)
		{
			try
			{
                var projects = _context.Projects;
                var users = _context.Users;

				var query = projects
	.GroupJoin(users, d => d.Manager, u => u.UserCode, (d, u) => new { Project = d, Manager = u })
	.SelectMany(x => x.Manager.DefaultIfEmpty(), (x, d) => new { x.Project, Manager = d })
	.GroupJoin(users, d => d.Project.CreatedBy, u => u.UserCode, (d, u) => new { d.Project, d.Manager, CreatedUser = u })
	.SelectMany(x => x.CreatedUser.DefaultIfEmpty(), (x, user) => new
	{
		Id = x.Project.Id,
		UpdatedBy = x.Project.UpdatedBy,
		UpdatedDate = x.Project.UpdatedDate,
		CreatedBy = x.Project.CreatedBy,
		CreatedDate = x.Project.CreatedDate,
		Status = x.Project.Status,
		Point = x.Project.Point,
		PriorityLevel = x.Project.PriorityLevel,
		MembersQuantity = x.Project.MembersQuantity,
		Description = x.Project.Description,
		Users = x.Project.Users,
		Department = x.Project.Department,
		Manager = x.Project.Manager,
		Process = x.Project.Process,
		EndTime = x.Project.EndTime,
		StartTime = x.Project.StartTime,
		ProjectName = x.Project.ProjectName,
		ProjectCode = x.Project.ProjectCode,
		ManagerName = x.Manager != null ? x.Manager.UserName : string.Empty,
		CreatedName = user != null ? user.UserName : string.Empty,
	}) ;
                //List<User> lstUser = _context.Users.ToList();
                var data = (from s in _context.Users select s);
                var lstProject = query.Skip(offset).Take(limit).ToList();
                if (lstProject.Count > 0) {
                    //return new JsonResult(new { status = true, data = lstUser });
                    return new JsonResult(new { status = true, rows = lstProject, total = projects.Count() });
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
			ViewBag.projectName = project.ProjectName;
			ViewBag.department = project.Department;

			return View(project);
		}

        [HttpGet]
        public PartialViewResult GetListProjectKaban()
        {
            try
            {
                var projects = _context.Projects;
                var users = _context.Users;

                var query = projects
    .GroupJoin(users, d => d.Manager, u => u.UserCode, (d, u) => new { Project = d, Manager = u })
    .SelectMany(x => x.Manager.DefaultIfEmpty(), (x, d) => new { x.Project, Manager = d })
    .GroupJoin(users, d => d.Project.CreatedBy, u => u.UserCode, (d, u) => new { d.Project, d.Manager, CreatedUser = u })
    .SelectMany(x => x.CreatedUser.DefaultIfEmpty(), (x, user) => new
    {
        Id = x.Project.Id,
        UpdatedBy = x.Project.UpdatedBy,
        UpdatedDate = x.Project.UpdatedDate,
        CreatedBy = x.Project.CreatedBy,
        CreatedDate = x.Project.CreatedDate,
        Status = x.Project.Status,
        Point = x.Project.Point,
        PriorityLevel = x.Project.PriorityLevel,
        MembersQuantity = x.Project.MembersQuantity,
        Description = x.Project.Description,
        Users = x.Project.Users,
        Department = x.Project.Department,
        Manager = x.Project.Manager,
        Process = x.Project.Process,
        EndTime = x.Project.EndTime,
        StartTime = x.Project.StartTime,
        ProjectName = x.Project.ProjectName,
        ProjectCode = x.Project.ProjectCode,
        ManagerName = x.Manager != null ? x.Manager.UserName : string.Empty,
        CreatedName = user != null ? user.UserName : string.Empty,
    });
                //List<User> lstUser = _context.Users.ToList();
                var data = (from s in _context.Users select s);
                var listTaskView = query.Select(project => new ProjectView
                {
                    Id = project.Id,
                    ProjectCode = project.ProjectCode,
                    ProjectName = project.ProjectName,
                    StartTime = project.StartTime,
                    EndTime = project.EndTime,
                    Manager = project.Manager, // Assuming Manager is a navigation property in your Project entity
                    Department = project.Department,
                    Users = project.Users,
                    Description = project.Description,
                    MembersQuantity = project.MembersQuantity,
                    PriorityLevel = project.PriorityLevel,
                    Point = project.Point,
                    Process = project.Process,
                    Status = project.Status,
                    CreatedDate = project.CreatedDate,
                    CreatedBy = project.CreatedBy,
                    UpdatedDate = project.UpdatedDate,
                    UpdatedBy = project.UpdatedBy,
                    CreatedName = project.CreatedName,
                    ManagerName = project.ManagerName
                }).ToList();
                return PartialView("~/Views/Project/Partial/_ListProject.cshtml", listTaskView);
            }
            catch (Exception ex)
            {
                return PartialView("~/Views/Project/Partial/_ListProject.cshtml");
            }
        }
    }
}
