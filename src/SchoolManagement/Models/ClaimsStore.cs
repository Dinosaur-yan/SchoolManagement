﻿using System.Collections.Generic;
using System.Security.Claims;

namespace SchoolManagement.Models
{
    public static class ClaimsStore
    {
        public static List<Claim> Claims = new List<Claim>
        {
            new Claim("Create Role", "Create Role"),
            new Claim("Edit Role", "Edit Role"),
            new Claim("Delete Role", "Delete Role"),
            new Claim("EditStudent", "EditStudent"),
        };
    }
}
