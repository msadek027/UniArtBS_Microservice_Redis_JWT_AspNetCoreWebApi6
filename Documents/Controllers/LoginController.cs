using Documents.DocumentCommon;
using Documents.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Documents.Controllers
{
   

    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerControllerOrder(0)]
    public class LoginController : ControllerBase
    {
        UserCredentialDAO loginRegistrationDAO = new UserCredentialDAO();
        Encryption encryption = new Encryption();
        private readonly IJwtAuth jwtAuth;

        public LoginController(IJwtAuth jwtAuth)
        {
            this.jwtAuth = jwtAuth;
        }

        [AllowAnonymous]
        [HttpPost("Authentication")]
        public IActionResult Authentication([FromForm] UserCredentialBEO userCredential)
        {
            string uid = encryption.Encrypt(userCredential.UserName);
            string pwd = encryption.Encrypt(userCredential.Password);
            string roleId = "";
            string employeeId = "";
            string employeeName = "";
            string designation = "";
            string joiningDate = "";

            bool isTrue = false;
            if ((userCredential.UserName != null) && (userCredential.Password != null))
            {
                var obj = loginRegistrationDAO.CheckUserCredential().Where(m => m.UserName.Equals(userCredential.UserName) && m.Password.Equals(userCredential.Password)).FirstOrDefault();
                if (obj != null)
                {
                    isTrue = true;
                    roleId = obj.RoleId;
                    employeeId = obj.EmployeeId;
                    employeeName = obj.EmployeeName;
                    designation = obj.Designation;
                    joiningDate = obj.JoiningDate;                            
                }
            }
       
            var token = jwtAuth.Authentication(userCredential.UserName, userCredential.Password);
            var refreshToken = jwtAuth.GenerateRefreshToken();
            if (token == null || isTrue == false)
                return Unauthorized();
            // return Ok(token);
            return Ok(new { Token = token, RefreshToken = refreshToken, RoleId = roleId, EmployeeId = employeeId, EmployeeName = employeeName, Designation = designation, JoiningDate = joiningDate });
                    
        }
        [AllowAnonymous]
        [HttpPost("Authentication2")]
        public IActionResult Authentication2([FromBody] UserCredentialBEO userCredential)
        {
            string uid = encryption.Encrypt(userCredential.UserName);
            string pwd = encryption.Encrypt(userCredential.Password);
            string roleId = "";
            string employeeId = "";
            string employeeName = "";
            string designation = "";
            string joiningDate = "";

            bool isTrue = false;
            if ((userCredential.UserName != null) && (userCredential.Password != null))
            {
                var obj = loginRegistrationDAO.CheckUserCredential().Where(m => m.UserName.Equals(userCredential.UserName) && m.Password.Equals(userCredential.Password)).FirstOrDefault();
                if (obj != null)
                {
                    isTrue = true;
                    roleId = obj.RoleId;
                    employeeId = obj.EmployeeId;
                    employeeName = obj.EmployeeName;
                    designation = obj.Designation;
                    joiningDate = obj.JoiningDate;
                }
            }

            var token = jwtAuth.Authentication(userCredential.UserName, userCredential.Password);
            var refreshToken = jwtAuth.GenerateRefreshToken();
            if (token == null || isTrue == false)
                return Unauthorized();
            // return Ok(token);
            return Ok(new { Token = token, RefreshToken = refreshToken, RoleId = roleId, EmployeeId = employeeId, EmployeeName = employeeName, Designation = designation, JoiningDate = joiningDate });

        }
        [AllowAnonymous]
        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(RefreshTokenBEO token)
        {
            var principal = jwtAuth.GetPrincipalFromExpiredToken(token.Token);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default
        
            var newAccessToken = jwtAuth.RGenerateAccessToken(username);        
            var newJwtToken = jwtAuth.GenerateRefreshToken();

            if (newJwtToken == null)
            {
                return Unauthorized("Invalid attempt!");
            }
         
            return Ok(new { Token = newAccessToken/*, RefreshToken = newJwtToken */});
        }
    }
}
