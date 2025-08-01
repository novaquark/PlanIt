using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;
using P4PlanLib;
using PlanIt.Components;
using PlanIt.Services;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load("DEV.env");

string? p4PlanServerUrl = Environment.GetEnvironmentVariable("P4PLAN_SERVER") ?? string.Empty;
string? p4PlanProjectWhitelist = Environment.GetEnvironmentVariable("P4PLAN_PROJECT_WHITELIST") ?? string.Empty;

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
