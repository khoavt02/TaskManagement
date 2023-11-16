using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public partial class User
{
    public int Id { get; set; }

    public string? UserCode { get; set; }

    public string? UserName { get; set; }

    public string? Account { get; set; }

    public string? Password { get; set; }

    public int? Role { get; set; }

    public string? PositionCode { get; set; }

    public string? DepartmentCode { get; set; }

    public string? PositionName { get; set; }

    public string? DepartmentName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdateBy { get; set; }

    public bool? Status { get; set; }
}
