using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using DataAccess;
using DataAccess.Repositories;
using DataAccess.Validators;
using FluentValidation;
using Application.Services;
using System.Text;
using Application.Abstractions;
using DataAccess.Models;
using Domain.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using WebApi;
using WebApi.MappingProfiles; 

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                       Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection("AdminSettings"));
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<BookMappingProfile>();
    cfg.AddProfile<AuthorMappingProfile>();
    cfg.AddProfile<UserMappingProfile>();
});

builder.Services.AddScoped<IValidator<UserEntity>, UserValidator>();
builder.Services.AddScoped<IValidator<BookEntity>, BookValidator>();
builder.Services.AddScoped<IValidator<AuthorEntity>, AuthorValidator>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ReadPolicy", policy =>
        policy.RequireRole("user", "admin"));
    options.AddPolicy("WritePolicy", policy =>
        policy.RequireRole("admin"));
    options.AddPolicy("UpdatePolicy", policy =>
        policy.RequireRole("admin"));
    options.AddPolicy("DeletePolicy", policy =>
        policy.RequireRole("admin"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; 
});

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication(); 
app.UseMiddleware<JwtValidationMiddleware>();
app.UseAuthorization();  
app.UseStaticFiles();

app.UseCors("AllowSpecificOrigin");

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationContext>();
    var adminSettings = services.GetRequiredService<IOptions<AdminSettings>>().Value;

    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }

    if (!context.Users.Any(u => u.Username == adminSettings.Username))
    {
        var admin = new UserEntity()
        {
            Username = adminSettings.Username,
            PasswordHash = adminSettings.PasswordHash, 
            Email = adminSettings.Email,
            Role = adminSettings.Role
        };

        context.Users.Add(admin);
        context.SaveChanges();

        Console.WriteLine("Аккаунт администратора создан.");
    }
    else
    {
        Console.WriteLine("Аккаунт администратора уже существует.");
    }
}

var retryPolicy = Policy.Handle<Exception>()
    .WaitAndRetry(
        retryCount: 5,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(10),
        onRetry: (exception, timeSpan, context) =>
        {
            Console.WriteLine($"Не удалось подключиться к базе данных: {exception.Message}. Повторная попытка через {timeSpan.TotalSeconds} секунд.");
        });

retryPolicy.Execute(() =>
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationContext>();

        context.Database.Migrate();
        Console.WriteLine("Миграции успешно применены.");
    }
});

app.Run();
    