using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public partial class Comment
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public int? ProjectId { get; set; }

    public int? CommentParent { get; set; }

    public string? Content { get; set; }

    public string? FileAttach { get; set; }

    public DateTime? CreatedTime { get; set; }

    public string? CreatedBy { get; set; }
}
