using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public partial class ModuleView
{
    public int Id { get; set; }

    public string? DisplayName { get; set; }

    public string?  ModuleName{ get; set; }

    public bool? Add { get; set; }

    public bool? Delete { get; set; }

    public bool? View { get; set; }

    public bool? Edit { get; set; }

    public bool? Export { get; set; }
    public bool? Review { get; set; }
    public bool? Comment { get; set; }
}
public class MyRequestModel
{
    public string Modules { get; set; }
    public int RoleGroupId { get; set; }
}