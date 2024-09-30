using E_Retalling_Portal.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Register RolePermissionService
builder.Services.AddSingleton<RolePermissionService>();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
});

// Set up Google authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
   options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie() // 
.AddGoogle(googleOptions =>
{
    var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
    googleOptions.ClientId = "354187502456-5c57ueucqrim692amg4emevq6ntpc99f.apps.googleusercontent.com";
    googleOptions.ClientSecret = "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";
    googleOptions.CallbackPath = new PathString("/signin-from-google");
	googleOptions.ClaimActions.MapJsonKey("externalId", "sub");
	googleOptions.ClaimActions.MapJsonKey("provider", "google");
})
.AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = "1568270060736221";
	facebookOptions.AppSecret = "7b662f21147322f4a933c0f1faf98308";
	facebookOptions.CallbackPath = new PathString("/signin-from-facebook");
	facebookOptions.Scope.Add("email"); 
	facebookOptions.SaveTokens = true;
	facebookOptions.ClaimActions.MapJsonKey("externalId", "id");
	facebookOptions.ClaimActions.MapJsonKey("provider", "facebook");
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
