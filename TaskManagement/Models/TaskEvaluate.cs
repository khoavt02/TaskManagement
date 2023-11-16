using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public partial class TaskEvaluate
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public int? ProjectId { get; set; }

    public string? Content { get; set; }

    public string? Description { get; set; }

    public decimal? Points { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }
}
