using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public partial class Module
{
    public int Id { get; set; }

    public string? ModuleCode { get; set; }
    public string? ModuleName { get; set; }
    public string? DisplayName { get; set; }
}
