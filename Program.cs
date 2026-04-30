using FirstApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ CORS (STRICT FIX)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// ⚠️ ORDER IS CRITICAL

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// ✅ MUST BE BEFORE AUTH + CONTROLLERS
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();