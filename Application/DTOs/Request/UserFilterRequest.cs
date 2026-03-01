using Application.Common;

namespace Application.DTOs.Request
{
    public class UserFilterRequest : QueryStringParameters
    {
        public string? UserName { get; init; }
        public string? Email { get; init; }
    }
}
