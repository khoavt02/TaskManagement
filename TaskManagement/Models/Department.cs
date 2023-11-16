using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public partial class Department
{
    public int Id { get; set; }

    public string? DepartmentCode { get; set; }

    public string? DepartmentName { get; set; }

    public string? Mannager { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? Status { get; set; }
}
