using PlatformService.Dtos;
using System.Text;
using System.Text.Json;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration) : ICommandDataClient
    {
        public async Task SendPlatformToCommand(PlatformReadDto platform)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platform), 
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsJsonAsync($"{configuration.GetSection("CommandService")}", httpContent);

            if(response.IsSuccessStatusCode)
                Console.WriteLine("200 - Success");
            else
                    Console.WriteLine("Failed");
        }
    }
}
