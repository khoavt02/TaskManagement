using Microsoft.AspNetCore.Mvc;
using System.Data;
using TaskManagement.Helper;
using TaskManagement.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManagement.Controllers
{
	public class UserController : Controller
	{
		private readonly TaskManagementContext _context;
		private readonly ILogger _logger;
		private readonly IHttpContextAccessor _contextAccessor;
		public UserController(TaskManagementContext context, IHttpContextAccessor contextAccessor)
		{
			this._context = context;
            this._contextAccessor = contextAccessor;

        }
        public IActionResult Index()
		{
            bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "User", "View");
            if (!hasPermission) return RedirectToAction("Author", "Home");
            ViewBag.lstPositions = _context.Positons.ToList();
			ViewBag.lstDepartments = _context.Departments.ToList();
			ViewBag.lstRoles = _context.RoleGroups.ToList();
			return View();
		}
		[HttpPost]
		public JsonResult AddUser(IFormCollection model)
		{
			try
			{

				if (model != null)
				{
					bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "User", "View");
					if (!hasPermission) return new JsonResult(new { status = false, message = "Bạn không có quyền tạo mới nhân viên!" });
					if (model["code"] == "" || model["name"] == "")
					{
						return new JsonResult(new { status = false, message = "Vui lòng nhập mã nhân viên và tên nhân viên!" });
					}
					if (model["account"] == "" || model["password"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng nhập tài khoản và mật khẩu cho nhân viên!" });
                    }
                    if (model["department"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng chọn phòng ban cho nhân viên!" });
                    }
                    if (model["postion"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng chọn chức vụ cho nhân viên!" });
                    }
                    if (model["status"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng chọn trạng thái cho nhân viên!" });
                    }
                    string password = new RSAHelpers().Sha256Hash(model["password"]);
					var user = new User()
					{
						UserCode = model["code"],
                        UserName = model["name"],
						Account = model["account"],
						Password = password,
						DepartmentCode = model["department"],
						DepartmentName = model["department_name"],
						Role = int.Parse(model["role"]),
						Status = model["status"] == "1" ? true : false,
						PositionCode = model["postion"],
						PositionName = model["postion_name"],
						CreatedBy = Request.Cookies["user_code"],
						CreatedDate = DateTime.Now,
					};
					_context.Add(user);
					_context.SaveChanges();
					return new JsonResult(new { status = true, message = "Thêm mới người dùng thành công!" });
				}
				else
				{
					return new JsonResult(new { status = false, message = "Tạo mới người dùng thất bại!" });

				}
			}
			catch (Exception ex)
			{
				return new JsonResult(new { status = false, message = "Error" + ex });
			}
		}
		[HttpPost]
		public JsonResult UpdateUser(IFormCollection model)
		{
			try
			{

				if (model != null)
				{
                    bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "User", "Edit");
                    if (!hasPermission) return new JsonResult(new { status = false, message = "Bạn không có quyền chỉnh sửa nhân viên!" });
                    if (model["code"] == "" || model["name"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng nhập mã nhân viên và tên nhân viên!" });
                    }
                    if (model["account"] == "" || model["password"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng nhập tài khoản và mật khẩu cho nhân viên!" });
                    }
                    if (model["department"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng chọn phòng ban cho nhân viên!" });
                    }
                    if (model["postion"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng chọn chức vụ cho nhân viên!" });
                    }
                    if (model["status"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng chọn trạng thái cho nhân viên!" });
                    }
                    var user = _context.Users.Where(x => x.Id == int.Parse(model["id"])).FirstOrDefault();
					user.UserName = model["name"];
					user.Account = model["account"];
					user.DepartmentCode = model["department"];
					user.DepartmentName = model["department_name"];
					user.Role = int.Parse(model["role"]);
					user.Status = int.Parse(model["status"]) == 1 ? true : false;
					user.PositionCode = model["postion"];
					user.PositionName = model["postion_name"];
					user.UpdateBy = Request.Cookies["user_code"];
					user.UpdatedDate = DateTime.Now;
					
					_context.Update(user);
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
		public JsonResult GetListUser( int offset, int limit)
		{
			try
			{
				var users = _context.Users;
				//var departments = _context.Departments;
				var roles = _context.RoleGroups;
                var query = users
            .GroupJoin(users, e => e.CreatedBy, d => d.UserCode, (e, d) => new { Employee = e, User = d })
            .SelectMany(x => x.User.DefaultIfEmpty(), (x, d) => new { x.Employee, User = d })
            .GroupJoin(roles, e => e.Employee.Role, r => r.Id, (e, r) => new { e.Employee, e.User, Roles = r })
            .SelectMany(x => x.Roles.DefaultIfEmpty(), (x, r) => new
            {
                Id = x.Employee.Id,
                UserName = x.Employee.UserName,
                UserCode = x.Employee.UserCode,
                DepartmentCode = x.Employee.DepartmentCode,
                DepartmentName = x.Employee.DepartmentName,
                Account = x.Employee.Account,
                PositionName = x.Employee.PositionName,
                PositionCode = x.Employee.PositionCode,
                CreatedDate = x.Employee.CreatedDate,
                Role = x.Employee.Role,
                Status = x.Employee.Status,
                CreatedBy = x.User != null ? x.User.UserName : "",
                RoleName = r != null ? r.Name : "Không có chức vụ"
            });
                //List<User> lstUser = _context.Users.ToList();
                var data = (from s in _context.Users select s);
                var lstUser = query.Skip(offset).Take(limit).ToList();
                if (lstUser.Count > 0) {
                    //return new JsonResult(new { status = true, data = lstUser });
                    return new JsonResult(new { status = true, rows = lstUser, total = data.Count() });
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
		public JsonResult GetDetailUserById(int id)
		{
			try
			{
				User user = _context.Users.Where(x => x.Id == id).FirstOrDefault();
				if (user != null)
				{
                    return new JsonResult(new { status = true, data = user });
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
