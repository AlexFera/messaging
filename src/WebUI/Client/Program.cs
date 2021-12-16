using WebUI.Client;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using WebUI.Client.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddHttpClient("Messaging.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
    .ConfigureHandler(
        authorizedUrls: new[] { builder.HostEnvironment.BaseAddress },
        scopes: new[] { "openid email adminapi.access" }));

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Messaging.ServerAPI").EnableIntercept(sp));

builder.Services.AddScoped<WebUI.Client.Services.IMessagesClient, WebUI.Client.Services.MessagesClient>();

builder.Services.AddOidcAuthentication(options =>
    {
        builder.Configuration.Bind("Local", options.ProviderOptions);
        options.UserOptions.RoleClaim = "role";
    }).AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory>();

builder.Services.AddLoadingBar();

builder.UseLoadingBar();
builder.Services.AddBlazoredToast();

builder.Services.AddDbContextFactory<ClientSideDbContext>(
           options => options.UseSqlite($"Filename={DataSynchronizer.SqliteDbFilename}"));
builder.Services.AddScoped<DataSynchronizer>();

await builder.Build().RunAsync();
