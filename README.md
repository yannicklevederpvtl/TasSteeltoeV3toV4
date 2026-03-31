# TasSteeltoeV3toV4

Minimal sample ASP.NET Core apps for **[VMware Tanzu Application Service (TAS)](https://techdocs.broadcom.com/)** showing **Steeltoe 3.x** vs **Steeltoe 4.x** on the same platform: Cloud Foundry configuration, management actuators (Apps Manager integration), and (in v4) OpenTelemetry plus log correlation.

Use this repo to compare patterns side by side or as a starting point for migrating .NET workloads from Steeltoe 3 to 4.

## Contents

| Folder | Steeltoe | Notes |
|--------|----------|--------|
| `TasSteeltoeSampleV3/` | 3.3.x | `WebHost` + `Startup`, `UseCloudHosting`, `AddAllActuators` on host builder |
| `TasSteeltoeSampleV4/` | 4.1.x | `WebApplication`, `AddCloudFoundryConfiguration`, `Services.AddAllActuators`, OpenTelemetry + `AddTracingLogProcessor` |

Each project includes a `manifest.yml` for `cf push` (different app names so both can run in the same space).

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Access to a TAS / Cloud Foundry org and space with the `dotnet_core_buildpack`
- Optional: [cf CLI](https://github.com/cloudfoundry/cli) for deployment

## Build locally

```bash
dotnet build TasSteeltoeSampleV3/TasSteeltoeSample.csproj
dotnet build TasSteeltoeSampleV4/TasSteeltoeSampleV4.csproj
```

## Deploy to TAS

```bash
cf target -o <org> -s <space>

cd TasSteeltoeSampleV3
cf push -f manifest.yml

cd ../TasSteeltoeSampleV4
cf push -f manifest.yml
```

Or publish first, then push the publish output:

```bash
dotnet publish TasSteeltoeSampleV4/TasSteeltoeSampleV4.csproj -c Release -r linux-x64 --self-contained false -o ./publish
cf push -f TasSteeltoeSampleV4/manifest.yml -p TasSteeltoeSampleV4/publish
```

After deployment, open **Apps Manager** for the app and use **Steeltoe** (e.g. Trace, Threads) as your platform allows.

## v4: OpenTelemetry (optional export)

`TasSteeltoeSampleV4` configures OpenTelemetry with optional Zipkin export when **`OTEL_EXPORTER_ZIPKIN_ENDPOINT`** is set (see `OpenTelemetryExtensions.cs`). This follows [Steeltoe v4 tracing guidance](https://steeltoe.io/docs/v4/tracing/index.html).

## Documentation

- [Migrating from Steeltoe 3](https://steeltoe.io/docs/v4/welcome/migrate-quick-steps.html)
- [What's new in Steeltoe 4](https://steeltoe.io/docs/v4/welcome/whats-new.html)
- [Distributed tracing (v4)](https://steeltoe.io/docs/v4/tracing/index.html)
- [Using Spring Boot Actuators with Apps Manager](https://techdocs.broadcom.com/us/en/vmware-tanzu/platform/elastic-application-runtime/10-3/eart/using-actuators.html) (platform docs)

## License

Sample code is provided as-is for demonstration. Add a license file if you need a specific OSS license for redistribution.
