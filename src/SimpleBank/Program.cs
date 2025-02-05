using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SimpleBank.Services;
using SimpleBank.DataAccess.DataContext;
using SimpleBank.DataAccess.Models;
using SimpleBank.BusinessLogic.Abstraction;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// Add services to the container.
builder.Services.AddControllersWithViews();

// Add ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add the background service
builder.Services.AddHostedService<ScheduledTransactionService>();
// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = false; 
    options.Password.RequiredLength = 4;   
    options.Password.RequireNonAlphanumeric = false; 
    options.Password.RequireLowercase = false; 
    options.Password.RequiredUniqueChars = 0;
    // Update to match new controller
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Access/AccessDenied";
    options.LoginPath = "/Access/Login";
    options.LogoutPath = "/Access/Register";

});

builder.Services.AddScoped<IAccountService, AccountService>();


//builder.Services.AddDefaultIdentity<ApplicationUser>()
//    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Access}/{action=Login}/{id?}");

// Catch-all route for 404 Not Found
app.MapControllerRoute(
    name: "notfound",
    pattern: "{*url}",
    defaults: new { controller = "Home", action = "NotFound" });
app.Run();
