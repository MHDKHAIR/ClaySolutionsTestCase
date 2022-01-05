using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace Application.Extentions
{
    public static class IdentitryResultExtentions
    {
        public static void ResolveIdentityErrorResult(this IdentityResult identityResult)
        {
            if (!identityResult.Succeeded)
            {
                var errors = string.Empty;
                identityResult.Errors.ToList().ForEach(e => errors += e.Description + Environment.NewLine);
                throw new ApplicationException(errors);
            }
        }
    }
}
