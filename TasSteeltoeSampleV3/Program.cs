using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Steeltoe.Common.Hosting;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Steeltoe.Management.Endpoint;

namespace TasSteeltoeSample;

public class Program
{
    public static void Main(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .AddCloudFoundryConfiguration()
            .UseCloudHosting()
            .AddAllActuators()
            .UseStartup<Startup>()
            .Build()
            .Run();
}
