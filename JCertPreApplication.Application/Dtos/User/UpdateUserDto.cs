using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Dtos.User
{
    public class UpdateUserDto
    {
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string? FullName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        public IFormFile? AvatarFile { get; set; }
    }
} 