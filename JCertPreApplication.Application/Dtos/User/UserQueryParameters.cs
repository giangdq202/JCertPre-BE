using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.User
{
    public class UserQueryParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchQuery { get; set; }
        public UserStatus? Status { get; set; }
        public Guid? RoleId { get; set; }
        public string? SortBy { get; set; } = "createdAt";
        public bool SortDescending { get; set; } = true;
    }
} 