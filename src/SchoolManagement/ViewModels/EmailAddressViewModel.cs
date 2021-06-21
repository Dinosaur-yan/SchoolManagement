using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.ViewModels
{
    public class EmailAddressViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
