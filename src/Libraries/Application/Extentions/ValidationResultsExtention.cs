using System;
using FluentValidation.Results;

namespace Application.Extentions
{
    public static class ValidationResultsExtention
    {
        public static void Resolve(this ValidationResult result)
        {
            if (!result.IsValid)
            {
                var errors = string.Empty;
                result.Errors.ForEach(e => errors += e.ErrorMessage + Environment.NewLine);
                throw new ApplicationException(errors);
            }
        }
    }
}
