using SchoolManagement.Models;
using System.Collections.Generic;

namespace SchoolManagement.ViewModels
{
    public class UserClaimsViewModel
    {
        public UserClaimsViewModel()
        {
            Claims = new List<UserClaim>();
        }

        public string UserId { get; set; }

        public List<UserClaim> Claims { get; set; }
    }
}
