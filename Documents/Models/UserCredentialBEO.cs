using System.ComponentModel.DataAnnotations;

namespace Documents.Models
{
    public class UserCredentialBEO
    {
        [Required]
        [StringLength(10, MinimumLength = 2)]
        public string UserName { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 2)]
        public string Password { get; set; }


        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string JoiningDate { get; set; }
        public string Designation { get; set; }

    }

    public class RefreshTokenBEO
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
