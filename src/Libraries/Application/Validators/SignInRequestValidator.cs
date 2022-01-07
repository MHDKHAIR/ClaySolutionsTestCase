using Application.DataTransfareObjects.Requests;
using FluentValidation;

namespace Application.Validators
{
    public class SignInRequestValidator : AbstractValidator<SignInRequestDto>
    {
        public SignInRequestValidator()
        {
            RuleFor(x => x.UserEmail).NotNull().NotEmpty().WithMessage("UserEmail is required");
            RuleFor(x => x.UserEmail).EmailAddress().WithMessage("UserEmail is not valid");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("UserEmail is not valid");
        }
    }
}
