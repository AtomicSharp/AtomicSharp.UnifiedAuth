using System.ComponentModel.DataAnnotations;

namespace AtomicSharp.UnifiedAuth.Controllers.Account
{
    public class LoginInputModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; init; }

        public bool RememberLogin { get; set; }

        public string ReturnUrl { get; init; }
    }
}
