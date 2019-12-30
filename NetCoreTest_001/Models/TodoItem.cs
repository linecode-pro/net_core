using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreTest_001.Models
{
    public class TodoItem
    {
        public int Id { get; set; } // идентификатор задачи

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }
        public string UserId { get; set; } // идентификатор пользователя

        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; } // дата создания задачи
    }
}
