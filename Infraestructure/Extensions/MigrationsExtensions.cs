using Domain.Identity;
using Infraestructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructure.Extensions
{
    public static class MigrationsExtensions
    {
        public static async Task ApplyMigrationsAndSeedAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<IdentityRole>>();

            // 1. Aplica as migrations pendentes e cria o banco se não existir
            await context.Database.MigrateAsync();

            string[] roles = { "SuperAdmin", "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            /* INSTRUÇÃO PARA O DONO DO SISTEMA:
               Crie um arquivo chamado 'credentials.txt' na raiz da solução (onde fica o .sln)
               Linha 1: Nome de usuário (ex: Nathan)
               Linha 2: Senha forte (ex: Senha@123)
            */
            var rootPath = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;
            var filePath = Path.Combine(rootPath ?? "", "acessoMaster.txt");

            if (File.Exists(filePath))
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                if (lines.Length >= 2)
                {
                    var userName = lines[0].Trim();
                    var password = lines[1].Trim();

                    if (await userManager.FindByNameAsync(userName) == null)
                    {
                        var superUser = new ApplicationUser
                        {
                            UserName = userName,
                            Email = "admin@system.com", 
                            SecurityStamp = Guid.NewGuid().ToString()
                        };

                        var result = await userManager.CreateAsync(superUser, password);
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(superUser, "SuperAdmin");
                            await userManager.AddToRoleAsync(superUser, "Admin");
                        }
                    }
                }
            }
        }
    }
}
