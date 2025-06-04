using System.Reflection;
using System.Text;
using BookAPI.Identity.Configurations;
using BookAPI.Identity.Models;
using BookAPI.Identity.Repositories;
using BookAPI.Identity.Services;
using BookAPI.Middleware;
using BookAPI.Services.Mapster;
using BookAPI.Services.Validators.BookValidators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// JWT Config
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
var jwtOpts = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

// EF Core
builder.Services.AddDbContext<BookDbContext>(options =>
    options.UseSqlite(builder.Configuration
        .GetConnectionString("BooksContext")));

// UnitOfWork + Repos + Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IAuthUserRepository, AuthUserRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Password hasher + JWT generator + clock
builder.Services.AddScoped<IPasswordHasher<AuthUser>, PasswordHasher<AuthUser>>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

// Fluent Validation
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();

// Exception handling
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

// Controllers + API behavior
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(opts =>
        opts.SuppressModelStateInvalidFilter = true);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new OpenApiInfo { Title = "Book API", Version = "v1",
        Description = "A clean and well-structured API for CRUD operations and reviewing books" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
    
    opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer eyJhbGci...')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    opts.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// JWT Bearer
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtOpts!.Issuer,
            ValidAudience            = jwtOpts.Audience,
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOpts.Secret))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Mapster
MapsterConfig.RegisterMappings();

// Global error handler (picks up CustomExceptionHandler)
app.UseExceptionHandler(options => { });

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Request logging
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    // run migrations + seed
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<BookDbContext>();
    db.Database.Migrate();
    await SeedData.InitializeAsync(db);

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

public partial class Program { }