using System.ComponentModel.DataAnnotations;

namespace Shopily.Domain.ViewModel.User
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [RegularExpression("^[a-zA-Z0-9._%+-]+@(gmail.com|abv.bg|yahoo.com|outlook.com|hotmail.com|aol.com|icloud.com|zoho.com|mail.com|protonmail.com|yandex.com|live.com|msn.com|tutanota.com|inbox.com)$", ErrorMessage = "Please enter a valid email address with one of the allowed domains.")]
        public string Email { get; set; }
    }
}
