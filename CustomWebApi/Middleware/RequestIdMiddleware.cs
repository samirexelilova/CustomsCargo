using Serilog.Context;

namespace CustomWebApi.Middleware
{

    public class RequestIdMiddleware(RequestDelegate next)
    {
        public const string HeaderName = "X-Request-Id";

        public async Task InvokeAsync(HttpContext context)
        {
            var id = context.Request.Headers[HeaderName].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(id) || id.Length > 64 || !id.All(c => char.IsLetterOrDigit(c) || c == '-'))
                id = Guid.NewGuid().ToString("N")[..12];

            context.Response.Headers[HeaderName] = id;

            using (LogContext.PushProperty("RequestId", id))
            {
                await next(context);
            }
        }
    }

}
