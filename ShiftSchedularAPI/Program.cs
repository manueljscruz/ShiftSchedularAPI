using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShiftSchedularAPI.Configurations;
using ShiftSchedularDAL.Data;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Define a CORS policy name
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container

// 1. Enable CORS for specific origins (allow Angular app on localhost:4200)
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://192.168.0.9:4200", "https://81a1-188-81-53-74.ngrok-free.app/") // Allow specific origin
              .AllowAnyHeader()                    // Allow all headers
              .AllowAnyMethod();                   // Allow all HTTP methods
    });
});

// 2. Add controllers and configure JSON options
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Prevents infinite loops from object cycles in JSON serialization
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

    // Enforces camelCase naming for JSON properties
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;

    // Ignore null values in JSON output
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// 3. Enable Swagger/OpenAPI with XML comments for better documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Adds support for showing controller comments in Swagger UI
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ShiftSchedularAPI", Version = "v1" });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath); // Requires an XML comments file to be generated
});

// 4. Configure database context using SQL Server
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

builder.Services.AddDbContext<DataContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("ShiftSchedularDAL"));
});

// 5. Add custom services (assumed implemented elsewhere)
string logDirectory = builder.Configuration.GetValue<string>("LogDirectory");
string baseUrl = builder.Configuration.GetValue<string>("BaseUrl");
EmailSettings emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();
builder.Services.AddServicesInjections(logDirectory, emailSettings, baseUrl);

// 6. Configure JWT Authentication and Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

// Configure the middleware pipeline

// 1. Use CORS to allow cross-origin requests
app.UseCors(MyAllowSpecificOrigins);

// 2. Enable Swagger only in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 3. Enable HTTPS redirection
app.UseHttpsRedirection();

// 4. Enable Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// 5. Map controllers
app.MapControllers();

// Run the application
app.Run();
