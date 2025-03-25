using System.ComponentModel.DataAnnotations;

namespace Shopily.Domain.ViewModel.User
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression("^[a-z0-9]*$", ErrorMessage = "Username can only contain lowercase letters and numbers.")] 
        public string Username { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "First Name should only contain alphabetic characters.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Last Name should only contain alphabetic characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression("^[a-zA-Z0-9._%+-]+@(gmail.com|abv.bg)$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression("(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}", ErrorMessage = "Password must be at least 8 characters long, including letters and numbers.")]
        public string Password { get; set; }
        public bool Role { get; set; }
        public bool IsAdmin { get; set; }
    }
}
