using Application.Common;

namespace Application.DTOs.Request
{
    public class RoleFilterRequest : QueryStringParameters
    {
        public string? Name { get; init; }
    }
}
