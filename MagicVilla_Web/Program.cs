using MagicVilla_Web;
using MagicVilla_Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddHttpClient<IVillaService,VillaService>();
builder.Services.AddScoped<IVillaService,VillaService>();

builder.Services.AddHttpClient<IVillaNumberService,VillaNumberService>();
builder.Services.AddScoped<IVillaNumberService,VillaNumberService>();

builder.Services.AddHttpClient<IAuthService,AuthService>();
builder.Services.AddScoped<IAuthService,AuthService>();


builder.Services.AddHttpClient<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddHttpClient<IDestinationService,DestinationService>();
builder.Services.AddScoped<IDestinationService,DestinationService>();

builder.Services.AddHttpClient<IGuestTypeService,GuestTypeService>();
builder.Services.AddScoped<IGuestTypeService,GuestTypeService>();

builder.Services.AddHttpClient<IRoomService,RoomService>();
builder.Services.AddScoped<IRoomService,RoomService>();

builder.Services.AddHttpClient<IFacilityService,FacilityService>();
builder.Services.AddScoped<IFacilityService,FacilityService>();

builder.Services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options=>
{
    options.Cookie.HttpOnly=true;
    options.ExpireTimeSpan=TimeSpan.FromMinutes(30);
    options.LoginPath="/Auth/Login";
    options.AccessDeniedPath="/Auth/AccessDenied";
    options.SlidingExpiration=true;
});

builder.Services.AddSession(options=>
{
    options.IdleTimeout=TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly=true;
    options.Cookie.IsEssential=true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.MapStaticAssets();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
