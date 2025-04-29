using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Http;

namespace Core.DTOs
{
    public class RegisterDTO
    {

        public required string NationalId { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required string PhoneNumber { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public Role Role { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public required Location Location { get; set; }
        public IFormFile? Image { get; set; }
    }
}
