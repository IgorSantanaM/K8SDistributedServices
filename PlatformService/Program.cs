using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Endpoints.Internal;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddOpenApi();
if (builder.Environment.IsDevelopment())
{
    services.AddDbContext<AppDbContext>(opt =>
    {
        opt.UseInMemoryDatabase("InMem");
    });
}
else
{
    services.AddDbContext<AppDbContext>(opt =>
    {
        var connectionString = builder.Configuration.GetConnectionString("PlatformSqlDb");
        opt.UseSqlServer(connectionString);
    });
}

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

services.AddScoped<IPlatformRepository, PlatformRepository>();

services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>(opt =>
{
    opt.BaseAddress = new Uri(builder.Configuration["CommandService"]!);
});

services.AddControllers();

var app = builder.Build();

app.UseEndpoints<Program>();
if(app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}
app.PrepPopulation();
app.MapOpenApi();
app.MapControllers();
app.UseHttpsRedirection();