using System.ComponentModel.DataAnnotations;

namespace SellinBE.Models.Dtos
{
    public class LoginDto
    {
        [Required]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
