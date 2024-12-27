using FINAL2.Model;
using FluentValidation;

namespace FINAL2.Validators
{
    public class LoginValidator : AbstractValidator<Login>
    {
        public LoginValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("FirstName should be filled");
            RuleFor(x => x.Password).NotEmpty().WithMessage("should be filled");

        }
    }
}