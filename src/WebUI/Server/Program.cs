using WebUI.Server.Filters;
using WebUI.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Messaging.Application;
using Messaging.Application.Common.Interfaces;
using Messaging.Infrastructure;
using Messaging.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

builder.Services.AddControllersWithViews(options =>
            options.Filters.Add<ApiExceptionFilterAttribute>());

// Customise default API behaviour
builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);

builder.Services.AddRazorPages();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer",
      options =>
           {
               options.Authority = "https://idsrv4.local.net";
               options.Audience = "adminapi";
               options.SaveToken = true;
           });

builder.Services.AddAuthorization(options => options.AddPolicy("api-access", policy =>
{
    policy.RequireAuthenticatedUser();
    policy.RequireClaim("scope", "openid", "email", "adminapi.access");
}));

// Register the Swagger services
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "Web API";
    settings.Version = "v1";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

// Register the Swagger generator and the Swagger UI middleware
app.UseOpenApi();
app.UseSwaggerUi3();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
