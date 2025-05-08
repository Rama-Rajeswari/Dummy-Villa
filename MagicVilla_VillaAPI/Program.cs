using System.Text;
using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Controller;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Middlewares;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddIdentity<ApplicationUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();


    builder.Services.AddScoped<IVillaRepository,VillaRepository>();
    builder.Services.AddScoped<IVillaNumberRepository,VillaNumberRepository>();
    builder.Services.AddScoped<IUserRepository,UserRepository>();
    builder.Services.AddScoped<IDestinationRepository, DestinationRepository>();
    builder.Services.AddScoped<IBookingRepository, BookingRepository>();
    builder.Services.AddScoped<IRoomRepository, RoomRepository>();
    builder.Services.AddScoped<IFacilityRepository,FacilityRepository>();
    builder.Services.AddScoped<IGuestTypeRepository,GuestTypeRepository>();
    builder.Services.AddScoped<IVillaFacilityRepository,VillaFacilityRepository>();
    builder.Services.AddScoped<IRoomGuestTypeRepository,RoomGuestTypeRepository>();
    builder.Services.AddScoped<IRoomPricingRepository, RoomPricingRepository>();


 builder.Services.AddAutoMapper(typeof(MappingConfig)); 


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});




 builder.Services.AddApiVersioning(options=>
 {
    options.AssumeDefaultVersionWhenUnspecified=true;
    options.DefaultApiVersion=new ApiVersion(1,0);
    options.ReportApiVersions=true;
 });

 builder.Services.AddVersionedApiExplorer(options=>{
    options.GroupNameFormat="'v'VVV";
    options.SubstituteApiVersionInUrl=true;
 });

 var key=builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(x=>
{
    x.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x=>{
           x.RequireHttpsMetadata=false;
           x.SaveToken=true;
           x.TokenValidationParameters=new TokenValidationParameters
           {
                 ValidateIssuerSigningKey=true,
                 IssuerSigningKey=new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                 ValidateIssuer=false,
                 ValidateAudience=false
            };
});;

 builder.Services.AddScoped<APIResponse>(); 

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>(); 
    options.Filters.Add<CustomStatusCodeFilter>();
    
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});





builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options=>{
    options.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme  
{
    Description=
    "JWT Authorization header using the Bearer scheme.\r\n\r\n"+
    "Enter 'Bearer'[space] and then your token in the text input below.\r\n\r\n"+
    "Example:\"Bearer 12345abcdef\"",
    Name="Authorization",
    In=ParameterLocation.Header,
    Scheme="Bearer"
});
options.AddSecurityRequirement(new OpenApiSecurityRequirement()
{
    {
        new OpenApiSecurityScheme
        {
            Reference=new OpenApiReference
            {
                Type=ReferenceType.SecurityScheme,
                Id="Bearer"
            },
            Scheme="oauth2",
            Name="Bearer",
            In=ParameterLocation.Header

        },
        new List<string>()

    }
});
options.SwaggerDoc("v1",new OpenApiInfo
{
    Version="v1.0",
    Title="Magic Villa V1",
    Description="API to manage Villa",
    TermsOfService=new Uri("https://example.com/terms"),
    Contact=new OpenApiContact
    {
        Name="Dotnetmastery",
        Url=new Uri("https://dotnetmastery.com")
    },
    License=new OpenApiLicense
    {
        Name="Example License",
        Url=new Uri("https://example.com/license")
    }
});
options.SwaggerDoc("v2",new OpenApiInfo
{
    Version="v2.0",
    Title="Magic Villa V2",
    Description="API to manage Villa",
    TermsOfService=new Uri("https://example.com/terms"),
    Contact=new OpenApiContact
    {
        Name="Dotnetmastery",
        Url=new Uri("https://dotnetmastery.com")
    },
    License=new OpenApiLicense
    {
        Name="Example License",
        Url=new Uri("https://example.com/license")
    }
});
});

var app = builder.Build();


app.UseCors("AllowAll");

app.MapControllers(); 




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()|| app.Environment.IsStaging() || app.Environment.IsProduction())
{
    
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FirstAPI V1");
         options.SwaggerEndpoint("/swagger/v2/swagger.json", "FirstAPI V2");
        options.RoutePrefix = string.Empty;  
    });
}

app.UseMiddleware<CustomMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();


app.UseHttpsRedirection();



app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");



app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
