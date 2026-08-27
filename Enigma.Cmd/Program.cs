using Enigma.Cmd;
using Enigma.Extensions.DependencyInjection;
using Enigma.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// The content root has to be the app's own directory rather than the working
// directory, or appsettings.json is missed whenever it is run from elsewhere.
var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Services.AddEnigmaServices();
builder.Services.Configure<EnigmaSettings>(builder.Configuration.GetSection("Enigma"));
builder.Services.AddHostedService<EnigmaConsole>();

await builder.Build().RunAsync();
