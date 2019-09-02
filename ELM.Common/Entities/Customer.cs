using ELM.Common.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ELM.Common.Entities
{
    [Table("Customer")]
    public class Customer : BaseEntity
    {

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(12)]
        [Display(Name = "Contact Phone")]
        public string Phone { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

    }
}
