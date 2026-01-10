namespace Otlob.Abstractions;

public static class ApiResultExtensions
{
    public static ObjectResult ToProblem(this ApiResult ApiResult)
    {
        if (ApiResult.IsSuccess)
            throw new InvalidOperationException("can't convert ApiResult to problem");

        var problem = Results.Problem(statusCode: ApiResult.ApiError.StatusCode);

        var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

        problemDetails!.Extensions = new Dictionary<string, object?>
        {
            {
                "errors", new[]
                {
                    ApiResult.ApiError.Code,
                    ApiResult.ApiError.Description
                }
            }
        };

        return new ObjectResult(problemDetails);
    }
}
