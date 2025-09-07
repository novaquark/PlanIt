using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using MudBlazor.Services;
using PlanIt.Services.Authentication;
using PlanIt.Services.Configuration;
using PlanIt.Services.Interfaces;
using PlanIt.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddFilter("Microsoft", LogLevel.Warning)
           .AddFilter("System", LogLevel.Warning)
           .AddFilter("PlanIt.Program", LogLevel.Debug)
           .AddDebug()
           .AddConsole();
});
var logger = loggerFactory.CreateLogger("PlanItApp");

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddSession();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddMudServices();
builder.Services.AddHttpClient();

// Register configuration services
builder.Services.AddSingleton<IDevelopmentModeService, DevelopmentModeService>();

// Register authentication services
builder.Services.AddSingleton<IUserSessionService, UserSessionService>();

// Check if we should use dummy client (when no P4Plan server is configured)
var serverUrl = builder.Configuration["P4PLAN_SERVER"] ?? string.Empty;
var projectWhitelist = builder.Configuration["P4PLAN_PROJECT_WHITELIST"] ?? string.Empty;
var useDummyClient = string.IsNullOrEmpty(serverUrl) || string.IsNullOrEmpty(projectWhitelist);

if (useDummyClient)
{
    logger.LogInformation("No P4Plan server configured - using Dummy Client for development");
    builder.Services.AddSingleton<IAppAuthenticationService, DevelopmentAuthenticationService>();
}
else
{
    logger.LogInformation("P4Plan server configured - using Production Authentication Service");
    builder.Services.AddSingleton<IAppAuthenticationService, AuthenticationService>();
}

// Configure authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token";
        options.LoginPath = useDummyClient ? "/" : "/login";
        options.Cookie.MaxAge = TimeSpan.FromDays(31);
        options.AccessDeniedPath = "/access-denied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(31);
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Register P4Plan client provider
if (useDummyClient)
{
    logger.LogInformation("Using Dummy Client for development mode");
    builder.Services.AddSingleton<IP4PlanClientProvider, P4PlanDummyClientProvider>();
}
else
{
    logger.LogInformation("Using real P4Plan Client with server: {ServerUrl}", serverUrl);
    builder.Services.AddSingleton<IP4PlanClientProvider>(serviceProvider => 
        new P4PlanClientProvider(serverUrl, projectWhitelist));
}

// Register project details service
builder.Services.AddSingleton<IProjectDetailsService>(serviceProvider =>
{
    var projectDetailsService = new ProjectDetailsService();
    var nextMilestone = builder.Configuration["P4PLAN_NEXT_MILESTONE"];
    if (!string.IsNullOrEmpty(nextMilestone))
    {
        projectDetailsService.NextMilestoneName = nextMilestone;
    }
    return projectDetailsService;
});

// Register logger
builder.Services.AddSingleton(x => logger);

var app = builder.Build();

// Configure the HTTP request pipeline
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