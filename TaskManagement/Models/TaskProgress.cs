using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public partial class TaskProgress
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public int? ProjectId { get; set; }

    public decimal? ProcessPercent { get; set; }

    public string? FileAttach { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }
}
