using System.Text.Json.Serialization;

namespace Application.DTOs.Response
{
    /// <summary>
    /// Representa a resposta enviada ao cliente após um login bem-sucedido ou para o endpoint de Refresh Token.
    /// </summary>
    public sealed record TokenResponse(
        [property: JsonPropertyName("accessToken")]
        string AccessToken,

        [property: JsonPropertyName("refreshToken")]
        string RefreshToken,

        [property: JsonPropertyName("expiration")]
        DateTime Expiration
    );
}
