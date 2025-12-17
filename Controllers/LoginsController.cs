using Microsoft.AspNetCore.Mvc;
using SecurityLib.Services;
using SecurityLib.Models;
using SellinBE.Models.Dtos;

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
            int a = 10;
            int b = 0;

            Console.WriteLine(a / b);

            return _cryptographyService.EncryptPassword(password);
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginDto login)
        {
            Credential credential = await _databaseService.ReadCredentialsOnDb(login.EmailAddress);

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
                    Message = "Wrong Credentials."
                });
            }
        }

    }
}
