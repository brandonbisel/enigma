using Enigma.App;
using Enigma.Extensions.DependencyInjection;
using Enigma.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// The library runs here unchanged. There is no server and no HttpClient: the
// cipher happens in the browser, so nothing typed into this page leaves it.
builder.Services.AddEnigmaServices();
builder.Services.AddSingleton<IKeySheetCatalogue>(new KeySheetCatalogue());

await builder.Build().RunAsync();
