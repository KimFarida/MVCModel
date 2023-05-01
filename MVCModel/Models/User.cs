using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MVCModel.Models
{
    public class User : IdentityUser
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }


        [Required(ErrorMessage = "The password is required.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }
}
