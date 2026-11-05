using Serilog.Context;

namespace CustomWebApi.Middleware
{
    /// <summary>
    /// Hər sorğuya Request ID verir: cavab header-inə (X-Request-Id) yazır və log-lara əlavə edir.
    /// Beləliklə bir sorğunun bütün log sətirlərini eyni ID ilə tapmaq olur.
    /// </summary>
    public class RequestIdMiddleware(RequestDelegate next)
    {
        public const string HeaderName = "X-Request-Id";

        public async Task InvokeAsync(HttpContext context)
        {
            var id = context.Request.Headers[HeaderName].FirstOrDefault();

            // Xaricdən gələn ID-yə güvənmirik: yalnız qısa və təhlükəsiz simvollar (log injection-un qarşısı)
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
