using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Documents.Models
{
    [SwaggerSchema(ReadOnly = true)]
    public class UserCredentialBEO
    {
      
        [Required(ErrorMessage = "User Name is required.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
      

    }
    public class UserCredentialDetailsBEO: UserCredentialBEO
    {
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
