using System.Runtime.Versioning;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HttpFileContentExample
{
    public class Program
    {
        [SupportedOSPlatform("windows")]
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        [SupportedOSPlatform("windows")]
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseHttpSys(options =>
                        {
                            options.AllowSynchronousIO = true;
                            options.MaxConnections = -1;
                            options.MaxRequestBodySize = null;
                            options.UrlPrefixes.Add("http://*:5000");
                        });
                        //.UseKestrel(options =>
                        //{
                        //    options.AllowSynchronousIO = true;
                        //    options.Limits.MaxRequestBodySize = null;
                        //    options.ListenAnyIP(5000);
                        //});
                });
    }
}
