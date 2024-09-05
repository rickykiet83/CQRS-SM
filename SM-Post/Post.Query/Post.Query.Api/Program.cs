using Microsoft.EntityFrameworkCore;
using Post.Query.Infrastructure.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Action<DbContextOptionsBuilder> configureDbContext = options =>
{
    options
        .UseLazyLoadingProxies()
        .UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
};
builder.Services.AddDbContext<DatabaseContext>(configureDbContext);
builder.Services.AddSingleton(new DatabaseContextFactory(configureDbContext));

// Create database amd tables from code
var dataContext = builder.Services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
dataContext.Database.EnsureCreated();
dataContext.Database.Migrate();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
