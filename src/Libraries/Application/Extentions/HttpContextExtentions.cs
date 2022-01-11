using Microsoft.AspNetCore.Http;

namespace Application.Extentions
{
    public static class HttpContextExtentions
    {
        public static string GetIpAddress(this HttpContext httpContext)
        {
            try
            {
                // get source ip address for the current request
                if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                    return httpContext.Request.Headers["X-Forwarded-For"];
                else
                    return httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
