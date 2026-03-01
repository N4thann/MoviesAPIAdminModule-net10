using Application.Common;

namespace Application.DTOs.Request.Studio
{
    public class StudioFilterRequest : QueryStringParameters
    {
        public string? Name { get; init; }
        public string? CountryName { get; init; }
        public int? FoundationYearBegin { get; init; }
        public int? FoundationYearEnd { get; init; }  
        public bool? Active { get; init; } = true;
    }
}
