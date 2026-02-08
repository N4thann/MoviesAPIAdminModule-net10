using Asp.Versioning;
using Infraestructure.Extensions;
using Microsoft.Extensions.FileProviders;
using MoviesAPIAdminModule.Extensions;
using MoviesAPIAdminModule.Filters;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddHttpContextAccessor(); // Necessário para UseStaticFiles
//builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions()); // Necessário para AWS S3
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(settings =>
{
    settings.PostProcess = document =>
    {
        document.Info.Title = "Movies API Admin Module";
        document.Info.Version = "v1";
        document.Info.Description = "API para administração de filmes, diretores e estúdios.";

        document.Info.Contact = new NSwag.OpenApiContact
        {
            Name = "Nathan Farias",
            Email = "francisco.nathan2@outlook.com",
            Url = "https://www.linkedin.com/in/nathan-farias-5bb97a24"
        };

        document.Info.License = new NSwag.OpenApiLicense
        {
            Name = "Exemplo",
            Url = "https://github.com/N4thann"
        };
    };

    // Define o esquema de segurança para tokens JWT Bearer em integração com Swagger / NSwag
    settings.AddSecurity("Bearer", new NSwag.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Description = "Insira o token JWT: Bearer {seu_token}",
    });

    settings.OperationProcessors.Add(
        new OperationSecurityScopeProcessor("Bearer"));
});

builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
                        new QueryStringApiVersionReader(),
                        new UrlSegmentApiVersionReader()
    );
});

builder.Services.AddScoped<ApiLoggingFilter>();

builder.Logging.ClearProviders(); // Remove todos os provedores de log configurados por padrão pelo ASP.NET Core (como EventLog, Console, Debug)
builder.Logging.AddConsole(); // Adiciona o provedor que escreve logs no console/terminal da aplicação
builder.Logging.AddDebug(); // Adiciona o provedor que escreve logs na janela de saída de depuração do Visual Studio

// Chamadas para métodos de extensão
// 1. Registrar serviços de infraestrutura primeiro (incluindo AddIdentity)
// 2. Registrar serviços da aplicação
// 3. Registrar serviços da Web API por último para que a configuração JWT substitua os padrões do Identity.
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddWebApiServices(builder.Configuration);

var app = builder.Build();

await app.Services.ApplyMigrationsAndSeedAsync();//Um método para aplicar as Migrations automaticamente.
//Uma lógica de Seed (população inicial) que leia o seu arquivo .txt.

if (app.Environment.IsDevelopment())
{
    app.ConfigureExceptionHandler();
    // 1. O gerador (onde o arquivo .json é criado)
    // Precisamos dizer para usar o MESMO caminho que a UI espera.
    app.UseOpenApi(settings =>
    {
        settings.Path = "/openapi/{documentName}/openapi.json";
    });

    // 2. A UI (onde o usuário a visualiza)
    // (O seu estava quase correto, apontando para o caminho correto)
    app.UseSwaggerUi(settings =>
    {
        // Este caminho DEVE ser o mesmo que 'settings.Path' acima
        settings.DocumentPath = "/openapi/{documentName}/openapi.json";
        settings.DocumentTitle = "Movies API - Docs";
    });
}

#region ===== CONFIGURAÇÃO FIXA DE ARQUIVOS ESTÁTICOS =====

// 1. Obter o caminho relativo de appsettings.json
var staticFilesPath = builder.Configuration.GetValue<string>("FileStorageSettings:LocalUploadPath");

if (string.IsNullOrEmpty(staticFilesPath))
    throw new InvalidOperationException("The key 'FileStorageSettings:LocalUploadPath' is not configured in appsettings.json.");


// 2. Construir o caminho físico completo usando a raiz do projeto (não a pasta bin)
var physicalPath = Path.Combine(builder.Environment.ContentRootPath, staticFilesPath);

// 3. Garantir que o diretório exista no disco
if (!Directory.Exists(physicalPath))
    Directory.CreateDirectory(physicalPath);


// 4. Configurar o middleware para servir arquivos da pasta correta
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(physicalPath),
    RequestPath = $"/{staticFilesPath.Replace("\\", "/")}"

});

#endregion

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting(); 
app.UseRateLimiter();

app.UseCors("AllowMyClient");

app.UseAuthentication();
app.UseAuthorization(); 
app.MapControllers();

app.Run();

