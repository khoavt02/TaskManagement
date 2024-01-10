using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using TaskManagement.Helper;
using TaskManagement.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Task = TaskManagement.Models.Task;

namespace TaskManagement.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskManagementContext _context;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        public TaskController(TaskManagementContext context)
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
        public IActionResult TaskCreate()
        {
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            ViewBag.lstProjects = _context.Projects.ToList();
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
        public JsonResult AddTask(IFormCollection model)
        {
            try
            {
                if (model != null)
                {
                    var TaskMaxId = _context.Tasks.Where(e => e.ProjectId == int.Parse(model["project_id"])).OrderByDescending(e => e.Id).FirstOrDefault();
                    Project project = _context.Projects.Where(e => e.Id == int.Parse(model["project_id"])).FirstOrDefault();
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
                    _context.Add(task);
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
                    _context.Add(task);
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
        public JsonResult UpdateTask(IFormCollection model)
        {
            try
            {
                if (model != null)
                {
                    var TaskMaxId = _context.Tasks.Where(e => e.TaskParent == int.Parse(model["task_id"])).OrderByDescending(e => e.Id).FirstOrDefault();
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
                    _context.Add(task);
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
        public JsonResult AddProccessTask(IFormCollection model)
        {
            try
            {
                if (model != null)
                {

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
                    _context.Add(taskProccess);
                    _context.Update(task);
                    if (task.TaskParent != null)
                    {
                        Task taskParent = _context.Tasks.Where(x => x.Id == task.TaskParent).FirstOrDefault();
                        List<Task> lstTaskChild = _context.Tasks.Where(x => x.TaskParent == taskParent.Id).ToList();
                        int countTaskChild = lstTaskChild.Count;
                        decimal totalProcess = 0;
                        if (countTaskChild > 0)
                        {
                            foreach (Task taskChild in lstTaskChild)
                            {
                                totalProcess = (decimal)(totalProcess + taskChild.ProcessPercent);
                            }
                        }
                        taskParent.ProcessPercent = totalProcess / countTaskChild;
                        if (taskParent.Status == "NEW")
                        {
                            taskParent.Status = "PROCESSING";
                        }
                        else if (taskParent.ProcessPercent == 100)
                        {
                            taskParent.Status = "COMPLETE";
                            taskParent.CompleteTime = DateTime.Now;

                        }
                        _context.Update(taskParent);
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
        public JsonResult AddReviewTask(IFormCollection model)
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
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            return View();
        }
        [HttpGet]
        public JsonResult GetListTask(int offset, int limit, string name, string from_date, string to_date, string status, string priority_level, int review)
        {
            try
            {
                var tasks = _context.Tasks;
                var users = _context.Users;
                var task_evaluate = _context.TaskEvaluates;
                //            var query = tasks
                //.GroupJoin(users,
                //           task => task.CreatedBy,
                //           user => user.UserCode,
                //           (task, userGroup) => new { Task = task, Users = userGroup })
                //.SelectMany(
                //           x => x.Users.DefaultIfEmpty(),
                //           (x, user) => new
                //           {
                //               Task = x.Task,
                //               Manager = user,
                //           })
                //.GroupJoin(task_evaluations,
                //           p => p.Task.Id, // Sửa lại điều kiện join với ProjectId thay vì p.Project.Id
                //           e => e.TaskId,
                //           (p, evaluations) => new { p.Task, p.Manager, Evaluations = evaluations })
                //.SelectMany(
                //           x => x.Evaluations.DefaultIfEmpty(),
                //           (x, evaluation) => new
                //           {
                //               Id = x.Task.Id,
                //               TaskCode = x.Task.TaskCode,
                //               TaskName = x.Task.TaskName,
                //               Description = x.Task.Description,
                //               TaskParent = x.Task.TaskParent,
                //               ProjectId = x.Task.ProjectId,
                //               AssignedUser = x.Task.AssignedUser,
                //               Status = x.Task.Status,
                //               EstimateTime = x.Task.EstimateTime,
                //               Level = x.Task.Level,
                //               Points = x.Task.Points,
                //               ProcessPercent = x.Task.ProcessPercent,
                //               StartTime = x.Task.StartTime,
                //               EndTime = x.Task.EndTime,
                //               CompleteTime = x.Task.CompleteTime,
                //               CreatedDate = x.Task.CreatedDate,
                //               CreateBy = x.Task.CreatedBy,
                //               UpdateDate = x.Task.UpdateDate,
                //               UpdateBy = x.Task.UpdateBy,
                //               CreatedName = x.Manager.UserName, // Sử dụng x.Manager để tránh lỗi khi Manager là null
                //               IsEvaluated = true ? x.Evaluations.FirstOrDefault().Id > 0 : false,
                //           });
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

                // Continue processing finalResult as needed

                // Tiếp tục xử lý câu truy vấn nếu cần



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
                var lstTask = query.OrderBy(x => x.TaskCode).Skip(offset).Take(limit).ToList();
                if (lstTask.Count > 0)
                {
                    //return new JsonResult(new { status = true, data = lstUser });
                    return new JsonResult(new { status = true, rows = lstTask, total = lstTask.Count() });
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
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            return View();
        }
        public IActionResult TaskKabanUser()
        {
            ViewBag.lstPositions = _context.Positons.ToList();
            ViewBag.lstDepartments = _context.Departments.ToList();
            ViewBag.lstRoles = _context.RoleGroups.ToList();
            ViewBag.lstUsers = _context.Users.ToList();
            return View();
        }
        [HttpGet]
        public JsonResult GetListTaskUser(int offset, int limit, string name, string from_date, string to_date, string status, string priority_level, int review)
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

                // Continue processing finalResult as needed

                // Tiếp tục xử lý câu truy vấn nếu cần



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
                query = query.Where(x => x.AssignedUser != null)
                              .ToList()
                              .Where(x => x.AssignedUser.Split(',').Any(user => user.Equals(userCode)))
                              .AsQueryable();
                var lstTask = query.OrderBy(x => x.TaskCode).Skip(offset).Take(limit).ToList();
                if (lstTask.Count > 0)
                {
                    //return new JsonResult(new { status = true, data = lstUser });
                    return new JsonResult(new { status = true, rows = lstTask, total = lstTask.Count() });
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
        #endregion
    }
}
