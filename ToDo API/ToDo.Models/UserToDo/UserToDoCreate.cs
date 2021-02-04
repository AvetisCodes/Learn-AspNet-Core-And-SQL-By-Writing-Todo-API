using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ToDo.Models.UserToDo
{
    public class UserToDoCreate
    {
        public int UserToDoId { get; set; }

        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MinLength(10, ErrorMessage = "Must be 10-50 characters")]
        [MaxLength(50, ErrorMessage = "Must be 10-50 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MinLength(25, ErrorMessage = "Must be 25-150 characters")]
        [MaxLength(150, ErrorMessage = "Must be 25-150 characters")]
        public string Description { get; set; }

        public bool IsComplete { get; set; }
    }
}
