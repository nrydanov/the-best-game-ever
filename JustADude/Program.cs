using DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace JustADude
{
    public class Program
    {
        public static GameContext db;

        public static void Main(string[] args)
        {
            db = new GameContext();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}