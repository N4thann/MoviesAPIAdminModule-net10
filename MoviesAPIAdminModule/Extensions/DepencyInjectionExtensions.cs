using Application.Interfaces;
using Application.Interfaces.Mediator;
using Domain.Identity;
using Domain.SeedWork.Interfaces;
using Infraestructure.Context;
using Infraestructure.Mediator;
using Infraestructure.Persistence;
using Infraestructure.Repository;
using Infraestructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoviesAPIAdminModule.RateLimitOptions;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

namespace MoviesAPIAdminModule.Extensions
{
    public static class DepencyInjectionExtensions
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region CORS
            var nomeDaPoliticaCORS = "AllowMyClient";

            services.AddCors(options =>
            {
                options.AddPolicy(name: nomeDaPoliticaCORS,
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5173") 
                              .AllowAnyHeader() 
                              .AllowAnyMethod(); 
                    });
            });

            #endregion

            #region AUTENTICAÇÃO JWT
            var secretKey = configuration["JWT:SecretKey"] ?? throw new ArgumentException("Invalid secret key!!");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
            #endregion

            #region AUTORIZAÇÃO POLICIES
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => 
                        policy.RequireRole("Admin"));

                options.AddPolicy("SuperAdminOnly", policy =>
                        policy.RequireRole("SuperAdmin"));

                options.AddPolicy("UserOnly", policy => 
                        policy.RequireRole("User"));

                options.AddPolicy("ExclusivePolicyOnly", policy =>
                    policy.RequireAssertion(context =>
                    context.User.HasClaim(claim => claim.Type == "id" && claim.Value == "Nathan")
                    || context.User.IsInRole("SuperAdmin")));

            });
            #endregion

            #region RATE LIMIT
            var myOptions = new MyRateLimitOptions();

            configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(myOptions);

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // --- CONFIGURAÇÃO DO LIMITADOR GLOBAL ---
                //options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>
                //{
                //    var partitionKey = httpcontext.User.Identity?.Name ?? httpcontext.Request.Headers.Host.ToString();

                //    return RateLimitPartition.GetFixedWindowLimiter(
                //        partitionKey: partitionKey,
                //        factory: partition => new FixedWindowRateLimiterOptions
                //        {
                //            AutoReplenishment = true, 
                //            PermitLimit = myOptions.PermitLimit, 
                //            QueueLimit = 0,
                //            Window = TimeSpan.FromSeconds(myOptions.Window)
                //        });
                //});

                // --- DEFINIÇÃO DE POLÍTICAS NOMEADAS ---
                options.AddFixedWindowLimiter(policyName: "fixedwindow", opt =>
                {
                    opt.PermitLimit = 1; 
                    opt.Window = TimeSpan.FromSeconds(5);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; 
                    opt.QueueLimit = 0; 
                });

                // Política 2: "sliding"
                options.AddSlidingWindowLimiter(policyName: "sliding", opt =>
                {
                    opt.PermitLimit = 10;
                    opt.Window = TimeSpan.FromSeconds(10); 
                    opt.SegmentsPerWindow = 2;
                    opt.QueueLimit = 5; 
                });
            });
            #endregion

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registrar o Mediator
            services.AddScoped<IMediator, InMemoryMediator>();

            services.AddScoped<ITokenService, TokenService>();

            // **Registro de Handlers de Comando e Query (Usando varredura de Assembly)**
            var applicationAssembly = typeof(ICommand).Assembly; // Ou qualquer tipo do seu Application.UseCases

            // Registra todos os ICommandHandler (sem retorno)
            services.RegisterCommandHandlers(applicationAssembly);

            // Registra todos os ICommandHandler (com retorno)
            services.RegisterCommandHandlersWithResult(applicationAssembly);

            // ATENÇÃO: Somente registramos Query Handlers COM resultado, pois IQuery<TResult>
            // sempre espera um resultado. O método RegisterQueryHandlers foi removido.
            services.RegisterQueryHandlersWithResult(applicationAssembly);

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>() //Representa usuários e as funções dos usuários
                        .AddEntityFrameworkStores<ApplicationDbContext>() //Utiliza o EF como mecanismo para armazenar os dados
                        .AddDefaultTokenProviders(); //Adicionando os provedores de token necessários

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

           
            // Deixe o código do S3 comentado para o futuro
            /* 
            services.AddAWSService<Amazon.S3.IAmazonS3>();
            services.AddTransient<IFileStorageService, S3StorageService>();
            */

            // Ative a implementação local para desenvolvimento
            services.AddTransient<IFileStorageService, LocalStorageService>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IDirectorRepository, DirectorRepository>();
            services.AddScoped<IStudioRepository, StudioRepository>();
            services.AddScoped<IMovieRepository, MovieRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
        // Métodos de extensão auxiliares para registrar handlers (implementados logo abaixo)
        private static IServiceCollection RegisterCommandHandlers(this IServiceCollection services, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) && !t.IsAbstract && !t.IsInterface)))
            {
                var commandType = type.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)).GetGenericArguments()[0];
                var interfaceType = typeof(ICommandHandler<>).MakeGenericType(commandType);
                services.AddScoped(interfaceType, type);
            }
            return services;
        }

        private static IServiceCollection RegisterCommandHandlersWithResult(this IServiceCollection services, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) && !t.IsAbstract && !t.IsInterface)))
            {
                var genericArguments = type.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)).GetGenericArguments();
                var commandType = genericArguments[0];
                var resultType = genericArguments[1];
                var interfaceType = typeof(ICommandHandler<,>).MakeGenericType(commandType, resultType);
                services.AddScoped(interfaceType, type);
            }
            return services;
        }

        // Este método está correto e é o único necessário para Queries,
        // pois IQueryHandler sempre tem dois argumentos genéricos.
        private static IServiceCollection RegisterQueryHandlersWithResult(this IServiceCollection services, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) && !t.IsAbstract && !t.IsInterface)))
            {
                var genericArguments = type.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)).GetGenericArguments();
                var queryType = genericArguments[0];
                var resultType = genericArguments[1];
                var interfaceType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType);
                services.AddScoped(interfaceType, type);
            }
            return services;
        }
    }
}

