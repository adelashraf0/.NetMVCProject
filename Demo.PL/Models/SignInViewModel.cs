using System.ComponentModel.DataAnnotations;

namespace Demo.PL.Models
{
    public class SignInViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Format For Email")]
        public string Email { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "MinLength 6 char")]
        [MaxLength(8)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
