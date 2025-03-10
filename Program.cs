using BookManagementSystem.Models;
using BookManagementSystem.Data;
using BookManagementSystem.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using BookManagementSystem.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Text;
using BookManagementSystem.Services;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.HttpOverrides;
using BookManagementSystem.Services.Interfaces;
using BookManagementSystem.Repositories.Interfaces;
using BookManagementSystem.Repositories;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Read Environment Variables
var jwtSecret = builder.Configuration["JwtSettings:SecretKey"] ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
var connectionString = builder.Configuration.GetConnectionString("BookManagementContext") ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

// Ensure Environment Variables Are Set
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("ERROR: Missing DATABASE_CONNECTION_STRING.");
    throw new Exception("Missing DATABASE_CONNECTION_STRING. Ensure it's set.");
}

if (string.IsNullOrEmpty(jwtSecret))
{
    Console.WriteLine("ERROR: Missing JWT_SECRET_KEY.");
    throw new Exception("Missing JWT_SECRET_KEY. Ensure it's set.");
}

// Allow localhost while avoiding port conflicts
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5055); // Ensure localhost:5055 is explicitly allowed
    options.ListenLocalhost(5056, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS support
    });
    options.ListenAnyIP(0); // Allow dynamic port binding if needed
});

// Register Controllers
builder.Services.AddControllers();

// Register Entity Framework Core with SQLite Database
builder.Services.AddDbContext<BookManagementContext>(options =>
    options.UseSqlite(connectionString));

// Configure Identity for User Authentication
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<BookManagementContext>()
    .AddDefaultTokenProviders();

// Enforce Strong Password Policies
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
});

// Register Swagger for API Documentation
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Book Management API",
        Version = "v1",
        Description = "REST API for managing books, authors, publishers, categories, and reviews",
        Contact = new OpenApiContact
        {
            Name = "Salma",
            Email = "salmadardour@icloud.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
    
    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    
    // Add JWT authentication support in Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
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
                }
            },
            Array.Empty<string>()
        }
    });
});

// Use Environment Variables for JWT Secret
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

// Register TokenService for JWT Handling
builder.Services.AddScoped<ITokenService, TokenService>();

// Add RoleManager & UserManager
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();

// Register Repositories
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

// Register Services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// Register health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<BookManagementContext>("database")
    .AddCheck("self", () => HealthCheckResult.Healthy());

// Add Logging (Useful for debugging and error tracking)
builder.Services.AddLogging();

// Configure CORS (Only Allow Trusted Origins)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://yourfrontend.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add Rate Limiting to Prevent Abuse
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Limit = 100,
            Period = "1m"
        }
    };
});
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

var app = builder.Build();

// Seed Roles & Admin User
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // Run Role & Admin User Seeding Asynchronously
    Task.Run(async () =>
    {
        try
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FullName = "System Administrator"
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                    Console.WriteLine("Admin User Created Successfully.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Seeding Roles/Admin: {ex.Message}");
        }
    }).Wait();
}

// Use Global Exception Middleware for Unified Error Handling
app.UseMiddleware<GlobalErrorHandlingMiddleware>();

// Enable Rate Limiting Middleware
app.UseIpRateLimiting();

// Enable HTTPS Redirection
app.UseHttpsRedirection();

// Enable Swagger UI for API Testing
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Management API v1");
    options.RoutePrefix = string.Empty;
});

// Apply Authentication & Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// Enable CORS Policy
app.UseCors("AllowSpecificOrigin");

// Add Health Check Endpoint
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description
            }),
            totalDuration = report.TotalDuration
        };

        await JsonSerializer.SerializeAsync(context.Response.Body, response);
    }
});

// Map Controllers (API Endpoints)
app.MapControllers();

// Run the Application
app.Run();