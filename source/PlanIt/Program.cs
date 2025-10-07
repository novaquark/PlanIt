using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using MudBlazor.Services;
using PlanIt.Authentication;
using PlanIt.Services;
using PlanIt.Services.Authentication;

var builder = WebApplication.CreateBuilder(args);

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddFilter("Microsoft", LogLevel.Warning)
           .AddFilter("System", LogLevel.Warning)
           .AddFilter("PlanIt.Program", LogLevel.Debug)
           .AddDebug()
           .AddConsole();
});
var logger = loggerFactory.CreateLogger("PlanItApp");

DotNetEnv.Env.Load("DEV.env");

string? p4PlanServerUrl = Environment.GetEnvironmentVariable("P4PLAN_SERVER") ?? string.Empty;
string? p4PlanProjectWhitelist = Environment.GetEnvironmentVariable("P4PLAN_PROJECT_WHITELIST") ?? string.Empty;
string? p4PlanNextMilestone = Environment.GetEnvironmentVariable("P4PLAN_NEXT_MILESTONE") ?? string.Empty;

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();

// Authentication service selection logic:
// - Use DevelopmentAuthenticationService only if in development environment AND no production env vars are configured
// - Use AuthenticationService if in production environment OR if production env vars are configured (even on dev PC)
var hasProductionConfig = !string.IsNullOrEmpty(p4PlanServerUrl) && !string.IsNullOrEmpty(p4PlanProjectWhitelist);

if (builder.Environment.IsDevelopment() && !hasProductionConfig)
{
    builder.Services.AddSingleton<IAuthenticationService, DevelopmentAuthenticationService>();
}
else
{
    builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
}

builder.Services.AddSingleton(x => logger);
builder.Services.AddSignalR();
builder.Services.AddSession();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddMudServices();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "auth_token";
                    options.LoginPath = "/";
                    options.Cookie.MaxAge = TimeSpan.FromDays(31);
                    options.AccessDeniedPath = "/access-denied";
                });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

if (!string.IsNullOrEmpty(p4PlanServerUrl) && !string.IsNullOrEmpty(p4PlanProjectWhitelist))
{
    builder.Services.AddSingleton<IP4PlanClientProvider>(serviceProvider => new P4PlanClientProvider(p4PlanServerUrl, p4PlanProjectWhitelist));
}
else
{
    builder.Services.AddSingleton<IP4PlanClientProvider>(serviceProvider => new P4PlanDummyClientProvider());
}

builder.Services.AddSingleton<IProjectDetailsService>(serviceProvider =>
{
    var projectDetailsService = new ProjectDetailsService();
    if (!string.IsNullOrEmpty(p4PlanNextMilestone))
    {
        projectDetailsService.NextMilestoneName = p4PlanNextMilestone;
    }
    return projectDetailsService;
});

builder.Services.AddScoped<PlanIt.Components.FilterToolbar.IFilterToolbarLookupService, PlanIt.Components.FilterToolbar.FilterToolbarLookupService>();

builder.Services.AddScoped<IWorkSummaryService, WorkSummaryService>();
var app = builder.Build();
app.UseSession();
app.UseResponseCompression();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();