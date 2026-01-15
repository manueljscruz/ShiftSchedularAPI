using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShiftSchedularAPI.Configurations;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.DbConstants;
using ShiftSchedularEntity.Converters;
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
    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

    // Log the allowed origins for debugging
    Console.WriteLine($"Configuring CORS with {allowedOrigins.Length} allowed origins:");
    foreach (var origin in allowedOrigins)
    {
        Console.WriteLine($"  - {origin}");
    }

    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins) // Allow specific origin
                  .AllowAnyHeader()                    // Allow all headers
                  .AllowCredentials()                   // Required for cookies only token
                  .AllowAnyMethod()                    // Allow all HTTP methods
                  .SetIsOriginAllowedToAllowWildcardSubdomains(); // Allow wildcard subdomains if needed
        }
        else
        {
            Console.WriteLine("WARNING: No allowed origins configured! CORS will block all requests.");
        }
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

    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
    options.JsonSerializerOptions.Converters.Add(new GuidConverter());
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

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Shift Scheduler - JWT Authorization",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
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

    // ADD THIS TO SEE WHAT IT'S READING
    // Console.WriteLine($"Connection String: {connectionString}");

    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("ShiftSchedularDAL"));
}, ServiceLifetime.Scoped);

// 5. Add custom services (assumed implemented elsewhere)
string logDirectory = builder.Configuration.GetValue<string>("LogDirectory");
string baseUrl = builder.Configuration.GetValue<string>("BaseUrl");
EmailSettings emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();
builder.Services.AddServicesInjections(logDirectory, emailSettings, baseUrl);
builder.Services.AddCustomRateLimiting(builder.Configuration);

// 6. Configure JWT Authentication and Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["access_token"];
                return Task.CompletedTask;
            }
        };

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

// 1. Enable Swagger only in development environment
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. Use CORS to allow cross-origin requests (must come before auth/authorization)
app.UseCors(MyAllowSpecificOrigins);

// 3. Enable HTTPS redirection
app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    await SeedRolesAndAdmin(scope.ServiceProvider);
}

// 4. Enable Authentication and Authorization (must come before rate limiter)
app.UseAuthentication();
app.UseAuthorization();

// 5. Enable Rate Limiter
app.UseRateLimiter();

// 5. Map controllers
app.MapControllers();

// Simple health check endpoint
app.MapGet("/", () => "API is running!");
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

// Run the application
app.Run();



// Seed method
static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Create roles
    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Create admin user
    var adminEmail = "admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            DisplayName = "System Administrator",
            GenderId = 3
        };

        await userManager.CreateAsync(adminUser, "Admin@123");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}