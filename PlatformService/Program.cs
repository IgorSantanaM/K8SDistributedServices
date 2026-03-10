using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Endpoints.Internal;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddOpenApi();
services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseInMemoryDatabase("InMem");
});

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

services.AddScoped<IPlatformRepository, PlatformRepository>();

services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>(opt =>
{
    opt.BaseAddress = new Uri(builder.Configuration["CommandService"]!);
});

services.AddControllers();

var app = builder.Build();

app.UseEndpoints<Program>();
if (app.Environment.IsDevelopment())
{
    app.PrepPopulation();
    app.MapOpenApi();
}
app.MapControllers();
app.UseHttpsRedirection();