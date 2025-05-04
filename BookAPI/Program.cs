using System.Reflection;
using BookAPI.Middleware;
using BookAPI.Services.Validators.BookValidators;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Book API",
        Description = "A clean and well-structured API for CRUD operations and reviewing books",
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
    

builder.Services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

if (builder.Environment.IsEnvironment("IntegrationTests"))
{
    builder.Services.AddDbContext<BookDbContext>(options => 
        options.UseInMemoryDatabase("TestDb"));
}
else
{
    builder.Services.AddDbContext<BookDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("BooksContext"));
    });
}


builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

app.UseExceptionHandler(options => { }); 

app.UseMiddleware<RequestLoggingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var serviceScope = app.Services.CreateScope())
    {
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<BookDbContext>();
        dbContext.Database.Migrate();
        await SeedData.InitializeAsync(dbContext);
    }
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }