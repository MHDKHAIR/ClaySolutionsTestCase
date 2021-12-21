namespace Domain.Common.Results
{
    public static class Extensions
    {
        public static IResult<T> Fail<T>(this IResult result) => Result<T>.Fail(result.Message);

        public static IResult<T> Success<T>(this T data) => Result<T>.Success(data);

        public static IResult<T> Success<T>(this T data, string message) => Result<T>.Success(data, message);
    }
}
