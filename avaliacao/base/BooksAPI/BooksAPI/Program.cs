using BooksAPI.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace BooksAPI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var (builder, services) = ApiConfigurator.GetAppBuilder(args);

        // Add services to the container.

        builder.Services.AddDbContext<Context>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
        });

        builder.Services.AddControllers().AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
            await app.RunDatabaseMigration<Program>(scope.ServiceProvider, app.Logger);

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync($"Books API ({app.Environment.EnvironmentName})");
            });

            endpoints.MapDefaultControllerRoute();
        });

        app.Run();
    }
}