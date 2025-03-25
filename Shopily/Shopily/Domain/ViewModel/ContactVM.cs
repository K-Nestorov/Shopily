using System.ComponentModel.DataAnnotations;

namespace Shopily.Domain.ViewModel
{
    public class ContactVM
    {
        [Required(ErrorMessage = "Името е задължително.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Имейлът е задължителен.")]
        [EmailAddress(ErrorMessage = "Въведете валиден имейл адрес.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Темата е задължителна.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Съобщението е задължително.")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Телефонен номер е задължителен.")]
        [Phone(ErrorMessage = "Въведете валиден телефонен номер.")]
        public string PhoneNumber { get; set; }

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
