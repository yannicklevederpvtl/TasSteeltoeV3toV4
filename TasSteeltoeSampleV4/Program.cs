using Steeltoe.Configuration.CloudFoundry;
using TasSteeltoeSampleV4;
using Steeltoe.Logging.DynamicConsole;
using Steeltoe.Management.Endpoint.Actuators.All;
using Steeltoe.Management.Tracing;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// VCAP_* into IConfiguration (TAS / Cloud Foundry).
builder.AddCloudFoundryConfiguration();

// Steeltoe v4: register actuators on services (Apps Manager integration).
builder.Services.AddAllActuators();

// Dynamic console logging + trace/span correlation in log lines (Sleuth-style).
// https://steeltoe.io/docs/v4/tracing/index.html#log-correlation
builder.Logging.AddDynamicConsole();
builder.Services.AddTracingLogProcessor();

// OpenTelemetry tracing/metrics per v4 migration (Zipkin when OTEL_EXPORTER_ZIPKIN_ENDPOINT is set).
// https://steeltoe.io/docs/v4/welcome/migrate-quick-steps.html#opentelemetry
builder.Services.ConfigureOpenTelemetry(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// HTTPS redirection can break actuator traffic on TAS (HTTP); match official samples.
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
