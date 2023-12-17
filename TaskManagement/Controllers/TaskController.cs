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
        [HttpGet]
        public JsonResult GetListDepartment(int offset, int limit)
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
                if (lstDepartment.Count > 0)
                {
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

        #region TaskDetail
        public IActionResult TaskDetail(int id)
        {
            ViewBag.lstPositions = _context.Positons.ToList();
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
                var task_evaluations = _context.TaskEvaluates.ToList();
                var query = tasks
     .GroupJoin(users,
                            task => task.CreatedBy,
                            user => user.UserCode,
                            (task, userGroup) => new { Task = task, Users = userGroup })
                        .SelectMany(
                            x => x.Users.DefaultIfEmpty(),
                            (x, user) => new
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
                                CreatedName = user.UserName,
                            });


                if (name != null)
                {
                    query = query.Where(x => x.TaskName == name || x.TaskCode == name);
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
                    DateTime parsedFromDate = DateTime.ParseExact(from_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime parsedToDate = DateTime.ParseExact(to_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    query = query.Where(x => (x.StartTime >= parsedFromDate && x.StartTime <= parsedToDate) ||
                         (x.EndTime >= parsedFromDate && x.EndTime <= parsedToDate));
                }
                if (review != null)
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
    }
}
