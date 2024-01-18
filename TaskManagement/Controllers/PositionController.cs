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
	public class PositionController : Controller
	{
		private readonly TaskManagementContext _context;
		private readonly ILogger _logger;
		private readonly IHttpContextAccessor _contextAccessor;
		public PositionController(TaskManagementContext context, IHttpContextAccessor contextAccessor)
		{
			this._context = context;
            this._contextAccessor = contextAccessor;
        }
       

        public IActionResult Index()
		{
            bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Position", "View");
            if (!hasPermission) return RedirectToAction("Author", "Home");
            return View();
		}
		[HttpPost]
		public JsonResult AddPosition(IFormCollection model)
		{
			try
			{
				
				if (model != null)
				{
                    bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Position", "Add");
                    if (!hasPermission) return new JsonResult(new { status = false, message = "Bạn không có quyền tạo chức vụ!" });
                    if (model["code"] == "" || model["name"]=="")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng nhập đầy đủ thông tin!" });
                    }
                    var position = new Positions()
					{
						Name = model["name"],
						Code = model["code"],
                        CreatedBy = HttpContext.Session.GetString("user_code"),
                        CreatedDate = DateTime.Now,
                    };
					_context.Add(position);
					_context.SaveChanges();
					return new JsonResult(new { status = true, message = "Tạo mới chức vụ thành công!" });
				}
				else
				{
					return new JsonResult(new { status = false, message = "Tạo mới chức vụ thất bại!" });

				}
			}
			catch (Exception ex)
			{
				return new JsonResult(new { status = false, message = "Error" + ex });
			}
		}
		[HttpPost]
		public JsonResult UpdatePostion(IFormCollection model)
		{
			try
			{

				if (model != null)
				{
                    bool hasPermission = AuthorizationHelper.CheckRole(this._contextAccessor, "Position", "Edit");
                    if (!hasPermission) return new JsonResult(new { status = false, message = "Bạn không có quyền chỉnh sửa chức vụ!" });
                    if (model["code"] == "" || model["name"] == "")
                    {
                        return new JsonResult(new { status = false, message = "Vui lòng nhập đầy đủ thông tin!" });
                    }
                    var position = _context.Positons.Where(x => x.Id == int.Parse(model["id"])).FirstOrDefault();
                    position.Name = model["name"];
                    position.Code = model["code"];
                    position.UpdatedBy = HttpContext.Session.GetString("user_code");
                    position.UpdatedDate = DateTime.Now;
					_context.Update(position);
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
		public JsonResult GetListPosition( int offset, int limit)
		{
			try
			{
                var postion = _context.Positons;
                var users = _context.Users;

                var query = postion
    .GroupJoin(users, d => d.UpdatedBy, u => u.UserCode, (d, u) => new { Postion = d, UpdatedBy = u })
    .SelectMany(x => x.UpdatedBy.DefaultIfEmpty(), (x, d) => new { x.Postion, UpdatedBy = d })
    .GroupJoin(users, d => d.Postion.CreatedBy, u => u.UserCode, (d, u) => new { d.Postion, d.UpdatedBy, CreatedUser = u })
    .SelectMany(x => x.CreatedUser.DefaultIfEmpty(), (x, user) => new
    {
		Id = x.Postion.Id,
		Name = x.Postion.Name,
		Code = x.Postion.Code,
		CreatedDate = x.Postion.CreatedDate,
		UpdatedDate = x.Postion.UpdatedDate,
        UpdatedName = x.UpdatedBy != null ? x.UpdatedBy.UserName : string.Empty,
        CreatedName = user != null ? user.UserName : string.Empty,
    });
				var data = _context.Positons.ToList();
                var lstPosition = query.Skip(offset).Take(limit).ToList();
                if (lstPosition.Count > 0) {
                    //return new JsonResult(new { status = true, data = lstUser });
                    return new JsonResult(new { status = true, rows = lstPosition, total = data.Count() });
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
		public JsonResult GetDetailPositionId(int id)
		{
			try
			{
				Positions positions = _context.Positons.Where(x => x.Id == id).FirstOrDefault();
				if (positions != null)
				{
                    return new JsonResult(new { status = true, data = positions });
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
