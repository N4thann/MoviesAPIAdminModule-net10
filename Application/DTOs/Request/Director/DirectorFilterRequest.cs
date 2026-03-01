using Application.Common;

namespace Application.DTOs.Request.Director
{
    public class DirectorFilterRequest : QueryStringParameters
    {
        public string? Name { get; init; }
        public string? CountryName { get; init; }
        public int? AgeBegin { get; init; }
        public int? AgeEnd { get; init; }
        public bool? Active { get; init; } = true;
    }
}
