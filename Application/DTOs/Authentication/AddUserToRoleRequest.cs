namespace Application.DTOs.Authentication
{
    public sealed record AddUserToRoleRequest(string Email, string RoleName);
}
