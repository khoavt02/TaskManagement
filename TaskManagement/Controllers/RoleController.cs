using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Reflection;
using TaskManagement.Helper;
using TaskManagement.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManagement.Controllers
{
	public class RoleController : Controller
	{
		private readonly TaskManagementContext _context;
		private readonly ILogger _logger;
		private readonly IHttpContextAccessor _contextAccessor;
		public RoleController(TaskManagementContext context, IHttpContextAccessor contextAccessor)
		{
			this._context = context;
            this._contextAccessor = contextAccessor;
        }
       

        #region RoleGroup
        public IActionResult Index()
		{
            bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Role", "View");
            if (!hasPermission) return RedirectToAction("Author", "Home");
            return View();
		}
		[HttpPost]
		public JsonResult AddRoleGroup(IFormCollection model)
		{
			try
			{
				
				if (model != null)
				{
					var role = new RoleGroup()
					{
						Name = model["name"],
						Status = model["status"] == "1" ? true : false,
						CreatedBy = HttpContext.Session.GetString("user_code"),
						CreatedDate = DateTime.Now,
					};
					_context.Add(role);
					_context.SaveChanges();
					return new JsonResult(new { status = true, message = "Thêm mới nhóm quyền thành công!" });
				}
				else
				{
					return new JsonResult(new { status = false, message = "Tạo mới nhóm quyền thất bại!" });

				}
			}
			catch (Exception ex)
			{
				return new JsonResult(new { status = false, message = "Error" + ex });
			}
		}
		[HttpPost]
		public JsonResult UpdateRoleGroup(IFormCollection model)
		{
			try
			{

				if (model != null)
				{
					var roleGroup = _context.RoleGroups.Where(x => x.Id == int.Parse(model["id"])).FirstOrDefault();
                    roleGroup.Name = model["name"];
                    roleGroup.Status = model["status"] == "1" ? true : false;
                    roleGroup.UpdatedBy = HttpContext.Session.GetString("user_code");
                    roleGroup.UpdatedDate = DateTime.Now;
					_context.Update(roleGroup);
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
		public JsonResult GetListRoleGroup( int offset, int limit)
		{
			try
			{
                var roleGroups = _context.RoleGroups;
                var users = _context.Users;

                var query = roleGroups
    .GroupJoin(users, d => d.UpdatedBy, u => u.UserCode, (d, u) => new { RoleGroup = d, UpdatedBy = u })
    .SelectMany(x => x.UpdatedBy.DefaultIfEmpty(), (x, d) => new { x.RoleGroup, UpdatedBy = d })
    .GroupJoin(users, d => d.RoleGroup.CreatedBy, u => u.UserCode, (d, u) => new { d.RoleGroup, d.UpdatedBy, CreatedUser = u })
    .SelectMany(x => x.CreatedUser.DefaultIfEmpty(), (x, user) => new
    {
		Id = x.RoleGroup.Id,
		Name = x.RoleGroup.Name,
		Status = x.RoleGroup.Status,
		CreatedDate = x.RoleGroup.CreatedDate,
		UpdatedDate = x.RoleGroup.UpdatedDate,
        UpdatedName = x.UpdatedBy != null ? x.UpdatedBy.UserName : string.Empty,
        CreatedName = user != null ? user.UserName : string.Empty,
    });
				var data = _context.RoleGroups.ToList();
                var lstRoleGroup = query.Skip(offset).Take(limit).ToList();
                if (lstRoleGroup.Count > 0) {
                    //return new JsonResult(new { status = true, data = lstUser });
                    return new JsonResult(new { status = true, rows = lstRoleGroup, total = data.Count() });
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
		public JsonResult GetDetailRoleGroupById(int id)
		{
			try
			{
				RoleGroup roleGroup = _context.RoleGroups.Where(x => x.Id == id).FirstOrDefault();
				if (roleGroup != null)
				{
                    return new JsonResult(new { status = true, data = roleGroup });
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
        #endregion

        #region RoleDetail
		public IActionResult RoleGroupDetail(int id)
		{
			var roleGroup = _context.RoleGroups.FirstOrDefault(x => x.Id == id);
			ViewData["roleName"] = roleGroup.Name;
			var lstRoles = _context.Roles.Where(x => x.RoleGroupId ==  id).ToList();	
			var lstModules = _context.Modules.ToList();
            var data = lstModules.GroupJoin(lstRoles,
						module => module.Id,
						lstRoles => lstRoles.ModuleId,
						(module, roles) => new { Module = module, Roles = roles })
						.SelectMany(
							x => x.Roles.DefaultIfEmpty(),
							(x, role) => new
							{
								Id = x.Module.Id,
								ModuleName = x.Module.ModuleName,
								DisplayName = x.Module.DisplayName,
								View = role?.View ?? false,
								Add = role?.Add ?? false,
								Edit = role?.Edit ?? false,
								Delete = role?.Delete ?? false,
								Export = role?.Export ?? false,
								Review = role?.Review ?? false,
								Comment = role?.Comment ?? false,
							}).ToList();


            ViewBag.lstRoles = _context.Roles.Where(x => x.RoleGroupId == id).ToList();
			ViewBag.lstModules = data;
			ViewBag.roleGroupId = id;
            return View();
		}

        [HttpPost]
        public JsonResult UpdateRoleDetail(IFormCollection model)
		{
			try {
                //var modules = JsonConvert.DeserializeObject<List<ModuleView>>(Modules);
                List<ModuleView> modules = JsonConvert.DeserializeObject<List<ModuleView>>(model["modules"]); 
                int roleGroupId = int.Parse(model["roleGroupId"]);
                if (roleGroupId > 0)
				{
					var role = _context.Roles.Where(x => x.RoleGroupId == roleGroupId).ToList();
					if(role.Count > 0)
					{
						foreach(var item in role)
						{
                            _context.Roles.Remove(item);
                        }
                    }
				}
                if (modules != null || modules.Count > 0)
                {
                    foreach (var module in modules)
                    {
						Role role = new Role
						{
							ModuleId = module.Id,
							ModuleName = module.ModuleName,
							RoleGroupId = roleGroupId,
							View = module.View,
							Add = module.Add,
							Edit = module.Edit,
							Delete = module.Delete,
							Export = module.Export,
							Review = module.Review
						};
						_context.Roles.Add(role);
                    }
					_context.SaveChanges();
                    return Json(new { status = true, message = "Cập nhật thành công" });
				}
				else
				{
                    return Json(new { status = false, message = "Cập nhật thất bại" });
                }

            }
			catch (Exception ex)
			{
                return new JsonResult(new { status = false, message = "Error" + ex });
            }
            
        }
        #endregion
    }
}