/*services.AddScoped<IMediator, InMemoryMediator>();

Esta linha registra a implementação do seu IMediator. Isso significa que, sempre que algo solicitar um IMediator (como o 
seu DirectorController), o ASP.NET Core fornecerá uma instância de InMemoryMediator.
var applicationAssembly = typeof(ICommand).Assembly;

Esta linha obtém o Assembly (o projeto compilado, geralmente a DLL) onde suas interfaces ICommand e ICommandHandler 
e suas implementações de Use Cases (Handlers) estão localizadas (provavelmente o projeto Application). Isso é crucial 
para que a reflexão possa varrer esses tipos.
services.RegisterCommandHandlers(applicationAssembly);

Este método auxiliar que você criou (e está implementado mais abaixo no seu código) faz o seguinte:
Ele varre o applicationAssembly.
Para cada tipo (Type) que ele encontra que implementa a interface genérica ICommandHandler<> (com 1 argumento genérico) e 
não é abstrato/interface, ele registra essa implementação.
Exemplo: Se você tivesse um DeleteDirectorUseCase implementando ICommandHandler<DeleteDirectorCommand>, ele seria registrado aqui.
services.RegisterCommandHandlersWithResult(applicationAssembly);

Este método auxiliar (também implementado mais abaixo) faz o seguinte:
Ele varre o applicationAssembly.
Para cada tipo (Type) que ele encontra que implementa a interface genérica ICommandHandler<,> (com 2 argumentos genéricos) e 
não é abstrato/interface, ele registra essa implementação.
Este é o método que registra o CreateDirectorUseCase! Quando ele encontra CreateDirectorUseCase implementando 
ICommandHandler<CreateDirectorCommand, DirectorInfoResponse>, ele executa uma lógica equivalente a:

"services.AddScoped(typeof(ICommandHandler<CreateDirectorCommand, DirectorInfoResponse>), typeof(CreateDirectorUseCase));"

Ele faz isso para todos os seus handlers que possuem resultado de forma automática.
services.RegisterQueryHandlersWithResult(applicationAssembly);

Faz o mesmo que o anterior, mas para interfaces IQueryHandler<,>.*/
