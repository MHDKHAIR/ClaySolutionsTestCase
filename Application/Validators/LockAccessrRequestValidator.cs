using System;
using Application.DataTransfareObjects.Requests;
using FluentValidation;

namespace Application.Validators
{
    public class LockAccessrRequestValidator : AbstractValidator<AccessLockRequestDto>
    {
        public LockAccessrRequestValidator()
        {
            RuleFor(x => x.DoorKeyCode).NotNull().NotEmpty().WithMessage("DoorKeyCode is required");
            RuleFor(x => x.Location).NotNull().WithMessage("Location is required");
            RuleFor(x => x.Location.Longitude).NotNull().NotEmpty().WithMessage("Longitude is required")
                .ExclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");
            RuleFor(x => x.Location.Latitude).NotNull().NotEmpty().WithMessage("Latitude is required")
                .ExclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");
        }
    }
}
