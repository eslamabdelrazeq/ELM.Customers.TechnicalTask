using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ELM.Common.Entities.Base
{
    public class BaseEntity
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public string CreatedByUserId { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string UpdatedByUserId { get; set; }
    }
}
