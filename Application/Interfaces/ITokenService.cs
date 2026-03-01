using Application.Common;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Interfaces
{
    /// <summary>
    /// Define o contrato para um serviço responsável por gerar e validar tokens JWT.
    /// Este serviço lida tanto com Access Tokens (curta duração) quanto com Refresh Tokens (longa duração).
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Gera um JWT (JSON Web Token) de acesso com base nas informações (claims) do usuário.
        /// Este token é de curta duração e é usado para autorizar o acesso a recursos protegidos da API.
        /// </summary>
        /// <param name="claims">Uma coleção de 'claims' que representam a identidade e as permissões do usuário (ex: ID, email, roles).</param>
        /// <param name="_config">A configuração da aplicação (IConfiguration) para ler as definições do JWT, como chave secreta e tempo de validade.</param>
        /// <returns>O token de acesso gerado como um objeto JwtSecurityToken.</returns>
        JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, JwtOptions options);
        /// <summary>
        /// Gera uma string aleatória e criptograficamente segura para ser usada como Refresh Token.
        /// Este token é de longa duração e sua única finalidade é ser trocado por um novo Access Token quando o original expirar.
        /// </summary>
        /// <returns>Uma string representando o Refresh Token gerado (geralmente em formato Base64).</returns>
        string GenerateRefreshToken();
        /// <summary>
        /// Valida a assinatura de um token de acesso expirado e extrai as informações do usuário (ClaimsPrincipal) contidas nele.
        /// Este método é uma parte crucial do fluxo de renovação de token, pois permite identificar o usuário
        /// de um token expirado de forma segura, sem validar seu tempo de vida.
        /// </summary>
        /// <param name="token">A string do token de acesso que já expirou.</param>
        /// <param name="_config">A configuração da aplicação para obter a chave secreta e validar a assinatura do token.</param>
        /// <returns>O 'ClaimsPrincipal' contendo a identidade e as claims do usuário extraídas do token. Lança uma exceção se a assinatura for inválida.</returns>
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token, JwtOptions options);
    }
}
