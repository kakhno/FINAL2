using FINAL2.Model;
using FluentValidation;

namespace FINAL2.Validators
{
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50).MinimumLength(0).WithMessage("please enter your name");
            RuleFor(x => x.Surname).NotEmpty().MaximumLength(50).MinimumLength(0).WithMessage("please enter your surname");
            RuleFor(x => x.Password).NotEmpty().MaximumLength(50).MinimumLength(0).WithMessage("please enter your password");
            RuleFor(x => x.Salary).NotEmpty().LessThan(10000).GreaterThan(0).WithMessage("please enteryour monthly income");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username should not be empty");
            RuleFor(x => x.Role).NotEmpty().WithMessage("please specify who are you");
            RuleFor(x => x.Age).NotEmpty().GreaterThan(0).LessThan(120).WithMessage("please enter your correct age");
            RuleFor(x => x.Email).EmailAddress().NotEmpty().WithMessage("email should be filled");



        }
    }
}

