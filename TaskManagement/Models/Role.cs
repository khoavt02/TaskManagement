using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public partial class Role
{
    public int Id { get; set; }

    public int? RoleGroupId { get; set; }

    public string? ModuleCode { get; set; }

    public bool? Add { get; set; }

    public bool? Delete { get; set; }

    public bool? View { get; set; }

    public bool? Edit { get; set; }

    public bool? Export { get; set; }
}
