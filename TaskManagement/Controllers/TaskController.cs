using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TaskManagement.Helper;
using TaskManagement.Models;
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
                        if(task.Status == "NEW" && taskProccess.ProcessPercent < 100)
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
                    taskParent.ProcessPercent = totalProcess/countTaskChild;
                    if(taskParent.Status == "NEW")
                    {
                        taskParent.Status = "PROCESSING";
                    }else if(taskParent.ProcessPercent == 100)
                    {
                        taskParent.Status = "COMPLETE";

                    }
                    _context.Update(taskParent);
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
    }
}
