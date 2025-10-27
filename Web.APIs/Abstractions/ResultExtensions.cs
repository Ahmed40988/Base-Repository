﻿namespace Web.APIs.Abstractions
{
    public static class ResultExtensions
    {
        public static ObjectResult ToProblem(this Result result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("can't convert result success to a problem");

            var problem_link = Results.Problem(statusCode: result.Error.StatusCode);
            var problemDetails = problem_link.GetType()
                .GetProperty(nameof(ProblemDetails))!
                .GetValue(problem_link) as ProblemDetails;

            problemDetails!.Extensions = new Dictionary<string, object?>
            {
                {
                    "error", new
                    {
                        code = result.Error.Code,
                        message = result.Error.Description
                    }
                }
            };

            return new ObjectResult(problemDetails)
            {
                StatusCode = result.Error.StatusCode
            };
        }
    }
}
