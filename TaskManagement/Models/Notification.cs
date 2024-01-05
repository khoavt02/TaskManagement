using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public partial class Notification
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Link { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime NotificationDateTime { get; set; }
    }
}
