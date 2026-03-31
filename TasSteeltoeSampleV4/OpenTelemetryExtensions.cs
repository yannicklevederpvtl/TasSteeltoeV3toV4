using System.Text.RegularExpressions;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Steeltoe.Common;
using B3Propagator = OpenTelemetry.Extensions.Propagators.B3Propagator;

namespace TasSteeltoeSampleV4;

/// <summary>
/// OpenTelemetry setup aligned with Steeltoe v4 guidance (no Steeltoe OTel package required).
/// See https://steeltoe.io/docs/v4/tracing/index.html
/// </summary>
internal static class OpenTelemetryExtensions
{
    private const string DefaultEgressIgnorePattern = "/api/v2/spans|/v2/apps/.*/permissions";
    private static readonly Regex EgressPathMatcher = new(
        DefaultEgressIgnorePattern,
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromSeconds(1));

    public static void ConfigureOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithMetrics(meterProviderBuilder =>
            {
                meterProviderBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                string? zipkinEndpoint = configuration.GetValue<string>("OTEL_EXPORTER_ZIPKIN_ENDPOINT");
                if (!string.IsNullOrEmpty(zipkinEndpoint))
                {
                    // Zipkin remains common on TAS; OTEL marks exporter obsolete in favor of OTLP.
#pragma warning disable CS0618
                    tracerProviderBuilder.AddZipkinExporter();
#pragma warning restore CS0618
                }
            });

        services.ConfigureOpenTelemetryTracerProvider((serviceProvider, tracerProviderBuilder) =>
        {
            var appInfo = serviceProvider.GetRequiredService<IApplicationInstanceInfo>();
            tracerProviderBuilder.SetResourceBuilder(
                ResourceBuilder.CreateDefault().AddService(appInfo.ApplicationName!));

            List<TextMapPropagator> propagators =
            [
                new B3Propagator(),
                new BaggagePropagator()
            ];

            Sdk.SetDefaultTextMapPropagator(new CompositeTextMapPropagator(propagators));
        });

        services.AddOptions<AspNetCoreTraceInstrumentationOptions>().Configure(options =>
        {
            options.Filter += context =>
                !context.Request.Path.StartsWithSegments("/actuator", StringComparison.OrdinalIgnoreCase) &&
                !context.Request.Path.StartsWithSegments("/cloudfoundryapplication", StringComparison.OrdinalIgnoreCase);
        });

        services.AddOptions<HttpClientTraceInstrumentationOptions>().Configure(options =>
        {
            options.FilterHttpRequestMessage += requestMessage =>
                !EgressPathMatcher.IsMatch(requestMessage.RequestUri?.PathAndQuery ?? string.Empty);
        });
    }
}
