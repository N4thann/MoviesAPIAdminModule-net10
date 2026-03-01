using Domain.Enums;

namespace Domain.SeedWork.Core
{
    public sealed record Failure(FailureType Type, string Message)
    {
        // Mapeia FailureType para códigos de status HTTP
        public int Code => Type switch
        {
            FailureType.Validation => 400,
            FailureType.Unauthorized => 401,
            FailureType.Forbidden => 403,
            FailureType.NotFound => 404,
            FailureType.Conflict => 409,
            FailureType.Unknown => 520, // Web Server Returned an Unknown Error
            _ => 500 // InternalServer e Infrastructure
        };

        // Erros estáticos
        public static readonly Failure InternalServerError = new(FailureType.InternalServer, "An unexpected internal server error occurred.");
        public static readonly Failure InvalidCredentials = new(FailureType.Unauthorized, "Invalid username or password.");
        public static readonly Failure TokenExpired = new(FailureType.Unauthorized, "The provided token has expired.");
        public static readonly Failure Unknown = new(FailureType.Unknown, "An unknown error occurred.");

        // Métodos de fábrica para erros dinâmicos
        public static Failure Validation(string message) => new(FailureType.Validation, message);
        public static Failure NotFound(string entityName, object key) => new(FailureType.NotFound, $"The {entityName} with key '{key}' was not found.");
        public static Failure Conflict(string message) => new(FailureType.Conflict, message);
        public static Failure Infrastructure(string message) => new(FailureType.Infrastructure, message);
        public static Failure Unauthorized(string message) => new(FailureType.Unauthorized, message);
    }
}
