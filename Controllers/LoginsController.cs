using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SecurityLib;
using SecurityLib.Models;
using SellinBE.Models.Dtos;
using System.Data;
using System.Security.Claims;

namespace SellinBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginsController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        private readonly CryptographyService _cryptographyService;
        private readonly TokenService _tokenService;
        private readonly JwtSettings _jwtSettings;

        public LoginsController(DatabaseService databaseService, 
                                CryptographyService cryptographyService,
                                TokenService tokenService,
                                JwtSettings jwtSettings)
        {
            _databaseService = databaseService;
            _cryptographyService = cryptographyService;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings;
        }

        [HttpGet]
        public KeyValuePair<string, string> SaltAndHash(string password)
        {
            return _cryptographyService.EncryptPassword(password);
        }

        [HttpPost]
        public ActionResult Login(LoginDto login)
        {
            Credential credential = _databaseService.ReadCredentialsOnDb(login.EmailAddress);

            bool login_result;
            if (credential == null)
            {
                return NotFound();
            }
            else
            {
                login_result = _cryptographyService.VerifyPassword(login.Password, credential.PasswordHash, credential.PasswordSalt);
            }

            if (login_result)
            {
                var token = _tokenService.GenerateJwtToken(credential);

                return Ok(new
                {
                    Success = true,
                    Message = "Access Granted",
                    Token = token,
                });
            }
            else
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "Wrong Credentials"
                });
            }
        }

    }
}
