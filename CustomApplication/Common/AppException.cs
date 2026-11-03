using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApplication.Common
{
    public class AppException(string message, int statusCode) : Exception(message)
    {
        public int StatusCode { get; } = statusCode;
    }

    public class NotFoundException(string message) : AppException(message, 404);
    public class ConflictException(string message) : AppException(message, 409);
    public class ForbiddenException(string message) : AppException(message, 403);
    public class UnauthorizedAppException(string message) : AppException(message, 401);
    public class LockedOutException(string message) : AppException(message, 423);

    public class RequestValidationException(IReadOnlyDictionary<string, string[]> errors)
        : AppException("Sorguda xətalar var", 422)
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
    }
}
