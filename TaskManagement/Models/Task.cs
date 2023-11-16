﻿using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public partial class Task
{
    public int Id { get; set; }

    public string? TaskCode { get; set; }

    public string? TastName { get; set; }

    public int? TaskParent { get; set; }

    public int? ProjectId { get; set; }

    public string? Status { get; set; }

    public decimal? EstimateTime { get; set; }

    public string? Level { get; set; }

    public decimal? Points { get; set; }

    public decimal? ProcessPercent { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateBy { get; set; }
}
