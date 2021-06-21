using Microsoft.AspNetCore.Identity;

namespace SchoolManagement.CustomerMiddlewares
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
        {
            return new IdentityError
            {
                Code = "DefaultError",
                Description = $"发生了未知的故障"
            };
        }

        public override IdentityError ConcurrencyFailure()
        {
            return new IdentityError
            {
                Code = "ConcurrencyFailure",
                Description = $"乐观并发失败，对象已被修改"
            };
        }

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code = "PasswordMismatch",
                Description = $"密码错误."
            };
        }

        public override IdentityError InvalidToken()
        {
            return new IdentityError
            {
                Code = "InvalidToken",
                Description = $"无效的令牌."
            };
        }

        public override IdentityError RecoveryCodeRedemptionFailed()
        {
            return new IdentityError
            {
                Code = "RecoveryCodeRedemptionFailed",
                Description = $"未使用修复码."
            };
        }

        public override IdentityError LoginAlreadyAssociated()
        {
            return new IdentityError
            {
                Code = "LoginAlreadyAssociated",
                Description = $"具有次登录的用户已经存在."
            };
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError
            {
                Code = "InvalidUserName",
                Description = $"用户名'{userName}'无效，只能包含字母或数字."
            };
        }

        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError
            {
                Code = "InvalidEmail",
                Description = $"邮箱'{email}'无效."
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = "DuplicateUserName",
                Description = $"用户名'{userName}'已被使用."
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = "DuplicateEmail",
                Description = $"邮箱'{email}'已被使用."
            };
        }

        public override IdentityError InvalidRoleName(string role)
        {
            return new IdentityError
            {
                Code = "InvalidRoleName",
                Description = $"角色名'{role}'无效."
            };
        }

        public override IdentityError DuplicateRoleName(string role)
        {
            return new IdentityError
            {
                Code = "DuplicateRoleName",
                Description = $"角色名'{role}'已被使用."
            };
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return new IdentityError
            {
                Code = "UserAlreadyHasPassword",
                Description = $"该用户已设置了密码."
            };
        }

        public override IdentityError UserLockoutNotEnabled()
        {
            return new IdentityError
            {
                Code = "UserLockoutNotEnabled",
                Description = $"此用户未启用锁定."
            };
        }

        public override IdentityError UserAlreadyInRole(string role)
        {
            return new IdentityError
            {
                Code = "UserAlreadyInRole",
                Description = $"用户已关联角色'{role}'."
            };
        }

        public override IdentityError UserNotInRole(string role)
        {
            return new IdentityError
            {
                Code = "UserNotInRole",
                Description = $"用户未关联角色'{role}'."
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = "PasswordTooShort",
                Description = $"密码必须至少是{length}字符."
            };
        }

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return new IdentityError
            {
                Code = "PasswordRequiresUniqueChars",
                Description = $"密码必须使用至少不同的{uniqueChars}字符."
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = "PasswordRequiresNonAlphanumeric",
                Description = $"密码必须至少有一个非字母数字字符."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = "PasswordRequiresDigit",
                Description = $"密码必须至少有一个数字(0 - 9)."
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = "PasswordRequiresLower",
                Description = $"密码必须至少有一个小写字母(a - z)."
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = "PasswordRequiresUpper",
                Description = $"密码必须至少有一个大写字母(A - Z)."
            };
        }
    }
}
