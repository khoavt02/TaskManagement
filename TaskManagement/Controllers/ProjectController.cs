using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TaskManagement.Helper;
using TaskManagement.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
		[HttpGet]
		public JsonResult GetListDepartment( int offset, int limit)
		{
			try
			{
                var departments = _context.Departments;
                var users = _context.Users;

                var query = departments
    .GroupJoin(users, d => d.Mannager, u => u.UserCode, (d, u) => new { Department = d, ManagerUser = u })
    .SelectMany(x => x.ManagerUser.DefaultIfEmpty(), (x, d) => new { x.Department, ManagerUser = d })
    .GroupJoin(users, d => d.Department.CreatedBy, u => u.UserCode, (d, u) => new { d.Department, d.ManagerUser, CreatedUser = u })
    .SelectMany(x => x.CreatedUser.DefaultIfEmpty(), (x, user) => new
    {
        Id = x.Department.Id,
        DepartmentCode = x.Department.DepartmentCode,
        DepartmentName = x.Department.DepartmentName,
        CreatedDate = x.Department.CreatedDate,
		Mannager = x.Department.Mannager,
		Status = x.Department.Status,
        Manager = x.ManagerUser != null ? x.ManagerUser.UserName : string.Empty,
        CreatedName = user != null ? user.UserName : string.Empty,
    });
                //List<User> lstUser = _context.Users.ToList();
                var data = (from s in _context.Users select s);
                var lstDepartment = query.Skip(offset).Take(limit).ToList();
                if (lstDepartment.Count > 0) {
                    //return new JsonResult(new { status = true, data = lstUser });
                    return new JsonResult(new { status = true, rows = lstDepartment, total = data.Count() });
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
		public JsonResult GetDetailDepartmentById(int id)
		{
			try
			{
				Department department = _context.Departments.Where(x => x.Id == id).FirstOrDefault();
				if (department != null)
				{
                    return new JsonResult(new { status = true, data = department });
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
	}
}
