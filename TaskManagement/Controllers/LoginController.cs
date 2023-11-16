using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using TaskManagement.Helper;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    public class LoginController : Controller
    {
        private readonly TaskManagementContext _context;
        private readonly ILogger _logger;
		private readonly IHttpContextAccessor _contextAccessor;
		public LoginController(TaskManagementContext context)
        {
            this._context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Login(IFormCollection model)
        {
            try 
            {
                string passwordSHA = new RSAHelpers().Sha256Hash(model["password"]);
                var user = _context.Users.Where(x => x.Account == (model["account"]).ToString() && x.Password == passwordSHA).FirstOrDefault();
                if (user != null) {
					Response.Cookies.Delete("AspNetCore.Security");
					Response.Cookies.Delete("AspNetCore.Security.Out");
					Response.Cookies.Append("account", user.Account);
					Response.Cookies.Append("user_code", user.UserCode);
					//string userData = JsonConvert.SerializeObject(user);
					//SessionHelpers.Set(_contextAccessor, userData, 10 * 365);
					return new JsonResult(new { status = true, message = "Đăng nhập thành công" });
				}
				else
                {
					return new JsonResult(new { status = false, message = "Tài khoản hoặc mật khẩu chưa đúng!" });

				}
			}
            catch(Exception ex) 
            {
				return new JsonResult(new { status = false, message = "Error" + ex });
			}
        }
        public IActionResult ChangePassword()
        {
            return View();
        }

		[HttpPost]
		public JsonResult ActionChangePassword(IFormCollection model)
		{
			try
			{
				var account = Request.Cookies["account"];
				var user = _context.Users.Where(x => x.Account == account).FirstOrDefault();
				var oldPassword = model["old_password"];
				var newPassword = model["new_password"];
				var rePassword = model["re_password"];
				string passwordSHA = new RSAHelpers().Sha256Hash(model["old_password"]);
				if(passwordSHA != user.Password)
				{
					return new JsonResult(new { status = true, message = "Mật khẩu cũ không đúng!" });
				}
				if (oldPassword != newPassword && newPassword == rePassword)
				{
					string newPass = new RSAHelpers().Sha256Hash(newPassword);
					user.Password = newPass;
					_context.Update(user);
					_context.SaveChanges();
					return new JsonResult(new { status = true, message = "Đổi mật khẩu thành cônng" });
				}
				else
				{
					return new JsonResult(new { status = false, message = "Đổi mật khẩu thất bại!" });

				}
			}
			catch (Exception ex)
			{
				return new JsonResult(new { status = false, message = "Error" + ex });
			}
		}

		[HttpPost]
		public IActionResult Logout()
		{
			Response.Clear();
			return RedirectToPage("/Login/Index");
		}

	}
}