namespace Domain.Enums
{
    public enum FailureType
    {
        Validation,         // 400
        Unauthorized,       // 401
        Forbidden,          // 403
        NotFound,           // 404
        Conflict,           // 409
        InternalServer,     // 500
        Infrastructure,      // 500
        Unknown             // 500
    }
}
