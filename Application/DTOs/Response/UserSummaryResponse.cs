namespace Application.DTOs.Response
{
    public sealed record UserSummaryResponse(
        string Id,
        string UserName,
        string Email,
        string? PhoneNumber
    );
}
