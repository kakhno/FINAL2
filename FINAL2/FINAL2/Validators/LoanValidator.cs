using FINAL2.Model;
using FluentValidation;

namespace FINAL2.Validators
{
    public class LoanValidator : AbstractValidator<Loan>
    {
        public LoanValidator()
        {

            RuleFor(x => x.Type)
                   .NotEmpty().WithMessage("Loan type is required.")
                   .IsInEnum().WithMessage("Invalid loan type.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than 0.");
            RuleFor(x => x.Currency).IsInEnum().WithMessage("Invalid currency.");
            RuleFor(x => x.Period).GreaterThan(0).WithMessage("Period must be greater than 0.");
            RuleFor(x => x.Status)
                   .NotEmpty().WithMessage("Status is required.")
                   .IsInEnum().WithMessage("Invalid status.");


        }
    }
}
