using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public partial class RoleGroup
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? UserCode { get; set; }

    public string? DepartmentCode { get; set; }
}
