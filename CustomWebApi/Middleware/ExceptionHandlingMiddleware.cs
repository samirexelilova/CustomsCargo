using CustomApplication.Common;
using CustomDomain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CustomWebApi.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                context.Response.StatusCode = 499;   
            }
            catch (RequestValidationException ex)
            {
                await Write(context, ex.StatusCode, ex.Message, ex.Errors);
            }
            catch (AppException ex)
            {
                await Write(context, ex.StatusCode, ex.Message);
            }
            catch (DomainException ex)
            {
                await Write(context, StatusCodes.Status409Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Gözlənilməz xəta: {Path}", context.Request.Path);
                await Write(context, StatusCodes.Status500InternalServerError, "Gözlənilməz xəta baş verdi.");
            }
        }

        private static async Task Write(HttpContext ctx, int status, string detail,
            IReadOnlyDictionary<string, string[]>? errors = null)
        {
            var problem = new ProblemDetails { Status = status, Detail = detail, Instance = ctx.Request.Path };
            if (errors is not null) problem.Extensions["errors"] = errors;

            ctx.Response.StatusCode = status;
            await ctx.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
        }
    }

}
