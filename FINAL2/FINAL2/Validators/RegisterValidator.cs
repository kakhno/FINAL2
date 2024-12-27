using FINAL2.Model;
using FluentValidation;

namespace FINAL2.Validators
{
    public class RegisterValidator : AbstractValidator<Register>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("username should be filled");
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50).MinimumLength(0).WithMessage("please enter your name");
            RuleFor(x => x.Surname).NotEmpty().MaximumLength(50).MinimumLength(0).WithMessage("please enter your surname");
            RuleFor(x => x.Password).NotEmpty().WithMessage("password should be filled");
            RuleFor(x => x.Email).EmailAddress().NotEmpty().WithMessage("email should be filled");
            RuleFor(x => x.Role).NotEmpty().WithMessage("Role should be filled");

        }
    }
}
