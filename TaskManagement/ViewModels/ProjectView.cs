using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public partial class ProjectView
{
    public int Id { get; set; }

    public string? ProjectCode { get; set; }

    public string? ProjectName { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }
    public string FormattedStartTime => StartTime.ToString("dd-MM-yyyy");
    public string FormattedEndTime => EndTime.ToString("dd-MM-yyyy");
    public DateTime? CompleteTime { get; set; }

    public string? Manager { get; set; }

    public string? Department { get; set; }
    public string? Users { get; set; }

    public string? Description { get; set; }

    public int? MembersQuantity { get; set; }

    public string? PriorityLevel { get; set; }

    public decimal? Point { get; set; }

    public decimal? Process { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public string? LinkFiles { get; set; }
    public string? DepartmentName { get; set; }
    public string CreatedName { get; set; } 
    public string ManagerName { get; set; } 
}
