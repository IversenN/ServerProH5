using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H5ServerSideProgrammering.Models.DB
{
    public class Login
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public List<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    }
}