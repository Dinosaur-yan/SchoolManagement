using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SchoolManagement.ViewModels
{
    public class UserEditViewModel
    {
        public UserEditViewModel()
        {
            Claims = new List<Claim>();
            Roles = new List<string>();
        }

        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string City { get; set; }

        public IList<Claim> Claims { get; set; }

        public List<string> Roles { get; set; }
    }
}
