using Application.DataTransfareObjects.Requests;
using FluentValidation;

namespace Application.Validators
{
    public class RegistrationRequestValidator : AbstractValidator<RegisterUserRequestDto>
    {
        public RegistrationRequestValidator()
        {
            RuleFor(x => x.FirstName).NotNull().NotEmpty().WithMessage("FirstName is required.");
            RuleFor(x => x.FirstName).MinimumLength(3);
            RuleFor(x => x.FirstName).MaximumLength(20);
            RuleFor(x => x.LastName).NotNull().NotEmpty().WithMessage("LastName is required.");
            RuleFor(x => x.LastName).MinimumLength(3);
            RuleFor(x => x.LastName).MaximumLength(20);
            RuleFor(x => x.UserEmail).NotEmpty().WithMessage("UserEmail is required").EmailAddress().WithMessage("UserEmail is not valid");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required").Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("ConfirmPassword is required")
                .When(x => x.Password.Equals(x.ConfirmPassword)).WithMessage("Password and ConfirmPassword must be the same");
            RuleFor(x => x.UserType).NotNull().Custom((type, context) =>
            {
                if (!(type is Domain.Enums.UserTypeEnum.Employee || type is Domain.Enums.UserTypeEnum.Guest))
                {
                    context.AddFailure("UserType must be 0 as Guest or 1 as Employee");
                }
            });
        }
    }
}
