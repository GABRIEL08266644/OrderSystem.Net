using System;
using System.ComponentModel.DataAnnotations;

namespace OrderSystem.Domain.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(@"^\+?[0-9\s\-]{7,15}$", ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; }

        public DateTime? RegistrationDate { get; set; }
    }
}
